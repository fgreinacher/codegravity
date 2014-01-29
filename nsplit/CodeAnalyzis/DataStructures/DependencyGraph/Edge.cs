using System.Collections.Generic;
using System.Linq;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    internal class Edge
    {
        private readonly int m_Source;
        private readonly int m_Target;
        private readonly DependencyKinds m_Kinds;

        public Edge(int source, int target, DependencyKinds kinds)
        {
            m_Source = source;
            m_Target = target;
            m_Kinds = kinds;
        }

        public int Source
        {
            get { return m_Source; }
        }

        public int Target
        {
            get { return m_Target; }
        }

        public DependencyKinds Kinds
        {
            get { return m_Kinds; }
        }

        public IEnumerable<Edge> FlattenFlags()
        {
            return Kinds.Flatten().Select(kind => new Edge(Source, Target, kind));
        } 

        protected bool Equals(Edge other)
        {
            return m_Source == other.m_Source && m_Target == other.m_Target && m_Kinds == other.m_Kinds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = m_Source;
                hashCode = (hashCode * 397) ^ m_Target;
                hashCode = (hashCode * 397) ^ (int)m_Kinds;
                return hashCode;
            }
        }
    }
}