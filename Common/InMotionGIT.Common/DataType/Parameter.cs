using System;
#region using

using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

#endregion

namespace InMotionGIT.Common.DataType
{

    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.Common.DataType")]
    [XmlRoot(Namespace = "urn:InMotionGIT.Common.DataType")]
    public class Parameter
    {

        #region Public properties, to expose the state of the entity

        [XmlAttribute()]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute()]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute()]
        public bool SpecialKind { get; set; }

        [XmlIgnore()]
        public object ValueToExecute { get; set; }

        [XmlAttribute()]
        [DefaultValue(0)]
        public int Size { get; set; }

        [XmlAttribute()]
        [DefaultValue(0)]
        public int Scale { get; set; }

        #endregion

        #region ReadOnly Properties

        [XmlIgnore()]
        [Browsable(false)]
        public string FormatDBType
        {
            get
            {
                string kind = Type;

                if (kind.Contains("System."))
                {
                    kind = kind.Split('.')[1];
                }

                switch (kind.ToUpper() ?? "")
                {
                    case "STRING":
                    case "BOOLEAN":
                        {
                            return "VARCHAR(1)";
                        }

                    case "INT32":
                    case "DECIMAL":
                        {
                            return "NUMBER";
                        }

                    case "DATETIME":
                        {
                            return "DATE";
                        }

                    default:
                        {
                            return "VARCHAR(1)";
                        }
                }
            }
        }

        [XmlIgnore()]
        [Browsable(false)]
        public bool IsStringType
        {
            get
            {
                string realType = Type;

                if (realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase))
                {
                    realType = realType.Split('.')[1];
                }

                return realType == "Char" || realType == "NChar" || realType == "VarChar" || realType == "NVarChar" || realType == "String";
            }
        }

        [XmlIgnore()]
        [Browsable(false)]
        public bool IsNumericType
        {
            get
            {
                string realType = Type;

                if (realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase))
                {
                    realType = realType.Split('.')[1];
                }

                return realType == "Byte" || realType == "Currency" || realType == "Decimal" || realType == "Integer" || realType == "Long" || realType == "Numeric" || realType == "Short" || realType == "Number" || realType == "Double" || realType == "Int32";
            }
        }

        public DbType ParameterKindDB
        {
            get
            {
                string realType = Type;

                if (realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase))
                {
                    realType = Type.Split('.')[1];
                }

                switch (realType.ToUpper(CultureInfo.CurrentCulture) ?? "")
                {
                    case "BOOLEAN":
                        {
                            return DbType.Boolean;
                        }

                    case "CHAR":
                        {
                            return DbType.AnsiStringFixedLength;
                        }

                    case "DATE":
                    case "DATETIME":
                        {
                            return DbType.DateTime;
                        }

                    case "NUMBER":
                    case "DECIMAL":
                    case "NUMERIC":
                        {
                            return DbType.Decimal;
                        }

                    case "DOUBLE":
                        {
                            return DbType.Currency;
                        }

                    case "INTEGER":
                    case "INT32":
                        {
                            return DbType.Int32;
                        }

                    case "VARCHAR":
                    case "STRING":
                        {
                            return DbType.AnsiString;
                        }

                    default:
                        {
                            return DbType.AnsiString;
                        }
                }
            }
        }

        public string get_ValueNullCondition(string expression)
        {
            string result = string.Format(CultureInfo.InvariantCulture, "({0}", expression);

            if (string.Equals(expression, "Nothing", StringComparison.CurrentCultureIgnoreCase))
            {
                result = "True";
            }

            else if (Information.IsNumeric(expression))
            {
                result = "False";
            }
            else
            {
                switch (ParameterKindDB)
                {
                    case DbType.Decimal:
                    case DbType.Currency:
                    case DbType.Byte:
                    case DbType.Double:
                    case DbType.Int16:
                    case DbType.Int64:
                    case DbType.Int32:
                        {

                            result = "False";
                            break;
                        }

                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                        {
                            if (string.Equals(expression, "Session(\"nUsercode\")", StringComparison.CurrentCultureIgnoreCase))
                            {

                                result += ".ToString = String.Empty)";
                            }
                            else
                            {
                                result += " = String.Empty)";
                            }

                            break;
                        }

                    case DbType.DateTime:
                        {
                            result += " = Date.MinValue)";
                            break;
                        }

                    case DbType.Boolean:
                        {
                            result += " = False)";
                            break;
                        }

                    default:
                        {
                            result = "False";
                            break;
                        }
                }
            }

            return result;
        }

        public string GetDefaultValue
        {
            get
            {
                string defaultValue = string.Empty;

                if (IsStringType)
                {
                    defaultValue = "String.Empty";
                }

                else if (IsNumericType)
                {
                    defaultValue = "0";
                }
                else
                {
                    string realType = Type;

                    if (realType.StartsWith("System.", StringComparison.CurrentCultureIgnoreCase))
                    {
                        realType = realType.Split('.')[1];
                    }

                    switch (realType.ToUpper(CultureInfo.CurrentCulture) ?? "")
                    {
                        case "BOOLEAN":
                            {
                                defaultValue = "False";
                                break;
                            }

                        case "DATE":
                        case "DATETIME":
                            {
                                defaultValue = "Date.MinValue";
                                break;
                            }

                        default:
                            {
                                defaultValue = "String.Empty";
                                break;
                            }
                    }
                }

                return defaultValue;
            }
        }

        #endregion

        #region Methods

        public DbParameter CreateCommandParameter(DbCommand command)
        {
            return Helpers.DataAccessLayer.CommandParameter(command, Name, ParameterKindDB, Size, true, ValueToExecute);
        }

        #endregion

    }

}