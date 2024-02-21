using System;

namespace InMotionGIT.Common.Domain.Attributes;


[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityVersionsAttribute : Attribute
{

    private Enumerations.EnumApplicationVersion _firstVersion = Enumerations.EnumApplicationVersion.None;
    private Enumerations.EnumApplicationVersion _secondVersion = Enumerations.EnumApplicationVersion.None;
    private Enumerations.EnumApplicationVersion _thirdVersion = Enumerations.EnumApplicationVersion.None;

    public Enumerations.EnumApplicationVersion FirstVersion
    {
        get
        {
            return _firstVersion;
        }
    }

    public Enumerations.EnumApplicationVersion SecondVersion
    {
        get
        {
            return _secondVersion;
        }
    }

    public Enumerations.EnumApplicationVersion ThirdVersion
    {
        get
        {
            return _thirdVersion;
        }
    }

    public EntityVersionsAttribute(Enumerations.EnumApplicationVersion FirstVersion)
    {
        _firstVersion = FirstVersion;
    }

    public EntityVersionsAttribute(Enumerations.EnumApplicationVersion FirstVersion, Enumerations.EnumApplicationVersion SecondVersion)
    {
        _firstVersion = FirstVersion;
        _secondVersion = SecondVersion;
    }

    public EntityVersionsAttribute(Enumerations.EnumApplicationVersion FirstVersion, Enumerations.EnumApplicationVersion SecondVersion, Enumerations.EnumApplicationVersion ThirdVersion)
    {
        _firstVersion = FirstVersion;
        _secondVersion = SecondVersion;
        _thirdVersion = ThirdVersion;
    }

}