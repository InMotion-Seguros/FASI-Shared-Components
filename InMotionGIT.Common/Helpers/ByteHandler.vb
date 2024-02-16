Imports System.Globalization
Imports System.IO
Imports System.Web

Namespace Helpers

    Public Class ByteHandler

        ''' <summary>
        ''' Method allows you to take a file and convert it to a byte array / Método permite tomar un archivo y convertirlo en un arreglo de bytes
        ''' </summary>
        ''' <param name="fileName">Name of the file to convert to bytes/ Nombre del archivo a  convertir en bytes</param>
        ''' <returns></returns>
        Public Shared Function FileToBytes(fileName As String) As Byte()
            Dim result As Byte() = Nothing
            If File.Exists(fileName) Then
                Using file As New FileStream(fileName, FileMode.Open, FileAccess.Read)
                    result = New Byte(file.Length - 1) {}
                    file.Read(result, 0, CInt(file.Length))
                End Using
            Else
                Throw New Exceptions.InMotionGITException("The file was not found, please check the path")
            End If
            Return result
        End Function

        ''' <summary>
        '''Method that allows to convert a byte array to file / Método que permite convertir un arreglo de bytes en file
        ''' </summary>
        ''' <param name="FileInBytes">File in binary format/ </param>
        ''' <param name="Path">Path where the file is stored/Ruta donde se almacena el archivo</param>
        ''' <param name="NameFile">Name of the file to save/Nombre del archivo a guardar</param>
        ''' <param name="Extension">File Extension/Extensión del archivo</param>
        ''' <returns></returns>
        Public Shared Function BytesToFile(FileInBytes As Byte(),
                                           Optional Path As String = "",
                                           Optional NameFile As String = "",
                                           Optional Extension As String = "") As String
            Dim result As String = String.Empty
            Dim pathReal As String = String.Empty
            If FileInBytes.IsNotEmpty Then
                If Path.IsNotEmpty Then
                    Dim lastChar As String = Path.Substring(Path.Length - 1, 1)
                    If Not lastChar.Equals("\") Then
                        Path = Path + "\"
                    End If
                    pathReal = Path
                Else
                    pathReal = IO.Path.GetTempPath()
                End If
                If NameFile.IsNotEmpty Then
                    pathReal = pathReal + NameFile
                Else
                    pathReal = pathReal + String.Format("temp_{0}", Date.Now.ToString("hh.mm.ss.fff", CultureInfo.InvariantCulture).Replace(".", "_"))
                End If
                If Extension.IsNotEmpty Then
                    If Extension.Contains(".") Then
                        pathReal = pathReal + Extension
                    Else
                        pathReal = String.Format("{0}.{1}", pathReal, Extension)
                    End If
                Else
                    pathReal = pathReal + ".temp"
                End If

                Try
                    File.WriteAllBytes(pathReal, FileInBytes)
                Catch ex As Exception
                    Throw New Exceptions.InMotionGITException(String.Format("An error occurred while trying to create the file '{0}', Detail:'{1}'", pathReal, ex.Message))
                End Try

                result = pathReal
            Else
                Throw New Exceptions.InMotionGITException("Byte array can not be empty")
            End If
            Return result
        End Function

        ''' <summary>
        '''Method that allows to convert a byte array to file / Método que permite convertir un arreglo de bytes en file
        ''' </summary>
        ''' <param name="FileInBytes">File in binary format/ </param>
        ''' <param name="NameFile">Name of the file to save/Nombre del archivo a guardar</param>
        ''' <param name="Extension">File Extension/Extensión del archivo</param>
        ''' <returns></returns>
        Public Shared Function BytesToFileHosted(ByVal FileInBytes As System.Byte(),
                                                 Optional NameFile As String = "",
                                                 Optional Extension As String = "") As String

            Dim rootPhysicalServer As String = String.Empty
            Dim rootPathRelative As String = "Url.Files".AppSettings
            Dim result As String = String.Empty
            If FileInBytes.IsNotEmpty Then

                rootPhysicalServer = String.Format("{0}{1}", HttpContext.Current.Server.MapPath("~"), rootPathRelative.Replace("/", "\"))

                If NameFile.IsEmpty Then
                    NameFile = String.Format("temp_{0}", Date.Now.ToString("hh.mm.ss.fff", CultureInfo.InvariantCulture).Replace(".", "_"))
                End If

                If Extension.IsNotEmpty Then
                    If Not Extension.Contains(".") Then
                        Extension = "." + Extension
                    Else
                        If Not Extension.Equals(System.IO.Path.GetExtension(Extension)) Then
                            Extension = System.IO.Path.GetExtension(Extension)
                        End If
                    End If
                Else
                    Extension = ".temp"
                End If

                NameFile = NameFile + Extension

                Try
                    If System.IO.File.Exists(String.Format("{0}\{1}", rootPhysicalServer, NameFile).Replace("\\", "\")) Then
                        System.IO.File.Delete(String.Format("{0}\{1}", rootPhysicalServer, NameFile).Replace("\\", "\"))
                    End If
                    System.IO.File.WriteAllBytes(String.Format("{0}\{1}", rootPhysicalServer, NameFile).Replace("\\", "\"), FileInBytes)
                Catch ex As Exception
                    Throw New InMotionGIT.Common.Exceptions.InMotionGITException(String.Format("An error occurred while trying to create the file '{0}', Detail:'{1}'", rootPhysicalServer, ex.Message))
                End Try

                result = "Url.Files".AppSettings + "/" + NameFile
            Else
                Throw New InMotionGIT.Common.Exceptions.InMotionGITException("Byte array can not be empty")
            End If
            Return result
        End Function

    End Class

End Namespace