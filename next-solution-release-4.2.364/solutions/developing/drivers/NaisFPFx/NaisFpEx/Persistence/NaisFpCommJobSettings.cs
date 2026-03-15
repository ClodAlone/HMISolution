using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace NaisFp
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class NaisFpCommJobSettings : CommJobSettings
    {
                #region Constructors

        public NaisFpCommJobSettings(Session session, NaisFpCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
        }
        
        public NaisFpCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected NaisFpCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _Address = String.Empty;
        }

        #region Properties

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

 
  
        #endregion


        #region IDataErrorInfo Members

        #endregion
    
    }
}
