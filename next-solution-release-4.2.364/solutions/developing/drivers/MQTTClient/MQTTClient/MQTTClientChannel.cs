using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
//using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Net.Security;
//using System.Globalization;
using DriverCodeBase.Helpers;

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
        ErrorCodeErrorPublishJson = 1009
    }
    public enum MQTTClientSubscriptionStates : byte
    {
        Todo = 0,
        Pending = 1,
        Done = 2
    }

    class MQTTClientChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the MQTTClientChannel object.
        /// </summary>
        public MQTTClientChannel(CommunicationDriver commdriver, MQTTClientChannelSettings settings)
            : base(commdriver, settings, false)
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
            //remCertificateValidationCallback = caCertificateValidationCallback;
            //locCertificateSelectionCallback = clientCertificateSelectionCallback;
            //_MQTTClientUseAzureConnection = settings.MQTTClientUseAzureConnection;
        }

        #endregion

        #region Abstracts Methods

        public override bool TestChannelComm()
        {
            return DeviceOpen();
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
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTT DBG - {0} DeviceOpen {1}",
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
        //object lockSubscriptionMaps = new object();
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
        //object lockPendingPublishMaps = new object();
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

        #endregion

        #region Override Methods

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
            {
                sleepCycle = 1;
            }

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            List<MQTTClientCommJob> nextJobList = new List<MQTTClientCommJob>();

            while (true)
            {
                nextJobList.Clear();
                ScheduleListJob();
                lock (lockThreadObject)
                {
                    if (SynchroJob != null)
                    {
                        nextJobList.Add(SynchroJob as MQTTClientCommJob);
                    }
                    else
                    {
                        GetNextMQTTClientPendingJobList(ref nextJobList);
                    }
                }

                // Even if there isn't any active job, check if the connection to the broker is on
                IsDeviceOpen();

                if (nextJobList.Count > 0)
                {
                    if (!IsDeviceOpen())
                    {
                        if (!DeviceOpen())
                        {
                            SetConnectionError();
                        }
                        else
                        {
                            lock (lockThreadObject)
                            {
                                ManageConnectionRestored();
                            }
                        }
                    }
                    if (IsDeviceOpen())
                    {
                        lock (lockThreadObject)
                        {
                            // Publish and subscribe topics
                            ExecuteJobList(nextJobList);
                        }
                    }
                }

                // Manage the replies or timeout errors for the pending subscription requests
                ManageSubscriptionReplies();

                // Manage the replies or timeout errors for the pending publish requests
                ManagePublishedReplies();

                // Manage received data
                ManageReceivedPublishMessages();

                // Check and, eventually, reset the bit of the error status in the state/command variable of the stations
                CheckStationErrorStatus();

                if ((nextJobList.Count > 0) && StopWorkerThread.WaitOne(sleepCycle))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                    {
                        break;
                    }
                }
                StopWorkerThread.WaitOne(0);
            }
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

        protected void PublishJobVariables(List<MQTTClientCommJob> list)
        {
            if (list.Count > 0)
            {
                foreach (var job in list)
                {
                    if (PublishJobVariable(job) == false)
                    {
                        String errorMsg = String.Format(Properties.Resources.MQTTClientErrorPublishing, job.TagName, Name);
                        CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.Min);
#if DEBUG
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTT DBG - {0} PublishJobVariables error: {1} publishing topic: {2}",
                                                           currentTime, errorMsg, job.TagName));
#endif
                        ProcessError(job, LastErrorCode);
                        if (SynchroJob != null && SynchroJob == job)
                        {
                            job.ResetSynchro.WaitOne(Timeout);
                            SynchroJob = null;
                            job.ResetSynchro.Reset();
                        }
                    }
                }
            }
        }

        protected void SubscribeJobVariables(List<MQTTClientCommJob> list)
        {
            if (list.Count > 0)
            {
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

                if(jobIndex > 0)
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
        }

        protected void ExecuteJobList(List<MQTTClientCommJob> list)
        {
            // Any job to be performed?
            if (list.Count > 0)
            {
                // Create the list of jobs that must publish a value and the list of jobs that must subscribe a topic
                List<MQTTClientCommJob> listPublish = new List<MQTTClientCommJob>();
                List<MQTTClientCommJob> listSubscribe = new List<MQTTClientCommJob>();
                foreach (var job in list)
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
                    }
                }

                // Publish the values
                if (listPublish.Count > 0)
                {
                    PublishJobVariables(listPublish);
                }

                // Subscribe the topics
                if (listSubscribe.Count > 0)
                {
                    SubscribeJobVariables(listSubscribe);
                }
            }
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
                    job.SetError((DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionTimeout);
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageSubscriptionFailed: timeout for the subscription of var {1}",
                                                       currentTime, job.TagName));
#endif
                }
                else
                {
                    job.SetError((DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscriptionFailed);
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageSubscriptionFailed: subscription failed for var {1}",
                                                       currentTime, job.TagName));
#endif
                }
            }
        }

        protected void ManageSubscriptionReplies()
        {
            // Make a copy of the received replies for the pending subscription requests
            Dictionary<ushort, byte[]> mapMsgIdSubAckMessageCopy = null;
            lock (lockSubscriptionReplies)
            {
                if (mapMsgIdSubAckMessage.Count > 0)
                {
                    mapMsgIdSubAckMessageCopy = new Dictionary<ushort, byte[]>(mapMsgIdSubAckMessage);
                    mapMsgIdSubAckMessage.Clear();
                }
            }

            //lock (lockMQTTClientMaps)
            //{
            //    // Manage received SUBACK messages
            //    if (mapMsgIdSubAckMessageCopy != null)
            //    {
            //        foreach(var msgID in mapMsgIdSubAckMessageCopy.Keys)
            //        {
            //            // Pending subscription request?
            //            if(mapMsgIdTopics.Keys.Contains(msgID))
            //            {
            //                // Array of the topics of the pending subscription request
            //                string[] topicArray = mapMsgIdTopics[msgID];
            //                // The size of the array of the topics of the pending subscription request must match the number of result codes
            //                if (topicArray.Count() == mapMsgIdSubAckMessageCopy[msgID].Count())
            //                {
            //                    for (int i = 0; i < topicArray.Count(); i++)
            //                    {
            //                        if (mapTopicSubscriptionState.Keys.Contains(topicArray[i]))
            //                        {
            //                            if ((mapMsgIdSubAckMessageCopy[msgID][i] & 0x80) == 0)
            //                            {
            //                                // Success
            //                                mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Done;
            //                            }
            //                            else
            //                            {
            //                                // Subscription failed
            //                                mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Todo;
            //                                ManageSubscriptionFailed(topicArray[i], false);
            //                            }
            //                        }
            //                    }
            //                }

            //                mapMsgIdTopics.Remove(msgID);
            //                mapMsgIdSendingTime.Remove(msgID);
            //            }
            //        }
            //    }

            //    // Check if the timeout is elapsed for any pending subscription request
            //    if(mapMsgIdSendingTime.Count > 0)
            //    {
            //        Dictionary<ushort, DateTime> mapMsgIdSendingTimeTemp = new Dictionary<ushort, DateTime>();
            //        foreach (var msgID in mapMsgIdSendingTime.Keys)
            //        {
            //            double dtime = (DateTime.UtcNow - mapMsgIdSendingTime[msgID]).TotalMilliseconds;
            //            if (dtime > Timeout)
            //            {
            //                if (mapMsgIdTopics.Keys.Contains(msgID))
            //                {
            //                    // Array of the topics of the pending subscription request
            //                    string[] topicArray = mapMsgIdTopics[msgID];
            //                    for (int i = 0; i < topicArray.Count(); i++)
            //                    {
            //                        if (mapTopicSubscriptionState.Keys.Contains(topicArray[i]))
            //                        {
            //                            // Timeout elapsed
            //                            mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Todo;
            //                            ManageSubscriptionFailed(topicArray[i], true);
            //                        }
            //                    }
            //                    // Delete the message ID entry in the dictionary of pending subscription topics
            //                    mapMsgIdTopics.Remove(msgID);
            //                }
            //            }
            //            else
            //            {
            //                mapMsgIdSendingTimeTemp[msgID] = mapMsgIdSendingTime[msgID];
            //            }
            //        }

            //        // Delete the message ID entries for which the timeout is elapsed from the dictionary of pending subscription requests
            //        mapMsgIdSendingTime.Clear();
            //        if(mapMsgIdSendingTimeTemp.Count > 0)
            //        {
            //            mapMsgIdSendingTime = mapMsgIdSendingTimeTemp;
            //        }
            //    }
            //}
            // Manage received SUBACK messages
            Dictionary<ushort, string[]> mapMsgIdTopicsCopy = null;
            Dictionary<string, byte> mapTopicSubscriptionStateCopy = null;
            lock (lockMQTTClientMaps)
            {
                mapMsgIdTopicsCopy = new Dictionary<ushort, string[]>(mapMsgIdTopics);
                mapTopicSubscriptionStateCopy = new Dictionary<string, byte>(mapTopicSubscriptionState);
            }
            if (mapMsgIdSubAckMessageCopy != null)
            { 
                List<ushort> msgidList = (from msgID in mapMsgIdSubAckMessageCopy.Keys.AsParallel()
                                          where (mapMsgIdTopicsCopy.Keys.Contains(msgID) && (mapMsgIdTopicsCopy[msgID].Count() == mapMsgIdSubAckMessageCopy[msgID].Count()))
                                          select msgID).ToList();
                Parallel.ForEach(msgidList, msgID =>
                {
                    string[] topicArray = mapMsgIdTopicsCopy[msgID];
                    Parallel.For(0, topicArray.Count(), i =>
                    {
                        if (mapTopicSubscriptionStateCopy.Keys.Contains(topicArray[i]))
                        {
                            if ((mapMsgIdSubAckMessageCopy[msgID][i] & 0x80) == 0)
                            {
                                // Success
                                lock (lockMQTTClientMaps)
                                {
                                    mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Done;
#if DEBUG
                                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManageSubscriptionReplies: subscription successfull for var {1}",
                                                                       currentTime, topicArray[i]));
#endif
                                }
                            }
                            else
                            {
                                lock (lockMQTTClientMaps)
                                {
                                    // Subscription failed
                                    mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Todo;
                                    ManageSubscriptionFailed(topicArray[i], false);
                                }
                            }
                        }
                    });

                    lock (lockMQTTClientMaps)
                    {
                        mapMsgIdTopics.Remove(msgID);
                        mapMsgIdSendingTime.Remove(msgID);
                    }
                });
            }

            mapMsgIdTopicsCopy.Clear();
            mapMsgIdTopicsCopy = null;
            Dictionary<ushort, DateTime> mapMsgIdSendingTimeCopy = null;
            lock (lockMQTTClientMaps)
            {
                mapMsgIdSendingTimeCopy = new Dictionary<ushort, DateTime>(mapPublishMsgIdSendingTime);
                mapMsgIdTopicsCopy = new Dictionary<ushort, string[]>(mapMsgIdTopics);
            }

            // Check if the timeout is elapsed for any pending subscription request
            if (mapMsgIdSendingTimeCopy.Count > 0)
            {
                List<ushort> msgidList = (from msgID in mapMsgIdSendingTimeCopy.Keys.AsParallel()
                                          where (mapMsgIdTopicsCopy.Keys.Contains(msgID) && ((DateTime.UtcNow - mapMsgIdSendingTimeCopy[msgID]).TotalMilliseconds) > Timeout)
                                          select msgID).ToList();
                Parallel.ForEach(msgidList, msgID =>
                {
                    // Array of the topics of the pending subscription request
                    string[] topicArray = mapMsgIdTopicsCopy[msgID];
                    for (int i = 0; i < topicArray.Count(); i++)
                    {
                        lock (lockMQTTClientMaps)
                        {
                            if (mapTopicSubscriptionState.Keys.Contains(topicArray[i]))
                            {
                                // Timeout elapsed
                                mapTopicSubscriptionState[topicArray[i]] = (byte)MQTTClientSubscriptionStates.Todo;
                                ManageSubscriptionFailed(topicArray[i], true);
                            }
                        }
                    }
                    // Delete the message ID entry in the dictionaries of pending subscription topics
                    lock (lockMQTTClientMaps)
                    {
                        mapMsgIdTopics.Remove(msgID);
                        mapMsgIdSendingTime.Remove(msgID);
                    }
                });
            }
        }

        protected void ManagePublishedReplies()
        {
            // Make a copy of the received Published notifications
            List<MqttMsgPublishedEventArgs> listReceivedPublishedEventsCopy = null;
            lock (lockPublishedMessages)
            {
                if (listPublishedMessages.Count > 0)
                {
                    listReceivedPublishedEventsCopy = new List<MqttMsgPublishedEventArgs>(listPublishedMessages);
                    listPublishedMessages.Clear();
                }
            }

            Dictionary<ushort, MQTTClientCommJob> mapPublishMessageIDJobCopy = null;
            lock (lockMQTTClientMaps)
            {
                mapPublishMessageIDJobCopy = new Dictionary<ushort, MQTTClientCommJob>(mapPublishMessageIDJob);
            }
            // Any notification of published messages?
            if (listReceivedPublishedEventsCopy != null)
            {
                List<MqttMsgPublishedEventArgs> eventargsListPublished = (from eventarg in listReceivedPublishedEventsCopy.AsParallel()
                                                                         where (mapPublishMessageIDJobCopy.Keys.Contains(eventarg.MessageId) && (eventarg.IsPublished == true))
                                                                         select eventarg).ToList();
                List<MqttMsgPublishedEventArgs> eventargsListNotPublished = (from eventarg in listReceivedPublishedEventsCopy.AsParallel()
                                                                             where (mapPublishMessageIDJobCopy.Keys.Contains(eventarg.MessageId) && (eventarg.IsPublished == false))
                                                                             select eventarg).ToList();
                Parallel.ForEach(eventargsListPublished, evArg =>
                {
                    // Get the job waiting for the PUBLISH confirmation
                    MQTTClientCommJob job = mapPublishMessageIDJobCopy[evArg.MessageId];
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManagePublishedReplies: received notification IsPublished == true for var {1}",
                                                       currentTime, job.TagName));
#endif
                    // Publish operation successfully completed
                    job.LastExecutionTime = DateTime.UtcNow;
                    RemovePendingJob(job);
                    job.IsPending = false;
                    job.Status = MQTTClientCommJobStatus.PublishReplyReceived;
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = job;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    OnJobExecuted(eJob);
                    if (SynchroJob != null && SynchroJob == job)
                    {
                        job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        job.ResetSynchro.Reset();
                    }

                    // Remove the message ID from the maps of the pending PUBLISH messages
                    lock (lockMQTTClientMaps)
                    {
                        mapPublishMessageIDJob.Remove(evArg.MessageId);
                        mapPublishMsgIdSendingTime.Remove(evArg.MessageId);
                    }
                });

                Parallel.ForEach(eventargsListNotPublished, evArg =>
                {
                    // Get the job waiting for the PUBLISH confirmation
                    MQTTClientCommJob job = mapPublishMessageIDJobCopy[evArg.MessageId];
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} ManagePublishedReplies: received notification IsPublished == false for var {1}",
                                                       currentTime, job.TagName));
#endif
                    // Error publishing the value
                    String errorMsg = String.Format(Properties.Resources.MQTTClientErrorPublishing, job.TagName, Name);
                    CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.Min);
                    ProcessError(job, LastErrorCode);
                    if (SynchroJob != null && SynchroJob == job)
                    {
                        job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        job.ResetSynchro.Reset();
                    }

                    // Remove the message ID from the maps of the pending PUBLISH messages
                    lock (lockMQTTClientMaps)
                    {
                        mapPublishMessageIDJob.Remove(evArg.MessageId);
                        mapPublishMsgIdSendingTime.Remove(evArg.MessageId);
                    }
                });
            }

            // Check if the timeout is elapsed for any pending PUBLISH request
            mapPublishMessageIDJobCopy.Clear();
            mapPublishMessageIDJobCopy = null;
            Dictionary<ushort, DateTime> mapPublishMsgIdSendingTimeCopy = null;
            lock (lockMQTTClientMaps)
            {
                mapPublishMsgIdSendingTimeCopy = new Dictionary<ushort, DateTime>(mapPublishMsgIdSendingTime);
                mapPublishMessageIDJobCopy = new Dictionary<ushort, MQTTClientCommJob>(mapPublishMessageIDJob);
            }

            if (mapPublishMsgIdSendingTimeCopy.Count > 0)
            {
                List<ushort> msgidList = (from msgID in mapPublishMsgIdSendingTimeCopy.Keys.AsParallel()
                                          where (mapPublishMessageIDJobCopy.Keys.Contains(msgID) && ((DateTime.UtcNow - mapPublishMsgIdSendingTimeCopy[msgID]).TotalMilliseconds) > Timeout)
                                          select msgID).ToList();
                Parallel.ForEach(msgidList, msgID =>
                {
                    // Get the pending job
                    MQTTClientCommJob job = mapPublishMessageIDJobCopy[msgID];

                    if (SynchroJob != null && SynchroJob == job)
                    {
                        job.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        job.ResetSynchro.Reset();
                    }
                    job.SetError((DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPublishTimeout);

                    // Delete the message ID entry in the dictionaries of pending PUBLISH request jobs
                    lock (lockMQTTClientMaps)
                    {
                        mapPublishMessageIDJob.Remove(msgID);
                        mapPublishMsgIdSendingTime.Remove(msgID);
                    }
                });
            }
        }

        protected void ManageReceivedPublishMessages()
        {
            // Make a copy of the received Publish messages
            List<MqttMsgPublishEventArgs> listReceivedPublishMessagesCopy = null;
            lock (lockReceivedPublishMessages)
            //lock (lockMQTTClientMaps)
            {
                if (listReceivedPublishMessages.Count > 0)
                {
                    listReceivedPublishMessagesCopy = new List<MqttMsgPublishEventArgs>(listReceivedPublishMessages);
                    listReceivedPublishMessages.Clear();
                }
            }

            //lock (lockSubscriptionMaps)
            lock (lockMQTTClientMaps)
            {
                // Manage received PUBLISH messages
                if (listReceivedPublishMessagesCopy != null)
                {
                    foreach (var msgPublishEventArgs in listReceivedPublishMessagesCopy)
                    {
                        if (StatisticsData != null)
                        {
                            lock (lockStatisic)
                            {
                                DiagnLastTaskRxBytes += msgPublishEventArgs.Message.Length;
                            }
                        }

                        string topic = msgPublishEventArgs.Topic;
                        if(mapTopicJobs.Keys.Contains(topic))
                        {
                            List<MQTTClientCommJob> jobList = mapTopicJobs[topic];
                            DateTime ExecutionTime = DateTime.UtcNow;
                            Parallel.ForEach(jobList, mJob =>
                            {
                                if(mJob.Status != MQTTClientCommJobStatus.PublishRequestPending)
                                {
                                    mJob.LastExecutionTime = ExecutionTime;
                                    mJob.StartExecutionTime = ExecutionTime;
                                    ExecuteJob(mJob);
                                    // Update the tag of a job associated to the topic with the received value
                                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                                    eJob.Job = mJob;
                                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                                    eJob.Values = msgPublishEventArgs.Message;
                                    OnJobExecuted(eJob);
                                }
                            });
                        }
                    }
                }
            }
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            return (true);
        }
        #endregion

        #region Event handlers

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            lock (lockReceivedPublishMessages)
            //lock (lockMQTTClientMaps)
            {
                listReceivedPublishMessages.Add(e);
            }
        }

        private void MqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            lock (lockSubscriptionReplies)
            //lock (lockMQTTClientMaps)
            {
                mapMsgIdSubAckMessage[e.MessageId] = e.GrantedQoSLevels;
            }
        }

        private void MqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            lock (lockPublishedMessages)
            //lock (lockMQTTClientMaps)
            {
                listPublishedMessages.Add(e);
            }
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

        public bool PublishJobVariable(CommJob job)
        {
            MQTTClientCommJob mj = (MQTTClientCommJob)job;
            if (mj == null)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishJobVariable error 1");
#endif
                // It should never return from here
                return (false);
            }

            if (mj.Status == MQTTClientCommJobStatus.PublishRequestPending)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("PublishJobVariable warning 1");
#endif
                // It should never return from here
                return (true);
            }

            var lkJob = mj.retLockList();
            lock (lkJob)
            {
                DateTime ExecutionTime = DateTime.UtcNow;
                job.LastExecutionTime = ExecutionTime;
                job.StartExecutionTime = ExecutionTime;
                ExecuteJob(job);
                MQTTClientStation mqttStation = (MQTTClientStation)job.Station;
                if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    job.TagsListToWrite.Clear();
                    job.TagsListToWrite.AddRange(job.TagsList);
                }
                if (job.TagsListToWrite.Count == 0)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                    mj.Status = MQTTClientCommJobStatus.PublishError;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("PublishJobVariable error 2");
#endif
                    LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                    return (false);
                }
                // For the JSon Format be sure to publish all the elements of a structure (all the tags of the jobs)
                if ((job.Type != DriverCodeBase.Enumerators.LinkType.UnconditionalOutput) && (mqttStation.MQTTClientMessageFormat == MQTTClientMessageFormats.JSON))
                {
                    job.TagsListToWrite.Clear();
                    job.TagsListToWrite.AddRange(job.TagsList);
                }
                for(int i=0; i<job.TagsListToWrite.Count; i++)
                {
                    Tag cand;
                    cand = job.TagsListToWrite[i];
                    if (!job.TagsListOnWriting.Contains(cand))
                    {
                        job.TagsListOnWriting.Add(cand);
                    }
                }
                job.TagsListToWrite.Clear();

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
                            RemovePendingJob(mj);
                            mj.Status = MQTTClientCommJobStatus.PublishReplyReceived;
#if DEBUG
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} PublishJobVariable: published var {1} with QOS {2}",
                                                               currentTime, mj.TagName, mj.QualityOfServiceLevel));
#endif
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = mj;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            OnJobExecuted(eJob);
                            if (SynchroJob != null && SynchroJob == job)
                            {
                                job.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                job.ResetSynchro.Reset();
                            }
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
#if DEBUG
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} PublishJobVariable: published var {1} with QOS {2}",
                                                               currentTime, mj.TagName, mj.QualityOfServiceLevel));
#endif
                        }
                    }
                    catch (Exception e)
                    {
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
                    RemovePendingJob(job);
                    job.IsPending = false;
                    mj.Status = MQTTClientCommJobStatus.PublishError;
                    lock (job.retLockList())
                    {
                        job.TagsListOnWriting.Clear();
                    }
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("PublishJobVariable error 3");
#endif
                    LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                    return (false);
                }
            }

            return (true);
        }

        void CheckStationErrorStatus()
        {
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                bool inErrorStatus = false;
                if(station.GetStateCommandVariableBit(ref inErrorStatus, (UInt16)StationVariableBits.StationErrorState))
                {
                    if(inErrorStatus == true)
                    {
                        if (InErrorJobs(station) == 0)
                        {
                            station.SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                    else
                    {
                        if (InErrorJobs(station) > 0)
                        {
                            station.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                }
            }
        }

        void ManageConnectionRestored()
        {
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
            if(connectionEstablished)
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

        public void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode)
        {
            MQTTClientCommJob MQTTClientPendingJob = (MQTTClientCommJob)pendingjob;
            RemovePendingJob(pendingjob);
            pendingjob.IsPending = false;
            pendingjob.LastExecutionTime = DateTime.UtcNow;
            MQTTClientPendingJob.Status = MQTTClientCommJobStatus.PublishError;
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = MQTTClientPendingJob;
            eJob.ErrorCode = errorCode;
            OnJobExecuted(eJob);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void GetNextMQTTClientPendingJobList(ref List<MQTTClientCommJob> nextJobList)
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    nextJobList = (from job in queue.AsParallel()
                                   where (((MQTTClientCommJob)job).MustPublish() || ((MQTTClientCommJob)job).MustSubscribe())
                                   select (MQTTClientCommJob)job).ToList();
                    while(!queue.IsEmpty)
                    {
                        CommJob j;
                        queue.TryDequeue(out j);
                    }
                }
            }
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
            base.Dispose();

            // Close connection with the broker
            if (connectionEstablished == true)
            {
                DeviceClose();
            }

            // Unregister managers for the events
            if (mqttClient != null)
            {
                mqttClient.MqttMsgPublishReceived -= MqttClient_MqttMsgPublishReceived;
                mqttClient.MqttMsgSubscribed -= MqttClient_MqttMsgSubscribed;
                mqttClient.MqttMsgPublished -= MqttClient_MqttMsgPublished;
            }
        }
        #endregion

    }
}
