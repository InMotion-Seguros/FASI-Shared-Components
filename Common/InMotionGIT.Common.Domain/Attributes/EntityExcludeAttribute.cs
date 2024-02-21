using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// 
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityExcludeAttribute : Attribute
{

    public EntityExcludeAttribute() : base()
    {
    }

}