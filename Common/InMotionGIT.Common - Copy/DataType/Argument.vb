Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace DataType

    <DataContract(Namespace:="urn:InMotionGIT.Common.DataType")>
    <Serializable()>
    <XmlType(Namespace:="urn:InMotionGIT.Common.DataType")>
    <XmlRoot(Namespace:="urn:InMotionGIT.Common.DataType")>
    Public Class Argument

        <DataMember()>
        <XmlAttribute()>
        Public Property [Alias] As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Type As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Assemblies As String

        <DataMember()>
        <XmlAttribute()>
        Public Property Name As String

        <DataMember()>
        <XmlAttribute()>
        Public Property IsCollection As Boolean

        <DataMember()>
        <XmlAttribute()>
        Public Property FileContent As Boolean

        Public Property Arguments() As List(Of Argument)

        <XmlIgnore()>
        Public ReadOnly Property FullName As String
            Get
                Return String.Format("{0}.{1}", Type, Name)
            End Get
        End Property

        <XmlIgnore()>
        Public ReadOnly Property RealType() As Type
            Get
                Dim result As Type = Nothing

                Dim currentFullName As String = FullName

                If Type.IndexOf("/") > -1 Then
                    currentFullName = Type.Split("/")(Type.Split("/").Length - 1)
                End If

                For Each AssemblyItem As Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
                    If Not AssemblyItem.FullName.StartsWith("inrule.", StringComparison.CurrentCultureIgnoreCase) Then
                        For Each typeItem As Type In AssemblyItem.GetTypes
                            If currentFullName = typeItem.FullName Then
                                result = typeItem
                                Exit For
                            End If
                        Next

                        If Not IsNothing(result) Then
                            Exit For
                        End If
                    End If
                Next

                Return result
            End Get
        End Property

        <XmlIgnore()>
        <BrowsableAttribute(False)>
        Public ReadOnly Property FormatDBType() As String
            Get
                Dim kind As String = FullName

                If kind.Contains("System.") Then
                    kind = kind.Split(".")(1)
                End If

                Select Case kind.ToUpper
                    Case "STRING", "BOOLEAN"
                        Return "VARCHAR(1)"

                    Case "INT16", "INT32", "DECIMAL", "INT64"
                        Return "NUMBER"

                    Case "DATETIME"
                        Return "DATE"

                    Case Else
                        Return "VARCHAR(1)"
                End Select
            End Get
        End Property

    End Class

End Namespace