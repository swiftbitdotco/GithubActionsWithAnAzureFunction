using GithubActions.AzureFunction.Tests.Unit.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Unit.TheHelloFunction
{
    public abstract class BaseTestClass : IClassFixture<TestFixture>
    {
        protected readonly HelloFunction Sut;

        protected BaseTestClass(TestFixture fixture)
        {
            fixture.Initialize();
            Sut = fixture.CreateHelloFunction();
        }
    }
}