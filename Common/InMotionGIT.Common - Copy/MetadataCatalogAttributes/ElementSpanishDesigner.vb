Namespace Attributes

    ''' <summary>
    ''' Define la etiqueta y tooltips en español asociado a la propiedad.
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class ElementSpanishDesigner
        Inherits ElementDesignerAttribute

        Public Sub New(ByVal caption As String)
            MyBase.New(Enumerations.EnumLanguage.Spanish, caption)
        End Sub

        Public Sub New(ByVal caption As String, ByVal toolTip As String)
            MyBase.New(Enumerations.EnumLanguage.Spanish, caption, toolTip)
        End Sub

    End Class

End Namespace