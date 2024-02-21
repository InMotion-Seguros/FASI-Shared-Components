#Region "Imports"

Imports Microsoft

#End Region

Namespace Helpers

    ''' <summary>
    '''Class allows certified operations - Clase permite realizar operaciones sobre certificados
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegistryHandler

#Region "Private fields, to hold the state of the entity"

        Public Shared _regKey As Win32.RegistryKey

#End Region

#Region "Setting"

        Public Shared Sub SetSetting()
            _regKey = Win32.Registry.CurrentUser.OpenSubKey("Software\Global Insurance Technology", True)
            If IsNothing(_regKey) Then
                _regKey = Win32.Registry.CurrentUser.OpenSubKey("Software", True)
                CreateSubKey("Global Insurance Technology")

            End If
            release()
            _regKey = Win32.Registry.CurrentUser.OpenSubKey("Software\Global Insurance Technology\Settings", True)
            If IsNothing(_regKey) Then
                _regKey = Win32.Registry.CurrentUser.OpenSubKey("Software\Global Insurance Technology", True)
                CreateSubKey("Settings")
            End If
            release()
            _regKey = Win32.Registry.CurrentUser.OpenSubKey("Software\Global Insurance Technology\Settings", True)
        End Sub

#End Region

#Region "Main Methods"

        Public Shared Function GetValue(name As String, defaultValue As String) As String
            SetSetting()
            Return _regKey.GetValue(name, defaultValue)
        End Function

        Public Shared Sub SetValue(name As String, value As String)
            SetSetting()
            _regKey.SetValue(name, value, Win32.RegistryValueKind.String)
        End Sub

        Public Shared Sub release()
            _regKey.Close()
        End Sub

        Public Shared Sub CreateSubKey(ByVal name As String)
            SetSetting()
            _regKey.CreateSubKey(name)
        End Sub

#End Region

    End Class

End Namespace