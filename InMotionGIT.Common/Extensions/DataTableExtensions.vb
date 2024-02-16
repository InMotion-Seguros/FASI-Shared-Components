Imports System.Runtime.CompilerServices
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the object data type
    ''' </summary>
    Public Module DataTableExtensions

        <Extension()>
        Public Function IsEmpty(ByVal value As DataTable) As Boolean
            Dim result As Boolean = True
            Return (Not IsNothing(value) AndAlso Not IsNothing(value.Rows) AndAlso value.Rows.Count > 0)
        End Function

        <Extension()>
        Public Function IsNotEmpty(ByVal value As DataTable) As Boolean
            Return Not IsNothing(value) AndAlso Not IsNothing(value.Rows) AndAlso value.Rows.Count > 0
        End Function

        <Extension()>
        Public Function FirstRow(ByVal value As DataTable) As DataRow
            Return value.Rows(0)
        End Function

        <Extension()>
        Public Function ReadOnlyMode(ByVal value As DataTable, Mode As Boolean)
            If Not IsNothing(value) AndAlso Not IsNothing(value.Columns) Then
                For Each ItemColumn As DataColumn In value.Columns
                    ItemColumn.ReadOnly = Mode
                Next
            End If
            Return value
        End Function

        ''' <summary>
        ''' Extensión de serialización por Newtonsoft.JSON, para datatable específicos.
        ''' </summary>
        ''' <param name="pObjectSerializator">Data Table a serializar</param>
        ''' <returns>datatable en formato json contenido en un string</returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function ToJSON(pObjectSerializator As DataTable) As String
            Return JsonConvert.SerializeObject(pObjectSerializator, New DataTableConverter())
        End Function

        <Extension()>
        Function ToCSV(dTable As DataTable, Optional columnList As String = "",
                                            Optional withHeader As Boolean = True,
                                            Optional headerQuote As Boolean = False,
                                            Optional stringQuote As Boolean = True,
                                            Optional rowSeparator As String = vbNewLine,
                                            Optional fieldSeparator As String = ",",
                                            Optional dateFormat As String = "yyyy/MM/dd HH:mm:ss") As String
            'As Byte()
            Dim sb As StringBuilder = New StringBuilder()
            Dim columnCount As Integer = 0
            Dim index As Integer = 0

            If columnList.IsEmpty Then
                For Each colItem As Data.DataColumn In dTable.Columns
                    columnList &= "," & colItem.ColumnName
                Next
                columnList = columnList.Substring(1)
            End If
            columnCount = columnList.Split(",").Length - 1

            If withHeader Then
                For Each columName As String In columnList.Split(",")
                    columName = columName.Trim
                    If headerQuote Then
                        sb.Append("""")
                    End If
                    sb.Append(dTable.Columns(columName).ColumnName.ToString())
                    If headerQuote Then
                        sb.Append("""")
                    End If
                    If index < columnCount Then
                        sb.Append(fieldSeparator)
                    End If
                    index += 1
                Next
                sb.Append(rowSeparator)
            End If

            For Each row As DataRow In dTable.Rows
                index = 0
                For Each columName As String In columnList.Split(",")
                    columName = columName.Trim
                    If dTable.Columns(columName).DataType.FullName = "System.String" Then
                        If stringQuote Then
                            sb.Append("""")
                        End If
                        sb.Append(row(columName).ToString().Replace("""", """"""))
                        If stringQuote Then
                            sb.Append("""")
                        End If
                    ElseIf dTable.Columns(columName).DataType.FullName = "System.DateTime" Then
                        If Not IsDBNull(row(columName)) AndAlso row(columName) <> DateTime.MinValue Then
                            sb.Append(DirectCast(row(columName), System.DateTime).ToString(dateFormat))
                        End If
                    Else
                        sb.Append(row(columName).ToString().Replace(".", "."))
                    End If

                    If index < columnCount Then
                        sb.Append(fieldSeparator)
                    End If
                    index += 1
                Next
                sb.Append(rowSeparator)
            Next

            'Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            Return sb.ToString
        End Function

        <Extension()>
        Function ToXLS(dTable As DataTable, Optional columnList As String = "",
                                            Optional withHeader As Boolean = True,
                                            Optional dateFormat As String = "yyyy/MM/dd HH:mm:ss") As String
            'As Byte()
            Dim sb As StringBuilder = New StringBuilder()
            Dim columnCount As Integer = 0
            Dim index As Integer = 0

            If columnList.IsEmpty Then
                For Each colItem As Data.DataColumn In dTable.Columns
                    columnList &= "," & colItem.ColumnName
                Next
                columnList = columnList.Substring(1)
            End If
            columnCount = columnList.Split(",").Length - 1

            sb.Append(ExcelHeader())

            sb.Append("<table>")
            If withHeader Then
                sb.Append("<thead><tr>")
                For Each columName As String In columnList.Split(",")
                    columName = columName.Trim
                    sb.Append("<th>" & dTable.Columns(columName).ColumnName & "</th>")
                Next
                sb.Append("</tr></thead>")
            End If

            sb.Append("<tbody>")
            For Each row As DataRow In dTable.Rows
                sb.Append("<tr>")
                For Each columName As String In columnList.Split(",")
                    sb.Append("<td>")
                    columName = columName.Trim
                    If dTable.Columns(columName).DataType.FullName = "System.String" Then
                        sb.Append(row(columName).ToString())
                    ElseIf dTable.Columns(columName).DataType.FullName = "System.DateTime" Then
                        If Not IsDBNull(row(columName)) AndAlso row(columName) <> DateTime.MinValue Then
                            sb.Append(DirectCast(row(columName), System.DateTime).ToString(dateFormat))
                        End If
                    Else
                        sb.Append(row(columName).ToString().Replace(".", "."))
                    End If
                    sb.Append("</td>")
                Next

                sb.Append("</tr>")
            Next
            sb.Append("</tbody>")
            sb.Append("</table>")
            sb.Append(ExcelFooter())
            'Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            Return sb.ToString
        End Function

        <Extension()>
        Function ToHTML(dTable As DataTable, Optional columnList As String = "",
                                             Optional withHeader As Boolean = True,
                                             Optional dateFormat As String = "yyyy/MM/dd HH:mm:ss") As String
            'As Byte()
            Dim sb As StringBuilder = New StringBuilder()
            Dim columnCount As Integer = 0
            Dim index As Integer = 0

            If columnList.IsEmpty Then
                For Each colItem As Data.DataColumn In dTable.Columns
                    columnList &= "," & colItem.ColumnName
                Next
                columnList = columnList.Substring(1)
            End If
            columnCount = columnList.Split(",").Length - 1

            sb.Append(HTMLHeader())

            sb.Append("<table>")
            If withHeader Then
                sb.Append("<thead><tr>")
                For Each columName As String In columnList.Split(",")
                    columName = columName.Trim
                    sb.Append("<th>" & dTable.Columns(columName).ColumnName & "</th>")
                Next
                sb.Append("</tr></thead>")
            End If

            sb.Append("<tbody>")
            For Each row As DataRow In dTable.Rows
                sb.Append("<tr>")
                For Each columName As String In columnList.Split(",")
                    sb.Append("<td>")
                    columName = columName.Trim
                    If dTable.Columns(columName).DataType.FullName = "System.String" Then
                        sb.Append(row(columName).ToString())
                    ElseIf dTable.Columns(columName).DataType.FullName = "System.DateTime" Then
                        If Not IsDBNull(row(columName)) AndAlso row(columName) <> DateTime.MinValue Then
                            sb.Append(DirectCast(row(columName), System.DateTime).ToString(dateFormat))
                        End If
                    Else
                        sb.Append(row(columName).ToString().Replace(".", "."))
                    End If
                    sb.Append("</td>")
                Next

                sb.Append("</tr>")
            Next
            sb.Append("</tbody>")
            sb.Append("</table>")
            sb.Append(HTMLFooter())
            'Return System.Text.Encoding.UTF8.GetBytes(sb.ToString)
            Return sb.ToString
        End Function

        Private Function ExcelHeader() As String
            Return "<html xmlns:o=""urn:schemas-microsoft-com:office:office""" & vbCrLf &
                   "xmlns:x=""urn:schemas-microsoft-com:office:excel""" & vbCrLf &
                   "xmlns=""http://www.w3.org/TR/REC-html40"">" & vbCrLf &
                   "" & vbCrLf &
                   "<head>" & vbCrLf &
                   "<meta http-equiv=Content-Type content=""text/html; charset=windows-1252"">" & vbCrLf &
                   "<meta name=ProgId content=Excel.Sheet>" & vbCrLf &
                   "<meta name=Generator content=""Microsoft Excel 11"">" & vbCrLf &
                   "" & vbCrLf &
                   "<!--[if gte mso 9]><xml>" & vbCrLf &
                   " <x:ExcelWorkbook>" & vbCrLf &
                   "  <x:ExcelWorksheets>" & vbCrLf &
                   "   <x:ExcelWorksheet>" & vbCrLf &
                   "    <x:Name>Worksheet Name</x:Name>" & vbCrLf &
                   "    <x:WorksheetOptions>" & vbCrLf &
                   "     <x:Selected/>" & vbCrLf &
                   "     <x:FreezePanes/>" & vbCrLf &
                   "     <x:FrozenNoSplit/>" & vbCrLf &
                   "     <x:SplitHorizontal>1</x:SplitHorizontal>" & vbCrLf &
                   "     <x:TopRowBottomPane>1</x:TopRowBottomPane>" & vbCrLf &
                   "     <x:SplitVertical>1</x:SplitVertical>" & vbCrLf &
                   "     <x:LeftColumnRightPane>1</x:LeftColumnRightPane>" & vbCrLf &
                   "     <x:ActivePane>0</x:ActivePane>" & vbCrLf &
                   "     <x:Panes>" & vbCrLf &
                   "      <x:Pane>" & vbCrLf &
                   "       <x:Number>3</x:Number>" & vbCrLf &
                   "      </x:Pane>" & vbCrLf &
                   "      <x:Pane>" & vbCrLf &
                   "       <x:Number>1</x:Number>" & vbCrLf &
                   "      </x:Pane>" & vbCrLf &
                   "      <x:Pane>" & vbCrLf &
                   "       <x:Number>2</x:Number>" & vbCrLf &
                   "      </x:Pane>" & vbCrLf &
                   "      <x:Pane>" & vbCrLf &
                   "       <x:Number>0</x:Number>" & vbCrLf &
                   "      </x:Pane>" & vbCrLf &
                   "     </x:Panes>" & vbCrLf &
                   "     <x:ProtectContents>False</x:ProtectContents>" & vbCrLf &
                   "     <x:ProtectObjects>False</x:ProtectObjects>" & vbCrLf &
                   "     <x:ProtectScenarios>False</x:ProtectScenarios>" & vbCrLf &
                   "    </x:WorksheetOptions>" & vbCrLf &
                   "   </x:ExcelWorksheet>" & vbCrLf &
                   "  </x:ExcelWorksheets>" & vbCrLf &
                   "  <x:ProtectStructure>False</x:ProtectStructure>" & vbCrLf &
                   "  <x:ProtectWindows>False</x:ProtectWindows>" & vbCrLf &
                   " </x:ExcelWorkbook>" & vbCrLf &
                   "</xml><![endif]-->" & vbCrLf &
                   "" & vbCrLf &
                   "</head>" & vbCrLf &
                   "<body>" & vbCrLf
        End Function

        Private Function ExcelFooter() As String
            Return "</body>" & vbCrLf &
                   "</html>" & vbCrLf
        End Function

        Private Function HTMLHeader() As String
            Return "<!DOCTYPE html>" & vbCrLf &
                    "<html>" & vbCrLf &
                    "<head>" & vbCrLf &
                    "<title></title>" & vbCrLf &
                    "<style>" & vbCrLf &
                    "table { border-collapse: collapse;}" & vbCrLf &
                    "th, td { padding: 8px;text-align: left;border-bottom: 1px solid #ddd; }" & vbCrLf &
                    "tr:hover {background-color: #f5f5f5}" & vbCrLf &
                    "</style>" & vbCrLf &
                    "</head>" & vbCrLf &
                    "<body>" & vbCrLf
        End Function

        Private Function HTMLFooter() As String
            Return "</body></html>" & vbCrLf
        End Function

    End Module

End Namespace