using System.Reflection;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit.CodeAnalyzis
{
    internal class DependencyGraphBuilder
    {
        private readonly Tree m_Tree;
        private readonly AdjacencyMatrix m_AdjacencyMatrix;

        public DependencyGraphBuilder(Tree tree)
        {
            m_Tree = tree;
            m_AdjacencyMatrix = new AdjacencyMatrix(tree.Count);
        }

        public void Add(Assembly assembly)
        {
            var types = assembly.Types();
            foreach (var type in types)
            {
                var uses = type.Uses();
                foreach (var dependecy in uses)
                {
                    Add(dependecy);
                }

                var implements = type.Implements();
                foreach (var dependecy in implements)
                {
                    Add(dependecy);
                }

                var calls = type.Calls();
                foreach (var dependecy in calls)
                {
                    Add(dependecy);
                }
            }
        }


        public void Add(Dependecy dependecy)
        {
            //NOTE: For instance IEnumerable.FullName == null
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependecy.Source.FullName==null) return;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (dependecy.Target.FullName==null) return;

            INode source;
            bool sourceFound = m_Tree.TryGet(dependecy.Source.FullName, out source);
            if (!sourceFound) return;

            INode target;
            bool targetFound = m_Tree.TryGet(dependecy.Target.FullName, out target);
            if (!targetFound) return;

            m_AdjacencyMatrix.Add(source.Id, target.Id, dependecy.Kind);
        }

        public AdjacencyMatrix AdjacencyMatrix
        {
            get { return m_AdjacencyMatrix; }
        }
    }
}