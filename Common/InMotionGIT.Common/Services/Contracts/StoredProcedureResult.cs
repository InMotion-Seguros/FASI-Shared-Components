using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{

    [DataContract()]
    public class StoredProcedureResult
    {

        [DataMember()]
        public int RowAffected { get; set; }

        [DataMember()]
        public Dictionary<string, object> OutParameter { get; set; } = new Dictionary<string, object>();

    }

}