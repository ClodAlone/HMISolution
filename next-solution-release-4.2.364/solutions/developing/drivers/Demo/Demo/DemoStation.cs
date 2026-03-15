using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace Demo
{
    internal sealed class DemoStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public DemoStation(CommunicationDriver commdriver, DemoStationSettings settings)
            : base(commdriver, settings)
        {
            _SimulationInterval = settings.SimulationInterval;
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

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DemoCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DemoCommJobSettings(session, commJob);
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                e.ChangedTags.AddRange(e.Job.TagsList);
            }

            base.ProcessJobValues(e);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        { return new DemoTag(td); }

        #endregion

        #region Override methods
        public override bool SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            return base.SetInUse(tagnodeid, bInUse, -1);
        }
        public override uint OnMethodCall(NodeId node, IList<object> inputArguments, IList<object> outputArguments)
        {
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(node))
                    return StatusCodes.BadNodeIdInvalid;
                CommJob job = mapTagJob[node];

                Tag tag = job.TagsList.Find(o => { return o.TagNode.NodeId == node; });
                
                List<Tag> tl = (from t in job.TagsList.AsParallel() where (t.TagNode.NodeId != node && t.DynSettings.MethodID == -1)
                                select t).ToList();
                bool localMethod = (tl.Count > 0);

                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                switch(tag.DynSettings.MethodID)
                {
                    case (int)DemoMethods.StartSim:
                        {
                            if (!localMethod)
                            {
                                DemoChannel d = Channel as DemoChannel;
                                if (d != null)
                                    d.StartSimulation();
                            }
                            else
                            { 
                                //start single job simulation
                                DemoCommJob d  = job as DemoCommJob;
                                d.StartSimulation();
                            }
                        }
                        break;
                    case (int)DemoMethods.StopSim:
                        {
                            if (!localMethod)
                            {
                                DemoChannel d = Channel as DemoChannel;
                                if (d != null)
                                    d.StopSimulation();
                            }
                            else
                            {
                                //stop single job simulation
                                DemoCommJob d = job as DemoCommJob;
                                d.StopSimulation();
                            }
                        }
                        break;
                    case (int)DemoMethods.IncSimulationTime:
                        if (!localMethod)
                            SimulationInterval += 100;
                        else
                        {
                            DemoCommJob d = job as DemoCommJob;
                            d.SimulationInterval += 100;
                        }
                        break;
                    case (int)DemoMethods.DecSimulationTime:
                        if (!localMethod)
                        {
                            if (SimulationInterval >= 100)
                                SimulationInterval -= 100;
                            else
                                SimulationInterval = 0;
                        }
                        else
                        {
                            DemoCommJob d = job as DemoCommJob;
                            if (d.SimulationInterval >= 100)
                                d.SimulationInterval -= 100;
                            else
                                d.SimulationInterval = 0;
                        }
                        break;
                }
            }
            return StatusCodes.Good;
        }
        #endregion

        #region Properties
        private uint _SimulationInterval = 500;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value;}
        }
        #endregion

    }
}
