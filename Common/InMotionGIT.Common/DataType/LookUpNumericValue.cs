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
    [Attributes.TypeStructure("Code")]
    public class LookUpNumericValue
    {

        [DataMember()]
        [XmlAttribute()]
        public int Code { get; set; }

        [DataMember()]
        [XmlAttribute()]
        public string Description { get; set; }

        public LookUpNumericValue() : base()
        {
        }

        public LookUpNumericValue(int code)
        {
            Code = code;
        }

        public LookUpNumericValue(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }

        public static explicit operator int(LookUpNumericValue value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value.Code;
            }
        }

        public static implicit operator LookUpNumericValue(int code)
        {
            return new LookUpNumericValue(code);
        }

        public static object LoadFromDataTable(DataTable tableInformation)
        {
            var Result = new List<LookUpNumericValue>();
            LookUpNumericValue item;
            foreach (DataRow row in tableInformation.Rows)
            {
                item = new LookUpNumericValue() { Code = Conversions.ToInteger(row["CODE"]), Description = Conversions.ToString(row["DESCRIPTION"]) };
                Result.Add(item);
            }
            return Result;
        }

    }

}