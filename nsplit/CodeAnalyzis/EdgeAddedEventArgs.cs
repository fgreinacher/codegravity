// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

#endregion

namespace nsplit.CodeAnalyzis
{
    internal class EdgeAddedEventArgs : EventArgs
    {
        private readonly Edge m_Edge;

        public EdgeAddedEventArgs(Edge edge)
        {
            m_Edge = edge;
        }

        public Edge Edge
        {
            get { return m_Edge; }
        }
    }
}