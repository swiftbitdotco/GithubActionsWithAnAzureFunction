using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;

namespace GithubActions.Pulumi.Factories
{
    public static class StorageAccountStaticWebsiteFactory
    {
        public static StorageAccountStaticWebsite Create(ResourceGroup resourceGroup, StorageAccount storageAccount, string name)
        {
            return new StorageAccountStaticWebsite(name, new StorageAccountStaticWebsiteArgs
                {
                    AccountName = storageAccount.Name,
                    ResourceGroupName = resourceGroup.Name,
                    IndexDocument = "index.html",
                },
                new CustomResourceOptions { DeleteBeforeReplace = true });
        }
    }
}