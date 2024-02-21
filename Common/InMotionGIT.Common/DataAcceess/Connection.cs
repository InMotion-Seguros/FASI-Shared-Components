using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace InMotionGIT.Common.DataAccess
{

    public class Connection : IDisposable
    {

        private static readonly Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
        private static string defaultConnectionString;
        private static int _defaultTimeOut;

        public static int DefaultTimeOut
        {
            get
            {
                return _defaultTimeOut;
            }
        }

        public static void AddNamedConnectionString(string connStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connStringName] is null)
            {
                throw new ArgumentException(string.Format("'{0}' not found on configuration", connStringName));
            }
            string connStringValue = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            connectionStrings.Add(connStringName, connStringValue);
        }

        public static Interfaces.ICommand GetAbsDBCommandFor(string connectionName)
        {
            return Command.WithConnection(connectionName);
        }

        public static Interfaces.ICommand GetAbsDBCommandForDefault()
        {
            return new Command();
        }

        private static Connection GetDBConnection()
        {
            return new Connection();
        }

        internal static SqlConnection GetDefaultConnection()
        {
            return new SqlConnection(defaultConnectionString);
        }

        internal static SqlConnection GetNamedConnection(string name)
        {
            if (!connectionStrings.ContainsKey(name))
            {
                throw new Exception(string.Format("named connection [{0}] has not been registered", name));
            }
            return new SqlConnection(connectionStrings[name]);
        }

        public static void SetDefaultConnectionString(string connStringName)
        {
            if (ConfigurationManager.ConnectionStrings[connStringName] is null)
            {
                throw new ArgumentException(string.Format("'{0}' not found on configuration", connStringName));
            }
            defaultConnectionString = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
        }

        public static void SetDefaultConnectionStringFromString(string connectionString)
        {
            defaultConnectionString = connectionString;
        }

        public static void SetDefaultTimeOut(int timeOut)
        {
            _defaultTimeOut = timeOut;
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        // Protected Overrides Sub Finalize()
        // ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        // Dispose(False)
        // MyBase.Finalize()
        // End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}