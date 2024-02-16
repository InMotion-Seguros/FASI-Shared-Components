Imports System.Configuration
Imports System.Globalization

Namespace Helpers

    Public Class QuickData

        'Public Shared Function ConvertQuotes(value As String) As String
        '    Return value.Replace("'", "''")
        'End Function

        Public Shared Function QueryExecute(query As String, dbConnection As System.Data.Common.DbConnection) As DataTable
            Using DataAccess As New Data()
                Dim result As DataTable
                With DataAccess
                    result = DataAccess.QueryExecute(query, dbConnection)
                End With
                Return result
            End Using
        End Function

        Public Shared Function QueryExecute(ByVal query As String, ByVal connectionStringName As String) As DataTable
            Using DataAccess As New Data()
                Dim result As DataTable
                With DataAccess
                    result = DataAccess.QueryExecute(query, connectionStringName)
                End With
                Return result
            End Using
        End Function

        Public Shared Function QueryScalar(Of T)(ByVal query As String, ByVal connectionStringName As String) As T
            Using DataAccess As New Data()
                Dim result As T
                With DataAccess
                    result = DataAccess.QueryScalar(Of T)(query, connectionStringName)
                End With
                Return result
            End Using
        End Function

        Public Shared Function CommandExecute(ByVal command As String, ByVal connectionStringName As String) As Long
            Using DataAccess As New Data()
                Dim result As Long
                With DataAccess
                    result = DataAccess.CommandExecute(command, connectionStringName)
                End With
                Return result
            End Using
        End Function

        'TODO: validar este caso con el provider de microsoft.
        Public Shared Function DbProviderParameterPrefix(parameterName As String, providerName As String) As String
            Dim result As String = String.Empty
            Select Case providerName.ToLower
                Case "oracle.dataaccess.client"
                    result = String.Format(":{0}", parameterName)
                Case Else
                    result = String.Format("@{0}", parameterName)
            End Select

            Return result
        End Function

        Public Shared Function DateFormat(ByVal repositoryName As String) As String
            Return ConfigurationManager.AppSettings(String.Format("{0}.DateFormat", repositoryName))
        End Function

        Public Shared Function ValueDateFormat(ByVal repositoryName As String, value As Date) As String
            Dim result As String = value.ToString(QuickData.DateFormat(repositoryName), New CultureInfo("en-US"))
            'result = result.Replace("/mm/", "/")
            Return result
        End Function

    End Class

End Namespace