using System.Runtime.Serialization;
using nsplit.Analyzer;

namespace nsplit.Api
{
    [DataContract(Name = "edge")]
    public class Edge
    {
        [DataMember(Name = "source")]
        public Node Source { get; set; }

        [DataMember(Name = "target")]
        public Node Target { get; set; }

        [DataMember(Name = "kind")]
        public DependencyKind Kind { get; set; }
    }
}