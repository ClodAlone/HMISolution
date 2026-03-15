using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace UFUAServerCMS
{
    [ServiceContract]
    public interface IUFUAServerCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        bool EnableScriptDebugging(Guid id, bool bEnable);
        [OperationContract]
        List<String> ScriptSynchronizing(Guid id, List<String> data);
        [OperationContract]
        String GetAlertMessage(bool bClear);
        [OperationContract]
        String GetApplicationName();

        [OperationContract]
        bool IsActiveServer();

    }
}
