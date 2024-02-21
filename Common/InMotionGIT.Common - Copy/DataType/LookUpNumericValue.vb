Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Attributes.TypeStructure("Code")>
    Public Class LookUpNumericValue

        <DataMember()>
        <XmlAttribute()>
        Public Property Code As Integer

        <DataMember()>
        <XmlAttribute()>
        Public Property Description As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal code As Integer)
            Me.Code = code
        End Sub

        Public Sub New(ByVal code As Integer, ByVal description As String)
            Me.Code = code
            Me.Description = description
        End Sub

        Public Overrides Function ToString() As String
            Return Description
        End Function

        Public Shared Narrowing Operator CType(ByVal value As LookUpNumericValue) As Integer
            If IsNothing(value) Then
                Return 0
            Else
                Return value.Code
            End If
        End Operator

        Public Shared Widening Operator CType(ByVal code As Integer) As LookUpNumericValue
            Return New LookUpNumericValue(code)
        End Operator

        Public Shared Function LoadFromDataTable(tableInformation As DataTable)
            Dim Result As New List(Of LookUpNumericValue)
            Dim item As LookUpNumericValue
            For Each row As Data.DataRow In tableInformation.Rows
                item = New LookUpNumericValue With {.Code = row("CODE"), .Description = row("DESCRIPTION")}
                Result.Add(item)
            Next
            Return Result
        End Function

    End Class

End Namespace