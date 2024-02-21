using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InMotionGIT.Common.DataAccess
{

    internal class CommandExecution : Interfaces.ICommandExecution
    {

        private string _commandText;
        private CommandType _commandType;
        private SqlConnection _sqlConnection;

        private string connectionName;
        private bool isLive;
        private List<SqlParameter> parameters;

        private bool useDefaultConnection;

        internal string CommandText
        {
            set
            {
                _commandText = value;
            }
        }

        internal CommandType CommandType
        {
            set
            {
                _commandType = value;
            }
        }

        internal SqlConnection SqlConnection
        {
            set
            {
                _sqlConnection = value;
            }
        }

        internal CommandExecution()
        {
            parameters = new List<SqlParameter>();
            useDefaultConnection = true;
        }

        internal CommandExecution(SqlConnection conn)
        {
            parameters = new List<SqlParameter>();
            SqlConnection = conn;
            isLive = true;
        }

        internal CommandExecution(string connName)
        {
            parameters = new List<SqlParameter>();
            useDefaultConnection = string.IsNullOrEmpty(connName);
            connectionName = connName;
        }

        public DataTable AsDataTable()
        {
            var result = new DataTable();
            ExecutionWrapper((comm) =>
            {
                var adp = new SqlDataAdapter(comm);
                adp.Fill(result);
            });
            return result;
        }

        public Hashtable AsHashTable()
        {
            Hashtable result = null;
            ExecutionWrapper((comm) => { using (IDataReader reader = comm.ExecuteReader()) { if (reader.Read()) { result = new Hashtable(reader.FieldCount); int col; var loopTo = reader.FieldCount - 1; for (col = 0; col <= loopTo; col++) { string colName = reader.GetName(col); var value = reader.GetValue(col); if (ReferenceEquals(value, DBNull.Value)) { value = null; } result.Add(colName, value); } } } });
            return result is null ? new Hashtable() : result;
        }

        public List<T> AsList<T>()
        {
            var result = new List<T>();
            ExecutionWrapper((comm) => { using (IDataReader reader = comm.ExecuteReader()) { while (reader.Read()) { var value = reader[0]; if (ReferenceEquals(value, DBNull.Value)) { value = null; } result.Add((T)value); } } });
            return result;
        }

        public List<T> AsList<T>(ModelMapper<T> mapper)
        {
            var result = new List<T>();
            ExecutionWrapper((comm) => { using (IDataReader reader = comm.ExecuteReader()) { while (reader.Read()) result.Add(mapper.Map(reader)); } });
            return result;
        }

        public void ExecuteNonQuery()
        {
            ExecutionWrapper((comm) => comm.ExecuteNonQuery());
        }

        public void ExecuteReader(Action<IDataReader> action)
        {
            ExecutionWrapper((comm) => { using (IDataReader reader = comm.ExecuteReader()) { while (reader.Read()) action.Invoke(reader); } });
        }

        public T ExecuteReaderSingle<T>(ModelMapper<T> mapper) where T : class, new()
        {
            T model = default;
            ExecutionWrapper((comm) => { using (IDataReader r = comm.ExecuteReader()) { if (r.Read()) { model = mapper.Map(r); } } });
            return model;
        }

        public void ExecuteReaderSingle(Action<IDataReader> action)
        {
            ExecutionWrapper((comm) => { using (IDataReader reader = comm.ExecuteReader()) { if (reader.Read()) { action.Invoke(reader); } } });
        }

        public T ExecuteScalar<T>()
        {
            T result = default;
            ExecutionWrapper((comm) =>
            {
                var r = comm.ExecuteScalar();
                if (ReferenceEquals(r, DBNull.Value))
                {
                    r = null;
                }
                result = (T)r;
            });
            return result;
        }

        public T ExecuteScalar<T>(T defaultValue)
        {
            T result = default;
            ExecutionWrapper((comm) =>
            {
                var r = comm.ExecuteScalar();
                if (r is null || ReferenceEquals(r, DBNull.Value))
                {
                    r = defaultValue;
                }
                result = (T)r;
            });
            return result;
        }

        private void ExecutionWrapper(Action<SqlCommand> commandAction)
        {
            var conn = GetSqlConnection();
            try
            {
                using (var comm = new SqlCommand(_commandText, conn))
                {
                    comm.CommandType = _commandType;
                    comm.CommandTimeout = Connection.DefaultTimeOut;
                    foreach (var p in parameters)
                        comm.Parameters.Add(p);
                    if (!isLive)
                    {
                        conn.Open();
                    }
                    commandAction.Invoke(comm);
                }
            }
            finally
            {
                if (!(isLive || conn is null))
                {
                    conn.Dispose();
                }
            }
        }

        private SqlConnection GetSqlConnection()
        {
            return !(_sqlConnection == null) ? _sqlConnection : useDefaultConnection ? Connection.GetDefaultConnection() : Connection.GetNamedConnection(connectionName);
        }

        public Interfaces.ICommandExecution WithParam<T>(string paramName, T paramValue)
        {
            parameters.Add(new SqlParameter(paramName, paramValue));
            return this;
        }

        public Interfaces.ICommandExecution WithParam(string paramName, object paramValue)
        {
            parameters.Add(new SqlParameter(paramName, paramValue));
            return this;
        }

        public Interfaces.ICommandExecution WithParam<T>(string paramName, SqlDbType dbType, T paramValue)
        {
            var p = new SqlParameter(paramName, dbType) { Value = paramValue };
            parameters.Add(p);
            return this;
        }

    }

}