using System.Collections.Generic;

namespace nsplit.DataStructures
{
    internal interface IQualifiedTreeNode
    {
        int Id { get; }
        string Name { get; }
        IEnumerable<IQualifiedTreeNode> Children();
        bool IsLeaf();
    }
}