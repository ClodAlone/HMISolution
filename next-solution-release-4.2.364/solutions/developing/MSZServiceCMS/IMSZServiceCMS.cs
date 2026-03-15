using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MSZServiceCMS
{
    [ServiceContract]
    public interface IMSZServiceCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        string Request(MSZRequest request);
    }
    [DataContract]
    public class MSZRequest
    {
        [DataMember]
        public String RequestID;
        [DataMember]
        public String RequestID2;
        [DataMember]
        public String RequestType;
        [DataMember]
        public String Request;
    }
}
