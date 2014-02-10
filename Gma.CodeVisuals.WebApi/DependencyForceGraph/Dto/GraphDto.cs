using System.Runtime.Serialization;

namespace Gma.CodeVisuals.WebApi.DependencyForceGraph.Dto
{
    [DataContract]
    public class GraphDto
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "tree")]
        public Tree Tree { get; set; }

        [DataMember(Name = "links")]
        public LinkDto[] Links { get; set; }
    }
}