using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
namespace ExampleDemo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ExampleDemoStationSettings : StationSettings, IDataErrorInfo
    {
        #region Constructors

        public ExampleDemoStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(ExampleDemoStationSettings));
        }
        protected ExampleDemoStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(ExampleDemoStationSettings st)
        {
            base.CopyProperties(st);

            SimulationInterval = st.SimulationInterval;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region Properties
        /// <summary>
        /// Starting simulation intervall of the station (milliseconds)
        /// </summary>
        private uint _SimulationInterval = 500;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set
            {
                SetPropertyValue("SimulationInterval", ref _SimulationInterval, value);
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

            if (propertyName == "SimulationInterval")
            {
                if (SimulationInterval < uint.MinValue || SimulationInterval > uint.MaxValue)
                    return string.Format(Properties.Resources.ValueOutOfRange, uint.MinValue, uint.MaxValue);
            }

            return null;
        }

        #endregion
    }
}
