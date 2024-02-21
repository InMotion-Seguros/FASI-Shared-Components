
namespace InMotionGIT.Common.Domain.DataAccess.Interfaces;


public interface ICommand
{

    ICommandExecution SqlCommand(string query);

    ICommandExecution StoredProc(string storedProcName);

}