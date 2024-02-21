using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{

    [DataContract()]
    public class QueryResult
    {

        [DataMember()]
        public int QueryCountResult { get; set; }

        [DataMember()]
        public DataTable Table { get; set; }

        [DataMember()]
        public Dictionary<string, object> OutputParameters { get; set; } = new Dictionary<string, object>();

    }

}