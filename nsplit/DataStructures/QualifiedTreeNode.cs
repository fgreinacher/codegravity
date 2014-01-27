using System.Collections.Generic;
using System.Linq;

namespace nsplit.DataStructures
{
    internal class QualifiedTreeNode : QualifiedTreeNodeBase
    {
        public override IEnumerable<IQualifiedTreeNode> Children()
        {
            return m_ChildNodes.Values.OrderBy(node => node.Name).Concat(m_Leafs.Values.OrderBy(l => l.Name).Cast<IQualifiedTreeNode>());
        }

        private readonly Dictionary<string, QualifiedTreeNode> m_ChildNodes;
        private readonly Dictionary<string, QualifiedTreeLeaf> m_Leafs;

        internal QualifiedTreeNode(string name, int id) : base(name, id)
        {
            m_ChildNodes = new Dictionary<string, QualifiedTreeNode>();
            m_Leafs = new Dictionary<string, QualifiedTreeLeaf>();
        }


        public override bool IsLeaf()
        {
            return false;
        }

        internal QualifiedTreeNode GetOrCreate(Queue<string> nodeNames, QualifiedTreeNodeFactory factory)
        {
            if (nodeNames.Count == 0) return this;
            var name = nodeNames.Dequeue();
            QualifiedTreeNode node;
            bool isOk = m_ChildNodes.TryGetValue(name, out node);
            if (!isOk)
            {
                node = factory.CreateNode(name);
                m_ChildNodes.Add(name, node);
            }
            return node.GetOrCreate(nodeNames, factory);
        }

        public void AddLeaf(QualifiedTreeLeaf leaf)
        {
            m_Leafs.Add(leaf.Name, leaf);
        }

        public bool TryGetLeaf(string leaf, out QualifiedTreeLeaf qualifiedTreeNode)
        {
            return m_Leafs.TryGetValue(leaf, out qualifiedTreeNode);
        }
    }
}