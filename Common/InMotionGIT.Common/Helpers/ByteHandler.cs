using System;
using System.Globalization;
using System.IO;
using System.Web;
using InMotionGIT.Common.Extensions;

namespace InMotionGIT.Common.Helpers
{

    public class ByteHandler
    {

        /// <summary>
        /// Method allows you to take a file and convert it to a byte array / Método permite tomar un archivo y convertirlo en un arreglo de bytes
        /// </summary>
        /// <param name="fileName">Name of the file to convert to bytes/ Nombre del archivo a  convertir en bytes</param>
        /// <returns></returns>
        public static byte[] FileToBytes(string fileName)
        {
            byte[] result = null;
            if (File.Exists(fileName))
            {
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    result = new byte[(int)(file.Length - 1L + 1)];
                    file.Read(result, 0, (int)file.Length);
                }
            }
            else
            {
                throw new Exceptions.InMotionGITException("The file was not found, please check the path");
            }
            return result;
        }

        /// <summary>
        /// Method that allows to convert a byte array to file / Método que permite convertir un arreglo de bytes en file
        /// </summary>
        /// <param name="FileInBytes">File in binary format/ </param>
        /// <param name="Path">Path where the file is stored/Ruta donde se almacena el archivo</param>
        /// <param name="NameFile">Name of the file to save/Nombre del archivo a guardar</param>
        /// <param name="Extension">File Extension/Extensión del archivo</param>
        /// <returns></returns>
        public static string BytesToFile(byte[] FileInBytes, string Path = "", string NameFile = "", string Extension = "")
        {
            string result = string.Empty;
            string pathReal = string.Empty;
            if (FileInBytes.IsNotEmpty())
            {
                if (Path.IsNotEmpty())
                {
                    string lastChar = Path.Substring(Path.Length - 1, 1);
                    if (!lastChar.Equals(@"\"))
                    {
                        Path = Path + @"\";
                    }
                    pathReal = Path;
                }
                else
                {
                    pathReal = System.IO.Path.GetTempPath();
                }
                if (NameFile.IsNotEmpty())
                {
                    pathReal = pathReal + NameFile;
                }
                else
                {
                    pathReal = pathReal + string.Format("temp_{0}", DateTime.Now.ToString("hh.mm.ss.fff", CultureInfo.InvariantCulture).Replace(".", "_"));
                }
                if (Extension.IsNotEmpty())
                {
                    if (Extension.Contains("."))
                    {
                        pathReal = pathReal + Extension;
                    }
                    else
                    {
                        pathReal = string.Format("{0}.{1}", pathReal, Extension);
                    }
                }
                else
                {
                    pathReal = pathReal + ".temp";
                }

                try
                {
                    File.WriteAllBytes(pathReal, FileInBytes);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.InMotionGITException(string.Format("An error occurred while trying to create the file '{0}', Detail:'{1}'", pathReal, ex.Message));
                }

                result = pathReal;
            }
            else
            {
                throw new Exceptions.InMotionGITException("Byte array can not be empty");
            }
            return result;
        }

        /// <summary>
        /// Method that allows to convert a byte array to file / Método que permite convertir un arreglo de bytes en file
        /// </summary>
        /// <param name="FileInBytes">File in binary format/ </param>
        /// <param name="NameFile">Name of the file to save/Nombre del archivo a guardar</param>
        /// <param name="Extension">File Extension/Extensión del archivo</param>
        /// <returns></returns>
        public static string BytesToFileHosted(byte[] FileInBytes, string NameFile = "", string Extension = "")
        {

            string rootPhysicalServer = string.Empty;
            string rootPathRelative = "Url.Files".AppSettings();
            string result = string.Empty;
            if (FileInBytes.IsNotEmpty())
            {

                rootPhysicalServer = string.Format("{0}{1}", HttpContext.Current.Server.MapPath("~"), rootPathRelative.Replace("/", @"\"));

                if (NameFile.IsEmpty())
                {
                    NameFile = string.Format("temp_{0}", DateTime.Now.ToString("hh.mm.ss.fff", CultureInfo.InvariantCulture).Replace(".", "_"));
                }

                if (Extension.IsNotEmpty())
                {
                    if (!Extension.Contains("."))
                    {
                        Extension = "." + Extension;
                    }
                    else if (!Extension.Equals(Path.GetExtension(Extension)))
                    {
                        Extension = Path.GetExtension(Extension);
                    }
                }
                else
                {
                    Extension = ".temp";
                }

                NameFile = NameFile + Extension;

                try
                {
                    if (File.Exists(string.Format(@"{0}\{1}", rootPhysicalServer, NameFile).Replace(@"\\", @"\")))
                    {
                        File.Delete(string.Format(@"{0}\{1}", rootPhysicalServer, NameFile).Replace(@"\\", @"\"));
                    }
                    File.WriteAllBytes(string.Format(@"{0}\{1}", rootPhysicalServer, NameFile).Replace(@"\\", @"\"), FileInBytes);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.InMotionGITException(string.Format("An error occurred while trying to create the file '{0}', Detail:'{1}'", rootPhysicalServer, ex.Message));
                }

                result = "Url.Files".AppSettings() + "/" + NameFile;
            }
            else
            {
                throw new Exceptions.InMotionGITException("Byte array can not be empty");
            }
            return result;
        }

    }

}