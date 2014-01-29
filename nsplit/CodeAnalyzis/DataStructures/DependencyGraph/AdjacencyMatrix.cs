using System.Collections.Generic;
using System.Linq;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    internal class AdjacencyMatrix
    {
        private readonly DependencyKinds[,] m_Matrix;

        public AdjacencyMatrix(int count)
        {
            m_Matrix = new DependencyKinds[count, count];
        }

        public void Add(Edge edge)
        {
            Add(edge.Source, edge.Target, edge.Kinds);
        }

        public void Add(int source, int target, DependencyKind kind)
        {
            Add(source, target, kind.ToFlags());
        }

        public void Add(int source, int target, DependencyKinds kinds)
        {
            var value = m_Matrix[source, target];
            m_Matrix[source, target] = value | kinds;
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

        public IEnumerable<Edge> All(INode node)
        {
            var allLeafs = node.GetLeafsRecursively().ToArray();
            var outDeps = allLeafs.SelectMany(n => Program.DependencyGraph.Out(node.Id));
            var inDeps = allLeafs.SelectMany(n => Program.DependencyGraph.In(node.Id));
            return outDeps.Concat(inDeps).SelectMany(e => e.FlattenFlags()).Distinct();
        }
    }
}