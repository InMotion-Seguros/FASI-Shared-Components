Namespace Attributes

    ''' <summary>
    ''' Define el titulo o nombre completo en ingles asociado a la clase.
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class EntityEnglishDesigner
        Inherits EntityDesignerAttribute

        Public Sub New(ByVal Title As String)
            MyBase.New(Enumerations.EnumLanguage.English, Title)
        End Sub

    End Class

End Namespace