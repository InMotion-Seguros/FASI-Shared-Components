Namespace DataAccess.Interfaces

    Public Interface ICommand

        Function SqlCommand(ByVal query As String) As ICommandExecution

        Function StoredProc(ByVal storedProcName As String) As ICommandExecution

    End Interface

End Namespace