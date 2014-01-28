#region usings

using System.Collections.Generic;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.TypeTree
{
    internal class NodeFactory
    {
        private static int _globalCounter;
        private readonly Dictionary<int, INode> m_NodesById;

        public NodeFactory()
        {
            m_NodesById = new Dictionary<int, INode>();
        }

        public bool TryGetNode(int id, out INode node)
        {
            return m_NodesById.TryGetValue(id, out node);
        }

        public Node CreateNode(string name)
        {
            var node = new Node(name, _globalCounter);
            RegisterNode(node);
            return node;
        }

        public Leaf CreateLeaf(string name)
        {
            var leaf = new Leaf(name, _globalCounter);
            RegisterNode(leaf);
            return leaf;
        }

        private void RegisterNode(NodeBase node)
        {
            m_NodesById.Add(_globalCounter, node);
            _globalCounter++;
        }
    }
}