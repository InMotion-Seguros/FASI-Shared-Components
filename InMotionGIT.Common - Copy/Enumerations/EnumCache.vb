Imports System.ComponentModel

Namespace Enumerations

    <DefaultValue(EnumCache.None)>
    Public Enum EnumCache
        None = 0
        CacheWithFullParameters = 1
        CacheWithCommand = 2
        CacheOnDemand = 3
    End Enum

End Namespace