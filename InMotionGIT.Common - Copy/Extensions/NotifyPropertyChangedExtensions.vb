#Region "using"

Imports System.Runtime.CompilerServices

#End Region

Namespace Extensions

    ''' <summary>
    ''' Extension methods for the DataRow type
    ''' </summary>
    Public Module NotifyPropertyChangedExtensions

#Region "Method Information"

        <Extension()>
        Public Function GetPropertyName(methodBase As System.Reflection.MethodBase) As String
            Return methodBase.Name.Substring(4)
        End Function

#End Region

    End Module

End Namespace