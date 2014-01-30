#region usings

using System.Collections.Generic;
using System.Runtime.Serialization;
using nsplit.CodeAnalyzis.DataStructures.DependencyGraph;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract(Name = "edge")]
    public class EdgeDto
    {
        [DataMember(Name = "sources")]
        public IEnumerable<int> Sources { get; set; }

        [DataMember(Name = "targets")]
        public IEnumerable<int> Targets { get; set; }

        [DataMember(Name = "kind")]
        public string Kinds { get; set; }
    }
}