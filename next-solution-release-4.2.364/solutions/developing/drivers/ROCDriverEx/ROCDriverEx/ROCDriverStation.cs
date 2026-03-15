using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;

namespace ROCDriver
{
    public class ROCDriverStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public ROCDriverStation(CommunicationDriver commdriver, ROCDriverStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            _StationGroup = settings.StationGroup;
            _CheckDeviceConnection = settings.CheckDeviceConnection;
            _PingPointType = settings.PingPointType;
            _PingLogicalNumber = settings.PingLogicalNumber;
            _PingParameter = settings.PingParameter;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as ROCDriverCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new ROCDriverCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as ROCDriverTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new ROCDriverCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new ROCDriverTag(td);
        }
        
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as ROCDriverCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new ROCDriverCommJobSettings(session, commJob);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        #endregion
        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            ROCDriverCommJob mJ = e.Job as ROCDriverCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer.Length != 0)
                {
                    e.ErrorCode = (DriverErrorCodes)ROCDriverProtocol.ParseData(Answer, ref mJ, ref ChangedTags);
                    if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    }
                }
            }
            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);
            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            ROCDriverCommJob mj = job as ROCDriverCommJob;

            if (mj == null) return false;

            // If testing the connection do not try to parse data (the data buffer is an empty array)
            ROCDriverChannel ch = (ROCDriverChannel)Channel;
            if (ch == null) return false;
            if(ch.TestConnection)
            {
                return (true);
            }

            if((DriverErrorCodes)ROCDriverProtocol.ParseData(receivedbuffer, ref mj, ref arguments) != DriverErrorCodes.ErrorNoError)
            {
                return false;
            }
            
            return true;
        }

        #endregion

        #region Methods

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
                    return 1; //x > y
                }
            }
        }

        DateTime lastCommunicationTime = DateTime.MinValue;

        public bool ConnectionMustBeChecked(uint checkFrequency)
        {
            if(lastCommunicationTime == DateTime.MinValue)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - ConnectionMustBeChecked - Station: {1} - return false 1",
                                                       currentTime, Name));
                }
#endif
                return false;
            }

            if(IsSuspended())
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - ConnectionMustBeChecked - Station: {1} - return false 2",
                                                       currentTime, Name));
                }
#endif
                return false;
            }

            if((DateTime.UtcNow - lastCommunicationTime).TotalSeconds < checkFrequency)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - ConnectionMustBeChecked - Station: {1} - return false 3 - TotalSeconds: {2}",
                                                       currentTime, Name, (DateTime.UtcNow - lastCommunicationTime).TotalSeconds));
                }
#endif
                return false;
            }

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - ConnectionMustBeChecked - Station: {1} - return true",
                                                   currentTime, Name));
            }
#endif

            return true;
        }

        private bool IsSuspended()
        {
            bool suspendBitNewValue = false;
            if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if (suspendBitNewValue)
                    return true;
            }

            return false;
        }

        public void SetLastCommunicationTime(DateTime lastCommTime)
        {
            lastCommunicationTime = lastCommTime;
        }
 
        public void SetConnectionError()
        {
            if (LastErrorCode == DriverErrorCodes.ErrorNoError)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                listJob = (from j in listJob.AsParallel()
                           where j.InUse && !j.InErrorState
                           select j).ToList();

                foreach (var commjob in listJob)
                {
                    if (((commjob.Type != LinkType.ExceptionOutput) && !commjob.ConditionalVariableSet))
                        commjob.SetErrorState((int)ROCDriverErrorCodes.ErrorInConnectionCheck, false);
                }
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                LastErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorInConnectionCheck;
                InErrorState = true;
            }
            if (StatisticsData != null)
            {
                StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                uint quality;
                String DiagnErrorMessage = String.Empty;
                CommDriver.GetDriverErrorInfo((int)ROCDriverErrorCodes.ErrorInConnectionCheck, out quality, out DiagnErrorMessage);
                StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
            }
        }

        #endregion

        #region Member


        #endregion

        #region Properties

        private byte _StationID;
        public byte StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private byte _StationGroup;
        public byte StationGroup
        {
            get { return _StationGroup; }
            set
            {
                _StationGroup = value;
            }
        }

        private bool _CheckDeviceConnection;
        public bool CheckDeviceConnection
        {
            get
            {
                return _CheckDeviceConnection;
            }
            set
            {
                _CheckDeviceConnection = value;
            }
        }

        /// <summary>
        /// Ping Point Type (optional)
        /// </summary>
        private byte _PingPointType;
        public byte PingPointType
        {
            get
            {
                return _PingPointType;
            }
            set
            {
                _PingPointType = value;
            }
        }

        /// <summary>
        /// Ping Logical Number (optional)
        /// </summary>
        private byte _PingLogicalNumber;
        public byte PingLogicalNumber
        {
            get
            {
                return _PingLogicalNumber;
            }
            set
            {
                _PingLogicalNumber = value;
            }
        }

        /// <summary>
        /// Ping Parameter (optional)
        /// </summary>
        private byte _PingParameter;
        public byte PingParameter
        {
            get
            {
                return _PingParameter;
            }
            set
            {
                _PingParameter = value;
            }
        }

        #endregion

    }
}
