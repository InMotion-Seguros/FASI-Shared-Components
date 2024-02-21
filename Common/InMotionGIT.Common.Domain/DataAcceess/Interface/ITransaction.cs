using System;

namespace InMotionGIT.Common.Domain.DataAccess.Interfaces;


public interface ITransaction : IDisposable
{

    void Commit();

    void Rollback();

    ICommandExecution SqlCommand(string query);

    ICommandExecution StoredProc(string storedProcName);

}