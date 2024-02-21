Imports System.Configuration

Namespace Helpers

    <Obsolete("Not used any more, se migro a las extencions AppSettingsExtensions", True)>
    Public NotInheritable Class ApplicationSetting

        Public Shared Function IsTrue(appSettingName As String) As Boolean
            Dim result As Boolean = False
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty AndAlso
               ConfigurationManager.AppSettings(appSettingName).ToLower.Equals("true") Then
                result = True
            End If

            Return result
        End Function

        Public Shared Function IsFalse(appSettingName As String) As Boolean
            Dim result As Boolean = True
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty AndAlso
               ConfigurationManager.AppSettings(appSettingName).ToLower.Equals("false") Then
                result = True
            End If

            Return result
        End Function

        Public Shared Function StringValue(appSettingName As String) As String
            Return StringValue(appSettingName, String.Empty)
        End Function

        Public Shared Function StringValue(appSettingName As String, defaultValue As String) As String
            Dim result As String = String.Empty
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty Then
                result = ConfigurationManager.AppSettings(appSettingName)
            End If
            If result.IsEmpty Then
                result = defaultValue
            End If

            Return result
        End Function

    End Class

End Namespace