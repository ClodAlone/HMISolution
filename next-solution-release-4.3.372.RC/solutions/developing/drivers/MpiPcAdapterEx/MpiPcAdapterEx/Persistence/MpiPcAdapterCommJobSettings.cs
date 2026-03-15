using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MpiPcAdapter
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MpiPcAdapterCommJobSettings : CommJobSettings
    {
        #region Constructors

        public MpiPcAdapterCommJobSettings(Session session, MpiPcAdapterCommJob job)
            : base(session, job)
        {
            _Area = job.Area;
            _Format = job.Format;
            _Trans = job.Trans;
            _Offset = job.Offset;
            _Length = job.Length;
            _DbNumber = job.DbNumber;
            _Bit = job.Bit;
            _German = job.German;
        }
        
        public MpiPcAdapterCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected MpiPcAdapterCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _Area = Step7Area.aP;
            _Format = Step7Format.frmBit;
            _Trans = Step7WordTrans.wtW;
            _Offset = 0;
            _Length = 1;
            _DbNumber = 0;
            _Bit = 0;
            _German = false;
        }

        #region Properties
        private Step7Area _Area;
        public Step7Area Area
        {
            get { return _Area; }
            set
            {
                SetPropertyValue("Area", ref _Area, value);
            }
        }

        private Step7Format _Format;
        public Step7Format Format
        {
            get { return _Format; }
            set { SetPropertyValue("Format", ref _Format, value); }
        }
        private Step7WordTrans _Trans;
        public Step7WordTrans Trans
        {
            get { return _Trans; }
            set { SetPropertyValue("Trans", ref _Trans, value); }
        }
        private int _Offset;
        public int Offset
        {
            get { return _Offset; }
            set { SetPropertyValue("Offset", ref _Offset, value); }
        }
        private int _Length;
        public int Length
        {
            get { return _Length; }
            set { SetPropertyValue("Length", ref _Length, value); }
        }
        private int _DbNumber;
        public int DbNumber
        {
            get { return _DbNumber; }
            set { SetPropertyValue("DbNumber", ref _DbNumber, value); }
        }
        private int _Bit;
        public int Bit
        {
            get { return _Bit; }
            set { SetPropertyValue("Bit", ref _Bit, value); }
        }
        private bool _German;
        public bool German
        {
            get { return _German; }
            set { SetPropertyValue("German", ref _German, value); }
        }
        #endregion


        #region IDataErrorInfo Members
        #endregion
    }
}
