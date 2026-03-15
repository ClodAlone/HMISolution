using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronFinsEthernet
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronFinsEthernetStationSettings : StationSettings
    {
                #region Constructors

        public OmronFinsEthernetStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OmronFinsEthernetStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(OmronFinsEthernetStationSettings st)
        {
            base.CopyProperties(st);
            _DestinationNetworkAddress = st.DestinationNetworkAddress;
            _DestinationNode = st.DestinationNode;
            _DestinationUnit = st.DestinationUnit;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _DestinationNetworkAddress = 0;
            _DestinationNode = 1;
            _DestinationUnit = 0;
        }

        #region Properties

        /// <summary>
        /// Enter the Destination Network Address(0...127 )
        /// </summary>
        private byte _DestinationNetworkAddress;
        public byte DestinationNetworkAddress
        {
            get
            {
                return _DestinationNetworkAddress;
            }
            set
            {
                SetPropertyValue("DestinationNetworkAddress", ref _DestinationNetworkAddress, value);
            }
        }

        /// <summary>
        /// Enter the Destination Node Number(0...254 )
        /// </summary>
        private byte _DestinationNode;
        public byte DestinationNode
        {
            get
            {
                return _DestinationNode;
            }
            set
            {
                SetPropertyValue("DestinationNode", ref _DestinationNode, value);
            }
        }

        /// <summary>
        /// Destination Unit Number(0...255 )
        /// </summary>
        private byte _DestinationUnit;
        public byte DestinationUnit
        {
            get
            {
                return _DestinationUnit;
            }
            set
            {
                SetPropertyValue("DestinationUnit", ref _DestinationUnit, value);
            }
        }

        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DestinationNetworkAddress")
            {
                if (_DestinationNetworkAddress < 0 || _DestinationNetworkAddress > 127)
                    return Properties.Resources.NetworkAddressOutOfRange;

            }
            if (propertyName == "DestinationNode")
            {
                if (_DestinationNode < 0 || _DestinationNode > 254)
                    return Properties.Resources.NodeOutOfRange;

            }

            if (propertyName == "DestinationUnit")
            {
                if (_DestinationUnit < 0 || _DestinationUnit > 255)
                    return Properties.Resources.UnitOutOfRange;

            }


            return null;
        }

        #endregion

    }
}
