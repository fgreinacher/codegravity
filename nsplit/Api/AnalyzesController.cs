// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.IO;
using System.Reflection;
using System.Web.Http;
using AutoMapper;
using nsplit.Ui;

#endregion

namespace nsplit.Api
{
    public class AnalyzesController : ApiController
    {
        [ActionName("start")]
        public StartResultDto GetStartResult(string file)
        {
            string message;
            Assembly assembly;
            bool isOk = 
                string.IsNullOrEmpty(file)
                    ? LoadDemoAssembly(out assembly, out message)
                    : new WebUi().TryLoadAssembly(new[] {file}, out assembly, out message);
            if (isOk) AppState.StartAnalyzes(assembly);

            return
                new StartResultDto
                {
                    IsOk = isOk,
                    Message = message
                };
        }

        private bool LoadDemoAssembly(out Assembly assembly, out string message)
        {
            assembly = typeof (Program).Assembly;
            message = assembly.GetName().Name;
            return true;
        }

        [ActionName("progress")]
        public ProgressResultDto GetProgress()
        {
            var progress = AppState.GetProgress();
            return Mapper.DynamicMap<ProgressResultDto>(progress);
        }
    }
}