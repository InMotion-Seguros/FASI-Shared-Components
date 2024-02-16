Imports System.IO

Namespace Helpers

    Public NotInheritable Class FileHandler

        ''' <summary>
        ''' Permite crear un archivo a patir del string
        ''' </summary>
        ''' <param name="fileName">Nombre y ruta del archivo a ser creado</param>
        ''' <param name="content">Contenido a ser almacenado en el arhcivo </param>
        ''' <returns>Verdadero en caso de poder crear el archivo, Falso en caso contraio</returns>
        Public Shared Function SaveContent(fileName As String, content As String) As Boolean
            Dim result As Boolean = False

            Try
                If content.Contains("utf-16") Then
                    content = content.Replace("utf-16", "utf-8")
                End If
                System.IO.File.WriteAllText(fileName, content, System.Text.ASCIIEncoding.UTF8)
                result = True
            Catch ex As Exception
                Helpers.LogHandler.ErrorLog("CreateFromString", "Error", ex)
            Finally

            End Try
            Return result
        End Function

        ''' <summary>
        ''' It allows duplicate a directory completely at both directories and archivos internos in on a specific route / Permite duplicar un directorio de manera completa tanto a nivel de directorios internos y archivos en una ruta específica
        ''' </summary>
        ''' <param name="rootSource"></param>
        ''' <param name="rootTarget"></param>
        ''' <returns></returns>
        Public Shared Function DuplicateFileAndDirectory(rootSource As String, rootTarget As String) As Boolean
            Dim result As Boolean = False
            If System.IO.Directory.Exists(rootSource) Then
                For Each file As String In System.IO.Directory.EnumerateFiles(rootSource, "*.*", SearchOption.AllDirectories)
                    Dim temporalPathSource As String = Path.GetDirectoryName(file)
                    Dim temporalPathTarget As String = temporalPathSource.Replace(rootSource, rootTarget)
                    Dim temporalNameFile As String = System.IO.Path.GetFileName(file)
                    If Not System.IO.Directory.Exists(temporalPathTarget) Then
                        System.IO.Directory.CreateDirectory(temporalPathTarget)
                    End If
                    System.IO.File.Copy(file, String.Format("{0}\{1}", temporalPathTarget, temporalNameFile))
                Next
                result = True
                Return result
            Else
                Throw New Exception("There is no way to duplicate, check that the route")
            End If
        End Function

        ''' <summary>
        '''  Reads a file and converts it into a vector of bytes/ Lee un archivo y lo convierte en un vector de bytes
        ''' </summary>
        ''' <param name="FileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        ''' <returns></returns>

        Public Shared Function FileToBinary(FileName As String) As Byte()
            Dim result As Byte() = Nothing
            If Exist(FileName) Then
                Using file As New FileStream(FileName, FileMode.Open, FileAccess.Read)
                    result = New Byte(file.Length - 1) {}
                    file.Read(result, 0, CInt(file.Length))
                End Using
            End If
            Return result
        End Function

        ''' <summary>
        ''' Checks if a file exists on a specific path / Verifica si existe un archivo en un ruta específica
        ''' </summary>
        ''' <param name="fileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        ''' <returns></returns>
        Public Shared Function Exist(fileName As String) As Boolean
            Dim result As Boolean = False
            If System.IO.File.Exists(fileName) Then
                result = True
            End If
            Return result
        End Function

        ''' <summary>
        ''' Checks if a file not exists on a specific path / Verifica si no existe un archivo en un ruta específica
        ''' </summary>
        ''' <param name="fileName">Name of the file you want to convert to bytes/ Nombre del archivo que deseas convertir a bytes</param>
        ''' <returns></returns>
        Public Shared Function NotExist(fileName As String) As Boolean
            Dim result As Boolean = False
            If Not System.IO.File.Exists(fileName) Then
                result = True
            End If
            Return result
        End Function

    End Class

End Namespace