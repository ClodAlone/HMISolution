using System;
using System.Collections.Generic;
using System.Net;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using IpDriverCodeBase;

namespace DICom
{
    public class DIComStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public DIComStation(CommunicationDriver commdriver, DIComStationSettings settings)
            : base(commdriver, settings)
        {
            _DeviceHostName = settings.DeviceHostName;
            recordConfigs = new Dictionary<int, Dictionary<int, List<DIComCommJob>>>();
            _ClientStateCommandTag = settings.ClientStateCommandTag;
            if (_ClientStateCommandTag != null && !NodeId.IsNull(_ClientStateCommandTag.NodeId))
            {
                clientStateCommandVariable = new StateCommandVariable(_ClientStateCommandTag.Name, _ClientStateCommandTag.NodeId.ToString());
            }
        }

        #endregion

        #region Data Members
        // Dictionary<record id, Dictionary<var var index, List<all jobs with same var name>>> recordConfigs;
        private Dictionary<int, Dictionary<int, List<DIComCommJob>>> recordConfigs;
        protected Object lockProcessJob = new Object();
        /// <summary>  The State/Command variable associated to the station. </summary>
        private StateCommandVariable clientStateCommandVariable = new StateCommandVariable();

        #endregion

        #region Methods

        public List<CommJob> GetListWholeJobCopy()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            return listJob;
        }

        public List<DIComCommJob> GetJosMatchingWithVar(DIComProtocol.DiComVar var)
        {
            List<DIComCommJob> listJobs = new List<DIComCommJob>();
            lock (lockListObject)
            {
                var l = ListWholeJob.FindAll(j => ((DIComCommJob)j).VarName.ToLower() == var.VarName.ToLower());
                if (l.Count != 0)
                    listJobs.AddRange(l.ConvertAll(j => (DIComCommJob)j));                
            }

            int jobIndex = 0;
            while (jobIndex < listJobs.Count)
            {
                if (listJobs[jobIndex].IsJobDataTypeMatchingVarType(var))
                {
                    jobIndex++;
                }
                else
                {
                    listJobs[jobIndex].SetQuality(Opc.Ua.StatusCodes.BadConfigurationError);
                    listJobs.RemoveAt(jobIndex);
                }
            }

            return listJobs;
        }

        
        public void SetVarConfig(DIComProtocol.DiComVar var)
        {
            // Dictionary<record id, Dictionary<var index, DIComCommJob>> recordConfigs;
            bool recordIDExist = recordConfigs.ContainsKey(var.RecordID);
            bool varIndexExist = recordIDExist && recordConfigs[var.RecordID].ContainsKey(var.VarIndex);

            if (!recordIDExist || !varIndexExist)
            {
                // get job with same VarName
                List<DIComCommJob> jobs = GetJosMatchingWithVar(var);
                if (jobs != null && jobs.Count>0)
                {
                    foreach (var j in jobs)
                    {
                        j.VarIndex = var.VarIndex;
                        if (var.IsVarTypeString())
                        {
                            j.TotalJobSize = var.VarSize;
                            j.TagsList[0].Size = var.VarSize;
                        }
                    }
                    if (!recordIDExist)                    
                        recordConfigs[var.RecordID] = new Dictionary<int, List<DIComCommJob>>();

                    if (!varIndexExist)
                        recordConfigs[var.RecordID][var.VarIndex] = new List<DIComCommJob>();

                    foreach (var job in jobs)
                    {
                        // current job does not exit into the list
                        if (recordConfigs[var.RecordID][var.VarIndex].Find(j=> j == job) == null)
                            recordConfigs[var.RecordID][var.VarIndex].Add(job);
                    }
                }
            }
        }

        public void ResetVarConfigs()
        {
            recordConfigs.Clear();
        }

        public List<DIComCommJob> GetJobsMatchRecordIDAndVarIndex(DIComProtocol.DiComVar var)
        {
            if (recordConfigs.ContainsKey(var.RecordID) && recordConfigs[var.RecordID].ContainsKey(var.VarIndex))
                return recordConfigs[var.RecordID][var.VarIndex];
            else
                return null;
        }
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as DIComCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DIComCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as DIComTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DIComCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new DIComTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DIComCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DIComCommJobSettings(session, commJob);
        }

        //public override uint OnWriteTag(NodeId tagnodeid, ref object value)
        //{
        //    uint ret = base.OnWriteTag(tagnodeid,ref value);
        //    if (ret == StatusCodes.Good)
        //    {
        //        //System.Diagnostics.Trace.TraceInformation("Station.OnWriteTag tag:{0} value:{1}", tagnodeid.ToString(), value.ToString());
        //        lock (lockListObject)
        //        {
        //            if (!mapTagJob.ContainsKey(tagnodeid))
        //                return StatusCodes.BadNodeIdInvalid;

        //            DIComCommJob job = mapTagJob[tagnodeid] as DIComCommJob;
        //            byte[] writeData;
        //            if (job.PrepareData(out writeData))
        //            {
        //                lock (job.retLockList())
        //                {
        //                    Channel.ExecuteJob(job);
        //                }
        //                List<DIComCommJob> changedJob;
        //                setMemoryData(job, ref writeData, out changedJob);
        //                changedJob.Remove(job);
        //                (Channel as DICommTcpServer).ExecuteReadJob(ref changedJob);
        //                job.LastExecutionTime = DateTime.UtcNow;
        //                ExecutedJobArgs eJob = new ExecutedJobArgs();
        //                eJob.Values = new byte[0];
        //                eJob.Job = job;
        //                (Channel as DICommTcpServer).PubliocOnJobExecuted(eJob);
        //            }
        //        }
        //    }

        //    return ret;
        //}
        #endregion

        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    DIComDynTagSettings dts = new DIComDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}VN{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.VarName);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}VN{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.VarName);
                    return cx.CompareTo(cy);                    
                }
            }
        }

        #region override Methods

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            DIComCommJob mJ = e.Job as DIComCommJob;
            if (mJ == null)
                return;
            lock (lockProcessJob)
            {

                // analyzing answer if no error exists before
                if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    //ModbusProtocol P = new ModbusProtocol();
                    byte[] Answer = (byte[])e.Values;
                    if (Answer != null && Answer.Length != 0)
                    {
                        List<object> ChangedTags = new List<object>();
                        if (/*P*/DIComProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        {
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);
                            }
                        }
                        else
                        { 
                            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                        }
                    }
                }

                //put all the job in error?
                e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

                base.ProcessJobValues(e);
            }
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            DIComCommJob mj = job as DIComCommJob;

            DIComProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #region ClientStateCommandVariable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the Client State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetClientStateCommandVariableBit(bool bitValue, UInt16 bitIndex)
        {
            return clientStateCommandVariable.SetStateCommandVariableBit(bitValue, bitIndex, GetCommDriver());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the Client State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetClientStateCommandVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            return clientStateCommandVariable.GetStateCommandVariableBit(ref bitValue, bitIndex);
        }
        #endregion

        #endregion

        #region Properties
        private string _DeviceHostName;
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set {_DeviceHostName = value; }
        }

        string _IpAddress = string.Empty;
        public string IpAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_IpAddress) && !string.IsNullOrEmpty(_DeviceHostName))
                {
                    if (UdpChannel.GetResolvedConnecionIPAddress(_DeviceHostName, out IPAddress resolvedIPAddress))
                        _IpAddress = resolvedIPAddress.ToString();
                }
                return _IpAddress;
            }
        }


        /// <summary>  The Client-State-Command variable of the station. </summary>
        private UFUAModel.TagEntityReference _ClientStateCommandTag;
        /// <summary>
        /// Gets or sets the the Client-State-Command variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference ClientStateCommandTag
        {
            get
            {
                return _ClientStateCommandTag;
            }
            set
            {
                _ClientStateCommandTag = value;
            }
        }
        #endregion
    }
}
