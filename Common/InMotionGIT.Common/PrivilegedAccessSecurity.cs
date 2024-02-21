﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using InMotionGIT.Common.Helpers;

namespace InMotionGIT.Common
{

    public class PrivilegedAccessSecurity
    {

        public static bool IsProvider()
        {
            bool result = false;
            if (ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.Provider"] != default | !string.IsNullOrEmpty(ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.Provider"]))
            {
                result = true;
            }
            return result;
        }

        public static string ConnectionString(string connectionName, string connectionStringFull)
        {
            string result = connectionStringFull.Replace(ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.Pattern"], Password(connectionName));
            return result;
        }

        public static string Password(string connectionStringName)
        {
            string result = "";
            var methodInfo = MethodLoad();
            var parametersArray = new Dictionary<string, object>();
            parametersArray.Add("Key", connectionStringName);
            parametersArray.Add("Provider", ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.Provider"]);
            Dictionary<string, string> resultMethod = (Dictionary<string, string>)methodInfo.Invoke(null, new[] { parametersArray });
            return resultMethod["Password"];
        }

        public static MethodInfo MethodLoad()
        {
            MethodInfo result = null;
            string key = "PrivilegedAccessSecurity.Provider";
            if (Caching.NotExist(key))
            {
                var assembly = Assembly.LoadFrom(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "InMotionGIT.Privileged.Access.Security.dll"));
                try
                {
                    result = assembly.GetTypes().Where(i => i.FullName.ToLower().Equals("InMotionGIT.Privileged.Access.Security.Manager".ToLower())).FirstOrDefault().GetMethods().Where(x => x.Name.ToLower().Equals("Process".ToLower())).FirstOrDefault();
                    Caching.SetItem(key, result);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                result = (MethodInfo)Caching.GetItem(key);
            }
            return result;
        }

    }
}