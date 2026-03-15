using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ADServerCMS
{
    [ServiceContract]
    public interface IADServerCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        String GetApplicationName();
    }
}
