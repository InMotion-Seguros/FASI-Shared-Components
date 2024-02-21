using System.Data;

namespace InMotionGIT.Common.Domain.DataAccess;


public abstract class ModelMapper<T>
{

    protected ModelMapper()
    {
    }

    public abstract T Map(IDataReader r);

}