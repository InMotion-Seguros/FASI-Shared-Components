using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace InMotionGIT.Common.Services.Interfaces
{

    [ServiceContract()]
    public interface IRestDataManager
    {

        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [OperationContract()]
        [Description("Ramos activos.")]
        Stream Query(string command);

    }

}