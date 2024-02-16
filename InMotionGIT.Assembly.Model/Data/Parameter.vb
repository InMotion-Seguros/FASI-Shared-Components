Imports System.Xml.Serialization

Namespace Data

    <Serializable()> _
    Public Class Parameter

        <XmlAttribute()> _
        Public Property IsOptional As Boolean

        <XmlAttribute()> _
        Public Property Name As String

        <XmlAttribute()> _
        Public Property TypeFullname As String

        <XmlAttribute()> _
        Public Property [Default] As String

        <XmlAttribute()>
        Public Property TypePassVariable As String

    End Class

End Namespace