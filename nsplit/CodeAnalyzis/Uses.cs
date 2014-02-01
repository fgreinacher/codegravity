// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;

#endregion

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