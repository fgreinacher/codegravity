using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit.CodeAnalyzis
{
    internal class DependencyGraph
    {
        private readonly Tree m_Tree;
        private readonly AdjacencyMatrix m_Matrix;
        private readonly Type[] m_Types;
        private readonly INode[] m_NodesById;

        public EventHandler<EdgeAddedEventArgs> OnEdgeAdded;
        public EventHandler<BuildProgressEventArgs> OnProgress;

        public DependencyGraph(Tree tree, Type[] types, AdjacencyMatrix matrix, INode[] nodesById)
        {
            m_Tree = tree;
            m_Types = types;
            m_Matrix = matrix;
            m_NodesById = nodesById;
        }

        public static DependencyGraph StartBuildAsync(Assembly assembly)
        {
            var types = assembly.Types().ToArray();
            var tree = BuildTree(types);
            var nodesById = tree.Nodes.Reverse().ToArray();
            var matrix = new AdjacencyMatrix(nodesById.Length);
            var graph = new DependencyGraph(tree, types, matrix, nodesById);

            var buildTask = new Task(graph.DoBuild);
            buildTask.Start();

            return graph;
        }


        private static Tree BuildTree(IEnumerable<Type> types)
        {
            var nodeFactory = new NodeFactory();
            var tree = new Tree(nodeFactory);
            
            foreach (var type in types)
            {
                var fullName = type.FullName;
                tree.Add(fullName);
            }
            return tree;
        }

        private void DoBuild()
        {
            DoBuildTask(m_Types, AnalyzerExtensions.Uses, "Analyzing uses:");
            DoBuildTask(m_Types, AnalyzerExtensions.Implements, "Analyzing implements:");
            DoBuildTask(m_Types, AnalyzerExtensions.Calls, "Analyzing calls:");
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
      

        private void DoBuildTask(Type[] types, Func<Type, IEnumerable<Dependency>> getter, string taskName)
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
                InvokeOnProgress(taskName, i, types.Length);
            }
        }

        private void InvokeOnProgress(string taskName, int actual, int max)
        {
            var handler = OnProgress;
            if (handler == null) return;
            var eventArgs = new BuildProgressEventArgs(taskName, actual, max);
            handler.Invoke(this, eventArgs);
        }

        private bool Add(Dependency dependency, out Edge edge)
        {
            edge = null;
            //NOTE: For instance IEnumerable.FullName == null
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependency.Source.FullName==null) return false;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependency.Target.FullName==null) return false;

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
    }

    internal class BuildProgressEventArgs : EventArgs
    {
        private readonly string m_TaskName;
        private readonly int m_Actual;
        private readonly int m_Max;

        public BuildProgressEventArgs(string taskName, int actual, int max)
        {
            m_TaskName = taskName;
            m_Actual = actual;
            m_Max = max;
        }

        public string TaskName
        {
            get { return m_TaskName; }
        }

        public int Actual
        {
            get { return m_Actual; }
        }

        public int Max
        {
            get { return m_Max; }
        }
    }

    internal class EdgeAddedEventArgs : EventArgs
    {
        private readonly Edge m_Edge;

        public EdgeAddedEventArgs(Edge edge)
        {
            m_Edge = edge;
        }

        public Edge Edge
        {
            get { return m_Edge; }
        }
    }
}