using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModBus
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusStationSettings : StationSettings
    {
        #region Constructors

        public ModbusStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            StationID = ((ModbusStationSettings)st).StationID;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
        }

        #region Properties

        /// <summary>
        /// Enter the numeric station address (0..247)
        /// </summary>
        private uint _StationID;
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
