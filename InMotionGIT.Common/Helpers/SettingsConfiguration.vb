Imports System.Configuration
Imports System.IO

Namespace Helpers

    Public Class SettingsConfiguration

        Public Shared Sub AddConnectionsString(ConnectionStrings As Services.Contracts.ConnectionString)
            Dim mode As Boolean = False
            Dim RootPathAssembly = New Uri(GetType(SettingsConfiguration).Assembly.CodeBase).LocalPath.Replace("\InMotionGIT.Common.DLL", "")
            Dim configuration As String = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            If Not configuration.Contains("web.config") Then
                mode = True
            End If
            If RootPathAssembly.ToLower.Contains("bin") Then
                RootPathAssembly = RootPathAssembly.Replace("\bin", "").Replace("\Debug", "").Replace("\Release", "")
            End If
            If mode Then
                RootPathAssembly = Path.Combine(RootPathAssembly, "App.config")
            Else
                RootPathAssembly = Path.Combine(RootPathAssembly, "web.config")
            End If
            Dim executionFileMap = New ExeConfigurationFileMap() With {.ExeConfigFilename = RootPathAssembly}
            Dim config = ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None)

            Dim connectionStringsSection = DirectCast(config.GetSection("connectionStrings"), ConnectionStringsSection)
            If connectionStringsSection.ConnectionStrings.Count = 0 Or
                IsNothing(connectionStringsSection.ConnectionStrings("CurrentConnectionsString")) Then
                connectionStringsSection.ConnectionStrings.Add(New ConnectionStringSettings With {.Name = "CurrentConnectionsString",
                                                                                                  .ConnectionString = ConnectionStrings.ConnectionString,
                                                                                                  .ProviderName = ConnectionStrings.ProviderName})
            Else
                With connectionStringsSection.ConnectionStrings("CurrentConnectionsString")
                    .ConnectionString = ConnectionStrings.ConnectionString
                    .ProviderName = ConnectionStrings.ProviderName
                End With
            End If

            config.Save(ConfigurationSaveMode.Minimal, True)
            ConfigurationManager.RefreshSection("connectionStrings")

        End Sub

    End Class

End Namespace