Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the object data type
    ''' </summary>
    Public Module DataSetExtensions

        <Extension()>
        Public Function IsEmpty(ByVal value As DataSet) As Boolean
            Return IsNothing(value)
        End Function

        <Extension()>
        Public Function IsNotEmpty(ByVal value As DataSet) As Boolean
            Return Not IsNothing(value)
        End Function

        <Extension()>
        Public Function HasData(ByVal value As DataSet) As Boolean
            Try
                If IsNothing(value) = False AndAlso value.Tables.Count > 0 AndAlso value.Tables(0).Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Module

End Namespace