Imports System.Runtime.Serialization

Namespace DataType

    <DataContract()>
    <Serializable()>
    Public Class ResultData
        Inherits Result

        <DataMember()> Public Property Count As Long
        <DataMember()> Public Property Data As Object

    End Class

End Namespace