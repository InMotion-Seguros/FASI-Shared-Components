Namespace Helpers

    Public Class MultiCompany

#Region "Field"

        Public Property Identification As Integer
        Public Property Name As String

#End Region

#Region "Method"

        Public Shared Function GetUserInfo(ByVal nCompanyId As Short) As Object
            Dim clsConfig As VisualTimeConfig
            Dim companyName As String = ""
            Dim companyUser As String = ""
            Dim companyPassword As String = ""
            Dim arrResult(2) As Object

            arrResult(0) = String.Empty
            arrResult(1) = String.Empty
            arrResult(2) = String.Empty
            clsConfig = New VisualTimeConfig
            If clsConfig.GetCompanySettings(nCompanyId, companyName, companyUser, companyPassword) Then
                arrResult(0) = companyName
                arrResult(1) = companyUser
                arrResult(2) = companyPassword
            End If

            GetUserInfo = arrResult
        End Function

        Public Shared Function IsMultiCompany() As Boolean
            Dim sMultiCom As String = String.Empty
            With New Values
                sMultiCom = .insGetSetting("MultiCompany", "No", "Database")
            End With
            Return (String.Compare(sMultiCom, "Yes", True) = 0)
        End Function

        Public Shared Function MultiCompanyList() As DataTable
            Dim clsConfig As VisualTimeConfig
            Dim companyName As String = String.Empty
            Dim intIndex As Integer

            ' Definicion del DataTable
            Dim List As New DataTable("List")
            Dim id As New DataColumn("id")
            Dim name As New DataColumn("name")

            ' Creacion del DataTable
            List.Columns.Add(id)
            List.Columns.Add(name)

            clsConfig = New VisualTimeConfig

            For intIndex = 1 To 20
                If clsConfig.GetCompanySettings(intIndex, companyName, "", "") Then
                    List.Rows.Add(intIndex.ToString, companyName)
                End If
            Next intIndex

            Return List
        End Function

#End Region

    End Class

End Namespace