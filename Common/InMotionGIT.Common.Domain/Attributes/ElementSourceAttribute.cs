using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Establece la procedencia de la propiedad a nivel de source
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ElementSourceAttribute : Attribute
{

    private string _source;

    public string Source
    {
        get
        {
            return _source;
        }
    }

    public ElementSourceAttribute(string source)
    {
        _source = source;
    }

}