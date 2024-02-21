using System;

namespace InMotionGIT.Common.Extensions
{

    // http://weblogs.asp.net/grantbarrington/archive/2009/01/19/enumhelper-getting-a-friendly-description-from-an-enum.aspx
    // http://calvinallen.net/archives/2012/extension-methods-against-an-enum-using-reflection/

    /// <summary>
    /// Extension methods for the enum data type
    /// </summary>
    static class EnumExtensions
    {

        /// <summary>
        /// Use this method to get the integer value of any enum.
        /// </summary>
        /// <param name="theEnum">The enum value.</param>
        public static int NumericValue(ref Enum theEnum)
        {
            return Convert.ToInt32(theEnum);
        }

        public static void FromString(ref Enum theEnum, string fromString)
        {
            theEnum = (Enum)Enum.Parse(theEnum.GetType(), fromString, true);
        }

    }

}