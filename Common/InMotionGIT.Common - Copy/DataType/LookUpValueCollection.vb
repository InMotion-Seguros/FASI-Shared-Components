Imports System.Collections.ObjectModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <CollectionDataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    Public Class LookUpValueCollection
        Inherits Collection(Of LookUpValue)

        Public Function AddLookUpValue(ByVal code As Integer, ByVal description As String) As LookUpValue
            Dim item As New LookUpValue(code, description)
            Me.Add(item)
            Return item
        End Function

        Public Sub LoadFromDataTable(tableInformation As DataTable)
            Dim item As LookUpValue
            For Each row As Data.DataRow In tableInformation.Rows
                item = New LookUpValue With {.Code = row("Code"), .Description = row("Description")}
                Me.Add(item)
            Next
        End Sub

        Public Function GetItemByCode(code As Integer) As LookUpValue
            Dim result As LookUpValue = Nothing
            For Each Item As LookUpValue In Me
                If Item.Code = code Then
                    result = Item
                    Exit For
                End If
            Next
            Return result
        End Function

    End Class

End Namespace