Imports InMotionGIT.Assembly.Model.Data

Public NotInheritable Class Factory

    Private Shared _catalogContent As EntityCollection = Nothing

    ''' <summary>
    ''' Inicializa y carga el catlogo para dar soporte a todas la funciones de esta clase
    ''' </summary>
    ''' <param name="repositoryPath">'Ruta de la carpeta Repository</param>
    Public Shared Sub Setup(repositoryPath As String)
        Dim CatalogFileName As String = String.Format("{0}\catalog.xml", repositoryPath)
        If IO.File.Exists(CatalogFileName) Then
            _catalogContent = InMotionGIT.Common.Helpers.Serialize.DeserializeFromFile(Of EntityCollection)(CatalogFileName)
        End If
    End Sub

    ''' <summary>
    ''' Devuelve el nombre de las librerias relacionadas con la clase a buscar
    ''' </summary>
    ''' <param name="repositoryPath">'Ruta de la carpeta Repository</param>
    ''' <param name="fullName">Nombre completo de la clase</param>
    ''' <returns>Librerias relacionadas </returns>
    Public Shared Function GetAssemblyByFullName(repositoryPath As String, fullName As String) As String
        Dim result As String = String.Empty

        If _catalogContent.IsEmpty Then
            Factory.Setup(repositoryPath)
        End If

        If _catalogContent.IsNotEmpty Then
            Dim entity As Entity = _catalogContent.GetItemByFullName(fullName)
            If entity.IsNotEmpty Then
                Dim assemblydependency As String = entity.assemblydependency
                If assemblydependency.IsNotEmpty Then
                    For Each item As String In assemblydependency.Split(",")
                        If result.IsNotEmpty Then
                            result += ","
                        End If
                        result += IO.Path.GetFileNameWithoutExtension(item)
                    Next
                End If
            End If
        End If
        Return result
    End Function

    ''' <summary>
    ''' Devuelve la información relacionadas con la clase a buscar
    ''' </summary>
    ''' <param name="repositoryPath">'Ruta de la carpeta Repository</param>
    ''' <param name="fullName">Nombre completo de la clase</param>
    ''' <returns>Información de la clase</returns>
    ''' <remarks></remarks>
    Public Shared Function GetEntityByFullName(repositoryPath As String, fullName As String) As Entity

        If _catalogContent.IsEmpty Then
            Factory.Setup(repositoryPath)
        End If

        If _catalogContent.IsNotEmpty Then
            Return _catalogContent.GetItemByFullName(fullName)
        Else
            Return Nothing
        End If
    End Function

End Class
