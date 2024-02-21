Imports System.Runtime.Serialization

Namespace DataType

    <DataContract()>
    <Serializable()>
    Public Class ResultBehavior
        Inherits Result

        <DataMember(EmitDefaultValue:=False)>
        Public Property DataBehavior As Object

    End Class

End Namespace