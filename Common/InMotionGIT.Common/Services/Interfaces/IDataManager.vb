Imports System.ServiceModel
Imports InMotionGIT.Common.Services.Contracts

Namespace Services.Interfaces

    <ServiceContract()>
    Public Interface IDataManager

        <OperationContract()>
        Function QueryExecuteToTable(command As DataCommand, resultEmpty As Boolean) As QueryResult

        <OperationContract()>
        Function QueryExecuteScalar(command As DataCommand) As Object

        <OperationContract()>
        Function QueryExecuteScalarToInteger(command As DataCommand) As Integer

        <OperationContract()>
        Function QueryExecuteScalarToDecimal(command As DataCommand) As Decimal

        <OperationContract>
        Function QueryExecuteToLookup(command As DataCommand) As List(Of DataType.LookUpValue)

        <OperationContract()>
        Function QueryExecuteScalarToString(command As DataCommand) As String

        <OperationContract()>
        Function QueryExecuteScalarToDate(command As DataCommand) As Date

        <OperationContract()>
        Function CommandExecute(command As DataCommand) As Integer

        <OperationContract(IsOneWay:=True)>
        Sub CommandExecuteAsynchronous(command As DataCommand)

        <OperationContract()>
        Function DataStructure(command As DataCommand) As String

        <OperationContract()>
        Function ResolveStatement(command As DataCommand) As String

        <OperationContract()>
        Function ObjectExist(command As DataCommand) As Boolean

        <OperationContract()>
        Function ProcedureExecuteToTable(command As DataCommand, resultEmpty As Boolean) As QueryResult

        <OperationContract()>
        Function ProcedureExecute(command As DataCommand) As StoredProcedureResult

        <OperationContract()>
        Function ProcedureExecuteResultSchema(command As DataCommand) As DataTable

        <OperationContract()>
        Function PackageExecuteScalar(commands As List(Of DataCommand)) As List(Of InMotionGIT.Common.DataType.LookUpPackage)

        <OperationContract()>
        Function PackageExecuteToLookUp(commands As List(Of DataCommand)) As List(Of InMotionGIT.Common.DataType.LookUpPackage)

        <OperationContract()>
        Function QueryExecuteToTableJSON(command As DataCommand, resultEmpty As Boolean) As String

        <OperationContract()>
        Function ConnectionStringGet(ConnectionStrinName As String, companyId As Integer) As ConnectionString

        <OperationContract()>
        Function ConnectionStringGetAll(CodeValidator As String, companyId As Integer) As List(Of ConnectionString)

        <OperationContract()>
        Function ConnectionStringUserAndPassword(ConectionStringName As String, companyId As Integer) As Credential

        <OperationContract()>
        Function GetDataBaseProvider(repositoryName As String) As String

        <OperationContract()>
        Function GetSettingValue(repositoryName As String, settingName As String) As String

        <OperationContract()>
        Function CurrentTime() As String

        <OperationContract()>
        Function AppInfo(path As String) As Services.Contracts.info

        <OperationContract()>
        Function Check(parameters As Dictionary(Of String, String)) As List(Of String)

    End Interface

End Namespace