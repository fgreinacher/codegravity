using System.Runtime.Serialization;

namespace nsplit.Api
{
    [DataContract]
    public class StartResultDto
    {
        [DataMember(Name = "ok")]
        public bool IsOk { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}