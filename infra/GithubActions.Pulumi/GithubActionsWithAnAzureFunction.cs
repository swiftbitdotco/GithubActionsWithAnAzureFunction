using System.IO;
using System.Threading.Tasks;
using GithubActions.Pulumi.Factories;
using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi.AzureNative.Resources;
using Deployment = Pulumi.Deployment;

namespace GithubActions.Pulumi
{
    internal class GithubActionsWithAnAzureFunction : Stack
    {
        private string GetName(string resourceType, string application, string azureRegion)
        {
            return $"{resourceType}-{application}-{Deployment.Instance.StackName}-{azureRegion}-001";
        }

        public GithubActionsWithAnAzureFunction()
        {
            var config = new Config();
            var azureRegion = config.Require("azureRegion");

            var workloadResources = "ghares";
            var workloadFunction = "ghafunc";

            var resourceGroupForResources = ResourceGroupFactory.Create(GetName("rg", workloadResources, azureRegion));
            var resourceGroupForFunction = ResourceGroupFactory.Create(GetName("rg", workloadFunction, azureRegion));

            // Storage account is required by Function App.
            // Also, we will upload the function code to the same storage account.
            var storageAccount = StorageAccountFactory.Create(resourceGroupForResources, $"st{workloadResources}{Deployment.Instance.StackName}{azureRegion}001");

            // Export the primary key of the Storage Account
            this.PrimaryStorageKey = Output.Tuple(resourceGroupForResources.Name, storageAccount.Name).Apply(names =>
                Output.CreateSecret(GetStorageAccountPrimaryKey(names.Item1, names.Item2)));

            var functionSku = config.Require("functionSku").Split('/');

            // Define a Consumption Plan for the Function App.
            // You can change the SKU to Premium or App Service Plan if needed.
            var appServicePlanName = GetName("plan", workloadFunction, azureRegion);
            var appServicePlan = new AppServicePlan(appServicePlanName, new AppServicePlanArgs
            {
                Name = appServicePlanName,

                ResourceGroupName = resourceGroupForResources.Name,

                // Run on Linux
                Kind = "Linux",

                // Consumption plan SKU
                Sku = new SkuDescriptionArgs
                {
                    Tier = functionSku[0],
                    Name = functionSku[1]
                },

                // For Linux, you need to change the plan to have Reserved = true property.
                Reserved = true
            }, new CustomResourceOptions { DeleteBeforeReplace = true });

            var container = new BlobContainer("zips-container", new BlobContainerArgs
            {
                AccountName = storageAccount.Name,
                PublicAccess = PublicAccess.None,
                ResourceGroupName = resourceGroupForResources.Name,
            }, new CustomResourceOptions { DeleteBeforeReplace = true });

            var functionAppPublishFolder = Path.Combine("C:\\dev\\swiftbit\\GithubActionsWithAzureFunction\\src", "GithubActions.AzureFunction", "bin", "Release", "netcoreapp3.1", "publish");
            var blob = new Blob("zip", new BlobArgs
            {
                AccountName = storageAccount.Name,
                ContainerName = container.Name,
                ResourceGroupName = resourceGroupForResources.Name,
                Type = BlobType.Block,
                Source = new FileArchive(functionAppPublishFolder)
            }, new CustomResourceOptions { DeleteBeforeReplace = true });

            var codeBlobUrl = SignedBlobReadUrl(blob, container, storageAccount, resourceGroupForResources);

            // Application insights
            var appInsightsName = GetName("appi", workloadFunction, azureRegion);
            var appInsights = new Component(appInsightsName, new ComponentArgs
            {
                ResourceName = appInsightsName,

                ApplicationType = ApplicationType.Web,
                Kind = "web",
                ResourceGroupName = resourceGroupForResources.Name,
            }, new CustomResourceOptions { DeleteBeforeReplace = true });

            var funcName = GetName("func", workloadFunction, azureRegion);
            var app = new WebApp(funcName, new WebAppArgs
            {
                Name = funcName,

                Kind = "FunctionApp",
                ResourceGroupName = resourceGroupForFunction.Name,
                ServerFarmId = appServicePlan.Id,
                SiteConfig = new SiteConfigArgs
                {
                    AppSettings = new[]
                    {
                    new NameValuePairArgs{
                        Name = "AzureWebJobsStorage",
                        Value = GetConnectionString(resourceGroupForResources.Name, storageAccount.Name),
                    },
                    new NameValuePairArgs{
                        Name = "runtime",
                        Value = "dotnet",
                    },
                    new NameValuePairArgs{
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet",
                    },
                    new NameValuePairArgs{
                        Name = "WEBSITE_RUN_FROM_PACKAGE",
                        Value = codeBlobUrl,
                    },
                    new NameValuePairArgs{
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
                    },
                    new NameValuePairArgs{
                        Name = "Greeting",
                        Value = "Hi",
                    },
                },
                },
            });

            this.Endpoint = Output.Format($"https://{app.DefaultHostName}/api/HelloFunction?name=Pulumi");
        }

        [Output]
        public Output<string> PrimaryStorageKey { get; set; }

        [Output]
        public Output<string> Endpoint { get; set; }

        private static async Task<string> GetStorageAccountPrimaryKey(string resourceGroupName, string accountName)
        {
            var accountKeys = await ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountName = accountName
            });
            return accountKeys.Keys[0].Value;
        }

        private static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account, ResourceGroup resourceGroup)
        {
            return Output.Tuple<string, string, string, string>(
                blob.Name, container.Name, account.Name, resourceGroup.Name).Apply(t =>
                {
                    (string blobName, string containerName, string accountName, string resourceGroupName) = t;

                    var blobSAS = ListStorageAccountServiceSAS.InvokeAsync(new ListStorageAccountServiceSASArgs
                    {
                        AccountName = accountName,
                        Protocols = HttpProtocol.Https,
                        SharedAccessStartTime = "2021-01-01",
                        SharedAccessExpiryTime = "2030-01-01",
                        Resource = SignedResource.C,
                        ResourceGroupName = resourceGroupName,
                        Permissions = Permissions.R,
                        CanonicalizedResource = "/blob/" + accountName + "/" + containerName,
                        ContentType = "application/json",
                        CacheControl = "max-age=5",
                        ContentDisposition = "inline",
                        ContentEncoding = "deflate",
                    });
                    return Output.Format($"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}?{blobSAS.Result.ServiceSasToken}");
                });
        }

        private static Output<string> GetConnectionString(Input<string> resourceGroupName, Input<string> accountName)
        {
            // Retrieve the primary storage account key.
            var storageAccountKeys = Output.All<string>(resourceGroupName, accountName).Apply(t =>
            {
                var resourceGroupName = t[0];
                var accountName = t[1];
                return ListStorageAccountKeys.InvokeAsync(
                    new ListStorageAccountKeysArgs
                    {
                        ResourceGroupName = resourceGroupName,
                        AccountName = accountName
                    });
            });
            return storageAccountKeys.Apply(keys =>
            {
                var primaryStorageKey = keys.Keys[0].Value;

                // Build the connection string to the storage account.
                return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
            });
        }
    }
}