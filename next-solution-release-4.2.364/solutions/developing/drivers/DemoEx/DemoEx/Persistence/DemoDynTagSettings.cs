using System;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;


namespace Demo
{
    public sealed class DemoDynTagSettings : DynTagSettings
    {
        #region Constructors
        public DemoDynTagSettings()
            : base()
        {            
            TagLinkType = (int)LinkType.Input;
            DemoType = (int)DemoProtocol.DemoTypes.Sin;
            MinValue = 0;
            MaxValue = 100;
            DeltaValue = DemoProtocol.SIN_NR_STEPS_PER_CYCLE;
            SimulationInterval = 0;
            NrCycles = 0;
            FactorSinCos = 1;
            IsMethodSupported = true;
        }

        #endregion

        #region Static Members

        private static readonly String DemoTypeParameter = "DT";
        private static readonly String DeltaValueParameter = "DV";
        private static readonly String SimulationIntervalParameter = "SI";
        private static readonly String MinValueParameter = "MIV";
        private static readonly String MaxValueParameter = "MAV";
        private static readonly String NrCyclesParameter = "NC";
        private static readonly String FactorSinCosParameter = "FSC";

        #endregion

        #region Override Functions
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            // method ?
            if (MethodID != -1)
            {
            }
            else
            {
                DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

                _DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)DemoProtocol.DemoTypes.Sin));
                _MinValue = (Double)helper.GetPartByName(MinValueParameter, 0);
                _MaxValue = (Double)helper.GetPartByName(MaxValueParameter, 100);
                _DeltaValue = (Double)helper.GetPartByName(DeltaValueParameter, 0);
                if (_DeltaValue == 0)
                {
                    if (_DemoType == (int)DemoProtocol.DemoTypes.Sin || _DemoType == (int)DemoProtocol.DemoTypes.Cos)
                        _DeltaValue = DemoProtocol.SIN_NR_STEPS_PER_CYCLE;
                    else
                        _DeltaValue = 1;
                }
                _SimulationInterval = (UInt32)helper.GetPartByName(SimulationIntervalParameter, 0);
                _NrCycles = (UInt32)helper.GetPartByName(NrCyclesParameter, 0);
                _FactorSinCos = (Double)helper.GetPartByName(FactorSinCosParameter, 1);
            }
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(DemoTypeParameter)))
                return false;

            // method ?
            if (MethodID != -1)
            {
            }
            else
            { 
                // optional parameters
                _DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)DemoProtocol.DemoTypes.Sin));
                _MinValue = (Double)helper.GetPartByName(MinValueParameter, (Double)0);
                _MaxValue = (Double)helper.GetPartByName(MaxValueParameter, (Double)100);
                _DeltaValue = (Double)helper.GetPartByName(DeltaValueParameter, (Double)0);
                if (_DeltaValue == 0)
                {
                    if (_DemoType == (int)DemoProtocol.DemoTypes.Sin || _DemoType == (int)DemoProtocol.DemoTypes.Cos)
                        _DeltaValue = DemoProtocol.SIN_NR_STEPS_PER_CYCLE;
                    else
                        _DeltaValue = 1;
                }
                _SimulationInterval = (uint)helper.GetPartByName(SimulationIntervalParameter, 0);
                _NrCycles = (UInt32)helper.GetPartByName(NrCyclesParameter, 0);
                _FactorSinCos = (Double)helper.GetPartByName(FactorSinCosParameter, (Double)1);
            }

            // optional parameters
            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            // method ?
            if (MethodID != -1)
            {
            }
            else
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", DemoTypeParameter, DynamicStringParser.CharAssign, (int)DemoType);
                dynamicstring.AppendFormat("|{0}{1}{2}", MinValueParameter, DynamicStringParser.CharAssign, MinValue);
                dynamicstring.AppendFormat("|{0}{1}{2}", MaxValueParameter, DynamicStringParser.CharAssign, MaxValue);
                dynamicstring.AppendFormat("|{0}{1}{2}", DeltaValueParameter, DynamicStringParser.CharAssign, DeltaValue);
                dynamicstring.AppendFormat("|{0}{1}{2}", SimulationIntervalParameter, DynamicStringParser.CharAssign, SimulationInterval);
                dynamicstring.AppendFormat("|{0}{1}{2}", NrCyclesParameter, DynamicStringParser.CharAssign, NrCycles);
                dynamicstring.AppendFormat("|{0}{1}{2}", FactorSinCosParameter, DynamicStringParser.CharAssign, FactorSinCos);
            }
            return dynamicstring.ToString();
        }       
        #endregion

        #region Properties      
        private int _DemoType;
        [Category("Device Data")]
        [Description("Simulation Type")]
        public int DemoType
        {
            get { return _DemoType; }
            set
            { 
                _DemoType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FactorSinCor"));
                OnPropertyChanged(new PropertyChangedEventArgs("MinValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("MaxValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaValue"));
            }
        }

        private Double _MinValue;
        [Category("Device Data")]
        [Description("Min Value")]        
        public Double MinValue
        {
            get { return _MinValue; }
            set
            { 
                _MinValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DemoType"));
                OnPropertyChanged(new PropertyChangedEventArgs("MaxValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaValue"));
            }
        }

        private Double _MaxValue;
        [Category("Device Data")]
        [Description("Max Value")]
        public Double MaxValue
        {
            get { return _MaxValue; }
            set 
            { 
                _MaxValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DemoType"));
                OnPropertyChanged(new PropertyChangedEventArgs("MinValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaValue"));
            }
        }

        private Double _DeltaValue;
        [Category("Device Data")]
        [Description("Delta Value")]
        public Double DeltaValue
        {
            get { return _DeltaValue; }
            set 
            {
                _DeltaValue = value;                
                OnPropertyChanged(new PropertyChangedEventArgs("DemoType"));
                OnPropertyChanged(new PropertyChangedEventArgs("MinValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("MaxValue"));
            }
        }

        /// <summary>
        /// Starting simulation intervall of the station (milliseconds)
        /// </summary>
        private uint _SimulationInterval;
        [Category("Device Data")]
        [Description("Simulation Interval")]        
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value; }
        }

        private uint _NrCycles;
        [Category("Device Data")]
        [Description("Nr Cycles")]
        public uint NrCycles
        {
            get { return _NrCycles; }
            set { _NrCycles = value; }
        }

        private double _FactorSinCos;
        [Category("Device Data")]
        [Description("FactorSinCos")]
        public double FactorSinCos
        {
            get { return _FactorSinCos; }
            set { 
                _FactorSinCos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeltaValue"));
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "MethodeID":
                    if (MethodID != (int)DemoDriver.DemoMethods.StartSim || MethodID != (int)DemoDriver.DemoMethods.StopSim)                        
                        return Properties.Resources.InvalidMethod;
                    break;             
                case "DemoType":
                    if (!DemoProtocol.IsDemoTypeCompatibleWithVarType(VarType, _DemoType))
                        return string.Format(Properties.Resources.ErrorTagTypeNotCompatibleWithDemoType, ((DemoProtocol.DemoTypes)_DemoType).ToString());
                    break;
                case "FactorSinCos":
                    if (MethodID == -1)
                    {
                        if (!DemoProtocol.AreParametersInRange(VarType, _DemoType, _MinValue, _MaxValue, _DeltaValue, _FactorSinCos))
                            return Properties.Resources.ErrorSimulationParametersOutOfDataTagTypeRange;                        
                    }
                    break;
                case "MinValue":
                    if (MethodID == -1)
                    {
                        if (!DemoProtocol.AreParametersInRange(VarType, _DemoType, _MinValue, _MaxValue, _DeltaValue, _FactorSinCos))
                            return Properties.Resources.ErrorSimulationParametersOutOfDataTagTypeRange;

                        if (_MinValue == _MaxValue)
                            return Properties.Resources.ErrorMinValueEqualMaxValue;
                    }
                    break;
                case "MaxValue":
                    if (MethodID == -1)
                    {
                        if (!DemoProtocol.AreParametersInRange(VarType, _DemoType, _MinValue, _MaxValue, _DeltaValue, _FactorSinCos))
                            return Properties.Resources.ErrorSimulationParametersOutOfDataTagTypeRange;

                        if (_MinValue == _MaxValue)
                            return Properties.Resources.ErrorMinValueEqualMaxValue;
                    }
                    break;
                case "DeltaValue":
                    if (MethodID == -1)
                    {
                        if (!DemoProtocol.AreParametersInRange(VarType, _DemoType, _MinValue, _MaxValue, _DeltaValue, _FactorSinCos))
                            return Properties.Resources.ErrorSimulationParametersOutOfDataTagTypeRange;

                        switch ((DemoProtocol.DemoTypes)_DemoType)
                        {
                            case DemoProtocol.DemoTypes.Sin:
                            case DemoProtocol.DemoTypes.Cos:
                                if (_DeltaValue <= 0)
                                    return Properties.Resources.ErrorNrStepsForCycleInvalid;

                                if (_DeltaValue % 1 != 0)
                                    return Properties.Resources.ErrorNrStepsForCycleInvalid;

                                break;

                            case DemoProtocol.DemoTypes.Ramp:
                            case DemoProtocol.DemoTypes.UpDownCounter:
                                if (_DeltaValue == 0)
                                    return Properties.Resources.ErrorDeltaValueIs0;
                                if (_DeltaValue < 0)
                                    return Properties.Resources.ErrorDeltaValueIsNegativeValue;

                                double range = 0;
                                if (_MinValue < 0)
                                    range += _MinValue;
                                else
                                    range -= _MinValue;

                                if (_MaxValue < 0)
                                    range += _MaxValue;
                                else
                                    range -= _MaxValue;

                                range = Math.Abs(range);

                                if (_DeltaValue > range)
                                    return Properties.Resources.ErrorDeltaValueExceedsRange;

                                break;
                        }
                    }
                    break;
                case "SimulationInterval":
                    if (_SimulationInterval < 0)
                        return Properties.Resources.ErrorSimulationIntervalInvalid;
                    break;
                case "NrCycles":                    
                    break;
            }

            return null;
        }

        #endregion
        
    }
}
