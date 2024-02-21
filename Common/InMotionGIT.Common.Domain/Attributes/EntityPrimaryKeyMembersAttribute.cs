using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Allows you to indicate the properties of the class to be used as primary key to be used to persist information in a table.
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityPrimaryKeyMembersAttribute : Attribute
{

    private string _primaryKeyMembers;

    /// <summary>
    /// List of properties that define the primary key
    /// </summary>
    public string PrimaryKeyMembers
    {
        get
        {
            return _primaryKeyMembers;
        }
    }

    /// <summary>
    /// Initializes a new instance of the EntityPrimaryKeyMembers class
    /// </summary>
    /// <param name="primaryKeyMembers">List of properties that define the primary key</param>
    /// <remarks>Ejemplo: &lt;EntityPrimaryKeyMembers("RecordOwner,KeyToAddressRecord,RecordEffectiveDate")&gt;</remarks>
    public EntityPrimaryKeyMembersAttribute(string primaryKeyMembers)
    {
        _primaryKeyMembers = primaryKeyMembers;
    }

    public EntityPrimaryKeyMembersAttribute() : base()
    {
    }

}