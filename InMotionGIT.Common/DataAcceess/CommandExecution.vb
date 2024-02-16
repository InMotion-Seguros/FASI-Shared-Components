Imports System.Data.SqlClient

Namespace DataAccess

    Friend Class CommandExecution
        Implements Interfaces.ICommandExecution

        Private _commandText As String
        Private _commandType As CommandType
        Private _sqlConnection As SqlConnection

        Private connectionName As String
        Private isLive As Boolean
        Private parameters As List(Of SqlParameter)

        Private useDefaultConnection As Boolean

        Friend WriteOnly Property CommandText As String
            Set(ByVal value As String)
                _commandText = value
            End Set
        End Property

        Friend WriteOnly Property CommandType As CommandType
            Set(ByVal value As CommandType)
                _commandType = value
            End Set
        End Property

        Friend WriteOnly Property SqlConnection As SqlConnection
            Set(ByVal value As SqlConnection)
                _sqlConnection = value
            End Set
        End Property

        Friend Sub New()
            Me.parameters = New List(Of SqlParameter)
            Me.useDefaultConnection = True
        End Sub

        Friend Sub New(ByVal conn As SqlConnection)
            Me.parameters = New List(Of SqlParameter)
            Me.SqlConnection = conn
            Me.isLive = True
        End Sub

        Friend Sub New(ByVal connName As String)
            Me.parameters = New List(Of SqlParameter)
            Me.useDefaultConnection = String.IsNullOrEmpty(connName)
            Me.connectionName = connName
        End Sub

        Public Function AsDataTable() As DataTable Implements Interfaces.ICommandExecution.AsDataTable
            Dim result As New DataTable
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Dim adp As New SqlDataAdapter(comm)
                                    adp.Fill(result)
                                End Sub)
            Return result
        End Function

        Public Function AsHashTable() As Hashtable Implements Interfaces.ICommandExecution.AsHashTable
            Dim result As Hashtable = Nothing
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using reader As IDataReader = comm.ExecuteReader
                                        If reader.Read Then
                                            result = New Hashtable(reader.FieldCount)
                                            Dim col As Integer
                                            For col = 0 To reader.FieldCount - 1
                                                Dim colName As String = reader.GetName(col)
                                                Dim value As Object = reader.GetValue(col)
                                                If (value Is DBNull.Value) Then
                                                    value = Nothing
                                                End If
                                                result.Add(colName, value)
                                            Next col
                                        End If
                                    End Using
                                End Sub)
            Return If((result Is Nothing), New Hashtable, result)
        End Function

        Public Function AsList(Of T)() As List(Of T) Implements Interfaces.ICommandExecution.AsList
            Dim result As New List(Of T)
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using reader As IDataReader = comm.ExecuteReader
                                        Do While reader.Read
                                            Dim value As Object = reader.Item(0)
                                            If (value Is DBNull.Value) Then
                                                value = Nothing
                                            End If
                                            result.Add(DirectCast(value, T))
                                        Loop
                                    End Using
                                End Sub)
            Return result
        End Function

        Public Function AsList(Of T)(ByVal mapper As ModelMapper(Of T)) As List(Of T) Implements Interfaces.ICommandExecution.AsList
            Dim result As New List(Of T)
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using reader As IDataReader = comm.ExecuteReader
                                        Do While reader.Read
                                            result.Add(mapper.Map(reader))
                                        Loop
                                    End Using
                                End Sub)
            Return result
        End Function

        Public Sub ExecuteNonQuery() Implements Interfaces.ICommandExecution.ExecuteNonQuery
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    comm.ExecuteNonQuery()
                                End Sub)
        End Sub

        Public Sub ExecuteReader(ByVal action As Action(Of IDataReader)) Implements Interfaces.ICommandExecution.ExecuteReader
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using reader As IDataReader = comm.ExecuteReader
                                        Do While reader.Read
                                            action.Invoke(reader)
                                        Loop
                                    End Using
                                End Sub)
        End Sub

        Public Function ExecuteReaderSingle(Of T As {Class, New})(ByVal mapper As ModelMapper(Of T)) As T Implements Interfaces.ICommandExecution.ExecuteReaderSingle
            Dim model As T = CType(Nothing, T)
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using r As IDataReader = comm.ExecuteReader
                                        If r.Read Then
                                            model = mapper.Map(r)
                                        End If
                                    End Using
                                End Sub)
            Return model
        End Function

        Public Sub ExecuteReaderSingle(ByVal action As Action(Of IDataReader)) Implements Interfaces.ICommandExecution.ExecuteReaderSingle
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Using reader As IDataReader = comm.ExecuteReader
                                        If reader.Read Then
                                            action.Invoke(reader)
                                        End If
                                    End Using
                                End Sub)
        End Sub

        Public Function ExecuteScalar(Of T)() As T Implements Interfaces.ICommandExecution.ExecuteScalar
            Dim result As T = CType(Nothing, T)
            Me.ExecutionWrapper(Sub(comm As SqlCommand)
                                    Dim r As Object = comm.ExecuteScalar
                                    If (r Is DBNull.Value) Then
                                        r = Nothing
                                    End If
                                    result = DirectCast(r, T)
                                End Sub)
            Return result
        End Function

        Public Function ExecuteScalar(Of T)(ByVal defaultValue As T) As T Implements Interfaces.ICommandExecution.ExecuteScalar
            Dim result As T = CType(Nothing, T)
            ExecutionWrapper(Sub(comm As SqlCommand)
                                 Dim r As Object = comm.ExecuteScalar
                                 If ((r Is Nothing) OrElse (r Is DBNull.Value)) Then
                                     r = defaultValue
                                 End If
                                 result = DirectCast(r, T)
                             End Sub)
            Return result
        End Function

        Private Sub ExecutionWrapper(ByVal commandAction As Action(Of SqlCommand))
            Dim conn As SqlConnection = Me.GetSqlConnection
            Try
                Using comm As SqlCommand = New SqlCommand(_commandText, conn)
                    comm.CommandType = _commandType
                    comm.CommandTimeout = Connection.DefaultTimeOut
                    Dim p As SqlParameter
                    For Each p In Me.parameters
                        comm.Parameters.Add(p)
                    Next
                    If Not Me.isLive Then
                        conn.Open()
                    End If
                    commandAction.Invoke(comm)
                End Using
            Finally
                If Not (Me.isLive OrElse (conn Is Nothing)) Then
                    conn.Dispose()
                End If
            End Try
        End Sub

        Private Function GetSqlConnection() As SqlConnection
            Return If(Not IsNothing(_sqlConnection), _sqlConnection, If(Me.useDefaultConnection, Connection.GetDefaultConnection, Connection.GetNamedConnection(Me.connectionName)))
        End Function

        Public Function WithParam(Of T)(ByVal paramName As String, ByVal paramValue As T) As Interfaces.ICommandExecution
            Me.parameters.Add(New SqlParameter(paramName, paramValue))
            Return Me
        End Function

        Public Function WithParam(ByVal paramName As String, ByVal paramValue As Object) As Interfaces.ICommandExecution Implements Interfaces.ICommandExecution.WithParam
            Me.parameters.Add(New SqlParameter(paramName, paramValue))
            Return Me
        End Function

        Public Function WithParam(Of T)(ByVal paramName As String, ByVal dbType As SqlDbType, ByVal paramValue As T) As Interfaces.ICommandExecution
            Dim p As New SqlParameter(paramName, dbType) With {
                .Value = paramValue
            }
            Me.parameters.Add(p)
            Return Me
        End Function

    End Class

End Namespace