using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace OmronEthernetIP
{
    public sealed class OmronEthernetIPDynTagSettings : DynTagSettings
    {
        const int MAX_IOI_STRING_LENGTH = 200;
        const int MAX_ABBREVIATED_TYPE_LENGTH = 4;
        const int MIN_READ_REQUEST_LENGTH = 10;
        const int MIN_WRITE_REQUEST_LENGTH = 13;
        const int MIN_READ_REPLY_LENGTH = 9;
        const int MIN_WRITE_REPLY_LENGTH = 6;
        const int MAX_STRING_LENGTH = 256;

        #region Constructors

        public OmronEthernetIPDynTagSettings()
            : base()
        {
            TagFormat = TagFormats.BOOL;
            ABAddress = string.Empty;

            _StructStringFieldLengths = string.Empty;

            _ParseOk = false;
            _DataFormat = DataFormats.INVALID;
            _ElemSize = 1;
            
            //Symbolic Address
            _SubElement = SubElements.Invalid;

            //Symbolic Address
            _TagName = string.Empty;
            _ModuleName = string.Empty;
            _Dim0 = 1;
            _Dim1 = 1;
            _Dim2 = 1;

            //Physical Address
            _FileType = FileTypes.Invalid;
            _FileNum = -1;
            _Slot = 0;
            _Word = 0;
            _Element = 0;
            _Bit = 0;
        }

        #endregion

        #region Static Members

        private static readonly String AddressParameter = "ABA";
        private static readonly String TagFormatParameter = "TEFRM";
        private static readonly String StructStringFieldLengthsParameter = "SSFL";
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            ABAddress = helper.GetPartByName(AddressParameter);
            TagFormat = (TagFormats)(helper.GetPartByName(TagFormatParameter, (UInt16)TagFormats.BOOL));
            StructStringFieldLengths = helper.GetPartByName(StructStringFieldLengthsParameter);
            
            ParseAddress();
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            ABAddress = helper.GetPartByName(AddressParameter);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(TagFormatParameter)))
                return false; 
            TagFormat = (TagFormats)(helper.GetPartByName(TagFormatParameter, (UInt16)TagFormats.BOOL));

            // optional parameter            
            _StructStringFieldLengths = helper.GetPartByName(StructStringFieldLengthsParameter);
            
            return ParseAddress();
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter, DynamicStringParser.CharAssign, ABAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", TagFormatParameter, DynamicStringParser.CharAssign, (int)TagFormat);

            // optional parameter
            if (IsObjectType && !String.IsNullOrWhiteSpace(StructStringFieldLengths))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StructStringFieldLengthsParameter, DynamicStringParser.CharAssign, StructStringFieldLengths);
            }

            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetNodeDynSetting(thistagdefinition);
        }
        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            return GetNodeDynSetting(thistagdefinition);
        }

        string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string memABAddress = ABAddress;
            TagFormats memTagFormat = TagFormat;
            //string stringLengths = StructStringFieldLengths;
            string Tree = OmronEthernetIPProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            TagFormat = OmronEthernetIPCommJob.GetEthType(thistagdefinition.DataType.Identifier.ToString());

            if (TagFormat == TagFormats.STRING)
            {
                string localABAddress = Tree;

                // manage string's length of struct's member
                OmronEthernetIPStructStringLength structStringFieldLengthsMapper = new OmronEthernetIPStructStringLength();
                structStringFieldLengthsMapper.Parse(_StructStringFieldLengths);
                // Search for the string length information
                if (structStringFieldLengthsMapper.IsGlobalStringLength())
                {
                    Tree += string.Format(":{0}", structStringFieldLengthsMapper.DefaultStringLength);
                }
                else
                {
                    if (structStringFieldLengthsMapper.HasMember(localABAddress))
                        Tree += string.Format(":{0}", structStringFieldLengthsMapper.GetMember(localABAddress).StringLength);
                    else
                        // If the size of the string has not been set, set the default value: 256
                        Tree += string.Format(":{0}", OmronEthernetIPProtocol.MAX_STRING_LENGTH);
                }
            }

            ABAddress += ("." + Tree.Replace('/', '.'));
            string dynsettings = ToString();
            ABAddress = memABAddress;
            TagFormat = memTagFormat;
            return dynsettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (OmronEthernetIPProtocol.MAX_DATA_SIZE >= ByteSize);
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            return (OmronEthernetIPProtocol.DataType(_DataFormat));
        }

        #endregion

        #region Functions

        public bool ParseAddress(string InAddress)
        {
            ABAddress = InAddress;
            return ParseAddress();
        }

        public bool ParseAddress()
        {

            // The address must not be empty and the first character must be a letter
            if (ABAddress == "" || (ABAddress.Length > MAX_IOI_STRING_LENGTH))
            {
                _ParseOk = false;
            }
            else
            {
                string AddressUp = string.Empty;
                TrimSpaces(ref AddressUp, ABAddress);
                _ParseOk = (ParseSymbolicAddress(ref AddressUp) == Properties.Resources.ParseAddressOk);
            }
            return _ParseOk;
        }

        private void TrimSpaces(ref string Dst, string Src)
        {
            int len = Src.Length;
            int i = 0;

            Dst = string.Empty;

            if (len == 0)
                return;

            do
            {
                if (Src[i] != ' ')
                    Dst += Src[i];
            } while (++i < len);
        }

        private string ParseSymbolicAddress(ref string AddressUp)
        {
            string localModuleName = _ModuleName;
            SubElements localSubElement = _SubElement;
            ushort localDim0 = _Dim0;
            ushort localDim1 = _Dim1;
            ushort localDim2 = _Dim2;

            Regex TagNameParser = new Regex(@"^(?<ModuleName>(?i)Program:\w+\.)?(?<TagName>[\w\[\]\.\,]+)(?<dim0>:\d+)?(?<dim1>:\d+)?(?<dim2>:\d+)?(?<SUBELEM>(?i)/ACC|(?i)/PRE)?$");
            Match TagNameMatch = TagNameParser.Match(AddressUp);
            if (!TagNameMatch.Success)
            {
                return Properties.Resources.ErrorAddressInvalid;
            }

            int ModuleNameLength = TagNameMatch.Groups["ModuleName"].Length;
            if (ModuleNameLength > 0)
            {
                localModuleName = "Program:" + TagNameMatch.Groups["ModuleName"].Value.Remove(ModuleNameLength - 1, 1).Remove(0, 8);
            }
            else
            {
                localModuleName = "";
            }

            if (TagNameMatch.Groups["dim0"].Value.Length > 1)
            {
                localDim0 = Convert.ToUInt16(TagNameMatch.Groups["dim0"].Value.Remove(0, 1));
                if (TagNameMatch.Groups["dim1"].Value.Length > 1)
                {
                    localDim1 = Convert.ToUInt16(TagNameMatch.Groups["dim1"].Value.Remove(0, 1));
                    if (TagNameMatch.Groups["dim2"].Value.Length > 1)
                    {
                        localDim2 = Convert.ToUInt16(TagNameMatch.Groups["dim2"].Value.Remove(0, 1));
                    }
                }
            }
            //else if (TagFormat == TagFormats.STRING)
            //{
            //    return Properties.Resources.ErrorStringLengthRequired;
            //}
            if (TagFormat == TagFormats.STRING)
            {
                if (TagNameMatch.Groups["dim0"].Value.Length > 1)
                    localDim0 = Convert.ToUInt16(TagNameMatch.Groups["dim0"].Value.Remove(0, 1));
                else
                    localDim0 = 0;

                if (localDim0 <= 0)
                {
                    _ElemSize = 0;
                    _Dim0 = 0;
                    return Properties.Resources.ErrorStringLengthRequired;
                }

                if (localDim0 > OmronEthernetIPProtocol.MAX_STRING_LENGTH)
                {
                    _ElemSize = 0;
                    _Dim0 = 0;
                    return string.Format(Properties.Resources.ErrorStringLengthOutOfRange, 82);
                }
            }

            Regex TagNodeParser = new Regex(@"\b(?<TagTree>[^\.]+)\.?");
            MatchCollection TagNodeMatches = TagNodeParser.Matches(TagNameMatch.Groups["TagName"].Value);
            if (TagNodeMatches.Count > 0)
            {
                Regex TagTreeNodeIndex = new Regex(@"^([^\[]+)(?<index>\[[\d,]+\])?");
                Regex TagTreeIndexValidator = new Regex(@"^\[(?<index1>\d+)(?<index2>,\d+)?(?<index3>,\d+)?\]$");
                foreach (Match match in TagNodeMatches)
                {
                    Match TagTreeNodeIndexMatch = TagTreeNodeIndex.Match(match.Value);
                    if (TagTreeNodeIndexMatch.Success)
                    {
                        if (TagTreeNodeIndexMatch.Groups["index"].Value.Length > 0)
                        {
                            Match TagTreeIndexMatch = TagTreeIndexValidator.Match(TagTreeNodeIndexMatch.Groups["index"].Value);
                            if (!TagTreeIndexMatch.Success)
                            {
                                return Properties.Resources.ErrorAddressInvalid;
                            }
                        }
                    }
                }
            }
            else
            {
                return Properties.Resources.ErrorAddressInvalid;
            }

            // The accepted maximum length of strings is 256
            if((_TagFormat == TagFormats.STRING) && (localDim0 > MAX_STRING_LENGTH))
            {
                return Properties.Resources.ErrorAddressInvalid;
            }

            // Set the element size and the data format (for compatibility)
            switch (_TagFormat)
            {
                case TagFormats.BOOL:
                    //case TagElementFormat_ARRAYOFSINT:
                    _ElemSize = 1;
                    _DataFormat = DataFormats.BIT;
                    break;

                case TagFormats.SINT:
                case TagFormats.USINT:
                case TagFormats.BYTE:
                case TagFormats.STRUCTURE:
                    _ElemSize = 1;
                    _DataFormat = DataFormats.BYTE;
                    break;
                
                case TagFormats.STRING:
                    _ElemSize = localDim0;
                    _DataFormat = DataFormats.BYTE;
                    break;

                case TagFormats.UINT:
                case TagFormats.INT:
                case TagFormats.WORD:
                    _ElemSize = 2;
                    _DataFormat = DataFormats.WORD;
                    break;

                case TagFormats.DINT:
                case TagFormats.UDINT:
                case TagFormats.DWORD:
                case TagFormats.REAL:
                    _ElemSize = 4;
                    _DataFormat = DataFormats.DWORD;
                    break;
                case TagFormats.ULINT:
                    _ElemSize = 8;
                    _DataFormat = DataFormats.ULINT;
                    break;
                case TagFormats.LWORD:
                    _ElemSize = 8;
                    _DataFormat = DataFormats.LWORD;
                    break;
                case TagFormats.LINT:
                    _ElemSize = 8;
                    _DataFormat = DataFormats.LINT;
                    break;
                case TagFormats.LREAL:
                    _ElemSize = 8;
                    _DataFormat = DataFormats.DOUBLE;
                    break;

                default:
                    _ElemSize = 1;
                    _DataFormat = DataFormats.BYTE;
                    break;
            }


            _TagName = TagNameMatch.Groups["TagName"].Value;

            _ModuleName = localModuleName;
            _SubElement = localSubElement;
            _Dim0 = localDim0;
            _Dim1 = localDim1;
            _Dim2 = localDim2;

            if (TagFormat > TagFormats.ULINT)
            {
                TagFormat = TagFormats.BOOL;
            }

            return Properties.Resources.ParseAddressOk;
        }

        #endregion

        #region Properties
        //private UFUAModel.DataType _VarType;
        //[Category("General")]
        //[Description("Variable Type")]
        //public override UFUAModel.DataType VarType
        //{
        //    get
        //    {
        //        return _VarType;
        //    }
        //    set
        //    {
        //        _VarType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}
        //private uint _ArrayDimension;
        //[Category("General")]
        //[Description("Array Dimension")]
        //public override uint ArrayDimension
        //{
        //    get
        //    {
        //        return _ArrayDimension;
        //    }
        //    set
        //    {
        //        _ArrayDimension = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        /// <summary>   Number of element to exchange. </summary>
        //private int _ElementNumber;
        //[Category("General")]
        //[Description("Element Number")]
        //public override int ElementNumber
        //{
        //    get { return _ElementNumber; }
        //    set
        //    {
        //        _ElementNumber = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

        private TagFormats _TagFormat;
        [Category("Device Data")]
        [Description("Tag Format")]
        public TagFormats TagFormat
        {
            get { return _TagFormat; }
            set 
            { 
                _TagFormat = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }
        private string _ABAddress;
        [Category("Device Data")]
        [Description("A-B Address")]
        [Size(SizeAttribute.Unlimited)]
        public string ABAddress
        {
            get { return _ABAddress; }
            set
            {
                _ABAddress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }

        private string _StructStringFieldLengths;
        [Category("Device Data")]
        [Description("Max. Lengths of the Structure Fields of String Type")]
        [Size(SizeAttribute.Unlimited)]
        public string StructStringFieldLengths
        {
            get { return _StructStringFieldLengths; }
            set
            {
                _StructStringFieldLengths = value;
                OnPropertyChanged("StructStringFieldLengths");
            }
        }

        //Symbolic & Physical Address

        private SubElements _SubElement;
        public SubElements SubElement
        {
            get { return _SubElement; }
        }

        //Symbolic Address

        private string _ModuleName;
        [Size(SizeAttribute.Unlimited)]
        public string ModuleName
        {
            get { return _ModuleName; }
        }

        private string _TagName;
        [Size(SizeAttribute.Unlimited)]
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

        public int Length
        {
            get { return _Dim0 * _Dim1 * _Dim2; }
        }        

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) {
                case "ABAddress":
                    if (!ParseAddress())
                        return Properties.Resources.ParseKo;
                    if ((uint)VarType != unchecked((uint)(-1)))
                    {
                        if ((uint)VarType != unchecked((uint)(-1)))
                            return ProtocolDataSizeValidation(VarType);
                    }
                    break;
                case "StructStringFieldLengths":
                    if (this.IsObjectType)
                    {
                        OmronEthernetIPStructStringLength oSSL = new OmronEthernetIPStructStringLength();
                        oSSL.Parse(_StructStringFieldLengths);
                        if (oSSL.ParsingError)
                            return Properties.Resources.ErrorInvalidStructureStringLengths;
                    }
                    break;
            }

            return null;
        }


        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion
    }
}
