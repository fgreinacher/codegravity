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
    internal static class AssemblyLoadHelper
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

        public static Assembly Resolver(string folderPath, ResolveEventArgs resolveArgs)
        {
            var name = resolveArgs.Name;
            var fileName = name.Substring(0, name.IndexOf(','));

            string fullPath = Path.Combine(folderPath, fileName + ".dll");
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(folderPath, fileName + ".exe");
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine("Can not resolve assembly [{0}] in folder [{1}].", name, folderPath);
                    return null;
                }
            }
            return Assembly.LoadFile(fullPath);
        }
    }
}