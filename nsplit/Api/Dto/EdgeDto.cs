#region usings

using System.Runtime.Serialization;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract(Name = "edge")]
    public class EdgeDto
    {
        [DataMember(Name = "source")]
        public VertexDto Source { get; set; }

        [DataMember(Name = "target")]
        public VertexDto Target { get; set; }

        [DataMember(Name = "kind")]
        public string Kind { get; set; }
    }
}