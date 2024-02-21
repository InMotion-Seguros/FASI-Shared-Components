using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Helpers;


/// <summary>
/// Permite el manejo de las variables expuestas por el HttpContext.
/// </summary>
public sealed class Context
{

    /// <summary>
    /// Recupera de forma unificada todas las variable form, querystring y de sesión, que este disponibles en el contexto.
    /// </summary>
    /// <returns>Diccionario con todas la variables del contexto.</returns>
    public static Dictionary<string, object> HttpValues()
    {
        HttpRequest request = null;
        System.Web.SessionState.HttpSessionState session = null;
        var result = new Dictionary<string, object>();

        if (HttpContext.Current is not null)
        {
            if (HttpContext.Current.Request is not null)
            {
                request = HttpContext.Current.Request;
            }
            if (HttpContext.Current.Session is not null)
            {
                session = HttpContext.Current.Session;
            }
        }

        if (request.IsNotEmpty())
        {
            foreach (string key in request.QueryString.AllKeys)
            {
                if (key.IsNotEmpty())
                {
                    result.Add(string.Format(CultureInfo.InvariantCulture, "QueryString.{0}", key), request.QueryString[key]);
                }
            }
            foreach (string key in request.Form.AllKeys)
            {
                if (key.IsNotEmpty())
                {
                    result.Add(string.Format(CultureInfo.InvariantCulture, "Form.{0}", key), request.Form[key]);
                }
            }
        }

        if (session.IsNotEmpty())
        {
            foreach (string key in session.Keys)
                result.Add(string.Format(CultureInfo.InvariantCulture, "Session.{0}", key), session[key]);
        }

        return result;
    }

    /// <summary>
    /// Recupera las variable del querystring que este disponibles en el contexto.
    /// </summary>
    /// <returns>Diccionario con todas la variables del querystring.</returns>
    public static Dictionary<string, object> QueryStringToDictionary()
    {
        var result = new Dictionary<string, object>();
        bool nZone = false;
        bool nMainAction = false;
        bool nAction = false;
        bool Action = false;

        foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
        {
            if (key.IsNotEmpty())
            {
                result.Add(key, HttpContext.Current.Request.QueryString[key]);

                if (key.Equals("nZone", StringComparison.CurrentCultureIgnoreCase))
                {
                    nZone = true;
                }
                else if (key.Equals("nAction", StringComparison.CurrentCultureIgnoreCase))
                {
                    nAction = true;
                }
                else if (key.Equals("nMainAction", StringComparison.CurrentCultureIgnoreCase))
                {
                    nMainAction = true;
                }
                else if (key.Equals("Action", StringComparison.CurrentCultureIgnoreCase))
                {
                    Action = true;
                }
            }
        }
        if (!nZone)
        {
            result.Add("nZone", "1");
        }
        if (!nAction)
        {
            result.Add("nAction", "0");
        }
        if (!nMainAction)
        {
            result.Add("nMainAction", "0");
        }
        if (!Action)
        {
            result.Add("Action", "");
        }
        return result;
    }

    /// <summary>
    /// Recupera las variable del form que este disponibles en el contexto.
    /// </summary>
    /// <returns>Diccionario con todas la variables del form.</returns>
    public static Dictionary<string, object> FormToDictionary()
    {
        var result = new Dictionary<string, object>();
        foreach (string key in HttpContext.Current.Request.Form.AllKeys)
            result.Add(key, HttpContext.Current.Request.Form[key]);
        return result;
    }

    /// <summary>
    /// Recupera las variable de la sesión que este disponibles en el contexto.
    /// </summary>
    /// <returns>Diccionario con todas la variables de la sesión.</returns>
    public static Dictionary<string, object> SessionToDictionary()
    {
        var result = new Dictionary<string, object>();
        foreach (string key in HttpContext.Current.Session.Keys)
            result.Add(key, HttpContext.Current.Session[key]);
        return result;
    }

}