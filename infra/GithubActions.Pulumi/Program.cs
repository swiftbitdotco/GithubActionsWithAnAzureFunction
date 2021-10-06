using System.Threading.Tasks;
using Pulumi;

namespace GithubActions.Pulumi
{
    internal class Program
    {
        private static Task<int> Main() => Deployment.RunAsync<GithubActionsWithAnAzureFunction>();
    }
}