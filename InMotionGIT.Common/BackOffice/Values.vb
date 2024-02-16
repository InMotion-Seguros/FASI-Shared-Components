Namespace BackOffice

    Public Class Values

        Public Function insGetSetting(ByVal Name As String, ByVal DefValue As String, Optional ByVal Group As String = "") As String
            Dim lclsConfig As New VisualTimeConfig

            insGetSetting = lclsConfig.LoadSetting(Name, DefValue, Group)
            lclsConfig = Nothing

        End Function

    End Class

End Namespace