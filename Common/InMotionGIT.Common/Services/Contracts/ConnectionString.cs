using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    public class ConnectionStrings
    {

        [DataMember()]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember()]
        public string ConnectionString { get; set; }

        [DataMember()]
        public string ProviderName { get; set; }

        [DataMember()]
        public string UserName { get; set; }

        [DataMember()]
        public string Password { get; set; }

        [DataMember()]
        public string Owners { get; set; }

        [DataMember()]
        public string ServiceName { get; set; }

        [DataMember()]
        public string DatabaseName { get; set; }

        [DataMember()]
        public Enumerations.EnumSourceType SourceType { get; set; }

    }

}