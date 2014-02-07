// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;

#endregion

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        [ActionName("graph")]
        public GraphDto GetGraph(string name)
        {
            return
                string.IsNullOrEmpty(name)
                    ? GetCurrent()
                    : GetSaved(name);
        }

        [ActionName("names")]
        public IEnumerable<string> GetNames()
        {
            return AppState.Storage.GetNames();
        }
        
        private GraphDto GetSaved(string name)
        {
            return AppState.Storage.Load(name);
        }

        private GraphDto GetCurrent()
        {
            var graph = AppState.Task.GetGraph();
            return Mapper.DynamicMap<GraphDto>(graph);
        }
    }
}