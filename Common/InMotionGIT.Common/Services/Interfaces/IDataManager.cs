using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using InMotionGIT.Common.Services.Contracts;

namespace InMotionGIT.Common.Services.Interfaces
{

    [ServiceContract()]
    public interface IDataManager
    {

        [OperationContract()]
        QueryResult QueryExecuteToTable(DataCommand command, bool resultEmpty);

        [OperationContract()]
        object QueryExecuteScalar(DataCommand command);

        [OperationContract()]
        int QueryExecuteScalarToInteger(DataCommand command);

        [OperationContract()]
        decimal QueryExecuteScalarToDecimal(DataCommand command);

        [OperationContract]
        List<DataType.LookUpValue> QueryExecuteToLookup(DataCommand command);

        [OperationContract()]
        string QueryExecuteScalarToString(DataCommand command);

        [OperationContract()]
        DateTime QueryExecuteScalarToDate(DataCommand command);

        [OperationContract()]
        int CommandExecute(DataCommand command);

        [OperationContract(IsOneWay = true)]
        void CommandExecuteAsynchronous(DataCommand command);

        [OperationContract()]
        string DataStructure(DataCommand command);

        [OperationContract()]
        string ResolveStatement(DataCommand command);

        [OperationContract()]
        bool ObjectExist(DataCommand command);

        [OperationContract()]
        QueryResult ProcedureExecuteToTable(DataCommand command, bool resultEmpty);

        [OperationContract()]
        StoredProcedureResult ProcedureExecute(DataCommand command);

        [OperationContract()]
        DataTable ProcedureExecuteResultSchema(DataCommand command);

        [OperationContract()]
        List<DataType.LookUpPackage> PackageExecuteScalar(List<DataCommand> commands);

        [OperationContract()]
        List<DataType.LookUpPackage> PackageExecuteToLookUp(List<DataCommand> commands);

        [OperationContract()]
        string QueryExecuteToTableJSON(DataCommand command, bool resultEmpty);

        [OperationContract()]
        ConnectionStrings ConnectionStringGet(string ConnectionStrinName, int companyId);

        [OperationContract()]
        List<ConnectionStrings> ConnectionStringGetAll(string CodeValidator, int companyId);

        [OperationContract()]
        Credential ConnectionStringUserAndPassword(string ConectionStringName, int companyId);

        [OperationContract()]
        string GetDataBaseProvider(string repositoryName);

        [OperationContract()]
        string GetSettingValue(string repositoryName, string settingName);

        [OperationContract()]
        string CurrentTime();

        [OperationContract()]
        info AppInfo(string path);

        [OperationContract()]
        List<string> Check(Dictionary<string, string> parameters);

    }

}