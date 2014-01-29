using System;

namespace nsplit.CodeAnalyzis
{
    public abstract class Dependecy
    {
        private readonly Type m_Source;
        private readonly Type m_Target;

        protected Dependecy(Type source, Type target)
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

        protected static Type Unnest(Type type)
        {
            if (type.IsNested && type.DeclaringType != null) return Unnest(type.DeclaringType);
            return type;
        }

        public override string ToString()
        {
            return string.Format("{0,20} => {1,-20}", Source.Name, Target.Name);
        }
    }
}