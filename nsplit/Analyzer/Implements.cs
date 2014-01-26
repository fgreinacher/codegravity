using System;

namespace nsplit.Analyzer
{
    public class Implements : Dependecy
    {
        public Implements(Type @from, Type to)
            : base(@from, to)
        {
        }

        public override DependencyKind DependencyKind
        {
            get { return DependencyKind.Implements; }
        }
    }
}