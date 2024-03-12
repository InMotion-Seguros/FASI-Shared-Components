using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMotionGIT.Azure.Integration
{
    public static class Manager
    {
        /// <summary>
        /// Obtiene valor de Azure KeyVault Secret por permisos de Registros de aplicaciones.
        /// </summary>
        /// <param name="uriKeyVault">URI almacen de azure keyvault</param>
        /// <param name="keySecret">Nombre de KeySecret que se requiere obtener valor</param>
        /// <param name="tenantId">Id de directorio del Registro de Aplicaciones con permisos en el almacen de secreto</param>
        /// <param name="clientId">Id del Registro de Aplicaciones con permisos en el almacen de secretos</param>
        /// <param name="clientSecret">Valor ClientSecret  que la aplicación usa para probar su identidad al solicitar un token. También se conoce como contraseña de aplicación. del Registro de Aplicaciones con permisos en el almacen de secreto</param>
        /// <returns>Retorna valor de KeySecret de Azure KeyVault</returns>
        public static string GetKeyVaultSecret(string uriKeyVault, string keySecret, string tenantId, string clientId, string clientSecret)
        {
            string result = string.Empty;
            try
            {
                var credenciales = new ClientSecretCredential(tenantId, clientId, clientSecret);
                var client = new SecretClient(new Uri(uriKeyVault), credenciales);
                var secret = client.GetSecret(keySecret);
                result = secret.Value.Value;
            }
            catch (Exception ex)
            {
                var message = "error get Azure KeyVault Secret";
                var exception = new Exception(message);
                InMotionGIT.Common.Helpers.LogHandler.ErrorLog("GetKeyVaultSecret", message, ex);
                throw exception;
            }

            return result;
        }
    }
}
