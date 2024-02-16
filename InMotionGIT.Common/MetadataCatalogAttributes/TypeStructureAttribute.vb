Namespace Attributes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Class)>
    Public NotInheritable Class TypeStructureAttribute
        Inherits Attribute

        Private _defaultMember As String

        ''' <summary>
        '''
        ''' </summary>
        Public ReadOnly Property DefaultMember() As String
            Get
                Return _defaultMember
            End Get
        End Property

        Public Sub New(ByVal defaultMember As String)
            _defaultMember = defaultMember
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace