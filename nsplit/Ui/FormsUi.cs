// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.IO;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace nsplit.Ui
{
    internal class FormsUi : AssemblyLoadUi
    {
        public override bool TryLoadAssembly(string[] args, out Assembly assembly, out string message)
        {
            message = string.Empty;
            var path = args.Length > 0 ? args[0] : string.Empty;
            if (File.Exists(path))
            {
                return TryLoadAssembly(path, out assembly, out message);
            }
            var dialog = new OpenFileDialog
            {
                Title = "Select an assembly to analyze",
                Filter = "All files (*.*)|*.*|Assemblies (*.dll, *.exe)|*.dll;*.exe",
                InitialDirectory = path,
                RestoreDirectory = true,
                FilterIndex = 2
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return TryLoadAssembly(dialog.FileName, out assembly, out message);
            }
            assembly = null;
            return false;
        }
    }
}