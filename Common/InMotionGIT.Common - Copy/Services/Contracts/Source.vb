Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class Source

        <DataMember>
        Public Property Ip() As String

        <DataMember>
        Public Property EffectDate() As Date

        <DataMember>
        Public Property Count() As Integer

    End Class

End Namespace