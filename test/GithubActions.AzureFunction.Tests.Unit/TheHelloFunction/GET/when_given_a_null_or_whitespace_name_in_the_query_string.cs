using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GithubActions.AzureFunction.Tests.Unit.TestInfrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Xunit;
using Xunit.Abstractions;

namespace GithubActions.AzureFunction.Tests.Unit.TheHelloFunction.GET
{
    public class when_given_a_null_or_whitespace_name_in_the_query_string : BaseTestClass
    {
        public when_given_a_null_or_whitespace_name_in_the_query_string(TestFixture fixture) : base(fixture)
        {
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task should_return_http_400_bad_request(string name)
        {
            // arrange
            var req = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Method = "post",
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    {"name", name}
                })
            };

            // act
            var httpResponse = await Sut.RunAsync(req);

            // assert
            httpResponse.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}