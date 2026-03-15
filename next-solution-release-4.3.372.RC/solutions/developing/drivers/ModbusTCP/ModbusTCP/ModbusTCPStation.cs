using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using Opc.Ua;

namespace ModbusTCP
{
    class ModbusTCPStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public ModbusTCPStation(CommunicationDriver commdriver, ModbusTCPStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            _AddressType = (AddressTypes)settings.AddressType;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as ModbusTCPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new ModbusTCPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as ModbusTCPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new ModbusTCPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new ModbusTCPTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as ModbusTCPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new ModbusTCPCommJobSettings(session, commJob);
        }

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
                    ModbusTCPDynTagSettings dts = new ModbusTCPDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}FN{5}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"), dts.FileNumber.ToString("000"),
                        dts.StringLength.ToString("00000")
                        );
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}FN{5}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"), dts.FileNumber.ToString("000"),
                        dts.StringLength.ToString("00000")
                        );
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            ModbusTCPCommJob mJ = e.Job as ModbusTCPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                //ModbusProtocol P = new ModbusProtocol();
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (/*P*/ModbusProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                //e.ChangedTags.AddRange(ChangedTags);
                else
                {
                    if (ChangedTags.Count > 0)
                        e.ErrorCode = (DriverErrorCodes)((ModbusErrorCodes)ChangedTags[0]);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            //base.ProcessJobValues(e);
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
                //System.Diagnostics.Debug.WriteLine("_DEBUG ProcessJobValues - {0} - {1} - true - {2} - {3}", DateTime.Now.ToString("HH:mm:ss.fff"), stationStateCommandVariable.varNodeId, e.ErrorCode, bAllJobsInError);
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("_DEBUG ProcessJobValues - {0} - {1} - false", DateTime.Now.ToString("HH:mm:ss.fff"), stationStateCommandVariable.varNodeId);
                if (Channel.InErrorJobs(this) == 0)
                {
                    SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                }
                if (((mJ.Executioncode != (byte)ExecutionCodes.ForceSingleCoil) && 
                (mJ.Executioncode != (byte)ExecutionCodes.PresetSingleRegister) && 
                (mJ.Executioncode != (byte)ExecutionCodes.MaskWrite4xRegister)) || (mJ.TagsListToWrite.Count == 0))
                {
                    e.Job.ResetConditionalVariable();
                }
            }

            if (e.ChangedTags.Count > 0)
            {
                uint quality;
                string error;
                GetCommDriver().GetDriverErrorInfo((int)e.ErrorCode, out quality, out error);
                foreach (var tag in e.ChangedTags)
                {
                    tag.Value.StatusCode = quality;
                    GetCommDriver().OnTagChanged(tag.TagNode.NodeId, tag.Value, e.Timestamp);
                }
            }
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            ModbusTCPCommJob mj = job as ModbusTCPCommJob;

            ModbusProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #region Properties

        private uint _StationID;
        public uint StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private AddressTypes _AddressType;
        public AddressTypes AddressType
        {
            get { return _AddressType; }
            set
            {
                _AddressType = value;
            }
        }

        #endregion

    }
}
