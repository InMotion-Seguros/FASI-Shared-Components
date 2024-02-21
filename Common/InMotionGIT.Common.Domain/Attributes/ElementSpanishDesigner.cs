
namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Define la etiqueta y tooltips en español asociado a la propiedad.
/// </summary>
/// <remarks></remarks>
public sealed class ElementSpanishDesigner : ElementDesignerAttribute
{

    public ElementSpanishDesigner(string caption) : base((int)Enumerations.EnumLanguage.Spanish, caption)
    {
    }

    public ElementSpanishDesigner(string caption, string toolTip) : base((int)Enumerations.EnumLanguage.Spanish, caption, toolTip)
    {
    }

}