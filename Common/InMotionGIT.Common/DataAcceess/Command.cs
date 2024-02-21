using System.Data;
using System.Data.SqlClient;

namespace InMotionGIT.Common.DataAccess
{

    public class Command : Interfaces.ICommand
    {

        // Fields
        internal string connectionName;

        private SqlConnection sqlConnection;

        // Methods
        internal Command()
        {
        }

        internal Command(SqlConnection connection)
        {
            sqlConnection = connection;
        }

        internal Command(string connectionName)
        {
            this.connectionName = connectionName;
        }

        private Interfaces.ICommandExecution GetCommand(CommandType commandType, string commandText)
        {
            CommandExecution ce;
            if (sqlConnection is not null)
            {
                ce = new CommandExecution(sqlConnection);
            }
            else
            {
                ce = new CommandExecution(connectionName);
            }
            ce.CommandType = commandType;
            ce.CommandText = commandText;
            return ce;
        }

        public Interfaces.ICommandExecution SqlCommand(string query)
        {
            return GetCommand(CommandType.Text, query);
        }

        public Interfaces.ICommandExecution StoredProc(string storedProcName)
        {
            return GetCommand(CommandType.StoredProcedure, storedProcName);
        }

        public static Interfaces.ICommand WithConnection(string connectionName)
        {
            return new Command(connectionName);
        }

        public static Interfaces.ICommand WithLiveConnection(SqlConnection connection)
        {
            return new Command(connection);
        }

    }

}

// Public Class Command

// Private Shared Function GetCommand(ByVal commandType As CommandType, ByVal commandText As String) As ICommandExecution
// Return New CommandExecution With { _
// .CommandType = commandType,
// .CommandText = commandText
// }
// End Function

// Public Shared Function SqlCommand(ByVal query As String) As ICommandExecution
// Return Command.GetCommand(CommandType.Text, query)
// End Function

// Public Shared Function StoredProc(ByVal storedProcName As String) As ICommandExecution
// Return Command.GetCommand(CommandType.StoredProcedure, storedProcName)
// End Function

// Public Shared Function WithConnection(ByVal connectionName As String) As ICommand
// Return New _AbsDBCommand(connectionName)
// End Function

// Public Shared Function WithLiveConnection(ByVal connection As SqlConnection) As ICommand
// Return New _AbsDBCommand(connection)
// End Function

// End Class