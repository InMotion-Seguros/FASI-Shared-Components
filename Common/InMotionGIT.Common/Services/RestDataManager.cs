using System.IO;
using System.ServiceModel.Web;
using System.Text;
using InMotionGIT.Common.Extensions;
using InMotionGIT.Common.Services.Interfaces;

namespace InMotionGIT.Common.Services
{

    public class RestDataManager : IRestDataManager
    {

        public Stream Query(string command)
        {
            string data = "{\"CheckDigit\": \"9\",\"CivilStatus\": 1,\"ClientID\": \"00000006329255\",\"CompleteClientName\": \"SOLER PAZ, NELSON E\",\"FirstName\": \"NELSON E\",\"LastName\": \"SOLER\"}";

            // Dim accept  As string = WebOperationContext.Current.IncomingRequest.Accept
            // Dim userAgentValue As string = WebOperationContext.Current.IncomingRequest.UserAgent

            // Dim response As OutgoingWebResponseContext  = WebOperationContext.Current.OutgoingResponse
            // response.StatusCode = System.Net.HttpStatusCode.UnsupportedMediaType
            // response.StatusDescription = "xxxx"

            if (command.IsNotEmpty())
            {
                {
                    var withBlock = new DataManager();
                    // data = .QueryExecuteToTableJSON(New Common.Services.Contracts.DataCommand With {.ConnectionStringName = "BackOfficeConnectionString",
                    // .Statement = "SELECT * FROM " & command}, True)
                    data = withBlock.QueryExecuteToTableJSON(new Contracts.DataCommand()
                    {
                        ConnectionStringName = "BackOfficeConnectionString",
                        Statement = command
                    }, true);
                    data = data.DecompressString();
                }
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            var result = new MemoryStream(Encoding.UTF8.GetBytes(data));
            return result;
        }

    }

}