using System.Data;

namespace InMotionGIT.Common.DataAccess
{

    public abstract class ModelMapper<T>
    {

        protected ModelMapper()
        {
        }

        public abstract T Map(IDataReader r);

    }

}