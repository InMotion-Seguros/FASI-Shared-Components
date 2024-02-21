Imports System.Globalization
Imports System.Web

Namespace Helpers

    ''' <summary>
    ''' Permite el manejo de las variables expuestas por el HttpContext.
    ''' </summary>
    Public NotInheritable Class Context

        ''' <summary>
        ''' Recupera de forma unificada todas las variable form, querystring y de sesión, que este disponibles en el contexto.
        ''' </summary>
        ''' <returns>Diccionario con todas la variables del contexto.</returns>
        Public Shared Function HttpValues() As Dictionary(Of String, Object)
            Dim request As System.Web.HttpRequest = Nothing
            Dim session As System.Web.SessionState.HttpSessionState = Nothing
            Dim result As New Dictionary(Of String, Object)

            If HttpContext.Current IsNot Nothing Then
                If HttpContext.Current.Request IsNot Nothing Then
                    request = HttpContext.Current.Request
                End If
                If HttpContext.Current.Session IsNot Nothing Then
                    session = HttpContext.Current.Session
                End If
            End If

            If request.IsNotEmpty Then
                For Each key As String In request.QueryString.AllKeys
                    If key.IsNotEmpty Then
                        result.Add(String.Format(CultureInfo.InvariantCulture, "QueryString.{0}", key), request.QueryString(key))
                    End If
                Next
                For Each key As String In request.Form.AllKeys
                    If key.IsNotEmpty Then
                        result.Add(String.Format(CultureInfo.InvariantCulture, "Form.{0}", key), request.Form(key))
                    End If
                Next
            End If

            If session.IsNotEmpty Then
                For Each key As String In session.Keys
                    result.Add(String.Format(CultureInfo.InvariantCulture, "Session.{0}", key), session(key))
                Next
            End If

            Return result
        End Function

        ''' <summary>
        ''' Recupera las variable del querystring que este disponibles en el contexto.
        ''' </summary>
        ''' <returns>Diccionario con todas la variables del querystring.</returns>
        Public Shared Function QueryStringToDictionary() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)
            Dim nZone As Boolean = False
            Dim nMainAction As Boolean = False
            Dim nAction As Boolean = False
            Dim Action As Boolean = False

            For Each key As String In HttpContext.Current.Request.QueryString.AllKeys
                If key.IsNotEmpty Then
                    result.Add(key, HttpContext.Current.Request.QueryString(key))

                    If key.Equals("nZone", StringComparison.CurrentCultureIgnoreCase) Then
                        nZone = True
                    ElseIf key.Equals("nAction", StringComparison.CurrentCultureIgnoreCase) Then
                        nAction = True
                    ElseIf key.Equals("nMainAction", StringComparison.CurrentCultureIgnoreCase) Then
                        nMainAction = True
                    ElseIf key.Equals("Action", StringComparison.CurrentCultureIgnoreCase) Then
                        Action = True
                    End If
                End If
            Next
            If Not nZone Then
                result.Add("nZone", "1")
            End If
            If Not nAction Then
                result.Add("nAction", "0")
            End If
            If Not nMainAction Then
                result.Add("nMainAction", "0")
            End If
            If Not Action Then
                result.Add("Action", "")
            End If
            Return result
        End Function

        ''' <summary>
        ''' Recupera las variable del form que este disponibles en el contexto.
        ''' </summary>
        ''' <returns>Diccionario con todas la variables del form.</returns>
        Public Shared Function FormToDictionary() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)
            For Each key As String In HttpContext.Current.Request.Form.AllKeys
                result.Add(key, HttpContext.Current.Request.Form(key))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Recupera las variable de la sesión que este disponibles en el contexto.
        ''' </summary>
        ''' <returns>Diccionario con todas la variables de la sesión.</returns>
        Public Shared Function SessionToDictionary() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)
            For Each key As String In HttpContext.Current.Session.Keys
                result.Add(key, HttpContext.Current.Session(key))
            Next
            Return result
        End Function

    End Class

End Namespace