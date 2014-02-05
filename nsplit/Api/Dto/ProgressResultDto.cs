using System.Runtime.Serialization;

namespace nsplit.Api
{
    [DataContract]
    public class ProgressResultDto
    {
        [DataMember(Name = "actual")]
        public int Actual { get; set; }

        [DataMember(Name = "max")]
        public int Max { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "finished")]
        public bool IsFinished { get; set; }
    }
}