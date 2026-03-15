using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronFinsEthernet
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronFinsEthernetDriverSettings : DriverSettings
    {
                #region Constructors

        public OmronFinsEthernetDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OmronFinsEthernetDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members

        #endregion
    
    }
}
