Namespace Attributes

    ''' <summary>
    ''' Define las características de tamaño, precisión y/o escala dependiendo del tipo asociado a la propiedad.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property)>
    Public NotInheritable Class ElementBehaviorAttribute
        Inherits Attribute

        Private _Scale As Integer
        Private _Size As Integer
        Private _Precision As Integer
        Private _formControl As Enumerations.EnumFormControls

        Public ReadOnly Property Scale() As Integer
            Get
                Return _Scale
            End Get
        End Property

        Public ReadOnly Property Size() As Integer
            Get
                Return _Size
            End Get
        End Property

        Public ReadOnly Property FormControl() As Enumerations.EnumFormControls
            Get
                Return _formControl
            End Get
        End Property

        Public ReadOnly Property Precision() As Integer
            Get
                Return _Precision
            End Get
        End Property

        Public Sub New(ByVal size As Integer)
            _Size = size
            _Precision = size
        End Sub

        Public Sub New(ByVal precision As Integer, ByVal scale As Integer)
            _Scale = scale
            _Size = precision
            _Precision = precision
        End Sub

        Public Sub New(ByVal formControl As Enumerations.EnumFormControls)
            _formControl = formControl
        End Sub

    End Class

End Namespace