// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.IO;
using System.Reflection;

#endregion

namespace nsplit.Ui
{
    internal class ConsoleUi : AssemblyLoadUi
    {
        public override bool TryLoadAssembly(string[] args, out Assembly assembly, out string message)
        {
            message = string.Empty;
            string filePath = string.Empty;
            assembly = null;
            string path;
            if (args != null && args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                path = string.Empty;
            }

            if (File.Exists(path))
            {
                string folderPath = Path.GetDirectoryName(path);
                RegisterFolderResolver(folderPath);
                filePath = path;
            }
            else
            {
                if (Directory.Exists(path))
                {
                    string folderPath = path;
                    RegisterFolderResolver(folderPath);
                    string filenName;
                    bool isOk = LetUserEnterAssemblyName(folderPath, out filenName);
                    if (!isOk) return false;
                }
                else
                {
                    Console.WriteLine("Path {0} was not found.", path);
                    Console.WriteLine("Press ENTER to continue in DEMO mode, any other key (ESC) to quit.");
                    var key = Console.ReadKey();
                    assembly = typeof (Program).Assembly;
                    return key.Key == ConsoleKey.Enter;
                }
            }

            assembly = Assembly.LoadFile(filePath);
            return true;
        }

        private static bool LetUserEnterAssemblyName(string folderPath, out string fileName)
        {
            var files = Directory.GetFiles(folderPath, "*.dll");
            for (int index = 0; index < files.Length; index++)
            {
                var file = files[index];
                Console.WriteLine("{0,-3}  {1}", index, file);
            }
            do
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("Type file NUMBER, NAME or Q to quit.");
                var input = Console.ReadLine() ?? string.Empty;
                int id;
                bool isInteger = int.TryParse(input, out id);
                if (input.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileName = null;
                    return false;
                }
                if (isInteger && id >= 0 && id <= files.Length)
                {
                    fileName = files[id];
                    return true;
                }
                fileName = input;
            } while (!File.Exists(fileName));
            return true;
        }
    }
}