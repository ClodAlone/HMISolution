using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronEthernetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronEthernetIPCommJobSettings : CommJobSettings
    {
        #region Constructors

        public OmronEthernetIPCommJobSettings(Session session, OmronEthernetIPCommJob job)
            : base(session, job)
        {
            //_AddressType = job.AddressType;
            _TagFormat = job.TagFormat;
            _ABAddress = job.ABAddress;

            _ParseOk = job.ParseOk;
            _DataFormat = job.DataFormat;
            _ElemSize = job.ElemSize;

            //Symbolic & Physical Address
            _SubElement = job.SubElement;

            //Symbolic Address
            _TagName = job.TagName;
            _ModuleName = job.ModuleName;
            _Dim0 = job.Dim0;
            _Dim1 = job.Dim1;
            _Dim2 = job.Dim2;

            //Physical Address
            _FileType = job.FileType;
            _FileNum = job.FileNum;
            _Slot = job.Slot;
            _Word = job.Word;
            _Element = job.Element;
            _Bit = job.Bit;

        }

        public OmronEthernetIPCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected OmronEthernetIPCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TagFormat = TagFormats.BOOL;
            _ABAddress = string.Empty;

            _ParseOk = false;
            _DataFormat = DataFormats.INVALID;
            _ElemSize = 1;

            _SubElement = SubElements.Invalid;

            _TagName = string.Empty;
            _ModuleName = string.Empty;
            _Dim0 = 1;
            _Dim1 = 1;
            _Dim2 = 1;

            _FileType = FileTypes.Invalid;
            _FileNum = -1;
            _Slot = 0;
            _Word = 0;
            _Element = 0;
            _Bit = 0;

        
        }
        
        
        #region Properties

        /// <summary>
        /// Tag Format\nPlease, select the device tag format (used only if ""Addressing Type"" is set to ""Tag Name"")
        /// </summary>
        private TagFormats _TagFormat;
        public TagFormats TagFormat
        {
            get { return _TagFormat; }
            set { SetPropertyValue("TagFormat", ref _TagFormat, value); }
        }

        /// <summary>
        /// A-B Address\nEnter start address in Allen-Bradley notation
        /// </summary>
        private string _ABAddress;
        [Size(SizeAttribute.Unlimited)]
        public string ABAddress
        {           
            get { return _ABAddress; }
            set { SetPropertyValue("ABAddress", ref _ABAddress, value); }
        }

        //Symbolic & Physical Address

        private SubElements _SubElement;
        public SubElements SubElement
        {
            get { return _SubElement; }
        }

        //Symbolic Address

        private string _ModuleName;
        public string ModuleName
        {
            get { return _ModuleName; }
        }

        private string _TagName;
        public string TagName
        {
            get { return _TagName; }
        }

        private ushort _Dim0;
        public ushort Dim0
        {
            get { return _Dim0; }
        }
        private ushort _Dim1;
        public ushort Dim1
        {
            get { return _Dim1; }
        }
        private ushort _Dim2;
        public ushort Dim2
        {
            get { return _Dim2; }
        }

        //Physical Address

        private FileTypes _FileType;
        public FileTypes FileType
        {
            get { return _FileType; }
        }

        private int _FileNum;
        public int FileNum
        {
            get { return _FileNum; }
        }

        private uint _Slot;
        public uint Slot
        {
            get { return _Slot; }
        }

        private uint _Word;
        public uint Word
        {
            get { return _Word; }
        }

        private uint _Element;
        public uint Element
        {
            get { return _Element; }
        }


        private uint _Bit;
        public uint Bit
        {
            get { return _Bit; }
        }

        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }

        private DataFormats _DataFormat;
        public DataFormats DataFormat
        {
            get { return _DataFormat; }
        }

        private ushort _ElemSize;
        public ushort ElemSize
        {
            get { return _ElemSize; }
        }

    
        #endregion


        #region IDataErrorInfo Members

        #endregion

    }
}
