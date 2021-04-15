using GithubActions.AzureFunction.Domain;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GithubActions.AzureFunction.Middleware
{
    public static class ConfigureSettingsExtensions
    {
        public static IFunctionsHostBuilder ConfigureSettings(this IFunctionsHostBuilder builder)
        {
            builder.AddConfigFromValuesSection<DomainConfig>();

            return builder;
        }

        private static void AddConfigFromValuesSection<T>(this IFunctionsHostBuilder builder) where T : class, new()
        {
            // https://medium.com/@aranmulholland/injecting-application-settings-into-azure-functions-using-ioptions-f9ca07ac80ce
            // https://docs.microsoft.com/bs-cyrl-ba/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings
            // this will bind to the "Values" section of the configuration

            builder.Services.AddOptions<T>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind(settings);
            });
            builder.Services.AddSingleton(ctx => ctx.GetService<IOptions<T>>().Value);
        }
    }
}