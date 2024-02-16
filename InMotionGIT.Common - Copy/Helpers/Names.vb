Namespace Helpers

    Public Class Names

        Public Shared Function Standard(ByVal name As String, ByVal mode As Enumerations.EnumFriendlyMode) As String
            Dim result As String = name

            If (mode And Enumerations.EnumFriendlyMode.Plural) = Enumerations.EnumFriendlyMode.Plural Then
                result = Inflector.Pluralize(result) ' GetPluralName(result)
            End If

            If (mode And Enumerations.EnumFriendlyMode.Singular) = Enumerations.EnumFriendlyMode.Singular Then
                result = Inflector.Singularize(result) 'GetSingularName(result)
            End If

            If (mode And Enumerations.EnumFriendlyMode.ExpandEachUpperCase) = Enumerations.EnumFriendlyMode.ExpandEachUpperCase Then
                result = FixFriendlyName(result)

                result = result.Replace(" Of ", " of ")
                result = result.Replace(" Or ", " or ")
                result = result.Replace(" To ", " to ")
                result = result.Replace(" Per ", " per ")
                result = result.Replace(" In ", " in ")
                result = result.Replace(" And ", " and ")
                result = result.Replace(" For ", " for ")
                result = result.Replace(" When ", " when ")
                result = result.Replace(" By ", " by ")
                result = result.Replace(" Is ", " is ")

                result = result.Replace(" De ", " de ")
                result = result.Replace(" Un ", " un ")
                result = result.Replace(" Para ", " para ")
                result = result.Replace(" Del ", " del ")
                result = result.Replace(" O ", " o ")
                result = result.Replace(" En ", " en ")
                result = result.Replace(" Al ", " al ")
                result = result.Replace(" La ", " la ")
                result = result.Replace(" A ", " a ")
                result = result.Replace(" Y ", " y ")
                result = result.Replace(" No ", " no ")
                result = result.Replace(" Se ", " se ")
                result = result.Replace(" Por ", " por ")
            End If

            If (mode And Enumerations.EnumFriendlyMode.VisualBasicName) = Enumerations.EnumFriendlyMode.VisualBasicName Then
                result = VisualBasicName(result)
            End If

            If (mode And Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecialSingular) = Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecialSingular Then
                result = Inflector.Singularize(result)
                result = VisualBasicName(result).Replace("[", String.Empty).Replace("]", String.Empty)
            End If

            If (mode And Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecial) = Enumerations.EnumFriendlyMode.VisualBasicNameNotSpecial Then
                result = VisualBasicName(result).Replace("[", String.Empty).Replace("]", String.Empty)
            End If

            If (mode And Enumerations.EnumFriendlyMode.XMLEncode) = Enumerations.EnumFriendlyMode.XMLEncode Then
                result = Names.XmlEncode(result)
            End If

            If (mode And Enumerations.EnumFriendlyMode.PascalCased) = Enumerations.EnumFriendlyMode.PascalCased Then
                result = result.Substring(0, 1).ToLower & result.Substring(1)
            End If

            If (mode And Enumerations.EnumFriendlyMode.VisualBasicName) = Enumerations.EnumFriendlyMode.VisualBasicName Then
                result = VisualBasicName(result)

                If Not IsNothing(result) Then
                    result = result.Replace(vbCrLf, String.Empty)
                    result = result.Replace(vbCr, String.Empty)
                    result = result.Replace(vbLf, String.Empty)

                    result = result.Replace("á", "a")
                    result = result.Replace("Á", "A")

                    result = result.Replace("é", "e")
                    result = result.Replace("É", "E")

                    result = result.Replace("í", "i")
                    result = result.Replace("Í", "I")

                    result = result.Replace("ó", "o")
                    result = result.Replace("Ó", "O")

                    result = result.Replace("ú", "u")
                    result = result.Replace("Ú", "U")

                    result = result.Replace("ñ", String.Empty)
                    result = result.Replace("Ñ", String.Empty)

                    result = result.Replace("?", String.Empty)
                    result = result.Replace("¿", String.Empty)

                    result = result.Replace("(", String.Empty)
                    result = result.Replace(")", String.Empty)
                End If

            ElseIf Not IsNothing(result) Then
                result = result.Replace(vbCrLf, " ")
                result = result.Replace(vbCr, " ")
                result = result.Replace(vbLf, " ")
                result = result.Replace("  ", " ")
            End If

            If result.IsNotEmpty() Then
                result = result.Trim
            End If

            Return result
        End Function

        Public Shared Function FixFriendlyName(ByVal name As String) As String
            Dim returnValue As String = String.Empty
            If name.Trim.Length > 0 Then
                Dim process As Boolean = False

                returnValue = name.Substring(0, 1)
                For index As Integer = 1 To name.Length - 1
                    If IsDiferenUpperLower(name.Substring(index - 1, 1), name.Substring(index, 1)) And index > 1 Then
                        If returnValue.Substring(returnValue.Length - 2, 1) <> " " Then 'returnValue.Length > 2 AndAlso

                            'If returnValue.Substring(returnValue.Length - 1, 1) = returnValue.Substring(returnValue.Length - 1, 1).ToUpper Then
                            'returnValue = String.Format("{0} {1}", returnValue.Substring(0, returnValue.Length - 1), returnValue.Substring(returnValue.Length - 1, 1))
                            'Else
                            returnValue += " "
                            'End If

                        End If
                    End If
                    If IsNumeric(name.Substring(index, 1)) And Not IsNumeric(returnValue.Substring(returnValue.Length - 1, 1)) Then
                        returnValue += String.Format(" {0}", name.Substring(index, 1))
                    Else
                        returnValue += name.Substring(index, 1)
                    End If

                Next
                returnValue = returnValue.Replace("_", " ")
                returnValue = returnValue.Replace("  ", " ")
            End If
            Return returnValue
        End Function

        Private Shared Function IsDiferenUpperLower(ByVal left As String, ByVal rigth As String) As Boolean
            Dim leftIslower As Boolean = (left = left.ToLower)
            Dim rigthIslower As Boolean = (rigth = rigth.ToLower)

            Return (leftIslower Xor rigthIslower)
        End Function

        Public Shared Function VisualBasicName(ByVal name As String) As String
            name = name.Replace("[", String.Empty)
            name = name.Replace("]", String.Empty)

            Select Case name.ToLower
                Case "module", "alias", "property", "interface", "operator"
                    name = String.Format("[{0}]", name)
            End Select

            name = name.Replace(" ", String.Empty)
            name = name.Replace("-", String.Empty)
            name = name.Replace(".", String.Empty)
            name = name.Replace("/", String.Empty)
            name = name.Replace("\", String.Empty)
            name = name.Replace(":", String.Empty)
            name = name.Replace("#", String.Empty)

            Return name
        End Function

        Private Shared Function XmlEncode(ByVal strText As String) As String
            Dim aryChars() As Integer = New Integer() {38, 60, 62, 34, 61, 39}

            For i As Integer = 0 To UBound(aryChars)
                strText = Replace(strText, Chr(aryChars(i)), "&#" & aryChars(i) & ";")
            Next
            XmlEncode = strText

        End Function

    End Class

End Namespace