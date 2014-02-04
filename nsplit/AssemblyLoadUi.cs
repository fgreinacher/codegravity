// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.IO;
using System.Reflection;

#endregion

namespace nsplit
{
    internal abstract class AssemblyLoadUi
    {
        public abstract bool TryLoadAssembly(string[] args, out Assembly assembly);

        protected bool TryLoadAssembly(string path, out Assembly assembly)
        {
            string folderPath = Path.GetDirectoryName(path);
            RegisterFolderResolver(folderPath);
            try
            {
                assembly = Assembly.LoadFile(path);
            }
            catch (BadImageFormatException ex1)
            {
                assembly = null;
                Console.WriteLine(ex1.Message);
            }
            catch (IOException ex2)
            {
                assembly = null;
                Console.WriteLine(ex2.Message);
            }
            return assembly != null;
        }

        protected void RegisterFolderResolver(string folderPath)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
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
            };
        }
    }
}