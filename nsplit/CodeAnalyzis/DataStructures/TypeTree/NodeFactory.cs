#region usings

using System.Collections.Generic;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.TypeTree
{
    internal class NodeFactory
    {
        private int m_GlobalCounter;
        private readonly Dictionary<int, INode> m_NodesById;

        public NodeFactory()
        {
            m_NodesById = new Dictionary<int, INode>();
        }

        public int Counter
        {
            get { return m_GlobalCounter; }
        }

        public bool TryGetNode(int id, out INode node)
        {
            return m_NodesById.TryGetValue(id, out node);
        }

        public Node CreateNode(string name)
        {
            var node = new Node(name, m_GlobalCounter);
            RegisterNode(node);
            return node;
        }

        public Leaf CreateLeaf(string name)
        {
            var leaf = new Leaf(name, m_GlobalCounter);
            RegisterNode(leaf);
            return leaf;
        }

        private void RegisterNode(NodeBase node)
        {
            m_NodesById.Add(m_GlobalCounter, node);
            m_GlobalCounter++;
        }
    }
}