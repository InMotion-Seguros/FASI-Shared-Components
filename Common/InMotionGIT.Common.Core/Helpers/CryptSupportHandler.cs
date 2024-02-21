using InMotionGIT.Common.Core.Extensions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace InMotionGIT.Common.Core.Helpers;

public class CryptSupport
{
    // **+Objective:
    // **+Version: $$Revision: $
    // +Objetivo:
    // +Version: $$Revision: $

    // **%Objective:
    // **%Parameters:
    // **%    sText    -
    // **%    Password -
    // %Objetivo:
    // %Parámetros:
    // %      sText    -
    // %      Password -
    public static string EncryptString(string sText, string Password = "")
    {
        string EncryptStringRet = default;
        // 'On Error GoTo ErrorHandler

        EncryptStringRet = HexEncryptString(ref sText);

        return EncryptStringRet;
    }

    // **%Objective:
    // **%Parameters:
    // **%    sText    -
    // **%    Password -
    // %Objetivo:
    // %Parámetros:
    // %      sText    -
    // %      Password -
    public static string DecryptString(string sText, string Password = "")
    {
        string DecryptStringRet = default;
        // 'On Error GoTo ErrorHandler

        DecryptStringRet = HexDecryptString(ref sText);

        return DecryptStringRet;
    }

    private static string _ASCIIEncryptString_saltvalue = default;

    // **%Objective: Password encryption routine
    // **%Parameters:
    // **%    s -
    // %Objetivo: Rutina de encriptamiento de password
    // %Parámetros:
    // %      s -
    public static string ASCIIEncryptString(string strS)
    {
        string ASCIIEncryptStringRet = default;
        var rnd = new Random();
        int Key;
        bool salt;
        int n;
        int lngI;
        string ss;
        int k1;
        int k2;
        int k3;
        int k4;
        int t;

        ASCIIEncryptStringRet = string.Empty;

        if (!string.IsNullOrEmpty(Strings.Trim(strS)))
        {
            Key = 1234567890;
            salt = false;

            if (salt)
            {
                for (lngI = 1; lngI <= 4; lngI++)
                {
                    t = 100 * (1 + Strings.Asc(Strings.Mid(_ASCIIEncryptString_saltvalue, lngI, 1))) * rnd.Next(1, int.MaxValue);
                    var midTmp = Conversions.ToString(Strings.Chr(t % 256));
                    StringType.MidStmtStr(ref _ASCIIEncryptString_saltvalue, lngI, 1, midTmp);
                }
                strS = Strings.Mid(_ASCIIEncryptString_saltvalue, 1, 2) + strS + Strings.Mid(_ASCIIEncryptString_saltvalue, 3, 2);
            }

            n = Strings.Len(strS);
            ss = Strings.Space(n);
            var sn = new int[n + 1];

            k1 = 11 + Key % 233;
            k2 = 7 + Key % 239;
            k3 = 5 + Key % 241;
            k4 = 3 + Key % 251;

            var loopTo = n;
            for (lngI = 1; lngI <= loopTo; lngI++)
                sn[lngI] = Strings.Asc(Strings.Mid(strS, lngI, 1));
            var loopTo1 = n;
            for (lngI = 2; lngI <= loopTo1; lngI++)
                sn[lngI] = sn[lngI] ^ sn[lngI - 1] ^ k1 * sn[lngI - 1] % 256;
            for (lngI = n - 1; lngI >= 1; lngI -= 1)
                sn[lngI] = sn[lngI] ^ sn[lngI + 1] ^ k2 * sn[lngI + 1] % 256;
            var loopTo2 = n;
            for (lngI = 3; lngI <= loopTo2; lngI++)
                sn[lngI] = sn[lngI] ^ sn[lngI - 2] ^ k3 * sn[lngI - 1] % 256;
            for (lngI = n - 2; lngI >= 1; lngI -= 1)
                sn[lngI] = sn[lngI] ^ sn[lngI + 2] ^ k4 * sn[lngI + 1] % 256;
            var loopTo3 = n;
            for (lngI = 1; lngI <= loopTo3; lngI++)
            {
                var midTmp1 = Conversions.ToString(Strings.Chr(sn[lngI]));
                StringType.MidStmtStr(ref ss, lngI, 1, midTmp1);
            }
            ASCIIEncryptStringRet = ss;
        }

        return ASCIIEncryptStringRet;
    }

    // **%Objective: Password de-encryptment routine
    // **%Parameters:
    // **%    s -
    // %Objetivo: Rutina de des-encriptamiento de password
    // %Parámetros:
    // %      s -
    public static string ASCIIDecryptString(string strS)
    {
        string ASCIIDecryptStringRet = default;
        int Key;
        bool salt;
        int n;
        int lngI;
        string ss;
        int k1;
        int k2;
        int k3;
        int k4;

        ASCIIDecryptStringRet = string.Empty;
        if (!string.IsNullOrEmpty(Strings.Trim(strS)))
        {
            Key = 1234567890;
            salt = false;

            n = Strings.Len(strS);
            ss = Strings.Space(n);
            var sn = new int[n + 1];

            k1 = 11 + Key % 233;
            k2 = 7 + Key % 239;
            k3 = 5 + Key % 241;
            k4 = 3 + Key % 251;

            var loopTo = n;
            for (lngI = 1; lngI <= loopTo; lngI++)
                sn[lngI] = Strings.Asc(Strings.Mid(strS, lngI, 1));

            var loopTo1 = n - 2;
            for (lngI = 1; lngI <= loopTo1; lngI++)
                sn[lngI] = sn[lngI] ^ sn[lngI + 2] ^ k4 * sn[lngI + 1] % 256;
            for (lngI = n; lngI >= 3; lngI -= 1)
                sn[lngI] = sn[lngI] ^ sn[lngI - 2] ^ k3 * sn[lngI - 1] % 256;
            var loopTo2 = n - 1;
            for (lngI = 1; lngI <= loopTo2; lngI++)
                sn[lngI] = sn[lngI] ^ sn[lngI + 1] ^ k2 * sn[lngI + 1] % 256;
            for (lngI = n; lngI >= 2; lngI -= 1)
                sn[lngI] = sn[lngI] ^ sn[lngI - 1] ^ k1 * sn[lngI - 1] % 256;

            var loopTo3 = n;
            for (lngI = 1; lngI <= loopTo3; lngI++)
            {
                var midTmp = Conversions.ToString(Strings.Chr(sn[lngI]));
                StringType.MidStmtStr(ref ss, lngI, 1, midTmp);
            }

            if (salt)
            {
                ASCIIDecryptStringRet = Strings.Mid(ss, 3, Strings.Len(ss) - 4);
            }
            else
            {
                ASCIIDecryptStringRet = ss;
            }
        }

        return ASCIIDecryptStringRet;
    }

    // %Objetivo: .
    // %Parámetros:
    // %    Text     - .
    // %    Password - .
    public static string HexEncryptString(ref string Text)
    {
        string HexEncryptStringRet = default;
        string strBuffer = string.Empty;
        string strOutput;
        short intIndex;
        short intCount;

        // 'On Error GoTo ErrorHandler

        strBuffer = ASCIIEncryptString(Text);
        intCount = (short)Strings.Len(strBuffer);
        strOutput = string.Empty;
        var loopTo = intCount;
        for (intIndex = 1; intIndex <= loopTo; intIndex++)
            strOutput = strOutput + Strings.Right("00" + Conversion.Hex(Strings.Asc(Strings.Mid(strBuffer, intIndex, 1))), 2);
        HexEncryptStringRet = strOutput;

        return HexEncryptStringRet;
    }

    // %Objetivo: .
    // %Parámetros:
    // %    Text     - .
    // %    Password - .
    public static string HexDecryptString(ref string Text)
    {
        string HexDecryptStringRet = default;
        string strOutput;
        short intIndex;
        short intCount;

        // 'On Error GoTo ErrorHandler

        intCount = (short)Strings.Len(Text);
        strOutput = string.Empty;
        var loopTo = intCount;
        for (intIndex = 1; intIndex <= loopTo; intIndex += 2)
            strOutput = strOutput + Strings.Chr(Hex2Int(Strings.Mid(Text, intIndex, 2)));
        HexDecryptStringRet = ASCIIDecryptString(strOutput);

        return HexDecryptStringRet;
    }

    // %Objetivo: .
    // %Parámetros:
    // %    sHex - .
    private static short Hex2Int(string sHex)
    {
        short result = 0;
        string Tmp;
        short lo1;
        short lo2;
        int hi1;
        int hi2;

        const string Hx = "&H";
        const int BigShift = 65536;
        const short LilShift = 256;
        const short Two = 2;

        // 'On Error GoTo ErrorHandler

        Tmp = sHex;
        if (Strings.UCase(Strings.Left(sHex, 2)) == "&H")
            Tmp = Strings.Mid(sHex, 3);
        Tmp = Strings.Right("0000000" + Tmp, 8);
        if (Information.IsNumeric(Hx + Tmp))
        {
            lo1 = Conversions.ToShort(Hx + Strings.Right(Tmp, Two));
            hi1 = Conversions.ToInteger(Hx + Strings.Mid(Tmp, 5, Two));
            lo2 = Conversions.ToShort(Hx + Strings.Mid(Tmp, 3, Two));
            hi2 = Conversions.ToInteger(Hx + Strings.Left(Tmp, Two));
            result = (short)Math.Round((hi2 * LilShift + lo2) * (decimal)BigShift + hi1 * LilShift + lo1);
        }

        return result;
    }

    private static string _StrEncode_saltvalue = default;

    // **%StrEncode: Password encryption routine
    // %StrEncode: Rutina de encriptamiento de password
    public static string StrEncode(string s)
    {
        string StrEncodeRet = default;
        string result = string.Empty;
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
            StrEncodeRet = ss;
        }
        return result;
    }

    // **%StrDecode: Password de-encryptment routine
    // %StrDecode: Rutina de des-encriptamiento de password
    public static string StrDecode(string s)
    {
        string result = string.Empty;
        int key;
        bool salt;
        int n;
        int i;
        string ss;
        int k1;
        int k2;
        int k3;
        int k4;

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
            {
                result = Strings.Mid(ss, 3, Strings.Len(ss) - 4);
            }
            else
            {
                result = ss;
            }
        }
        return result;
    }
}