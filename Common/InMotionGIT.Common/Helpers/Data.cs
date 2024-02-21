using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    public class Data : IDisposable
    {

        #region Private fields, to hold the state of the entity

        private DbProviderFactory _dbProviderFactory;
        private DbConnection _dbConnection;

        private string _owner;
        private string _sysdate;

        #endregion

        #region Public properties

        public string ProviderName { get; set; }
        public string DateFormat { get; set; }

        #endregion

        #region  IDisposable Support 

        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Release();
                }
                if (!(_dbConnection == null))
                {
                    _dbConnection.Close();
                    _dbConnection = null;
                }
                _dbProviderFactory = null;
                // TODO: free your own state (unmanaged objects).
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Constructors Methods

        public Data() : base()
        {
        }

        public Data(string connectionStringName)
        {
            OpenConnection(connectionStringName);
        }

        public Data(string connectionString, string provider, string owner)
        {
            _owner = owner;
            OpenConnectionString(connectionString, provider);
        }

        #endregion

        #region Connection Methods

        public static string RealConnectionStringName(string repositoryName)
        {
            var ConnectionSetting = ConfigurationManager.ConnectionStrings[repositoryName];

            if (ConnectionSetting == null)
            {
                ConnectionSetting = ConfigurationManager.ConnectionStrings[string.Format("Linked.{0}", repositoryName)];
            }

            if (ConnectionSetting == null)
            {
                ConnectionSetting = ConfigurationManager.ConnectionStrings["BackOfficeConnectionString"];
            }

            if (ConnectionSetting == null)
            {
                throw new Exceptions.InMotionGITException("the connection setting for database not found");
            }

            return ConnectionSetting.Name;
        }

        public static ConnectionStringSettings GetConnectionString(string repositoryName)
        {
            var ConnectionSetting = ConfigurationManager.ConnectionStrings[repositoryName];

            if (ConnectionSetting == null)
            {
                ConnectionSetting = ConfigurationManager.ConnectionStrings[string.Format("Linked.{0}", repositoryName)];
            }

            if (ConnectionSetting == null)
            {
                ConnectionSetting = ConfigurationManager.ConnectionStrings["BackOffice"];

                if (ConnectionSetting == null & Debugger.IsAttached)
                {
                    ConnectionSetting = ConfigurationManager.OpenMachineConfiguration().ConnectionStrings.ConnectionStrings["BackOffice"];
                }
            }

            if (ConnectionSetting == null)
            {
                throw new Exception("the connection setting for database not found");
            }

            return ConnectionSetting;
        }

        private bool OpenConnection(string connectionStringName)
        {
            var ConnectionSetting = GetConnectionString(connectionStringName);
            bool result = OpenConnectionString(ConnectionSetting.ConnectionString, ConnectionSetting.ProviderName);

            if (result)
            {
                _owner = "{0}.Owner".SpecialFormater(ConnectionSetting.Name).AppSettings();
                DateFormat = "{0}.DateFormat".SpecialFormater(ConnectionSetting.Name).AppSettings();
                if (DateFormat.IsEmpty())
                {
                    DateFormat = "MM/dd/yyyy";
                }
                if (ConnectionSetting.ProviderName.ToLower() == "system.data.sqlclient")
                {
                    _sysdate = "GETDATE()";
                }
                else
                {
                    _sysdate = "SYSDATE";
                }
            }
            return result;
        }

        private bool OpenConnectionString(string connectionString, string provider)
        {
            bool result = true;
            ProviderName = provider;
            try
            {
                _dbProviderFactory = DbProviderFactories.GetFactory(provider);
            }
            catch (Exception ex)
            {
                throw new Exception("the database .net provider may not be installed", ex);
                result = false;
            }

            if (!(_dbProviderFactory == null))
            {
                try
                {
                    _dbConnection = _dbProviderFactory.CreateConnection();
                    _dbConnection.ConnectionString = connectionString;
                    _dbConnection.Open();
                }

                // TODO: Se deben tipificar los error bajo GIT Exception usando un codificación para alinear los errores
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exception("Oracle TNS could not resolve the connect identifier specified", ex);
                    }

                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exception("the username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exception("invalid username/password", ex);
                    }

                    switch (ex.Number)
                    {
                        case 53:
                            {
                                throw new Exception("the server was not found or was not accessible", ex);
                            }

                        case 18456:
                            {
                                throw new Exception("the username/password is invalid", ex);
                            }

                        case 4060:
                            {
                                throw new Exception("the initial catalog/username is invalid", ex);
                            }

                        default:
                            {
                                if (ex.ErrorCode == -2146232060 && ex.Message.StartsWith("A network-related or instance-specific error occurred"))
                                {
                                    throw new Exception("the server was not found or was not accessible", ex);
                                }
                                else
                                {
                                    throw new Exception("we have a technical problem", ex);
                                }
                            }
                    }
                    result = false;
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exception("Oracle TNS could not resolve the connect identifier specified", ex);
                    }

                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exception("the username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exception("we have a technical problem", ex);
                    }

                    result = false;
                }
            }

            return result;
        }

        public static bool TestConnection(string DataSource, string catalog, string user, string password, string provider)
        {
            bool result = true;
            string connectionString = string.Format("Data Source={0};user id={1};password={2}", DataSource, user, password);
            if (provider == "System.Data.SqlClient")
            {
                connectionString += string.Format(";Initial Catalog={0}", catalog);
            }
            using (var DataAccess = new Data())
            {
                result = DataAccess.OpenConnectionString(connectionString, provider);
                DataAccess.Release();
            }
            return result;
        }

        public static bool TestConnection(string connectionStringName)
        {
            var ConnectionSetting = GetConnectionString(connectionStringName);
            bool result = false;

            if (!(ConnectionSetting == null))
            {
                using (var DataAccess = new Data())
                {
                    result = DataAccess.OpenConnectionString(ConnectionSetting.ConnectionString, ConnectionSetting.ProviderName);
                    DataAccess.Release();
                }
            }

            return result;
        }

        public void Release()
        {
            if (!(_dbConnection == null))
            {
                _dbConnection.Close();
                _dbConnection = null;
            }
            _dbProviderFactory = null;
        }

        /// <summary>
        /// Create a new parameter of RefCursor type
        /// </summary>
        /// <param name="parameterInstance"></param>
        /// <returns>DbParameter with DbType RefCursor</returns>
        /// <remarks>
        /// Usage: command.Parameters.Add(CreateRefCursorParameter(dbProviderFactory.CreateParameter()))
        /// </remarks>
        private static DbParameter CreateRefCursorParameter(DbParameter parameterInstance)
        {
            var parameterType = parameterInstance.GetType();
            var oracleDbType = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType");
            DbParameter refCursorParameter = (DbParameter) Activator.CreateInstance(parameterType, new object[] { "RC1", Enum.Parse(oracleDbType, "RefCursor") });
            refCursorParameter.Direction = ParameterDirection.Output;

            return (DbParameter)refCursorParameter;
        }

        #endregion

        #region Execution Methods

        internal DataTable QueryExecute(string query, DbConnection dbConnection)
        {
            _dbConnection = dbConnection;

            if (_dbConnection == null)
            {
                return null;
            }
            else
            {
                return QueryExecute(query);
            }
        }

        public DataTable QueryExecute(string query, string connectionStringName)
        {
            if (_dbConnection.IsEmpty() || _dbConnection.State != ConnectionState.Open)
            {
                OpenConnection(connectionStringName);
            }
            if (_dbConnection == null)
            {
                return null;
            }
            else
            {
                return QueryExecute(query);
            }
        }

        public DataTable QueryExecute(string query)
        {
            DbCommand dbcmd = null;
            DbDataAdapter vloDataAdapter = null;
            DataTable result = null;

            dbcmd = _dbProviderFactory.CreateCommand();
            dbcmd.Connection = _dbConnection;
            dbcmd.CommandText = PreprocessStatement(query);

            vloDataAdapter = _dbProviderFactory.CreateDataAdapter();

            result = new DataTable();
            vloDataAdapter.SelectCommand = dbcmd;

            vloDataAdapter.Fill(result);

            return result;
        }

        public T QueryScalar<T>(string query, string repositoryName)
        {
            DbCommand dbcmd = null;
            var result = default(T);

            if (_dbConnection.IsEmpty() || _dbConnection.State != ConnectionState.Open)
            {
                OpenConnection(repositoryName);
            }

            if (!(_dbConnection == null))
            {

                dbcmd = _dbProviderFactory.CreateCommand();
                dbcmd.Connection = _dbConnection;
                dbcmd.CommandText = PreprocessStatement(query);

                result = Conversions.ToGenericParameter<T>(dbcmd.ExecuteScalar());
            }

            return result;
        }

        /// <summary>
        /// Execute the queries
        /// </summary>
        /// <param name="command">Query to execute</param>
        public long CommandExecute(string command)
        {
            return CommandExecute(command, string.Empty);
        }

        /// <summary>
        /// Execute the queries
        /// </summary>
        /// <param name="command">Query to execute</param>
        /// <param name="connectionStringName">Name of the input of ConnectionStrings</param>
        public long CommandExecute(string command, string connectionStringName)
        {
            DbCommand dbcmd = null;
            long rowAffected = 0L;

            if (_dbConnection.IsEmpty() || _dbConnection.State != ConnectionState.Open)
            {
                OpenConnection(connectionStringName);
            }

            if (!(_dbConnection == null))
            {
                dbcmd = _dbProviderFactory.CreateCommand();

                dbcmd.Connection = _dbConnection;
                dbcmd.CommandText = PreprocessStatement(command);
                rowAffected = dbcmd.ExecuteNonQuery();
            }
            return rowAffected;
        }

        public Y QueryExecuteWithMap<T, Y>(string query, string connectionStringName)
            where T : new()
            where Y : new()
        {
            return MapDataToBusinessEntityCollection<T, Y>(QueryExecute(query, connectionStringName).CreateDataReader());
        }

        public DataTable QueryExecute(string statement, Dictionary<string, object> parameters)
        {
            DataTable result;

            if (parameters.IsNotEmpty() && parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> item in parameters)
                {
                    if (string.Equals(item.Key, "[RECORDEFFECTIVEDATE]", StringComparison.CurrentCultureIgnoreCase) || item.Key.EndsWith(":D}"))
                    {
                        statement = statement.Replace(item.Key, DateTime.Parse(Conversions.ToString(item.Value)).ToString(DateFormat));
                    }
                    else
                    {
                        statement = statement.Replace(item.Key, Conversions.ToString(item.Value));
                    }
                }
            }
            result = QueryExecute(PreprocessStatement(statement));
            return result;
        }

        #endregion

        #region Mapping Data Methods

        public static List<T> MapDataToBusinessEntityCollection<T>(DataTable dr) where T : new()
        {
            var businessEntityType = typeof(T);
            var entitys = new List<T>();
            var hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            PropertyInfo info;
            T newObject;

            foreach (var currentInfo in properties)
            {
                info = currentInfo;
                hashtable[info.Name.ToUpper()] = info;
            }

            foreach (DataRow row in dr.Rows)
            {
                newObject = new T();
                foreach (DataColumn column in dr.Columns)
                {
                    if (!(row[column.ColumnName] is DBNull))
                    {
                        info = (PropertyInfo)hashtable[column.ColumnName.ToUpper()];
                        if (info is not null && info.CanWrite)
                        {
                            info.SetValue(newObject, row[column.ColumnName], null);
                        }
                    }
                }

                entitys.Add(newObject);
            }
            return entitys;
        }

        public static List<T> MapDataToBusinessEntityCollection<T>(IDataReader dr) where T : new()
        {
            var businessEntityType = typeof(T);
            var entitys = new List<T>();
            var hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            PropertyInfo info;
            T newObject;

            foreach (var currentInfo in properties)
            {
                info = currentInfo;
                hashtable[info.Name.ToUpper()] = info;
            }

            while (dr.Read())
            {
                newObject = new T();

                for (int index = 0, loopTo = dr.FieldCount - 1; index <= loopTo; index++)
                {
                    if (!dr.IsDBNull(index))
                    {
                        info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                        if (info is not null && info.CanWrite)
                        {
                            info.SetValue(newObject, dr.GetValue(index), null);
                        }
                    }
                }
                entitys.Add(newObject);
            }
            dr.Close();
            return entitys;
        }

        public static Y MapDataToBusinessEntityCollection<T, Y>(IDataReader dr)
            where T : new()
            where Y : new()
        {
            var businessEntityType = typeof(T);
            var entitys = new Y();
            IList ientitys = (IList)entitys;
            var hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            PropertyInfo info;
            T newObject;

            foreach (var currentInfo in properties)
            {
                info = currentInfo;
                hashtable[info.Name.ToUpper()] = info;
            }

            while (dr.Read())
            {
                newObject = new T();

                for (int index = 0, loopTo = dr.FieldCount - 1; index <= loopTo; index++)
                {
                    info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                    if (info is not null && info.CanWrite && !dr.IsDBNull(index))
                    {

                        if (dr.GetValue(index).GetType().ToString() == "System.Decimal" && info.PropertyType.ToString() == "System.Int32")
                        {
                            info.SetValue(newObject, Convert.ToInt32(dr.GetValue(index)), null);
                        }
                        else if (dr.GetValue(index).GetType().ToString() == "System.Int32" && info.PropertyType.ToString() == "System.Decimal")
                        {
                            info.SetValue(newObject, Convert.ToDecimal(dr.GetValue(index)), null);
                        }
                        else
                        {
                            info.SetValue(newObject, dr.GetValue(index), null);
                        }

                    }
                }
                ientitys.Add(newObject);
            }
            dr.Close();
            return entitys;
        }

        #endregion

        #region Replace Data Methods

        private string PreprocessStatement(string statement)
        {
            if (ProviderName.ToLower() == "system.data.sqlclient")
            {
                statement = statement.Replace("SYSDATE", "GETDATE()");
                statement = statement.Replace("@:", "@");
            }
            // statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
            // statement = statement.Replace(" ISNULL(", " ISNULL(")
            else
            {
                // statement = statement.Replace("SYSDATE", "SYSDATE")
                statement = statement.Replace("@:", ":");
                statement = statement.Replace(" (NOLOCK)", "");
                statement = statement.Replace(" ISNULL(", " NVL(");
            }
            statement = statement.Replace("{OWNER}", _owner);
            return statement;
        }

        // TODO: por eliminar
        public static string PreprocessStatement(string statement, string providerName)
        {
            if (providerName.ToLower() == "system.data.sqlclient")
            {
                statement = statement.Replace("SYSDATE", "GETDATE()");
                statement = statement.Replace("@:", "@");
            }
            // statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
            // statement = statement.Replace(" ISNULL(", " ISNULL(")
            else
            {
                // statement = statement.Replace("SYSDATE", "SYSDATE")
                statement = statement.Replace("@:", ":");
                statement = statement.Replace(" (NOLOCK)", "");
                statement = statement.Replace(" ISNULL(", " NVL(");
            }
            return statement;
        }

        public string ValueDateFormat(string repositoryName, DateTime value)
        {
            return value.ToString(Conversions.ToString(DateFormat[Conversions.ToInteger(repositoryName)]));
        }

        // Public Function DateFormat(ByVal repositoryName As String) As String
        // Return ConfigurationManager.AppSettings(String.Format("{0}.DateFormat", repositoryName))
        // End Function

        #endregion

        #region DataBase Provider Methods

        public string DbProviderParameterPrefix(string parameterName)
        {
            string result = string.Empty;

            switch (ProviderName.ToLower() ?? "")
            {
                case "oracle.dataaccess.client":
                    {
                        result = string.Format(":{0}", parameterName);
                        break;
                    }

                default:
                    {
                        result = string.Format("@{0}", parameterName);
                        break;
                    }
            }

            return result;
        }

        public static string GetDataBaseProvider(string repositoryName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[repositoryName];
            string result = "SQL";

            if (!(connectionString == null))
            {
                switch (connectionString.ProviderName.ToLower() ?? "")
                {
                    case "system.data.sqlclient":
                        {
                            result = "SQL";
                            break;
                        }

                    case "oracle.dataaccess.client":
                    case "system.data.oracleclient":
                        {
                            result = "Oracle";
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            return result;
        }

        #endregion

    }

}