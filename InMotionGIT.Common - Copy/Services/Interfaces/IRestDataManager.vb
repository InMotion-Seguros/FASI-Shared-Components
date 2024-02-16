Imports System.ComponentModel
Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Web

Namespace Services.Interfaces

    <ServiceContract()>
    Public Interface IRestDataManager

        <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
        <OperationContract()>
        <Description("Ramos activos.")>
        Function Query(command As String) As Stream

    End Interface

End Namespace