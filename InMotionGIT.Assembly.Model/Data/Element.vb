Imports System.Xml.Serialization
Imports System.ComponentModel

Namespace Data

    ''' <summary>
    ''' 
    ''' </summary>
    <Serializable()>
    <DebuggerDisplay("{name} {type}")>
    Partial Public Class Element

#Region "Public properties, to expose the state of the entity"

        ''' <summary>
        ''' Identificacion unica para las miembros
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property memberid As Integer

        <XmlAttribute()>
        Public Property type() As Enumerations.EnumMemberType

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property name() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property basetypefullname() As String

        <XmlAttribute(), DefaultValue(GetType(Boolean), "True")>
        Public Property canread() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "True")>
        Public Property canwrite() As Boolean

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property summary() As String

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property iscollection() As Boolean

        ''' <summary>
        ''' Tipo de las instancia en que se basa la colección
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property ItemTypeFullName() As String

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isenum() As Boolean

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property parameterDeclare() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property friendlyname() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property pluralfriendlyname() As String

        Public Property Parameters As ParameterCollection

        <XmlIgnore()>
        Public Property Tag As Object

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property EnglishCaption() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property EnglishToolTip() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property SpanishCaption() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property SpanishToolTip() As String

        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property Scale() As Integer

        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property Size() As Integer

        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property Precision() As Integer

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property IsRequired() As Boolean

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property LookUpTableName() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property LookUpKeyField() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property LookUpDescriptionField() As String

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property IsShared As Boolean

        Public Sub New()
            canread = True
            canwrite = True
        End Sub

#End Region

        Public ReadOnly Property isComplexType As Boolean
            Get
                Return ((Not basetypefullname.ToLower.StartsWith("system.") AndAlso
                         Not String.Equals(basetypefullname, "System.Runtime.Serialization.ExtensionDataObject", StringComparison.CurrentCultureIgnoreCase) AndAlso
                         Not isenum) Or iscollection)
            End Get
        End Property

        Public ReadOnly Property isClass As Boolean
            Get
                Return (Not basetypefullname.ToLower.StartsWith("system.") OrElse
                        String.Equals(basetypefullname, "System.Runtime.Serialization.ExtensionDataObject", StringComparison.CurrentCultureIgnoreCase)) AndAlso Not isenum
            End Get
        End Property

    End Class

End Namespace