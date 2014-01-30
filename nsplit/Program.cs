#region usings

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using nsplit.Helper;

#endregion

namespace nsplit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Assembly assembly = typeof (Program).Assembly;

            Registry.BuildUp(assembly);

            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            string webFolder = Path.Combine(GetExePath(), "html");

            Func<HttpRequestMessage, bool> matchesAllExceptApi =
                request => !request.RequestUri.AbsolutePath.StartsWith("/api/");

            var webServerOnFolder = new WebServerOnFolder(
                matchesAllExceptApi,
                webFolder,
                FileType.Html,
                FileType.Css,
                FileType.Javascript,
                FileType.Gif,
                FileType.Png);

            config.MessageHandlers.Add(webServerOnFolder);
            config.Routes.MapHttpRoute(
                "API",
                "api/{controller}/{action}",
                new {id = RouteParameter.Optional});

            using (var server = new HttpSelfHostServer(config))
            {
                try
                {
                    server.OpenAsync().Wait();
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine(ex.ToString());
                    Process.Start("readme.html");
                    return;
                }
                Process.Start("http://localhost:8080/index.html");
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }

        private static string GetExePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}