Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Attributes.TypeStructure("Code")>
    Public Class LookUpValue

        <DataMember()>
        <XmlAttribute()>
        Public Property LanguageId As Integer

        <DataMember()>
        <XmlAttribute()>
        Public Property LanguagCode As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Code As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Description As String

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

        Public Overrides Function ToString() As String
            Return Description
        End Function

        Public Shared Narrowing Operator CType(ByVal value As LookUpValue) As Integer
            If IsNothing(value) Then
                Return String.Empty
            Else
                Return value.Code
            End If
        End Operator

        Public Shared Widening Operator CType(ByVal code As String) As LookUpValue
            Return New LookUpValue(code)
        End Operator

        Public Shared Function LoadFromDataTable(tableInformation As DataTable)
            Dim Result As New List(Of LookUpValue)
            Dim item As LookUpValue
            For Each row As Data.DataRow In tableInformation.Rows
                item = New LookUpValue With {.Code = row("CODE"), .Description = row("DESCRIPTION")}
                Result.Add(item)
            Next
            Return Result
        End Function

    End Class

End Namespace