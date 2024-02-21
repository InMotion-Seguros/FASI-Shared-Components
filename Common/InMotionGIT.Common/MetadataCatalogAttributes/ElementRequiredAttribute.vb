Namespace Attributes

    ''' <summary>
    ''' Establece si la propiedad debe llenarse de forma obligatoria.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property)>
    Public NotInheritable Class ElementRequiredAttribute
        Inherits Attribute

        Private _IsRequired As Boolean

        Public ReadOnly Property IsRequired() As Boolean
            Get
                Return _IsRequired
            End Get
        End Property

        Public Sub New(ByVal isRequired As Boolean)
            _IsRequired = isRequired
        End Sub

    End Class

End Namespace