using System;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

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