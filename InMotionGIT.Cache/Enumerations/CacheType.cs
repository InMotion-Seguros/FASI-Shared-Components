
using System.ComponentModel;

namespace Enumerations
{

    [DefaultValue(EnumCache.Memory)]
    public enum EnumCache
    {
        Memory = 0,
        Redis = 1
    }
}