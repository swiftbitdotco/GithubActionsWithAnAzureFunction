using Pulumi;
using Pulumi.AzureNative.Resources;

namespace GithubActions.Pulumi.Factories
{
    public static class ResourceGroupFactory
    {
        public static ResourceGroup Create(string name)
        {
            var resourceGroup = new ResourceGroup(name, new ResourceGroupArgs
            {
                ResourceGroupName = name
            },
                new CustomResourceOptions { DeleteBeforeReplace = true }
            );
            return resourceGroup;
        }
    }
}