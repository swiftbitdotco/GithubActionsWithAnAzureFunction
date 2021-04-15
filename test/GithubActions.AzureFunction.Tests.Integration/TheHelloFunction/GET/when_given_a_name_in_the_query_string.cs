using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction.GET
{
    public class when_given_a_name_in_the_query_string : BaseTestClass
    {
        public when_given_a_name_in_the_query_string(FunctionTestFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task should_return_http_200_ok()
        {
            // arrange
            var name = "John";

            // act
            var httpResponse = await Client.GetAsync($"HelloFunction?name={name}");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}