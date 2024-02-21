Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the string data type
    ''' </summary>
    Public Module StringExtensions

        ''' <summary>
        '''  Extension permite el formate de un string
        ''' </summary>
        ''' <param name="value">The string value to check.</param>
        <Extension()>
        Public Function SpecialFormater(ByVal value As String, ParamArray args As Object()) As String
            Return String.Format(value, args)
        End Function

        ''' <summary>
        ''' Determines whether the specified string is null or empty.
        ''' </summary>
        ''' <param name="value">The string value to check.</param>
        <Extension()>
        Public Function IsEmpty(ByVal value As String) As Boolean
            Return IsNothing(value) OrElse
                   String.IsNullOrEmpty(value) OrElse
                   value.Length = 0
        End Function

        ''' <summary>
        ''' Determines whether the specified string is not null or empty.
        ''' </summary>
        ''' <param name="value">The string value to check.</param>
        <Extension()>
        Public Function IsNotEmpty(ByVal value As String) As Boolean
            Return Not value.IsEmpty
        End Function

        ''' <summary>
        ''' Checks whether the string is empty and returns a default value in case.
        ''' </summary>
        ''' <param name="value">The string to check.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <returns>Either the string or the default value.</returns>
        <Extension()>
        Public Function IfEmpty(ByVal value As String, defaultValue As String) As String
            If value.IsEmpty Then
                Return defaultValue
            Else
                Return value
            End If
        End Function

        <Extension()>
        Public Function ClosedList(ByVal value As String) As String

            Return ClosedList(value:=value, endString:=",")
        End Function

        <Extension()>
        Public Function ClosedList(ByVal value As String, endString As String) As String
            If value.EndsWith(endString) Then
                value = value.Substring(0, value.Length - endString.Length)
            End If

            Return value.Trim
        End Function

        <Extension()>
        Public Function Filter(ByVal value As String, RegularExpression As String) As String
            Return value.Filter(RegularExpression, False)
        End Function

        <Extension()>
        Public Function Filter(ByVal value As String, RegularExpression As String, Exclude As Boolean) As String
            Dim result As String = String.Empty
            Dim listMatch As New List(Of String)

            For Each match As Match In Regex.Matches(value, RegularExpression, RegexOptions.IgnoreCase)
                For Each ItemMach In match.Groups
                    listMatch.Add(ItemMach.Value)
                Next
            Next
            If Exclude Then
                result = value
                For Each ItemList As String In listMatch
                    result = result.Replace(ItemList, String.Empty)
                Next
            Else
                result = String.Empty
                result = String.Join(String.Empty, listMatch.ToArray())
            End If
            Return result
        End Function

        <Extension()>
        Public Function OnlyNumbers(ByVal value As String) As String
            Dim output As StringBuilder = New StringBuilder
            For i = 0 To value.Length - 1
                If value(i) = "." OrElse IsNumeric(value(i)) Then
                    output.Append(value(i))
                End If
            Next
            Return output.ToString()
        End Function

        ''' <summary>
        ''' Metodo de comprescion de string, esto ose utiliza en el string de serializacion de datatable por medio de json
        ''' </summary>
        ''' <param name="Text">Strign a comprimir</param>
        ''' <returns>String comprimido</returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function CompressString(ByVal Text As String) As String
            Dim buffer__1 As Byte() = Encoding.UTF8.GetBytes(Text)
            Dim memoryStream = New IO.MemoryStream()
            Using gZipStream = New GZipStream(memoryStream, CompressionMode.Compress, True)
                gZipStream.Write(buffer__1, 0, buffer__1.Length)
            End Using

            memoryStream.Position = 0

            Dim compressedData = New Byte(memoryStream.Length - 1) {}
            memoryStream.Read(compressedData, 0, compressedData.Length)

            Dim gZipBuffer = New Byte(compressedData.Length + 3) {}
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length)
            Buffer.BlockCopy(BitConverter.GetBytes(buffer__1.Length), 0, gZipBuffer, 0, 4)
            System.Diagnostics.Debug.WriteLine("Size original:" + buffer__1.Length.ToString)
            System.Diagnostics.Debug.WriteLine("Size Comopres:" + gZipBuffer.Length.ToString)
            Return Convert.ToBase64String(gZipBuffer)
        End Function

        ''' <summary>
        ''' Metodo de descomprecion de string
        ''' </summary>
        ''' <param name="Text">String a descomprimir</param>
        ''' <returns>Retorna el strign descomprimido</returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function DecompressString(ByVal Text As String) As String
            Dim gZipBuffer As Byte() = Convert.FromBase64String(Text)
            Using memoryStream = New IO.MemoryStream()
                Dim dataLength As Integer = BitConverter.ToInt32(gZipBuffer, 0)
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4)

                Dim buffer = New Byte(dataLength - 1) {}

                memoryStream.Position = 0
                Using gZipStream = New GZipStream(memoryStream, CompressionMode.Decompress)
                    gZipStream.Read(buffer, 0, buffer.Length)
                End Using

                Return Encoding.UTF8.GetString(buffer)
            End Using
        End Function

        <Extension()>
        Public Function Deserialize(ByVal objectString As String) As DataTable
            Dim descompresString As String = objectString.DecompressString()
            Return JsonConvert.DeserializeObject(Of DataTable)(descompresString, New DataTableConverter())
        End Function

        <Extension()>
        Public Function ReplaceIgnoreCase(ByVal originalString As String, ByVal oldValue As String, newValue As String) As String
            If originalString.IsEmpty Then
                originalString = String.Empty
            End If
            If oldValue.IsEmpty Then
                oldValue = String.Empty
            End If
            If newValue.IsEmpty Then
                newValue = String.Empty
            End If
            Dim startIndex As Integer

            Do While True
                startIndex = originalString.IndexOf(oldValue, startIndex, StringComparison.CurrentCultureIgnoreCase)
                If startIndex = -1 Then Exit Do
                originalString = originalString.Substring(0, startIndex) & newValue & originalString.Substring(startIndex + oldValue.Length)
                startIndex += newValue.Length
            Loop

            Return originalString
        End Function

        <Extension()>
        Public Function TagValue(value As String, startTag As String, endTag As String) As String
            Dim result As String = String.Empty
            Dim startIndex As Integer = value.IndexOf(startTag, StringComparison.CurrentCultureIgnoreCase)
            Dim endIndex As Integer = 0

            If startIndex > -1 Then
                result = value.Substring(startIndex + 1)
                endIndex = result.IndexOf(endTag, StringComparison.CurrentCultureIgnoreCase)
                If endIndex > -1 Then
                    If endIndex = 0 Then
                        result = String.Empty
                    Else
                        result = result.Substring(0, endIndex)
                    End If

                End If
            End If

            Return result
        End Function

        ''' <summary>
        ''' 'Nueva función MaskedFormat para crear la máscara de confidencialidad según nivel de esquema
        ''' </summary>
        ''' <param name="value">Valor a formatear</param>
        ''' <param name="displayRule"> Mostrar primeros o últimos carácteres</param>
        ''' <param name="numbersOfPositions">Número de carácteres a mostrar</param>
        ''' <returns>Valor con máscara</returns>
        <Extension()>
        Public Function MaskedFormat(value As String, displayRule As Integer, numbersOfPositions As Integer) As String
            If String.IsNullOrEmpty(value) Then
                value = String.Empty
            Else
                Dim maskedStringLenght As Integer = value.Trim.Length - numbersOfPositions
                Dim visibleString As String = String.Empty
                Dim maskedString As String = String.Empty

                Select Case displayRule
                    Case 0 'None
                        maskedString = value.Trim

                    Case 1 'First Characters
                        If maskedStringLenght < 0 Then
                            visibleString = value.Trim
                            maskedString = String.Empty
                        Else
                            visibleString = value.Trim.Substring(0, numbersOfPositions)
                            maskedString = value.Trim.Substring(numbersOfPositions)
                        End If

                    Case 2 'Last Characters
                        If maskedStringLenght < 0 Then
                            visibleString = value.Trim
                            maskedString = String.Empty
                        Else
                            visibleString = value.Trim.Substring(maskedStringLenght)
                            maskedString = value.Trim.Substring(0, maskedStringLenght)
                        End If
                End Select

                For Each character As Char In maskedString.Trim
                    maskedString = maskedString.Trim.Replace(character, "*")
                Next

                If displayRule = 2 Then 'Last Characters
                    value = maskedString & visibleString
                Else 'First Characters & None
                    value = visibleString & maskedString
                End If
            End If

            Return value
        End Function

    End Module

End Namespace