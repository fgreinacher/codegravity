using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using nsplit.Analyzer;

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        //public Graph GetAllDependencies()
        //{
        //    Graph graph = new Graph();
        //    //Assembly assembly = Assembly.LoadFile(@"D:\temp\NSplit\Gma.DataStructures.StringSearch.dll");
        //    var assembly = typeof(TypeA).Assembly;

        //    var dependecies = assembly
        //            .Types()
        //            .SelectMany(type => type.Dependecies());

        //    foreach (var dependecy in dependecies)
        //    {
        //        if (dependecy.To.Assembly == assembly)
        //        {
        //            Console.WriteLine(dependecy);
        //            var source = new Node() {Name = dependecy.From.Name};
        //            var target = new Node() { Name = dependecy.To.Name };
        //            graph.Edges.Add(new Edge() {Source = source, Target = target});
        //        }
        //    }

        //    return graph;
        //}


        public IEnumerable<Edge> GetEdgesByNode(string node)
        {
            Type type;

            if (!Program.Types.TryGetValue(node, out type))
            {
                yield break;
            }

            IEnumerable<Dependecy> dependecies = type.Dependecies();

            foreach (Dependecy dependecy in dependecies)
            {
                if (dependecy.To.FullName != null && Program.Types.ContainsKey(dependecy.To.FullName))
                {
                    var source = new Node {Name = dependecy.From.FullName};
                    var target = new Node {Name = dependecy.To.FullName};
                    yield return new Edge {Source = source, Target = target};
                }
            }
        }

        public IEnumerable<Node> GetNodes()
        {
            return Program
                .Types
                .Values
                .Select(type => new Node {Name = type.FullName});
        }
    }
}