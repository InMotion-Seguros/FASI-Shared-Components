Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the Services.Contracts.DataParameter data type
    ''' </summary>
    Public Module DataCommandExtensions

        <Extension()>
        Public Sub AddParameter(ByRef command As Services.Contracts.DataCommand, name As String, type As DbType, size As Integer, isNull As Boolean, value As Object, direction As ParameterDirection)
            If command.Parameters.IsEmpty Then
                command.Parameters = New List(Of Services.Contracts.DataParameter)
            End If
            command.Parameters.Add(New Services.Contracts.DataParameter With {.Name = name, .Type = type, .Size = size, .IsNull = isNull, .Value = value, .Direction = direction})
        End Sub

        <Extension()>
        Public Sub AddParameter(ByRef command As Services.Contracts.DataCommand, name As String, type As DbType, size As Integer, isNull As Boolean, value As Object)
            If command.Parameters.IsEmpty Then
                command.Parameters = New List(Of Services.Contracts.DataParameter)
            End If

            command.Parameters.Add(New Services.Contracts.DataParameter With {.Name = name, .Type = type, .Size = size, .IsNull = isNull, .Value = value, .Direction = ParameterDirection.Input})
        End Sub

        <Extension()>
        Public Function AddParameter(ByRef command As Services.Contracts.DataCommand, name As String) As Services.Contracts.DataParameter
            Dim result As Services.Contracts.DataParameter = Nothing

            For Each parameter As Services.Contracts.DataParameter In command.Parameters
                If String.Equals(parameter.Name, name, StringComparison.CurrentCultureIgnoreCase) Then
                    result = parameter
                    Exit For
                End If
            Next

            Return result
        End Function

    End Module

End Namespace