Imports System.IO

Namespace Helpers

    Public Class Directory

        Public Shared Function GetPathRoot() As String
            Dim result As String
            result = String.Format("{0}", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location))
            Return result
        End Function

    End Class

End Namespace