using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace UFWebClient.HTML5
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AlarmSinkService
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void SetClientTimeOffset(int clientoffset)
        {
            global::AlarmProvider.SetClientTimeOffset(clientoffset);
        }
        [OperationContract]
        public int GetClientTimeOffset()
        {
            return global::AlarmProvider.GetClientTimeOffset();
        }
        [OperationContract]
        public bool IsConnected()
        {
            return global::AlarmProvider.IsConnected();
        }

        [OperationContract]
        public void AckAll()
        {
            global::AlarmProvider.AckAll();
        }

        [OperationContract]
        public void ConfirmAll()
        {
            global::AlarmProvider.ConfirmAll();
        }

        [OperationContract]
        public void AckSelected(String[] list)
        {
            global::AlarmProvider.AckSelected(list);
        }

        [OperationContract]
        public void ConfirmSelected(String[] list)
        {
            global::AlarmProvider.ConfirmSelected(list);
        }

        [OperationContract]
        public void Refresh()
        {
            global::AlarmProvider.Refresh();
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
