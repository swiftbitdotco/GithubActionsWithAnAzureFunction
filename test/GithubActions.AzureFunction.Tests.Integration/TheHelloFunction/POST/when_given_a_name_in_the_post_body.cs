using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GithubActions.AzureFunction.Tests.Integration.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TheHelloFunction.POST
{
    public class when_given_a_name_in_the_post_body : BaseTestClass
    {
        public when_given_a_name_in_the_post_body(FunctionTestFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task should_return_http_200_ok()
        {
            // arrange
            var name = "Paul";

            // act
            var httpResponse = await Client.PostAsync("HelloFunction", new PostBody
            {
                Name = name
            });

            // assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}