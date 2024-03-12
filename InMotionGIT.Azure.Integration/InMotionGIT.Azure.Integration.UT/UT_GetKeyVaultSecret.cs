using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;

namespace InMotionGIT.Azure.Integration.UT
{
    [TestClass]
    public class UT_GetKeyVaultSecret
    {
        [TestMethod]
        public void GetKeyVaultSecret()
        {

            string AZURE_TENANT_ID = ConfigurationManager.AppSettings["AZURE_TENANT_ID"];
            string AZURE_CLIENT_ID = ConfigurationManager.AppSettings["AZURE_CLIENT_ID"];
            string AZURE_CLIENT_SECRET = ConfigurationManager.AppSettings["AZURE_CLIENT_SECRET"];
            string KEY_VAULT_URI = ConfigurationManager.AppSettings["KEY_VAULT_URI"];

            var strSecret = InMotionGIT.Azure.Integration.Manager.GetKeyVaultSecret(
                            KEY_VAULT_URI,
                            "Key",
                            AZURE_TENANT_ID,
                            AZURE_CLIENT_ID,
                            AZURE_CLIENT_SECRET);

            Assert.IsFalse(string.IsNullOrEmpty(strSecret));
        }
    }
}
