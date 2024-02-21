using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    [Obsolete("Esta clase esta obsoleta, Se migro a un paquete llamado \"InMotionGIT.Cache\" con la clase Manager.", false)]
    public class Caching
    {

        /// <summary>
        /// Determina si una clave existe en el cache
        /// </summary>
        /// <param name="key"></param>
        public static bool Exist(string key)
        {
            ObjectCache cache = MemoryCache.Default;

            return cache.Contains(key);
        }

        /// <summary>
        /// Determina si una clave no existe en el cache
        /// </summary>
        /// <param name="key"></param>
        public static bool NotExist(string key)
        {
            ObjectCache cache = MemoryCache.Default;

            return !cache.Contains(key);
        }

        /// <summary>
        /// Retorna el objeto asociado a la clave.
        /// </summary>
        /// <param name="key"></param>
        public static object GetItem(string key)
        {
            ObjectCache cache = MemoryCache.Default;
            return cache[key];
        }

        public static void SetItem(string key, object item)
        {
            SetItem(key, item, string.Empty, string.Empty, 0.0d);
        }

        public static void SetItem(string key, object item, double timeout)
        {
            SetItem(key, item, string.Empty, string.Empty, timeout);
        }

        public static void SetItem(string key, object item, string serviceName, string entityName)
        {
            SetItem(key, item, string.Empty, string.Empty, 0.0d);
        }

        public static void SetItem(string key, object item, string serviceName, string entityName, double timeout)
        {
            ObjectCache cache = MemoryCache.Default;
            double TotalMinutes = timeout;

            if (TotalMinutes == 0.0d)
            {
                if (serviceName.IsNotEmpty() && entityName.IsNotEmpty())
                {
                    TotalMinutes = Conversions.ToDouble(ConfigurationManager.AppSettings[string.Format(CultureInfo.InvariantCulture, "CacheExpiration.{0}.{1}", serviceName, entityName)]);
                }

                if (TotalMinutes == 0.0d && serviceName.IsNotEmpty())
                {
                    TotalMinutes = Conversions.ToDouble(ConfigurationManager.AppSettings[string.Format(CultureInfo.InvariantCulture, "CacheExpiration.{0}", serviceName)]);
                }

                if (TotalMinutes == 0.0d)
                {
                    TotalMinutes = Conversions.ToDouble(ConfigurationManager.AppSettings["CacheExpiration"]);
                }

                if (TotalMinutes == 0.0d)
                {
                    TotalMinutes = 20.0d;
                }
            }

            if (timeout == -1)
            {
                cache.Set(key, item, new CacheItemPolicy());
            }
            else
            {
                cache.Set(key, item, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TotalMinutes) });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="expiration"></param>
        /// <example>Helpers.Caching.SetItem(token, object, DateTimeOffset.Now.AddMinutes(2))</example>
        public static void SetItem(string key, object item, DateTimeOffset expiration)
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Set(key, item, new CacheItemPolicy() { AbsoluteExpiration = expiration });
        }

        /// <summary>
        /// Elimina el objeto asociado a la clave.
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            ObjectCache cache = MemoryCache.Default;
            cache.Remove(key);
        }

        public static string CacheCatalog()
        {
            ObjectCache cache = MemoryCache.Default;
            var buffer = new StringBuilder();
            buffer.AppendLine("<table>");
            foreach (KeyValuePair<string, object> item in cache)
            {
                buffer.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Key, Information.TypeName(item.Value), GetSizeOfObject(item.Value));
                buffer.AppendLine();
            }
            buffer.AppendLine("</table>");
            return buffer.ToString();
        }

        private static long GetSizeOfObject(object item)
        {
            long result = 0L;
            try
            {
                using (var stream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, item);

                    result = stream.Length;
                }
            }
            catch (Exception ex)
            {
                result = 0L;
            }

            return result;
        }

        public static void Clean()
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
                MemoryCache.Default.Remove(cacheKey);
        }

        public static void RemoveStartWith(string value)
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                if (cacheKey.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    MemoryCache.Default.Remove(cacheKey);
                }

            }
        }

    }

}