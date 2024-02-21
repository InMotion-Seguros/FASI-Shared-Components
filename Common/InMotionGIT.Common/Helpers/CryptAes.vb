Imports System.IO
Imports System.Security.Cryptography

Namespace Helpers

    Public Class CryptAes

        ''' <summary>
        ''' Encrypts the given string using the AES algorithm @128bits, a key and an initialization vector.
        ''' </summary>
        ''' <param name="plainText">Text to be encrypted</param>
        ''' <param name="Key">Key used for the algorithm (stored in the config file)</param>
        ''' <param name="IV">Initialization vector</param>
        ''' <returns></returns>
        Public Shared Function EncryptingString(ByVal plainText As String, ByVal Key As String, ByRef IV As String) As String
            Dim encrypted() As Byte
            Using aesAlg As New AesManaged()
                IV = Convert.ToBase64String(aesAlg.IV)
                aesAlg.Key = Convert.FromBase64String(Key)

                Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
                Using msEncrypt As New MemoryStream()
                    Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                        Using swEncrypt As New StreamWriter(csEncrypt)
                            swEncrypt.Write(plainText)
                        End Using
                        encrypted = msEncrypt.ToArray()
                    End Using
                End Using
            End Using
            Return Convert.ToBase64String(encrypted)
        End Function

        ''' <summary>
        ''' Decrypts the given string using the AES algorithm @128bits, a key and an initialization vector.
        ''' </summary>
        ''' <param name="cipherText">Cipher text to be decrypted</param>
        ''' <param name="Key">Key used for the algorithm (stored in the config file)</param>
        ''' <param name="IV">Initialization vector</param>
        ''' <returns></returns>
        Public Shared Function DecryptString(ByVal cipherText As String, ByVal Key As String, ByVal IV As String) As String
            Dim plaintext As String = Nothing
            Using aesAlg As New AesManaged
                aesAlg.Key = Convert.FromBase64String(Key)
                aesAlg.IV = Convert.FromBase64String(IV)
                Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)
                Using msDecrypt As New MemoryStream(Convert.FromBase64String(cipherText))
                    Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                        Using srDecrypt As New StreamReader(csDecrypt)
                            plaintext = srDecrypt.ReadToEnd()
                        End Using
                    End Using
                End Using
            End Using
            Return plaintext
        End Function

    End Class

End Namespace