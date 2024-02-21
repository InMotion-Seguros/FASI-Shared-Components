Namespace Attributes

    ''' <summary>
    ''' Establece la procedencia de la propiedad a nivel de source
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property)>
    Public NotInheritable Class ElementSourceAttribute
        Inherits Attribute

        Private _source As String

        Public ReadOnly Property Source() As String
            Get
                Return _source
            End Get
        End Property

        Public Sub New(ByVal source As String)
            _source = source
        End Sub

    End Class

End Namespace