using System;
using System.Collections.Generic;

#region using

using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using InMotionGIT.Common.Attributes;
using InMotionGIT.Common.Extensions;
using InMotionGIT.Common.Helpers;
using InMotionGIT.Common.Services.Contracts;
using InMotionGIT.Common.Services.Interfaces;

#endregion using

namespace InMotionGIT.Common.Services;

// <AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
[EntityExclude()]
public class DataManager : IDataManager
{
    /// <summary>
    /// Método que permite devolver valores en modo Lookup / Method to return values mode Lookup
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public List<DataType.LookUpValue> QueryExecuteToLookup(DataCommand command)
    {
        var result = new List<DataType.LookUpValue>();
        DataTable resultTable;
        using (var db = ConnectionOpenLocal(command))
        {
            if (!string.IsNullOrEmpty(command.QueryCount))
            {
                using (var commandItem = db.CreateCommand())
                {
                    commandItem.CommandType = CommandType.Text;
                    commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.QueryCount, db.ToString());
                    BuildParameters(command, commandItem);
                    command.QueryCountResult = DataAccessLayer.QueryExecuteScalar<int>(commandItem, db, command.TableName);
                    commandItem.Connection = null;
                    commandItem.Dispose();
                }
            }

            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                resultTable = DataAccessLayer.QueryExecuteToTable(commandItem, db, CommandBehavior.Default, command.TableName, new Dictionary<string, string>() { { "CompanyId", command.CompanyId.ToString() } });

                commandItem.Connection = null;
                commandItem.Dispose();

                if (resultTable.Columns.Count >= 2)
                {
                    if (resultTable.IsNotEmpty() && resultTable.Rows.Count != 1)
                    {
                        foreach (DataRow Item in resultTable.Rows)
                            result.Add(new DataType.LookUpValue() { Code = Item.StringValue(resultTable.Columns[0].ColumnName).Trim(), Description = Item.StringValue(resultTable.Columns[1].ColumnName).Trim() });
                    }
                }
                else
                {
                    throw new Exceptions.InMotionGITException("The query must have at least two columns for the lookupvalue");
                }
            }

            db.Close();
            db.Dispose();
        }

        return result;
    }

    public string QueryExecuteToTableJSON(DataCommand command, bool resultEmpty)
    {
        DataTable result = null;

        int argcompanyId = command.CompanyId;
        using (var db = DataAccessLayer.OpenDbConnection(command.ConnectionStringName, ref argcompanyId))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteToTableJSON(commandItem, db, CommandBehavior.Default, command.TableName, resultEmpty, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        command.CompanyId = argcompanyId;
        return result.ToJSON().CompressString();
    }

    public QueryResult QueryExecuteToTable(DataCommand command, bool resultEmpty)
    {
        var result = new QueryResult();
        using (var db = ConnectionOpenLocal(command))
        {
            if (!string.IsNullOrEmpty(command.QueryCount))
            {
                using (var commandItem = db.CreateCommand())
                {
                    commandItem.CommandType = CommandType.Text;
                    commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.QueryCount, db.ToString());
                    BuildParameters(command, commandItem);
                    command.QueryCountResult = DataAccessLayer.QueryExecuteScalar<int>(commandItem, db, command.TableName, command.Fields);
                    commandItem.Connection = null;
                    commandItem.Dispose();
                }
            }

            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result.Table = DataAccessLayer.QueryExecuteToTable(commandItem, db, CommandBehavior.Default, command.TableName, resultEmpty, command.Fields);
                if (result.Table.IsNotEmpty() && result.Table.Rows.Count > 0)
                {
                    result.QueryCountResult = result.Table.Rows.Count;
                }
                if (!command.IgnoreMaxNumberOfRecords)
                {
                    if (command.MaxNumberOfRecord != 0)
                    {
                        if (result.Table.IsNotEmpty() && result.Table.Rows.Count > command.MaxNumberOfRecord)
                        {
                            var newDataTable = new DataTable();
                            newDataTable = result.Table.Clone();
                            newDataTable.Rows.Clear();
                            for (int i = 0, loopTo = command.MaxNumberOfRecord - 1; i <= loopTo; i++)
                                newDataTable.ImportRow(result.Table.Rows[i]);
                            result.Table = newDataTable;
                        }
                    }
                }
                commandItem.Connection = null;
                commandItem.Dispose();
            }

            db.Close();
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// Over method for connectionstringsraw
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DbConnection ConnectionOpenLocal(DataCommand command)
    {
        DbConnection result = null;
        if (command.ConnectionStringsRaw.IsEmpty())
        {
            int argcompanyId = command.CompanyId;
            result = DataAccessLayer.OpenDbConnection(command.ConnectionStringName, ref argcompanyId);
            command.CompanyId = argcompanyId;
        }
        else if (command.ConnectionStringsRaw.IsNotEmpty())
        {
            result = (DbConnection)DataAccessLayer.OpenDbConnectionRaw(command.ConnectionStringsRaw.ConnectionString, command.ConnectionStringsRaw.ProviderName);
        }
        return result;
    }

    /// <summary>
    /// Over method for connectionstringsraw
    /// </summary>
    /// <param name="connectionName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DbConnection ConnectionOpenLocal(string connectionName)
    {
        DbConnection result = null;
        result = DataAccessLayer.OpenDbConnection(connectionName);
        return result;
    }

    public object QueryExecuteScalar(DataCommand command)
    {
        object result = null;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteScalar<object>(commandItem, db, command.TableName, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }

        return result;
    }

    public int QueryExecuteScalarToInteger(DataCommand command)
    {
        int result = 0;
        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteScalar<int>(commandItem, db, command.TableName, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        if (result.IsEmpty())
        {
            result = 0;
        }
        return result;
    }

    /// <summary>
    /// Método que permite devolver el valor en decimal/ Method to return the value in decimal
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public decimal QueryExecuteScalarToDecimal(DataCommand command)
    {
        decimal result = 0m;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteScalar<decimal>(commandItem, db, command.TableName, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        if (result.IsEmpty())
        {
            result = 0.0m;
        }
        return result;
    }

    public string QueryExecuteScalarToString(DataCommand command)
    {
        string result = string.Empty;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteScalar<string>(commandItem, db, command.TableName, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        if (result.IsEmpty())
        {
            result = string.Empty;
        }
        return result;
    }

    public DateTime QueryExecuteScalarToDate(DataCommand command)
    {
        var result = DateTime.MinValue;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.QueryExecuteScalar<DateTime>(commandItem, db, command.TableName, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        if (result.IsEmpty())
        {
            result = DateTime.MinValue;
        }
        return result;
    }

    public void CommandExecuteAsynchronous(DataCommand command)
    {
        int result = 0;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.CommandExecute(commandItem, db, command.TableName, command.Operation, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
    }

    public int CommandExecute(DataCommand command)
    {
        int result = 0;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                result = DataAccessLayer.CommandExecute(commandItem, db, command.TableName, command.Operation, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        return result;
    }

    public string DataStructure(DataCommand command)
    {
        string result = "Ok";

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.Text;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                DataAccessLayer.CommandExecute(commandItem, db, command.TableName, "Data Structure", command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        return result;
    }

    /// <summary>
    /// Build the command for making to script - Construye el comando para convertirlo en un script
    /// </summary>
    /// <param name="command"> Command executed - Comando ejecutado</param>
    /// <returns>Real statement - Comando real</returns>
    public string ResolveStatement(DataCommand command)
    {
        string result = string.Empty;

        using (var db = ConnectionOpenLocal(command))
        {
            string statement = command.Statement;
            string value = "'Null'";

            foreach (DataParameter item in command.Parameters)
            {
                if (!item.IsNull)
                {
                    switch (item.Type)
                    {
                        case DbType.AnsiString:
                        case DbType.AnsiStringFixedLength:
                            {
                                value = string.Format(CultureInfo.InvariantCulture, "'{0}'", item.Value.ToString().Replace("'", "''"));
                                break;
                            }

                        default:
                            {
                                value = string.Format(CultureInfo.InvariantCulture, "{0}", item.Value.ToString().Replace("'", string.Empty));
                                break;
                            }
                    }
                }

                statement = statement.Replace(string.Format(CultureInfo.InvariantCulture, "@:{0}", item.Name), value);
            }

            result = DataAccessLayer.PreprocessStatement(statement, db.ToString());
        }

        return result;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public bool ObjectExist(DataCommand command)
    {
        bool result;

        using (var db = ConnectionOpenLocal(command))
        {
            string providerName = db.ToString();

            using (var commandItem = db.CreateCommand())
            {
                if (DataAccessLayer.IsOracle(providerName))
                {
                    commandItem.CommandType = CommandType.Text;
                    commandItem.CommandText = DataAccessLayer.PreprocessStatement("SELECT COUNT(ALL_OBJECTS.OBJECT_NAME)" + " FROM ALL_OBJECTS" + " WHERE ALL_OBJECTS.OWNER       =@:OWNER" + " AND ALL_OBJECTS.OBJECT_TYPE =@:TYPE" + " AND ALL_OBJECTS.OBJECT_NAME =@:NAME", providerName);
                }
                else
                {
                    throw new NotImplementedException();
                }
                DataAccessLayer.CommandParameter(commandItem, "OWNER", DbType.AnsiString, 30, true, command.Owner.ToUpper());
                DataAccessLayer.CommandParameter(commandItem, "TYPE", DbType.AnsiString, 30, true, command.ObjectType.ToUpper());
                DataAccessLayer.CommandParameter(commandItem, "NAME", DbType.AnsiString, 30, true, command.TableName.ToUpper());

                result = DataAccessLayer.QueryExecuteScalar<int>(commandItem, db, "Object Exist") > 0;
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }

        return result;
    }

    #region Helpers

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="commandItem"></param>
    /// <remarks></remarks>
    private static void BuildParameters(DataCommand command, DbCommand commandItem)
    {
        if (command.Parameters.IsNotEmpty() && command.Parameters.Count > 0)
        {
            foreach (DataParameter item in command.Parameters)
            {
                LogHandler.TraceLog("COMMON - Nelson Direction" + item.Name + " = " + item.Direction.ToString());
                DataAccessLayer.CommandParameter(commandItem, item.Name, item.Type, item.Size, !item.IsNull, item.Value, item.Direction);
            }
        }
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

    #endregion Helpers

    public StoredProcedureResult ProcedureExecute(DataCommand command)
    {
        int resultRowAffected = 0;
        var result = new StoredProcedureResult();
        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.StoredProcedure;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);
                resultRowAffected = DataAccessLayer.CommandExecute(commandItem, db, command.TableName, command.Operation, command.Fields);
                result.RowAffected = resultRowAffected;
                var tempParamtterOut = StoredParemeterOut(commandItem.Parameters);
                if (!(tempParamtterOut == null) && tempParamtterOut.Count != 0)
                {
                    result.OutParameter = new Dictionary<string, object>();
                    foreach (var item in tempParamtterOut)
                    {
                        {
                            var withBlock = result.OutParameter;
                            if (!withBlock.ContainsKey(item.ParameterName))
                            {
                                withBlock.Add(item.ParameterName, item.Value);
                            }
                        }
                    }
                }
                commandItem.Connection = null;
                commandItem.Dispose();
            }
            db.Close();
            db.Dispose();
        }
        return result;
    }

    private List<DbParameter> StoredParemeterOut(DbParameterCollection dbParameterCollection)
    {
        var result = new List<DbParameter>();
        foreach (DbParameter item in dbParameterCollection)
        {
            if (item.Direction == ParameterDirection.Output | item.Direction == ParameterDirection.InputOutput)
            {
                result.Add(item);
                if (item.Value is DBNull)
                {
                    item.Value = null;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Permite la ejecución de un procedure tipado con retorno de un table
    /// </summary>
    /// <param name="command"></param>
    /// <param name="resultEmpty"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public QueryResult ProcedureExecuteToTable(DataCommand command, bool resultEmpty)
    {
        var result = new QueryResult();
        using (var db = ConnectionOpenLocal(command))
        {
            if (!string.IsNullOrEmpty(command.QueryCount))
            {
                using (var commandItem = db.CreateCommand())
                {
                    commandItem.CommandType = CommandType.Text;
                    commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.QueryCount, db.ToString());

                    BuildParameters(command, commandItem);

                    command.QueryCountResult = DataAccessLayer.QueryExecuteScalar<int>(commandItem, db, command.TableName);
                    commandItem.Connection = null;
                    commandItem.Dispose();
                }
            }

            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.StoredProcedure;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());

                BuildParameters(command, commandItem);

                if (DataAccessLayer.IsOracle(db.ToString()))
                {
                    commandItem.Parameters.Add(CreateRefCursorParameter(commandItem.CreateParameter()));
                }

                result.QueryCountResult = command.QueryCountResult;
                result.Table = DataAccessLayer.QueryExecuteToTable(commandItem, db, CommandBehavior.Default, command.TableName, resultEmpty, new Dictionary<string, string>() { { "CompanyId", command.CompanyId.ToString() } });

                if (result.Table.IsNotEmpty() && result.Table.Rows.Count > 0)
                {
                    result.QueryCountResult = result.Table.Rows.Count;
                }

                var temporalParameters = StoredParemeterOut(commandItem.Parameters);

                if (!(temporalParameters == null) && temporalParameters.Count != 0)
                {
                    result.OutputParameters = new Dictionary<string, object>();

                    foreach (var item in temporalParameters)
                    {
                        {
                            var withBlock = result.OutputParameters;
                            if (!withBlock.ContainsKey(item.ParameterName))
                            {
                                withBlock.Add(item.ParameterName, item.Value);
                            }
                        }
                    }
                }

                if (!command.IgnoreMaxNumberOfRecords)
                {
                    if (command.MaxNumberOfRecord != 0)
                    {
                        if (result.Table.IsNotEmpty() && result.Table.Rows.Count > command.MaxNumberOfRecord)
                        {
                            var newDataTable = new DataTable();
                            newDataTable = result.Table.Clone();
                            newDataTable.Rows.Clear();

                            for (int i = 0, loopTo = command.MaxNumberOfRecord - 1; i <= loopTo; i++)
                                newDataTable.ImportRow(result.Table.Rows[i]);

                            result.Table = newDataTable;
                        }
                    }
                }

                commandItem.Dispose();
            }
        }

        return result;
    }

    /// <summary>
    /// Sobre carga para poder envir un conjunto de commandos y ejecutar como instruccion atomica
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<DataType.LookUpPackage> PackageExecuteScalar(List<DataCommand> commands)
    {
        return PackageExecuteScalar(commands, "");
    }

    /// <summary>
    /// Método poder enviar un conjunto de commandos y ejecutar como instrucción atómica
    /// </summary>
    /// <param name="commands"></param>
    /// <param name="connectionName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<DataType.LookUpPackage> PackageExecuteScalar(List<DataCommand> commands, string connectionName)
    {
        var result = new List<DataType.LookUpPackage>();
        var connections = (from itemDb in commands
                           where itemDb.ConnectionStringName.IsNotEmpty()
                           select itemDb.ConnectionStringName).Distinct().ToList();

        foreach (var ItemConnection in connections)
        {
            var commandsByConnections = (from itemDb in commands
                                         where itemDb.ConnectionStringName.IsNotEmpty() && itemDb.ConnectionStringName.Equals(ItemConnection)
                                         select itemDb).ToList();

            int argcompanyId = commandsByConnections[0].CompanyId;
            using (var db = DataAccessLayer.OpenDbConnection(ItemConnection, ref argcompanyId))
            {
                foreach (DataCommand command in commandsByConnections)
                {
                    using (var commandItem = db.CreateCommand())
                    {
                        commandItem.CommandType = CommandType.Text;
                        commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                        BuildParameters(command, commandItem);
                        result.Add(new DataType.LookUpPackage() { Key = command.TableName, Count = DataAccessLayer.QueryExecuteScalar(commandItem, db, command.TableName) });
                        commandItem.Connection = null;
                        commandItem.Dispose();
                    }
                }
                db.Close();
                db.Dispose();
            }
            commandsByConnections[0].CompanyId = argcompanyId;
        }
        return result;
    }

    /// <summary>
    /// Sobre carga para poder envir un conjunto de commandos y ejecutar como instruccion atomica
    /// </summary>
    /// <param name="commands"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<DataType.LookUpPackage> PackageExecuteToLookUp(List<DataCommand> commands)
    {
        return PackageExecuteToLookUp(commands, "");
    }

    /// <summary>
    /// Método poder enviar un conjunto de commandos y ejecutar como instrucción atómica
    /// </summary>
    /// <param name="commands"></param>
    /// <param name="connectionName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<DataType.LookUpPackage> PackageExecuteToLookUp(List<DataCommand> commands, string connectionName)
    {
        var result = new List<DataType.LookUpPackage>();
        var connections = (from itemDb in commands
                           where itemDb.ConnectionStringName.IsNotEmpty()
                           select itemDb.ConnectionStringName).Distinct().ToList();
        foreach (var ItemConnection in connections)
        {
            var commandsByConnections = (from itemDb in commands
                                         where itemDb.ConnectionStringName.IsNotEmpty() && itemDb.ConnectionStringName.Equals(ItemConnection)
                                         select itemDb).ToList();

            int argcompanyId = commandsByConnections[0].CompanyId;
            using (var db = DataAccessLayer.OpenDbConnection(ItemConnection, ref argcompanyId))
            {
                foreach (DataCommand command in commandsByConnections)
                {
                    DataTable resultTable;
                    using (var commandItem = db.CreateCommand())
                    {
                        commandItem.CommandType = CommandType.Text;
                        commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                        BuildParameters(command, commandItem);
                        resultTable = DataAccessLayer.QueryExecuteToTable(commandItem, db, CommandBehavior.Default, command.TableName, true, command.Fields);
                        commandItem.Connection = null;
                        commandItem.Dispose();
                    }
                    List<DataType.LookUpValue> resultItems;

                    if (resultTable.Columns.Count >= 2 || resultTable.Columns.Count == 1 && string.Equals(command.LookUp.Code, command.LookUp.Description, StringComparison.CurrentCultureIgnoreCase))
                    {
                        resultItems = new List<DataType.LookUpValue>();

                        if (resultTable.IsNotEmpty() && resultTable.Rows.Count >= 1)
                        {
                            foreach (DataRow Item in resultTable.Rows)
                                resultItems.Add(new DataType.LookUpValue()
                                {
                                    Code = Item.StringValue(resultTable.Columns[command.LookUp.Code].ColumnName).Trim(),
                                    Description = Item.StringValue(resultTable.Columns[command.LookUp.Description].ColumnName).Trim()
                                });
                        }
                    }
                    else
                    {
                        throw new Exceptions.InMotionGITException("The query must have at least two columns for the lookupvalue");
                    }

                    result.Add(new DataType.LookUpPackage() { Key = command.TableName, Items = resultItems });
                }

                db.Close();
                db.Dispose();
            }
            commandsByConnections[0].CompanyId = argcompanyId;
        }
        return result;
    }

    /// <summary>
    /// Retorna el la structura de un sp
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable ProcedureExecuteResultSchema(DataCommand command)
    {
        DataTable result = null;

        using (var db = ConnectionOpenLocal(command))
        {
            using (var commandItem = db.CreateCommand())
            {
                commandItem.CommandType = CommandType.StoredProcedure;
                commandItem.CommandText = DataAccessLayer.PreprocessStatement(command.Statement, db.ToString());
                BuildParameters(command, commandItem);

                if (DataAccessLayer.IsOracle(db.ToString()))
                {
                    commandItem.Parameters.Add(CreateRefCursorParameter(commandItem.CreateParameter()));
                }
                result = DataAccessLayer.QueryExecuteToTable(commandItem, db, CommandBehavior.SchemaOnly, command.TableName, true, command.Fields);
                commandItem.Connection = null;
                commandItem.Dispose();
            }

            db.Close();
            db.Dispose();
        }

        return result;
    }

    #region Tools

    public Contracts.ConnectionStrings ConnectionStringGet(string ConnectionStrinName, int companyId)
    {
        return Helpers.ConnectionStrings.ConnectionStringGet(ConnectionStrinName, companyId);
    }

    public List<Contracts.ConnectionStrings> ConnectionStringGetAll(string ConnectionStrinName, int companyId)
    {
        return Helpers.ConnectionStrings.ConnectionStringGetAll(ConnectionStrinName, companyId);
    }

    public Credential ConnectionStringUserAndPassword(string ConectionStringName, int companyId)
    {
        return Helpers.ConnectionStrings.ConnectionStringUserAndPassword(ConectionStringName, companyId);
    }

    #endregion Tools

    /// <summary>
    /// Obtiene el provider de ConnectionString que se solicita.
    /// </summary>
    /// <param name="repositoryName">Nombre del ConnectionString</param>
    /// <returns></returns>
    public string GetDataBaseProvider(string repositoryName)
    {
        string result = string.Empty;
        try
        {
            int companyDefault = int.MinValue;
            if ("BackOffice.CompanyDefault".AppSettings().IsNotEmpty())
            {
                companyDefault = "BackOffice.CompanyDefault".AppSettings<int>();
            }
            else
            {
                companyDefault = 1;
            }

            var resultConnectionString = Helpers.ConnectionStrings.ConnectionStringGet(repositoryName, companyDefault);
            if (resultConnectionString.IsNotEmpty())
            {
                if (resultConnectionString.ProviderName.ToLower().Contains("ora"))
                {
                    result = "ORACLE";
                }
                else
                {
                    result = resultConnectionString.ProviderName;
                }
            }
            else
            {
                throw Exceptions.ServiceFaultException.Factory(string.Format("ConnectionString '{0}', It's not found", repositoryName));
            }
        }
        catch (FaultException exServices)
        {
            throw exServices;
        }
        catch (Exception ex)
        {
            throw Exceptions.ServiceFaultException.Factory(string.Format("An error occurred while looking up the '{0}' connectionstring", repositoryName), ex);
        }
        return result;
    }

    /// <summary>
    /// Devuelve el valor del appSetting según el nombre del setting
    /// </summary>
    /// <param name="repositoryName">nombre del repositorio</param>
    /// <param name="settingName">nombre del setting</param>
    public string GetSettingValue(string repositoryName, string settingName)
    {
        string result = string.Empty;

        switch (settingName ?? "")
        {
            case "DateFormat":
                {
                    result = ConfigurationManager.AppSettings[string.Format(CultureInfo.InvariantCulture, "Linked.{0}.DateFormat", repositoryName)];
                    break;
                }

            case "NotesFormat":
                {
                    result = ConfigurationManager.AppSettings[string.Format(CultureInfo.InvariantCulture, "Linked.{0}.NotesFormat", repositoryName)];

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.ToUpper(CultureInfo.CurrentCulture);
                    }

                    break;
                }
        }

        return result;
    }

    public string CurrentTime()
    {
        return DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
    }

    public info AppInfo(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            string rootFolder = "WebApplicationPath".AppSettings().ToLower().Replace("webapplication", string.Empty);
            string pathWebApplication = string.Format("{0}{1}", rootFolder, "WebApplication");
            string pathServices = string.Format("{0}{1}", rootFolder, "Services");
            string pathExtencion = "Path.Extensions".AppSettings();
            var root = new info();
            root.Name = rootFolder;
            root.Childs = new List<info>();
            root.Childs.Add(info.Process(pathWebApplication));
            root.Childs.Add(info.Process(pathServices));
            root.Childs.Add(info.Process(pathExtencion));
            return root;
        }
        else
        {
            return info.Process(path);
        }
    }

    /// <summary>
    /// Method que permite realizar check
    /// </summary>
    /// <returns></returns>
    public List<string> Check(Dictionary<string, string> parameters)
    {
        var result = new List<string>();
        var watch = new Stopwatch();
        string message = "";
        watch.Start();
        var connections = ConfigurationManager.ConnectionStrings;
        string[] values = new string[] { "LocalSqlServer", "OraAspNetConString" };

        // Se realiza un chocheo de los connectionsStrings"
        foreach (ConnectionStringSettings item in connections)
        {
            if (values.Count(c => item.Name.Contains(c)) == 0)
            {
                try
                {
                    int argcompanyId = 1;
                    var testConnection = DataAccessLayer.OpenDbConnection(item.Name, ref argcompanyId);
                    if (testConnection.IsNotEmpty())
                    {
                        testConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    result.Add(string.Format("The ConnectionStrings named '{0}' could not be opened.", item.Name));
                }
            }
        }
        watch.Stop();
        message = "{0}              {1}{0}              Id:{2}{0}              Mode:{2}".SpecialFormater(Environment.NewLine, "Check", parameters["Id"], "DataManager.Mode".AppSettings());
        if (parameters.IsNotEmpty())
        {
            message += "              Parámetros:{0}".SpecialFormater(parameters.ToStringExtended()) + Environment.NewLine;
        }
        message += "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
        if (result.Count != 0)
        {
            message += "              Estado de check:{0}".SpecialFormater(string.Join(",", result));
        }
        LogHandler.TraceLog("DataAccessFactory", message);

        return result;
    }
}