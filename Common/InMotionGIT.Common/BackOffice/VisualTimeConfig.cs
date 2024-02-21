using System.Configuration;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.BackOffice
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
                    configFile = strDrive + @"\VisualTIMENet" + @"\Configuration\VisualTIMEConfig.xml";
                }

                VisualTIMEConfigContent = LoadFileToText(configFile);
            }
        }

        #endregion

        public static string LoadFileToText(string sFileName)
        {
            string LoadFileToTextRet = default;
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

        public string LoadSetting(string sKey, object Default_Renamed = null, string sGroup = "Settings", bool bDecrypt = false)
        {
            string LoadSettingRet = default;
            string lstrGroup;

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
                    LoadSettingRet = CryptSupport.DecryptString(GetBlock(ref lstrGroup, sKey, true));
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

        private string GetBlock(ref string sSource, string sTag, bool bNotDelete = false)
        {
            string GetBlockRet = default;
            string strLabel;
            int lngIniPosition;
            int lngEndPosition;

            strLabel = "<" + Strings.UCase(sTag) + ">";
            lngIniPosition = Strings.InStr(Strings.UCase(sSource), strLabel);
            if (lngIniPosition > 0)
            {
                lngIniPosition = lngIniPosition + Strings.Len(strLabel);
                strLabel = "</" + Strings.UCase(sTag) + ">";
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

        public string Encrypt(string sStream)
        {
            string EncryptRet = default;
            EncryptRet = CryptSupport.EncryptString(sStream);
            return EncryptRet;
        }

        public string Decrypt(string sStream)
        {
            string DecryptRet = default;
            DecryptRet = CryptSupport.DecryptString(sStream);
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

            indexBegin = multiCompanies.IndexOf("<Company id='" + id.ToString() + "'") + ("<Company id='" + id.ToString() + "'").Length;
            if (indexBegin > ("<Company id='" + id.ToString() + "'").Length)
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

    }

}