Imports System.Configuration
Imports System.Reflection
Imports InMotionGIT.Common.Helpers

Public Class PrivilegedAccessSecurity

    Public Shared Function IsProvider() As Boolean
        Dim result As Boolean = False
        If Not ConfigurationManager.AppSettings("InMotionGIT.Privileged.Access.Security.Provider") = Nothing Or Not ConfigurationManager.AppSettings("InMotionGIT.Privileged.Access.Security.Provider") = "" Then
            result = True
        End If
        Return result
    End Function

    Public Shared Function ConnectionString(connectionName As String, connectionStringFull As String) As String
        Dim result As String = connectionStringFull.Replace(ConfigurationManager.AppSettings("InMotionGIT.Privileged.Access.Security.Pattern"), Password(connectionName))
        Return result
    End Function

    Public Shared Function Password(connectionStringName As String) As String
        Dim result As String = ""
        Dim methodInfo As MethodInfo = MethodLoad()
        Dim parametersArray As New Dictionary(Of String, Object)
        parametersArray.Add("Key", connectionStringName)
        parametersArray.Add("Provider", ConfigurationManager.AppSettings("InMotionGIT.Privileged.Access.Security.Provider"))
        Dim resultMethod As Dictionary(Of String, String) = methodInfo.Invoke(Nothing, {parametersArray})
        Return resultMethod("Password")
    End Function

    Public Shared Function MethodLoad() As MethodInfo
        Dim result As MethodInfo = Nothing
        Dim key As String = "PrivilegedAccessSecurity.Provider"
        If Caching.NotExist(key) Then
            Dim assembly As Assembly = Assembly.LoadFrom(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(New System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), "InMotionGIT.Privileged.Access.Security.dll"))
            Try
                result = assembly.GetTypes().Where(Function(i) i.FullName.ToLower().Equals("InMotionGIT.Privileged.Access.Security.Manager".ToLower())).FirstOrDefault().GetMethods().Where(Function(x) x.Name.ToLower().Equals("Process".ToLower())).FirstOrDefault()
                Caching.SetItem(key, result)
            Catch ex As Exception

            End Try
        Else
            result = Caching.GetItem(key)
        End If
        Return result
    End Function

End Class