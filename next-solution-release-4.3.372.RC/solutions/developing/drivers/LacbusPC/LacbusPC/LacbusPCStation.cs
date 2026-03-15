using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DriverCodeBase;
using DevExpress.Xpo;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;

namespace LacbusPC
{
    class LacbusPCStation : Station
    {
        public const ushort COMMUNICATION_DURATION = 3600;

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public LacbusPCStation(CommunicationDriver commdriver, LacbusPCStationSettings settings)
            : base(commdriver, settings)
        {
            _LacbusPCRTUNumber = settings.LacbusPCRTUNumber;
            _LacbusPCProtocolType = settings.LacbusPCProtocolType;
            _LacbusPCPhoneNumber = settings.LacbusPCPhoneNumber;
            _LacbusPCAutomaticPollRTU = settings.LacbusPCAutomaticPollRTU;
            _LacbusPCCommunicationDuration = settings.LacbusPCCommunicationDuration;
            pollRTUCommunicationDuration = LacbusPCCommunicationDuration;
            if (pollRTUCommunicationDuration < 1)
            {
                pollRTUCommunicationDuration = 1;
            }
            LacbusPCAutomaticPollRTUFrequency = settings.LacbusPCAutomaticPollRTUFrequency;
        }

        #endregion

        #region Data members
        public int pollRTURequestMessageNumber = 0;
        public DateTime lastPollRTURequestTime = DateTime.MinValue;
        public bool pollRTURequestPending = false;
        protected Object lockPollRequestObject = new Object();
        public ushort pollRTUCommunicationDuration = 1;
        public bool rtuIsConnected = false;
        public DateTime lastActivePollStateTime = DateTime.MinValue;
        public bool activePollState = false;
        public DateTime lastDataReceivedTime = DateTime.MinValue;
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as LacbusPCCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new LacbusPCCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as LacbusPCTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new LacbusPCCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new LacbusPCTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as LacbusPCCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new LacbusPCCommJobSettings(session, commJob);
        }

        public override bool Startup()
        {
            // Call the base class method
            if (!base.Startup())
            {
                return false;
            }

            // Build the dictionary of the input jobs
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (Channel != null)
                {
                    LacbusPCChannel lChannel = (LacbusPCChannel)Channel;
                    lock (lockListObject)
                    {
                        foreach (var job in ListWholeJob)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)job;
                            if ((lJob.Type == LinkType.Input) || (lJob.Type == LinkType.InputOutput))
                            {
                                UInt64 searchKey = LacbusPcProtocol.CalculateJobSearchKey(LacbusPCRTUNumber, lJob.LacbusPCDatumNumber, (byte)lJob.LacbusPCDatumType, (byte)lJob.LacbusPCDatumCategory);
                                lChannel.AddToMapInputJobs(searchKey, lJob);
                            }
                        }
                    }
                }
            });

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the job executed action. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="ExecutedJobArgs"> The ExecutedJobArgs to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void OnJobExecuted(object sender, ExecutedJobArgs e)
        {
            if (e.Job.Station != this)
            {
                return;
            }

            //GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
            //{
                if (StatisticsData != null)
                {
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalRxBytes.ToString(), e.RxBytes);
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTxBytes.ToString(), e.TxBytes);
                    e.RxBytes = 0;
                    e.TxBytes = 0;

                    if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                    {
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                        uint quality;
                        String DiagnErrorMessage = "";
                        CommDriver.GetDriverErrorInfo((int)e.ErrorCode, out quality, out DiagnErrorMessage);
                        StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
                    }
                    else
                    {
                        if (e.Job.IsRead)
                        {
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsRead.ToString());
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsRead.ToString(), e.Job.ExchangedTag);
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesRead.ToString(), e.Job.ExchangedByte);
                        }
                        else
                        {
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsWrite.ToString());
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsWrite.ToString(), e.Job.ExchangedTag);
                            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesWrite.ToString(), e.Job.ExchangedByte);
                        }
                        TotalPartialJobs++;
                        TotalPartialTags += e.Job.ExchangedTag;
                        TotalPartialBytes += e.Job.ExchangedByte;
                    }
                }
                ProcessJobValues(e);
                ProcessJobLists(e.Job, e.ErrorCode);
                lock (e.Job.retLockList())
                {
                    if (e.Job.RWState == CommJob.RWStates.ReadForRW &&
                        e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        e.Job.RWState = CommJob.RWStates.WriteForRW;
                        ChannelBase.ChangeStateJob(e.Job, CommJobState.PollingNow);
                    }
                    else
                        e.Job.RWState = CommJob.RWStates.Standard;
                }

                if (StatisticsData != null)
                {
                    if (InErrorState)
                        StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                    else
                        StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);
                }

                e.Job.IsPending = false;
            //});
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            LacbusPCCommJob lJ = e.Job as LacbusPCCommJob;
            if (lJ == null)
            {
                return;
            }

            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (ParseReceivedData(Answer, ref lJ, ref ChangedTags))
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                            {
                                e.ChangedTags.Add(j);
                            }
                        }
                    }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
                else
                {
                    e.ChangedTags.AddRange(lJ.TagsList);
                }
            }
            else if (e.ErrorCode == DriverErrorCodes.ErrorTimeOut)
            {
                lock (lockBool)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        foreach (var commjob in ListWholeJob)
                            if (commjob.InUse)
                                listJob.Add(commjob);
                    }

                    foreach (CommJob commjob in listJob)
                    {
                        commjob.SetErrorState((int)e.ErrorCode);
                        LacbusPCCommJob j = commjob as LacbusPCCommJob;
                        ChannelBase.ChangeStateJob(commjob, CommJobState.PollingInError);
                    }
                }
            }

            bool bAllJobsInError = e.GeneralError;
            bool inInErrorState = e.Job.InErrorState;

            if (e.ErrorCode != DriverErrorCodes.ErrorNoError && bAllJobsInError)
            {
                //all jobs in error
                List<CommJob> erlist = new List<CommJob>();
                lock (lockListObject)
                {
                    erlist.AddRange(ListWholeJob);
                }
                foreach (var j in erlist)
                {
                    /*When a general error occurs, the jobs are put in error, it is necessary 
                     * to distinguish the jobs that have conditional variables or jobs excepionOutput, 
                     * because at the resumption of communication (eg Timeout), 
                     * jobs of type exceptionoutput and jobs with conditional variables, 
                     * would keep in error the station until their execution.*/
                    if ((e.Job == j) || ((j.Type != LinkType.ExceptionOutput) && !j.ConditionalVariableSet))
                        j.SetErrorState((int)e.ErrorCode);
                }
            }
            else if ((e.ChangedTags == null) || (e.ChangedTags.Count == 0)) 
            {
                e.Job.SetErrorState((int)e.ErrorCode);
            }

            if (inInErrorState && !e.Job.InErrorState)
                Channel.QuickPollingJobInError();

            if ((e.ErrorCode != DriverErrorCodes.ErrorNoError) || (bAllJobsInError == true))
            {
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
            }
            else
            {
                if (Channel.InErrorJobs(this) == 0)
                {
                    SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                }
                e.Job.ResetConditionalVariable();
            }

            if (e.ChangedTags.Count > 0)
            {
                uint quality;
                string error;
                GetCommDriver().GetDriverErrorInfo((int)e.ErrorCode, out quality, out error);
                LacbusPCCommJob lJob = (LacbusPCCommJob)e.Job;
                lJob.SetLacbusJobErrorState(quality, error, e.Timestamp);
                foreach (var tag in e.ChangedTags)
                {
                    GetCommDriver().OnTagChangedValueQualityTimestamp(tag.TagNode.NodeId, tag.Value, quality, e.Timestamp);
                }
            }
        }

        #endregion

        #region Specific Methods

        public bool MustSendPollRequest(int timeout)
        {
            bool returnValue = false;

            lock (lockPollRequestObject)
            {
                if (IsActive() &&
                    (ListWholeJob.Count > 0) &&
                    (pollRTURequestPending == false) &&
                    (LacbusPCAutomaticPollRTU == true) &&
                    ((LacbusPCProtocolType == LacbusPcUnderlyingProtocols.LacbusRTU) || (LacbusPCProtocolType == LacbusPcUnderlyingProtocols.SofbusPL)) &&
                    (activePollState == false) &&
                    ((lastActivePollStateTime == DateTime.MinValue) || ((DateTime.UtcNow - lastActivePollStateTime).TotalSeconds > LacbusPCAutomaticPollRTUFrequency)))
                {
                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - MustSendPollRequest return True for station {1} - lastActivePollStateTime == {2} (DateTime.UtcNow - lastActivePollStateTime).TotalSeconds == {3}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), Name, lastActivePollStateTime, (DateTime.UtcNow - lastActivePollStateTime).TotalSeconds);
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        public bool AutomaticPollingEnabled()
        {
            bool returnValue = false;

            if ((LacbusPCAutomaticPollRTU == true) &&
                ((LacbusPCProtocolType == LacbusPcUnderlyingProtocols.LacbusRTU) || (LacbusPCProtocolType == LacbusPcUnderlyingProtocols.SofbusPL)))
            {
                returnValue = true;
            }

            return (returnValue);
        }

        public void ManagePollSendError()
        {
            lock (lockPollRequestObject)
            {
                lastPollRTURequestTime = DateTime.UtcNow;
                pollRTURequestPending = false;
                pollRTUCommunicationDuration = (ushort)(Channel.PollingTimeInError / 1000);
                if (pollRTUCommunicationDuration < 1)
                {
                    pollRTUCommunicationDuration = 1;
                }
            }

            ManagePollRTURequestFailed();
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station: {1} - ManagePollSendError error sending the RTU Poll Request", DateTime.Now, Name);
#endif
        }

        public void ManagePollReply(int errorCode)
        {
            lock (lockPollRequestObject)
            {
                if (errorCode == 0)
                {
                    lastPollRTURequestTime = DateTime.UtcNow;
                    pollRTUCommunicationDuration = LacbusPCCommunicationDuration;
                }
                else
                {
                    lastPollRTURequestTime = DateTime.UtcNow;
                    pollRTUCommunicationDuration = (ushort)(Channel.PollingTimeInError / 1000);
                }
                if (pollRTUCommunicationDuration < 1)
                {
                    pollRTUCommunicationDuration = 1;
                }
                pollRTURequestPending = false;
            }
            if (errorCode != 0)
            {
                ManagePollRTURequestFailed();
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station: {1} - ManagePollReply received reply with error code {2}", DateTime.Now, Name, errorCode);
#endif
        }

        public bool CheckPollTimeout(DateTime currentUTCTime, int timeout)
        {
            bool returnValue = true;

            lock (lockPollRequestObject)
            {
                if ((currentUTCTime - lastPollRTURequestTime).TotalMilliseconds > timeout)
                {
                    pollRTURequestPending = false;
                    lastPollRTURequestTime = currentUTCTime;
                    pollRTUCommunicationDuration = (ushort)(Channel.PollingTimeInError / 1000);
                    if (pollRTUCommunicationDuration < 1)
                    {
                        pollRTUCommunicationDuration = 1;
                    }
                    returnValue = false;
                }
            }
            if (returnValue == false)
            {
                ManagePollRTURequestFailed();
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - CheckPollTimeout: timeout elapsed", DateTime.Now, Name);
#endif
            }

            return (returnValue);
        }

        public void ManageNoCommunicationNotification()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManageNoCommunicationNotification has been called", DateTime.Now, Name);
#endif
        }

        public void ManageRTUFaultNotification()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManageRTUFaultNotification has been called, rtuIsConnected = {2}", DateTime.Now, Name, rtuIsConnected);
#endif
            if(rtuIsConnected)
            {
                rtuIsConnected = false;
                LastErrorCode = (DriverErrorCodes)(LacbusPCErrorCodes.ErrorRTUDisconnected + LacbusPCRTUNumber);
                if (ListWholeJob.Count > 0)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }
                    Parallel.ForEach(listJob, job =>
                    {
                        // Deactivate the job
                        LacbusPCCommJob lJob = (LacbusPCCommJob)job;
                        lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                    });

                    ExecutedJobArgs e = new ExecutedJobArgs();
                    e.Job = listJob[0];
                    e.ErrorCode = (DriverErrorCodes)(LacbusPCErrorCodes.ErrorRTUDisconnected + LacbusPCRTUNumber);
                    // Put all the jobs in error
                    e.GeneralError = true;

                    base.ProcessJobValues(e);
                }

                InErrorState = true;
            }

            // Change the polling frequency of the station
            lock (lockPollRequestObject)
            {
                pollRTUCommunicationDuration = (ushort)(Channel.PollingTimeInError / 1000);
                if (pollRTUCommunicationDuration < 1)
                {
                    pollRTUCommunicationDuration = 1;
                }
            }
        }

        public bool IsActive()
        {
            bool returnValue = true;

            bool suspendBitValue = false;
            if (GetStateCommandVariableBit(ref suspendBitValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if(suspendBitValue == true)
                {
                    return (false);
                }
            }

            return (returnValue);
        }

        private bool ParseReceivedData(byte[] receivebuffer, ref LacbusPCCommJob tcJ,
                                       ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool managelockStatus = tcJ.LockMustBeManaged();

            uint ArraySize = tcJ.TagsList[0].TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            int TotalJobSize = 0;
            if(!managelockStatus)
            {
                if (!tcJ.isProtocolBool() || (tcJ.ElementNumber == 0 && (uint)tcJ.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
                {
                    if (tcJ.ElementNumber > 0 || tcJ.ProtocolDataSizeBig())
                    {
                        if (tcJ.TagsList[0].TagNode.ArrayDimension == 0)
                            TotalJobSize = (int)(tcJ.GetProtocolDataByteSize());
                        else
                            TotalJobSize = (int)(tcJ.GetProtocolDataByteSize() * tcJ.TagsList[0].TagNode.ArrayDimension);
                    }
                    else
                        TotalJobSize = (int)tcJ.TotalJobSize;

                }
                else
                    TotalJobSize = (int)((ArraySize + 7) / 8);
            }
            else
            {
                TotalJobSize = (int)tcJ.TotalJobSize;
            }

            int ReceivedBytes = receivebuffer.Length;
            if (ReceivedBytes < TotalJobSize)
            {
                return false;
            }
            else if (ReceivedBytes > TotalJobSize)
            {
                ReceivedBytes = TotalJobSize;
            }

            byte[] tempBuffer;
            if (!tcJ.isProtocolBool() || managelockStatus ||
                (tcJ.ElementNumber == 0 && (uint)tcJ.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
            {
                tempBuffer = new byte[ReceivedBytes];
                Array.Copy(receivebuffer, tempBuffer, ReceivedBytes);
            }
            else
            {
                ReceivedBytes = (int)tcJ.TagsList[0].TagNode.ArrayDimension;
                if (ReceivedBytes == 0)
                    ReceivedBytes = 1;
                tempBuffer = new byte[ReceivedBytes];
                for (ushort bitIndex = 0; bitIndex < ReceivedBytes; bitIndex++)
                {
                    tempBuffer[bitIndex] = (byte)((receivebuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                }
            }

            if(!managelockStatus)
            {
                tcJ.SetJobData(tempBuffer, ref changed);
            }
            else
            {
                tcJ.SetJobValueAndLockStatus(tempBuffer, ref changed);
            }

            items.AddRange(changed);

            return true;
        }

        public void ManagePollRTURequestFailed()
        {
            rtuIsConnected = false;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManagePollRTURequestFailed has been called, rtuIsConnected set to = {2}", DateTime.Now, Name, rtuIsConnected);
#endif
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }

                var listInputJobs = (from j in listJob.AsParallel() where j.Type == LinkType.Input || j.Type == LinkType.InputOutput select j).ToList();
                foreach (var j in listInputJobs)
                {
                    // Deactivate the job
                    LacbusPCCommJob lJob = (LacbusPCCommJob)j;
                    lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                    // Put the job in error
                    ExecutedJobArgs e = new ExecutedJobArgs();
                    e.Job = lJob;
                    e.ErrorCode = (DriverErrorCodes)(LacbusPCErrorCodes.ErrorCodeRTUPollRequestFailed + LacbusPCRTUNumber);
                    base.ProcessJobValues(e);
                }
            }
        }

        public void ManageConnectionBroken()
        {
            rtuIsConnected = false;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManageConnectionBroken has been called, rtuIsConnected set to = {2}", DateTime.Now, Name, rtuIsConnected);
#endif
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var job in listJob)
                {
                    // Deactivate the job
                    LacbusPCCommJob lJob = (LacbusPCCommJob)job;
                    lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                }

                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(LacbusPCErrorCodes.ErrorCodeConnectionBroken);
                // Put all the jobs in error
                e.GeneralError = true;

                base.ProcessJobValues(e);
            }

            // Change the polling frequency of the station
            lock (lockPollRequestObject)
            {
                pollRTURequestPending = false;
                pollRTUCommunicationDuration = (ushort)(Channel.PollingTimeInError / 1000);
                if (pollRTUCommunicationDuration < 1)
                {
                    pollRTUCommunicationDuration = 1;
                }
            }

            FirstTime = true;
        }

        public void ManageConnectionRestored()
        {
            rtuIsConnected = true;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManageConnectionRestored has been called, rtuIsConnected set to = {2}", DateTime.Now, Name, rtuIsConnected);
#endif
        }

        public void ManageRTUConnectionRestored()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Station {1} - ManageRTUConnectionRestored has been called, rtuIsConnected = {2}", DateTime.Now, Name, rtuIsConnected);
#endif
            if (rtuIsConnected == false)
            {
                rtuIsConnected = true;
                InErrorState = false;
            }
            SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
        }

        #endregion

        #region Properties

        private UInt16 _LacbusPCRTUNumber;
        public UInt16 LacbusPCRTUNumber
        {
            get { return _LacbusPCRTUNumber; }
            set
            {
                _LacbusPCRTUNumber = value;
            }
        }

        private LacbusPcUnderlyingProtocols _LacbusPCProtocolType;
        public LacbusPcUnderlyingProtocols LacbusPCProtocolType
        {
            get { return _LacbusPCProtocolType; }
            set
            {
                _LacbusPCProtocolType = value;
            }
        }

        private string _LacbusPCPhoneNumber;
        public string LacbusPCPhoneNumber
        {
            get { return _LacbusPCPhoneNumber; }
            set
            {
                _LacbusPCPhoneNumber = value;
            }
        }

        private bool _LacbusPCAutomaticPollRTU;
        public bool LacbusPCAutomaticPollRTU
        {
            get { return _LacbusPCAutomaticPollRTU; }
            set { _LacbusPCAutomaticPollRTU = value; }
        }

        private UInt16 _LacbusPCCommunicationDuration;
        public UInt16 LacbusPCCommunicationDuration
        {
            get { return _LacbusPCCommunicationDuration; }
            set { _LacbusPCCommunicationDuration = value; }
        }

        private UInt16 _LacbusPCAutomaticPollRTUFrequency;
        public UInt16 LacbusPCAutomaticPollRTUFrequency
        {
            get { return _LacbusPCAutomaticPollRTUFrequency; }
            set { _LacbusPCAutomaticPollRTUFrequency = value; }
        }

        #endregion

    }
}
