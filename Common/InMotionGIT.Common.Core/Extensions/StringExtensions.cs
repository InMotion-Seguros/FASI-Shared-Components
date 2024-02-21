using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InMotionGIT.Common.Core.Extensions;

/// <summary>
/// Extension methods for the string data type
/// </summary>
public static class StringExtensions
{
    public static string AbsoluteUri(this string value)
    {
        return (new Uri(value)).AbsoluteUri;
    }

    public static bool EqualIgnoringCase(this string value, string item)
    {
        return value.ToLower().Equals(item.ToLower(), StringComparison.CurrentCultureIgnoreCase);
    }

    public static T DeserializeJSON<T>(this string body)
    {
        return InMotionGIT.Common.Core.Helpers. SerializeHandler<T>.DeserializeJSON(body);
    }

    public static string SerializeJSON<T>(this T current, bool withFormat = true, bool preserveReferences = true, bool IgnoreNull = false, TypeNameHandling typeNameHandling = TypeNameHandling.All)
    {
        return InMotionGIT.Common.Core.Helpers. SerializeHandler<T>.SerializeJSON(current, withFormat, preserveReferences, IgnoreNull, typeNameHandling);
    }

    public static string Decrypt(this string value)
    {
        return value.IsNotEmpty() ? Helpers.CryptSupportNew.DecryptString(value) : "";
    }

    public static string Encrypt(this string value)
    {
        return value.IsNotEmpty() ? Helpers.CryptSupportNew.EncryptString(value) : "";
    }

    public static bool IsNumeric(this string input)
    {
        if (Regex.IsMatch(input, @"^\d+$"))
            return true;
        else
            return false;
    }

    public static string Formater(this string input, params object[] args)
    {
        return string.Format(input, args);
    }

    /// <summary>
    /// Turns a string into a properly XML Encoded string.
    /// Uses simple string replacement.
    ///
    /// Also see XmlUtils.XmlString() which uses XElement
    /// to handle additional extended characters.
    /// </summary>
    /// <param name="text">Plain text to convert to XML Encoded string</param>
    ///             <param name="isAttribute">
    ///             If true encodes single and double quotes.
    ///             When embedding element values quotes don't need to be encoded.
    ///             When embedding attributes quotes need to be encoded.
    ///             </param>
    ///             <returns>XML encoded string</returns>
    /// <exception cref="T:System.InvalidOperationException">Invalid character in XML string</exception>
    public static string XmlString(this string text, bool isAttribute = false)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }
        StringBuilder sb = new StringBuilder(text.Length);
        foreach (char chr in text)
        {
            if (chr == '<')
            {
                sb.Append("&lt;");
            }
            else if (chr == '>')
            {
                sb.Append("&gt;");
            }
            else if (chr == '&')
            {
                sb.Append("&amp;");
            }
            else if (isAttribute && chr == '"')
            {
                sb.Append("&quot;");
            }
            else if (isAttribute && chr == '\'')
            {
                sb.Append("&apos;");
            }
            else if (chr == '\n')
            {
                sb.Append(isAttribute ? "&#xA;" : "\n");
            }
            else if (chr == '\r')
            {
                sb.Append(isAttribute ? "&#xD;" : "\r");
            }
            else if (chr == '\t')
            {
                sb.Append(isAttribute ? "&#x9;" : "\t");
            }
            else
            {
                if (chr < ' ')
                {
                    throw new InvalidOperationException("Invalid character in Xml String. Chr " + Convert.ToInt16(chr).ToString() + " is illegal.");
                }
                sb.Append(chr);
            }
        }
        return sb.ToString();
    }

    public static bool EqualsIgnoringCase(this string input, string value)
    {
        bool result = false;

        if (string.Equals(input.ToLower(), value.IsNotEmpty() ? value.ToLower() : "", StringComparison.CurrentCultureIgnoreCase))
            result = true;

        return result;
    }

    /// <summary>
    /// Extension permite el formate de un string
    /// </summary>
    /// <param name="value">The string value to check.</param>
    public static string SpecialFormater(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    /// <summary>
    /// Determines whether the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    public static bool IsEmpty(this string value)
    {
        return value == null || string.IsNullOrEmpty(value) || value.Length == 0;
    }

    /// <summary>
    /// Determines whether the specified string is not null or empty.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    public static bool IsNotEmpty(this string value)
    {
        return !value.IsEmpty();
    }

    /// <summary>
    /// Checks whether the string is empty and returns a default value in case.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Either the string or the default value.</returns>
    public static string IfEmpty(this string value, string defaultValue)
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }
        else
        {
            return value;
        }
    }

    public static string Capitalize(this string value)
    {
        string result = value;

        if (result.IsNotEmpty() && result.Length > 1)
        {
            result = result.Substring(0, 1).ToUpper() + result.Substring(1).ToLower();
        }
        return result;
    }

    public static string ClosedList(this string value)
    {
        return value.ClosedList(endString: ",");
    }

    public static string ClosedList(this string value, string endString)
    {
        if (value.EndsWith(endString))
        {
            value = value.Substring(0, value.Length - endString.Length);
        }

        return value.Trim();
    }

    public static string Filter(this string value, string RegularExpression)
    {
        return value.Filter(RegularExpression, false);
    }

    public static string Filter(this string value, string RegularExpression, bool Exclude)
    {
        string result = string.Empty;
        var listMatch = new List<string>();

        foreach (Match match in Regex.Matches(value, RegularExpression, RegexOptions.IgnoreCase))
        {
            foreach (Group ItemMach in match.Groups)
                listMatch.Add(Conversions.ToString(ItemMach.Value));
        }
        if (Exclude)
        {
            result = value;
            foreach (string ItemList in listMatch)
                result = result.Replace(ItemList, string.Empty);
        }
        else
        {
            result = string.Empty;
            result = string.Join(string.Empty, listMatch.ToArray());
        }
        return result;
    }

    public static string OnlyNumbers(this string value)
    {
        var output = new StringBuilder();
        for (int i = 0, loopTo = value.Length - 1; i <= loopTo; i++)
        {
            if (Conversions.ToString(value[i]) == "." || Information.IsNumeric(value[i]))
            {
                output.Append(value[i]);
            }
        }
        return output.ToString();
    }

    /// <summary>
    /// Metodo de comprescion de string, esto ose utiliza en el string de serializacion de datatable por medio de json
    /// </summary>
    /// <param name="Text">Strign a comprimir</param>
    /// <returns>String comprimido</returns>
    /// <remarks></remarks>
    public static string CompressString(this string Text)
    {
        byte[] buffer__1 = Encoding.UTF8.GetBytes(Text);
        var memoryStream = new System.IO.MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer__1, 0, buffer__1.Length);
        }

        memoryStream.Position = 0L;

        byte[] compressedData = new byte[(int)(memoryStream.Length - 1L + 1)];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        byte[] gZipBuffer = new byte[compressedData.Length + 3 + 1];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer__1.Length), 0, gZipBuffer, 0, 4);
        Debug.WriteLine("Size original:" + buffer__1.Length.ToString());
        Debug.WriteLine("Size Comopres:" + gZipBuffer.Length.ToString());
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Metodo de descomprecion de string
    /// </summary>
    /// <param name="Text">String a descomprimir</param>
    /// <returns>Retorna el strign descomprimido</returns>
    /// <remarks></remarks>
    public static string DecompressString(this string Text)
    {
        byte[] gZipBuffer = Convert.FromBase64String(Text);
        using (var memoryStream = new System.IO.MemoryStream())
        {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

            byte[] buffer = new byte[dataLength];

            memoryStream.Position = 0L;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gZipStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }
    }

    public static DataTable Deserialize(this string objectString)
    {
        string descompresString = objectString.DecompressString();
        return JsonConvert.DeserializeObject<DataTable>(descompresString, new DataTableConverter());
    }

    public static string ReplaceIgnoreCase(this string originalString, string oldValue, string newValue)
    {
        if (originalString.IsEmpty())
        {
            originalString = string.Empty;
        }
        if (oldValue.IsEmpty())
        {
            oldValue = string.Empty;
        }
        if (newValue.IsEmpty())
        {
            newValue = string.Empty;
        }
        var startIndex = default(int);

        while (true)
        {
            startIndex = originalString.IndexOf(oldValue, startIndex, StringComparison.CurrentCultureIgnoreCase);
            if (startIndex == -1)
                break;
            originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);
            startIndex += newValue.Length;
        }

        return originalString;
    }

    public static string TagValue(this string value, string startTag, string endTag)
    {
        string result = string.Empty;
        int startIndex = value.IndexOf(startTag, StringComparison.CurrentCultureIgnoreCase);
        int endIndex = 0;

        if (startIndex > -1)
        {
            result = value.Substring(startIndex + 1);
            endIndex = result.IndexOf(endTag, StringComparison.CurrentCultureIgnoreCase);
            if (endIndex > -1)
            {
                if (endIndex == 0)
                {
                    result = string.Empty;
                }
                else
                {
                    result = result.Substring(0, endIndex);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 'Nueva función MaskedFormat para crear la máscara de confidencialidad según nivel de esquema
    /// </summary>
    /// <param name="value">Valor a formatear</param>
    /// <param name="displayRule"> Mostrar primeros o últimos carácteres</param>
    /// <param name="numbersOfPositions">Número de carácteres a mostrar</param>
    /// <returns>Valor con máscara</returns>
    public static string MaskedFormat(this string value, int displayRule, int numbersOfPositions)
    {
        if (string.IsNullOrEmpty(value))
        {
            value = string.Empty;
        }
        else
        {
            int maskedStringLenght = value.Trim().Length - numbersOfPositions;
            string visibleString = string.Empty;
            string maskedString = string.Empty;

            switch (displayRule)
            {
                case 0: // None
                    {
                        maskedString = value.Trim();
                        break;
                    }

                case 1: // First Characters
                    {
                        if (maskedStringLenght < 0)
                        {
                            visibleString = value.Trim();
                            maskedString = string.Empty;
                        }
                        else
                        {
                            visibleString = value.Trim().Substring(0, numbersOfPositions);
                            maskedString = value.Trim().Substring(numbersOfPositions);
                        }

                        break;
                    }

                case 2: // Last Characters
                    {
                        if (maskedStringLenght < 0)
                        {
                            visibleString = value.Trim();
                            maskedString = string.Empty;
                        }
                        else
                        {
                            visibleString = value.Trim().Substring(maskedStringLenght);
                            maskedString = value.Trim().Substring(0, maskedStringLenght);
                        }

                        break;
                    }
            }

            foreach (char character in maskedString.Trim())
                maskedString = maskedString.Trim().Replace(Conversions.ToString(character), "*");

            if (displayRule == 2) // Last Characters
            {
                value = maskedString + visibleString;
            }
            else // First Characters & None
            {
                value = visibleString + maskedString;
            }
        }

        return value;
    }
}