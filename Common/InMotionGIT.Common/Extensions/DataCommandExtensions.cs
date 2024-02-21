using System;
using System.Collections.Generic;
using System.Data;

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Extension methods for the Services.Contracts.DataParameter data type
    /// </summary>
    public static class DataCommandExtensions
    {

        public static void AddParameter(ref Services.Contracts.DataCommand command, string name, DbType type, int size, bool isNull, object value, ParameterDirection direction)
        {
            if (command.Parameters.IsEmpty())
            {
                command.Parameters = new List<Services.Contracts.DataParameter>();
            }
            command.Parameters.Add(new Services.Contracts.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = direction });
        }

        public static void AddParameter(ref Services.Contracts.DataCommand command, string name, DbType type, int size, bool isNull, object value)
        {
            if (command.Parameters.IsEmpty())
            {
                command.Parameters = new List<Services.Contracts.DataParameter>();
            }

            command.Parameters.Add(new Services.Contracts.DataParameter() { Name = name, Type = type, Size = size, IsNull = isNull, Value = value, Direction = ParameterDirection.Input });
        }

        public static Services.Contracts.DataParameter AddParameter(ref Services.Contracts.DataCommand command, string name)
        {
            Services.Contracts.DataParameter result = null;

            foreach (Services.Contracts.DataParameter parameter in command.Parameters)
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

}