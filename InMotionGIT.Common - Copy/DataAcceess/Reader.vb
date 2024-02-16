Namespace DataAccess

    Public Class Reader

        Private reader As IDataReader

        Public Sub New(ByVal r As IDataReader)
            If (r Is Nothing) Then
                Throw New ArgumentNullException("r")
            End If
            Me.reader = r
        End Sub

        Private Shared Function _Get(Of T)(ByVal reader As IDataReader, ByVal col As Integer) As T
            Dim dummy As Object = reader.Item(col)
            If (dummy Is DBNull.Value) Then
                dummy = Nothing
            End If
            Return DirectCast(dummy, T)
        End Function

        Private Shared Function _Get(Of T)(ByVal reader As IDataReader, ByVal col As String) As T
            Dim dummy As Object = reader.Item(col)
            If (dummy Is DBNull.Value) Then
                dummy = Nothing
            End If
            Return DirectCast(dummy, T)
        End Function

        Public Function [Get](Of T)(ByVal col As String) As T
            Return _Get(Of T)(Me.reader, col)
        End Function

        Public Shared Function Read(Of T)(ByVal r As IDataReader, ByVal col As Integer) As T
            Return _Get(Of T)(r, col)
        End Function

        Public Shared Function Read(Of T)(ByVal r As IDataReader, ByVal col As String) As T
            Return _Get(Of T)(r, col)
        End Function

    End Class

End Namespace