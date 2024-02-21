Imports System.Data.SqlClient

Namespace DataAccess

    Public Class Transaction
        Implements Interfaces.ITransaction, IDisposable

        ' Fields
        Private _transaction As SqlTransaction = New SqlConnection().BeginTransaction

        ' Methods
        Private Sub New()
        End Sub

        Public Sub Commit() Implements Interfaces.ITransaction.Commit
            Me._transaction.Commit()
        End Sub

        Public Sub Dispose() Implements System.IDisposable.Dispose
            Throw New NotImplementedException
        End Sub

        Public Shared Function FromDefault() As Interfaces.ITransaction
            Return New Transaction
        End Function

        Public Shared Function FromNamedConnection(ByVal connectionName As String) As Interfaces.ITransaction
            Return New Transaction
        End Function

        Private Shared Function GetCommand(ByVal commandType As CommandType, ByVal commandText As String) As Interfaces.ICommandExecution
            Return New CommandExecution With {
                .CommandType = commandType,
                .CommandText = commandText
                }
        End Function

        Public Sub Rollback() Implements Interfaces.ITransaction.Rollback
            Me._transaction.Rollback()
        End Sub

        Public Function SqlCommand(ByVal query As String) As Interfaces.ICommandExecution Implements Interfaces.ITransaction.SqlCommand
            Return Transaction.GetCommand(CommandType.Text, query)
        End Function

        Public Function StoredProc(ByVal storedProcName As String) As Interfaces.ICommandExecution Implements Interfaces.ITransaction.StoredProc
            Return Transaction.GetCommand(CommandType.StoredProcedure, storedProcName)
        End Function

    End Class

End Namespace