Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    Public Class LocalizedString

        <DataMember()>
        <XmlAttribute()>
        Public Property Language As Integer

        <DataMember()>
        <XmlAttribute(), DefaultValue("")>
        Public Property Value As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(language As Integer, value As String)
            Me.Language = language
            Me.Value = value
        End Sub

        Public Function Clone() As LocalizedString
            Return New LocalizedString() _
                                      With {.Language = Language,
                                            .Value = Value}
        End Function

    End Class

End Namespace