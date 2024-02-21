Imports System.Xml.Serialization
Imports System.ComponentModel

Namespace Data

    ''' <summary>
    ''' Class Summary
    ''' </summary>
    <Serializable()>
    <DebuggerDisplay("{FullName} EntityHeirarchy")>
    Partial Public Class EntityHierarchy

#Region "Field"
        Private _parentCore As EntityHierarchy
        Private _childrenCore As New ArrayList()
        Private _cellsCore() As Object

        Private _ParentNode As EntityHierarchy

        Sub New()
            ' TODO: Complete member initialization
        End Sub

        Public Property ParentNode() As EntityHierarchy
            Get
                Return _ParentNode
            End Get
            Set(ByVal value As EntityHierarchy)
                _ParentNode = value
            End Set
        End Property

        Private _TypeElement As Enumerations.EnumTypeElement
        Public Property TypeElement() As Enumerations.EnumTypeElement
            Get
                Return _TypeElement
            End Get
            Set(ByVal value As Enumerations.EnumTypeElement)
                _TypeElement = value
            End Set
        End Property

        Private _RootNode As EntityHierarchy
        Public Property RootNode() As EntityHierarchy
            Get
                Return _RootNode
            End Get
            Set(ByVal value As EntityHierarchy)
                _RootNode = value
            End Set
        End Property

        Public Property ParentCore() As EntityHierarchy
            Get
                Return _parentCore
            End Get
            Set(ByVal value As EntityHierarchy)
                _parentCore = value
                If Not (Me._parentCore Is Nothing) Then
                    Me._parentCore._childrenCore.Add(Me)
                End If
            End Set
        End Property

        Public Property ChildrenCore() As ArrayList
            Get
                Return _childrenCore
            End Get
            Set(ByVal value As ArrayList)
                Me._childrenCore = value
            End Set
        End Property

        Public Property CellsCore() As Object
            Get
                Return _cellsCore
            End Get
            Set(ByVal value As Object)
                _cellsCore = value
            End Set
        End Property

#End Region

#Region "Private fields, to hold the state of the entity"

        Private _elements As ElementCollection

#End Region

#Region "Public properties, to expose the state of the entity"

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

        ''' <summary>
        ''' Indica si la entidad es de uso comun
        ''' </summary>
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
        Public Property isenum() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isinterface() As Boolean

        <XmlAttribute(), DefaultValue(GetType(Boolean), "False")>
        Public Property isclass() As Boolean

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

#End Region

        Public Sub New(ByVal parent As EntityHierarchy, ByVal cells() As Object)
            ' Specifies the parent node for the new node.
            Me._parentCore = parent
            ' Provides data for the node's cell.
            Me._cellsCore = cells
            If Not (Me._parentCore Is Nothing) Then
                Me._parentCore._childrenCore.Add(Me)
            End If
        End Sub

    End Class

    'Public Class EntityHeirachyFound
    '    Inherits TreeListOperation

    '    Private foundNode As List(Of TreeListNode)

    '    Private column As Columns.TreeListColumn

    '    Private substr As String

    '    Public Sub New(ByVal column As Columns.TreeListColumn, ByVal substr As String)

    '        Me.foundNode = Nothing

    '        Me.column = column

    '        Me.substr = substr

    '    End Sub

    '    Public Overrides Sub Execute(ByVal nodeSelected As TreeListNode)
            
    '        Dim result As String = ""
    '        While nodeSelected IsNot Nothing
    '            Dim s As String = nodeSelected(column).ToString()
    '            If s.StartsWith(substr) Then
    '                Me.Node.Add(nodeSelected)

    '            End If
    '            nodeSelected = nodeSelected.ParentNode
    '        End While
    '    End Sub

    '    Public Overrides Function NeedsVisitChildren(ByVal node As TreeListNode) As Boolean

    '        Return foundNode Is Nothing

    '    End Function

    '    Public ReadOnly Property Node As List(Of TreeListNode)

    '        Get
    '            If IsNothing(foundNode) Then
    '                foundNode = New List(Of TreeListNode)
    '            End If
    '            Return foundNode

    '        End Get

    '    End Property

    'End Class

End Namespace