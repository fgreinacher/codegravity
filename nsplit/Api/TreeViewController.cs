using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using nsplit.Api.Dto;
using nsplit.CodeAnalyzis.DataStructures.TypeTree;

namespace nsplit.Api
{
    public class TreeViewController : ApiController
    {
        [ActionName("node")]
        public NodeDto GetNode(string id)
        {
            int idNo = (id == "#") ? 0 : int.Parse(id);

            INode node;
            bool isOk = GetTree().TryGet(idNo, out node);
            if (!isOk) return null;
            return Mapper.DynamicMap<NodeDto>(node);
        }

         [ActionName("children")]
        public IEnumerable<NodeDto> GetChildren(string id)
        {
            int idNo = (id == "#") ? 0 : int.Parse(id);

            INode parent;
            bool isOk = GetTree().TryGet(idNo, out parent);
            if (!isOk) return Enumerable.Empty<NodeDto>();
            return 
                parent
                    .Children()
                    .Select(Mapper.DynamicMap<NodeDto>);
        }

        private Tree GetTree()
        {
            return Program.TypeTree;
        }
    }
}