Namespace Attributes

    ''' <summary>
    ''' Define la etiqueta y tooltips en ingles asociado a la propiedad.
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class ElementEnglishDesigner
        Inherits ElementDesignerAttribute

        Public Sub New(ByVal caption As String)
            MyBase.New(Enumerations.EnumLanguage.English, caption)
        End Sub

        Public Sub New(ByVal caption As String, ByVal toolTip As String)
            MyBase.New(Enumerations.EnumLanguage.English, caption, toolTip)
        End Sub

    End Class

End Namespace