using System;
using System.Data.Common;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Extension methods for the DbDataReader type
    /// </summary>
    public static class DbDataReaderExtensions
    {

        public static string StringValue(this DbDataReader value, int ordinal)
        {
            string result = string.Empty;
            if (!value.IsDBNull(ordinal))
            {
                result = value.GetString(ordinal).Trim();
            }
            return result;
        }

        public static DateTime DateTimeValue(this DbDataReader value, int ordinal)
        {
            var result = DateTime.MinValue;
            if (!value.IsDBNull(ordinal))
            {
                result = value.GetDateTime(ordinal);
            }
            return result;
        }

        public static decimal NumericValue(this DbDataReader value, int ordinal)
        {
            decimal result = 0m;
            if (!value.IsDBNull(ordinal))
            {
                result = value.GetDecimal(ordinal);
            }
            return result;
        }

        public static bool BooleanValue(this DbDataReader value, int ordinal)
        {
            bool result = Conversions.ToBoolean(0);
            if (!value.IsDBNull(ordinal))
            {
                result = value.GetDecimal(ordinal) == 1m;
            }
            return result;
        }

    }

}