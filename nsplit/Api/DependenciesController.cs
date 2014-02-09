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
            return Program.Storage.Load(name);
        }

        [ActionName("names")]
        public IEnumerable<string> GetNames()
        {
            return Program.Storage.GetNames();
        }
    }
}