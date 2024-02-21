Imports System.Xml.Serialization
Imports System.Collections.ObjectModel

Namespace Data

    ''' <summary>
    ''' 
    ''' </summary>
    <Serializable()> _
    Partial Public Class EntityCollection
        Inherits Collection(Of Entity)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Function GetItemByFullName(fullName As String) As Entity
            Dim result As Entity = Nothing

            For Each Item As Entity In Me
                If String.Equals(fullName, Item.FullName, StringComparison.CurrentCultureIgnoreCase) Then
                    result = Item
                    Exit For
                End If
            Next

            Return result
        End Function

        Public Function GetItemByClassId(classId As Integer) As Entity
            Dim result As Entity = Nothing

            For Each entityData As Entity In Me
                If entityData.ClassId = classId Then
                    result = entityData

                    Exit For
                End If
            Next

            Return result
        End Function

        Public Sub validate()
            For Each Item As Entity In Me
                Dim count As Integer = Item.elements.Count - 1
                For current As Integer = 0 To count
                    For index As Integer = current + 1 To count
                        If Not IsNothing(Item.elements(current).EnglishCaption) AndAlso
                           Not IsNothing(Item.elements(index).EnglishCaption) AndAlso
                            String.Equals(Item.elements(current).EnglishCaption, Item.elements(index).EnglishCaption, StringComparison.CurrentCultureIgnoreCase) AndAlso
                            Item.elements(current).type = Item.elements(index).type AndAlso Item.elements(index).isenum = False Then
                            Debug.WriteLine(String.Format("{0}({1}/{2})-({3}/{4}))", Item.name,
                                                                                        Item.elements(current).name, Item.elements(index).name,
                                                                                        Item.elements(current).EnglishCaption, Item.elements(index).EnglishCaption))

                        End If
                    Next
                Next
            Next
        End Sub

    End Class

End Namespace