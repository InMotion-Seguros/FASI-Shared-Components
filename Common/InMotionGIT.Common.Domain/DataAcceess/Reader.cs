using System;
using System.Data;

namespace InMotionGIT.Common.Domain.DataAccess;


public class Reader
{

    private IDataReader reader;

    public Reader(IDataReader r)
    {
        if (r is null)
        {
            throw new ArgumentNullException("r");
        }
        reader = r;
    }

    private static T _Get<T>(IDataReader reader, int col)
    {
        var dummy = reader[col];
        if (ReferenceEquals(dummy, DBNull.Value))
        {
            dummy = null;
        }
        return (T)dummy;
    }

    private static T _Get<T>(IDataReader reader, string col)
    {
        var dummy = reader[col];
        if (ReferenceEquals(dummy, DBNull.Value))
        {
            dummy = null;
        }
        return (T)dummy;
    }

    public T Get<T>(string col)
    {
        return _Get<T>(reader, col);
    }

    public static T Read<T>(IDataReader r, int col)
    {
        return _Get<T>(r, col);
    }

    public static T Read<T>(IDataReader r, string col)
    {
        return _Get<T>(r, col);
    }

}