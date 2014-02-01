using System;

namespace nsplit.CodeAnalyzis
{
    public abstract class Dependency
    {
        private readonly Type m_Source;
        private readonly Type m_Target;
        private readonly DependencyKind m_Kind;

        protected Dependency(Type source, Type target, DependencyKind kind)
        {
            m_Source = Unnest(source);
            m_Target = Unnest(target);
            m_Kind = kind;
        }

        public Type Target
        {
            get { return m_Target; }
        }

        public Type Source
        {
            get { return m_Source; }
        }

        public DependencyKind Kind
        {
            get { return m_Kind; }
        }

        protected static Type Unnest(Type type)
        {
            if (type.IsNested && type.DeclaringType != null) return Unnest(type.DeclaringType);
            return type;
        }

        public override int GetHashCode()
        {
            return m_Source.GetHashCode() ^ m_Target.GetHashCode() ^ m_Kind.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var otherDependency = other as Dependency;
            return otherDependency != null &&
                   otherDependency.m_Source == m_Source &&
                   otherDependency.m_Target == m_Target &&
                   otherDependency.m_Kind == m_Kind;
        }

        public override string ToString()
        {
            return string.Format("{0,20} => {1,-20}", Source.Name, Target.Name);
        }
    }
}