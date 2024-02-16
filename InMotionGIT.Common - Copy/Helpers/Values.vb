Namespace Helpers

    Public Class Values

        '% insGetSetting: se toman los valore del registro
        Public Function insGetSetting(ByVal Name As String, ByVal DefValue As String, Optional ByVal Group As String = "") As String
            Dim lclsConfig As New VisualTimeConfig

            insGetSetting = lclsConfig.LoadSetting(Name, DefValue, Group)
            'UPGRADE_NOTE: Object lclsConfig may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            lclsConfig = Nothing

        End Function

    End Class

End Namespace