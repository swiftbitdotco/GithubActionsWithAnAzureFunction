using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction
{
    public class when_given_a_name_in_the_query_string
    {
        private readonly FunctionTestFixture _fixture;
        private readonly ICustomHttpClient _client;

        public when_given_a_name_in_the_query_string(ITestOutputHelper testOutputHelper)
        {
            _fixture = new FunctionTestFixture(testOutputHelper);
            _client = _fixture.CreateClient();
        }

        [Fact]
        public async Task should_return_http_200_ok()
        {
            // arrange
            var name = "Michael";

            // act
            var httpResponse = await _client.GetAsync($"HelloFunction?name={name}");

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}