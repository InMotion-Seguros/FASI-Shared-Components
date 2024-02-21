using InMotionGIT.Common.Extensions;
using System.Linq;
using System.Web;

namespace InMotionGIT.Common.Helpers
{
    public class SessionHandler
    {
        /// <summary>
        /// Almacena objetos en la sección.
        /// </summary>
        /// <param name="key">Nombre con que se almacena el objeto</param>
        /// <param name="value">Objeto a almacenar</param>
        public static void Save(string key, object value)
        {
            System.Web.SessionState.HttpSessionState session = null;
            if (HttpContext.Current is not null)
            {
                if (HttpContext.Current.Request is not null)
                {
                    session = HttpContext.Current.Session;
                }
            }

            if (session.IsNotEmpty())
            {
                string valueFound = HttpContext.Current.Session.Keys.Cast<string>().FirstOrDefault(itemKey => itemKey.Equals(key));
                if (valueFound.IsEmpty())
                {
                    session.Add(key, value);
                }
                else
                {
                    session[key] = value;
                }
            }
        }

        /// <summary>
        /// Elimina el objetos en la sección.
        /// </summary>
        /// <param name="key">Nombre del objeto a eliminar</param>
        public static void Remove(string key)
        {
            System.Web.SessionState.HttpSessionState session = null;
            if (HttpContext.Current is not null)
            {
                if (HttpContext.Current.Request is not null)
                {
                    session = HttpContext.Current.Session;
                }
            }

            if (session.IsNotEmpty())
            {
                session.Remove(key);
            }
        }

        /// <summary>
        /// Retorna el objeto en la sección.
        /// </summary>
        /// <param name="key">Nombre con que se almacena el objeto</param>
        public static object Retrieve(string key)
        {
            object result = null;
            System.Web.SessionState.HttpSessionState session = null;
            if (HttpContext.Current is not null)
            {
                if (HttpContext.Current.Request is not null)
                {
                    session = HttpContext.Current.Session;
                }
            }

            if (session.IsNotEmpty())
            {
                string valueFound = HttpContext.Current.Session.Keys.Cast<string>().FirstOrDefault(itemKey => itemKey.Equals(key));
                if (valueFound.IsNotEmpty())
                {
                    result = session[key];
                }
            }
            return result;
        }

        /// <summary>
        /// Verifica si existe el key de un objeto en sección
        /// </summary>
        /// <param name="key">Nombre a verificar la existencia en el sección</param>
        /// <returns></returns>
        public static bool Exist(string key)
        {
            bool result = false;
            System.Web.SessionState.HttpSessionState session = null;
            if (HttpContext.Current is not null)
            {
                if (HttpContext.Current.Request is not null)
                {
                    session = HttpContext.Current.Session;
                    if (session.IsNotEmpty())
                    {
                        string valueFound = HttpContext.Current.Session.Keys.Cast<string>().FirstOrDefault(itemKey => itemKey.Equals(key));
                        if (valueFound.IsNotEmpty())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}