Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class Credential

        <DataMember()>
        Public Property ConnectionStringName As String

        <DataMember()>
        Public Property User As String

        <DataMember()>
        Public Property Password As String

    End Class

End Namespace