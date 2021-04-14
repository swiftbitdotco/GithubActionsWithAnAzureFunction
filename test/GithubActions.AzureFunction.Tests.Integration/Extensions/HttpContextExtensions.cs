using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubActions.AzureFunction.Tests.Integration.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<string> TryGetPrettyPrintJson(this HttpContent httpContent)
        {
            var rawJson = await httpContent.ReadAsStringAsync();

            return rawJson.TryGetPrettyPrintJson();
        }

        public static string TryGetPrettyPrintJson(this string rawJson)
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