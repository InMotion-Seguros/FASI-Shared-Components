Imports System.Configuration
Imports System.Data.Common
Imports System.Globalization
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web
Imports Oracle.DataAccess.Client

Namespace Helpers

    Public Class DataAccessLayer
        Implements IDisposable

#Region "Private Members"

        Private _dbProviderFactory As DbProviderFactory = Nothing
        Private _dbConnection As DbConnection = Nothing

#End Region

#Region "Public properties"

        Public Property ProviderName As String
        ' Public Property DateFormat As String

#End Region

#Region "Public Members"

        Public Sub Release()
            If Not IsNothing(_dbConnection) Then
                CloseDbConnection(_dbConnection)
            End If
            _dbProviderFactory = Nothing
        End Sub

        Public Function ExecuteQuery(ByVal query As String, ByVal connectionStringName As String, companyId As Integer) As DataTable
            Dim result As DataTable = Nothing

            If IsNothing(_dbConnection) Then
                _dbConnection = OpenDbConnection(connectionStringName)
            End If

            If Not IsNothing(_dbConnection) Then
                Dim commandItem As DbCommand = _dbConnection.CreateCommand()
                commandItem.CommandType = CommandType.Text
                commandItem.CommandText = query
                Dim dataReaderItem As DbDataReader = DataAccessLayer.QueryExecute(commandItem, _dbConnection, CommandBehavior.Default, "Dynamic", connectionStringName, companyId)

                result = New DataTable
                result.Load(dataReaderItem)
                If String.IsNullOrEmpty(result.TableName) Then
                    result.TableName = "Result"
                End If
                dataReaderItem.Close()
                dataReaderItem = Nothing
            End If

            Return result
        End Function

        Public Function ExecuteQueryScalar(ByVal query As String, ByVal connectionStringName As String) As Integer
            Dim result As Long = 0
            Dim internalResult As Object = Nothing

            If IsNothing(_dbConnection) Then
                _dbConnection = OpenDbConnection(connectionStringName)
            End If

            If Not IsNothing(_dbConnection) Then
                Dim commandItem As DbCommand = _dbConnection.CreateCommand()
                commandItem.CommandType = CommandType.Text
                commandItem.CommandText = query
                internalResult = commandItem.ExecuteScalar
                If Not IsDBNull(internalResult) Then
                    result = internalResult
                End If
            End If

            Return result
        End Function

        Public Function ProcedureExecuteWithDataTableResultset(ByVal storedProcedureName As String, ByVal parameterList As List(Of DataType.Parameter), ByVal connectionStringName As String) As DataTable
            Dim result As DataTable = Nothing

            If IsNothing(_dbConnection) Then
                _dbConnection = OpenDbConnection(connectionStringName)
            End If

            If Not IsNothing(_dbConnection) Then
                Dim commandItem As DbCommand = _dbConnection.CreateCommand()
                commandItem.CommandType = CommandType.StoredProcedure
                commandItem.CommandText = PreprocessStatement(storedProcedureName, _dbConnection.ToString)

                For Each item As DataType.Parameter In parameterList
                    item.CreateCommandParameter(commandItem)
                Next

                result = ExecuteWithDataTable(commandItem, _dbConnection, False)
            End If

            Return result
        End Function

#End Region

#Region "Shared Members"

        ''' <summary>
        ''' Método que consume por los servicios tipo json en su canal
        ''' </summary>
        ''' <param name="command">Comando a ejecutar</param>
        ''' <param name="currentConnection">Connexion actual</param>
        ''' <param name="behavior">Ambiente de ejecución</param>
        ''' <param name="table">Nombre de la tabla</param>
        ''' <param name="returnEmptyDataTable">retornar data-table vació</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function QueryExecuteToTableJSON(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String, returnEmptyDataTable As Boolean, parameters As Dictionary(Of String, Object)) As DataTable
            Return QueryExecuteToTable(command, currentConnection, behavior, table, returnEmptyDataTable, parameters)
        End Function

        Public Shared Function QueryExecuteToTable(statement As String, currentConnection As DbConnection, behavior As CommandBehavior, table As String, parameters As Dictionary(Of String, Object)) As DataTable
            Dim result As DataTable = Nothing
            Dim commandItem As DbCommand = currentConnection.CreateCommand()
            commandItem.CommandType = CommandType.Text
            commandItem.CommandText = PreprocessStatement(statement, currentConnection.ToString)
            Dim dataReaderItem As DbDataReader = QueryExecute(commandItem, currentConnection, behavior, table)
            If dataReaderItem.HasRows Then
                result = New DataTable
                result.Load(dataReaderItem)
                result.TableName = table
            End If
            dataReaderItem.Close()
            dataReaderItem = Nothing
            Return result
        End Function

        Public Shared Function QueryExecuteToTable(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String, returnEmptyDataTable As Boolean, parameters As Dictionary(Of String, Object)) As DataTable
            Dim result As DataTable = Nothing
            Dim Message As String = ""
            Dim indexStack As Integer = IIf("DataManager.Mode".AppSettingsOnEquals("remote"), 4, 2)

            Dim dataReaderItem As DbDataReader = QueryExecute(command, currentConnection, behavior, table, Message, True)

            If behavior = CommandBehavior.SchemaOnly Then
                result = dataReaderItem.GetSchemaTable
            Else
                If dataReaderItem.HasRows OrElse
                   returnEmptyDataTable Then
                    Dim watch As Stopwatch
                    watch = New Stopwatch
                    watch.Start()
                    result = New DataTable
                    result.Load(dataReaderItem)
                    result.TableName = table
                    watch.Stop()
                    If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                        If parameters.IsNotEmpty() Then
                            Message = "{2}{0}{1}{0}{2}{0}{3}".SpecialFormater("              ", "Parámetros:" + parameters.ToStringExtended(), Environment.NewLine, Message)
                        End If
                        Message = Message + "              Time retrieve={0} ms".SpecialFormater(watch.ElapsedMilliseconds)
                        Message = Message + Environment.NewLine + "              Rows={0}".SpecialFormater(result.Rows.Count)
                        If "DataAccessLayer.Debug.DetailsCall".AppSettings(Of Boolean) Then
                            Dim detailsCall As String = AssemblyHandler.GetFrameProcess(indexStack)
                            If detailsCall.IsNotEmpty Then
                                Message &= vbCrLf & String.Format("{0}", detailsCall.ToString.Replace("<<I>>", "              "))
                            End If
                        End If
                        LogHandler.TraceLog("DataAccessLayer", Message)
                    End If
                End If
            End If

            dataReaderItem.Close()
            dataReaderItem = Nothing
            Return result
        End Function

        Public Shared Function QueryExecuteToTable(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String, parameters As Dictionary(Of String, Object)) As DataTable
            Return QueryExecuteToTable(command, currentConnection, behavior, table, False, parameters)
        End Function

        Public Shared Function QueryExecuteToTableString(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String, parameters As Dictionary(Of String, Object)) As String
            Dim dTable As DataTable = QueryExecuteToTable(command, currentConnection, behavior, table, parameters)
            Dim result As String = String.Empty

            If dTable.IsNotEmpty Then
                Using xmlitem As New IO.StringWriter
                    dTable.WriteXml(xmlitem, XmlWriteMode.WriteSchema)
                    result = xmlitem.ToString
                End Using
            End If

            command.Dispose()
            command = Nothing

            Return result
        End Function

        Public Shared Function StringToTable(stringTable As String) As DataTable
            Dim dTable As New DataTable
            If stringTable.IsNotEmpty Then
                Using xmlStream As New IO.StringReader(stringTable)
                    dTable.ReadXml(xmlStream)
                End Using
            End If
            Return dTable
        End Function

        Public Shared Function QueryExecuteWithMapper(Of T As New)(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String) As T
            Dim result As DbDataReader = Nothing
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            Try
                watch = New Stopwatch
                watch.Start()
                result = command.ExecuteReader(behavior)
                watch.Stop()
                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              HasRows={0}", result.HasRows)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
                command.Connection = Nothing
                command = Nothing
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try

            Dim businessEntityType As Type = GetType(T)
            Dim hashtable As New Hashtable()
            Dim properties As PropertyInfo() = businessEntityType.GetProperties()
            Dim info As PropertyInfo
            Dim newObject As T
            Dim value As Object
            For Each info In properties
                hashtable(info.Name.ToUpper()) = info
            Next
            If result.Read() Then
                newObject = New T

                For index As Integer = 0 To result.FieldCount - 1
                    info = DirectCast(hashtable(result.GetName(index).ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite AndAlso Not result.IsDBNull(index) Then
                        value = Convert.ChangeType(result.GetValue(index), info.PropertyType)
                        info.SetValue(newObject, value, Nothing)
                    End If
                Next
            End If
            result.Close()
            result = Nothing
            Return newObject
        End Function

        Public Shared Function QueryExecuteWithMapper(Of T As New, Y As New)(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String) As Y
            Dim result As DbDataReader = Nothing
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            Try
                watch = New Stopwatch
                watch.Start()
                result = command.ExecuteReader(behavior)

                watch.Stop()

                If "DataAccessLayer.Debug".AppSettings(Of Boolean) = True Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              HasRows={0}", result.HasRows)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
                command.Connection = Nothing
                command = Nothing
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try

            Dim businessEntityType As Type = GetType(T)
            Dim entitys As New Y
            Dim ientitys As IList = DirectCast(entitys, IList)
            Dim hashtable As New Hashtable()
            Dim properties As PropertyInfo() = businessEntityType.GetProperties()
            Dim info As PropertyInfo
            Dim newObject As T
            Dim value As Object
            For Each info In properties
                hashtable(info.Name.ToUpper()) = info
            Next
            Do While result.Read()
                newObject = New T

                For index As Integer = 0 To result.FieldCount - 1
                    info = DirectCast(hashtable(result.GetName(index).ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite AndAlso Not result.IsDBNull(index) Then
                        value = Convert.ChangeType(result.GetValue(index), info.PropertyType)
                        info.SetValue(newObject, value, Nothing)

                    End If
                Next
                ientitys.Add(newObject)
            Loop
            result.Close()
            Return entitys
        End Function

        Public Shared Function QueryExecute(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String) As DbDataReader
            Return QueryExecute(command, currentConnection, behavior, table, String.Empty, False)
        End Function

        Public Shared Function QueryExecute(command As DbCommand, currentConnection As DbConnection, behavior As CommandBehavior, table As String, ByRef Message As String, IsRetrieve As Boolean) As DbDataReader
            Dim result As DbDataReader = Nothing
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            For attempts As Integer = 1 To 3
                command.Connection = currentConnection
                Try
                    watch = New Stopwatch
                    watch.Start()
                    If currentConnection.ToString.StartsWith("Oracle.DataAccess.Client", StringComparison.CurrentCultureIgnoreCase) Then
                        With DirectCast(command, OracleCommand)
                            .InitialLONGFetchSize = -1
                        End With
                    End If

                    result = command.ExecuteReader(behavior)
                    watch.Stop()

                    If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                        Dim messageInternal As String = DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False)
                        If Not IsRetrieve Then
                            messageInternal &= "              HasRows={0}".SpecialFormater(result.HasRows)
                        End If
                        messageInternal &= vbCrLf & "              Time Execution={0} ms".SpecialFormater(watch.ElapsedMilliseconds)
                        If "DataAccessLayer.Debug.DetailsCall".AppSettings(Of Boolean) Then
                            Dim detailsCall As String = AssemblyHandler.MethodName(System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower())
                            If detailsCall.IsNotEmpty Then
                                messageInternal &= vbCrLf & "{0}".SpecialFormater(detailsCall.ToString.Replace("<<I>>", "              "))
                            End If

                        End If
                        If Not IsRetrieve Then
                            LogHandler.TraceLog("DataAccessLayer", messageInternal)
                        Else
                            Message = messageInternal
                        End If
                    End If

                    Exit For
                Catch exOracle As OracleException
                    If (exOracle.Message.StartsWith("ORA-03135:") OrElse
                        exOracle.Message.StartsWith("ORA-03113:") OrElse
                        exOracle.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        exOracle.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        exOracle.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) AndAlso attempts < 3 Then
                        Dim magicMethod As MethodInfo = currentConnection.GetType.GetMethod("ClearAllPools")

                        For connectAttempts As Integer = 1 To 3
                            command.Connection = Nothing
                            If currentConnection.State = ConnectionState.Open Then
                                currentConnection.Close()
                            End If
                            If magicMethod.IsNotEmpty Then
                                magicMethod.Invoke(currentConnection, New Object() {})
                            End If
                            LogHandler.WarningLog("DataAccessLayer", String.Format("Retry due to disconnection for query on table '{2}' ({0}/{3}). {1}", attempts, exOracle.Message, table, connectAttempts))
                            Thread.Sleep(1000)
                            Try
                                currentConnection.Open()
                                Exit For
                            Catch ex2 As Exception
                                If connectAttempts >= 3 Then
                                    Dim temporalException = Exceptions.DataAccessException.Factory(exOracle, command, table, "Query")
                                    ConnectionClosed(command, currentConnection)
                                    Throw temporalException
                                End If

                            End Try
                        Next
                    Else
                        command.Connection = Nothing
                        If currentConnection.State = ConnectionState.Open Then
                            currentConnection.Close()
                        End If
                        Dim parameter = New Dictionary(Of String, Object)
                        With parameter
                            '.Add("connectionName", ConnectionStringName)
                            .Add("connectionString", currentConnection.ToString())
                            '.Add("companyId", CompanyId.ToString())
                        End With

                        Throw ExceptionOracleProcess(exOracle, command, table, "Query", parameter)
                    End If
                Catch ex As Exception
                    If (ex.Message.StartsWith("ORA-03135:") OrElse
                        ex.Message.StartsWith("ORA-03113:") OrElse
                        ex.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        ex.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        ex.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) AndAlso attempts < 3 Then
                        Dim magicMethod As MethodInfo = currentConnection.GetType.GetMethod("ClearAllPools")

                        If magicMethod.IsNotEmpty Then
                            magicMethod.Invoke(currentConnection, New Object() {})
                        End If
                        'command.Connection = Nothing
                        'If currentConnection.State = ConnectionState.Open Then
                        '    currentConnection.Close()
                        'End If
                        LogHandler.WarningLog("DataAccessLayer", String.Format("Retry due to disconnection for query on table '{2}' ({0}). {1}", attempts, ex.Message, table))
                        Thread.Sleep(1000)
                        Try
                            currentConnection.Open()
                        Catch ex2 As Exception
                            Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                            ConnectionClosed(command, currentConnection)
                            Throw temporalException
                        End Try
                    Else
                        Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                        ConnectionClosed(command, currentConnection)
                        Throw temporalException
                        Exit For
                    End If
                End Try
            Next
            Return result
        End Function

        Private Shared Function ExceptionOracleProcess(exOracle As OracleException, command As DbCommand, nameObject As String, commandKind As String) As Exceptions.DataAccessException
            Return ExceptionOracleProcess(exOracle, command, nameObject, commandKind, Nothing)
        End Function

        Private Shared Function ExceptionOracleProcess(exOracle As OracleException, command As DbCommand, nameObject As String, commandKind As String, parameters As Dictionary(Of String, Object)) As Exceptions.DataAccessException
            Dim code As Integer = exOracle.Number
            Dim IsDeveloperMode As Boolean = False
            Dim message As String = ""
            Dim messageValue As String = String.Empty

            If parameters.IsNotEmpty() AndAlso parameters.Count <> 0 Then
                ' messageValue = String.Format("The ConnectionString used is: '{0}'", parameters("connectionString"))
                If (parameters.ContainsKey("connectionName")) Then
                    messageValue = String.Format("Its ConnectionName is: '{0}'", parameters("connectionName"))
                End If

                If (messageValue.IsNotEmpty() AndAlso parameters.ContainsKey("companyId")) Then
                    messageValue = messageValue + String.Format(", The Id of the company is: '{0}'", parameters("companyId"))
                End If
            End If

            If "Working.Mode".AppSettings().ToLower() = "Development" Then
                IsDeveloperMode = True
            End If

            Dim exceptionTemporal As Exceptions.DataAccessException = Nothing
            Select Case code
                        'ORA-28000: the account is locked
                Case 28000
                    message = "The account is locked, This happens for several attempts with the user or incorrect password, please contact your database administrator"

                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If

                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)
                        'ORA-01017: Invalid username/password; logon denied
                Case 1017
                    message = "Invalid user-name/password, The error is generated incorrect password or user, please contact your database administrator"
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)
                        'ORA-28001: the password has expired
                Case 28001
                    message = "The password has expired, this error is generated by the user password expired, please contact your database administrator"
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)
                        'Error con el tns
                Case 12154, 12153, 12152, 12151, 12150
                    message = String.Format("There is an error related to the TNS, the specific error code is {0}", code)
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)

                Case 936
                    message = String.Format("Missing expression, the specific error code is {0}", code)
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)

                Case 942
                    message = String.Format("The table or view named '{0}' does not exist,  the specific error code is {1}", nameObject, code)
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)

                Case 904
                    Dim reg = New Regex(""".*?""")
                    Dim matches = reg.Matches(exOracle.Message)
                    If matches.IsNotEmpty AndAlso matches.Count <> 0 Then
                        Dim item As System.Text.RegularExpressions.Match
                        If matches.Count = 0 Then
                            item = (From itemMatches In matches Select itemMatches).FirstOrDefault
                        Else
                            item = matches(1)
                        End If

                        exceptionTemporal = New Exceptions.DataAccessException(String.Format("The table '{0}' does not contain column '{1}'", nameObject, item.ToString.Replace(Chr(34), String.Empty)), exOracle, command, nameObject, commandKind)
                    Else
                        exceptionTemporal = New Exceptions.DataAccessException(String.Format("The table '{0}' does not contain column ", nameObject), exOracle, command, nameObject, commandKind)
                    End If
                Case Else
                    If Not IsDeveloperMode Then
                        message = String.Format("An error occurred while trying to perform a command in the database,  the specific error code is {0}", code)
                    Else
                        message = String.Format("An error occurred while trying to perform a command in the database. Detail:'{0}'", exOracle.Message)
                    End If
                    If messageValue.IsNotEmpty() Then
                        message = String.Format("{0},{1}{2}", message, Environment.NewLine, messageValue)
                    End If
                    exceptionTemporal = New Exceptions.DataAccessException(message, exOracle, command, nameObject, commandKind)
            End Select

            Return exceptionTemporal
        End Function

        Public Shared Function ProcedureExecute(command As DbCommand, currentConnection As DbConnection, parameters As Dictionary(Of String, Object)) As Integer
            Return CommandExecute(command, currentConnection, command.CommandText, "ProcedureExecute", parameters)
        End Function

        Public Shared Function ProcedureExecuteWithDataReaderResultset(command As DbCommand, currentConnection As DbConnection) As DbDataReader
            Return ExecuteWithResultset(command, currentConnection)
        End Function

        Public Shared Function ProcedureExecuteWithDataTableResultset(command As DbCommand, currentConnection As DbConnection) As DataTable
            Return ExecuteWithDataTable(command, currentConnection, False)
        End Function

        Public Shared Function ProcedureExecuteWithDataTableResultset(command As DbCommand, currentConnection As DbConnection, schemaOnly As Boolean) As DataTable
            Return ExecuteWithDataTable(command, currentConnection, schemaOnly)
        End Function

        Public Shared Function QueryExecuteScalar(Of T)(command As DbCommand, currentConnection As DbConnection, table As String, Optional parameters As Dictionary(Of String, Object) = Nothing) As T
            Dim result As T
            Dim watch As Stopwatch
            Dim internalResult As Object = Nothing
            Dim indexStack As Integer = IIf("DataManager.Mode".AppSettingsOnEquals("remote"), 4, 2)

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            Try
                watch = New Stopwatch
                watch.Start()
                internalResult = command.ExecuteScalar
                If Not IsDBNull(internalResult) Then
                    result = internalResult
                End If
                watch.Stop()
                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = "{2}              Mode: {1}{2}              Id: {0}{2}              {3}".SpecialFormater(parameters("Id"), "DataManager.Mode".AppSettings(), Environment.NewLine, DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False))
                    message &= vbCrLf & String.Format("              Scalar={0}", result)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    If "DataAccessLayer.Debug.DetailsCall".AppSettings(Of Boolean) Then
                        Dim detailsCall As String = AssemblyHandler.GetFrameProcess(indexStack)
                        If detailsCall.IsNotEmpty Then
                            message &= vbCrLf & String.Format("{0}", detailsCall.ToString.Replace("<<I>>", "              "))
                        End If

                    End If
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try
            Return result
        End Function

        Public Shared Function QueryExecuteScalar(Of T)(statement As String, currentConnection As DbConnection, table As String) As T
            Dim result As T
            Dim command As DbCommand = currentConnection.CreateCommand()
            Dim watch As Stopwatch
            Dim internalResult As Object = Nothing

            command.CommandType = CommandType.Text
            command.CommandText = PreprocessStatement(statement, currentConnection.ToString)
            Try
                watch = New Stopwatch
                watch.Start()
                internalResult = command.ExecuteScalar
                If Not IsDBNull(internalResult) Then
                    result = internalResult
                End If
                watch.Stop()
                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              Scalar={0}", result)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try
            Return result
        End Function

        Public Shared Function QueryExecuteScalar(command As DbCommand, currentConnection As DbConnection, table As String) As Integer
            Dim result As Integer = 0
            Dim watch As Stopwatch
            Dim internalResult As Object = Nothing

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            Try
                watch = New Stopwatch
                watch.Start()
                internalResult = command.ExecuteScalar
                If Not IsDBNull(internalResult) Then
                    result = internalResult
                End If
                watch.Stop()
                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, table, "Query", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              Scalar={0}", result)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, table, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try
            Return result
        End Function

        Public Shared Function CommandExecute(command As DbCommand, currentConnection As DbConnection, table As String, commandKind As String, parameters As Dictionary(Of String, Object)) As Integer
            Dim result As Integer = 0
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            For attempts As Integer = 1 To 3
                command.Connection = currentConnection
                Try
                    watch = New Stopwatch
                    watch.Start()
                    result = command.ExecuteNonQuery()
                    watch.Stop()
                    If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                        Dim message As String = DataAccessLayer.MakeCommandSummary(command, table, commandKind, Nothing, Nothing, False)
                        If parameters.IsNotEmpty() Then
                            message = "{2}{0}{4}{2}{0}{1}{0}{2}{0}{3}".SpecialFormater("              ", "Parámetros:" + parameters.ToStringExtended(), Environment.NewLine, message, "Mode:" + "DataManager.Mode".AppSettings())
                        End If
                        message &= vbCrLf & String.Format("              Record Affected={0}", result)
                        message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)

                        If "DataAccessLayer.Debug.DetailsCall".AppSettings(Of Boolean) Then

                            Dim detailsCall As String = AssemblyHandler.MethodName(System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower())
                            If detailsCall.IsNotEmpty Then
                                message &= vbCrLf & String.Format("{0}", detailsCall.ToString.Replace("<<I>>", "              "))
                            End If

                        End If
                        LogHandler.TraceLog("DataAccessLayer", message)
                    End If
                    If Not HasOutputParameters(command) Then
                        command.Dispose()
                    End If
                    Exit For
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionString", currentConnection.ToString())
                    End With
                    Dim temporalException = ExceptionOracleProcess(exOracle, command, table, "Query", parameter)
                    ConnectionClosed(command, currentConnection)
                    Throw temporalException
                Catch ex As Exception
                    Dim temporalException As Exception
                    If (ex.Message.StartsWith("ORA-03135:") OrElse
                        ex.Message.StartsWith("ORA-03113:") OrElse
                        ex.Message.IndexOf("End-of-file on communication channel", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        ex.Message.IndexOf("fin de archivo en el canal de comunicación", StringComparison.CurrentCultureIgnoreCase) > -1 OrElse
                        ex.Message.IndexOf("TNS:packet writer failure", StringComparison.CurrentCultureIgnoreCase) > -1) AndAlso attempts < 3 Then
                        Dim magicMethod As MethodInfo = currentConnection.GetType.GetMethod("ClearAllPools")

                        If magicMethod.IsNotEmpty Then
                            magicMethod.Invoke(currentConnection, New Object() {})
                        End If

                        LogHandler.WarningLog("DataAccessLayer", String.Format("Retry due to disconnection for '{3}' command on table '{2}' ({0}). {1}", attempts, ex.Message, table, commandKind))
                        Thread.Sleep(500)
                        Try
                            currentConnection.Open()
                        Catch ex2 As Exception
                            temporalException = Exceptions.DataAccessException.Factory(ex, command, table, commandKind)
                            ConnectionClosed(command, currentConnection)
                            Throw temporalException
                        End Try
                    Else
                        temporalException = Exceptions.DataAccessException.Factory(ex, command, table, commandKind)
                        ConnectionClosed(command, currentConnection)
                        Throw temporalException
                        Exit For
                    End If

                End Try
            Next
            Return result
        End Function

        Public Shared Sub ConnectionClosed(Command As DbCommand, currentConnection As DbConnection)
            Command.Connection = Nothing
            If currentConnection.State = ConnectionState.Open Then
                currentConnection.Close()
            End If
        End Sub

        Public Shared Function OpenDbConnection(connectionName As String) As DbConnection
            Return OpenDbConnection(connectionName, connectionName)
        End Function

        ''' <summary>
        '''  
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CompanyIdSelect() As Integer
            Dim Result As Integer = 0
            If "BackOffice.IsMultiCompany".AppSettings(Of Boolean) Then
                If Not IsNothing(HttpContext.Current) Then
                    If Not IsNothing(HttpContext.Current.Session) Then
                        Result = HttpContext.Current.Session("CompanyId")
                    End If
                End If
            End If
            Return Result
        End Function

        Public Shared Function OpenDbConnection(connectionName As String, ByRef companyId As Integer) As DbConnection
            Return OpenDbConnection(connectionName, connectionName, companyId)
        End Function

        Public Shared Function OpenDbConnectionRaw(connectionStringRaw As String, providerNameRaw As String)
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim currentConnection As DbConnection = Nothing
            If connectionStringRaw.IsEmpty Then
                Throw New Exceptions.DataAccessException(String.Format("The connection string '{0}' not found.", connectionStringRaw), Nothing)
            End If
            Try
                currentProviderFactories = DbProviderFactories.GetFactory(providerNameRaw)
            Catch ex As Exception

                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex)
            End Try

            If currentProviderFactories.IsNotEmpty Then
                Try

                    currentConnection = currentProviderFactories.CreateConnection()
                    currentConnection.ConnectionString = connectionStringRaw
                    currentConnection.Open()
                Catch ex As System.Data.SqlClient.SqlException
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                    End If

                    Select Case ex.Number
                        Case 53
                            Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)

                        Case 18456
                            Throw New Exceptions.DataAccessException("The user-name/password is invalid", ex)

                        Case 4060
                            Throw New Exceptions.DataAccessException("The initial catalog/username is invalid", ex)

                        Case Else
                            If ex.ErrorCode = -2146232060 AndAlso ex.Message.StartsWith("A network-related or instance-specific error occurred") Then
                                Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)
                            Else
                                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                            End If
                    End Select
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionString", currentConnection)
                    End With
                    Throw ExceptionOracleProcess(exOracle, Nothing, "OpenDbConnectionRaw", "Open", parameter)
                Catch ex As Exception
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                    End If

                End Try
            End If
            Return currentConnection
        End Function

        Public Shared Function OpenDbConnection(connectionName As String, connectionExtendName As String, ByRef companyId As Integer) As DbConnection
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim currentConnection As DbConnection = Nothing
            Dim ConnectionString As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(connectionName)
            Dim mess As String = String.Format("{0}_{1}", connectionName, companyId)
            Dim companyIdValue As Integer = companyId

            If IsNothing(ConnectionString) Then
                ConnectionString = ConfigurationManager.ConnectionStrings(String.Format("Linked.{0}", connectionName))
            End If

            If ConnectionString.IsEmpty Then
                Throw New Exceptions.DataAccessException(String.Format("The connection string '{0}' not found.", connectionName), Nothing)
            End If
            Try
                currentProviderFactories = DbProviderFactories.GetFactory(ConnectionString.ProviderName)
            Catch ex As Exception

                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex)
            End Try

            If currentProviderFactories.IsNotEmpty Then
                Try

                    currentConnection = currentProviderFactories.CreateConnection()
                    currentConnection.ConnectionString = ConectionStringMultiCompany(ConnectionString.ConnectionString, connectionName, companyIdValue)
                    If PrivilegedAccessSecurity.IsProvider Then
                        currentConnection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(connectionName, ConnectionString.ConnectionString)
                    End If
                    currentConnection.Open()
                Catch ex As System.Data.SqlClient.SqlException
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                    End If

                    Select Case ex.Number
                        Case 53
                            Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)

                        Case 18456
                            Throw New Exceptions.DataAccessException("The username/password is invalid", ex)

                        Case 4060
                            Throw New Exceptions.DataAccessException("The initial catalog/username is invalid", ex)

                        Case Else
                            If ex.ErrorCode = -2146232060 AndAlso ex.Message.StartsWith("A network-related or instance-specific error occurred") Then
                                Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)
                            Else
                                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                            End If
                    End Select
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionName", connectionName)
                        .Add("connectionString", ConnectionString.ToString())
                        .Add("companyId", companyIdValue.ToString())
                    End With
                    Throw ExceptionOracleProcess(exOracle, Nothing, "OpenDbConnection", "Open", parameter)
                Catch ex As Exception
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database", ex)
                    End If

                End Try
            End If
            Return currentConnection
        End Function

        Public Shared Function OpenDbConnection(connectionName As String, connectionExtendName As String) As DbConnection
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim currentConnection As DbConnection = Nothing
            Dim ConnectionString As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(connectionName)

            If ConnectionString.IsEmpty Then
                Throw New Exceptions.DataAccessException(String.Format("The connection string '{0}' not found.", connectionName), Nothing)
            End If
            Try
                currentProviderFactories = DbProviderFactories.GetFactory(ConnectionString.ProviderName)
            Catch ex As Exception
                Throw New Exceptions.DataAccessException("The database .net provider may not be installed", ex)
            End Try

            If currentProviderFactories.IsNotEmpty Then
                Try

                    currentConnection = currentProviderFactories.CreateConnection()
                    Dim companyId As Integer = CompanyIdSelect()
                    currentConnection.ConnectionString = ConectionStringMultiCompany(ConnectionString.ConnectionString, connectionName, companyId)
                    If PrivilegedAccessSecurity.IsProvider Then
                        currentConnection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(connectionName, ConnectionString.ConnectionString)
                    End If
                    currentConnection.Open()
                Catch ex As System.Data.SqlClient.SqlException
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("Invalid username/password", ex)
                    End If

                    Select Case ex.Number
                        Case 53
                            Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)

                        Case 18456
                            Throw New Exceptions.DataAccessException("The username/password is invalid", ex)

                        Case 4060
                            Throw New Exceptions.DataAccessException("The initial catalog/username is invalid", ex)

                        Case Else
                            If ex.ErrorCode = -2146232060 AndAlso ex.Message.StartsWith("A network-related or instance-specific error occurred") Then
                                Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)
                            Else
                                Throw ex
                            End If
                    End Select
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionName", connectionName)
                        .Add("connectionString", ConnectionString.ToString())
                    End With
                    Throw ExceptionOracleProcess(exOracle, Nothing, "OpenDbConnection", "Open", parameter)
                Catch ex As Exception
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw ex
                    End If

                End Try
            End If
            Return currentConnection
        End Function

        Public Shared Sub CloseDbConnection(currentConnection As DbConnection)
            If currentConnection.IsNotEmpty Then
                If currentConnection.State = ConnectionState.Open Then
                    currentConnection.Close()
                    currentConnection.Dispose()
                End If
            End If
        End Sub

        Public Shared Function CreateQueryParameter(command As DbCommand, tableName As String, name As String, kind As DbType, size As Integer, value As Object, ByRef whereStatement As String) As DbParameter
            Dim parameter As DbParameter = CommandParameter(command, name, kind, size, True, value)
            If whereStatement.Trim.Equals("where", StringComparison.InvariantCultureIgnoreCase) Then
                whereStatement += String.Format(" {0}.{1} = @:{1}", tableName, name)
            Else
                whereStatement += String.Format(" AND {0}.{1} = @:{1}", tableName, name)
            End If
            Return parameter
        End Function

        Public Shared Function CreateQueryParameter(command As DbCommand, tableName As String, name As String, kind As DbType, size As Integer, value As Object, ByRef whereStatement As String, cancellationDateColumn As String) As DbParameter
            Dim parameter As DbParameter = CommandParameter(command, name, kind, size, True, value)
            whereStatement += String.Format(" AND {2}.{0} <= @:{0} AND ({2}.{1} IS NULL OR {2}.{1} > @:{0})", name, cancellationDateColumn, tableName)

            Return parameter
        End Function

        Public Shared Function CommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object) As DbParameter
            Return CommandParameter(command, name, kind, size, setValue, value, ParameterDirection.Input)
        End Function

        ''' <summary>
        ''' Custom for parameter type Oracle.DataAccess.Client.OracleDbType.TimeStamp
        ''' </summary>
        ''' <param name="parameterInstance"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateDateTimeOffsetParameter(parameterInstance As DbParameter) As DbParameter
            Dim parameterType As Type = parameterInstance.GetType
            Dim refCursorParameter As Object = Nothing
            Dim fail As Boolean = True

            If Not IsNothing(parameterType) Then
                Dim oracleDbType As Type = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType")
                If Not IsNothing(oracleDbType) Then
                    refCursorParameter = Activator.CreateInstance(parameterType, New Object() {"RC1", [Enum].Parse(oracleDbType, "TimeStamp")})
                    If Not IsNothing(refCursorParameter) Then
                        refCursorParameter.Direction = ParameterDirection.Output
                        fail = False
                    End If
                End If
            End If
            If fail Then
                Throw New Exceptions.InMotionGITException("It is trying to use a procedure with a xml type parameter, but you can not create xml ttype parameter due to provider, try changing it.")
            End If

            Return refCursorParameter
        End Function

        Public Shared Function CommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object, direction As ParameterDirection) As DbParameter
            Dim parameter As DbParameter = command.CreateParameter()
            Dim IsSQLServer As Boolean = command.GetType = GetType(System.Data.SqlClient.SqlCommand)
            Select Case kind
                Case DbType.Xml
                    If IsSQLServer Then
                        parameter.DbType = kind
                    Else
                        parameter = CreateXmlParameter(parameter)
                    End If
                Case DbType.DateTimeOffset
                    If IsSQLServer Then
                        parameter.DbType = kind
                    Else
                        parameter = CreateDateTimeOffsetParameter(parameter)
                    End If

                Case Else
                    parameter.DbType = kind
            End Select

            With parameter
                .Direction = direction
                .ParameterName = name
                .Size = size
                If setValue Then

                    If TypeName(value) = "XDocument" Then
                        .Value = DirectCast(value, XDocument).ToString
                    Else
                        .Value = value
                    End If
                Else
                    .Value = DBNull.Value
                End If
            End With
            command.Parameters.Add(parameter)
            Return parameter
        End Function

        Public Shared Function CreateCommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object, ByRef fieldList As String, ByRef valueList As String) As DbParameter
            Return CreateCommandParameter(command, name, kind, size, setValue, value, fieldList, valueList, False)
        End Function

        Public Shared Function CreateCommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object, ByRef fieldList As String, ByRef valueList As String, encrypted As Boolean) As DbParameter
            Dim parameter As DbParameter = CommandParameter(command, name, kind, size, setValue, value)

            If Not String.IsNullOrEmpty(fieldList) Then
                fieldList &= ","
            End If
            fieldList &= name
            If Not String.IsNullOrEmpty(valueList) Then
                valueList &= ","
            End If
            If encrypted Then
                valueList &= String.Format("INSUDB.EXTENCRYPTION.EncryptData(@:{0})", name)
            Else
                valueList &= String.Format("@:{0}", name)
            End If

            Return parameter
        End Function

        Public Shared Function UpdateCommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object, ByRef valueList As String) As DbParameter
            Return UpdateCommandParameter(command, name, kind, size, setValue, value, valueList, False)
        End Function

        Public Shared Function UpdateCommandParameter(command As DbCommand, name As String, kind As DbType, size As Integer, setValue As Boolean, value As Object, ByRef valueList As String, encrypted As Boolean) As DbParameter
            Dim parameter As DbParameter = CommandParameter(command, name, kind, size, setValue, value)

            If Not String.IsNullOrEmpty(valueList) Then
                valueList &= ","
            End If
            valueList &= String.Format("{0}=", name)

            If encrypted Then
                valueList &= String.Format("INSUDB.EXTENCRYPTION.EncryptData(@:{0})", name)
            Else
                valueList &= String.Format("@:{0}", name)
            End If

            Return parameter
        End Function

#End Region

#Region "Helpers"

        Private Shared Function ExecuteWithDataTable(command As DbCommand, currentConnection As DbConnection, schemaOnly As Boolean) As DataTable
            Dim reader As DbDataReader = ExecuteWithResultset(command, currentConnection)
            Dim result As DataTable = Nothing
            If schemaOnly Then
                result = reader.GetSchemaTable
            Else
                result = New DataTable
                result.Load(reader)
                If String.IsNullOrEmpty(result.TableName) Then
                    result.TableName = "Result"
                End If
            End If
            If reader.IsNotEmpty AndAlso
               Not reader.IsClosed Then
                reader.Close()
                reader = Nothing
            End If
            Return result
        End Function

        Private Shared Function ExecuteWithOutResultset(command As DbCommand, currentConnection As DbConnection) As Long
            Dim recordAffected As Long = 0
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            Try
                watch = New Stopwatch
                watch.Start()
                recordAffected = command.ExecuteNonQuery()
                watch.Stop()

                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, command.CommandText, "ExecuteWithOutResultset", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              Record Affected={0}", recordAffected)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, command.CommandText, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try
            Return recordAffected
        End Function

        Private Shared Function ExecuteWithResultset(command As DbCommand, currentConnection As DbConnection) As DbDataReader
            Dim reader As DbDataReader = Nothing
            Dim watch As Stopwatch

            command.CommandText = PreprocessStatement(command.CommandText, currentConnection.ToString)
            command.Connection = currentConnection
            If command.CommandType = CommandType.StoredProcedure AndAlso
               currentConnection.GetType.Name.ToUpper(CultureInfo.CurrentCulture).Contains("ORACLE") Then

                If IsMicrosoftOracleProvider(currentConnection.ToString) Then
                    command.Parameters.Add(CreateRefCursorParameter())
                Else
                    command.Parameters.Add(CreateRefCursorParameter(command.CreateParameter()))
                End If

            End If
            Try
                watch = New Stopwatch
                watch.Start()
                reader = command.ExecuteReader(CommandBehavior.SingleResult)
                watch.Stop()

                If "DataAccessLayer.Debug".AppSettings(Of Boolean) Then
                    Dim message As String = DataAccessLayer.MakeCommandSummary(command, command.CommandText, "Query", Nothing, Nothing, False)
                    message &= vbCrLf & String.Format("              HasRows={0}", reader.HasRows)
                    message &= vbCrLf & String.Format("              {0} ms", watch.ElapsedMilliseconds)
                    LogHandler.TraceLog("DataAccessLayer", message)
                End If
                command.Connection = Nothing
                command = Nothing
            Catch ex As Exception
                Dim temporalException = Exceptions.DataAccessException.Factory(ex, command, command.CommandText, "Query")
                ConnectionClosed(command, currentConnection)
                Throw temporalException
            End Try
            Return reader
        End Function

        ''' <summary>
        ''' Create a new parameter of RefCursor type
        ''' </summary>
        ''' <param name="parameterInstance"></param>
        ''' <returns>DbParameter with DbType RefCursor</returns>
        ''' <remarks>
        ''' Usage: command.Parameters.Add(CreateRefCursorParameter(dbProviderFactory.CreateParameter()))
        ''' </remarks>
        Private Shared Function CreateRefCursorParameter(ByVal parameterInstance As DbParameter) As DbParameter
            Dim parameterType As Type = parameterInstance.GetType
            Dim oracleDbType As Type = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType")
            Dim refCursorParameter As Object = Activator.CreateInstance(parameterType, New Object() {"RC1", [Enum].Parse(oracleDbType, "RefCursor")})
            refCursorParameter.Direction = ParameterDirection.Output

            Return refCursorParameter
        End Function

        Private Shared Function CreateRefCursorParameter() As DbParameter
            Dim refCursorParameter As Object = New System.Data.OracleClient.OracleParameter("RC1", OracleClient.OracleType.Cursor)
            refCursorParameter.Direction = ParameterDirection.Output

            Return refCursorParameter
        End Function

        Public Shared Function IsOracleProvider(connectionName As String) As Boolean
            Dim ProviderName As String = ConfigurationManager.ConnectionStrings(connectionName).ProviderName
            Return IsOracle(ProviderName)
        End Function

        Friend Shared Function IsMicrosoftOracleProvider(providerName As String) As Boolean
            Return (providerName.StartsWith("System.Data.OracleClient", StringComparison.CurrentCultureIgnoreCase))
        End Function

        Friend Shared Function IsOracle(providerName As String) As Boolean
            Return (providerName.StartsWith("Oracle.DataAccess.Client", StringComparison.CurrentCultureIgnoreCase) OrElse
                    providerName.StartsWith("Oracle.ManagedDataAccess.Client", StringComparison.CurrentCultureIgnoreCase) OrElse
                    providerName.StartsWith("System.Data.OracleClient", StringComparison.CurrentCultureIgnoreCase))
        End Function

        Public Shared Function PreprocessStatement(commandText As String, providerName As String) As String
            commandText = commandText.Replace(" (NOLOCK)  AND ", " (NOLOCK) WHERE ")
            commandText = commandText.Replace(" WHERE AND ", " WHERE ")
            If IsOracle(providerName) Then
                commandText = commandText.Replace("GETDATE()", "SYSDATE")
                commandText = commandText.Replace("@:", ":")
                commandText = commandText.Replace(" (NOLOCK)", "")
                commandText = commandText.Replace(" ISNULL(", " NVL(")
            Else
                commandText = commandText.Replace("SYSDATE", "GETDATE()")
                commandText = commandText.Replace("@:", "@")
                commandText = commandText.Replace(" NVL(", " ISNULL(")
                'statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
                'statement = statement.Replace(" ISNULL(", " ISNULL(")
            End If
            commandText = commandText.Replace("{OWNER}", "")

            Return commandText
        End Function

        Friend Shared Function MakeCommandSummary(command As DbCommand,
                                                  table As String,
                                                  commandKind As String,
                                                  ByRef commandText As String,
                                                  ByRef parameters As Dictionary(Of String, String),
                                                  addTagInnerException As Boolean) As String
            Dim extra As String = String.Empty
            Dim parameterName As String = String.Empty
            Dim parameterValue As String = String.Empty
            If command.IsNotEmpty Then
                If command.CommandType = CommandType.StoredProcedure Then
                    commandText = String.Format("Procedure {0}", command.CommandText)
                Else
                    commandText = command.CommandText
                End If
                extra = commandText
                If command.Parameters.IsNotEmpty Then
                    parameters = New Dictionary(Of String, String)
                    For Each item As DbParameter In command.Parameters
                        parameterName &= String.Format(":{0},", item.ParameterName)
                        If IsDBNull(item.Value) Then
                            parameters.Add(item.ParameterName, "Null")
                            extra &= vbCrLf
                            If Not addTagInnerException Then
                                extra &= "             "
                            End If
                            extra &= String.Format(" {0}=Null", item.ParameterName)
                            parameterValue &= "Null,"
                        Else
                            parameters.Add(item.ParameterName, String.Format("{0}", item.Value))
                            extra &= vbCrLf
                            If Not addTagInnerException Then
                                extra &= "             "
                            End If
                            extra &= String.Format(" {0} ({2}) ={1}", item.ParameterName, item.Value, item.Direction)
                            If item.DbType = DbType.StringFixedLength Then
                                parameterValue &= String.Format("'{0}',", item.Value)
                            ElseIf item.DbType = DbType.Date Then
                                parameterValue &= String.Format("TO_DATE('{0}', 'MM/DD/YYYY HH24:MI:SS'),", DirectCast(item.Value, Date).ToString("MM/dd/yyyy HH:mm:ss"))
                            Else
                                parameterValue &= String.Format("{0},", item.Value)
                            End If

                        End If
                    Next
                End If
                If parameterValue.IsNotEmpty And parameterValue.EndsWith(",") Then
                    parameterValue = parameterValue.Substring(0, parameterValue.Length - 1)
                End If

                'If addTagInnerException Then
                '    extra &= " @@InnerException@@"
                'End If
            End If

            Return extra
        End Function

        Public Shared Function DateValueWithFormat(ByVal connectionName As String, value As Date) As String
            Dim dateFormat As String = "{0}.DateFormat".SpecialFormater(connectionName).AppSettings()
            Dim result As String = value.ToString

            If dateFormat.IsNotEmpty Then
                Return value.ToString(dateFormat, New CultureInfo("en-US"))
            End If

            Return result
        End Function

#End Region

#Region "IDisposable Support"

        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Release()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

#Region "CIAMulti"

        Public Shared Function ConectionStringMultiCompany(stringConection As String, stringConectionName As String, ByRef companyId As Integer) As String

            Dim _Resul As String = stringConection
            Dim _User As String = String.Empty
            Dim _PassWord As String = String.Empty

            If "BackOffice.IsMultiCompany".AppSettings(Of Boolean) Then

                If "Core.Mapper".AppSettings().IsNotEmpty() Then

                    'En caso de llegar el id de la compañía vació, pero a nivel de configuración este establecido la compañía por default,
                    'entonces se establece el id compañía indicado.
                    If companyId = 0 Then
                        companyId = "BackOffice.CompanyDefault".AppSettings(Of Integer)
                    End If

                    If companyId > 0 Then
                        Dim conectionVecto As String() = "Core.Mapper".AppSettings().Split(",")
                        Dim exist As Boolean
                        If conectionVecto.Length = 0 Then
                            exist = String.Equals("Core.Mapper".AppSettings(), stringConectionName)
                        Else
                            exist = conectionVecto.Contains(stringConectionName)
                            If Not exist AndAlso
                               Not stringConectionName.StartsWith("Linked.", StringComparison.CurrentCultureIgnoreCase) Then
                                exist = conectionVecto.Contains("Linked.{0}".SpecialFormater(stringConectionName))
                            End If
                        End If
                        Dim arrResult As Object() = MultiCompany.GetUserInfo(companyId)
                        If Not IsNothing(arrResult) Then
                            _User = arrResult(1).ToString
                            _PassWord = arrResult(2).ToString
                        End If
                        If exist Then
                            If _User <> String.Empty AndAlso _PassWord <> String.Empty Then
                                If Not _Resul.EndsWith(";") Then
                                    _Resul = ";{0};User ID={1};Password={2}".SpecialFormater(_Resul, BackOffice.CryptSupport.HexDecryptString(_User), BackOffice.CryptSupport.HexDecryptString(_PassWord))
                                Else
                                    _Resul = "{0};User ID={1};Password={2}".SpecialFormater(_Resul, BackOffice.CryptSupport.HexDecryptString(_User), BackOffice.CryptSupport.HexDecryptString(_PassWord))
                                End If
                            End If
                        End If
                    End If
                End If
            Else

            End If
            Return _Resul
        End Function

#End Region

#Region "DataBase Provider Methods"

        'TODO: validar este caso con el provider de microsoft.
        Public Shared Function DbProviderParameterPrefix(parameterName As String, providerName As String) As String
            Dim result As String = String.Empty
            Select Case providerName.ToLower
                Case "oracle.dataaccess.client", "oracle.manageddataaccess.client"
                    result = String.Format(":{0}", parameterName)
                Case Else
                    result = String.Format("@{0}", parameterName)
            End Select

            Return result
        End Function

        Public Function DbProviderParameterPrefix(parameterName As String) As String
            Dim result As String = String.Empty

            Select Case ProviderName.ToLower
                Case "oracle.dataaccess.client", "oracle.manageddataAccess.client"
                    result = String.Format(":{0}", parameterName)
                Case Else
                    result = String.Format("@{0}", parameterName)
            End Select

            Return result
        End Function

        Public Shared Function GetDataBaseProvider(repositoryName As String) As String
            Dim connectionString As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(repositoryName)
            Dim result As String = "SQL"

            If Not IsNothing(connectionString) Then
                Select Case connectionString.ProviderName.ToLower
                    Case "system.data.sqlclient"
                        result = "SQL"

                    Case "oracle.dataaccess.client", "oracle.manageddataaccess.client", "system.data.oracleclient"
                        result = "Oracle"

                    Case Else
                End Select
            End If

            Return result
        End Function

#End Region

#Region "Tools"

        Public Shared Function TestConnectionLive(ByVal DataSource As String,
                                                  ByVal ServicesName As String,
                                                  ByVal catalog As String,
                                                  ByVal user As String,
                                                  ByVal password As String,
                                                  ByVal ServerType As Enumerations.EnumSourceType) As Services.Contracts.ConnectionString
            Dim result As New Services.Contracts.ConnectionString
            Dim connectionString As String = String.Empty
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim provider As String = String.Empty
            Select Case ServerType
                Case Enumerations.EnumSourceType.SqlServer
                    provider = "System.Data.SqlClient"
                    connectionString += String.Format("Data Source={0};user id={1};password={2}", DataSource, user, password)
                    connectionString += String.Format(";Initial Catalog={0}", catalog)

                Case Enumerations.EnumSourceType.Oracle
                    provider = "Oracle.DataAccess.Client"
                    connectionString += String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));", DataSource, ServicesName)
                    connectionString += String.Format("User ID={0};Password={1}", user, password)

            End Select
            Try
                result.ConnectionString = connectionString
                result.ProviderName = provider
                result.DatabaseName = catalog
                result.Owners = user
                result.Password = password
                result.ServiceName = ServicesName
                result.UserName = user
                currentProviderFactories = DbProviderFactories.GetFactory(provider)
            Catch ex As Exception
                result = Nothing
                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex)
            End Try

            Dim db As DbConnection = OpenDbConnectionRaw(connectionString, result.ProviderName)
            If db.IsNotEmpty Then
                db.Close()
            Else
                result = Nothing
            End If
            Return result
        End Function

        Public Shared Function TestConnection(ByVal DataSource As String, ServicesName As String, ByVal catalog As String, ByVal user As String, ByVal password As String, ByVal ServerType As Enumerations.EnumSourceType) As Services.Contracts.ConnectionString
            Dim result As New Services.Contracts.ConnectionString
            Dim connectionString As String = String.Empty
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim provider As String = String.Empty
            Select Case ServerType
                Case Enumerations.EnumSourceType.SqlServer
                    provider = "System.Data.SqlClient"
                    connectionString += String.Format("Data Source={0};user id={1};password={2}", DataSource, user, password)
                    connectionString += String.Format(";Initial Catalog={0}", catalog)

                Case Enumerations.EnumSourceType.Oracle
                    provider = "Oracle.DataAccess.Client"
                    connectionString += String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));", DataSource, ServicesName)
                    connectionString += String.Format("User ID={0};Password={1}", user, password)

            End Select
            Try
                result.ConnectionString = connectionString
                result.ProviderName = provider
                result.DatabaseName = catalog
                result.Owners = user
                result.Password = password
                result.ServiceName = ServicesName
                result.UserName = user
                currentProviderFactories = DbProviderFactories.GetFactory(provider)
            Catch ex As Exception
                result = Nothing
                Throw New Exceptions.DataAccessException("An error has occurred while attempting to open the connection to the database. The database .net provider may not be installed", ex)
            End Try

            Dim db As DbConnection = OpenDbConnection(connectionString)
            If db.IsNotEmpty Then
                db.Close()
            Else
                result = Nothing
            End If
            Return result
        End Function

        Public Shared Function OpenLocalDbConnection(connectionString As String, ProviderName As String) As DbConnection
            Dim currentProviderFactories As DbProviderFactory = Nothing
            Dim currentConnection As DbConnection = Nothing

            Try
                currentProviderFactories = DbProviderFactories.GetFactory(ProviderName)
            Catch ex As Exception
                Throw New Exceptions.DataAccessException("The database .net provider may not be installed", ex)
            End Try

            If currentProviderFactories.IsNotEmpty Then
                Try

                    currentConnection = currentProviderFactories.CreateConnection()
                    currentConnection.ConnectionString = connectionString
                    currentConnection.Open()
                Catch ex As System.Data.SqlClient.SqlException
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw New Exceptions.DataAccessException("Invalid username/password", ex)
                    End If

                    Select Case ex.Number
                        Case 53
                            Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)

                        Case 18456
                            Throw New Exceptions.DataAccessException("The username/password is invalid", ex)

                        Case 4060
                            Throw New Exceptions.DataAccessException("The initial catalog/username is invalid", ex)

                        Case Else
                            If ex.ErrorCode = -2146232060 AndAlso ex.Message.StartsWith("A network-related or instance-specific error occurred") Then
                                Throw New Exceptions.DataAccessException("The server was not found or was not accessible", ex)
                            Else
                                Throw ex
                            End If
                    End Select
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionString", connectionString.ToString())
                    End With
                    Throw ExceptionOracleProcess(exOracle, Nothing, "OpenLocalDbConnection", "Open", parameter)
                Catch ex As Exception
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exceptions.DataAccessException("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exceptions.DataAccessException("The username/password is invalid", ex)
                    Else
                        Throw ex
                    End If

                End Try
            End If
            Return currentConnection
        End Function

        Public Function ExecuteQuery(ByVal query As String, connectionString As String, providerName As String, connectionStringName As String, companyId As Integer) As DataTable
            If IsNothing(_dbConnection) Then
                _dbConnection = OpenLocalDbConnection(connectionString, providerName)
            End If
            Return ExecuteQuery(query, connectionStringName, companyId)
        End Function

        Private Shared Function CreateXmlParameter(ByVal parameterInstance As DbParameter) As DbParameter
            'Private Shared Function CreateXmlParameter(command As DbCommand, name As String, kind As System.Data.DbType, size As Integer, setValue As Boolean, value As Object, direction As System.Data.ParameterDirection) As DbParameter
            'Dim refCursorParameter As Object = New System.Data.OracleClient.OracleParameter(name, Oracle.DataAccess.Client.OracleDbType.XmlType)
            'refCursorParameter.Direction = direction

            'Return refCursorParameter

            Dim parameterType As Type = parameterInstance.GetType
            Dim refCursorParameter As Object = Nothing
            Dim fail As Boolean = True

            If Not IsNothing(parameterType) Then
                Dim oracleDbType As Type = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType")

                If Not IsNothing(oracleDbType) Then
                    refCursorParameter = Activator.CreateInstance(parameterType, New Object() {"RC1", [Enum].Parse(oracleDbType, "XmlType")})

                    If Not IsNothing(refCursorParameter) Then
                        refCursorParameter.Direction = ParameterDirection.Output
                        fail = False
                    End If
                End If
            End If

            If fail Then
                Throw New Exceptions.InMotionGITException("It is trying to use a procedure with a xml type parameter, but you can not create xml ttype parameter due to provider, try changing it.")
            End If

            Return refCursorParameter
        End Function

        Public Shared Function ValueDateFormat(ByVal repositoryName As String, value As Date) As String
            Dim result As String = value.ToString(DateFormat(repositoryName), New CultureInfo("en-US"))
            'result = result.Replace("/mm/", "/")
            Return result
        End Function

        Public Shared Function DateFormat(ByVal repositoryName As String) As String
            Return "{0}.DateFormat".SpecialFormater(repositoryName).AppSettings
        End Function

        Public Shared Function RealConnectionStringName(repositoryName As String) As String
            Dim ConnectionSetting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(repositoryName)

            If IsNothing(ConnectionSetting) Then
                ConnectionSetting = ConfigurationManager.ConnectionStrings("Linked.{0}".SpecialFormater(repositoryName))
            End If

            If IsNothing(ConnectionSetting) Then
                ConnectionSetting = ConfigurationManager.ConnectionStrings("BackOfficeConnectionString")
            End If

            If IsNothing(ConnectionSetting) Then
                Throw New Exceptions.InMotionGITException("the connection setting for database not found")
            End If

            Return ConnectionSetting.Name
        End Function

        Public Shared Function GetConnectionString(repositoryName As String) As ConnectionStringSettings
            Dim ConnectionSetting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(repositoryName)

            If IsNothing(ConnectionSetting) Then
                ConnectionSetting = ConfigurationManager.ConnectionStrings(String.Format("Linked.{0}", repositoryName))
            End If

            If IsNothing(ConnectionSetting) Then
                ConnectionSetting = ConfigurationManager.ConnectionStrings("BackOffice")

                If IsNothing(ConnectionSetting) And Debugger.IsAttached Then
                    ConnectionSetting = ConfigurationManager.OpenMachineConfiguration.ConnectionStrings.ConnectionStrings("BackOffice")
                End If
            End If

            If IsNothing(ConnectionSetting) Then
                Throw New Exception("the connection setting for database not found")
            End If

            Return ConnectionSetting
        End Function

        Public Shared Function TestConnection(ByVal DataSource As String, ByVal catalog As String, ByVal user As String, ByVal password As String, ByVal provider As String) As Boolean
            Dim result As Boolean = True
            Dim connectionString As String = String.Format("Data Source={0};user id={1};password={2}", DataSource, user, password)
            If provider = "System.Data.SqlClient" Then
                connectionString += String.Format(";Initial Catalog={0}", catalog)
            End If
            result = (New DataAccessLayer).OpenConnectionString(connectionString, provider)

            Return result
        End Function

        Public Shared Function TestConnection(ByVal connectionStringName As String) As Boolean
            Dim ConnectionSetting As ConnectionStringSettings = GetConnectionString(connectionStringName)
            Dim result As Boolean = False

            If Not IsNothing(ConnectionSetting) Then
                result = (New DataAccessLayer).OpenConnectionString(ConnectionSetting.ConnectionString,
                                                         ConnectionSetting.ProviderName)
            End If

            Return result
        End Function

        Private Function OpenConnectionString(ByVal connectionString As String, ByVal provider As String) As Boolean
            Dim result As Boolean = True
            ProviderName = provider
            Try
                _dbProviderFactory = DbProviderFactories.GetFactory(provider)
            Catch ex As Exception
                Throw New Exception("the database .net provider may not be installed", ex)
                result = False
            End Try

            If Not IsNothing(_dbProviderFactory) Then
                Try
                    _dbConnection = _dbProviderFactory.CreateConnection()
                    _dbConnection.ConnectionString = connectionString
                    _dbConnection.Open()

                    'TODO: Se deben tipificar los error bajo GIT Exception usando un codificación para alinear los errores
                Catch ex As System.Data.SqlClient.SqlException
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exception("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exception("the username/password is invalid", ex)
                    End If

                    Select Case ex.Number
                        Case 53
                            Throw New Exception("the server was not found or was not accessible", ex)

                        Case 18456
                            Throw New Exception("the username/password is invalid", ex)

                        Case 4060
                            Throw New Exception("the initial catalog/username is invalid", ex)

                        Case Else
                            If ex.ErrorCode = -2146232060 AndAlso ex.Message.StartsWith("A network-related or instance-specific error occurred") Then
                                Throw New Exception("the server was not found or was not accessible", ex)
                            Else
                                Throw New Exception("we have a technical problem", ex)
                            End If
                    End Select
                    result = False
                Catch exOracle As OracleException
                    Dim parameter = New Dictionary(Of String, Object)
                    With parameter
                        .Add("connectionString", connectionString.ToString())
                    End With
                    Throw ExceptionOracleProcess(exOracle, Nothing, "OpenConnectionString", "Open")
                Catch ex As Exception
                    If ex.Message.StartsWith("ORA-12154:") Then
                        Throw New Exception("Oracle TNS could not resolve the connect identifier specified", ex)

                    ElseIf ex.Message.StartsWith("ORA-01017:") Or ex.Message.StartsWith("ORA-1017:") Then
                        Throw New Exception("the username/password is invalid", ex)
                    Else
                        Throw New Exception("we have a technical problem", ex)
                    End If

                    result = False
                End Try
            End If

            Return result
        End Function

        Private Shared Function HasOutputParameters(command As DbCommand) As Boolean
            Dim result As Boolean = False

            If command.IsNotEmpty AndAlso command.Parameters.IsNotEmpty Then
                For Each item As DbParameter In command.Parameters
                    If item.Direction <> ParameterDirection.Input Then
                        result = True
                        Exit For
                    End If
                Next
            End If

            Return result
        End Function

#End Region

        Public Sub New()

        End Sub

    End Class

End Namespace