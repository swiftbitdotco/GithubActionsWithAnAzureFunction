using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubActions.AzureFunction.Tests.Integration.TestInfrastructure
{
    public static class HttpContentExtensions
    {
        public static async Task<string> TryGetPrettyPrintContentAsync(this HttpContent httpContent, string contentType)
        {
            var content = await httpContent.ReadAsStringAsync();

            try
            {
                if (contentType.Contains("xml"))
                {
                    return TryGetPrettyPrintXmlAsync(content);
                }

                return TryGetPrettyPrintJsonAsync(content);
            }
            catch
            {
                return content;
            }
        }

        private static string TryGetPrettyPrintXmlAsync(string rawXml)
        {
            try
            {
                return XDocument.Parse(rawXml).ToString();
            }
            catch
            {
                return rawXml;
            }
        }

        private static string TryGetPrettyPrintJsonAsync(string rawJson)
        {
            try
            {
                return JToken.Parse(rawJson).ToString(Formatting.Indented);
            }
            catch
            {
                return rawJson;
            }
        }
    }
}