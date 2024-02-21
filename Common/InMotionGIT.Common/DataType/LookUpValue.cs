using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.DataType
{

    [DataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
    [XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Common.Attributes.TypeStructureAttribute("Code")]
    public class LookUpValue
    {

        [DataMember()]
        [XmlAttribute()]
        public string Code { get; set; }

        [DataMember()]
        [XmlAttribute()]
        public string Description { get; set; }

        public LookUpValue() : base()
        {
        }

        public LookUpValue(string code)
        {
            Code = code;
        }

        public LookUpValue(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }

        public static explicit operator int(LookUpValue value)
        {
            if (value == null)
            {
                return Conversions.ToInteger(string.Empty);
            }
            else
            {
                return Conversions.ToInteger(value.Code);
            }
        }

        public static implicit operator LookUpValue(string code)
        {
            return new LookUpValue(code);
        }

        public static object LoadFromDataTable(DataTable tableInformation)
        {
            var Result = new List<LookUpValue>();
            LookUpValue item;
            foreach (DataRow row in tableInformation.Rows)
            {
                item = new LookUpValue() { Code = Conversions.ToString(row["CODE"]), Description = Conversions.ToString(row["DESCRIPTION"]) };
                Result.Add(item);
            }
            return Result;
        }

    }

}