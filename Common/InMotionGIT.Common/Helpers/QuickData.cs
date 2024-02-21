using System;
using System.Configuration;
using System.Data;
using System.Globalization;

namespace InMotionGIT.Common.Helpers
{

    public class QuickData
    {

        // Public Shared Function ConvertQuotes(value As String) As String
        // Return value.Replace("'", "''")
        // End Function

        public static DataTable QueryExecute(string query, System.Data.Common.DbConnection dbConnection)
        {
            using (var DataAccess = new Data())
            {
                DataTable result;
                result = DataAccess.QueryExecute(query, dbConnection);
                return result;
            }
        }

        public static DataTable QueryExecute(string query, string connectionStringName)
        {
            using (var DataAccess = new Data())
            {
                DataTable result;
                result = DataAccess.QueryExecute(query, connectionStringName);
                return result;
            }
        }

        public static T QueryScalar<T>(string query, string connectionStringName)
        {
            using (var DataAccess = new Data())
            {
                T result;
                result = DataAccess.QueryScalar<T>(query, connectionStringName);
                return result;
            }
        }

        public static long CommandExecute(string command, string connectionStringName)
        {
            using (var DataAccess = new Data())
            {
                long result;
                result = DataAccess.CommandExecute(command, connectionStringName);
                return result;
            }
        }

        // TODO: validar este caso con el provider de microsoft.
        public static string DbProviderParameterPrefix(string parameterName, string providerName)
        {
            string result = string.Empty;
            switch (providerName.ToLower() ?? "")
            {
                case "oracle.dataaccess.client":
                    {
                        result = string.Format(":{0}", parameterName);
                        break;
                    }

                default:
                    {
                        result = string.Format("@{0}", parameterName);
                        break;
                    }
            }

            return result;
        }

        public static string DateFormat(string repositoryName)
        {
            return ConfigurationManager.AppSettings[string.Format("{0}.DateFormat", repositoryName)];
        }

        public static string ValueDateFormat(string repositoryName, DateTime value)
        {
            string result = value.ToString(DateFormat(repositoryName), new CultureInfo("en-US"));
            // result = result.Replace("/mm/", "/")
            return result;
        }

    }

}