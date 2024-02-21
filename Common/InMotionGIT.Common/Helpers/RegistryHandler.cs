

#region Imports
using Microsoft.VisualBasic.CompilerServices;

#endregion

namespace InMotionGIT.Common.Helpers
{

    /// <summary>
    /// Class allows certified operations - Clase permite realizar operaciones sobre certificados
    /// </summary>
    /// <remarks></remarks>
    public class RegistryHandler
    {

        #region Private fields, to hold the state of the entity

        public static Microsoft.Win32.RegistryKey _regKey;

        #endregion

        #region Setting

        public static void SetSetting()
        {
            _regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Global Insurance Technology", true);
            if (_regKey == null)
            {
                _regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);
                CreateSubKey("Global Insurance Technology");

            }
            release();
            _regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Global Insurance Technology\Settings", true);
            if (_regKey == null)
            {
                _regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Global Insurance Technology", true);
                CreateSubKey("Settings");
            }
            release();
            _regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Global Insurance Technology\Settings", true);
        }

        #endregion

        #region Main Methods

        public static string GetValue(string name, string defaultValue)
        {
            SetSetting();
            return Conversions.ToString(_regKey.GetValue(name, defaultValue));
        }

        public static void SetValue(string name, string value)
        {
            SetSetting();
            _regKey.SetValue(name, value, Microsoft.Win32.RegistryValueKind.String);
        }

        public static void release()
        {
            _regKey.Close();
        }

        public static void CreateSubKey(string name)
        {
            SetSetting();
            _regKey.CreateSubKey(name);
        }

        #endregion

    }

}