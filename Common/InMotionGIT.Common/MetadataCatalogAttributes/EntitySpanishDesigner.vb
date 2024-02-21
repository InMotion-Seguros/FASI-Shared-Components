Namespace Attributes

    ''' <summary>
    ''' Define el titulo o nombre completo en español  asociado a la clase.
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class EntitySpanishDesigner
        Inherits EntityDesignerAttribute

        Public Sub New(ByVal Title As String)
            MyBase.New(Enumerations.EnumLanguage.Spanish, Title)
        End Sub

    End Class

End Namespace