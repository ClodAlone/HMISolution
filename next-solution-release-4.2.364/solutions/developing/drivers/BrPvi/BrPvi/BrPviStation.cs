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

namespace BrPvi
{
    public enum InterfaceTypes
    {
        Ethernet,
        Serial
    }

    public enum FlowControlTypes
    {
        None,
        RtsOff,
        RS232Mode,
        RS422Mode
    }

    public enum ParityTypes
    {
        None,
        Odd,
        Even,
        Mark,
        Space
    }

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

        public override bool Startup()
        {
            bool Result = false;

            lock (_lockActivateDeactivate)
            {
                bool Reactivated = (((BrPviChannel)Channel).AnyPVIObjectOfStationDeactivated(this));

                if (Channel != null && !Reactivated)
                {
                    // Add jobs to the channel dictionary that links jobs with PVI objects
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }

                    BrPviChannel brChannel = (BrPviChannel)Channel;
                    foreach (var job in listJob)
                    {
                        BrPviCommJob brJob = (BrPviCommJob)job;
                        BuildJobPviObjNames(brJob);
                        if (!String.IsNullOrWhiteSpace(brJob.BrPviTaskCompleteName))
                        {
                            brChannel.AddJobToPviObjDictionary(brJob.BrPviTaskCompleteName, brJob);
                        }
                        if (!String.IsNullOrWhiteSpace(brJob.BrPviVariableCompleteName))
                        {
                            brChannel.AddJobToPviObjDictionary(brJob.BrPviVariableCompleteName, brJob);
                        }
                    }
                }

                // Call the base class method
                Result = base.Startup();
                if (Result && Reactivated)
                    if (pviObjectsInitialized)  // reactive station's items only when station is reactivated, not at startup
                        ((BrPviChannel)Channel).ActivateDeactivateAllPVIObjectOfStation(this, BrPviCommJob.PviObjectState.Activated);
                    else // this situation occour when pvimotior was stopped when item is deactive; pvimonitor don't reload so force driver to do it
                        ((BrPviChannel)Channel).ReloadAllActivePVIObjectOfStation(this);
            }

            return Result;
        }

        /// <summary>
        /// Remove all tag's PVI elements associated to the station e reset job flag; when station will be reactivated, 
        /// driver automatically regenerate all data
        /// </summary>
        /// <returns></returns>
        public override bool SuspendStation()
        {
            bool Result = false;

            lock (_lockActivateDeactivate)
            {
                ((BrPviChannel)Channel).ActivateDeactivateAllPVIObjectOfStation(this, BrPviCommJob.PviObjectState.DeActivated);

                Result = base.SuspendStation();
            }

            return Result;
        }

        public List<CommJob> GetListWholeJobCopy()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            return listJob;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            BrPviCommJob brJ = e.Job as BrPviCommJob;
            if (brJ == null)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - BrPviStation.ProcessJobValues - {0} called for PVI Tag: {1}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), brJ.BrPviVariableCompleteName));

            //if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            //{
            //    if (e.Values != null)
            //    {
            //        byte[] Answer = (byte[])e.Values;
            //        List<object> ChangedTags = new List<object>();
            //        if (ParseReceivedData(Answer, ref tcJ, ref ChangedTags))
            //        {
            //            foreach (var tag in ChangedTags)
            //            {
            //                var j = tag as Tag;
            //                if (j != null)
            //                {
            //                    e.ChangedTags.Add(j);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
            //        }
            //    }
            //}

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<Tag> ChangedTags = new List<Tag>();
                brJ.SetJobData(Answer, ref ChangedTags);
                brJ.Status = BrPviCommJobStatus.Idle;
                foreach (var tag in ChangedTags)
                {
                    var t = tag as Tag;
                    if (t != null)
                    {
                        e.ChangedTags.Add(t);
                    }
                }
#if DEBUG
                String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                String DbgText =
                String.Format("BR_DEBUG - BrPviStation.ProcessJobValues - {0} Num. of changed tags: {1} - PVI Tag: {2} - Number of Data Bytes: {3}",
                              dbgTime, e.ChangedTags.Count, brJ.BrPviVariableCompleteName, Answer.Count());
                if (e.ChangedTags.Count > 0)
                {
                    DbgText += String.Format(" - Tag: {0}", e.ChangedTags[0].TagNode.NodeId);
                }
                System.Diagnostics.Debug.WriteLine(DbgText);
#endif
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
                        BrPviCommJob j = commjob as BrPviCommJob;
                        j.Status = BrPviCommJobStatus.Idle;
                        ChannelBase.ChangeStateJob(commjob, CommJobState.PollingInError);
                    }
                }
            }

            base.ProcessJobValues(e);
        }

        //public void BaseProcessJobValues(ExecutedJobArgs e)
        //{
        //    base.ProcessJobValues(e);
        //}

        public override bool ParseDynamicTags(IList<Tag> tags)
        {            
            //start aggregation thread/procedure for this station.
            lock (lockListObject)
            {
                if (tags == null || tags.Count == 0)
                    return false;

                if (mapGroupedJobs == null)
                    mapGroupedJobs = new Dictionary<string, List<CommJob>>();

                List<Tag> tList = SortTags(tags);

                tags.Clear();

                List<Tag> innerList = new List<Tag>();
                List<Tag> lMethods = new List<Tag>();

                //BrPviChannel tcChannel = Channel as BrPviChannel;
                //byte tcVersion = tcChannel.BrPviVersion;
                //byte tcVersion = 0;

                foreach (var tag in tList)
                {
                    innerList.Clear();
                    lMethods.Clear();
                    if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled())
                    {
                        //struttura
                        List<TagDefinition> pList = new List<TagDefinition>();
                        GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                        string dynsettings = string.Empty;
                        DynTagSettings dtCalcBase = tag.DynSettings;
                        BrPviDynTagSettings dtCalc = dtCalcBase as BrPviDynTagSettings;
                        //dtCalc.BrPviVersion = tcVersion;
                        TagDefinition olddtag;
                        for (int i = 0; i < pList.Count; i++)
                        {

                            TagDefinition dtag = pList[i];
                            olddtag = dtag;
                            if (dtag.DynamicSettings.Length == 0)
                            {
                                if (dynsettings.Length == 0)
                                {
                                    dynsettings = dtCalc.GetFirstDynSetting(tag, dtag);
                                }
                                else
                                {
                                    //dtCalc.TryParse(dynsettings);
                                    dynsettings = dtCalc.GetNextDynSetting(pList[i - 1], dtag);
                                    //dynsettings = dtCalc.GetNextDynSetting(pList[i - 1]);
                                }
                                dtag.DynamicSettings = dynsettings;
                            }
                            //get dynsettings for each tagdefinition.
                            Tag t = GetCommDriver().CreateTag(dtag);
                            if (t.DynSettings.MethodID > -1)
                            {
                                //ricordati del metodo...
                                lMethods.Add(t);
                                if (dynsettings.Length > 0)
                                    dtag = olddtag;
                                continue;
                            }
                            else
                            {
                                if (t.bIsValid)
                                {
                                    innerList.Add(t);
                                }
                                else
                                {
                                    CommDriver.OnTagChanged(t.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError));
                                    CommunicationDriver.log.Error(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tag.DynSettings.MethodID > -1)
                            lMethods.Add(tag);
                        innerList.Add(tag);
                    }
                    foreach (var candTag in innerList)
                    {
                        CommJob candJob = CreateJob(candTag);
                        // if job creation failed, tag's DynamicSettings is not valid --> use job info to populate tag error
                        if (!candJob.IsValid)
                        {
                            candTag.bIsValid = false;
                            candTag.InvalidReason = candJob.InvalidReason;
                        }
                        
                        if (!candJob.IsValid || mapTagJob.ContainsKey(candTag.TagNode.NodeId))
                            continue;

                        tags.Add(candTag);

                        List<CommJob> tmpList;
                        if (mapGroupedJobs.ContainsKey(candJob.GroupString))
                            tmpList = mapGroupedJobs[candJob.GroupString];
                        else
                            tmpList = new List<CommJob>();

                        if (tmpList.Count == 0)
                        {
                            tmpList.Add(candJob);
                            mapTagJob.Add(candTag.TagNode.NodeId, candJob);
                            if (!ListWholeJob.Contains(candJob))
                                ListWholeJob.Add(candJob);

                            if (lMethods.Count > 0)
                            {
                                if (!mapJobMethods.Keys.Contains(candJob))
                                    mapJobMethods[candJob] = new List<NodeId>();
                                foreach (var m in lMethods)
                                {
                                    if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                        mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                    if (!mapMethodJobs[m.TagNode.NodeId].Contains(candJob))
                                        mapMethodJobs[m.TagNode.NodeId].Add(candJob);

                                    if (!mapJobMethods[candJob].Contains(m.TagNode.NodeId))
                                        mapJobMethods[candJob].Add(m.TagNode.NodeId);

                                    if (!candJob.TagsList.Contains(m))
                                        candJob.TagsList.Add(m);
                                    AddToMapTagJob(m.TagNode.NodeId, candJob);
                                    GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                }
                            }
                        }
                        else
                        {
                            bool bAggregated = false;
                            foreach (var testJob in tmpList)
                            {
                                uint newIndex = 0;

                                var aggType = testJob.TestAggregateJob(candJob, out newIndex);

                                bool bMethod = mapJobMethods.Keys.Contains(testJob) && mapJobMethods.Keys.Contains(candJob);
                                if (bMethod)
                                {
                                    bMethod = mapJobMethods[testJob].Count == mapJobMethods[candJob].Count;
                                    if (bMethod)
                                    {
                                        foreach (var p in mapJobMethods[testJob])
                                        {
                                            if (!mapJobMethods[testJob].Contains(p))
                                            {
                                                aggType = JobAggregationType.JobAggregImpossible;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (aggType != JobAggregationType.JobAggregImpossible)
                                {
                                    //Aggregate
                                    testJob.AggregateJob(candJob, aggType, newIndex);

                                    foreach (var a in candJob.TagsList)
                                        mapTagJob[a.TagNode.NodeId] = testJob;

                                    if (lMethods.Count > 0)
                                    {
                                        if (!mapJobMethods.Keys.Contains(testJob))
                                            mapJobMethods[testJob] = new List<NodeId>();
                                        foreach (var m in lMethods)
                                        {
                                            if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                                mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                            if (!mapMethodJobs[m.TagNode.NodeId].Contains(testJob))
                                                mapMethodJobs[m.TagNode.NodeId].Add(testJob);

                                            if (!mapJobMethods[testJob].Contains(m.TagNode.NodeId))
                                                mapJobMethods[testJob].Add(m.TagNode.NodeId);

                                            if (!testJob.TagsList.Contains(m))
                                                testJob.TagsList.Add(m);
                                            AddToMapTagJob(m.TagNode.NodeId, testJob);
                                            GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                        }
                                    }


                                    bAggregated = true;
                                    break;
                                }
                            }
                            if (!bAggregated)
                            {
                                //add candJob to the list
                                tmpList.Add(candJob);
                                mapTagJob.Add(candTag.TagNode.NodeId, candJob);
                                if (!ListWholeJob.Contains(candJob))
                                    ListWholeJob.Add(candJob);

                                if (lMethods.Count > 0)
                                {
                                    if (!mapJobMethods.Keys.Contains(candJob))
                                        mapJobMethods[candJob] = new List<NodeId>();
                                    foreach (var m in lMethods)
                                    {
                                        if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                            mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                        if (!mapMethodJobs[m.TagNode.NodeId].Contains(candJob))
                                            mapMethodJobs[m.TagNode.NodeId].Add(candJob);

                                        if (!mapJobMethods[candJob].Contains(m.TagNode.NodeId))
                                            mapJobMethods[candJob].Add(m.TagNode.NodeId);

                                        if (!candJob.TagsList.Contains(m))
                                            candJob.TagsList.Add(m);
                                        AddToMapTagJob(m.TagNode.NodeId, candJob);
                                        GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                    }
                                }
                            }
                        }
                        UpdateTagValue(candTag.TagNode.NodeId, new DataValue()
                        {
                            Value = candTag.TagNode.InitialValue,
                            ServerTimestamp = DateTime.UtcNow,
                            SourceTimestamp = DateTime.MinValue,
                            StatusCode = candTag.TagNode.InitialValue != null ? StatusCodes.Good : StatusCodes.BadWaitingForInitialData
                        });

                        mapGroupedJobs[candJob.GroupString] = tmpList;
                    }
                }
            }
            return true;
        }

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
                case InterfaceTypes.Ethernet:
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
                        case (int)FlowControlTypes.None:
                            _BrPviDeviceDescr += String.Format(" /RS=-1");
                            break;
                        case (int)FlowControlTypes.RtsOff:
                            _BrPviDeviceDescr += String.Format(" /RS=0");
                            break;
                        case (int)FlowControlTypes.RS422Mode:
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
            if (_BrPviInterfaceType == InterfaceTypes.Ethernet)
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

        public void CreatePviObjects()
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            // Line
            BrPviPviObject pviLineObj = brChannel.GetOrCreatePviObject(_BrPviLineName, _BrPviLineDescr, "EV=ed", PviObjectTypes.POBJ_LINE);
            if (pviLineObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviLineObj.Name, this);
            }
            else
            {
                return;
            }

            // Device
            BrPviPviObject pviDeviceObj = brChannel.GetOrCreatePviObject(_BrPviDeviceName, _BrPviDeviceDescr, "EV=ed", PviObjectTypes.POBJ_DEVICE);
            if (pviDeviceObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviDeviceObj.Name, this);
            }
            else
            {
                return;
            }

            // Station
            BrPviPviObject pviStationObj = brChannel.GetOrCreatePviObject(_BrPviStationName, _BrPviStationDescr, "EV=ed", PviObjectTypes.POBJ_STATION);
            if (pviStationObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviStationObj.Name, this);
            }
            else
            {
                return;
            }

            // CPU
            BrPviPviObject pviCpuObj = brChannel.GetOrCreatePviObject(_BrPviCpuName, _BrPviCpuDescr, "EV=ed", PviObjectTypes.POBJ_CPU);
            if (pviCpuObj != null)
            {
                brChannel.AddStationToPviObjDictionary(pviCpuObj.Name, this);
            }
            else
            {
                return;
            }
        }

        public void CreateJobPviObjects(BrPviCommJob job)
        {
            BrPviChannel brChannel = (BrPviChannel)Channel;

            // Task
            string taskCompleteName = String.Empty;
            string taskDescription = String.Empty;
            if (!String.IsNullOrWhiteSpace(job.BrPviTaskName))
            {
                int spaceIndex = job.BrPviTaskName.IndexOf(" ");
                if (spaceIndex < 0)
                {
                    taskCompleteName = String.Format("{0}/{1}", BrPviCpuName, job.BrPviTaskName);
                    taskDescription = String.Format("CD={0}", job.BrPviTaskName);
                }
                else
                {
                    taskCompleteName = String.Format("{0}/\"{1}\"", BrPviCpuName, job.BrPviTaskName);
                    taskDescription = String.Format("CD=\"{0}\"", job.BrPviTaskName);
                }

                BrPviPviObject pviTaskObj = brChannel.GetOrCreatePviObject(taskCompleteName, taskDescription, "EV=ed", PviObjectTypes.POBJ_TASK);
                if (pviTaskObj != null)
                {
                    brChannel.AddJobToPviObjDictionary(pviTaskObj.Name, job);
                }
                else
                {
                    return;
                }
            }

            // Variable
            string variableName = String.Empty;
            string variableDescription = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskCompleteName))
            {
                variableName = String.Format("{0}/{1}", taskCompleteName, job.BrPviVariableName);
            }
            else
            {
                variableName = String.Format("{0}/{1}", BrPviCpuName, job.BrPviVariableName);
            }
            job.BrPviVariableCompleteName = variableName;

            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.CreateJobPviObjects {0} - Variable: {1} - Job Num. of Tags: {2}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), job.BrPviVariableCompleteName, job.TagsList.Count);

            if ((job.Type == LinkType.Input) || (job.Type == LinkType.InputOutput))
            {
                variableDescription = String.Format("CD=\"{0}\" RF={1}", job.BrPviVariableName, job.BrPviRefreshRate);
            }
            else
            {
                variableDescription = String.Format("CD=\"{0}\" AT=w RF={1}", job.BrPviVariableName, job.BrPviRefreshRate);
            }

            BrPviPviObject pviVarObj = brChannel.GetOrCreatePviObject(variableName, variableDescription, "EV=ed", PviObjectTypes.POBJ_PVAR);
            if (pviVarObj != null)
            {
                brChannel.AddJobToPviObjDictionary(pviVarObj.Name, job);
            }
            else
            {
                return;
            }

            SetPviObjectEvMask(job);
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

        public void ManageCreationEvent(BrPviPviObject pviObj)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManageCreationEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);
            // Error
            if (pviObj.LastError != 0)
            {
                if(pviObjectsInitialized == true)
                {
                    pviObjectsInitialized = false;
                }

                if (ListWholeJob.Count > 0)
                {
                    BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                    brCommDriver.LastPviCreateError = String.Format(Properties.Resources.BrErrorCreationPviObject, pviObj.Name, pviObj.LastError); 
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }
                    foreach (var j in listJob)
                    {
                        BrPviCommJob brJob = (BrPviCommJob)j;
                        brJob.Status = BrPviCommJobStatus.Idle;
                        ExecutedJobArgs e = new ExecutedJobArgs();
                        e.Job = brJob;
                        if(pviObj.LastError != (uint)DriverErrorCodes.ErrorTimeOut)
                        {
                            e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeCreateFailure;
                        }
                        else
                        {
                            e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                        }
                        //base.ProcessJobValues(e);
                        OnJobExecuted(brJob, e);
                    }
                    LastErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeCreateFailure;
                    //InErrorState = true;
                }
            }
            // Creation of the PVI object successfully performed
            else
            {
                if (pviObjectsInitialized == false)
                {
                    if(pviObj.Type == PviObjectTypes.POBJ_CPU)
                    {
                        pviObjectsInitialized = true;
                    }
                }
                SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            }
        }

        public void ManagePviEvent(BrPviPviObject pviObj)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviStation.ManagePviEvent {0} - Called for pviObj: {1} - type: {2} - error: {3}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, pviObj.Type, pviObj.LastError);
            // Error
            if (pviObj.LastError != 0)
            {
                // if station was suspend don't report any communication error
                bool suspendBitNewValue = false;
                if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
                {                    
                    if (suspendBitNewValue)
                    {
                        // Link broken --> enable driver to reload to PVIMonitor all tags
                        if (pviObj.LastError == PviComManager.PVI_ERROR_LINK_BROKEN)
                            this.pviObjectsInitialized = false;
                        return;
                    }
                }

                if (ListWholeJob.Count > 0)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }

                    BrPviDriver brCommDriver = (BrPviDriver)CommDriver;
                    brCommDriver.LastPviEventError = String.Format(Properties.Resources.BrErrorReceivedErrorEvent, pviObj.LastError, pviObj.Name);
                    foreach (var j in listJob)
                    {                        
                        BrPviCommJob brJob = (BrPviCommJob)j;
                        brJob.Status = BrPviCommJobStatus.Idle;
                        ExecutedJobArgs e = new ExecutedJobArgs();
                        e.Job = brJob;
                        if (pviObj.LastError != (uint)DriverErrorCodes.ErrorTimeOut)
                        {
                            e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeEventError;
                        }
                        else
                        {
                            e.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                        }
                        //base.ProcessJobValues(e);
                        OnJobExecuted(brJob,e);
                    }
                }
                LastErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeEventError;
                //InErrorState = true;                
            }
            else
            {
                SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            }
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

        public void ManageConnectionBroken()
        {
            // if station was suspend don't report any communication error
            bool suspendBitNewValue = false;
            if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if (suspendBitNewValue)
                    return;
            }

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
                    BrPviCommJob brJob = (BrPviCommJob)job;
                    brJob.pviObjectState = BrPviCommJob.PviObjectState.NotInitialized; //false

                    // Deactivate the job
                    if (brJob.Status != BrPviCommJobStatus.Idle)
                    {
                        brJob.Status = BrPviCommJobStatus.Idle;
                    }
                });
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeConnectionBroken;
                // Put all the jobs in error
                e.GeneralError = true;
                //base.ProcessJobValues(e);                
                OnJobExecuted(e.Job,e);
                LastErrorCode = (DriverErrorCodes)BrPviErrorCodes.ErrorCodeEventError;
                //InErrorState = true;
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

        private InterfaceTypes _BrPviInterfaceType;
        public InterfaceTypes BrPviInterfaceType
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
        #endregion

    }
}
