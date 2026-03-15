using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;
using DevExpress.Utils;

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

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            HostName = ((OpcClientDriverChannelSettings)ch).HostName;
            BackupHostList = ((OpcClientDriverChannelSettings)ch).BackupHostList;
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

        [Size(SizeAttribute.Unlimited)]
        private string _BackupHostList;
        public string BackupHostList
        {
            get { return _BackupHostList; }
            set
            {
                SetPropertyValue("BackupHostList", ref _BackupHostList, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "BackupHostList":
                    if (!string.IsNullOrEmpty(_BackupHostList))
                    {
                        if (OpcClientDriverProtocol.GetValidBackupHostList(_BackupHostList) == null)
                            return Properties.Resources.ErrorInvalidBackupHostList;
                    }
                    break;
            }
            
            return null;
        }

        #endregion
    }
}
