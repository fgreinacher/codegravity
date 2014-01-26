using System.Runtime.Serialization;

namespace nsplit.Api
{
    [DataContract(Name = "edge")]
    public class Edge
    {
        [DataMember(Name = "source")]
        public Node Source { get; set; }

        [DataMember(Name = "target")]
        public Node Target { get; set; }
    }
}