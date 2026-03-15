using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace MelsecQEth
{
    public class MelsecQEthStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public MelsecQEthStation(CommunicationDriver commdriver, MelsecQEthStationSettings settings)
            : base(commdriver, settings)
        {
            NetworkNumber = settings.NetworkNumber;
            PcNumber = settings.PcNumber;
            MaxRetriesBeforeError = settings.MaxRetriesBeforeError;
            PlcType = (MelsecQEthProtocol.PlcTypes)settings.PlcType;
            LabelMaxAggregatedRequests = (uint)settings.LabelNrMaxAggregatedRequests;
            ResetJobsRuntimeAggregationLimits();
        }

        #endregion

        #region data members

        private object _LabelNrMaxJobsRuntimeAggregationLimitsLock = new object();
        private uint _LabelNrMaxJobsReadRuntimeAggregationLimits = uint.MaxValue;
        private uint _LabelNrMaxJobsWriteRuntimeAggregationLimits = uint.MaxValue;
        private uint _LabelNrMaxJobs1stTimeRuntimeAggregationLimits = uint.MaxValue;
        #endregion

        #region Override Methods

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new MelsecQEthTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as MelsecQEthCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new MelsecQEthCommJobSettings(session, commJob);
        }

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as MelsecQEthCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new MelsecQEthCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as MelsecQEthTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new MelsecQEthCommJob(this, conf);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            MelsecQEthCommJob mJ = e.Job as MelsecQEthCommJob;
            if (mJ == null)
            {
                return;
            }

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    //MelsecQProtocol P = new MelsecQProtocol();
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (mJ.ParseData(Answer, ref ChangedTags))
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
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            MelsecQEthCommJob mj = job as MelsecQEthCommJob;

            mj.ParseData(receivedbuffer, ref arguments);

            return true;
        }

        public void ResetInternalJobsState()
        {
            ResetJobsRuntimeAggregationLimits();
            List<MelsecQEthCommJob> listJobs;
            lock (lockListObject)
            {
                listJobs = ListWholeJob.ConvertAll(x => (MelsecQEthCommJob)x).ToList();
            }

            if (listJobs.Count > 0)
            {
                Parallel.ForEach(listJobs, job =>
                {
                    job.AlreadyWritten = false;
                    job.AlreadyExchanged = false;
                    job.BadNotFoundStateUncertain = false;  
                    if (job.StringSettingsAutoDetect != MelsecQEthProtocol.StringSettingsAutoDetectStates.NotRequested)
                        job.StringSettingsAutoDetect = MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested;
                });
            }
        }

        /// <summary>
        /// Reset the nr of jobs (int.MaxValue) that can be insert into the frame 
        /// </summary>
        public void ResetJobsRuntimeAggregationLimits()
        {
            lock (_LabelNrMaxJobsRuntimeAggregationLimitsLock)
            {
                _LabelNrMaxJobsWriteRuntimeAggregationLimits = _LabelMaxAggregatedRequests;
                _LabelNrMaxJobsReadRuntimeAggregationLimits = _LabelMaxAggregatedRequests;
                _LabelNrMaxJobs1stTimeRuntimeAggregationLimits = _LabelMaxAggregatedRequests;
                if (_LabelNrMaxJobs1stTimeRuntimeAggregationLimits > Properties.Settings.Default.LabelNrMaxJobs1stTimeRuntimeAggregationLimits)
                    _LabelNrMaxJobs1stTimeRuntimeAggregationLimits = Properties.Settings.Default.LabelNrMaxJobs1stTimeRuntimeAggregationLimits;
            }
        }

        /// <summary>
        /// Set the nr of jobs that can be insert into the frame 
        /// </summary>
        /// <param name="write"></param>
        /// <returns></returns>
        public void SetNrMaxJobsRuntimeAggregationLimits(MelsecQEthProtocol.RuntimeAggregationLimits mode, int value)
        {
            lock (_LabelNrMaxJobsRuntimeAggregationLimitsLock)
            {
                switch (mode)
                {
                    case MelsecQEthProtocol.RuntimeAggregationLimits.Write:
                        {
                            uint newLimit = (uint)(value - (value / 10));
                                if (newLimit == 0)
                                    newLimit = 1;
                                if (_LabelNrMaxJobsWriteRuntimeAggregationLimits > newLimit)
                                {
                                    _LabelNrMaxJobsWriteRuntimeAggregationLimits = newLimit;
                                    CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.WarningLabelTooManyRequestToPlcWrite, newLimit, this.Name), EventSeverity.Min);
                                }
                                break;
                        }
                    case MelsecQEthProtocol.RuntimeAggregationLimits.Read:
                        {
                            uint newLimit = (uint)(value - (value / 10));
                            if (newLimit == 0)
                                newLimit = 1;
                            if (_LabelNrMaxJobsReadRuntimeAggregationLimits > newLimit)
                            {
                                _LabelNrMaxJobsReadRuntimeAggregationLimits = newLimit;
                                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.WarningLabelTooManyRequestToPlcRead, newLimit, this.Name), EventSeverity.Min);
                            }
                            break;
                        }
                    case MelsecQEthProtocol.RuntimeAggregationLimits.NotExchanged:
                        _LabelNrMaxJobs1stTimeRuntimeAggregationLimits = (uint)value;
                        break;
                }
            }
        }

        /// <summary>
        /// Get the nr of jobs that can be insert into the frame 
        /// </summary>
        /// <param name="write"></param>
        /// <returns></returns>
        public uint GetNrMaxJobsRuntimeAggregationLimits(MelsecQEthProtocol.RuntimeAggregationLimits mode)
        {
            lock (_LabelNrMaxJobsRuntimeAggregationLimitsLock)
            {
                switch (mode)
                {
                    case MelsecQEthProtocol.RuntimeAggregationLimits.Write:
                        return _LabelNrMaxJobsWriteRuntimeAggregationLimits;
                    case MelsecQEthProtocol.RuntimeAggregationLimits.Read:
                        return _LabelNrMaxJobsReadRuntimeAggregationLimits;
                    case MelsecQEthProtocol.RuntimeAggregationLimits.NotExchanged:
                        return _LabelNrMaxJobs1stTimeRuntimeAggregationLimits;
                    default:
                        return uint.MaxValue;
                }
            }
        }

        #endregion

        #region static methods
        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0; //==
                }
                else
                {
                    return -1;// x < y
                }
            }
            else
            {
                //x!= null
                if (y == null)
                {
                    return 1; //x > y
                }
                else
                {
                    MelsecQEthDynTagSettings dts = new MelsecQEthDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string address;
                    if (dts.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                    {
                        MelsecQAddress adr = new MelsecQAddress(MelsecQEthProtocol.AddressTypes.DataArea, dts.Address);
                        address = String.Format("{0}{1}", adr.DataAreaString, adr.StartAddress.ToString("000000"));                        
                    }
                    else
                    {
                        address = dts.Address;
                    }
                    string cx = string.Format("DS{0}TL{1}MI{2}Addr{3}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), address,
                        dts.StringLength.ToString("00000"));
                        
                    dts.TryParse(y.TagNode.DynamicSettings);
                    if (dts.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                    {
                        MelsecQAddress adr = new MelsecQAddress(MelsecQEthProtocol.AddressTypes.DataArea, dts.Address);
                        address = String.Format("{0}{1}", adr.DataAreaString, adr.StartAddress.ToString("000000"));
                    }
                    else
                    {
                        address = dts.Address;
                    }
                    string cy = string.Format("DS{0}TL{1}MI{2}Addr{3}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), address,
                        dts.StringLength.ToString("00000"));
                    return cx.CompareTo(cy);
                }
            }
        }
        #endregion
 
        #region Properties

        private uint _NetworkNumber;
        public uint NetworkNumber
        {
            get { return _NetworkNumber; }
            set
            {
                _NetworkNumber = value;
            }
        }
        private uint _PcNumber;
        public uint PcNumber
        {
            get { return _PcNumber; }
            set
            {
                _PcNumber = value;
            }
        }

        /// <summary>
        /// Enter the PLC type
        /// </summary>
        private MelsecQEthProtocol.PlcTypes _PlcType;
        public MelsecQEthProtocol.PlcTypes PlcType
        {
            get { return _PlcType; }
            set
            {
                _PlcType = value;
            }
        }

        /// <summary>
        /// The maximum number of simultaneous requests for tags with 'Label' addressing 
        /// </summary>
        private uint _LabelMaxAggregatedRequests;
        public uint LabelMaxAggregatedRequests
        {
            get { return _LabelMaxAggregatedRequests; }
            set
            {
                _LabelMaxAggregatedRequests = value;
            }
        }
        #endregion
    }
}
