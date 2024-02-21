using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.Domain.DataAccess;


[DataContract()]
public class StoredProcedureResult
{

    [DataMember()]
    public int RowAffected { get; set; }

    [DataMember()]
    public Dictionary<string, object> OutParameter { get; set; } = new Dictionary<string, object>();

}