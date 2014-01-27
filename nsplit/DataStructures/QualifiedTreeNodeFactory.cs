using System.Collections.Generic;

namespace nsplit.DataStructures
{
    internal class QualifiedTreeNodeFactory
    {
        private readonly Dictionary<int, IQualifiedTreeNode> m_NodesById;
        private static int _globalCounter = 0;

        public QualifiedTreeNodeFactory()
        {
            m_NodesById = new Dictionary<int, IQualifiedTreeNode>();
        }

        public bool TryGetNode(int id, out IQualifiedTreeNode qualifiedTreeNode)
        {
            return m_NodesById.TryGetValue(id, out qualifiedTreeNode);
        }

        internal QualifiedTreeNode CreateNode(string name)
        {
            var node = new QualifiedTreeNode(name, _globalCounter);
            RegisterNode(node);
            return node;
        }

        private void RegisterNode(QualifiedTreeNodeBase node)
        {
            m_NodesById.Add(_globalCounter, node);
            _globalCounter++;
        }

        public QualifiedTreeLeaf CreateLeaf(string name)
        {
            var leaf = new QualifiedTreeLeaf(name, _globalCounter);
            RegisterNode(leaf);
            return leaf;
        }
    }
}