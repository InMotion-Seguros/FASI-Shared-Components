Imports System.ComponentModel
Imports System.Configuration
Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Clase de extencion para appsettings
    ''' </summary>
    Public Module AppSettingsExtensions

        <Extension()>
        Public Function IsTrue(appSettingName As String) As Boolean
            Dim result As Boolean = False
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty AndAlso
               ConfigurationManager.AppSettings(appSettingName).ToLower.Equals("true") Then
                result = True
            End If

            Return result
        End Function

        <Extension()>
        Public Function IsFalse(appSettingName As String) As Boolean
            Dim result As Boolean = True
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty AndAlso
               ConfigurationManager.AppSettings(appSettingName).ToLower.Equals("false") Then
                result = True
            End If

            Return result
        End Function

        <Extension()>
        Public Function StringValue(appSettingName As String) As String
            Return StringValue(appSettingName, String.Empty)
        End Function

        <Extension()>
        Public Function StringValue(appSettingName As String, defaultValue As String) As String
            Dim result As String = String.Empty
            If ConfigurationManager.AppSettings(appSettingName).IsNotEmpty Then
                result = ConfigurationManager.AppSettings(appSettingName)
            End If
            If result.IsEmpty Then
                result = defaultValue
            End If

            Return result
        End Function

        <Extension()>
        Function AppSettings(Of T)(ByVal key As String, Optional throwIfNotExit As Boolean = False, ByVal Optional valueDefault As T = Nothing) As T
            Dim result As T
            Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(T))
            If ConfigurationManager.AppSettings(key) IsNot Nothing Then
                result = converter.ConvertFrom(ConfigurationManager.AppSettings(key))
            Else
                If throwIfNotExit AndAlso ConfigurationManager.AppSettings(key) Is Nothing Then
                    Throw New Exception("No existe el AppSetting solicitado '{0}'".SpecialFormater(key))
                Else
                    result = valueDefault
                End If
            End If
            Return result
        End Function

        <Extension()>
        Function AppSettings(ByVal key As String, Optional throwIfNotExit As Boolean = False) As String
            Return AppSettings(Of String)(key, throwIfNotExit)
        End Function

        <Extension()>
        Function AppSettingsOnEquals(ByVal key As String, valueComper As String, Optional throwIfNotExit As Boolean = False, Optional IgnoreCase As Boolean = True) As Boolean
            If IgnoreCase Then
                Return AppSettings(Of String)(key, throwIfNotExit).Equals(valueComper, StringComparison.CurrentCultureIgnoreCase)
            Else
                Return AppSettings(Of String)(key, throwIfNotExit).Equals(valueComper)
            End If

        End Function

    End Module

End Namespace