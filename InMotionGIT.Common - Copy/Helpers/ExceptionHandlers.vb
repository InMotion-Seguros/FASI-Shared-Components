Imports System.Configuration
Imports System.IO

Namespace Helpers

    Public Class ExceptionHandlers

        Public Shared Function TraceInnerExceptionMessage(ByVal ex As System.Exception, includeStackInfo As Boolean) As String
            Return TraceInnerExceptionMessage(ex, 0) & vbCrLf & ex.StackTrace
        End Function

        Public Shared Function TraceInnerExceptionMessage(ByVal ex As System.Exception) As String
            Return TraceInnerExceptionMessage(ex, 0)
        End Function

        Public Shared Function TraceInnerExceptionMessage(ByVal ex As System.Exception, ByVal level As Integer) As String
            Dim result As String = String.Empty

            result += ex.Message & vbCrLf
            Trace.IndentLevel = level
            Trace.WriteLine(ex.Message)
            If Not IsNothing(ex.InnerException) Then
                result += TraceInnerExceptionMessage(ex.InnerException, +level)
            End If
            Return result
        End Function

        Public Shared Sub LogInnerException(ByVal key As String, ByVal message As String, ByVal ex As System.Exception)
            Dim result As String = TraceInnerExceptionMessage(ex)

            ErrorLog(key, message, 0)
            ErrorLog(String.Empty, result, 0)
        End Sub

        Public Shared Sub ErrorLog(ByVal key As String, ByVal message As String, ByVal count As Integer)
            ErrorLog(key, message, count, "")
        End Sub

        Public Shared Sub ErrorLog(ByVal key As String, ByVal message As String, ByVal count As Integer, ByVal logFile As String)
            Dim fs As FileStream
            Dim filename As String = String.Empty

            If String.IsNullOrEmpty(logFile) Then
                filename = ConfigurationManager.AppSettings("Path.Logs") & "\" & Date.Now.ToString("yyyyMMdd") & ".log"
            Else
                filename = ConfigurationManager.AppSettings("Path.Logs") & "\" & logFile
            End If
            Try

                If System.IO.File.Exists(filename) Then
                    fs = System.IO.File.Open(filename, FileMode.Append)
                Else
                    fs = System.IO.File.Create(filename)
                End If

                Dim sw As StreamWriter = New StreamWriter(fs)
                Dim thisDate As DateTime = DateTime.Now
                If String.IsNullOrEmpty(key) Then
                    If Not String.IsNullOrEmpty(message) Then
                        sw.WriteLine(message)
                    End If
                Else
                    sw.WriteLine(String.Format("{0} {1}{2} {3}", thisDate.ToString("hh:mm:ss.fff"), "".PadLeft(count, " "), key, message))
                End If

                sw.Close()

                fs.Close()
            Catch ex As Exception

            End Try
        End Sub

    End Class

End Namespace