// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Gma.CodeVisuals.WebApi.DependencyForceGraph;
using Gma.CodeVisuals.WebApi.Server;

#endregion

namespace Gma.CodeVisuals.WebApi
{
    internal class Program
    {
        public const string HttpLocalhost = "http://localhost:8080";

        public static Storage Storage;

        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject.ToString());
                Environment.Exit(1);
            };

            var storagePath = args.Length > 0
                ? args[0]
                : Storage.GetDefaultPath();
            Storage = new Storage(storagePath);
            StartHttpServer();
        }

        private static void StartHttpServer()
        {
            var config = new HttpSelfHostConfiguration(HttpLocalhost)
            {
                HostNameComparisonMode = HostNameComparisonMode.Exact
            };

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
                FileType.Json,
                FileType.Ico);

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
                    Console.WriteLine("Server started on [{0}]", HttpLocalhost);
                }
                catch (AggregateException ex)
                {
                    Console.WriteLine("Failed to start server on [{0}]", HttpLocalhost);
                    Console.WriteLine(ex.ToString());
                    Process.Start("port8080problem.html");
                    return;
                }
                Process.Start(HttpLocalhost + "/index.html");
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