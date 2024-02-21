using System;
using System.IO;
using System.Security.Cryptography;

namespace InMotionGIT.Common.Helpers
{

    public class CryptAes
    {

        /// <summary>
        /// Encrypts the given string using the AES algorithm @128bits, a key and an initialization vector.
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="Key">Key used for the algorithm (stored in the config file)</param>
        /// <param name="IV">Initialization vector</param>
        /// <returns></returns>
        public static string EncryptingString(string plainText, string Key, ref string IV)
        {
            byte[] encrypted;
            using (var aesAlg = new AesManaged())
            {
                IV = Convert.ToBase64String(aesAlg.IV);
                aesAlg.Key = Convert.FromBase64String(Key);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts the given string using the AES algorithm @128bits, a key and an initialization vector.
        /// </summary>
        /// <param name="cipherText">Cipher text to be decrypted</param>
        /// <param name="Key">Key used for the algorithm (stored in the config file)</param>
        /// <param name="IV">Initialization vector</param>
        /// <returns></returns>
        public static string DecryptString(string cipherText, string Key, string IV)
        {
            string plaintext = null;
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = Convert.FromBase64String(Key);
                aesAlg.IV = Convert.FromBase64String(IV);
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

    }

}