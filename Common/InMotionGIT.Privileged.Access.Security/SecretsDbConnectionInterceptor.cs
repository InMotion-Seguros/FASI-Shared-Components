using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace InMotionGIT.Privileged.Access.Security
{
    public class SecretsDbConnectionInterceptor : IDbConnectionInterceptor
    {
        void IDbConnectionInterceptor.BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            var originalConnectionString = connection.ConnectionString;
            try
            {
                if (PrivilegedAccessSecurity.IsProvider())
                {
                    foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
                    {
                        if (item.ConnectionString.Equals(originalConnectionString))
                        {
                            connection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(item.Name, connection.ConnectionString);
                            break;
                        }

                        if (item.ConnectionString.Contains(originalConnectionString))
                        {
                            connection.ConnectionString = PrivilegedAccessSecurity.ConnectionString(item.Name, connection.ConnectionString);
                            break;
                        }
                    }
                }
                else
                    connection.ConnectionString = originalConnectionString;
            }
            catch (Exception e)
            {
                //InMotionGIT.Common.Helpers.LogHandler.ErrorLog("InMotionGIT.Privileged.Access.Security.SecretsDbConnectionInterceptor", $"key:{key}, Reason:{ex.Reason}, Message:{ex.Message}", ex);
                connection.ConnectionString = originalConnectionString;
            }
        }

        void IDbConnectionInterceptor.ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
        }

        void IDbConnectionInterceptor.StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
        }
    }
}