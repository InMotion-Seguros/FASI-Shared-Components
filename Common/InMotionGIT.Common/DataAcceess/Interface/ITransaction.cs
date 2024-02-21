using System;

namespace InMotionGIT.Common.DataAccess.Interfaces
{

    public interface ITransaction : IDisposable
    {

        void Commit();

        void Rollback();

        ICommandExecution SqlCommand(string query);

        ICommandExecution StoredProc(string storedProcName);

    }

}