using System;

namespace InMotionGIT.Common.Attributes
{

    /// <summary>
    /// Define como acceder la tabla que contiene la lista de valores hacer usado por la propiedad.
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ElementLookupAttribute : Attribute
    {

        private string _TableName;
        private string _KeyField;
        private string _DescriptionField;
        private string _languageField;

        public string TableName
        {
            get
            {
                return _TableName;
            }
        }

        public string KeyField
        {
            get
            {
                return _KeyField;
            }
        }

        public string DescriptionField
        {
            get
            {
                return _DescriptionField;
            }
        }

        public string LanguageField
        {
            get
            {
                return _languageField;
            }
        }

        public ElementLookupAttribute(string tableName, string keyField, string descriptionField)
        {
            _TableName = tableName;
            _KeyField = keyField;
            _DescriptionField = descriptionField;
        }

        public ElementLookupAttribute(string tableName, string keyField, string descriptionField, string languageField)
        {
            _TableName = tableName;
            _KeyField = keyField;
            _DescriptionField = descriptionField;
            _languageField = languageField;
        }

    }

}