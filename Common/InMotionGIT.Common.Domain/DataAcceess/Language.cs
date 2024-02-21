using System.Runtime.Serialization;

namespace InMotionGIT.Common.Domain.DataAccess;


[DataContract()]
public class Language : DataType.LookUpValue
{

    [DataMember()]
    public string CulturalCode { get; set; }

    [DataMember()]
    public int LanguageId { get; set; }

}