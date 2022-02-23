using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace GithubActions.Pulumi.Factories
{
    public static class StorageAccountFactory
    {
        public static StorageAccount Create(ResourceGroup resourceGroup, string name)
        {
            return new StorageAccount(name, new StorageAccountArgs
            {
                AccountName = name,
                ResourceGroupName = resourceGroup.Name,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS
                },
                Kind = Kind.StorageV2
            }, new CustomResourceOptions { DeleteBeforeReplace = true });
        }
    }
}