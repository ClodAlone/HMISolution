using System;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace ExampleDemo
{
    internal sealed class ExampleDemoCommJob : CommJob
    {
        #region Constructors
        public ExampleDemoCommJob(Station station, ExampleDemoCommJobSettings settings)
            : base(station, settings)
        {
            _DemoType = settings.DemoType;
            CheckJobValid();
            CommonVal = null;
            lastTrigoTime = DateTime.UtcNow;
        }

        /*void AddPrototypeMethodToTagList(Station station, NodeId node)
        {
            List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
            station.GetCommDriver().OnTagPrototypeQuery(node, ref tList);
            foreach (var tag in tList)
            {
                if (tag.DataType.IdType == IdType.Guid)
                    AddPrototypeMethodToTagList(station, tag.NodeId);
                else
                {
                    Tag t = station.GetCommDriver().CreateTag(tag);
                    if (t.DynSettings.MethodID > -1)
                    {
                        station.AddToMapTagJob(t.TagNode.NodeId, this);
                        station.GetCommDriver().AddToTagToStationMap(t.TagNode.NodeId, station);
                        TagsList.Add(t);
                    }
                    else 
                    {
                        TagsList.Add(t);
                    }
                }

                System.Diagnostics.Trace.TraceInformation(string.Format("member: {2} type:{0} Settings:{1}", (tag.DataType != null ? tag.DataType.Identifier : "Method"), tag.DynamicSettings, tag.NodeId.ToString()));
            }
        } steve 040811*/

        public ExampleDemoCommJob(Station station, ExampleDemoTag defTag)
            : base(station, defTag)
        {

            /*if (defTag.TagNode.DataType.IdType == IdType.Guid)
            {
                AddPrototypeMethodToTagList(station, defTag.TagNode.NodeId);
                var ProtoList = (from t in TagsList.AsParallel() where t.TagNode.DataType.IdType == IdType.Guid select t).ToList();
                foreach (var t in ProtoList)
                {
                    TagsList.Remove(t);
                }
            } steve 040811*/

            _DemoType = (ExampleDemoTypes) defTag.DemoDynSettings.DemoType;
            CheckJobValid();
        }

        public ExampleDemoCommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        protected ExampleDemoCommJob()
        {
            CheckJobValid();
        }
        #endregion

        public override void GetJobData(ref object jobData) { }

        #region Static methods
        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;

                if ((DemoType == ExampleDemoTypes.Random) && (nType == (uint)BuiltInType.Boolean ||
                nType == (uint)BuiltInType.Byte ||
                nType == (uint)BuiltInType.Double ||
                nType == (uint)BuiltInType.Float ||
                nType == (uint)BuiltInType.Int16 ||
                nType == (uint)BuiltInType.Int32 ||
                nType == (uint)BuiltInType.Int64 ||
                nType == (uint)BuiltInType.Integer ||
                nType == (uint)BuiltInType.SByte ||
                nType == (uint)BuiltInType.UInt16 ||
                nType == (uint)BuiltInType.UInt32 ||
                nType == (uint)BuiltInType.UInt64 ||
                nType == (uint)BuiltInType.UInteger ||
                    nType == (uint)BuiltInType.String
                ))
                    return true;

                if ((DemoType == ExampleDemoTypes.Cos || DemoType == ExampleDemoTypes.Sin || DemoType == ExampleDemoTypes.Ramp) && 
                    (nType == (uint)BuiltInType.Byte ||
                nType == (uint)BuiltInType.Double ||
                nType == (uint)BuiltInType.Float ||
                nType == (uint)BuiltInType.Int16 ||
                nType == (uint)BuiltInType.Int32 ||
                nType == (uint)BuiltInType.Int64 ||
                nType == (uint)BuiltInType.Integer ||
                nType == (uint)BuiltInType.SByte ||
                nType == (uint)BuiltInType.UInt16 ||
                nType == (uint)BuiltInType.UInt32 ||
                nType == (uint)BuiltInType.UInt64 ||
                nType == (uint)BuiltInType.UInteger))
                    return true;
            }

            return false;
        }
        #endregion

        #region Methods
        private void CheckJobValid()
        {
            _SamplingInterval = 0;
            foreach (var d in TagsList)
            {
                if (!IsTypeAdmitted(d.TagNode.DataType) && d.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.InvalidTagType, d.TagNode.NodeId.ToString(), DemoType);
                    return;
                }
            }
        }

        public override uint GetMaxJobSize()
        {
            return 100;
        }
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;//steve 280711
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new ExampleDemoTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void StartSimulation()
        {
            _SimulationRunning = true;
        }
        public void StopSimulation()
        {
            _SimulationRunning = false;
        }

        #endregion
        #region Member
        public double? CommonVal = null;
        public DateTime lastTrigoTime ;
        #endregion

        #region Properties
        private ExampleDemoTypes _DemoType;
        public ExampleDemoTypes DemoType
        {
            get { return _DemoType; }
            set
            {
                _DemoType = value;
            }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}DT{1:00}", ret, (uint)DemoType);
            }
        }

        private bool _SimulationRunning = true;
        public bool SimulationRunning
        {
            get { return _SimulationRunning; }
            set
            {
                _SimulationRunning = value;
            }
        }

        private bool _BeenExecuted = false;
        public bool BeenExecuted
        {
            get { return _BeenExecuted; }
            set
            {
                _BeenExecuted = value;
            }
        }
        private int _SimulationInterval = -1;
        public int SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value; }
        }
        #endregion

    }
}
