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
        private readonly NodeBase m_Parent;
        private readonly string m_Name;

        internal NodeBase(string name, int id, NodeBase parent)
        {
            m_Name = name;
            m_Id = id;
            m_Parent = parent;
        }

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public NodeBase Parent
        {
            get { return m_Parent; }
        }

        public abstract IEnumerable<INode> Children();
        public abstract bool IsLeaf();

        public IEnumerable<INode> Leafs()
        {
            if (IsLeaf()) yield return this;
            foreach (var child in Children())
            {
                foreach (var leaf in child.Leafs())
                {
                    yield return leaf;
                }
            }
        }

        public IEnumerable<INode> Path()
        {
            var current = this;
            while (current!=null)
            {
                yield return current;
                current = current.Parent;
            }
        } 
    }
}