using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Simotion
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SimotionCommJobSettings : CommJobSettings
    {
        #region Constructors

        public SimotionCommJobSettings(Session session, SimotionCommJob job)
            : base(session, job)
        {
            _StringLength = job.StringLength;
            _S7DataFormat = job.S7DataFormat;
            _TiaStartAddress = job.StartAddress;
            //_Trans = job.Trans;
        }

        public SimotionCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected SimotionCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StringLength = 0;
            _TiaStartAddress = "";
            _S7DataFormat = S7DataFormats.Bool;
            //_Trans = Step7WordTrans.wtT;
        }

        #region Properties

        private string _TiaStartAddress;
        [Size(SizeAttribute.Unlimited)]
        public string TiaStartAddress
        {
            get { return _TiaStartAddress; }
            set
            {
                _TiaStartAddress = value;
            }
        }
        private S7DataFormats _S7DataFormat;
        public S7DataFormats S7DataFormat
        {
            get { return _S7DataFormat; }
            set { SetPropertyValue("Format", ref _S7DataFormat, value); }
        }
        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { SetPropertyValue("String Length", ref _StringLength, value); }
        }

        //private Step7WordTrans _Trans;
        //public Step7WordTrans Trans
        //{
        //    get { return _Trans; }
        //    set { SetPropertyValue("Trans", ref _Trans, value); }
        //}
        #endregion


        #region IDataErrorInfo Members

        #endregion
    }
}
