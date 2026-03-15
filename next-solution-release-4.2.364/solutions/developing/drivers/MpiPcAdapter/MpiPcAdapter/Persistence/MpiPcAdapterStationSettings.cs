using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MpiPcAdapter
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MpiPcAdapterStationSettings : StationSettings
    {
        #region Constructors

        public MpiPcAdapterStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MpiPcAdapterStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(MpiPcAdapterStationSettings st)
        {
            base.CopyProperties(st);
            _DeviceID = st._DeviceID;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _DeviceID = 1;
        }

        #region Properties
        private byte _DeviceID;
        public byte DeviceID
        {
            get { return _DeviceID; }
            set { SetPropertyValue("DeviceID", ref _DeviceID, value); ; }
        }
        #endregion

        #region IDataErrorInfo Members

        //protected override String PerformValidation(String propertyName)
        //{
        //    string sBase = base.PerformValidation(propertyName);
        //    if (sBase != null)
        //        return sBase;

        //    if (propertyName == "DeviceID")
        //    {
        //        //if(DeviceID > 0xff)
        //    }

        //    return null;
        //}

        #endregion

    }
}
