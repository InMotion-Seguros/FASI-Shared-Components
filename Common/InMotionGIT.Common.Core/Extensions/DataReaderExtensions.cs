using System;

#region using

using System.Data.Common;
using Microsoft.VisualBasic.CompilerServices;

#endregion using

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the DbDataReader type
/// </summary>
public static class DataReaderExtensions
{
    #region Numeric Extension

    public static decimal NumericValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        decimal result = 0m;

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetDecimal(ordinal);
        }

        return result;
    }

    public static int IntegerValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        int result = 0;

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetInt32(ordinal);
        }

        return result;
    }

    #endregion Numeric Extension

    #region DateTime Extension

    public static DateTime DateTimeValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        var result = DateTime.MinValue;

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetDateTime(ordinal);
        }

        return result;
    }

    #endregion DateTime Extension

    #region Boolean Extension

    public static bool BooleanValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        bool result = Conversions.ToBoolean(0);

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetInt32(ordinal) == 1;
        }

        return result;
    }

    public static bool BooleanCharValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        bool result = Conversions.ToBoolean(0);

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetString(ordinal).Trim() == "1";
        }

        return result;
    }

    #endregion Boolean Extension

    #region String Extension

    public static string StringValue(this DbDataReader value, string name)
    {
        int ordinal = value.GetOrdinal(name);
        string result = string.Empty;

        if (!(value == null) && !value.IsDBNull(ordinal))
        {
            result = value.GetString(ordinal);
            result = result.Trim();
        }

        return result;
    }

    #endregion String Extension
}