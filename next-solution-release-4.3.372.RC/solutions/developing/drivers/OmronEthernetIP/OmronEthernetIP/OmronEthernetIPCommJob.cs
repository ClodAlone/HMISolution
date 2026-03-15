using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Text;
using System.Text.RegularExpressions;

namespace OmronEthernetIP
{
    public class OmronEthernetIPCommJob : CommJob
    {
        #region Constructors
        public OmronEthernetIPCommJob(Station station, OmronEthernetIPCommJobSettings settings)
            : base(station, settings)
        {
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
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;

            InitOmronEthernetIPCommJob();
            CheckJobValid();
        }

        public OmronEthernetIPCommJob(Station station, OmronEthernetIPTag defTag)
            : base(station, defTag)
        {
            //The condition serves to avoid an exception in case the Tag parese,
            //returns false by the "OmronEthernetIPTag::BuildDynamicSettings" method
            if (defTag.bIsValid)
            {
                _TagFormat = defTag.OmronEthernetIPDynSettings.TagFormat;
                _ABAddress = defTag.OmronEthernetIPDynSettings.ABAddress;

                _ParseOk = defTag.OmronEthernetIPDynSettings.ParseOk;
                _DataFormat = defTag.OmronEthernetIPDynSettings.DataFormat;
                _ElemSize = defTag.OmronEthernetIPDynSettings.ElemSize;

                //Symbolic & Physical Address
                _SubElement = defTag.OmronEthernetIPDynSettings.SubElement;

                //Symbolic Address
                _TagName = defTag.OmronEthernetIPDynSettings.TagName;
                _ModuleName = defTag.OmronEthernetIPDynSettings.ModuleName;
                _Dim0 = defTag.OmronEthernetIPDynSettings.Dim0;
                _Dim1 = defTag.OmronEthernetIPDynSettings.Dim1;
                _Dim2 = defTag.OmronEthernetIPDynSettings.Dim2;

                //Physical Address
                _FileType = defTag.OmronEthernetIPDynSettings.FileType;
                _FileNum = defTag.OmronEthernetIPDynSettings.FileNum;
                _Slot = defTag.OmronEthernetIPDynSettings.Slot;
                _Word = defTag.OmronEthernetIPDynSettings.Word;
                _Element = defTag.OmronEthernetIPDynSettings.Element;
                _Bit = defTag.OmronEthernetIPDynSettings.Bit;
                if (ProtocolDataSizeSmall())
                    ElementNumber = 1;

                if (ElementNumber > 0 || ProtocolDataSizeBig())
                {
                    if (defTag.TagNode.ArrayDimension == 0)
                        TotalJobSize = GetProtocolDataByteSize();
                    else
                        TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
                }

                InitOmronEthernetIPCommJob();
                CheckJobValid();
            }
            else
            {
                IsValid = false;
                conditionalVariableHasBeenSet = false;
            }
        }

        public OmronEthernetIPCommJob(Station station)
            : base(station)
        {
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;
            InitOmronEthernetIPCommJob();
            CheckJobValid();

        }

        protected OmronEthernetIPCommJob()
        {
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;
            InitOmronEthernetIPCommJob();
            CheckJobValid();

        }

        public void InitOmronEthernetIPCommJob()
        {
            CommandType = CommandTypes.Invalid;
            ReadTagStart = 0;
            ReadTagEnd = 0;
            PartialArrayStart = 0;
            PartialArrayEnd = 0;
            answer = null;
            ExecuteTask = false;
            TerminateTask = false;
        }

        public void ResetOmronEthernetIPCommJob()
        {
            ReadTagStart = 0;
            ReadTagEnd = 0;
            PartialArrayStart = 0;
            PartialArrayEnd = 0;
            ExecuteTask = false;
            TerminateTask = false;
        }

        #endregion
        #region data Member

        private TagFormats _NodeFormat;
        public ushort ReadTagStart;
        public ushort ReadTagEnd;
        public ushort PartialArrayStart;
        public ushort PartialArrayEnd;
        public byte[] answer;

        public bool ExecuteTask;
        public bool TerminateTask;

        public  byte[] TagNameRequestBuffer;
        public int TagIndexRequestSize = 0;
        public ushort TagIndexRequest = 0;

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
            else
                tagSize = (ushort)cand.Size;

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
            else if(TagFormat == TagFormats.STRING)
            {
                tagSize = (ushort)Encoding.UTF8.GetBytes( cand.Value.ToString()).Length;
            }
            else
                tagSize = (ushort)cand.Size;

            return tagSize;
        }

        public ushort GetNumOfElements(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand)
        {
            ushortUnion NumOfElements;

            if (TagFormat == TagFormats.STRUCTURE)
            {
                NumOfElements = new ushortUnion(1);
            }
            else if (TagFormat == TagFormats.STRING)
            {
                NumOfElements = new ushortUnion((ushort)1);
            }
            else
            {            
                if (PartialArrayEnd == PartialArrayStart)
                {
                    // if last char is brackets, is an array 
                    if (ABAddress.EndsWith("]"))
                    {
                        NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize));
                    }
                    else
                    {
                        NumOfElements = new ushortUnion(1);
                    }                    
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

        public ushort GetWriteNumOfElements(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand)
        {
            ushortUnion NumOfElements;

            if (TagFormat == TagFormats.STRUCTURE)
            {
                NumOfElements = new ushortUnion(1);
            }
            else if (TagFormat == TagFormats.STRING)
            {
                NumOfElements = new ushortUnion((ushort)Encoding.UTF8.GetBytes(cand.Value.ToString()).Length);
                pdu[pduPointer.USHORT++] = 1;
                pdu[pduPointer.USHORT++] = 0;

                pdu[pduPointer.USHORT++] = NumOfElements.LOBYTE;
                pdu[pduPointer.USHORT++] = NumOfElements.HIBYTE;

                return 4;
            }
            else
            {

                if (PartialArrayEnd == PartialArrayStart)
                {
                    if (ABAddress.Contains(']'))
                    {
                        NumOfElements = new ushortUnion((ushort)(adjSize(cand) / ElemSize));
                    }
                    else
                    {
                        NumOfElements = new ushortUnion(1);
                    }
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

        public ushort GetTagFormat(ref byte[] pdu, ref ushortUnion pduPointer)
        {
            return GetTagFormat(ref pdu, ref pduPointer, _TagFormat);
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
                    pdu[pduPointer.USHORT++] = 0xC2;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.USINT:
                    pdu[pduPointer.USHORT++] = 0xC6;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.BYTE:
                    pdu[pduPointer.USHORT++] = 0xD1;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.INT:
                    pdu[pduPointer.USHORT++] = 0xC3;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.UINT:
                    pdu[pduPointer.USHORT++] = 0xC7;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.WORD:
                    pdu[pduPointer.USHORT++] = 0xD2;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.DINT:
                    pdu[pduPointer.USHORT++] = 0xC4;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.UDINT:
                    pdu[pduPointer.USHORT++] = 0xC8;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.DWORD:
                    pdu[pduPointer.USHORT++] = 0xD3;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.REAL:
                    pdu[pduPointer.USHORT++] = 0xCA;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.LREAL:
                    pdu[pduPointer.USHORT++] = 0xCB;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.LWORD:
                    pdu[pduPointer.USHORT++] = 0xD4;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.LINT:
                    pdu[pduPointer.USHORT++] = 0xC5;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.ULINT:
                    pdu[pduPointer.USHORT++] = 0xC9;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.STRING:
                    pdu[pduPointer.USHORT++] = 0xD0;
                    pdu[pduPointer.USHORT++] = 0;
                    break;

                case TagFormats.STRUCTURE:
                    return GetTagFormat(ref pdu, ref pduPointer, _NodeFormat);

                default:
                    pdu[pduPointer.USHORT++] = 0;
                    pdu[pduPointer.USHORT++] = 0;
                    break;
            }

            return 2;
        }

        public ushort GetTagWriteData(ref byte[] pdu, ref ushortUnion pduPointer, Tag cand)
        {
            ushort writeLenght = adjSizeWrite(cand);
            byte[] tmpBuffer = new byte[writeLenght];            
            ushort offset = 0;

            lock (lockListObject)
            {
                // driver support big array --> same job can be splitted into more request --> check only in the first cycle
                if (PartialArrayEnd == 0)
                {
                    if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                    {
                        if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                        {
                            if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                            {
                                if (TagsListToWrite.Contains(cand))
                                {
                                    // use here TagsListToWrite instead of TagsListOnWriting because in this driver are managed big array and string 
                                    // --> data are moved from TagsListToWrite to TagsListOnWriting outside this function
                                    TagsListToWrite.Remove(cand);
                                }
                                if (TagFormat != TagFormats.STRING)
                                {
                                    return 0;
                                } 
                                else
                                {
                                    return 0xFFFF;
                                }                               
                            }
                        }
                    }
                    cand.LastValue = cand.Value.Value;
                }
                cand.GetTagBuffer(ref tmpBuffer, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
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
                if ((ElementNumber > 0 && (uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
                {
                    byte[] tmpData = new byte[(((ArraySize + 7) / 8 + ElemSize - 1) / ElemSize) * ElemSize];
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        if (tmpBuffer[ArrayIndex] != 0)
                            tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                    }
                    tmpBuffer = tmpData;
                }
                if ((DataFormat == DataFormats.BIT) && (TagsList[0].TagNode.ArrayDimension > 0) && (!ABAddress.Contains(']')))
                {
                    byte[] tmpData = new byte[(((ArraySize + 7) / 8 + ElemSize - 1) / ElemSize) * ElemSize];
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        if (tmpBuffer[ArrayIndex] != 0)
                            tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                    }
                    tmpBuffer = new byte[tmpData.Length];
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

            if ((TagFormat != TagFormats.BOOL) || ((ArraySize > 1) && (ABAddress.Contains(']'))))
            {
                Array.Copy(tmpBuffer, offset, pdu, pduPointer.USHORT, writeLenght);

                if (SwapBytes)
                    SwapByteBuffer(ref pdu, pduPointer.USHORT, (int)writeLenght);
                if (SwapWords)
                    SwapWordBuffer(ref pdu, pduPointer.USHORT, (int)writeLenght);

                pduPointer.USHORT += (ushort)writeLenght;

                return ((ushort)writeLenght);
            }
            else if ((DataFormat == DataFormats.BIT) && (TagsList[0].TagNode.ArrayDimension > 0) && (!ABAddress.Contains(']')))
            {
                int i = 0;
                int numberOfBits = (int)writeLenght;
                for (i = 0; i < numberOfBits; i++)
                {
                    pdu[pduPointer.USHORT++] = tmpBuffer[offset + i]; 
                }
                if(writeLenght == 1)
                { 
                    pdu[pduPointer.USHORT++] = 0;
                    return ((ushort)(writeLenght + 1));
                }
                else
                {
                    return ((ushort)(writeLenght));
                }
            }
            // When writing a single Boolean data add a byte, set to 0, for the "Forced set/reset information"
            else
            {
                int i = 0;
                int numberOfBits = (int)writeLenght;
                for (i = 0; i < numberOfBits; i++)
                {
                    pdu[pduPointer.USHORT++] = tmpBuffer[offset + i];
                    pdu[pduPointer.USHORT++] = 0;
                }
                return ((ushort)(writeLenght * 2));
            }
        }

        public ushort GetAsciiTagnameLength(string subName = "")
        {
            ushort tagPathLength = 0;

            Regex TagNodeParser = new Regex(@"\b(?<TagTree>[^\.]+)?");
            MatchCollection TagNodeMatches = TagNodeParser.Matches(_TagName);
            if (TagNodeMatches.Count > 0)
            {
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
                                if (_ModuleName != string.Empty)
                                    tagPathLength += GetSymbolSize(_ModuleName);
                                tagPathLength += GetSymbolSize(match.Value);
                            }
                            else
                            {
                                tagPathLength += GetSymbolSize(match.Value);
                            }
                        }
                        else
                        {
                            if (_ModuleName != string.Empty)
                                tagPathLength += GetSymbolSize(_ModuleName);
                            tagPathLength += GetSymbolSize(match.Value.Remove(match.Value.Length - indexLength, indexLength));
                            Match TagTreeIndexMatch = TagTreeIndexValidator.Match(TagTreeNodeIndexMatch.Groups["index"].Value);
                            if (TagTreeIndexMatch.Success)
                            {
                                if (TagTreeIndexMatch.Groups["index1"].Value.Length > 0)
                                {
                                    ushort memIndex = tagPathLength;
                                    ushort arrayIndex = Convert.ToUInt16(TagTreeIndexMatch.Groups["index1"].Value);
                                    if (arrayIndex > 0xff)
                                    {
                                        tagPathLength += 4;
                                    }
                                    else
                                    {
                                        tagPathLength += 2;
                                    }
                                    arrayIndex = (ushort)(tagPathLength - memIndex);
                                    if (TagTreeIndexMatch.Groups["index2"].Value.Length > 0)
                                    {
                                        memIndex = tagPathLength;
                                        arrayIndex = Convert.ToUInt16(TagTreeIndexMatch.Groups["index2"].Value.Remove(0, 1));
                                        if (arrayIndex > 0xff)
                                        {
                                            tagPathLength += 4;
                                        }
                                        else
                                        {
                                            tagPathLength += 2;
                                        }
                                        arrayIndex = (ushort)(tagPathLength - memIndex);
                                        if (TagTreeIndexMatch.Groups["index3"].Value.Length > 0)
                                        {
                                            memIndex = tagPathLength;
                                            arrayIndex = Convert.ToUInt16(TagTreeIndexMatch.Groups["index3"].Value.Remove(0, 1));
                                            if (arrayIndex > 0xff)
                                            {
                                                tagPathLength += 4;
                                            }
                                            else
                                            {
                                                tagPathLength += 2;
                                            }
                                            arrayIndex = (ushort)(tagPathLength - memIndex);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        level++;
                    }
                }
            }
            else
            {
                return 0;
            }

            return (ushort)(tagPathLength);
        }

        public ushort GetAsciiTagname(OmronEthernetIPStation s,ref byte[] pdu, ref ushortUnion pduPointer , Tag tag, string subName = "")
        {
            ushort sizeWords = pduPointer.USHORT;
            if (TagNameRequestBuffer == null)
            {
                pdu[pduPointer.USHORT++] = 0;

                Regex TagNodeParser = new Regex(@"\b(?<TagTree>[^\.]+)?");
                MatchCollection TagNodeMatches = TagNodeParser.Matches(_TagName);

                if (TagNodeMatches.Count > 0)
                {
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
                                    string keyPlcTagInstanceInfo = !string.IsNullOrWhiteSpace(_ModuleName)  ? _ModuleName + "." + match.Value : match.Value; 
                                    if (_ModuleName != string.Empty)
                                        GetSymbol(_ModuleName, pdu, ref pduPointer);
                                    GetSymbol(match.Value, pdu, ref pduPointer);
                                }
                                else
                                {
                                    GetSymbol(match.Value, pdu, ref pduPointer);
                                }
                            }
                            else
                            {
                                if (_ModuleName != string.Empty)
                                    GetSymbol(_ModuleName, pdu, ref pduPointer);
                                GetSymbol(match.Value.Remove(match.Value.Length - indexLength, indexLength), pdu, ref pduPointer);
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
                                            //writeArrayIndex(ref pdu, ref  pduPointer, Convert.ToUInt16(TagTreeIndexMatch.Groups["index2"].Value.Remove(0, 1)));
                                            memIndex = pduPointer.USHORT;
                                            TagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index2"].Value.Remove(0, 1));
                                            writeArrayIndex(ref pdu, ref pduPointer, TagIndexRequest);
                                            TagIndexRequestSize = pduPointer.USHORT - memIndex;
                                            if (TagTreeIndexMatch.Groups["index3"].Value.Length > 0)
                                            {
                                                //writeArrayIndex(ref pdu, ref  pduPointer, Convert.ToUInt16(TagTreeIndexMatch.Groups["index3"].Value.Remove(0, 1)));
                                                memIndex = pduPointer.USHORT;
                                                TagIndexRequest = Convert.ToUInt16(TagTreeIndexMatch.Groups["index3"].Value.Remove(0, 1));
                                                writeArrayIndex(ref pdu, ref pduPointer, TagIndexRequest);
                                                TagIndexRequestSize = pduPointer.USHORT - memIndex;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    pduPointer.USHORT = sizeWords;
                                    return 0;
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
                    string Tree = OmronEthernetIPProtocol.GetDynTagTree(tag.DynSettings as OmronEthernetIPDynTagSettings);
                    OmronEthernetIPProtocol.getOffTree(ref Tree);
                    while (!string.IsNullOrEmpty(Tree) )
                    {
                        GetSymbol(OmronEthernetIPProtocol.getOffTree(ref Tree), pdu, ref pduPointer);
                    }
                }

                TagNameRequestBuffer = new byte[pduPointer.USHORT - sizeWords];

                Array.Copy(pdu, sizeWords, TagNameRequestBuffer, 0, TagNameRequestBuffer.Count());
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

            //if (PartialArrayEnd != PartialArrayStart)
            //{
            //    writeArrayIndex(ref pdu, ref  pduPointer, PartialArrayStart);
            //}

            pdu[sizeWords] = (byte)((pduPointer.USHORT - sizeWords) / 2);

            return (ushort)(pduPointer.USHORT - sizeWords);
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

        public static TagFormats GetEthType(string Identifier)
        {
            switch (Identifier)
            {
                case "1":
                    return TagFormats.BOOL;
                case "2":
                    return TagFormats.SINT;
                case "3":
                    return TagFormats.BYTE;
                case "4":
                    return TagFormats.INT;
                case "5":
                    return TagFormats.UINT;
                case "6":
                    return TagFormats.DINT;
                case "7":
                    return TagFormats.UDINT;
                case "8":
                    return TagFormats.LINT;
                case "9":
                    return TagFormats.ULINT;
                case "10":
                    return TagFormats.REAL;
                case "11":
                    return TagFormats.LREAL;
                case "12":
                    return TagFormats.STRING;
            }
            return TagFormats.BOOL;
        }

        public static void GetSymbol(string symbol, byte[] pdu, ref ushortUnion pduPointer)
        {
            pdu[pduPointer.USHORT++] = 0x91; //Extended Symbol Segment
            ushort sizeIndex = pduPointer.USHORT;
            pdu[pduPointer.USHORT++] = 0;
            Encoding utf8 = Encoding.UTF8;
            byte[] asciiBytes = utf8.GetBytes(symbol);
            while (pdu[sizeIndex] < (byte)asciiBytes.Length && asciiBytes[pdu[sizeIndex]] != 0)
            {
                pdu[pduPointer.USHORT++] = asciiBytes[pdu[sizeIndex]++];
            }
            if (pdu[sizeIndex] % 2 == 1)
            {
                pdu[pduPointer.USHORT++] = 0;
            }
        }

        private ushort GetSymbolSize(string symbol)
        {
            Encoding ascii = Encoding.ASCII;
            byte[] asciiBytes = Encoding.UTF8.GetBytes(symbol);
            return (ushort)(asciiBytes.Length + asciiBytes.Length % 2 + 2);
        }

        public ushort GetTagReadBufferSize(Tag tag, string subName = "")
        {
            ushort outValue = 0;
            if (TagNameRequestBuffer == null)
            {
                outValue = GetAsciiTagnameLength(subName);
            }
            else
            {
                outValue = (ushort)(TagNameRequestBuffer.Count() - 1); // - 1 for ignoring the initial '0' byte of TagNameRequestBuffer
                if (!string.IsNullOrEmpty(subName))
                    outValue += (ushort)(((subName.Length + 3) / 2) * 2);
            }

            return outValue;
        }

        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
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
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }
                size += GetTagSize(t);
            }
 
            if (! _ParseOk )
            {
                IsValid = false;
                InvalidReason = string.Format("{0}", Properties.Resources.ParseKo);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            uint DataTypeBitSize = TagsList[0].TagNode.ArrayDimension == 0 ? 1 : TagsList[0].TagNode.ArrayDimension;
            if (ElementNumber == 0)
                DataTypeBitSize = DataTypeBitSize * GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override uint getProtocolDataType()
        {
            switch (OmronEthernetIPProtocol.DataType(_DataFormat))
            {
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.SByte:
                    return (uint)BuiltInType.SByte;
                case UFUAModel.DataType.Int16:
                    return (uint)BuiltInType.Int16;
                case UFUAModel.DataType.Int32:
                    return (uint)BuiltInType.Int32;
                case UFUAModel.DataType.Float:
                    return (uint)BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return (uint)BuiltInType.Double;
                case UFUAModel.DataType.Int64:
                    return (uint)BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                default:
                    return 0;
            }
        }

        public override uint GetMaxJobSize()
        {
            return OmronEthernetIPProtocol.MAX_DATA_SIZE;
        }
 
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public void ClearWriteTagLists()
        {
            lock (lockListObject)
            {
                TagsListOnWriting.Clear();
                TagsListToWrite.Clear();
            }
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListToWrite);
                TagsListToWrite.Clear();
            }

            List<byte> outData = new List<byte>();
            //prepare a write request
            listToWrite.Sort(CompareTagByOffset);
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            do
            {
                if (cand != null)
                {
                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];
                listToWrite.Remove(cand);
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
                    cand.LastValue = cand.Value.Value;
                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
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

                outData.AddRange(jobdata);

            } while (listToWrite.Count > 0);

            lock (lockListObject)
            {
                listOnWriting.ForEach((tag) =>
                {
                    if (!TagsListOnWriting.Contains(tag))
                        TagsListOnWriting.Add(tag);
                });
                listOnWriting.Clear();

                if (listToWrite.Count > 0)
                {
                    var tempListToWrite = new List<Tag>();
                    tempListToWrite.AddRange(TagsListToWrite);
                    TagsListToWrite.Clear();
                    listToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    tempListToWrite.Clear();
                }
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
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
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

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(TagsList[TagIndex]);
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
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    changed.Add(TagsList[TagIndex]);
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

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
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
        #endregion

        #region Properties

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

        //Symbolic Address

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

        private ushort _ElemSize;
        public ushort ElemSize
        {
            get { return _ElemSize; }
        }

        public CommandTypes CommandType { get; set; }
        
        #endregion
    }
}
