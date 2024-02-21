

#region using
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.VisualBasic.CompilerServices;
using InMotionGIT.Common.Extensions;

#endregion using

namespace InMotionGIT.Common.Proxy.Helpers
{
    public class Language
    {
        #region Data Base Methods

        public static List<Common.Services.Contracts.Language> Languages()
        {
            var result = new List<Common.Services.Contracts.Language>();
            string key = "LanguagesAll";
            if (Common.Helpers.Caching.NotExist(key))
            {
                {
                    var withBlock = new DataManagerFactory(" SELECT " + " 	LOOKUP.* " + " FROM " + " 	LOOKUPMASTER " + " INNER JOIN LOOKUP ON LOOKUPMASTER.LOOKUPID = LOOKUP.LOOKUPID " + " WHERE " + " 	LOOKUPMASTER.KEY = 'Language' " + " AND LOOKUP.RECORDSTATUS = 1 ", "TabLanguage", "FrontOfficeConnectionString");

                    withBlock.Cache = Enumerations.EnumCache.CacheWithCommand;
                    var languageList = withBlock.QueryExecuteToTable(true);
                    string culturalCode = string.Empty;

                    if (languageList.IsNotEmpty() && languageList.Rows.Count != 0)
                    {
                        foreach (DataRow item in languageList.Rows)
                        {
                            if (!(languageList.Columns["HOMOLOGOUSCODE"] == null))
                            {
                                culturalCode = item.StringValue("HOMOLOGOUSCODE");
                            }
                            else
                            {
                                culturalCode = string.Empty;
                            }
                            result.Add(new Common.Services.Contracts.Language() { Code = item.IntegerValue("Code").ToString(), 
                                                                                  LanguageId = item.IntegerValue("LANGUAGEiD"), 
                                                                                  Description = item.StringValue("DESCRIPTION"), 
                                                                                  CulturalCode = culturalCode });
                        }
                    }
                    Common.Helpers.Caching.SetItem(key, result);
                }
            }
            else
            {
                result = (List<Common.Services.Contracts.Language>)Common.Helpers.Caching.GetItem(key);
            }
            return result;
        }

        public static List<Common.Services.Contracts.Language> LanguagesByCultural(string culturalCode)
        {
            var resultAll = Languages();
            var result = (from itemDb in resultAll
                          where itemDb.CulturalCode.ToLower().Equals(culturalCode.ToLower())
                          select itemDb).ToList();
            if (result.IsNotEmpty())
            {
                result = (from itemDb in resultAll
                          where itemDb.CulturalCode.ToLower().Equals("en".ToLower())
                          select itemDb).ToList();
            }
            return result;
        }

        public static List<Common.Services.Contracts.Language> LanguagesById(int languageId)
        {
            var resultAll = Languages();
            var result = (from itemDb in resultAll
                          where Operators.ConditionalCompareObjectEqual(itemDb.LanguageId, languageId, false)
                          select itemDb).ToList();
            if (result.IsEmpty())
            {
                result = (from itemDb in resultAll
                          where Operators.ConditionalCompareObjectEqual(itemDb.LanguageId, 1, false)
                          select itemDb).ToList();
            }
            return result;
        }

        public static Dictionary<int, string> LanguageToDictionary()
        {
            return LanguageToDictionary(1);
        }

        public static Dictionary<int, string> LanguageToDictionary(int language)
        {
            var result = new Dictionary<int, string>();
            var resultAll = Languages();
            var filterd = (from itemDb in resultAll
                           where Operators.ConditionalCompareObjectEqual(itemDb.LanguageId, language, false)
                           select itemDb).ToList();
            foreach (var item in filterd)
                result.Add(Conversions.ToInteger(item.Code), Conversions.ToString(item.Description));
            // Try
            // With New DataManagerFactory(String.Format(CultureInfo.InvariantCulture,
            // "SELECT TabLanguage.LanguageCode, TransLanguage.Description " &
            // "FROM TabLanguage " &
            // "LEFT JOIN TransLanguage ON TransLanguage.LanguageCode = TabLanguage.LanguageCode AND TransLanguage.LanguageID = {0} " &
            // "WHERE TabLanguage.RecordStatus = 1 ORDER BY Description", language),
            // "TabLanguage", "FrontOfficeConnectionString")

            // .Cache = Enumerations.EnumCache.CacheWithCommand
            // languageList = .QueryExecuteToTable(True)
            // End With
            // Catch ex As Exception

            // End Try

            // If Not IsNothing(languageList) AndAlso (Not IsNothing(languageList.Rows) AndAlso languageList.Rows.Count > 0) Then
            // For Each languageItem As DataRow In languageList.Rows
            // result.Add(languageItem("LanguageCode"), languageItem("Description"))
            // Next

            // Common.Helpers.Caching.SetItem("Languages", result, -1)
            // End If

            return result;
        }

        public static int SetLanguageCodeByDescription(string value)
        {
            var languagesList = LanguageToDictionary();
            int result = 0;

            foreach (KeyValuePair<int, string> languageItem in languagesList)
            {
                if (string.Equals(languageItem.Value, value, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = languageItem.Key;
                    break;
                }
            }

            return result;
        }

        public static string SetLanguageDescriptionByCode(int value)
        {
            var languagesList = LanguageToDictionary();
            string result = string.Empty;

            foreach (KeyValuePair<int, string> languageItem in languagesList)
            {
                if (languageItem.Key == value)
                {
                    result = languageItem.Value;
                    break;
                }
            }

            return result;
        }

        public static int DescriptionToEnumLanguage(string description, int language)
        {
            var result = default(int);
            var languagesList = LanguageToDictionary(language);

            foreach (KeyValuePair<int, string> languageItem in languagesList)
            {
                if (string.Equals(languageItem.Value, description, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = languageItem.Key;

                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Extrae todas los valores de determinado key en todos los recursos./Extracts all key values determined in all resources.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<DataType.LookUpValue> GetAllTraductions(string keyName)
        {
            var result = new List<DataType.LookUpValue>();
            string key = string.Format("GetAllTraductions_{0}", keyName);
            if (Common.Helpers.Caching.NotExist(key))
            {
                var resourceManager = My.Resources.Resources.ResourceManager;
                if (resourceManager.GetString(keyName).IsNotEmpty())
                {
                    string[] vector = resourceManager.GetString(keyName).Split(';');
                    foreach (var vectorItem in vector)
                        result.Add(new DataType.LookUpValue()
                        {
                            Code = vectorItem.Split(':')[0],
                            Description = vectorItem.Split(':')[1]
                        });
                }

                Common.Helpers.Caching.SetItem(key, result);
            }
            else
            {
                result = (List<DataType.LookUpValue>)Common.Helpers.Caching.GetItem(key);
            }

            return result;
        }

        /// <summary>
        /// Extrae todas los valores de determinado key en todos los recursos pero por código de traducción ./Extracts all key values determined in all resources by code lenguaje
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<DataType.LookUpValue> GetAllTraductionsByCodeLanguageId(string physicalApplicationPath,  string keyName)
        {
            string key = string.Format("GetAllTraductionsByCodeLanguageId_{0}", keyName);
            var result = new List<DataType.LookUpValue>();
            string[] namePathFile = System.IO.Directory.GetFiles(string.Format("{0}App_GlobalResources", physicalApplicationPath), "*.resx");
            if (Common.Helpers.Caching.NotExist(key))
            {
                foreach (var itemPathFile in namePathFile)
                {
                    var loResource = new XmlDocument();
                    loResource.Load(itemPathFile);
                    var loRoot = loResource.SelectSingleNode(string.Format("root/data[@name='{0}']/value", keyName));
                    if (!(loRoot == null))
                    {
                        string value = loRoot.InnerText;
                        string[] nameFile = System.IO.Path.GetFileNameWithoutExtension(itemPathFile).Split('.');
                        string culturalName = string.Empty;
                        if (nameFile.Count() == 1)
                        {
                            culturalName = "en";
                        }
                        else
                        {
                            culturalName = nameFile[1].ToLower();
                        }

                        int LangCode = GetLanguageIdCurrentContext(culturalName);
                        if (LangCode > 0)
                        {
                            result.Add(new DataType.LookUpValue() { Code = LangCode.ToString(), Description = value });
                        }
                    }
                }
                if (result.IsNotEmpty())
                {
                    result = result.OrderBy(X => X.Code).ToList();
                    var resullTemporal = new List<DataType.LookUpValue>();

                    foreach (DataType.LookUpValue Item in result)
                    {
                        var temporalIteim = (from itemLocal in resullTemporal
                                             where Operators.ConditionalCompareObjectEqual(itemLocal.Code, Item.Code, false) && itemLocal.Description.Equals(Item.Description)
                                             select itemLocal).FirstOrDefault;
                        if (temporalIteim.IsEmpty())
                        {
                            resullTemporal.Add(Item);
                        }
                    }

                    result = resullTemporal;
                }
                Common.Helpers.Caching.SetItem(key, result);
            }
            else
            {
                result = (List<DataType.LookUpValue>) Common.Helpers.Caching.GetItem(key);
            }

            return result;
        }

        /// <summary>
        /// obtiene la traducción del valor del lenguaje/ obtains the value of language translation
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetLanguageNameByLanguageId(int languageId)
        {
            string result = string.Empty;
            var listValueLanguageTraductions = LookUpLanguageByCurrentInfo(languageId);
            result = Conversions.ToString((from itemLanguageEnable in listValueLanguageTraductions
                                           where itemLanguageEnable.Code.Equals(languageId.ToString())
                                           select itemLanguageEnable.Description).FirstOrDefault());
            if (string.IsNullOrEmpty(result))
            {
                result = Conversions.ToString((from itemLanguageEnable in listValueLanguageTraductions
                                               select itemLanguageEnable.Description).FirstOrDefault());
            }
            return result;
        }

        /// <summary>
        /// Obtiene los posibles valores con el código de lenguaje del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<DataType.LookUpValue> LookUpLanguageByCurrentInfo()
        {
            return LookUpLanguageByCurrentInfo(-1);
        }

        /// <summary>
        /// Obtiene los posibles valores con el código de lenguaje del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<DataType.LookUpValue> LookUpLanguageByCurrentInfo(int LenguageId)
        {
            var cultureTemporal = Thread.CurrentThread.CurrentCulture;
            var listCultureInfoEnable = new List<DataType.LookUpValue>();
            if (LenguageId == -1)
            {
                LenguageId = GetLanguageIdCurrentContext();
            }
            else
            {
                LenguageId = ExistCode(LenguageId);
            }
            var languageById = LanguagesById(LenguageId);
            if (languageById.IsNotEmpty())
            {
                foreach (var item in languageById)
                    listCultureInfoEnable.Add(new DataType.LookUpValue() { Code = item.Code, Description = item.Description });
            }
            return listCultureInfoEnable;
        }

        /// <summary>
        /// Obtiene los posibles valores con el código de language del thread.currentinfo.name actual o sobrecargado con un id definido por el usuario/Possible values obtained with the current language code or overloaded thread.currentinfo.name a user defined id
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static DataType.LookUpValueCollection LookUpLanguageByCurrentInfoExtend()
        {
            var value = LookUpLanguageByCurrentInfo();
            var result = new DataType.LookUpValueCollection();
            foreach (var item in value)
                result.Add(new DataType.LookUpValue() { Code = item.Code, Description = item.Description });
            return result;
        }

        /// <summary>
        /// Obtiene el id del lenguage con el actual thread.curren.curreninfo.name/Gets the id of the current language thread.curren.curreninfo.name
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int GetLanguageIdCurrentContext()
        {
            return GetLanguageIdCurrentContext(string.Empty);
        }

        /// <summary>
        /// Obtiene el id del lenguaje con el actual currentinfo.nama defindo por el usuarioGets the id of language to the current user currentinfo.nama defindo
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int GetLanguageIdCurrentContext(string CultureInfoName)
        {
            var cultureTemporal = Thread.CurrentThread.CurrentCulture;
            var listCultureInfoEnable = Languages();
            if (listCultureInfoEnable.IsEmpty() || listCultureInfoEnable.Count == 0)
            {
                throw new Common.Exceptions.InMotionGITException("The 'frontoffice' table 'TABLANGUAGE' is empty");
            }

            if (CultureInfoName.IsEmpty())
            {
                if (!(HttpContext.Current.Session["App_LanguageId"] == null))
                {
                    return Convert.ToInt32(HttpContext.Current.Session["App_LanguageId"].ToString());
                }
                CultureInfoName = cultureTemporal.Name.ToLower();
            }
            else
            {
                CultureInfoName = CultureInfoName.ToLower();
            }

            string foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                      where itemLanguageEnable.CulturalCode.ToLower().Equals(CultureInfoName)
                                                      select itemLanguageEnable.Code).FirstOrDefault());
            if (string.IsNullOrEmpty(foundValue))
            {
                if (CultureInfoName.Split('-').Count() == 1)
                {
                    foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                       where itemLanguageEnable.CulturalCode.ToLower().StartsWith(CultureInfoName)
                                                       select itemLanguageEnable.Code).FirstOrDefault());
                }
                else
                {
                    foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                       where itemLanguageEnable.CulturalCode.ToLower().StartsWith(CultureInfoName.Split('-')[0])
                                                       select itemLanguageEnable.Code).FirstOrDefault());
                }
            }

            return string.IsNullOrEmpty(foundValue) ? 0 : int.Parse(foundValue);
        }

        public static bool CulturalCodeAllowed(string CultureInfoName)
        {
            bool result = false;
            var listCultureInfoEnable = Languages();
            string foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                      where itemLanguageEnable.CulturalCode.ToLower().Equals(CultureInfoName)
                                                      select itemLanguageEnable.Code).FirstOrDefault());
            if (foundValue.IsNotEmpty())
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Retorna un cultureInfo.Name válido por cultureInfo.Name/Returns a valid cultureInfo.Name by cultureInfo.Name
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetLanguageNameByCultureInfo(string cultureName)
        {
            cultureName = cultureName.ToLower();
            string result = string.Empty;
            var listCultureInfoEnable = GetAllCultureInfoName();
            string foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                      where itemLanguageEnable.Description.ToLower().Equals(cultureName)
                                                      select itemLanguageEnable.Description).FirstOrDefault());
            if (string.IsNullOrEmpty(foundValue))
            {
                if (cultureName.Split('-').Count() == 1)
                {
                    foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                       where itemLanguageEnable.Description.ToLower().StartsWith(cultureName)
                                                       select itemLanguageEnable.Description).FirstOrDefault());
                }
                else
                {
                    foundValue = Conversions.ToString((from itemLanguageEnable in listCultureInfoEnable
                                                       where itemLanguageEnable.Description.ToLower().StartsWith(cultureName.Split('-')[0])
                                                       select itemLanguageEnable.Description).FirstOrDefault());
                }
            }
            result = foundValue;
            return result;
        }

        /// <summary>
        /// Obtiene un loopUp de todos los cultureinfo.name disponible en base de datos/Gets a loopUp of all available cultureinfo.name database
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<DataType.LookUpValue> GetAllCultureInfoName()
        {
            var listCultureInfoEnable = new List<DataType.LookUpValue>();
            var listCultureInfoEnableAll = Languages();
            var fitered = (from itemDb in listCultureInfoEnableAll
                           select new { itemDb.Code, itemDb.CulturalCode }).Distinct().ToList();

            foreach (var item in fitered)
                listCultureInfoEnable.Add(new DataType.LookUpValue() { Code = item.Code, Description = item.CulturalCode });

            return listCultureInfoEnable;
        }

        /// <summary>
        /// Verifica la existencia de un id de lenguaje con respecto a los existentes en base de datos/Checks for an id of language with respect to the existing database
        /// </summary>
        /// <param name="LanguageCode"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ExistCode(int LanguageCode)
        {
            string result = string.Empty;
            var listCultureInfoEnable = Languages();

            result = Conversions.ToString((from CultureInfoItem in listCultureInfoEnable
                                           where Operators.ConditionalCompareObjectEqual(CultureInfoItem.LanguageId, LanguageCode, false)
                                           select CultureInfoItem.LanguageId).FirstOrDefault());

            if (result.IsEmpty())
            {
                result = Conversions.ToString((from CultureInfoItem in listCultureInfoEnable
                                               select CultureInfoItem.LanguageId).FirstOrDefault());
            }
            return Conversions.ToInteger(result);
        }

        /// <summary>
        /// Se obtiene el cultureinfo.name por medio de código /The cultureinfo.name is obtained through code
        /// </summary>
        /// <param name="LanguageCode"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetCultureInfoByCode(int LanguageCode)
        {
            string result = string.Empty;
            var listCultureInfoEnable = Languages();
            result = Conversions.ToString((from CultureInfoItem in listCultureInfoEnable
                                           where Operators.ConditionalCompareObjectEqual(CultureInfoItem.Code, LanguageCode, false)
                                           select CultureInfoItem.CulturalCode).FirstOrDefault());

            if (result.IsEmpty())
            {
                result = Conversions.ToString((from CultureInfoItem in listCultureInfoEnable
                                               select CultureInfoItem.CulturalCode).FirstOrDefault());
            }
            return result;
        }

        #endregion Data Base Methods

        #region Extension Methods

        public static string GetResourceExtension(int currentLanguage)
        {
            string result = string.Empty;
            var languagesAll = Languages();

            result = Conversions.ToString((from itemDb in languagesAll
                                           where Operators.ConditionalCompareObjectEqual(itemDb.Code, currentLanguage, false)
                                           select itemDb.CulturalCode).FirstOrDefault());

            return result;
        }

        public static int GetResourceLanguage(string extension)
        {
            int result = -1;
            var languagesAll = Languages();
            result = Conversions.ToInteger((from itemDb in languagesAll
                                            where Operators.ConditionalCompareObjectEqual(itemDb.CulturalCode, extension, false)
                                            select itemDb.LanguageId).FirstOrDefault());
            return result;
        }

        #endregion Extension Methods

        #region Get Flag Image Methods

        public static Image GetLanguageLargeImage(int key)
        {
            switch (key)
            {
                case 1:
                    {
                        return My.Resources.Resources.unitedStates;
                    }

                case 2:
                    {
                        return My.Resources.Resources.spain;
                    }

                case 3:
                    {
                        return My.Resources.Resources.portugal;
                    }

                case 4:
                    {
                        return My.Resources.Resources.netherlands;
                    }

                default:
                    {
                        break;
                    }
            }

            return null;
        }

        public static Image GetLanguageImage(int key)
        {
            switch (key)
            {
                case 1:
                    {
                        return My.Resources.Resources.us;
                    }

                case 2:
                    {
                        return My.Resources.Resources.es;
                    }

                case 3:
                    {
                        return My.Resources.Resources.pt;
                    }

                case 4:
                    {
                        return My.Resources.Resources.nl;
                    }

                default:
                    {
                        break;
                    }
            }

            return null;
        }

        #endregion Get Flag Image Methods
    }
}