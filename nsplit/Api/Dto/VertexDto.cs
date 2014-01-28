#region usings

using System.Runtime.Serialization;

#endregion

namespace nsplit.Api.Dto
{
    [DataContract(Name = "node")]
    public class VertexDto
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}