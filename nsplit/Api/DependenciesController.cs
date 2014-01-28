using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using nsplit.Analyzer;
using nsplit.Api.Dto;
using nsplit.DataStructures.Graph;

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        public IEnumerable<EdgeDto> GetEdgesByNode(string node)
        {
            Type type;
            if (!Program.Types.TryGetValue(node, out type))
            {
                return Enumerable.Empty<EdgeDto>();
            }

            return 
                GetDependencies(type)
                    .Select(Mapper.DynamicMap<EdgeDto>);
        }

        private static IEnumerable<Edge> GetDependencies(Type type)
        {
            IEnumerable<Dependecy> dependecies = type.Dependecies();
            foreach (Dependecy dependecy in dependecies)
            {
                if (dependecy.To.FullName==null || !Program.Types.ContainsKey(dependecy.To.FullName)) continue;
                var source = new Vertex {Name = dependecy.From.FullName};
                var target = new Vertex {Name = dependecy.To.FullName};
                yield return new Edge {Source = source, Target = target, Kind = dependecy.DependencyKind};
            }
        }

        public IEnumerable<VertexDto> GetNodes()
        {
            return Program
                .Types
                .Values
                .Select(type => new Vertex {Name = type.FullName})
                .Select(Mapper.DynamicMap<VertexDto>);
        }
    }
}