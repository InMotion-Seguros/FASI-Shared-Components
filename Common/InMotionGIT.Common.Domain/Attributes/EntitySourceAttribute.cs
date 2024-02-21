using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Establece la procedencia del la entidad a nivel de modelo
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EntitySourceAttribute : Attribute
{

    private string _source;

    /// <summary>
    /// List of properties that define the primary key
    /// </summary>
    public string Source
    {
        get
        {
            return _source;
        }
    }

    /// <summary>
    /// Initializes a new instance of the EntityPrimaryKeyMembers class
    /// </summary>
    /// <param name="source">List of properties that define the primary key</param>
    /// <remarks>Ejemplo: &lt;EntityPrimaryKeyMembers("RecordOwner,KeyToAddressRecord,RecordEffectiveDate")&gt;</remarks>
    public EntitySourceAttribute(string source)
    {
        _source = source;
    }

    public EntitySourceAttribute() : base()
    {
    }

}