using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using nsplit.Api.Dto;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        [ActionName("edges")]
        public IEnumerable<EdgeDto> GetEdges(string id)
        {
            int idNo = (id == "#") ? 0 : int.Parse(id);
            INode node;
            if (!Program.TypeTree.TryGet(idNo, out node))
            {
                return Enumerable.Empty<EdgeDto>();
            }

            return Program.DependencyGraph.All(node).Select(Mapper.DynamicMap<EdgeDto>);
        }
    }
}