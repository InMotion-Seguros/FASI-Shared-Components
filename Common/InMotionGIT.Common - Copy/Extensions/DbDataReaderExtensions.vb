Imports System.Data.Common
Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the DbDataReader type
    ''' </summary>
    Public Module DbDataReaderExtensions

        <Extension()>
        Public Function StringValue(ByVal value As DbDataReader, ordinal As Integer) As String
            Dim result As String = String.Empty
            If Not value.IsDBNull(ordinal) Then
                result = value.GetString(ordinal).Trim
            End If
            Return result
        End Function

        <Extension()>
        Public Function DateTimeValue(ByVal value As DbDataReader, ordinal As Integer) As Date
            Dim result As Date = Date.MinValue
            If Not value.IsDBNull(ordinal) Then
                result = value.GetDateTime(ordinal)
            End If
            Return result
        End Function

        <Extension()>
        Public Function NumericValue(ByVal value As DbDataReader, ordinal As Integer) As Decimal
            Dim result As Decimal = 0
            If Not value.IsDBNull(ordinal) Then
                result = value.GetDecimal(ordinal)
            End If
            Return result
        End Function

        <Extension()>
        Public Function BooleanValue(ByVal value As DbDataReader, ordinal As Integer) As Boolean
            Dim result As Boolean = 0
            If Not value.IsDBNull(ordinal) Then
                result = (value.GetDecimal(ordinal) = 1)
            End If
            Return result
        End Function

    End Module

End Namespace