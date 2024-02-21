using System;
using System.Collections.Generic;
using System.Data;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the  Domain.DataAccess.DataParameter data type
/// </summary>
public static class DataCommandExtensions
{
    public static void AddParameter(ref Domain.DataAccess.DataCommand command, string name, DbType type, int size, bool isNull, object value, ParameterDirection direction)
    {
        if (command.Parameters.IsEmpty())
        {
            command.Parameters = new List<Domain.DataAccess.DataParameter>();
        }
        command.Parameters.Add(new  Domain.DataAccess.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = direction });
    }

    public static void AddParameter(ref Domain.DataAccess.DataCommand command, string name, DbType type, int size, bool isNull, object value)
    {
        if (command.Parameters.IsEmpty())
        {
            command.Parameters = new List< Domain.DataAccess.DataParameter>();
        }

        command.Parameters.Add(new  Domain.DataAccess.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = ParameterDirection.Input });
    }

    public static Domain.DataAccess.DataParameter AddParameter(ref Domain.DataAccess.DataCommand command, string name)
    {
         Domain.DataAccess.DataParameter result = null;

        foreach ( Domain.DataAccess.DataParameter parameter in command.Parameters)
        {
            if (string.Equals(parameter.Name, name, StringComparison.CurrentCultureIgnoreCase))
            {
                result = parameter;
                break;
            }
        }

        return result;
    }
}