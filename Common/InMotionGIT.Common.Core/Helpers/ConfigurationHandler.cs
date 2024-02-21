using InMotionGIT.Common.Core.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace InMotionGIT.Common.Core;

public static class ConfigurationHandler
{
    public static string fileConfiguration = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    public static T AppSettings<T>(this string key, T valueDefault = default)
    {
        T result = default;

        JObject rw = JObject.Parse(System.IO.File.ReadAllText(fileConfiguration));
        if (rw.ContainsKey("AppSettings") && ((JObject)rw["AppSettings"]).ContainsKey(key))
        {
            result = rw["AppSettings"][key].Value<T>();
        }
        else
        {
            if (!EqualityComparer<T>.Default.Equals(result, valueDefault))
            {
                result = valueDefault;
            }
            else
                throw new Exception("No existe el AppSetting solicitado '{0}'".SpecialFormater(key));
        }

        return result;
    }

    public static bool AppSettingsExist(this string key)
    {
        bool result = false;
        JObject rw = JObject.Parse(System.IO.File.ReadAllText(fileConfiguration));
        if (rw.ContainsKey("AppSettings") && ((JObject)rw["AppSettings"]).ContainsKey(key))
        {
            result = true;
        }
        return result;
    }

    public static string AppSettings(this string key)
    {
        return AppSettings<string>(key);
    }

    public static bool BoolValue(string settingName, bool defaultValue = false)
    {
        string current = settingName.AppSettings();
        if (current.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return (current.ToLower() == "true" ||
                    current.ToLower() == "verdadero" ||
                    current.ToLower() == "yes" ||
                    current.ToLower() == "si");
        }
    }

    //public static bool AppSettingsCheck(this string key)
    //{
    //    bool result = false;
    //    if (AppSettings<bool>(key).IsNotEmpty() && AppSettings(key).Equals("True", StringComparison.CurrentCultureIgnoreCase))
    //    {
    //        result = true;
    //    }
    //    return result;
    //}

    public static InMotionGIT.Common.Domain.Configuration.ConnectionStrings ConnectionStrings(string connectionStringName)
    {
        InMotionGIT.Common.Domain.Configuration.ConnectionStrings result = null;
        InMotionGIT.Common.Domain.Configuration.FASIConfiguration databaseSettings;
        JObject rw = JObject.Parse(System.IO.File.ReadAllText(fileConfiguration));
        if (rw.ContainsKey("DatabaseSettings"))
        {
            databaseSettings = JsonConvert.DeserializeObject<InMotionGIT.Common.Domain.Configuration.FASIConfiguration>(rw["FASIConfiguration"].ToString());
            if (databaseSettings.ConnectionStrings.ContainsKey(connectionStringName))
            {
                result = databaseSettings.ConnectionStrings[connectionStringName];
            }
            else
            {
                var findKey = false;
                foreach (var key in databaseSettings.ConnectionStrings.Keys)
                {
                    if (key.EqualsIgnoringCase(connectionStringName))
                    {
                        result = databaseSettings.ConnectionStrings[connectionStringName];
                        findKey = true;
                        break;
                    }
                }
                if (!findKey)
                    throw new Exception("El connecctionStringName no existe'{0}'".SpecialFormater(connectionStringName));
            }
        }
        else
            throw new Exception("No existe connecctionString disponibles '{0}'");

        return result;
    }

    public static InMotionGIT.Common.Domain.Configuration.FASIConfiguration Configuration()
    {
        InMotionGIT.Common.Domain.Configuration.FASIConfiguration result = null;
        JObject rw = JObject.Parse(System.IO.File.ReadAllText(fileConfiguration));
        if (rw.ContainsKey("FASIConfiguration"))
        {
            result = JsonConvert.DeserializeObject<InMotionGIT.Common.Domain.Configuration.FASIConfiguration>(rw["FASIConfiguration"].ToString());
        }
        else
            throw new Exception("No existe configuracion en el archivo '{0}'".Formater(fileConfiguration));

        return result;
    }
}