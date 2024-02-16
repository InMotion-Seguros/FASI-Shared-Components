Imports System.IO
Imports System.Security.Cryptography

Namespace Helpers

    Public Class MD5Helper

        Public Shared Function CheckSum(path As String) As String
            Dim result As String = ""
            Try
                Dim manager = MD5.Create()
                Dim content = File.OpenRead(path)
                result = BitConverter.ToString(manager.ComputeHash(content)).Replace("-", String.Empty)
                content.Close()
            Catch ex As Exception

            End Try

            Return result
        End Function

    End Class

End Namespace