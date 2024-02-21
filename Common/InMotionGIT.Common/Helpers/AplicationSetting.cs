using System;
using System.Configuration;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Helpers
{

    [Obsolete("Not used any more, se migro a las extencions AppSettingsExtensions", true)]
    public sealed class ApplicationSetting
    {

        public static bool IsTrue(string appSettingName)
        {
            bool result = false;
            if (ConfigurationManager.AppSettings[appSettingName].IsNotEmpty() && ConfigurationManager.AppSettings[appSettingName].ToLower().Equals("true"))
            {
                result = true;
            }

            return result;
        }

        public static bool IsFalse(string appSettingName)
        {
            bool result = true;
            if (ConfigurationManager.AppSettings[appSettingName].IsNotEmpty() && ConfigurationManager.AppSettings[appSettingName].ToLower().Equals("false"))
            {
                result = true;
            }

            return result;
        }

        public static string StringValue(string appSettingName)
        {
            return StringValue(appSettingName, string.Empty);
        }

        public static string StringValue(string appSettingName, string defaultValue)
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

    }

}