using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace CoDeSys
{    
    public class CoDeSysStation : Station
    {
        #region Data Members
        protected Object _lockActivateDeactivate = new Object();
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public CoDeSysStation(CommunicationDriver commdriver, CoDeSysStationSettings settings)
            : base(commdriver, settings)
        {
            //LastTransOK = true;
            //BuildInfoMaps = false;
            //PLCAddress = settings.PLCAddress;
            //PlcVersion = settings.PlcVersion;
            //Port = settings.PlcPort;
            //UpdateRate = settings.UpdateRate;
            //Motorola = settings.Motorola;
            //BufferSize = settings.BufferSize;
            //LogIn = settings.LogIn;
            //Protocol = settings.Protocol;
            //UseDirectReading = settings.UseDirectReading;
            //MaxVarPerCycle = settings.MaxVarPerCycle;
            //UsePing = false;            
        }
        
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as CoDeSysCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new CoDeSysCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as CoDeSysTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new CoDeSysCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new CoDeSysTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as CoDeSysCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new CoDeSysCommJobSettings(session, commJob);
        }

        #endregion

        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            CoDeSysCommJob mJ = e.Job as CoDeSysCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (CoDeSysProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == (DriverErrorCodes)CoDeSysChannel.CoDeSysErrorCodes.ErrorConnectionBroken);

            if (e.ErrorCode != DriverErrorCodes.ErrorNoError && e.GeneralError)
            {
                //all jobs in error
                List<CommJob> erlist = new List<CommJob>();
                lock (lockListObject)
                {
                    erlist.AddRange(ListWholeJob);
                }

                System.Threading.Tasks.Parallel.ForEach(erlist, j =>
               {
                   if (((j.Type != LinkType.ExceptionOutput) && !j.ConditionalVariableSet))
                       lock (j.retLockList())
                           j.IsPending = false;
               });
            }
            base.ProcessJobValues(e);
        }


        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            CoDeSysCommJob mj = job as CoDeSysCommJob;

            CoDeSysProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

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
                    CoDeSysDynTagSettings dts = new CoDeSysDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}VT{3}",
                                                dts.DeviceSize.ToString("000"),
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"),
                                                dts.CoDeSysVarType);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}VT{3}",
                                                dts.DeviceSize.ToString("000"),
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"),
                                                dts.CoDeSysVarType); 
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

        public void SetGeneralError(DriverErrorCodes errorCode)
        {
            ExecutedJobArgs e = new ExecutedJobArgs();

            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }            
            e.Job = listJob[0];
            e.ErrorCode = errorCode;
            // ErrorTimeOut and ErrorConnectionBroken set e.GeneralError  = true
            //base.ProcessJobValues(e);  
            System.Threading.Tasks.Parallel.ForEach(listJob, j =>
            {
                lock (j.retLockList())
                    j.IsPending = false;
            });
            OnJobExecuted(e.Job, e);
            LastErrorCode = errorCode;
        }
        #endregion

        /// <summary>
        /// Called on station start up and on resume from Command stata variable
        /// </summary>
        /// <returns></returns>
        public override bool Startup()
        {
            bool Result = false;

            lock (_lockActivateDeactivate)
            {
                Result = base.Startup();
            }

            return Result;
        }

        /// <summary>
        /// Called when command stata variable was set to 2 --> station disable
        /// </summary>
        /// <returns></returns>
        public override bool SuspendStation()
        {
            bool Result = false;

            lock (_lockActivateDeactivate)
            {
                Result = base.SuspendStation();
            }

            return Result;
        }

        #region Properties

        ///// <summary>
        ///// PLC Address
        ///// </summary>
        //private string _PLCAddress;
        //public string PLCAddress
        //{
        //    get { return _PLCAddress; }
        //    set { _PLCAddress = value; }
        //}

        ///// <summary>
        ///// Enter the CoDeSys PLC Version
        ///// </summary>
        //private CoDeSysProtocol.PlcVersion _PlcVersion;
        //public CoDeSysProtocol.PlcVersion PlcVersion
        //{
        //    get { return _PlcVersion; }
        //    set {_PlcVersion = value; }
        //}

        ///// <summary>
        ///// Port Number
        ///// </summary>
        //private uint _Port;
        //public uint Port
        //{
        //    get { return _Port; }
        //    set { _Port = value; }
        //}

        ///// <summary>
        ///// Cycling update rate
        ///// </summary>
        //private uint _UpdateRate;
        //public uint UpdateRate
        //{
        //    get { return _UpdateRate; }
        //    set { _UpdateRate = value; }
        //}

        ///// <summary>
        ///// Motorola byte order = default value = False
        ///// </summary>
        //private bool _Motorola;
        //public bool Motorola
        //{
        //    get { return _Motorola; }
        //    set { _Motorola = value; }
        //}

        ///// <summary>
        ///// Communication BufferSize
        ///// </summary>
        //private uint _BufferSize;
        //public uint BufferSize
        //{
        //    get { return _BufferSize; }
        //    set { _BufferSize = value; }
        //}

        ///// <summary>
        ///// LogIn to PLC
        ///// </summary>
        //private bool _LogIn;
        //public bool LogIn
        //{
        //    get { return _LogIn; }
        //    set { _LogIn = value; }
        //}

        ///// <summary>
        ///// Protocol Type
        ///// </summary>
        //private CoDeSysProtocol.Protocol _Protocol;
        //public CoDeSysProtocol.Protocol Protocol
        //{
        //    get { return _Protocol; }
        //    set { Protocol = value; }
        //}

        ///// <summary>
        ///// Enable to retrive data from PLC plling device
        ///// </summary>
        //private bool _UseDirectReading;
        //public bool UseDirectReading
        //{
        //    get { return _UseDirectReading; }
        //    set { _UseDirectReading = value; }
        //}

        ///// <summary>
        ///// Task aggregation threshold limit
        ///// </summary>
        //private uint _MaxVarPerCycle;
        //public uint MaxVarPerCycle
        //{
        //    get { return _MaxVarPerCycle; }
        //    set { _MaxVarPerCycle = value; }
        //}

        ///// <summary>
        ///// Use ping to test presence of PLC 
        ///// </summary>
        //private bool _UsePing;
        //public bool UsePing
        //{
        //    get { return _UsePing; }
        //    set { _UsePing = value; }
        //}

        #endregion

    }
}
