using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace Demo
{
    class DemoStation : Station
    {
        // bit nr of state variable to start/stop simulation
        public enum CommandType : UInt16
        {
            StartStopSimulation = 2
        }

        public enum State
        {
            LaunchAtStartUp,
            Running,
            Pause
        }

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public DemoStation(CommunicationDriver commdriver, DemoStationSettings settings)
            : base(commdriver, settings)
        {
            MaxRetriesBeforeError = 0;
            _SimulationInterval = settings.SimulationInterval;
            if ((bool)settings.LaunchSimulationAtStartUp)
                _SimulationState = State.LaunchAtStartUp;
            else
                _SimulationState = State.Pause;

            if (stationStateCommandVariable.hasBeenSet)
                stationStateCommandVariable.AddBitsConversion((UInt16)CommandType.StartStopSimulation, (UInt16)CommandType.StartStopSimulation, StateCommandVariable.StateCommandTypes.Command, 1);
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as DemoCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DemoCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as DemoTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DemoCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new DemoTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DemoCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DemoCommJobSettings(session, commJob);
        }
        #endregion

        #region Override Methods

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            DemoCommJob mJ = e.Job as DemoCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                List<Tag> Answer = e.Values as List<Tag>;                
                if (Answer != null)
                {
                    List<object> ChangedTags = new List<object>();
                    if (DemoProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == DriverErrorCodes.ErrorDeviceOpenFailed);

            base.ProcessJobValues(e);
        }

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            DemoCommJob j = null;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                j = mapTagJob[tagnodeid] as DemoCommJob;

                if (j.Type == LinkType.Input)
                    return (StatusCodes.BadNotWritable);
            }

            if (j.OnWriteTagInternal(tagnodeid, ref value))
            {
                Channel.ExecuteJob(j);                        
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Values = j.GetCurrentValues();
                eJob.Job = j;
                ((DemoChannel)Channel).PublicOnJobExecuted(eJob);
            }

            return StatusCodes.Good;
        }

        // functionality not suported by this driver
        public override uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, IList<object> args, bool takedata = true, NodeId tagNodeId = null, object value = null)
        {
            args[0] = (int)DriverErrorCodes.ErrorTimeOut;

            return StatusCodes.BadNotSupported;
        }

        /// <summary>
        /// Tag management associated to a conditional variable 
        /// </summary>
        /// <param name="value">Conditional var value</param>
        public override void ManageUpdatedValueForTheStateCommandVariable(NodeId node, DataValue value)
        {
            base.ManageUpdatedValueForTheStateCommandVariable(node, value);

            // force state variable bit to launch (start) simulation 
            if (_SimulationState == State.LaunchAtStartUp)
                base.SetStateCommandVariableBit(true, (UInt16)CommandType.StartStopSimulation);

            bool bitValue = false;
            if (GetStateCommandVariableBit(ref bitValue, (UInt16)CommandType.StartStopSimulation))
            {
                if (bitValue)
                    StartStopSimulation(State.Running);
                else
                    StartStopSimulation(State.Pause);
            }
        }

        public override uint OnMethodCall(NodeId node, IList<object> inputArguments, IList<object> outputArguments)
        {
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(node))
                    return StatusCodes.BadNodeIdInvalid;
                
                CommJob job = mapTagJob[node];

                Tag tag = job.TagsList.Find(o => { return o.TagNode.NodeId == node; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                switch (tag.DynSettings.MethodID)
                {
                    case (int)DemoDriver.DemoMethods.StartSim:
                        StartStopSimulation(State.Running);
                        break;
                    case (int)DemoDriver.DemoMethods.StopSim:
                        StartStopSimulation(State.Pause);                        
                        break;                    
                }
            }
            return StatusCodes.Good;
        }

        #endregion

        #region Properties
        private uint _SimulationInterval;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value; }
        }

        private State _SimulationState;
        public State SimulationState
        {
            get { return _SimulationState; }
            set { _SimulationState = value; }
        }
        #endregion

        #region Local Methods
        private void StartStopSimulation(State state)
        {
            // stop scheduler when assign variable
            if (_SimulationState != state)
            {
                _SimulationState = state;
                if (_SimulationState == State.Running)
                {
                    ((DemoChannel)Channel).GetCommDriver().SuspendScheduler();
                    List<CommJob> listJobs = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJobs.AddRange(ListWholeJob);
                    }

                    Parallel.ForEach(listJobs, job =>
                    {
                        ((DemoCommJob)job).RestartSimulation();
                    });
                    ((DemoChannel)Channel).GetCommDriver().RestartScheduler();
                    ((DemoChannel)Channel).ReStartScheduler();
                }
            }                    
        }
        #endregion
    }
}
