using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Opc.Ua;

namespace Databoom
{
    class DataboomChannel : ChannelList
    {
        #region Constructors
        /// <summary>
        /// Initializes the DataboomChannel object.
        /// </summary>
        public DataboomChannel(CommunicationDriver commdriver, DataboomChannelSettings settings)
            : base(commdriver, settings)
        {
            deviceToken = settings.DeviceToken;

            if (CommDriver.AggregationLimit > 0 && CommDriver.AggregationLimit < DataboomProtocol.JOBS_AGGREGATION_LIMIT)
                jobsAggregationLimit = CommDriver.AggregationLimit;
            else
                jobsAggregationLimit = DataboomProtocol.JOBS_AGGREGATION_LIMIT;

            databoomCommDriver = (DataboomDriver)CommDriver;
        }
        #endregion

        #region Abstracts Methods
        public override bool IsDeviceOpen()
        {
            return (clientHttpIsConnected && clientHttpPost != null && clientHttpGet != null);
        }

        public override bool DeviceOpen()
        {
            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            string errorMessage = string.Empty;
            differenceFromCloudServerDB = new TimeSpan(0);

            DeviceClose();

            CreateClientHTTP(ref clientHttpPost);

            CreateClientHTTP(ref clientHttpGet);
            
            if (DataboomProtocol.CheckInternetConnection())
            {
                errorCode = GetTimeDifferenceFromCloudServerDB(differenceFromCloudServerDB, out errorMessage);
            }    
            else
            {
                errorCode = (DriverErrorCodes)DataboomProtocol.DataboomErrorCodes.ErrorCodeNoInternetConnection;
                errorMessage = Properties.Resources.DatabommErrorInternetNotAvailable;
            }

            clientHttpIsConnected = (errorCode == DriverErrorCodes.ErrorNoError);          
            if (!clientHttpIsConnected  && (errorCode != (DriverErrorCodes)clientHttpCurrentErrorCode))
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
                clientHttpCurrentErrorCode = (int)errorCode;
            }

            clientHttpCurrentErrorCode = (int)errorCode;

            SetStateCommandVariableBit(!clientHttpIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);

            return clientHttpIsConnected;
        }

        public override bool DeviceClose()
        {
            if (clientHttpGet != null)
            {
                clientHttpGet.Dispose();
                clientHttpGet = null;
            }
            if (clientHttpPost != null)
            {
                clientHttpPost.Dispose();
                clientHttpPost = null;
            }

            clientHttpIsConnected = false;

            SetStateCommandVariableBit(!clientHttpIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);

            return true;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members               
        DataboomDriver databoomCommDriver = null;
        uint jobsAggregationLimit = 0;
        HttpClient clientHttpPost = null;
        HttpClient clientHttpGet = null;
        bool clientHttpIsConnected = false;
        int clientHttpCurrentErrorCode = 0;
        TimeSpan differenceFromCloudServerDB = new TimeSpan(0);
        private string deviceToken = null;
        #endregion

        #region Override Methods
        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            var sortedJobList = (from job in jobList
                                 orderby ((DataboomCommJob)job).FrequencyOfSendingSignal, job.LastExecutionTime ascending
                                 select job).ToList();

            while (sortedJobList.Count() > 0)
            {
                List<CommJob> list = new List<CommJob>();
                bool first = true;
                TimeSpan frequencyOfSendingSignal = new TimeSpan();
                // limit the nr max of jobs (tags) can be written in a single Http message 
                while (sortedJobList.Count() > 0)
                {
                    if (first)
                    {
                        first = false;
                        frequencyOfSendingSignal = ((DataboomCommJob)sortedJobList[0]).FrequencyOfSendingSignal;
                    }

                    if (frequencyOfSendingSignal == ((DataboomCommJob)sortedJobList[0]).FrequencyOfSendingSignal)
                    {
                        list.Add(sortedJobList[0]);
                        sortedJobList.RemoveAt(0);
                        if (list.Count() == jobsAggregationLimit)
                            break;
                    }
                }

                if (list.Count > 0)
                    exList.Add(list);
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            DateTime dt = DataboomProtocol.GetDateTimeUtcNowNoMSec();
            DateTime currentExecutionTime = ((DataboomCommJob)list[0]).GetCurrentScheduleTime(dt);

            if (conn == DriverErrorCodes.ErrorNoError)
            {
                // gest last write date/time (the oldest one) of jobs
                if (GetPendingWriteTime(list, out DateTime pendingWriteTime, out TimeSpan frequencyOfSendingSignal))
                {
                    int nrSends = 0;
                    while (pendingWriteTime < currentExecutionTime)
                    {
                        pendingWriteTime = pendingWriteTime.Add(frequencyOfSendingSignal);

                        // get job's data between last written data and next sampling interval
                        DataboomProtocol.DataToSend data = PrepareWriteRequest(list, pendingWriteTime);
                        if (data != null)
                        {
                            conn = WriteDataToCloudServerDB(data);
                            if (conn != DriverErrorCodes.ErrorNoError)
                            {
                                // try to reschedule as soon as possible
                                currentExecutionTime = DateTime.UtcNow.Subtract(frequencyOfSendingSignal);
                                break;
                            }
                        }

                        RemovePendingData(list, pendingWriteTime);

                        nrSends++;

                        // write only a portion of pending data
                        if (Properties.Settings.Default.UnWrittenValuesRestoredAtTimeMaxSize != 0 && nrSends >= Properties.Settings.Default.UnWrittenValuesRestoredAtTimeMaxSize)
                        {
                            // if not all pending data were written, try to reschedule as soon as possible
                            currentExecutionTime = DateTime.UtcNow.Subtract(frequencyOfSendingSignal);
                            break;
                        }                        
                    }
                }
            }            

            foreach (CommJob j in list)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j, LastExecutionTime = currentExecutionTime };
                OnJobExecuted(eJob);
            }
            
            return false;
        }

        public override int ChannelScheduleProcedure(DateTime dtNow, out int nrRemainingJobsInQueues)
        {
            int nNextScheduleInterval = -1;
            int nNextDelayUse = -1;

            List<CommJob> startList = new List<CommJob>();
            List<CommJob> wList = null;
            List<CommJob> rList = null;

            lock (lockListObject)
            {
                startList.AddRange(channelJobList);
                            
                wList = (from job in startList
                         where !job.IsPending && job.InUse
                                && ((dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval)
                         orderby job.LastExecutionTime ascending
                         select job).ToList();

                rList = (from job in startList
                         where !job.IsPending && !job.IsQueued && job.InUse &&
                         ((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval)
                         orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                         select job).ToList();
                
                if (wList.Count > 0)
                {
                    wList.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingInUse;
                    });
                }

                if (rList.Count > 0)
                {
                    var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(rList[0].SamplingInterval);
                    if (dtNextSchedule > dtNow)
                        nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    if (nNextDelayUse <= 0)
                        nNextDelayUse = 0;
                }
            }

            if (wList.Count > 0)
            {
                RescheduleLastQueuedJobsList(CommJobState.PollingInUse, wList);

                List<List<CommJob>> exList = new List<List<CommJob>>();
                SplitInExecutionLists(wList, ref exList);
                if (exList.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingInUse, exList, out nInUseMax);

                lock (lockListObject)
                {
                    wList.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.UnScheduled;
                    });
                }
            }
            else
            {
                nInUseMax = GetScheduledListJobQueue(CommJobState.PollingInUse).Count();
            }

            //search lower delays
            if (nNextDelayUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayUse < nNextScheduleInterval))
                nNextScheduleInterval = nNextDelayUse;

            nrRemainingJobsInQueues = NrRemainingJobsInQueues();

            nNextScheduleInterval = EvaluateMinScheduleInterval(nNextScheduleInterval, nrRemainingJobsInQueues);
            
            return nNextScheduleInterval;
            
        }

        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            base.SubscribeJob(job, state);
            ((DataboomCommJob)job).StartStopSampling(((DataboomCommJob)job).GetInUseState());
        }
        #endregion

        #region Local Methods
        private DriverErrorCodes GetTimeDifferenceFromCloudServerDB(TimeSpan currentTimeDifference, out string errorMessage)
        {
            DriverErrorCodes result = (DriverErrorCodes)DataboomProtocol.DataboomErrorCodes.ErrorCodeFailedGettingServiceDateTime;
            errorMessage = String.Empty;

            // if no date/time server was configured, true --> OK
            if (string.IsNullOrWhiteSpace(databoomCommDriver.UrlClockPost))
            {
                result = DriverErrorCodes.ErrorNoError;
            }
            else
            {
                try
                {
                    HttpResponseMessage response = clientHttpGet.GetAsync(databoomCommDriver.UrlClockPost).Result; // Blocking call!
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            errorMessage = string.Format("{0} {1}", response.StatusCode, databoomCommDriver.UrlClockPost);
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            errorMessage = string.Format("{0} {1}", response.StatusCode, databoomCommDriver.ApiKey);
                        else
                            errorMessage = response.StatusCode.ToString();                        
                    }
                    else
                    {
                        //dateServer = response.Headers.Date.ToString();
                        if (response.Headers.Date.HasValue)
                        {                            
                            currentTimeDifference = DateTime.UtcNow.Subtract((DateTime)response.Headers.Date.Value.UtcDateTime);
                            if (currentTimeDifference.TotalSeconds < 1)
                                currentTimeDifference = new TimeSpan(0);

                            result = DriverErrorCodes.ErrorNoError;
                        }
                    }
                }
                catch (Exception e)
                {
                    errorMessage = string.Format("{0} : {1}", Properties.Resources.DataboomErrorGettingRemoteDateTime, e.Message);
                }
            }

            return result;
        }

        private DriverErrorCodes WriteDataToCloudServerDB(DataboomProtocol.DataToSend item)
        {
            DriverErrorCodes result = (DriverErrorCodes)DataboomProtocol.DataboomErrorCodes.ErrorCodeErrorPublish;
            string errorMessage = string.Empty;

            try
            {
                HttpResponseMessage response = clientHttpPost.PostAsync(databoomCommDriver.UrlToPostTo, item.GetHttpContent()).Result; // Blocking call!
                if (!response.IsSuccessStatusCode)
                {
                    if ((response.StatusCode == System.Net.HttpStatusCode.NotFound) || (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed))
                        errorMessage = string.Format("{0} {1}", response.StatusCode, databoomCommDriver.UrlToPostTo);
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        errorMessage = string.Format("{0} {1}", response.StatusCode, databoomCommDriver.ApiKey);
                    else
                        errorMessage = response.StatusCode.ToString();
                }
                else
                {
                    result = DriverErrorCodes.ErrorNoError;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("{0} : {1}", Properties.Resources.DataboomErrorPublish, e.Message);
            }

            if (result != DriverErrorCodes.ErrorNoError)
            {
                clientHttpIsConnected = false;
                SetStateCommandVariableBit(!clientHttpIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                if ((int)result != clientHttpCurrentErrorCode)
                {
                    CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
                    clientHttpCurrentErrorCode = (int)result;
                }
            }

            return result;
        }

        private void RemovePendingData(List<CommJob> list, DateTime currentExecutionTime)
        {
            foreach (var job in list)
                ((DataboomCommJob)job).RemovePendingData(currentExecutionTime);
        }

        private bool GetPendingWriteTime(List<CommJob> list, out DateTime pendingWriteTime, out TimeSpan frequencyOfSendingSignal)
        {
            pendingWriteTime = DateTime.MaxValue;
            frequencyOfSendingSignal = new TimeSpan(0);

            if (list.Count > 0)
                frequencyOfSendingSignal = ((DataboomCommJob)list[0]).FrequencyOfSendingSignal;

            foreach (DataboomCommJob job in list)
            {
                if (job.LastDataWrittenTime < pendingWriteTime)
                    pendingWriteTime = job.LastDataWrittenTime;
            }

            return pendingWriteTime != DateTime.MaxValue;
        }

        private DateTime CalculateTimeCloudServerDB(DateTime localUtcDateTime)
        {
            return localUtcDateTime.Add(differenceFromCloudServerDB);
        }

        private DateTime CalculatePublishTimeToCloudServerDB(DateTime localUtcDateTime, DateTime lastExecutionTime)
        {
            // calculcate publishing date/time to CloudServerDB (offset from localtime --> offset value is calculdate into DeviceOpen())
            DateTime dateServer = CalculateTimeCloudServerDB(localUtcDateTime);

            // new publish date is the current datime (with CloudServerDB time difference) subtract difference from sampling interval time with schedule time
            return dateServer.Subtract(localUtcDateTime - lastExecutionTime);
        }

        private DataboomProtocol.DataToSend PrepareWriteRequest(List<CommJob> list, DateTime dt)
        {            
            DataboomProtocol.DataToSend msg = null;

            // all jobs have the same FrequencyOfSendingSignal; get the "current" sampling date/time                                     
            DateTime currentExecutionTime = ((DataboomCommJob)list[0]).GetCurrentScheduleTime(dt);
            
            DateTime dateServer = CalculatePublishTimeToCloudServerDB(dt, dt);
            
            var payload = new DataboomProtocol.Credentials
            {
                device = deviceToken,
                date = dateServer.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz"),
                signals = new List<DataboomProtocol.Signal>()
            };
            
            foreach (DataboomCommJob j in list)
            {
                DataboomProtocol.Signal sig = new DataboomProtocol.Signal();
                sig.name = j.TagName;
                
                if (j.GetDataValue(currentExecutionTime, out string sigValue))
                {
                    sig.value = sigValue;
#if DEBUG

                    System.Diagnostics.Debug.WriteLine(String.Format("DBG - PrepareWriteRequest ExecutionTime:{0}, TagName:{1}, TagValue:{2},", currentExecutionTime, sig.name, sig.value));
#endif
                    //Added a new signal at the list, for create a JSON string
                    payload.signals.Add(sig);
                }
            }

            if (payload.signals.Count > 0)
            {
                msg = new DataboomProtocol.DataToSend();
                //Creating message (create a JSON string) to send
                msg.Message = JsonConvert.SerializeObject(payload);
                msg.CurrentExecutionTime = currentExecutionTime;                
            }

            return msg;
        }

        private void CreateClientHTTP(ref HttpClient clientHttp)
        {
            clientHttp = new HttpClient();
            clientHttp.DefaultRequestHeaders.Accept.Clear();
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header    
            clientHttp.DefaultRequestHeaders.Add("apikey", databoomCommDriver.ApiKey);
            int timeout = Timeout;
            if (Properties.Settings.Default.TimeoutMinValue != 0 && Timeout < Properties.Settings.Default.TimeoutMinValue)
                timeout = (int)Properties.Settings.Default.TimeoutMinValue;

            clientHttp.Timeout = TimeSpan.FromMilliseconds(timeout);            
        }        
        #endregion                
    }
}
