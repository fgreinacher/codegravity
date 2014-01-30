using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit
{
    internal static class Registry
    {
        private static DependencyGraph _instance;

        public static void Build(Assembly assembly)
        {
            _instance = DependencyGraph.StartBuildAsync(assembly);
        }

        public static INode GetNode(string id)
        {
            int idNo = (id == "#") ? 0 : Int32.Parse(id);
            return _instance.GetNode(idNo);
        }

        public static INode GetNode(int idNo)
        {
            return _instance.GetNode(idNo);
        }

        public static IEnumerable<Edge> InOut(string id)
        {
            return _instance.InOut(id);
        }

    }
}