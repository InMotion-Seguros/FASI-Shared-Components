Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the date data type
    ''' </summary>
    Public Module DateExtensions

        ''' <summary>
        ''' Determines whether the specified date is null or empty.
        ''' </summary>
        ''' <param name="value">The date value to check.</param>
        <Extension()>
        Public Function IsEmpty(ByVal value As Date) As Boolean
            Return IsNothing(value) OrElse
                   value = Date.MinValue
        End Function

        ''' <summary>
        ''' Determines whether the specified date is not null or empty.
        ''' </summary>
        ''' <param name="value">The date value to check.</param>
        <Extension()>
        Public Function IsNotEmpty(ByVal value As Date) As Boolean
            Return Not value.IsEmpty
        End Function

        ''' <summary>
        ''' Returns the current value adding 23 hours, 59 minutes and 59 seconds.
        ''' </summary>
        ''' <param name="value">The date value to check.</param>
        <Extension()>
        Public Function EndTimeOfDay(value As Date) As Date
            Dim newDateTime As New Date(value.Year, value.Month, value.Day)

            newDateTime = newDateTime.AddHours(23)
            newDateTime = newDateTime.AddMinutes(59)
            newDateTime = newDateTime.AddSeconds(59)

            Return newDateTime
        End Function

        ''' <summary>
        ''' Checks whether the string is date and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The date to check.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the date or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As Date, defaultValue As Date) As Date
            If value.IsEmpty Then
                Return defaultValue
            Else
                Return value
            End If
        End Function

        ''' <summary>
        ''' Returns the first day of the month of the provided date.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns>The first day of the month.</returns>
        <Extension()>
        Public Function FirstDayOfMonth(ByVal value As Date) As Date
            Return New Date(value.Year, value.Month, 1)
        End Function

        ''' <summary>
        ''' Returns the last day of the month of the provided date.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns>The last day of the month</returns>
        <Extension()>
        Public Function LastDayOfMonth(ByVal value As Date) As Date
            Return New DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month))
        End Function

        ''' <summary>
        ''' Gets a DateTime representing Next Day
        ''' </summary>
        ''' <param name="value">The current day</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function Tomorrow(ByVal value As Date) As Date
            Return value.AddDays(1)
        End Function

        ''' <summary>
        ''' Gets a DateTime representing Previous Day
        ''' </summary>
        ''' <param name="value">The current day</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function Yesterday(ByVal value As Date) As Date
            Return value.AddDays(-1)
        End Function

        ''' <summary>
        ''' Gets the first day of the week using the current culture.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns>The first day of the week</returns>
        <Extension()>
        Public Function FirstDayOfWeek(ByVal value As Date) As Date
            Return value.FirstDayOfWeek(CultureInfo.CurrentCulture)
        End Function

        ''' <summary>
        ''' Gets the first day of the week using the current culture.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <param name="cultureInfo">The culture to determine the first weekday of a week.</param>
        ''' <returns>The first day of the week</returns>
        <Extension()>
        Public Function FirstDayOfWeek(ByVal value As Date, cultureInfo As Globalization.CultureInfo) As Date
            Dim _firstDayOfWeek As DayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek
            While value.DayOfWeek <> _firstDayOfWeek
                value = value.AddDays(-1)
            End While
            Return value
        End Function

        ''' <summary>
        ''' Gets the last day of the week using the current culture.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns>The last day of the week</returns>
        <Extension()>
        Public Function LastDayOfWeek(ByVal value As Date) As Date
            Return value.LastDayOfWeek(CultureInfo.CurrentCulture)
        End Function

        ''' <summary>
        ''' Gets the last day of the week using the current culture.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <param name="cultureInfo">The culture to determine the first weekday of a week.</param>
        ''' <returns>The last day of the week</returns>
        <Extension()>
        Public Function LastDayOfWeek(ByVal value As Date, cultureInfo As Globalization.CultureInfo) As Date
            Return value.FirstDayOfWeek(cultureInfo).AddDays(6)
        End Function

        ''' <summary>
        ''' Get of Number Day of week.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns></returns>
        <Extension()>
        Public Function NumericDayOfWeek(ByVal value As Date) As Integer
            Return DirectCast(CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(value), Integer)
        End Function

        ''' <summary>
        ''' Get the week of the month.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <returns>Week number of the month</returns>
        <Extension()>
        Public Function WeekOfMonth(ByVal value As Date) As Integer
            Return (CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstDay, DayOfWeek.Monday) -
                    CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value.FirstDayOfMonth, CalendarWeekRule.FirstDay, DayOfWeek.Monday)) + 1
        End Function

        ''' <summary>
        ''' Returns the value associated with the extension dynamically.
        ''' </summary>
        ''' <param name="value">The date.</param>
        ''' <param name="extensionName">Extension name.</param>
        ''' <returns>The value returned by the extension</returns>
        <Extension()>
        Public Function ApplyExtension(ByVal value As Date, extensionName As String) As Date

            If extensionName.StartsWith("today.+", StringComparison.CurrentCultureIgnoreCase) AndAlso
                IsNumeric(extensionName.Substring(6)) Then
                value = Today.AddDays(extensionName.ToLower.Replace("today.", String.Empty))
            ElseIf extensionName.StartsWith("today.-", StringComparison.CurrentCultureIgnoreCase) AndAlso
                IsNumeric(extensionName.Substring(6)) Then
                value = Today.AddDays(extensionName.ToLower.Replace("today.", String.Empty))
            Else
                Select Case extensionName.ToLower
                    Case "today.firstdayofmonth", "firstdayofmonth"
                        value = value.FirstDayOfMonth()
                    Case "today.lastdayofmonth", "lastdayofmonth"
                        value = value.LastDayOfMonth()
                    Case "today.tomorrow", "tomorrow"
                        value = value.Tomorrow()
                    Case "today.yesterday", "yesterday"
                        value = value.Yesterday()
                    Case "today.firstdayofweek", "firstdayofweek"
                        value = value.FirstDayOfWeek()
                    Case "today.lastdayofweek", "lastdayofweek"
                        value = value.LastDayOfWeek()
                    Case "today"
                        value = Today
                End Select
            End If

            Return value
        End Function

    End Module

End Namespace