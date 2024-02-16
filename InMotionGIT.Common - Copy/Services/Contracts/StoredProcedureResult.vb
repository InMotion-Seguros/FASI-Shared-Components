Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class StoredProcedureResult

        <DataMember()>
        Public Property RowAffected As Integer

        <DataMember()>
        Public Property OutParameter As New Dictionary(Of String, Object)

    End Class

End Namespace