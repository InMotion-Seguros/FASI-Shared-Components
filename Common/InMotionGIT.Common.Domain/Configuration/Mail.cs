#region using

using System.Configuration;
using Microsoft.VisualBasic.CompilerServices;

#endregion

namespace InMotionGIT.Common.Domain.Configuration
{
    public class Mail
    {
        /// <summary>
        /// Behavior for sen e-mail.
        /// Comportamiento para enviar correos.
        /// </summary>
        public Enumerations.EnumMailMode Mode { set; get; } = Enumerations.EnumMailMode.NetMail;

        /// <summary>
        /// SMTP (Simple Mail Transfer Protocol).
        /// SMTP (Protocolo Simple de Transferencia de Correo).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>

        public string Host { get; set; } = "west.exch083.serverdata.net";

        /// <summary>
        /// SMTP Port (Simple Mail Transfer Protocol).
        /// SMTP Puerto (Protocolo Simple de Transferencia de Correo).
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Port { get; set; } = 587;

        /// <summary>
        /// E-mail for support of send
        /// Correo electrónico para el soporte del envio
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SupportMail { get; set; } = "OnLineServices@inmotiongit.com";
         

        /// <summary>
        /// E-mail for send the mail.
        /// Correo electrónico para enviar el correo
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks> 
        public string CredentialUserName { get ; set; } 

        /// <summary>
        /// Password for send the e-mail.
        /// Contraseña del correo electrónico.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string CredentialPassword {get; set; }

        /// <summary>
        /// Enable connection for secure port.
        /// Habilitar la conexión por puerto seguro.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks> 
        public bool EnableSSL {  get; set; }

        /// <summary>
        /// Address where the templates are located.
        /// Dirección donde están localizadas las plantillas.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string TemplatesPath { get; set; } = @"...\templates";
    }
}