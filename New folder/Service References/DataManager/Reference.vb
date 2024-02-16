﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace DataManager
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0"),  _
     System.ServiceModel.ServiceContractAttribute(ConfigurationName:="DataManager.IDataManager")>  _
    Public Interface IDataManager
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteToTable", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteToTableResponse")>  _
        Function QueryExecuteToTable(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean, ByVal parameters As System.Collections.Generic.Dictionary(Of String, Object)) As InMotionGIT.Common.Services.Contracts.QueryResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteScalar", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteScalarResponse"),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Data.ParameterDirection)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Data.DbType)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpPackage))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.DataType.LookUpPackage)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.DataType.LookUpValue)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpValue))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.Dictionary(Of String, Object))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of String))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Enumerations.EnumSourceType)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.DataCommand)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.ConnectionString)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataParameter))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.DataParameter)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.QueryResult)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.StoredProcedureResult)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataCommand))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.ConnectionString))),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.Credential)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(InMotionGIT.Common.Services.Contracts.info)),  _
         System.ServiceModel.ServiceKnownTypeAttribute(GetType(System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.info)))>  _
        Function QueryExecuteScalar(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal parameters As System.Collections.Generic.Dictionary(Of String, Object)) As Object
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteScalarToInteger", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteScalarToIntegerResponse")>  _
        Function QueryExecuteScalarToInteger(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal parameter As System.Collections.Generic.Dictionary(Of String, Object)) As Integer
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteScalarToDecimal", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteScalarToDecimalResponse")>  _
        Function QueryExecuteScalarToDecimal(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Decimal
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteToLookup", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteToLookupResponse")>  _
        Function QueryExecuteToLookup(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpValue)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteScalarToString", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteScalarToStringResponse")>  _
        Function QueryExecuteScalarToString(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteScalarToDate", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteScalarToDateResponse")>  _
        Function QueryExecuteScalarToDate(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Date
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/CommandExecute", ReplyAction:="http://tempuri.org/IDataManager/CommandExecuteResponse")>  _
        Function CommandExecute(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Integer
        
        <System.ServiceModel.OperationContractAttribute(IsOneWay:=true, Action:="http://tempuri.org/IDataManager/CommandExecuteAsynchronous")>  _
        Sub CommandExecuteAsynchronous(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/DataStructure", ReplyAction:="http://tempuri.org/IDataManager/DataStructureResponse")>  _
        Function DataStructure(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ResolveStatement", ReplyAction:="http://tempuri.org/IDataManager/ResolveStatementResponse")>  _
        Function ResolveStatement(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ObjectExist", ReplyAction:="http://tempuri.org/IDataManager/ObjectExistResponse")>  _
        Function ObjectExist(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Boolean
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ProcedureExecuteToTable", ReplyAction:="http://tempuri.org/IDataManager/ProcedureExecuteToTableResponse")>  _
        Function ProcedureExecuteToTable(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean) As InMotionGIT.Common.Services.Contracts.QueryResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ProcedureExecute", ReplyAction:="http://tempuri.org/IDataManager/ProcedureExecuteResponse")>  _
        Function ProcedureExecute(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As InMotionGIT.Common.Services.Contracts.StoredProcedureResult
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ProcedureExecuteResultSchema", ReplyAction:="http://tempuri.org/IDataManager/ProcedureExecuteResultSchemaResponse")>  _
        Function ProcedureExecuteResultSchema(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As System.Data.DataTable
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/PackageExecuteScalar", ReplyAction:="http://tempuri.org/IDataManager/PackageExecuteScalarResponse")>  _
        Function PackageExecuteScalar(ByVal commands As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataCommand)) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpPackage)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/PackageExecuteToLookUp", ReplyAction:="http://tempuri.org/IDataManager/PackageExecuteToLookUpResponse")>  _
        Function PackageExecuteToLookUp(ByVal commands As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataCommand)) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpPackage)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/QueryExecuteToTableJSON", ReplyAction:="http://tempuri.org/IDataManager/QueryExecuteToTableJSONResponse")>  _
        Function QueryExecuteToTableJSON(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ConnectionStringGet", ReplyAction:="http://tempuri.org/IDataManager/ConnectionStringGetResponse")>  _
        Function ConnectionStringGet(ByVal ConnectionStrinName As String, ByVal companyId As Integer) As InMotionGIT.Common.Services.Contracts.ConnectionString
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ConnectionStringGetAll", ReplyAction:="http://tempuri.org/IDataManager/ConnectionStringGetAllResponse")>  _
        Function ConnectionStringGetAll(ByVal CodeValidator As String, ByVal companyId As Integer) As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.ConnectionString)
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/ConnectionStringUserAndPassword", ReplyAction:="http://tempuri.org/IDataManager/ConnectionStringUserAndPasswordResponse")>  _
        Function ConnectionStringUserAndPassword(ByVal ConectionStringName As String, ByVal companyId As Integer) As InMotionGIT.Common.Services.Contracts.Credential
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/GetDataBaseProvider", ReplyAction:="http://tempuri.org/IDataManager/GetDataBaseProviderResponse")>  _
        Function GetDataBaseProvider(ByVal repositoryName As String) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/GetSettingValue", ReplyAction:="http://tempuri.org/IDataManager/GetSettingValueResponse")>  _
        Function GetSettingValue(ByVal repositoryName As String, ByVal settingName As String) As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/CurrentTime", ReplyAction:="http://tempuri.org/IDataManager/CurrentTimeResponse")>  _
        Function CurrentTime() As String
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/AppInfo", ReplyAction:="http://tempuri.org/IDataManager/AppInfoResponse")>  _
        Function AppInfo(ByVal path As String) As InMotionGIT.Common.Services.Contracts.info
        
        <System.ServiceModel.OperationContractAttribute(Action:="http://tempuri.org/IDataManager/Check", ReplyAction:="http://tempuri.org/IDataManager/CheckResponse")>  _
        Function Check() As System.Collections.Generic.List(Of String)
    End Interface
    
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Public Interface IDataManagerChannel
        Inherits DataManager.IDataManager, System.ServiceModel.IClientChannel
    End Interface
    
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")>  _
    Partial Public Class DataManagerClient
        Inherits System.ServiceModel.ClientBase(Of DataManager.IDataManager)
        Implements DataManager.IDataManager
        
        Public Sub New()
            MyBase.New
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String)
            MyBase.New(endpointConfigurationName)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As String)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal endpointConfigurationName As String, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(endpointConfigurationName, remoteAddress)
        End Sub
        
        Public Sub New(ByVal binding As System.ServiceModel.Channels.Binding, ByVal remoteAddress As System.ServiceModel.EndpointAddress)
            MyBase.New(binding, remoteAddress)
        End Sub
        
        Public Function QueryExecuteToTable(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean, ByVal parameters As System.Collections.Generic.Dictionary(Of String, Object)) As InMotionGIT.Common.Services.Contracts.QueryResult Implements DataManager.IDataManager.QueryExecuteToTable
            Return MyBase.Channel.QueryExecuteToTable(command, resultEmpty, parameters)
        End Function
        
        Public Function QueryExecuteScalar(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal parameters As System.Collections.Generic.Dictionary(Of String, Object)) As Object Implements DataManager.IDataManager.QueryExecuteScalar
            Return MyBase.Channel.QueryExecuteScalar(command, parameters)
        End Function
        
        Public Function QueryExecuteScalarToInteger(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal parameter As System.Collections.Generic.Dictionary(Of String, Object)) As Integer Implements DataManager.IDataManager.QueryExecuteScalarToInteger
            Return MyBase.Channel.QueryExecuteScalarToInteger(command, parameter)
        End Function
        
        Public Function QueryExecuteScalarToDecimal(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Decimal Implements DataManager.IDataManager.QueryExecuteScalarToDecimal
            Return MyBase.Channel.QueryExecuteScalarToDecimal(command)
        End Function
        
        Public Function QueryExecuteToLookup(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpValue) Implements DataManager.IDataManager.QueryExecuteToLookup
            Return MyBase.Channel.QueryExecuteToLookup(command)
        End Function
        
        Public Function QueryExecuteScalarToString(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String Implements DataManager.IDataManager.QueryExecuteScalarToString
            Return MyBase.Channel.QueryExecuteScalarToString(command)
        End Function
        
        Public Function QueryExecuteScalarToDate(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Date Implements DataManager.IDataManager.QueryExecuteScalarToDate
            Return MyBase.Channel.QueryExecuteScalarToDate(command)
        End Function
        
        Public Function CommandExecute(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Integer Implements DataManager.IDataManager.CommandExecute
            Return MyBase.Channel.CommandExecute(command)
        End Function
        
        Public Sub CommandExecuteAsynchronous(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) Implements DataManager.IDataManager.CommandExecuteAsynchronous
            MyBase.Channel.CommandExecuteAsynchronous(command)
        End Sub
        
        Public Function DataStructure(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String Implements DataManager.IDataManager.DataStructure
            Return MyBase.Channel.DataStructure(command)
        End Function
        
        Public Function ResolveStatement(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As String Implements DataManager.IDataManager.ResolveStatement
            Return MyBase.Channel.ResolveStatement(command)
        End Function
        
        Public Function ObjectExist(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As Boolean Implements DataManager.IDataManager.ObjectExist
            Return MyBase.Channel.ObjectExist(command)
        End Function
        
        Public Function ProcedureExecuteToTable(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean) As InMotionGIT.Common.Services.Contracts.QueryResult Implements DataManager.IDataManager.ProcedureExecuteToTable
            Return MyBase.Channel.ProcedureExecuteToTable(command, resultEmpty)
        End Function
        
        Public Function ProcedureExecute(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As InMotionGIT.Common.Services.Contracts.StoredProcedureResult Implements DataManager.IDataManager.ProcedureExecute
            Return MyBase.Channel.ProcedureExecute(command)
        End Function
        
        Public Function ProcedureExecuteResultSchema(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand) As System.Data.DataTable Implements DataManager.IDataManager.ProcedureExecuteResultSchema
            Return MyBase.Channel.ProcedureExecuteResultSchema(command)
        End Function
        
        Public Function PackageExecuteScalar(ByVal commands As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataCommand)) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpPackage) Implements DataManager.IDataManager.PackageExecuteScalar
            Return MyBase.Channel.PackageExecuteScalar(commands)
        End Function
        
        Public Function PackageExecuteToLookUp(ByVal commands As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.DataCommand)) As System.Collections.Generic.List(Of InMotionGIT.Common.DataType.LookUpPackage) Implements DataManager.IDataManager.PackageExecuteToLookUp
            Return MyBase.Channel.PackageExecuteToLookUp(commands)
        End Function
        
        Public Function QueryExecuteToTableJSON(ByVal command As InMotionGIT.Common.Services.Contracts.DataCommand, ByVal resultEmpty As Boolean) As String Implements DataManager.IDataManager.QueryExecuteToTableJSON
            Return MyBase.Channel.QueryExecuteToTableJSON(command, resultEmpty)
        End Function
        
        Public Function ConnectionStringGet(ByVal ConnectionStrinName As String, ByVal companyId As Integer) As InMotionGIT.Common.Services.Contracts.ConnectionString Implements DataManager.IDataManager.ConnectionStringGet
            Return MyBase.Channel.ConnectionStringGet(ConnectionStrinName, companyId)
        End Function
        
        Public Function ConnectionStringGetAll(ByVal CodeValidator As String, ByVal companyId As Integer) As System.Collections.Generic.List(Of InMotionGIT.Common.Services.Contracts.ConnectionString) Implements DataManager.IDataManager.ConnectionStringGetAll
            Return MyBase.Channel.ConnectionStringGetAll(CodeValidator, companyId)
        End Function
        
        Public Function ConnectionStringUserAndPassword(ByVal ConectionStringName As String, ByVal companyId As Integer) As InMotionGIT.Common.Services.Contracts.Credential Implements DataManager.IDataManager.ConnectionStringUserAndPassword
            Return MyBase.Channel.ConnectionStringUserAndPassword(ConectionStringName, companyId)
        End Function
        
        Public Function GetDataBaseProvider(ByVal repositoryName As String) As String Implements DataManager.IDataManager.GetDataBaseProvider
            Return MyBase.Channel.GetDataBaseProvider(repositoryName)
        End Function
        
        Public Function GetSettingValue(ByVal repositoryName As String, ByVal settingName As String) As String Implements DataManager.IDataManager.GetSettingValue
            Return MyBase.Channel.GetSettingValue(repositoryName, settingName)
        End Function
        
        Public Function CurrentTime() As String Implements DataManager.IDataManager.CurrentTime
            Return MyBase.Channel.CurrentTime
        End Function
        
        Public Function AppInfo(ByVal path As String) As InMotionGIT.Common.Services.Contracts.info Implements DataManager.IDataManager.AppInfo
            Return MyBase.Channel.AppInfo(path)
        End Function
        
        Public Function Check() As System.Collections.Generic.List(Of String) Implements DataManager.IDataManager.Check
            Return MyBase.Channel.Check
        End Function
    End Class
End Namespace
