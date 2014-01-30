using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using nsplit.Api.Dto;

namespace nsplit.Api
{
    public class DependenciesController : ApiController
    {
        [ActionName("edges")]
        public IEnumerable<EdgeDto> GetEdges(string id)
        {
            return 
                Registry
                    .InOut(id)
                    .Select(edge=>new EdgeDto()
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
                .Select(n=>n.Id);
        }
    }
}