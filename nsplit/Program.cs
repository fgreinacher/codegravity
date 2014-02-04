// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

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
        [STAThread]
        private static void Main(string[] args)
        {
            Assembly assembly;

            Console.WriteLine("Press ENTER to continue, press ESC for DEMO mode, any other key to quit.");
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    assembly = typeof (Program).Assembly;
                    break;
                case ConsoleKey.Enter:
                    AssemblyLoadUi ui = new FormsUi();
                    bool isOk = ui.TryLoadAssembly(args, out assembly);
                    if (!isOk)
                    {
                        Console.WriteLine("Error occured. Press any key to quit.");
                        Console.ReadKey();
                    }
                    break;
                default:
                    return;
            }


            AppState.Build(assembly);
            StartHttpServer();
        }

        private static void StartHttpServer()
        {
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

        private static string GetExePath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}