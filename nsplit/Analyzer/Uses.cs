using System;

namespace nsplit.Analyzer
{
    public class Uses : Dependecy
    {
        public Uses(Type @from, Type to)
            : base(@from, to)
        {
        }

        public override DependencyKind DependencyKind
        {
            get { return DependencyKind.Uses; }
        }
    }
}