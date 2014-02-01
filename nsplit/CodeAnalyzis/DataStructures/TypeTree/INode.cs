// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

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