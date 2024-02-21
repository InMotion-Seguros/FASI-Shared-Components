using System;
using System.ComponentModel;
using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Clase de extencion para appsettings
    /// </summary>
    public static class AppSettingsExtensions
    {

        public static bool IsTrue(this string appSettingName)
        {
            bool result = false;
            if (ConfigurationManager.AppSettings[appSettingName].IsNotEmpty() && ConfigurationManager.AppSettings[appSettingName].ToLower().Equals("true"))
            {
                result = true;
            }

            return result;
        }

        public static bool IsFalse(this string appSettingName)
        {
            bool result = true;
            if (ConfigurationManager.AppSettings[appSettingName].IsNotEmpty() && ConfigurationManager.AppSettings[appSettingName].ToLower().Equals("false"))
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
            if (ConfigurationManager.AppSettings[appSettingName].IsNotEmpty())
            {
                result = ConfigurationManager.AppSettings[appSettingName];
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
            if (ConfigurationManager.AppSettings[key] is not null)
            {
                result = Conversions.ToGenericParameter<T>(converter.ConvertFrom(ConfigurationManager.AppSettings[key]));
            }
            else if (throwIfNotExit && ConfigurationManager.AppSettings[key] is null)
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

}