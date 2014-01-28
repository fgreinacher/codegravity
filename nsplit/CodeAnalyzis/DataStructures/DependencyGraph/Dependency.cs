using System;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    public abstract class Dependency
    {
        private readonly Type m_Source;
        private readonly Type m_Target;

        protected Dependency(Type source, Type target)
        {
            m_Source = Unnest(source);
            m_Target = Unnest(target);
        }

        public abstract DependencyKind Kind { get; }

        public Type Target
        {
            get { return m_Target; }
        }

        public Type Source
        {
            get { return m_Source; }
        }

        private static Type Unnest(Type type)
        {
            if (type.IsNested && type.DeclaringType != null) return Unnest(type.DeclaringType);
            return type;
        }

        public override string ToString()
        {
            return string.Format("{0,20} => {1,-20}", Source.Name, Target.Name);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ Target.GetHashCode() ^ Kind.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Dependency;
            return other != null &&
                   other.Source == Source &&
                   other.Target == Target &&
                   other.Kind == Kind;
        }
    }
}