using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace TwinCAT
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class TwinCATCommJobSettings : CommJobSettings
    {
        #region Constructors

        public TwinCATCommJobSettings(Session session, TwinCATCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
            _Length = job.Length;
        }
        
        public TwinCATCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected TwinCATCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _Address = String.Empty;
            _Length = 0;
        }

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _Address;
        [Size(SizeAttribute.Unlimited)]
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

        /// <summary>
        /// Length (property used for string tags)
        /// </summary>
        private UInt32 _Length;
        public UInt32 Length
        {
            get { return _Length; }
            set { SetPropertyValue("Length", ref _Length, value); }
        }

        #endregion

        #region IDataErrorInfo Members

        #endregion

    }
}
