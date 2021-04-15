using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction.GET
{
    public class when_given_a_null_or_whitespace_name_in_the_query_string : BaseTestClass
    {
        public when_given_a_null_or_whitespace_name_in_the_query_string(FunctionTestFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Theory]
        [InlineData("John")]
        [InlineData("Paul")]
        [InlineData("George")]
        [InlineData("Ringo")]
        public async Task should_return_http_200_ok(string name)
        {
            // act
            var httpResponse = await Client.GetAsync($"HelloFunction?name={name}");

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}