using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization; 

namespace InMotionGIT.Common.Domain.DataAccess;


[DataContract()]
public class DataCommand
{

    [DataMember()]
    public string ConnectionStringName;

    [DataMember()]
    public string TableName { get; set; }

    [DataMember()]
    public string Statement { get; set; }

    [DataMember()]
    public string Operation { get; set; }

    [DataMember()]
    [XmlArray(ElementName = "Fields")]
    [XmlArrayItem(typeof(Dictionary<string, string>))]
    public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();

    [DataMember()]
    public List<DataParameter> Parameters { get; set; }

    [DataMember()]
    public string Owner { get; set; }

    [DataMember()]
    public string ObjectType { get; set; }

    [DataMember()]
    public int CompanyId { get; set; }

    [DataMember()]
    public ConnectionStrings ConnectionStringsRaw { get; set; }

    [DataMember()]
    public int MaxNumberOfRecord { get; set; }

    [DataMember()]
    public bool IgnoreMaxNumberOfRecords { get; set; }

    [DataMember()]
    public string QueryCount { get; set; }

    [DataMember()]
    public int QueryCountResult { get; set; }

    [DataMember()]
    public DataType. LookUpValue LookUp { get; set; }

}