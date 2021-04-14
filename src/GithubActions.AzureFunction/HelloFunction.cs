using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace GithubActions.AzureFunction
{
    public class HelloFunction
    {
        private readonly ILogger<HelloFunction> _logger;

        public HelloFunction(ILogger<HelloFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(HelloFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var query = req.Url.Query;
            var queryParts = HttpUtility.ParseQueryString(query);
            var name = queryParts.Get("name");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return !string.IsNullOrWhiteSpace(name)
                ? new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}