using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit.Abstractions;
using Xunit.Extensions.AssemblyFixture;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction
{
    public abstract class BaseTestClass : IAssemblyFixture<FunctionTestFixture>
    {
        protected readonly ICustomHttpClient Client;

        protected BaseTestClass(FunctionTestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Client = fixture.CreateClient(testOutputHelper);
        }
    }
}