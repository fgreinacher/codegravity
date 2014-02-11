// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gma.CodeVisuals.Generator.DependencyForceGraph.DataStructures.DependencyGraph;
using Gma.CodeVisuals.Generator.DependencyForceGraph.DataStructures.TypeTree;
using Gma.CodeVisuals.Generator.DependencyForceGraph.Do;

#endregion

namespace Gma.CodeVisuals.Generator.DependencyForceGraph
{
    internal class Analyzer
    {
        private readonly Action<AnalyzesProgress> m_OnProgres;
        private AdjacencyMatrix m_Matrix;
        private Tree m_Tree;

        public Analyzer()
            : this(null)
        {
        }

        public Analyzer(Action<AnalyzesProgress> onProgres)
        {
            m_OnProgres = onProgres;
            m_Tree = new Tree(new NodeFactory(), string.Empty);
            Progress = AnalyzesProgress.Started();
        }

        public AnalyzesProgress Progress { get; private set; }

        public Graph GetGraph()
        {
            var tree = m_Tree.Root;
            var links = m_Matrix.All();
            return new Graph(tree, links);
        }


        public Graph Analyze(IEnumerable<Assembly> assemblies, string rootName)
        {
            var types = assemblies
                .SelectMany(a => a.Types())
                .ToArray();
            Analyze(types, rootName);
            return GetGraph();
        }

        private void Analyze(Type[] types, string rootName)
        {
            m_Tree = BuildTree(types, rootName);
            var nodesById = m_Tree.Nodes.Reverse().ToArray();
            m_Matrix = new AdjacencyMatrix(nodesById.Length);
            InvokeOnProgress(new AnalyzesProgress(string.Format("{0} types found.", types.Length), 0, 1, false));
            if (types.Length == 0) return;
            DoAnalyzeTasks(types);
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

        private void DoAnalyzeTasks(Type[] types)
        {
            DoAnalyzeTask(types, AnalyzerExtensions.Uses, "Analyzing uses");
            DoAnalyzeTask(types, AnalyzerExtensions.Implements, "Analyzing implements");
            DoAnalyzeTask(types, AnalyzerExtensions.Contains, "Analyzing contain");
            DoAnalyzeTask(types, AnalyzerExtensions.Calls, "Analyzing calls");
        }


        private void DoAnalyzeTask(Type[] types, Func<Type, IEnumerable<Dependency>> getter, string taskName)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                foreach (var dependecy in getter(type))
                {
                    Edge edge;
                    Add(dependecy, out edge);
                }
                InvokeOnProgress(new AnalyzesProgress(taskName, i, types.Length, false));
            }
            InvokeOnProgress(new AnalyzesProgress(taskName, types.Length, types.Length, false));
        }

        private void InvokeOnProgress(AnalyzesProgress eventArgs)
        {
            Progress = eventArgs;
            if (m_OnProgres == null) return;
            m_OnProgres.Invoke(eventArgs);
        }

        private void Add(Dependency dependency, out Edge edge)
        {
            edge = null;
            INode source;
            bool sourceFound = m_Tree.TryGet(dependency.Source.FullName, out source);
            if (!sourceFound) return;

            INode target;
            bool targetFound = m_Tree.TryGet(dependency.Target.FullName, out target);
            if (!targetFound) return;

            m_Matrix.Add(source.Id, target.Id, dependency.Kind, out edge);
        }
    }
}