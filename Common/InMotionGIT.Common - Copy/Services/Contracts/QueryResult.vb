Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class QueryResult

        <DataMember()>
        Public Property QueryCountResult As Integer

        <DataMember()>
        Public Property Table As DataTable

        <DataMember()>
        Public Property OutputParameters As New Dictionary(Of String, Object)

    End Class

End Namespace