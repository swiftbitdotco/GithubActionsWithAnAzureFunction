using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace GithubActions.AzureFunction.Tests.Integration.TestInfrastructure
{
    public interface IFunctionFactory : IDisposable
    {
        void StartHostForLocalDevelopment();
    }

    public sealed class FunctionFactory : IFunctionFactory
    {
        private Process _host;

        public void StartHostForLocalDevelopment()
        {
            if (_host != null)
            {
                return;
            }

            const string filename = "func";

            const string args = "host start --verbose";

            var basePath = GetWorkingDirectory();

            var hostProcess = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args,
                WorkingDirectory = basePath,

                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            _host = Process.Start(hostProcess);

            Thread.Sleep(5000);
        }

        private static string GetWorkingDirectory()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(HelloFunction)).GetAssemblyLocation());

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new NullReferenceException("Assembly.GetAssembly(typeof(HelloFunction) is null");
            }

            basePath = basePath.Replace("test", "src");
            basePath = basePath.Replace(".Tests.Integration", string.Empty);
            return basePath;
        }

        public void Dispose()
        {
            _host.CloseMainWindow();
            _host.Dispose();
        }
    }
}