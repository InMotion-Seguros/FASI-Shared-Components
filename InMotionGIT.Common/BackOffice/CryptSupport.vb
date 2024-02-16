Imports VB = Microsoft.VisualBasic

Namespace BackOffice

    Public Class CryptSupport

        Public Shared Function EncryptString(ByVal sText As String, Optional ByVal Password As String = "") As String
            EncryptString = HexEncryptString(sText)
        End Function

        Public Shared Function DecryptString(ByVal sText As String, Optional ByVal Password As String = "") As String
            DecryptString = HexDecryptString(sText)
        End Function

        Public Shared Function ASCIIEncryptString(ByVal strS As String) As String
            Dim rnd As New Random
            Dim Key As Integer
            Dim salt As Boolean
            Dim n As Integer
            Dim lngI As Integer
            Dim ss As String
            Dim k1 As Integer
            Dim k2 As Integer
            Dim k3 As Integer
            Dim k4 As Integer
            Dim t As Integer

            ASCIIEncryptString = String.Empty

            Static saltvalue As String

            If Trim(strS) <> String.Empty Then

                Key = 1234567890
                salt = False

                If salt Then
                    For lngI = 1 To 4
                        t = 100 * (1 + Asc(Mid(saltvalue, lngI, 1))) * rnd.Next(1, Integer.MaxValue)
                        Mid(saltvalue, lngI, 1) = Chr(t Mod 256)
                    Next
                    strS = Mid(saltvalue, 1, 2) & strS & Mid(saltvalue, 3, 2)
                End If

                n = Len(strS)
                ss = Space(n)
                Dim sn(n) As Integer

                k1 = 11 + (Key Mod 233)
                k2 = 7 + (Key Mod 239)
                k3 = 5 + (Key Mod 241)
                k4 = 3 + (Key Mod 251)

                For lngI = 1 To n
                    sn(lngI) = Asc(Mid(strS, lngI, 1))
                Next lngI
                For lngI = 2 To n
                    sn(lngI) = sn(lngI) Xor sn(lngI - 1) Xor ((k1 * sn(lngI - 1)) Mod 256)
                Next lngI
                For lngI = n - 1 To 1 Step -1
                    sn(lngI) = sn(lngI) Xor sn(lngI + 1) Xor (k2 * sn(lngI + 1)) Mod 256
                Next lngI
                For lngI = 3 To n
                    sn(lngI) = sn(lngI) Xor sn(lngI - 2) Xor (k3 * sn(lngI - 1)) Mod 256
                Next lngI
                For lngI = n - 2 To 1 Step -1
                    sn(lngI) = sn(lngI) Xor sn(lngI + 2) Xor (k4 * sn(lngI + 1)) Mod 256
                Next lngI
                For lngI = 1 To n
                    Mid(ss, lngI, 1) = Chr(sn(lngI))
                Next lngI
                ASCIIEncryptString = ss
            End If
        End Function

        Public Shared Function ASCIIDecryptString(ByVal strS As String) As String
            Dim Key As Integer
            Dim salt As Boolean
            Dim n As Integer
            Dim lngI As Integer
            Dim ss As String
            Dim k1 As Integer
            Dim k2 As Integer
            Dim k3 As Integer
            Dim k4 As Integer

            ASCIIDecryptString = String.Empty
            If Trim(strS) <> String.Empty Then

                Key = 1234567890
                salt = False

                n = Len(strS)
                ss = Space(n)
                Dim sn(n) As Integer

                k1 = 11 + (Key Mod 233)
                k2 = 7 + (Key Mod 239)
                k3 = 5 + (Key Mod 241)
                k4 = 3 + (Key Mod 251)

                For lngI = 1 To n
                    sn(lngI) = Asc(Mid(strS, lngI, 1))
                Next lngI

                For lngI = 1 To n - 2
                    sn(lngI) = sn(lngI) Xor sn(lngI + 2) Xor (k4 * sn(lngI + 1)) Mod 256
                Next lngI
                For lngI = n To 3 Step -1
                    sn(lngI) = sn(lngI) Xor sn(lngI - 2) Xor (k3 * sn(lngI - 1)) Mod 256
                Next lngI
                For lngI = 1 To n - 1
                    sn(lngI) = sn(lngI) Xor sn(lngI + 1) Xor (k2 * sn(lngI + 1)) Mod 256
                Next lngI
                For lngI = n To 2 Step -1
                    sn(lngI) = sn(lngI) Xor sn(lngI - 1) Xor (k1 * sn(lngI - 1)) Mod 256
                Next lngI

                For lngI = 1 To n
                    Mid(ss, lngI, 1) = Chr(sn(lngI))
                Next lngI

                If salt Then
                    ASCIIDecryptString = Mid(ss, 3, Len(ss) - 4)
                Else
                    ASCIIDecryptString = ss
                End If
            End If
        End Function

        Public Shared Function HexEncryptString(ByRef Text As String) As String
            Dim strBuffer As String = String.Empty
            Dim strOutput As String
            Dim intIndex As Short
            Dim intCount As Short

            strBuffer = ASCIIEncryptString(Text)
            intCount = Len(strBuffer)
            strOutput = String.Empty
            For intIndex = 1 To intCount
                strOutput = strOutput & Right("00" & Hex(Asc(Mid(strBuffer, intIndex, 1))), 2)
            Next
            HexEncryptString = strOutput
        End Function

        Public Shared Function HexDecryptString(ByRef Text As String) As String
            Dim strOutput As String
            Dim intIndex As Short
            Dim intCount As Short

            intCount = Len(Text)
            strOutput = String.Empty
            For intIndex = 1 To intCount Step 2
                strOutput = strOutput & Chr(CInt(Hex2Int(Mid(Text, intIndex, 2))))
            Next
            HexDecryptString = ASCIIDecryptString(strOutput)
        End Function

        Private Shared Function Hex2Int(ByVal sHex As String) As Short
            Dim result As Short = 0
            Dim Tmp As String
            Dim lo1 As Short
            Dim lo2 As Short
            Dim hi1 As Integer
            Dim hi2 As Integer

            Const Hx As String = "&H"
            Const BigShift As Integer = 65536
            Const LilShift As Short = 256
            Const Two As Short = 2

            Tmp = sHex
            If UCase(Left(sHex, 2)) = "&H" Then Tmp = Mid(sHex, 3)
            Tmp = Right("0000000" & Tmp, 8)
            If IsNumeric(Hx & Tmp) Then
                lo1 = CShort(Hx & Right(Tmp, Two))
                hi1 = CInt(Hx & Mid(Tmp, 5, Two))
                lo2 = CShort(Hx & Mid(Tmp, 3, Two))
                hi2 = CInt(Hx & Left(Tmp, Two))
                result = CDec(hi2 * LilShift + lo2) * BigShift + (hi1 * LilShift) + lo1
            End If
            Return result
        End Function

        Public Shared Function StrEncode(ByVal s As String) As String
            Dim result As String = String.Empty
            Dim key As Integer
            Dim salt As Boolean
            Dim n As Integer
            Dim i As Integer
            Dim ss As String
            Dim k1 As Integer
            Dim k2 As Integer
            Dim k3 As Integer
            Dim k4 As Integer
            Dim t As Integer

            Static saltvalue As String
            If Trim(s) <> String.Empty Then

                key = 1234567890
                salt = False

                If salt Then
                    For i = 1 To 4
                        t = 100 * (1 + Asc(Mid(saltvalue, i, 1))) * Rnd() * (VB.Timer() + 1)
                        Mid(saltvalue, i, 1) = Chr(t Mod 256)
                    Next
                    s = Mid(saltvalue, 1, 2) & s & Mid(saltvalue, 3, 2)
                End If

                n = Len(s)
                ss = Space(n)
                Dim sn(n) As Integer

                k1 = 11 + (key Mod 233) : k2 = 7 + (key Mod 239)
                k3 = 5 + (key Mod 241) : k4 = 3 + (key Mod 251)

                For i = 1 To n : sn(i) = Asc(Mid(s, i, 1)) : Next

                For i = 2 To n : sn(i) = sn(i) Xor sn(i - 1) Xor ((k1 * sn(i - 1)) Mod 256) : Next
                For i = n - 1 To 1 Step -1 : sn(i) = sn(i) Xor sn(i + 1) Xor (k2 * sn(i + 1)) Mod 256 : Next
                For i = 3 To n : sn(i) = sn(i) Xor sn(i - 2) Xor (k3 * sn(i - 1)) Mod 256 : Next
                For i = n - 2 To 1 Step -1 : sn(i) = sn(i) Xor sn(i + 2) Xor (k4 * sn(i + 1)) Mod 256 : Next

                For i = 1 To n : Mid(ss, i, 1) = Chr(sn(i)) : Next
                result = ss
            End If
            Return result
        End Function

        Public Shared Function StrDecode(ByVal s As String) As String
            Dim result As String = String.Empty
            Dim key As Integer
            Dim salt As Boolean
            Dim n As Integer
            Dim i As Integer
            Dim ss As String
            Dim k1 As Integer
            Dim k2 As Integer
            Dim k3 As Integer
            Dim k4 As Integer

            If Trim(s) <> String.Empty Then

                key = 1234567890
                salt = False

                n = Len(s)
                ss = Space(n)
                Dim sn(n) As Integer

                k1 = 11 + (key Mod 233) : k2 = 7 + (key Mod 239)
                k3 = 5 + (key Mod 241) : k4 = 3 + (key Mod 251)

                For i = 1 To n : sn(i) = Asc(Mid(s, i, 1)) : Next

                For i = 1 To n - 2 : sn(i) = sn(i) Xor sn(i + 2) Xor (k4 * sn(i + 1)) Mod 256 : Next
                For i = n To 3 Step -1 : sn(i) = sn(i) Xor sn(i - 2) Xor (k3 * sn(i - 1)) Mod 256 : Next
                For i = 1 To n - 1 : sn(i) = sn(i) Xor sn(i + 1) Xor (k2 * sn(i + 1)) Mod 256 : Next
                For i = n To 2 Step -1 : sn(i) = sn(i) Xor sn(i - 1) Xor (k1 * sn(i - 1)) Mod 256 : Next

                For i = 1 To n : Mid(ss, i, 1) = Chr(sn(i)) : Next i

                If salt Then
                    result = Mid(ss, 3, Len(ss) - 4)
                Else
                    result = ss

                End If
            End If
            Return result
        End Function

    End Class

End Namespace