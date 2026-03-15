using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
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
    }

    class PubNubChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the PubNubChannel object.
        /// </summary>
        public PubNubChannel(CommunicationDriver commdriver, PubNubChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        #endregion

        #region Abstracts Methods

        public override bool IsDeviceOpen() { return true; }
        public override bool DeviceOpen() { return true; }
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
        //public Dictionary<String, String> mapVariablesPublishReplies = new Dictionary<String, String>();
        //protected Object lockVariablesPublishRepliesMapObject = new Object();
        public List<String> listVariablesPublishReplies = new List<String>();
        protected Object lockVariablesPublishRepliesListObject = new Object();
        public Dictionary<String, String> mapVariablesPublishErrors = new Dictionary<String, String>();
        protected Object lockVariablesPublishErrorsMapObject = new Object();
        //public Dictionary<String, String> mapVariablesSubscriptionValues = new Dictionary<String, String>();
        //protected Object lockVariablesSubscriptionValuesMapObject = new Object();
        List<string> subscriptionValueMessagesList = new List<string>();
        protected Object lockSubscriptionValueMessagesListObject = new Object();
        protected DateTime subscriptionTime = DateTime.UtcNow;
        Guid channelToken = Guid.NewGuid();
       #endregion

        #region Override Methods

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            List<PubNubCommJob> nextJobList = new List<PubNubCommJob>();
            PubNubDriver pubNubCommDriver = (PubNubDriver)CommDriver;
            // Allocate the Pubnub object
            //pubNubCommDriver.pubnub = new PubNubMessaging.Core.Pubnub(pubNubCommDriver.PublishKey, pubNubCommDriver.SubscribeKey);

            while (true)
            {
                nextJobList.Clear();

                // Initial subscription of the PubNub channel
                if ((subscriptionDone == false) && (subscriptionInProgress == false))
                {
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} WT Invoking Subscription",
                                                       currentTime));
#endif
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
                        String errorMsg = String.Format(Properties.Resources.PubNubExceptionInSubscription, e.Message, Name);
                        CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.High);
#if DEBUG
                        currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} {1}"),
                                                           currentTime, errorMsg);
#endif
                    }
                }
                else if(subscriptionInProgress == true)
                {
                    double dtime = (DateTime.UtcNow - subscriptionTime).TotalMilliseconds;
                    if (dtime > Timeout)
                    {
                        subscriptionDone = false;
                        subscriptionInProgress = false;
                        String errorMsg = String.Format(Properties.Resources.PubNubErrorSubscriptionTimeout, Name);
                        CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.High);
#if DEBUG
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Timeout on Subscription",
                                                           currentTime));
#endif
                    }
                }

                //CommJob nextjob = null;
                if (subscriptionDone == true)
                {
                    if (ListJobPending.Count == 0)
                    {
                        ScheduleListJob();
                        lock (lockThreadObject)
                        {
                            if (SynchroJob != null)
                            {
                                //nextjob = SynchroJob;
                            }
                            else
                            {
                                // Only output jobs are executed (variables to be published)
                                //nextjob = GetNextPubNubPendingJob();
                                GetNextPubNubPendingJobList(ref nextJobList);
                            }
                        }

                        //if (nextjob != null)
                        //{
                        //    ListJobPending.Add(nextjob);
                        //}
                        if(nextJobList.Count > 0)
                        {
                            ListJobPending.AddRange(nextJobList);
                        }
                    }

                    lock (lockThreadObject)
                    {                     
                        if(nextJobList.Count > 0)
                        {
                            foreach(var job in nextJobList)
                            {
                                PubNubCommJob nextjob = (PubNubCommJob)job;
                                if (nextjob != null)
                                {
                                    if (PublishJobVariable(nextjob) == false)
                                    {
                                        if (SynchroJob != null && SynchroJob == nextjob)
                                        {
                                            nextjob.ResetSynchro.WaitOne(Timeout);
                                            SynchroJob = null;
                                            nextjob.ResetSynchro.Reset();
                                        }
                                        PubNubCommJob pnj = (PubNubCommJob)nextjob;
                                        String errorMsg = String.Format(Properties.Resources.PubNubErrorPublishing, pnj.TagName, Name);
                                        CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.Min);
#if DEBUG
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} {1}",
                                                                           currentTime, errorMsg));
#endif
                                        ProcessError(nextjob, LastErrorCode);
                                    }
                                }
                            }
                        }

                        //if (ListJobPending.Count == 0)
                        //{
                        //    ProcessPublishErrors(null);
                        //    ProcessPublishReplies(null);
                        //}
                        //else
                        //{
                        //    ProcessPublishErrors(ListJobPending[0]);
                        //    ProcessPublishReplies(ListJobPending[0]);
                        //}

                        ProcessSubscriptionMessages();

                        if (ListJobPending.Count > 0)
                        {
                            foreach (var job in ListJobPending)
                            {
                                PubNubCommJob pJob = (PubNubCommJob)job;
                                if((pJob.Status == PubNubCommJobStatus.Idle) ||
                                   (pJob.Status == PubNubCommJobStatus.PublishReplyReceived) ||
                                   (pJob.Status == PubNubCommJobStatus.PublishError))
                                {
                                    ListJobExecuted.Add(job);
                                }
                            }

                            foreach (var job in ListJobExecuted)
                            {
                                job.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == job)
                                {
                                    job.ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    job.ResetSynchro.Reset();
                                }
                                ListJobPending.Remove(job);
                            }

                            ListJobExecuted.Clear();
                        }

                        if (ListJobPending.Count > 0)
                        {
                            if (!MultiPointProtocol)
                            {
                                //double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                                //if (dtime > Timeout)
                                //{
                                //    if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                //    {
                                //        ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                //        SynchroJob = null;
                                //        ListJobPending[0].ResetSynchro.Reset();
                                //    }

                                //    //Timeout error
                                //    //ProcessTimeoutError();
                                //    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                //    ProcessError(ListJobPending[0], LastErrorCode);
                                //}
                                CheckTimeoutError(ListJobPending);
                            }
                        }
                    }
                }
                else
                {
                    ProcessSubscriptionErrors(ListJobPending);
                }

                //if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
                if ((nextJobList.Count > 0) && StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
                StopWorkerThread.WaitOne(0);
            }
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            return (true);
        }
        #endregion

        #region Properties

        #endregion

        #region methods

        public void OnJobPublished(ExecutedJobArgs e)
        {
            if (e.ErrorCode == (DriverCodeBase.Enumerators.DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPublish)
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

            var lkJob = pnj.retLockList();
            lock (lkJob)
            {
                ExecuteJob(job);
                if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (job.TagsListToWrite.Count == 0)
                    {
                        job.TagsListToWrite.AddRange(job.TagsList);
                    }
                }
                if (job.TagsListToWrite.Count == 0)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                    pnj.Status = PubNubCommJobStatus.PublishError;
                    System.Diagnostics.Debug.WriteLine("PublishJobVariable error 2");
                    LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                    return (false);
                }
                Tag cand;
                cand = job.TagsListToWrite[0];
                job.TagsListToWrite.Remove(cand);
                if (!job.TagsListOnWriting.Contains(cand))
                {
                    job.TagsListOnWriting.Add(cand);
                }

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
#if DEBUG
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} {1}",
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
                    pnj.Status = PubNubCommJobStatus.PublishError;
                    lock (job.retLockList())
                    {
                        job.TagsListOnWriting.Clear();
                    }
                    System.Diagnostics.Debug.WriteLine("PublishJobVariable error 3");
                    LastErrorCode = (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage;
                    return (false);
                }
            }

            return (true);
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
                        if (SynchroJob != null && SynchroJob == job)
                        {
                            job.ResetSynchro.WaitOne(Timeout);
                            SynchroJob = null;
                            job.ResetSynchro.Reset();
                        }
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
        }

        public void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode)
        {
            PubNubCommJob pubnubPendingJob = (PubNubCommJob)pendingjob;
            RemovePendingJob(pendingjob);
            pendingjob.IsPending = false;
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

        void ProcessSubscriptionErrors(List<CommJob> jobList)
        {
            bool errorSubscritionReceived = false;
            string pubnubErrorMessage = String.Empty;
            lock (lockSubscriptionErrorMessagesListObject)
            {
                if (subscriptionErrorMessagesList.Count > 0)
                {
                    pubnubErrorMessage = subscriptionErrorMessagesList[0];
                    subscriptionErrorMessagesList.Clear();
                    errorSubscritionReceived = true;
                }
            }
            if (errorSubscritionReceived == true)
            {
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

#if DEBUG
                String errorMsg = String.Format(Properties.Resources.PubNubErrorSubscribing, pubnubErrorMessage, Name);
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Error in Subscription: {1}",
                                                   currentTime, errorMsg));
#endif

                // Set to inactive current jobs
                foreach (var job in jobList)
                {
                    PubNubCommJob pJob = (PubNubCommJob)job;
                    pJob.Status = PubNubCommJobStatus.Idle;
                    if (SynchroJob != null && SynchroJob == pJob)
                    {
                        pJob.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        pJob.ResetSynchro.Reset();
                    }
                }

                // Set the error state of the jobs
                string stationErrorMessage = String.Format(Properties.Resources.PubNubErrorSubscribing, pubnubErrorMessage, Name);
                foreach (PubNubStation pStation in CommDriver.GetChannelStations(this))
                {
                    pStation.ManageSubscriptionError(stationErrorMessage);
                }
            }
        }

        void ProcessPublishErrors(CommJob pendingJob)
        {
            Dictionary<String, String> mapErrorsCopy = null;
            lock (lockVariablesPublishErrorsMapObject)
            {
                if(mapVariablesPublishErrors.Count > 0)
                {
                    if(pendingJob != null)
                    {
                        mapErrorsCopy = new Dictionary<String, String>(mapVariablesPublishErrors);
                    }
                    mapVariablesPublishErrors.Clear();
                }
            }

            if((pendingJob != null) && (mapErrorsCopy != null))
            {
                PubNubCommJob pJob = (PubNubCommJob)pendingJob;
                if(mapErrorsCopy.ContainsKey(pJob.TagName))
                {
                    RemovePendingJob(pendingJob);
                    pJob.Status = PubNubCommJobStatus.PublishError;
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorPublish;
                    lock (pendingJob.retLockList())
                    {
                        pendingJob.TagsListOnWriting.Clear();
                    }
                    eJob.Job = pJob;
                    OnJobExecuted(eJob);
                }
                mapErrorsCopy.Clear();
            }
        }

        void ProcessPublishReplies(CommJob pendingJob)
        {
            List<String> listRepliesCopy = null;
            lock (lockVariablesPublishRepliesListObject)
            {
                if (listVariablesPublishReplies.Count > 0)
                {
                    if (pendingJob != null)
                    {
                        listRepliesCopy = new List<String>(listVariablesPublishReplies);
                    }
                    listVariablesPublishReplies.Clear();
                }
            }

            if ((pendingJob != null) && (listRepliesCopy != null))
            {
                if(listRepliesCopy[0].Contains("Sent"))
                {
                    RemovePendingJob(pendingJob);
                    PubNubCommJob pJob = (PubNubCommJob)pendingJob;
                    pJob.Status = PubNubCommJobStatus.PublishReplyReceived;
#if DEBUG
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} ProcessPublishReplies: received publish confirmation for var {1}",
                                                       currentTime, pJob.TagName));
#endif
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = pJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    OnJobExecuted(eJob);

                }
                listRepliesCopy.Clear();
            }
        }

        private void ProcessSubscriptionMessages()
        {
            List<String> subscriptionMessages = null;
            lock (lockSubscriptionValueMessagesListObject)
            {
                if (subscriptionValueMessagesList.Count > 0)
                {
                    subscriptionMessages = new List<string>();
                    subscriptionMessages.AddRange(subscriptionValueMessagesList);
                }
                subscriptionValueMessagesList.Clear();
            }

            if(subscriptionMessages == null)
            {
                return;
            }

            foreach(var msg in subscriptionMessages)
            {
                // Parse the message
                string receivedMessage = msg;
                string variableName = String.Empty;
                string variableNodeId = String.Empty;
                string variableValue = String.Empty;
                Guid msgToken = Guid.Empty;
                if(!ParseSubscriptionMessage(receivedMessage, out variableName, out variableNodeId, out variableValue, out msgToken))
                {
                    continue;
                }

                List<PubNubCommJob> listJobs = new List<PubNubCommJob>();
                GetJobListForSubscriptionData(variableName, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        PubNubCommJob pJob = (PubNubCommJob)jobVal;
                        if(pJob == null)
                        {
                            continue;
                        }

                        // Check if the value has been previously published by this job (in this case discard it)
                        if( (channelToken != msgToken) || (variableNodeId != pJob.getTagNodeId()))
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = pJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            eJob.Values = variableValue;
                            OnJobExecuted(eJob);
                        }
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

        public void AddSubscriptionErrorMessages(string errorMessage)
        {
            lock (lockSubscriptionErrorMessagesListObject)
            {
                subscriptionErrorMessagesList.Add(errorMessage);
            }
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

        public void AddSubscriptionValues(string subscriptionMessage)
        {
            lock (lockSubscriptionValueMessagesListObject)
            {
                subscriptionValueMessagesList.Add(subscriptionMessage);
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected PubNubCommJob GetNextPubNubPendingJob()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    while (!queue.IsEmpty)
                    {
                        CommJob j = null;
                        if (queue.TryDequeue(out j) == true)
                        {
                            PubNubCommJob pj = (PubNubCommJob)j;
                            if (pj != null)
                            {
                                if (pj.MustPublish())
                                {
                                    return (pj);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        protected void GetNextPubNubPendingJobList(ref List<PubNubCommJob> nextJobList)
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    while (!queue.IsEmpty)
                    {
                        CommJob j = null;
                        if (queue.TryDequeue(out j) == true)
                        {
                            PubNubCommJob pj = (PubNubCommJob)j;
                            if (pj != null)
                            {
                                if (pj.MustPublish())
                                {
                                    nextJobList.Add(pj);
                                }
                            }
                        }
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
            AddSubscriptionValues(resultMessage);
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
            if(pubnubError.StatusCode != 112)
            {
                subscriptionDone = false;
                AddSubscriptionErrorMessages(pubnubError.Message);
#if DEBUG
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Subscr Err Msg: {1}",
                                                   currentTime, pubnubError.ToString()));
#endif
            }
            // Channel already subscribed
            else
            {
                subscriptionInProgress = false;
                subscriptionDone = true;
                //AddSubscriptionErrorMessages(pubnubError.ToString());
#if DEBUG
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Subscr Err Msg: {1}",
                                                   currentTime, pubnubError.ToString()));
#endif
            }
        }

        public void ManageUnsubscribeSubscribeMessage(string subscribeMessage)
        {
            subscriptionInProgress = false;
            subscriptionDone = false;
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Unsubscr Subs Msg: {1}",
                                               currentTime, subscribeMessage));
#endif
        }

        public void ManageUnsubscribeConnectMessage(string connectMessage)
        {
            subscriptionInProgress = false;
            subscriptionDone = false;
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Unsubscr Conn Msg: {1}",
                                               currentTime, connectMessage));
#endif
        }

        public void ManageUnsubscribeDisconnectMessage(string disconnectMessage)
        {
            subscriptionInProgress = false;
            subscriptionDone = false;
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Unsubscr Disconn Msg: {1}",
                                               currentTime, disconnectMessage));
#endif
        }

        public void ManageUnsubscribeConnectErrorMessage(PubnubClientError pubnubError)
        {
            subscriptionInProgress = false;
            subscriptionDone = false;
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Unsubscr Err Msg: {1}",
                                               currentTime, pubnubError.ToString()));
#endif
        }

        public void ManagePublishUserMessage(string userMessage)
        {
            string resultMessage = String.Empty;
            if(ParsePublishUserMessage(userMessage, out resultMessage) == true)
            {
                AddPublishReplies(resultMessage);
            }
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Publish Complete Msg: {1} User Msg: {2}",
                                               currentTime, userMessage, resultMessage));
#endif
        }

        public void ManagePublishErrorMessage(PubnubClientError pubnubError)
        {
            string errorMessage = pubnubError.ToString();
            string variableName = String.Empty;
            if (ExtractVariableNameFromMessage(errorMessage, out variableName) == true)
            {
                AddPublishErrors(variableName, errorMessage);
            }
#if DEBUG
            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Publish Err Msg: {1}",
                                               currentTime, errorMessage));
#endif
        }

        #endregion
    }
}
