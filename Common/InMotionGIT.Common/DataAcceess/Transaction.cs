using System;
using System.Data;
using System.Data.SqlClient;

namespace InMotionGIT.Common.DataAccess
{

    public class Transaction : Interfaces.ITransaction, IDisposable
    {

        // Fields
        private SqlTransaction _transaction = new SqlConnection().BeginTransaction();

        // Methods
        private Transaction()
        {
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public static Interfaces.ITransaction FromDefault()
        {
            return new Transaction();
        }

        public static Interfaces.ITransaction FromNamedConnection(string connectionName)
        {
            return new Transaction();
        }

        private static Interfaces.ICommandExecution GetCommand(CommandType commandType, string commandText)
        {
            return new CommandExecution()
            {
                CommandType = commandType,
                CommandText = commandText
            };
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public Interfaces.ICommandExecution SqlCommand(string query)
        {
            return GetCommand(CommandType.Text, query);
        }

        public Interfaces.ICommandExecution StoredProc(string storedProcName)
        {
            return GetCommand(CommandType.StoredProcedure, storedProcName);
        }

    }

}