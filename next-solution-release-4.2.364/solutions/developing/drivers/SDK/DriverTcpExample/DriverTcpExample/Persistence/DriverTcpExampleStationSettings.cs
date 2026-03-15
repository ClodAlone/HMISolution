////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DriverTcpExampleStationSettings.cs
//
// summary:	Implements the driver TCP example station settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverTcpExample
{
    /// <summary>   Settings for the drivers's station(DriverTcpExampleStation). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverTcpExampleStationSettings : StationSettings, IDataErrorInfo
    {
                #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(StationSettings));
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected DriverTcpExampleStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from DriverTcpExampleStationSettings "st". </summary>
        ///
        /// <param name="st">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(DriverTcpExampleStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
        }

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
        }

        #region Properties

        /// <summary>   Enter the numeric station address (0..247). </summary>
        private uint _StationID;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier of the station. </summary>
        ///
        /// <value> The identifier of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                SetPropertyValue("StationID", ref _StationID, value);
            }
        }
        
        #endregion



        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                var value = GetType().GetProperty(propertyName).GetValue(this, null);

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverTcpExampleStationSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StationID")
            {
                if(StationID < 0 || StationID > 247)
                    return Properties.Resources.StationIDOutOfRange;

            }

            return null;
        }

        #endregion

    }
}
