using System;

namespace InMotionGIT.Common.Domain.Attributes;


/// <summary>
/// Define las características de tamaño, precisión y/o escala dependiendo del tipo asociado a la propiedad.
/// </summary>
/// <remarks></remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ElementBehaviorAttribute : Attribute
{

    private int _Scale;
    private int _Size;
    private int _Precision;
    private Enumerations.EnumFormControls _formControl;

    public int Scale
    {
        get
        {
            return _Scale;
        }
    }

    public int Size
    {
        get
        {
            return _Size;
        }
    }

    public Enumerations.EnumFormControls FormControl
    {
        get
        {
            return _formControl;
        }
    }

    public int Precision
    {
        get
        {
            return _Precision;
        }
    }

    public ElementBehaviorAttribute(int size)
    {
        _Size = size;
        _Precision = size;
    }

    public ElementBehaviorAttribute(int precision, int scale)
    {
        _Scale = scale;
        _Size = precision;
        _Precision = precision;
    }

    public ElementBehaviorAttribute(Enumerations.EnumFormControls formControl)
    {
        _formControl = formControl;
    }

}