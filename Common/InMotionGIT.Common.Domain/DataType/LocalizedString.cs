using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Domain.DataType;


[DataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
[Serializable()]
[XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
[XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
public class LocalizedString
{

    [DataMember()]
    [XmlAttribute()]
    public int Language { get; set; }

    [DataMember()]
    [XmlAttribute()]
    [DefaultValue("")]
    public string Value { get; set; }

    public LocalizedString() : base()
    {
    }

    public LocalizedString(int language, string value)
    {
        Language = language;
        Value = value;
    }

    public LocalizedString Clone()
    {
        return new LocalizedString()
        {
            Language = Language,
            Value = Value
        };
    }

}