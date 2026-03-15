using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace UFProcessServiceCMS
{
    [ServiceContract]
    public interface IProcessServiceCMS
    {
        [OperationContract]
        bool IsStarted();
        [OperationContract]
        bool IsStopping();
        [OperationContract]
        bool IsRunningAsService();
        [OperationContract]
        void StopServer();
        [OperationContract]
        bool IsServerStateRunning();
        [OperationContract]
        String GetServerStatus();
        [OperationContract]
        BalloonInfo GetBalloonMessage(bool bClear);
    }

    [DataContract]
    public class BalloonInfo
    {
        [DataMember]
        public String Message;
        [DataMember]
        public int IconType;
    }
}
