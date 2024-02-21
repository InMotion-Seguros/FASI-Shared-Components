Imports System.Runtime.CompilerServices

Namespace Extensions

    'http://weblogs.asp.net/grantbarrington/archive/2009/01/19/enumhelper-getting-a-friendly-description-from-an-enum.aspx
    'http://calvinallen.net/archives/2012/extension-methods-against-an-enum-using-reflection/

    ''' <summary>
    ''' Extension methods for the enum data type
    ''' </summary>
    Module EnumExtensions

        ''' <summary>
        ''' Use this method to get the integer value of any enum.
        ''' </summary>
        ''' <param name="theEnum">The enum value.</param>
        <Extension()>
        Public Function NumericValue(ByRef theEnum As System.Enum) As Integer
            Return Convert.ToInt32(theEnum)
        End Function

        <Extension()>
        Public Sub FromString(ByRef theEnum As System.Enum,
                              ByVal fromString As String)
            theEnum = System.Enum.Parse(theEnum.GetType(), fromString, True)
        End Sub

    End Module

End Namespace