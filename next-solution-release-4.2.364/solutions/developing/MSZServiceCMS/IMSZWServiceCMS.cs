using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MSZServiceCMS
{
    [ServiceContract]
    public interface IMSZWServiceCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        string Request(MSZWRequest request);
    }
    [DataContract]
    public class MSZWRequest
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
