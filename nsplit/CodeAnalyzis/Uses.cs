using System;

namespace nsplit.CodeAnalyzis
{
    public class Uses : Dependency
    {
        public Uses(Type source, Type target)
            : base(source, target, DependencyKind.Uses)
        {
        }
    }
}