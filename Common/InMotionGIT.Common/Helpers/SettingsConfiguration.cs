using System;
using System.Configuration;
using System.IO;

namespace InMotionGIT.Common.Helpers
{

    public class SettingsConfiguration
    {

        public static void AddConnectionsString(Services.Contracts.ConnectionStrings ConnectionStrings)
        {
            bool mode = false;
            string RootPathAssembly = new Uri(typeof(SettingsConfiguration).Assembly.CodeBase).LocalPath.Replace(@"\InMotionGIT.Common.DLL", "");
            string configuration = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (!configuration.Contains("web.config"))
            {
                mode = true;
            }
            if (RootPathAssembly.ToLower().Contains("bin"))
            {
                RootPathAssembly = RootPathAssembly.Replace(@"\bin", "").Replace(@"\Debug", "").Replace(@"\Release", "");
            }
            if (mode)
            {
                RootPathAssembly = Path.Combine(RootPathAssembly, "App.config");
            }
            else
            {
                RootPathAssembly = Path.Combine(RootPathAssembly, "web.config");
            }
            var executionFileMap = new ExeConfigurationFileMap() { ExeConfigFilename = RootPathAssembly };
            var config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);

            ConnectionStringsSection connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (connectionStringsSection.ConnectionStrings.Count == 0 | connectionStringsSection.ConnectionStrings["CurrentConnectionsString"] == null)
            {
                connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings()
                {
                    Name = "CurrentConnectionsString",
                    ConnectionString = ConnectionStrings.ConnectionString,
                    ProviderName = ConnectionStrings.ProviderName
                });
            }
            else
            {
                {
                    var withBlock = connectionStringsSection.ConnectionStrings["CurrentConnectionsString"];
                    withBlock.ConnectionString = ConnectionStrings.ConnectionString;
                    withBlock.ProviderName = ConnectionStrings.ProviderName;
                }
            }

            config.Save(ConfigurationSaveMode.Minimal, true);
            ConfigurationManager.RefreshSection("connectionStrings");

        }

    }

}