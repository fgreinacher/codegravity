﻿#region usings

using System.Collections.Generic;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.TypeTree
{
    internal interface INode
    {
        int Id { get; }
        string Name { get; }
        IEnumerable<INode> Children();
        bool IsLeaf();
        IEnumerable<INode> Leafs();
        IEnumerable<INode> Path();
    }
}