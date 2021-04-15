using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction
{
    [Collection(FunctionCollectionFixtureNames.All)]
    public abstract class BaseTestClass
    {
        protected readonly ICustomHttpClient Client;

        protected BaseTestClass(FunctionTestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Client = fixture.SetOutput(testOutputHelper).CreateClient();
        }
    }
}