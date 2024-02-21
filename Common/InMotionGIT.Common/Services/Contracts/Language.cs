using System.Runtime.Serialization;

namespace InMotionGIT.Common.Services.Contracts
{

    [DataContract()]
    public class Language : DataType.LookUpValue
    {

        [DataMember()]
        public string CulturalCode { get; set; }

        [DataMember()]
        public int LanguageId { get; set; }

    }

}