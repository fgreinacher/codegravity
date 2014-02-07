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
    internal class Analyzer
    {
        private readonly AdjacencyMatrix m_Matrix;
        private readonly INode[] m_NodesById;
        private readonly Tree m_Tree;
        private readonly Type[] m_Types;
        private readonly string m_Name;

        public Analyzer(Tree tree, Type[] types, AdjacencyMatrix matrix, INode[] nodesById, string name)
        {
            m_Tree = tree;
            m_Types = types;
            m_Matrix = matrix;
            m_NodesById = nodesById;
            m_Name = name;
        }

        public event EventHandler<EdgeAddedEventArgs> OnEdgeAdded;
        public event EventHandler<AnalyzesProgressEventArgs> OnProgress;

        public static Analyzer Empty()
        {
            var types = new Type[0];
            var tree = BuildTree(types, string.Empty);
            var nodesById = new INode[0];
            var matrix = new AdjacencyMatrix(0);
            var graph = new Analyzer(tree, types, matrix, nodesById, "NULL");
            return graph;
        }

        public static Analyzer StartAnalyzesAsync(Assembly assembly, CancellationToken token)
        {
            var graph = AnalyzeSyncPart(assembly);

            Task.Factory
                .StartNew(graph.Analyze, token)
                .ContinueWith(task => graph.InvokeOnProgress(AnalyzesProgressEventArgs.Finished()), token);

            return graph;
        }

        public static Analyzer Analyze(Assembly assembly)
        {
            var graph = AnalyzeSyncPart(assembly);
            graph.Analyze();
            return graph;
        }

        private static Analyzer AnalyzeSyncPart(Assembly assembly)
        {
            var types = assembly.Types().ToArray();
            var rootName = assembly.GetName().Name;
            var tree = BuildTree(types, rootName);
            var nodesById = tree.Nodes.Reverse().ToArray();
            var matrix = new AdjacencyMatrix(nodesById.Length);
            var graph = new Analyzer(tree, types, matrix, nodesById, assembly.GetName().Name);
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

        public Graph GetGraph()
        {
            var tree = m_NodesById[0];
            var links = m_Matrix.All();
            return new Graph(tree, links, m_Name);
        }
    }
}