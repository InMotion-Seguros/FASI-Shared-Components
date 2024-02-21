 
using System.Diagnostics; 
using System.Runtime.Serialization; 

namespace InMotionGIT.Common.Domain.General;


[DebuggerDisplay("{PathFullName}")]
[DataContract()]
public class info
{
    
    public info()
    {

    }

    [DataMember()]
    public string Name { get; set; }

    [DataMember()]
    public DateTime LastWrite { get; set; }

    [DataMember()]
    public long Length { get; set; }

    [DataMember()]
    public bool IsFolder { get; set; }

    [DataMember()]
    public string CheckSum { get; set; }

    [DataMember()]
    public string PathFullName { get; set; }

    [DataMember()]
    public List<info> Childs { get; set; }
   

}