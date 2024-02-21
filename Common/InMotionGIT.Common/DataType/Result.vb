Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract()>
    <Serializable()>
    Public Class Result

        <DataMember(EmitDefaultValue:=False)>
        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property Success As Boolean

        <DataMember(EmitDefaultValue:=False)>
        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property Code As Integer

        <DataMember(EmitDefaultValue:=False)>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property Reason As String

    End Class

End Namespace