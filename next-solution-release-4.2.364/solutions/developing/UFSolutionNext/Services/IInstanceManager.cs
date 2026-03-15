using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services.InstanceManagerSvc
{
    [ServiceContract]
    public interface IInstanceManager
    {
        [OperationContract]
        void ActivateApplication(String[] commands);
    }
}
