Imports System.IO
Imports System.ServiceModel.Web
Imports System.Text
Imports InMotionGIT.Common.Services.Interfaces

Namespace Services

    Public Class RestDataManager
        Implements Interfaces.IRestDataManager

        Public Function Query(command As String) As Stream Implements IRestDataManager.Query
            Dim data As String = "{""CheckDigit"": ""9"",""CivilStatus"": 1,""ClientID"": ""00000006329255"",""CompleteClientName"": ""SOLER PAZ, NELSON E"",""FirstName"": ""NELSON E"",""LastName"": ""SOLER""}"

            'Dim accept  As string = WebOperationContext.Current.IncomingRequest.Accept
            'Dim userAgentValue As string = WebOperationContext.Current.IncomingRequest.UserAgent

            'Dim response As OutgoingWebResponseContext  = WebOperationContext.Current.OutgoingResponse
            'response.StatusCode = System.Net.HttpStatusCode.UnsupportedMediaType
            'response.StatusDescription = "xxxx"

            If command.IsNotEmpty Then
                With New Common.Services.DataManager
                    'data = .QueryExecuteToTableJSON(New Common.Services.Contracts.DataCommand With {.ConnectionStringName = "BackOfficeConnectionString",
                    '                                                                                 .Statement = "SELECT * FROM " & command}, True)
                    data = .QueryExecuteToTableJSON(New Common.Services.Contracts.DataCommand With {.ConnectionStringName = "BackOfficeConnectionString",
                                                                                                     .Statement = command}, True)
                    data = data.DecompressString
                End With
            End If
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8"
            Dim result As New MemoryStream(Encoding.UTF8.GetBytes(data))
            Return result
        End Function

    End Class

End Namespace