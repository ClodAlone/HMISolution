using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text;
using System.Globalization;
using UFUAModel.Extensions;

namespace Demo
{
    public class DemoCommJob : CommJob
    {        
        #region Constructors
        public DemoCommJob(Station station, DemoCommJobSettings settings)
            : base(station, settings)
        {
            _SimulationInterval = settings.SimulationInterval;
            _DemoType = settings.DemoType;
            _Direction = DemoProtocol.Direction.Undefined;
            _MinValue = settings.MinValue;
            _MaxValue = settings.MaxValue;
            _DeltaValue = settings.DeltaValue;            
            _NrCycles = settings.NrCycles;
            _FactorSinCos = settings.FactorSinCos;
            _CommonVal = null;
            CheckJobValid();
            SetInitialValue();
        }

        public DemoCommJob(Station station, DemoTag defTag)
            : base(station, defTag)
        {
            // if not set, use station simulation interval value
            if (defTag.DemoDynSettings.SimulationInterval == 0)
                _SimulationInterval = ((DemoStation)station).SimulationInterval;
            else
                _SimulationInterval = defTag.DemoDynSettings.SimulationInterval;
            _DemoType = (DemoProtocol.DemoTypes)(defTag.DemoDynSettings.DemoType);
            _Direction = DemoProtocol.Direction.Undefined;
            _MinValue = defTag.DemoDynSettings.MinValue;
            _MaxValue = defTag.DemoDynSettings.MaxValue;
            _DeltaValue = defTag.DemoDynSettings.DeltaValue;
            _NrCycles = defTag.DemoDynSettings.NrCycles;
            _FactorSinCos = defTag.DemoDynSettings.FactorSinCos;
            _CommonVal = null;
            CheckJobValid();
            SetInitialValue();
        }

        public DemoCommJob(Station station)
            : base(station)
        {
            _SimulationInterval = DemoProtocol.SIMULATION_INTERVAL_DEFAULT_VALUE;
            CheckJobValid();
        }

        protected DemoCommJob()
        {
            _SimulationInterval = DemoProtocol.SIMULATION_INTERVAL_DEFAULT_VALUE;
            CheckJobValid();
        }
        #endregion

        #region Members
        private double? _CommonVal;
        private uint _NrCyclesExecuted;
        private uint _NrCyclesPartital;
        #endregion

        #region override Methods
        public override uint GetMaxJobSize()
        {
            return 0;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {

        }
       
        public override bool SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            bool inUse = base.SetInUse(tagnodeid, bInUse, samplinginterval);
            StartStopSampling(inUse);
            return inUse;
        }

        public override bool IsJobAggregable()
        {
            return false;
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {

            List<Tag> listJobData = jobData as List<Tag>;
            if (listJobData as List<Tag> == null)
                return;

            lock (lockListObject)
            {
                changed.AddRange(listJobData);

                FirstTime = false;
            }
        }
        #endregion

        #region Methods     

        public void StartStopSampling(bool inUse)
        {          
            if (inUse)
            {
                // create a date time variable without msec
                LastExecutionTime = DemoProtocol.GetDateTimeUtcNowNoMSec(1);
            }            
        }

        public bool GetInUseState()
        {
            // reset data value buffer all time Job is SubScribed or change InUse state
            lock (lockListObject)
            {
                if (TagsList.Count == 0)
                    return false;
                else
                    return TagsList[0].InUse;
            }
        }

        internal bool OnWriteTagInternal(NodeId tagnodeid, ref object value)
        {
            Tag t = TagsList.Find(tag => tag.TagNode.NodeId == tagnodeid);
            if (t == null)
                return false;

            try
            {
                Array arr = value as Array;
                if (arr != null)
                {
                    if (arr.Length > 0)
                        _CommonVal = (Double)DataTypeExtensions.ChangeType(arr.GetValue(0), UFUAModel.DataType.Double);
                }
                else
                {
                    _CommonVal = (Double)DataTypeExtensions.ChangeType(value, UFUAModel.DataType.Double);
                }
            } catch (Exception ex)
            {

            }

            UpdateJobTag(t, _CommonVal);

            return true;
        }

        private bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (nType == (uint)BuiltInType.Boolean ||
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
                )
                    return true;
                return false;
            }

            return false;
        }

        private void CheckJobValid()
        {
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
            }

            // don't check job associated to a structure 
            if (TagsList.Count == 1)
            {
                // verify that all parameter are in the compatible with tag data type
                if (!DemoProtocol.AreParametersInRange((uint)TagsList[0].TagNode.DataType.Identifier, (int)_DemoType, _MinValue, _MaxValue, _DeltaValue, _FactorSinCos))
                {
                    IsValid = false;
                    InvalidReason = Properties.Resources.ErrorSimulationParametersOutOfDataTagTypeRange;
                    return;
                }
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public DateTime GetNextScheduleTime()
        {
            return LastExecutionTime.AddMilliseconds(_SimulationInterval);
        }

        private void SetInitialValue()
        {
            if (TagsList[0].TagNode.InitialValue != null)
            {
                if (TagsList[0].TagNode.ArrayDimension > 0)
                {
                    Array arr = TagsList[0].TagNode.InitialValue as Array;

                    if (Double.TryParse(arr.GetValue(0).ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double dummy))
                        _CommonVal = dummy;
                }
                else
                {
                    if (Double.TryParse(TagsList[0].TagNode.InitialValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double dummy))
                        _CommonVal = dummy;
                }
            }
            else
            {
                _CommonVal = _MinValue;
            }

            switch (_DemoType)
            {
                case DemoProtocol.DemoTypes.Sin:
                case DemoProtocol.DemoTypes.Cos:
                    if (_DeltaValue == 0)
                        _DeltaValue = DemoProtocol.SIN_NR_STEPS_PER_CYCLE;
                    break;

                case DemoProtocol.DemoTypes.Ramp:
                case DemoProtocol.DemoTypes.SquareWave:
                case DemoProtocol.DemoTypes.UpDownCounter:
                    if (_DeltaValue == 0)
                        _DeltaValue = 1;
                    
                    if (_MinValue < _MaxValue)
                        _Direction = DemoProtocol.Direction.Up;
                    else
                        _Direction = DemoProtocol.Direction.Down;
                    break;
                case DemoProtocol.DemoTypes.Random:
                    break;
            }

            if (!_CommonVal.HasValue)
                _CommonVal = 0;

            _NrCyclesExecuted = 0;
            _NrCyclesPartital = 0;
        }


        private void UpdateAllJobTags(Double? value)
        {
            foreach (Tag tag in TagsList)
                UpdateJobTag(tag, value);
        }

        private void UpdateJobTag(Tag tag, Double? value)
        {
            if (tag.TagNode.ArrayDimension > 0)
            {
                tag.Value.Value = DemoProtocol.CreateArrayFromBuiltInType((uint)tag.TagNode.DataType.Identifier, tag.TagNode.ArrayDimension);
                Array arr = tag.Value.Value as Array;
                for (int i = 0; i < tag.TagNode.ArrayDimension; i++)
                {
                    if ((uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.BuiltInType.String)
                        arr.SetValue(value.ToString(), i);
                    else
                        arr.SetValue(DataTypeExtensions.ChangeType(value, DemoProtocol.GetUFUAModelFromBuiltInType((uint)tag.TagNode.DataType.Identifier)), i);
                }
            }
            else
            {
                if ((uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.BuiltInType.String)
                    tag.Value.Value = value.ToString();
                else
                    tag.Value.Value = DataTypeExtensions.ChangeType(value, DemoProtocol.GetUFUAModelFromBuiltInType((uint)tag.TagNode.DataType.Identifier));
            }            
        }

        private void UpdateValueRamp()
        {
            switch (_Direction)
            {
                case DemoProtocol.Direction.Up:
                    if (_CommonVal >= _MaxValue)
                        _CommonVal = _MinValue;

                    _CommonVal += _DeltaValue;
                    if (_CommonVal >= _MaxValue)
                    {
                        _CommonVal = _MaxValue;
                        _NrCyclesExecuted++;
                    }
                    break;

                case DemoProtocol.Direction.Down:
                    if (_CommonVal <= _MaxValue)
                        _CommonVal = _MinValue;

                    _CommonVal -= _DeltaValue;
                    if (_CommonVal <= _MinValue)
                    {
                        _CommonVal = _MinValue;
                        _NrCyclesExecuted++;
                    }
                    break;
            }

            UpdateAllJobTags(_CommonVal);            
        }

        private void UpdateValueUpDownCounter()
        {            
            switch (_Direction)
            {
                case DemoProtocol.Direction.Up:
                    _CommonVal += _DeltaValue;
                    if (_CommonVal >= _MaxValue)
                    {
                        _CommonVal = _MaxValue;
                        _Direction = DemoProtocol.Direction.Down;
                        _NrCyclesPartital++;
                    }
                    break;

                case DemoProtocol.Direction.Down:
                    _CommonVal -= _DeltaValue;
                    if (_CommonVal <= _MinValue)
                    {
                        _CommonVal = _MinValue;
                        _Direction = DemoProtocol.Direction.Up;
                        _NrCyclesPartital++;
                    }
                    break;
            }

            if (_NrCyclesPartital == 2)
            {
                _NrCyclesPartital = 0;
                _NrCyclesExecuted++;
            }

            UpdateAllJobTags(_CommonVal);
        }

        private void UpdateValueSin()
        {            
            _CommonVal = (_FactorSinCos * Math.Sin(((2 * Math.PI) / _DeltaValue) * _NrCyclesPartital));
            
            if (_NrCyclesPartital == (uint)_DeltaValue)
            {
                _NrCyclesPartital = 0;
                _NrCyclesExecuted++;
            }

            _NrCyclesPartital++;
            
            UpdateAllJobTags(_CommonVal);
        }

        private void UpdateValueCos()
        {            
            _CommonVal = (_FactorSinCos * Math.Cos(((2 * Math.PI) / _DeltaValue) * _NrCyclesPartital));
            
            if (_NrCyclesPartital == (uint)_DeltaValue)
            {
                _NrCyclesPartital = 0;
                _NrCyclesExecuted++;
            }

            _NrCyclesPartital++;

            UpdateAllJobTags(_CommonVal);
        }

        private double GetRndValue(Tag tag)
        {
            Random rndGen = new Random(Guid.NewGuid().GetHashCode());
            if (DemoProtocol.IsTagBoolean(tag))
                return (rndGen.Next() > (Int32.MaxValue / 2) ? _MinValue : _MaxValue);
            else
                return rndGen.NextDouble() * (_MaxValue - _MinValue) + _MinValue;
        }

        private void UpdateValueRnd()
        {
            foreach (Tag tag in TagsList)
            {
                if (tag.TagNode.ArrayDimension > 0)
                {
                    tag.Value.Value = DemoProtocol.CreateArrayFromBuiltInType((uint)tag.TagNode.DataType.Identifier, tag.TagNode.ArrayDimension);
                    Array arr = tag.Value.Value as Array;
                    for (int i = 0; i < tag.TagNode.ArrayDimension; i++)
                    {
                        if ((uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.BuiltInType.String)
                            arr.SetValue(GetRndValue(tag).ToString(), i);
                        else
                            arr.SetValue(DataTypeExtensions.ChangeType(GetRndValue(tag), DemoProtocol.GetUFUAModelFromBuiltInType((uint)tag.TagNode.DataType.Identifier)), i);
                    }
                }
                else
                {
                    if ((uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.BuiltInType.String)
                        tag.Value.Value = GetRndValue(tag).ToString();
                    else
                        tag.Value.Value = DataTypeExtensions.ChangeType(GetRndValue(tag), DemoProtocol.GetUFUAModelFromBuiltInType((uint)tag.TagNode.DataType.Identifier));
                }
            }

            _NrCyclesExecuted++;
        }

        private void UpdateValueSquareWave()
        {            
            switch (_Direction)
            {
                case DemoProtocol.Direction.Up:
                    _CommonVal = _MaxValue;
                    _Direction = DemoProtocol.Direction.Down;
                    _NrCyclesPartital++;
                    break;

                case DemoProtocol.Direction.Down:
                    _CommonVal = _MinValue;
                    _Direction = DemoProtocol.Direction.Up;
                    _NrCyclesPartital++;
                    break;
            }


            if (_NrCyclesPartital == 2)
            {
                _NrCyclesPartital = 0;
                _NrCyclesExecuted++;
            }

            UpdateAllJobTags(_CommonVal);
        }

        public void UpdateValue()
        {
            if (!NrCyclesDone())
            {
                switch (_DemoType)
                {
                    case DemoProtocol.DemoTypes.Sin:
                        UpdateValueSin();
                        break;
                    case DemoProtocol.DemoTypes.Cos:
                        UpdateValueCos();
                        break;
                    case DemoProtocol.DemoTypes.Ramp:
                        UpdateValueRamp();
                        break;
                    case DemoProtocol.DemoTypes.SquareWave:
                        UpdateValueSquareWave();
                        break;
                    case DemoProtocol.DemoTypes.UpDownCounter:
                        UpdateValueUpDownCounter();
                        break;
                    case DemoProtocol.DemoTypes.Random:
                        UpdateValueRnd();
                        break;
                }
            }
        }

        public List<Tag> GetCurrentValues()
        {
            return TagsList;
        }

        private bool NrCyclesDone()
        {
            if (_NrCycles > 0)
                return (_NrCyclesExecuted == _NrCycles);
            else
                return false;
        }

        public void RestartSimulation()
        {
            if (_NrCycles > 0)
            {
                _NrCyclesPartital = 0;
                _NrCyclesExecuted = 0;
                SetInitialValue();
            }
            LastExecutionTime = DemoProtocol.GetDateTimeUtcNowNoMSec(1);
        }
        #endregion

        #region Properties                       
        public override uint SamplingInterval
        {
            get {

                uint interval = CommJob.JOB_NOT_SCHEDULABLE;
                if (((DemoStation)Station).SimulationState == DemoStation.State.Running || ((DemoStation)Station).SimulationState == DemoStation.State.LaunchAtStartUp)
                {
                    if (!NrCyclesDone())
                        return (uint)SimulationInterval;                
                }
                return interval;
            }            
        }        
        
        private DemoProtocol.Direction _Direction;

        private DemoProtocol.DemoTypes _DemoType;
        public DemoProtocol.DemoTypes DemoType
        {
            get { return _DemoType; }
            set { _DemoType = value; }
        }

        private uint _SimulationInterval;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value; }
        }

        private double _MinValue;
        public double MinValue
        {
            get { return _MinValue; }
            set { _MinValue = value; }
        }

        private double _MaxValue;
        public double MaxValue
        {
            get { return _MaxValue; }
            set { _MaxValue = value; }
        }

        private double _DeltaValue;
        public double DeltaValue
        {
            get { return _DeltaValue; }
            set { _DeltaValue = value; }
        }

        private uint _NrCycles;
        public uint NrCycles
        {
            get { return _NrCycles; }
            set { _NrCycles = value; }
        }

        private double _FactorSinCos;
        public double FactorSinCos
        {
            get { return _FactorSinCos; }
            set { _FactorSinCos = value; }
        }
        #endregion
    }
}
