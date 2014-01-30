#region usings

using System;
using System.Collections.Generic;
using System.Reflection;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

#endregion

namespace nsplit
{
    internal static class Registry
    {
        private static DependencyGraph _instance;
        private static int _currentProgress;

        public static void Build(Assembly assembly)
        {
            _instance = DependencyGraph.StartBuildAsync(assembly);
            _currentProgress = 0;
            _instance.OnProgress += (sender, e) =>
            {
                if (e.Actual == 0) { Console.WriteLine();}

                int progress = (e.Actual*100/e.Max);
                if (_currentProgress == progress) return;
                Console.Write("\r{0,10} :\t{1}%", e.TaskName, progress);
                _currentProgress = progress;
            };
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