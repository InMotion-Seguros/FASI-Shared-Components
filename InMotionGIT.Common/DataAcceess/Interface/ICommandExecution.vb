Namespace DataAccess.Interfaces

    Public Interface ICommandExecution

        Function AsDataTable() As DataTable

        Function AsHashTable() As Hashtable

        Function AsList(Of T)() As List(Of T)

        Function AsList(Of T)(ByVal mapper As ModelMapper(Of T)) As List(Of T)

        Sub ExecuteNonQuery()

        Sub ExecuteReader(ByVal action As Action(Of IDataReader))

        Function ExecuteReaderSingle(Of T As {Class, New})(ByVal mapper As ModelMapper(Of T)) As T

        Sub ExecuteReaderSingle(ByVal action As Action(Of IDataReader))

        Function ExecuteScalar(Of T)() As T

        Function ExecuteScalar(Of T)(ByVal defaultValue As T) As T

        Function WithParam(ByVal paramName As String, ByVal paramValue As Object) As ICommandExecution

    End Interface

End Namespace