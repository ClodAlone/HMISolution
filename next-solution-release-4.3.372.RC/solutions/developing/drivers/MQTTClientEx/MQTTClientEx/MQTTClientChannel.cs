using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
//using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Net.Security;
//using System.Globalization;
using DriverCodeBaseEx.Helpers;
using System.Threading;

namespace MQTTClient
{
    public enum MQTTClientErrorCodes : int
    {
        ErrorCodeErrorPreparingPublishMessage = 1000,
        ErrorCodeErrorSubscription = 1001,
        ErrorCodeErrorPublish = 1002,
        ErrorCodeErrorMqttClientObjectAllocation = 1003,
        ErrorCodeErrorSubscriptionFailed = 1004,
        ErrorCodeErrorSubscriptionTimeout = 1005,
        ErrorCodeErrorPublishTimeout = 1006,
        ErrorCodeErrorConnectionBroken = 1007,
        ErrorCodeErrorMqttClientObjectInitializationException = 1008,
        ErrorCodeErrorPublishJson = 1009,
        ErrorCodeErrorDataNotAvailable = 1010,
        ErrorCodeErrorBadDecodingError = 1011
    }
    public enum MQTTClientSubscriptionStates : byte
    {
        Todo = 0,
        Pending = 1,
        Done = 2
    }

    class MQTTClientChannel : ChannelList
    {
        #region Constructors

        /// <summary>
        /// Initializes the MQTTClientChannel object.
        /// </summary>
        public MQTTClientChannel(CommunicationDriver commdriver, MQTTClientChannelSettings settings)
            : base(commdriver, settings)
        {
            _MQTTServerHostName = settings.MQTTServerHostName;
            _MQTTServerHostPort = settings.MQTTServerHostPort;
            _MQTTClientIdentifier = settings.MQTTClientIdentifier;
            _MQTTClientUsername = settings.MQTTClientUsername;
            _MQTTClientPassword = settings.MQTTClientPassword;
            _MQTTClientProtocolForSecureConnection = settings.MQTTClientProtocolForSecureConnection;
            _MQTTClientUseSecureConnection = settings.MQTTClientUseSecureConnection;
            _MQTTClientCACertificateFile = settings.MQTTClientCACertificateFile;
            _MQTTClientClientCertificateFile = settings.MQTTClientClientCertificateFile;
            _MQTTClientCleanSession = settings.MQTTClientCleanSession;
            _MQTTClientKeepAliveTime = settings.MQTTClientKeepAliveTime;
            _MQTTClientMaxNumberOfPendingRequests = settings.MQTTClientMaxNumberOfPendingRequests;
            allRepliesReceived = new ManualResetEvent(false);
            mqttChannelDisposed = false;
        }

        #endregion

        #region Abstracts Methods

        public override bool TestChannelComm()
        {
            return DeviceOpen();
        }

        public override DriverErrorCodes CheckDevice(List<CommJob> exjoblist, object thischannel)
        {
            //return conn;
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                {
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                    SetConnectionError();
                }
                else
                {
                    ManageConnectionRestored();
                }
            }

            return conn;
        }

        public override bool IsDeviceOpen()
        {
            lock (lockDevice)
            {
                if (mqttClient == null)
                {
                    return (false);
                }

                if(!mqttClient.IsConnected)
                {
                    ManageConnectionBroken();
                }

                SetStateCommandVariableBit(!mqttClient.IsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (mqttClient.IsConnected);
            }
        }

        public override bool DeviceOpen()
        {
            lock (lockDevice)
            {
                if (mqttClient == null)
                {
                    connectionEstablished = false;

                    // Allocate the MQTTClient object
                    if (_MQTTClientUseSecureConnection == true)
                    {
                        //if((caCertificate == null) && !String.IsNullOrWhiteSpace(_MQTTClientCACertificateFile))
                        //{
                        //    try
                        //    {
                        //        caCertificate = new X509Certificate(_MQTTClientCACertificateFile);
                        //    }
                        //    catch(Exception e)
                        //    {
                        //        string exceptionError = String.Empty;
                        //        exceptionError = String.Format(Properties.Resources.MQTTClientExceptionAllocatingCertificate, e.Message, _MQTTClientCACertificateFile, Name);
                        //        LastErrorMessage = exceptionError;
                        //        return false;
                        //    }
                        //}
                        if ((clientCertificate == null) && !String.IsNullOrWhiteSpace(_MQTTClientClientCertificateFile))
                        {
                            try
                            {
                                clientCertificate = new X509Certificate2(_MQTTClientClientCertificateFile);
                            }
                            catch (Exception e)
                            {
                                string exceptionError = String.Empty;
                                exceptionError = String.Format(Properties.Resources.MQTTClientExceptionAllocatingCertificate, e.Message, _MQTTClientClientCertificateFile, Name);
                                LastErrorMessage = exceptionError;
                                return false;
                            }
                        }
                    }

                    try
                    {
                        mqttClient = new MqttClient(MQTTServerHostName, (int)MQTTServerHostPort, MQTTClientUseSecureConnection, caCertificate, clientCertificate, MQTTClientProtocolForSecureConnection, remCertificateValidationCallback, locCertificateSelectionCallback);
                    }
                    catch (Exception e)
                    {
                        if (lastConnectionErrorCode == MqttMsgConnack.CONN_ACCEPTED)
                        {
                            lastConnectionErrorCode = (uint)MQTTClientErrorCodes.ErrorCodeErrorMqttClientObjectInitializationException;
                            LastErrorMessage = String.Format(Properties.Resources.MQTTClientErrorClientObjectInitializationException, e.Message, Name);
                        }
                        mqttClient = null;
                        return false;
                    }


                    // Registers managers for the events
                    if (mqttClient != null)
                    {
                        mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                        mqttClient.MqttMsgSubscribed += MqttClient_MqttMsgSubscribed;
                        mqttClient.MqttMsgPublished += MqttClient_MqttMsgPublished;
                        lastConnectionErrorCode = MqttMsgConnack.CONN_ACCEPTED;
                    }
                    else
                    {
                        if (lastConnectionErrorCode == MqttMsgConnack.CONN_ACCEPTED)
                        {
                            lastConnectionErrorCode = (uint)MQTTClientErrorCodes.ErrorCodeErrorMqttClientObjectAllocation;
                            LastErrorMessage = String.Format(Properties.Resources.MQTTClientErrorClientObjectAllocation, Name);
                        }
                    }
                }

                if ((mqttClient != null) && !mqttClient.IsConnected)
                {
                    connectionEstablished = false;

                    // Connect to the MQTT broker
                    try
                    {
                        if(String.IsNullOrWhiteSpace(MQTTClientIdentifier))
                        {
                            MQTTClientIdentifier = Guid.NewGuid().ToString();
                        }
                        //if(MQTTClientUseAzureConnection)
                        //{
                        //    string resourceUri = MQTTClientUsername;
                        //    string key = MQTTClientPassword;
                        //    string policyName = String.Empty;
                        //    int expiryInSeconds = MQTTClientKeepAliveTime * 60;
                        //    //if (expiryInSeconds < 3600)
                        //    //{
                        //    //    expiryInSeconds = 3600;
                        //    //}
                        //    MQTTClientPassword = generateSasToken(resourceUri, key, policyName, expiryInSeconds);
                        //}
                        byte connectResult = mqttClient.Connect(MQTTClientIdentifier, MQTTClientUsername, MQTTClientPassword, MQTTClientCleanSession, MQTTClientKeepAliveTime);
                        if (connectResult != MqttMsgConnack.CONN_ACCEPTED)
                        {
                            if (lastConnectionErrorCode == MqttMsgConnack.CONN_ACCEPTED)
                            {
                                string connectionError = String.Empty;
                                switch (connectResult)
                                {
                                    case MqttMsgConnack.CONN_REFUSED_PROT_VERS:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedError1, Name);
                                        break;
                                    case MqttMsgConnack.CONN_REFUSED_IDENT_REJECTED:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedError2, Name);
                                        break;
                                    case MqttMsgConnack.CONN_REFUSED_SERVER_UNAVAILABLE:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedError3, Name);
                                        break;
                                    case MqttMsgConnack.CONN_REFUSED_USERNAME_PASSWORD:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedError4, Name);
                                        break;
                                    case MqttMsgConnack.CONN_REFUSED_NOT_AUTHORIZED:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedError5, Name);
                                        break;
                                    default:
                                        connectionError = String.Format(Properties.Resources.MQTTClientConnectionRefusedErrorUnknown, Name, connectResult);
                                        break;
                                }

                                LastErrorMessage = connectionError;
                            }
                            DeviceClose();
                            return false;
                        }

                        connectionEstablished = true;
                        lastConnectionErrorCode = connectResult;
                        if (connectResult == MqttMsgConnack.CONN_ACCEPTED)
                        {
                            string connectionRestoredMessage = String.Format(Properties.Resources.MQTTClientConnectionEstablished, Name);
                            CommDriver.OnSystemEvent(null, connectionRestoredMessage, EventSeverity.Low);
#if DEBUG
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} DeviceOpen {1}",
                                                               currentTime, connectionRestoredMessage));
#endif
                        }
                    }
                    catch(Exception e)
                    {
                        string connectionError = String.Format(Properties.Resources.MQTTClientConnectionException, e.Message, Name);
                        LastErrorMessage = connectionError;
                        if(lastServerCertificationValidationError != SslPolicyErrors.None)
                        {
                            string validationError = String.Format(Properties.Resources.MQTTClientErrorValidatingServerCertificate, lastServerCertificationValidationError);
                            LastErrorMessage += " ";
                            LastErrorMessage += validationError;
                        }
                        DeviceClose();
                        return false;
                    }
                }

                if (mqttClient == null)
                {
                    return (false);
                }

                if (mqttClient.IsConnected)
                {
                    LastErrorMessage = String.Empty;
                }

                return (mqttClient.IsConnected);
            }
        }

        //static string generateSasToken(string resourceUri, string key, string policyName, int expiryInSeconds = 3600)
        //{
        //    TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
        //    string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + expiryInSeconds);

        //    string stringToSign = WebUtility.UrlEncode(resourceUri).ToLower() + "\n" + expiry;

        //    HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
        //    string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        //    string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", WebUtility.UrlEncode(resourceUri).ToLower(), WebUtility.UrlEncode(signature), expiry);

        //    if (!String.IsNullOrEmpty(policyName))
        //    {
        //        token += "&skn=" + policyName;
        //    }

        //    return token;
        //}

        public override bool DeviceClose()
        {
            lock (lockDevice)
            {
                connectionEstablished = false;
                if (mqttClient != null)
                {
                    if(mqttClient.IsConnected)
                    {
                        mqttClient.Disconnect();
                    }
                }
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return true;
            }
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members

        object lockDevice = new object();
        MqttClient mqttClient;
        uint lastConnectionErrorCode = MqttMsgConnack.CONN_ACCEPTED;
        // Job topics: <Topic name, List of jobs> 
        Dictionary<string, List<MQTTClientCommJob>> mapTopicJobs = new Dictionary<string, List<MQTTClientCommJob>>();
        // Subscription state of topics: <Topic name, Subscription state>
        Dictionary<string, byte> mapTopicSubscriptionState = new Dictionary<string, byte>();
        // Pending subscriptions: <Message ID, array of Topic names>
        Dictionary<ushort, string[]> mapMsgIdTopics = new Dictionary<ushort, string[]>();
        // Pending subscription messages: <Message ID, Sending time>
        Dictionary<ushort, DateTime> mapMsgIdSendingTime = new Dictionary<ushort, DateTime>();
        object lockSubscriptionReplies = new object();
        // Subscriptions replies (SUBACK messages): <Message ID, message>
        Dictionary<ushort, byte[]> mapMsgIdSubAckMessage = new Dictionary<ushort, byte[]>();
        object lockReceivedPublishMessages = new object();
        // Received Publish messages
        List<MqttMsgPublishEventArgs> listReceivedPublishMessages = new List<MqttMsgPublishEventArgs>();
        object lockPublishedMessages = new object();
        // Notifications of published messages
        List<MqttMsgPublishedEventArgs> listPublishedMessages = new List<MqttMsgPublishedEventArgs>();
        // Pending PUBLISH messages: <Message ID, Sending time>
        Dictionary<ushort, DateTime> mapPublishMsgIdSendingTime = new Dictionary<ushort, DateTime>();
        // Pending PUBLISH messages: <Message ID, Job>
        Dictionary<ushort, MQTTClientCommJob> mapPublishMessageIDJob = new Dictionary<ushort, MQTTClientCommJob>();
        bool connectionEstablished = false;
        object lockMQTTClientMaps = new object();
        // Remote certificate
        X509Certificate caCertificate = null;
        // Remote certificate
        X509Certificate clientCertificate = null;
        // Delegate for validating the remote certificate
        RemoteCertificateValidationCallback remCertificateValidationCallback = null;
        // Delegate for selecting the local certificate
        LocalCertificateSelectionCallback locCertificateSelectionCallback = null;
        SslPolicyErrors lastServerCertificationValidationError = SslPolicyErrors.None;
        // Job topics: <Topic name, Last received value> 
        Dictionary<string, byte[]> mapTopicLastReceivedValue = new Dictionary<string, byte[]>();

        ManualResetEvent allRepliesReceived = null;
        object lockMQTTInternalLists = new object();
        List<MQTTClientCommJob> PublishCompletedJobList = new List<MQTTClientCommJob>();
        List<MQTTClientCommJob> PublishErrorJobList = new List<MQTTClientCommJob>();
        List<MQTTClientCommJob> PublishPendingJobList = new List<MQTTClientCommJob>();
        List<MQTTClientCommJob> SubscriptionCompletedJobList = new List<MQTTClientCommJob>();
        List<MQTTClientCommJob> SubscriptionErrorJobList = new List<MQTTClientCommJob>();
        List<MQTTClientCommJob> SubscriptionPendingJobList = new List<MQTTClientCommJob>();
        List<ExecutedJobArgs> SynchronousReadJobList = new List<ExecutedJobArgs>();

        bool mqttChannelDisposed = false;
        #endregion

        #region Override Methods

        public override bool Startup()
        {
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - MQTTClientChannel.Startup - Called", currentTime);
#endif
            if (!bChannelStarted)
            {
                lock (lockThreadObject)
                {
                    mqttChannelDisposed = false;
                    if (allRepliesReceived != null)
                    {
                        allRepliesReceived.Reset();
                    }
                    else
                    {
                        allRepliesReceived = new ManualResetEvent(false);
                    }
                }
            }

            return base.Startup();
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            // Added for compatibility with old projects
            if( MQTTClientMaxNumberOfPendingRequests < 1 )
            {
                MQTTClientMaxNumberOfPendingRequests = 1;
            }

            // sort by station name and last execution time only when more stations are configured
            if (CommDriver.GetChannelStations(this).Count() > 1)
            {
                jobList.OrderBy(c => c.Station.Name).ThenBy(c => c.LastExecutionTime).ToList();
            }

            int jobIndex = 0;
            while (jobIndex < jobList.Count())
            {
                bool first = true;
                string station = string.Empty;
                List<CommJob> list = new List<CommJob>();
                uint numberOfJobsPerList = 0;
                while (jobIndex < jobList.Count())
                {
                    MQTTClientCommJob j = jobList.ElementAt(jobIndex) as MQTTClientCommJob;
                    if (first)
                    {
                        station = j.Station.Name;
                        first = false;
                    }
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - SplitInExecutionLists - Topic {1}", currentTime, j.TagName);
                    }
#endif

                    if (j.Station != null && j.Station.Name == station)
                    {
                        if (j.MustPublish() || (j.MustSubscribe()))
                        {
#if DEBUG
                            {
                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                if (j.MustPublish())
                                {
                                    System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - SplitInExecutionLists - Topic {1} must be published", currentTime, j.TagName);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - SplitInExecutionLists - Topic {1} must be subscribed", currentTime, j.TagName);
                                }
                            }
#endif
                            if(numberOfJobsPerList >= MQTTClientMaxNumberOfPendingRequests)
                            {
                                break;
                            }
                            numberOfJobsPerList++;
                            list.Add(j);
                            jobList.RemoveAt(jobIndex);
                            jobIndex--;
                        }
                    }
                   jobIndex++;
                }
                if(list.Count > 0)
                {
                    exList.Add(list);
                }
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - SplitInExecutionLists - station: {1} - list.Count: {2}", currentTime, station, list.Count);
                }
#endif
            }
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine("MQTTClient DBG - {0} - SplitInExecutionLists - exList.Count: {1}", currentTime, exList.Count);
            }
#endif
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ExecuteJobList - list.Count: {1} - conn: {2}",
                                                   currentTime, list.Count, conn));
#endif
                // back compatibility with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
         
            ExecuteJobList(list);

            // restart check connection timer --> used  
            CyclicCheckReStart();

            return ((GetCountPublishPendingJobList() + GetCountSubscriptionPendingJobList()) > 0);
        }

        public byte GetSubscriptionState(MQTTClientCommJob job)
        {
            if ((job.Type == LinkType.Input) || (job.Type == LinkType.InputOutput))
            {
                //lock (lockSubscriptionMaps)
                lock(lockMQTTClientMaps)
                {
                    if(String.IsNullOrWhiteSpace(job.TagName) == false)
                    {
                        if (!mapTopicJobs.ContainsKey(job.TagName))
                        {
                            List<MQTTClientCommJob> jobList = new List<MQTTClientCommJob>();
                            jobList.Add(job);
                            mapTopicJobs[job.TagName] = jobList;
                        }
                        else if(!mapTopicJobs[job.TagName].Contains(job))
                        {
                            mapTopicJobs[job.TagName].Add(job);
                        }

                        if (!mapTopicSubscriptionState.ContainsKey(job.TagName))
                        {
                            mapTopicSubscriptionState[job.TagName] = (byte)MQTTClientSubscriptionStates.Todo;
                        }

                        return(mapTopicSubscriptionState[job.TagName]);
                    }
                    else
                    {
                        return ((byte)MQTTClientSubscriptionStates.Done);
                    }
                }
            }
            else
            {
                return ((byte)MQTTClientSubscriptionStates.Done);
            }
        }

        public void AddJobToTopicJobsDictionary(MQTTClientCommJob job)
        {
            if ((job.Type == LinkType.Input) || (job.Type == LinkType.InputOutput))
            {
                lock (lockMQTTClientMaps)
                {
                    if (String.IsNullOrWhiteSpace(job.TagName) == false)
                    {
                        if (!mapTopicJobs.ContainsKey(job.TagName))
                        {
                            List<MQTTClientCommJob> jobList = new List<MQTTClientCommJob>();
                            jobList.Add(job);
                            mapTopicJobs[job.TagName] = jobList;
                        }
                        else if (!mapTopicJobs[job.TagName].Contains(job))
                        {
                            mapTopicJobs[job.TagName].Add(job);
                        }
                    }
                }
            }
        }

        protected void SetConnectionError()
        {
            // Set the error state of the jobs
            foreach (MQTTClientStation mStation in CommDriver.GetChannelStations(this))
            {
                mStation.SetConnectionError();
            }
            if (LastErrorCode == DriverErrorCodes.ErrorNoError)
            {
                LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken;
                CommDriver.OnSystemEvent(null, LastErrorMessage, EventSeverity.Low);
            }
            if (StatisticsData != null)
            {
                StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                uint quality;
                String DiagnErrorMessage = String.Empty;
                CommDriver.GetDriverErrorInfo((int)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken, out quality, out DiagnErrorMessage);
                StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
            }
        }

        protected void PublishAndWaitJobVariables(List<MQTTClientCommJob> list)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} PublishAndWaitJobVariables - list.Count: {1}",
                                                   currentTime, list.Count));
            }
#endif
            foreach (var job in list)
            {
                if (PublishAndWaitJobVariable(job) == false)
                {
#if DEBUG
                    {
                        String errorMsg = String.Format(Properties.Resources.MQTTClientErrorPublishing, job.TagName, Name);
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTT DBG - {0} PublishAndWaitJobVariables error: {1} publishing topic: {2}",
                                                            currentTime, errorMsg, job.TagName));
                    }
#endif
                    ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                    ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                    //ProcessError(job, LastErrorCode);
                    AddToPublishErrorJobList(job);
                    //job.SetPublishCompleted();
                    RemoveFromSubscriptionPendingJobList(job);
                }
            }

            // All done?
            if ((GetCountPublishPendingJobList() < 1) && (GetCountSubscriptionPendingJobList() < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} PublishAndWaitJobVariables setting allRepliesReceived",
                                                       currentTime));
                }
#endif
                lock(lockThreadObject)
                {
                    if (allRepliesReceived != null)
                    {
                        allRepliesReceived.Set();
                    }
                }
            }
        }

        protected void SubscribeAndWaitJobVariables(List<MQTTClientCommJob> list)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} SubscribeAndWaitJobVariables - list.Count: {1}",
                                                   currentTime, list.Count));
            }
#endif
            // Prepare the array of topics (without any repetition) and the array of QOS levels
            string[] topics = new string[list.Count];
            byte[] qos = new byte[list.Count];
            int jobIndex = 0;
            object syncObject = new object();
            Parallel.ForEach(list, job =>
            {
                lock (syncObject)
                {
                    if (!topics.Contains(job.TagName))
                    {
                        topics[jobIndex] = job.TagName;
                        qos[jobIndex++] = (byte)job.QualityOfServiceLevel;
                    }
                }
            });

            if (jobIndex > 0)
            {
                // Copy the array of topics and the array of the "Quality of Service" levels 
                string[] topicNames = new string[jobIndex];
                byte[] qosLevels = new byte[jobIndex];
                Parallel.For(0, jobIndex, i =>
                {
                    topicNames[i] = topics[i];
                    qosLevels[i] = qos[i];
                    // Set the state of the subscription for this topic to "pending"
                    lock (lockMQTTClientMaps)
                    {
                        mapTopicSubscriptionState[topicNames[i]] = (byte)MQTTClientSubscriptionStates.Pending;
                    }
                });

                // Subscribe the topics
                ushort msgID = mqttClient.Subscribe(topicNames, qosLevels);

                lock (lockMQTTClientMaps)
                {
                    // Set the Message ID for the subscription of these topics
                    mapMsgIdTopics[msgID] = topicNames;

                    // Set the time stamp of the request of the subscription
                    mapMsgIdSendingTime[msgID] = DateTime.UtcNow;
                }
            }
        }

        private void ExecuteJobList(List<CommJob> list)
        {
            List<MQTTClientCommJob> currExecList = list.ConvertAll(x => (MQTTClientCommJob)x);
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ExecuteJobList - currExecList.Count: {1}",
                                               currentTime, currExecList.Count));
#endif
            // Any job to be performed?
            if (currExecList.Count > 0)
            {
                // Empty Internal job lists
                ClearInternalJobLists();

                // Create the list of jobs that must publish a value and the list of jobs that must subscribe a topic
                List<MQTTClientCommJob> listPublish = new List<MQTTClientCommJob>();
                List<MQTTClientCommJob> listSubscribe = new List<MQTTClientCommJob>();
                foreach (var job in currExecList)
                {
                    if (job != null)
                    {
                        if (job.MustPublish())
                        {
                            listPublish.Add(job);
                        }
                        if (job.MustSubscribe())
                        {
                            listSubscribe.Add(job);
                        }

                        // Special case: the job has not to publish the topic, nor to subscribe it (topic already subscribed)
                        if (!job.MustPublish() && !job.MustSubscribe())
                        {
                            if (job.SyncroExec)
                            {
                                // Syncronous job that must read the value of a topic that has been already subscribed
                                byte[] lastReadValue = GetLastReceivedValue(job.TagName);
                                if (lastReadValue == null)
                                {
                                    // Data not yet received --> Set the job in error
                                    ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                                    ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                                    job.Status = MQTTClientCommJobStatus.Idle;
                                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                                    eJob.Job = job;
                                    eJob.ErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorDataNotAvailable;
                                    OnJobExecuted(eJob);
                                }
                                else
                                {
                                    // Set the last received value
                                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                                    eJob.Job = job;
                                    eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                                    byte[] receivedMessage = new byte[lastReadValue.Length];
                                    Array.Copy(lastReadValue, receivedMessage, lastReadValue.Length);
                                    eJob.Values = receivedMessage;
                                    OnJobExecuted(eJob);
                                }
                            }
                            else
                            {
                                // Nothing to do
                                job.RestoreInitialStatus();
                                RemovePendingJob(job);
                            }
                        }
                    }
                }

                // Reset the event that signals that all the subscription and publish replies have been received
                lock(lockThreadObject)
                {
                    if (allRepliesReceived != null)
                    {
                        allRepliesReceived.Reset();
                    }
                }

                // Fill the lists of pending jobs that must publish or subscribe variables
                if (listPublish.Count > 0)
                {
                    AddRangeToPublishPendingJobList(listPublish);
                }
                if (listSubscribe.Count > 0)
                {
                    AddRangeToSubscriptionPendingJobList(listSubscribe);
                }

                // Publish the values
                if (listPublish.Count > 0)
                {
                    PublishAndWaitJobVariables(listPublish);
                }

                // Subscribe the topics
                if (listSubscribe.Count > 0)
                {
                    SubscribeAndWaitJobVariables(listSubscribe);
                }
            }
        }


        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ProcessNewDataList - list.Count: {1} - conn: {2} - PublishCompletedJobList.Count: {3} - PublishErrorJobList.Count: {4} - SubscriptionCompletedJobList.Count: {5} - SubscriptionErrorJobList.Count: {6} - PublishPendingJobList.Count: {7} - SubscriptionPendingJobList.Count: {8}",
                                                   currentTime, list.Count, conn, GetCountPublishCompletedJobList(), GetCountPublishErrorJobList(), GetCountSubscriptionCompletedJobList(), GetCountSubscriptionErrorJobList(), GetCountPublishPendingJobList(), GetCountSubscriptionPendingJobList()));
            }
#endif

            if (conn != DriverErrorCodes.ErrorNoError)
            {
                foreach (MQTTClientCommJob job in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = job;
                    eJob.ErrorCode = conn;
                    OnJobExecuted(eJob);
                }
            }
            else
            {
                // Manage variables successfully published
                if (GetCountPublishCompletedJobList() > 0)
                {
                    List<MQTTClientCommJob> publishedJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        publishedJobs.AddRange(PublishCompletedJobList);
                    }
                    foreach (MQTTClientCommJob job in publishedJobs)
                    {
                        job.Status = MQTTClientCommJobStatus.PublishReplyReceived;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = job;
                        eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                        OnJobExecuted(eJob);
                    }

                    // Restart check connection timer  
                    CyclicCheckReStart();
                }

                // Manage publishing failures
                if (GetCountPublishErrorJobList() > 0)
                {
                    List<MQTTClientCommJob> failedPublishJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        failedPublishJobs.AddRange(PublishErrorJobList);
                    }
                    foreach (MQTTClientCommJob job in failedPublishJobs)
                    {
                        ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                        ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                        job.Status = MQTTClientCommJobStatus.PublishError;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = job;
                        eJob.ErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPublish;
                        OnJobExecuted(eJob);
                    }
                }

                // Manage variables successfully subscribed
                if (GetCountSubscriptionCompletedJobList() > 0)
                {
                    List<MQTTClientCommJob> subscribedJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        subscribedJobs.AddRange(SubscriptionCompletedJobList);
                    }
                    foreach (MQTTClientCommJob job in subscribedJobs)
                    {
                        // Special case: synchronous reading job that has subscribed the tag and is waiting for its value
                        if (job.SyncroExec && !ContainsPublishCompletedJobList(job) && !ContainsPublishErrorJobList(job))
                        {
                            byte[] lastReadValue = GetLastReceivedValue(job.TagName);
                            if (lastReadValue == null)
                            {
                                // Data not yet received --> Set the job in error
                                ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                                ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                                job.Status = MQTTClientCommJobStatus.Idle;
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.Job = job;
                                eJob.ErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorDataNotAvailable;
                                OnJobExecuted(eJob);
                            }
                            else
                            {
                                // Set the last received value
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.Job = job;
                                eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                                byte[] receivedMessage = new byte[lastReadValue.Length];
                                Array.Copy(lastReadValue, receivedMessage, lastReadValue.Length);
                                eJob.Values = receivedMessage;
                                OnJobExecuted(eJob);
                            }
                        }
                        else
                        {
                            job.RestoreInitialStatus();
                            RemovePendingJob(job);
                        }
                    }

                    // Restart check connection timer  
                    CyclicCheckReStart();
                }

                // Manage subscription failures
                if (GetCountSubscriptionErrorJobList() > 0)
                {
                    List<MQTTClientCommJob> failedSubscriptionJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        failedSubscriptionJobs.AddRange(SubscriptionErrorJobList);
                    }

                    foreach (MQTTClientCommJob job in failedSubscriptionJobs)
                    {
                        ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                        ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                        job.Status = MQTTClientCommJobStatus.Idle;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = job;
                        eJob.ErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionFailed;
                        OnJobExecuted(eJob);
                    }
                }

                // Manage timeout errors
                if (GetCountPublishPendingJobList() > 0)
                {
                    List<MQTTClientCommJob> timedoutPublishJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        timedoutPublishJobs.AddRange(PublishPendingJobList);
                    }

                    // Delete the entries in the dictionaries of pending PUBLISH request jobs
                    lock (lockMQTTClientMaps)
                    {
                        mapPublishMessageIDJob.Clear();
                        mapPublishMsgIdSendingTime.Clear();
                    }

                    // Set in error the jobs
                    DateTime errorTime = DateTime.UtcNow;
                    foreach (MQTTClientCommJob j in timedoutPublishJobs)
                    {
                        j.Status = MQTTClientCommJobStatus.PublishError;
                        j.LastExecutionTime = DateTime.UtcNow;
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorTimeOut, Job = j, LastExecutionTime = errorTime };
                        OnJobExecuted(eJob);
                    }
                }

                if (GetCountSubscriptionPendingJobList() > 0)
                {
                    List<MQTTClientCommJob> timedoutSubscriptionJobs = new List<MQTTClientCommJob>();
                    lock (lockMQTTInternalLists)
                    {
                        timedoutSubscriptionJobs.AddRange(SubscriptionPendingJobList);
                    }

                    // Delete the entries in the dictionaries of pending subscription topics
                    lock (lockMQTTClientMaps)
                    {
                        mapMsgIdTopics.Clear();
                        mapMsgIdSendingTime.Clear();
                    }

                    // Set in error the jobs
                    DateTime errorTime = DateTime.UtcNow;
                    foreach (MQTTClientCommJob j in timedoutSubscriptionJobs)
                    {
                        // Set the subscription state of the topic
                        if (mapTopicSubscriptionState.Keys.Contains(j.TagName))
                        {
                            // Timeout elapsed
                            mapTopicSubscriptionState[j.TagName] = (byte)MQTTClientSubscriptionStates.Todo;
                        }

                        j.LastExecutionTime = DateTime.UtcNow;
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorTimeOut, Job = j, LastExecutionTime = errorTime };
                        OnJobExecuted(eJob);
                    }
                }            
            }

            // Empty the internal job lists
            ClearInternalJobLists();

            // Check and, eventually, reset the bit of the error status in the state/command variable of the stations
            CheckStationErrorStatus();

            return true;
        }

        protected void ManageSubscriptionFailed(string topic, bool timeoutError)
        {
            List<MQTTClientCommJob> jobList = new List<MQTTClientCommJob>();
            //lock (lockSubscriptionMaps)
            lock (lockMQTTClientMaps)
            {
                if (mapTopicJobs.Keys.Contains(topic))
                {
                    jobList.AddRange(mapTopicJobs[topic]);
                }
            }
            foreach(var job in jobList)
            {
                if(timeoutError)
                {
                    ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                    ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                    job.SetError((DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionTimeout);
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageSubscriptionFailed: timeout for the subscription of var {1}",
                                                       currentTime, job.TagName));
#endif
                }
                else
                {
                    ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                    ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                    job.SetError((DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionFailed);
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageSubscriptionFailed: subscription failed for var {1}",
                                                       currentTime, job.TagName));
#endif
                }
            }
        }

        protected void ManageWaitedSubscriptionReplies(MqttMsgSubscribedEventArgs msgSubscribedEventArgs)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ManageWaitedSubscriptionReplies called",
                                                   currentTime));
            }
#endif

            Dictionary<ushort, string[]> mapMsgIdTopicsCopy = null;
            Dictionary<string, byte> mapTopicSubscriptionStateCopy = null;

            // Manage received SUBACK messages
            if (msgSubscribedEventArgs != null)
            {
                ushort msgID = msgSubscribedEventArgs.MessageId;
                byte[] grantedQoSLevels = msgSubscribedEventArgs.GrantedQoSLevels;

                // More topics can be subscribed with just one SUBSCRIBE request
                lock (lockMQTTClientMaps)
                {
                    mapMsgIdTopicsCopy = new Dictionary<ushort, string[]>(mapMsgIdTopics);
                    mapTopicSubscriptionStateCopy = new Dictionary<string, byte>(mapTopicSubscriptionState);
                }
                if (mapMsgIdTopicsCopy.Keys.Contains(msgID) && mapMsgIdTopicsCopy[msgID].Count() == grantedQoSLevels.Count())
                {
                    string[] topicArray = mapMsgIdTopicsCopy[msgID];
                    Parallel.For(0, topicArray.Count(), i =>
                    {
                        if (mapTopicSubscriptionStateCopy.Keys.Contains(topicArray[i]))
                        {
                            if ((grantedQoSLevels[i] & 0x80) == 0)
                            {
                                // Success
                                lock (lockMQTTClientMaps)
                                {
                                    mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Done;
                                    List<MQTTClientCommJob> topicJobList = GetTopicJobList(topicArray[i]);
                                    if (topicJobList != null)
                                    {
#if DEBUG
                                        {
                                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedSubscriptionReplies - Found {1} jobs for topic {2}",
                                                                               currentTime, topicJobList.Count(), topicArray[i]));
                                        }
#endif
                                        foreach (MQTTClientCommJob job in topicJobList)
                                        {
                                            //job.RestoreInitialStatus();
                                            AddToSubscriptionCompletedJobList(job);
                                            //job.SetSubscriptionCompleted();
                                            RemoveFromSubscriptionPendingJobList(job);
                                        }
                                    }
#if DEBUG
                                    else
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedSubscriptionReplies - Jobs not found for topic {1}",
                                                                           currentTime, topicArray[i]));
                                    }

                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedSubscriptionReplies: subscription successfull for topic {1}",
                                                                           currentTime, topicArray[i]));
                                    }
#endif
                                }
                            }
                            else
                            {
                                lock (lockMQTTClientMaps)
                                {
                                    // Subscription failed
                                    mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Todo;
                                    List<MQTTClientCommJob> topicJobList = GetTopicJobList(topicArray[i]);
                                    if (topicJobList != null)
                                    {
                                        AddRangeToSubscriptionErrorJobList(topicJobList);
                                        foreach (MQTTClientCommJob job in topicJobList)
                                        {
                                            RemoveFromSubscriptionPendingJobList(job);
                                        }
                                    }
#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedSubscriptionReplies: subscription failed for topic {1}",
                                                                           currentTime, topicArray[i]));
                                    }
#endif
                                }
                            }
                        }
                    });
                }
                mapMsgIdTopicsCopy.Clear();
                mapMsgIdTopicsCopy = null;
            }

            // All done?
            if((GetCountPublishPendingJobList() < 1) && (GetCountSubscriptionPendingJobList() < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedSubscriptionReplies setting allRepliesReceived",
                                                       currentTime));
                }
#endif
                lock(lockThreadObject)
                {
                    if (allRepliesReceived != null)
                    {
                        allRepliesReceived.Set();
                    }
                }
            }
        }

        protected void ManageWaitedPublishReplies(MqttMsgPublishedEventArgs evArg)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ManageWaitedPublishReplies called",
                                                   currentTime));
            }
#endif

            // Make a copy of the pending PUBLISH messages
            Dictionary<ushort, MQTTClientCommJob> mapPublishMessageIDJobCopy = null;
            lock (lockMQTTClientMaps)
            {
                mapPublishMessageIDJobCopy = new Dictionary<ushort, MQTTClientCommJob>(mapPublishMessageIDJob);
            }

            // Any notification of published messages?
            if (evArg != null)
            {
                if (mapPublishMessageIDJobCopy.Keys.Contains(evArg.MessageId))
                {
                    // Get the job waiting for the PUBLISH confirmation
                    MQTTClientCommJob job = mapPublishMessageIDJobCopy[evArg.MessageId];
                    if (evArg.IsPublished == true)
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedPublishReplies: received notification IsPublished == true for var {1}",
                                                               currentTime, job.TagName));
                        }
#endif

                        // Publish operation successfully completed
                        job.LastExecutionTime = DateTime.UtcNow;
                        job.Status = MQTTClientCommJobStatus.PublishReplyReceived;
                        AddToPublishCompletedJobList(job);
                    }
                    else
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedPublishReplies: received notification IsPublished == false for var {1}",
                                                               currentTime, job.TagName));
                        }
#endif
                        // Error publishing the value
                        String errorMsg = String.Format(Properties.Resources.MQTTClientErrorPublishing, job.TagName, Name);
                        ((MQTTClientDriver)CommDriver).LastChannelInError = Name;
                        ((MQTTClientDriver)CommDriver).LastTagInError = job.TagName;
                        AddToPublishErrorJobList(job);
                    }
                    //job.SetPublishCompleted();
                    RemoveFromPublishPendingJobList(job);

                    // Remove the message ID from the maps of the pending PUBLISH messages
                    lock (lockMQTTClientMaps)
                    {
                        mapPublishMessageIDJob.Remove(evArg.MessageId);
                        mapPublishMsgIdSendingTime.Remove(evArg.MessageId);
                    }
                }
            }

            // All done?
            if ((GetCountPublishPendingJobList() < 1) && (GetCountSubscriptionPendingJobList() < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageWaitedPublishReplies setting allRepliesReceived",
                                                       currentTime));
                }
#endif
                if(allRepliesReceived != null)
                {
                    allRepliesReceived.Set();
                }
            }
        }

        public override void ResetNewDataEvent()
        {
            // Do nothing
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            if(allRepliesReceived != null)
            {
                // Wait for receiving replies to all the pending publish/subscription requests
                if (!allRepliesReceived.WaitOne(Timeout))
                {
                    //Timeout
                    lock (lockThreadObject)
                    {
                        conn = DriverErrorCodes.ErrorTimeOut;
                        ReceiveBuffer.Clear();
                    }
                }
                else
                {
                    conn = DriverErrorCodes.ErrorNoError;
                    lock (lockThreadObject)
                    {
                        if (allRepliesReceived != null)
                        {
                            allRepliesReceived.Reset();
                        }
                    }
                }
            }
        }

        protected List<MQTTClientCommJob> GetTopicJobList(string topic)
        {
            List<MQTTClientCommJob> jobList = null;
            lock (lockMQTTClientMaps)
            {
                if (mapTopicJobs.Keys.Contains(topic))
                {
                    jobList = mapTopicJobs[topic];
                }
            }

            return (jobList);
        }

        protected void ManageReceivedPublishMessages(MqttMsgPublishEventArgs msgPublishEventArgs)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ManageReceivedPublishMessages called",
                                                   currentTime));
            }
#endif

            // Manage the received PUBLISH message
            if (msgPublishEventArgs != null)
            {
                // Update statistics
                if (StatisticsData != null)
                {
                    lock (lockStatisic)
                    {
                        DiagnLastTaskRxBytes += msgPublishEventArgs.Message.Length;
                    }
                }

                // Set the last received value for the topic
                string topic = msgPublishEventArgs.Topic;
                byte[] lastReceivedMessage = new byte[msgPublishEventArgs.Message.Length];
                Array.Copy(msgPublishEventArgs.Message, lastReceivedMessage, msgPublishEventArgs.Message.Length);
                SetLastReceivedValue(topic, lastReceivedMessage);

                // Get the list of jobs associated to the topic of the received value
                List<MQTTClientCommJob> jobList = GetTopicJobList(topic);

#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - ManageReceivedPublishMessages - jobList.Count: {1}",
                                                       currentTime, jobList.Count));
                }
#endif

                DateTime ExecutionTime = DateTime.UtcNow;
                foreach(MQTTClientCommJob mJob in jobList)
                {
                    if (mJob.Status != MQTTClientCommJobStatus.PublishRequestPending)
                    {
                        if (!mJob.SyncroExec)
                        {
                            mJob.LastExecutionTime = ExecutionTime;
                            mJob.StartExecutionTime = ExecutionTime;
                            ExecuteJob(mJob);
                            // Update the tag of a job associated to the topic with the received value
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = mJob;
                            eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                            byte[] receivedMessage = new byte[msgPublishEventArgs.Message.Length];
                            Array.Copy(msgPublishEventArgs.Message, receivedMessage, msgPublishEventArgs.Message.Length);
                            eJob.Values = receivedMessage;
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            // restart check connection timer --> used  
            CyclicCheckReStart();
        }

        #endregion

        #region Methods

        void SetLastReceivedValue(string topic, byte[] lastReceivedValue)
        {
            lock (lockMQTTClientMaps)
            {
                mapTopicLastReceivedValue[topic] = lastReceivedValue;
            }
        }

        byte[] GetLastReceivedValue(string topic)
        {
            lock (lockMQTTClientMaps)
            {
                if(mapTopicLastReceivedValue.Keys.Contains(topic))
                {
                    return (mapTopicLastReceivedValue[topic]);
                }
                else
                {
                    return (null);
                }
            }
        }

        void ClearInternalJobLists()
        {
            lock (lockMQTTInternalLists)
            {
                PublishPendingJobList.Clear();
                PublishCompletedJobList.Clear();
                PublishErrorJobList.Clear();
                SubscriptionPendingJobList.Clear();
                SubscriptionCompletedJobList.Clear();
                SubscriptionErrorJobList.Clear();
                SynchronousReadJobList.Clear();
            }
        }

        void AddRangeToPublishPendingJobList(List<MQTTClientCommJob> jobList)
        {
            lock (lockMQTTInternalLists)
            {
                PublishPendingJobList.AddRange(jobList);
            }
        }

        void RemoveFromPublishPendingJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                PublishPendingJobList.Remove(job);
            }
        }

        int GetCountPublishPendingJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return (PublishPendingJobList.Count);
            }
        }

        void AddRangeToSubscriptionPendingJobList(List<MQTTClientCommJob> jobList)
        {
            lock (lockMQTTInternalLists)
            {
                SubscriptionPendingJobList.AddRange(jobList);
            }
        }

        void RemoveFromSubscriptionPendingJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                SubscriptionPendingJobList.Remove(job);
            }
        }

        int GetCountSubscriptionPendingJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return (SubscriptionPendingJobList.Count);
            }
        }

        void AddToPublishCompletedJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                PublishCompletedJobList.Add(job);
            }
        }

        int GetCountPublishCompletedJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return(PublishCompletedJobList.Count);
            }
        }

        bool ContainsPublishCompletedJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                return (PublishCompletedJobList.Contains(job));
            }
        }

        void AddToPublishErrorJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                PublishErrorJobList.Add(job);
            }
        }

        int GetCountPublishErrorJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return (PublishErrorJobList.Count);
            }
        }

        bool ContainsPublishErrorJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                return (PublishErrorJobList.Contains(job));
            }
        }

        void AddToSubscriptionCompletedJobList(MQTTClientCommJob job)
        {
            lock (lockMQTTInternalLists)
            {
                SubscriptionCompletedJobList.Add(job);
            }
        }

        int GetCountSubscriptionCompletedJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return (SubscriptionCompletedJobList.Count);
            }
        }

        void AddRangeToSubscriptionErrorJobList(List<MQTTClientCommJob> jobList)
        {
            lock (lockMQTTInternalLists)
            {
                SubscriptionErrorJobList.AddRange(jobList);
            }
        }

        int GetCountSubscriptionErrorJobList()
        {
            lock (lockMQTTInternalLists)
            {
                return (SubscriptionErrorJobList.Count);
            }
        }

        #endregion

        #region Event handlers

        // Handles PUBLISH messages (new data) received from the broker 
        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            ManageReceivedPublishMessages(e);
        }

        // Handles replies to the SUBSCRIBE requests
        private void MqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MqttClient_MqttMsgSubscribed calling ManageWaitedSubscriptionReplies",
                                                    currentTime));
            }
#endif
            ManageWaitedSubscriptionReplies(e);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MqttClient_MqttMsgSubscribed after ManageWaitedSubscriptionReplies",
                                                    currentTime));
            }
#endif
        }

        // Handles replies to the PUBLISH requests
        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MqttClient_MqttMsgPublished calling ManageWaitedPublishReplies",
                                                       currentTime));
            }
#endif
            //ManagePublishedReplies(e);
            ManageWaitedPublishReplies(e);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MqttClient_MqttMsgPublished after ManageWaitedPublishReplies",
                                                   currentTime));
            }
#endif
        }
        #endregion

        #region Connection's check timer management                
        protected override bool CyclicCheck()
        {
            CheckConnection();

            return true;
        }        

        public override void Suspend()
        {
            base.Suspend();

            // Reset the subscription state of topics
            lock(lockMQTTClientMaps)
            {
                if(mapTopicSubscriptionState != null)
                {
                    mapTopicSubscriptionState.Clear();
                }
            }

            // Empty internal job lists
            ClearInternalJobLists();
        }

        #endregion

        #region Properties

        private string _MQTTServerHostName;
        public string MQTTServerHostName
        {
            get
            {
                return _MQTTServerHostName;
            }
            set
            {
                _MQTTServerHostName = value;
            }
        }

        private uint _MQTTServerHostPort;
        public uint MQTTServerHostPort
        {
            get
            {
                return _MQTTServerHostPort;
            }
            set
            {
                _MQTTServerHostPort = value;
            }
        }

        private string _MQTTClientIdentifier;
        public string MQTTClientIdentifier
        {
            get
            {
                return _MQTTClientIdentifier;
            }
            set
            {
                _MQTTClientIdentifier = value;
            }
        }

        private string _MQTTClientUsername;
        public string MQTTClientUsername
        {
            get
            {
                return _MQTTClientUsername;
            }
            set
            {
                _MQTTClientUsername = value;
            }
        }

        private string _MQTTClientPassword;
        public string MQTTClientPassword
        {
            get
            {
                return _MQTTClientPassword;
            }
            set
            {
                _MQTTClientPassword = value;
            }
        }

        private bool _MQTTClientUseSecureConnection;
        public bool MQTTClientUseSecureConnection
        {
            get
            {
                return _MQTTClientUseSecureConnection;
            }
            set
            {
                _MQTTClientUseSecureConnection = value;
            }
        }

        private MqttSslProtocols _MQTTClientProtocolForSecureConnection;
        public MqttSslProtocols MQTTClientProtocolForSecureConnection
        {
            get
            {
                return _MQTTClientProtocolForSecureConnection;
            }
            set
            {
                _MQTTClientProtocolForSecureConnection = value;
            }
        }

        private string _MQTTClientCACertificateFile;
        public string MQTTClientCACertificateFile
        {
            get
            {
                return _MQTTClientCACertificateFile;
            }
            set
            {
                _MQTTClientCACertificateFile = value;
            }
        }

        private string _MQTTClientClientCertificateFile;
        public string MQTTClientClientCertificateFile
        {
            get
            {
                return _MQTTClientClientCertificateFile;
            }
            set
            {
                _MQTTClientClientCertificateFile = value;
            }
        }

        private bool _MQTTClientCleanSession;
        public bool MQTTClientCleanSession
        {
            get
            {
                return _MQTTClientCleanSession;
            }
            set
            {
                _MQTTClientCleanSession = value;
            }
        }

        private UInt16 _MQTTClientKeepAliveTime;
        public UInt16 MQTTClientKeepAliveTime
        {
            get
            {
                return _MQTTClientKeepAliveTime;
            }
            set
            {
                _MQTTClientKeepAliveTime = value;
            }
        }

        private uint _MQTTClientMaxNumberOfPendingRequests;
        public uint MQTTClientMaxNumberOfPendingRequests
        {
            get
            {
                return _MQTTClientMaxNumberOfPendingRequests;
            }
            set
            {
                _MQTTClientMaxNumberOfPendingRequests = value;
            }
        }

        //private bool _MQTTClientUseAzureConnection;
        //public bool MQTTClientUseAzureConnection
        //{
        //    get
        //    {
        //        return _MQTTClientUseAzureConnection;
        //    }
        //    set
        //    {
        //        _MQTTClientUseAzureConnection = value;
        //    }
        //}

        #endregion

        #region methods

        public bool PublishAndWaitJobVariable(CommJob job)
        {
            MQTTClientCommJob mj = (MQTTClientCommJob)job;
            if (mj == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishAndWaitJobVariable error 1");
#endif
                // It should never return from here
                return (false);
            }

            if (mj.Status == MQTTClientCommJobStatus.PublishRequestPending)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishAndWaitJobVariable warning 1");
#endif
                // It should never return from here
                return (true);
            }

            DateTime ExecutionTime = DateTime.UtcNow;
            job.LastExecutionTime = ExecutionTime;
            job.StartExecutionTime = ExecutionTime;
            ExecuteJob(job);
            MQTTClientStation mqttStation = (MQTTClientStation)job.Station;
            if (job.GetTagListOnWritingCount() == 0)
            {
                mj.Status = MQTTClientCommJobStatus.PublishError;
                //AddToPublishErrorJobList(mj);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishAndWaitJobVariable error 2");
#endif
                LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                return (false);
            }
            // For the JSon Format be sure to publish all the elements of a structure (all the tags of the jobs)
            if ((job.Type != DriverCodeBaseEx.Enumerators.LinkType.UnconditionalOutput) && (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.JSON))
            {
                job.ClearTagListOnWriting();
                job.FillWholeTagsListOnWriting();
            }

            // Get the message to be published
            string messageToBePublished = mj.GetPublishMessagePayload();

            if (String.IsNullOrWhiteSpace(messageToBePublished) == false)
            {
                // Publish the message.
                try
                {
                    ushort msgID = mqttClient.Publish(mj.TagName, Encoding.UTF8.GetBytes(messageToBePublished), (byte)mj.QualityOfServiceLevel, mj.Retained);

                    // If the hysteresis could be applied to the job, store the last published value
                    if ((mj.HysteresysCanBeApplied()))
                    {
                        ((MQTTClientTag)mj.TagsList[0]).LastPublishedValue = job.TagsList[0].Value.Value;
                    }

                    if (StatisticsData != null)
                    {
                        lock (lockStatisic)
                        {
                            DiagnLastTaskTxBytes += messageToBePublished.Length;
                        }
                    }

                    if (mj.QualityOfServiceLevel == QualityOfServiceLevels.AtMostOnce_0)
                    {
                        // Acnowledge not requested
                        job.LastExecutionTime = DateTime.UtcNow;
                        job.StartExecutionTime = DateTime.UtcNow;
                        mj.Status = MQTTClientCommJobStatus.PublishReplyReceived;
                        AddToPublishCompletedJobList(mj);
                        //mj.SetPublishCompleted();
                        RemoveFromPublishPendingJobList(mj);
                    }
                    else
                    {
                        //lock(lockPendingPublishMaps)
                        lock (lockMQTTClientMaps)
                        {
                            mj.Status = MQTTClientCommJobStatus.PublishRequestPending;
                            // Set the time stamp of the request of the subscription
                            mapPublishMsgIdSendingTime[msgID] = DateTime.UtcNow;
                            // Associate the publish message to the job
                            mapPublishMessageIDJob[msgID] = mj;
                        }
                    }
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} PublishAndWaitJobVariable: published var {1} with QOS {2}",
                                                            currentTime, mj.TagName, mj.QualityOfServiceLevel));
                    }
#endif
                }
                catch (Exception e)
                {
                    job.LastExecutionTime = DateTime.UtcNow;
                    mj.Status = MQTTClientCommJobStatus.PublishError;
                    String errorMsg = String.Format(Properties.Resources.MQTTClientExceptionPublishing, e.Message, mj.TagName, Name);
                    CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.High);
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} {1}",
                                                        currentTime, errorMsg));
#endif
                    return (false);
                }
            }
            else
            {
                job.LastExecutionTime = DateTime.UtcNow;
                mj.Status = MQTTClientCommJobStatus.PublishError;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishAndWaitJobVariable error 3");
#endif
                LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                return (false);
            }

            return (true);
        }

        void CheckStationErrorStatus()
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - CheckStationErrorStatus called",
                                                   currentTime));
            }
#endif
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                MQTTClientStation mqttStation = (MQTTClientStation)station;
                bool inErrorStatus = false;
                if (station.GetStateCommandVariableBit(ref inErrorStatus, (UInt16)StationVariableBits.StationErrorState))
                {
                    if (inErrorStatus == true)
                    {
                        if (mqttStation.NumberOfJobsInError() == 0)
                        {
                            station.SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                    else
                    {
                        if (mqttStation.NumberOfJobsInError() > 0)
                        {
                            station.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                }
            }
        }

        void ManageConnectionRestored()
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageConnectionRestored Called",
                                                   currentTime));
            }
#endif
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);

            // Set the quality of the jobs to uncertain
            foreach (MQTTClientStation mStation in CommDriver.GetChannelStations(this))
            {
                mStation.ManageConnectionRestored();
            }
        }

        void ManageConnectionBroken()
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageConnectionBroken Called",
                                                   currentTime));
            }
#endif
            if (connectionEstablished)
            {
                connectionEstablished = false;
                // Empty subscription maps and lists
                lock (lockMQTTClientMaps)
                {
                    if (mapTopicSubscriptionState.Count > 0)
                    {
                        mapTopicSubscriptionState.Clear();
                    }
                    if (mapMsgIdTopics.Count > 0)
                    {
                        mapMsgIdTopics.Clear();
                    }
                    if (mapMsgIdSendingTime.Count > 0)
                    {
                        mapMsgIdSendingTime.Clear();
                    }

                    if (mapPublishMsgIdSendingTime.Count > 0)
                    {
                        mapPublishMsgIdSendingTime.Clear();
                    }
                    if (mapPublishMessageIDJob.Count > 0)
                    {
                        mapPublishMessageIDJob.Clear();
                    }
                    if(mapTopicLastReceivedValue.Count > 0)
                    {
                        mapTopicLastReceivedValue.Clear();
                    }
                }
                lock (lockSubscriptionReplies)
                {
                    if (mapMsgIdSubAckMessage.Count > 0)
                    {
                        mapMsgIdSubAckMessage.Clear();
                    }
                }

                // Empty PUBLISH messages maps and lists
                lock (lockReceivedPublishMessages)
                {
                    if (listReceivedPublishMessages.Count > 0)
                    {
                        listReceivedPublishMessages.Clear();
                    }
                }
                lock (lockPublishedMessages)
                {
                    if (listPublishedMessages.Count > 0)
                    {
                        listPublishedMessages.Clear();
                    }
                }

                // Set the error state of the jobs
                foreach (MQTTClientStation mStation in CommDriver.GetChannelStations(this))
                {
                    mStation.SetConnectionError();
                }

                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            }
        }

        public DriverErrorCodes CheckConnection()
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} CheckConnection Called",
                                                   currentTime));
            }
#endif
            //return conn;
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                {
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                    SetConnectionError();
                }
                else
                {
                    ManageConnectionRestored();
                }
            }

            // Check and, eventually, reset the bit of the error status in the state/command variable of the stations
            CheckStationErrorStatus();

            return conn;
        }

        #endregion

        #region Security Delegates

        //public bool caCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    if (certificate == null)
        //    {
        //        return (false);
        //    }

        //    bool certificateValidated = false;
        //    lastServerCertificationValidationError = sslPolicyErrors;

        //    if (sslPolicyErrors == SslPolicyErrors.None)
        //    {
        //        certificateValidated = true;
        //    }
        //    else
        //    {
        //    }

        //    return (certificateValidated);
        //}

        //public X509Certificate clientCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        //{
        //    if( (acceptableIssuers != null) &&
        //        (acceptableIssuers.Length > 0) &&
        //        (localCertificates != null) &&
        //        (localCertificates.Count > 0))
        //    {
        //        foreach(X509Certificate certificate in localCertificates)
        //        {
        //            string issuer = certificate.Issuer;
        //            if(Array.IndexOf(acceptableIssuers, issuer) != -1)
        //            {
        //                return (certificate);
        //            }
        //        }
        //    }
        //    if ((localCertificates != null) && (localCertificates.Count > 0))
        //    {
        //        return (localCertificates[0]);
        //    }
        //    else
        //    {
        //        return (null);
        //    }
        //}
        #endregion

        #region IDisposable

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            if (mqttChannelDisposed)
                return;
            mqttChannelDisposed = true;

            ManualResetEvent allrepliesreceived = null;
            lock (lockThreadObject)
                allrepliesreceived = allRepliesReceived;                      
            if (allrepliesreceived != null)
                allrepliesreceived.Set();

            base.Dispose();
            lock (lockThreadObject)
            {
                if (allRepliesReceived != null)
                {
                    allRepliesReceived.Dispose();
                    allRepliesReceived = null;
                }
            }

            // Close the connection with the broker
            if (connectionEstablished == true)
            {
                DeviceClose();
            }

            // Empty internal job lists
            ClearInternalJobLists();

            // Unregister managers for the events
            if (mqttClient != null)
            {
                mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
                mqttClient.MqttMsgSubscribed -= MqttClient_MqttMsgSubscribed;
                mqttClient.MqttMsgPublished -= MqttClient_MqttMsgPublished;
            }
            mqttClient = null;
        }
    #endregion

    }
}
