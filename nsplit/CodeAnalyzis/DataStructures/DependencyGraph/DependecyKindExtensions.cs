// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using nsplit.CodeAnalyzis.Do;

#endregion

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
            foreach (DependencyKind kind in Enum.GetValues(typeof (DependencyKind)))
            {
                var mask = kind.ToFlags();
                var matches = kinds & mask;
                if (matches == DependencyKinds.None) continue;
                yield return mask;
            }
        }
    }
}