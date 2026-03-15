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

namespace EtherNetIP
{
    public sealed class EtherNetIPDynTagSettings : DynTagSettings
    {        
        #region Constructors
        public EtherNetIPDynTagSettings()
            : base()
        {
            AddressType = AddressTypes.DataFile;
            TagFormat = TagFormats.BOOL;
            ABAddress = string.Empty;

            _ParseOk = false;
            _DataFormat = DataFormats.INVALID;
            _ElemSize = 1;

            //Symbolic & Physical Address
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
        private static readonly String AddressTypeParameter = "ATYPE";
        private static readonly String TagFormatParameter = "TEFRM";


        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            ABAddress = helper.GetPartByName(AddressParameter);
            AddressType = (AddressTypes)(helper.GetPartByName(AddressTypeParameter, (UInt16)AddressTypes.DataFile));
            TagFormat = (TagFormats)(helper.GetPartByName(TagFormatParameter, (UInt16)TagFormats.BOOL));

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

            // optional parameters
            AddressType = (AddressTypes)(helper.GetPartByName(AddressTypeParameter, (UInt16)AddressTypes.DataFile));
            if (AddressType == AddressTypes.TagName)
                if (String.IsNullOrEmpty(helper.GetPartByName(TagFormatParameter)))
                    return false;
            TagFormat = (TagFormats)(helper.GetPartByName(TagFormatParameter, (UInt16)TagFormats.BOOL));

            return ParseAddress();
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter, DynamicStringParser.CharAssign, ABAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressTypeParameter, DynamicStringParser.CharAssign, (int)AddressType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", TagFormatParameter, DynamicStringParser.CharAssign, (int)TagFormat);

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
            string Tree = EtherNetIpProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            ABAddress += ("." + Tree.Replace('/', '.'));
            TagFormat = EtherNetIPCommJob.GetEthType(thistagdefinition.DataType.Identifier.ToString());
            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean && thistagdefinition.ArrayDimension != 0)
                TagFormat = TagFormats.ARRAYOF32BITS;
            // assign to string member, default string size (such as during import from device/file)
            if (TagFormat == TagFormats.STRING)
                ABAddress += string.Format(":{0}", EtherNetIpProtocol.STRING_MAX_LENGHT);            
            string dynsettings = ToString();
            ABAddress = memABAddress;
            TagFormat = memTagFormat;
            return dynsettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            //return (EtherNetIpProtocol.MAX_DATA_SIZE >= ByteSize);
            if (AddressType == AddressTypes.TagName)
                return (EtherNetIpProtocol.MAX_DATA_SIZE >= ByteSize);
            else
                return (EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE >= ByteSize);
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            return (EtherNetIpProtocol.DataType(_DataFormat));
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
            if (ABAddress == "" || (ABAddress.Length > EtherNetIpProtocol.MAX_IOI_STRING_LENGTH))
            {
                _ParseOk = false;
            }
            else
            {
                string AddressUp = string.Empty;
                TrimSpaces(ref AddressUp, ABAddress);

                if (AddressType != AddressTypes.TagName)
                {
                    AddressUp = AddressUp.ToUpper();
                    _ParseOk = (ParsePhysicalAddress(ref AddressUp) == Properties.Resources.ParseAddressOk);
                }
                else
                {
                    _ParseOk = (ParseSymbolicAddress(ref AddressUp) == Properties.Resources.ParseAddressOk);
                }
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


            if ((TagFormat == TagFormats.TIMER) ||
            (TagFormat == TagFormats.COUNTER))
            {
                if (TagNameMatch.Groups["SUBELEM"].Length > 0)
                {
                    switch (TagNameMatch.Groups["SUBELEM"].Value)
                    {
                        case "/pre":
                        case "/PRE":
                            localSubElement = SubElements.PRE;
                            break;
                        case "/acc":
                        case "/ACC":
                            localSubElement = SubElements.ACC;
                            break;
                        default:
                            return Properties.Resources.ErrorAddressSubelemRequired;
                    }
                }
                else
                {
                    return Properties.Resources.ErrorAddressSubelemRequired;
                }
            }
            else if (TagNameMatch.Groups["SUBELEM"].Length > 0)
            {
                return Properties.Resources.ErrorAddressSubelemNotRequired;
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
            
            if (TagFormat == TagFormats.STRING)
            {
                if (TagNameMatch.Groups["dim0"].Value.Length > 1)
                    localDim0 = Convert.ToUInt16(TagNameMatch.Groups["dim0"].Value.Remove(0, 1));
                else
                    localDim0 = 0;

                if (localDim0 <= 0) {
                    _ElemSize = 0;
                    _Dim0 = 0;
                    return Properties.Resources.ErrorStringLengthRequired;
                }

                if (localDim0 > EtherNetIpProtocol.STRING_MAX_LENGHT)
                {
                    _ElemSize = 0;
                    _Dim0 = 0;
                    return string.Format(Properties.Resources.ErrorStringLengthOutOfRange, EtherNetIpProtocol.STRING_MAX_LENGHT);
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

            // Set the element size and the data format (for compatibility)
            switch (_TagFormat)
            {
                case TagFormats.BOOL:
                    //case TagElementFormat_ARRAYOFSINT:
                    _ElemSize = 1;
                    _DataFormat = DataFormats.BIT;
                    break;

                case TagFormats.ARRAYOF32BITS:
                    _ElemSize = 4;
                    _DataFormat = DataFormats.BIT;
                    break;

                case TagFormats.SINT:
                case TagFormats.STRUCTURE:
                    _ElemSize = 1;
                    _DataFormat = DataFormats.BYTE;
                    break;

                case TagFormats.STRING:
                    _ElemSize = (byte)localDim0;
                    _DataFormat = DataFormats.BYTE;
                    break;

                case TagFormats.INT:
                    //case TagElementFormat_ARRAYOFINT:
                    _ElemSize = 2;
                    _DataFormat = DataFormats.WORD;
                    break;

                case TagFormats.DINT:
                case TagFormats.REAL:
                case TagFormats.TIMER:
                case TagFormats.COUNTER:
                    //case TagElementFormat_ARRAYOFDINT:
                    //case TagElementFormat_ARRAYOFREAL:
                    _ElemSize = 4;
                    _DataFormat = DataFormats.DWORD;
                    break;

                case TagFormats.LINT:
                    _ElemSize = 8;
                    _DataFormat = DataFormats.LWORD;
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

            //unsupported data types were casted as BOOL
            if (TagFormat > TagFormats.LINT)
            {
                TagFormat = TagFormats.BOOL;
            }

            return Properties.Resources.ParseAddressOk;
        }

        private string ParsePhysicalAddress(ref string AddressUp)
        {
            /*vengono supportati solo le aree:
            O = Output
            I = Input
            S = Status
            B = Binary
            T = Timer
            C = Counter
            N = Integer
            F = Float
            L = Long Integer
            ST = String
            Per timer e counter si deve specificare il sottoelemento
            ad es: T4:20/PRE oppure C5:12/ACC
            */

            FileTypes localFileType = _FileType;
            int localFileNum = _FileNum;
            uint localSlot = _Slot;
            uint localWord = _Word;
            uint localElement = _Element;
            uint localBit = _Bit;
            SubElements localSubElement = _SubElement;
            DataFormats localDataFormat = DataFormats.WORD;
            byte localElementSize = 2;

            Regex TagNameParser = new Regex(@"^(?<FileType>[A-Za-z]{1,2})(?<file>\d{1,4})(?<index1>:\d{1,4})(?<index2>\.\d{1,5})?(?<bit>\/\d{1,3})?(?<SUBELEM>\/ACC|\/PRE)?$");
            Match TagNameMatch = TagNameParser.Match(AddressUp);
            if (!TagNameMatch.Success)
            {
                return Properties.Resources.ErrorAddressInvalid;
            }
            localFileNum = Convert.ToInt32(TagNameMatch.Groups["file"].Value);

            switch (TagNameMatch.Groups["FileType"].Value)
            {
                case "O":
                    localFileType = FileTypes.Output;
                    break;
                case "I":
                    localFileType = FileTypes.Input;
                    break;
                case "S":
                    localFileType = FileTypes.Status;
                    break;
                case "B":
                    localFileType = FileTypes.Binary;
                    break;
                case "T":
                    localFileType = FileTypes.Timer;
                    break;
                case "C":
                    localFileType = FileTypes.Counter;
                    break;
                case "R":
                    localFileType = FileTypes.Control;
                    break;
                case "N":
                    localFileType = FileTypes.Integer;
                    break;
                case "F":
                    localFileType = FileTypes.Float;
                    break;
                case "L":
                    localFileType = FileTypes.LongInteger;
                    break;
                case "ST":
                    localFileType = FileTypes.String;
                    break;
                default:
                    return Properties.Resources.ErrorAddressSubelemRequired;
            }
            localFileNum = Convert.ToInt32(TagNameMatch.Groups["file"].Value);

            if (TagNameMatch.Groups["index1"].Value.Length <= 1)
            {
                return Properties.Resources.ErrorAddressInvalid;
            }

            switch (TagNameMatch.Groups["FileType"].Value)
            {
                case "O":
                case "I":
                    if (TagNameMatch.Groups["SUBELEM"].Value.Length > 1)
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    if (TagNameMatch.Groups["index2"].Value.Length > 1)
                    {
                        localSlot = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                        localWord = Convert.ToUInt32(TagNameMatch.Groups["index2"].Value.Remove(0, 1));
                    }
                    else
                    {
                        localSlot = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    }
                    break;
                case "S":
                case "B":
                case "N":
                    if ((TagNameMatch.Groups["SUBELEM"].Value.Length > 1) ||
                        (TagNameMatch.Groups["index2"].Value.Length > 1))
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;
                case "T":
                case "C":
                    if ((TagNameMatch.Groups["bit"].Value.Length > 1) ||
                        (TagNameMatch.Groups["SUBELEM"].Value.Length <= 1))
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;

                case "R":
                    if (TagNameMatch.Groups["SUBELEM"].Value.Length > 1)
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;
                case "F":
                    if ((TagNameMatch.Groups["bit"].Value.Length > 1) ||
                        (TagNameMatch.Groups["SUBELEM"].Value.Length > 1))
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;
                case "L":
                    if ((TagNameMatch.Groups["SUBELEM"].Value.Length > 1) ||
                        (TagNameMatch.Groups["index2"].Value.Length > 1))
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;
                case "ST":
                    if ((TagNameMatch.Groups["index1"].Value.Length < 1) ||
                        (TagNameMatch.Groups["SUBELEM"].Value.Length > 1) ||
                        (TagNameMatch.Groups["index2"].Value.Length > 1) ||
                        (TagNameMatch.Groups["bit"].Value.Length > 1))
                    {
                        return Properties.Resources.ErrorAddressInvalid;
                    }
                    localElement = Convert.ToUInt32(TagNameMatch.Groups["index1"].Value.Remove(0, 1));
                    break;
            }

            if (localFileType == FileTypes.LongInteger)
            {
                localDataFormat = DataFormats.DWORD;
                localElementSize = 4;
            }

            if (TagNameMatch.Groups["bit"].Value.Length > 1)
            {
                localBit = Convert.ToUInt32(TagNameMatch.Groups["bit"].Value.Remove(0, 1));
                if (localFileType == FileTypes.Float)
                {
                    return Properties.Resources.ErrorAddressInvalid;
                }
                else if (localFileType != FileTypes.LongInteger && (localBit < 0 || localBit > 15))
                {
                    return Properties.Resources.ErrorAddressInvalid;
                }
                else if (localFileType == FileTypes.LongInteger && (localBit < 0 || localBit > 31))
                {
                    return Properties.Resources.ErrorAddressInvalid;
                }

                localDataFormat = DataFormats.BIT;
                localElementSize = 1;
            }

            if (TagNameMatch.Groups["SUBELEM"].Value.Length > 1)
            {
                switch (TagNameMatch.Groups["SUBELEM"].Value)
                {
                    case "/ACC":
                        localSubElement = SubElements.ACC;
                        break;
                    case "/PRE":
                        localSubElement = SubElements.PRE;
                        break;
                }
            }
            if (localFileType == FileTypes.Float)
            {
                localDataFormat = DataFormats.DWORD;
                localElementSize = 4;
            }
            else if (localFileType == FileTypes.String)
            {
                _Dim0 = 84;
                localDataFormat = DataFormats.BYTE;
                localElementSize = 84;
            }


            _DataFormat = localDataFormat;
            _FileType = localFileType;
            _FileNum = localFileNum;
            _Slot = localSlot;
            _Word = localWord;
            _Element = localElement;
            _Bit = localBit;
            _SubElement = localSubElement;
            _ElemSize = localElementSize;

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
        //    }
        //}

        ///// <summary>   Number of element to exchange. </summary>
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Station Name. </summary>
        ///
        /// <value> The name of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("General")]
        //[Description("Station Name")]
        //public string StationName
        //{
        //    get
        //    {
        //        return base.StationName;
        //    }
        //    set
        //    {
        //        base.StationName = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
        //    }
        //}

        private AddressTypes _AddressType;
        [Category("Device Data")]
        [Description("Address Type")]
        public AddressTypes AddressType
        {
            get { return _AddressType; }
            set
            {
                _AddressType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("StationName"));
            }
        }
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
                OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }

        private Dictionary<string, EtherNetIPStationSettings> _stationSettingList;
        public Dictionary<string, EtherNetIPStationSettings> stationSettingList
        {
            get { return _stationSettingList; }
            set
            {
                _stationSettingList = value;
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

        private byte _ElemSize;
        public byte ElemSize
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

            if (propertyName == "ABAddress")
            {
                if (!ParseAddress())
                    return Properties.Resources.ParseKo;
                if (InvalidBoleanArray())
                    return UFUAModel.Properties.Resources.ErrorInvalidBoleanArray;
                if ((uint)VarType != unchecked((uint)(-1)))
                {
                    return ProtocolDataSizeValidation(VarType);
                }
            }
            if (propertyName == "ArrayDimension")
            {
                if (ParseAddress())
                {
                    if (InvalidBoleanArray())
                        return UFUAModel.Properties.Resources.ErrorInvalidBoleanArray;
                }
            }

            if (propertyName == "AddressType")
            {
                if (!string.IsNullOrWhiteSpace(base.StationName))
                {
                    //if (!ParseAddress())
                    //    return Properties.Resources.ErrorAddressType;
                    if (InvalidBoleanArray())
                        return UFUAModel.Properties.Resources.ErrorInvalidBoleanArray;
                    if (InvalidAddressType())
                        return Properties.Resources.ErrorAddressTypeIncompatibleStation;
                }
            }
            if (propertyName == "StationName")
            {
                if (InvalidAddressType())
                    return Properties.Resources.ErrorAddressTypeIncompatibleStation;
            }


            return null;
        }
        private bool InvalidBoleanArray()
        {
            uint DataTypeBitSize = ArrayDimension == 0 ? 1 : ArrayDimension;
            if (ElementNumber == 0)
                DataTypeBitSize = DataTypeBitSize * GetDataTypeBitSize(VarType);
            bool bDataTypeBitSize = false;
            if (DataFormat == DataFormats.BIT && AddressType == AddressTypes.DataFile)
            {
                if (FileType != FileTypes.LongInteger)
                {
                    bDataTypeBitSize = (DataTypeBitSize > 16);
                }
                else
                {
                    bDataTypeBitSize = (DataTypeBitSize > 32);
                }
            }
            //return (DataFormat == DataFormats.BIT && AddressType == AddressTypes.DataFile && (DataTypeBitSize > 16 || (Bit != 0 && DataTypeBitSize > 1)));
            return (DataFormat == DataFormats.BIT && AddressType == AddressTypes.DataFile && (bDataTypeBitSize || (Bit != 0 && DataTypeBitSize > 1)));
        }
        private bool InvalidAddressType()
        {
            if (stationSettingList == null || !stationSettingList.ContainsKey(StationName))
            {
                return false;
            }
            return (AddressType == AddressTypes.DataFile && stationSettingList[StationName].PlcType == PlcTypes.Micro800_series);
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
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
                case "StationName":
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ABAddress"));
                    break;
            }            
        }
        #endregion
    }
}
