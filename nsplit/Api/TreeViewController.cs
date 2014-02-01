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
        [ActionName("deep")]
        public DeepNodeDto GetDeep()
        {
            var root = Registry.GetNode(0);
            return Mapper.DynamicMap<DeepNodeDto>(root);
        }

        [ActionName("node")]
        public NodeDto GetNode(string id)
        {
            var node = Registry.GetNode(id);
            return Mapper.DynamicMap<NodeDto>(node);
        }

        [ActionName("children")]
        public IEnumerable<NodeDto> GetChildren(string id)
        {
            INode parent = Registry.GetNode(id);
            
             return 
                parent
                    .Children()
                    .Select(Mapper.DynamicMap<NodeDto>);
        }
    }
}