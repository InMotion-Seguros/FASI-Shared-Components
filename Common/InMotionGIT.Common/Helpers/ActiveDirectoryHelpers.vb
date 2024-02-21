#Region "Services"

Imports System.DirectoryServices.AccountManagement

#End Region

Namespace Helpers

    Public Class ActiveDirectoryHelpers

        ''' <summary>
        ''' Validate el user and password in server from active directory
        ''' </summary>
        ''' <param name="username">Username of user</param>
        ''' <param name="password">Password of user in active directory</param>
        ''' <returns></returns>
        Public Shared Function ValidateUserPassword(username As String, password As String) As Boolean
            Dim result As Boolean = False
            Try
                Using client As New PrincipalContext(ContextType.Domain, "FrontOffice.Security.Doman.Name".AppSettings().ToString)
                    result = client.ValidateCredentials(username, password)
                End Using
            Catch ex As Exception
                InMotionGIT.Common.Helpers.LogHandler.ErrorLog("Error in 'ValidateUserPassword'", ex.Message, ex)
            End Try
            Return result
        End Function

    End Class

End Namespace