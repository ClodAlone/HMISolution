using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFX
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXCommJobSettings : CommJobSettings
    {
        #region Constructors

        public MelsecFXCommJobSettings(Session session, MelsecFXCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
        }

        public MelsecFXCommJobSettings(Session session)
            : base(session)
        {
        }

        public MelsecFXCommJobSettings()
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
