using System.Data;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.Domain.DataAccess;


[DataContract()]
public class DataParameter
{

    [DataMember()]
    public string Name { get; set; }

    [DataMember()]
    public DbType Type { get; set; }

    [DataMember()]
    public int Size { get; set; }

    [DataMember()]
    public bool IsNull { get; set; }

    [DataMember()]
    public object Value { get; set; }

    [DataMember()]
    public ParameterDirection Direction { get; set; }

}