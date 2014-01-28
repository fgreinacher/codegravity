using System;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    public class Uses : Dependecy
    {
        public Uses(Type source, Type target)
            : base(source, target)
        {
        }

        public override DependencyKind Kind
        {
            get { return DependencyKind.Uses; }
        }
    }
}