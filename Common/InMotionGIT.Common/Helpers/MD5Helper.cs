using System;
using System.IO;
using System.Security.Cryptography;

namespace InMotionGIT.Common.Helpers
{

    public class MD5Helper
    {

        public static string CheckSum(string path)
        {
            string result = "";
            try
            {
                var manager = MD5.Create();
                var content = File.OpenRead(path);
                result = BitConverter.ToString(manager.ComputeHash(content)).Replace("-", string.Empty);
                content.Close();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

    }

}