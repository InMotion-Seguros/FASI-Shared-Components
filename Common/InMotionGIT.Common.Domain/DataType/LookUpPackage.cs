using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Domain.DataType;


[DataContract(Namespace = "urn:InMotionGIT.Common.LookUpPackage")]
[Serializable()]
[XmlType(Namespace = "urn:InMotionGIT.Common.LookUpPackage")]
[XmlRoot(Namespace = "urn:InMotionGIT.Common.LookUpPackage")]
[Attributes.TypeStructure("Code")]
public class LookUpPackage
{

    [DataMember()]
    [XmlAttribute()]
    public string Key { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public List<LookUpValue> Items { get; set; }

    [DataMember()]
    [XmlAttribute()]
    public int Count { get; set; }

    public LookUpPackage() : base()
    {
    }

}