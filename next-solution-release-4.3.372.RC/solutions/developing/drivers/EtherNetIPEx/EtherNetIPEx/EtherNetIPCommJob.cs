using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text;
using System.Text.RegularExpressions;

namespace EtherNetIP
{
    public class EtherNetIPCommJob : CommJob
    {
        #region Constructors
        public EtherNetIPCommJob(Station station, EtherNetIPCommJobSettings settings)
            : base(station, settings)
        {
            _AddressType = settings.AddressType;
            _TagFormat = settings.TagFormat;
            _ABAddress = settings.ABAddress;

            _ParseOk = settings.ParseOk;
            _DataFormat = settings.DataFormat;
            _ElemSize = settings.ElemSize;

            //Symbolic & Physical Address
            _SubElement = settings.SubElement;

            //Symbolic Address
            _TagName = settings.TagName;
            _ModuleName = settings.ModuleName;
            _Dim0 = settings.Dim0;
            _Dim1 = settings.Dim1;
            _Dim2 = settings.Dim2;

            //Physical Address
            _FileType = settings.FileType;
            _FileNum = settings.FileNum;
            _Slot = settings.Slot;
            _Word = settings.Word;
            _Element = settings.Element;
            _Bit = settings.Bit;
            if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
                ElementNumber = 1;

            InitEtherNetIPCommJob();
            CheckJobValid();

        }

        public EtherNetIPCommJob(Station station, EtherNetIPTag defTag)
            : base(station, defTag)
        {

            _AddressType = defTag.EtherNetIPDynSettings.AddressType;
            _TagFormat = defTag.EtherNetIPDynSettings.TagFormat;
            _ABAddress = defTag.EtherNetIPDynSettings.ABAddress;

            _ParseOk = defTag.EtherNetIPDynSettings.ParseOk;
            _DataFormat = defTag.EtherNetIPDynSettings.DataFormat;
            _ElemSize = defTag.EtherNetIPDynSettings.ElemSize;

            //Symbolic & Physical Address
            _SubElement = defTag.EtherNetIPDynSettings.SubElement;

            //Symbolic Address
            _TagName = defTag.EtherNetIPDynSettings.TagName;
            _ModuleName = defTag.EtherNetIPDynSettings.ModuleName;
            _Dim0 = defTag.EtherNetIPDynSettings.Dim0;
            _Dim1 = defTag.EtherNetIPDynSettings.Dim1;
            _Dim2 = defTag.EtherNetIPDynSettings.Dim2;

            //Physical Address
            _FileType = defTag.EtherNetIPDynSettings.FileType;
            _FileNum = defTag.EtherNetIPDynSettings.FileNum;
            _Slot = defTag.EtherNetIPDynSettings.Slot;
            _Word = defTag.EtherNetIPDynSettings.Word;
            _Element = defTag.EtherNetIPDynSettings.Element;
            _Bit = defTag.EtherNetIPDynSettings.Bit;
            if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
                ElementNumber = 1;

            if (ElementNumber > 0  || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            InitEtherNetIPCommJob();
            CheckJobValid();

        }

        public EtherNetIPCommJob(Station station)
            : base(station)
        {
            if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
                ElementNumber = 1;
            InitEtherNetIPCommJob();
            CheckJobValid();

        }

        protected EtherNetIPCommJob()
        {
            if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
                ElementNumber = 1;
            InitEtherNetIPCommJob();
            CheckJobValid();

        }

        public void InitEtherNetIPCommJob(bool setCmd = true)
        {
            if(setCmd)
                CommandType = CommandTypes.Invalid;
            else
                CommandType = (ReadRequest() ? CommandTypes.ReadCmd : CommandTypes.WriteCmd);

            ReadTagStart = 0;
            ReadTagEnd = 0;
            PartialArrayStart = 0;
            PartialArrayEnd = 0;
            answer = null;
            TerminateTask = false;
        }

        public void Init(bool write)
        {
            if (write)
            {
                CommandType = CommandTypes.WriteCmd;
            }
            else
            { 
                CommandType = CommandTypes.ReadCmd;
                if (AddressType == AddressTypes.TagName && answer == null)
                    answer = new byte[TotalJobSize];
            }
            TerminateTask = false;            
        }

        //for InputOutput jobs with Movicon data type smaller than plc data type, before write, read it --> only 1 job at time
        public bool IsSingleWriteInputOutputSmallToBig()
        {
            return (Type == LinkType.InputOutput && (GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier) < GetProtocolDataByteSize())); // && TagsListToWrite.Count > 0));
        }

        #endregion
        #region data Member

        private TagFormats _NodeFormat;
        public ushort ReadTagStart;
        public ushort ReadTagEnd;
        public ushort PartialArrayStart;
        public ushort PartialArrayEnd;
        public byte[] answer;

        //public bool ExecuteTask;
        public bool TerminateTask;

        public  byte[] TagNameRequestBuffer;
        public ushort TagIndexRequest = 0;
        public int TagIndexRequestSize = 0;
        public bool BuildInfoMaps;

        #endregion

        #region Methods


        public ushort adjSize(Tag cand)
        {
            ushort tagSize;
            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (cand.TagNode.ArrayDimension == 0)
                    tagSize = (ushort)GetProtocolDataByteSize();
                else
                    tagSize = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            }
            else if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                // Micro800 bit's array element = 1 byte
                if (((EtherNetIPStation)Station).PlcType == PlcTypes.Micro800_series)
                    tagSize = (UInt16)cand.Size;
                else
                    tagSize = (UInt16)((cand.Size + 7) / 8);
            }
            else
                tagSize = (ushort)cand.Size;
            if (TagFormat == TagFormats.ARRAYOF32BITS)
                return (ushort)(((tagSize + 31) / 32) * 4);
            else
                return tagSize;
        }

        public ushort adjSizeWrite(Tag cand)
        {
            ushort tagSize;
            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (cand.TagNode.ArrayDimension == 0)
                    tagSize = (ushort)GetProtocolDataByteSize();
                else
                    tagSize = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            }
            else if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                // Micro800 bit's array element = 1 byte
                if (((EtherNetIPStation)Station).PlcType == PlcTypes.Micro800_series)
                    tagSize = (UInt16)cand.Size;
                else
                    tagSize = (UInt16)((cand.Size + 7) / 8);
            }
            else
                tagSize = (ushort)cand.Size;
            return tagSize;
        }


        public ushort GetNumOfElements(ref byte[] pdu, ref ushortUnion pduPointer, ushort numOfElements)
        {
            ushortUnion NumOfElements = new ushortUnion(numOfElements);

            pdu[pduPointer.USHORT++] = NumOfElements.LOBYTE;
            pdu[pduPointer.USHORT++] = NumOfElements.HIBYTE;

            return 2;
        }

        public ushort GetNumOfElements(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand, bool isWrite = false)
        {
            ushortUnion NumOfElements;

            if (TagFormat == TagFormats.STRUCTURE)
            {
                NumOfElements = new ushortUnion(1);
            }
            else if (TagFormat == TagFormats.STRING)
            {
                //On the strings to receive the answer with the length plus the data. 
                //It is necessary to consider them as a structure, 
                //and not require individual members, 
                //but the name of the tag is passed by passing as length 1, 
                //this identifying that it is a structure.
                if(isWrite)
                {
                    NumOfElements = new ushortUnion((ushort)Length);
                }
                else
                {
                    NumOfElements = new ushortUnion((ushort)1);
                }      
            }
            else
            {
                
                if (PartialArrayEnd == PartialArrayStart)
                {
                    NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize));
                }
                else
                {
                    if (PartialArrayEnd == 0)
                        NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize - PartialArrayStart));
                    else                    
                        NumOfElements = new ushortUnion((ushort)(PartialArrayEnd - PartialArrayStart));              
                }
            }


            pdu[pduPointer.USHORT++] = NumOfElements.LOBYTE;
            pdu[pduPointer.USHORT++] = NumOfElements.HIBYTE;

            return 2;
        }

        public ushort GetNumOfElementsMicro800(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand)
        {
            ushortUnion NumOfElements;

            if (TagFormat == TagFormats.STRUCTURE || TagFormat == TagFormats.STRING)
            {
                NumOfElements = new ushortUnion(1);
            }
            else
            {
                if (PartialArrayEnd == PartialArrayStart)
                {
                    NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize));
                }
                else
                {
                    // Special case: data cannot be contained in one single frame
                    if (PartialArrayEnd == 0)
                        NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize - PartialArrayStart));
                    else
                        NumOfElements = new ushortUnion((ushort)(PartialArrayEnd - PartialArrayStart));
                }
            }


            pdu[pduPointer.USHORT++] = NumOfElements.LOBYTE;
            pdu[pduPointer.USHORT++] = NumOfElements.HIBYTE;

            return 2;
        }

        public ushort GetTagFormat(ref byte[] pdu, ref ushortUnion pduPointer)
        {
            return GetTagFormat(ref pdu, ref pduPointer, _TagFormat);
        }
        public ushort GetTagFormatMicro800(ref byte[] pdu, ref ushortUnion pduPointer)
        {
            switch (_TagFormat)
            {
                case TagFormats.STRING:
                    pdu[pduPointer.USHORT++] = 0xDA;
                    pdu[pduPointer.USHORT++] = 0;
                    break;
                default:
                    return GetTagFormat(ref pdu, ref pduPointer, _TagFormat);
            }
            return 2;
        }

        internal ushort GetTagFormat(ref byte[] pdu, ref ushortUnion pduPointer, TagFormats Format)
        {
            // Abbreviated Type
            switch (Format)
            {
                case TagFormats.BOOL:
                    pdu[pduPointer.USHORT++] = 0xC1;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.SINT:
                    //case TagElementFormat_ARRAYOFSINT:
                case TagFormats.STRING:
                    pdu[pduPointer.USHORT++] = 0xC2;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.INT:
                    //case TagElementFormat_ARRAYOFINT:
                    pdu[pduPointer.USHORT++] = 0xC3;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.DINT:
                    //case TagElementFormat_ARRAYOFDINT:
                case TagFormats.TIMER:
                case TagFormats.COUNTER:
                    pdu[pduPointer.USHORT++] = 0xC4;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.REAL:
                    //case TagElementFormat_ARRAYOFREAL:
                    pdu[pduPointer.USHORT++] = 0xCA;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.ARRAYOF32BITS:
                    pdu[pduPointer.USHORT++] = 0xD3;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.STRUCTURE:
                    return GetTagFormat(ref pdu, ref pduPointer, _NodeFormat);

                case TagFormats.LINT:
                    pdu[pduPointer.USHORT++] = 0xC5;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.ULINT:
                    pdu[pduPointer.USHORT++] = 0xC9;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.LWORD:
                    pdu[pduPointer.USHORT++] = 0xD4;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                default:
                    break;
            }

            return 2;
        }

        // Version of GetJobData for Cip devices
        public ushort GetTagWriteData(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand)
        {
            ushort writeLenght = adjSizeWrite(cand);
            byte[] tmpBuffer = new byte[writeLenght];
            ushort offset = 0;

            lock (lockListObject)
            {                
                EtherNetIPStation StationEth = (EtherNetIPStation)Station;
                switch (StationEth.PlcType)
                {
                    case PlcTypes.SLC500_MicroLogix:                
                        EtherNetIPTag candEth = (EtherNetIPTag)cand;
                        candEth.GetTagBufferSLC500_MicroLogix(ref tmpBuffer, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        break;
                    case PlcTypes.Micro800_series:                        
                        EtherNetIPTag candEthMicro = (EtherNetIPTag)cand;
                        candEthMicro.GetTagBufferMicro800(ref tmpBuffer, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        break;
                    default:
                        cand.GetTagBuffer(ref tmpBuffer, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        break;
                }

            }
            uint ArraySize = cand.TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            if (ProtocolDataSizeBig())
            {
                List<byte> correctData = new List<byte>();
                UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                {
                    byte[] tmpdata = new byte[sizeDataType];
                    if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                        Array.Copy(tmpBuffer, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                    else
                    {
                        if ((1 << (ArrayIndex % 8) & tmpBuffer[ArrayIndex / 8]) == 0)
                            tmpdata[0] = 0;
                        else
                            tmpdata[0] = 1;
                    }
                    cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                    correctData.AddRange(tmpdata);
                }
                tmpBuffer = correctData.ToArray();
            }
            else if (isProtocolBool())
            {
                if (ElementNumber > 0 && (uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                {
                    byte[] tmpData = new byte[(((ArraySize + 7) / 8 + ElemSize - 1) / ElemSize) * ElemSize];
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        if (tmpBuffer[ArrayIndex] != 0)
                            tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                    }
                    tmpBuffer = tmpData;
                }
            }

            writeLenght = (ushort)tmpBuffer.Count();
            if (PartialArrayStart != PartialArrayEnd)
            {
                offset = (ushort)(PartialArrayStart * ElemSize);
                if (PartialArrayEnd == 0)
                    writeLenght = (ushort)(writeLenght - offset);
                else
                    writeLenght = (ushort)(PartialArrayEnd * ElemSize - offset);

                PartialArrayStart = PartialArrayEnd;
            }
            Array.Copy(tmpBuffer, offset, pdu, pduPointer.USHORT, writeLenght);

            if (SwapBytes)
                SwapByteBuffer(ref pdu, pduPointer.USHORT, (int)writeLenght);
            if (SwapWords)
                SwapWordBuffer(ref pdu, pduPointer.USHORT, (int)writeLenght);

            pduPointer.USHORT += (ushort)writeLenght;

            return ((ushort)writeLenght);
        }


        
        public ushort GetTagWriteDataStringLength(ref byte[] pdu, ref ushortUnion pduPointer, ushort startData, ushort endData)
        {
            // len of string
            uintUnion stringLen = new uintUnion(startData);
            while (stringLen.UINT < endData && pdu[stringLen.UINT] != 0)
                stringLen.UINT++;
            stringLen.UINT -= startData;
            pdu[pduPointer.USHORT++] = stringLen.LOUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = stringLen.LOUSHORT.HIBYTE;
            pdu[pduPointer.USHORT++] = stringLen.HIUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = stringLen.HIUSHORT.HIBYTE;

            return 4;
        }

        public ushort GetAsciiTagname(EtherNetIPStation s,ref byte[] pdu, ref ushortUnion pduPointer , Tag tag, string subName = "")
        {
            ushort sizeWords = pduPointer.USHORT;
            if (TagNameRequestBuffer == null || BuildInfoMaps != s.BuildInfoMaps)
            {
                pdu[pduPointer.USHORT++] = 0;

                //Split a string into sub-strings, using a period as a separator character.
                Regex TagNodeParser = new Regex(@"\b(?<TagTree>[^\.]+)?");
                MatchCollection TagNodeMatches = TagNodeParser.Matches(_TagName);

                if (TagNodeMatches.Count > 0)
                {
                    ushort nParentTemplateInstance = 0;
                    int level = 0;
                    Regex TagTreeNodeIndex = new Regex(@"^([^\[]+)(?<index>\[[\d,]+\])?");
                    Regex TagTreeIndexValidator = new Regex(@"^\[(?<index1>\d+)(?<index2>,\d+)?(?<index3>,\d+)?\]$");
                    foreach (Match match in TagNodeMatches)
                    {
                        Match TagTreeNodeIndexMatch = TagTreeNodeIndex.Match(match.Value);
                        if (TagTreeNodeIndexMatch.Success)
                        {
                            int indexLength = TagTreeNodeIndexMatch.Groups["index"].Value.Length;
                            String value = indexLength == 0 ? match.Value : match.Value.Remove(match.Value.Length - indexLength, indexLength);

                            if (level == 0)
                            {
                                string keyPlcTagInstanceInfo = !string.IsNullOrWhiteSpace(_ModuleName)  ? _ModuleName + "." + value : value;
                                    
                                if (s.BuildInfoMaps && s.m_mapPlcTagInstanceInfo.ContainsKey(keyPlcTagInstanceInfo))
                                {
                                    ushortUnion ushortAux;
                                    // Class == Program
                                    if (s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nProgramInstance != 0)
                                    {
                                        pdu[pduPointer.USHORT++] = 0x20;
                                        pdu[pduPointer.USHORT++] = 0x68;
                                        // Program instance (2 bytes)
                                        pdu[pduPointer.USHORT++] = 0x25;
                                        pdu[pduPointer.USHORT++] = 0;
                                        ushortAux = new ushortUnion(s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nProgramInstance);
                                        pdu[pduPointer.USHORT++] = ushortAux.LOBYTE;
                                        pdu[pduPointer.USHORT++] = ushortAux.HIBYTE;
                                    }

                                    // Add the instance of the variable
                                    // Class == Symbol Object
                                    pdu[pduPointer.USHORT++] = 0x20;
                                    pdu[pduPointer.USHORT++] = 0x6b;
                                    // Program instance (2 bytes)
                                    pdu[pduPointer.USHORT++] = 0x25;
                                    pdu[pduPointer.USHORT++] = 0;
                                    ushortAux = new ushortUnion(s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nInstance);
                                    pdu[pduPointer.USHORT++] = ushortAux.LOBYTE;
                                    pdu[pduPointer.USHORT++] = ushortAux.HIBYTE;
                                    // Is a structure or an array of structures?
                                    nParentTemplateInstance = s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nTemplateInstance;
                                }
                                else
                                {
                                    if (_ModuleName != string.Empty)
                                        GetSymbol(_ModuleName, pdu, ref pduPointer);

                                    GetSymbol(value, pdu, ref pduPointer);
                                }
                                    
                            }
                            else
                            {
                                if (nParentTemplateInstance > 0 && s.m_mapPlcTemplateInfo.ContainsKey(nParentTemplateInstance))
                                {
                                    if (s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo.ContainsKey(value))
                                    {
                                        writeArrayIndex(ref pdu, ref  pduPointer, Convert.ToUInt16(s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo[value].m_nIndex));
                                        nParentTemplateInstance = s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo[value].m_nTemplateInstance;
                                    }
                                    else
                                    {
                                        nParentTemplateInstance = 0;
                                        GetSymbol(value, pdu, ref pduPointer);
                                    }
                                }
                                else
                                {
                                    nParentTemplateInstance = 0;
                                    GetSymbol(value, pdu, ref pduPointer);
                                }
                            }

                            //Management of Array Index 
                            if (indexLength > 0)
                            { 
                                Match TagTreeIndexMatch = TagTreeIndexValidator.Match(TagTreeNodeIndexMatch.Groups["index"].Value);
                                if (TagTreeIndexMatch.Success)
                                {
                                    if (TagTreeIndexMatch.Groups["index1"].Value.Length > 0)
                                    {
                                        int memIndex = pduPointer.USHORT;
                                        TagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index1"].Value);
                                        writeArrayIndex(ref pdu, ref pduPointer, TagIndexRequest);
                                        TagIndexRequestSize = pduPointer.USHORT - memIndex;
                                        if (TagTreeIndexMatch.Groups["index2"].Value.Length > 0)
                                        {
                                            memIndex = pduPointer.USHORT;
                                            TagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index2"].Value.Remove(0, 1));
                                            writeArrayIndex(ref pdu, ref pduPointer, TagIndexRequest);
                                            TagIndexRequestSize = pduPointer.USHORT - memIndex;
                                            if (TagTreeIndexMatch.Groups["index3"].Value.Length > 0)
                                            {
                                                memIndex = pduPointer.USHORT;
                                                TagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index3"].Value.Remove(0, 1));
                                                writeArrayIndex(ref pdu, ref pduPointer, TagIndexRequest);
                                                TagIndexRequestSize = pduPointer.USHORT - memIndex;
                                            }
                                        }
                                    }
                                }
                            }
                            level++;
                        }
                    }
                }
                else
                {
                    pduPointer.USHORT = sizeWords;
                    return 0;
                }


                _NodeFormat = _TagFormat;
                if (_TagFormat == TagFormats.STRUCTURE)
                {
                    _NodeFormat = GetEthType(tag.TagNode.DataType.Identifier.ToString());
                    string Tree = EtherNetIpProtocol.GetDynTagTree(tag.DynSettings as EtherNetIPDynTagSettings);
                    EtherNetIpProtocol.getOffTree(ref Tree);
                    while (!string.IsNullOrEmpty(Tree) )
                    {
                        GetSymbol(EtherNetIpProtocol.getOffTree(ref Tree), pdu, ref pduPointer);
                    }
                }

                TagNameRequestBuffer = new byte[pduPointer.USHORT - sizeWords];

                Array.Copy(pdu, sizeWords, TagNameRequestBuffer, 0, TagNameRequestBuffer.Count());
                BuildInfoMaps = s.BuildInfoMaps;
            }
            else
            {
                Array.Copy(TagNameRequestBuffer, 0, pdu, pduPointer.USHORT, TagNameRequestBuffer.Count());
                pduPointer.USHORT += (ushort)TagNameRequestBuffer.Count();
            }

            if (PartialArrayEnd != PartialArrayStart)
            {
                pduPointer.USHORT = (ushort)(pduPointer.USHORT - TagIndexRequestSize);
                writeArrayIndex(ref pdu, ref pduPointer, (ushort)(PartialArrayStart + TagIndexRequest));
            }

            if (!string.IsNullOrEmpty(subName))
            {
                GetSymbol(subName, pdu, ref pduPointer);
            }

            pdu[sizeWords] = (byte)((pduPointer.USHORT - sizeWords) / 2);

            return (ushort)(pduPointer.USHORT - sizeWords);
        }

        public ushort GetAsciiTagNameLength(EtherNetIPStation s, Tag tag, string subName = "")
        {
            ushort pduLength = 0;
            ushort sizeWords = 0;
            ushort localTagIndexRequest = TagIndexRequest;
            int localTagIndexRequestSize = TagIndexRequestSize;
            if (TagNameRequestBuffer == null || BuildInfoMaps != s.BuildInfoMaps)
            {
                pduLength++;

                Regex TagNodeParser = new Regex(@"\b(?<TagTree>[^\.]+)?");
                MatchCollection TagNodeMatches = TagNodeParser.Matches(_TagName);

                if (TagNodeMatches.Count > 0)
                {
                    ushort nParentTemplateInstance = 0;
                    int level = 0;
                    Regex TagTreeNodeIndex = new Regex(@"^([^\[]+)(?<index>\[[\d,]+\])?");
                    Regex TagTreeIndexValidator = new Regex(@"^\[(?<index1>\d+)(?<index2>,\d+)?(?<index3>,\d+)?\]$");
                    foreach (Match match in TagNodeMatches)
                    {
                        Match TagTreeNodeIndexMatch = TagTreeNodeIndex.Match(match.Value);
                        if (TagTreeNodeIndexMatch.Success)
                        {
                            int indexLength = TagTreeNodeIndexMatch.Groups["index"].Value.Length;
                            if (indexLength == 0)
                            {
                                if (level == 0)
                                {
                                    string keyPlcTagInstanceInfo = !string.IsNullOrWhiteSpace(_ModuleName) ? _ModuleName + "." + match.Value : match.Value;
                                    if (s.BuildInfoMaps && s.m_mapPlcTagInstanceInfo.ContainsKey(keyPlcTagInstanceInfo))
                                    {
                                        // Class == Program
                                        if (s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nProgramInstance != 0)
                                        {
                                            pduLength += EtherNetIpProtocol.CIP_CLASS_INFO_LENGTH;
                                            // Program instance (2 bytes)
                                            pduLength += EtherNetIpProtocol.CIP_2BYTES_INSTANCE_INFO_LENGTH;
                                        }

                                        // Add the instance of the variable
                                        // Class == Symbol Object
                                        pduLength += EtherNetIpProtocol.CIP_CLASS_INFO_LENGTH;
                                        // Program instance (2 bytes)
                                        pduLength += EtherNetIpProtocol.CIP_2BYTES_INSTANCE_INFO_LENGTH;
                                        // Is a structure or an array of structures?
                                        nParentTemplateInstance = s.m_mapPlcTagInstanceInfo[keyPlcTagInstanceInfo].m_nTemplateInstance;
                                    }
                                    else
                                    {
                                        if (_ModuleName != string.Empty)
                                            pduLength += GetSymbolLength(_ModuleName);

                                        pduLength += GetSymbolLength(match.Value);
                                    }
                                }
                                else
                                {
                                    if (nParentTemplateInstance > 0 && s.m_mapPlcTemplateInfo.ContainsKey(nParentTemplateInstance))
                                    {
                                        if (s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo.ContainsKey(match.Value))
                                        {
                                            System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength call 1 to addArrayLength", DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                                            pduLength += addArrayLength(Convert.ToUInt16(s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo[match.Value].m_nIndex));
                                            nParentTemplateInstance = s.m_mapPlcTemplateInfo[nParentTemplateInstance].m_mapFieldInfo[match.Value].m_nTemplateInstance;
                                        }
                                        else
                                        {
                                            nParentTemplateInstance = 0;
                                            pduLength += GetSymbolLength(match.Value);
                                        }
                                    }
                                    else
                                    {
                                        nParentTemplateInstance = 0;
                                        pduLength += GetSymbolLength(match.Value);
                                    }
                                }
                            }
                            else
                            {
                                if (_ModuleName != string.Empty)
                                    pduLength += GetSymbolLength(_ModuleName);
                                pduLength += GetSymbolLength(match.Value.Remove(match.Value.Length - indexLength, indexLength));
                                Match TagTreeIndexMatch = TagTreeIndexValidator.Match(TagTreeNodeIndexMatch.Groups["index"].Value);
                                if (TagTreeIndexMatch.Success)
                                {
                                    if (TagTreeIndexMatch.Groups["index1"].Value.Length > 0)
                                    {
                                        int memIndex = pduLength;
                                        localTagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index1"].Value);
                                        System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength call 2 to addArrayLength", DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                                        pduLength += addArrayLength(localTagIndexRequest);
                                        localTagIndexRequestSize = pduLength - memIndex;
                                        if (TagTreeIndexMatch.Groups["index2"].Value.Length > 0)
                                        {
                                            memIndex = pduLength;
                                            localTagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index2"].Value.Remove(0, 1));
                                            System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength call 3 to addArrayLength", DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                                            pduLength += addArrayLength(localTagIndexRequest);
                                            localTagIndexRequestSize = pduLength - memIndex;
                                            if (TagTreeIndexMatch.Groups["index3"].Value.Length > 0)
                                            {
                                                memIndex = pduLength;
                                                localTagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index3"].Value.Remove(0, 1));
                                                System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength call 4 to addArrayLength", DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                                                pduLength += addArrayLength(localTagIndexRequest);
                                                localTagIndexRequestSize = pduLength - memIndex;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    pduLength = sizeWords;
                                    return 0;
                                }
                            }
                            level++;
                        }
                    }
                }
                else
                {
                    pduLength = sizeWords;
                    return 0;
                }

                _NodeFormat = _TagFormat;
                if (_TagFormat == TagFormats.STRUCTURE)
                {
                    _NodeFormat = GetEthType(tag.TagNode.DataType.Identifier.ToString());
                    string Tree = EtherNetIpProtocol.GetDynTagTree(tag.DynSettings as EtherNetIPDynTagSettings);
                    EtherNetIpProtocol.getOffTree(ref Tree);
                    while (!string.IsNullOrEmpty(Tree))
                    {
                        pduLength += GetSymbolLength(EtherNetIpProtocol.getOffTree(ref Tree));
                    }
                }
            }
            else
            {
                pduLength += (ushort)TagNameRequestBuffer.Count();
            }

            System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength - PartialArrayStart = {1} - PartialArrayEnd = {2} - localTagIndexRequest = {3} - localTagIndexRequestSize = {4}", DateTime.UtcNow.ToString("HH:mm:ss.fff"), PartialArrayStart, PartialArrayEnd, localTagIndexRequest, localTagIndexRequestSize));

            if (PartialArrayStart > 0)
            {
                pduLength = (ushort)(pduLength - localTagIndexRequestSize);
                System.Diagnostics.Debug.WriteLine(String.Format("DBG REQ LENGTH - {0} - GetAsciiTagNameLength call 5 to addArrayLength", DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                pduLength += addArrayLength((ushort)(PartialArrayStart + localTagIndexRequest));
            }

            if (!string.IsNullOrEmpty(subName))
            {
                pduLength += GetSymbolLength(subName);
            }

            return (ushort)(pduLength - sizeWords);
        }

        void writeArrayIndex(ref byte[] pdu, ref ushortUnion pduPointer, ushort index)
        {
            if (index > 0xff)
            {
                ushortUnion longIndex = new ushortUnion(index);
                pdu[pduPointer.USHORT++] = 0x29;
                pdu[pduPointer.USHORT++] = 0x0;
                pdu[pduPointer.USHORT++] = longIndex.LOBYTE;
                pdu[pduPointer.USHORT++] = longIndex.HIBYTE;
            }
            else
            {
                pdu[pduPointer.USHORT++] = 0x28;
                pdu[pduPointer.USHORT++] = (byte)index;
            }

        }

        byte addArrayLength(ushort index)
        {
            if (index > Byte.MaxValue)
            {
                return (EtherNetIpProtocol.CIP_ARRAY_INDEX_LONG_LENGTH);
            }
            else
            {
                return (EtherNetIpProtocol.CIP_ARRAY_INDEX_SHORT_LENGTH);
            }
        }


        public static TagFormats GetEthType(string Identifier)
        {
            switch (Identifier)
            {
                case "1":
                    return TagFormats.BOOL;
                case "2":
                case "3":
                    return TagFormats.SINT;
                case "4":
                case "5":
                    return TagFormats.INT;
                case "6":
                case "7":
                    return TagFormats.DINT;
                case "8":
                    return TagFormats.LINT;
                case "9":
                    return TagFormats.ULINT;
                case "10":
                case "11":
                    return TagFormats.REAL;
                case "12":
                    return TagFormats.STRING;
            }
            return TagFormats.BOOL;
        }


        public static void GetSymbol(string symbol, byte[] pdu, ref ushortUnion pduPointer)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            pdu[pduPointer.USHORT++] = 0x91; //Extended Symbol Segment
            ushort sizeIndex = pduPointer.USHORT;
            pdu[pduPointer.USHORT++] = 0;
            byte[] unicodeBytes = unicode.GetBytes(symbol);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            while (pdu[sizeIndex] < (byte)asciiBytes.Length && asciiBytes[pdu[sizeIndex]] != 0)
            {
                pdu[pduPointer.USHORT++] = asciiBytes[pdu[sizeIndex]++];
            }
            if (pdu[sizeIndex] % 2 == 1)
            {
                pdu[pduPointer.USHORT++] = 0;
            }
        }

        public static ushort GetSymbolLength(string symbol)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            ushort symbolLength = 2; //Extended Symbol Segment
            ushort sizeIndex = 0;
            byte[] unicodeBytes = unicode.GetBytes(symbol);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            while (sizeIndex < (byte)asciiBytes.Length && asciiBytes[sizeIndex] != 0)
            {
                symbolLength++;
                sizeIndex++;
            }
            if (sizeIndex % 2 == 1)
            {
                symbolLength++;
            }
            return (symbolLength);
        }

        private ushort GetSymbolSize(string symbol)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            byte[] unicodeBytes = unicode.GetBytes(symbol);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            return (ushort)(asciiBytes.Length + asciiBytes.Length % 2 + 2);
        }

        public ushort GetTagReadBufferSize(Tag tag, string subName = "")
        {
            EtherNetIPStation s = (EtherNetIPStation)Station;
            return (GetAsciiTagNameLength(s, tag, subName));
        }

        public bool IsTypeAdmitted(NodeId type, bool IsArray = false)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if( IsArray && 
                    (AddressType == AddressTypes.DataFile) &&
                    (nType == (uint)BuiltInType.String))
                {
                    return false;
                }
                if (nType == (uint)BuiltInType.Boolean ||
                    nType == (uint)BuiltInType.Byte ||
                    nType == (uint)BuiltInType.Double ||
                    nType == (uint)BuiltInType.Float ||
                    nType == (uint)BuiltInType.Int16 ||
                    nType == (uint)BuiltInType.Int32 ||
                    nType == (uint)BuiltInType.Int64 ||
                    nType == (uint)BuiltInType.Integer ||
                    nType == (uint)BuiltInType.SByte ||
                    nType == (uint)BuiltInType.UInt16 ||
                    nType == (uint)BuiltInType.UInt32 ||
                    nType == (uint)BuiltInType.UInt64 ||
                    nType == (uint)BuiltInType.UInteger||
                    nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }

            return false;
        }

        public bool IsFileTypeIO()
        {
            return((FileType == FileTypes.Output) || (FileType == FileTypes.Input));
        }

        public bool IsFileTypeTC()
        {
            return ((FileType == FileTypes.Counter) || (FileType == FileTypes.Timer));
        }


        public String ABString(uint ByteOffset)
        {
            String strFileTypes = "OISBTCRNF";
            String strAddress;
            uint ElemOffset;
            uint BitOffset = 0;
            if (DataFormat != DataFormats.BIT)
            {
                ElemOffset = ByteOffset / ElemSize;
            }
            else
            {
                ElemOffset = (ByteOffset + Bit) / 16;
                BitOffset = (ByteOffset + Bit) % 16;
            }
            
            switch(FileType)
            {
                case FileTypes.Counter:
                case FileTypes.Timer:
                    strAddress = string.Format("{0}{1}:{2}/{3}", strFileTypes[(byte)FileType], FileNum, Element + ElemOffset, SubElement);
                    break;

                case FileTypes.Input:
                case FileTypes.Output:
                    if ((Word + ElemOffset) == 0)
                    {
                        strAddress = string.Format("{0}{1}:{2}", strFileTypes[(byte)FileType],FileNum,Slot);
                    }
                    else
                    {
                        strAddress = string.Format("{0}{1}:{2}.{3}", strFileTypes[(byte)FileType], FileNum, Slot, Word + ElemOffset);
                    }
                    if(DataFormat == DataFormats.BIT)
                    {
                        strAddress = string.Format("{0}/{1}", strAddress, Bit + BitOffset);
                    }
                    break;

                case FileTypes.Status:
                case FileTypes.Binary:
                case FileTypes.Integer:
                    strAddress = string.Format("{0}{1}:{2}.{3}", strFileTypes[(byte)FileType], FileNum, Slot, Word + ElemOffset);
                    if(DataFormat == DataFormats.BIT)
                    {
                        strAddress = string.Format("{0}/{1}", strAddress, Bit + BitOffset);
                    }
                    break;
 
                case FileTypes.Control:
                    strAddress = string.Format("{0}{1}:{2}", strFileTypes[(byte)FileType], FileNum, Element + ElemOffset);
                    if(DataFormat == DataFormats.BIT)
                    {
                        strAddress = string.Format("{0}/{1}", strAddress, Bit + BitOffset);
                    }
                    break;

                case FileTypes.Float:
                    strAddress = string.Format("{0}{1}:{2}", strFileTypes[(byte)FileType], FileNum, Element + ElemOffset);
                    break;

                default:
                    strAddress = "";
                    break;
            }
            return strAddress;
        }

        //public uint GetReadRequestLength()
        //{
        //    return EtherNetIpProtocol.MIN_READ_REQUEST_LENGTH;
        //}

        //public uint GetReadResponseLength()
        //{
        //    if (!IsValid)
        //        return 0;

        //    return _ElemSize;
        //}


        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        return 1;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        return 2;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        return 4;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        return 8;
                    case (uint)BuiltInType.String:
                        return (uint)Length;
                        
                    default:
                        return 0;
                }
            }
            return 0;
        }

        List<DriverBaseInterfaces.TagDefinition> GetSimpleTagList(DriverBaseInterfaces.TagDefinition t)
        {
            List<DriverBaseInterfaces.TagDefinition> l = new List<DriverBaseInterfaces.TagDefinition>();
            if (t.DataType.IdType == IdType.Guid)
            {
                //prototype?
                List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
                Station.GetCommDriver().OnTagPrototypeQuery(t.NodeId, ref tList);
                foreach (var a in tList)
                {
                    l.AddRange(GetSimpleTagList(a));
                }
            }
            else
                l.Add(t);
            return l;
        }

        private void CheckJobValid()
        {
            if (TagsList.Count() == 0)
            {
                IsValid = false;
                InvalidReason = Properties.Resources.ErrorJobEmptyMicro800;
                return;
            }

            if ((Station as EtherNetIPStation).PlcType ==  PlcTypes.Micro800_series)
            {
                if (AddressType == AddressTypes.DataFile)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorInvalidAddressMicro800, TagsList[0].TagNode.NodeId.ToString());
                    return;
                }
                switch (TagFormat)
                {
                    case TagFormats.BOOL:
                    case TagFormats.SINT:
                    case TagFormats.DINT:
                    case TagFormats.INT:
                    case TagFormats.REAL:
                    case TagFormats.STRING:
                    case TagFormats.LINT:
                    case TagFormats.ULINT:
                    case TagFormats.LWORD:
                        break;
                    default:
                        IsValid = false;
                        InvalidReason = string.Format(Properties.Resources.ErrorInvalidFormatMicro800, TagsList[0].TagNode.NodeId.ToString(), TagFormat.ToString());
                        return;
                }

            }
            if((AddressType == AddressTypes.DataFile))
            {
                if ((FileType == FileTypes.LongInteger) && ((Station as EtherNetIPStation).PlcType == PlcTypes.PLC5))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorInvalidStationMicro800, TagsList[0].TagNode.NodeId.ToString(), FileType.ToString());
                    Station.GetCommDriver().OnSystemEvent(null,
                                  string.Format(Properties.Resources.ErrorInvalidTypeForStation, TagsList[0].TagNode.NodeId.ToString(), FileType.ToString()),
                                  Opc.Ua.EventSeverity.Max);
                    return;
                }
                else if(((FileType == FileTypes.LongInteger) || (FileType == FileTypes.String)) && ((Station as EtherNetIPStation).PlcType == PlcTypes.ControlLogix_CompactLogix))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorInvalidStationMicro800, TagsList[0].TagNode.NodeId.ToString(), FileType.ToString());
                    Station.GetCommDriver().OnSystemEvent(null,
                                  string.Format(Properties.Resources.ErrorInvalidTypeForStation, TagsList[0].TagNode.NodeId.ToString(), FileType.ToString()),
                                  Opc.Ua.EventSeverity.Max);
                    return;
                }
            }
            //if (((Station as EtherNetIPStation).PlcType != PlcTypes.SLC500_MicroLogix) && ((Station as EtherNetIPStation).PlcType != PlcTypes.PLC5) &&
            //    (AddressType == AddressTypes.DataFile) && ((FileType == FileTypes.LongInteger) || (FileType == FileTypes.String)))
            //{
            //    IsValid = false;
            //    InvalidReason = string.Format(Properties.Resources.ErrorInvalidStationMicro800 , TagsList[0].TagNode.NodeId.ToString(), FileType.ToString());
            //    Station.GetCommDriver().OnSystemEvent(null, 
            //                  string.Format(Properties.Resources.ErrorInvalidTypeForStation, TagsList[0].TagNode.NodeId.ToString(), FileType.ToString()), 
            //                  Opc.Ua.EventSeverity.Max);
            //    return;
            //}

            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType, (t.ArrayDimension > 0 )))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorInvalidTypeTagMicro800, t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }
                size += GetTagSize(t);
            }
 
            if (! _ParseOk )
            {
                IsValid = false;
                InvalidReason = Properties.Resources.ParseKo;
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorExcedeJobMicro800, tagnamelist);
                return;
            }
            uint DataTypeBitSize = TagsList[0].TagNode.ArrayDimension == 0 ? 1 : TagsList[0].TagNode.ArrayDimension;
            if (ElementNumber == 0)
                DataTypeBitSize = DataTypeBitSize * GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);

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
            //if (DataFormat == DataFormats.BIT && AddressType == AddressTypes.DataFile && (DataTypeBitSize > 16 || (Bit != 0 && DataTypeBitSize > 1)))
            if (DataFormat == DataFormats.BIT && AddressType == AddressTypes.DataFile && (bDataTypeBitSize || (Bit != 0 && DataTypeBitSize > 1)))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorBooleanArrayIsNotAcceptMicro800, tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public override uint getProtocolDataType()
        {
            switch (EtherNetIpProtocol.DataType(_DataFormat))
            {
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                default:
                    return 0;
            }
        }
        public override uint GetMaxJobSize()
        {
            if(AddressType == AddressTypes.TagName)
            {
                return EtherNetIpProtocol.MAX_DATA_SIZE;
            }
            else
            {
                if (DataFormat == DataFormats.BIT)
                    return (EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE * 8);
                else
                    return EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE;
            }
        }
 
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            EtherNetIPCommJob testJob = candJob as EtherNetIPCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if ((testJob.AddressType != _AddressType) ||
               (testJob.AddressType != AddressTypes.DataFile))
                return JobAggregationType.JobAggregImpossible;

            if (testJob.Type != Type || testJob.FileNum != FileNum
                || testJob.DataFormat != DataFormat)
                return JobAggregationType.JobAggregImpossible;

            if (testJob.FileType != FileType)
                return JobAggregationType.JobAggregImpossible;

            if (testJob.Slot != Slot)
                return JobAggregationType.JobAggregImpossible;

            if (IsFileTypeTC() || FileTypes.Control == FileType)
                return JobAggregationType.JobAggregImpossible;
           
            if (!_ParseOk )
            {
                return JobAggregationType.JobAggregImpossible;
            }

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                return JobAggregationType.JobAggregImpossible;

            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = _Element;
            uint startTest = testJob.Element;

            if (((EtherNetIPStation)Station).PlcType != PlcTypes.SLC500_MicroLogix)
            {
                //if only input tasks are performed, it is possible to read more words simultaneously, 
                //but if writing tasks are also performed it is necessary to divided and group bits that are contained in the same word, 
                //this limitation is dictated by the Function code command (0xAB) bit writing with the mask .
                if ((Type != LinkType.Input) && (DataFormat == DataFormats.BIT))
                {
                    // The element must be the same
                    if ((_Element != testJob.Element) || (_Word != testJob.Word))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }

                    // The word must be the same
                    if ((testJob.Bit < 16) && (Bit > 15) ||
                        (testJob.Bit > 15) && (Bit < 16))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            if (IsFileTypeIO())
            {
                if (DataFormat != DataFormats.BIT)
                {
                    start = _Word;
                    startTest = testJob.Word;
                }
                else
                {
                    start = _Word * 16 + _Bit;
                    startTest = testJob.Word * 16 + testJob.Bit;
                    Granularity *= 16;
                }
            }
            else if (DataFormat == DataFormats.BIT)
            {
                if (FileType == FileTypes.LongInteger)
                {
                    start = _Element * 32 + _Bit;
                    startTest = testJob.Element * 32 + testJob.Bit;
                    Granularity *= 32;
                }
                else
                {
                    start = _Element * 16 + _Bit;
                    startTest = testJob.Element * 16 + testJob.Bit;
                    Granularity *= 16;
                }
            }

            uint end = start + (TotalJobSize / _ElemSize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / _ElemSize) - 1;

            //////// dont' aggregate bit of differente word in a word/double-word Area 
            //if (((EtherNetIPStation)Station).PlcType == PlcTypes.SLC500_MicroLogix)
            //{
            //    if (DataFormat == DataFormats.BIT && (FileType == FileTypes.Integer || FileType == FileTypes.LongInteger))
            //    {
            //        if (_Element != testJob.Element)
            //            return JobAggregationType.JobAggregImpossible;
            //    }
            //}

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / _ElemSize;
                int endTag = (int)(startTag + (tag.Size / _ElemSize) - 1);
                if ((startTest >= startTag && (int)startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }
 
            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) * _ElemSize;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset = newoffset;
                    newoffset += candJob.TagsList[i].Size;
                }
                return JobAggregationType.JobAggregFits;
            }

            if (startTest >= start && startTest <= end + Granularity)
                if (((endTest - start + 1) * _ElemSize) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = (startTest - start) * _ElemSize;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (endTest - end) * _ElemSize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
                if (((end - startTest + 1) * _ElemSize) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (start - startTest) * _ElemSize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    for(int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new EtherNetIPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                     break;
                case JobAggregationType.JobAggregForward:
                    for(int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new EtherNetIPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    for(int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset += ExtraBytes;
                        TagsList.Add(new EtherNetIPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }

                    TotalJobSize += ExtraBytes;


                    if (IsFileTypeIO())
                    {
                        _Word = ((EtherNetIPCommJob)candJob).Word;
                        if (DataFormat == DataFormats.BIT)
                        {
                            _Bit = ((EtherNetIPCommJob)candJob).Bit;
                        }
                    }
                    else
                    {
                        _Element = ((EtherNetIPCommJob)candJob).Element;
                    }

                    break;
                default:
                    return false;
            }
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            return true;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();
            Tag startTag = null;
            List<byte> outData = new List<byte>();

            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    //ExecuteTask = false;
                    TerminateTask = false;
                    return;
                }

                listToWrite.AddRange(TagsListOnWriting);
                startTag = listToWrite[0];

                //prepare a write request
                listToWrite.Sort(CompareTagByOffset);
                Tag cand = null;
                byte[] jobdata;
                UInt16 nData = 0;
                bool bLookForFirstTag = true;
                do
                {
                    if (cand != null)
                    {
                        //Condition to insert in the writing list more tags that belong to the same word and slot
                        if (((EtherNetIPStation)Station).PlcType == PlcTypes.SLC500_MicroLogix && CommandType == CommandTypes.WriteCmd && this.DataFormat == DataFormats.BIT)
                        {
                            switch (this.FileType)
                            {
                                case FileTypes.Integer:
                                    // for bit area, ByteOffset is the nr of bit from the 1st tag on the list
                                    if (cand.ByteOffset / 16 != listToWrite[0].ByteOffset / 16)
                                        break;
                                    break;
                                case FileTypes.LongInteger:
                                    // for bit area, ByteOffset is the nr of bit from the 1st tag on the list
                                    if (cand.ByteOffset / 32 != listToWrite[0].ByteOffset / 32)
                                        break;
                                    break;
                                default:
                                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                                        break;
                                    break;
                            }                            
                        }
                        else if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                            break;
                    }
                    cand = listToWrite[0];
                    listToWrite.Remove(cand);
                    if (bLookForFirstTag && cand != startTag)
                    {
                        cand = null;
                        continue;
                    }
                    bLookForFirstTag = false;

                    if (!listOnWriting.Contains(cand))
                        listOnWriting.Add(cand);
                    if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        nData = (UInt16)((cand.Size + 7) / 8);
                    else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                    {
                        if (cand.TagNode.ArrayDimension == 0)
                            nData = (ushort)(GetProtocolDataByteSize());
                        else
                            nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
                    }
                    else
                        nData = (UInt16)cand.Size;

                    lock (lockListObject)
                    {
                        jobdata = new byte[nData];
                        EtherNetIPStation StationEth = (EtherNetIPStation)Station;
                        if (StationEth.PlcType == PlcTypes.SLC500_MicroLogix)
                        {
                            EtherNetIPTag candEth = (EtherNetIPTag) cand;
                            candEth.GetTagBufferSLC500_MicroLogix(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        }
                        else
                        {
                            cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                        }
                    }

                    uint ArraySize = cand.TagNode.ArrayDimension;
                    if (ArraySize == 0)
                        ArraySize = 1;
                    if (ProtocolDataSizeBig())
                    {
                        List<byte> correctData = new List<byte>();
                        UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                        UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            byte[] tmpdata = new byte[sizeDataType];
                            if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                                Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                            else
                            {
                                if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                                    tmpdata[0] = 0;
                                else
                                    tmpdata[0] = 1;
                            }
                            cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                            correctData.AddRange(tmpdata);
                        }
                        jobdata = correctData.ToArray();
                    }
                    else if (isProtocolBool())
                    {
                        if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        {
                            byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
                            for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
                            {
                                if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
                                    tmpData[ArrayIndex] = 1;
                            }
                            jobdata = tmpData;
                        }
                    }

                    if (SwapBytes)
                    {
                        SwapByteBuffer(ref jobdata);
                    }
                    if (SwapWords)
                    {
                        SwapWordBuffer(ref jobdata);
                    }

                    outData.AddRange(jobdata);

                } while (listToWrite.Count > 0);

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();
                listToWrite.Clear();
            }

            if (isProtocolBool())
            {
                byte[] tmpData = new byte[(outData.Count() + 7) / 8];
                for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
                {
                    if (outData[ArrayIndex] != 0)
                        tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                }
                jobData = tmpData;
            }
            else
                jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            //for (int i = 0; i < TagsList.Count; i++)
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (AddressType != AddressTypes.DataFile)
                            {
                                switch (((EtherNetIPStation)Station).PlcType)
                                {
                                    case PlcTypes.Micro800_series:
                                        EtherNetIPTag candTag = TagsList[TagIndex] as EtherNetIPTag;                                        
                                        if (candTag.SetTagValueSLC500_MicroLogix(ref rec, (int)candTag.ByteOffset))
                                        {
                                            changed.Add(TagsList[TagIndex]);
                                        }
                                        break;
                                    default:
                                        if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                        {
                                            changed.Add(TagsList[TagIndex]);
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (((EtherNetIPTag)TagsList[TagIndex]).SetTagValueSLC500_MicroLogix (ref rec, (int)TagsList[TagIndex].ByteOffset))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (AddressType != AddressTypes.DataFile)
                            {
                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                            else
                            {
                                if (((EtherNetIPTag)TagsList[TagIndex]).SetTagValueSLC500_MicroLogix(ref tmpData, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (AddressType != AddressTypes.DataFile)
                            {
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                            else
                            {
                                if (((EtherNetIPTag)TagsList[TagIndex]).SetTagValueSLC500_MicroLogix(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                        else
                        {
                            if (!ProtocolDataSizeBig())
                            {
                                uint elemsize = 0;
                                if (ElementNumber > 0)
                                {
                                    elemsize = GetProtocolDataByteSize();
                                }

                                if (AddressType != AddressTypes.DataFile)
                                {
                                    if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    {
                                        changed.Add(TagsList[TagIndex]);
                                    }
                                }
                                else
                                {
                                    if (((EtherNetIPTag)TagsList[TagIndex]).SetTagValueSLC500_MicroLogix(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    {
                                        changed.Add(TagsList[TagIndex]);
                                    }
                                }
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (AddressType != AddressTypes.DataFile)
                                {
                                    if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    {
                                        changed.Add(TagsList[TagIndex]);
                                    }
                                }
                                else
                                {
                                    if (((EtherNetIPTag)TagsList[TagIndex]).SetTagValueSLC500_MicroLogix(ref tmpData, 0))
                                    {
                                        changed.Add(TagsList[TagIndex]);
                                    }
                                }
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public override bool IsJobAggregable()
        {
            return (AddressType == AddressTypes.DataFile);
        }

        public override bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return (IsJobAggregable() && base.InputOutputRequiresReadAfterContinuosWriteBase());
        }

        public static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        /// <summary>
        /// Return the nr of bit from job's initial address
        /// </summary>
        /// <returns></returns>
        public uint GetNrBitsFromInitialAddressFrom1stTagToWrite()
        {
            if (this.GetTagListOnWritingCount() > 0)
                return TagsListOnWriting[0].ByteOffset;
            else
                return 0;
        }

        public uint GetFirstBitNrFromLastTagToWrite()
        {
            if (this.GetTagListOnWritingCount() > 0)
                return ((EtherNetIPTag)TagsListOnWriting[0]).EtherNetIPDynSettings.Bit;
            else
                return 0;
        }

        public uint GetLastBitNrFromLastTagToWrite()
        {
            if (this.GetTagListOnWritingCount() > 0)
                return ((EtherNetIPTag)TagsListOnWriting[TagsListOnWriting.Count - 1]).EtherNetIPDynSettings.Bit;
            else
                return 0;
        }

        public uint GetFirstElementFromLastTagToWrite()
        {
            if (this.GetTagListOnWritingCount() > 0)
                return ((EtherNetIPTag)TagsListOnWriting[0]).EtherNetIPDynSettings.Element;
            else
                return 0;
        }
        #endregion

        #region Properties
        private AddressTypes _AddressType;
        public AddressTypes AddressType
        {
            get { return _AddressType; }
            set { _AddressType = value; }
        }

        private TagFormats _TagFormat;
        public TagFormats TagFormat
        {
            get { return _TagFormat; }
            set { _TagFormat = value; }
        }

        private string _ABAddress;
        public string ABAddress
        {           
            get { return _ABAddress; }
            set { _ABAddress = value; }
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

        public int Length
        {
            get { return _Dim0 * _Dim1 * _Dim2; }
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
            set { _Word = value; }
        }

        private uint _Element;
        public uint Element
        {
            get { return _Element; }
            set { _Element = value; }
        }


        private uint _Bit;
        public uint Bit
        {
            get { return _Bit; }
            set { _Bit = value; }
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

        public CommandTypes CommandType { get; set; }

        /*
        private EtherNetIPCommJobSettings _JobSettings;
        public EtherNetIPCommJobSettings JobSettings
        {
            get 
            {
                if ((Settings as EtherNetIPCommJobSettings) != null)
                    _JobSettings = (EtherNetIPCommJobSettings)Settings;
                
                return _JobSettings; 
            }
        }
        */

        #endregion
    }
}
