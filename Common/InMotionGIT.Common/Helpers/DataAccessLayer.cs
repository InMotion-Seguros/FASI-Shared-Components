using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel.PeerResolvers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml.Linq; 
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Oracle.DataAccess.Client;

namespace InMotionGIT.Common.Helpers
{
    public class DataAccessLayer : IDisposable
    {
        #region Private Members

        private DbProviderFactory _dbProviderFactory = null;
        private DbConnection _dbConnection = null;

        #endregion Private Members

        #region Public properties

        public string ProviderName { get; set; }
        // Public Property DateFormat As String

        #endregion Public properties

        #region Public Members

        public void Release()
        {
            if (!(_dbConnection == null))
            {
                CloseDbConnection(_dbConnection);
            }
            _dbProviderFactory = null;
        }

        public DataTable ExecuteQuery(string query, string connectionStringName, int companyId)
        {
            DataTable result = null;

            if (_dbConnection == null)
            {
                _dbConnection = OpenDbConnection(connectionStringName);
            }

            if (!(_dbConnection == null))
            {
                var commandItem = _dbConnection.CreateCommand();
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = query;
                var dataReaderItem = QueryExecute(commandItem, _dbConnection, CommandBehavior.Default, "Dynamic", ref connectionStringName, Conversions.ToBoolean(companyId), null);

                result = new DataTable();
                result.Load(dataReaderItem);
                if (string.IsNullOrEmpty(result.TableName))
                {
                    result.TableName = "Result";
                }
                dataReaderItem.Close();
                dataReaderItem = null;
            }

            return result;
        }

        public int ExecuteQueryScalar(string query, string connectionStringName)
        {
            long result = 0L;
            object internalResult = null;

            if (_dbConnection == null)
            {
                _dbConnection = OpenDbConnection(connectionStringName);
            }

            if (!(_dbConnection == null))
            {
                var commandItem = _dbConnection.CreateCommand();
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = query;
                internalResult = commandItem.ExecuteScalar();
                if (!(internalResult is DBNull))
                {
                    result = Conversions.ToLong(internalResult);
                }
            }

            return (int)result;
        }

        public DataTable ProcedureExecuteWithDataTableResultset(string storedProcedureName, List<DataType.Parameter> parameterList, string connectionStringName)
        {
            DataTable result = null;

            if (_dbConnection == null)
            {
                _dbConnection = OpenDbConnection(connectionStringName);
            }

            if (!(_dbConnection == null))
            {
                var commandItem = _dbConnection.CreateCommand();
                commandItem.CommandType = CommandType.StoredProcedure;
                commandItem.CommandText = PreprocessStatement(storedProcedureName, _dbConnection.ToString());

                foreach (DataType.Parameter item in parameterList)
                    item.CreateCommandParameter(commandItem);

                result = ExecuteWithDataTable(commandItem, _dbConnection, false);
            }

            return result;
        }

        #endregion Public Members

        #region Shared Members

        /// <summary>
        /// Método que consume por los servicios tipo json en su canal
        /// </summary>
        /// <param name="command">Comando a ejecutar</param>
        /// <param name="currentConnection">Connexion actual</param>
        /// <param name="behavior">Ambiente de ejecución</param>
        /// <param name="table">Nombre de la tabla</param>
        /// <param name="returnEmptyDataTable">retornar data-table vació</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DataTable QueryExecuteToTableJSON(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, bool returnEmptyDataTable, Dictionary<string, string> parameters)
        {
            return QueryExecuteToTable(command, currentConnection, behavior, table, returnEmptyDataTable, parameters);
        }

        public static DataTable QueryExecuteToTable(string statement, DbConnection currentConnection, CommandBehavior behavior, string table)
        {
            return QueryExecuteToTable(statement, currentConnection, behavior, table, new Dictionary<string, string>());
        }

        public static DataTable QueryExecuteToTable(string statement, DbConnection currentConnection, CommandBehavior behavior, string table, Dictionary<string, string> parameters = null)
        {
            DataTable result = null;
            var commandItem = currentConnection.CreateCommand();
            commandItem.CommandType = CommandType.Text;
            commandItem.CommandText = PreprocessStatement(statement, currentConnection.ToString());
            var dataReaderItem = QueryExecute(commandItem, currentConnection, behavior, table);
            if (dataReaderItem.HasRows)
            {
                result = new DataTable();
                result.Load(dataReaderItem);
                result.TableName = table;
            }
            dataReaderItem.Close();
            dataReaderItem = null;
            return result;
        }

        public static DataTable QueryExecuteToTable(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, bool returnEmptyDataTable, Dictionary<string, string> parameters)
        {
            DataTable result = null;
            string Message = "";
            int indexStack = Conversions.ToInteger(Interaction.IIf("DataManager.Mode".AppSettingsOnEquals("remote"), 4, 2));

            var dataReaderItem = QueryExecute(command, currentConnection, behavior, table, ref Message, true, parameters);

            if (behavior == CommandBehavior.SchemaOnly)
            {
                result = dataReaderItem.GetSchemaTable();
            }
            else if (dataReaderItem.HasRows || returnEmptyDataTable)
            {
                var watch = new Stopwatch();
                watch.Start();
                result = new DataTable();
                result.Load(dataReaderItem);
                result.TableName = table;
                watch.Stop();
                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    if (parameters.IsNotEmpty())
                    {
                        Message = "{2}{0}{1}{0}{2}{0}{3}".SpecialFormater("              ", "Parámetros:" + parameters.ToStringExtended(), Environment.NewLine, Message);
                    }
                    Message = Message + "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
                    Message = Message + Environment.NewLine + "              Rows={0}".SpecialFormater(result.Rows.Count);
                    if ("DataAccessLayer.Debug.DetailsCall".AppSettings<bool>())
                    {
                        string detailsCall = AssemblyHandler.GetFrameProcess(indexStack);
                        if (detailsCall.IsNotEmpty())
                        {
                            Message += Constants.vbCrLf + string.Format("{0}", detailsCall.ToString().Replace("<<I>>", "              "));
                        }
                    }
                    LogHandler.TraceLog("DataAccessLayer", Message);
                }
            }

            dataReaderItem.Close();
            dataReaderItem = null;
            return result;
        }

        public static DataTable QueryExecuteToTable(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, Dictionary<string, string> parameters)
        {
            return QueryExecuteToTable(command, currentConnection, behavior, table, false, parameters);
        }

        public static string QueryExecuteToTableString(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, Dictionary<string, string> parameters)
        {
            var dTable = QueryExecuteToTable(command, currentConnection, behavior, table, parameters);
            string result = string.Empty;

            if (dTable.IsNotEmpty())
            {
                using (var xmlitem = new System.IO.StringWriter())
                {
                    dTable.WriteXml(xmlitem, XmlWriteMode.WriteSchema);
                    result = xmlitem.ToString();
                }
            }

            command.Dispose();
            command = null;

            return result;
        }

        public static DataTable StringToTable(string stringTable)
        {
            var dTable = new DataTable();
            if (stringTable.IsNotEmpty())
            {
                using (var xmlStream = new System.IO.StringReader(stringTable))
                {
                    dTable.ReadXml(xmlStream);
                }
            }
            return dTable;
        }

        public static T QueryExecuteWithMapper<T>(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table) where T : new()
        {
            DbDataReader result = null;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            try
            {
                watch = new Stopwatch();
                watch.Start();
                result = command.ExecuteReader(behavior);
                watch.Stop();
                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              HasRows={0}", result.HasRows);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
                command.Connection = null;
                command = null;
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }

            var businessEntityType = typeof(T);
            var hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            PropertyInfo info;
            var newObject = default(T);
            object value;
            foreach (var currentInfo in properties)
            {
                info = currentInfo;
                hashtable[info.Name.ToUpper()] = info;
            }
            if (result.Read())
            {
                newObject = new T();

                for (int index = 0, loopTo = result.FieldCount - 1; index <= loopTo; index++)
                {
                    info = (PropertyInfo)hashtable[result.GetName(index).ToUpper()];
                    if (info is not null && info.CanWrite && !result.IsDBNull(index))
                    {
                        value = Convert.ChangeType(result.GetValue(index), info.PropertyType);
                        info.SetValue(newObject, value, null);
                    }
                }
            }
            result.Close();
            result = null;
            return newObject;
        }

        public static Y QueryExecuteWithMapper<T, Y>(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table)
            where T : new()
            where Y : new()
        {
            DbDataReader result = null;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            try
            {
                watch = new Stopwatch();
                watch.Start();
                result = command.ExecuteReader(behavior);

                watch.Stop();

                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              HasRows={0}", result.HasRows);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
                command.Connection = null;
                command = null;
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }

            var businessEntityType = typeof(T);
            var entitys = new Y();
            IList ientitys = (IList)entitys;
            var hashtable = new Hashtable();
            PropertyInfo[] properties = businessEntityType.GetProperties();
            PropertyInfo info;
            T newObject;
            object value;
            foreach (var currentInfo in properties)
            {
                info = currentInfo;
                hashtable[info.Name.ToUpper()] = info;
            }
            while (result.Read())
            {
                newObject = new T();

                for (int index = 0, loopTo = result.FieldCount - 1; index <= loopTo; index++)
                {
                    info = (PropertyInfo)hashtable[result.GetName(index).ToUpper()];
                    if (info is not null && info.CanWrite && !result.IsDBNull(index))
                    {
                        value = Convert.ChangeType(result.GetValue(index), info.PropertyType);
                        info.SetValue(newObject, value, null);
                    }
                }
                ientitys.Add(newObject);
            }
            result.Close();
            return entitys;
        }

        public static DbDataReader QueryExecute(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table)
        {
            string value = string.Empty;
            return QueryExecute(command, currentConnection, behavior, table, ref value, false, new Dictionary<string, string>());
        }

        public static DbDataReader QueryExecute(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, Dictionary<string, string> parameters = null)
        {
            string value = string.Empty;
            return QueryExecute(command, currentConnection, behavior, table, ref value, false, parameters);
        }

        public static DbDataReader QueryExecute(DbCommand command, DbConnection currentConnection, CommandBehavior behavior, string table, ref string Message, bool IsRetrieve, Dictionary<string, string> parameters = null)
        {
            DbDataReader result = null;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            for (int attempts = 1; attempts <= 3; attempts++)
            {
                command.Connection = currentConnection;
                try
                {
                    watch = new Stopwatch();
                    watch.Start();
                    if (currentConnection.ToString().StartsWith("Oracle.DataAccess.Client", StringComparison.CurrentCultureIgnoreCase))
                    {
                        {
                            var withBlock = (OracleCommand)command;
                            withBlock.InitialLONGFetchSize = -1;
                        }
                    }

                    result = command.ExecuteReader(behavior);
                    watch.Stop();

                    if ("DataAccessLayer.Debug".AppSettings<bool>())
                    {
                        string argcommandText = null;
                        Dictionary<string, string> argparameters = null;
                        string messageInternal = MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false);
                        if (parameters.IsNotEmpty())
                        {
                            messageInternal = "{2}{0}{4}{2}{0}{1}{0}{2}{0}{3}".SpecialFormater("              ", "Parámetros:" + parameters.ToStringExtended(), Environment.NewLine, messageInternal, "Mode:" + "DataManager.Mode".AppSettings());
                        }
                        if (!IsRetrieve)
                        {
                            messageInternal += "              HasRows={0}".SpecialFormater(result.HasRows);
                        }
                        messageInternal += Constants.vbCrLf + "              Time Execution={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
                        if ("DataAccessLayer.Debug.DetailsCall".AppSettings<bool>())
                        {
                            string detailsCall = AssemblyHandler.MethodName(MethodBase.GetCurrentMethod().Name.ToLower());
                            if (detailsCall.IsNotEmpty())
                            {
                                messageInternal += Constants.vbCrLf + "{0}".SpecialFormater(detailsCall.ToString().Replace("<<I>>", "              "));
                            }
                        }
                        if (!IsRetrieve)
                        {
                            LogHandler.TraceLog("DataAccessLayer", messageInternal);
                        }
                        else
                        {
                            Message = messageInternal;
                        }
                    }

                    break;
                }
                catch (OracleException exOracle)
                {
                    if ((exOracle.Message.StartsWith("ORA-03135:") || exOracle.Message.StartsWith("ORA-03113:") || exOracle.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 || exOracle.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 || exOracle.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) && attempts < 3)
                    {
                        var magicMethod = currentConnection.GetType().GetMethod("ClearAllPools");

                        for (int connectAttempts = 1; connectAttempts <= 3; connectAttempts++)
                        {
                            command.Connection = null;
                            if (currentConnection.State == ConnectionState.Open)
                            {
                                currentConnection.Close();
                            }
                            if (magicMethod.IsNotEmpty())
                            {
                                magicMethod.Invoke(currentConnection, new object[] { });
                            }
                            LogHandler.WarningLog("DataAccessLayer", string.Format("Retry due to disconnection for query on table '{2}' ({0}/{3}). {1}", attempts, exOracle.Message, table, connectAttempts));
                            Thread.Sleep(1000);
                            try
                            {
                                currentConnection.Open();
                                break;
                            }
                            catch (Exception ex2)
                            {
                                if (connectAttempts >= 3)
                                {
                                    var temporalException = Exceptions.DataAccessException.Factory(exOracle, command, table, "Query");
                                    ConnectionClosed(command, currentConnection);
                                    throw temporalException;
                                }
                            }
                        }
                    }
                    else
                    {
                        command.Connection = null;
                        if (currentConnection.State == ConnectionState.Open)
                        {
                            currentConnection.Close();
                        }
                        var parameter = new Dictionary<string, object>();
                        // .Add("connectionName", ConnectionStringName)
                        // .Add("companyId", CompanyId.ToString())
                        parameter.Add("connectionString", currentConnection.ToString());

                        throw ExceptionOracleProcess(exOracle, command, table, "Query", parameter);
                    }
                }
                catch (Exception ex)
                {
                    if ((ex.Message.StartsWith("ORA-03135:") || ex.Message.StartsWith("ORA-03113:") || ex.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 || ex.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 || ex.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) && attempts < 3)
                    {
                        var magicMethod = currentConnection.GetType().GetMethod("ClearAllPools");

                        if (magicMethod.IsNotEmpty())
                        {
                            magicMethod.Invoke(currentConnection, new object[] { });
                        }
                        // command.Connection = Nothing
                        // If currentConnection.State = ConnectionState.Open Then
                        // currentConnection.Close()
                        // End If
                        LogHandler.WarningLog("DataAccessLayer", string.Format("Retry due to disconnection for query on table '{2}' ({0}). {1}", attempts, ex.Message, table));
                        Thread.Sleep(1000);
                        try
                        {
                            currentConnection.Open();
                        }
                        catch (Exception ex2)
                        {
                            var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                            ConnectionClosed(command, currentConnection);
                            throw temporalException;
                        }
                    }
                    else
                    {
                        var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                        ConnectionClosed(command, currentConnection);
                        throw temporalException;
                        break;
                    }
                }
            }
            return result;
        }

        private static Exceptions.DataAccessException ExceptionOracleProcess(OracleException exOracle, DbCommand command, string nameObject, string commandKind)
        {
            return ExceptionOracleProcess(exOracle, command, nameObject, commandKind, null);
        }

        private static Exceptions.DataAccessException ExceptionOracleProcess(OracleException exOracle, DbCommand command, string nameObject, string commandKind, Dictionary<string, object> parameters)
        {
            int code = exOracle.Number;
            bool IsDeveloperMode = false;
            string message = "";
            string messageValue = string.Empty;

            if (parameters.IsNotEmpty() && parameters.Count != 0)
            {
                // messageValue = String.Format("The ConnectionString used is: '{0}'", parameters("connectionString"))
                if (parameters.ContainsKey("connectionName"))
                {
                    messageValue = string.Format("Its ConnectionName is: '{0}'", parameters["connectionName"]);
                }

                if (messageValue.IsNotEmpty() && parameters.ContainsKey("companyId"))
                {
                    messageValue = messageValue + string.Format(", The Id of the company is: '{0}'", parameters["companyId"]);
                }
            }

            if ("Working.Mode".AppSettings().ToLower() == "Development")
            {
                IsDeveloperMode = true;
            }

            Exceptions.DataAccessException exceptionTemporal = null;
            switch (code)
            {
                // ORA-28000: the account is locked
                case 28000:
                    {
                        message = "The account is locked, This happens for several attempts with the user or incorrect password, please contact your database administrator";

                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }

                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }
                // ORA-01017: Invalid username/password; logon denied
                case 1017:
                    {
                        message = "Invalid user-name/password, The error is generated incorrect password or user, please contact your database administrator";
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }
                // ORA-28001: the password has expired
                case 28001:
                    {
                        message = "The password has expired, this error is generated by the user password expired, please contact your database administrator";
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }
                // Error con el tns
                case 12154:
                case 12153:
                case 12152:
                case 12151:
                case 12150:
                    {
                        message = string.Format("There is an error related to the TNS, the specific error code is {0}", code);
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }

                case 936:
                    {
                        message = string.Format("Missing expression, the specific error code is {0}", code);
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }

                case 942:
                    {
                        message = string.Format("The table or view named '{0}' does not exist,  the specific error code is {1}", nameObject, code);
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }

                case 904:
                    {
                        var reg = new Regex("\".*?\"");
                        var matches = reg.Matches(exOracle.Message);
                        if (matches.IsNotEmpty() && matches.Count != 0)
                        {
                            Match item;
                            if (matches.Count == 0)
                            {
                                item = matches.Cast<Match>().FirstOrDefault();
                            }
                            else
                            {
                                item = matches[1];
                            }

                            exceptionTemporal = new Exceptions.DataAccessException(string.Format("The table '{0}' does not contain column '{1}'", nameObject, item.ToString().Replace(Conversions.ToString('"'), string.Empty)), exOracle, command, nameObject, commandKind);
                        }
                        else
                        {
                            exceptionTemporal = new Exceptions.DataAccessException(string.Format("The table '{0}' does not contain column ", nameObject), exOracle, command, nameObject, commandKind);
                        }

                        break;
                    }

                default:
                    {
                        if (!IsDeveloperMode)
                        {
                            message = string.Format("An error occurred while trying to perform a command in the database,  the specific error code is {0}", code);
                        }
                        else
                        {
                            message = string.Format("An error occurred while trying to perform a command in the database. Detail:'{0}'", exOracle.Message);
                        }
                        if (messageValue.IsNotEmpty())
                        {
                            message = string.Format("{0},{1}{2}", message, Environment.NewLine, messageValue);
                        }
                        exceptionTemporal = new Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind);
                        break;
                    }
            }

            return exceptionTemporal;
        }

        public static int ProcedureExecute(DbCommand command, DbConnection currentConnection)
        {
            return CommandExecute(command, currentConnection, command.CommandText, "ProcedureExecute", new Dictionary<string, string>());
        }

        public static int ProcedureExecute(DbCommand command, DbConnection currentConnection, Dictionary<string, string> parameters = null)
        {
            return CommandExecute(command, currentConnection, command.CommandText, "ProcedureExecute", parameters);
        }

        public static DbDataReader ProcedureExecuteWithDataReaderResultset(DbCommand command, DbConnection currentConnection)
        {
            return ExecuteWithResultset(command, currentConnection);
        }

        public static DataTable ProcedureExecuteWithDataTableResultset(DbCommand command, DbConnection currentConnection)
        {
            return ExecuteWithDataTable(command, currentConnection, false);
        }

        public static DataTable ProcedureExecuteWithDataTableResultset(DbCommand command, DbConnection currentConnection, bool schemaOnly)
        {
            return ExecuteWithDataTable(command, currentConnection, schemaOnly);
        }

        public static T QueryExecuteScalar<T>(DbCommand command, DbConnection currentConnection, string table, Dictionary<string, string> parameters = null)
        {
            var result = default(T);
            string message = string.Empty;
            Stopwatch watch;
            object internalResult = null;
            int indexStack = Conversions.ToInteger(Interaction.IIf("DataManager.Mode".AppSettingsOnEquals("remote"), 4, 2));

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            try
            {
                watch = new Stopwatch();
                watch.Start();
                internalResult = command.ExecuteScalar();
                if (!(internalResult is DBNull))
                {
                    result = Conversions.ToGenericParameter<T>(internalResult);
                }
                watch.Stop();
                TraceLog(parameters, watch, message, internalResult, "QueryExecuteScalar", indexStack, command, table);
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }
            return result;
        }

        public static T QueryExecuteScalar<T>(string statement, DbConnection currentConnection, string table)
        {
            var result = default(T);
            var command = currentConnection.CreateCommand();
            Stopwatch watch;
            object internalResult = null;

            command.CommandType = CommandType.Text;
            command.CommandText = PreprocessStatement(statement, currentConnection.ToString());
            try
            {
                watch = new Stopwatch();
                watch.Start();
                internalResult = command.ExecuteScalar();
                if (!(internalResult is DBNull))
                {
                    result = Conversions.ToGenericParameter<T>(internalResult);
                }
                watch.Stop();
                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              Scalar={0}", result);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }
            return result;
        }

        public static int QueryExecuteScalar(DbCommand command, DbConnection currentConnection, string table)
        {
            int result = 0;
            Stopwatch watch;
            object internalResult = null;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            try
            {
                watch = new Stopwatch();
                watch.Start();
                internalResult = command.ExecuteScalar();
                if (!(internalResult is DBNull))
                {
                    result = Conversions.ToInteger(internalResult);
                }
                watch.Stop();
                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              Scalar={0}", result);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }
            return result;
        }

        public static int CommandExecute(DbCommand command, DbConnection currentConnection, string table, string commandKind)
        {
            return CommandExecute(command, currentConnection, table, commandKind, new Dictionary<string, string>());
        }

        public static int CommandExecute(DbCommand command, DbConnection currentConnection, string table, string commandKind, Dictionary<string, string> parameters = null)
        {
            int result = 0;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            for (int attempts = 1; attempts <= 3; attempts++)
            {
                command.Connection = currentConnection;
                try
                {
                    watch = new Stopwatch();
                    watch.Start();
                    result = command.ExecuteNonQuery();
                    watch.Stop();
                    if ("DataAccessLayer.Debug".AppSettings<bool>())
                    {
                        string argcommandText = null;
                        Dictionary<string, string> argparameters = null;
                        string message = MakeCommandSummary(command, table, commandKind, ref argcommandText, ref argparameters, false);
                        if (parameters.IsNotEmpty())
                        {
                            message = "{2}{0}{4}{2}{0}{1}{0}{2}{0}{3}".SpecialFormater("              ", "Parámetros:" + parameters.ToStringExtended(), Environment.NewLine, message, "Mode:" + "DataManager.Mode".AppSettings());
                        }
                        message += Constants.vbCrLf + string.Format("              Record Affected={0}", result);
                        message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);

                        if ("DataAccessLayer.Debug.DetailsCall".AppSettings<bool>())
                        {
                            string detailsCall = AssemblyHandler.MethodName(MethodBase.GetCurrentMethod().Name.ToLower());
                            if (detailsCall.IsNotEmpty())
                            {
                                message += Constants.vbCrLf + string.Format("{0}", detailsCall.ToString().Replace("<<I>>", "              "));
                            }
                        }
                        LogHandler.TraceLog("DataAccessLayer", message);
                    }
                    if (!HasOutputParameters(command))
                    {
                        command.Dispose();
                    }
                    break;
                }
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionString", currentConnection.ToString());
                    var temporalException = ExceptionOracleProcess(exOracle, command, table, "Query", parameter);
                    ConnectionClosed(command, currentConnection);
                    throw temporalException;
                }
                catch (Exception ex)
                {
                    Exception temporalException;
                    if ((ex.Message.StartsWith("ORA-03135:") || ex.Message.StartsWith("ORA-03113:") || ex.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 || ex.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 || ex.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) && attempts < 3)
                    {
                        var magicMethod = currentConnection.GetType().GetMethod("ClearAllPools");

                        if (magicMethod.IsNotEmpty())
                        {
                            magicMethod.Invoke(currentConnection, new object[] { });
                        }

                        LogHandler.WarningLog("DataAccessLayer", string.Format("Retry due to disconnection for '{3}' command on table '{2}' ({0}). {1}", attempts, ex.Message, table, commandKind));
                        Thread.Sleep(500);
                        try
                        {
                            currentConnection.Open();
                        }
                        catch (Exception ex2)
                        {
                            temporalException = Exceptions.DataAccessException.Factory(ex, command, table, commandKind);
                            ConnectionClosed(command, currentConnection);
                            throw temporalException;
                        }
                    }
                    else
                    {
                        temporalException = Exceptions.DataAccessException.Factory(ex, command, table, commandKind);
                        ConnectionClosed(command, currentConnection);
                        throw temporalException;
                        break;
                    }
                }
            }
            return result;
        }

        public static void ConnectionClosed(DbCommand Command, DbConnection currentConnection)
        {
            Command.Connection = null;
            if (currentConnection.State == ConnectionState.Open)
            {
                currentConnection.Close();
            }
        }

        public static DbConnection OpenDbConnection(string connectionName)
        {
            return OpenDbConnection(connectionName, connectionName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static int CompanyIdSelect()
        {
            int Result = 0;
            if ("BackOffice.IsMultiCompany".AppSettings<bool>())
            {
                if (!(HttpContext.Current == null))
                {
                    if (!(HttpContext.Current.Session == null))
                    {
                        Result = Conversions.ToInteger(HttpContext.Current.Session["CompanyId"]);
                    }
                }
            }
            return Result;
        }

        public static DbConnection OpenDbConnection(string connectionName, ref int companyId)
        {
            return OpenDbConnection(connectionName, connectionName, ref companyId);
        }

        public static object OpenDbConnectionRaw(string connectionStringRaw, string providerNameRaw)
        {
            DbProviderFactory currentProviderFactories = null;
            DbConnection currentConnection = null;
            if (connectionStringRaw.IsEmpty())
            {
                throw new Exceptions.DataAccessException(string.Format("The connection string '{0}' not found.", connectionStringRaw), null);
            }
            try
            {
                currentProviderFactories = DbProviderFactories.GetFactory(providerNameRaw);
            }
            catch (Exception ex)
            {
                throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex);
            }

            if (currentProviderFactories.IsNotEmpty())
            {
                try
                {
                    currentConnection = currentProviderFactories.CreateConnection();
                    currentConnection.ConnectionString = connectionStringRaw;
                    currentConnection.Open();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                    }

                    switch (ex.Number)
                    {
                        case 53:
                            {
                                throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                            }

                        case 18456:
                            {
                                throw new Exceptions.DataAccessException("The user-name/password is invalid", ex);
                            }

                        case 4060:
                            {
                                throw new Exceptions.DataAccessException("The initial catalog/username is invalid", ex);
                            }

                        default:
                            {
                                if (ex.ErrorCode == -2146232060 && ex.Message.StartsWith("A network-related or instance-specific error occurred"))
                                {
                                    throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                                }
                                else
                                {
                                    throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                                }
                            }
                    }
                }
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionString", currentConnection);
                    throw ExceptionOracleProcess(exOracle, null, "OpenDbConnectionRaw", "Open", parameter);
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                    }
                }
            }
            return currentConnection;
        }

        public static DbConnection OpenDbConnection(string connectionName, string connectionExtendName, ref int companyId)
        {
            DbProviderFactory currentProviderFactories = null;
            DbConnection currentConnection = null;
            var ConnectionString = ConfigurationManager.ConnectionStrings[connectionName];
            string mess = string.Format("{0}_{1}", connectionName, companyId);
            int companyIdValue = companyId;

            if (ConnectionString == null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[string.Format("Linked.{0}", connectionName)];
            }

            if (ConnectionString.IsEmpty())
            {
                throw new Exceptions.DataAccessException(string.Format("The connection string '{0}' not found.", connectionName), null);
            }
            try
            {
                currentProviderFactories = DbProviderFactories.GetFactory(ConnectionString.ProviderName);
            }
            catch (Exception ex)
            {
                throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex);
            }

            if (currentProviderFactories.IsNotEmpty())
            {
                try
                {
                    currentConnection = currentProviderFactories.CreateConnection();
                    currentConnection.ConnectionString = ConectionStringMultiCompany(ConnectionString.ConnectionString, connectionName, ref companyIdValue);
                    if (PrivilegedAccessSecurity.IsProvider())
                    {
                        currentConnection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(connectionName, ConnectionString.ConnectionString);
                    }
                    currentConnection.Open();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                    }

                    switch (ex.Number)
                    {
                        case 53:
                            {
                                throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                            }

                        case 18456:
                            {
                                throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                            }

                        case 4060:
                            {
                                throw new Exceptions.DataAccessException("The initial catalog/username is invalid", ex);
                            }

                        default:
                            {
                                if (ex.ErrorCode == -2146232060 && ex.Message.StartsWith("A network-related or instance-specific error occurred"))
                                {
                                    throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                                }
                                else
                                {
                                    throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                                }
                            }
                    }
                }
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionName", connectionName);
                    parameter.Add("connectionString", ConnectionString.ToString());
                    parameter.Add("companyId", companyIdValue.ToString());
                    throw ExceptionOracleProcess(exOracle, null, "OpenDbConnection", "Open", parameter);
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex);
                    }
                }
            }
            return currentConnection;
        }

        public static DbConnection OpenDbConnection(string connectionName, string connectionExtendName)
        {
            DbProviderFactory currentProviderFactories = null;
            DbConnection currentConnection = null;
            var ConnectionString = ConfigurationManager.ConnectionStrings[connectionName];

            if (ConnectionString.IsEmpty())
            {
                throw new Exceptions.DataAccessException(string.Format("The connection string '{0}' not found.", connectionName), null);
            }
            try
            {
                currentProviderFactories = DbProviderFactories.GetFactory(ConnectionString.ProviderName);
            }
            catch (Exception ex)
            {
                throw new Exceptions.DataAccessException("The database .net provider may not be installed", ex);
            }

            if (currentProviderFactories.IsNotEmpty())
            {
                try
                {
                    currentConnection = currentProviderFactories.CreateConnection();
                    int companyId = CompanyIdSelect();
                    currentConnection.ConnectionString = ConectionStringMultiCompany(ConnectionString.ConnectionString, connectionName, ref companyId);
                    if (PrivilegedAccessSecurity.IsProvider())
                    {
                        currentConnection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(connectionName, ConnectionString.ConnectionString);
                    }
                    currentConnection.Open();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("Invalid username/password", ex);
                    }

                    switch (ex.Number)
                    {
                        case 53:
                            {
                                throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                            }

                        case 18456:
                            {
                                throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                            }

                        case 4060:
                            {
                                throw new Exceptions.DataAccessException("The initial catalog/username is invalid", ex);
                            }

                        default:
                            {
                                if (ex.ErrorCode == -2146232060 && ex.Message.StartsWith("A network-related or instance-specific error occurred"))
                                {
                                    throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                    }
                }
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionName", connectionName);
                    parameter.Add("connectionString", ConnectionString.ToString());
                    throw ExceptionOracleProcess(exOracle, null, "OpenDbConnection", "Open", parameter);
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            return currentConnection;
        }

        public static void CloseDbConnection(DbConnection currentConnection)
        {
            if (currentConnection.IsNotEmpty())
            {
                if (currentConnection.State == ConnectionState.Open)
                {
                    currentConnection.Close();
                    currentConnection.Dispose();
                }
            }
        }

        public static DbParameter CreateQueryParameter(DbCommand command, string tableName, string name, DbType kind, int size, object value, ref string whereStatement)
        {
            var parameter = CommandParameter(command, name, kind, size, true, value);
            if (whereStatement.Trim().Equals("where", StringComparison.InvariantCultureIgnoreCase))
            {
                whereStatement += string.Format(" {0}.{1} = @:{1}", tableName, name);
            }
            else
            {
                whereStatement += string.Format(" AND {0}.{1} = @:{1}", tableName, name);
            }
            return parameter;
        }

        public static DbParameter CreateQueryParameter(DbCommand command, string tableName, string name, DbType kind, int size, object value, ref string whereStatement, string cancellationDateColumn)
        {
            var parameter = CommandParameter(command, name, kind, size, true, value);
            whereStatement += string.Format(" AND {2}.{0} <= @:{0} AND ({2}.{1} IS NULL OR {2}.{1} > @:{0})", name, cancellationDateColumn, tableName);

            return parameter;
        }

        public static DbParameter CommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value)
        {
            return CommandParameter(command, name, kind, size, setValue, value, ParameterDirection.Input);
        }

        /// <summary>
        /// Custom for parameter type Oracle.DataAccess.Client.OracleDbType.TimeStamp
        /// </summary>
        /// <param name="parameterInstance"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static DbParameter CreateDateTimeOffsetParameter(DbParameter parameterInstance)
        {
            var parameterType = parameterInstance.GetType();
            DbParameter refCursorParameter = null;
            bool fail = true;

            if (!(parameterType == null))
            {
                var oracleDbType = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType");
                if (!(oracleDbType == null))
                {
                    refCursorParameter = (DbParameter)Activator.CreateInstance(parameterType, new object[] { "RC1", Enum.Parse(oracleDbType, "TimeStamp") });
                    if (!(refCursorParameter == null))
                    {
                        refCursorParameter.Direction = ParameterDirection.Output;
                        fail = false;
                    }
                }
            }
            if (fail)
            {
                throw new Exceptions.InMotionGITException("It is trying to use a procedure with a xml type parameter, but you can not create xml ttype parameter due to provider, try changing it.");
            }

            return (DbParameter)refCursorParameter;
        }

        public static DbParameter CommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value, ParameterDirection direction)
        {
            var parameter = command.CreateParameter();
            bool IsSQLServer = command.GetType() == typeof(System.Data.SqlClient.SqlCommand);
            switch (kind)
            {
                case DbType.Xml:
                    {
                        if (IsSQLServer)
                        {
                            parameter.DbType = kind;
                        }
                        else
                        {
                            parameter = CreateXmlParameter(parameter);
                        }

                        break;
                    }
                case DbType.DateTimeOffset:
                    {
                        if (IsSQLServer)
                        {
                            parameter.DbType = kind;
                        }
                        else
                        {
                            parameter = CreateDateTimeOffsetParameter(parameter);
                        }

                        break;
                    }

                default:
                    {
                        parameter.DbType = kind;
                        break;
                    }
            }

            parameter.Direction = direction;
            parameter.ParameterName = name;
            parameter.Size = size;
            if (setValue)
            {
                if (Information.TypeName(value) == "XDocument")
                {
                    parameter.Value = ((XDocument)value).ToString();
                }
                else
                {
                    parameter.Value = value;
                }
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static DbParameter CreateCommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value, ref string fieldList, ref string valueList)
        {
            return CreateCommandParameter(command, name, kind, size, setValue, value, ref fieldList, ref valueList, false);
        }

        public static DbParameter CreateCommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value, ref string fieldList, ref string valueList, bool encrypted)
        {
            var parameter = CommandParameter(command, name, kind, size, setValue, value);

            if (!string.IsNullOrEmpty(fieldList))
            {
                fieldList += ",";
            }
            fieldList += name;
            if (!string.IsNullOrEmpty(valueList))
            {
                valueList += ",";
            }
            if (encrypted)
            {
                valueList += string.Format("INSUDB.EXTENCRYPTION.EncryptData(@:{0})", name);
            }
            else
            {
                valueList += string.Format("@:{0}", name);
            }

            return parameter;
        }

        public static DbParameter UpdateCommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value, ref string valueList)
        {
            return UpdateCommandParameter(command, name, kind, size, setValue, value, ref valueList, false);
        }

        public static DbParameter UpdateCommandParameter(DbCommand command, string name, DbType kind, int size, bool setValue, object value, ref string valueList, bool encrypted)
        {
            var parameter = CommandParameter(command, name, kind, size, setValue, value);

            if (!string.IsNullOrEmpty(valueList))
            {
                valueList += ",";
            }
            valueList += string.Format("{0}=", name);

            if (encrypted)
            {
                valueList += string.Format("INSUDB.EXTENCRYPTION.EncryptData(@:{0})", name);
            }
            else
            {
                valueList += string.Format("@:{0}", name);
            }

            return parameter;
        }

        #endregion Shared Members

        #region Helpers

        private static DataTable ExecuteWithDataTable(DbCommand command, DbConnection currentConnection, bool schemaOnly)
        {
            var reader = ExecuteWithResultset(command, currentConnection);
            DataTable result = null;
            if (schemaOnly)
            {
                result = reader.GetSchemaTable();
            }
            else
            {
                result = new DataTable();
                result.Load(reader);
                if (string.IsNullOrEmpty(result.TableName))
                {
                    result.TableName = "Result";
                }
            }
            if (reader.IsNotEmpty() && !reader.IsClosed)
            {
                reader.Close();
                reader = null;
            }
            return result;
        }

        private static long ExecuteWithOutResultset(DbCommand command, DbConnection currentConnection)
        {
            long recordAffected = 0L;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            try
            {
                watch = new Stopwatch();
                watch.Start();
                recordAffected = command.ExecuteNonQuery();
                watch.Stop();

                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, command.CommandText, "ExecuteWithOutResultset", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              Record Affected={0}", recordAffected);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, command.CommandText, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }
            return recordAffected;
        }

        private static DbDataReader ExecuteWithResultset(DbCommand command, DbConnection currentConnection)
        {
            DbDataReader reader = null;
            Stopwatch watch;

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString());
            command.Connection = currentConnection;
            if (command.CommandType == CommandType.StoredProcedure && currentConnection.GetType().Name.ToUpper(CultureInfo.CurrentCulture).Contains("ORACLE"))
            {
                if (IsMicrosoftOracleProvider(currentConnection.ToString()))
                {
                    command.Parameters.Add(CreateRefCursorParameter());
                }
                else
                {
                    command.Parameters.Add(CreateRefCursorParameter(command.CreateParameter()));
                }
            }
            try
            {
                watch = new Stopwatch();
                watch.Start();
                reader = command.ExecuteReader(CommandBehavior.SingleResult);
                watch.Stop();

                if ("DataAccessLayer.Debug".AppSettings<bool>())
                {
                    string argcommandText = null;
                    Dictionary<string, string> argparameters = null;
                    string message = MakeCommandSummary(command, command.CommandText, "Query", ref argcommandText, ref argparameters, false);
                    message += Constants.vbCrLf + string.Format("              HasRows={0}", reader.HasRows);
                    message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                    LogHandler.TraceLog("DataAccessLayer", message);
                }
                command.Connection = null;
                command = null;
            }
            catch (Exception ex)
            {
                var temporalException = Exceptions.DataAccessException.Factory(ex, command, command.CommandText, "Query");
                ConnectionClosed(command, currentConnection);
                throw temporalException;
            }
            return reader;
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
            var refCursorParameter = (DbParameter)Activator.CreateInstance(parameterType, new object[] { "RC1", Enum.Parse(oracleDbType, "RefCursor") });
            refCursorParameter.Direction = ParameterDirection.Output;

            return (DbParameter)refCursorParameter;
        }

        private static DbParameter CreateRefCursorParameter()
        {
            dynamic refCursorParameter = new System.Data.OracleClient.OracleParameter("RC1", System.Data.OracleClient.OracleType.Cursor);
            refCursorParameter.Direction = ParameterDirection.Output;

            return (DbParameter)refCursorParameter;
        }

        public static bool IsOracleProvider(string connectionName)
        {
            string ProviderName = ConfigurationManager.ConnectionStrings[connectionName].ProviderName;
            return IsOracle(ProviderName);
        }

        internal static bool IsMicrosoftOracleProvider(string providerName)
        {
            return providerName.StartsWith("System.Data.OracleClient", StringComparison.CurrentCultureIgnoreCase);
        }

        internal static bool IsOracle(string providerName)
        {
            return providerName.StartsWith("Oracle.DataAccess.Client", StringComparison.CurrentCultureIgnoreCase) || providerName.StartsWith("Oracle.ManagedDataAccess.Client", StringComparison.CurrentCultureIgnoreCase) || providerName.StartsWith("System.Data.OracleClient", StringComparison.CurrentCultureIgnoreCase);
        }

        public static string PreprocessStatement(string commandText, string providerName)
        {
            commandText = commandText.Replace(" (NOLOCK)  AND ", " (NOLOCK) WHERE ");
            commandText = commandText.Replace(" WHERE AND ", " WHERE ");
            if (IsOracle(providerName))
            {
                commandText = commandText.Replace("GETDATE()", "SYSDATE");
                commandText = commandText.Replace("@:", ":");
                commandText = commandText.Replace(" (NOLOCK)", "");
                commandText = commandText.Replace(" ISNULL(", " NVL(");
            }
            else
            {
                commandText = commandText.Replace("SYSDATE", "GETDATE()");
                commandText = commandText.Replace("@:", "@");
                commandText = commandText.Replace(" NVL(", " ISNULL(");
                // statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
                // statement = statement.Replace(" ISNULL(", " ISNULL(")
            }
            commandText = commandText.Replace("{OWNER}", "");

            return commandText;
        }

        internal static string MakeCommandSummary(DbCommand command, string table, string commandKind, ref string commandText, ref Dictionary<string, string> parameters, bool addTagInnerException)
        {
            string extra = string.Empty;
            string parameterName = string.Empty;
            string parameterValue = string.Empty;
            if (command.IsNotEmpty())
            {
                if (command.CommandType == CommandType.StoredProcedure)
                {
                    commandText = string.Format("Procedure {0}", command.CommandText);
                }
                else
                {
                    commandText = command.CommandText;
                }
                extra = commandText;
                if (command.Parameters.IsNotEmpty())
                {
                    parameters = new Dictionary<string, string>();
                    foreach (DbParameter item in command.Parameters)
                    {
                        parameterName += string.Format(":{0},", item.ParameterName);
                        if (item.Value is DBNull)
                        {
                            parameters.Add(item.ParameterName, "Null");
                            extra += Constants.vbCrLf;
                            if (!addTagInnerException)
                            {
                                extra += "             ";
                            }
                            extra += string.Format(" {0}=Null", item.ParameterName);
                            parameterValue += "Null,";
                        }
                        else
                        {
                            parameters.Add(item.ParameterName, string.Format("{0}", item.Value));
                            extra += Constants.vbCrLf;
                            if (!addTagInnerException)
                            {
                                extra += "             ";
                            }
                            extra += string.Format(" {0} ({2}) ={1}", item.ParameterName, item.Value, item.Direction);
                            if (item.DbType == DbType.StringFixedLength)
                            {
                                parameterValue += string.Format("'{0}',", item.Value);
                            }
                            else if (item.DbType == DbType.Date)
                            {
                                parameterValue += string.Format("TO_DATE('{0}', 'MM/DD/YYYY HH24:MI:SS'),", ((DateTime)item.Value).ToString("MM/dd/yyyy HH:mm:ss"));
                            }
                            else
                            {
                                parameterValue += string.Format("{0},", item.Value);
                            }
                        }
                    }
                }
                if (parameterValue.IsNotEmpty() & parameterValue.EndsWith(","))
                {
                    parameterValue = parameterValue.Substring(0, parameterValue.Length - 1);
                }

                // If addTagInnerException Then
                // extra &= " @@InnerException@@"
                // End If
            }

            return extra;
        }

        public static string DateValueWithFormat(string connectionName, DateTime value)
        {
            string dateFormat = "{0}.DateFormat".SpecialFormater(connectionName).AppSettings();
            string result = value.ToString();

            if (dateFormat.IsNotEmpty())
            {
                return value.ToString(dateFormat, new CultureInfo("en-US"));
            }

            return result;
        }

        #endregion Helpers

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Release();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        // Protected Overrides Sub Finalize()
        // ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        #region CIAMulti

        public static string ConectionStringMultiCompany(string stringConection, string stringConectionName, ref int companyId)
        {
            string _Resul = stringConection;
            string _User = string.Empty;
            string _PassWord = string.Empty;

            if ("BackOffice.IsMultiCompany".AppSettings<bool>())
            {
                if ("Core.Mapper".AppSettings().IsNotEmpty())
                {
                    // En caso de llegar el id de la compañía vació, pero a nivel de configuración este establecido la compañía por default,
                    // entonces se establece el id compañía indicado.
                    if (companyId == 0)
                    {
                        companyId = "BackOffice.CompanyDefault".AppSettings<int>();
                    }

                    if (companyId > 0)
                    {
                        string[] conectionVecto = "Core.Mapper".AppSettings().Split(',');
                        bool exist;
                        if (conectionVecto.Length == 0)
                        {
                            exist = string.Equals("Core.Mapper".AppSettings(), stringConectionName);
                        }
                        else
                        {
                            exist = conectionVecto.Contains(stringConectionName);
                            if (!exist && !stringConectionName.StartsWith("Linked.", StringComparison.CurrentCultureIgnoreCase))
                            {
                                exist = conectionVecto.Contains("Linked.{0}".SpecialFormater(stringConectionName));
                            }
                        }
                        object[] arrResult = (object[])MultiCompany.GetUserInfo((short)companyId);
                        if (!(arrResult == null))
                        {
                            _User = arrResult[1].ToString();
                            _PassWord = arrResult[2].ToString();
                        }
                        if (exist)
                        {
                            if (!string.IsNullOrEmpty(_User) && !string.IsNullOrEmpty(_PassWord))
                            {
                                if (!_Resul.EndsWith(";"))
                                {
                                    _Resul = ";{0};User ID={1};Password={2}".SpecialFormater(_Resul, BackOffice.CryptSupport.HexDecryptString(ref _User), BackOffice.CryptSupport.HexDecryptString(ref _PassWord));
                                }
                                else
                                {
                                    _Resul = "{0};User ID={1};Password={2}".SpecialFormater(_Resul, BackOffice.CryptSupport.HexDecryptString(ref _User), BackOffice.CryptSupport.HexDecryptString(ref _PassWord));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
            }
            return _Resul;
        }

        #endregion CIAMulti

        #region DataBase Provider Methods

        // TODO: validar este caso con el provider de microsoft.
        public static string DbProviderParameterPrefix(string parameterName, string providerName)
        {
            string result = string.Empty;
            switch (providerName.ToLower() ?? "")
            {
                case "oracle.dataaccess.client":
                case "oracle.manageddataaccess.client":
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

        public string DbProviderParameterPrefix(string parameterName)
        {
            string result = string.Empty;

            switch (ProviderName.ToLower() ?? "")
            {
                case "oracle.dataaccess.client":
                case "oracle.manageddataAccess.client":
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
                    case "oracle.manageddataaccess.client":
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

        #endregion DataBase Provider Methods

        #region Tools

        public static Services.Contracts.ConnectionStrings TestConnectionLive(string DataSource, string ServicesName, string catalog, string user, string password, Enumerations.EnumSourceType ServerType)
        {
            var result = new Services.Contracts.ConnectionStrings();
            string connectionString = string.Empty;
            DbProviderFactory currentProviderFactories = null;
            string provider = string.Empty;
            switch (ServerType)
            {
                case Enumerations.EnumSourceType.SqlServer:
                    {
                        provider = "System.Data.SqlClient";
                        connectionString += string.Format("Data Source={0};user id={1};password={2}", DataSource, user, password);
                        connectionString += string.Format(";Initial Catalog={0}", catalog);
                        break;
                    }

                case Enumerations.EnumSourceType.Oracle:
                    {
                        provider = "Oracle.DataAccess.Client";
                        connectionString += string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));", DataSource, ServicesName);
                        connectionString += string.Format("User ID={0};Password={1}", user, password);
                        break;
                    }
            }
            try
            {
                result.ConnectionString = connectionString;
                result.ProviderName = provider;
                result.DatabaseName = catalog;
                result.Owners = user;
                result.Password = password;
                result.ServiceName = ServicesName;
                result.UserName = user;
                currentProviderFactories = DbProviderFactories.GetFactory(provider);
            }
            catch (Exception ex)
            {
                result = null;
                throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex);
            }

            DbConnection db = (DbConnection)OpenDbConnectionRaw(connectionString, result.ProviderName);
            if (db.IsNotEmpty())
            {
                db.Close();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static Services.Contracts.ConnectionStrings TestConnection(string DataSource, string ServicesName, string catalog, string user, string password, Enumerations.EnumSourceType ServerType)
        {
            var result = new Services.Contracts.ConnectionStrings();
            string connectionString = string.Empty;
            DbProviderFactory currentProviderFactories = null;
            string provider = string.Empty;
            switch (ServerType)
            {
                case Enumerations.EnumSourceType.SqlServer:
                    {
                        provider = "System.Data.SqlClient";
                        connectionString += string.Format("Data Source={0};user id={1};password={2}", DataSource, user, password);
                        connectionString += string.Format(";Initial Catalog={0}", catalog);
                        break;
                    }

                case Enumerations.EnumSourceType.Oracle:
                    {
                        provider = "Oracle.DataAccess.Client";
                        connectionString += string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));", DataSource, ServicesName);
                        connectionString += string.Format("User ID={0};Password={1}", user, password);
                        break;
                    }
            }
            try
            {
                result.ConnectionString = connectionString;
                result.ProviderName = provider;
                result.DatabaseName = catalog;
                result.Owners = user;
                result.Password = password;
                result.ServiceName = ServicesName;
                result.UserName = user;
                currentProviderFactories = DbProviderFactories.GetFactory(provider);
            }
            catch (Exception ex)
            {
                result = null;
                throw new Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex);
            }

            var db = OpenDbConnection(connectionString);
            if (db.IsNotEmpty())
            {
                db.Close();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static DbConnection OpenLocalDbConnection(string connectionString, string ProviderName)
        {
            DbProviderFactory currentProviderFactories = null;
            DbConnection currentConnection = null;

            try
            {
                currentProviderFactories = DbProviderFactories.GetFactory(ProviderName);
            }
            catch (Exception ex)
            {
                throw new Exceptions.DataAccessException("The database .net provider may not be installed", ex);
            }

            if (currentProviderFactories.IsNotEmpty())
            {
                try
                {
                    currentConnection = currentProviderFactories.CreateConnection();
                    currentConnection.ConnectionString = connectionString;
                    currentConnection.Open();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw new Exceptions.DataAccessException("Invalid username/password", ex);
                    }

                    switch (ex.Number)
                    {
                        case 53:
                            {
                                throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                            }

                        case 18456:
                            {
                                throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                            }

                        case 4060:
                            {
                                throw new Exceptions.DataAccessException("The initial catalog/username is invalid", ex);
                            }

                        default:
                            {
                                if (ex.ErrorCode == -2146232060 && ex.Message.StartsWith("A network-related or instance-specific error occurred"))
                                {
                                    throw new Exceptions.DataAccessException("The server was not found or was not accessible", ex);
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                    }
                }
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionString", connectionString.ToString());
                    throw ExceptionOracleProcess(exOracle, null, "OpenLocalDbConnection", "Open", parameter);
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("ORA-12154:"))
                    {
                        throw new Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex);
                    }
                    else if (ex.Message.StartsWith("ORA-01017:") | ex.Message.StartsWith("ORA-1017:"))
                    {
                        throw new Exceptions.DataAccessException("The username/password is invalid", ex);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            return currentConnection;
        }

        public DataTable ExecuteQuery(string query, string connectionString, string providerName, string connectionStringName, int companyId)
        {
            if (_dbConnection == null)
            {
                _dbConnection = OpenLocalDbConnection(connectionString, providerName);
            }
            return ExecuteQuery(query, connectionStringName, companyId);
        }

        private static DbParameter CreateXmlParameter(DbParameter parameterInstance)
        {
            // Private Shared Function CreateXmlParameter(command As DbCommand, name As String, kind As System.Data.DbType, size As Integer, setValue As Boolean, value As Object, direction As System.Data.ParameterDirection) As DbParameter
            // Dim refCursorParameter As Object = New System.Data.OracleClient.OracleParameter(name, Oracle.DataAccess.Client.OracleDbType.XmlType)
            // refCursorParameter.Direction = direction

            // Return refCursorParameter

            var parameterType = parameterInstance.GetType();
            dynamic refCursorParameter = null;
            bool fail = true;

            if (!(parameterType == null))
            {
                var oracleDbType = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType");

                if (!(oracleDbType == null))
                {
                    refCursorParameter = Activator.CreateInstance(parameterType, new object[] { "RC1", Enum.Parse(oracleDbType, "XmlType") });

                    if (!(refCursorParameter == null))
                    {
                        refCursorParameter.Direction = ParameterDirection.Output;
                        fail = false;
                    }
                }
            }

            if (fail)
            {
                throw new Exceptions.InMotionGITException("It is trying to use a procedure with a xml type parameter, but you can not create xml ttype parameter due to provider, try changing it.");
            }

            return (DbParameter)refCursorParameter;
        }

        public static string ValueDateFormat(string repositoryName, DateTime value)
        {
            string result = value.ToString(DateFormat(repositoryName), new CultureInfo("en-US"));
            // result = result.Replace("/mm/", "/")
            return result;
        }

        public static string DateFormat(string repositoryName)
        {
            return "{0}.DateFormat".SpecialFormater(repositoryName).AppSettings();
        }

        public static string RealConnectionStringName(string repositoryName)
        {
            var ConnectionSetting = ConfigurationManager.ConnectionStrings[repositoryName];

            if (ConnectionSetting == null)
            {
                ConnectionSetting = ConfigurationManager.ConnectionStrings["Linked.{0}".SpecialFormater(repositoryName)];
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

        public static bool TestConnection(string DataSource, string catalog, string user, string password, string provider)
        {
            bool result = true;
            string connectionString = string.Format("Data Source={0};user id={1};password={2}", DataSource, user, password);
            if (provider == "System.Data.SqlClient")
            {
                connectionString += string.Format(";Initial Catalog={0}", catalog);
            }
            result = new DataAccessLayer().OpenConnectionString(connectionString, provider);

            return result;
        }

        public static bool TestConnection(string connectionStringName)
        {
            var ConnectionSetting = GetConnectionString(connectionStringName);
            bool result = false;

            if (!(ConnectionSetting == null))
            {
                result = new DataAccessLayer().OpenConnectionString(ConnectionSetting.ConnectionString, ConnectionSetting.ProviderName);
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
                catch (OracleException exOracle)
                {
                    var parameter = new Dictionary<string, object>();
                    parameter.Add("connectionString", connectionString.ToString());
                    throw ExceptionOracleProcess(exOracle, null, "OpenConnectionString", "Open");
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

        private static bool HasOutputParameters(DbCommand command)
        {
            bool result = false;

            if (command.IsNotEmpty() && command.Parameters.IsNotEmpty())
            {
                foreach (DbParameter item in command.Parameters)
                {
                    if (item.Direction != ParameterDirection.Input)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion Tools

        public DataAccessLayer()
        {
        }

        public static void TraceLog(Dictionary<string, string> parameters, Stopwatch watch, string message, object result, string method, int indexStack, DbCommand command, string table)
        {
            if ("DataAccessLayer.Debug".AppSettings<bool>())
            {
                string argcommandText = null;
                Dictionary<string, string> argparameters = null;
                message = "{2}              Mode: {1}{2}              Id: {0}{2}              {3}".SpecialFormater(parameters["Id"], "DataManager.Mode".AppSettings(), Environment.NewLine, MakeCommandSummary(command, table, "Query", ref argcommandText, ref argparameters, false));
                if (parameters.IsNotEmpty())
                {
                    message += "{1}              Parámetros:{0}".SpecialFormater(parameters.ToStringExtended(), Environment.NewLine);
                }
                message += Constants.vbCrLf + string.Format("              Scalar={0}", result);
                message += Constants.vbCrLf + string.Format("              {0} ms", watch.ElapsedMilliseconds);
                if ("DataAccessLayer.Debug.DetailsCall".AppSettings<bool>())
                {
                    string detailsCall = AssemblyHandler.GetFrameProcess(indexStack);
                    if (detailsCall.IsNotEmpty())
                    {
                        message += Constants.vbCrLf + string.Format("{0}", detailsCall.ToString().Replace("<<I>>", "              "));
                    }
                }
                LogHandler.TraceLog("DataAccessLayer", message);
            }
        }
    }
}