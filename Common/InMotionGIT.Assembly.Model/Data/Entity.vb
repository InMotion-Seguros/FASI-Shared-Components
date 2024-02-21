Imports System.Xml.Serialization
Imports System.ComponentModel

Namespace Data

    ''' <summary>
    ''' Class Summary
    ''' </summary>
    <Serializable()>
    <DebuggerDisplay("{FullName} Class")>
    Partial Public Class Entity

#Region "Private fields, to hold the state of the entity"

        Private _elements As ElementCollection

#End Region

#Region "Public properties, to expose the state of the entity"

        ''' <summary>
        ''' Identificacion unica para las clase
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Integer), "0")>
        Public Property classid As Integer

        ''' <summary>
        ''' Nombre y ruta del assembly , asi como de sus depedencias
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property assemblydependency() As String

        ''' <summary>
        ''' Namespace de acceso a la entidad
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property namespaceIdentify() As String

        ''' <summary>
        ''' Nombre de la entidad
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property name() As String

        ''' <summary>
        ''' Alias de la entidad
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property [Alias]() As String

        ''' <summary>
        ''' Tipo de la entidad
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property basetypefullname() As String

        ''' <summary>
        ''' Descripción de la entidad
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property summary() As String

        ''' <summary>
        ''' Indica si la entidad es una colección
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property iscollection() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property filecontent As Boolean

        ''' <summary>
        ''' Indica si la entidad debe ser excluida del catalogo
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property exclude() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property commonlyused() As Boolean

        ''' <summary>
        ''' Tipo de las instancia en que se basa la colección
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property ItemTypeFullName() As String

        ''' <summary>
        ''' Indica si se trata de una lista enumerada
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isabstract() As Boolean

        ''' <summary>
        ''' Indica si se trata de una lista enumerada
        ''' </summary>
        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isenum() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isinterface() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property havemethods() As Boolean

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property friendlyname() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property pluralfriendlyname() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property EnglishTitle() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property SpanishTitle() As String

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property Interfaces() As String

        Public Property elements() As ElementCollection
            Get
                If IsNothing(_elements) Then
                    _elements = New ElementCollection
                End If
                Return _elements
            End Get
            Set(ByVal value As ElementCollection)
                _elements = value
            End Set
        End Property

        <XmlIgnore()>
        Property Tag As Object

        <XmlIgnore()>
        Public ReadOnly Property FullName As String
            Get
                Return String.Format("{0}.{1}", namespaceIdentify, name)
            End Get
        End Property

        Public Property Entities() As List(Of Entity)

        <XmlAttribute(), DefaultValue(GetType(String), "")>
        Public Property IEnumerableType() As String

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property Custom() As Boolean

#End Region

    End Class

End Namespace