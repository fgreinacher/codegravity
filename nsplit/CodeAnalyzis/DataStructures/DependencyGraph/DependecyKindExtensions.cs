using System;
using System.Collections.Generic;

namespace nsplit.CodeAnalyzis.DataStructures.DependencyGraph
{
    public static class DependecyKindExtensions
    {
        public static DependencyKinds ToFlags(this DependencyKind kind)
        {
            return (DependencyKinds) (int) kind;
        }

        public static IEnumerable<DependencyKinds> Flatten(this DependencyKinds kinds)
        {
            foreach (DependencyKind kind in Enum.GetValues(typeof(DependencyKind)))
            {
                var mask = kind.ToFlags();
                var matches = kinds & mask;
                if (matches==DependencyKinds.None) continue;
                yield return mask;
            }
        }

    }
}