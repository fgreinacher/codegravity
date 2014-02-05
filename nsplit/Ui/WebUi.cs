// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Reflection;

#endregion

namespace nsplit.Ui
{
    internal class WebUi : AssemblyLoadUi
    {
        public override bool TryLoadAssembly(string[] args, out Assembly assembly, out string message)
        {
            return TryLoadAssembly(args[0], out assembly, out message);
        }
    }
}