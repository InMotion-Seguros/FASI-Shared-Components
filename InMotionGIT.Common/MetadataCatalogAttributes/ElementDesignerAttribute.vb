Namespace Attributes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property)>
    Public MustInherit Class ElementDesignerAttribute
        Inherits Attribute

        Private _Language As Integer = 1
        Private _Caption As String
        Private _ToolTip As String

        Public ReadOnly Property Language() As Integer
            Get
                Return _Language
            End Get
        End Property

        Public ReadOnly Property Caption() As String
            Get
                Return _Caption
            End Get
        End Property

        Public ReadOnly Property ToolTip() As String
            Get
                Return _ToolTip
            End Get
        End Property

        Protected Sub New(ByVal language As Integer, ByVal caption As String)
            _Language = language
            _Caption = caption
        End Sub

        Protected Sub New(ByVal language As Integer, ByVal caption As String, ByVal toolTip As String)
            _Language = language
            _Caption = caption
            _ToolTip = toolTip
        End Sub

    End Class

End Namespace