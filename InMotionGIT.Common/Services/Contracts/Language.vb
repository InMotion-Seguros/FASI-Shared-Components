Imports System.Runtime.Serialization

Namespace Services.Contracts

    <DataContract()>
    Public Class Language
        Inherits DataType.LookUpValue

        <DataMember()>
        Public Property CulturalCode As String

        <DataMember()>
        Public Property LanguageId As Integer

    End Class

End Namespace