
namespace InMotionGIT.Common.Attributes
{

    /// <summary>
    /// Define la etiqueta y tooltips en ingles asociado a la propiedad.
    /// </summary>
    /// <remarks></remarks>
    public sealed class ElementEnglishDesigner : ElementDesignerAttribute
    {

        public ElementEnglishDesigner(string caption) : base((int)Enumerations.EnumLanguage.English, caption)
        {
        }

        public ElementEnglishDesigner(string caption, string toolTip) : base((int)Enumerations.EnumLanguage.English, caption, toolTip)
        {
        }

    }

}