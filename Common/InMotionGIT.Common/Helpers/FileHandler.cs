using System;
using System.IO;

namespace InMotionGIT.Common.Helpers
{

    public sealed class FileHandler
    {

        /// <summary>
        /// Permite crear un archivo a patir del string
        /// </summary>
        /// <param name="fileName">Nombre y ruta del archivo a ser creado</param>
        /// <param name="content">Contenido a ser almacenado en el arhcivo </param>
        /// <returns>Verdadero en caso de poder crear el archivo, Falso en caso contraio</returns>
        public static bool SaveContent(string fileName, string content)
        {
            bool result = false;

            try
            {
                if (content.Contains("utf-16"))
                {
                    content = content.Replace("utf-16", "utf-8");
                }
                File.WriteAllText(fileName, content, System.Text.Encoding.UTF8);
                result = true;
            }
            catch (Exception ex)
            {
                LogHandler.ErrorLog("CreateFromString", "Error", ex);
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// It allows duplicate a directory completely at both directories and archivos internos in on a specific route / Permite duplicar un directorio de manera completa tanto a nivel de directorios internos y archivos en una ruta específica
        /// </summary>
        /// <param name="rootSource"></param>
        /// <param name="rootTarget"></param>
        /// <returns></returns>
        public static bool DuplicateFileAndDirectory(string rootSource, string rootTarget)
        {
            bool result = false;
            if (System.IO.Directory.Exists(rootSource))
            {
                foreach (string file in System.IO.Directory.EnumerateFiles(rootSource, "*.*", SearchOption.AllDirectories))
                {
                    string temporalPathSource = Path.GetDirectoryName(file);
                    string temporalPathTarget = temporalPathSource.Replace(rootSource, rootTarget);
                    string temporalNameFile = Path.GetFileName(file);
                    if (!System.IO.Directory.Exists(temporalPathTarget))
                    {
                        System.IO.Directory.CreateDirectory(temporalPathTarget);
                    }
                    File.Copy(file, string.Format(@"{0}\{1}", temporalPathTarget, temporalNameFile));
                }
                result = true;
                return result;
            }
            else
            {
                throw new Exception("There is no way to duplicate, check that the route");
            }
        }

        /// <summary>
        /// Reads a file and converts it into a vector of bytes/ Lee un archivo y lo convierte en un vector de bytes
        /// </summary>
        /// <param name="FileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        /// <returns></returns>

        public static byte[] FileToBinary(string FileName)
        {
            byte[] result = null;
            if (Exist(FileName))
            {
                using (var file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    result = new byte[(int)(file.Length - 1L + 1)];
                    file.Read(result, 0, (int)file.Length);
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if a file exists on a specific path / Verifica si existe un archivo en un ruta específica
        /// </summary>
        /// <param name="fileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        /// <returns></returns>
        public static bool Exist(string fileName)
        {
            bool result = false;
            if (File.Exists(fileName))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Checks if a file not exists on a specific path / Verifica si no existe un archivo en un ruta específica
        /// </summary>
        /// <param name="fileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        /// <returns></returns>
        public static bool NotExist(string fileName)
        {
            bool result = false;
            if (!File.Exists(fileName))
            {
                result = true;
            }
            return result;
        }

    }

}