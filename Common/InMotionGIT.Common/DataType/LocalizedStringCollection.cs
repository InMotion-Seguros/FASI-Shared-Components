using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.DataType
{

    [CollectionDataContract(Namespace = "urn:InMotionGIT.Common.DataType")]
    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
    [XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LocalizedStringCollection : Collection<LocalizedString>
    {

        public LocalizedStringCollection() : base()
        {
        }

        public LocalizedStringCollection(bool enumBased) : base()
        {
            if (enumBased)
            {
                SetValue((int)Enumerations.EnumLanguage.English, string.Empty);
                SetValue((int)Enumerations.EnumLanguage.Spanish, string.Empty);
            }
        }

        public LocalizedStringCollection(IList<LocalizedString> list) : base(list)
        {
        }

        public void SetUpValues(int language, Dictionary<int, string> values)
        {
            Dictionary<int, string> languages = (Dictionary<int, string>)Helpers.Caching.GetItem("Languages");
            LocalizedString textresourceInstance;
            bool existLanguage;
            string defaultValue = string.Empty;

            foreach (KeyValuePair<int, string> itemValue in values)
            {
                if (itemValue.Key == language)
                {
                    defaultValue = itemValue.Value;
                }

                textresourceInstance = null;

                foreach (LocalizedString Item in this)
                {

                    if (Item.Language == itemValue.Key)
                    {
                        textresourceInstance = Item;
                        textresourceInstance.Value = itemValue.Value;

                        break;
                    }
                }

                if (textresourceInstance == null)
                {
                    textresourceInstance = new LocalizedString(itemValue.Key, itemValue.Value);
                    Add(textresourceInstance);
                }
            }

            if (!(languages == null))
            {

                foreach (KeyValuePair<int, string> languageItem in languages)
                {
                    existLanguage = false;

                    foreach (LocalizedString Item in this)
                    {
                        if (Item.Language == languageItem.Key)
                        {
                            existLanguage = true;

                            break;
                        }
                    }

                    if (!existLanguage)
                    {
                        textresourceInstance = new LocalizedString(languageItem.Key, defaultValue);
                        Add(textresourceInstance);
                    }
                }
            }
        }

        public LocalizedString SetValue(int language, string value)
        {
            LocalizedString textresourceInstance = null;

            foreach (LocalizedString item in this)
            {
                if (item.Language == language)
                {
                    textresourceInstance = item;
                    textresourceInstance.Value = value;
                    break;
                }
            }

            if (textresourceInstance == null)
            {
                textresourceInstance = new LocalizedString(language, value);
                Add(textresourceInstance);
            }

            return textresourceInstance;
        }

        public void SetAllValue(string value)
        {
            Dictionary<int, string> languages = (Dictionary<int, string>)Helpers.Caching.GetItem("Languages");

            foreach (KeyValuePair<int, string> languageItem in languages)
                SetValue(languageItem.Key, value);
        }

        public string GetUpValue(int language, int defaultLanguage)
        {
            string result = string.Empty;

            try
            {
                result = GetValue(language);

                if (string.IsNullOrEmpty(result))
                {
                    result = GetValue(defaultLanguage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidEnumArgumentException(string.Format(CultureInfo.InvariantCulture, "The language '{0}' not is valid", language));
            }

            if (result == null)
            {
                result = string.Empty;
            }

            return result;
        }

        public string GetValue(int language)
        {
            string result = string.Empty;

            foreach (LocalizedString item in this)
            {
                if (item.Language == language)
                {
                    result = item.Value;
                    break;
                }
            }

            if (result == null)
            {
                result = string.Empty;
            }
            if (result.IsNotEmpty())
            {
                result = result.Trim();
                // Esta línea fue comentada ya que la misma provocaba problemas en el generador de persistencia.
                // result = result.Replace("_", String.Empty)
            }
            return result;
        }

        public string GetValue(int language, Enumerations.EnumFriendlyMode mode)
        {
            return Helpers.Names.Standard(GetValue(language), mode);
        }

        public string GetValue(string language, Enumerations.EnumFriendlyMode mode)
        {
            return Helpers.Names.Standard(GetValue(Conversions.ToInteger(language)), mode);
        }

        public LocalizedStringCollection Clone()
        {
            var newLocalizedStrings = new LocalizedStringCollection();

            foreach (LocalizedString _item in this)
                newLocalizedStrings.Add(_item.Clone());

            return newLocalizedStrings;
        }

        public override string ToString()
        {
            string result;

            if (Count > 0)
            {
                result = "Text";
            }
            else
            {
                result = string.Empty;
            }

            return result;
        }

    }

}