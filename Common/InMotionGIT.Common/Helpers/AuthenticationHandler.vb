Imports VB = Microsoft.VisualBasic

Namespace Helpers

    Public Class AuthenticationHandler

#Region "Random Password Methods"

        Public Shared Function CreateAdministratorPassword(requiredPasswordLength As Integer, nonAlphanumericCharacters As Integer) As String
            Dim newPassword As String = String.Empty
            Dim characters As String = String.Empty
            Dim passwordLength As Integer = requiredPasswordLength

            If nonAlphanumericCharacters > 0 Then
                characters = CreateRandomAlphanumericCharacters(nonAlphanumericCharacters)
                passwordLength = requiredPasswordLength - characters.Length
            End If

            For index As Integer = 0 To passwordLength - 1
                Select Case index
                    Case 0
                        newPassword += "a"
                    Case 1
                        newPassword += "d"
                    Case 2
                        newPassword += "m"
                    Case 3
                        newPassword += "i"
                    Case 4
                        newPassword += "n"
                    Case Else
                        newPassword += "1"
                End Select
            Next

            If characters.Length > 0 Then
                newPassword += characters
            End If

            Return newPassword
        End Function

        Public Shared Function CreateRandomPassword(requiredPasswordLength As Integer, nonAlphanumericCharacters As Integer) As String
            Dim newPassword As String = String.Empty
            Dim characters As String = String.Empty
            Dim passwordLength As Integer = requiredPasswordLength
            Const allowedChars As String = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789"

            If nonAlphanumericCharacters > 0 Then
                characters = CreateRandomAlphanumericCharacters(nonAlphanumericCharacters)
                passwordLength = requiredPasswordLength - characters.Length
            End If

            Dim newRandom As New Random()

            For index As Integer = 0 To passwordLength - 1
                newPassword += allowedChars(newRandom.[Next](0, allowedChars.Length))
            Next

            If characters.Length > 0 Then
                newPassword += characters
            End If

            Return newPassword
        End Function

        Public Shared Function IsCorrectPasswordBySettings(currentPassword As String, requiredPasswordLength As Integer, nonAlphanumericCharacters As Integer) As Boolean
            Dim result As Boolean = True

            If String.IsNullOrEmpty(currentPassword) OrElse currentPassword.Length < requiredPasswordLength Then
                result = False
            Else
                Dim countAlphanumericCharacters As Integer = 0

                For Each character As Char In currentPassword
                    Select Case character
                        Case "!", "@", "$", "?", "_", "-"
                            countAlphanumericCharacters += 1

                        Case Else
                    End Select
                Next

                If nonAlphanumericCharacters > countAlphanumericCharacters Then
                    result = False
                End If
            End If

            Return result
        End Function

        Private Shared Function CreateRandomAlphanumericCharacters(charactersLength As Integer) As String
            Const allowedChars As String = "!@$?_-"

            Dim chars As Char() = New Char(charactersLength - 1) {}
            Dim newRandom As New Random()

            For index As Integer = 0 To charactersLength - 1
                chars(index) = allowedChars(newRandom.[Next](0, allowedChars.Length))
            Next

            Return New String(chars)
        End Function

#End Region

        '**%StrEncode: Password encryption routine
        '%StrEncode: Rutina de encriptamiento de password
        Public Shared Function StrEncode(ByVal s As String) As String
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
            Dim result As String = String.Empty

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

        '**%StrDecode: Password de-encryptment routine
        '%StrDecode: Rutina de des-encriptamiento de password
        Public Shared Function StrDecode(ByVal s As String) As String
            Dim key As Integer
            Dim salt As Boolean
            Dim n As Integer
            Dim i As Integer
            Dim ss As String
            Dim k1 As Integer
            Dim k2 As Integer
            Dim k3 As Integer
            Dim k4 As Integer
            Dim result As String = String.Empty

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

                If salt Then result = Mid(ss, 3, Len(ss) - 4) Else result = ss
            End If
            Return result
        End Function

    End Class

End Namespace