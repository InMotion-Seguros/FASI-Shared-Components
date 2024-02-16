Namespace Helpers

    Public NotInheritable Class KeyValidator

        ''' <summary>
        ''' It generates a key based on the number of the week in the month the number of concatenated day of the week.
        ''' </summary>
        ''' <returns>Key generated</returns>
        Public Shared Function GenerateKey() As String
            Return String.Format("{0}{1}", Date.Now.WeekOfMonth, Date.Now.NumericDayOfWeek)
        End Function

        ''' <summary>
        ''' Validator key for boor
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Public Shared Function KeyValidator(key As String) As Boolean
            Dim result As String = GenerateKey()
            Return result.Equals(key)
        End Function

    End Class

End Namespace