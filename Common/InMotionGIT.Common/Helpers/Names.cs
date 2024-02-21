using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    public class Names
    {

        public static string Standard(string name, Enumerations.EnumFriendlyMode mode)
        {
            string result = name;

            if ((mode & Enumerations.EnumFriendlyMode.Plural) == Enumerations.EnumFriendlyMode.Plural)
            {
                result = Inflector.Pluralize(result); // GetPluralName(result)
            }

            if ((mode & Enumerations.EnumFriendlyMode.Singular) == Enumerations.EnumFriendlyMode.Singular)
            {
                result = Inflector.Singularize(result); // GetSingularName(result)
            }

            if ((mode & Enumerations.EnumFriendlyMode.ExpandEachUpperCase) == Enumerations.EnumFriendlyMode.ExpandEachUpperCase)
            {
                result = FixFriendlyName(result);

                result = result.Replace(" Of ", " of ");
                result = result.Replace(" Or ", " or ");
                result = result.Replace(" To ", " to ");
                result = result.Replace(" Per ", " per ");
                result = result.Replace(" In ", " in ");
                result = result.Replace(" And ", " and ");
                result = result.Replace(" For ", " for ");
                result = result.Replace(" When ", " when ");
                result = result.Replace(" By ", " by ");
                result = result.Replace(" Is ", " is ");

                result = result.Replace(" De ", " de ");
                result = result.Replace(" Un ", " un ");
                result = result.Replace(" Para ", " para ");
                result = result.Replace(" Del ", " del ");
                result = result.Replace(" O ", " o ");
                result = result.Replace(" En ", " en ");
                result = result.Replace(" Al ", " al ");
                result = result.Replace(" La ", " la ");
                result = result.Replace(" A ", " a ");
                result = result.Replace(" Y ", " y ");
                result = result.Replace(" No ", " no ");
                result = result.Replace(" Se ", " se ");
                result = result.Replace(" Por ", " por ");
            }

            if ((mode & Enumerations.EnumFriendlyMode.VisualBasicName) == Enumerations.EnumFriendlyMode.VisualBasicName)
            {
                result = VisualBasicName(result);
            }

            if ((mode & Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecialSingular) == Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecialSingular)
            {
                result = Inflector.Singularize(result);
                result = VisualBasicName(result).Replace("[", string.Empty).Replace("]", string.Empty);
            }

            if ((mode & Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecial) == Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecial)
            {
                result = VisualBasicName(result).Replace("[", string.Empty).Replace("]", string.Empty);
            }

            if ((mode & Enumerations.EnumFriendlyMode.XMLEncode) == Enumerations.EnumFriendlyMode.XMLEncode)
            {
                result = XmlEncode(result);
            }

            if ((mode & Enumerations.EnumFriendlyMode.PascalCased) == Enumerations.EnumFriendlyMode.PascalCased)
            {
                result = result.Substring(0, 1).ToLower() + result.Substring(1);
            }

            if ((mode & Enumerations.EnumFriendlyMode.VisualBasicName) == Enumerations.EnumFriendlyMode.VisualBasicName)
            {
                result = VisualBasicName(result);

                if (!(result == null))
                {
                    result = result.Replace(Constants.vbCrLf, string.Empty);
                    result = result.Replace(Constants.vbCr, string.Empty);
                    result = result.Replace(Constants.vbLf, string.Empty);

                    result = result.Replace("á", "a");
                    result = result.Replace("Á", "A");

                    result = result.Replace("é", "e");
                    result = result.Replace("É", "E");

                    result = result.Replace("í", "i");
                    result = result.Replace("Í", "I");

                    result = result.Replace("ó", "o");
                    result = result.Replace("Ó", "O");

                    result = result.Replace("ú", "u");
                    result = result.Replace("Ú", "U");

                    result = result.Replace("ñ", string.Empty);
                    result = result.Replace("Ñ", string.Empty);

                    result = result.Replace("?", string.Empty);
                    result = result.Replace("¿", string.Empty);

                    result = result.Replace("(", string.Empty);
                    result = result.Replace(")", string.Empty);
                }
            }

            else if (!(result == null))
            {
                result = result.Replace(Constants.vbCrLf, " ");
                result = result.Replace(Constants.vbCr, " ");
                result = result.Replace(Constants.vbLf, " ");
                result = result.Replace("  ", " ");
            }

            if (result.IsNotEmpty())
            {
                result = result.Trim();
            }

            return result;
        }

        public static string FixFriendlyName(string name)
        {
            string returnValue = string.Empty;
            if (name.Trim().Length > 0)
            {
                bool process = false;

                returnValue = name.Substring(0, 1);
                for (int index = 1, loopTo = name.Length - 1; index <= loopTo; index++)
                {
                    if (IsDiferenUpperLower(name.Substring(index - 1, 1), name.Substring(index, 1)) & index > 1)
                    {
                        if (returnValue.Substring(returnValue.Length - 2, 1) != " ") // returnValue.Length > 2 AndAlso
                        {

                            // If returnValue.Substring(returnValue.Length - 1, 1) = returnValue.Substring(returnValue.Length - 1, 1).ToUpper Then
                            // returnValue = String.Format("{0} {1}", returnValue.Substring(0, returnValue.Length - 1), returnValue.Substring(returnValue.Length - 1, 1))
                            // Else
                            returnValue += " ";
                            // End If

                        }
                    }
                    if (Information.IsNumeric(name.Substring(index, 1)) & !Information.IsNumeric(returnValue.Substring(returnValue.Length - 1, 1)))
                    {
                        returnValue += string.Format(" {0}", name.Substring(index, 1));
                    }
                    else
                    {
                        returnValue += name.Substring(index, 1);
                    }

                }
                returnValue = returnValue.Replace("_", " ");
                returnValue = returnValue.Replace("  ", " ");
            }
            return returnValue;
        }

        private static bool IsDiferenUpperLower(string left, string rigth)
        {
            bool leftIslower = (left ?? "") == (left.ToLower() ?? "");
            bool rigthIslower = (rigth ?? "") == (rigth.ToLower() ?? "");

            return leftIslower ^ rigthIslower;
        }

        public static string VisualBasicName(string name)
        {
            name = name.Replace("[", string.Empty);
            name = name.Replace("]", string.Empty);

            switch (name.ToLower() ?? "")
            {
                case "module":
                case "alias":
                case "property":
                case "interface":
                case "operator":
                    {
                        name = string.Format("[{0}]", name);
                        break;
                    }
            }

            name = name.Replace(" ", string.Empty);
            name = name.Replace("-", string.Empty);
            name = name.Replace(".", string.Empty);
            name = name.Replace("/", string.Empty);
            name = name.Replace(@"\", string.Empty);
            name = name.Replace(":", string.Empty);
            name = name.Replace("#", string.Empty);

            return name;
        }

        private static string XmlEncode(string strText)
        {
            string XmlEncodeRet = default;
            int[] aryChars = new int[] { 38, 60, 62, 34, 61, 39 };

            for (int i = 0, loopTo = Information.UBound(aryChars); i <= loopTo; i++)
                strText = Strings.Replace(strText, Conversions.ToString(Strings.Chr(aryChars[i])), "&#" + aryChars[i] + ";");
            XmlEncodeRet = strText;
            return XmlEncodeRet;

        }

    }

}