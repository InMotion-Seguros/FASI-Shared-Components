Imports System.Globalization
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Namespace Helpers

    ''' <summary>
    ''' Library that allows you to encrypt and decrypt using native libraries with parameters of the framework, not to use the size or the key, the Liberia uses the following AppSetting:
    '''  1. Security.Key: Defines the key to generate or decrypt the encrypted text.
    '''  2. Security.Size: Defines the size of the key used in the encrypted or decrypted
    ''' / Libreria que permite encriptar y desencriptar con parámetros utilizando librerias nativas del framework, de no utilizar el size o el key, la liberia utiliza los siguientes AppSetting:
    '''  1. Security.Key: Define el clave para generar el texto encriptado o desencriptar.
    '''  2. Security.Size: Define el tamano de la clave a utilizar en el encriptado o desencripta
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CryptSupportNew

        ''' <summary>
        ''' Text encryption method / Método de encriptación de texto
        ''' </summary>
        ''' <param name="text">Text to encrypt/ Texto a encriptar</param>
        ''' <param name="Password"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EncryptString(ByVal text As String, Optional ByVal Password As String = "") As String

            Return EncryptStringInternal(text)

        End Function

        ''' <summary>
        ''' Decryption method / Método de desencriptación
        ''' </summary>
        ''' <param name="sText">Text decrypt/Texto a desecriptar</param>
        ''' <param name="Password"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DecryptString(ByVal sText As String, Optional ByVal Password As String = "") As String

            Return DecryptStringInternal(sText)

        End Function

        ''' <summary>
        ''' Generates the key according to the requested size
        ''' </summary>
        ''' <param name="SecurityKeySize">key Size</param>
        ''' <param name="SecurityKey">Text for Security Key</param>
        ''' <returns>Key of Formated</returns>
        ''' <remarks></remarks>
        Private Shared Function KeyGenerate(SecurityKeySize As Integer, SecurityKey As String) As Byte()
            Dim Result As Byte()
            Select Case SecurityKeySize
                Case 128
                    Dim aesKey As Byte() = UTF8Encoding.UTF8.GetBytes(SecurityKey)
                    Result = SHA1.Create().ComputeHash(aesKey)
                Case 192
                    Dim aesKey As Byte() = UTF8Encoding.UTF8.GetBytes(SecurityKey)
                    Result = MD5.Create().ComputeHash(aesKey)
                Case 256
                    Dim aesKey As Byte() = UTF8Encoding.UTF8.GetBytes(SecurityKey)
                    Result = SHA256.Create().ComputeHash(aesKey)
                Case Else
                    Dim aesKey As Byte() = UTF8Encoding.UTF8.GetBytes(SecurityKey)
                    Result = SHA1.Create().ComputeHash(aesKey)
            End Select
            Return Result
        End Function

        ''' <summary>
        ''' Método de cifrado de texto con el parametro de 'textPlaint'/ Text encryption method with the parameter 'textPlaint'
        ''' </summary>
        ''' <param name="textPlaint">Text decrypt/Texto a desecriptar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EncryptStringInternal(textPlaint As String) As String
            Dim Result As String = String.Empty
            Dim key As String = IIf("Security.Key".AppSettings().IsEmpty(), "FD109115ABDA0FCA1A623D1B016BA909", "Security.Key".AppSettings)
            Dim size As Integer = IIf("Security.Size".AppSettings().IsEmpty(), 128, "Security.Size".AppSettings(Of Integer))
            Result = EncryptStringInternal(textPlaint, key, size)
            Return Result
        End Function

        ''' <summary>
        ''' Text encryption method with the parameter 'textPlaint', 'key' and 'size', Método de cifrado de texto con el parametro de 'textPlaint', 'key' y 'size'
        ''' </summary>
        ''' <param name="textPlaint">Text decrypt/Texto a desecriptar</param>
        ''' <param name="key">Encryption key/Clave de encriptación </param>
        ''' <param name="size">Encryption size/Tamaño de la clave</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EncryptStringInternal(textPlaint As String, key As String, size As Integer) As String
            Dim Result As String = String.Empty
            Dim aesKey As Byte() = KeyGenerate(size, key)
            Using myAes As Aes = Aes.Create()
                Result = EncryptStringToBytes_Aes(textPlaint, aesKey, size)
            End Using
            Return Result
        End Function

        ''' <summary>
        ''' Configurable Encryption Method / Método de cifrado configurable
        ''' </summary>
        ''' <param name="plainText">Text to encrypt/ Texto para cifrar</param>
        ''' <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        ''' <param name="Size">Key size/Tamaño de la clave</param>
        ''' <returns>Encrypted Text</returns>
        ''' <remarks></remarks>
        Private Shared Function EncryptStringToBytes_Aes(ByVal plainText As String, ByVal Key() As Byte, Size As Integer) As String
            Dim Result As String = String.Empty
            ' Check arguments.
            If plainText Is Nothing OrElse plainText.Length <= 0 Then
                Throw New ArgumentNullException("plainText")
            End If
            If Key Is Nothing OrElse Key.Length <= 0 Then
                Throw New ArgumentNullException("Key")
            End If
            Dim encrypted() As Byte
            ' Create an Aes object
            ' with the specified key and IV.
            Using aesAlg As Aes = Aes.Create()
                aesAlg.KeySize = Size
                aesAlg.BlockSize = 128

                Dim keys = New Rfc2898DeriveBytes(Key, Key, 1000)
                aesAlg.Key = keys.GetBytes(aesAlg.KeySize / 8)
                aesAlg.IV = keys.GetBytes(aesAlg.BlockSize / 8)

                ' Create a decrytor to perform the stream transform.
                Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)
                ' Create the streams used for encryption.
                Using msEncrypt As New MemoryStream()
                    Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                        Using swEncrypt As New StreamWriter(csEncrypt)

                            'Write all data to the stream.
                            swEncrypt.Write(plainText)
                        End Using
                        encrypted = msEncrypt.ToArray()
                    End Using
                End Using
            End Using
            For Each Item In encrypted
                Result = Result + String.Format("{0:X2}", Item)
            Next

            Return Result

        End Function

        ''' <summary>
        ''' Method that of 'DecryptStringInternal' with parameter 'textPlaint' / Metodo que realiza un 'DecryptStringInternal', con los parametro de 'textPlaint'
        ''' </summary>
        ''' <param name="textPlaint">Text to encrypted/Texto encriptado</param>
        ''' <returns>De Encrypt Text</returns>
        ''' <remarks></remarks>
        Public Shared Function DecryptStringInternal(textPlaint As String) As String
            Dim key As String = IIf("Security.Key".AppSettings().IsEmpty(), "FD109115ABDA0FCA1A623D1B016BA909", "Security.Key".AppSettings)
            Dim size As Integer = IIf("Security.Size".AppSettings().IsEmpty(), 128, "Security.Size".AppSettings(Of Integer))
            Return DecryptStringInternal(textPlaint, key, size)
        End Function

        ''' <summary>
        ''' Method that takes a throw of 'DecryptStringInternal' with parameters 'textPlaint', 'key' and 'size' / Metodo que realiza un 'DecryptStringInternal', con los parametros de 'textPlaint' , 'key' y 'size'
        ''' </summary>
        ''' <param name="textPlaint">Text to encrypted/Texto encriptado</param>
        ''' <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        ''' <param name="Size">Key size/Tamaño de la clave</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DecryptStringInternal(textPlaint As String, key As String, size As Integer) As String
            Dim Result As String = String.Empty

            Dim aesKey As Byte() = KeyGenerate(size, key)

            Dim ListBinary As New List(Of Byte)
            For i = 0 To textPlaint.Length - 1
                Dim bytesString = textPlaint.Substring(i, 2)
                ListBinary.Add(Byte.Parse(bytesString, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                i = i + 1
            Next
            Dim Result2 As String = String.Empty
            For Each Item In ListBinary.ToArray
                Result2 = Result2 + String.Format("{0:X2}", Item)
            Next

            Using myAes As Aes = Aes.Create()
                Result = DecryptStringFromBytes_Aes(ListBinary.ToArray, aesKey, size)
            End Using

            Return Result.Trim
        End Function

        ''' <summary>
        ''' Configurable De Encryption Method / Configurable De Método de cifrado
        ''' </summary>
        ''' <param name="cipherText">Text to encrypted</param>
        ''' <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        ''' <param name="Size">Key size/Tamaño de la clave</param>
        ''' <returns>De Encrypt Text</returns>
        ''' <remarks></remarks>
        Private Shared Function DecryptStringFromBytes_Aes(ByVal cipherText() As Byte, ByVal key() As Byte, size As Integer) As String
            ' Check arguments.
            If cipherText Is Nothing OrElse cipherText.Length <= 0 Then
                Throw New ArgumentNullException("cipherText")
            End If
            If key Is Nothing OrElse key.Length <= 0 Then
                Throw New ArgumentNullException("Key")
            End If

            ' Declare the string used to hold
            ' the decrypted text.
            Dim plaintext As String = Nothing

            ' Create an Aes object
            ' with the specified key and IV.
            Using aesAlg As Aes = Aes.Create()
                aesAlg.KeySize = size
                aesAlg.BlockSize = 128

                Dim keys = New Rfc2898DeriveBytes(key, key, 1000)
                aesAlg.Key = keys.GetBytes(aesAlg.KeySize / 8)
                aesAlg.IV = keys.GetBytes(aesAlg.BlockSize / 8)
                ' aesAlg.Padding = PaddingMode.Zeros

                ' Create a decrytor to perform the stream transform.
                Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)

                ' Create the streams used for decryption.
                Using msDecrypt As New MemoryStream(cipherText)

                    Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

                        Using srDecrypt As New StreamReader(csDecrypt)

                            ' Read the decrypted bytes from the decrypting stream
                            ' and place them in a string.
                            plaintext = srDecrypt.ReadToEnd()
                        End Using
                    End Using
                End Using
            End Using

            Return plaintext

        End Function

    End Class

End Namespace