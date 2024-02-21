Imports System.Configuration

Public Class VisualTimeConfig

#Region "Field"

    Public Shared Property VisualTIMEConfigContent As String

#End Region

#Region "Contructor"

    Public Sub New()

        If Len(VisualTIMEConfigContent) = 0 Then
            Dim configFile As String = ConfigurationManager.AppSettings("BackOfficeConfigurationFile")

            If String.IsNullOrEmpty(configFile) Then
                Dim strDrive As String
                strDrive = My.Application.Info.DirectoryPath
                If strDrive > String.Empty Then
                    strDrive = Left(strDrive, 2)
                Else
                    strDrive = "D:"
                End If
                configFile = String.Format("{0}\VisualTIMENet\Configuration\VisualTIMEConfig.xml", strDrive)
            End If

            VisualTIMEConfigContent = LoadFileToText(configFile)
        End If
    End Sub

#End Region

    '%Objetivo: .
    '%Parámetros:
    '%    sFileName -
    Public Shared Function LoadFileToText(ByVal sFileName As String) As String

        ''On Error GoTo ErrorHandler

        If IO.File.Exists(sFileName) Then
            LoadFileToText = IO.File.ReadAllText(sFileName)
        Else
            LoadFileToText = String.Empty
        End If

        Exit Function
    End Function

    '**%Objective:
    '**%Parameters:
    '**%    sKey     -
    '**%    Default  -
    '**%    sGroup   -
    '**%    bDecrypt -
    '%Objetivo:
    '%Parámetros:
    '%      sKey     -
    '%      Default  -
    '%      sGroup   -
    '%      bDecrypt -
    Public Function LoadSetting(ByVal sKey As String, Optional ByVal Default_Renamed As Object = Nothing, Optional ByVal sGroup As String = "Settings", Optional ByVal bDecrypt As Boolean = False) As String
        Dim lstrGroup As String

        'On Error GoTo ErrorHandler

        sGroup = Replace(sGroup, Space(1), String.Empty)
        lstrGroup = GetBlock(VisualTIMEConfigContent, sGroup, True)
        If lstrGroup <> String.Empty Then
            If Not bDecrypt Then
                LoadSetting = GetBlock(lstrGroup, sKey, True)
            Else
                LoadSetting = Helpers.CryptSupport.DecryptString(GetBlock(lstrGroup, sKey, True))
            End If
        Else
            LoadSetting = String.Empty
        End If

        If LoadSetting.Length = 0 And Default_Renamed IsNot Nothing Then
            LoadSetting = Default_Renamed
        End If

        Exit Function
    End Function

    '**%Objective:
    '**%Parameters:
    '**%    sSource    -
    '**%    sTag       -
    '**%    bNotDelete -
    '%Objetivo:
    '%Parámetros:
    '%      sSource    -
    '%      sTag       -
    '%      bNotDelete -
    Private Function GetBlock(ByRef sSource As String, ByVal sTag As String, Optional ByVal bNotDelete As Boolean = False) As String
        Dim strLabel As String
        Dim lngIniPosition As Integer
        Dim lngEndPosition As Integer

        'On Error GoTo ErrorHandler

        strLabel = String.Format("<{0}>", UCase(sTag))
        lngIniPosition = InStr(UCase(sSource), strLabel)
        If lngIniPosition > 0 Then
            lngIniPosition = lngIniPosition + Len(strLabel)
            strLabel = String.Format("</{0}>", UCase(sTag))
            lngEndPosition = InStr(lngIniPosition, UCase(sSource), strLabel)
            If lngEndPosition > 0 Then
                GetBlock = Mid(sSource, lngIniPosition, lngEndPosition - lngIniPosition)
                If Not bNotDelete Then
                    sSource = Left(sSource, lngIniPosition + 1) & Mid(sSource, lngEndPosition)
                End If
            Else
                GetBlock = String.Empty
            End If
        Else
            GetBlock = String.Empty
        End If

        Exit Function
    End Function

    '**%Objective:
    '**%Parameters:
    '**%    sStream -
    '%Objetivo:
    '%Parámetros:
    '%      sStream -
    Public Function Encrypt(ByVal sStream As String) As String
        'On Error GoTo ErrorHandler

        Encrypt = Helpers.CryptSupport.EncryptString(sStream)

        Exit Function

    End Function

    '**%Objective:
    '**%Parameters:
    '**%    sStream -
    '%Objetivo:
    '%Parámetros:
    '%      sStream -
    Public Function Decrypt(ByVal sStream As String) As String
        'On Error GoTo ErrorHandler

        Decrypt = Helpers.CryptSupport.DecryptString(sStream)

        Exit Function

    End Function

    Public Function GetCompanySettings(ByVal id As Short, ByRef companyName As String, ByRef companyUser As String, ByRef companyPassword As String) As Boolean
        Dim multiCompanies As String = GetBlock(VisualTIMEConfigContent, "MultiCompanies", True)
        Dim indexBegin As Integer
        Dim tags() As String

        companyName = String.Empty
        companyUser = String.Empty
        companyPassword = String.Empty

        indexBegin = multiCompanies.IndexOf(String.Format("<Company id='{0}'", id)) + (String.Format("<Company id='{0}'", id)).Length
        If indexBegin > (String.Format("<Company id='{0}'", id)).Length Then
            multiCompanies = multiCompanies.Substring(indexBegin, multiCompanies.IndexOf("/>", indexBegin) - indexBegin).Trim
            multiCompanies = multiCompanies.Replace("name=", "=")
            multiCompanies = multiCompanies.Replace("user=", "=")
            multiCompanies = multiCompanies.Replace("password=", "=")
            multiCompanies = multiCompanies.Replace("'", "")
            tags = multiCompanies.Split("=")

            companyName = tags(1).Trim
            companyUser = tags(2).Trim
            companyPassword = tags(3).Trim
        End If

        GetCompanySettings = (companyName <> String.Empty)
    End Function

    ''' <summary>
    ''' Devuelve un setting existe en el archivo visualtimeconfig.xml
    ''' </summary>
    ''' <param name="section">Sección que agrupa al setting</param>
    ''' <param name="key">Clave del setting</param>
    ''' <param name="decrypt">Indica si el valor debe set desencriptado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Setting(section As String, key As String, decrypt As Boolean) As String
        Return (New VisualTimeConfig).LoadSetting(key, , section, decrypt)
    End Function

End Class