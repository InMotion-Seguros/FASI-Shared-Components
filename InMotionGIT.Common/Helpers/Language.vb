Namespace Helpers

    Public NotInheritable Class Language

        Public Shared Function EnumLanguageToString(full As Boolean) As String
            Dim languagesList As Dictionary(Of Integer, String) = Helpers.Caching.GetItem("Languages")
            Dim result As String = String.Empty

            For Each languageItem As KeyValuePair(Of Integer, String) In languagesList
                If Not String.IsNullOrEmpty(result) Then
                    result += ","
                End If

                result += languageItem.Key.ToString
            Next

            Return result
        End Function

        'Public Shared Function EnumLanguageToDictionary(full As Boolean) As Dictionary(Of Integer, String)
        '    Return EnumLanguageToDictionary(EnumLanguage.English, full)
        'End Function

        Public Shared Function CultureNamesToEnumLanguage(codeLanguage As String) As Integer
            Dim result As Integer

            If codeLanguage.ToLower.StartsWith("es") Then
                result = 2

            ElseIf codeLanguage.ToLower.StartsWith("en") Then
                result = 1

            ElseIf codeLanguage.ToLower.StartsWith("pt") Then
                result = 3
            Else
                result = 0
            End If

            Return result
        End Function

        Public Shared Function DescriptionToEnumLanguage(description As String) As Integer

            Return DescriptionToEnumLanguage(description, 1)
        End Function

        Public Shared Function DescriptionToEnumLanguage(description As String, language As Integer) As Integer
            Dim result As Integer
            Dim languagesList As Dictionary(Of Integer, String) = Helpers.Caching.GetItem("Languages")

            For Each languageItem As KeyValuePair(Of Integer, String) In languagesList
                If String.Equals(languageItem.Value, description, StringComparison.CurrentCultureIgnoreCase) Then

                    result = languageItem.Key

                    Exit For
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' Extracts the name of the enum Cultural Language/Extrae el cultural name del enum de lenguaje
        ''' </summary>
        ''' <param name="language">Lenguaje/Language</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LanguageToCultureName(language As Integer) As String
            Dim result As String = String.Empty
            Select Case language
                Case 1
                    result = "EN-US"

                Case 2
                    result = "ES-CR"

                Case Else
                    result = "PT-BR"
            End Select

            Return result
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="CultureName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CultureNameToLanguage(CultureName As String) As Integer
            Dim result As Integer

            If CultureName.StartsWith("en", StringComparison.CurrentCultureIgnoreCase) Then
                result = 1
            ElseIf CultureName.StartsWith("es", StringComparison.CurrentCultureIgnoreCase) Then
                result = 2
            Else
                result = 3
            End If

            Return result
        End Function

    End Class

End Namespace