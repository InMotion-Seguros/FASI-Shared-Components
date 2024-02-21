using System.ComponentModel;

namespace InMotionGIT.Common.Domain.Enumerations;


[DefaultValue(None)]
public enum EnumCache
{
    None = 0,
    CacheWithFullParameters = 1,
    CacheWithCommand = 2,
    CacheOnDemand = 3
}