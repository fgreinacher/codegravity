// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

#endregion

namespace nsplit
{
    internal static class AppState
    {
        private static DependencyGraph _instance;
        private static AnalyzesProgressEventArgs _currentProgress;
        private static CancellationTokenSource _tokenSource;

        public static void StartAnalyzes(Assembly assembly)
        {
            if (_tokenSource != null)
            {
                Console.WriteLine("Analyzes canceled.");
                _tokenSource.Cancel();
            }
            _tokenSource = new CancellationTokenSource();

            Console.WriteLine("Analyzes started.\n\rAssembly: [{0}]", assembly.GetName());
            _instance = DependencyGraph.StartAnalyzesAsync(assembly, _tokenSource.Token);
            _currentProgress = AnalyzesProgressEventArgs.Started();
            _instance.OnProgress += (sender, e) =>
            {
                if (e.IsFinished) {Console.WriteLine("Analyzes finished.");}
                _currentProgress = e;
                _tokenSource = null;
            };
        }

        public static AnalyzesProgressEventArgs GetProgress()
        {
            return _currentProgress;
        }

        public static IEnumerable<Edge> GetAll()
        {
            return _instance.All();
        }

        public static INode GetNode(string id)
        {
            int idNo = (id == "#") ? 0 : Int32.Parse(id);
            return _instance.GetNode(idNo);
        }

        public static INode GetNode(int idNo)
        {
            return _instance.GetNode(idNo);
        }

        public static IEnumerable<Edge> InOut(string id)
        {
            return _instance.InOut(id);
        }
    }
}