using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Xml;
using nsplit.Analyzer;
using nsplit.DataStructures;
using nsplit.Helper;

namespace nsplit
{
    internal class Program
    {
        public static Dictionary<string, Type> Types;
        public static QualifiedTree TypeTree;


        private static void Main(string[] args)
        {
            Types = new Dictionary<string, Type>();
            Assembly assembly = typeof(XmlReader).Assembly;
            foreach (Type type in assembly.Types().Take(30))
            {
                Types.Add(type.FullName, type);
            }

            //-----------------------------------------
            var typeTreeBuilder = new TypeTreeBuilder();
            typeTreeBuilder.Add(assembly);
            TypeTree = typeTreeBuilder.Tree;
            //-----------------------------------------



            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            var currentExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "html");

            Func<HttpRequestMessage, bool> matchesAllExceptApi = request => !request.RequestUri.AbsolutePath.StartsWith("/api/");
            var webServerOnFolder = new WebServerOnFolder(
                matchesAllExceptApi, 
                currentExePath,
                FileType.Html,
                FileType.Css,
                FileType.Javascript,
                FileType.Gif,
                FileType.Png);

            config.MessageHandlers.Add(webServerOnFolder);
            config.Routes.MapHttpRoute(
                "API", 
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional});

            //Reomove XML to see JSON in Browser
            MediaTypeHeaderValue appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

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
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }
    }
}