// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;
using nsplit.CodeAnalyzis.Do;
using nsplit.Helper;

#endregion

namespace nsplit.CodeAnalyzis
{
    internal class Analyzer
    {
        private readonly Action<AnalyzesProgress> m_OnProgres;
        private Tree m_Tree;
        private AdjacencyMatrix m_Matrix;

        public Analyzer(Action<AnalyzesProgress> onProgres)
        {
            m_OnProgres = onProgres;
            m_Tree = new Tree(new NodeFactory(), string.Empty);
        }

        public Graph GetGraph()
        {
            var tree = m_Tree.Root;
            var links = m_Matrix.All();
            return new Graph(tree, links);
        }

        public Graph Analyze(string assemblyPath)
        {
            var directory = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
            ResolveEventHandler handler = (sender, args) => Resolve(args, directory);
            AppDomain.CurrentDomain.AssemblyResolve += handler;
            try
            {
                var assembly = Assembly.LoadFile(assemblyPath);
                Analyze(assembly);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= handler;
            }
            return GetGraph();
        }

        private Graph Analyze(Assembly assembly)
        {
            var types = assembly.Types().ToArray();
            var rootName = assembly.GetName().Name;
            m_Tree = BuildTree(types, rootName);
            var nodesById = m_Tree.Nodes.Reverse().ToArray();
            m_Matrix = new AdjacencyMatrix(nodesById.Length);
            DoAnalyzeTasks(types);
            return GetGraph();
        }

        private static Assembly Resolve(ResolveEventArgs args, string directory)
        {
            var name = args.Name;
            var fileName = name.Substring(0, name.IndexOf(','));
            var fullPath = Path.Combine(directory, fileName + ".dll");
            if (File.Exists(fullPath)) return Assembly.LoadFile(fullPath);
            fullPath = Path.Combine(directory, fileName + ".exe");
            if (File.Exists(fullPath)) return Assembly.LoadFile(fullPath);
            Console.WriteLine("Can not resolve assembly [{0}] in folder [{1}].", name, directory);
            return null;
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
                    bool added = Add(dependecy, out edge);
                    if (!added) continue;
                }
                InvokeOnProgress(new AnalyzesProgress(taskName, i, types.Length, false));
            }
            InvokeOnProgress(new AnalyzesProgress(taskName, types.Length, types.Length, false));
        }

        private void InvokeOnProgress(AnalyzesProgress eventArgs)
        {
            if (m_OnProgres == null) return;
            m_OnProgres.Invoke(eventArgs);
        }

        private bool Add(Dependency dependency, out Edge edge)
        {
            edge = null;
            //NOTE: For instance IEnumerable.FullName == null
            if (dependency.Source.FullName == null) return false;
            if (dependency.Target.FullName == null) return false;

            INode source;
            bool sourceFound = m_Tree.TryGet(dependency.Source.FullName, out source);
            if (!sourceFound) return false;

            INode target;
            bool targetFound = m_Tree.TryGet(dependency.Target.FullName, out target);
            if (!targetFound) return false;

            return m_Matrix.Add(source.Id, target.Id, dependency.Kind, out edge);
        }
    }
}