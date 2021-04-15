using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GithubActions.AzureFunction.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace GithubActions.AzureFunction
{
    public class HelloFunction
    {
        private readonly ILogger<HelloFunction> _logger;
        private readonly DomainConfig _config;

        public HelloFunction(ILogger<HelloFunction> logger, DomainConfig config)
        {
            _logger = logger;
            _config = config;
        }

        [FunctionName(nameof(HelloFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var name = req.Query["name"].FirstOrDefault();

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return !string.IsNullOrWhiteSpace(name)
                ? new OkObjectResult($"{_config.Greeting}, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}