using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
//using System.Web.Script.Serialization;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;



using Utilities;
using System.Text;
using System.Reflection;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using System.Net.NetworkInformation;

namespace Databoom
{
    public enum DataboomErrorCodes : int
    {
        ErrorCodeErrorPreparingPublishMessage = 1000,
        ErrorCodeErrorSubscription = 1001,
        ErrorCodeErrorPublish = 1002,
        ErrorCodeNoInternetConnection = 1003,
        ErrorCodeExceptionThreadSending = 1003,
    }

    class DataboomChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the DataboomChannel object.
        /// </summary>
        public DataboomChannel(CommunicationDriver commdriver, DataboomChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _DeviceToken = settings.DeviceToken;
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
        public Dictionary<String, List<DataboomCommJob>> mapSubscribedVariablesJobs = new Dictionary<String, List<DataboomCommJob>>();
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

        /// <summary>   The lock list object. </summary>
        protected Object lockWriteToFileObject = new Object();
        #endregion

        #region Sending task
        public const int NUMBERAGGREGATESJOBS = 20;
        private BlockingCollection<object> queueSendingSignals = new BlockingCollection<object>();
        private BlockingCollection<object> queueReplygSignals = new BlockingCollection<object>();
        private bool isStartThread = false;
        private System.Threading.Thread SendingSignalthread = null;
        public HttpClient clientHttpPost = null;
        public HttpClient clientHttpGet = null;
        List<DataboomCommJob> jobCompleteList = new List<DataboomCommJob>();
        List<DataboomCommJob> jobSendingList = new List<DataboomCommJob>();
        Dictionary<UInt32, List<DataboomCommJob>> pendingWriteStateJobs = new Dictionary<UInt32, List<DataboomCommJob>>();
        UInt32 idTokenSendingMessage = 0;
        readonly Object lockDataboomListObject = new Object();
        string conn = string.Empty;

        public struct MessageOfSending
        {
            public UInt32 sendingID;
            public string sendingstring;
        };
        public struct MessageOfReply
        {
            public UInt32 sendingID;
            public string StatusCode;
            public string ReasonPhrase;
        };
        #endregion

        #region Class
        public class Credentials
        {
            public string device { get; set; }
            public string date { get; set; }
            public List<Signal> signals { get; set; }
        }

        public class Signal
        {
            public string name { get; set; }

            public string value { get; set; }
        }

        #endregion

        #region Override Methods
        //
        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);


        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
            {
                sleepCycle = 1;
            }

            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;

            DataboomDriver DataboomCommDriver = (DataboomDriver)CommDriver;

            jobSendingList.Clear();
            // Thread for sending HTTP messages 
            SendingSignalthread = new Thread(SendingSignals);
            SendingSignalthread.IsBackground = true;
            SendingSignalthread.Start();
            string dateServer = string.Empty;
            string messageServerError = string.Empty;
            DateTime lastScheduleTime = DateTime.MinValue;

            object msgReply;

            while (true)
            {
                while (queueReplygSignals.TryTake(out msgReply))
                {
                    DataboomRemovePendingJob(ref msgReply);
                }

                if ((lastScheduleTime == DateTime.MinValue) ||
                    ((DateTime.UtcNow - lastScheduleTime).TotalSeconds >= 1))
                {
                    lastScheduleTime = DateTime.UtcNow;
                    if (ThereIsAListOfExpiredJobs())
                    {
                        messageServerError = GetDateFromCloudServerDB(ref dateServer);
                        CrerteMessageFromSndingOrSave(dateServer, messageServerError);
                    }
                }

                if (StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
                StopWorkerThread.WaitOne(0);

            }

            // Message for terminating the sending thread
            MessageOfSending msgDrestroyThread;
            msgDrestroyThread.sendingID = 0;
            msgDrestroyThread.sendingstring = "q";
            queueSendingSignals.Add(msgDrestroyThread);
            // Wait for the end of the thread
            SendingSignalthread.Join();
            jobSendingList.Clear();

            DeletependingWriteStateJobs();

            try
            {
                clientHttpGet.Dispose();
                clientHttpGet = null;
                clientHttpPost.Dispose();
                clientHttpPost = null;
            }
            catch (Exception)
            {
            }
        }

        public bool CrerteMessageFromSndingOrSave(string dateServer, string messageServerError)
        {
            List<MessageOfSending> strinJsonToSendig = new List<MessageOfSending>();
            CreateStringsListJson(dateServer, ref strinJsonToSendig);

            if (string.IsNullOrEmpty(messageServerError))
            {
                //Prepare  message for sending
                LoadQueueSendingMessage(ref strinJsonToSendig);
                strinJsonToSendig.Clear();
            }
            else
            {
                SaveMessagesListIntoTheFile(ref strinJsonToSendig);
                foreach (CommJob job in jobSendingList)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut, Job = job };
                    eAJob.Job = job;
                    OnJobExecuted(eAJob);
                }
                if (LastErrorCode != DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut)
                {
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                    CommDriver.OnSystemEvent(ObjectIds.Server, messageServerError, Opc.Ua.EventSeverity.High);
                }
            }
            jobSendingList.Clear();
            return (true);
        }
        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            DataboomCommJob j = job as DataboomCommJob;
            //#if DEBUG
            //            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            //            System.Diagnostics.Debug.WriteLine(String.Format("SubscribeJob DBG - {0} Job name - {1}",
            //                                               currentTime, j.TagName));
            //#endif
            if (!jobCompleteList.Contains(j))
            {
                jobCompleteList.Add(j);

            }

            base.SubscribeJob(job, state);
        }
        public void DataBoomUnsubscribeJob(CommJob job)
        {
            DataboomCommJob j = job as DataboomCommJob;
            //#if DEBUG
            //            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            //            System.Diagnostics.Debug.WriteLine(String.Format("DataBoomUnsubscribeJob DBG - {0} Job name - {1}",
            //                                               currentTime, j.TagName));
            //#endif
            if (jobCompleteList.Contains(j))
            {
                jobCompleteList.Remove(j);
            }
            base.UnsubscribeJob(job);
        }
        public override bool ProcessNewData(CommJob pendingjob)
        {
            return (true);
        }

        #endregion

        #region Local Methods


        protected bool ThereIsAListOfExpiredJobs()
        {
            bool res = false;
            DateTime date = DateTime.UtcNow;

            var sync = new object();
            Parallel.ForEach(
                  jobCompleteList,
                  job => {
                      if ((job.lastExecutionTimeSchede == DateTime.MinValue) ||
                         ((DateTime.UtcNow - job.lastExecutionTimeSchede).TotalSeconds >= job.FrequencyOfSendingSignal))
                      {
                          if (!jobSendingList.Contains(job))
                          {
                              job.lastExecutionTimeSchede = date;
                              lock (sync)
                              {
                                  job.manageTagsListToWrite();
                                  jobSendingList.Add(job);
                              }
                          }
                      }
                  });


            if (jobSendingList.Count > 0)
            {
                res = true;
            }

            return res;
        }

        public string GetDateFromCloudServerDB(ref string dateServer)
        {
            string messageError = string.Empty;
            dateServer = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            DataboomDriver DataboomCommDriver = (DataboomDriver)CommDriver;

            if (!CheckInternetConnection())
            {
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                messageError = Properties.Resources.DatabommErrorInternetNotAvailable;
                return (messageError);

            }
            try
            {
                CreateClientHTTP(ref clientHttpGet);
                HttpResponseMessage response = clientHttpGet.GetAsync(DataboomCommDriver.UrlClockPost).Result; // Blocking call!
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        messageError = response.StatusCode.ToString() + " " + DataboomCommDriver.UrlClockPost.ToString();
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        messageError = response.StatusCode.ToString() + " " + DataboomCommDriver.ApiKey.ToString();
                    else
                        messageError = response.StatusCode.ToString();

                    SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);

                    return (messageError);
                }
                else
                {
                    dateServer = response.Headers.Date.ToString();
                }
            }
            catch (Exception e)
            {
                messageError = "Exception request date and time";
                return (messageError);
            }
            return (messageError);
        }

        bool CreateStringsListJson(string dateServer, ref List<MessageOfSending> strinJsonToSendig)
        {
            uint maximumNumberOfJobsToSending = NUMBERAGGREGATESJOBS;

            if ((CommDriver.AggregationLimit > 0) &&
                (CommDriver.AggregationLimit < NUMBERAGGREGATESJOBS))
            {
                maximumNumberOfJobsToSending = CommDriver.AggregationLimit;
            }

            uint numberJobsEntered = 0;
            bool result = false;
            List<DataboomCommJob> lisOfJobsIntoTheMessagge = null;
            MessageOfSending msg;

            var payload = new Credentials
            {
                device = DeviceToken,
                date = dateServer,
                signals = new List<Signal>()
            };
            foreach (DataboomCommJob job in jobSendingList)
            {
                if (numberJobsEntered < maximumNumberOfJobsToSending)
                {
                    if (lisOfJobsIntoTheMessagge == null)
                    {
                        lisOfJobsIntoTheMessagge = new List<DataboomCommJob>();
                    }

                    Signal sig = new Signal();
                    sig.copyPropertiesFrom(job.GetSignalValue());

                    //Added a new signal at the list, for cerate a JSON string
                    payload.signals.Add(sig);

                    //Added a new signal at the list, for cerate a JSON string
                    lisOfJobsIntoTheMessagge.Add(job);
                    numberJobsEntered++;
                }
                else
                {
                    Signal sig = new Signal();
                    sig.copyPropertiesFrom(job.GetSignalValue());

                    //Added a new signal at the list, for cerate a JSON string
                    payload.signals.Add(sig);

                    //Added a new signal at the list, for cerate a JSON string
                    lisOfJobsIntoTheMessagge.Add(job);

                    // Creating a JSON string
                    string jsonString = JsonString(ref payload);

                    //Saving a jobs list with token id 
                    pendingWriteStateJobs[idTokenSendingMessage] = lisOfJobsIntoTheMessagge;
                    lisOfJobsIntoTheMessagge = null;

                    //Creating message to sending
                    msg.sendingID = idTokenSendingMessage;
                    msg.sendingstring = jsonString;
                    unchecked
                    {
                        idTokenSendingMessage++;
                    }

                    //message added to the list of messages to send
                    strinJsonToSendig.Add(msg);
                    payload.signals.Clear();

                    numberJobsEntered = 0;
                }
            }
            if (payload.signals.Count > 0)
            {
                string jsonString = JsonString(ref payload);

                //Saving a jobs list with token id
                pendingWriteStateJobs[idTokenSendingMessage] = lisOfJobsIntoTheMessagge;
                lisOfJobsIntoTheMessagge = null;

                //Creating message to sending
                msg.sendingID = idTokenSendingMessage;
                msg.sendingstring = jsonString;
                unchecked
                {
                    idTokenSendingMessage++;
                }

                //message added to the list of messages to send
                strinJsonToSendig.Add(msg);
                payload.signals.Clear();
                numberJobsEntered = 0;
            }
            return (result);
        }

        public string JsonString(ref Credentials cred)
        {
            return JsonConvert.SerializeObject(cred);
        }

        public void CreateClientHTTP(ref HttpClient clientHttp)
        {
            if (clientHttp == null)
            {
                DataboomDriver DataboomCommDriver = (DataboomDriver)CommDriver;
                clientHttp = new HttpClient();
                clientHttp.DefaultRequestHeaders.Accept.Clear();
                clientHttp.DefaultRequestHeaders
                                            .Accept
                                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header    
                clientHttp.DefaultRequestHeaders.Add("apikey", DataboomCommDriver.ApiKey);
                // Set timeout (minimum value == 5000)
                double timeout = 5000;
                if (Timeout > 5000)
                {
                    timeout = Timeout;
                }

                clientHttp.Timeout = TimeSpan.FromMilliseconds(timeout);
            }
        }

        public bool LoadQueueSendingMessage(ref List<MessageOfSending> strinJsonToSendig)
        {
            bool res = false;

            foreach (MessageOfSending msg in strinJsonToSendig)
            {
                queueSendingSignals.Add(msg);
#if DEBUG
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("LoadQueueSendingMessage DBG - {0} sendstring - {1}",
                                                   currentTime, (msg.sendingstring.ToString())));
#endif
                if (!res)
                {
                    res = true;
                }
            }

            return (res);
        }


        private void SendingSignals()
        {
            Thread.CurrentThread.Name = "SendingSignals";
            isStartThread = true;
            DataboomDriver DataboomCommDriver = (DataboomDriver)CommDriver;
            MessageOfReply mssRpl;
            object item;

            while (true)
            {
                item = queueSendingSignals.Take();

                // Message for terminating the thread 
                if (((MessageOfSending)item).sendingstring == "q")
                {
                    break;
                }
                else
                {
                    try
                    {
                        if (!CheckInternetConnection())
                        {
                            mssRpl.sendingID = ((MessageOfSending)item).sendingID;
                            mssRpl.StatusCode = "No Internet";
                            mssRpl.ReasonPhrase = Properties.Resources.DatabommErrorInternetNotAvailable;
                            SaveFileBackupMsgDataBoom(((MessageOfSending)item));
                            queueReplygSignals.Add(mssRpl);
                            continue;
                        }
                        CreateClientHTTP(ref clientHttpPost);
                        //#if DEBUG
                        //                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        //                        System.Diagnostics.Debug.WriteLine(String.Format("SendingSignals DBG - {0} sendstring - {1}",
                        //                                                           currentTime, ((MessageOfSending)item).sendingstring.ToString()));
                        //#endif

                        StringContent httpContent = new StringContent(((MessageOfSending)item).sendingstring, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = clientHttpPost.PostAsync(DataboomCommDriver.UrlToPostTo, httpContent).Result; // Blocking call!
                        if (response.IsSuccessStatusCode)
                        {
                            mssRpl.ReasonPhrase = response.ReasonPhrase.ToString();
                            ReadFileBacKupMsgDataBoom();
                        }
                        else
                        {
                            string messageError = string.Empty;
                            if ((response.StatusCode == System.Net.HttpStatusCode.NotFound) ||
                                (response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed))
                                messageError = response.StatusCode.ToString() + " " + DataboomCommDriver.UrlToPostTo.ToString();
                            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                messageError = response.StatusCode.ToString() + " " + DataboomCommDriver.ApiKey.ToString();
                            else
                                messageError = response.StatusCode.ToString();

                            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);

                            mssRpl.ReasonPhrase = messageError;
                            SaveFileBackupMsgDataBoom(((MessageOfSending)item));
                        }
#if DEBUG
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("SendingSignals DBG - {0} sendstring - {1}",
                                                           currentTime, response.StatusCode.ToString()));
#endif
                        mssRpl.sendingID = ((MessageOfSending)item).sendingID;
                        mssRpl.StatusCode = response.StatusCode.ToString();
                        queueReplygSignals.Add(mssRpl);
                    }
                    catch (Exception e)
                    {
                        mssRpl.sendingID = ((MessageOfSending)item).sendingID;
                        mssRpl.StatusCode = "Exception";
                        mssRpl.ReasonPhrase = "";
                        queueReplygSignals.Add(mssRpl);
                    }
                }

            } //End While

            isStartThread = false;
        }

        ////check internet connection
        //[System.Runtime.InteropServices.DllImport("wininet.dll")]
        //private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        //public static bool CheckInternetConnection()
        //{
        //    int desc;
        //    return InternetGetConnectedState(out desc, 0);
        //    return true;
        //}

        public static bool CheckInternetConnection()
        {         
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var gatewayAddr in adapter.GetIPProperties().GatewayAddresses)
                {
                    // if gateway address is NOT 0.0.0.0 and the network card status is UP then we've found the main network card
                    if (gatewayAddr.Address.ToString() != "0.0.0.0" && adapter.OperationalStatus == OperationalStatus.Up)
                        return true;
                }
            }

            return false;
        }

        public bool DataboomRemovePendingJob(ref object msgReply)
        {
            bool ret = false;
            if (pendingWriteStateJobs.ContainsKey(((MessageOfReply)msgReply).sendingID))
            {
                List<DataboomCommJob> listJobs = pendingWriteStateJobs[((MessageOfReply)msgReply).sendingID];
                if ((listJobs != null) && (listJobs.Count > 0))
                {
                    string StatusCode = ((MessageOfReply)msgReply).StatusCode;
                    foreach (var job in listJobs)
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = job };
                        if (StatusCode == "OK")
                        {
                            eAJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        }
                        else
                        {
                            eAJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                        }
                        eAJob.Job = job;
                        OnJobExecuted(eAJob);
                    }

                    listJobs.Clear();

                    if (StatusCode != "OK")
                    { 
                        if (LastErrorCode != DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut)
                        {
                            LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                            CommDriver.OnSystemEvent(ObjectIds.Server, ((MessageOfReply)msgReply).ReasonPhrase, Opc.Ua.EventSeverity.High);
                        }
                }
                }
               

                pendingWriteStateJobs.Remove(((MessageOfReply)msgReply).sendingID);
            }
            return (ret);
        }

        private void DeletependingWriteStateJobs()
        {
            if (pendingWriteStateJobs.Count < 1)
            {
                return;
            }
            foreach (KeyValuePair<UInt32, List<DataboomCommJob>> kvp in pendingWriteStateJobs)
            {
                List<DataboomCommJob> listJob = kvp.Value;
                if ((listJob != null) && (listJob.Count > 0))
                {
                    listJob.Clear();
                }
            }
            pendingWriteStateJobs.Clear();
        }

        #endregion

        #region Properties


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of subscribed job in the channel. </summary>
        ///
        /// <value> The subscribed jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long DataboomSubscribedJobs
        {
            get
            {
                long result = 0;
                lock (lockDataboomListObject)
                {
                    result = jobCompleteList.Count;
                }

                return result;
            }
        }

        #endregion

        #region methods

        public void OnJobPublished(ExecutedJobArgs e)
        {
            if (e.ErrorCode == (DriverCodeBase.Enumerators.DriverErrorCodes)DataboomErrorCodes.ErrorCodeErrorPublish)
            {
                DataboomCommJob pnj = (DataboomCommJob)e.Job;
                String errorMsg = String.Format(Properties.Resources.DataboomErrorPublishing, pnj.TagName, Name);
                CommDriver.OnSystemEvent(null, errorMsg, EventSeverity.Min);
            }

            base.OnJobExecuted(e);
        }

        #region File Managment 
        public bool ReadFileBacKupMsgDataBoom()
        {
            DataboomStation pStation =  (DataboomStation)CommDriver.GetChannelStations(this).First();
            string projectFile = GetPathToTheFileSaveData(pStation.Name.ToString());
            MessageOfSending MsgSending;

            if (projectFile == string.Empty)
            {
                return (false);
            }

            if (!File.Exists(projectFile))
            {
                return (true);
            }

            using (StreamReader reader = new StreamReader(projectFile))
            {
                while (!reader.EndOfStream)
                {
                    MsgSending.sendingID = Convert.ToUInt32(reader.ReadLine());
                    MsgSending.sendingstring = reader.ReadLine();
                    queueSendingSignals.Add(MsgSending);
                }
            }

            File.Delete(projectFile);

            return (true);
        }

        public bool SaveMessagesListIntoTheFile(ref List<MessageOfSending> strinJsonToSendig)
        {
            foreach (MessageOfSending msg in strinJsonToSendig)
            {
                SaveFileBackupMsgDataBoom(msg);
//#if DEBUG
//                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
//                System.Diagnostics.Debug.WriteLine(String.Format("LoadQueueSendingMessage DBG - {0} sendstring - {1}",
//                                                   currentTime, (msg.sendingstring.ToString())));
//#endif
  
            }
            return (true);
        }


        public bool SaveFileBackupMsgDataBoom(MessageOfSending MsgSending)
        {
            DataboomStation pStation = (DataboomStation)CommDriver.GetChannelStations(this).First();
            string projectFile = GetPathToTheFileSaveData(pStation.Name.ToString());

            if (projectFile == string.Empty)
            {
                return false;
            }

            lock (lockWriteToFileObject)
            {
                if (!File.Exists(projectFile))
                {
                    if (!CreateSubfolder(projectFile))
                        return false;

                    using (StreamWriter sw = File.CreateText(projectFile))
                    {
                        sw.WriteLine(MsgSending.sendingID);
                        sw.WriteLine(MsgSending.sendingstring);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(projectFile))
                    {
                        sw.WriteLine(MsgSending.sendingID);
                        sw.WriteLine(MsgSending.sendingstring);
                    }
                }
            }
            
            return (true);
        }

        bool CreateSubfolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            string filename = Path.GetFileName(path);
            string path_temp =@path.Substring(0, path.IndexOf(filename));
            try
            {
                Directory.CreateDirectory(path_temp);
            }
            catch (Exception e)
            {
                return false;
            }

                return true;
        }

        string GetPathToTheFileSaveData(string StationName)
        {
            DataboomDriver DataboomCommDriver = (DataboomDriver)CommDriver;
            string conn = CommunicationDriver.GetConnectionString(DataboomCommDriver.StrConnectionString, "Drivers", CommDriver.DriverName + Name, ".saveData");
            string fileBase;
            InMemoryDataStore inMemory;
            CommunicationDriver.GetSpecificDataLayer(conn, out fileBase, out inMemory, out bool targetIsFile);
            if (fileBase == null && inMemory == null)//Save project to SQL
            {
                Int32 SearchStart = conn.IndexOf("catalog=") + 8;
                Int32 SearchEnd = conn.IndexOf(";", SearchStart);
                string projectName;

                if (SearchEnd == -1)
                { 
                    projectName = conn.Substring(SearchStart);
                }
                else
                {
                    projectName = conn.Substring(SearchStart,(SearchEnd - SearchStart));
                }
                
                fileBase = String.Format("{0}\\{1}\\{2}\\{3}\\{4}\\{5}",
                                         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                         Properties.Settings.Default.CompanyName,
                                         Properties.Settings.Default.CommonApplicationFolder,
                                         projectName,
                                         CommDriver.DriverName,
                                         Name + ".saveData");
            }
            return fileBase;
        }

        //IDataLayer GetSpecificDataLayer(string conn, out string filebase, out InMemoryDataStore InMemory, bool xml = true)
        //{
        //    IDataLayer dl = null;
        //    ConnectionStringParser helper = new ConnectionStringParser(conn);
        //    string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

        //    filebase = null;
        //    InMemory = null;

        //    if (providerType != InMemoryDataStore.XpoProviderTypeString)
        //    {
        //        dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        //    }
        //    else if (xml)
        //    {
        //        filebase = helper.GetPartByName("data source");
        //        InMemory = CommunicationDriver.GetDataStore(filebase);
        //        dl = new SimpleDataLayer(InMemory);
        //    }
        //    return dl;
        //}

        #endregion

        public void AddToMapSubscribedVariablesJobs(String variableName, DataboomCommJob job)
        {
            if (!String.IsNullOrWhiteSpace(variableName))
            {
                lock (lockSubscribedVarMap)
                {
                    if (!mapSubscribedVariablesJobs.ContainsKey(variableName))
                    {
                        List<DataboomCommJob> ListJobs = new List<DataboomCommJob>();
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

        #endregion

        #region properties

        /// <summary>   Device Token. </summary>
        private string _DeviceToken;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Token. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceToken
        {
            get { return _DeviceToken; }
        }


        #endregion
    }
}
