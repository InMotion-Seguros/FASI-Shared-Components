using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.DataType
{

    [CollectionDataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
    [XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
    public class LookUpValueExtendCollection : Collection<LookUpValueExtend>
    {

        public LookUpValueExtend AddLookUpValue(int code, string description)
        {
            var item = new LookUpValueExtend(code.ToString(), description);
            Add(item);
            return item;
        }

        public LookUpValueExtend AddLookUpValue(int code, string description, string shortDescription)
        {
            var item = new LookUpValueExtend(code.ToString(), description, shortDescription);
            Add(item);
            return item;
        }

        public void LoadFromDataTable(DataTable tableInformation)
        {
            LookUpValueExtend item;
            foreach (DataRow row in tableInformation.Rows)
            {
                item = new LookUpValueExtend() { Code = Conversions.ToString(row["Code"]), Description = Conversions.ToString(row["Description"]), ShortDescription = Conversions.ToString(row["ShortDescription"]) };
                Add(item);
            }
        }

        public LookUpValueExtend GetItemByCode(int code)
        {
            LookUpValueExtend result = null;
            foreach (LookUpValueExtend Item in this)
            {
                if (Conversions.ToDouble(Item.Code) == code)
                {
                    result = Item;
                    break;
                }
            }
            return result;
        }

    }

}