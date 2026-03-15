using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OpcClientDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OpcClientDriverChannelSettings : ChannelSettings
    {
        #region Constructors

        public OpcClientDriverChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(OpcClientDriverChannelSettings));
        }

        private OpcClientDriverChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        public void CopyProperties(OpcClientDriverChannelSettings ch)
        {
            base.CopyProperties(ch);
            HostName = ch.HostName;
        }
        /////////////////////////////

        #region Properties
        private string _HostName;
        public string HostName
        {
            get { return _HostName; }
            set
            {
                SetPropertyValue("Name", ref _HostName, value);
            }
        }
        #endregion


        #region IDataErrorInfo Members
        
        #endregion

    }
}
