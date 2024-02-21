using System;
using System.Data;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InMotionGIT.Common.Extensions
{
    /// <summary>
    /// Extension methods for the object data type
    /// </summary>
    public static class DataTableExtensions
    {
        public static bool IsEmpty(this DataTable value)
        {
            bool result = true;
            return !(value == null) && !(value.Rows == null) && value.Rows.Count > 0;
        }

        public static bool IsNotEmpty(this DataTable value)
        {
            return !(value == null) && !(value.Rows == null) && value.Rows.Count > 0;
        }

        public static DataRow FirstRow(this DataTable value)
        {
            return value.Rows[0];
        }

        public static object ReadOnlyMode(this DataTable value, bool Mode)
        {
            if (!(value == null) && !(value.Columns == null))
            {
                foreach (DataColumn ItemColumn in value.Columns)
                    ItemColumn.ReadOnly = Mode;
            }
            return value;
        }

        /// <summary>
        /// Extensión de serialización por Newtonsoft.JSON, para datatable específicos.
        /// </summary>
        /// <param name="pObjectSerializator">Data Table a serializar</param>
        /// <returns>datatable en formato json contenido en un string</returns>
        /// <remarks></remarks>
        public static string ToJSON(this DataTable pObjectSerializator)
        {
            return JsonConvert.SerializeObject(pObjectSerializator, new DataTableConverter());
        }

        public static string ToCSV(this DataTable dTable, string columnList = "", bool withHeader = true, bool headerQuote = false, bool stringQuote = true, string rowSeparator = Constants.vbNewLine, string fieldSeparator = ",", string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            // As Byte()
            var sb = new StringBuilder();
            int columnCount = 0;
            int index = 0;

            if (columnList.IsEmpty())
            {
                foreach (DataColumn colItem in dTable.Columns)
                    columnList += "," + colItem.ColumnName;
                columnList = columnList.Substring(1);
            }
            columnCount = columnList.Split(',').Length - 1;

            if (withHeader)
            {
                foreach (string column in columnList.Split(','))
                {
                    var columName = column.Trim();
                    if (headerQuote)
                    {
                        sb.Append("\"");
                    }
                    sb.Append(dTable.Columns[columName].ColumnName.ToString());
                    if (headerQuote)
                    {
                        sb.Append("\"");
                    }
                    if (index < columnCount)
                    {
                        sb.Append(fieldSeparator);
                    }
                    index += 1;
                }
                sb.Append(rowSeparator);
            }

            foreach (DataRow row in dTable.Rows)
            {
                index = 0;
                foreach (string column in columnList.Split(','))
                {
                    var columName = column.Trim();
                    if (dTable.Columns[columName].DataType.FullName == "System.String")
                    {
                        if (stringQuote)
                        {
                            sb.Append("\"");
                        }
                        sb.Append(row[columName].ToString().Replace("\"", "\"\""));
                        if (stringQuote)
                        {
                            sb.Append("\"");
                        }
                    }
                    else if (dTable.Columns[columName].DataType.FullName == "System.DateTime")
                    {
                        if (!(row[columName] is DBNull) && Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(row[columName], DateTime.MinValue, false)))
                        {
                            sb.Append(((DateTime)row[columName]).ToString(dateFormat));
                        }
                    }
                    else
                    {
                        sb.Append(row[columName].ToString().Replace(".", "."));
                    }

                    if (index < columnCount)
                    {
                        sb.Append(fieldSeparator);
                    }
                    index += 1;
                }
                sb.Append(rowSeparator);
            }

            // Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            return sb.ToString();
        }

        public static string ToXLS(this DataTable dTable, string columnList = "", bool withHeader = true, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            // As Byte()
            var sb = new StringBuilder();
            int columnCount = 0;
            int index = 0;

            if (columnList.IsEmpty())
            {
                foreach (DataColumn colItem in dTable.Columns)
                    columnList += "," + colItem.ColumnName;
                columnList = columnList.Substring(1);
            }
            columnCount = columnList.Split(',').Length - 1;

            sb.Append(ExcelHeader());

            sb.Append("<table>");
            if (withHeader)
            {
                sb.Append("<thead><tr>");
                foreach (string colum in columnList.Split(','))
                {
                    var columName = colum.Trim();
                    sb.Append("<th>" + dTable.Columns[columName].ColumnName + "</th>");
                }
                sb.Append("</tr></thead>");
            }

            sb.Append("<tbody>");
            foreach (DataRow row in dTable.Rows)
            {
                sb.Append("<tr>");
                foreach (string colum in columnList.Split(','))
                {
                    sb.Append("<td>");
                    var columName = colum.Trim();
                    if (dTable.Columns[columName].DataType.FullName == "System.String")
                    {
                        sb.Append(row[columName].ToString());
                    }
                    else if (dTable.Columns[columName].DataType.FullName == "System.DateTime")
                    {
                        if (!(row[columName] is DBNull) && Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(row[columName], DateTime.MinValue, false)))
                        {
                            sb.Append(((DateTime)row[columName]).ToString(dateFormat));
                        }
                    }
                    else
                    {
                        sb.Append(row[columName].ToString().Replace(".", "."));
                    }
                    sb.Append("</td>");
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append(ExcelFooter());
            // Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            return sb.ToString();
        }

        public static string ToHTML(this DataTable dTable, string columnList = "", bool withHeader = true, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            // As Byte()
            var sb = new StringBuilder();
            int columnCount = 0;
            int index = 0;

            if (columnList.IsEmpty())
            {
                foreach (DataColumn colItem in dTable.Columns)
                    columnList += "," + colItem.ColumnName;
                columnList = columnList.Substring(1);
            }
            columnCount = columnList.Split(',').Length - 1;

            sb.Append(HTMLHeader());

            sb.Append("<table>");
            if (withHeader)
            {
                sb.Append("<thead><tr>");
                foreach (string colum in columnList.Split(','))
                {
                    var columName = colum.Trim();
                    sb.Append("<th>" + dTable.Columns[columName].ColumnName + "</th>");
                }
                sb.Append("</tr></thead>");
            }

            sb.Append("<tbody>");
            foreach (DataRow row in dTable.Rows)
            {
                sb.Append("<tr>");
                foreach (string colum in columnList.Split(','))
                {
                    sb.Append("<td>");
                    var columName = colum.Trim();
                    if (dTable.Columns[columName].DataType.FullName == "System.String")
                    {
                        sb.Append(row[columName].ToString());
                    }
                    else if (dTable.Columns[columName].DataType.FullName == "System.DateTime")
                    {
                        if (!(row[columName] is DBNull) && Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(row[columName], DateTime.MinValue, false)))
                        {
                            sb.Append(((DateTime)row[columName]).ToString(dateFormat));
                        }
                    }
                    else
                    {
                        sb.Append(row[columName].ToString().Replace(".", "."));
                    }
                    sb.Append("</td>");
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append(HTMLFooter());
            // Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            return sb.ToString();
        }

        private static string ExcelHeader()
        {
            return "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"" + Constants.vbCrLf + "xmlns:x=\"urn:schemas-microsoft-com:office:excel\"" + Constants.vbCrLf + "xmlns=\"http://www.w3.org/TR/REC-html40\">" + Constants.vbCrLf + "" + Constants.vbCrLf + "<head>" + Constants.vbCrLf + "<meta http-equiv=Content-Type content=\"text/html; charset=windows-1252\">" + Constants.vbCrLf + "<meta name=ProgId content=Excel.Sheet>" + Constants.vbCrLf + "<meta name=Generator content=\"Microsoft Excel 11\">" + Constants.vbCrLf + "" + Constants.vbCrLf + "<!--[if gte mso 9]><xml>" + Constants.vbCrLf + " <x:ExcelWorkbook>" + Constants.vbCrLf + "  <x:ExcelWorksheets>" + Constants.vbCrLf + "   <x:ExcelWorksheet>" + Constants.vbCrLf + "    <x:Name>Worksheet Name</x:Name>" + Constants.vbCrLf + "    <x:WorksheetOptions>" + Constants.vbCrLf + "     <x:Selected/>" + Constants.vbCrLf + "     <x:FreezePanes/>" + Constants.vbCrLf + "     <x:FrozenNoSplit/>" + Constants.vbCrLf + "     <x:SplitHorizontal>1</x:SplitHorizontal>" + Constants.vbCrLf + "     <x:TopRowBottomPane>1</x:TopRowBottomPane>" + Constants.vbCrLf + "     <x:SplitVertical>1</x:SplitVertical>" + Constants.vbCrLf + "     <x:LeftColumnRightPane>1</x:LeftColumnRightPane>" + Constants.vbCrLf + "     <x:ActivePane>0</x:ActivePane>" + Constants.vbCrLf + "     <x:Panes>" + Constants.vbCrLf + "      <x:Pane>" + Constants.vbCrLf + "       <x:Number>3</x:Number>" + Constants.vbCrLf + "      </x:Pane>" + Constants.vbCrLf + "      <x:Pane>" + Constants.vbCrLf + "       <x:Number>1</x:Number>" + Constants.vbCrLf + "      </x:Pane>" + Constants.vbCrLf + "      <x:Pane>" + Constants.vbCrLf + "       <x:Number>2</x:Number>" + Constants.vbCrLf + "      </x:Pane>" + Constants.vbCrLf + "      <x:Pane>" + Constants.vbCrLf + "       <x:Number>0</x:Number>" + Constants.vbCrLf + "      </x:Pane>" + Constants.vbCrLf + "     </x:Panes>" + Constants.vbCrLf + "     <x:ProtectContents>False</x:ProtectContents>" + Constants.vbCrLf + "     <x:ProtectObjects>False</x:ProtectObjects>" + Constants.vbCrLf + "     <x:ProtectScenarios>False</x:ProtectScenarios>" + Constants.vbCrLf + "    </x:WorksheetOptions>" + Constants.vbCrLf + "   </x:ExcelWorksheet>" + Constants.vbCrLf + "  </x:ExcelWorksheets>" + Constants.vbCrLf + "  <x:ProtectStructure>False</x:ProtectStructure>" + Constants.vbCrLf + "  <x:ProtectWindows>False</x:ProtectWindows>" + Constants.vbCrLf + " </x:ExcelWorkbook>" + Constants.vbCrLf + "</xml><![endif]-->" + Constants.vbCrLf + "" + Constants.vbCrLf + "</head>" + Constants.vbCrLf + "<body>" + Constants.vbCrLf;
        }

        private static string ExcelFooter()
        {
            return "</body>" + Constants.vbCrLf + "</html>" + Constants.vbCrLf;
        }

        private static string HTMLHeader()
        {
            return "<!DOCTYPE html>" + Constants.vbCrLf + "<html>" + Constants.vbCrLf + "<head>" + Constants.vbCrLf + "<title></title>" + Constants.vbCrLf + "<style>" + Constants.vbCrLf + "table { border-collapse: collapse;}" + Constants.vbCrLf + "th, td { padding: 8px;text-align: left;border-bottom: 1px solid #ddd; }" + Constants.vbCrLf + "tr:hover {background-color: #f5f5f5}" + Constants.vbCrLf + "</style>" + Constants.vbCrLf + "</head>" + Constants.vbCrLf + "<body>" + Constants.vbCrLf;
        }

        private static string HTMLFooter()
        {
            return "</body></html>" + Constants.vbCrLf;
        }
    }
}