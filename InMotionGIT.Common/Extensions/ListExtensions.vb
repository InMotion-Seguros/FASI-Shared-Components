Imports System.Runtime.CompilerServices

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the list data type
    ''' </summary>
    Public Module ListExtensions

        ''' <summary>
        ''' Defines whether a list is empty or contains elements within it/ Define si una lista es empty or no contiene elementos dentro de ella
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function IsEmptyAndNotContainsItems(Of T)(ByVal source As List(Of T)) As Boolean
            Dim result As Boolean = False
            If IsNothing(source) Then
                Return True
            End If
            If Not source.Count <> 0 Then
                Return True
            End If
            Return result
        End Function

        ''' <summary>
        ''' Creates a string separated by the character indicated property values that difinio/Crea un string separado por el carácter que se indique de  los valores de la propiedad que se difinio
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="separator"></param>
        ''' <param name="propertyName"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function CreateConcatenation(Of T)(ByVal source As List(Of T), separator As String, propertyName As String) As String
            Dim result As String = String.Empty
            If Not IsNothing(source) AndAlso source.Count <> 0 Then
                Dim temporalObject As Object = source.FirstOrDefault()
                If Not IsNothing(temporalObject) Then
                    Dim isExistProperty As Boolean = ObjectExtensions.ExistsProperty(temporalObject, propertyName)
                    If isExistProperty Then
                        Dim listaVector As New List(Of String)
                        For Each ItemSource In source
                            listaVector.Add(ItemSource.GetType().GetProperty(propertyName).GetValue(ItemSource, Nothing))
                        Next
                        If Not listaVector.IsEmptyAndNotContainsItems Then
                            result = String.Join(separator, listaVector)
                        End If
                    End If
                End If
            End If
            Return result
        End Function

        <Extension()>
        Public Sub AddByDistinct(ByRef source As List(Of String), value As String)
            If source.IsEmpty Then
                source = New List(Of String)
            End If
            Dim foundItem = (From itemDb In source Where itemDb.Equals(value) Select itemDb).FirstOrDefault()
            If foundItem.IsEmpty Then
                source.Add(value)
            End If
        End Sub

        <Extension()>
        Public Sub AddByDistinct(ByRef source As List(Of Int32), value As Int32)
            If source.IsEmpty Then
                source = New List(Of Int32)
            End If
            Dim foundItem = (From itemDb In source Where itemDb = value Select itemDb).FirstOrDefault()
            If foundItem.IsEmpty Then
                source.Add(value)
            End If
        End Sub

        <Extension()>
        Public Sub AddByDistinct(ByRef source As List(Of Int64), value As Int64)
            If source.IsEmpty Then
                source = New List(Of Int64)
            End If
            Dim foundItem = (From itemDb In source Where itemDb = value Select itemDb).FirstOrDefault()
            If foundItem.IsEmpty Then
                source.Add(value)
            End If
        End Sub

    End Module

End Namespace