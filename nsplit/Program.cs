#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Xml;
using AutoMapper;
using nsplit.Helper;

#endregion

namespace nsplit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //RegisterFolderResolver(folderPath);
            //Assembly assembly = Assembly.LoadFile(Path.Combine(folderPath, assemblyToAnalyze + ".dll"));

            Assembly assembly = typeof(Program).Assembly;

            Registry.Build(assembly);

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
                FileType.Png,
                FileType.Json);

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
                Console.ReadKey();
            }
        }

        private static void RegisterFolderResolver(string folderPath)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                var name = resolveArgs.Name;
                var fileName = name.Substring(0, name.IndexOf(',')) + ".dll";

                string fullPath = Path.Combine(folderPath, fileName);
                if (!File.Exists(fullPath))
                {
                    return null;
                }
                return Assembly.LoadFile(fullPath);
            };
        }

        private static string GetExePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}