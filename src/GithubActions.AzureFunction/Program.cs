using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GithubActions.AzureFunction
{
    public class Program
    {
        private static Task Main(string[] args)
        {
            var host = new HostBuilder()
                           .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddCommandLine(args);
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                })
                .Build();

            return host.RunAsync();
        }
    }
}