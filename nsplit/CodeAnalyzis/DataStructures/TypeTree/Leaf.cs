#region usings

using System.Collections.Generic;
using System.Linq;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.TypeTree
{
    internal class Leaf : NodeBase
    {
        public Leaf(string name, int id) : base(name, id)
        {
        }

        public override IEnumerable<INode> Children()
        {
            return Enumerable.Empty<INode>();
        }

        public override bool IsLeaf()
        {
            return true;
        }
    }
}