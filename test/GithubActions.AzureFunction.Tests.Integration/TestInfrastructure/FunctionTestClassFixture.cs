using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssemblyFixture;

[assembly: TestFramework(AssemblyFixtureFramework.TypeName, AssemblyFixtureFramework.AssemblyName)]

namespace GithubActions.AzureFunction.Tests.Integration.TestInfrastructure
{
    public sealed class FunctionTestFixture : IDisposable
    {
        public ITestOutputHelper Output { get; private set; }
        private readonly IFunctionFactory _server;

        public FunctionTestFixture()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configRoot = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddUserSecrets<FunctionTestFixture>()
                .AddEnvironmentVariables()
                .Build();

            // load test settings
            TestSettings = LoadSection<TestSettings>(configRoot);

            // TODO: load & print app-specific configs to the console in order to see their values in the build server output
            // e.g. LoadSection<MyServiceSettings>(configRoot);

            if (TestSettings.IsRunningOnLocalHost)
            {
                _server = new FunctionFactory();
                _server.StartHostForLocalDevelopment();
                //_server = FunctionFactorySingleton.Instance;
            }
        }

        /// <summary>
        /// ITestOutputHelper is set in the constructor of the Test class
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public FunctionTestFixture SetOutput(ITestOutputHelper output)
        {
            Output = output;
            return this;
        }

        private static T LoadSection<T>(IConfiguration configuration) where T : class, new()
        {
            var concreteSection = new T();
            var name = concreteSection.GetType().Name;

            configuration.GetSection(name).Bind(concreteSection);

            return concreteSection;
        }

        private TestSettings TestSettings { get; }

        public ICustomHttpClient CreateClient()
        {
            TestSettings.BaseUrl.Should().NotBeNullOrWhiteSpace($"{nameof(TestSettings.BaseUrl)} should not be null or whitespace");
            //TestSettings.ApiKeyHeader.Should().NotBeNullOrWhiteSpace($"{nameof(TestSettings.ApiKeyHeader)} should not be null or whitespace");
            //TestSettings.ApiKeyValue.Should().NotBeNullOrWhiteSpace($"{nameof(TestSettings.ApiKeyValue)} should not be null or whitespace");

            Output.Should().NotBeNull($"The '{nameof(FunctionTestFixture)}.{nameof(Output)}' property was not set in the test constructor");

            var client = new HttpClient
            {
                BaseAddress = new Uri(TestSettings.BaseUrl),
            };
            //client.DefaultRequestHeaders.Add(TestSettings.ApiKeyHeader, TestSettings.ApiKeyValue);

            CheckUri(client.BaseAddress.AbsoluteUri);
            return new CustomHttpClient(client, Output);
        }

        private void CheckUri(string absoluteUri)
        {
            absoluteUri.Should().EndWith("/", "without a trailing slash, the last part of the url path is discarded when using HttpClient.");

            var test = absoluteUri
                .Replace("http://", null)
                .Replace("https://", null);
            test.Should().NotContain("//", "No double-slashes allowed in the url.");
        }

        public void Dispose()
        {
            _server?.Dispose();
            Output = null;
        }
    }
}