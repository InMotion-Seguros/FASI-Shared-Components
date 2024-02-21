using System;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Helpers
{

    public class AuthenticationHandler
    {

        #region Random Password Methods

        public static string CreateAdministratorPassword(int requiredPasswordLength, int nonAlphanumericCharacters)
        {
            string newPassword = string.Empty;
            string characters = string.Empty;
            int passwordLength = requiredPasswordLength;

            if (nonAlphanumericCharacters > 0)
            {
                characters = CreateRandomAlphanumericCharacters(nonAlphanumericCharacters);
                passwordLength = requiredPasswordLength - characters.Length;
            }

            for (int index = 0, loopTo = passwordLength - 1; index <= loopTo; index++)
            {
                switch (index)
                {
                    case 0:
                        {
                            newPassword += "a";
                            break;
                        }
                    case 1:
                        {
                            newPassword += "d";
                            break;
                        }
                    case 2:
                        {
                            newPassword += "m";
                            break;
                        }
                    case 3:
                        {
                            newPassword += "i";
                            break;
                        }
                    case 4:
                        {
                            newPassword += "n";
                            break;
                        }

                    default:
                        {
                            newPassword += "1";
                            break;
                        }
                }
            }

            if (characters.Length > 0)
            {
                newPassword += characters;
            }

            return newPassword;
        }

        public static string CreateRandomPassword(int requiredPasswordLength, int nonAlphanumericCharacters)
        {
            string newPassword = string.Empty;
            string characters = string.Empty;
            int passwordLength = requiredPasswordLength;
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            if (nonAlphanumericCharacters > 0)
            {
                characters = CreateRandomAlphanumericCharacters(nonAlphanumericCharacters);
                passwordLength = requiredPasswordLength - characters.Length;
            }

            var newRandom = new Random();

            for (int index = 0, loopTo = passwordLength - 1; index <= loopTo; index++)
                newPassword += Conversions.ToString(allowedChars[newRandom.Next(0, allowedChars.Length)]);

            if (characters.Length > 0)
            {
                newPassword += characters;
            }

            return newPassword;
        }

        public static bool IsCorrectPasswordBySettings(string currentPassword, int requiredPasswordLength, int nonAlphanumericCharacters)
        {
            bool result = true;

            if (string.IsNullOrEmpty(currentPassword) || currentPassword.Length < requiredPasswordLength)
            {
                result = false;
            }
            else
            {
                int countAlphanumericCharacters = 0;

                foreach (char character in currentPassword)
                {
                    switch (character)
                    {
                        case '!':
                        case '@':
                        case '$':
                        case '?':
                        case '_':
                        case '-':
                            {
                                countAlphanumericCharacters += 1;
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                if (nonAlphanumericCharacters > countAlphanumericCharacters)
                {
                    result = false;
                }
            }

            return result;
        }

        private static string CreateRandomAlphanumericCharacters(int charactersLength)
        {
            const string allowedChars = "!@$?_-";

            char[] chars = new char[charactersLength];
            var newRandom = new Random();

            for (int index = 0, loopTo = charactersLength - 1; index <= loopTo; index++)
                chars[index] = allowedChars[newRandom.Next(0, allowedChars.Length)];

            return new string(chars);
        }

        private static string _StrEncode_saltvalue = default;

        #endregion

        // **%StrEncode: Password encryption routine
        // %StrEncode: Rutina de encriptamiento de password
        public static string StrEncode(string s)
        {
            int key;
            bool salt;
            int n;
            int i;
            string ss;
            int k1;
            int k2;
            int k3;
            int k4;
            int t;
            string result = string.Empty;
            if (!string.IsNullOrEmpty(Strings.Trim(s)))
            {

                key = 1234567890;
                salt = false;

                if (salt)
                {
                    for (i = 1; i <= 4; i++)
                    {
                        t = (int)Math.Round((double)(100 * (1 + Strings.Asc(Strings.Mid(_StrEncode_saltvalue, i, 1))) * VBMath.Rnd()) * (DateAndTime.Timer + 1d));
                        var midTmp = Conversions.ToString(Strings.Chr(t % 256));
                        StringType.MidStmtStr(ref _StrEncode_saltvalue, i, 1, midTmp);
                    }
                    s = Strings.Mid(_StrEncode_saltvalue, 1, 2) + s + Strings.Mid(_StrEncode_saltvalue, 3, 2);
                }

                n = Strings.Len(s);
                ss = Strings.Space(n);
                var sn = new int[n + 1];

                k1 = 11 + key % 233;
                k2 = 7 + key % 239;
                k3 = 5 + key % 241;
                k4 = 3 + key % 251;

                var loopTo = n;
                for (i = 1; i <= loopTo; i++)
                    sn[i] = Strings.Asc(Strings.Mid(s, i, 1));

                var loopTo1 = n;
                for (i = 2; i <= loopTo1; i++)
                    sn[i] = sn[i] ^ sn[i - 1] ^ k1 * sn[i - 1] % 256;
                for (i = n - 1; i >= 1; i -= 1)
                    sn[i] = sn[i] ^ sn[i + 1] ^ k2 * sn[i + 1] % 256;
                var loopTo2 = n;
                for (i = 3; i <= loopTo2; i++)
                    sn[i] = sn[i] ^ sn[i - 2] ^ k3 * sn[i - 1] % 256;
                for (i = n - 2; i >= 1; i -= 1)
                    sn[i] = sn[i] ^ sn[i + 2] ^ k4 * sn[i + 1] % 256;

                var loopTo3 = n;
                for (i = 1; i <= loopTo3; i++)
                {
                    var midTmp1 = Conversions.ToString(Strings.Chr(sn[i]));
                    StringType.MidStmtStr(ref ss, i, 1, midTmp1);
                }
                result = ss;
            }
            return result;
        }

        // **%StrDecode: Password de-encryptment routine
        // %StrDecode: Rutina de des-encriptamiento de password
        public static string StrDecode(string s)
        {
            int key;
            bool salt;
            int n;
            int i;
            string ss;
            int k1;
            int k2;
            int k3;
            int k4;
            string result = string.Empty;

            if (!string.IsNullOrEmpty(Strings.Trim(s)))
            {

                key = 1234567890;
                salt = false;

                n = Strings.Len(s);
                ss = Strings.Space(n);
                var sn = new int[n + 1];

                k1 = 11 + key % 233;
                k2 = 7 + key % 239;
                k3 = 5 + key % 241;
                k4 = 3 + key % 251;

                var loopTo = n;
                for (i = 1; i <= loopTo; i++)
                    sn[i] = Strings.Asc(Strings.Mid(s, i, 1));

                var loopTo1 = n - 2;
                for (i = 1; i <= loopTo1; i++)
                    sn[i] = sn[i] ^ sn[i + 2] ^ k4 * sn[i + 1] % 256;
                for (i = n; i >= 3; i -= 1)
                    sn[i] = sn[i] ^ sn[i - 2] ^ k3 * sn[i - 1] % 256;
                var loopTo2 = n - 1;
                for (i = 1; i <= loopTo2; i++)
                    sn[i] = sn[i] ^ sn[i + 1] ^ k2 * sn[i + 1] % 256;
                for (i = n; i >= 2; i -= 1)
                    sn[i] = sn[i] ^ sn[i - 1] ^ k1 * sn[i - 1] % 256;

                var loopTo3 = n;
                for (i = 1; i <= loopTo3; i++)
                {
                    var midTmp = Conversions.ToString(Strings.Chr(sn[i]));
                    StringType.MidStmtStr(ref ss, i, 1, midTmp);
                }

                if (salt)
                    result = Strings.Mid(ss, 3, Strings.Len(ss) - 4);
                else
                    result = ss;
            }
            return result;
        }

    }

}