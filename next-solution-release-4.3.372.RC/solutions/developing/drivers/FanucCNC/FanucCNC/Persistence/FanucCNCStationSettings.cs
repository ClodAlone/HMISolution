using System;
using DriverCodeBase;
using DevExpress.Xpo;

namespace FanucCNC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FanucCNCStationSettings : StationSettings
    {
        #region Constructors

        public FanucCNCStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FanucCNCStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void CopyProperties(FanucCNCStationSettings st)
        {
            base.CopyProperties(st);
            //_CNCPath = st.CNCPath;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_CNCPath = 0;
        }

        #region Properties        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Change the channel. </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////      
        //protected override void OnChanged(string propertyName, object oldValue, object newValue)
        //{
        //    if ((!string.IsNullOrEmpty(propertyName)) && (propertyName == "Channel"))
        //    {
        //        //RaisePropertyChangedEvent("CPUSlot");
        //        //RaisePropertyChangedEvent("PlcType");
        //    }
        //    base.OnChanged(propertyName, oldValue, newValue);
        //}

        #endregion

        #region IDataErrorInfo Members

        //protected override String PerformValidation(String propertyName)
        //{
        //    string sBase = base.PerformValidation(propertyName);
        //    if (sBase != null)
        //        return sBase;

        //    //switch (propertyName) {
        //    //    case "CNCPath":
        //    //        if (_CNCPath < 0)
        //    //            return string.Format(Properties.Resources.ErrorCNCPathOutOfRange, uint.MaxValue);
        //    //        break;
        //    //}
        //    return null;
        //}

        #endregion

        #region Properties        
        ///// <summary>
        ///// Specificy the CNC path
        ///// </summary>
        //private short _CNCPath;
        //public short CNCPath
        //{
        //    get { return _CNCPath; }
        //    set { SetPropertyValue("CNCPath", ref _CNCPath, value); }
        //}
        #endregion
    }
}
