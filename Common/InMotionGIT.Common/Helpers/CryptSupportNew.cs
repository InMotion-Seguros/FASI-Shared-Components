using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using InMotionGIT.Common.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    /// <summary>
    /// Library that allows you to encrypt and decrypt using native libraries with parameters of the framework, not to use the size or the key, the Liberia uses the following AppSetting:
    /// 1. Security.Key: Defines the key to generate or decrypt the encrypted text.
    /// 2. Security.Size: Defines the size of the key used in the encrypted or decrypted
    /// / Libreria que permite encriptar y desencriptar con parámetros utilizando librerias nativas del framework, de no utilizar el size o el key, la liberia utiliza los siguientes AppSetting:
    /// 1. Security.Key: Define el clave para generar el texto encriptado o desencriptar.
    /// 2. Security.Size: Define el tamano de la clave a utilizar en el encriptado o desencripta
    /// </summary>
    /// <remarks></remarks>
    public class CryptSupportNew
    {

        /// <summary>
        /// Text encryption method / Método de encriptación de texto
        /// </summary>
        /// <param name="text">Text to encrypt/ Texto a encriptar</param>
        /// <param name="Password"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string EncryptString(string text, string Password = "")
        {

            return EncryptStringInternal(text);

        }

        /// <summary>
        /// Decryption method / Método de desencriptación
        /// </summary>
        /// <param name="sText">Text decrypt/Texto a desecriptar</param>
        /// <param name="Password"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DecryptString(string sText, string Password = "")
        {

            return DecryptStringInternal(sText);

        }

        /// <summary>
        /// Generates the key according to the requested size
        /// </summary>
        /// <param name="SecurityKeySize">key Size</param>
        /// <param name="SecurityKey">Text for Security Key</param>
        /// <returns>Key of Formated</returns>
        /// <remarks></remarks>
        private static byte[] KeyGenerate(int SecurityKeySize, string SecurityKey)
        {
            byte[] Result;
            switch (SecurityKeySize)
            {
                case 128:
                    {
                        byte[] aesKey = Encoding.UTF8.GetBytes(SecurityKey);
                        Result = SHA1.Create().ComputeHash(aesKey);
                        break;
                    }
                case 192:
                    {
                        byte[] aesKey = Encoding.UTF8.GetBytes(SecurityKey);
                        Result = MD5.Create().ComputeHash(aesKey);
                        break;
                    }
                case 256:
                    {
                        byte[] aesKey = Encoding.UTF8.GetBytes(SecurityKey);
                        Result = SHA256.Create().ComputeHash(aesKey);
                        break;
                    }

                default:
                    {
                        byte[] aesKey = Encoding.UTF8.GetBytes(SecurityKey);
                        Result = SHA1.Create().ComputeHash(aesKey);
                        break;
                    }
            }
            return Result;
        }

        /// <summary>
        /// Método de cifrado de texto con el parametro de 'textPlaint'/ Text encryption method with the parameter 'textPlaint'
        /// </summary>
        /// <param name="textPlaint">Text decrypt/Texto a desecriptar</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string EncryptStringInternal(string textPlaint)
        {
            string Result = string.Empty;
            string key = Conversions.ToString(Interaction.IIf("Security.Key".AppSettings().IsEmpty(), "FD109115ABDA0FCA1A623D1B016BA909", "Security.Key".AppSettings()));
            int size = Conversions.ToInteger(Interaction.IIf("Security.Size".AppSettings().IsEmpty(), 128, "Security.Size".AppSettings<int>()));
            Result = EncryptStringInternal(textPlaint, key, size);
            return Result;
        }

        /// <summary>
        /// Text encryption method with the parameter 'textPlaint', 'key' and 'size', Método de cifrado de texto con el parametro de 'textPlaint', 'key' y 'size'
        /// </summary>
        /// <param name="textPlaint">Text decrypt/Texto a desecriptar</param>
        /// <param name="key">Encryption key/Clave de encriptación </param>
        /// <param name="size">Encryption size/Tamaño de la clave</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string EncryptStringInternal(string textPlaint, string key, int size)
        {
            string Result = string.Empty;
            byte[] aesKey = KeyGenerate(size, key);
            using (var myAes = Aes.Create())
            {
                Result = EncryptStringToBytes_Aes(textPlaint, aesKey, size);
            }
            return Result;
        }

        /// <summary>
        /// Configurable Encryption Method / Método de cifrado configurable
        /// </summary>
        /// <param name="plainText">Text to encrypt/ Texto para cifrar</param>
        /// <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        /// <param name="Size">Key size/Tamaño de la clave</param>
        /// <returns>Encrypted Text</returns>
        /// <remarks></remarks>
        private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, int Size)
        {
            string Result = string.Empty;
            // Check arguments.
            if (plainText is null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key is null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = Size;
                aesAlg.BlockSize = 128;

                var keys = new Rfc2898DeriveBytes(Key, Key, 1000);
                aesAlg.Key = keys.GetBytes((int)Math.Round(aesAlg.KeySize / 8d));
                aesAlg.IV = keys.GetBytes((int)Math.Round(aesAlg.BlockSize / 8d));

                // Create a decrytor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {

                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            foreach (var Item in encrypted)
                Result = Result + string.Format("{0:X2}", Item);

            return Result;

        }

        /// <summary>
        /// Method that of 'DecryptStringInternal' with parameter 'textPlaint' / Metodo que realiza un 'DecryptStringInternal', con los parametro de 'textPlaint'
        /// </summary>
        /// <param name="textPlaint">Text to encrypted/Texto encriptado</param>
        /// <returns>De Encrypt Text</returns>
        /// <remarks></remarks>
        public static string DecryptStringInternal(string textPlaint)
        {
            string key = Conversions.ToString(Interaction.IIf("Security.Key".AppSettings().IsEmpty(), "FD109115ABDA0FCA1A623D1B016BA909", "Security.Key".AppSettings()));
            int size = Conversions.ToInteger(Interaction.IIf("Security.Size".AppSettings().IsEmpty(), 128, "Security.Size".AppSettings<int>()));
            return DecryptStringInternal(textPlaint, key, size);
        }

        /// <summary>
        /// Method that takes a throw of 'DecryptStringInternal' with parameters 'textPlaint', 'key' and 'size' / Metodo que realiza un 'DecryptStringInternal', con los parametros de 'textPlaint' , 'key' y 'size'
        /// </summary>
        /// <param name="textPlaint">Text to encrypted/Texto encriptado</param>
        /// <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        /// <param name="Size">Key size/Tamaño de la clave</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DecryptStringInternal(string textPlaint, string key, int size)
        {
            string Result = string.Empty;

            byte[] aesKey = KeyGenerate(size, key);

            var ListBinary = new List<byte>();
            for (int i = 0, loopTo = textPlaint.Length - 1; i <= loopTo; i++)
            {
                string bytesString = textPlaint.Substring(i, 2);
                ListBinary.Add(byte.Parse(bytesString, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                i = i + 1;
            }
            string Result2 = string.Empty;
            foreach (var Item in ListBinary.ToArray())
                Result2 = Result2 + string.Format("{0:X2}", Item);

            using (var myAes = Aes.Create())
            {
                Result = DecryptStringFromBytes_Aes(ListBinary.ToArray(), aesKey, size);
            }

            return Result.Trim();
        }

        /// <summary>
        /// Configurable De Encryption Method / Configurable De Método de cifrado
        /// </summary>
        /// <param name="cipherText">Text to encrypted</param>
        /// <param name="Key">Private Key Encryption/El cifrado de clave privada</param>
        /// <param name="Size">Key size/Tamaño de la clave</param>
        /// <returns>De Encrypt Text</returns>
        /// <remarks></remarks>
        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, int size)
        {
            // Check arguments.
            if (cipherText is null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key is null || key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = size;
                aesAlg.BlockSize = 128;

                var keys = new Rfc2898DeriveBytes(key, key, 1000);
                aesAlg.Key = keys.GetBytes((int)Math.Round(aesAlg.KeySize / 8d));
                aesAlg.IV = keys.GetBytes((int)Math.Round(aesAlg.BlockSize / 8d));
                // aesAlg.Padding = PaddingMode.Zeros

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {

                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {

                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;

        }

    }

}