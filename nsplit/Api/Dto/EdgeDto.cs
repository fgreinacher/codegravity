#region usings

using System.Runtime.Serialization;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract(Name = "edge")]
    public class EdgeDto
    {
        [DataMember(Name = "source")]
        public int Source { get; set; }

        [DataMember(Name = "target")]
        public int Target { get; set; }

        [DataMember(Name = "kind")]
        public string Kinds { get; set; }
    }
}