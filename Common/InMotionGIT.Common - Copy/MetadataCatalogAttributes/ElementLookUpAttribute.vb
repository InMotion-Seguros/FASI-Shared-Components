Namespace Attributes

    ''' <summary>
    ''' Define como acceder la tabla que contiene la lista de valores hacer usado por la propiedad.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property)>
    Public NotInheritable Class ElementLookupAttribute
        Inherits Attribute

        Private _TableName As String
        Private _KeyField As String
        Private _DescriptionField As String
        Private _languageField As String

        Public ReadOnly Property TableName() As String
            Get
                Return _TableName
            End Get
        End Property

        Public ReadOnly Property KeyField() As String
            Get
                Return _KeyField
            End Get
        End Property

        Public ReadOnly Property DescriptionField() As String
            Get
                Return _DescriptionField
            End Get
        End Property

        Public ReadOnly Property LanguageField() As String
            Get
                Return _languageField
            End Get
        End Property

        Public Sub New(ByVal tableName As String, ByVal keyField As String, ByVal descriptionField As String)
            _TableName = tableName
            _KeyField = keyField
            _DescriptionField = descriptionField
        End Sub

        Public Sub New(ByVal tableName As String, ByVal keyField As String, ByVal descriptionField As String, ByVal languageField As String)
            _TableName = tableName
            _KeyField = keyField
            _DescriptionField = descriptionField
            _languageField = languageField
        End Sub

    End Class

End Namespace