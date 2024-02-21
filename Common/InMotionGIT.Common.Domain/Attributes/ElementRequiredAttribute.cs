using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Establece si la propiedad debe llenarse de forma obligatoria.
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ElementRequiredAttribute : Attribute
{

    private bool _IsRequired;

    public bool IsRequired
    {
        get
        {
            return _IsRequired;
        }
    }

    public ElementRequiredAttribute(bool isRequired)
    {
        _IsRequired = isRequired;
    }

}