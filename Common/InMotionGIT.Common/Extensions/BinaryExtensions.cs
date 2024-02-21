using System;
using System.Diagnostics;
using System.IO.Compression;

namespace InMotionGIT.Common.Extensions
{

    /// <summary>
    /// Extension methods for the string data type
    /// </summary>
    public static class BinaryExtensions
    {

        /// <summary>
        /// Metodo de comprescion de string, esto ose utiliza en el string de serializacion de datatable por medio de json
        /// </summary>
        /// <param name="Text">Strign a comprimir</param>
        /// <returns>String comprimido</returns>
        /// <remarks></remarks>
        public static byte[] Compress(this byte[] Text)
        {
            byte[] buffer__1 = Text;
            var memoryStream = new System.IO.MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer__1, 0, buffer__1.Length);
            }

            memoryStream.Position = 0L;

            byte[] compressedData = new byte[(int)(memoryStream.Length - 1L + 1)];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            byte[] gZipBuffer = new byte[compressedData.Length + 3 + 1];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer__1.Length), 0, gZipBuffer, 0, 4);
            Debug.WriteLine("Size original:" + buffer__1.Length.ToString());
            Debug.WriteLine("Size Comopres:" + gZipBuffer.Length.ToString());
            return gZipBuffer;
        }

        /// <summary>
        /// Metodo de descomprecion de string
        /// </summary>
        /// <param name="Text">String a descomprimir</param>
        /// <returns>Retorna el strign descomprimido</returns>
        /// <remarks></remarks>
        public static byte[] Decompress(this byte[] Text)
        {
            byte[] gZipBuffer = Text;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                byte[] buffer = new byte[dataLength];

                memoryStream.Position = 0L;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return buffer;
            }
        }

    }

}