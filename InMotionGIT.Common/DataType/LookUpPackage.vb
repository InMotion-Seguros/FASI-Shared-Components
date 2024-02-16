Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.LookUpPackage")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.LookUpPackage")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.LookUpPackage")>
    <Attributes.TypeStructure("Code")>
    Public Class LookUpPackage

        <DataMember()>
        <XmlAttribute()>
        Public Property Key As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Items As List(Of InMotionGIT.Common.DataType.LookUpValue)

        <DataMember()>
        <XmlAttribute()>
        Public Property Count As Integer

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace