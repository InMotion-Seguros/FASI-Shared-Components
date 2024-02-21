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
    public class LookUpValueCollection : Collection<LookUpValue>
    {

        public LookUpValue AddLookUpValue(int code, string description)
        {
            var item = new LookUpValue(code.ToString(), description);
            Add(item);
            return item;
        }

        public void LoadFromDataTable(DataTable tableInformation)
        {
            LookUpValue item;
            foreach (DataRow row in tableInformation.Rows)
            {
                item = new LookUpValue() { Code = Conversions.ToString(row["Code"]), Description = Conversions.ToString(row["Description"]) };
                Add(item);
            }
        }

        public LookUpValue GetItemByCode(int code)
        {
            LookUpValue result = null;
            foreach (LookUpValue Item in this)
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