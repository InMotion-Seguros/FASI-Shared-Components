 
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization; 

namespace InMotionGIT.Common.Domain.DataType;


[Attributes. EntityCommonlyUsed()]
[DataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
[Serializable()]
[XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
[XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
[DebuggerDisplay("Notify {ErrorId} {Message}")]
public class Notify
{

    #region Public properties, to expose the state of the entity

    /// <summary>
    /// Nombre de la propiedad o campo que da origen a la notificación
    /// </summary>
    [DataMember()]
    [XmlAttribute()]
    public string Source { get; set; }

    /// <summary>
    /// Severidad de la notificación
    /// </summary>
    [DataMember()]
    [XmlAttribute()]
    public Enumerations.EnumNotifySeverity Severity { get; set; }

    /// <summary>
    /// Número que identifica de la notificación
    /// </summary>
    [DataMember()]
    [XmlAttribute()]
    public long Id { get; set; }

    /// <summary>
    /// Mensaje a ser desplegado por la notificación
    /// </summary>
    [DataMember()]
    [XmlAttribute()]
    public string Message { get; set; }

    #endregion

    /// <summary>
    /// Constructor base.
    /// </summary>
    public Notify() : base()
    {
    }

    /// <summary>
    /// Permite saber si existe un error en la colección de notificación.
    /// </summary>
    /// <param name="notifyList">colección de notificación</param>
    /// <returns>Verdadero en caso de existir un error, falso en caso contrario</returns>
    public static bool HasErrors(List<Notify> notifyList)
    {
        bool result = false;
        if (!(notifyList == null))
        {
            foreach (Notify Item in notifyList)
            {
                if (Item.Severity == Enumerations.EnumNotifySeverity.Error)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Permite saber si existe una advertencia en la colección de notificación.
    /// </summary>
    /// <param name="notifyList">colección de notificación</param>
    /// <returns>Verdadero en caso de existir una advertencia, falso en caso contrario</returns>
    public static bool HasWarnings(List<Notify> notifyList)
    {
        bool result = false;
        if (!(notifyList == null))
        {
            foreach (Notify Item in notifyList)
            {
                if (Item.Severity == Enumerations.EnumNotifySeverity.Warning)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Permite saber si existe un mensaje en la colección de notificación.
    /// </summary>
    /// <param name="notifyList">colección de notificación</param>
    /// <returns>Verdadero en caso de existir un mensaje, falso en caso contrario</returns>
    public static bool HasMessages(List<Notify> notifyList)
    {
        bool result = false;
        if (!(notifyList == null))
        {
            foreach (Notify Item in notifyList)
            {
                if (Item.Severity == Enumerations.EnumNotifySeverity.Message)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

}