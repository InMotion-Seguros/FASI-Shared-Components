Imports System.IO.Compression
Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the string data type
    ''' </summary>
    Public Module BinaryExtensions

        ''' <summary>
        ''' Metodo de comprescion de string, esto ose utiliza en el string de serializacion de datatable por medio de json
        ''' </summary>
        ''' <param name="Text">Strign a comprimir</param>
        ''' <returns>String comprimido</returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function Compress(ByVal Text As Byte()) As Byte()
            Dim buffer__1 As Byte() = Text
            Dim memoryStream = New IO.MemoryStream()
            Using gZipStream = New GZipStream(memoryStream, CompressionMode.Compress, True)
                gZipStream.Write(buffer__1, 0, buffer__1.Length)
            End Using

            memoryStream.Position = 0

            Dim compressedData = New Byte(memoryStream.Length - 1) {}
            memoryStream.Read(compressedData, 0, compressedData.Length)

            Dim gZipBuffer = New Byte(compressedData.Length + 3) {}
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length)
            Buffer.BlockCopy(BitConverter.GetBytes(buffer__1.Length), 0, gZipBuffer, 0, 4)
            System.Diagnostics.Debug.WriteLine("Size original:" + buffer__1.Length.ToString)
            System.Diagnostics.Debug.WriteLine("Size Comopres:" + gZipBuffer.Length.ToString)
            Return gZipBuffer
        End Function

        ''' <summary>
        ''' Metodo de descomprecion de string
        ''' </summary>
        ''' <param name="Text">String a descomprimir</param>
        ''' <returns>Retorna el strign descomprimido</returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function Decompress(ByVal Text As Byte()) As Byte()
            Dim gZipBuffer As Byte() = Text
            Using memoryStream = New IO.MemoryStream()
                Dim dataLength As Integer = BitConverter.ToInt32(gZipBuffer, 0)
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4)

                Dim buffer = New Byte(dataLength - 1) {}

                memoryStream.Position = 0
                Using gZipStream = New GZipStream(memoryStream, CompressionMode.Decompress)
                    gZipStream.Read(buffer, 0, buffer.Length)
                End Using

                Return buffer
            End Using
        End Function

    End Module

End Namespace