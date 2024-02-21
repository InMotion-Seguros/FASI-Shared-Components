Namespace Attributes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class)>
    Public MustInherit Class EntityDesignerAttribute
        Inherits Attribute

        Private _Language As Integer = 1
        Private _Title As String

        Public ReadOnly Property Language() As Integer
            Get
                Return _Language
            End Get
        End Property

        Public ReadOnly Property Title() As String
            Get
                Return _Title
            End Get
        End Property

        Protected Sub New(ByVal language As Integer, ByVal title As String)
            _Language = language
            _Title = title
        End Sub

    End Class

End Namespace