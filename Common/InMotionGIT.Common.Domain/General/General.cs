using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Domain.General;

public class General
{
    #region Properties Default

    /// <summary>
    /// Default language of the web portal.
    /// Idioma predeterminado del portal web.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string DefaultLanguage { get; set; } = "es";

    /// <summary>
    /// Default theme of the web portal.
    /// Tema predeterminado del portal web.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>

    public string DefaultTheme { get; set; } = "HorizontalWhiteBluePlus";

    /// <summary>
    /// Define la URL al home del aplicación
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string UrlHome { get; set; }

    #endregion Properties Default
}