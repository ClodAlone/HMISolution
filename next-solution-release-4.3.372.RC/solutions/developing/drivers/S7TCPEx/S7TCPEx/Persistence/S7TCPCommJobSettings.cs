using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TCPCommJobSettings : CommJobSettings
    {
        #region Constructors

        public S7TCPCommJobSettings(Session session, S7TCPCommJob job)
            : base(session, job)
        {
            _Area = job.Area;
            _Format = job.Format;
            _Trans = job.Trans;
            _Offset = job.Offset;
            // add byte (contain string length) subtract at runtime to address plc variable
            if (job.LenStringEnable)
            {
                if ((uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.BuiltInType.String)
                    _Offset++;
            }
            _Length = job.Length;
            _DbNumber = job.DbNumber;
            _Bit = job.Bit;
            _German = job.German;
            _S7_200 = job.S7_200;
            _LenStringEnable = job.LenStringEnable;
            _SwapDWords = job.SwapDWords;
        }

        public S7TCPCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected S7TCPCommJobSettings()
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
            _S7_200 = false;
            _LenStringEnable = false;
            _SwapDWords = false;
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
        private bool _S7_200;
        public bool S7_200
        {
            get { return _S7_200; }
            set { SetPropertyValue("S7_200", ref _S7_200, value); }
        }
        private bool _LenStringEnable;
        public bool LenStringEnable
        {
            get { return _LenStringEnable; }
            set { SetPropertyValue("LenStringEnable", ref _LenStringEnable, value); }
        }
        private bool _SwapDWords;
        public bool SwapDWords
        {
            get { return _SwapDWords; }
            set
            {
                SetPropertyValue("SwapDWords", ref _SwapDWords, value);
            }
        }
        #endregion


        #region IDataErrorInfo Members

        #endregion
    }
}
