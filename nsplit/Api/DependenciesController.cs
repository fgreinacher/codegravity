using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using nsplit.Api.Dto;
using nsplit.CodeAnalyzis;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        static DependenciesController()
        {
            CrateMappings();
        }

        private static void CrateMappings()
        {
            Mapper.CreateMap<Type, VertexDto>().ConvertUsing(t => new VertexDto {Name = t.FullName});
            Mapper.CreateMap<Dependecy, EdgeDto>();
        }

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

        private static IEnumerable<Dependecy> GetDependencies(Type type)
        {
            return 
                type
                    .Dependecies()
                    .Where(d => d.Target.FullName != null) //TODO: This is workaround against IEnumerable<T> Must find a gracefulls way
                    .Where(d=>Program.Types.ContainsKey(d.Target.FullName));
        }

        public IEnumerable<VertexDto> GetNodes()
        {
            return Program
                .Types
                .Values
                .Select(Mapper.DynamicMap<VertexDto>);
        }
    }
}