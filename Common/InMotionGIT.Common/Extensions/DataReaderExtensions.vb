#Region "using"

Imports System.Data.Common
Imports System.Runtime.CompilerServices

#End Region

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the DbDataReader type
    ''' </summary>
    Public Module DataReaderExtensions

#Region "Numeric Extension"

        <Extension()>
        Public Function NumericValue(value As DbDataReader, name As String) As Decimal
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As Decimal = 0

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = value.GetDecimal(ordinal)
            End If

            Return result
        End Function

        <Extension()>
        Public Function IntegerValue(value As DbDataReader, name As String) As Integer
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As Integer = 0

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = value.GetInt32(ordinal)
            End If

            Return result
        End Function

#End Region

#Region "DateTime Extension"

        <Extension()>
        Public Function DateTimeValue(value As DbDataReader, name As String) As Date
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As Date = Date.MinValue

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = value.GetDateTime(ordinal)
            End If

            Return result
        End Function

#End Region

#Region "Boolean Extension"

        <Extension()>
        Public Function BooleanValue(value As DbDataReader, name As String) As Boolean
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As Boolean = 0

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = (value.GetInt32(ordinal) = 1)
            End If

            Return result
        End Function

        <Extension()>
        Public Function BooleanCharValue(value As DbDataReader, name As String) As Boolean
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As Boolean = 0

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = (value.GetString(ordinal).Trim = "1")
            End If

            Return result
        End Function

#End Region

#Region "String Extension"

        <Extension()>
        Public Function StringValue(value As DbDataReader, name As String) As String
            Dim ordinal As Integer = value.GetOrdinal(name)
            Dim result As String = String.Empty

            If Not IsNothing(value) AndAlso Not value.IsDBNull(ordinal) Then
                result = value.GetString(ordinal)
                result = result.Trim
            End If

            Return result
        End Function

#End Region

    End Module

End Namespace