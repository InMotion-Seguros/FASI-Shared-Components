using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace InMotionGIT.Common.DataAccess.Interfaces
{

    public interface ICommandExecution
    {

        DataTable AsDataTable();

        Hashtable AsHashTable();

        List<T> AsList<T>();

        List<T> AsList<T>(ModelMapper<T> mapper);

        void ExecuteNonQuery();

        void ExecuteReader(Action<IDataReader> action);

        T ExecuteReaderSingle<T>(ModelMapper<T> mapper) where T : class, new();

        void ExecuteReaderSingle(Action<IDataReader> action);

        T ExecuteScalar<T>();

        T ExecuteScalar<T>(T defaultValue);

        ICommandExecution WithParam(string paramName, object paramValue);

    }

}