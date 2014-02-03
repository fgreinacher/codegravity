// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using nsplit.Api.Dto;

#endregion

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        [ActionName("links")]
        public IEnumerable<LinkDto> GetAll()
        {
            return
                Registry
                    .GetAll()
                    .Select(Mapper.DynamicMap<LinkDto>);
        }

        [ActionName("edges")]
        public IEnumerable<EdgeDto> GetEdges(string id)
        {
            return
                Registry
                    .InOut(id)
                    .Select(edge => new EdgeDto
                    {
                        Kinds = edge.Kinds.ToString(),
                        Sources = Path(edge.Source),
                        Targets = Path(edge.Target)
                    });
        }

        private static IEnumerable<int> Path(int id)
        {
            return Registry
                .GetNode(id)
                .Path()
                .Select(n => n.Id);
        }
    }
}