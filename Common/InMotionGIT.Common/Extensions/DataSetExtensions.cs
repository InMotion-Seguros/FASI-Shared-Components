using System;
using System.Data;

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Extension methods for the object data type
    /// </summary>
    public static class DataSetExtensions
    {

        public static bool IsEmpty(this DataSet value)
        {
            return value == null;
        }

        public static bool IsNotEmpty(this DataSet value)
        {
            return !(value == null);
        }

        public static bool HasData(this DataSet value)
        {
            try
            {
                if (value == null == false && value.Tables.Count > 0 && value.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}