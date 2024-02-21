using System;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{

    [DataContract()]
    public class Source
    {

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public DateTime EffectDate { get; set; }

        [DataMember]
        public int Count { get; set; }

    }

}