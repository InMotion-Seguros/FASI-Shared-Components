Imports System.Data.SqlClient

Namespace DataAccess

    Public Class Command
        Implements Interfaces.ICommand

        ' Fields
        Friend connectionName As String

        Private sqlConnection As SqlConnection

        ' Methods
        Friend Sub New()
        End Sub

        Friend Sub New(ByVal connection As SqlConnection)
            Me.sqlConnection = connection
        End Sub

        Friend Sub New(ByVal connectionName As String)
            Me.connectionName = connectionName
        End Sub

        Private Function GetCommand(ByVal commandType As CommandType, ByVal commandText As String) As Interfaces.ICommandExecution
            Dim ce As CommandExecution
            If (Not Me.sqlConnection Is Nothing) Then
                ce = New CommandExecution(Me.sqlConnection)
            Else
                ce = New CommandExecution(Me.connectionName)
            End If
            ce.CommandType = commandType
            ce.CommandText = commandText
            Return ce
        End Function

        Public Function SqlCommand(ByVal query As String) As Interfaces.ICommandExecution Implements Interfaces.ICommand.SqlCommand
            Return Me.GetCommand(CommandType.Text, query)
        End Function

        Public Function StoredProc(ByVal storedProcName As String) As Interfaces.ICommandExecution Implements Interfaces.ICommand.StoredProc
            Return Me.GetCommand(CommandType.StoredProcedure, storedProcName)
        End Function

        Public Shared Function WithConnection(ByVal connectionName As String) As Interfaces.ICommand
            Return New Command(connectionName)
        End Function

        Public Shared Function WithLiveConnection(ByVal connection As SqlConnection) As Interfaces.ICommand
            Return New Command(connection)
        End Function

    End Class

End Namespace

'Public Class Command

'    Private Shared Function GetCommand(ByVal commandType As CommandType, ByVal commandText As String) As ICommandExecution
'        Return New CommandExecution With { _
'            .CommandType = commandType,
'            .CommandText = commandText
'        }
'    End Function

'    Public Shared Function SqlCommand(ByVal query As String) As ICommandExecution
'        Return Command.GetCommand(CommandType.Text, query)
'    End Function

'    Public Shared Function StoredProc(ByVal storedProcName As String) As ICommandExecution
'        Return Command.GetCommand(CommandType.StoredProcedure, storedProcName)
'    End Function

'    Public Shared Function WithConnection(ByVal connectionName As String) As ICommand
'        Return New _AbsDBCommand(connectionName)
'    End Function

'    Public Shared Function WithLiveConnection(ByVal connection As SqlConnection) As ICommand
'        Return New _AbsDBCommand(connection)
'    End Function

'End Class