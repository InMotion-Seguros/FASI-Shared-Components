

#region using

#endregion

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Extension methods for the DataRow type
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {

        #region Method Information

        public static string GetPropertyName(this System.Reflection.MethodBase methodBase)
        {
            return methodBase.Name.Substring(4);
        }

        #endregion

    }

}