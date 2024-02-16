Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Caching
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text

Namespace Helpers

    Public Class Caching

        ''' <summary>
        ''' Determina si una clave existe en el cache
        ''' </summary>
        ''' <param name="key"></param>
        Public Shared Function Exist(key As String) As Boolean
            Dim cache As ObjectCache = MemoryCache.Default

            Return cache.Contains(key)
        End Function

        ''' <summary>
        ''' Determina si una clave no existe en el cache
        ''' </summary>
        ''' <param name="key"></param>
        Public Shared Function NotExist(key As String) As Boolean
            Dim cache As ObjectCache = MemoryCache.Default

            Return Not cache.Contains(key)
        End Function

        ''' <summary>
        ''' Retorna el objeto asociado a la clave.
        ''' </summary>
        ''' <param name="key"></param>
        Public Shared Function GetItem(key As String) As Object
            Dim cache As ObjectCache = MemoryCache.Default
            Return cache(key)
        End Function

        Public Shared Sub SetItem(key As String, item As Object)
            SetItem(key, item, String.Empty, String.Empty, 0.0)
        End Sub

        Public Shared Sub SetItem(key As String, item As Object, timeout As Double)
            SetItem(key, item, String.Empty, String.Empty, timeout)
        End Sub

        Public Shared Sub SetItem(key As String, item As Object, serviceName As String, entityName As String)
            SetItem(key, item, String.Empty, String.Empty, 0.0)
        End Sub

        Public Shared Sub SetItem(key As String, item As Object, serviceName As String, entityName As String, timeout As Double)
            Dim cache As ObjectCache = MemoryCache.Default
            Dim TotalMinutes As Double = timeout

            If TotalMinutes = 0.0 Then
                If serviceName.IsNotEmpty AndAlso entityName.IsNotEmpty Then
                    TotalMinutes = ConfigurationManager.AppSettings(String.Format(CultureInfo.InvariantCulture,
                                                                                  "CacheExpiration.{0}.{1}", serviceName, entityName))
                End If

                If TotalMinutes = 0.0 AndAlso serviceName.IsNotEmpty Then
                    TotalMinutes = ConfigurationManager.AppSettings(String.Format(CultureInfo.InvariantCulture,
                                                                                  "CacheExpiration.{0}", serviceName))
                End If

                If TotalMinutes = 0.0 Then
                    TotalMinutes = ConfigurationManager.AppSettings("CacheExpiration")
                End If

                If TotalMinutes = 0.0 Then
                    TotalMinutes = 20.0
                End If
            End If

            If timeout = -1 Then
                cache.Set(key, item, New CacheItemPolicy())
            Else
                cache.Set(key, item, New CacheItemPolicy() With {.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(TotalMinutes)})
            End If
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="item"></param>
        ''' <param name="expiration"></param>
        ''' <example>Helpers.Caching.SetItem(token, object, DateTimeOffset.Now.AddMinutes(2))</example>
        Public Shared Sub SetItem(key As String, item As Object, expiration As DateTimeOffset)
            Dim cache As ObjectCache = MemoryCache.Default
            cache.Set(key, item, New CacheItemPolicy() With {.AbsoluteExpiration = expiration})
        End Sub

        ''' <summary>
        ''' Elimina el objeto asociado a la clave.
        ''' </summary>
        ''' <param name="key"></param>
        Public Shared Sub Remove(key As String)
            Dim cache As ObjectCache = MemoryCache.Default
            cache.Remove(key)
        End Sub

        Public Shared Function CacheCatalog() As String
            Dim cache As ObjectCache = MemoryCache.Default
            Dim buffer As New StringBuilder
            With buffer
                .AppendLine("<table>")
                For Each item As KeyValuePair(Of String, Object) In cache
                    .AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.Key, TypeName(item.Value), GetSizeOfObject(item.Value))
                    .AppendLine()
                Next
                .AppendLine("</table>")
            End With
            Return buffer.ToString
        End Function

        Private Shared Function GetSizeOfObject(item As Object) As Long
            Dim result As Long = 0
            Try
                Using stream As MemoryStream = New MemoryStream()
                    Dim binaryFormatter As BinaryFormatter = New BinaryFormatter()
                    binaryFormatter.Serialize(stream, item)

                    result = stream.Length
                End Using
            Catch ex As Exception
                result = 0
            End Try

            Return result
        End Function

        Public Shared Sub Clean()
            Dim cacheKeys As List(Of String) = MemoryCache.Default.Select(Function(kvp) kvp.Key).ToList()
            For Each cacheKey As String In cacheKeys
                MemoryCache.Default.Remove(cacheKey)
            Next
        End Sub

        Public Shared Sub RemoveStartWith(value As String)
            Dim cacheKeys As List(Of String) = MemoryCache.Default.Select(Function(kvp) kvp.Key).ToList()
            For Each cacheKey As String In cacheKeys
                If cacheKey.StartsWith(value, StringComparison.CurrentCultureIgnoreCase) Then
                    MemoryCache.Default.Remove(cacheKey)
                End If

            Next
        End Sub

    End Class

End Namespace