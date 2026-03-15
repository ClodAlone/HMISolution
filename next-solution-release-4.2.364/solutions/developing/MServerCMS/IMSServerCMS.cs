using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using MSModel;

namespace MSServerCMS
{
    [ServiceContract]
    public interface IMSServerCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        bool UpdateScheduler(ChangedSchedArgs sched);
        [OperationContract]
        String GetApplicationName();
    }
    
}
