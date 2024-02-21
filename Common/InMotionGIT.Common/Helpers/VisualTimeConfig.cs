﻿using System.Configuration;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common
{

    public class VisualTimeConfig
    {

        #region Field

        public static string VisualTIMEConfigContent { get; set; }

        #endregion

        #region Contructor

        public VisualTimeConfig()
        {

            if (Strings.Len(VisualTIMEConfigContent) == 0)
            {
                string configFile = ConfigurationManager.AppSettings["BackOfficeConfigurationFile"];

                if (string.IsNullOrEmpty(configFile))
                {
                    string strDrive;
                    strDrive = My.MyProject.Application.Info.DirectoryPath;
                    if (Operators.CompareString(strDrive, string.Empty, false) > 0)
                    {
                        strDrive = Strings.Left(strDrive, 2);
                    }
                    else
                    {
                        strDrive = "D:";
                    }
                    configFile = string.Format(@"{0}\VisualTIMENet\Configuration\VisualTIMEConfig.xml", strDrive);
                }

                VisualTIMEConfigContent = LoadFileToText(configFile);
            }
        }

        #endregion

        // %Objetivo: .
        // %Parámetros:
        // %    sFileName -
        public static string LoadFileToText(string sFileName)
        {
            string LoadFileToTextRet = default;

            // 'On Error GoTo ErrorHandler

            if (System.IO.File.Exists(sFileName))
            {
                LoadFileToTextRet = System.IO.File.ReadAllText(sFileName);
            }
            else
            {
                LoadFileToTextRet = string.Empty;
            }

            return LoadFileToTextRet;
        }

        // **%Objective:
        // **%Parameters:
        // **%    sKey     -
        // **%    Default  -
        // **%    sGroup   -
        // **%    bDecrypt -
        // %Objetivo:
        // %Parámetros:
        // %      sKey     -
        // %      Default  -
        // %      sGroup   -
        // %      bDecrypt -
        public string LoadSetting(string sKey, object Default_Renamed = null, string sGroup = "Settings", bool bDecrypt = false)
        {
            string LoadSettingRet = default;
            string lstrGroup;

            // On Error GoTo ErrorHandler

            sGroup = Strings.Replace(sGroup, Strings.Space(1), string.Empty);
            string argsSource = VisualTIMEConfigContent;
            lstrGroup = GetBlock(ref argsSource, sGroup, true);
            VisualTIMEConfigContent = argsSource;
            if (!string.IsNullOrEmpty(lstrGroup))
            {
                if (!bDecrypt)
                {
                    LoadSettingRet = GetBlock(ref lstrGroup, sKey, true);
                }
                else
                {
                    LoadSettingRet = Helpers.CryptSupport.DecryptString(GetBlock(ref lstrGroup, sKey, true));
                }
            }
            else
            {
                LoadSettingRet = string.Empty;
            }

            if (LoadSettingRet.Length == 0 & Default_Renamed is not null)
            {
                LoadSettingRet = Conversions.ToString(Default_Renamed);
            }

            return LoadSettingRet;
        }

        // **%Objective:
        // **%Parameters:
        // **%    sSource    -
        // **%    sTag       -
        // **%    bNotDelete -
        // %Objetivo:
        // %Parámetros:
        // %      sSource    -
        // %      sTag       -
        // %      bNotDelete -
        private string GetBlock(ref string sSource, string sTag, bool bNotDelete = false)
        {
            string GetBlockRet = default;
            string strLabel;
            int lngIniPosition;
            int lngEndPosition;

            // On Error GoTo ErrorHandler

            strLabel = string.Format("<{0}>", Strings.UCase(sTag));
            lngIniPosition = Strings.InStr(Strings.UCase(sSource), strLabel);
            if (lngIniPosition > 0)
            {
                lngIniPosition = lngIniPosition + Strings.Len(strLabel);
                strLabel = string.Format("</{0}>", Strings.UCase(sTag));
                lngEndPosition = Strings.InStr(lngIniPosition, Strings.UCase(sSource), strLabel);
                if (lngEndPosition > 0)
                {
                    GetBlockRet = Strings.Mid(sSource, lngIniPosition, lngEndPosition - lngIniPosition);
                    if (!bNotDelete)
                    {
                        sSource = Strings.Left(sSource, lngIniPosition + 1) + Strings.Mid(sSource, lngEndPosition);
                    }
                }
                else
                {
                    GetBlockRet = string.Empty;
                }
            }
            else
            {
                GetBlockRet = string.Empty;
            }

            return GetBlockRet;
        }

        // **%Objective:
        // **%Parameters:
        // **%    sStream -
        // %Objetivo:
        // %Parámetros:
        // %      sStream -
        public string Encrypt(string sStream)
        {
            string EncryptRet = default;
            // On Error GoTo ErrorHandler

            EncryptRet = Helpers.CryptSupport.EncryptString(sStream);

            return EncryptRet;

        }

        // **%Objective:
        // **%Parameters:
        // **%    sStream -
        // %Objetivo:
        // %Parámetros:
        // %      sStream -
        public string Decrypt(string sStream)
        {
            string DecryptRet = default;
            // On Error GoTo ErrorHandler

            DecryptRet = Helpers.CryptSupport.DecryptString(sStream);

            return DecryptRet;

        }

        public bool GetCompanySettings(short id, ref string companyName, ref string companyUser, ref string companyPassword)
        {
            bool GetCompanySettingsRet = default;
            string argsSource = VisualTIMEConfigContent;
            string multiCompanies = GetBlock(ref argsSource, "MultiCompanies", true);
            VisualTIMEConfigContent = argsSource;
            int indexBegin;
            string[] tags;

            companyName = string.Empty;
            companyUser = string.Empty;
            companyPassword = string.Empty;

            indexBegin = multiCompanies.IndexOf(string.Format("<Company id='{0}'", id)) + string.Format("<Company id='{0}'", id).Length;
            if (indexBegin > string.Format("<Company id='{0}'", id).Length)
            {
                multiCompanies = multiCompanies.Substring(indexBegin, multiCompanies.IndexOf("/>", indexBegin) - indexBegin).Trim();
                multiCompanies = multiCompanies.Replace("name=", "=");
                multiCompanies = multiCompanies.Replace("user=", "=");
                multiCompanies = multiCompanies.Replace("password=", "=");
                multiCompanies = multiCompanies.Replace("'", "");
                tags = multiCompanies.Split('=');

                companyName = tags[1].Trim();
                companyUser = tags[2].Trim();
                companyPassword = tags[3].Trim();
            }

            GetCompanySettingsRet = !string.IsNullOrEmpty(companyName);
            return GetCompanySettingsRet;
        }

        /// <summary>
    /// Devuelve un setting existe en el archivo visualtimeconfig.xml
    /// </summary>
    /// <param name="section">Sección que agrupa al setting</param>
    /// <param name="key">Clave del setting</param>
    /// <param name="decrypt">Indica si el valor debe set desencriptado</param>
    /// <returns></returns>
    /// <remarks></remarks>
        public static string Setting(string section, string key, bool decrypt)
        {
            return new VisualTimeConfig().LoadSetting(key, sGroup: section, bDecrypt: decrypt);
        }

    }
}