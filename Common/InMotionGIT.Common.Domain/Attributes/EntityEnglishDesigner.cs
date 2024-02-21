
namespace InMotionGIT.Common.Domain.Attributes;

/// <summary>
/// Define el titulo o nombre completo en ingles asociado a la clase.
/// </summary>
/// <remarks></remarks>
public sealed class EntityEnglishDesigner : EntityDesignerAttribute
{

    public EntityEnglishDesigner(string Title) : base((int)Enumerations.EnumLanguage.English, Title)
    {
    }

}