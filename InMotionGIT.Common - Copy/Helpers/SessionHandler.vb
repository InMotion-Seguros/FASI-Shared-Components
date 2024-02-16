Imports System.Web

Namespace Helpers

    Public Class SessionHandler

        ''' <summary>
        ''' Almacena objetos en la sección.
        ''' </summary>
        ''' <param name="key">Nombre con que se almacena el objeto</param>
        ''' <param name="value">Objeto a almacenar</param>
        Public Shared Sub Save(key As String, value As Object)
            Dim session As System.Web.SessionState.HttpSessionState = Nothing
            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    session = HttpContext.Current.Session
                End If
            End If

            If session.IsNotEmpty Then
                Dim valueFound As String = (From itemKey In session.Keys Where itemKey.ToString.Equals(key) Select itemKey).FirstOrDefault
                If valueFound.IsEmpty Then
                    session.Add(key, value)
                Else
                    session(key) = value
                End If
            End If
        End Sub

        ''' <summary>
        '''  Elimina el objetos en la sección.
        ''' </summary>
        ''' <param name="key">Nombre del objeto a eliminar</param>
        Public Shared Sub Remove(key As String)
            Dim session As System.Web.SessionState.HttpSessionState = Nothing
            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    session = HttpContext.Current.Session
                End If
            End If

            If session.IsNotEmpty Then
                session.Remove(key)
            End If
        End Sub

        ''' <summary>
        ''' Retorna el objeto en la sección.
        ''' </summary>
        ''' <param name="key">Nombre con que se almacena el objeto</param>
        Public Shared Function Retrieve(key As String) As Object
            Dim result As Object = Nothing
            Dim session As System.Web.SessionState.HttpSessionState = Nothing
            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    session = HttpContext.Current.Session
                End If
            End If

            If session.IsNotEmpty Then
                Dim valueFound As String = (From itemKey In session.Keys Where itemKey.ToString.Equals(key) Select itemKey).FirstOrDefault
                If valueFound.IsNotEmpty Then
                    result = session(key)
                End If
            End If
            Return result
        End Function

        ''' <summary>
        ''' Verifica si existe el key de un objeto en sección
        ''' </summary>
        ''' <param name="key">Nombre a verificar la existencia en el sección</param>
        ''' <returns></returns>
        Public Shared Function Exist(key As String) As Boolean
            Dim result As Boolean = False
            Dim session As System.Web.SessionState.HttpSessionState = Nothing
            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    session = HttpContext.Current.Session
                    If session.IsNotEmpty Then
                        Dim valueFound As String = (From itemKey In session.Keys
                                                    Where itemKey.ToString.Equals(key)
                                                    Select itemKey).FirstOrDefault
                        If valueFound.IsNotEmpty Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

    End Class

End Namespace