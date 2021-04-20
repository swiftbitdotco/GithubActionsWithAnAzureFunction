using System.Collections.Generic;
using GithubActions.AzureFunction.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GithubActions.AzureFunction.Tests.Unit.TestInfrastructure
{
    public sealed class TestFixture
    {
        public TestHost TestHost;

        public void Initialize(IEnumerable<ServiceDescriptor> replacements = null)
        {
            if (replacements == null)
            {
                replacements = new List<ServiceDescriptor>();
            }

            TestHost = new TestHost(replacements);
        }

        public HelloFunction CreateHelloFunction()
        {
            var logger = TestHost.ServiceProvider.GetRequiredService<ILogger<HelloFunction>>();
            var domainConfig = TestHost.ServiceProvider.GetRequiredService<DomainConfig>();
            return new HelloFunction(logger, domainConfig);
        }
    }
}