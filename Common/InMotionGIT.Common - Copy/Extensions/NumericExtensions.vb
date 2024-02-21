Imports System.Configuration
Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the numerics data type
    ''' </summary>
    Public Module NumericExtensions

#Region "Integer"

        ''' <summary>
        ''' Determines whether the specified integer is null or zero.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsEmpty(ByVal value As Integer) As Boolean
            Return IsNothing(value) OrElse
                   value = 0
        End Function

        ''' <summary>
        ''' Determines whether the specified Integer is not null or empty.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsNotEmpty(ByVal value As Integer) As Boolean
            Return Not value.IsEmpty
        End Function

        ''' <summary>
        ''' Checks whether the Integer is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The Integer to check.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the Integer or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Integer, defaultValue As Integer) As Integer
            If value.IsEmpty Then
                Return defaultValue
            Else
                Return value
            End If
        End Function

        ''' <summary>
        ''' Checks whether the Integer is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The Integer to check.</param>
        ''' <param name="appSettingKey">AppSetting Key Name</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the Integer or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Integer, appSettingKey As String, defaultValue As Integer) As Integer
            If value.IsEmpty Then
                value = ConfigurationManager.AppSettings(appSettingKey)
                If value.IsEmpty Then
                    Return defaultValue
                Else
                    Return value
                End If
            Else
                Return value
            End If
        End Function

#End Region

#Region "Long"

        ''' <summary>
        ''' Determines whether the specified integer is null or zero.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsEmpty(ByVal value As Long) As Boolean
            Return IsNothing(value) OrElse
                   value = 0
        End Function

        ''' <summary>
        ''' Determines whether the specified Integer is not null or empty.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsNotEmpty(ByVal value As Long) As Boolean
            Return Not value.IsEmpty
        End Function

        ''' <summary>
        ''' Checks whether the Integer is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The Integer to check.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the Integer or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Long, defaultValue As Long) As Long
            If value.IsEmpty Then
                Return defaultValue
            Else
                Return value
            End If
        End Function

        ''' <summary>
        ''' Checks whether the Integer is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The Integer to check.</param>
        ''' <param name="appSettingKey">AppSetting Key Name</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the Integer or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Long, appSettingKey As String, defaultValue As Long) As Integer
            If value.IsEmpty Then
                value = ConfigurationManager.AppSettings(appSettingKey)
                If value.IsEmpty Then
                    Return defaultValue
                Else
                    Return value
                End If
            Else
                Return value
            End If
        End Function

#End Region

#Region "Decimal"

        ''' <summary>
        ''' Determines whether the specified integer is null or zero.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsEmpty(ByVal value As Decimal) As Boolean
            Return IsNothing(value) OrElse
                   value = 0
        End Function

        ''' <summary>
        ''' Determines whether the specified Integer is not null or empty.
        ''' </summary>
        ''' <param name="value">The Integer value to check.</param>
        <Extension()>
        Public Function IsNotEmpty(ByVal value As Decimal) As Boolean
            Return Not value.IsEmpty
        End Function

        ''' <summary>
        ''' Checks whether the Integer is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The Integer to check.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the Integer or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Decimal, defaultValue As Decimal) As Decimal
            If value.IsEmpty Then
                Return defaultValue
            Else
                Return value
            End If
        End Function

#End Region

    End Module

End Namespace