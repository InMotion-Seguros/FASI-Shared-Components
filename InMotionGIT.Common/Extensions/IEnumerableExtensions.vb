Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module IEnumerableExtensions

        ''' <summary>
        ''' This method allows to make a 'String.Join' of an object that allows the IEnumerable / Este método permite hacer un 'String.Join' de un objeto que permite el IEnumerable
        ''' </summary>
        ''' <typeparam name="T">Type of object/ Tipo del objeto</typeparam>
        ''' <typeparam name="TProp">Type property for which you want to join/Tipo propiedad por la que se desea hacer el join</typeparam>
        ''' <param name="source">Colección o objeto que heredan de IEnumerable/Collection or object inheriting from IEnumerable</param>
        ''' <param name="[property]">Property for which you want to join/Propiedad por la que se desea hacer el join</param>
        ''' <param name="separator">Value with which it is desired to carry out the separation of the values/ vValor con los que se desea realizar la separación de los valores</param>
        ''' <returns></returns>
        <Extension>
        Public Function JoinToString(Of T, TProp)(source As IEnumerable(Of T), [property] As Func(Of T, TProp), separator As String) As String

            Dim result As String = String.Empty
            If separator Is Nothing Then
                Throw New ArgumentNullException("separator")
            End If

            If source.IsEmpty Then
                Throw New ArgumentNullException("source")
            End If

            If source.Count = 0 Then
                Throw New ArgumentNullException("source")
            End If

            Dim list As List(Of TProp) = source.[Select]([property]).ToList()

            result = String.Join(separator, list)

            Return result
        End Function

        <Extension>
        Public Function GetProperties(Of T, TProp)(seq As IEnumerable(Of T), selector As Func(Of T, TProp)) As List(Of TProp)
            Return seq.[Select](selector).ToList()
        End Function

        'Public Sub ForEachToJoin(Of T)(col As IEnumerable(Of T), action As Action(Of T))
        '    Dim contet As Integer
        '    If action Is Nothing Then
        '        Throw New ArgumentNullException("action")
        '    End If
        '    For Each item In col
        '        contet
        '        action(item)
        '    Next
        'End Sub

    End Module

End Namespace