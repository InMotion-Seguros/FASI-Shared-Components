Imports System.Collections.ObjectModel
Imports System.Xml.Serialization

Namespace Base

    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.UI.Model")>
    <XmlRoot(Namespace:="urn:InMotionGIT.UI.Model")>
    Public Class ActionCollection
        Inherits Collection(Of Action)

        Public Function Clone() As ActionCollection
            Dim _actionCollection As New ActionCollection
            Dim newItem As Action

            For Each ruleItem As Action In Me
                newItem = ruleItem.Clone
                _actionCollection.Add(newItem)
            Next

            Return _actionCollection
        End Function

    End Class

End Namespace