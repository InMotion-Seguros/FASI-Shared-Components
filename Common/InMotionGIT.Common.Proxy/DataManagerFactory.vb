#Region "using"

Imports System.Globalization
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Hosting
Imports InMotionGIT.Common.Services.Contracts

#End Region

''' <summary>
''' DataManagerFactory
''' </summary>
Public NotInheritable Class DataManagerFactory
    Implements IDisposable

#Region "Private fields, to hold the state of the entity"

    Private _command As Services.Contracts.DataCommand
    Private _parameters As List(Of Services.Contracts.DataParameter)
    Private _commands As List(Of Services.Contracts.DataCommand)
    Private _IgnoreMaxNumberOfRecords As Boolean = False
    Private _resultProcedure As Services.Contracts.StoredProcedureResult
    Private _companyId As Integer
    Private _id As String

#End Region

#Region "Public properties, to expose the state of the entity"

    Public Property ForceLocalMode As Boolean

    Public Property Cache As Enumerations.EnumCache = Enumerations.EnumCache.None

    Public Property CacheRefresh As Boolean

    Public Property CacheFilter As String

    Public Property ConnectionStringsRaw As Services.Contracts.ConnectionString

    Public Property DataManagerURLForce As String

    Public Property QueryCount As String

    Public Property QueryCountResult As Integer

    Public Property MaxNumberOfRecord As Integer = 0

    Public Property AllowHistoryInfo As Boolean = False
    Public Property AllowHistoryInfo2 As Boolean = False

    ''' <summary>
    ''' Instrucción SQL a ser ejecutada.
    ''' </summary>
    Public Property Statement() As String
        Get
            Return _command.Statement
        End Get

        Set(value As String)
            _command.Statement = value
        End Set
    End Property

    ''' <summary>
    ''' Identificación de la compañía actual.
    ''' </summary>
    Public Property CompanyId() As Integer
        Get
            Return _command.CompanyId
        End Get

        Set(value As Integer)
            _command.CompanyId = value
        End Set
    End Property

    ''' <summary>
    ''' Identificador de secuencia para id correlations
    ''' </summary>
    ''' <returns></returns>
    Public Property Id() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Instancia la clase de acceso a datos
    ''' </summary>
    Private Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la cadena de conexión.
    ''' </summary>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    Public Sub New(ConnectionStringName As String)
        _id = Guid.NewGuid.ToString()
        _companyId = CompanyIdSelect()
        _command = New Services.Contracts.DataCommand With {.ConnectionStringName = ConnectionStringName,
                                         .TableName = "Undefined",
                                         .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la cadena de conexión y la identificación de la compañía actual.
    ''' </summary>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    ''' <param name="companyId">Identificación de la compañía actual.</param>
    Public Sub New(ConnectionStringName As String, companyId As Integer)
        _id = Guid.NewGuid.ToString()
        _companyId = companyId
        _command = New Services.Contracts.DataCommand With {.ConnectionStringName = ConnectionStringName,
                                         .TableName = "Undefined",
                                         .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, así como la cadena de conexión.
    ''' </summary>
    ''' <param name="statement">Instrucción SQL a ser ejecutada.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    Public Sub New(statement As String, connectionStringName As String)
        _id = Guid.NewGuid.ToString()
        _companyId = CompanyIdSelect()
        _command = New Services.Contracts.DataCommand With {.TableName = "Undefined",
                                         .Operation = "Query",
                                         .ConnectionStringName = connectionStringName,
                                         .Statement = statement,
                                         .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal así como la identificación de la compañía actual.
    ''' </summary>
    ''' <param name="statement">Instrucción SQL a ser ejecutada.</param>
    ''' <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    ''' <param name="companyId">Identificación de la compañía actual.</param>
    Public Sub New(statement As String, tableName As String, companyId As Integer)
        _id = Guid.NewGuid.ToString()
        _companyId = companyId
        _command = New Services.Contracts.DataCommand With {.TableName = IIf(String.IsNullOrEmpty(tableName), "Undefined", tableName),
                                         .Operation = "Query",
                                         .ConnectionStringName = "BackOfficeConnectionString",
                                         .Statement = statement,
                                         .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal así como la cadena de conexión.
    ''' </summary>
    ''' <param name="statement">Instrucción SQL a ser ejecutada.</param>
    ''' <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    Public Sub New(statement As String, tableName As String, connectionStringName As String)
        _id = Guid.NewGuid.ToString()
        _companyId = CompanyIdSelect()
        _command = New Services.Contracts.DataCommand With {.TableName = IIf(String.IsNullOrEmpty(tableName), "Undefined", tableName),
                                        .Operation = "Query",
                                        .ConnectionStringName = connectionStringName,
                                        .Statement = statement,
                                        .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo la instrucción SQL a ser ejecutada, el nombre de la tabla principal, la cadena de conexión y la identificación de la compañía actual.
    ''' </summary>
    ''' <param name="statement">Instrucción SQL a ser ejecutada.</param>
    ''' <param name="tableName">Nombre de la tabla principal usada en la instrucción SQL.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    ''' <param name="companyId">Identificación de la compañía actual.</param>
    Public Sub New(statement As String, tableName As String, connectionStringName As String, companyId As Integer)
        _id = Guid.NewGuid.ToString()
        _companyId = companyId
        _command = New Services.Contracts.DataCommand With {.TableName = IIf(String.IsNullOrEmpty(tableName), "Undefined", tableName),
                                        .Operation = "Query",
                                        .ConnectionStringName = connectionStringName,
                                        .Statement = statement,
                                        .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo el procedimiento almacenado a ser ejecutado y la cadena de conexión.
    ''' </summary>
    ''' <param name="procedure">Indicador que se desea ejecutar un procedimiento almacenado.</param>
    ''' <param name="procedureName">Nombre del procedimiento almacenado a ser ejecutada.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    Public Sub New(procedure As Boolean, procedureName As String, connectionStringName As String)
        _id = Guid.NewGuid.ToString()
        _companyId = CompanyIdSelect()
        _command = New Services.Contracts.DataCommand With {.TableName = procedureName,
                                         .Operation = "Procedure",
                                         .ConnectionStringName = connectionStringName,
                                         .Statement = procedureName,
                                         .CompanyId = _companyId}
    End Sub

    ''' <summary>
    ''' Instancia la clase de acceso a datos estableciendo el procedimiento almacenado a ser ejecutado, la cadena de conexión y la identificación de la compañía actual.
    ''' </summary>
    ''' <param name="procedure">Indicador que se desea ejecutar un procedimiento almacenado.</param>
    ''' <param name="procedureName">Nombre del procedimiento almacenado a ser ejecutada.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    ''' <param name="companyId">Identificación de la compañía actual.</param>
    Public Sub New(procedure As Boolean, procedureName As String, connectionStringName As String, companyId As Integer)
        _id = Guid.NewGuid.ToString()
        _companyId = companyId
        _command = New Services.Contracts.DataCommand With {.TableName = IIf(String.IsNullOrEmpty(procedureName), "Undefined", procedureName),
                                         .Operation = "Procedure",
                                         .ConnectionStringName = connectionStringName,
                                         .Statement = procedureName,
                                         .CompanyId = _companyId}
    End Sub

#End Region

#Region "Execute Scalar Methods"

    ''' <summary>
    ''' Método que permite devolver un valor del tipo entero como único resultado de una instrucción 'select'.
    ''' </summary>
    ''' <returns>Valor del tipo entero</returns>
    Public Function QueryExecuteScalarToInteger() As Integer
        Dim result As Integer = 0
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Query"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .QueryExecuteScalarToInteger(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.QueryExecuteScalarToInteger(_command)
            End Using
        End If

        TraceLog(watch, local, message, result, "QueryExecuteScalarToInteger")

        Return result
    End Function

    ''' <summary>
    ''' Método que permite devolver un valor del tipo decimal como único resultado de una instrucción 'select'.
    ''' </summary>
    ''' <returns>Valor del tipo decimal</returns>
    Public Function QueryExecuteScalarToDecimal() As Decimal
        Dim result As Decimal
        Dim resultFiltered As String = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Query"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Services.DataManager
                        result = .QueryExecuteScalarToDecimal(_command)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.QueryExecuteScalarToDecimal(_command)
                    End Using
                End If
            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As String = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCacheString(_command, False)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteScalarToDecimal(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteScalarToDecimal(_command)
                        End Using
                    End If
                    QueryCacheAddString(_command, False, result)
                End If
            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As String = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCacheString(_command, True)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteScalarToDecimal(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteScalarToDecimal(_command)
                        End Using
                    End If
                    QueryCacheAddString(_command, True, result)
                End If
            Case Enumerations.EnumCache.CacheOnDemand
                Throw New NotImplementedException("The 'CacheOnDemand' not implemented under the method QueryExecuteScalarToDecimal")
            Case Else
                Throw New NotImplementedException("You have not selected any kind of cache")
        End Select

        TraceLog(watch, local, message, result, "QueryExecuteScalarToDecimal")

        Return result
    End Function

    ''' <summary>
    ''' Método que permite devolver un valor del tipo texto como único resultado de una instrucción 'select'.
    ''' </summary>
    ''' <returns>Valor del tipo texto</returns>
    Public Function QueryExecuteScalarToString() As String
        Dim result As String = String.Empty
        Dim resultFiltered As String = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Query"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Services.DataManager
                        result = .QueryExecuteScalarToString(_command)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.QueryExecuteScalarToString(_command)
                    End Using
                End If
            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As String = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCacheString(_command, False)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteScalarToString(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteScalarToString(_command)
                        End Using
                    End If
                    QueryCacheAddString(_command, False, result)
                End If
            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As String = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCacheString(_command, True)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteScalarToString(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteScalarToString(_command)
                        End Using
                    End If
                    QueryCacheAddString(_command, True, result)
                End If
            Case Enumerations.EnumCache.CacheOnDemand
                Throw New NotImplementedException("The 'CacheOnDemand' not implemented under the method QueryExecuteScalarToString")
            Case Else
                Throw New NotImplementedException("You have not selected any kind of cache")
        End Select

        TraceLog(watch, local, message, result, "QueryExecuteScalarToString")

        Return result
    End Function

    ''' <summary>
    ''' Método que permite devolver un valor del tipo fecha como único resultado de una instrucción 'select'.
    ''' </summary>
    ''' <returns>Valor del tipo fecha</returns>
    Public Function QueryExecuteScalarToDate() As Date
        Dim result As Date = Date.MinValue
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Query"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .QueryExecuteScalarToDate(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.QueryExecuteScalarToDate(_command)
            End Using
        End If

        TraceLog(watch, local, message, result, "QueryExecuteScalarToDate")

        Return result
    End Function

    ''' <summary>
    ''' Método que permite devolver un valor basado en el tipo genérico como único resultado de una instrucción 'select'.
    ''' </summary>
    ''' <returns>Valor basado en el tipo genérico</returns>
    Public Function QueryExecuteScalar(Of T)() As T
        Dim result As Object = Nothing

        Select Case GetType(T).ToString
            Case "System.String"
                result = QueryExecuteScalarToString()

            Case "System.Int32"
                result = QueryExecuteScalarToInteger()

            Case "System.Date"
                result = QueryExecuteScalarToDate()

        End Select

        Return result
    End Function

#End Region

#Region "SQL Statement Execute Methods"

    ''' <summary>
    ''' Ejecuta una instrucción SQL sin esperar un "Record-set" como resultado.
    ''' </summary>
    ''' <returns>Cantidad de filas afectadas.</returns>
    Public Function CommandExecute() As Integer
        Return CommandExecute("Execute")
    End Function

    ''' <summary>
    ''' Ejecuta una instrucción SQL sin esperar un "Record-set" como resultado.
    ''' </summary>
    ''' <param name="operation">Tipo de operación a realizar "Update", "Insert" o "Delete", este valor usado para referencias.</param>
    ''' <returns></returns>
    Public Function CommandExecute(operation As String) As Integer
        Dim result As Integer = 0
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = operation

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        SetConfiguration(_command)

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            With New Services.DataManager
                result = .CommandExecute(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.CommandExecute(_command)
            End Using
        End If

        TraceLog(watch, local, message, result, "CommandExecute")

        Return result
    End Function

    ''' <summary>
    ''' Ejecuta una instrucción SQL esperando un "Record-set" como resultado.
    ''' </summary>
    ''' <returns>"Record-set" en forma de "DataTable".</returns>
    Public Function QueryExecuteToTable() As DataTable
        Return QueryExecuteToTable(False)
    End Function

    Public Function Check() As List(Of String)
        Dim result As New List(Of String)
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            With New Services.DataManager
                result = .Check(Fields())
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.Check(Fields())
            End Using
        End If

        TraceLog(watch, local, message, result, "Check")

        Return result
    End Function

    Public Function OpenConnection(ConnectionStringName As String) As Boolean
        Dim result As Boolean = False
        With New Common.Services.DataManager
            Dim con = .ConnectionOpenLocal(ConnectionStringName)
            If con.IsNotEmpty Then
                result = True
            End If
            con.Close()
        End With
        Return result
    End Function

    ''' <summary>
    ''' Ejecuta una instrucción SQL esperando un "Record-set" como resultado.
    ''' </summary>
    '''<param name="resultEmpty">Indica que para el caso donde no exista ningún "Record-set" de igual forma se devuelva una instancia vacía de una "DataTable".</param>
    ''' <returns>"Record-set" en forma de "DataTable".</returns>
    Public Function QueryExecuteToTable(resultEmpty As Boolean) As DataTable
        Dim result As DataTable = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing
        Dim fromCache As Boolean = False

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        Dim QueryResult As New Services.Contracts.QueryResult
        Dim resultFiltered As DataTable = Nothing
        With _command
            .Operation = "Query"
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Common.Services.DataManager
                        QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                    End Using
                End If
            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, False)
                End If
                If Not IsNothing(tempData) Then
                    QueryResult.Table = tempData
                    fromCache = True
                Else
                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                        End Using
                    End If
                    QueryCacheAdd(_command, False, QueryResult.Table)
                End If
                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                End If

            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, True)
                End If
                If Not IsNothing(tempData) Then
                    QueryResult.Table = tempData
                    fromCache = True
                Else
                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                        End Using
                    End If
                    QueryCacheAdd(_command, True, QueryResult.Table)
                End If
                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                End If

            Case Enumerations.EnumCache.CacheOnDemand
                Dim tempOndemandContainer As OndemandContainer = Nothing
                If Not CacheRefresh Then
                    Dim temporalCache = IsExistQueryCacheOnDemand(_command)
                    If IsNothing(temporalCache) Then
                        tempOndemandContainer = temporalCache
                        fromCache = True
                    Else
                        If GetType(OndemandContainer) = IsExistQueryCacheOnDemand(_command).GetType Then
                            tempOndemandContainer = temporalCache
                            fromCache = True
                        Else
                            tempOndemandContainer = Nothing
                        End If
                    End If

                End If
                If Not IsNothing(tempOndemandContainer) Then
                    Dim isfund = (From itemfound In tempOndemandContainer.KeyWichtParameters
                                  Where itemfound.Contains(GetMd5Hash(_command)) Select itemfound).SingleOrDefault
                    If Not IsNothing(isfund) Then
                        QueryResult.Table = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                        End If
                    Else
                        If local Then
                            With New Common.Services.DataManager
                                QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                            End With
                        Else
                            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                                QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                            End Using
                        End If
                        QueryCacheAddOnDemand(_command, True, QueryResult.Table)
                        QueryResult.Table = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                        End If
                    End If
                Else
                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                        End Using
                    End If
                    QueryCacheAddOnDemand(_command, True, QueryResult.Table)
                End If
            Case Else
                If local Then
                    With New Common.Services.DataManager
                        QueryResult = .QueryExecuteToTable(_command, resultEmpty)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        QueryResult = dataService.QueryExecuteToTable(_command, resultEmpty)
                    End Using
                End If
        End Select

        Me.QueryCountResult = QueryResult.QueryCountResult
        If Not IsNothing(resultFiltered) Then
            If Not IsNothing(QueryResult.Table) Then
                QueryResult.Table.ReadOnlyMode(False)
            End If
            If Not IsNothing(resultFiltered) Then
                resultFiltered.ReadOnlyMode(False)
            End If
            result = resultFiltered
        Else
            If Not IsNothing(QueryResult.Table) Then
                QueryResult.Table.ReadOnlyMode(False)
            End If
            result = QueryResult.Table
        End If

        TraceLog(watch, local, message, result, "QueryExecuteToTable")

        Return result
    End Function

    ''' <summary>
    ''' Ejecuta una instrucción SQL del tipo definición de estructura.
    ''' </summary>
    ''' <param name="statement">Instrucción SQL del tipo definición de estructura.</param>
    ''' <returns>Resultado de la ejecución.</returns>
    Public Function DataStructure(statement As String) As String
        Dim result As String = "Ok"
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        With _command
            .Statement = statement
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .DataStructure(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.DataStructure(_command)
            End Using
        End If

        TraceLog(watch, local, message, result, "DataStructure")

        Return result
    End Function

    ''' <summary>
    ''' Retorna la instrucción SQL con el reemplazo de valores para cada parámetro.
    ''' </summary>
    ''' <param name="operation">Tipo de operación a realizar "Update", "Insert" o "Delete", este valor usado para referencias.</param>
    ''' <returns>Instrucción SQL.</returns>
    Public Function ResolveStatement(operation As String) As String
        Dim result As String = "Ok"

        With _command
            .Operation = operation
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .ResolveStatement(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.ResolveStatement(_command)
            End Using
        End If
        Return result
    End Function

#End Region

#Region "Procedures Execute Methods"

    ''' <summary>
    ''' Ejecuta un procedimiento almacenado esperando un "Record-set" como resultado.
    ''' </summary>
    ''' <returns>"Record-set" en forma de "DataTable".</returns>
    Public Function ProcedureExecuteToTable() As DataTable
        Return ProcedureExecuteToTable(False)
    End Function

    ''' <summary>
    ''' Ejecuta un procedimiento almacenado esperando un "Record-set" como resultado.
    ''' </summary>
    '''<param name="resultEmpty">Indica que para el caso donde no exista ningún "Record-set" de igual forma se devuelva una instancia vacía de una "DataTable".</param>
    ''' <returns>"Record-set" en forma de "DataTable".</returns>
    Public Function ProcedureExecuteToTable(resultEmpty As Boolean) As DataTable
        Dim QueryResult As New Services.Contracts.QueryResult
        Dim resultFiltered As DataTable = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "ProcedureToTable"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If

            .ConnectionStringsRaw = Me.ConnectionStringsRaw
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then

                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Common.Services.DataManager
                        QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)

                    End Using
                End If

            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As DataTable = Nothing

                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, False)
                End If

                If Not IsNothing(tempData) Then
                    QueryResult.Table = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)
                        End Using
                    End If

                    QueryCacheAdd(_command, False, QueryResult.Table)
                End If

                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                End If

            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As DataTable = Nothing

                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, True)
                End If

                If Not IsNothing(tempData) Then
                    QueryResult.Table = tempData
                Else

                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)
                        End Using
                    End If

                    QueryCacheAdd(_command, True, QueryResult.Table)
                End If

                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                End If

            Case Enumerations.EnumCache.CacheOnDemand
                Dim tempOndemandContainer As OndemandContainer = Nothing

                If Not CacheRefresh Then
                    Dim temporalCache = IsExistQueryCacheOnDemand(_command)

                    If IsNothing(temporalCache) Then
                        tempOndemandContainer = temporalCache
                    Else
                        If GetType(OndemandContainer) = IsExistQueryCacheOnDemand(_command).GetType Then
                            tempOndemandContainer = temporalCache
                        Else
                            tempOndemandContainer = Nothing
                        End If
                    End If
                End If

                If Not IsNothing(tempOndemandContainer) Then
                    Dim isfund = (From itemfound In tempOndemandContainer.KeyWichtParameters
                                  Where itemfound.Contains(GetMd5Hash(_command)) Select itemfound).SingleOrDefault

                    If Not IsNothing(isfund) Then
                        QueryResult.Table = tempOndemandContainer.Data

                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                        End If
                    Else
                        If local Then
                            With New Common.Services.DataManager
                                QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)

                            End With
                        Else
                            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                                QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)

                            End Using
                        End If

                        QueryCacheAddOnDemand(_command, True, QueryResult.Table)
                        QueryResult.Table = tempOndemandContainer.Data

                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(QueryResult.Table, CacheFilter, _parameters)
                        End If
                    End If
                Else
                    If local Then
                        With New Common.Services.DataManager
                            QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)

                        End Using
                    End If

                    QueryCacheAddOnDemand(_command, True, QueryResult.Table)
                End If

            Case Else
                If local Then
                    With New Common.Services.DataManager
                        QueryResult = .ProcedureExecuteToTable(_command, resultEmpty)

                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        QueryResult = dataService.ProcedureExecuteToTable(_command, resultEmpty)

                    End Using
                End If
        End Select

        If Not IsNothing(QueryResult) AndAlso Not IsNothing(QueryResult.OutputParameters) AndAlso QueryResult.OutputParameters.Count > 0 Then
            Dim key As String = String.Empty
            Dim temporalParameter As Services.Contracts.DataParameter = Nothing

            For Each parameterData As KeyValuePair(Of String, Object) In QueryResult.OutputParameters
                key = parameterData.Key
                If Not IsNothing(_parameters) Then
                    temporalParameter = (From itemParameter In _parameters Where itemParameter.Name.Equals(key)).SingleOrDefault

                    If Not IsNothing(temporalParameter) Then
                        temporalParameter.Value = parameterData.Value
                        InMotionGIT.Common.Helpers.LogHandler.TraceLog("Proxy - Nelson " & key & " = " & parameterData.Value)
                    End If
                End If
            Next
        End If

        Me.QueryCountResult = QueryResult.QueryCountResult

        If Not IsNothing(resultFiltered) Then
            If Not IsNothing(QueryResult.Table) Then
                QueryResult.Table.ReadOnlyMode(False)
            End If

            If Not IsNothing(resultFiltered) Then
                resultFiltered.ReadOnlyMode(False)
            End If
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteToTable")
            Return resultFiltered
        Else
            If Not IsNothing(QueryResult.Table) Then
                QueryResult.Table.ReadOnlyMode(False)
            End If
            TraceLog(watch, local, message, QueryResult.Table, "ProcedureExecuteToTable")
            Return QueryResult.Table
        End If
    End Function

    ''' <summary>
    ''' Ejecuta un procedimiento almacenado para obtener la estructura de un "Record-set" como resultado. Operación especial de uso interno.
    ''' </summary>
    ''' <returns>"Record-set" en forma de "DataTable".</returns>
    Public Function ProcedureExecuteResultSchema() As DataTable
        Dim result As DataTable = Nothing
        Dim resultFiltered As DataTable = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing
        With _command
            .Operation = "ProcedureToTable"
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
        End With

        Dim local As Boolean = True

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Common.Services.DataManager
                        result = .ProcedureExecuteResultSchema(_command)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.ProcedureExecuteResultSchema(_command)
                    End Using
                End If
            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, False)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .ProcedureExecuteResultSchema(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.ProcedureExecuteResultSchema(_command)
                        End Using
                    End If
                    QueryCacheAdd(_command, False, result)
                End If
                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                End If
            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, True)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .ProcedureExecuteResultSchema(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.ProcedureExecuteResultSchema(_command)
                        End Using
                    End If
                    QueryCacheAdd(_command, True, result)
                End If
                If Not String.IsNullOrEmpty(CacheFilter) Then
                    resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                End If

            Case Enumerations.EnumCache.CacheOnDemand
                Dim tempOndemandContainer As OndemandContainer = Nothing
                If Not CacheRefresh Then
                    Dim temporalCache As OndemandContainer = IsExistQueryCacheOnDemand(_command)
                    If IsNothing(temporalCache) Then
                        tempOndemandContainer = temporalCache
                    Else
                        If GetType(OndemandContainer) = IsExistQueryCacheOnDemand(_command).GetType Then
                            tempOndemandContainer = temporalCache
                        Else
                            tempOndemandContainer = Nothing
                        End If
                    End If
                End If
                If Not IsNothing(tempOndemandContainer) Then
                    Dim isfund = (From itemfound In tempOndemandContainer.KeyWichtParameters
                                  Where itemfound.Contains(GetMd5Hash(_command)) Select itemfound).SingleOrDefault
                    If Not IsNothing(isfund) Then
                        result = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                        End If
                    Else
                        If local Then
                            With New Common.Services.DataManager
                                result = .ProcedureExecuteResultSchema(_command)
                            End With
                        Else
                            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                                result = dataService.ProcedureExecuteResultSchema(_command)
                            End Using
                        End If
                        QueryCacheAddOnDemand(_command, True, result)
                        result = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                        End If
                    End If
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .ProcedureExecuteResultSchema(_command)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.ProcedureExecuteResultSchema(_command)
                        End Using
                    End If
                    QueryCacheAddOnDemand(_command, True, result)
                End If
            Case Else
                If local Then
                    With New Common.Services.DataManager
                        result = .ProcedureExecuteResultSchema(_command)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.ProcedureExecuteResultSchema(_command)
                    End Using
                End If
        End Select

        If Not IsNothing(resultFiltered) Then
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteResultSchema")
            Return resultFiltered
        Else
            TraceLog(watch, local, message, resultFiltered, "ProcedureExecuteResultSchema")
            Return result
        End If

        Return result
    End Function

    ''' <summary>
    ''' Ejecuta un procedimiento almacenado sin esperar un "Record-set" como resultado.
    ''' </summary>
    ''' <returns>Cantidad de filas afectadas.</returns>
    Public Function ProcedureExecute() As Integer
        Dim resultRowAffected As Integer = 0
        Dim result As Services.Contracts.StoredProcedureResult
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Procedure"

            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .ProcedureExecute(_command)
                resultRowAffected = result.RowAffected
                _resultProcedure = result
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.ProcedureExecute(_command)
                resultRowAffected = result.RowAffected
                _resultProcedure = result
            End Using
        End If

        If Not IsNothing(result) Then
            Dim key As String = String.Empty
            For Each itemResult In result.OutParameter
                key = itemResult.Key
                Dim tempParameer = (From itemParamter In _parameters Where itemParamter.Name.Equals(key)).SingleOrDefault
                If Not IsNothing(tempParameer) Then
                    tempParameer.Value = itemResult.Value
                End If
            Next
        End If

        TraceLog(watch, local, message, resultRowAffected, "ProcedureExecute")

        Return resultRowAffected
    End Function

#End Region

#Region "Parameters Methods"

    Public Sub AddParameter(name As String, type As DbType, size As Integer, isNull As Boolean, value As Object, direction As ParameterDirection)
        If _parameters.IsEmpty Then
            _parameters = New List(Of Services.Contracts.DataParameter)
        End If

        _parameters.Add(New Services.Contracts.DataParameter With {.Name = name, .Type = type, .Size = size, .IsNull = isNull, .Value = value, .Direction = direction})
    End Sub

    Public Sub AddParameter(name As String, type As DbType, size As Integer, isNull As Boolean, value As Object)
        If _parameters.IsEmpty Then
            _parameters = New List(Of Services.Contracts.DataParameter)
        End If

        _parameters.Add(New Services.Contracts.DataParameter With {.Name = name, .Type = type, .Size = size, .IsNull = isNull, .Value = value, .Direction = ParameterDirection.Input})
    End Sub

    Public Function ParameterByName(name As String) As Services.Contracts.DataParameter
        Dim result As Services.Contracts.DataParameter = Nothing

        For Each parameter As Services.Contracts.DataParameter In _parameters
            If String.Equals(parameter.Name, name, StringComparison.CurrentCultureIgnoreCase) Then

                result = parameter
                Exit For
            End If
        Next

        Return result
    End Function

#End Region

#Region "Tools"

    ''' <summary>
    ''' Indica si la cadena de conexión usa un proveedor del tipo "Oracle".
    ''' </summary>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    ''' <returns>Verdadero en caso de ser tipo "Oracle", falso en caso contrario.</returns>
    Public Shared Function ConnectionStringIsOracle(connectionStringName As String) As Boolean
        Dim isOracle As Boolean = False
        Dim temporalConnectionString = (New Proxy.DataManagerFactory("").DataServiceInstance()).GetDataBaseProvider(connectionStringName)
        If temporalConnectionString.IsNotEmpty Then
            If temporalConnectionString.ToLower.Contains("Oracle".ToLower) Then
                isOracle = True
            Else
                isOracle = False
            End If
        Else
            Throw New Exceptions.InMotionGITException(String.Format("ConnectionString '{0}', It's not found", connectionStringName))
        End If
        Return isOracle
    End Function

    ''' <summary>
    ''' Permite hacer el mapeo de forma automática entre un datatable y una clase, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    ''' </summary>
    ''' <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    ''' <param name="dr">Datatable usando como fuentes de datos</param>
    ''' <returns>Instancia de la clase T con la información de la primera fila del datatable</returns>
    Public Shared Function Mapper(Of T As New)(dr As DataTable) As T
        Dim businessEntityType As Type = GetType(T)
        Dim entitys As New List(Of T)()
        Dim hashtable As New Hashtable()
        Dim properties As PropertyInfo() = businessEntityType.GetProperties()
        Dim info As PropertyInfo
        Dim newObject As T = Nothing

        For Each info In properties
            hashtable(info.Name.ToUpper()) = info
        Next

        If dr.Rows.Count > 0 Then
            Dim row As DataRow = dr.Rows(0)
            newObject = New T
            For Each column As DataColumn In dr.Columns
                If Not IsDBNull(row(column.ColumnName)) Then
                    info = DirectCast(hashtable(column.ColumnName.ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite Then

                        If info.PropertyType Is GetType(String) Then
                            info.SetValue(newObject, row(column.ColumnName).ToString.Trim, Nothing)
                        Else
                            info.SetValue(newObject, row(column.ColumnName), Nothing)
                        End If

                    End If
                End If
            Next
        End If
        Return newObject
    End Function

    ''' <summary>
    ''' Permite hacer el mapeo de forma automática entre un datatable y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    ''' </summary>
    ''' <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    ''' <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    ''' <param name="dr">Datatable usando como fuentes de datos</param>
    ''' <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    Public Shared Function Mapper(Of T As New, Y As New)(dr As DataTable) As Y
        Dim businessEntityType As Type = GetType(T)
        Dim entitys As New Y
        Dim ientitys As IList = DirectCast(entitys, IList)
        Dim hashtable As New Hashtable()
        Dim properties As PropertyInfo() = businessEntityType.GetProperties()
        Dim info As PropertyInfo
        Dim newObject As T

        For Each info In properties
            hashtable(info.Name.ToUpper()) = info
        Next

        For Each row As DataRow In dr.Rows
            newObject = New T
            For Each column As DataColumn In dr.Columns
                If Not IsDBNull(row(column.ColumnName)) Then
                    info = DirectCast(hashtable(column.ColumnName.ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite Then

                        If info.PropertyType Is GetType(String) Then
                            info.SetValue(newObject, row(column.ColumnName).ToString.Trim, Nothing)
                        Else
                            info.SetValue(newObject, row(column.ColumnName), Nothing)
                        End If

                    End If
                End If
            Next
            ientitys.Add(newObject)
        Next

        Return entitys
    End Function

    ''' <summary>
    ''' Permite hacer el mapeo de forma automática entre un data-table y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    ''' </summary>
    ''' <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    ''' <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    ''' <param name="ExplicitPropertyMapping">Se definen qué propiedades deben apuntar de manera explícita a la columna,la relación es columna con propiedad</param>
    ''' <param name="dr">Data-table usando como fuentes de datos</param>
    ''' <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    Public Shared Function Mapper(Of T As New, Y As New)(dr As DataTable, ExplicitPropertyMapping As Dictionary(Of String, String)) As Y
        Dim businessEntityType As Type = GetType(T)
        Dim entitys As New Y
        Dim ientitys As IList = DirectCast(entitys, IList)
        Dim hashtable As New Hashtable()
        Dim properties As PropertyInfo() = businessEntityType.GetProperties()
        Dim info As PropertyInfo
        Dim newObject As T

        Dim newExplicitPropertyMapping As New Dictionary(Of String, String)

        For Each Item In ExplicitPropertyMapping.Keys
            With newExplicitPropertyMapping
                .Add(Item.ToUpper, ExplicitPropertyMapping(Item).ToUpper)
            End With
        Next

        For Each info In properties
            hashtable(info.Name.ToUpper()) = info
        Next

        For Each row As DataRow In dr.Rows
            newObject = New T
            For Each column As DataColumn In dr.Columns
                If Not IsDBNull(row(column.ColumnName)) Then
                    If Not newExplicitPropertyMapping.ContainsKey(column.ColumnName.ToUpper()) Then
                        info = DirectCast(hashtable(column.ColumnName.ToUpper()), PropertyInfo)
                    Else
                        info = DirectCast(hashtable(newExplicitPropertyMapping(column.ColumnName.ToUpper())), PropertyInfo)
                    End If
                    If (info IsNot Nothing) AndAlso info.CanWrite Then

                        If info.PropertyType Is GetType(String) Then
                            info.SetValue(newObject, row(column.ColumnName).ToString.Trim, Nothing)
                        Else
                            info.SetValue(newObject, row(column.ColumnName), Nothing)
                        End If

                    End If

                End If
            Next
            ientitys.Add(newObject)
        Next

        Return entitys
    End Function

    ''' <summary>
    ''' Permite hacer el mapeo de forma automática entre un data-table y una collection, partiendo de que los nombres de las columnas son los mismo que la propiedades de la clase.
    ''' </summary>
    ''' <typeparam name="T">Tipo de clase a ser usando en el mapeo</typeparam>
    ''' <typeparam name="Y">Tipo de la colección usada para almacenar la instancia de T</typeparam>
    ''' <param name="ExplicitPropertyType">Se definen qué propiedades deben apuntar de manera explícita a la columna,la relación es columna con propiedad</param>
    ''' <param name="dr">Data-table usando como fuentes de datos</param>
    ''' <returns>Instancia de la colección Y con instancias de la clase T que contiene la información de todas las filas del datatable</returns>
    Public Shared Function Mapper(Of T As New, Y As New)(dr As DataTable, ExplicitPropertyType As Dictionary(Of String, Type)) As Y
        Dim businessEntityType As Type = GetType(T)
        Dim entitys As New Y
        Dim ientitys As IList = DirectCast(entitys, IList)
        Dim hashtable As New Hashtable()
        Dim properties As PropertyInfo() = businessEntityType.GetProperties()
        Dim info As PropertyInfo
        Dim newObject As T

        Dim newExplicitPropertyType As New Dictionary(Of String, Type)

        For Each Item In ExplicitPropertyType.Keys
            With newExplicitPropertyType
                .Add(Item.ToUpper, ExplicitPropertyType(Item))
            End With
        Next

        For Each info In properties
            hashtable(info.Name.ToUpper()) = info
        Next

        For Each row As DataRow In dr.Rows
            newObject = New T
            For Each column As DataColumn In dr.Columns
                If Not IsDBNull(row(column.ColumnName)) Then
                    info = DirectCast(hashtable(column.ColumnName.ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite Then

                        If info.PropertyType Is GetType(String) Then
                            info.SetValue(newObject, row(column.ColumnName).ToString.Trim, Nothing)
                        ElseIf info.PropertyType Is GetType(Boolean) Then
                            If Not newExplicitPropertyType.ContainsKey(column.ColumnName.ToUpper()) Then
                                info.SetValue(newObject, row(column.ColumnName), Nothing)
                            Else
                                Dim value As Boolean = row(column.ColumnName).ToString().ToLower().Equals("y")
                                info.SetValue(newObject, value, Nothing)
                            End If
                        Else
                            info.SetValue(newObject, row(column.ColumnName), Nothing)
                        End If

                    End If

                End If
            Next
            ientitys.Add(newObject)
        Next

        Return entitys
    End Function

    ''' <summary>
    ''' Retorna el nombre del proveedor de ADO.Net usado por un cadena de conexión.
    ''' </summary>
    ''' <param name="connectionStringName">Nombre de la cadena de conexión</param>
    ''' <returns>Nombre del proveedor </returns>
    Public Shared Function GetDataBaseProvider(connectionStringName As String) As String
        Dim dataService As DataManager.DataManagerClient = Nothing
        Dim result As String

        If "DataManager.URL".AppSettings().IsEmpty Then
            dataService = New DataManager.DataManagerClient()
        Else
            dataService = InstanceDataManagerClient("DataManager.URL".AppSettings())
        End If

        result = dataService.GetDataBaseProvider(connectionStringName)
        dataService.Close()

        Return result
    End Function

    ''' <summary>
    ''' Retorna la identificación de la compañía actual en caso de que el sistema este configurado multi-compañía.
    ''' </summary>
    ''' <returns>Identificación de la compañía.</returns>
    Public Function CompanyIdSelect() As Integer
        Dim Result As Integer = "BackOffice.CompanyDefault".AppSettings(Of Integer)
        If "BackOffice.IsMultiCompany".AppSettings(Of Boolean) Then
            If Not IsNothing(HttpContext.Current) Then
                If Not IsNothing(HttpContext.Current.Session) AndAlso HttpContext.Current.Session("CompanyId") <> String.Empty Then
                    Result = HttpContext.Current.Session("CompanyId")
                End If
            End If
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Permite verificar la existencia de un objeto a nivel de la base de datos.
    ''' </summary>
    ''' <param name="owner">Propietario del objecto.</param>
    ''' <param name="type">Tipo de objeto.</param>
    ''' <param name="name">Nombre de objeto.</param>
    ''' <param name="ConnectionStringName">Nombre de la cadena de conexión.</param>
    ''' <returns>Verdadero en caso de existir el objeto, falso en caso contrario.</returns>
    Public Shared Function ObjectExist(owner As String, type As String, name As String, connectionStringName As String) As Boolean
        Dim local As Boolean = True
        Dim result As Boolean

        If String.IsNullOrEmpty(owner) Then
            owner = "INSUDB"
        End If

        Dim _command As New Services.Contracts.DataCommand With {.ConnectionStringName = connectionStringName,
                                                            .Owner = owner, .ObjectType = type, .TableName = name}

        If "DataManager.Mode".AppSettingsOnEquals("remote") Then
            local = False
        End If

        If local Then
            With New Services.DataManager
                result = .ObjectExist(_command)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = (New DataManagerFactory).DataServiceInstance()
                result = dataService.ObjectExist(_command)
            End Using
        End If

        Return result
    End Function

#End Region

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ''' <summary>
    ''' Implantación de la interface “IDisposable”.
    ''' </summary>
    ''' <param name="disposing">Usado para detectar llamada redundantes.</param>
    Protected Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _command = Nothing
                _parameters = Nothing
                _commands = Nothing
                _resultProcedure = Nothing
                ConnectionStringsRaw = Nothing
            End If
        End If
        Me.disposedValue = True
    End Sub

    ''' <summary>
    ''' Implantación de la interface “IDisposable”.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

#Region "Cache"

    Private Function QueryCacheAdd(command As Services.Contracts.DataCommand, OnlyCommand As Boolean, data As DataType.LookUpPackage) As Boolean
        Dim Result As Boolean = Nothing
        Dim hash As String = ""

        If OnlyCommand Then
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            hash = GetMd5Hash(command)
        End If

        If Not (Common.Helpers.Caching.Exist(hash)) Then
            Common.Helpers.Caching.SetItem(hash, data)
        End If

        Return Result
    End Function

    ''' <summary>
    ''' Add the hash of the query and the result thereof
    ''' </summary>
    ''' <param name="command">Command executed</param>
    ''' <param name="data">Result of the query executed</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function QueryCacheAdd(command As Services.Contracts.DataCommand, OnlyCommand As Boolean, data As DataTable) As Boolean
        Dim Result As Boolean = Nothing
        Dim hash As String = ""
        If OnlyCommand Then
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            hash = GetMd5Hash(command)
        End If
        If Not (Common.Helpers.Caching.Exist(hash)) Then
            Common.Helpers.Caching.SetItem(hash, data)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Add the hash of the query and the result thereof
    ''' </summary>
    ''' <param name="command">Command executed</param>
    ''' <param name="data">Result of the query executed</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function QueryCacheListAdd(command As Services.Contracts.DataCommand, OnlyCommand As Boolean, data As List(Of DataType.LookUpValue)) As Boolean
        Dim Result As Boolean = Nothing
        Dim hash As String = ""
        If OnlyCommand Then
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            hash = GetMd5Hash(command)
        End If
        If Not (Common.Helpers.Caching.Exist(hash)) Then
            Common.Helpers.Caching.SetItem(hash, data)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' Add the hash of the query and the result thereof
    ''' </summary>
    ''' <param name="command">Command executed</param>
    ''' <param name="data">Result of the query executed</param>
    Private Sub QueryCacheAddString(command As Services.Contracts.DataCommand, OnlyCommand As Boolean, data As String)
        If data.IsNotEmpty Then
            Dim hash As String = String.Empty

            If OnlyCommand Then
                hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
            Else
                hash = GetMd5Hash(command)
            End If
            If Not (Common.Helpers.Caching.Exist(hash)) Then
                Common.Helpers.Caching.SetItem(hash, data)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Add the hash of the query and the result thereof
    ''' </summary>
    ''' <param name="command">Command executed</param>
    ''' <param name="data">Result of the query executed</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function QueryCacheAddOnDemand(command As Services.Contracts.DataCommand, OnlyCommand As Boolean, data As DataTable) As Boolean
        Dim Result As Boolean = Nothing
        Dim hashCommandText As String = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Dim hashCommand As String = GetMd5Hash(command)
        Dim tempOndemand As Proxy.DataManagerFactory.OndemandContainer
        If Not (Common.Helpers.Caching.Exist(hashCommandText)) Then
            tempOndemand = New Proxy.DataManagerFactory.OndemandContainer
            With tempOndemand
                .Key = hashCommandText
                .Data = data
                .KeyWichtParameters.Add(hashCommand)
            End With
            Common.Helpers.Caching.SetItem(hashCommandText, tempOndemand)
        Else
            tempOndemand = TryCast(Common.Helpers.Caching.GetItem(hashCommandText), OndemandContainer)
            If Not IsNothing(tempOndemand) Then
                Dim isfund = (From itemfound In tempOndemand.KeyWichtParameters Where itemfound.Contains(hashCommand) Select itemfound).SingleOrDefault
                If IsNothing(isfund) Then
                    If Not IsNothing(tempOndemand.Data) Then
                        For Each ItemRow As DataRow In data.Rows
                            tempOndemand.Data.ImportRow(ItemRow)
                        Next
                    End If
                    tempOndemand.KeyWichtParameters.Add(hashCommand)
                End If
            End If
        End If
        Return Result
    End Function

    ''' <summary>
    ''' If there is a query that is executed in cache
    ''' </summary>
    ''' <param name="command">Executing query</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsExistQueryCache(command As Services.Contracts.DataCommand, OnlyCommand As Boolean) As DataTable
        Dim Result As DataTable = Nothing
        Dim hash As String = MD5Command(command, OnlyCommand)
        If (Common.Helpers.Caching.Exist(hash)) Then
            Result = Common.Helpers.Caching.GetItem(hash)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' If there is a lookup that is executed in cache
    ''' </summary>
    ''' <param name="command">Executing query</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsExistLookUpCache(command As Services.Contracts.DataCommand, OnlyCommand As Boolean) As InMotionGIT.Common.DataType.LookUpPackage
        Dim result As InMotionGIT.Common.DataType.LookUpPackage = Nothing
        Dim hash As String = MD5Command(command, OnlyCommand)
        If (Common.Helpers.Caching.Exist(hash)) Then
            result = Common.Helpers.Caching.GetItem(hash)
        End If
        Return result
    End Function

    Private Function MD5Command(command As Services.Contracts.DataCommand, OnlyCommand As Boolean) As String
        Dim result As String = ""
        If OnlyCommand Then
            result = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            result = GetMd5Hash(command)
        End If
        Return result
    End Function

    ''' <summary>
    ''' If there is a query that is executed in cache
    ''' </summary>
    ''' <param name="command">Executing query</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsExistQueryCacheList(command As Services.Contracts.DataCommand, OnlyCommand As Boolean) As List(Of DataType.LookUpValue)
        Dim Result As List(Of DataType.LookUpValue) = Nothing
        Dim hash As String = ""
        If OnlyCommand Then
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            hash = GetMd5Hash(command)
        End If
        If (Common.Helpers.Caching.Exist(hash)) Then
            Result = Common.Helpers.Caching.GetItem(hash)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' If there is a query that is executed in cache
    ''' </summary>
    ''' <param name="command">Executing query</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsExistQueryCacheString(command As Services.Contracts.DataCommand, OnlyCommand As Boolean) As String
        Dim Result As String = Nothing
        Dim hash As String = ""
        If OnlyCommand Then
            hash = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Else
            hash = GetMd5Hash(command)
        End If
        If (Common.Helpers.Caching.Exist(hash)) Then
            Result = Common.Helpers.Caching.GetItem(hash)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' IsExistQueryCacheOnDemand
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsExistQueryCacheOnDemand(command As Services.Contracts.DataCommand) As OndemandContainer
        Dim Result As OndemandContainer = Nothing
        Dim hashCommandTesx As String = GetMd5Hash(command.ConnectionStringName + command.Statement)
        Dim hashCommand As String = GetMd5Hash(command)
        If (Common.Helpers.Caching.Exist(hashCommandTesx)) Then
            Result = Common.Helpers.Caching.GetItem(hashCommandTesx)
        End If
        Return Result
    End Function

    ''' <summary>
    ''' MD5 generator to run the query
    ''' </summary>
    ''' <param name="command">Query</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetMd5Hash(command As Services.Contracts.DataCommand) As String
        Using md5Hash As MD5 = MD5.Create()
            Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(command.ConnectionStringName + Common.Helpers.Serialize.SerializeJSON(Of Services.Contracts.DataCommand)(command)))
            Dim sBuilder As New StringBuilder()
            Dim i As Integer
            For i = 0 To data.Length - 1
                sBuilder.Append(data(i).ToString("x2"))
            Next i
            Return sBuilder.ToString()
        End Using
    End Function

    ''' <summary>
    ''' Sobre carga de contructor para string
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetMd5Hash(command As String) As String
        Using md5Hash As MD5 = MD5.Create()
            Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(command))
            Dim sBuilder As New StringBuilder()
            Dim i As Integer
            For i = 0 To data.Length - 1
                sBuilder.Append(data(i).ToString("x2"))
            Next i
            Return sBuilder.ToString()
        End Using
    End Function

    Private Shared Function VerifyMd5Hash(ByVal input As Object, ByVal hash As String) As Boolean
        Dim hashOfInput As String = GetMd5Hash(input)
        Dim comparer As StringComparer = StringComparer.OrdinalIgnoreCase
        If 0 = comparer.Compare(hashOfInput, hash) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Importart datos de otro datatable
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <param name="pFilter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function DataTableImport(Data As DataTable, pFilter As String, _parameters As List(Of Services.Contracts.DataParameter)) As DataTable
        Dim resultFiltered As DataTable = Nothing
        pFilter = ProcessFilter(pFilter, _parameters)
        Dim row As DataRow() = Data.Select(pFilter)
        If row.Count <> 0 Then
            resultFiltered = Data.Copy()
            resultFiltered.Clear()
            For Each ItemRow In row
                resultFiltered.ImportRow(ItemRow)
            Next
        End If
        Return resultFiltered
    End Function

    Private Function ProcessFilter(pFilter As String, _parameters As List(Of Services.Contracts.DataParameter)) As String
        Dim Result As String = pFilter
        If Not String.IsNullOrEmpty(pFilter) AndAlso Not IsNothing(_parameters) Then
            For Each ItemParameters In _parameters
                Select Case ItemParameters.Type
                    Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.Guid, DbType.String, DbType.StringFixedLength, DbType.Date, DbType.DateTime, DbType.DateTime2, DbType.DateTimeOffset
                        Result = Result.Replace("@:" + ItemParameters.Name, String.Format("'{0}'", ItemParameters.Value.ToString().Trim()))
                    Case Else
                        Result = Result.Replace("@:" + ItemParameters.Name, ItemParameters.Value)
                End Select
            Next
        End If
        Return Result
    End Function

#End Region

#Region "Package Commands"

    ''' <summary>
    ''' AddCommand
    ''' </summary>
    ''' <param name="statement"></param>
    ''' <param name="tableName"></param>
    ''' <param name="operation"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddCommand(statement As String, tableName As String, operation As String) As Services.Contracts.DataCommand
        Return AddCommand(statement, tableName, operation, String.Empty)
    End Function

    ''' <summary>
    ''' AddCommand
    ''' </summary>
    ''' <param name="statement"></param>
    ''' <param name="tableName"></param>
    ''' <param name="operation"></param>
    ''' <param name="connectionStringsName">Nombre de la connexion a realizar el commando</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddCommand(statement As String, tableName As String, operation As String, connectionStringsName As String) As Services.Contracts.DataCommand
        Dim result As New Services.Contracts.DataCommand With {
                                            .Statement = statement,
                                            .TableName = tableName,
                                            .Operation = operation,
                                            .ConnectionStringName = IIf(connectionStringsName.IsEmpty(), _command.ConnectionStringName, connectionStringsName)}
        If _commands.IsEmpty Then
            _commands = New List(Of Services.Contracts.DataCommand)
        End If

        _commands.Add(result)
        Return result
    End Function

    Public Function AddCommand(statement As String, LookUp As InMotionGIT.Common.DataType.LookUpValue, tableName As String, operation As String, connectionStringsName As String) As Services.Contracts.DataCommand
        Dim result As New Services.Contracts.DataCommand With {
                                            .Statement = statement,
                                            .TableName = tableName,
                                            .Operation = operation,
                                            .LookUp = LookUp,
                                            .ConnectionStringName = IIf(connectionStringsName.IsEmpty(), _command.ConnectionStringName, connectionStringsName)}
        If _commands.IsEmpty Then
            _commands = New List(Of Services.Contracts.DataCommand)
        End If

        _commands.Add(result)
        Return result
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PackageExecuteScalar() As List(Of InMotionGIT.Common.DataType.LookUpPackage)
        Dim result As New List(Of InMotionGIT.Common.DataType.LookUpPackage)
        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        If local Then
            With New Services.DataManager
                result = .PackageExecuteScalar(_commands)
            End With
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.PackageExecuteScalar(_commands)
            End Using
        End If
        Return result
    End Function

    Public Function PackageExecuteToLookUp(Optional cacheMode As Enumerations.EnumCache = Enumerations.EnumCache.CacheWithFullParameters) As List(Of DataType.LookUpPackage)
        Dim result As New List(Of DataType.LookUpPackage)
        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then

                local = False
            End If
        Else
            local = True
        End If

        Select Case cacheMode
            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As DataType.LookUpPackage = Nothing
                Dim newCommands As New List(Of Services.Contracts.DataCommand)
                Dim newResult As New List(Of DataType.LookUpPackage)

                For Each commandData As Services.Contracts.DataCommand In _commands
                    tempData = Nothing

                    If Not CacheRefresh Then
                        tempData = IsExistLookUpCache(commandData, False)
                    End If

                    If IsNothing(tempData) Then
                        newCommands.Add(commandData)
                    Else
                        newResult.Add(tempData)
                    End If
                Next

                If newCommands.Count > 0 Then
                    If local Then
                        With New Services.DataManager
                            result = .PackageExecuteToLookUp(newCommands)
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.PackageExecuteToLookUp(newCommands)
                        End Using
                    End If

                    For Each commandData As Services.Contracts.DataCommand In newCommands
                        For Each newLookUpPackage As DataType.LookUpPackage In result

                            If commandData.TableName = newLookUpPackage.Key Then
                                QueryCacheAdd(commandData, False, newLookUpPackage)

                                Exit For
                            End If

                        Next
                    Next
                End If

                If newResult.Count > 0 Then
                    For Each newLookUpPackage As DataType.LookUpPackage In newResult
                        result.Add(newLookUpPackage)
                    Next
                End If

            Case Else
                If local Then
                    With New Services.DataManager
                        result = .PackageExecuteToLookUp(_commands)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.PackageExecuteToLookUp(_commands)
                    End Using
                End If
        End Select

        Return result
    End Function

#End Region

#Region "Internal use"

    ''' <summary>
    ''' Retorna una instancia del proxy del servicio de datos.
    ''' </summary>
    ''' <returns>Instancia del proxy del servicio de datos.</returns>
    Private Function DataServiceInstance() As DataManager.DataManagerClient
        Dim dataService As DataManager.DataManagerClient = Nothing
        If Not IsNothing(Me.DataManagerURLForce) AndAlso Not Me.DataManagerURLForce.IsEmpty Then
            dataService = InstanceDataManagerClient(Me.DataManagerURLForce)
        Else
            If "DataManager.URL".AppSettings.IsEmpty Then
                dataService = New DataManager.DataManagerClient()
            Else
                dataService = InstanceDataManagerClient("DataManager.URL".AppSettings)
            End If
        End If

        Return dataService
    End Function

    ''' <summary>
    ''' Obtiene una instancia del cliente 'DataManagerClient', condicionado al url (Https o Https)
    ''' </summary>
    ''' <param name="url">URL al que se debe apuntar la instancia de 'DataManagerClient'</param>
    ''' <returns>Retornar una Instancia de 'DataManagerClient', configurada al url que se solicitó </returns>
    Private Shared Function InstanceDataManagerClient(url As String) As Proxy.DataManager.DataManagerClient
        Dim result As Proxy.DataManager.DataManagerClient
        If url.ToLower.Contains("https") Then
            If HostingEnvironment.IsHosted Then
                result = New DataManager.DataManagerClient("BasicHttpBinding_IDataManager",
                                                      String.Format(CultureInfo.InvariantCulture,
                                                                    "{0}/DataManager.svc", url))
            Else
                result = New DataManager.DataManagerClient("BasicHttpBinding_IDataManagerHttps",
                                                     String.Format(CultureInfo.InvariantCulture,
                                                                   "{0}/DataManager.svc", url))
            End If

            If "Certificate.ForcedSelfSigned".AppSettings(Of Boolean) Then
                Common.Helpers.Certificate.OverrideCertificateValidation()
            End If
        Else
            result = New DataManager.DataManagerClient("BasicHttpBinding_IDataManager",
                                                       String.Format(CultureInfo.InvariantCulture,
                                                                     "{0}/DataManager.svc", url))
        End If
        Return result
    End Function

    ''' <summary>
    ''' Establece los parámetros de configuración asociados al limite de registros a ser retornados.
    ''' </summary>
    ''' <param name="command">Instancia de comando de datos.</param>
    Private Sub SetConfiguration(command As Services.Contracts.DataCommand)
        If MaxNumberOfRecord.IsEmpty Then
            If "DataAccessLayer.MaxNumberOfRecords".AppSettings().IsNotEmpty Then
                MaxNumberOfRecord = "DataAccessLayer.MaxNumberOfRecords".AppSettings(Of Integer)
            End If
            If Not "DataAccessLayer.IgnoreMaxNumberOfRecords".AppSettings(Of Boolean) Then
                _IgnoreMaxNumberOfRecords = True
            End If
        Else
            _IgnoreMaxNumberOfRecords = False
        End If

        With command
            .MaxNumberOfRecord = Me.MaxNumberOfRecord
            .IgnoreMaxNumberOfRecords = Me._IgnoreMaxNumberOfRecords
            .QueryCount = Me.QueryCount
        End With

        Dim Sql As String = command.Statement
        If Me.AllowHistoryInfo Then
            Dim beginIndex As Integer = Sql.IndexOf("@@BEGIN_HISTORICAL_MODE@@") + 25

            If beginIndex > -1 Then
                Dim endIndex As Integer = Sql.IndexOf("@@END_HISTORICAL_MODE@@", beginIndex)

                If endIndex > -1 Then
                    Dim condition As String = Sql.Substring(beginIndex, endIndex - beginIndex)

                    Sql = Sql.Replace(condition, String.Empty)

                    Sql = Sql.Replace(" AND @@BEGIN_HISTORICAL_MODE@@", String.Empty)
                    Sql = Sql.Replace("@@BEGIN_HISTORICAL_MODE@@", String.Empty)
                    Sql = Sql.Replace("@@END_HISTORICAL_MODE@@", String.Empty)
                End If
            End If
        Else
            If Sql.IndexOf("@@BEGIN_HISTORICAL_MODE@@") > -1 Then
                Sql = Sql.Replace("@@BEGIN_HISTORICAL_MODE@@", String.Empty)
                Sql = Sql.Replace("@@END_HISTORICAL_MODE@@", String.Empty)
            End If
        End If
        command.Statement = Sql

    End Sub

#End Region

    ''' <summary>
    ''' Ejecuta una instrucción 'select' usada para una lista de valores. la instrucción select debe retornar solo dos columnas la que representa el código y la que representa la descripción.
    ''' </summary>
    ''' <returns>Lista de valores del tipo código y descripción.</returns>
    Public Function QueryExecuteToLookup() As List(Of DataType.LookUpValue)
        Return QueryExecuteToLookup(String.Empty)
    End Function

    ''' <summary>
    ''' Ejecuta una instrucción 'select' usada para una lista de valores. la instrucción select debe retornar solo dos columnas la que representa el código y la que representa la descripción.
    ''' </summary>
    '''<param name="emptyOption">Si está lleno indica que se debe agregar un elemento a la lista normalmente usando para indicar un valor vacío.</param>
    ''' <returns>Lista de valores del tipo código y descripción.</returns>
    Public Function QueryExecuteToLookup(emptyOption As String) As List(Of DataType.LookUpValue)
        Dim result As List(Of DataType.LookUpValue)
        Dim resultFiltered As DataTable = Nothing
        With _command
            .Operation = "Query"
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .ConnectionStringsRaw = Me.ConnectionStringsRaw
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        If Not CacheRefresh Then
            result = IsExistQueryCacheList(_command, False)
            If result.IsEmpty() Then
                If local Then
                    With New Common.Services.DataManager
                        result = .QueryExecuteToLookup(_command)
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.QueryExecuteToLookup(_command)
                    End Using
                End If
                If result.IsNotEmpty Then
                    If emptyOption.IsNotEmpty Then
                        Dim emptyCode As String = "0"

                        If emptyOption.Equals("0") Then
                            emptyOption = String.Empty
                        ElseIf emptyOption.Equals("-1") Then
                            emptyCode = "-1"
                        ElseIf emptyOption.Contains(":") Then
                            emptyCode = emptyOption.Split(":")(0)
                            emptyOption = emptyOption.Split(":")(1)
                        End If
                        If emptyCode.IsEmpty Then
                            emptyCode = "0"
                        End If
                        result.Insert(0, New DataType.LookUpValue With {.Code = emptyCode, .Description = emptyOption})
                    End If
                End If
                QueryCacheListAdd(_command, False, result)
            End If
        Else
            If local Then
                With New Common.Services.DataManager
                    result = .QueryExecuteToLookup(_command)
                End With
            Else
                Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                    result = dataService.QueryExecuteToLookup(_command)
                End Using
            End If
            If result.IsNotEmpty Then
                If emptyOption.IsNotEmpty Then
                    Dim emptyCode As String = "0"

                    If emptyOption.Equals("0") Then
                        emptyOption = String.Empty
                    ElseIf emptyOption.Equals("-1") Then
                        emptyCode = "-1"
                    ElseIf emptyOption.Contains(":") Then
                        emptyCode = emptyOption.Split(":")(0)
                        emptyOption = emptyOption.Split(":")(1)
                    End If
                    If emptyCode.IsEmpty Then
                        emptyCode = "0"
                    End If
                    result.Insert(0, New DataType.LookUpValue With {.Code = emptyCode, .Description = emptyOption})
                End If
            End If
            QueryCacheListAdd(_command, False, result)
        End If
        Return result
    End Function

    ''' <summary>
    ''' Get All connections validating for code.
    ''' </summary>
    ''' <param name="CodeValidator"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConnectionStringAll(CodeValidator As String) As List(Of Services.Contracts.ConnectionString)
        Dim local As Boolean = True
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            Return Common.Helpers.ConnectionStrings.ConnectionStringGetAll(CodeValidator, CompanyId)
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                Dim companyId As Integer = CompanyIdSelect()
                Return dataService.ConnectionStringGetAll(CodeValidator, companyId)
            End Using
        End If

    End Function

    ''' <summary>
    ''' Get one connectionstring specific
    ''' </summary>
    ''' <param name="ConnectionStrinName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConnectionStringGet(ConnectionStrinName As String) As Services.Contracts.ConnectionString
        Dim local As Boolean = True
        Dim companyId As Integer = CompanyIdSelect()
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            Return Common.Helpers.ConnectionStrings.ConnectionStringGet(ConnectionStrinName, companyId)
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                Return dataService.ConnectionStringGet(ConnectionStrinName, companyId)
            End Using
        End If
    End Function

    ''' <summary>
    ''' Get Credential for ConnectionString in specific
    ''' </summary>
    ''' <param name="ConecctionStringName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConnectionStringUserAndPassword(ConecctionStringName As String) As Services.Contracts.Credential
        Dim local As Boolean = True
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            Return Common.Helpers.ConnectionStrings.ConnectionStringUserAndPassword(ConecctionStringName, CompanyId)
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                Dim companyId As Integer = CompanyIdSelect()
                Return dataService.ConnectionStringUserAndPassword(ConecctionStringName, companyId)
            End Using
        End If
    End Function

    Public Function AppInfo(path As String) As Services.Contracts.info
        Dim local As Boolean = True
        Dim companyId As Integer = CompanyIdSelect()
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        If local Then
            Return InMotionGIT.Common.Services.Contracts.info.Process(path)
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                Return dataService.AppInfo(path)
            End Using
        End If
    End Function

    Public Function GetSettingValue(repositoryName As String, settingName As String) As String
        Dim local As Boolean = True
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        Dim result As String = String.Empty

        If local Then
            result = (New Services.DataManager).GetSettingValue(repositoryName, settingName)
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.GetSettingValue(repositoryName, settingName)
            End Using
        End If
        Return result
    End Function

    Public Shared Function DateValue(repositoryName As String, value As Date) As String
        Dim key As String = String.Format("{0}.DateFormat", repositoryName)
        Dim dateformat As String = String.Empty

        If Common.Helpers.Caching.Exist(key) Then
            dateformat = Common.Helpers.Caching.GetItem(key)
        Else
            With New Proxy.DataManagerFactory
                dateformat = .GetSettingValue(repositoryName, "DateFormat")
            End With
            If dateformat.IsNotEmpty Then
                Common.Helpers.Caching.SetItem(key, dateformat)
            End If
        End If

        Dim result As String = value.ToString(dateformat, New CultureInfo("en-US"))
        'result = result.Replace("/mm/", "/")
        Return result
    End Function

#Region "Research"

    Public Sub CommandExecuteAsynchronous()
        CommandExecuteAsynchronous(True)
    End Sub

    Private Sub CommandExecuteAsynchronousInternal(parameterInternal As Dictionary(Of String, Object))
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        Try

            If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
                watch = New Stopwatch
                watch.Start()
            End If

            With _command
                .Operation = "CommandExecuteAsynchronous"
                If _parameters.IsNotEmpty Then
                    .Parameters = _parameters '.ToArray
                End If
                .ConnectionStringsRaw = Me.ConnectionStringsRaw
                .Fields = Fields()
            End With

            SetConfiguration(_command)

            Dim local As Boolean = True

            If Not ForceLocalMode Then
                If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                    local = False
                End If
            Else
                local = True
            End If

            If local Then
                With New Services.DataManager
                    .CommandExecuteAsynchronous(_command)
                End With
            Else
                Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                    dataService.CommandExecuteAsynchronous(_command)
                End Using
            End If

            If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) AndAlso Not local Then
                watch.Stop()
                message = "{2}              Mode: {1}{2}              Id: {0}{2}              {3}".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command))
                If _command.Fields.IsNotEmpty() Then
                    message &= "{1}              Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine)
                End If
                message &= "              Time Executed={0} ms".SpecialFormater(watch.ElapsedMilliseconds)
                If "DataAccessLayer.Debug.Proxy.Detail".AppSettings(Of Boolean) Then
                    message &= Environment.NewLine + StackTraceSummary()
                End If
                InMotionGIT.Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message)
            End If
        Catch ex As Exception
            Common.Helpers.LogHandler.ErrorLog("CommandExecuteAsynchronousInternal", "CommandExecuteAsynchronousInternal Error", ex, String.Empty, False)
        End Try
    End Sub

    Public Sub CommandExecuteAsynchronous(ByVal Async As Boolean)

        Dim action As Action(Of Object) =
                        Sub(parameterContainer As Object)
                            Dim parameterInternal As Dictionary(Of String, Object) = DirectCast(parameterContainer, Dictionary(Of String, Object))
                            CommandExecuteAsynchronousInternal(parameterInternal)
                        End Sub

        Dim parameters As New Dictionary(Of String, Object)
        If Async Then
            Dim AddUsersSecurityTraceAsyn As New Task(action, parameters)
            AddUsersSecurityTraceAsyn.Start()
        Else
            CommandExecuteAsynchronousInternal(parameters)
        End If

    End Sub

    ''' <summary>
    ''' Overrible of method QueryExecuteToTableJSON
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QueryExecuteToTableJSON() As DataTable
        Return QueryExecuteToTableJSON(False)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="resultEmpty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function QueryExecuteToTableJSON(resultEmpty As Boolean) As DataTable
        Dim result As DataTable = Nothing
        Dim resultFiltered As DataTable = Nothing
        Dim message As String = String.Empty
        Dim watch As Stopwatch = Nothing

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) Then
            watch = New Stopwatch
            watch.Start()
        End If

        With _command
            .Operation = "Query"
            If _parameters.IsNotEmpty Then
                .Parameters = _parameters '.ToArray
            End If
            .Fields = Fields()
        End With

        Dim local As Boolean = True

        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If

        SetConfiguration(_command)

        Select Case Cache
            Case Enumerations.EnumCache.None
                If local Then
                    With New Common.Services.DataManager
                        result = .QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                    End With
                Else
                    Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                        result = dataService.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                    End Using
                End If

            Case Enumerations.EnumCache.CacheWithFullParameters
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, False)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End Using
                    End If
                    QueryCacheAdd(_command, False, result)
                End If

            Case Enumerations.EnumCache.CacheWithCommand
                Dim tempData As DataTable = Nothing
                If Not CacheRefresh Then
                    tempData = IsExistQueryCache(_command, True)
                End If
                If Not IsNothing(tempData) Then
                    result = tempData
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End Using
                    End If
                    QueryCacheAdd(_command, True, result)
                End If
            Case Enumerations.EnumCache.CacheOnDemand
                Dim tempOndemandContainer As OndemandContainer = Nothing
                If Not CacheRefresh Then
                    Dim temporalCache = IsExistQueryCacheOnDemand(_command)
                    If IsNothing(temporalCache) Then
                        tempOndemandContainer = temporalCache
                    Else
                        If GetType(OndemandContainer) = IsExistQueryCacheOnDemand(_command).GetType Then
                            tempOndemandContainer = temporalCache
                        Else
                            tempOndemandContainer = Nothing
                        End If
                    End If
                End If
                If Not IsNothing(tempOndemandContainer) Then
                    Dim isfund = (From itemfound In tempOndemandContainer.KeyWichtParameters
                                  Where itemfound.Contains(GetMd5Hash(_command)) Select itemfound).SingleOrDefault
                    If Not IsNothing(isfund) Then
                        result = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                        End If
                    Else
                        If local Then
                            With New Common.Services.DataManager
                                result = .QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                            End With
                        Else
                            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                                result = dataService.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                            End Using
                        End If
                        QueryCacheAddOnDemand(_command, True, result)
                        result = tempOndemandContainer.Data
                        If Not String.IsNullOrEmpty(CacheFilter) Then
                            resultFiltered = DataTableImport(result, CacheFilter, _parameters)
                        End If
                    End If
                Else
                    If local Then
                        With New Common.Services.DataManager
                            result = .QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End With
                    Else
                        Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                            result = dataService.QueryExecuteToTableJSON(_command, resultEmpty).Deserialize()
                        End Using
                    End If
                    QueryCacheAddOnDemand(_command, True, result)
                End If

        End Select

        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) AndAlso Not local Then
            watch.Stop()
            message = "{2}              Mode: {1}{2}              Id: {0}{2}              Type Chache:{4}{2}              {3}".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command), Cache)
            If _command.Fields.IsNotEmpty() Then
                message &= "{1}              Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine)
            End If
            message &= "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds)
            If Not IsNothing(result) AndAlso Not IsNothing(result.Rows) Then
                message &= Environment.NewLine + String.Format("              Rows={0}", result.Rows.Count)
            Else
                message &= Environment.NewLine + String.Format("              Rows={0}", 0)
            End If
            message &= Environment.NewLine + String.Format("              Cache={0}", Cache)
            If "DataAccessLayer.Debug.Proxy.Detail".AppSettings(Of Boolean) Then
                message &= Environment.NewLine + StackTraceSummary()
            End If
            InMotionGIT.Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message)
        End If

        If Not IsNothing(resultFiltered) Then
            Return resultFiltered
        Else
            Return result
        End If

    End Function

#End Region

#Region "Class Helpers"

    Private Class OndemandContainer
        Private _ListKeyWichtParameters As List(Of String)

        Public Property KeyWichtParameters() As List(Of String)
            Get
                If IsNothing(_ListKeyWichtParameters) Then
                    _ListKeyWichtParameters = New List(Of String)
                End If
                Return _ListKeyWichtParameters
            End Get
            Set(ByVal value As List(Of String))
                _ListKeyWichtParameters = value
            End Set
        End Property

        Public Property Key As String
        Public Property Data As DataTable
    End Class

#End Region

    Public Function CurrentTime() As String
        Dim local As Boolean = True
        If Not ForceLocalMode Then
            If "DataManager.Mode".AppSettingsOnEquals("remote") Then
                local = False
            End If
        Else
            local = True
        End If
        Dim result As String

        If local Then
            result = (New Services.DataManager).CurrentTime
        Else
            Using dataService As DataManager.DataManagerClient = DataServiceInstance()
                result = dataService.CurrentTime
            End Using
        End If
        Return result
    End Function

    Private Shared Function StackTraceSummary() As String
        Dim result As New StringBuilder
        Dim stackTrace As New StackTrace()
        Dim stackFrames As StackFrame() = stackTrace.GetFrames()
        If stackFrames.IsNotEmpty AndAlso stackFrames.Length > 0 Then
            For Each stackCall As StackFrame In stackFrames
                With stackCall
                    If .GetMethod().IsNotEmpty AndAlso .GetMethod().Name.IsNotEmpty Then
                        Dim [nameSpace] As String = String.Empty
                        If .GetMethod().ReflectedType.IsNotEmpty AndAlso .GetMethod().ReflectedType.FullName.IsNotEmpty Then
                            Dim className As String = .GetMethod().ReflectedType.FullName
                            [nameSpace] = String.Format("{0}.{1}", className, .GetMethod().Name)
                        End If
                        If [nameSpace].IsEmpty Then
                            [nameSpace] = "Empty"
                        End If
                        If Not [nameSpace].StartsWith("System.") Then
                            If result.Length = 0 Then
                                result.AppendLine(String.Format("              Stack: {0}", [nameSpace]))
                            Else
                                result.AppendLine(String.Format("                     {0}", [nameSpace]))
                            End If

                        End If
                    End If
                End With
            Next
        End If
        Return result.ToString
    End Function

    Private Shared Function CommandSummary(command As InMotionGIT.Common.Services.Contracts.DataCommand) As String
        Dim result As String = String.Empty

        If command.IsNotEmpty Then
            result = String.Format("{0} {1}{2}", command.Operation, command.Statement, vbCrLf)

            If command.Parameters.IsNotEmpty Then

                For Each item As Services.Contracts.DataParameter In command.Parameters
                    result &= String.Format("              {0}=", item.Name)

                    If IsDBNull(item.Value) Then
                        result &= "Null"
                    Else
                        If item.Type = DbType.StringFixedLength Then
                            result &= String.Format("'{0}',", item.Value)
                        ElseIf item.Type = DbType.Date Then
                            result &= String.Format("TO_DATE('{0}', 'MM/DD/YYYY HH24:MI:SS'),", DirectCast(item.Value, Date).ToString("MM/dd/yyyy HH:mm:ss"))
                        Else
                            result &= String.Format("{0},", item.Value)
                        End If

                    End If
                    result &= vbCrLf
                Next
            End If
        End If

        Return result
    End Function

    Public Sub TraceLog(watch As Stopwatch, local As Boolean, message As String, result As Object, method As String)
        If "DataAccessLayer.Debug.Proxy".AppSettings(Of Boolean) AndAlso Not local Then
            watch.Stop()
            message = "{2}              Method: {5}            {2}              {3}              Mode: {1}{2}              Id: {0}{2}              Type Chache:{4}{2}              ".SpecialFormater(_id, "DataManager.Mode".AppSettings(), Environment.NewLine, CommandSummary(_command), Cache, method)
            If _command.Fields.IsNotEmpty() Then
                message = message.TrimEnd(Environment.NewLine)
                message &= "Parámetros:{0}".SpecialFormater(_command.Fields.ToStringExtended(), Environment.NewLine)
            End If
            message &= "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds)
            Dim value As String

            Select Case result.GetType().Name
                Case "String", "Int32"
                    value = result.ToString()
                Case "List(Of String)"
                    value = String.Join(",", DirectCast(result, List(Of String)))
                Case "DataTable"
                    Dim valueTable = DirectCast(result, DataTable)
                    If Not IsNothing(result) AndAlso Not IsNothing(result.Rows) Then
                        message &= Environment.NewLine + String.Format("              Rows={0}", valueTable.Rows.Count)
                    Else
                        message &= Environment.NewLine + String.Format("              Rows={0}", 0)
                    End If
                Case Else

            End Select

            message &= "              Scalar={0}".SpecialFormater(value)
            If "DataAccessLayer.Debug.Proxy.Detail".AppSettings(Of Boolean) Then
                message &= Environment.NewLine + StackTraceSummary()
            End If
            InMotionGIT.Common.Helpers.LogHandler.TraceLog("DataAccessFactory", message)
        End If
    End Sub

    Private Function Fields() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"CompanyId", _command.CompanyId},
            {
                "Id", _id
            },
            {
                "Origen", "Logs.Prefix".StringValue()
            }
        }
    End Function

End Class