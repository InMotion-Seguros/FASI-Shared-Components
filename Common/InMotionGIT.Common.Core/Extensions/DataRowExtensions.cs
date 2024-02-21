using System;
using System.Data;

#region using

using System.IO;
using System.Xml.Linq;
using Microsoft.VisualBasic.CompilerServices;

#endregion using

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the DataRow type
/// </summary>
public static class DataRowExtensions
{
    #region Binary (BLOB) Extension

    public static string FileContent(this DataRow value, string name, string path, string filename, string extension)
    {
        string result = string.Empty;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            string localfilename = path;
            if (filename.IsEmpty())
            {
                localfilename += @"\" + Guid.NewGuid().ToString();
            }
            else
            {
                localfilename += @"\" + filename;
            }
            if (extension.StartsWith("."))
            {
                localfilename += extension;
            }
            else
            {
                localfilename += "." + extension;
            }
            localfilename = localfilename.Replace(@"\\", @"\");

            using (var FS = new FileStream(localfilename, FileMode.Create))
            {
                byte[] blob = (byte[])value[name];
                FS.Write(blob, 0, blob.Length);
                FS.Close();
            }

            result = localfilename;
        }

        return result;
    }

    #endregion Binary (BLOB) Extension

    #region Numeric Extension

    public static decimal NumericValue(this DataRow value, string name)
    {
        decimal result = 0m;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToDecimal(value[name]);
        }

        return result;
    }

    public static int IntegerValue(this DataRow value, string name)
    {
        int result = 0;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToInteger(value[name]);
        }

        return result;
    }

    public static byte ByteValue(this DataRow value, string name)
    {
        byte result = 0;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToByte(value[name]);
        }

        return result;
    }

    public static object NumericValueWithFormatDefault(this DataRow value, string name, string format, object defaultValue, ref bool specified)
    {
        var result = defaultValue;

        if (value.IsNotNull(name))
        {
            decimal @internal = value.NumericValue(name);

            if (format.IsNotEmpty())
            {
                result = @internal.ToString(format, new System.Globalization.CultureInfo("en-US", false));
            }
            else
            {
                result = @internal;
            }

            specified = true;
        }
        else
        {
            specified = false;
        }

        return result;
    }

    #endregion Numeric Extension

    #region Double Extension

    public static double DoubleValue(this DataRow value, string name)
    {
        double result = 0d;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToDouble(value[name]);
        }

        return result;
    }

    #endregion Double Extension

    #region DateTime Extension

    public static DateTime DateTimeValue(this DataRow value, string name)
    {
        var result = DateTime.MinValue;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToDate(value[name]);
        }

        return result;
    }

    public static object DateTimeValueWithFormatDefault(this DataRow value, string name, string format, object defaultValue)
    {
        var result = defaultValue;

        if (value.IsNotNull(name))
        {
            var @internal = value.DateTimeValue(name);

            if (format.IsNotEmpty())
            {
                result = @internal.ToString(format);
            }
            else
            {
                result = @internal;
            }
        }

        return result;
    }

    #endregion DateTime Extension

    #region Boolean Extension

    public static bool BooleanValue(this DataRow value, string name)
    {
        bool result = Conversions.ToBoolean(0);

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(value[name], 1, false));
        }

        return result;
    }

    public static bool BooleanCharValue(this DataRow value, string name)
    {
        bool result = Conversions.ToBoolean(0);

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(value[name], "1", false));
        }

        return result;
    }

    #endregion Boolean Extension

    #region String Extension

    public static string StringValue(this DataRow value, string name)
    {
        string result = string.Empty;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToString(value[name]);
            result = result.Trim();
        }

        return result;
    }

    public static string StringValueWithDefault(this DataRow value, string name, object defaultValue, ref bool specified)
    {
        string result = Conversions.ToString(defaultValue);

        if (value.IsNotNull(name))
        {
            result = value.StringValue(name);
            specified = true;
        }
        else
        {
            specified = false;
        }

        return result;
    }

    public static DateTime StringHourValue(this DataRow value, string name)
    {
        var result = DateTime.MinValue;
        int currentHour = 0;
        int currentMinute = 0;
        string currentValue;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            currentValue = Conversions.ToString(value[name]);
            currentValue = currentValue.Trim();
            if (currentValue.IndexOf(":") > 0)
            {
                currentHour = Conversions.ToInteger(currentValue.Split(':')[0]);
                currentMinute = Conversions.ToInteger(currentValue.Split(':')[1]);
            }
        }

        result = result.AddHours(currentHour);
        result = result.AddMinutes(currentMinute);

        return result;
    }

    #endregion String Extension

    #region Specials Extension

    public static Guid GuidValue(this DataRow value, string name)
    {
        var result = Guid.NewGuid();

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = (Guid)value[name];
        }

        return result;
    }

    public static XDocument XmlValue(this DataRow value, string name)
    {
        var result = new XDocument();

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = XDocument.Parse(value.StringValue(name));
        }

        return result;
    }

    public static bool SwitchValue(this DataRow value, string name)
    {
        bool result = false;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(value[name], 1, false));
        }

        return result;
    }

    public static bool SwitchCharValue(this DataRow value, string name)
    {
        bool result = false;

        if (!(value[name] == null) && !(value[name] is DBNull))
        {
            result = Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(value[name], "1", false));
        }

        return result;
    }

    #endregion Specials Extension

    #region Behavior Functions

    public static bool IsNotNull(this DataRow value, string name)
    {
        return !(value[name] is DBNull);
    }

    public static T EnumValue<T>(this DataRow value, string name, T enumType, ref bool specified, ref bool witherror)
    {
        T result = default;

        if (value.IsNotNull(name))
        {
            try
            {
                result = Conversions.ToGenericParameter<T>(Enum.Parse(enumType.GetType(), Conversions.ToString(value[name])));
                specified = true;
            }
            catch (ArgumentException ex)
            {
                result = default;
                specified = false;
                witherror = true;
            }
        }
        else
        {
            result = default;
            specified = false;
        }

        return result;
    }

    #endregion Behavior Functions
}