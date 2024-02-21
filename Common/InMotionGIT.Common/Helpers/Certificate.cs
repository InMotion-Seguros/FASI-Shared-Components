
#region Imports

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace InMotionGIT.Common.Helpers
{

    /// <summary>
    /// Class allows certified operations - Clase permite realizar operaciones sobre certificados
    /// </summary>
    /// <remarks></remarks>
    public class Certificate
    {

        /// <summary>
        /// Handler that allows a self-certification, in this case self-signed cerificados - Manejador que permite realizar una auto certificación, en este caso de cerificados autofirmados
        /// </summary>
        /// <remarks></remarks>
        public static void OverrideCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertValidate);
        }

        /// <summary>
        /// Method that allows the validation of 'ServicePointManager' - Metodo que permite la validación sobre 'ServicePointManager'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }

    }

}