Imports System.Collections.ObjectModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <CollectionDataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    Public Class LookUpValueExtendCollection
        Inherits Collection(Of LookUpValueExtend)

        Public Function AddLookUpValue(ByVal code As Integer, ByVal description As String) As LookUpValueExtend
            Dim item As New LookUpValueExtend(code, description)
            Me.Add(item)
            Return item
        End Function

        Public Function AddLookUpValue(ByVal code As Integer, ByVal description As String, shortDescription As String) As LookUpValueExtend
            Dim item As New LookUpValueExtend(code, description, shortDescription)
            Me.Add(item)
            Return item
        End Function

        Public Sub LoadFromDataTable(tableInformation As DataTable)
            Dim item As LookUpValueExtend
            For Each row As Data.DataRow In tableInformation.Rows
                item = New LookUpValueExtend With {.Code = row("Code"), .Description = row("Description"), .ShortDescription = row("ShortDescription")}
                Me.Add(item)
            Next
        End Sub

        Public Function GetItemByCode(code As Integer) As LookUpValueExtend
            Dim result As LookUpValueExtend = Nothing
            For Each Item As LookUpValueExtend In Me
                If Item.Code = code Then
                    result = Item
                    Exit For
                End If
            Next
            Return result
        End Function

    End Class

End Namespace