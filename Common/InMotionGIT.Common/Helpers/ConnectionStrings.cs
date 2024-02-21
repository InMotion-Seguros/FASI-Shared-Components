using System.Collections.Generic;
using System.Configuration;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers;


public class ConnectionStrings
{

    /// <summary>
    /// Methodo que optiene el user y el password por medio de conectionStringName
    /// </summary>
    /// <param name="ConectionStringName">El nombre de conectionstring</param>
    /// <returns>La clase credential, que contiene el nombre y el password de la coneccion solicitada</returns>
    /// <remarks></remarks>
    public static Services.Contracts.Credential ConnectionStringUserAndPassword(string ConectionStringName, int companyId)
    {
        Services.Contracts.Credential result = null;
        string _User = string.Empty;
        string _PassWord = string.Empty;
        string tempConnectionString = string.Empty;
        ConnectionStringSettings _settingsConnecions = null;
        // En caso de llegar el id de la compañia vacio, pero a nivel de configuración este establecido la compañia por default,
        // entonces se establece el id compañia indicado.
        if (companyId == 0)
        {
            companyId = "BackOffice.CompanyDefault".AppSettings<int>();
        }

        if ("BackOffice.IsMultiCompany".AppSettings<bool>())
        {

            string tempConnectionStringName = "{0}.Mapper".SpecialFormater(ConectionStringName).AppSettings();
            if (!string.IsNullOrEmpty(tempConnectionStringName))
            {
                _settingsConnecions = ConfigurationManager.ConnectionStrings[tempConnectionStringName];
                if (!(_settingsConnecions == null))
                {
                    tempConnectionString = _settingsConnecions.ToString();
                    return GetCredentialUserAndPasss(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, false);
                }
            }

            if (tempConnectionString.IsNotEmpty() && "Core.Mapper".AppSettings().IsNotEmpty())
            {

                string[] conectionVecto = "Core.Mapper".AppSettings().Split(',');
                bool found = false;
                foreach (string itemVector in conectionVecto)
                {
                    if (itemVector.Equals(ConectionStringName))
                    {
                        found = true;
                        tempConnectionStringName = ConectionStringName;
                        _settingsConnecions = ConfigurationManager.ConnectionStrings[tempConnectionStringName];
                        if (!(_settingsConnecions == null))
                        {
                            tempConnectionString = _settingsConnecions.ToString();
                        }
                        break;
                    }
                }
                if (found)
                {
                    return GetCredentialUserAndPasss(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, false);
                }

            }
            if (string.IsNullOrEmpty(tempConnectionStringName))
            {
                _settingsConnecions = ConfigurationManager.ConnectionStrings[ConectionStringName];
                if (!(_settingsConnecions == null))
                {
                    tempConnectionString = _settingsConnecions.ToString();
                    return GetCredentialUserAndPasss(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, true);
                }
            }
        }
        else
        {
            _settingsConnecions = ConfigurationManager.ConnectionStrings[ConectionStringName];
            if (!(_settingsConnecions == null))
            {
                tempConnectionString = _settingsConnecions.ToString();
                return GetCredentialUserAndPasss(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, true);
            }
        }
        return result;
    }

    /// <summary>
    /// 'Metodo de creacion de credential
    /// </summary>
    /// <param name="connectionStrinName">Nombre del connectionstring</param>
    /// <param name="connectionString">Connectionstring</param>
    /// <param name="provider">Nombre del proveedor</param>
    /// <param name="companyId">Company id</param>
    /// <param name="filter">Filto concatenacion de user and password</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private static Services.Contracts.Credential GetCredentialUserAndPasss(string connectionStrinName, string connectionString, string provider, int companyId, bool filter)
    {
        var result = new Services.Contracts.Credential() { ConnectionStringName = connectionStrinName };
        if (provider.Contains("System.Data.SqlClient"))
        {
            var _conTemp = new SqlConnectionStringBuilder(connectionString);
            result.User = _conTemp.UserID;
            result.Password = _conTemp.Password;
        }
        else if (!filter)
        {
            var _tempCompan = BackOffice.MultiCompany.GetUserInfo((short)companyId);
            result.User = BackOffice.CryptSupport.DecryptString(Conversions.ToString(_tempCompan[1]));
            result.Password = BackOffice.CryptSupport.DecryptString(Conversions.ToString(_tempCompan[2]));
        }
        else
        {
            var _conTemp = new OracleConnectionStringBuilder(connectionString);
            result.User = _conTemp.UserID;
            result.Password = _conTemp.Password;
        }
        return result;
    }

    /// <summary>
    /// Method que retorna todas las connectionString en un lista de tipo conecctionstring
    /// </summary>
    /// <returns>Retorna una coleccion de tipo ConnectionStrings</returns>
    /// <remarks></remarks>
    public static List<Services.Contracts.ConnectionStrings> ConnectionStringGetAll(string CodeValidator, int companyId)
    {
        var result = new List<Services.Contracts.ConnectionStrings>();
        if (KeyValidatorType.KeyValidator(CodeValidator) | CodeValidator.Equals("1"))
        {
            var ConnectionStrings = ConfigurationManager.ConnectionStrings;
            if (ConnectionStrings.Count != 0)
            {
                var listConnectionsExclud = new List<string>();
                listConnectionsExclud.Add("LocalSqlServer".ToLower());
                listConnectionsExclud.Add("LocalMySqlServer".ToLower());
                listConnectionsExclud.Add("OraAspNetConString".ToLower());
                foreach (ConnectionStringSettings ItemConnections in ConnectionStrings)
                {
                    if (!listConnectionsExclud.ToArray().Contains(ItemConnections.Name.ToLower()))
                    {
                        result.Add(ConnectionStringGet(ItemConnections.Name, companyId));
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Method que retorna todas las connectionString en un lista de tipo conecctionstring
    /// </summary>
    /// <returns>Retorna una coleccion de tipo ConnectionStrings</returns>
    /// <remarks></remarks>
    public static Services.Contracts.ConnectionStrings ConnectionStringGet(string ConectionStringName, int companyId)
    {
        Services.Contracts.ConnectionStrings result = null;
        string _User = string.Empty;
        string _PassWord = string.Empty;
        string tempConnectionString = string.Empty;
        ConnectionStringSettings _settingsConnecions = null;
        // En caso de llegar el id de la compañia vacio, pero a nivel de configuración este establecido la compañia por default,
        // entonces se establece el id compañia indicado.
        if (companyId == 0)
        {
            companyId = "BackOffice.CompanyDefault".AppSettings<int>();
        }

        if ("BackOffice.IsMultiCompany".AppSettings<bool>())
        {
            string tempConnectionStringName = "{0}.Mapper".SpecialFormater(ConectionStringName).AppSettings();
            if (tempConnectionStringName.IsNotEmpty())
            {
                _settingsConnecions = ConfigurationManager.ConnectionStrings[tempConnectionStringName];
                if (!(_settingsConnecions == null))
                {
                    tempConnectionString = _settingsConnecions.ToString();
                    return GetConnectionStrings(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, false);
                }
            }

            if (tempConnectionString.IsNotEmpty() && "Core.Mapper".AppSettings().IsNotEmpty())
            {
                string[] conectionVecto = "Core.Mapper".AppSettings().Split(',');
                bool found = false;
                foreach (string itemVector in conectionVecto)
                {
                    if (itemVector.Equals(ConectionStringName))
                    {
                        found = true;
                        tempConnectionStringName = ConectionStringName;
                        _settingsConnecions = ConfigurationManager.ConnectionStrings[tempConnectionStringName];
                        if (!(_settingsConnecions == null))
                        {
                            tempConnectionString = _settingsConnecions.ToString();
                        }
                        break;
                    }
                }
                if (found)
                {
                    if (!(_settingsConnecions == null))
                    {
                        return GetConnectionStrings(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, false);
                    }
                }
            }

            if (string.IsNullOrEmpty(tempConnectionStringName))
            {
                _settingsConnecions = ConfigurationManager.ConnectionStrings[ConectionStringName];
                if (!(_settingsConnecions == null))
                {
                    tempConnectionString = _settingsConnecions.ToString();
                    return GetConnectionStrings(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, true);
                }
            }
        }
        else
        {
            _settingsConnecions = ConfigurationManager.ConnectionStrings[ConectionStringName];
            if (!(_settingsConnecions == null))
            {
                _settingsConnecions = ConfigurationManager.ConnectionStrings[ConectionStringName];
                tempConnectionString = _settingsConnecions.ToString();
                return GetConnectionStrings(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, true);
            }
        }
        return result;
    }

    /// <summary>
    /// 'Crea la Instancia de ConnectionStrings
    /// </summary>
    /// <param name="connectionStrinName">Nombre del connectionString</param>
    /// <param name="connectionString">Conectionstring</param>
    /// <param name="provider">Nombre del proveedor</param>
    /// <param name="companyId">Company Id</param>
    /// <param name="filter">Filto concatenacion de user and password</param>
    /// <returns></returns>
    /// <remarks></remarks>
    private static Services.Contracts.ConnectionStrings GetConnectionStrings(string connectionStrinName, string connectionString, string provider, int companyId, bool filter)
    {
        var result = new Services.Contracts.ConnectionStrings() { Name = connectionStrinName };
        if (provider.Contains("System.Data.SqlClient"))
        {
            var _conTemp = new SqlConnectionStringBuilder(connectionString);

            result.ProviderName = "System.Data.SqlClient";
            result.ConnectionString = connectionString;
            result.UserName = _conTemp.UserID;
            result.Password = _conTemp.Password;
            result.SourceType = Enumerations.EnumSourceType.SqlServer;
            result.ServiceName = _conTemp.DataSource;
            result.DatabaseName = _conTemp.InitialCatalog;
        }
        else
        {
            string temporalConnnections;
            temporalConnnections = Regex.Replace(connectionString, @"Min Pool Size=(\d+);", "");
            temporalConnnections = Regex.Replace(temporalConnnections, @"Incr Pool Size=(\d+);", "");
            temporalConnnections = Regex.Replace(temporalConnnections, @"Decr Pool Size=(\d+);", "");
            temporalConnnections = Regex.Replace(temporalConnnections, @"Connection Lifetime=(\d+);", "");
            temporalConnnections = Regex.Replace(temporalConnnections, @"Connection Timeout=(\d+);", "");
            temporalConnnections = Regex.Replace(temporalConnnections, @"Statement Cache Size=(\d+);", "");
            var _conTemp = new OracleConnectionStringBuilder(temporalConnnections);
            var _tempCompan = BackOffice.MultiCompany.GetUserInfo((short)companyId);
            result.ProviderName = "Oracle.DataAccess.Client";

            string _User = BackOffice.CryptSupport.DecryptString(Conversions.ToString(_tempCompan[1]));
            string _Password = BackOffice.CryptSupport.DecryptString(Conversions.ToString(_tempCompan[2]));

            result.UserName = _User;
            result.Password = _Password;
            result.SourceType = Enumerations.EnumSourceType.Oracle;
            result.ServiceName = _conTemp.DataSource;

            if (!filter)
            {
                string stringUserAndPasswor = "User ID={0};Password={1}".SpecialFormater(_User, _Password);
                result.ConnectionString = "{0};{1}".SpecialFormater(connectionString, stringUserAndPasswor);
            }
            else
            {
                result.ProviderName = "Oracle.DataAccess.Client";
                if (connectionString.Contains("User ID"))
                {
                    result.ProviderName = "Oracle.DataAccess.Client";
                    result.ConnectionString = connectionString;
                }
                else
                {
                    string stringUserAndPasswor = "User ID={0};Password={1}".SpecialFormater(_User, _Password);
                    result.ConnectionString = "{0};{1}".SpecialFormater(connectionString, stringUserAndPasswor);
                }
            }
        }

        return result;
    }

}