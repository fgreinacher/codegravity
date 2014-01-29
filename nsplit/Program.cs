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
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;
using nsplit.Helper;

namespace nsplit
{
    internal class Program
    {
        public static AdjacencyMatrix DependencyGraph;
        public static Tree TypeTree;


        private static void Main(string[] args)
        {
            Assembly assembly = typeof(Program).Assembly;

            //-----------------------------------------
            var typeTreeBuilder = new TypeTreeBuilder();
            typeTreeBuilder.Add(assembly);
            TypeTree = typeTreeBuilder.Tree;

            var dependen = new DependencyGraphBuilder(TypeTree);
            dependen.Add(assembly);
            DependencyGraph = dependen.AdjacencyMatrix;
            //-----------------------------------------


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