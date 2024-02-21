Imports System.Configuration
Imports System.Data.Common
Imports System.Reflection

Namespace Helpers

    Public Class Data
        Implements IDisposable

#Region "Private fields, to hold the state of the entity"

        Private _dbProviderFactory As DbProviderFactory
        Private _dbConnection As DbConnection

        Private _owner As String
        Private _sysdate As String

#End Region

#Region "Public properties"

        Public Property ProviderName As String
        Public Property DateFormat As String

#End Region

#Region " IDisposable Support "

        Private disposedValue As Boolean

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    Release()
                End If
                If Not IsNothing(_dbConnection) Then
                    _dbConnection.Close()
                    _dbConnection = Nothing
                End If
                _dbProviderFactory = Nothing
                ' TODO: free your own state (unmanaged objects).
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

#Region "Constructors Methods"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(connectionStringName As String)
            OpenConnection(connectionStringName)
        End Sub

        Public Sub New(connectionString As String, provider As String, owner As String)
            _owner = owner
            OpenConnectionString(connectionString, provider)
        End Sub

#End Region

#Region "Connection Methods"

        Public Shared Function RealConnectionStringName(repositoryName As String) As String
            Dim ConnectionSetting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(repositoryName)

            If IsNothing(ConnectionSetting) Then
                ConnectionSetting = ConfigurationManager.ConnectionStrings(String.Format("Linked.{0}", repositoryName))
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

        Private Function OpenConnection(ByVal connectionStringName As String) As Boolean
            Dim ConnectionSetting As ConnectionStringSettings = GetConnectionString(connectionStringName)
            Dim result As Boolean = OpenConnectionString(ConnectionSetting.ConnectionString,
                                        ConnectionSetting.ProviderName)

            If result Then
                _owner = "{0}.Owner".SpecialFormater(ConnectionSetting.Name).AppSettings
                DateFormat = "{0}.DateFormat".SpecialFormater(ConnectionSetting.Name).AppSettings
                If DateFormat.IsEmpty Then
                    DateFormat = "MM/dd/yyyy"
                End If
                If ConnectionSetting.ProviderName.ToLower = "system.data.sqlclient" Then
                    _sysdate = "GETDATE()"
                Else
                    _sysdate = "SYSDATE"
                End If
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
                    Else
                        Throw New Exception("invalid username/password", ex)
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

        Public Shared Function TestConnection(ByVal DataSource As String, ByVal catalog As String, ByVal user As String, ByVal password As String, ByVal provider As String) As Boolean
            Dim result As Boolean = True
            Dim connectionString As String = String.Format("Data Source={0};user id={1};password={2}", DataSource, user, password)
            If provider = "System.Data.SqlClient" Then
                connectionString += String.Format(";Initial Catalog={0}", catalog)
            End If
            Using DataAccess As New Data()
                result = DataAccess.OpenConnectionString(connectionString, provider)
                DataAccess.Release()
            End Using
            Return result
        End Function

        Public Shared Function TestConnection(ByVal connectionStringName As String) As Boolean
            Dim ConnectionSetting As ConnectionStringSettings = GetConnectionString(connectionStringName)
            Dim result As Boolean = False

            If Not IsNothing(ConnectionSetting) Then
                Using DataAccess As New Data()
                    result = DataAccess.OpenConnectionString(ConnectionSetting.ConnectionString,
                                                             ConnectionSetting.ProviderName)
                    DataAccess.Release()
                End Using
            End If

            Return result
        End Function

        Public Sub Release()
            If Not IsNothing(_dbConnection) Then
                _dbConnection.Close()
                _dbConnection = Nothing
            End If
            _dbProviderFactory = Nothing
        End Sub

        ''' <summary>
        ''' Create a new parameter of RefCursor type
        ''' </summary>
        ''' <param name="parameterInstance"></param>
        ''' <returns>DbParameter with DbType RefCursor</returns>
        ''' <remarks>
        ''' Usage: command.Parameters.Add(CreateRefCursorParameter(dbProviderFactory.CreateParameter()))
        ''' </remarks>
        Private Shared Function CreateRefCursorParameter(parameterInstance As DbParameter) As DbParameter
            Dim parameterType As Type = parameterInstance.GetType
            Dim oracleDbType As Type = parameterType.Assembly.GetType("Oracle.DataAccess.Client.OracleDbType")
            Dim refCursorParameter As Object = Activator.CreateInstance(parameterType, New Object() {"RC1", [Enum].Parse(oracleDbType, "RefCursor")})
            refCursorParameter.Direction = ParameterDirection.Output

            Return refCursorParameter
        End Function

#End Region

#Region "Execution Methods"

        Friend Function QueryExecute(query As String, dbConnection As System.Data.Common.DbConnection) As DataTable
            _dbConnection = dbConnection

            If IsNothing(_dbConnection) Then
                Return Nothing
            Else
                Return QueryExecute(query)
            End If
        End Function

        Public Function QueryExecute(query As String, connectionStringName As String) As DataTable
            If _dbConnection.IsEmpty OrElse
               _dbConnection.State <> ConnectionState.Open Then
                OpenConnection(connectionStringName)
            End If
            If IsNothing(_dbConnection) Then
                Return Nothing
            Else
                Return QueryExecute(query)
            End If
        End Function

        Public Function QueryExecute(query As String) As DataTable
            Dim dbcmd As DbCommand = Nothing
            Dim vloDataAdapter As DbDataAdapter = Nothing
            Dim result As DataTable = Nothing

            dbcmd = _dbProviderFactory.CreateCommand
            dbcmd.Connection = _dbConnection
            dbcmd.CommandText = PreprocessStatement(query)

            vloDataAdapter = _dbProviderFactory.CreateDataAdapter()

            result = New DataTable
            vloDataAdapter.SelectCommand = dbcmd

            vloDataAdapter.Fill(result)

            Return result
        End Function

        Public Function QueryScalar(Of T)(ByVal query As String, ByVal repositoryName As String) As T
            Dim dbcmd As DbCommand = Nothing
            Dim result As T

            If _dbConnection.IsEmpty OrElse
                _dbConnection.State <> ConnectionState.Open Then
                OpenConnection(repositoryName)
            End If

            If Not IsNothing(_dbConnection) Then

                dbcmd = _dbProviderFactory.CreateCommand
                dbcmd.Connection = _dbConnection
                dbcmd.CommandText = PreprocessStatement(query)

                result = dbcmd.ExecuteScalar()
            End If

            Return result
        End Function

        ''' <summary>
        '''Execute the queries
        ''' </summary>
        ''' <param name="command">Query to execute</param>
        Public Function CommandExecute(ByVal command As String) As Long
            Return CommandExecute(command, String.Empty)
        End Function

        ''' <summary>
        '''Execute the queries
        ''' </summary>
        ''' <param name="command">Query to execute</param>
        ''' <param name="connectionStringName">Name of the input of ConnectionStrings</param>
        Public Function CommandExecute(ByVal command As String, ByVal connectionStringName As String) As Long
            Dim dbcmd As DbCommand = Nothing
            Dim rowAffected As Long = 0

            If _dbConnection.IsEmpty OrElse
                _dbConnection.State <> ConnectionState.Open Then
                OpenConnection(connectionStringName)
            End If

            If Not IsNothing(_dbConnection) Then
                dbcmd = _dbProviderFactory.CreateCommand

                With dbcmd
                    .Connection = _dbConnection
                    .CommandText = PreprocessStatement(command)
                    rowAffected = .ExecuteNonQuery()
                End With
            End If
            Return rowAffected
        End Function

        Public Function QueryExecuteWithMap(Of T As New, Y As New)(ByVal query As String, ByVal connectionStringName As String) As Y
            Return MapDataToBusinessEntityCollection(Of T, Y)(QueryExecute(query, connectionStringName).CreateDataReader)
        End Function

        Public Function QueryExecute(statement As String,
                                  parameters As Dictionary(Of String, Object)) As DataTable
            Dim result As DataTable

            If parameters.IsNotEmpty AndAlso parameters.Count > 0 Then
                For Each item As KeyValuePair(Of String, Object) In parameters
                    If String.Equals(item.Key, "[RECORDEFFECTIVEDATE]", StringComparison.CurrentCultureIgnoreCase) OrElse
                        item.Key.EndsWith(":D}") Then
                        statement = statement.Replace(item.Key, Date.Parse(item.Value).ToString(DateFormat))
                    Else
                        statement = statement.Replace(item.Key, item.Value)
                    End If
                Next
            End If
            result = QueryExecute(PreprocessStatement(statement))
            Return result
        End Function

#End Region

#Region "Mapping Data Methods"

        Public Shared Function MapDataToBusinessEntityCollection(Of T As New)(dr As DataTable) As List(Of T)
            Dim businessEntityType As Type = GetType(T)
            Dim entitys As New List(Of T)()
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
                            info.SetValue(newObject, row(column.ColumnName), Nothing)
                        End If
                    End If
                Next

                entitys.Add(newObject)
            Next
            Return entitys
        End Function

        Public Shared Function MapDataToBusinessEntityCollection(Of T As New)(dr As IDataReader) As List(Of T)
            Dim businessEntityType As Type = GetType(T)
            Dim entitys As New List(Of T)()
            Dim hashtable As New Hashtable()
            Dim properties As PropertyInfo() = businessEntityType.GetProperties()
            Dim info As PropertyInfo
            Dim newObject As T

            For Each info In properties
                hashtable(info.Name.ToUpper()) = info
            Next

            Do While dr.Read()
                newObject = New T

                For index As Integer = 0 To dr.FieldCount - 1
                    If Not dr.IsDBNull(index) Then
                        info = DirectCast(hashtable(dr.GetName(index).ToUpper()), PropertyInfo)
                        If (info IsNot Nothing) AndAlso info.CanWrite Then
                            info.SetValue(newObject, dr.GetValue(index), Nothing)
                        End If
                    End If
                Next
                entitys.Add(newObject)
            Loop
            dr.Close()
            Return entitys
        End Function

        Public Shared Function MapDataToBusinessEntityCollection(Of T As New, Y As New)(dr As IDataReader) As Y
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

            Do While dr.Read()
                newObject = New T

                For index As Integer = 0 To dr.FieldCount - 1
                    info = DirectCast(hashtable(dr.GetName(index).ToUpper()), PropertyInfo)
                    If (info IsNot Nothing) AndAlso info.CanWrite AndAlso Not dr.IsDBNull(index) Then

                        If dr.GetValue(index).GetType.ToString = "System.Decimal" AndAlso
                           info.PropertyType.ToString = "System.Int32" Then
                            info.SetValue(newObject, Convert.ToInt32(dr.GetValue(index)), Nothing)
                        ElseIf dr.GetValue(index).GetType.ToString = "System.Int32" AndAlso
                           info.PropertyType.ToString = "System.Decimal" Then
                            info.SetValue(newObject, Convert.ToDecimal(dr.GetValue(index)), Nothing)
                        Else
                            info.SetValue(newObject, dr.GetValue(index), Nothing)
                        End If

                    End If
                Next
                ientitys.Add(newObject)
            Loop
            dr.Close()
            Return entitys
        End Function

#End Region

#Region "Replace Data Methods"

        Private Function PreprocessStatement(statement As String) As String
            If ProviderName.ToLower = "system.data.sqlclient" Then
                statement = statement.Replace("SYSDATE", "GETDATE()")
                statement = statement.Replace("@:", "@")
                'statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
                'statement = statement.Replace(" ISNULL(", " ISNULL(")
            Else
                'statement = statement.Replace("SYSDATE", "SYSDATE")
                statement = statement.Replace("@:", ":")
                statement = statement.Replace(" (NOLOCK)", "")
                statement = statement.Replace(" ISNULL(", " NVL(")
            End If
            statement = statement.Replace("{OWNER}", _owner)
            Return statement
        End Function

        'TODO: por eliminar
        Public Shared Function PreprocessStatement(statement As String, providerName As String) As String
            If providerName.ToLower = "system.data.sqlclient" Then
                statement = statement.Replace("SYSDATE", "GETDATE()")
                statement = statement.Replace("@:", "@")
                'statement = statement.Replace(" (NOLOCK)", " (NOLOCK)")
                'statement = statement.Replace(" ISNULL(", " ISNULL(")
            Else
                'statement = statement.Replace("SYSDATE", "SYSDATE")
                statement = statement.Replace("@:", ":")
                statement = statement.Replace(" (NOLOCK)", "")
                statement = statement.Replace(" ISNULL(", " NVL(")
            End If
            Return statement
        End Function

        Public Function ValueDateFormat(repositoryName As String, value As Date) As String
            Return value.ToString(Me.DateFormat(repositoryName))
        End Function

        'Public Function DateFormat(ByVal repositoryName As String) As String
        '    Return ConfigurationManager.AppSettings(String.Format("{0}.DateFormat", repositoryName))
        'End Function

#End Region

#Region "DataBase Provider Methods"

        Public Function DbProviderParameterPrefix(parameterName As String) As String
            Dim result As String = String.Empty

            Select Case ProviderName.ToLower
                Case "oracle.dataaccess.client"
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

                    Case "oracle.dataaccess.client", "system.data.oracleclient"
                        result = "Oracle"

                    Case Else
                End Select
            End If

            Return result
        End Function

#End Region

    End Class

End Namespace