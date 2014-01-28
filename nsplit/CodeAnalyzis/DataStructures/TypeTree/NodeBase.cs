#region usings

using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.TypeTree
{
    [DebuggerDisplay("{Name}:{Id}}")]
    internal abstract class NodeBase : INode
    {
        private readonly int m_Id;
        private readonly string m_Name;

        internal NodeBase(string name, int id)
        {
            m_Name = name;
            m_Id = id;
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public abstract IEnumerable<INode> Children();
        public abstract bool IsLeaf();
    }
}