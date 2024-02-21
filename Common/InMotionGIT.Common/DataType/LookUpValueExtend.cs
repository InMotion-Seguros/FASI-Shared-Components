using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace InMotionGIT.Common.DataType
{

    [DataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
    [XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Attributes.TypeStructure("Code")]
    public class LookUpValueExtend : LookUpValue
    {

        [DataMember()]
        [XmlAttribute()]
        [DefaultValue("")]
        public string ShortDescription { get; set; }

        [DataMember()]
        [XmlAttribute()]
        [DefaultValue("")]
        public string ParentCode { get; set; }

        public LookUpValueExtend() : base()
        {
        }

        public LookUpValueExtend(string code)
        {
            Code = code;
        }

        public LookUpValueExtend(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public LookUpValueExtend(string code, string description, string shortDescription)
        {
            Code = code;
            Description = description;
            ShortDescription = shortDescription;
        }

        public LookUpValueExtend(string code, string description, string shortDescription, string parentCode)
        {
            Code = code;
            Description = description;
            ShortDescription = shortDescription;
            ParentCode = parentCode;
        }

    }

}