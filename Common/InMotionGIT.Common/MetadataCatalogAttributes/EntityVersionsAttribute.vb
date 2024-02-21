Namespace Attributes

    <AttributeUsage(AttributeTargets.Class)>
    Public NotInheritable Class EntityVersionsAttribute
        Inherits Attribute

        Private _firstVersion As Enumerations.EnumApplicationVersion = Enumerations.EnumApplicationVersion.None
        Private _secondVersion As Enumerations.EnumApplicationVersion = Enumerations.EnumApplicationVersion.None
        Private _thirdVersion As Enumerations.EnumApplicationVersion = Enumerations.EnumApplicationVersion.None

        Public ReadOnly Property FirstVersion() As Enumerations.EnumApplicationVersion
            Get
                Return _firstVersion
            End Get
        End Property

        Public ReadOnly Property SecondVersion() As Enumerations.EnumApplicationVersion
            Get
                Return _secondVersion
            End Get
        End Property

        Public ReadOnly Property ThirdVersion() As Enumerations.EnumApplicationVersion
            Get
                Return _thirdVersion
            End Get
        End Property

        Public Sub New(ByVal FirstVersion As Enumerations.EnumApplicationVersion)
            _firstVersion = FirstVersion
        End Sub

        Public Sub New(ByVal FirstVersion As Enumerations.EnumApplicationVersion, SecondVersion As Enumerations.EnumApplicationVersion)
            _firstVersion = FirstVersion
            _secondVersion = SecondVersion
        End Sub

        Public Sub New(ByVal FirstVersion As Enumerations.EnumApplicationVersion, SecondVersion As Enumerations.EnumApplicationVersion, ThirdVersion As Enumerations.EnumApplicationVersion)
            _firstVersion = FirstVersion
            _secondVersion = SecondVersion
            _thirdVersion = ThirdVersion
        End Sub

    End Class

End Namespace