using CyberArk.AIM.NetPasswordSDK;
using CyberArk.AIM.NetPasswordSDK.Exceptions;
using System.Configuration;

namespace InMotionGIT.Privileged.Access.Security.CyberArk
{
    public static class Manager
    {
        public static string GetPasswordSArk(string key)
        {
            PSDKPasswordRequest passRequest;
            passRequest = new PSDKPasswordRequest();

            PSDKPassword password;
            string sPassword = "", Safe = "", Folder = "", Object = "", Reason = "", UserName = "";

            string AppId = "";
            int ConnectionPort = 0, ConnectionTimeout = 0;

            AppId = ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.AppID"];
            ConnectionPort = int.Parse(ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.ConnectionPort"]);
            ConnectionTimeout = int.Parse(ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.ConnectionTimeout"]);
            Safe = ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.Safe"];
            Folder = ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.Folder"];
            Object = key;
            Reason = ConfigurationManager.AppSettings["InMotionGIT.Privileged.Access.Security.CyberArk.Reason"];
            UserName = key;

            try
            {
                if (AppId != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"1. Inicializa AppID, key:{key}, AppID:{AppId}");
                    passRequest.AppID = AppId;
                }

                if (ConnectionPort != 0)
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"2. Inicializa ConnectionPort, key:{key}, ConnectionPort:{ConnectionPort}");
                    passRequest.ConnectionPort = ConnectionPort;
                }

                if (ConnectionTimeout != 0)
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"3. Inicializa ConnectionTimeout, key:{key}, ConnectionTimeout:{ConnectionTimeout}");
                    passRequest.ConnectionTimeout = ConnectionTimeout;
                }

                if (Safe != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"4. Inicializa Safe, key:{key}, Safe:{Safe}");
                    passRequest.Safe = Safe;
                }

                if (Folder != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"5. Inicializa Folder, key:{key}, Folder:{Folder}");
                    passRequest.Folder = Folder;
                }

                if (Object != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"6. Inicializa Object, key:{key}, Object:{Object}");
                    passRequest.Object = Object;
                }

                if (Reason != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"7. Inicializa Reason, key:{key}, Reason:{Reason}");
                    passRequest.Reason = Reason;
                }

                if (UserName != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"8. Inicializa UserName, key:{key}, UserName:{UserName}");
                    passRequest.UserName = UserName;
                }

                password = PasswordSDK.GetPassword(passRequest);

                var SecureContent = password.SecureContent;

                sPassword = new System.Net.NetworkCredential("", SecureContent).Password;

                InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", "Se obtiene Password");
                if (sPassword != "")
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"Password contiene informacion, key:{key}, Password:{sPassword}");
                }
                else
                {
                    InMotionGIT.Common.Helpers.LogHandler.TraceLog("InMotionGIT.Privileged.Access.Security.CyberArk", "Password esta vacia");
                }

            }
            catch (PSDKException ex)
            {
                InMotionGIT.Common.Helpers.LogHandler.ErrorLog("InMotionGIT.Privileged.Access.Security.CyberArk", $"key:{key}, Reason:{ex.Reason}, Message:{ex.Message}", ex);
            }
            return sPassword;
        }
    }
}