using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TypeStructureAttribute : Attribute
{

    private string _defaultMember;

    /// <summary>
    /// 
    /// </summary>
    public string DefaultMember
    {
        get
        {
            return _defaultMember;
        }
    }

    public TypeStructureAttribute(string defaultMember)
    {
        _defaultMember = defaultMember;
    }

    public TypeStructureAttribute() : base()
    {
    }

}