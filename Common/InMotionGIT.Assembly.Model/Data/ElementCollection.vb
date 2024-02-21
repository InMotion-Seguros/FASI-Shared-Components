Imports System.Xml.Serialization
Imports System.Collections.ObjectModel

Namespace Data

    ''' <summary>
    ''' 
    ''' </summary>
    <Serializable()> _
    Partial Public Class ElementCollection
        Inherits Collection(Of element)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Function AddElement() As element
            Dim item As New element
            Me.Add(item)
            Return item
        End Function

        Public Function GetItemByName(name As String, type As Enumerations.EnumMemberType) As Element
            Dim result As Element = Nothing
            Dim isenum As Boolean = (type = Enumerations.EnumMemberType.Enumeration)

            For Each Item As Element In Me
                If String.Equals(name, Item.name, StringComparison.CurrentCultureIgnoreCase) AndAlso type = Item.type AndAlso Item.isenum = isenum Then
                    result = Item
                    Exit For
                End If
            Next

            Return result
        End Function

        Public Function GetItemByElementId(elementId As Integer) As Element
            Dim result As Element = Nothing

            For Each elementData As Element In Me
                If elementData.MemberId = elementId Then
                    result = elementData

                    Exit For
                End If
            Next

            Return result
        End Function

    End Class

End Namespace