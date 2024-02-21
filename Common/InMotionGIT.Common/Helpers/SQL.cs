
namespace InMotionGIT.Common.Helpers
{

    public class SQL
    {

        public static string SafeSqlLikeClauseLiteral(string inputSQL)
        {
            string s = inputSQL;
            s = inputSQL.Replace("'", "''");
            s = s.Replace("[", "[[]");
            s = s.Replace("%", "[%]");
            s = s.Replace("_", "[_]");
            return s;
        }

    }

}