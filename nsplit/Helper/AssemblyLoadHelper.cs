// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.IO;
using System.Reflection;

#endregion

namespace nsplit.Helper
{
    internal class AssemblyLoadHelper : IDisposable
    {



        public static bool TryLoad(string path, out Assembly assembly, out string message)
        {
            try
            {
                assembly = Assembly.LoadFile(path);
            }
            catch (BadImageFormatException ex1)
            {
                assembly = null;
                message = ex1.Message;
                return false;
            }
            catch (IOException ex2)
            {
                assembly = null;
                message = ex2.Message;
                return false;
            }
            message = assembly.GetName().Name;
            return true;
        }



        public void Dispose()
        {
            
        }
    }
}