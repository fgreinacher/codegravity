using System;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    [Flags]
    public enum DependencyKinds
    {
        None = 0,
        MethodCall = 1,
        Uses = 2,
        Implements = 4,
    }
}