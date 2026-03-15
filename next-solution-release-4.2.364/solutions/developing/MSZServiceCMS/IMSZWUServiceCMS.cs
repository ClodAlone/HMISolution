using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MSZServiceCMS
{
    [ServiceContract]
    public interface IMSZWUServiceCMS : UFProcessServiceCMS.IProcessServiceCMS
    {
        [OperationContract]
        MSZUWResponse Request(MSZUWRequest request);
    }
    [DataContract]
    public class MSZUWRequest
    {
        [DataMember]
        public Guid RequestID;
        [DataMember]
        public String RequestID1;
        [DataMember]
        public String RequestID2;
        [DataMember]
        public String RequestID3;
        [DataMember]
        public string RequestType;
        [DataMember]
        public String Request;
        [DataMember]
        public int ContinuationPoint;
        public MSZUWRequest()
        {
            ContinuationPoint = -1;
        }
    }
    public class MSZUWResponse
    {
        [DataMember]
        public String RequestID1;
        [DataMember]
        public List<String> Response;
        [DataMember]
        public int ContinuationPoint;
        [DataMember]
        public String LastErrorMessage;
        public MSZUWResponse()
        {
            ContinuationPoint = -1;
            Response = new List<string>();
        }
    }
}
