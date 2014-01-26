using System.Runtime.Serialization;

namespace nsplit.Api
{
    [DataContract(Name = "node")]
    public class Node
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}