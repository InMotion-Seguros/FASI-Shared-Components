
namespace InMotionGIT.Common.Attributes
{

    /// <summary>
    /// Define el titulo o nombre completo en español  asociado a la clase.
    /// </summary>
    /// <remarks></remarks>
    public sealed class EntitySpanishDesigner : EntityDesignerAttribute
    {

        public EntitySpanishDesigner(string Title) : base((int)Enumerations.EnumLanguage.Spanish, Title)
        {
        }

    }

}