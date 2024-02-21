using System;
#region Services

using System.DirectoryServices.AccountManagement;
using InMotionGIT.Common.Extensions;

#endregion

namespace InMotionGIT.Common.Helpers
{

    public class ActiveDirectoryHelpers
    {

        /// <summary>
        /// Validate el user and password in server from active directory
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user in active directory</param>
        /// <returns></returns>
        public static bool ValidateUserPassword(string username, string password)
        {
            bool result = false;
            try
            {
                using (var client = new PrincipalContext(ContextType.Domain, "FrontOffice.Security.Doman.Name".AppSettings().ToString()))
                {
                    result = client.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                LogHandler.ErrorLog("Error in 'ValidateUserPassword'", ex.Message, ex);
            }
            return result;
        }

    }

}