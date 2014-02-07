// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using AutoMapper;
using nsplit.Helper;

#endregion

namespace nsplit.Api
{
    public class AnalyzesController : ApiController
    {
        [ActionName("start")]
        public StartResultDto GetStartResult(string file, bool save)
        {
            string message;
            Assembly assembly;
            Type[] types;
            bool isOk =
                string.IsNullOrEmpty(file)
                    ? LoadDemoAssembly(out assembly, out message)
                    : AssemblyLoadHelper.TryLoad(file, out assembly, out message);
            if (isOk)
            {
                AppState.Task.Cancel();
                AppState.Task = AnalyzerTask.Create(assembly);
                if (save) AppState.Task.OnFinished(task =>
                {
                    var name = assembly.GetName().Name;
                    var result = task.GetGraph();
                    var graph = Mapper.DynamicMap<GraphDto>(result);
                    AppState.Storage.Save(name, graph);
                });
            }

            return
                new StartResultDto
                {
                    IsOk = isOk,
                    Message = message
                };
        }

        [ActionName("progress")]
        public ProgressResultDto GetProgress()
        {
            var progress = AppState.Task.GetProgress();
            return Mapper.DynamicMap<ProgressResultDto>(progress);
        }

        private bool LoadDemoAssembly(out Assembly assembly, out string message)
        {
            assembly = typeof(Program).Assembly;
            message = assembly.GetName().Name;
            return true;
        }
    }
}