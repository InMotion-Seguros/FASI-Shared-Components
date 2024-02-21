using System.IO;

namespace InMotionGIT.Common.Helpers
{

    public class Directory
    {

        public static string GetPathRoot()
        {
            string result;
            result = string.Format("{0}", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            return result;
        }

    }

}