using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{

    [DataContract()]
    public class Credential
    {

        [DataMember()]
        public string ConnectionStringName { get; set; }

        [DataMember()]
        public string User { get; set; }

        [DataMember()]
        public string Password { get; set; }

    }

}