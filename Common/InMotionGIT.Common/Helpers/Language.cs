using System;
using System.Collections.Generic;

namespace InMotionGIT.Common.Helpers
{

    public sealed class Language
    {

        public static string EnumLanguageToString(bool full)
        {
            Dictionary<int, string> languagesList = (Dictionary<int, string>)Caching.GetItem("Languages");
            string result = string.Empty;

            foreach (KeyValuePair<int, string> languageItem in languagesList)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ",";
                }

                result += languageItem.Key.ToString();
            }

            return result;
        }

        // Public Shared Function EnumLanguageToDictionary(full As Boolean) As Dictionary(Of Integer, String)
        // Return EnumLanguageToDictionary(EnumLanguage.English, full)
        // End Function

        public static int CultureNamesToEnumLanguage(string codeLanguage)
        {
            int result;

            if (codeLanguage.ToLower().StartsWith("es"))
            {
                result = 2;
            }

            else if (codeLanguage.ToLower().StartsWith("en"))
            {
                result = 1;
            }

            else if (codeLanguage.ToLower().StartsWith("pt"))
            {
                result = 3;
            }
            else
            {
                result = 0;
            }

            return result;
        }

        public static int DescriptionToEnumLanguage(string description)
        {

            return DescriptionToEnumLanguage(description, 1);
        }

        public static int DescriptionToEnumLanguage(string description, int language)
        {
            var result = default(int);
            Dictionary<int, string> languagesList = (Dictionary<int, string>)Caching.GetItem("Languages");

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
        /// Extracts the name of the enum Cultural Language/Extrae el cultural name del enum de lenguaje
        /// </summary>
        /// <param name="language">Lenguaje/Language</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string LanguageToCultureName(int language)
        {
            string result = string.Empty;
            switch (language)
            {
                case 1:
                    {
                        result = "EN-US";
                        break;
                    }

                case 2:
                    {
                        result = "ES-CR";
                        break;
                    }

                default:
                    {
                        result = "PT-BR";
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CultureName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int CultureNameToLanguage(string CultureName)
        {
            int result;

            if (CultureName.StartsWith("en", StringComparison.CurrentCultureIgnoreCase))
            {
                result = 1;
            }
            else if (CultureName.StartsWith("es", StringComparison.CurrentCultureIgnoreCase))
            {
                result = 2;
            }
            else
            {
                result = 3;
            }

            return result;
        }

    }

}