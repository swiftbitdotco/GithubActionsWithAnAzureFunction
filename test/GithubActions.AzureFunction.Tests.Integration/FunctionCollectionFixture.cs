using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;

namespace GithubActions.AzureFunction.Tests.Integration
{
    [CollectionDefinition(FunctionCollectionFixtureNames.All, DisableParallelization = false)]
    public class FunctionCollectionFixture : ICollectionFixture<FunctionTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}