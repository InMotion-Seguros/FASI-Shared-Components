Namespace Events

    Public Class ProgressEventArgs
        Inherits EventArgs

        Property Message As String
        Property Level As Integer
        Property Index As Integer

        Public Sub New(ByVal message As String, ByVal level As Integer, ByVal index As Integer)
            Me.Message = message
            Me.Level = level
            Me.Index = index
        End Sub

    End Class

End Namespace