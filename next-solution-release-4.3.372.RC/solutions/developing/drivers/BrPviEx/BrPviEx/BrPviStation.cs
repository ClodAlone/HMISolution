using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;

namespace BrPvi
{
    public class BrPviStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public BrPviStation(CommunicationDriver commdriver, BrPviStationSettings settings)
            : base(commdriver, settings)
        {
            _BrPviRoutingPath = settings.BrPviRoutingPath;
            _BrPviResponseTimeout = settings.BrPviResponseTimeout;
            _BrPviInterfaceType = settings.BrPviInterfaceType;
            _BrPviSourceStationPort = settings.BrPviSourceStationPort;
            _BrPviSourceStationID = settings.BrPviSourceStationID;
            _BrPviDestinationStationIPAddress = settings.BrPviDestinationStationIPAddress;
            _BrPviDestinationStationPort = settings.BrPviDestinationStationPort;
            _BrPviDestinationStationID = settings.BrPviDestinationStationID;
            _BrPviSerialPort = settings.BrPviSerialPort;
            _BrPviSerialBaudrate = settings.BrPviSerialBaudrate;
            _BrPviSerialParity = settings.BrPviSerialParity;
            _BrPviSerialFlowControl = settings.BrPviSerialFlowControl;
            _BrPviRemoveDisabledItemAfterSecs = settings.BrPviRemoveDisabledItemAfterSecs;
            BuildPviObjectNames();
        }

        #endregion

        #region Data members
        public bool pviObjectsInitialized = false;
        protected Object _lockActivateDeactivate = new Object();
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as BrPviCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new BrPviCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as BrPviTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new BrPviCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new BrPviTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as BrPviCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new BrPviCommJobSettings(session, commJob);
        }

        //public override bool Startup()
        //{
        //    bool Result = false;

        //    lock (_lockActivateDeactivate)
        //    {
        //        bool Reactivated = (((BrPviChannel)Channel).AnyPVIObjectOfStationDeactivated(this));

        //        //if (Channel != null && !Reactivated)
        //        //{
        //        //    // Add jobs to the channel dictionary that links jobs with PVI objects
        //        //    var listJob = new List<CommJob>();
        //        //    lock (lockListObject)
        //        //    {
        //        //        listJob.AddRange(ListWholeJob);
        //        //    }

        //        //    BrPviChannel brChannel = (BrPviChannel)Channel;
        //        //    foreach (var job in listJob)
        //        //    {
        //        //        BrPviCommJob brJob = (BrPviCommJob)job;
        //        //        BuildJobPviObjNames(brJob);
        //        //        if (!String.IsNullOrWhiteSpace(brJob.BrPviTaskCompleteName))
        //        //        {
        //        //            brChannel.AddJobToPviObjDictionary(brJob.BrPviTaskCompleteName, brJob);
        //        //        }
        //        //        if (!String.IsNullOrWhiteSpace(brJob.BrPviVariableCompleteName))
        //        //        {
        //        //            brChannel.AddJobToPviObjDictionary(brJob.BrPviVariableCompleteName, brJob);
        //        //        }
        //        //    }
        //        //}

        //        // Call the base class method
        //        Result = base.Startup();
        //        //if (Result && Reactivated)
        //        //    if (pviObjectsInitialized)  // reactive station's items only when station is reactivated, not at startup
        //        //        ((BrPviChannel)Channel).ActivateDeactivateAllPVIObjectOfStation(this, BrPviCommJob.PviObjectState.Activated);
        //        //    else // this situation occour when pvimotior was stopped when item is deactive; pvimonitor don't reload so force driver to do it
        //        //        ((BrPviChannel)Channel).ReloadAllActivePVIObjectOfStation(this);
        //    }

        //    return Result;
        //}

        ///// <summary>
        ///// Remove all tag's PVI elements associated to the station e reset job flag; when station will be reactivated, 
        ///// driver automatically regenerate all data
        ///// </summary>
        ///// <returns></returns>
        //public override bool SuspendStation()
        //{
        //    bool Result = false;

        //    lock (_lockActivateDeactivate)
        //    {
        //        ((BrPviChannel)Channel).ActivateDeactivateAllPVIObjectOfStation(this, BrPviCommJob.PviObjectState.DeActivated);

        //        Result = base.SuspendStation();
        //    }

        //    return Result;
        //}

        //public List<CommJob> GetListWholeJobCopy()
        //{
        //    var listJob = new List<CommJob>();
        //    lock (lockListObject)
        //    {
        //        listJob.AddRange(ListWholeJob);
        //    }
        //    return listJob;
        //}

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            BrPviCommJob brJ = e.Job as BrPviCommJob;
            if (brJ == null)
            {
                return;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviStation.ProcessJobValues - {0} called for PVI Tag: {1}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJ.BrPviVariableCompleteName));
#endif
            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<Tag> ChangedTags = new List<Tag>();
                brJ.SetJobData(Answer, ref ChangedTags);
                foreach (var tag in ChangedTags)
                {
                    var t = tag as Tag;
                    if (t != null)
                        e.ChangedTags.Add(t);
                }
#if DEBUG
                String DbgText = String.Format("BR_DEBUG - BrPviStation.ProcessJobValues - {0} Num. of changed tags: {1} - PVI Tag: {2} - Number of Data Bytes: {3}",
                              DateTime.Now.ToString("HH:mm:ss.fff"), e.ChangedTags.Count, brJ.BrPviVariableCompleteName, Answer.Count());
                if (e.ChangedTags.Count > 0)
                    DbgText += String.Format(" - Tag: {0}", e.ChangedTags[0].TagNode.NodeId);
                System.Diagnostics.Debug.WriteLine(DbgText);
#endif
            }

            //else if (e.ErrorCode == DriverErrorCodes.ErrorTimeOut)
            //{
            //    //lock (lockBool)
            //    //{
            //        var listJob = new List<CommJob>();
            //        lock (lockListObject)
            //        {
            //            foreach (var commjob in ListWholeJob)
            //                if (commjob.InUse)
            //                    listJob.Add(commjob);
            //        }

            //        foreach (CommJob commjob in listJob)
            //        {
            //            commjob.SetErrorState((int)e.ErrorCode);
            //            BrPviCommJob j = commjob as BrPviCommJob;
            //            ChannelBase.ChangeStateJob(commjob, CommJobState.PollingInError);
            //        }
            //    //}
            //}

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeConnectionBroken);
            if (e.ErrorCode == (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeConnectionBroken)
                SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
            
            base.ProcessJobValues(e);
        }

        //public void BaseProcessJobValues(ExecutedJobArgs e)
        //{
        //    base.ProcessJobValues(e);
        //}
        #endregion

        #region Specific Methods

        void BuildPviObjectNames()
        {
            // PVI Line
            _BrPviLineName = "@Pvi/LNINA2";
            _BrPviLineDescr = "CD=LnIna2";

            // PVI Device
            switch (_BrPviInterfaceType)
            {
                case BrPviProcol.InterfaceTypes.Ethernet:
                    _BrPviDeviceName = String.Format("{0}/DEV{1}", _BrPviLineName, _BrPviSourceStationID);
                    _BrPviDeviceDescr = String.Format("CD=\"/IF=tcpip /LOPO={0} /SA={1}\"",
                                                      _BrPviSourceStationPort, _BrPviSourceStationID);
                    break;

                default: // InterfaceTypes.Serial
                    _BrPviDeviceName = String.Format("{0}/COM{1}", _BrPviLineName, _BrPviSerialPort);
                    _BrPviDeviceDescr = String.Format("CD=\"/IF=com{0} /BD={1}",
                                                      _BrPviSerialPort, _BrPviSerialBaudrate);
                    switch (_BrPviSerialFlowControl)
                    {
                        case (int)BrPviProcol.FlowControlTypes.None:
                            _BrPviDeviceDescr += String.Format(" /RS=-1");
                            break;
                        case (int)BrPviProcol.FlowControlTypes.RtsOff:
                            _BrPviDeviceDescr += String.Format(" /RS=0");
                            break;
                        case (int)BrPviProcol.FlowControlTypes.RS422Mode:
                            _BrPviDeviceDescr += String.Format(" /RS=422");
                            break;
                        default: // FlowControlTypes.RS232Mode
                            _BrPviDeviceDescr += String.Format(" /RS=232");
                            break;
                    }
                    _BrPviDeviceDescr += String.Format(" /PA={0}\"", _BrPviSerialParity);
                    break;
            }

            // PVI Station
            _BrPviStationName = String.Format("{0}/Station", _BrPviDeviceName);
            _BrPviStationDescr = "CD=NA";

            // PVI CPU
            // Check if the station name contains space characters
            int spaceIndex = Name.IndexOf(" ");
            if (spaceIndex < 0)
            {
                _BrPviCpuName = String.Format("{0}/{1}", _BrPviStationName, Name);
            }
            else
            {
                _BrPviCpuName = String.Format("{0}/\"{1}\"", _BrPviStationName, Name);
            }
            _BrPviCpuDescr = "CD=\"";
            if (!String.IsNullOrWhiteSpace(_BrPviRoutingPath))
            {
                _BrPviCpuDescr += String.Format("/CN={0} ", _BrPviRoutingPath);
            }
            if (_BrPviInterfaceType == BrPviProcol.InterfaceTypes.Ethernet)
            {
                _BrPviCpuDescr += String.Format("/DA={0} ", _BrPviDestinationStationID);
                if (!String.IsNullOrWhiteSpace(_BrPviDestinationStationIPAddress))
                {
                    _BrPviCpuDescr += String.Format("/DAIP={0} ", _BrPviDestinationStationIPAddress);
                }
                _BrPviCpuDescr += String.Format("/REPO={0} ", _BrPviDestinationStationPort);
            }
            _BrPviCpuDescr += String.Format("/RT={0}\"", _BrPviResponseTimeout);
        }

        public bool CreatePviObjects()
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            // Line
            BrPviPviObject pviLineObj = brChannel.GetOrCreatePviObject(this.Name, _BrPviLineName, _BrPviLineDescr, "EV=ed", PviObjectTypes.POBJ_LINE);
            if (pviLineObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviLineObj.Name, this);
            }
            else
            {
                return false;
            }

            // Device
            BrPviPviObject pviDeviceObj = brChannel.GetOrCreatePviObject(this.Name, _BrPviDeviceName, _BrPviDeviceDescr, "EV=ed", PviObjectTypes.POBJ_DEVICE);
            if (pviDeviceObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviDeviceObj.Name, this);
            }
            else
            {
                return false;
            }

            // Station
            BrPviPviObject pviStationObj = brChannel.GetOrCreatePviObject(this.Name, _BrPviStationName, _BrPviStationDescr, "EV=ed", PviObjectTypes.POBJ_STATION);
            if (pviStationObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviStationObj.Name, this);
            }
            else
            {
                return false;
            }

            // CPU
            BrPviPviObject pviCpuObj = brChannel.GetOrCreatePviObject(this.Name, _BrPviCpuName, _BrPviCpuDescr, "EV=ed", PviObjectTypes.POBJ_CPU);
            if (pviCpuObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviCpuObj.Name, this);
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool CreateJobPviObjects(BrPviCommJob job)
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            // remove unmanaged pvi objects --> only 1 at time
            brChannel.EmptyPviObjects();

            job.CalculateBrPviVariableName(out string taskCompleteName, out string taskDescription, out string variableName, out string variableDescription);

            // Task
            //string taskCompleteName = String.Empty;
            //string taskDescription = String.Empty;
            if (!String.IsNullOrWhiteSpace(job.BrPviTaskName))
            {
                //int spaceIndex = job.BrPviTaskName.IndexOf(" ");
                //if (spaceIndex < 0)
                //{
                //    taskCompleteName = String.Format("{0}/{1}", BrPviCpuName, job.BrPviTaskName);
                //    taskDescription = String.Format("CD={0}", job.BrPviTaskName);
                //}
                //else
                //{
                //    taskCompleteName = String.Format("{0}/\"{1}\"", BrPviCpuName, job.BrPviTaskName);
                //    taskDescription = String.Format("CD=\"{0}\"", job.BrPviTaskName);
                //}

                BrPviPviObject pviTaskObj = brChannel.GetOrCreatePviObject(this.Name, taskCompleteName, taskDescription, "EV=ed", PviObjectTypes.POBJ_TASK);
                if (pviTaskObj != null)
                {
                    brChannel.AddJobToPviObjDictionary(pviTaskObj.Name, job);
                }
                else
                {
                    return false;
                }
            }

            // Variable
            //variableName = String.Empty;
            //variableDescription = String.Empty;
            //if (!String.IsNullOrWhiteSpace(taskCompleteName))
            //{
            //    variableName = String.Format("{0}/{1}", taskCompleteName, job.BrPviVariableName);
            //}
            //else
            //{
            //    variableName = String.Format("{0}/{1}", BrPviCpuName, job.BrPviVariableName);
            //}
            job.BrPviVariableCompleteName = variableName;

            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.CreateJobPviObjects {0} - Variable: {1} - Job Num. of Tags: {2}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), job.BrPviVariableCompleteName, job.TagsList.Count);

            //if ((job.Type == LinkType.Input) || (job.Type == LinkType.InputOutput))
            //{
            //    variableDescription = String.Format("CD=\"{0}\" RF={1}", job.BrPviVariableName, job.BrPviRefreshRate);
            //}
            //else
            //{
            //    variableDescription = String.Format("CD=\"{0}\" AT=w RF={1}", job.BrPviVariableName, job.BrPviRefreshRate);
            //}

            BrPviPviObject pviVarObj = brChannel.GetOrCreatePviObject(job.Station.Name, variableName, variableDescription, "EV=ed", PviObjectTypes.POBJ_PVAR);
            if (pviVarObj != null)
            {
                brChannel.AddJobToPviObjDictionary(pviVarObj.Name, job);
            }
            else
            {
                return false;
            }

            SetPviObjectEvMask(job);

            return true;
        }

        public void DeleteJobPviObjects(BrPviCommJob job)
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            brChannel.DeleteJobPviObjectes(job.Station.Name, job.BrPviVariableCompleteName);
        }
        public void DisableJobPviObjectes(BrPviCommJob job)
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            brChannel.DisableJobPviObjectes(job.Station.Name, job.BrPviVariableCompleteName);
        }

        public void SetPviObjectEvMask(BrPviCommJob job)
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            String EvMask = string.Empty;
            if (job.pviObjectState != BrPviCommJob.PviObjectState.DeActivated)
                EvMask = "ed";

            EvMask += '\0';

            brChannel.SetPviObjectEvMask(job.BrPviVariableCompleteName, EvMask);
        }

        public void BuildJobPviObjNames(BrPviCommJob brJob)
        {
            // Task
            string taskCompleteName = String.Empty;
            if (!String.IsNullOrWhiteSpace(brJob.BrPviTaskName))
            {
                int spaceIndex = brJob.BrPviTaskName.IndexOf(" ");
                if (spaceIndex < 0)
                {
                    taskCompleteName = String.Format("{0}/{1}", BrPviCpuName, brJob.BrPviTaskName);
                }
                else
                {
                    taskCompleteName = String.Format("{0}/\"{1}\"", BrPviCpuName, brJob.BrPviTaskName);
                }
            }
            brJob.BrPviTaskCompleteName = taskCompleteName;

            // Variable
            string variableName = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskCompleteName))
            {
                variableName = String.Format("{0}/{1}", taskCompleteName, brJob.BrPviVariableName);
            }
            else
            {
                variableName = String.Format("{0}/{1}", BrPviCpuName, brJob.BrPviVariableName);
            }
            brJob.BrPviVariableCompleteName = variableName;

            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BuildJobPviObjNames {0} - Variable: {1}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJob.BrPviVariableCompleteName);
        }



        private void ManageEvent(BrPviProcol.BrPviErrorCodes baseErrorCode, BrPviPviObject pviObj, bool restartScheduler = false)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManagePviEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);
            // Error
            if (pviObj.LastError != 0)
            {
                if (ListWholeJob.Count > 0)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }

                    BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                    BrPviChannel brChannel = (BrPviChannel)GetChannel();
                    switch (baseErrorCode)
                    {
                        case BrPviProcol.BrPviErrorCodes.ErrorCodeEventError: // ManagePviEvent, ManagePviErrorEvent
                            brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorReceivedErrorEvent, pviObj.LastError, pviObj.Name);
                            break;
                        case BrPviProcol.BrPviErrorCodes.ErrorCodeCreateFailure:    // ManageCreationEvent
                            brCommDriver.LastPviCreateError = String.Format(Properties.Resources.BrErrorCreationPviObject, pviObj.Name, pviObj.LastError);
                            break;
                    }
                    foreach (var j in listJob)
                    {
                        BrPviCommJob brJob = (BrPviCommJob)j;

                        BrPviPviObject jobPviObj = brChannel.GetPviObject(brJob.BrPviVariableCompleteName);
                        if (jobPviObj != null)
                            jobPviObj.LastError = pviObj.LastError;

                        //brJob.Status = BrPviCommJobStatus.Idle;
                        ExecutedJobArgs e = new ExecutedJobArgs();
                        e.Job = brJob;
                        if (!pviObj.IsLastErrorFatal())
                            e.ErrorCode = (DriverErrorCodes)baseErrorCode;
                        else
                            e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                        brChannel.OnJobExecuted(e);
                    }
                    if (restartScheduler)
                        brChannel.RestartScheduler();
                }
                LastErrorCode = (DriverErrorCodes)baseErrorCode;
            }
            else // Creation of the PVI object successfully performed
            {
                if (pviObjectsInitialized == false)
                {
                    if (pviObj.Type == PviObjectTypes.POBJ_CPU)
                        pviObjectsInitialized = true;
                }
                SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            }
        }

        public void ManagePviEvent(BrPviPviObject pviObj, bool restartScheduler = false)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManagePviEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);

            ManageEvent(BrPviProcol.BrPviErrorCodes.ErrorCodeEventError, pviObj, restartScheduler);            
        }

        public void ManageCreationEvent(BrPviPviObject pviObj)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManageCreationEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);

            ManageEvent(BrPviProcol.BrPviErrorCodes.ErrorCodeCreateFailure, pviObj);
        }

        public void ManagePviErrorEvent(BrPviPviObject pviObj)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManagePviEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);

            ManageEvent(BrPviProcol.BrPviErrorCodes.ErrorCodeEventError, pviObj);
        }

        public void ManageDataReceivedEvent(PviObjEvent pviEvent)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManageDataReceivedEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, pviEvent.PviObj.Type, pviEvent.PviObj.LastError);
            if (pviObjectsInitialized == false)
            {
                if (pviEvent.PviObj.Type == PviObjectTypes.POBJ_CPU)
                {
                    pviObjectsInitialized = true;
                }
            }
            SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
        }

        public void ManageConnectionBroken(List<CommJob> list)
        {
            //pviObjectsInitialized = false;
            //ResetJobsState();

            foreach (CommJob job in list)
            {
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = job;
                // Put all the jobs in error
                e.ErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeConnectionBroken;
                ((BrPviChannel)e.Job.Station.GetChannel()).OnJobExecuted(e);
                LastErrorCode = (DriverErrorCodes)BrPviProcol.BrPviErrorCodes.ErrorCodeEventError;
            }
        }

        public void ResetJobsState()
        {
            pviObjectsInitialized = false;
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                Parallel.ForEach(listJob, job =>
                {
                    ((BrPviCommJob)job).ResetBrPviInternalSettings();
                });                
            }
        }

        private uint UpdateTagValue(NodeId tagnodeid, DataValue value)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];
            }
            BrPviCommJob tcJob = job as BrPviCommJob;
            return tcJob.UpdateTagValue(tagnodeid, value);
        }

        private bool ParseReceivedData(byte[] receivebuffer, ref BrPviCommJob tcJ,
                                       ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();

            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = tcJ.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            uint ArraySize = tcJ.TagsList[0].TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            int TotalJobSize;
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
            if (!tcJ.isProtocolBool() || (tcJ.ElementNumber == 0 && (uint)tcJ.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
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

            tcJ.SetJobData(tempBuffer, ref changed);
            if (areArguments)
            {
                if (tcJ.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < tcJ.TagsList.Count; k++)
                    {
                        items[k + 1] = tcJ.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            BrPviCommJob brj = job as BrPviCommJob;
            ParseReceivedData(receivedbuffer, ref brj, ref arguments);
            return true;
        }        
        #endregion

        #region Properties

        private string _BrPviRoutingPath;
        public string BrPviRoutingPath
        {
            get { return _BrPviRoutingPath; }
            set
            {
                _BrPviRoutingPath = value;
            }
        }

        private int _BrPviResponseTimeout;
        public int BrPviResponseTimeout
        {
            get { return _BrPviResponseTimeout; }
            set
            {
                _BrPviResponseTimeout = value;
            }
        }

        private BrPviProcol.InterfaceTypes _BrPviInterfaceType;
        public BrPviProcol.InterfaceTypes BrPviInterfaceType
        {
            get { return _BrPviInterfaceType; }
            set
            {
                _BrPviInterfaceType = value;
            }
        }

        private short _BrPviSourceStationPort;
        public short BrPviSourceStationPort
        {
            get { return _BrPviSourceStationPort; }
            set
            {
                _BrPviSourceStationPort = value;
            }
        }

        private byte _BrPviSourceStationID;
        public byte BrPviSourceStationID
        {
            get { return _BrPviSourceStationID; }
            set
            {
                _BrPviSourceStationID = value;
            }
        }

        private string _BrPviDestinationStationIPAddress;
        public string BrPviDestinationStationIPAddress
        {
            get { return _BrPviDestinationStationIPAddress; }
            set
            {
                _BrPviDestinationStationIPAddress = value;
            }
        }

        private short _BrPviDestinationStationPort;
        public short BrPviDestinationStationPort
        {
            get { return _BrPviDestinationStationPort; }
            set
            {
                _BrPviDestinationStationPort = value;
            }
        }

        private byte _BrPviDestinationStationID;
        public byte BrPviDestinationStationID
        {
            get { return _BrPviDestinationStationID; }
            set
            {
                _BrPviDestinationStationID = value;
            }
        }

        private byte _BrPviSerialPort;
        public byte BrPviSerialPort
        {
            get { return _BrPviSerialPort; }
            set
            {
                _BrPviSerialPort = value;
            }
        }

        private int _BrPviSerialBaudrate;
        public int BrPviSerialBaudrate
        {
            get { return _BrPviSerialBaudrate; }
            set
            {
                _BrPviSerialBaudrate = value;
            }
        }

        private int _BrPviSerialParity;
        public int BrPviSerialParity
        {
            get { return _BrPviSerialParity; }
            set
            {
                _BrPviSerialParity = value;
            }
        }

        private int _BrPviSerialFlowControl;
        public int BrPviSerialFlowControl
        {
            get { return _BrPviSerialFlowControl; }
            set
            {
                _BrPviSerialFlowControl = value;
            }
        }

        private string _BrPviLineName;
        public string BrPviLineName
        {
            get { return _BrPviLineName; }
            set
            {
                _BrPviLineName = value;
            }
        }

        private string _BrPviLineDescr;
        public string BrPviLineDescr
        {
            get { return _BrPviLineDescr; }
            set
            {
                _BrPviLineDescr = value;
            }
        }

        private string _BrPviDeviceName;
        public string BrPviDeviceName
        {
            get { return _BrPviDeviceName; }
            set
            {
                _BrPviDeviceName = value;
            }
        }

        private string _BrPviDeviceDescr;
        public string BrPviDeviceDescr
        {
            get { return _BrPviDeviceDescr; }
            set
            {
                _BrPviDeviceDescr = value;
            }
        }

        private string _BrPviStationName;
        public string BrPviStationName
        {
            get { return _BrPviStationName; }
            set
            {
                _BrPviStationName = value;
            }
        }

        private string _BrPviStationDescr;
        public string BrPviStationDescr
        {
            get { return _BrPviStationDescr; }
            set
            {
                _BrPviStationDescr = value;
            }
        }

        private string _BrPviCpuName;
        public string BrPviCpuName
        {
            get { return _BrPviCpuName; }
            set
            {
                _BrPviCpuName = value;
            }
        }

        private string _BrPviCpuDescr;
        public string BrPviCpuDescr
        {
            get { return _BrPviCpuDescr; }
            set
            {
                _BrPviCpuDescr = value;
            }
        }

        private uint _BrPviRemoveDisabledItemAfterSecs;
        public uint BrPviRemoveDisabledItemAfterSecs
        {
            get { return _BrPviRemoveDisabledItemAfterSecs; }
            set
            {
                _BrPviRemoveDisabledItemAfterSecs = value;
            }
        }
        #endregion
    }
}
