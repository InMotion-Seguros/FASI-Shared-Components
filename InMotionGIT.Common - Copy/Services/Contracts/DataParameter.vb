Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class DataParameter

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property Type As DbType

        <DataMember()>
        Public Property Size As Integer

        <DataMember()>
        Public Property IsNull As Boolean

        <DataMember()>
        Public Property Value As Object

        <DataMember()>
        Public Property Direction As ParameterDirection

    End Class

End Namespace