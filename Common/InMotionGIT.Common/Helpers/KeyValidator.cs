using System;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Helpers
{

    public sealed class KeyValidatorType
    {

        /// <summary>
        /// It generates a key based on the number of the week in the month the number of concatenated day of the week.
        /// </summary>
        /// <returns>Key generated</returns>
        public static string GenerateKey()
        {
            return string.Format("{0}{1}", DateTime.Now.WeekOfMonth(), DateTime.Now.NumericDayOfWeek());
        }

        /// <summary>
        /// Validator key for boor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyValidator(string key)
        {
            string result = GenerateKey();
            return result.Equals(key);
        }

    }

}