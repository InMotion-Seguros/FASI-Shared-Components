using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Clase de extencion para appsettings
/// </summary>
public static class AppSettingsExtensions
{
    public static bool IsTrue(this string appSettingName)
    {
        bool result = false;
        if (appSettingName.AppSettings().IsNotEmpty() && appSettingName.AppSettings().ToLower().Equals("true"))
        {
            result = true;
        }

        return result;
    }

    public static bool IsFalse(this string appSettingName)
    {
        bool result = true;
        if (appSettingName.AppSettings().IsNotEmpty() && appSettingName.AppSettings().ToLower().Equals("false"))
        {
            result = true;
        }

        return result;
    }

    public static string StringValue(this string appSettingName)
    {
        return appSettingName.StringValue(string.Empty);
    }

    public static string StringValue(this string appSettingName, string defaultValue)
    {
        string result = string.Empty;
        if (appSettingName.AppSettings().IsNotEmpty())
        {
            result = appSettingName.AppSettings();
        }
        if (result.IsEmpty())
        {
            result = defaultValue;
        }

        return result;
    }

    public static T AppSettings<T>(this string key, bool throwIfNotExit = false, T valueDefault = default)
    {
        T result;
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (key.AppSettings() is not null)
        {
            result = ConfigurationHandler.AppSettings<T>(key);
        }
        else if (throwIfNotExit &&  key.AppSettings() is null)
        {
            throw new Exception("No existe el AppSetting solicitado '{0}'".SpecialFormater(key));
        }
        else
        {
            result = valueDefault;
        }
        return result;
    }

    public static string AppSettings(this string key, bool throwIfNotExit = false)
    {
        return key.AppSettings<string>(throwIfNotExit);
    }

    public static bool AppSettingsOnEquals(this string key, string valueComper, bool throwIfNotExit = false, bool IgnoreCase = true)
    {
        if (IgnoreCase)
        {
            return key.AppSettings<string>(throwIfNotExit).Equals(valueComper, StringComparison.CurrentCultureIgnoreCase);
        }
        else
        {
            return key.AppSettings<string>(throwIfNotExit).Equals(valueComper);
        }
    }
}