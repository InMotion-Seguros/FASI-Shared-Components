Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Base

    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.UI.Model")>
    <XmlRoot(Namespace:="urn:InMotionGIT.UI.Model")>
    Public MustInherit Class Action

#Region "Public Properties"

        <XmlAttribute()>
        <BrowsableAttribute(False)>
        Public Property ControlName As String

#End Region

        Public MustOverride Function Description() As String

#Region "Functions"

        Public MustOverride Function Clone() As Action

#End Region

    End Class

End Namespace