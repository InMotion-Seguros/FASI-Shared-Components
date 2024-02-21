using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;
using InMotionGIT.Common.Core.Extensions;
namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the numerics data type
/// </summary>
public static class NumericExtensions
{
    #region Integer

    /// <summary>
    /// Determines whether the specified integer is null or zero.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsEmpty(this int value)
    {
        return (object)value == null || value == 0;
    }

    /// <summary>
    /// Determines whether the specified Integer is not null or empty.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsNotEmpty(this int value)
    {
        return !value.IsEmpty();
    }

    /// <summary>
    /// Checks whether the Integer is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The Integer to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the Integer or the default value.</returns>
    public static int IfEmpty(this int value, int defaultValue)
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Checks whether the Integer is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The Integer to check.</param>
    /// <param name="appSettingKey">AppSetting Key Name</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the Integer or the default value.</returns>
    public static int IfEmpty(this int value, string appSettingKey, int defaultValue)
    {
        if (value.IsEmpty())
        {
            value = appSettingKey.AppSettings<int>();
            if (value.IsEmpty())
            {
                return defaultValue;
            }
            else
            {
                return value;
            }
        }
        else
        {
            return value;
        }
    }

    #endregion Integer

    #region Long

    /// <summary>
    /// Determines whether the specified integer is null or zero.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsEmpty(this long value)
    {
        return (object)value == null || value == 0L;
    }

    /// <summary>
    /// Determines whether the specified Integer is not null or empty.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsNotEmpty(this long value)
    {
        return !value.IsEmpty();
    }

    /// <summary>
    /// Checks whether the Integer is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The Integer to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the Integer or the default value.</returns>
    public static long IfEmpty(this long value, long defaultValue)
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Checks whether the Integer is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The Integer to check.</param>
    /// <param name="appSettingKey">AppSetting Key Name</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the Integer or the default value.</returns>
    public static int IfEmpty(this long value, string appSettingKey, long defaultValue)
    {
        if (value.IsEmpty())
        {
            value =  appSettingKey.AppSettings<int>();
            if (value.IsEmpty())
            {
                return (int)defaultValue;
            }
            else
            {
                return (int)value;
            }
        }
        else
        {
            return (int)value;
        }
    }

    #endregion Long

    #region Decimal

    /// <summary>
    /// Determines whether the specified integer is null or zero.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsEmpty(this decimal value)
    {
        return (object)value == null || value == 0m;
    }

    /// <summary>
    /// Determines whether the specified Integer is not null or empty.
    /// </summary>
    /// <param name="value">The Integer value to check.</param>
    public static bool IsNotEmpty(this decimal value)
    {
        return !value.IsEmpty();
    }

    /// <summary>
    /// Checks whether the Integer is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The Integer to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the Integer or the default value.</returns>
    public static decimal IfEmpty(this decimal value, decimal defaultValue)
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return value;
        }
    }

    #endregion Decimal
}