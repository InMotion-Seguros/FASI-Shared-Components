using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace InMotionGIT.Cache
{
    /// <summary>
    /// Class manager of caching in difrente providers
    /// </summary>
    public static class CachingManager
    {
        private static Enumerations.EnumCache _type;
        private static readonly ConnectionMultiplexer _redisConnection;
        private static IDatabase _cache;

        static CachingManager()
        {
            switch ("Cache.Type".AppSettings<string>(valueDefault: "memory").ToLower())
            {
                default:
                case "memory":
                    _type = Enumerations.EnumCache.Memory;
                    break;

                case "redis":
                    _type = Enumerations.EnumCache.Redis;
                    if (_redisConnection.IsEmpty())
                    {
                        _redisConnection = ConnectionMultiplexer.Connect("Cache.Redis.ConnectionString".AppSettings());
                        _cache = _redisConnection.GetDatabase();
                    }
                    break;
            }
        }

        /// <summary>
        /// Determina si una key existe en el _cache
        /// </summary>
        /// <param name="key"></param>
        public static bool Exist(this string key)
        {
            var result = false;
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    result = MemoryCache.Default.Contains(key);
                    break;

                case Enumerations.EnumCache.Redis:
                    result = _cache.KeyExists(key);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Determina si una key no existe en el _cache
        /// </summary>
        /// <param name="key"></param>
        public static bool NotExist(this string key)
        {
            var result = false;
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    result = !MemoryCache.Default.Contains(key);
                    break;

                case Enumerations.EnumCache.Redis:
                    result = !_cache.KeyExists(key);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Retorna el objeto asociado a la key.
        /// </summary>
        /// <param name="key"></param>
        public static T GetItem<T>(this string key)
        {
            T result = default;
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    result = (T)MemoryCache.Default[key];
                    break;

                case Enumerations.EnumCache.Redis:
                    result = JsonConvert.DeserializeObject<T>(_cache.StringGet(key));
                    break;
            }
            return result;
        }

        public static void SetItem(this string key, object item)
        {
            SetItem(key, item, string.Empty, string.Empty, 0.0d);
        }

        public static void SetItem(this string key, object item, double timeout)
        {
            SetItem(key, item, string.Empty, string.Empty, timeout);
        }

        public static void SetItem(this string key, object item, string serviceName, string entityName)
        {
            SetItem(key, item, string.Empty, string.Empty, 0.0d);
        }

        public static void SetItem(this string key, object item, string serviceName, string entityName, double timeout)
        {
            double TotalMinutes = timeout;
            if (TotalMinutes == 0.0d)
            {
                if (serviceName.IsNotEmpty() && entityName.IsNotEmpty())
                {
                    TotalMinutes = "CacheExpiration.{0}.{1}".SpecialFormater(serviceName, entityName).AppSettings<double>();
                }

                if (TotalMinutes == 0.0d && serviceName.IsNotEmpty())
                {
                    TotalMinutes = "CacheExpiration.{0}".SpecialFormater(serviceName).AppSettings<double>();
                }

                if (TotalMinutes == 0.0d)
                {
                    TotalMinutes = "CacheExpiration".AppSettings<double>();
                }

                if (TotalMinutes == 0.0d)
                {
                    TotalMinutes = 20.0d;
                }
            }
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:

                    if (timeout == -1)
                    {
                        MemoryCache.Default.Set(key, item, new CacheItemPolicy());
                    }
                    else
                    {
                        MemoryCache.Default.Set(key, item, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TotalMinutes) });
                    }
                    break;

                case Enumerations.EnumCache.Redis:
                    _cache.StringSet(key, JsonConvert.SerializeObject(item), TimeSpan.FromMinutes(TotalMinutes));
                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="expiration"></param>
        /// <example>Helpers.Caching.SetItem(token, object, DateTimeOffset.Now.AddMinutes(2))</example>
        public static void SetItem(this string key, object item, DateTimeOffset expiration)
        {
            MemoryCache.Default.Set(key, item, new CacheItemPolicy() { AbsoluteExpiration = expiration });
        }

        /// <summary>
        /// Elimina el objeto asociado a la key.
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(this string key)
        {
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    MemoryCache.Default.Remove(key);
                    break;

                case Enumerations.EnumCache.Redis:
                    _cache.KeyDelete(key);
                    break;
            }
        }

        public static string CacheCatalog()
        {
            var buffer = new StringBuilder();
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    buffer.AppendLine("<table>");
                    foreach (KeyValuePair<string, object> item in MemoryCache.Default)
                    {
                        buffer.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Key, Information.TypeName(item.Value), GetSizeOfObject(item.Value));
                        buffer.AppendLine();
                    }
                    buffer.AppendLine("</table>");
                    break;

                case Enumerations.EnumCache.Redis:

                    buffer.AppendLine("<table>");
                    foreach (var key in ((RedisKey[])_cache.Execute("KEYS", "*")))
                    {
                        buffer.AppendFormat(@"<tr>
                                                  <td>{0}</td>
                                                  <td>{1}</td>
                                             </tr>", key, GetSizeOfObject((string)_cache.StringGet(key).ToString()));
                        buffer.AppendLine();
                    }
                    buffer.AppendLine("</table>");

                    break;
            }

            return buffer.ToString();
        }

        private static long GetSizeOfObject(this object item)
        {
            long result = 0L;
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
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
                    break;

                case Enumerations.EnumCache.Redis:
                    try
                    {
                        Encoding encoding = Encoding.UTF8;
                        byte[] bytes = encoding.GetBytes((string)item);

                        // Obtener el tamaño en bytes
                        result = bytes.Length;
                    }
                    catch (Exception ex)
                    {
                        result = 0L;
                    }

                    break;
            }

            return result;
        }

        public static void Clean()
        {
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
                    foreach (string cacheKey in cacheKeys)
                        MemoryCache.Default.Remove(cacheKey);
                    break;

                case Enumerations.EnumCache.Redis:
                    var todasLasClaves = _cache.Execute("KEYS", "*");
                    foreach (var key in (RedisKey[])todasLasClaves)
                    {
                        _cache.KeyDelete(key);
                    }
                    break;
            }
        }

        public static void RemoveStartWith(this string value)
        {
            switch (_type)
            {
                default:
                case Enumerations.EnumCache.Memory:
                    var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
                    foreach (string cacheKey in cacheKeys)
                        if (cacheKey.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            MemoryCache.Default.Remove(cacheKey);
                        }

                    break;

                case Enumerations.EnumCache.Redis:
                    var todasLasClaves = _cache.Execute("KEYS", "*");
                    foreach (var key in (RedisKey[])todasLasClaves)
                    {
                        if (key.ToString().StartsWith(value))
                        {
                            _cache.KeyDelete(key);
                        }
                    }
                    break;
            }
        }
    }
}