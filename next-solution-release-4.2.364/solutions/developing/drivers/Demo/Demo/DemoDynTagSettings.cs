using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;

namespace Demo
{
    public class DemoDynTagSettings : DynTagSettings
    {
        #region Constructors

        public DemoDynTagSettings()
            : base()
        {
            DemoType = (int)DemoTypes.Sin;

            IsMethodSupported = true;
        }

        #endregion

        #region Static Members

        private static readonly String DemoTypeParameter = "DT";
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)DemoTypes.Sin));
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(DemoTypeParameter)))
                return false;

            // optional parameters
            DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)DemoTypes.Sin));
            
            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DemoTypeParameter, DynamicStringParser.CharAssign, (int)DemoType);
            
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
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DemoType")
            {
                DemoTypes dt = (DemoTypes)DemoType;
                if (dt != DemoTypes.Cos && dt != DemoTypes.Ramp &&
                    dt != DemoTypes.Random && dt != DemoTypes.Sin)
                    return Properties.Resources.InvalidDemoType;


            }

            else if (propertyName == "MethodeID")
            {
                if (MethodID != (int)DemoMethods.StartSim ||
                    MethodID != (int)DemoMethods.StopSim ||
                    MethodID != (int)DemoMethods.IncSimulationTime ||
                    MethodID != (int)DemoMethods.DecSimulationTime)
                {
                    return Properties.Resources.InvalidMethod;
                }
            }
            return null;
        }

        #endregion
    }
}
