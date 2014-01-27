using System.Collections.Generic;
using System.Diagnostics;

namespace nsplit.DataStructures
{
    [DebuggerDisplay("{Name}:{Id}}")]
    internal abstract class QualifiedTreeNodeBase : IQualifiedTreeNode
    {
        private readonly string m_Name;
        private readonly int m_Id;

        internal QualifiedTreeNodeBase(string name, int id)
        {
            m_Name = name;
            m_Id = id;
        }

        public int Id { get { return m_Id; } }
        public string Name { get { return m_Name; } }

        public abstract IEnumerable<IQualifiedTreeNode> Children();
        public abstract bool IsLeaf();
    }
}