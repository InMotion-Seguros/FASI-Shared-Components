using System;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.DataType
{

    [DataContract()]
    [Serializable()]
    public class ResultData : Result
    {

        [DataMember()]
        public long Count { get; set; }
        [DataMember()]
        public object Data { get; set; }

    }

}