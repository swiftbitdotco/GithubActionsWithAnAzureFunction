namespace GithubActions.AzureFunction.Tests.Integration.TestInfrastructure
{
    public class TestSettings
    {
        public string ApiKeyHeader { get; set; }
        public string ApiKeyValue { get; set; }

        /// <summary>
        /// Use either "https://localhost/api/" or your url in Azure...
        /// </summary>
        public string BaseUrl { get; set; }

        public bool IsRunningOnLocalHost => !string.IsNullOrWhiteSpace(BaseUrl) && BaseUrl.ToLower().Contains("localhost");
    }
}