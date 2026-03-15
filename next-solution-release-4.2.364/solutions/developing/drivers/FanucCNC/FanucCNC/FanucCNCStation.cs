using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;

namespace FanucCNC
{    
    public class FanucCNCStation : Station
    {
        #region Data Members
        protected Object _lockActivateDeactivate = new Object();
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public FanucCNCStation(CommunicationDriver commdriver, FanucCNCStationSettings settings)
            : base(commdriver, settings)
        {
            //_CNCPath = settings.CNCPath;            
        }
        
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as FanucCNCCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new FanucCNCCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as FanucCNCTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new FanucCNCCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new FanucCNCTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as FanucCNCCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new FanucCNCCommJobSettings(session, commJob);
        }

        #endregion

        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            FanucCNCCommJob mJ = e.Job as FanucCNCCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (FanucCNCProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConnectionBroken);

            base.ProcessJobValues(e);
        }


        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            FanucCNCCommJob mj = job as FanucCNCCommJob;

            FanucCNCProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    FanucCNCDynTagSettings dts = new FanucCNCDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}FC{3}ADR{4}ADRBIT{5}",
                                                dts.DeviceSize.ToString("000"),
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"),
                                                dts.FunctionCode,
                                                (dts.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng != null ? ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)dts.FunctionSettings).AddressNumber.ToString("000000") : string.Empty),
                                                (dts.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng != null ? (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)dts.FunctionSettings).AddressBitNumber+1).ToString("0") : string.Empty));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}FC{3}ADR{4}ADRBIT{5}",
                                                dts.DeviceSize.ToString("000"),
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"),
                                                dts.FunctionCode,
                                                (dts.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng != null ? ((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)dts.FunctionSettings).AddressNumber.ToString("000000") : string.Empty),
                                                (dts.FunctionSettings as FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng != null ? (((FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng)dts.FunctionSettings).AddressBitNumber+1).ToString("0") : string.Empty));
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


        //public void void UpdateObservedTag(NodeId node, DataValue value)
        //{
        //    // Update the value of the state/command variable
        //    if ((stateCommandVariableHasBeenSet == true) && (node == stationStateCommandVariable.varNodeId))
        //    {
        //        ManageUpdatedValueForTheStateCommandVariable(value);
        //    }

        //    // Update the value of the conditional variable of a job.
        //    else
        //    {
        //        // Get the station that manages the node ID and pass to it the updated value of the variable
        //        if (ObservedTagToJobsMap.Keys.Contains(node) == true)
        //        {
        //            List<CommJob> jobList = ObservedTagToJobsMap[node];
        //            foreach (var job in jobList)
        //                ((FanucCNCCommJob)job).UpdateObservedTag(node, value);
        //        }
        //    }
        //}

        //#region Properties        
        ///// <summary>
        ///// Specificy the CNC path
        ///// </summary>
        //private short _CNCPath;
        //public short CNCPath
        //{
        //    get { return _CNCPath; }
        //}
        //#endregion
    }
}
