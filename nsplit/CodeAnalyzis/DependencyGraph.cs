// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;
using nsplit.CodeAnalyzis.Do;

#endregion

namespace nsplit.CodeAnalyzis
{
    internal class DependencyGraph
    {
        private readonly AdjacencyMatrix m_Matrix;
        private readonly INode[] m_NodesById;
        private readonly Tree m_Tree;
        private readonly Type[] m_Types;

        public DependencyGraph(Tree tree, Type[] types, AdjacencyMatrix matrix, INode[] nodesById)
        {
            m_Tree = tree;
            m_Types = types;
            m_Matrix = matrix;
            m_NodesById = nodesById;
        }

        public event EventHandler<EdgeAddedEventArgs> OnEdgeAdded;
        public event EventHandler<AnalyzesProgressEventArgs> OnProgress;

        public static DependencyGraph StartAnalyzesAsync(Assembly assembly, CancellationToken token)
        {
            var types = assembly.Types().ToArray();
            var rootName = assembly.GetName().Name;
            var tree = BuildTree(types, rootName);
            var nodesById = tree.Nodes.Reverse().ToArray();
            var matrix = new AdjacencyMatrix(nodesById.Length);
            var graph = new DependencyGraph(tree, types, matrix, nodesById);

            Task.Factory
                .StartNew(graph.Analyze, token)
                .ContinueWith(task => graph.InvokeOnProgress(AnalyzesProgressEventArgs.Finished()), token);

            return graph;
        }


        private static Tree BuildTree(IEnumerable<Type> types, string rootName)
        {
            var nodeFactory = new NodeFactory();
            var tree = new Tree(nodeFactory, rootName);

            foreach (var type in types)
            {
                var fullName = type.FullName;
                tree.Add(fullName);
            }
            return tree;
        }

        private void Analyze()
        {
            DoAnalyzeTask(m_Types, AnalyzerExtensions.Uses, "Analyzing uses");
            DoAnalyzeTask(m_Types, AnalyzerExtensions.Implements, "Analyzing implements");
            DoAnalyzeTask(m_Types, AnalyzerExtensions.Contains, "Analyzing contain");
            DoAnalyzeTask(m_Types, AnalyzerExtensions.Calls, "Analyzing calls");
        }

        public INode GetNode(int idNo)
        {
            return m_NodesById[idNo];
        }

        public IEnumerable<Edge> InOut(string id)
        {
            int idNo = (id == "#") ? 0 : Int32.Parse(id);
            INode node = GetNode(idNo);
            var allLeafs = node.Leafs().ToArray();
            var outDeps = allLeafs.SelectMany(n => m_Matrix.Out(node.Id));
            var inDeps = allLeafs.SelectMany(n => m_Matrix.In(node.Id));
            return outDeps.Concat(inDeps).SelectMany(e => e.FlattenFlags()).Distinct();
        }


        private void DoAnalyzeTask(Type[] types, Func<Type, IEnumerable<Dependency>> getter, string taskName)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                foreach (var dependecy in getter(type))
                {
                    Edge edge;
                    bool added = Add(dependecy, out edge);
                    if (!added) continue;
                    InvokeOnEdgeAdded(edge);
                }
                InvokeOnProgress(new AnalyzesProgressEventArgs(taskName, i, types.Length, false));
            }
            InvokeOnProgress(new AnalyzesProgressEventArgs(taskName, types.Length, types.Length, false));
        }

        private void InvokeOnProgress(AnalyzesProgressEventArgs eventArgs)
        {
            var handler = OnProgress;
            if (handler == null) return;
            handler.Invoke(this, eventArgs);
        }

        private bool Add(Dependency dependency, out Edge edge)
        {
            edge = null;
            //NOTE: For instance IEnumerable.FullName == null
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependency.Source.FullName == null) return false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependency.Target.FullName == null) return false;

            INode source;
            bool sourceFound = m_Tree.TryGet(dependency.Source.FullName, out source);
            if (!sourceFound) return false;

            INode target;
            bool targetFound = m_Tree.TryGet(dependency.Target.FullName, out target);
            if (!targetFound) return false;

            return m_Matrix.Add(source.Id, target.Id, dependency.Kind, out edge);
        }

        private void InvokeOnEdgeAdded(Edge edge)
        {
            var handler = OnEdgeAdded;
            if (handler == null) return;
            var eventArgs = new EdgeAddedEventArgs(edge);
            handler.Invoke(this, eventArgs);
        }

        public IEnumerable<Edge> All()
        {
            return m_Matrix.All();
        }
    }
}