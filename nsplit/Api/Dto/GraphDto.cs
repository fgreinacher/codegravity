using System.Runtime.Serialization;
using nsplit.Api.Dto;

namespace nsplit.Api
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