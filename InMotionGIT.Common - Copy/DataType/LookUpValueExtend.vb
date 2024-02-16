Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Attributes.TypeStructure("Code")>
    Public Class LookUpValueExtend
        Inherits LookUpValue

        <DataMember()>
        <XmlAttribute(), DefaultValue("")>
        Public Property ShortDescription As String

        <DataMember()>
        <XmlAttribute(), DefaultValue("")>
        Public Property ParentCode As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal code As String)
            Me.Code = code
        End Sub

        Public Sub New(ByVal code As String, ByVal description As String)
            Me.Code = code
            Me.Description = description
        End Sub

        Public Sub New(ByVal code As String, ByVal description As String, shortDescription As String)
            Me.Code = code
            Me.Description = description
            Me.ShortDescription = shortDescription
        End Sub

        Public Sub New(ByVal code As String, ByVal description As String, shortDescription As String, parentCode As String)
            Me.Code = code
            Me.Description = description
            Me.ShortDescription = shortDescription
            Me.ParentCode = parentCode
        End Sub

    End Class

End Namespace