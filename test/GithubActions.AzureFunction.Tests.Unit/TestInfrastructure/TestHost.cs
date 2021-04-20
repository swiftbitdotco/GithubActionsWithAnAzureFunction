using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace GithubActions.AzureFunction.Tests.Unit.TestInfrastructure
{
    public class TestHost
    {
        public TestHost(IEnumerable<ServiceDescriptor> replacements)
        {
            var startup = new Startup();
            var host = new HostBuilder()

                .ConfigureWebJobs(startup.Configure)
                .ConfigureServices(services =>
                {
                    foreach (var replacement in replacements)
                    {
                        services.Replace(replacement);
                    }
                })
                .Build();

            ServiceProvider = host.Services;
        }

        public IServiceProvider ServiceProvider { get; }
    }
}