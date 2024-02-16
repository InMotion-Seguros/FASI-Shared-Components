Imports System.Configuration
Imports System.Data.SqlClient

Namespace DataAccess

    Public Class Connection
        Implements IDisposable

        Private Shared ReadOnly connectionStrings As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Private Shared defaultConnectionString As String
        Private Shared _defaultTimeOut As Integer

        Public Shared ReadOnly Property DefaultTimeOut As Integer
            Get
                Return Connection._defaultTimeOut
            End Get
        End Property

        Public Shared Sub AddNamedConnectionString(ByVal connStringName As String)
            If (ConfigurationManager.ConnectionStrings.Item(connStringName) Is Nothing) Then
                Throw New ArgumentException(String.Format("'{0}' not found on configuration", connStringName))
            End If
            Dim connStringValue As String = ConfigurationManager.ConnectionStrings.Item(connStringName).ConnectionString
            Connection.connectionStrings.Add(connStringName, connStringValue)
        End Sub

        Public Shared Function GetAbsDBCommandFor(ByVal connectionName As String) As Interfaces.ICommand
            Return Command.WithConnection(connectionName)
        End Function

        Public Shared Function GetAbsDBCommandForDefault() As Interfaces.ICommand
            Return New Command
        End Function

        Private Shared Function GetDBConnection() As Connection
            Return New Connection
        End Function

        Friend Shared Function GetDefaultConnection() As SqlConnection
            Return New SqlConnection(Connection.defaultConnectionString)
        End Function

        Friend Shared Function GetNamedConnection(ByVal name As String) As SqlConnection
            If Not Connection.connectionStrings.ContainsKey(name) Then
                Throw New Exception(String.Format("named connection [{0}] has not been registered", name))
            End If
            Return New SqlConnection(Connection.connectionStrings.Item(name))
        End Function

        Public Shared Sub SetDefaultConnectionString(ByVal connStringName As String)
            If (ConfigurationManager.ConnectionStrings.Item(connStringName) Is Nothing) Then
                Throw New ArgumentException(String.Format("'{0}' not found on configuration", connStringName))
            End If
            Connection.defaultConnectionString = ConfigurationManager.ConnectionStrings.Item(connStringName).ConnectionString
        End Sub

        Public Shared Sub SetDefaultConnectionStringFromString(ByVal connectionString As String)
            Connection.defaultConnectionString = connectionString
        End Sub

        Public Shared Sub SetDefaultTimeOut(ByVal timeOut As Integer)
            Connection._defaultTimeOut = timeOut
        End Sub

#Region "IDisposable Support"

        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

    End Class

End Namespace