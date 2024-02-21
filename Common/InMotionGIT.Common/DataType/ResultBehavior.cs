using System;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.DataType
{

    [DataContract()]
    [Serializable()]
    public class ResultBehavior : Result
    {

        [DataMember(EmitDefaultValue = false)]
        public object DataBehavior { get; set; }

    }

}