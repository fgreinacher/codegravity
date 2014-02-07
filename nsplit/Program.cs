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
using AutoMapper;
using nsplit.Api;
using nsplit.CodeAnalyzis;
using nsplit.Helper;

#endregion

namespace nsplit
{
    internal class Program
    {
        public const string HttpLocalhost = "http://localhost:8080";

        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject.ToString());
                Environment.Exit(1);
            };

            if (args.Length > 0)
            {
                var path = args[0];
                string directoryName = Path.GetDirectoryName(path) ?? Path.GetTempPath();
                var storage = new Storage(Path.Combine(directoryName, "_DependencyAnalyzes"));

                string folderPath = Path.GetDirectoryName(path);
                ResolveEventHandler handler = (sender, resolveArgs) => AssemblyLoadHelper.Resolver(folderPath, resolveArgs);
                AppDomain.CurrentDomain.AssemblyResolve += handler;

                string message;
                Assembly assembly;
                bool isLoaded = AssemblyLoadHelper.TryLoad(path, out assembly, out message );
                if (!isLoaded)
                {
                    Console.WriteLine(message);
                    Environment.Exit(1);
                };
                var graph = Analyzer.Analyze(assembly).GetGraph();
                var dto = Mapper.DynamicMap<GraphDto>(graph);
                storage.Save(dto);
                return;
            }

            var storagePath = Path.Combine(GetExePath(), "data");
            AppState.Storage = new Storage(storagePath);
            AppState.Task = AnalyzerTask.Idle();
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
                    Process.Start("readme.html");
                    return;
                }
                Process.Start(HttpLocalhost + "/index.html");
                Console.ReadKey();
            }
        }


        private static string GetExePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}