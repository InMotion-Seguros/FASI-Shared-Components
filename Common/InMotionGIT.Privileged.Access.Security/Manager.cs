using InMotionGIT.Common.Extensions;
using System;
using System.Collections.Generic;

namespace InMotionGIT.Privileged.Access.Security
{
    /// <summary>
    /// Backups con el dll de codigo fuente del projecto
    /// </summary>
    public static class Manager
    {
        public static Dictionary<string, string> Process(Dictionary<string, object> parameters)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var key = "Provider";
            if (parameters.ContainsKey(key))
            {
                switch (parameters[key])
                {
                    case "CyberArk":
                        //if (ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.Dummy"] != null)
                        //{
                        //    Dictionary<string, string> root = InMotionGIT.Common.Helpers.Serialize.DeserializeJSON<Dictionary<string, string>>(System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), "source.json")));
                        //    if (root.ContainsKey((string)parameters["Key"]))
                        //        result.Add("Password", root[(string)parameters["Key"]]);
                        //    else
                        //    {
                        //        Exception ex = new Exception($"No se encontro el key {parameters["Key"]}");
                        //        InMotionGIT.Common.Helpers.LogHandler.ErrorLog("InMotionGIT.Privileged.Access.Security.Process-Dummy", $"No se encontro el key {parameters["Key"]}", ex);
                        //        throw ex;
                        //    }
                        //}
                        //else
                        //{
                        var password = CyberArk.Manager.GetPasswordSArk((string)parameters["Key"]);
                        if (password.IsEmpty())
                            password = "frontoffice";
                        result.Add("Password", password);
                        //}

                        return result;

                    default:
                        Exception exception = new Exception($"No se a seleccionado ningun provider valido, {parameters[key]}");
                        InMotionGIT.Common.Helpers.LogHandler.ErrorLog("InMotionGIT.Privileged.Access.Security.Process", $"No se a seleccionado ningun provider valido, {parameters[key]}", exception);
                        throw exception;
                }
            }
            else
            {
                Exception exception = new Exception("En los parametros no existe el key 'Provider'");
                InMotionGIT.Common.Helpers.LogHandler.ErrorLog("InMotionGIT.Privileged.Access.Security.Process", "Se requiere el key 'provider' en los parametros", exception);
                throw exception;
            }
        }
    }
}