using System;

namespace nsplit.Analyzer
{
    public abstract class Dependecy
    {
        private readonly Type m_From;
        private readonly Type m_To;

        protected Dependecy(Type @from, Type to)
        {
            m_From = Unnest(@from);
            m_To = Unnest(to);
        }

        public abstract DependencyKind DependencyKind { get; }

        public Type To
        {
            get { return m_To; }
        }

        public Type From
        {
            get { return m_From; }
        }

        protected static Type Unnest(Type type)
        {
            if (type.IsNested && type.DeclaringType != null) return Unnest(type.DeclaringType);
            return type;
        }

        public override string ToString()
        {
            return string.Format("{0,20} => {1,-20}", From.Name, To.Name);
        }
    }
}