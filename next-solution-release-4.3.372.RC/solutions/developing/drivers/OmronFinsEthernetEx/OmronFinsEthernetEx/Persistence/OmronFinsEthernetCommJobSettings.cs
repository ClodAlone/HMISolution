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
    public class OmronFinsEthernetCommJobSettings : CommJobSettings
    {
                #region Constructors

        public OmronFinsEthernetCommJobSettings(Session session, OmronFinsEthernetCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
            _DataConversionType = job.DataConversionType;
        }
        
        public OmronFinsEthernetCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected OmronFinsEthernetCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _Address = String.Empty;
            _DataConversionType = DataConversionTypes.None;
        }

        #region Properties

        private string _Address;
        public string Address
        {
            get { return _Address;   }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        private DataConversionTypes _DataConversionType;
        public DataConversionTypes DataConversionType
        {
            get { return _DataConversionType; }
            set { SetPropertyValue("DataConversionType", ref _DataConversionType, value); }
        }

        
        #endregion


        #region IDataErrorInfo Members

        #endregion
    
    }
}
