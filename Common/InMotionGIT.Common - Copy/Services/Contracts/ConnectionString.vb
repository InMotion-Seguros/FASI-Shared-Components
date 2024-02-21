Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class ConnectionString

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property ConnectionString As String

        <DataMember()>
        Property ProviderName As String

        <DataMember()>
        Public Property UserName As String

        <DataMember()>
        Public Property Password As String

        <DataMember()>
        Public Property Owners As String

        <DataMember()>
        Public Property ServiceName As String

        <DataMember()>
        Public Property DatabaseName As String

        <DataMember()>
        Public Property SourceType As Enumerations.EnumSourceType

    End Class

End Namespace