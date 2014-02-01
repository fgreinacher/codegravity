// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;

#endregion

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    internal class AdjacencyMatrix
    {
        private readonly DependencyKinds[,] m_Matrix;

        public AdjacencyMatrix(int count)
        {
            m_Matrix = new DependencyKinds[count, count];
        }

        public bool Add(int source, int target, DependencyKind kind, out Edge edge)
        {
            return Add(source, target, kind.ToFlags(), out edge);
        }

        public bool Add(int source, int target, DependencyKinds kinds, out Edge edge)
        {
            edge = null;
            var value = m_Matrix[source, target];
            if (value == kinds) return false;
            m_Matrix[source, target] = value | kinds;
            edge = new Edge(source, target, kinds);
            return true;
        }

        public IEnumerable<Edge> Out(int id)
        {
            for (int i = 0; i < m_Matrix.GetLength(1); i++)
            {
                var value = m_Matrix[id, i];
                if (value == DependencyKinds.None) continue;
                yield return new Edge(id, i, value);
            }
        }

        public IEnumerable<Edge> In(int id)
        {
            for (int i = 0; i < m_Matrix.GetLength(0); i++)
            {
                var value = m_Matrix[i, id];
                if (value == DependencyKinds.None) continue;
                yield return new Edge(i, id, value);
            }
        }

        public IEnumerable<Edge> All()
        {
            for (int i = 0; i < m_Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < m_Matrix.GetLength(1); j++)
                {
                    var value = m_Matrix[i, j];
                    if (value == DependencyKinds.None) continue;
                    yield return new Edge(i, j, value);
                }
            }
        }
    }
}