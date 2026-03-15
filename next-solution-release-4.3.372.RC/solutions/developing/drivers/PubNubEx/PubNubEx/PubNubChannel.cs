using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using PubNubMessaging.Core;
using Utilities;
using System.Web.Script.Serialization;

namespace PubNub
{
    public enum PubNubErrorCodes : int
    {
        ErrorCodeErrorPreparingPublishMessage = 1000,
        ErrorCodeErrorSubscription = 1001,
        ErrorCodeErrorPublish = 1002,
        ErrorCodeErrorSubscriptionException = 1003,
        ErrorCodeErrorSubscriptionTimeout = 1004
    }

    class PubNubChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the PubNubChannel object.
        /// </summary>
        public PubNubChannel(CommunicationDriver commdriver, PubNubChannelSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region Abstracts Methods

        public override bool DeviceClose() { return true; }
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members
        protected object lockSubscribedVarMap = new object();
        public Dictionary<String, List<PubNubCommJob>> mapSubscribedVariablesJobs = new Dictionary<String, List<PubNubCommJob>>();
        public bool subscriptionDone = false;
        public bool subscriptionInProgress = false;
        List<string> subscriptionErrorMessagesList = new List<string>();
        protected Object lockSubscriptionErrorMessagesListObject = new Object();
        public List<String> listVariablesPublishReplies = new List<String>();
        protected Object lockVariablesPublishRepliesListObject = new Object();
        public Dictionary<String, String> mapVariablesPublishErrors = new Dictionary<String, String>();
        protected Object lockVariablesPublishErrorsMapObject = new Object();
        List<string> subscriptionValueMessagesList = new List<string>();
        protected Object lockSubscriptionValueMessagesListObject = new Object();
        protected DateTime subscriptionTime = DateTime.UtcNow;
        Guid channelToken = Guid.NewGuid();
        #endregion

        #region Override Methods

        public override bool Startup()
        {
            return base.Startup();
        }

        public override bool IsDeviceOpen()
        {
            return (subscriptionDone);
        }

        public override bool DeviceOpen()
        {
            LastErrorMessage = String.Empty;
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            PubNubDriver pubNubCommDriver = (PubNubDriver)CommDriver;

            // Initial subscription of the PubNub channel
            if ((subscriptionDone == false) && (subscriptionInProgress == false))
            {
                subscriptionInProgress = true;
                subscriptionTime = DateTime.UtcNow;
                try
                {
                    pubNubCommDriver.pubnub.Subscribe<String>(Name,
                                                              ManageSubscribeReturnMessage,
                                                              ManageSubscribeConnectStatusMessage,
                                                              ManageSubscribeConnectErrorMessage);
                }
                catch (Exception e)
                {
                    subscriptionInProgress = false;
                    subscriptionDone = false;
                    LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorSubscriptionException;
                    LastErrorMessage = String.Format(Properties.Resources.PubNubExceptionInSubscription, e.Message, Name);
                    pubNubCommDriver.lastSubscribeErrorMessage = LastErrorMessage;
                    return (false);
                }
            }
            else if (subscriptionInProgress == true)
            {
                double dtime = (DateTime.UtcNow - subscriptionTime).TotalMilliseconds;
                if (dtime > Timeout)
                {
                    subscriptionDone = false;
                    subscriptionInProgress = false;
                    LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorSubscriptionTimeout;
                    LastErrorMessage = String.Format(Properties.Resources.PubNubErrorSubscriptionTimeout, Name);
                    pubNubCommDriver.lastSubscribeErrorMessage = LastErrorMessage;
                }
            }

            return (subscriptionDone);
        }

        public override DriverErrorCodes CheckDevice(CommJob exjob, object thischannel)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                {
                    conn = LastErrorCode;
                }
            }

            return conn;
        }

        public override void ResetNewDataEvent()
        {
            // Do nothing
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            // Do nothing
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            PubNubCommJob pJob = (PubNubCommJob)job;
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibility with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            else if(subscriptionDone == false)
            {
                return false;
            }

            // Check if the job must publish data
            if(pJob.MustPublish())
            {
                if(pJob.RestoreInitialQuality)
                {
                    pJob.RestoreInitialQuality = false;
                }
                PublishJobVariable(pJob);
            }

            // Avoid to execute WaitNewDataEvent in commExecution
            return false;
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            PubNubCommJob pubnubJob = (PubNubCommJob)pendingjob;

            CheckTimeoutError(pendingjob);

            // Check and, eventually, reset the bit of the error status in the state/command variable of the stations
            CheckStationErrorStatus(conn);

            lock (lockThreadObject)
            {
                if (subscriptionDone && (pubnubJob.Status != PubNubCommJobStatus.PublishRequestPending) && pubnubJob.Status != PubNubCommJobStatus.PublishError)
                {
                    RemovePendingJob(pendingjob);
                    pendingjob.LastExecutionTime = DateTime.UtcNow;
                }
            }

            return true;
        }

        #endregion

        #region Properties

        #endregion

        #region methods

        void CheckStationErrorStatus(DriverErrorCodes conn)
        {
            if((conn == DriverErrorCodes.ErrorNoError) && subscriptionDone)
            {
                foreach (var station in CommDriver.GetChannelStations(this))
                {
                    PubNubStation pubnubStation = (PubNubStation)station;
                    bool inErrorStatus = false;
                    if (station.GetStateCommandVariableBit(ref inErrorStatus, (UInt16)StationVariableBits.StationErrorState))
                    {
                        if (inErrorStatus == true)
                        {
                            if (pubnubStation.NumberOfJobsInError() == 0)
                            {
                                station.SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                            }
                        }
                        else
                        {
                            if (pubnubStation.NumberOfJobsInError() > 0)
                            {
                                station.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                            }
                        }
                    }
                }
            }
            else if(conn != DriverErrorCodes.ErrorNoError)
            {
                foreach (var station in CommDriver.GetChannelStations(this))
                {
                    PubNubStation pubnubStation = (PubNubStation)station;
                    bool inErrorStatus = false;
                    // Timeout or Exception in subscription
                    if ((LastErrorCode == (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorSubscriptionException) || (LastErrorCode == (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorSubscriptionTimeout))
                    {
                        pubnubStation.ManageSubscriptionError(LastErrorMessage, LastErrorCode);
                    }
                    else if (station.GetStateCommandVariableBit(ref inErrorStatus, (UInt16)StationVariableBits.StationErrorState))
                    {
                        if (inErrorStatus == false)
                        {
                            station.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                }
            }
        }

        public void OnJobPublished(ExecutedJobArgs e)
        {
            if (e.ErrorCode == (DriverCodeBaseEx.Enumerators.DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPublish)
            {
                PubNubCommJob pnj = (PubNubCommJob)e.Job;
                PubNubDriver pubnubDriver = (PubNubDriver)CommDriver;
                pubnubDriver.lastPublishErrorMessage = pnj.lastPublishErrorMessage;
            }

            base.OnJobExecuted(e);
        }

        public bool PublishJobVariable(CommJob job)
        {
            PubNubCommJob pnj = (PubNubCommJob)job;
            if (pnj == null)
            {
                System.Diagnostics.Debug.WriteLine("PublishJobVariable error 1");
                // It should never return from here
                return (false);
            }

            if (pnj.Status == PubNubCommJobStatus.PublishRequestPending)
            {
                System.Diagnostics.Debug.WriteLine("PublishJobVariable warning 1");
                // It should never return from here
                return (true);
            }

            if(pnj.RestoreInitialQuality)
            {
                pnj.RestoreInitialQuality = false;
            }

            ExecuteJob(job);
            if (job.GetTagListOnWritingCount() == 0)
            {
                RemovePendingJob(job);
                pnj.Status = PubNubCommJobStatus.PublishError;
                System.Diagnostics.Debug.WriteLine("PublishJobVariable error 2");
                LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                return (false);
            }
            List<Tag> jobTagListToWrite = job.GetTagListOnWriting();
            if(jobTagListToWrite.Count == 0)
            {
                RemovePendingJob(job);
                pnj.Status = PubNubCommJobStatus.PublishError;
                System.Diagnostics.Debug.WriteLine("PublishJobVariable error 2");
                LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                return (false);
            }
            Tag cand;
            cand = jobTagListToWrite[0];

            // Get the message to be published
            string messageToBePublished = String.Empty;
            pnj.GetMessageToBePublished(ref messageToBePublished, channelToken);

            if (String.IsNullOrWhiteSpace(messageToBePublished) == false)
            {
                // Publish the message
                PubNubDriver pubNubCommDriver = (PubNubDriver)CommDriver;
                try
                {
                    pubNubCommDriver.pubnub.Publish<String>(Name,
                                                            messageToBePublished,
                                                            pnj.ManagePublishUserMessage,
                                                            pnj.ManagePublishErrorMessage);
                    job.LastExecutionTime = DateTime.UtcNow;
                    job.StartExecutionTime = DateTime.UtcNow;
                    pnj.Status = PubNubCommJobStatus.PublishRequestPending;
                }
                catch (Exception e)
                {
                    String errorMsg = String.Format(Properties.Resources.PubNubExceptionPublishing, e.Message, pnj.TagName, Name);
                    CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.High);
                    return (false);                      
                }
            }
            else
            {
                job.LastExecutionTime = DateTime.UtcNow;
                RemovePendingJob(job);
                pnj.Status = PubNubCommJobStatus.PublishError;
                job.ClearTagListOnWriting();
                System.Diagnostics.Debug.WriteLine("PublishJobVariable error 3");
                LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                return (false);
            }

            return (true);
        }

        protected void CheckTimeoutError(CommJob job)
        {
            if (job != null)
            {
                double dtime = (DateTime.UtcNow - job.StartExecutionTime).TotalMilliseconds;
                PubNubCommJob pJob = (PubNubCommJob)job;
                if ((pJob.Status == PubNubCommJobStatus.PublishRequestPending) && (dtime > Timeout))
                {
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Timeout elapsed: {1} for tag: {2} job state: {3}",
                                                       currentTime, dtime, pJob.TagName, pJob.Status));
#endif

                    //Timeout error
                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                    ProcessError(job, LastErrorCode);
                }
            }
        }

        protected void CheckTimeoutError(List<CommJob>jobList)
        {
            if(jobList.Count > 0)
            {
                List<CommJob> checkJobList = new List<CommJob>();
                checkJobList.AddRange(jobList);
                foreach(var job in checkJobList)
                {
                    double dtime = (DateTime.UtcNow - job.StartExecutionTime).TotalMilliseconds;
                    PubNubCommJob pJob = (PubNubCommJob)job;
                    if ((dtime > Timeout) && (pJob.Status == PubNubCommJobStatus.PublishRequestPending))
                    {
                        //Timeout error
                        LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                        ProcessError(job, LastErrorCode);
                    }
                }
            }
        }

        public void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode)
        {
            PubNubCommJob pubnubPendingJob = (PubNubCommJob)pendingjob;
            RemovePendingJob(pendingjob);
            pendingjob.LastExecutionTime = DateTime.UtcNow;
            pubnubPendingJob.Status = PubNubCommJobStatus.PublishError;
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = pubnubPendingJob;
            eJob.ErrorCode = errorCode;
            OnJobExecuted(eJob);
        }

        void ProcessTimeoutError()
        {
            lock (lockSubscriptionErrorMessagesListObject)
            {
                subscriptionErrorMessagesList.Clear();
            }
            lock (lockVariablesPublishRepliesListObject)
            {
                listVariablesPublishReplies.Clear();
            }
            lock (lockVariablesPublishErrorsMapObject)
            {
                mapVariablesPublishErrors.Clear();
            }
            lock (lockSubscriptionValueMessagesListObject)
            {
                subscriptionValueMessagesList.Clear();
            }

            subscriptionDone = false;
            subscriptionInProgress = false;
        }

        void ProcessSubscriptionErrors(string errorMessage)
        {
            string pubnubErrorMessage = errorMessage;

            subscriptionInProgress = false;
            lock (lockVariablesPublishRepliesListObject)
            {
                listVariablesPublishReplies.Clear();
            }
            lock (lockVariablesPublishErrorsMapObject)
            {
                mapVariablesPublishErrors.Clear();
            }
            lock (lockSubscriptionValueMessagesListObject)
            {
                subscriptionValueMessagesList.Clear();
            }

            // Set the error state of the jobs
            string stationErrorMessage = String.Format(Properties.Resources.PubNubErrorSubscribing, pubnubErrorMessage, Name);
            foreach (PubNubStation pStation in CommDriver.GetChannelStations(this))
            {
                pStation.ManageSubscriptionError(stationErrorMessage);
            }
        }

        void ProcessConnectionRestored()
        {
            foreach (PubNubStation pStation in CommDriver.GetChannelStations(this))
            {
                pStation.ManageConnectionRestored();
            }
        }

        // Manage received unsolicited values
        private void ProcessSubscriptionMessages(string subscriptionMessage)
        {
            if(String.IsNullOrWhiteSpace(subscriptionMessage))
            {
                return;
            }

            // Parse the message
            string receivedMessage = subscriptionMessage;
            string variableName = String.Empty;
            string variableNodeId = String.Empty;
            string variableValue = String.Empty;
            Guid msgToken = Guid.Empty;
            if (!ParseSubscriptionMessage(receivedMessage, out variableName, out variableNodeId, out variableValue, out msgToken))
            {
                return;
            }

            List<PubNubCommJob> listJobs = new List<PubNubCommJob>();
            GetJobListForSubscriptionData(variableName, ref listJobs);
            if (listJobs.Count > 0)
            {
                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    PubNubCommJob pJob = (PubNubCommJob)jobVal;
                    if (pJob == null)
                    {
                        continue;
                    }

                    // Check if the value has been previously published by this job (in this case discard it)
                    if ((channelToken != msgToken) || (variableNodeId != pJob.getTagNodeId()))
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = pJob;
                        eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                        eJob.Values = variableValue;
                        OnJobExecuted(eJob);
                    }
                }
            }
        }

        public void GetJobListForSubscriptionData(String searchKey, ref List<PubNubCommJob> jobList)
        {
            lock (lockSubscribedVarMap)
            {
                if (mapSubscribedVariablesJobs.ContainsKey(searchKey))
                {
                    jobList = mapSubscribedVariablesJobs[searchKey];
                }
            }
        }

        bool ParseSubscriptionMessage(string message, out string variableName, out string variableNodeId, out string variableValue, out Guid receivedToken)
        {
            bool returnValue = false;
            string subscriptionResult = String.Empty;
            variableName = String.Empty;
            variableNodeId = String.Empty;
            variableValue = String.Empty;
            receivedToken = Guid.Empty;

            if (!String.IsNullOrWhiteSpace(message))
            {
                PubNubDriver pubNubCommDriver = (PubNubDriver)CommDriver;
                List<object> deserializedMessage = pubNubCommDriver.pubnub.JsonPluggableLibrary.DeserializeToListOfObject(message);
                if ((deserializedMessage != null) && (deserializedMessage.Count > 0))
                {
                    object resultObject = (object)deserializedMessage[0];
                    if(resultObject != null)
                    {
                        subscriptionResult = (string)resultObject;
                        if (!String.IsNullOrWhiteSpace(subscriptionResult))
                        {
                            string[] messageParts = subscriptionResult.Split('|');
                            if (messageParts.GetLength(0) >= 3)
                            {
                                ExtDataValue dataValue = messageParts[2].FromXml<ExtDataValue>();
                                variableName = messageParts[0];
                                variableNodeId = messageParts[1];
                                variableValue = messageParts[2];
                                receivedToken = dataValue.Token;
                                returnValue = true;
                            }
                        }
                    }
                }
            }

            return (returnValue);
        }

        public void AddPublishReplies(string publishMessage)
        {
            lock (lockVariablesPublishRepliesListObject)
            {
                listVariablesPublishReplies.Add(publishMessage);
            }
        }

        public void AddPublishErrors(string variableName, string errorMessage)
        {
            lock (lockVariablesPublishErrorsMapObject)
            {
                mapVariablesPublishErrors[variableName] = errorMessage;
            }
        }

        public void AddToMapSubscribedVariablesJobs(String variableName, PubNubCommJob job)
        {
            if (!String.IsNullOrWhiteSpace(variableName))
            {
                lock (lockSubscribedVarMap)
                {
                    if (!mapSubscribedVariablesJobs.ContainsKey(variableName))
                    {
                        List<PubNubCommJob> ListJobs = new List<PubNubCommJob>();
                        ListJobs.Add(job);
                        mapSubscribedVariablesJobs[variableName] = ListJobs;
                    }
                    else if (!mapSubscribedVariablesJobs[variableName].Contains(job))
                    {
                        mapSubscribedVariablesJobs[variableName].Add(job);
                    }
                }

            }
        }

        bool ParsePublishUserMessage(string userMessage, out string publishResult)
        {
            bool returnValue = false;
            publishResult = String.Empty;
            PubNubDriver pubNubCommDriver = (PubNubDriver)CommDriver;
            List<object> deserializedMessage = pubNubCommDriver.pubnub.JsonPluggableLibrary.DeserializeToListOfObject(userMessage);
            if ((deserializedMessage != null) && (deserializedMessage.Count > 1))
            {
                object resultObject = (object)deserializedMessage[1];
                if(resultObject != null)
                {
                    publishResult = (string)resultObject;
                    if(!String.IsNullOrWhiteSpace(publishResult))
                    {
                        returnValue = true;
                    }
                }
            }

            return (returnValue);
        }

        bool ExtractVariableNameFromMessage(string message, out string variableName)
        {
            bool returnValue = false;
            variableName = String.Empty;
            if (!String.IsNullOrWhiteSpace(message))
            {
                string[] messageParts = message.Split(' ');
                if (!String.IsNullOrWhiteSpace(messageParts[0]))
                {
                    variableName = messageParts[0];
                    returnValue = true;
                }
            }

            return (returnValue);
        }
        #endregion

        #region PubNub CallBacks

        public void ManageSubscribeReturnMessage(string resultMessage)
        {
            ProcessSubscriptionMessages(resultMessage);
        }

        public void ManageSubscribeConnectStatusMessage(string connectMessage)
        {
            subscriptionInProgress = false;
            subscriptionDone = true;
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Subscr Conn Msg: {1}",
                                               currentTime, connectMessage));
#endif
        }

        public void ManageSubscribeConnectErrorMessage(PubnubClientError pubnubError)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Subscr Err Msg: {1}",
                                                   currentTime, pubnubError.ToString()));
            }
#endif
            // Channel already subscribed (StatusCode == 112) or Internet connection restored (StatusCode == 110)
            if ((pubnubError.StatusCode != 112) && (pubnubError.StatusCode != 110))
            {
                subscriptionDone = false;
                ProcessSubscriptionErrors(pubnubError.Message);
            }
            else
            {
                subscriptionInProgress = false;
                subscriptionDone = true;
                ProcessConnectionRestored();
            }
        }

        #endregion
    }
}
