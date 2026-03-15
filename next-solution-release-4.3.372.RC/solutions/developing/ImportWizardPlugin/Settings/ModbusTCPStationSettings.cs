using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBase;

namespace ImportWizardPlugin.Settings
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPStationSettings : StationSettings
    {
        #region Constructors

        public ModbusTCPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(StationSettings));
        }
        protected ModbusTCPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        //steve 080711
        public void CopyProperties(ModbusTCPStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
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

    }
}
