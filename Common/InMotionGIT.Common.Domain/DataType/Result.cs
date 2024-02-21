using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Domain.DataType;


[DataContract()]
[Serializable()]
public class Result
{

    [DataMember(EmitDefaultValue = false)]
    [XmlAttribute()]
    [DefaultValue(typeof(bool), "False")]
    public bool Success { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlAttribute()]
    [DefaultValue(typeof(int), "0")]
    public int Code { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlAttribute()]
    [DefaultValue(typeof(string), "")]
    public string Reason { get; set; }

}