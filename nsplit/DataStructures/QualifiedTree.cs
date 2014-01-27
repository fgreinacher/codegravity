using System.Collections.Generic;

namespace nsplit.DataStructures
{
    internal class QualifiedTree
    {
        private readonly QualifiedTreeNodeFactory m_QualifiedTreeNodeFactory;

        public QualifiedTree(QualifiedTreeNodeFactory qualifiedTreeNodeFactory)
        {
            m_QualifiedTreeNodeFactory = qualifiedTreeNodeFactory;
            m_Root = m_QualifiedTreeNodeFactory.CreateNode(string.Empty);
        }

        private readonly QualifiedTreeNode m_Root;

        public IQualifiedTreeNode Add(string fullName)
        {
            var qualifiedName = QualifiedName.Parse(fullName);
            var names = new Queue<string>(qualifiedName.Nodes);
            var node = m_Root.GetOrCreate(names, m_QualifiedTreeNodeFactory);
            var leaf = m_QualifiedTreeNodeFactory.CreateLeaf(qualifiedName.Leaf);
            node.AddLeaf(leaf);
            return leaf;
        }

        public bool TryGet(int id, out IQualifiedTreeNode leaf)
        {
            return m_QualifiedTreeNodeFactory.TryGetNode(id, out leaf);
        }

        public bool TryGet(string fullName, out IQualifiedTreeNode leaf)
        {
            var qualifiedName = QualifiedName.Parse(fullName);
            var names = new Queue<string>(qualifiedName.Nodes);
            var node = m_Root.GetOrCreate(names, m_QualifiedTreeNodeFactory);
            QualifiedTreeLeaf result;
            bool isOk = node.TryGetLeaf(qualifiedName.Leaf, out result);
            leaf = result;
            return isOk;
        }
    }
}