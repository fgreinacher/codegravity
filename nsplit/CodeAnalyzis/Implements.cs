using System;

namespace nsplit.CodeAnalyzis
{
    public class Implements : Dependency
    {
        public Implements(Type source, Type target)
            : base(source, target)
        {
        }

        public override DependencyKind Kind
        {
            get { return DependencyKind.Implements; }
        }
    }
}