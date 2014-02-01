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
using System.Xml;
using nsplit.Helper;

#endregion

namespace nsplit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string folderPath = @"c:\temp\tia\";
            RegisterFolderResolver(folderPath);
            //Assembly assembly = Assembly.LoadFile(Path.Combine(folderPath, assemblyToAnalyze + ".dll"));

            Assembly assembly = typeof (Program).Assembly;

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
                Process.Start("http://localhost:8080/index2.html");
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