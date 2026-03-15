using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFXTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXTCPCommJobSettings : CommJobSettings
    {
        #region Constructors

        public MelsecFXTCPCommJobSettings(Session session, MelsecFXTCPCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
        }

        public MelsecFXTCPCommJobSettings(Session session)
            : base(session)
        {
        }

        public MelsecFXTCPCommJobSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            _Address = String.Empty;
        }

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
