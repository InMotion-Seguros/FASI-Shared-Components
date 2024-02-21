using System;
using System.Globalization;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the date data type
/// </summary>
public static class DateExtensions
{
    /// <summary>
    /// Determines whether the specified date is null or empty.
    /// </summary>
    /// <param name="value">The date value to check.</param>
    public static bool IsEmpty(this DateTime value)
    {
        return (object)value == null || value == DateTime.MinValue;
    }

    /// <summary>
    /// Determines whether the specified date is not null or empty.
    /// </summary>
    /// <param name="value">The date value to check.</param>
    public static bool IsNotEmpty(this DateTime value)
    {
        return !value.IsEmpty();
    }

    /// <summary>
    /// Returns the current value adding 23 hours, 59 minutes and 59 seconds.
    /// </summary>
    /// <param name="value">The date value to check.</param>
    public static DateTime EndTimeOfDay(this DateTime value)
    {
        var newDateTime = new DateTime(value.Year, value.Month, value.Day);

        newDateTime = newDateTime.AddHours(23d);
        newDateTime = newDateTime.AddMinutes(59d);
        newDateTime = newDateTime.AddSeconds(59d);

        return newDateTime;
    }

    /// <summary>
    /// Checks whether the string is date and returns a default value in case.
    /// </summary>
    /// <param name="value">The date to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the date or the default value.</returns>
    public static DateTime IfEmpty(this DateTime value, DateTime defaultValue)
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Returns the first day of the month of the provided date.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns>The first day of the month.</returns>
    public static DateTime FirstDayOfMonth(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, 1);
    }

    /// <summary>
    /// Returns the last day of the month of the provided date.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns>The last day of the month</returns>
    public static DateTime LastDayOfMonth(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
    }

    /// <summary>
    /// Gets a DateTime representing Next Day
    /// </summary>
    /// <param name="value">The current day</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime Tomorrow(this DateTime value)
    {
        return value.AddDays(1d);
    }

    /// <summary>
    /// Gets a DateTime representing Previous Day
    /// </summary>
    /// <param name="value">The current day</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime Yesterday(this DateTime value)
    {
        return value.AddDays(-1);
    }

    /// <summary>
    /// Gets the first day of the week using the current culture.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns>The first day of the week</returns>
    public static DateTime FirstDayOfWeek(this DateTime value)
    {
        return value.FirstDayOfWeek(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the first day of the week using the current culture.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <param name="cultureInfo">The culture to determine the first weekday of a week.</param>
    /// <returns>The first day of the week</returns>
    public static DateTime FirstDayOfWeek(this DateTime value, CultureInfo cultureInfo)
    {
        var _firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
        while (value.DayOfWeek != _firstDayOfWeek)
            value = value.AddDays(-1);
        return value;
    }

    /// <summary>
    /// Gets the last day of the week using the current culture.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns>The last day of the week</returns>
    public static DateTime LastDayOfWeek(this DateTime value)
    {
        return value.LastDayOfWeek(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the last day of the week using the current culture.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <param name="cultureInfo">The culture to determine the first weekday of a week.</param>
    /// <returns>The last day of the week</returns>
    public static DateTime LastDayOfWeek(this DateTime value, CultureInfo cultureInfo)
    {
        return value.FirstDayOfWeek(cultureInfo).AddDays(6d);
    }

    /// <summary>
    /// Get of Number Day of week.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns></returns>
    public static int NumericDayOfWeek(this DateTime value)
    {
        return (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(value);
    }

    /// <summary>
    /// Get the week of the month.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <returns>Week number of the month</returns>
    public static int WeekOfMonth(this DateTime value)
    {
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstDay, DayOfWeek.Monday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value.FirstDayOfMonth(), CalendarWeekRule.FirstDay, DayOfWeek.Monday) + 1;
    }

    /// <summary>
    /// Returns the value associated with the extension dynamically.
    /// </summary>
    /// <param name="value">The date.</param>
    /// <param name="extensionName">Extension name.</param>
    /// <returns>The value returned by the extension</returns>
    public static DateTime ApplyExtension(this DateTime value, string extensionName)
    {
        if (extensionName.StartsWith("today.+", StringComparison.CurrentCultureIgnoreCase) && Information.IsNumeric(extensionName.Substring(6)))
        {
            value = DateTime.Today.AddDays(Conversions.ToDouble(extensionName.ToLower().Replace("today.", string.Empty)));
        }
        else if (extensionName.StartsWith("today.-", StringComparison.CurrentCultureIgnoreCase) && Information.IsNumeric(extensionName.Substring(6)))
        {
            value = DateTime.Today.AddDays(Conversions.ToDouble(extensionName.ToLower().Replace("today.", string.Empty)));
        }
        else
        {
            switch (extensionName.ToLower() ?? "")
            {
                case "today.firstdayofmonth":
                case "firstdayofmonth":
                    {
                        value = value.FirstDayOfMonth();
                        break;
                    }
                case "today.lastdayofmonth":
                case "lastdayofmonth":
                    {
                        value = value.LastDayOfMonth();
                        break;
                    }
                case "today.tomorrow":
                case "tomorrow":
                    {
                        value = value.Tomorrow();
                        break;
                    }
                case "today.yesterday":
                case "yesterday":
                    {
                        value = value.Yesterday();
                        break;
                    }
                case "today.firstdayofweek":
                case "firstdayofweek":
                    {
                        value = value.FirstDayOfWeek();
                        break;
                    }
                case "today.lastdayofweek":
                case "lastdayofweek":
                    {
                        value = value.LastDayOfWeek();
                        break;
                    }
                case "today":
                    {
                        value = DateTime.Today;
                        break;
                    }
            }
        }

        return value;
    }
}