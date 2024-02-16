Imports System.Configuration
Imports System.Data.OracleClient
Imports System.Data.SqlClient
Imports System.Text.RegularExpressions

Namespace Helpers

    Public Class ConnectionStrings

        ''' <summary>
        ''' Methodo que optiene el user y el password por medio de conectionStringName
        ''' </summary>
        ''' <param name="ConectionStringName">El nombre de conectionstring</param>
        ''' <returns>La clase credential, que contiene el nombre y el password de la coneccion solicitada</returns>
        ''' <remarks></remarks>
        Public Shared Function ConnectionStringUserAndPassword(ConectionStringName As String, companyId As Integer) As Services.Contracts.Credential
            Dim result As Services.Contracts.Credential = Nothing
            Dim _User As String = String.Empty
            Dim _PassWord As String = String.Empty
            Dim tempConnectionString As String = String.Empty
            Dim _settingsConnecions As ConnectionStringSettings = Nothing
            'En caso de llegar el id de la compañia vacio, pero a nivel de configuración este establecido la compañia por default,
            'entonces se establece el id compañia indicado.
            If companyId = 0 Then
                companyId = "BackOffice.CompanyDefault".AppSettings(Of Integer)
            End If

            If "BackOffice.IsMultiCompany".AppSettings(Of Boolean) Then

                Dim tempConnectionStringName As String = "{0}.Mapper".SpecialFormater(ConectionStringName).AppSettings
                If Not String.IsNullOrEmpty(tempConnectionStringName) Then
                    _settingsConnecions = ConfigurationManager.ConnectionStrings(tempConnectionStringName)
                    If Not IsNothing(_settingsConnecions) Then
                        tempConnectionString = _settingsConnecions.ToString
                        Return GetCredentialUserAndPasss(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, False)
                    End If
                End If

                If tempConnectionString.IsNotEmpty() AndAlso "Core.Mapper".AppSettings().IsNotEmpty() Then

                    Dim conectionVecto As String() = "Core.Mapper".AppSettings().Split(",")
                    Dim found As Boolean = False
                    For Each itemVector As String In conectionVecto
                        If itemVector.Equals(ConectionStringName) Then
                            found = True
                            tempConnectionStringName = ConectionStringName
                            _settingsConnecions = ConfigurationManager.ConnectionStrings(tempConnectionStringName)
                            If Not IsNothing(_settingsConnecions) Then
                                tempConnectionString = _settingsConnecions.ToString
                            End If
                            Exit For
                        End If
                    Next
                    If found Then
                        Return GetCredentialUserAndPasss(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, False)
                    End If

                End If
                If String.IsNullOrEmpty(tempConnectionStringName) Then
                    _settingsConnecions = ConfigurationManager.ConnectionStrings(ConectionStringName)
                    If Not IsNothing(_settingsConnecions) Then
                        tempConnectionString = _settingsConnecions.ToString
                        Return GetCredentialUserAndPasss(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, True)
                    End If
                End If
            Else
                _settingsConnecions = ConfigurationManager.ConnectionStrings(ConectionStringName)
                If Not IsNothing(_settingsConnecions) Then
                    tempConnectionString = _settingsConnecions.ToString
                    Return GetCredentialUserAndPasss(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, True)
                End If
            End If
            Return result
        End Function

        ''' <summary>
        ''' 'Metodo de creacion de credential
        ''' </summary>
        ''' <param name="connectionStrinName">Nombre del connectionstring</param>
        ''' <param name="connectionString">Connectionstring</param>
        ''' <param name="provider">Nombre del proveedor</param>
        ''' <param name="companyId">Company id</param>
        ''' <param name="filter">Filto concatenacion de user and password</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetCredentialUserAndPasss(connectionStrinName As String, connectionString As String, provider As String, companyId As Integer, filter As Boolean) As Services.Contracts.Credential
            Dim result As New Services.Contracts.Credential With {.ConnectionStringName = connectionStrinName}
            If provider.Contains("System.Data.SqlClient") Then
                Dim _conTemp As New SqlConnectionStringBuilder(connectionString)
                result.User = _conTemp.UserID
                result.Password = _conTemp.Password
            Else
                If Not filter Then
                    Dim _tempCompan = BackOffice.MultiCompany.GetUserInfo(companyId)
                    result.User = BackOffice.CryptSupport.DecryptString(_tempCompan(1))
                    result.Password = BackOffice.CryptSupport.DecryptString(_tempCompan(2))
                Else
                    Dim _conTemp As New OracleConnectionStringBuilder(connectionString)
                    result.User = _conTemp.UserID
                    result.Password = _conTemp.Password
                End If
            End If
            Return result
        End Function

        ''' <summary>
        ''' Method que retorna todas las connectionString en un lista de tipo conecctionstring
        ''' </summary>
        ''' <returns>Retorna una coleccion de tipo ConnectionStrings</returns>
        ''' <remarks></remarks>
        Public Shared Function ConnectionStringGetAll(CodeValidator As String, companyId As Integer) As List(Of Services.Contracts.ConnectionString)
            Dim result As New List(Of Services.Contracts.ConnectionString)
            If KeyValidator.KeyValidator(CodeValidator) Or CodeValidator.Equals("1") Then
                Dim ConnectionStrings As ConnectionStringSettingsCollection = ConfigurationManager.ConnectionStrings
                If ConnectionStrings.Count <> 0 Then
                    Dim listConnectionsExclud As New List(Of String)
                    With listConnectionsExclud
                        .Add("LocalSqlServer".ToLower())
                        .Add("LocalMySqlServer".ToLower())
                        .Add("OraAspNetConString".ToLower())
                    End With
                    For Each ItemConnections As ConnectionStringSettings In ConnectionStrings
                        If Not listConnectionsExclud.ToArray.Contains(ItemConnections.Name.ToLower) Then
                            result.Add(ConnectionStringGet(ItemConnections.Name, companyId))
                        End If
                    Next
                End If
            End If
            Return result
        End Function

        ''' <summary>
        ''' Method que retorna todas las connectionString en un lista de tipo conecctionstring
        ''' </summary>
        ''' <returns>Retorna una coleccion de tipo ConnectionStrings</returns>
        ''' <remarks></remarks>
        Public Shared Function ConnectionStringGet(ConectionStringName As String, companyId As Integer) As Services.Contracts.ConnectionString
            Dim result As Services.Contracts.ConnectionString = Nothing
            Dim _User As String = String.Empty
            Dim _PassWord As String = String.Empty
            Dim tempConnectionString As String = String.Empty
            Dim _settingsConnecions As ConnectionStringSettings = Nothing
            'En caso de llegar el id de la compañia vacio, pero a nivel de configuración este establecido la compañia por default,
            'entonces se establece el id compañia indicado.
            If companyId = 0 Then
                companyId = "BackOffice.CompanyDefault".AppSettings(Of Integer)
            End If

            If "BackOffice.IsMultiCompany".AppSettings(Of Boolean) Then
                Dim tempConnectionStringName As String = "{0}.Mapper".SpecialFormater(ConectionStringName).AppSettings()
                If tempConnectionStringName.IsNotEmpty() Then
                    _settingsConnecions = ConfigurationManager.ConnectionStrings(tempConnectionStringName)
                    If Not IsNothing(_settingsConnecions) Then
                        tempConnectionString = _settingsConnecions.ToString
                        Return GetConnectionStrings(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, False)
                    End If
                End If

                If tempConnectionString.IsNotEmpty() AndAlso "Core.Mapper".AppSettings().IsNotEmpty() Then
                    Dim conectionVecto As String() = "Core.Mapper".AppSettings().Split(",")
                    Dim found As Boolean = False
                    For Each itemVector As String In conectionVecto
                        If itemVector.Equals(ConectionStringName) Then
                            found = True
                            tempConnectionStringName = ConectionStringName
                            _settingsConnecions = ConfigurationManager.ConnectionStrings(tempConnectionStringName)
                            If Not IsNothing(_settingsConnecions) Then
                                tempConnectionString = _settingsConnecions.ToString
                            End If
                            Exit For
                        End If
                    Next
                    If found Then
                        If Not IsNothing(_settingsConnecions) Then
                            Return GetConnectionStrings(tempConnectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, False)
                        End If
                    End If
                End If

                If String.IsNullOrEmpty(tempConnectionStringName) Then
                    _settingsConnecions = ConfigurationManager.ConnectionStrings(ConectionStringName)
                    If Not IsNothing(_settingsConnecions) Then
                        tempConnectionString = _settingsConnecions.ToString
                        Return GetConnectionStrings(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, True)
                    End If
                End If
            Else
                _settingsConnecions = ConfigurationManager.ConnectionStrings(ConectionStringName)
                If Not IsNothing(_settingsConnecions) Then
                    _settingsConnecions = ConfigurationManager.ConnectionStrings(ConectionStringName)
                    tempConnectionString = _settingsConnecions.ToString
                    Return GetConnectionStrings(ConectionStringName, tempConnectionString, _settingsConnecions.ProviderName, companyId, True)
                End If
            End If
            Return result
        End Function

        ''' <summary>
        ''' 'Crea la Instancia de ConnectionStrings
        ''' </summary>
        ''' <param name="connectionStrinName">Nombre del connectionString</param>
        ''' <param name="connectionString">Conectionstring</param>
        ''' <param name="provider">Nombre del proveedor</param>
        ''' <param name="companyId">Company Id</param>
        ''' <param name="filter">Filto concatenacion de user and password</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetConnectionStrings(connectionStrinName As String, connectionString As String, provider As String, companyId As Integer, filter As Boolean) As Services.Contracts.ConnectionString
            Dim result As New Services.Contracts.ConnectionString With {.Name = connectionStrinName}
            If provider.Contains("System.Data.SqlClient") Then
                Dim _conTemp As New SqlConnectionStringBuilder(connectionString)

                With result
                    result.ProviderName = "System.Data.SqlClient"
                    result.ConnectionString = connectionString
                    .UserName = _conTemp.UserID
                    .Password = _conTemp.Password
                    .SourceType = Enumerations.EnumSourceType.SqlServer
                    .ServiceName = _conTemp.DataSource
                    .DatabaseName = _conTemp.InitialCatalog
                End With
            Else
                Dim temporalConnnections As String
                With connectionString
                    temporalConnnections = Regex.Replace(connectionString, "Min Pool Size=(\d+);", "")
                    temporalConnnections = Regex.Replace(temporalConnnections, "Incr Pool Size=(\d+);", "")
                    temporalConnnections = Regex.Replace(temporalConnnections, "Decr Pool Size=(\d+);", "")
                    temporalConnnections = Regex.Replace(temporalConnnections, "Connection Lifetime=(\d+);", "")
                    temporalConnnections = Regex.Replace(temporalConnnections, "Connection Timeout=(\d+);", "")
                    temporalConnnections = Regex.Replace(temporalConnnections, "Statement Cache Size=(\d+);", "")
                End With
                Dim _conTemp As New OracleConnectionStringBuilder(temporalConnnections)
                Dim _tempCompan = BackOffice.MultiCompany.GetUserInfo(companyId)
                result.ProviderName = "Oracle.DataAccess.Client"

                Dim _User As String = BackOffice.CryptSupport.DecryptString(_tempCompan(1))
                Dim _Password As String = BackOffice.CryptSupport.DecryptString(_tempCompan(2))

                With result
                    .UserName = _User
                    .Password = _Password
                    .SourceType = Enumerations.EnumSourceType.Oracle
                    .ServiceName = _conTemp.DataSource
                End With

                If Not filter Then
                    Dim stringUserAndPasswor As String = "User ID={0};Password={1}".SpecialFormater(_User, _Password)
                    result.ConnectionString = "{0};{1}".SpecialFormater(connectionString, stringUserAndPasswor)
                Else
                    result.ProviderName = "Oracle.DataAccess.Client"
                    If connectionString.Contains("User ID") Then
                        result.ProviderName = "Oracle.DataAccess.Client"
                        result.ConnectionString = connectionString
                    Else
                        Dim stringUserAndPasswor As String = "User ID={0};Password={1}".SpecialFormater(_User, _Password)
                        result.ConnectionString = "{0};{1}".SpecialFormater(connectionString, stringUserAndPasswor)
                    End If
                End If
            End If

            Return result
        End Function

    End Class

End Namespace