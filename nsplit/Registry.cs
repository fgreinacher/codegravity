using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit
{
    internal class Registry
    {
        private static AdjacencyMatrix _dependencyGraph;
        private static Tree _typeTree;
        private static INode[] _typesById;

        public static INode GetNode(string id)
        {
            int idNo = (id == "#") ? 0 : Int32.Parse(id);
            return GetNode(idNo);
        }

        public static INode GetNode(int idNo)
        {
            return _typesById[idNo];
        }

        public static void BuildUp(Assembly assembly)
        {
            var typeTreeBuilder = new TypeTreeBuilder();
            typeTreeBuilder.Add(assembly);
            _typeTree = typeTreeBuilder.Tree;

            _typesById = _typeTree.Nodes.Reverse().ToArray();

            var dependen = new DependencyGraphBuilder(_typeTree);
            dependen.Add(assembly);
            _dependencyGraph = dependen.AdjacencyMatrix;
        }

        public static IEnumerable<Edge> InOut(string id)
        {
            INode node = GetNode(id);
            var allLeafs = node.Leafs().ToArray();
            var outDeps = allLeafs.SelectMany(n => _dependencyGraph.Out(node.Id));
            var inDeps = allLeafs.SelectMany(n => _dependencyGraph.In(node.Id));
            return outDeps.Concat(inDeps).SelectMany(e => e.FlattenFlags()).Distinct();
        }
    }
}