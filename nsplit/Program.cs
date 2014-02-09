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
        
        public static Storage Storage;

        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine(e.ExceptionObject.ToString());
                Environment.Exit(1);
            };

            var storagePath = Path.Combine(GetExePath(), "data");
            Storage = new Storage(storagePath);
           
            if (args.Length > 0)
            {
                Analyze(args[0], true, Storage);
                return;
            }

            StartHttpServer();
        }

        public static void Analyze(string assemblyPath, bool saveResults, Storage storage)
        {
            var ePrev = AnalyzesProgress.Started();
            var analyzer = new Analyzer(eCurrent =>
            {
                ePrev = DoProgress(eCurrent, ePrev);
            });

            var graph = analyzer.Analyze(assemblyPath);
            var dto = Mapper.DynamicMap<GraphDto>(graph);
            if (saveResults)storage.Save(dto);
        }

        private static AnalyzesProgress DoProgress(AnalyzesProgress eCurrent, AnalyzesProgress ePrev)
        {
            if (eCurrent.IsFinished)
            {
                Console.WriteLine("Analyzes finished.");
                return ePrev;
            }
            var currentPercentage = eCurrent.Actual*100/eCurrent.Max;
            int prevPercentage = ePrev.Actual*100/ePrev.Max;
            if (currentPercentage != prevPercentage)
            {
                Console.WriteLine("\r{0}\t{1}", eCurrent.Message, currentPercentage);
                ePrev = eCurrent;
            }
            return ePrev;
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