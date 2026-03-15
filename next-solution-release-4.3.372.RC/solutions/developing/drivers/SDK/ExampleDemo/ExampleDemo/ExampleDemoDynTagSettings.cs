using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;

namespace ExampleDemo
{
    public class ExampleDemoDynTagSettings : DynTagSettings, IDataErrorInfo
    {
        #region Constructors

        public ExampleDemoDynTagSettings()
            : base()
        {
            DemoType = (int)ExampleDemoTypes.Sin;
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

            DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)ExampleDemoTypes.Sin));
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // method ?
            if (MethodID != -1)
            {
            }
            else
            {
                // required parameter
                if (String.IsNullOrEmpty(helper.GetPartByName(DemoTypeParameter)))
                    return false;
            }

            // optional parameters
            DemoType = (int)(helper.GetPartByName(DemoTypeParameter, (int)ExampleDemoTypes.Sin));
            
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

        public new string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public new string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DemoType")
            {
                ExampleDemoTypes dt = (ExampleDemoTypes)DemoType;
                if (dt != ExampleDemoTypes.Cos && dt != ExampleDemoTypes.Ramp &&
                    dt != ExampleDemoTypes.Random && dt != ExampleDemoTypes.Sin)
                    return Properties.Resources.InvalidDemoType;


            }

            else if (propertyName == "MethodeID")
            {
                if (MethodID != (int)ExampleDemoMethods.StartSim ||
                    MethodID != (int)ExampleDemoMethods.StopSim ||
                    MethodID != (int)ExampleDemoMethods.IncSimulationTime ||
                    MethodID != (int)ExampleDemoMethods.DecSimulationTime)
                {
                    return Properties.Resources.InvalidMethod;
                }
            }
            return null;
        }

        #endregion
    }
}
