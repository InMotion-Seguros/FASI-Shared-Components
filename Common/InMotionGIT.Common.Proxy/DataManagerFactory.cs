using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

#region using

using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Proxy;

#endregion using

/// <summary>
/// DataManagerFactory
/// </summary>
public sealed class DataManagerFactory : IDisposable
{
    #region Private fields, to hold the state of the entity

    private Common.Services.Contracts.DataCommand _command;
    private List<Common.Services.Contracts.DataParameter> _parameters;
    private List<Common.Services.Contracts.DataCommand> _commands;
    private bool _IgnoreMaxNumberOfRecords = false;
    private Common.Services.Contracts.StoredProcedureResult _resultProcedure;
    private int _companyId;
    private string _id;

    #endregion Private fields, to hold the state of the entity

    #region Public properties, to expose the state of the entity

    public bool ForceLocalMode { get; set; }

    public Enumerations.EnumCache Cache { get; set; } = Enumerations.EnumCache.None;

    public bool CacheRefresh { get; set; }

    public string CacheFilter { get; set; }

    public InMotionGIT.Common.Services.Contracts.ConnectionStrings ConnectionStringsRaw { get; set; }

    public string DataManagerURLForce { get; set; }

    public string QueryCount { get; set; }

    public int QueryCountResult { get; set; }

    public int MaxNumberOfRecord { get; set; } = 0;

    public bool AllowHistoryInfo { get; set; } = false;
    public bool AllowHistoryInfo2 { get; set; } = false;

    /// <summary>
    /// Instrucción SQL a ser ejecutada.
    /// </summary>
    public string Statement
    {
        get
        {
            return _command.Statement;
        }

        set
        {
            _command.Statement = value;
        }
    }

    /// <summary>
    /// Identificación de la compañía actual.
    /// </summary>
    public int CompanyId
    {
        get
        {
            return _command.CompanyId;
        }

        set
        {
            _command.CompanyId = value;
        }
    }

    /// <summary>
    /// Identificador de secuencia para id correlations
    /// </summary>
    /// <returns></returns>
    public string Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    #endregion Public properties, to expose the state of the entity

    #region Constructors

    /// <summary>
    /// Instancia la clase de acceso a datos
    /// </summary>
    private DataManagerFactory() : base()
    {
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la cadena de conexión.
    /// </summary>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    public DataManagerFactory(string ConnectionStringName)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = CompanyIdSelect();
        _command = new Common.Services.Contracts.DataCommand()
        {
            ConnectionStringName = ConnectionStringName,
            TableName = "Undefined",
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la cadena de conexión y la identificación de la compañía actual.
    /// </summary>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    /// <param name="companyId">Identificación de la compañía actual.</param>
    public DataManagerFactory(string ConnectionStringName, int companyId)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = companyId;
        _command = new Common.Services.Contracts.DataCommand()
        {
            ConnectionStringName = ConnectionStringName,
            TableName = "Undefined",
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, así como la cadena de conexión.
    /// </summary>
    /// <param name="statement">Instrucción SQL a ser ejecutada.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    public DataManagerFactory(string statement, string connectionStringName)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = CompanyIdSelect();
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = "Undefined",
            Operation = "Query",
            ConnectionStringName = connectionStringName,
            Statement = statement,
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal así como la identificación de la compañía actual.
    /// </summary>
    /// <param name="statement">Instrucción SQL a ser ejecutada.</param>
    /// <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    /// <param name="companyId">Identificación de la compañía actual.</param>
    public DataManagerFactory(string statement, string tableName, int companyId)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = companyId;
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = tableName.IsEmpty() ? "Undefined" : tableName,
            Operation = "Query",
            ConnectionStringName = "BackOfficeConnectionString",
            Statement = statement,
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal así como la cadena de conexión.
    /// </summary>
    /// <param name="statement">Instrucción SQL a ser ejecutada.</param>
    /// <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    public DataManagerFactory(string statement, string tableName, string connectionStringName)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = CompanyIdSelect();
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = tableName.IsEmpty() ? "Undefined" : tableName,
            Operation = "Query",
            ConnectionStringName = connectionStringName,
            Statement = statement,
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal, la cadena de conexión y la identificación de la compañía actual.
    /// </summary>
    /// <param name="statement">Instrucción SQL a ser ejecutada.</param>
    /// <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    /// <param name="companyId">Identificación de la compañía actual.</param>
    public DataManagerFactory(string statement, string tableName, string connectionStringName, int companyId)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = companyId;
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = tableName.IsEmpty() ? "Undefined" : tableName,
            Operation = "Query",
            ConnectionStringName = connectionStringName,
            Statement = statement,
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo el procedimiento almacenado a ser ejecutado y la cadena de conexión.
    /// </summary>
    /// <param name="procedure">Indicador que se desea ejecutar un procedimiento almacenado.</param>
    /// <param name="procedureName">Nombre del procedimiento almacenado a ser ejecutada.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    public DataManagerFactory(bool procedure, string procedureName, string connectionStringName)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = CompanyIdSelect();
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = procedureName,
            Operation = "Procedure",
            ConnectionStringName = connectionStringName,
            Statement = procedureName,
            CompanyId = _companyId
        };
    }

    /// <summary>
    /// Instancia la clase de acceso a datos estableciendo el procedimiento almacenado a ser ejecutado, la cadena de conexión y la identificación de la compañía actual.
    /// </summary>
    /// <param name="procedure">Indicador que se desea ejecutar un procedimiento almacenado.</param>
    /// <param name="procedureName">Nombre del procedimiento almacenado a ser ejecutada.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    /// <param name="companyId">Identificación de la compañía actual.</param>
    public DataManagerFactory(bool procedure, string procedureName, string connectionStringName, int companyId)
    {
        _id = Guid.NewGuid().ToString();
        _companyId = companyId;
        _command = new Common.Services.Contracts.DataCommand()
        {
            TableName = procedureName.IsEmpty() ? "Undefined" : procedureName,
            Operation = "Procedure",
            ConnectionStringName = connectionStringName,
            Statement = procedureName,
            CompanyId = _companyId
        };
    }

    #endregion Constructors

    #region Execute Scalar Methods

    /// <summary>
    /// Método que permite devolver un valor del tipo entero como único resultado de una instrucción 'select'.
    /// </summary>
    /// <returns>Valor del tipo entero</returns>
    public int QueryExecuteScalarToInteger()
    {
        int result = 0;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock1 = new Services.DataManagerClient();
                result = withBlock1.QueryExecuteScalarToIntegerAsync(_command).Result.QueryExecuteScalarToIntegerResult;
            }
        }
        else
        {
            result = DataServiceInstance().QueryExecuteScalarToIntegerAsync(_command).Result.QueryExecuteScalarToIntegerResult;
        }

        TraceLog(watch, local, message, result, "QueryExecuteScalarToInteger");

        return result;
    }

    /// <summary>
    /// Método que permite devolver un valor del tipo decimal como único resultado de una instrucción 'select'.
    /// </summary>
    /// <returns>Valor del tipo decimal</returns>
    public decimal QueryExecuteScalarToDecimal()
    {
        decimal result;
        string resultFiltered = null;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        switch (Cache)
        {
            case Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            result = withBlock1.QueryExecuteScalarToDecimal(_command);
                        }
                    }
                    else
                    {
                        result = DataServiceInstance().QueryExecuteScalarToDecimalAsync(_command).Result.QueryExecuteScalarToDecimalResult;
                    }

                    break;
                }
            case Enumerations.EnumCache.CacheWithFullParameters:
                {
                    string tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCacheString(_command, false);
                    }
                    if (!(tempData == null))
                    {
                        result = Conversions.ToDecimal(tempData);
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                result = withBlock2.QueryExecuteScalarToDecimal(_command);
                            }
                        }
                        else
                        {
                            result = DataServiceInstance().QueryExecuteScalarToDecimalAsync(_command).Result.QueryExecuteScalarToDecimalResult;
                        }
                        QueryCacheAddString(_command, false, result.ToString());
                    }

                    break;
                }
            case Enumerations.EnumCache.CacheWithCommand:
                {
                    string tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCacheString(_command, true);
                    }
                    if (!(tempData == null))
                    {
                        result = Conversions.ToDecimal(tempData);
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                result = withBlock3.QueryExecuteScalarToDecimal(_command);
                            }
                        }
                        else
                        {
                            result = DataServiceInstance().QueryExecuteScalarToDecimalAsync(_command).Result.QueryExecuteScalarToDecimalResult;
                        }
                        QueryCacheAddString(_command, true, result.ToString());
                    }

                    break;
                }
            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    throw new NotImplementedException("The 'CacheOnDemand' not implemented under the method QueryExecuteScalarToDecimal");
                }

            default:
                {
                    throw new NotImplementedException("You have not selected any kind of cache");
                }
        }

        TraceLog(watch, local, message, result, "QueryExecuteScalarToDecimal");

        return result;
    }

    /// <summary>
    /// Método que permite devolver un valor del tipo texto como único resultado de una instrucción 'select'.
    /// </summary>
    /// <returns>Valor del tipo texto</returns>
    public string QueryExecuteScalarToString()
    {
        string result = string.Empty;
        string resultFiltered = null;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        switch (Cache)
        {
            case var @case when @case == Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            result = withBlock1.QueryExecuteScalarToString(_command);
                        }
                    }
                    else
                    {
                        result = DataServiceInstance().QueryExecuteScalarToStringAsync(_command).Result.QueryExecuteScalarToStringResult;
                    }

                    break;
                }
            case var case1 when case1 == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    string tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCacheString(_command, false);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                result = withBlock2.QueryExecuteScalarToString(_command);
                            }
                        }
                        else
                        {
                            result = DataServiceInstance().QueryExecuteScalarToStringAsync(_command).Result.QueryExecuteScalarToStringResult;
                        }
                        QueryCacheAddString(_command, false, result);
                    }

                    break;
                }
            case var case2 when case2 == Enumerations.EnumCache.CacheWithCommand:
                {
                    string tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCacheString(_command, true);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                result = withBlock3.QueryExecuteScalarToString(_command);
                            }
                        }
                        else
                        {
                            result = DataServiceInstance().QueryExecuteScalarToStringAsync(_command).Result.QueryExecuteScalarToStringResult;
                        }
                        QueryCacheAddString(_command, true, result);
                    }

                    break;
                }
            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    throw new NotImplementedException("The 'CacheOnDemand' not implemented under the method QueryExecuteScalarToString");
                }

            default:
                {
                    throw new NotImplementedException("You have not selected any kind of cache");
                }
        }

        TraceLog(watch, local, message, result, "QueryExecuteScalarToString");

        return result;
    }

    /// <summary>
    /// Método que permite devolver un valor del tipo fecha como único resultado de una instrucción 'select'.
    /// </summary>
    /// <returns>Valor del tipo fecha</returns>
    public DateTime QueryExecuteScalarToDate()
    {
        var result = DateTime.MinValue;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock1 = new Common.Services.DataManager();
                result = withBlock1.QueryExecuteScalarToDate(_command);
            }
        }
        else
        {
            result = DataServiceInstance().QueryExecuteScalarToDateAsync(_command).Result.QueryExecuteScalarToDateResult;
        }

        TraceLog(watch, local, message, result, "QueryExecuteScalarToDate");

        return result;
    }

    /// <summary>
    /// Método que permite devolver un valor basado en el tipo genérico como único resultado de una instrucción 'select'.
    /// </summary>
    /// <returns>Valor basado en el tipo genérico</returns>
    public T QueryExecuteScalar<T>()
    {
        object result = null;

        switch (typeof(T).ToString() ?? "")
        {
            case "System.String":
                {
                    result = QueryExecuteScalarToString();
                    break;
                }

            case "System.Int32":
                {
                    result = QueryExecuteScalarToInteger();
                    break;
                }

            case "System.Date":
                {
                    result = QueryExecuteScalarToDate();
                    break;
                }
        }

        return (T)result;
    }

    #endregion Execute Scalar Methods

    #region SQL Statement Execute Methods

    /// <summary>
    /// Ejecuta una instrucción SQL sin esperar un "Record-set" como resultado.
    /// </summary>
    /// <returns>Cantidad de filas afectadas.</returns>
    public int CommandExecute()
    {
        return CommandExecute("Execute");
    }

    /// <summary>
    /// Ejecuta una instrucción SQL sin esperar un "Record-set" como resultado.
    /// </summary>
    /// <param name="operation">Tipo de operación a realizar "Update", "Insert" o "Delete", este valor usado para referencias.</param>
    /// <returns></returns>
    public int CommandExecute(string operation)
    {
        int result = 0;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = operation;

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        SetConfiguration(_command);

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            {
                var withBlock1 = new Common.Services.DataManager();
                result = withBlock1.CommandExecute(_command);
            }
        }
        else
        {
            result = DataServiceInstance().CommandExecuteAsync(_command).Result.CommandExecuteResult;
        }

        TraceLog(watch, local, message, result, "CommandExecute");

        return result;
    }

    /// <summary>
    /// Ejecuta una instrucción SQL esperando un "Record-set" como resultado.
    /// </summary>
    /// <returns>"Record-set" en forma de "DataTable".</returns>
    public DataTable QueryExecuteToTable()
    {
        return QueryExecuteToTable(false);
    }

    public List<string> Check()
    {
        var result = new List<string>();
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            {
                var withBlock = new Common.Services.DataManager();
                result = withBlock.Check(Fields());
            }
        }
        else
        {
            result = DataServiceInstance().CheckAsync(Convert(Fields())).Result.CheckResult.ToList();
        }

        TraceLog(watch, local, message, result, "Check");

        return result;
    }

    public bool OpenConnection(string ConnectionStringName)
    {
        bool result = false;
        {
            var withBlock = new Common.Services.DataManager();
            var con = withBlock.ConnectionOpenLocal(ConnectionStringName);
            if (con.IsNotEmpty())
            {
                result = true;
            }
            con.Close();
        }
        return result;
    }

    /// <summary>
    /// Ejecuta una instrucción SQL esperando un "Record-set" como resultado.
    /// </summary>
    /// <param name="resultEmpty">Indica que para el caso donde no exista ningún "Record-set" de igual forma se devuelva una instancia vacía de una "DataTable".</param>
    /// <returns>"Record-set" en forma de "DataTable".</returns>
    public DataTable QueryExecuteToTable(bool resultEmpty)
    {
        DataTable result = null;
        string message = string.Empty;
        Stopwatch watch = null;
        bool fromCache = false;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        var QueryResult = new Common.Services.Contracts.QueryResult();
        DataTable resultFiltered = null;
        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        switch (Cache)
        {
            case var @case when @case == Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            QueryResult = withBlock1.QueryExecuteToTable(_command, resultEmpty);
                        }
                    }
                    else
                    {
                        using (var dataService = DataServiceInstance())
                        {
                            QueryResult = dataService.QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                        }
                    }

                    break;
                }
            case var case1 when case1 == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, false);
                    }
                    if (!(tempData == null))
                    {
                        QueryResult.Table = tempData;
                        fromCache = true;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                QueryResult = withBlock2.QueryExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                        }
                        QueryCacheAdd(_command, false, QueryResult.Table);
                    }
                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                    }

                    break;
                }

            case var case2 when case2 == Enumerations.EnumCache.CacheWithCommand:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, true);
                    }
                    if (!(tempData == null))
                    {
                        QueryResult.Table = tempData;
                        fromCache = true;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                QueryResult = withBlock3.QueryExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                        }
                        QueryCacheAdd(_command, true, QueryResult.Table);
                    }
                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                    }

                    break;
                }

            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    OndemandContainer tempOndemandContainer = null;
                    if (!CacheRefresh)
                    {
                        var temporalCache = IsExistQueryCacheOnDemand(_command);
                        if (temporalCache == null)
                        {
                            tempOndemandContainer = temporalCache;
                            fromCache = true;
                        }
                        else if (typeof(OndemandContainer) == IsExistQueryCacheOnDemand(_command).GetType())
                        {
                            tempOndemandContainer = temporalCache;
                            fromCache = true;
                        }
                        else
                        {
                            tempOndemandContainer = null;
                        }
                    }
                    if (!(tempOndemandContainer == null))
                    {
                        var isfund = tempOndemandContainer.KeyWichtParameters.Select(c => c.Contains(GetMd5Hash(_command))).FirstOrDefault();
                        if (!(isfund == null))
                        {
                            QueryResult.Table = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                            }
                        }
                        else
                        {
                            if (local)
                            {
                                {
                                    var withBlock4 = new Common.Services.DataManager();
                                    QueryResult = withBlock4.QueryExecuteToTable(_command, resultEmpty);
                                }
                            }
                            else
                            {
                                QueryResult = DataServiceInstance().QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                            }
                            QueryCacheAddOnDemand(_command, true, QueryResult.Table);
                            QueryResult.Table = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                            }
                        }
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock5 = new Common.Services.DataManager();
                                QueryResult = withBlock5.QueryExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                        }
                        QueryCacheAddOnDemand(_command, true, QueryResult.Table);
                    }

                    break;
                }

            default:
                {
                    if (local)
                    {
                        {
                            var withBlock6 = new Common.Services.DataManager();
                            QueryResult = withBlock6.QueryExecuteToTable(_command, resultEmpty);
                        }
                    }
                    else
                    {
                        QueryResult = DataServiceInstance().QueryExecuteToTableAsync(_command, resultEmpty).Result.QueryExecuteToTableResult;
                    }

                    break;
                }
        }

        QueryCountResult = QueryResult.QueryCountResult;
        if (!(resultFiltered == null))
        {
            if (!(QueryResult.Table == null))
            {
                QueryResult.Table.ReadOnlyMode(false);
            }
            if (!(resultFiltered == null))
            {
                resultFiltered.ReadOnlyMode(false);
            }
            result = resultFiltered;
        }
        else
        {
            if (!(QueryResult.Table == null))
            {
                QueryResult.Table.ReadOnlyMode(false);
            }
            result = QueryResult.Table;
        }

        TraceLog(watch, local, message, result, "QueryExecuteToTable");

        return result;
    }

    /// <summary>
    /// Ejecuta una instrucción SQL del tipo definición de estructura.
    /// </summary>
    /// <param name="statement">Instrucción SQL del tipo definición de estructura.</param>
    /// <returns>Resultado de la ejecución.</returns>
    public string DataStructure(string statement)
    {
        string result = "Ok";
        string message = string.Empty;
        Stopwatch watch = null;

        {
            ref var withBlock = ref _command;
            withBlock.Statement = statement;
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.Fields = Fields();
        }

        bool local = true;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock1 = new Common.Services.DataManager();
                result = withBlock1.DataStructure(_command);
            }
        }
        else
        {
            result = DataServiceInstance().DataStructureAsync(_command).Result.DataStructureResult;
        }

        TraceLog(watch, local, message, result, "DataStructure");

        return result;
    }

    /// <summary>
    /// Retorna la instrucción SQL con el reemplazo de valores para cada parámetro.
    /// </summary>
    /// <param name="operation">Tipo de operación a realizar "Update", "Insert" o "Delete", este valor usado para referencias.</param>
    /// <returns>Instrucción SQL.</returns>
    public string ResolveStatement(string operation)
    {
        string result = "Ok";

        {
            ref var withBlock = ref _command;
            withBlock.Operation = operation;
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock1 = new Common.Services.DataManager();
                result = withBlock1.ResolveStatement(_command);
            }
        }
        else
        {
            result = DataServiceInstance().ResolveStatementAsync(_command).Result.ResolveStatementResult;
        }
        return result;
    }

    #endregion SQL Statement Execute Methods

    #region Procedures Execute Methods

    /// <summary>
    /// Ejecuta un procedimiento almacenado esperando un "Record-set" como resultado.
    /// </summary>
    /// <returns>"Record-set" en forma de "DataTable".</returns>
    public DataTable ProcedureExecuteToTable()
    {
        return ProcedureExecuteToTable(false);
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado esperando un "Record-set" como resultado.
    /// </summary>
    /// <param name="resultEmpty">Indica que para el caso donde no exista ningún "Record-set" de igual forma se devuelva una instancia vacía de una "DataTable".</param>
    /// <returns>"Record-set" en forma de "DataTable".</returns>
    public DataTable ProcedureExecuteToTable(bool resultEmpty)
    {
        var QueryResult = new Common.Services.Contracts.QueryResult();
        DataTable resultFiltered = null;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "ProcedureToTable";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }

            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        switch (Cache)
        {
            case var @case when @case == Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            QueryResult = withBlock1.ProcedureExecuteToTable(_command, resultEmpty);
                        }
                    }
                    else
                    {
                        QueryResult = DataServiceInstance().ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                    }

                    break;
                }

            case var case1 when case1 == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    DataTable tempData = null;

                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, false);
                    }

                    if (!(tempData == null))
                    {
                        QueryResult.Table = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                QueryResult = withBlock2.ProcedureExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                        }

                        QueryCacheAdd(_command, false, QueryResult.Table);
                    }

                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                    }

                    break;
                }

            case var case2 when case2 == Enumerations.EnumCache.CacheWithCommand:
                {
                    DataTable tempData = null;

                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, true);
                    }

                    if (!(tempData == null))
                    {
                        QueryResult.Table = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                QueryResult = withBlock3.ProcedureExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                        }

                        QueryCacheAdd(_command, true, QueryResult.Table);
                    }

                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                    }

                    break;
                }

            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    OndemandContainer tempOndemandContainer = null;

                    if (!CacheRefresh)
                    {
                        var temporalCache = IsExistQueryCacheOnDemand(_command);

                        if (temporalCache == null)
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else if (typeof(OndemandContainer) == IsExistQueryCacheOnDemand(_command).GetType())
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else
                        {
                            tempOndemandContainer = null;
                        }
                    }

                    if (!(tempOndemandContainer == null))
                    {
                        var isfund = tempOndemandContainer.KeyWichtParameters.Select(c => c.Contains(GetMd5Hash(_command))).FirstOrDefault();
                        if (!(isfund == null))
                        {
                            QueryResult.Table = tempOndemandContainer.Data;

                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                            }
                        }
                        else
                        {
                            if (local)
                            {
                                {
                                    var withBlock4 = new Common.Services.DataManager();
                                    QueryResult = withBlock4.ProcedureExecuteToTable(_command, resultEmpty);
                                }
                            }
                            else
                            {
                                QueryResult = DataServiceInstance().ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                            }

                            QueryCacheAddOnDemand(_command, true, QueryResult.Table);
                            QueryResult.Table = tempOndemandContainer.Data;

                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters);
                            }
                        }
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock5 = new Common.Services.DataManager();
                                QueryResult = withBlock5.ProcedureExecuteToTable(_command, resultEmpty);
                            }
                        }
                        else
                        {
                            QueryResult = DataServiceInstance().ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                        }

                        QueryCacheAddOnDemand(_command, true, QueryResult.Table);
                    }

                    break;
                }

            default:
                {
                    if (local)
                    {
                        {
                            var withBlock6 = new Common.Services.DataManager();
                            QueryResult = withBlock6.ProcedureExecuteToTable(_command, resultEmpty);
                        }
                    }
                    else
                    {
                        using (var dataService = DataServiceInstance())
                        {
                            QueryResult = dataService.ProcedureExecuteToTableAsync(_command, resultEmpty).Result.ProcedureExecuteToTableResult;
                        }
                    }

                    break;
                }
        }

        if (!(QueryResult == null) && !(QueryResult.OutputParameters == null) && QueryResult.OutputParameters.Count > 0)
        {
            string key = string.Empty;
            Common.Services.Contracts.DataParameter temporalParameter = default;

            foreach (KeyValuePair<string, object> parameterData in QueryResult.OutputParameters)
            {
                key = parameterData.Key;
                if (!(_parameters == null))
                {
                    temporalParameter = (from itemParameter in _parameters
                                         where itemParameter.Name.Equals(key)
                                         select itemParameter).SingleOrDefault();

                    if (!(temporalParameter == null))
                    {
                        temporalParameter.Value = parameterData.Value;
                        Common.Helpers.LogHandler.TraceLog("Proxy - Nelson " + key + " = ", parameterData.Value.ToString());
                    }
                }
            }
        }

        QueryCountResult = QueryResult.QueryCountResult;

        if (!(resultFiltered == null))
        {
            if (!(QueryResult.Table == null))
            {
                QueryResult.Table.ReadOnlyMode(false);
            }

            if (!(resultFiltered == null))
            {
                resultFiltered.ReadOnlyMode(false);
            }
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteToTable");
            return resultFiltered;
        }
        else
        {
            if (!(QueryResult.Table == null))
            {
                QueryResult.Table.ReadOnlyMode(false);
            }
            TraceLog(watch, local, message, QueryResult.Table, "ProcedureExecuteToTable");
            return QueryResult.Table;
        }
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado para obtener la estructura de un "Record-set" como resultado. Operación especial de uso interno.
    /// </summary>
    /// <returns>"Record-set" en forma de "DataTable".</returns>
    public DataTable ProcedureExecuteResultSchema()
    {
        DataTable result = null;
        DataTable resultFiltered = null;
        string message = string.Empty;
        Stopwatch watch = null;
        {
            ref var withBlock = ref _command;
            withBlock.Operation = "ProcedureToTable";
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
        }

        bool local = true;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        switch (Cache)
        {
            case var @case when @case == Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            result = withBlock1.ProcedureExecuteResultSchema(_command);
                        }
                    }
                    else
                    {
                        using (var dataService = DataServiceInstance())
                        {
                            result = dataService.ProcedureExecuteResultSchemaAsync(_command).Result.ProcedureExecuteResultSchemaResult;
                        }
                    }

                    break;
                }
            case var case1 when case1 == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, false);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                result = withBlock2.ProcedureExecuteResultSchema(_command);
                            }
                        }
                        else
                        {
                            using (var dataService = DataServiceInstance())
                            {
                                result = dataService.ProcedureExecuteResultSchemaAsync(_command).Result.ProcedureExecuteResultSchemaResult;
                            }
                        }
                        QueryCacheAdd(_command, false, result);
                    }
                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                    }

                    break;
                }
            case var case2 when case2 == Enumerations.EnumCache.CacheWithCommand:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, true);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                result = withBlock3.ProcedureExecuteResultSchema(_command);
                            }
                        }
                        else
                        {
                            using (var dataService = DataServiceInstance())
                            {
                                result = dataService.ProcedureExecuteResultSchema(_command);
                            }
                        }
                        QueryCacheAdd(_command, true, result);
                    }
                    if (!string.IsNullOrEmpty(CacheFilter))
                    {
                        resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                    }

                    break;
                }

            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    OndemandContainer tempOndemandContainer = null;
                    if (!CacheRefresh)
                    {
                        var temporalCache = IsExistQueryCacheOnDemand(_command);
                        if (temporalCache == null)
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else if (typeof(OndemandContainer) == IsExistQueryCacheOnDemand(_command).GetType())
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else
                        {
                            tempOndemandContainer = null;
                        }
                    }
                    if (!(tempOndemandContainer == null))
                    {
                        var isfund = tempOndemandContainer.KeyWichtParameters.Select(c => c.Contains(GetMd5Hash(_command))).FirstOrDefault();
                        if (!(isfund == null))
                        {
                            result = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                            }
                        }
                        else
                        {
                            if (local)
                            {
                                {
                                    var withBlock4 = new Common.Services.DataManager();
                                    result = withBlock4.ProcedureExecuteResultSchema(_command);
                                }
                            }
                            else
                            {
                                using (var dataService = DataServiceInstance())
                                {
                                    result = dataService.ProcedureExecuteResultSchema(_command);
                                }
                            }
                            QueryCacheAddOnDemand(_command, true, result);
                            result = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                            }
                        }
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock5 = new Common.Services.DataManager();
                                result = withBlock5.ProcedureExecuteResultSchema(_command);
                            }
                        }
                        else
                        {
                            using (var dataService = DataServiceInstance())
                            {
                                result = dataService.ProcedureExecuteResultSchema(_command);
                            }
                        }
                        QueryCacheAddOnDemand(_command, true, result);
                    }

                    break;
                }

            default:
                {
                    if (local)
                    {
                        {
                            var withBlock6 = new Common.Services.DataManager();
                            result = withBlock6.ProcedureExecuteResultSchema(_command);
                        }
                    }
                    else
                    {
                        using (var dataService = DataServiceInstance())
                        {
                            result = dataService.ProcedureExecuteResultSchema(_command);
                        }
                    }

                    break;
                }
        }

        if (!(resultFiltered == null))
        {
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteResultSchema");
            return resultFiltered;
        }
        else
        {
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteResultSchema");
            return result;
        }

        return result;
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado sin esperar un "Record-set" como resultado.
    /// </summary>
    /// <returns>Cantidad de filas afectadas.</returns>
    public int ProcedureExecute()
    {
        int resultRowAffected = 0;
        Common.Services.Contracts.StoredProcedureResult result;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Procedure";

            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock1 = new Common.Services.DataManager();
                result = withBlock1.ProcedureExecute(_command);
                resultRowAffected = result.RowAffected;
                _resultProcedure = result;
            }
        }
        else
        {
            using (var dataService = DataServiceInstance())
            {
                result = dataService.ProcedureExecuteAsync(_command).Result.ProcedureExecuteResult;
                resultRowAffected = result.RowAffected;
                _resultProcedure = result;
            }
        }

        if (!(result == null))
        {
            string key = string.Empty;
            foreach (var itemResult in result.OutParameter)
            {
                key = Conversions.ToString(itemResult.Key);
                var tempParameer = (from itemParamter in _parameters
                                    where itemParamter.Name.Equals(key)
                                    select itemParamter).SingleOrDefault();
                if (!(tempParameer == null))
                {
                    tempParameer.Value = itemResult.Value;
                }
            }
        }

        TraceLog(watch, local, message, resultRowAffected, "ProcedureExecute");

        return resultRowAffected;
    }

    #endregion Procedures Execute Methods

    #region Parameters Methods

    public void AddParameter(string name, DbType type, int size, bool isNull, object value, ParameterDirection direction)
    {
        if (_parameters.IsEmpty())
        {
            _parameters = new List<Common.Services.Contracts.DataParameter>();
        }

        _parameters.Add(new Common.Services.Contracts.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = direction });
    }

    public void AddParameter(string name, DbType type, int size, bool isNull, object value)
    {
        if (_parameters.IsEmpty())
        {
            _parameters = new List<Common.Services.Contracts.DataParameter>();
        }

        _parameters.Add(new Common.Services.Contracts.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = ParameterDirection.Input });
    }

    public Common.Services.Contracts.DataParameter ParameterByName(string name)
    {
        Common.Services.Contracts.DataParameter result = default;

        foreach (var parameter in _parameters)
        {
            if (string.Equals(parameter.Name, name, StringComparison.CurrentCultureIgnoreCase))
            {
                result = parameter;
                break;
            }
        }

        return result;
    }

    #endregion Parameters Methods

    #region Tools

    /// <summary>
    /// Indica si la cadena de conexión usa un proveedor del tipo "Oracle".
    /// </summary>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    /// <returns>Verdadero en caso de ser tipo "Oracle", falso en caso contrario.</returns>
    public static bool ConnectionStringIsOracle(string ConnectionStringName)
    {
        bool isOracle = false;
        string temporalConnectionString = new DataManagerFactory("").DataServiceInstance().GetDataBaseProviderAsync(ConnectionStringName).Result.GetDataBaseProviderResult;
        if (temporalConnectionString.IsNotEmpty())
        {
            if (temporalConnectionString.ToLower().Contains("Oracle".ToLower()))
            {
                isOracle = true;
            }
            else
            {
                isOracle = false;
            }
        }
        else
        {
            throw new Exceptions.InMotionGITException(string.Format("ConnectionString '{0}', It's not found", ConnectionStringName));
        }
        return isOracle;
    }

    /// <summary>
    /// Permite hacer el mapeo de forma automática entre un datatable y una clase, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    /// </summary>
    /// <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    /// <param name="dr">Datatable usando como fuentes de datos</param>
    /// <returns>Instancia de la clase T con la información de la primera fila del datatable</returns>
    public static T Mapper<T>(DataTable dr) where T : new()
    {
        var businessEntityType = typeof(T);
        var entitys = new List<T>();
        var hashtable = new Hashtable();
        PropertyInfo[] properties = businessEntityType.GetProperties();
        PropertyInfo info;
        T newObject = default;

        foreach (var currentInfo in properties)
        {
            info = currentInfo;
            hashtable[info.Name.ToUpper()] = info;
        }

        if (dr.Rows.Count > 0)
        {
            var row = dr.Rows[0];
            newObject = new T();
            foreach (DataColumn column in dr.Columns)
            {
                if (!(row[column.ColumnName] is DBNull))
                {
                    info = (PropertyInfo)hashtable[column.ColumnName.ToUpper()];
                    if (info is not null && info.CanWrite)
                    {
                        if (ReferenceEquals(info.PropertyType, typeof(string)))
                        {
                            info.SetValue(newObject, row[column.ColumnName].ToString().Trim(), null);
                        }
                        else
                        {
                            info.SetValue(newObject, row[column.ColumnName], null);
                        }
                    }
                }
            }
        }
        return newObject;
    }

    /// <summary>
    /// Permite hacer el mapeo de forma automática entre un datatable y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    /// </summary>
    /// <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    /// <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    /// <param name="dr">Datatable usando como fuentes de datos</param>
    /// <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    public static Y Mapper<T, Y>(DataTable dr)
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
                        if (ReferenceEquals(info.PropertyType, typeof(string)))
                        {
                            info.SetValue(newObject, row[column.ColumnName].ToString().Trim(), null);
                        }
                        else
                        {
                            info.SetValue(newObject, row[column.ColumnName], null);
                        }
                    }
                }
            }
            ientitys.Add(newObject);
        }

        return entitys;
    }

    /// <summary>
    /// Permite hacer el mapeo de forma automática entre un data-table y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    /// </summary>
    /// <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    /// <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    /// <param name="ExplicitPropertyMapping">Se definen qué propiedades deben apuntar de manera explícita a la columna,la relación es columna con propiedad</param>
    /// <param name="dr">Data-table usando como fuentes de datos</param>
    /// <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    public static Y Mapper<T, Y>(DataTable dr, Dictionary<string, string> ExplicitPropertyMapping)
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

        var newExplicitPropertyMapping = new Dictionary<string, string>();

        foreach (var Item in ExplicitPropertyMapping.Keys)
            newExplicitPropertyMapping.Add(Item.ToUpper(), ExplicitPropertyMapping[Item].ToUpper());

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
                    if (!newExplicitPropertyMapping.ContainsKey(column.ColumnName.ToUpper()))
                    {
                        info = (PropertyInfo)hashtable[column.ColumnName.ToUpper()];
                    }
                    else
                    {
                        info = (PropertyInfo)hashtable[newExplicitPropertyMapping[column.ColumnName.ToUpper()]];
                    }
                    if (info is not null && info.CanWrite)
                    {
                        if (ReferenceEquals(info.PropertyType, typeof(string)))
                        {
                            info.SetValue(newObject, row[column.ColumnName].ToString().Trim(), null);
                        }
                        else
                        {
                            info.SetValue(newObject, row[column.ColumnName], null);
                        }
                    }
                }
            }
            ientitys.Add(newObject);
        }

        return entitys;
    }

    /// <summary>
    /// Permite hacer el mapeo de forma automática entre un data-table y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    /// </summary>
    /// <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    /// <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    /// <param name="ExplicitPropertyType">Se definen qué propiedades deben apuntar de manera explícita a la columna,la relación es columna con propiedad</param>
    /// <param name="dr">Data-table usando como fuentes de datos</param>
    /// <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    public static Y Mapper<T, Y>(DataTable dr, Dictionary<string, Type> ExplicitPropertyType)
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

        var newExplicitPropertyType = new Dictionary<string, Type>();

        foreach (var Item in ExplicitPropertyType.Keys)
            newExplicitPropertyType.Add(Item.ToUpper(), ExplicitPropertyType[Item]);

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
                        if (ReferenceEquals(info.PropertyType, typeof(string)))
                        {
                            info.SetValue(newObject, row[column.ColumnName].ToString().Trim(), null);
                        }
                        else if (ReferenceEquals(info.PropertyType, typeof(bool)))
                        {
                            if (!newExplicitPropertyType.ContainsKey(column.ColumnName.ToUpper()))
                            {
                                info.SetValue(newObject, row[column.ColumnName], null);
                            }
                            else
                            {
                                bool value = row[column.ColumnName].ToString().ToLower().Equals("y");
                                info.SetValue(newObject, value, null);
                            }
                        }
                        else
                        {
                            info.SetValue(newObject, row[column.ColumnName], null);
                        }
                    }
                }
            }
            ientitys.Add(newObject);
        }

        return entitys;
    }

    /// <summary>
    /// Retorna el nombre del proveedor de ADO.Net usado por un cadena de conexión.
    /// </summary>
    /// <param name="connectionStringName">Nombre de la cadena de conexión</param>
    /// <returns>Nombre del proveedor </returns>
    public static string GetDataBaseProvider(string connectionStringName)
    {
        Common.Services.DataManager dataService = null;
        string result;

        if ("DataManager.URL".AppSettings().IsEmpty())
        {
            dataService = new Common.Services.DataManager();
        }
        else
        {
            dataService = InstanceDataManagerClient("DataManager.URL".AppSettings());
        }

        result = dataService.GetDataBaseProvider(connectionStringName);
        dataService.Close();

        return result;
    }

    /// <summary>
    /// Retorna la identificación de la compañía actual en caso de que el sistema este configurado multi-compañía.
    /// </summary>
    /// <returns>Identificación de la compañía.</returns>
    public int CompanyIdSelect(IHttpContextAccessor httpContextAccessor)
    {
        int Result = "BackOffice.CompanyDefault".AppSettings<int>();
        if ("BackOffice.IsMultiCompany".AppSettings<bool>())
        {
            if (!(HttpContext.Current == null))
            {
                if (!(HttpContext.Current.Session == null) && Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(HttpContext.Current.Session["CompanyId"], string.Empty, false)))
                {
                    Result = Conversions.ToInteger(HttpContext.Current.Session["CompanyId"]);
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// Permite verificar la existencia de un objeto a nivel de la base de datos.
    /// </summary>
    /// <param name="owner">Propietario del objecto.</param>
    /// <param name="type">Tipo de objeto.</param>
    /// <param name="name">Nombre de objeto.</param>
    /// <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    /// <returns>Verdadero en caso de existir el objeto, falso en caso contrario.</returns>
    public static bool ObjectExist(string owner, string type, string name, string ConnectionStringName)
    {
        bool local = true;
        bool result;

        if (string.IsNullOrEmpty(owner))
        {
            owner = "INSUDB";
        }

        var _command = new Common.Services.Contracts.DataCommand()
        {
            ConnectionStringName = ConnectionStringName,
            Owner = owner,
            ObjectType = type,
            TableName = name
        };

        if ("DataManager.Mode".AppSettingsOnEquals("remote"))
        {
            local = false;
        }

        if (local)
        {
            {
                var withBlock = new Common.Services.DataManager();
                result = withBlock.ObjectExist(_command);
            }
        }
        else
        {
            using (var dataService = new DataManagerFactory().DataServiceInstance())
            {
                result = dataService.ObjectExistAsync(_command).Result.ObjectExistResult;
            }
        }

        return result;
    }

    #endregion Tools

    #region IDisposable Support

    private bool disposedValue; // To detect redundant calls

    /// <summary>
    /// Implantación de la interface “IDisposable”.
    /// </summary>
    /// <param name="disposing">Usado para detectar llamada redundantes.</param>
    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _command = null;
                _parameters = null;
                _commands = null;
                _resultProcedure = null;
                ConnectionStringsRaw = default;
            }
        }
        disposedValue = true;
    }

    /// <summary>
    /// Implantación de la interface “IDisposable”.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support

    #region Cache

    private bool QueryCacheAdd(Common.Services.Contracts.DataCommand command, bool OnlyCommand, DataType.LookUpPackage data)
    {
        bool Result = default;
        string hash = "";

        if (OnlyCommand)
        {
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            hash = GetMd5Hash(command);
        }

        if (!Common.Helpers.Caching.Exist(hash))
        {
            Common.Helpers.Caching.SetItem(hash, data);
        }

        return Result;
    }

    /// <summary>
    /// Add the hash of the query and the result thereof
    /// </summary>
    /// <param name="command">Command executed</param>
    /// <param name="data">Result of the query executed</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private bool QueryCacheAdd(Common.Services.Contracts.DataCommand command, bool OnlyCommand, DataTable data)
    {
        bool Result = default;
        string hash = "";
        if (OnlyCommand)
        {
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            hash = GetMd5Hash(command);
        }
        if (!Common.Helpers.Caching.Exist(hash))
        {
            Common.Helpers.Caching.SetItem(hash, data);
        }
        return Result;
    }

    /// <summary>
    /// Add the hash of the query and the result thereof
    /// </summary>
    /// <param name="command">Command executed</param>
    /// <param name="data">Result of the query executed</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private bool QueryCacheListAdd(Common.Services.Contracts.DataCommand command, bool OnlyCommand, List<DataType.LookUpValue> data)
    {
        bool Result = default;
        string hash = "";
        if (OnlyCommand)
        {
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            hash = GetMd5Hash(command);
        }
        if (!Common.Helpers.Caching.Exist(hash))
        {
            Common.Helpers.Caching.SetItem(hash, data);
        }
        return Result;
    }

    /// <summary>
    /// Add the hash of the query and the result thereof
    /// </summary>
    /// <param name="command">Command executed</param>
    /// <param name="data">Result of the query executed</param>
    private void QueryCacheAddString(Common.Services.Contracts.DataCommand command, bool OnlyCommand, string data)
    {
        if (data.IsNotEmpty())
        {
            string hash = string.Empty;

            if (OnlyCommand)
            {
                hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
            }
            else
            {
                hash = GetMd5Hash(command);
            }
            if (!Common.Helpers.Caching.Exist(hash))
            {
                Common.Helpers.Caching.SetItem(hash, data);
            }
        }
    }

    /// <summary>
    /// Add the hash of the query and the result thereof
    /// </summary>
    /// <param name="command">Command executed</param>
    /// <param name="data">Result of the query executed</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private bool QueryCacheAddOnDemand(Common.Services.Contracts.DataCommand command, bool OnlyCommand, DataTable data)
    {
        bool Result = default;
        string hashCommandText = GetMd5Hash(command.ConnectionStringName + command.Statement);
        string hashCommand = GetMd5Hash(command);
        OndemandContainer tempOndemand;
        if (!Common.Helpers.Caching.Exist(hashCommandText))
        {
            tempOndemand = new OndemandContainer();
            {
                ref var withBlock = ref tempOndemand;
                withBlock.Key = hashCommandText;
                withBlock.Data = data;
                withBlock.KeyWichtParameters.Add(hashCommand);
            }
            Common.Helpers.Caching.SetItem(hashCommandText, tempOndemand);
        }
        else
        {
            tempOndemand = Common.Helpers.Caching.GetItem(hashCommandText) as OndemandContainer;
            if (!(tempOndemand == null))
            {
                string isfund = (from itemfound in tempOndemand.KeyWichtParameters
                                 where itemfound.Contains(hashCommand)
                                 select itemfound).SingleOrDefault();
                if (isfund == null)
                {
                    if (!(tempOndemand.Data == null))
                    {
                        foreach (DataRow ItemRow in data.Rows)
                            tempOndemand.Data.ImportRow(ItemRow);
                    }
                    tempOndemand.KeyWichtParameters.Add(hashCommand);
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// If there is a query that is executed in cache
    /// </summary>
    /// <param name="command">Executing query</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private DataTable IsExistQueryCache(Common.Services.Contracts.DataCommand command, bool OnlyCommand)
    {
        DataTable Result = null;
        string hash = MD5Command(command, OnlyCommand);
        if (Common.Helpers.Caching.Exist(hash))
        {
            Result = (DataTable)Common.Helpers.Caching.GetItem(hash);
        }
        return Result;
    }

    /// <summary>
    /// If there is a lookup that is executed in cache
    /// </summary>
    /// <param name="command">Executing query</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private InMotionGIT.Common.DataType.LookUpPackage IsExistLookUpCache(Common.Services.Contracts.DataCommand command, bool OnlyCommand)
    {
        Common.DataType.LookUpPackage result = default;
        string hash = MD5Command(command, OnlyCommand);
        if (Common.Helpers.Caching.Exist(hash))
        {
            result = (Common.DataType.LookUpPackage)Common.Helpers.Caching.GetItem(hash);
        }
        return result;
    }

    private string MD5Command(Common.Services.Contracts.DataCommand command, bool OnlyCommand)
    {
        string result = "";
        if (OnlyCommand)
        {
            result = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            result = GetMd5Hash(command);
        }
        return result;
    }

    /// <summary>
    /// If there is a query that is executed in cache
    /// </summary>
    /// <param name="command">Executing query</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private List<DataType.LookUpValue> IsExistQueryCacheList(Common.Services.Contracts.DataCommand command, bool OnlyCommand)
    {
        List<DataType.LookUpValue> Result = null;
        string hash = "";
        if (OnlyCommand)
        {
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            hash = GetMd5Hash(command);
        }
        if (Common.Helpers.Caching.Exist(hash))
        {
            Result = (List<DataType.LookUpValue>)Common.Helpers.Caching.GetItem(hash);
        }
        return Result;
    }

    /// <summary>
    /// If there is a query that is executed in cache
    /// </summary>
    /// <param name="command">Executing query</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private string IsExistQueryCacheString(Common.Services.Contracts.DataCommand command, bool OnlyCommand)
    {
        string Result = null;
        string hash = "";
        if (OnlyCommand)
        {
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement);
        }
        else
        {
            hash = GetMd5Hash(command);
        }
        if (Common.Helpers.Caching.Exist(hash))
        {
            Result = (string)Common.Helpers.Caching.GetItem(hash);
        }
        return Result;
    }

    /// <summary>
    /// IsExistQueryCacheOnDemand
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    private OndemandContainer IsExistQueryCacheOnDemand(Common.Services.Contracts.DataCommand command)
    {
        OndemandContainer Result = null;
        string hashCommandTesx = GetMd5Hash(command.ConnectionStringName + command.Statement);
        string hashCommand = GetMd5Hash(command);
        if (Common.Helpers.Caching.Exist(hashCommandTesx))
        {
            Result = (OndemandContainer)Common.Helpers.Caching.GetItem(hashCommandTesx);
        }
        return Result;
    }

    /// <summary>
    /// MD5 generator to run the query
    /// </summary>
    /// <param name="command">Query</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string GetMd5Hash(Common.Services.Contracts.DataCommand command)
    {
        using (var md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(command.ConnectionStringName + InMotionGIT.Common.Helpers.Serializer.SerializeJSON<Common.Services.Contracts.DataCommand>(command)));
            var sBuilder = new StringBuilder();
            int i;
            var loopTo = data.Length - 1;
            for (i = 0; i <= loopTo; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }
    }

    /// <summary>
    /// Sobre carga de contructor para string
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string GetMd5Hash(string command)
    {
        using (var md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(command));
            var sBuilder = new StringBuilder();
            int i;
            var loopTo = data.Length - 1;
            for (i = 0; i <= loopTo; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }
    }

    private static bool VerifyMd5Hash(object input, string hash)
    {
        string hashOfInput = GetMd5Hash(Conversions.ToString(input));
        var comparer = StringComparer.OrdinalIgnoreCase;
        if (0 == comparer.Compare(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Importart datos de otro datatable
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="pFilter"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable DataTableImport(DataTable Data, string pFilter, List<Common.Services.Contracts.DataParameter> _parameters)
    {
        DataTable resultFiltered = null;
        pFilter = ProcessFilter(pFilter, _parameters);
        DataRow[] row = Data.Select(pFilter);
        if (row.Count() != 0)
        {
            resultFiltered = Data.Copy();
            resultFiltered.Clear();
            foreach (var ItemRow in row)
                resultFiltered.ImportRow(ItemRow);
        }
        return resultFiltered;
    }

    private string ProcessFilter(string pFilter, List<Common.Services.Contracts.DataParameter> _parameters)
    {
        string Result = pFilter;
        if (!string.IsNullOrEmpty(pFilter) && !(_parameters == null))
        {
            foreach (var ItemParameters in _parameters)
            {
                switch (ItemParameters.Type)
                {
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.Guid:
                    case DbType.String:
                    case DbType.StringFixedLength:
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        {
                            Result = Result.Replace("@:" + ItemParameters.Name, string.Format("'{0}'", ItemParameters.Value.ToString().Trim()));
                            break;
                        }

                    default:
                        {
                            Result = Result.Replace("@:" + ItemParameters.Name, ItemParameters.Value.ToString());
                            break;
                        }
                }
            }
        }
        return Result;
    }

    #endregion Cache

    #region Package Commands

    /// <summary>
    /// AddCommand
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="tableName"></param>
    /// <param name="operation"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public Common.Services.Contracts.DataCommand AddCommand(string statement, string tableName, string operation)
    {
        return AddCommand(statement, tableName, operation, string.Empty);
    }

    /// <summary>
    /// AddCommand
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="tableName"></param>
    /// <param name="operation"></param>
    /// <param name="connectionStringsName">Nombre de la connexion a realizar el commando</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public Common.Services.Contracts.DataCommand AddCommand(string statement, string tableName, string operation, string connectionStringsName)
    {
        var result = new Common.Services.Contracts.DataCommand()
        {
            Statement = statement,
            TableName = tableName,
            Operation = operation,
            ConnectionStringName = connectionStringsName.IsEmpty() ? _command.ConnectionStringName : connectionStringsName
        };
        if (_commands.IsEmpty())
        {
            _commands = new List<Common.Services.Contracts.DataCommand>();
        }

        _commands.Add(result);
        return result;
    }

    public Common.Services.Contracts.DataCommand AddCommand(string statement, InMotionGIT.Common.DataType.LookUpValue LookUp, string tableName, string operation, string connectionStringsName)
    {
        var result = new Common.Services.Contracts.DataCommand()
        {
            Statement = statement,
            TableName = tableName,
            Operation = operation,
            LookUp = LookUp,
            ConnectionStringName = connectionStringsName.IsEmpty() ? _command.ConnectionStringName : connectionStringsName
        };
        if (_commands.IsEmpty())
        {
            _commands = new List<Common.Services.Contracts.DataCommand>();
        }

        _commands.Add(result);
        return result;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<InMotionGIT.Common.DataType.LookUpPackage> PackageExecuteScalar()
    {
        var result = new List<InMotionGIT.Common.DataType.LookUpPackage>();
        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        if (local)
        {
            {
                var withBlock = new Common.Services.DataManager();
                result = withBlock.PackageExecuteScalar(_commands);
            }
        }
        else
        {
            using (var dataService = DataServiceInstance())
            {
                result = dataService.PackageExecuteScalarAsync(_commands.ToArray()).Result.PackageExecuteScalarResult.ToList();
            }
        }
        return result;
    }

    public List<DataType.LookUpPackage> PackageExecuteToLookUp(Enumerations.EnumCache cacheMode = Enumerations.EnumCache.CacheWithFullParameters)
    {
        var result = new List<DataType.LookUpPackage>();
        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        switch (cacheMode)
        {
            case var @case when @case == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    DataType.LookUpPackage tempData = default;
                    var newCommands = new List<Common.Services.Contracts.DataCommand>();
                    var newResult = new List<DataType.LookUpPackage>();

                    foreach (Common.Services.Contracts.DataCommand commandData in _commands)
                    {
                        tempData = default;

                        if (!CacheRefresh)
                        {
                            tempData = IsExistLookUpCache(commandData, false);
                        }

                        if (tempData == null)
                        {
                            newCommands.Add(commandData);
                        }
                        else
                        {
                            newResult.Add(tempData);
                        }
                    }

                    if (newCommands.Count > 0)
                    {
                        if (local)
                        {
                            {
                                var withBlock = new Common.Services.DataManager();
                                result = withBlock.PackageExecuteToLookUp(newCommands);
                            }
                        }
                        else
                        {
                            using (var dataService = DataServiceInstance())
                            {
                                result = new List<DataType.LookUpPackage>(dataService.PackageExecuteToLookUpAsync(newCommands.ToArray()).Result.PackageExecuteToLookUpResult);
                            }
                        }

                        foreach (Common.Services.Contracts.DataCommand commandData in newCommands)
                        {
                            foreach (DataType.LookUpPackage newLookUpPackage in result)
                            {
                                if (commandData.TableName == newLookUpPackage.Key)
                                {
                                    QueryCacheAdd(commandData, false, newLookUpPackage);

                                    break;
                                }
                            }
                        }
                    }

                    if (newResult.Count > 0)
                    {
                        foreach (DataType.LookUpPackage newLookUpPackage in newResult)
                            result.Add(newLookUpPackage);
                    }

                    break;
                }

            default:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            result = withBlock1.PackageExecuteToLookUp(_commands);
                        }
                    }
                    else
                    {
                        using (var dataService = DataServiceInstance())
                        {
                            result = dataService.PackageExecuteToLookUpAsync(_commands.ToArray()).Result.PackageExecuteToLookUpResult.ToList();
                        }
                    }

                    break;
                }
        }

        return result;
    }

    #endregion Package Commands

    #region Internal use

    /// <summary>
    /// Retorna una instancia del proxy del servicio de datos.
    /// </summary>
    /// <returns>Instancia del proxy del servicio de datos.</returns>
    private InMotionGIT.Common.Proxy.Services.DataManagerClient DataServiceInstance()
    {
        InMotionGIT.Common.Proxy.Services.DataManagerClient dataService = null;
        if (!(DataManagerURLForce == null) && !DataManagerURLForce.IsEmpty())
        {
            dataService = InstanceDataManagerClient(DataManagerURLForce);
        }
        else if ("DataManager.URL".AppSettings().IsEmpty())
        {
            dataService = new InMotionGIT.Common.Proxy.Services.DataManagerClient();
        }
        else
        {
            dataService = InstanceDataManagerClient("DataManager.URL".AppSettings());
        }

        return dataService;
    }

    /// <summary>
    /// Obtiene una instancia del cliente 'DataManagerClient', condicionado al url (Https o Https)
    /// </summary>
    /// <param name="url">URL al que se debe apuntar la instancia de 'DataManagerClient'</param>
    /// <returns>Retornar una Instancia de 'DataManagerClient', configurada al url que se solicitó </returns>
    private static InMotionGIT.Common.Proxy.Services.DataManagerClient InstanceDataManagerClient(string url)
    {
        InMotionGIT.Common.Proxy.Services.DataManagerClient result;
        if (url.ToLower().Contains("https"))
        {
            if (HostingEnvironment.IsHosted)
            {
                result = new InMotionGIT.Common.Proxy.Services.DataManagerClient(Services.DataManagerClient.EndpointConfiguration.WSHttpBinding_IDataManager, string.Format(CultureInfo.InvariantCulture, "{0}/DataManager.svc", url));
            }
            else
            {
                result = new InMotionGIT.Common.Proxy.Services.DataManagerClient(Services.DataManagerClient.EndpointConfiguration.WSHttpBinding_IDataManager, string.Format(CultureInfo.InvariantCulture, "{0}/DataManager.svc", url));
            }

            if ("Certificate.ForcedSelfSigned".AppSettings<bool>())
            {
                Common.Helpers.Certificate.OverrideCertificateValidation();
            }
        }
        else
        {
            result = new InMotionGIT.Common.Proxy.Services.DataManagerClient(Services.DataManagerClient.EndpointConfiguration.WSHttpBinding_IDataManager, string.Format(CultureInfo.InvariantCulture, "{0}/DataManager.svc", url));
        }
        return result;
    }

    /// <summary>
    /// Establece los parámetros de configuración asociados al limite de registros a ser retornados.
    /// </summary>
    /// <param name="command">Instancia de comando de datos.</param>
    private void SetConfiguration(Common.Services.Contracts.DataCommand command)
    {
        if (MaxNumberOfRecord.IsEmpty())
        {
            if ("DataAccessLayer.MaxNumberOfRecords".AppSettings().IsNotEmpty())
            {
                MaxNumberOfRecord = "DataAccessLayer.MaxNumberOfRecords".AppSettings<int>();
            }
            if (!"DataAccessLayer.IgnoreMaxNumberOfRecords".AppSettings<bool>())
            {
                _IgnoreMaxNumberOfRecords = true;
            }
        }
        else
        {
            _IgnoreMaxNumberOfRecords = false;
        }

        command.MaxNumberOfRecord = MaxNumberOfRecord;
        command.IgnoreMaxNumberOfRecords = _IgnoreMaxNumberOfRecords;
        command.QueryCount = QueryCount;

        string Sql = command.Statement;
        if (AllowHistoryInfo)
        {
            int beginIndex = Sql.IndexOf("@@BEGIN_HISTORICAL_MODE@@") + 25;

            if (beginIndex > -1)
            {
                int endIndex = Sql.IndexOf("@@END_HISTORICAL_MODE@@", beginIndex);

                if (endIndex > -1)
                {
                    string condition = Sql.Substring(beginIndex, endIndex - beginIndex);

                    Sql = Sql.Replace(condition, string.Empty);

                    Sql = Sql.Replace(" AND @@BEGIN_HISTORICAL_MODE@@", string.Empty);
                    Sql = Sql.Replace("@@BEGIN_HISTORICAL_MODE@@", string.Empty);
                    Sql = Sql.Replace("@@END_HISTORICAL_MODE@@", string.Empty);
                }
            }
        }
        else if (Sql.IndexOf("@@BEGIN_HISTORICAL_MODE@@") > -1)
        {
            Sql = Sql.Replace("@@BEGIN_HISTORICAL_MODE@@", string.Empty);
            Sql = Sql.Replace("@@END_HISTORICAL_MODE@@", string.Empty);
        }
        command.Statement = Sql;
    }

    #endregion Internal use

    /// <summary>
    /// Ejecuta una instrucción 'select' usada para una lista de valores. la instrucción select debe retornar solo dos columnas la que representa el código y la que representa la descripción.
    /// </summary>
    /// <returns>Lista de valores del tipo código y descripción.</returns>
    public List<DataType.LookUpValue> QueryExecuteToLookup()
    {
        return QueryExecuteToLookup(string.Empty);
    }

    /// <summary>
    /// Ejecuta una instrucción 'select' usada para una lista de valores. la instrucción select debe retornar solo dos columnas la que representa el código y la que representa la descripción.
    /// </summary>
    /// <param name="emptyOption">Si está lleno indica que se debe agregar un elemento a la lista normalmente usando para indicar un valor vacío.</param>
    /// <returns>Lista de valores del tipo código y descripción.</returns>
    public List<DataType.LookUpValue> QueryExecuteToLookup(string emptyOption)
    {
        List<DataType.LookUpValue> result;
        DataTable resultFiltered = null;
        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        if (!CacheRefresh)
        {
            result = IsExistQueryCacheList(_command, false);
            if (result.IsEmpty())
            {
                if (local)
                {
                    {
                        var withBlock1 = new Common.Services.DataManager();
                        result = withBlock1.QueryExecuteToLookup(_command);
                    }
                }
                else
                {
                    using (var dataService = DataServiceInstance())
                    {
                        result = dataService.QueryExecuteToLookupAsync(_command).Result.QueryExecuteToLookupResult.ToList();
                    }
                }
                if (result.IsNotEmpty())
                {
                    if (emptyOption.IsNotEmpty())
                    {
                        string emptyCode = "0";

                        if (emptyOption.Equals("0"))
                        {
                            emptyOption = string.Empty;
                        }
                        else if (emptyOption.Equals("-1"))
                        {
                            emptyCode = "-1";
                        }
                        else if (emptyOption.Contains(":"))
                        {
                            emptyCode = emptyOption.Split(':')[0];
                            emptyOption = emptyOption.Split(':')[1];
                        }
                        if (emptyCode.IsEmpty())
                        {
                            emptyCode = "0";
                        }
                        result.Insert(0, new DataType.LookUpValue() { Code = emptyCode, Description = emptyOption });
                    }
                }
                QueryCacheListAdd(_command, false, result);
            }
        }
        else
        {
            if (local)
            {
                {
                    var withBlock2 = new Common.Services.DataManager();
                    result = withBlock2.QueryExecuteToLookup(_command);
                }
            }
            else
            {
                using (var dataService = DataServiceInstance())
                {
                    result = dataService.QueryExecuteToLookupAsync(_command).Result.QueryExecuteToLookupResult.ToList();
                }
            }
            if (result.IsNotEmpty())
            {
                if (emptyOption.IsNotEmpty())
                {
                    string emptyCode = "0";

                    if (emptyOption.Equals("0"))
                    {
                        emptyOption = string.Empty;
                    }
                    else if (emptyOption.Equals("-1"))
                    {
                        emptyCode = "-1";
                    }
                    else if (emptyOption.Contains(":"))
                    {
                        emptyCode = emptyOption.Split(':')[0];
                        emptyOption = emptyOption.Split(':')[1];
                    }
                    if (emptyCode.IsEmpty())
                    {
                        emptyCode = "0";
                    }
                    result.Insert(0, new DataType.LookUpValue() { Code = emptyCode, Description = emptyOption });
                }
            }
            QueryCacheListAdd(_command, false, result);
        }
        return result;
    }

    /// <summary>
    /// Get All connections validating for code.
    /// </summary>
    /// <param name="CodeValidator"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public List<InMotionGIT.Common.Services.Contracts.ConnectionStrings> ConnectionStringAll(string CodeValidator)
    {
        bool local = true;
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            return Common.Helpers.ConnectionStrings.ConnectionStringGetAll(CodeValidator, CompanyId);
        }
        else
        {
            using (var dataService = DataServiceInstance())
            {
                int companyId = CompanyIdSelect();
                return dataService.ConnectionStringGetAllAsync(CodeValidator, companyId).Result.ConnectionStringGetAllResult;
            }
        }
    }

    /// <summary>
    /// Get one connectionstring specific
    /// </summary>
    /// <param name="ConnectionStrinName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public InMotionGIT.Common.Services.Contracts.ConnectionStrings ConnectionStringGet(string ConnectionStrinName)
    {
        bool local = true;
        int companyId = CompanyIdSelect();
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            return Common.Helpers.ConnectionStrings.ConnectionStringGet(ConnectionStrinName, companyId);
        }
        else
        {
            using (var dataService = DataServiceInstance())
            {
                return dataService.ConnectionStringGetAllAsync(ConnectionStrinName, companyId).Result.ConnectionStringGetAllResult;
            }
        }
    }

    /// <summary>
    /// Get Credential for ConnectionString in specific
    /// </summary>
    /// <param name="ConecctionStringName"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public Common.Services.Contracts.Credential ConnectionStringUserAndPassword(string ConecctionStringName)
    {
        bool local = true;
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            return Common.Helpers.ConnectionStrings.ConnectionStringUserAndPassword(ConecctionStringName, CompanyId);
        }
        else
        {
            int companyId = CompanyIdSelect();
            return DataServiceInstance().ConnectionStringUserAndPasswordAsync(ConecctionStringName, companyId).Result.ConnectionStringUserAndPasswordResult;
        }
    }

    public Common.Services.Contracts.info AppInfo(string path)
    {
        bool local = true;
        int companyId = CompanyIdSelect();
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        if (local)
        {
            return Common.Services.Contracts.info.Process(path);
        }
        else
        {
            return DataServiceInstance().AppInfoAsync(path).Result.AppInfoResult;
        }
    }

    public string GetSettingValue(string repositoryName, string settingName)
    {
        bool local = true;
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        string result = string.Empty;

        if (local)
        {
            result = new Common.Services.DataManager().GetSettingValue(repositoryName, settingName);
        }
        else
        {
            result = DataServiceInstance().GetSettingValueAsync(repositoryName, settingName).Result.GetSettingValueResult;
        }
        return result;
    }

    public static string DateValue(string repositoryName, DateTime value)
    {
        string key = string.Format("{0}.DateFormat", repositoryName);
        string dateformat = string.Empty;

        if (Common.Helpers.Caching.Exist(key))
        {
            dateformat = (string)Common.Helpers.Caching.GetItem(key);
        }
        else
        {
            {
                var withBlock = new DataManagerFactory();
                dateformat = withBlock.GetSettingValue(repositoryName, "DateFormat");
            }
            if (dateformat.IsNotEmpty())
            {
                Common.Helpers.Caching.SetItem(key, dateformat);
            }
        }

        string result = value.ToString(dateformat, new CultureInfo("en-US"));
        // result = result.Replace("/mm/", "/")
        return result;
    }

    #region Research

    public void CommandExecuteAsynchronous()
    {
        CommandExecuteAsynchronous(true);
    }

    private void CommandExecuteAsynchronousInternal(Dictionary<string, object> parameterInternal)
    {
        string message = string.Empty;
        Stopwatch watch = null;

        try
        {
            if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
            {
                watch = new Stopwatch();
                watch.Start();
            }

            {
                ref var withBlock = ref _command;
                withBlock.Operation = "CommandExecuteAsynchronous";
                if (_parameters.IsNotEmpty())
                {
                    withBlock.Parameters = _parameters; // .ToArray
                }
                withBlock.ConnectionStringsRaw = ConnectionStringsRaw;
                withBlock.Fields = Fields();
            }

            SetConfiguration(_command);

            bool local = true;

            if (!ForceLocalMode)
            {
                if ("DataManager.Mode".AppSettingsOnEquals("remote"))
                {
                    local = false;
                }
            }
            else
            {
                local = true;
            }

            if (local)
            {
                {
                    var withBlock1 = new Common.Services.DataManager();
                    withBlock1.CommandExecuteAsynchronous(_command);
                }
            }
            else
            {
                DataServiceInstance().CommandExecuteAsynchronousAsync(_command);
            }

            if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>() && !local)
            {
                watch.Stop();
                message = "{2}              Mode: {1}{2}              Id: {0}{2}              {3}".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command));
                if (_command.Fields.IsNotEmpty())
                {
                    message += "{1}              Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine);
                }
                message += "              Time Executed={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
                if ("DataAccessLayer.Debug.Proxy.Detail".AppSettings<bool>())
                {
                    message += Environment.NewLine + StackTraceSummary();
                }
                Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message);
            }
        }
        catch (Exception ex)
        {
            Common.Helpers.LogHandler.ErrorLog("CommandExecuteAsynchronousInternal", "CommandExecuteAsynchronousInternal Error", ex, string.Empty, false);
        }
    }

    public void CommandExecuteAsynchronous(bool Async)
    {
        Action<object> action = (parameterContainer) =>
{
    Dictionary<string, object> parameterInternal = (Dictionary<string, object>)parameterContainer;
    CommandExecuteAsynchronousInternal(parameterInternal);
};

        var parameters = new Dictionary<string, object>();
        if (Async)
        {
            var AddUsersSecurityTraceAsyn = new Task(action, parameters);
            AddUsersSecurityTraceAsyn.Start();
        }
        else
        {
            CommandExecuteAsynchronousInternal(parameters);
        }
    }

    /// <summary>
    /// Overrible of method QueryExecuteToTableJSON
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable QueryExecuteToTableJSON()
    {
        return QueryExecuteToTableJSON(false);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="resultEmpty"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public DataTable QueryExecuteToTableJSON(bool resultEmpty)
    {
        DataTable result = null;
        DataTable resultFiltered = null;
        string message = string.Empty;
        Stopwatch watch = null;

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>())
        {
            watch = new Stopwatch();
            watch.Start();
        }

        {
            ref var withBlock = ref _command;
            withBlock.Operation = "Query";
            if (_parameters.IsNotEmpty())
            {
                withBlock.Parameters = _parameters; // .ToArray
            }
            withBlock.Fields = Fields();
        }

        bool local = true;

        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }

        SetConfiguration(_command);

        switch (Cache)
        {
            case var @case when @case == Enumerations.EnumCache.None:
                {
                    if (local)
                    {
                        {
                            var withBlock1 = new Common.Services.DataManager();
                            result = withBlock1.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize();
                        }
                    }
                    else
                    {
                        
                            result = DataServiceInstance().QueryExecuteToTableJSONAsync(_command, resultEmpty).Result.QueryExecuteToTableJSONResult.Deserialize();
                        
                    }

                    break;
                }

            case var case1 when case1 == Enumerations.EnumCache.CacheWithFullParameters:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, false);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock2 = new Common.Services.DataManager();
                                result = withBlock2.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize();
                            }
                        }
                        else
                        {
                            
                                result = DataServiceInstance().QueryExecuteToTableJSONAsync(_command, resultEmpty).Result.QueryExecuteToTableJSONResult.Deserialize();
                            
                        }
                        QueryCacheAdd(_command, false, result);
                    }

                    break;
                }

            case var case2 when case2 == Enumerations.EnumCache.CacheWithCommand:
                {
                    DataTable tempData = null;
                    if (!CacheRefresh)
                    {
                        tempData = IsExistQueryCache(_command, true);
                    }
                    if (!(tempData == null))
                    {
                        result = tempData;
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock3 = new Common.Services.DataManager();
                                result = withBlock3.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize();
                            }
                        }
                        else
                        {
                            
                                result = DataServiceInstance().QueryExecuteToTableJSONAsync(_command, resultEmpty).Result.QueryExecuteToTableJSONResult.Deserialize();
                            
                        }
                        QueryCacheAdd(_command, true, result);
                    }

                    break;
                }
            case var case3 when case3 == Enumerations.EnumCache.CacheOnDemand:
                {
                    OndemandContainer tempOndemandContainer = null;
                    if (!CacheRefresh)
                    {
                        var temporalCache = IsExistQueryCacheOnDemand(_command);
                        if (temporalCache == null)
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else if (typeof(OndemandContainer) == IsExistQueryCacheOnDemand(_command).GetType())
                        {
                            tempOndemandContainer = temporalCache;
                        }
                        else
                        {
                            tempOndemandContainer = null;
                        }
                    }
                    if (!(tempOndemandContainer == null))
                    {
                        var isfund = tempOndemandContainer.KeyWichtParameters.Select(c => c.Contains(GetMd5Hash(_command))).FirstOrDefault();
                        if (!(isfund == null))
                        {
                            result = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                            }
                        }
                        else
                        {
                            if (local)
                            {
                                {
                                    var withBlock4 = new Common.Services.DataManager();
                                    result = withBlock4.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize();
                                }
                            }
                            else
                            {
                               
                                    result = DataServiceInstance().QueryExecuteToTableJSONAsync(_command, resultEmpty).Result.QueryExecuteToTableJSONResult.Deserialize();
                                
                            }
                            QueryCacheAddOnDemand(_command, true, result);
                            result = tempOndemandContainer.Data;
                            if (!string.IsNullOrEmpty(CacheFilter))
                            {
                                resultFiltered = DataTableImport(result, CacheFilter, _parameters);
                            }
                        }
                    }
                    else
                    {
                        if (local)
                        {
                            {
                                var withBlock5 = new Common.Services.DataManager();
                                result = withBlock5.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize();
                            }
                        }
                        else
                        {
                            
                                result = DataServiceInstance().QueryExecuteToTableJSONAsync(_command, resultEmpty).Result.QueryExecuteToTableJSONResult.Deserialize();
                            
                        }
                        QueryCacheAddOnDemand(_command, true, result);
                    }

                    break;
                }
        }

        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>() && !local)
        {
            watch.Stop();
            message = "{2}              Mode: {1}{2}              Id: {0}{2}              Type Chache:{4}{2}              {3}".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command), Cache);
            if (_command.Fields.IsNotEmpty())
            {
                message += "{1}              Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine);
            }
            message += "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
            if (!(result == null) && !(result.Rows == null))
            {
                message += Environment.NewLine + string.Format("              Rows={0}", result.Rows.Count);
            }
            else
            {
                message += Environment.NewLine + string.Format("              Rows={0}", 0);
            }
            message += Environment.NewLine + string.Format("              Cache={0}", Cache);
            if ("DataAccessLayer.Debug.Proxy.Detail".AppSettings<bool>())
            {
                message += Environment.NewLine + StackTraceSummary();
            }
            Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message);
        }

        if (!(resultFiltered == null))
        {
            return resultFiltered;
        }
        else
        {
            return result;
        }
    }

    #endregion Research

    #region Class Helpers

    private class OndemandContainer
    {
        private List<string> _ListKeyWichtParameters;

        public List<string> KeyWichtParameters
        {
            get
            {
                if (_ListKeyWichtParameters == null)
                {
                    _ListKeyWichtParameters = new List<string>();
                }
                return _ListKeyWichtParameters;
            }
            set
            {
                _ListKeyWichtParameters = value;
            }
        }

        public string Key { get; set; }
        public DataTable Data { get; set; }
    }

    #endregion Class Helpers

    public string CurrentTime()
    {
        bool local = true;
        if (!ForceLocalMode)
        {
            if ("DataManager.Mode".AppSettingsOnEquals("remote"))
            {
                local = false;
            }
        }
        else
        {
            local = true;
        }
        string result;

        if (local)
        {
            result = new Common.Services.DataManager().CurrentTime().ToString();
        }
        else
        {
            
                result = DataServiceInstance().CurrentTimeAsync().Result.CurrentTimeResult;
            
        }
        return result;
    }

    private static string StackTraceSummary()
    {
        var result = new StringBuilder();
        var stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.IsNotEmpty() && stackFrames.Length > 0)
        {
            foreach (StackFrame stackCall in stackFrames)
            {
                if (stackCall.GetMethod().IsNotEmpty() && stackCall.GetMethod().Name.IsNotEmpty())
                {
                    string nameSpace = string.Empty;
                    if (stackCall.GetMethod().ReflectedType.IsNotEmpty() && stackCall.GetMethod().ReflectedType.FullName.IsNotEmpty())
                    {
                        string className = stackCall.GetMethod().ReflectedType.FullName;
                        nameSpace = string.Format("{0}.{1}", className, stackCall.GetMethod().Name);
                    }
                    if (nameSpace.IsEmpty())
                    {
                        nameSpace = "Empty";
                    }
                    if (!nameSpace.StartsWith("System."))
                    {
                        if (result.Length == 0)
                        {
                            result.AppendLine(string.Format("              Stack: {0}", nameSpace));
                        }
                        else
                        {
                            result.AppendLine(string.Format("                     {0}", nameSpace));
                        }
                    }
                }
            }
        }
        return result.ToString();
    }

    private static string CommandSummary(InMotionGIT.Common.Services.Contracts.DataCommand command)
    {
        string result = string.Empty;

        if (command.IsNotEmpty())
        {
            result = string.Format("{0} {1}{2}", command.Operation, command.Statement, Constants.vbCrLf);

            if (command.Parameters.IsNotEmpty())
            {
                foreach (Common.Services.Contracts.DataParameter item in command.Parameters)
                {
                    result += string.Format("              {0}=", item.Name);

                    if (item.Value is DBNull)
                    {
                        result += "Null";
                    }
                    else if (item.Type == DbType.StringFixedLength)
                    {
                        result += string.Format("'{0}',", item.Value);
                    }
                    else if (item.Type == DbType.Date)
                    {
                        result += string.Format("TO_DATE('{0}', 'MM/DD/YYYY HH24:MI:SS'),", ((DateTime)item.Value).ToString("MM/dd/yyyy HH:mm:ss"));
                    }
                    else
                    {
                        result += string.Format("{0},", item.Value);
                    }
                    result += Constants.vbCrLf;
                }
            }
        }

        return result;
    }

    public void TraceLog(Stopwatch watch, bool local, string message, object result, string method)
    {
        if ("DataAccessLayer.Debug.Proxy".AppSettings<bool>() && !local)
        {
            watch.Stop();
            message = "{2}              Method: {5}            {2}              {3}              Mode: {1}{2}              Id: {0}{2}              Type Chache:{4}{2}              ".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command), Cache, method);
            if (_command.Fields.IsNotEmpty())
            {
                message = message.TrimEnd(Conversions.ToChar(Environment.NewLine));
                message += "Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine);
            }
            message += "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds);
            var value = default(string);

            switch (result.GetType().Name ?? "")
            {
                case "String":
                case "Int32":
                    {
                        value = result.ToString();
                        break;
                    }
                case "List(Of String)":
                    {
                        value = string.Join(",", (List<string>)result);
                        break;
                    }
                case "DataTable":
                    {
                        DataTable valueTable = (DataTable)result;
                        if (!(result == null) && !(valueTable.Rows == null))
                        {
                            message += Environment.NewLine + string.Format("              Rows={0}", valueTable.Rows.Count);
                        }
                        else
                        {
                            message += Environment.NewLine + string.Format("              Rows={0}", 0);
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            message += "              Scalar={0}".SpecialFormater(value);
            if ("DataAccessLayer.Debug.Proxy.Detail".AppSettings<bool>())
            {
                message += Environment.NewLine + StackTraceSummary();
            }
            Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message);
        }
    }

    public static InMotionGIT.Common.Proxy.Services.ArrayOfKeyValueOfstringstringKeyValueOfstringstring[] Convert(Dictionary<string, string> parameters)
    {
        return parameters.Select(kv => new InMotionGIT.Common.Proxy.Services.ArrayOfKeyValueOfstringstringKeyValueOfstringstring
        {
            Key = kv.Key,
            Value = kv.Value
        }).ToArray(); ;
    }

    private Dictionary<string, string> Fields()
    {
        return new Dictionary<string, string>() { { "CompanyId", _command.CompanyId.ToString() }, { "Id", _id }, { "Origen", "Logs.Prefix".StringValue() } };
    }
}