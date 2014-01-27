using System.Collections.Generic;
using System.Linq;

namespace nsplit.DataStructures
{
    internal class QualifiedTreeLeaf : QualifiedTreeNodeBase
    {
        public QualifiedTreeLeaf(string name, int id) : base(name, id)
        {
        }

        public override IEnumerable<IQualifiedTreeNode> Children()
        {
            return Enumerable.Empty<IQualifiedTreeNode>(); 
        }

        public override bool IsLeaf()
        {
            return true;
        }
    }
}