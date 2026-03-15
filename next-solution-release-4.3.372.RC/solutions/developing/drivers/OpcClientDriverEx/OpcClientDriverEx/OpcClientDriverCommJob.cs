using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using OPCUAViewModel;
using ViewModelLib;
using UFInterfaces;
using UFUAModel.Extensions;
using System.Threading;
using OPCUAViewModel.Services;

#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
#endif


namespace OpcClientDriver
{
    /// <summary>   Contains OPCUAEntityReference settings. </summary>
    struct OPCItemSettings
    {
        internal string HostName;
        internal string AppName;
        internal string EndpointUrl;
        internal string RelativePath;
        internal string ItemName;
        internal ExpandedNodeId TypeDefinitionNodeId;
        internal NodeId ResolvedNodeId;
    }
    struct Position
    {
        internal int Element;
        internal int Number;
        internal string Type;
        internal bool isArray;
    }

    struct structMembers
    {
        internal string Name;
        internal BuiltInType Type;
        internal int ArrDim;
        internal int TypeDim;
        internal int Offset;
        internal string TypeName;
    }
    public class OpcClientDriverCommJob : CommJob, IEntityReference, IDisposable
    {
        #region Declarations
        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        PropertyObserver<SessionViewModel> observerSession;
        Position position;
        Position element;
        OPCItemSettings currOPCItemSettings;
        string privateSessionName;

        static int excludeLengthByte = 5;

        Dictionary<string, List<structMembers>> StructComposition = new Dictionary<string, List<structMembers>>();

        private static List<LocalizedText> StructList = new List<LocalizedText>();

        /// <summary>
        /// StructName nd StructMembers
        /// </summary>
        private static Dictionary<LocalizedText, string> dictionary = new Dictionary<LocalizedText, string>();
        /// <summary>
        /// EnumTypes
        /// </summary>
        private static List<string> enumList = new List<string>();
        
        private ReferenceDescription descriptionExtensionObject = null;
        #endregion

        #region Constructors
        public OpcClientDriverCommJob(Station station, OpcClientDriverCommJobSettings settings)
            : base(station, settings)
        {
            CopySettings(settings);
            if (OPCItem == null)
            {
                OPCItem = new OPCUAEntityReference();
                InitOPCItem();
            }

            CheckJobValid();
        }

        public OpcClientDriverCommJob(Station station, OpcClientDriverTag defTag)
            : base(station, defTag)
        {
            CopySettings(defTag.OpcClientDynSettings);
            if (OPCItem == null)
            {
                OPCItem = new OPCUAEntityReference(!((OpcClientDriverDriver)Station.GetCommDriver()).DoNotUseServerRedundancy);                
                InitOPCItem();
            }

            CheckJobValid();
        }

        public OpcClientDriverCommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        protected OpcClientDriverCommJob()
        {            
            CheckJobValid();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (OPCItem != null)
                {
                    OPCItem.SetInUse(this, false);
                    OPCItem = null;
                }
                if (observer != null)
                {
                    observer.Dispose();
                    observer = null;
                }
                if (observerMonitoredModel != null)
                {
                    observerMonitoredModel.Dispose();
                    observerMonitoredModel = null;
                }
                if (observerSession != null)
                {
                    observerSession.Dispose();
                    observerSession = null;
                }
            }
        }
        #endregion

        #region Internal Methods

        internal Position CountingBytes(Position position, string dataTypeName, byte[] actualReadValue, bool runtime, string firstTagName = null, int arrayDimension = 0, string completeTagName = null)
        {
            position.isArray = (arrayDimension > 0) ? true : false;
            switch (dataTypeName)
            {
                case "SByte":
                case "Boolean":
                case "Char":
                case "Byte":
                    if (arrayDimension == 0)
                    {
                        position.Element += 1;
                        position.Number = 1;
                    }
                    else
                    {
                        position.Element += 1 * arrayDimension;
                        position.Number = 1 * arrayDimension;
                    }
                    break;
                case "WChar":
                case "UInt16":
                case "Int16":
                    if (arrayDimension == 0)
                    {
                        position.Element += 2;
                        position.Number = 2;
                    }
                    else
                    {
                        position.Element += 2 * arrayDimension;
                        position.Number = 2 * arrayDimension;
                    }
                    break;
                case "Float":
                case "Int32":
                case "UInt32":
                    if (arrayDimension == 0)
                    {
                        position.Element += 4;
                        position.Number = 4;
                    }
                    else
                    {
                        position.Element += 4 * arrayDimension;
                        position.Number = 4 * arrayDimension;
                    }
                    break;
                case "Int64":
                case "Double":
                case "Date":
                case "Time_Of_Day":
                case "DateTime":
                case "UInt64":
                    if (arrayDimension == 0)
                    {
                        position.Element += 8;
                        position.Number = 8;
                    }
                    else
                    {
                        position.Element += 8 * arrayDimension;
                        position.Number = 8 * arrayDimension;
                    }
                    break;
                case "CharArray":
                case "String":
                    int lenght = (5 + (int)actualReadValue[position.Element]);
                    position.Number = lenght;
                    position.Element += lenght - 1;
                    break;
                default:
                    if (runtime)
                    {
                        try
                        {
                            Position element_CB = new Position();
                            element_CB = ByteCount(dataTypeName, firstTagName, actualReadValue, position.Element);
                            return element_CB;
                        }
                        catch (Exception Ex)
                        {
                            if (!enumList.Contains(dataTypeName))
                                dataTypeName = "UInt32";
                            position.Element += 4;
                            position.Number = 4;
                        }
                        break;
                    }
                    else if (enumList.Contains(dataTypeName) || CheckEnumType(dataTypeName))
                    {
                        if (arrayDimension == 0)
                        {
                            position.Element += 4;
                            position.Number = 4;
                        }
                        else if (arrayDimension > 0)
                        {
                            position.Element += 4 * arrayDimension;
                            position.Number = 4 * arrayDimension;
                        }
                        break;
                    }
                    else
                        break;
            }
            position.Type = dataTypeName;
            if (runtime && position.isArray)
                position.Element += 4;
            return position;
        }

        internal string SaveStructure(Opc.Ua.Client.DataDictionary datadictionary, string substring, NodeId nodeId, bool checkSingleStructure, string structName = null)
        {
            string members = string.Empty;
            int index, structIndex = -1;
            int DataTypeNameIndex = -1;
            string submember = string.Empty;
            string DataTypeName = string.Empty;
            string TagName = string.Empty;
            string nameOfTag = string.Empty;
            string isArray = "LengthField";
            int TagNameIndex = 0;
            UInt32 arrayDimension = 0;

            var attribute = OPCItem.NodeIdViewModel.NodeAttributes;
            ExtensionObject actualValue = (ExtensionObject)attribute[13].Value;
            byte[] actBody = (byte[])actualValue.Body;
            element = CountingBytes(element, DataTypeName, actBody, false);

            while (TagNameIndex != -1)
            {
                if (!string.IsNullOrEmpty(structName))
                {
                    if (structName.Contains("\\"))
                        structName = structName.Replace("\\", "_x005C_");
                    if (substring.Contains(structName))
                    {
                        structIndex = substring.IndexOf("<StructuredType Name=" + "\"" + structName);
                        if (structIndex != -1)
                            substring = substring.Substring(structIndex);
                        checkSingleStructure = true;
                    }
                }

                TagNameIndex = substring.IndexOf("Field Name=") + 12;
                if (TagNameIndex < 12)
                {
                    TagNameIndex = -1;
                    continue;
                }
                substring = substring.Substring(TagNameIndex);
                index = substring.IndexOf("\"");
                TagName = substring.Substring(0, index);
                substring = substring.Substring(index);
                if (checkSingleStructure)
                {
                    if (substring.IndexOf("TypeName=\\\"") < substring.IndexOf("TypeName=\"q") && substring.IndexOf("TypeName=\"q") > substring.IndexOf("/>"))
                        DataTypeNameIndex = substring.IndexOf("TypeName=\\\"") + 13;
                    else
                        DataTypeNameIndex = substring.IndexOf("TypeName=\"q") + 13;
                }
                else
                    DataTypeNameIndex = substring.IndexOf("TypeName=\"q") + 13;
                substring = substring.Substring(DataTypeNameIndex);
                index = substring.IndexOf("\"");
                DataTypeName = substring.Substring(0, index);
                if (DataTypeName.Contains("_x005C_"))
                    DataTypeName = DataTypeName.Replace("_x005C_", "\\");
                if (substring.Substring(index + 2, 11) == isArray)
                {
                    substring = substring.Substring(index + 15);
                    index = substring.IndexOf("\"");
                    nameOfTag = substring.Substring(0, index);

                    byte[] valueToRead = new byte[4]; ;
                    for (int i = 0; i < element.Number; i++)
                    {
                        valueToRead[i] = actBody[element.Element - element.Number + i];
                    }

                    arrayDimension = BitConverter.ToUInt32(valueToRead, 0);
                    int idx = members.IndexOf(nameOfTag);
                    if (idx != -1)
                    {
                        int ide = members.IndexOf(";", idx);
                        if (ide != -1)
                        {
                            members = members.Remove(idx, ide - idx + 1);
                        }
                        else
                        {
                            members = members.Remove(idx);
                        }
                    }
                }
                element = CountingBytes(element, DataTypeName, actBody, false, null, (int)arrayDimension);
                members = members + TagName + ":" + DataTypeName + "#" + arrayDimension + ";";
                arrayDimension = 0;
                if (DataTypeName == ("DateTime"))
                    continue;
                var tempnodeId = nodeId.ToString();
                var indexs = tempnodeId.LastIndexOf("s=") + 2;
                var NodeId = tempnodeId.Substring(0, indexs) + DataTypeName;
                var xmlString = datadictionary.GetSchema(NodeId);
                if (xmlString != null)
                {
                    if (!dictionary.ContainsKey(DataTypeName))
                        dictionary.Add(DataTypeName, submember = SaveStructure(datadictionary, xmlString, NodeId, checkSingleStructure, DataTypeName));
                    if (!StructList.Contains(DataTypeName))
                        StructList.Add(DataTypeName);
                }
                if (checkSingleStructure)
                {
                    if (substring.IndexOf("</StructuredType") < substring.IndexOf("<Field Name"))
                        break;
                }
            }
            return members;
        }

        internal bool CheckEnumType(string dataTypeName)
        {
            BrowseDescription nodeBrowse = new BrowseDescription
            {
                NodeId = ExpandedNodeId.ToNodeId(Objects.DataTypesFolder, OPCItem.NodeIdViewModel.sessionViewModel.NamespaceUris),
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ResultMask = (uint)BrowseResultMask.DisplayName
            };
            ReferenceDescriptionCollection enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);

            if (enumeratedRefence.Count > 0)
            {
                nodeBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[0].NodeId, OPCItem.NodeIdViewModel.sessionViewModel.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.DisplayName
                };
                ReferenceDescriptionCollection enumeratedRefence1 = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                nodeBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence1[5].NodeId, OPCItem.NodeIdViewModel.sessionViewModel.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.DisplayName
                };

                ReferenceDescriptionCollection enumeratedRefence2 = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                foreach (var r in enumeratedRefence2)
                {
                    if (r.DisplayName == dataTypeName)
                    {
                        nodeBrowse = new BrowseDescription
                        {
                            NodeId = ExpandedNodeId.ToNodeId(r.NodeId, OPCItem.NodeIdViewModel.sessionViewModel.NamespaceUris),
                            BrowseDirection = BrowseDirection.Forward,
                            ReferenceTypeId = ReferenceTypeIds.HasProperty,
                            IncludeSubtypes = true,
                            NodeClassMask = 0,
                            ResultMask = (uint)BrowseResultMask.DisplayName
                        };
                        ReferenceDescriptionCollection enumeratedRefence3 = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                        if (enumeratedRefence3 != null)
                        {
                            if (!enumList.Contains(dataTypeName))
                                enumList.Add(dataTypeName);
                            return true;
                        }
                    }
                }

            }
            return false;
        }
        bool exit = false;

        internal Position ByteCount(LocalizedText structName, string tagName, byte[] actualReadValue, int numberOfByte)
        {
            position.Element = numberOfByte;
            string firstTagName = tagName;
            if (tagName.IndexOf("/") != -1)
            {
                firstTagName = firstTagName.Substring(firstTagName.IndexOf("/") + 1);
            }
            string members = string.Empty;
            exit = false;
            int index = -1;
            int lastIndex = 0;
            int commaIndex = 0;
            string memberName = string.Empty;
            string DataType = string.Empty;
            int arrayDimension;

            List<structMembers> memberslist;
            if (!StructComposition.ContainsKey(structName.Text))
            {
                //fill the structure composition, for future use
                memberslist = new List<structMembers>();
                StructComposition.Add(structName.Text, memberslist);
            }
            memberslist = StructComposition[structName.Text];
            dictionary.TryGetValue(structName, out members);
            if (memberslist != null && memberslist.Count == 0 && !string.IsNullOrEmpty(members))
            {
                FillStructMembersComposition(members, ref memberslist);
            }
            string normTagName = tagName;
            index = normTagName.LastIndexOf(':');
            while (index != -1)
            {
                var post = normTagName.Substring(index + 1);
                int nidx = normTagName.LastIndexOf('/', index);
                if (nidx != -1)
                {
                    var pre = normTagName.Substring(0, nidx + 1);
                    normTagName = pre + post;
                }
                else
                    normTagName = post;
                index = normTagName.LastIndexOf(':');
            }

            var l = (from m in memberslist where m.Name == normTagName select m).ToList();
            if (l.Count > 0 && l[0].Offset >= 0)
            {
                //got the position
                position.isArray = l[0].ArrDim > 0;
                position.Element += l[0].Offset;
                position.Number = (position.isArray ? l[0].TypeDim * l[0].ArrDim : l[0].TypeDim);
                position.Type = l[0].TypeName;
                if (position.isArray)
                    position.Element += 4;
                //return position;
            }
            else
            {
                for (int i = 0; i < memberslist.Count; i++)
                {
                    var m = memberslist[i];
                    position.isArray = m.ArrDim > 0;
                    position.Type = m.TypeName;
                    if (m.Type != BuiltInType.String)
                    {
                        position.Number = (position.isArray ? m.TypeDim * m.ArrDim : m.TypeDim);
                        position.Element += position.Number;

                    }
                    else
                    {
                        //string
                        int lenght = (5 + (int)actualReadValue[position.Element]);
                        position.Element += lenght - 1;
                        position.Number = lenght;
                    }

                    if (position.isArray)
                        position.Element += 4;

                    if (m.Name == normTagName)
                        break;
                }
            }


            return position;
        }

        void GetTypeDimension(string typeName, out BuiltInType type, out int dim)
        {
            dim = -1;
            type = BuiltInType.Null;
            switch (typeName)
            {
                case "SByte":
                    type = BuiltInType.SByte;
                    dim = 1;
                    break;
                case "Boolean":
                    type = BuiltInType.Boolean;
                    dim = 1;
                    break;
                case "Char":
                case "Byte":
                    type = BuiltInType.Boolean;
                    dim = 1;
                    break;
                case "WChar":
                case "UInt16":
                    type = BuiltInType.UInt16;
                    dim = 2;
                    break;
                case "Int16":
                    type = BuiltInType.Int16;
                    dim = 2;
                    break;
                case "Float":
                    type = BuiltInType.Float;
                    dim = 4;
                    break;
                case "Int32":
                    type = BuiltInType.Int32;
                    dim = 4;
                    break;
                case "UInt32":
                    type = BuiltInType.UInt32;
                    dim = 4;
                    break;
                case "Int64":
                    type = BuiltInType.Int64;
                    dim = 8;
                    break;
                case "Double":
                    type = BuiltInType.Double;
                    dim = 8;
                    break;
                case "Date":
                case "Time_Of_Day":
                case "DateTime":
                    type = BuiltInType.DateTime;
                    dim = 8;
                    break;
                case "UInt64":
                    type = BuiltInType.UInt64;
                    dim = 8;
                    break;
                case "CharArray":
                case "String":
                    type = BuiltInType.String;
                    dim = 0;
                    break;
                default:
                    break;
            }
        }

        void FillStructMembersComposition(string members, ref List<structMembers> memberslist, string parent = null)
        {
            var mbrs = members.Split(';');
            if (mbrs.Length > 0)
            {
                string name;
                int arrDim;
                for (int i = 0; i < mbrs.Length; i++)
                {
                    var item = mbrs[i];
                    if (item.Trim().Length == 0)
                        continue;
                    int index = item.IndexOf(':');
                    if (index != -1)
                    {
                        name = item.Substring(0, index);
                        item = item.Substring(index + 1);
                        index = item.IndexOf('#');
                        if (index != -1)
                        {
                            BuiltInType type;
                            int typedim;
                            var typeName = item.Substring(0, index);
                            GetTypeDimension(typeName, out type, out typedim);
                            arrDim = Convert.ToInt16(item.Substring(index + 1));
                            if (type == BuiltInType.Null)
                            {
                                string submembers;
                                dictionary.TryGetValue(typeName, out submembers);
                                if (!string.IsNullOrEmpty(submembers))
                                    FillStructMembersComposition(submembers, ref memberslist, (parent != null ? string.Format("{0}/{1}", parent, name) : name));
                                else
                                {
                                    if (CheckEnumType(typeName))
                                    {
                                        //present in enumList
                                        int currOffset = (memberslist.Count > 0 ? memberslist[memberslist.Count - 1].Offset : 0);
                                        typedim = 4;
                                        if (type == BuiltInType.String)
                                            currOffset = -1;
                                        if (currOffset >= 0)
                                            currOffset += (arrDim > 0 ? typedim * arrDim : typedim);
                                        memberslist.Add(new structMembers()
                                        {
                                            Name = (parent == null ? name : string.Format("{0}/{1}", parent, name)),
                                            ArrDim = arrDim,
                                            Type = type,
                                            TypeDim = typedim,
                                            Offset = currOffset,
                                            TypeName = typeName
                                        });
                                    }
                                }
                            }
                            else
                            {
                                int currOffset = (memberslist.Count > 0 ? memberslist[memberslist.Count - 1].Offset : 0);
                                if (type == BuiltInType.String)
                                    currOffset = -1;
                                if (currOffset >= 0)
                                    currOffset += (arrDim > 0 ? typedim * arrDim : typedim);
                                memberslist.Add(new structMembers() { Name = (parent == null ? name : string.Format("{0}/{1}", parent, name)), ArrDim = arrDim,
                                    Type = type, TypeDim = typedim, Offset = currOffset, TypeName = typeName });
                            }
                        }
                    }
                }
            }
        }
        internal byte[] byteConverterToWrite(string stringType, object data)
        {
            byte[] stringAnswer = new byte[position.Number];
            InternalByteConverterToWrite(stringType, data, ref stringAnswer);

            return stringAnswer;
        }
        internal void InternalByteConverterToWrite(string stringType, object data, ref byte[] outdata)
        {
            switch (stringType)
            {
                case "WChar":
                case "UInt16":
                    var ValueUint16 = Convert.ToUInt16(data);
                    outdata = BitConverter.GetBytes(ValueUint16);
                    break;
                case "Int16":
                    var ValueInt16 = Convert.ToInt16(data);
                    outdata = BitConverter.GetBytes(ValueInt16);
                    break;
                case "Float":
                    var ValueFloat = Convert.ToSingle(data);
                    outdata = BitConverter.GetBytes(ValueFloat);
                    break;
                case "Int32":
                    var ValueInt32 = Convert.ToInt32(data);
                    outdata = BitConverter.GetBytes(ValueInt32);
                    break;
                case "UInt32":
                    var ValueUInt32 = Convert.ToUInt32(data);
                    outdata = BitConverter.GetBytes(ValueUInt32);
                    break;
                case "Int64":
                    var ValueInt64 = Convert.ToInt64(data);
                    outdata = BitConverter.GetBytes(ValueInt64);
                    break;
                case "Double":
                    var ValueDouble = Convert.ToDouble(data);
                    outdata = BitConverter.GetBytes(ValueDouble);
                    break;
                case "UInt64":
                    var ValueUInt64 = Convert.ToUInt64(data);
                    outdata = BitConverter.GetBytes(ValueUInt64);
                    break;
                case "CharArray":
                case "String":
                    var ValueString = (string)data;
                    outdata = System.Text.Encoding.Default.GetBytes(ValueString);
                    break;
                default:
                    if (enumList.Contains(stringType))
                    {
                        ValueUInt32 = Convert.ToUInt32(data);
                        outdata = BitConverter.GetBytes(ValueUInt32);
                    }
                    break;

            }
        }
        internal object byteConverterToRead(string stringType, byte[] data)
        {
            object value = new object();
            switch (stringType)
            {
                case "WChar":
                    value = BitConverter.ToChar(data, 0);
                    break;
                case "UInt16":
                    value = BitConverter.ToUInt16(data, 0);
                    break;
                case "Int16":
                    value = BitConverter.ToInt16(data, 0);
                    break;
                case "Float":
                    value = BitConverter.ToSingle(data, 0);
                    break;
                case "Int32":
                    value = BitConverter.ToInt32(data, 0);
                    break;
                case "UInt32":
                    value = BitConverter.ToUInt32(data, 0);
                    break;
                case "Int64":
                    value = BitConverter.ToInt64(data, 0);
                    break;
                case "Double":
                    value = BitConverter.ToDouble(data, 0);
                    break;
                case "UInt64":
                    value = BitConverter.ToUInt64(data, 0);
                    break;
                case "String":
                case "CharArray":
                    value = System.Text.Encoding.Default.GetString(data);
                    break;
                case "Boolean":
                    value = BitConverter.ToBoolean(data, 0);
                    break;
                case "Byte":
                    value = data;
                    break;
                default:
                    if (enumList.Contains(stringType))
                        value = BitConverter.ToUInt32(data, 0);
                    else value = null;
                    break;

            }

            return value;
        }

        internal void CopySettings(OpcClientDriverCommJobSettings settings)
        {
            currOPCItemSettings.HostName = settings.HostName;
            currOPCItemSettings.AppName = settings.AppName;
            currOPCItemSettings.EndpointUrl = settings.EndpointUrl;
            currOPCItemSettings.RelativePath = settings.RelativePath;
            currOPCItemSettings.ItemName = settings.ItemName;
            currOPCItemSettings.TypeDefinitionNodeId = settings.TypeDefinitionNodeId;
            currOPCItemSettings.ResolvedNodeId = settings.ResolvedNodeId;
        }
        internal void CopySettings(OpcClientDriverDynTagSettings settings)
        {
            currOPCItemSettings.HostName = settings.HostName;
            currOPCItemSettings.AppName = settings.AppName;
            currOPCItemSettings.EndpointUrl = settings.EndpointUrl;
            currOPCItemSettings.RelativePath = settings.RelativePath;
            currOPCItemSettings.ItemName = settings.ItemName;
            currOPCItemSettings.TypeDefinitionNodeId = settings.TypeDefinitionNodeId;
            currOPCItemSettings.ResolvedNodeId = settings.ResolvedNodeId;
        }
        internal void InitOPCItem()
        {
            OpcClientDriverStation station = this.Station as OpcClientDriverStation;

            if (currOPCItemSettings.HostName != null)
                OPCItem.HostName = currOPCItemSettings.HostName;
            if (currOPCItemSettings.AppName != null)
                OPCItem.AppName = currOPCItemSettings.AppName;
            if (currOPCItemSettings.EndpointUrl != null)
                OPCItem.EndpointUrl = currOPCItemSettings.EndpointUrl;
            if (currOPCItemSettings.RelativePath != null)
                OPCItem.RelativePath = currOPCItemSettings.RelativePath;
            if (currOPCItemSettings.ItemName != null)
                OPCItem.HumanReadable = currOPCItemSettings.ItemName;
            if (currOPCItemSettings.TypeDefinitionNodeId != null)
                OPCItem.TypeDefinitionNodeId = currOPCItemSettings.TypeDefinitionNodeId;
            if (currOPCItemSettings.ResolvedNodeId != null)
                OPCItem.ResolvedNodeId = currOPCItemSettings.ResolvedNodeId;
            if ((station.UseServerDiscovery || station.UsePollingRead)
                && !station.map.ContainsKey(OPCItem.AppName))
            {
                AppNameSettings app = new AppNameSettings();
                app.EndpointRenamed = OPCItem.EndpointUrl;
                app.UseAlwaysSecureConnections = station.UseServerDiscovery;
                app.UsePollingRead = station.UsePollingRead;
                station.map.Add(OPCItem.AppName, app);
            }

            if (string.IsNullOrEmpty(OPCItem.HostName) && !string.IsNullOrEmpty(OPCItem.EndpointUrl))
            {
                //get host name for endpoint url
                var seu = OPCItem.EndpointUrl;
                int start = seu.IndexOf("://");
                if (start != -1)
                {
                    start += 3;
                    int end = seu.IndexOf(":", start); //normal format
                    if (end == -1)
                        end = seu.IndexOf("/", start); //net.pipe
                    if (end == -1)
                        OPCItem.HostName = seu.Substring(start);
                    else
                        OPCItem.HostName = seu.Substring(start, end - start);
                }
            }

            privateSessionName = GetPrivateSessionName();
        }

        bool bInitialized = false;
        internal bool PrepareExecution(String sessionname, List<CommJob> listJob, ManualResetEvent syncDataValueChangedCompleted = null, ManualResetEvent syncNodeIdViewModelCompleted = null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("PrepareExecution {0} bInitialized:{1}", Name, bInitialized);
#endif
            var mystation = Station as OpcClientDriverStation;
            if (mystation == null)
                return false;

            if (bInitialized)
                return true;

            if (OPCItem == null)
            {
                OPCItem = new OPCUAEntityReference();
                InitOPCItem();
                //privateSessionName = GetPrivateSessionName();
                CheckJobValid();
            }


            bInitialized = true;

            var channel = this.Station.GetChannel();

            observer = new PropertyObserver<OPCUAEntityReference>(OPCItem);
            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                if (observerMonitoredModel != null)
                    observerMonitoredModel.Dispose();

                var monitoredItemViewModel = n.MonitoredItemViewModel;
                if (monitoredItemViewModel != null)
                {
                    observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(monitoredItemViewModel);
                    observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                    {
                        if (m.DataValue != null)
                        {
                            if (!(m.DataValue.Value is ExtensionObject))
                            {
                                DataValueChanged(m.DataValue);
                                if (SyncroExec)
                                {
                                    if (syncDataValueChangedCompleted != null)
                                        syncDataValueChangedCompleted.Set();
                                }
                            }
                            else
                            {
                                List<OpcClientDriverCommJob> list = new List<OpcClientDriverCommJob>();
                                foreach (OpcClientDriverCommJob j in listJob)
                                {
                                    if (j.OPCItem == OPCItem)
                                        list.Add(j);
                                }
                                DataValueChangedFromExtensionObject(m.DataValue, OPCItem, list);
                            }
                        }

                    });


                    if (monitoredItemViewModel.DataValue != null && !(monitoredItemViewModel.DataValue.Value is ExtensionObject))
                    {
                        DataValueChanged(monitoredItemViewModel.DataValue);
                        if (SyncroExec)
                        {
                            if (syncDataValueChangedCompleted != null)
                                syncDataValueChangedCompleted.Set();
                        }
                    }
                    else
                    {
                        List<OpcClientDriverCommJob> list = new List<OpcClientDriverCommJob>();
                        foreach (OpcClientDriverCommJob j in listJob)
                        {
                            if (j.OPCItem == OPCItem)
                                list.Add(j);
                        }
                        if (monitoredItemViewModel.DataValue != null && OPCItem.NodeIdViewModel != null)
                            DataValueChangedFromExtensionObject(monitoredItemViewModel.DataValue, OPCItem, list);
                    }

                    SessionViewModel session = null;
                    var subscription = monitoredItemViewModel.GetSubscriptionViewModelParent();
                    if (subscription != null)
                        session = subscription.GetSessionViewModelParent();
                    if (session != null && !string.IsNullOrEmpty(privateSessionName))
                    {
                        if (observerSession != null)
                            observerSession.Dispose();

                        if (session.Connected)
                            mystation.StopTimeoutTimer(privateSessionName);
                        else
                        {
                            observerSession = new PropertyObserver<SessionViewModel>(session);
                            observerSession.RegisterHandler(s => s.Connected, s =>
                            {
                                if (s.Connected)
                                {
                                    observerSession.UnregisterHandler(p => p.Connected);
                                    mystation.StopTimeoutTimer(privateSessionName);
                                }
                            });
                        }
                    }
                }
            });

            observer.RegisterHandler(n => n.NodeIdViewModel, n =>
            {
                if (n.NodeIdViewModel != null)
                {
                    if (SyncroExec)
                    {
                        if (syncNodeIdViewModelCompleted != null)
                            syncNodeIdViewModelCompleted.Set();
                    }
                }
            });

            // if a tag is put in a blacklist means that cannot be subscribed --> don't exist
            observer.RegisterHandler(n => n.InBlackList, n =>
            {
                if (n.InBlackList)
                    DataValueChanged(new DataValue(new Variant(), StatusCodes.BadNotFound, DateTime.UtcNow));
            });

            try
            {
                mystation.SetSessionSettings(sessionname, ((OpcClientDriverChannel)Station.GetChannel()).BackupHostList);
                OPCItem.Resolve(sessionname);
                var session = OPCItem.GetSession(sessionname);

                OPCItem.SetInUse(this, true);

                if (OPCItem.MonitoredItemViewModel != null)
                    session = OPCItem.MonitoredItemViewModel.GetSubscriptionViewModelParent().GetSessionViewModelParent();
                if (session == null && !string.IsNullOrEmpty(privateSessionName))
                    mystation.AddJobInitialized(this, privateSessionName);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        string GetPrivateSessionName()
        {
            if (Station != null && !string.IsNullOrEmpty(Station.Name) && OPCItem != null && !string.IsNullOrEmpty(OPCItem.EndpointUrl))
                return string.Format("{0}_{1}", Station.Name, OPCItem.EndpointUrl);
            return null;
        }
        internal bool Unsubscribe(string sessionname)
        {
            bool bRet = false;
            if (OPCItem != null)
                OPCItem.SetInUse(this, false);

            if (observer != null)
                observer.Dispose();
            if (observerMonitoredModel != null)
                observerMonitoredModel.Dispose();
            if (observerSession != null)
                observerSession.Dispose();

            observer = null;
            observerMonitoredModel = null;
            observerSession = null;



            //if (string.IsNullOrEmpty(privateSessionName))
            //    privateSessionName = GetPrivateSessionName();

            OPCItem = null;
            bInitialized = false;
            var mystation = Station as OpcClientDriverStation;

            if (mystation != null && !string.IsNullOrEmpty(privateSessionName))
                mystation.RemoveJobInitialized(this, privateSessionName);

            return bRet;
        }

        internal bool IsTypeAdmitted(NodeId type)
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
                nType == (uint)BuiltInType.UInteger ||
                nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }

            return false;
        }
        #endregion

        #region Private Methods

        void DataValueChanged(DataValue dataValue)
        {
            if (TagsList.Count > 0)
            {
                int dataTypeval = GetTagIdTypeToUFUAModelDataType(TagsList[0].TagNode);
                if ((dataTypeval != -1) && (dataTypeval != (int)UFUAModel.DataType.String))
                {
                    dataValue.Value = UFUAModel.Extensions.DataTypeExtensions.ChangeType(dataValue.Value, (UFUAModel.DataType)dataTypeval, (int)TagsList[0].TagNode.ArrayDimension);
                }

                if (dataTypeval == (int)UFUAModel.DataType.String && dataValue.Value == null)
                    dataValue.Value = string.Empty;
                else if (dataTypeval == -1)
                    dataValue.StatusCode = StatusCodes.BadTypeMismatch;

                if (dataTypeval == (int)UFUAModel.DataType.String && dataValue.Value == null)
                    dataValue.Value = string.Empty;
                else if (dataTypeval == -1)
                    dataValue.StatusCode = StatusCodes.BadTypeMismatch;

                ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = this, Values = dataValue };
                if (StatusCode.IsBad(dataValue.StatusCode))
                    eJob.ErrorCode = (DriverErrorCodes)dataValue.StatusCode.Code;

                if (!SyncroExec)
                    Station.OnJobExecuted(this, eJob);
            }
        }
        public static object thisLock = new object();
        void DataValueChangedFromExtensionObject(DataValue dataValue, OPCUAEntityReference opcItem, List<OpcClientDriverCommJob> s)
        {
            int index = 0;
            bool checkSingleStructure = false;
            ReferenceDescription description = null;
            bool skip = false;
            string dataTypeName = string.Empty;

            try
            {
                var m_encodings = OPCItem.NodeIdViewModel.sessionViewModel.Session.ReadAvailableEncodings((NodeId)OPCItem.NodeIdViewModel.nodeId);
                ReferenceDescription encodings = m_encodings[0];
                description = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDescription((NodeId)encodings.NodeId);
            }
            catch (Exception ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, string.Format(string.Format(Properties.Resources.ErrorReadingStruct, ex.Message), OPCItem.HumanReadable, ex.Message), EventSeverity.High);
            }

            Position elementRead = new Position();
            try
            {
                if (!StructList.Contains(description.DisplayName) || string.IsNullOrEmpty(description.DisplayName.Text))
                {
                    BrowseDescription nodeToBrowse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(OPCItem.NodeIdViewModel.nodeId, OPCItem.NodeIdViewModel.session.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasComponent,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.DisplayName
                    };
                    ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
                    string members = string.Empty;
                    int arrayDimension = 0;
                    lock (thisLock)
                    {
                        if (typeReferences != null)
                        {
                            for (int i = 0; i < typeReferences.Count; i++)
                            {
                                nodeToBrowse = new BrowseDescription
                                {
                                    NodeId = ExpandedNodeId.ToNodeId(typeReferences[i].NodeId, OPCItem.NodeIdViewModel.session.NamespaceUris),
                                    BrowseDirection = BrowseDirection.Forward,
                                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                                    IncludeSubtypes = true,
                                    NodeClassMask = 0,
                                    ResultMask = (uint)BrowseResultMask.DisplayName
                                };
                                ReferenceDescriptionCollection typeReferencesChild = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
                                if (typeReferencesChild.Count > 0)
                                {
                                    var nodeIdViewModel = new OPCUAViewModel.NodeIdViewModel(typeReferencesChild[0].NodeId, OPCItem.NodeIdViewModel.sessionViewModel);
                                    UFUAModel.DataType dataType = nodeIdViewModel.DataType.ToDataType();
                                    dataTypeName = dataType.ToString();
                                    arrayDimension = typeReferencesChild.Count;
                                }
                                else
                                {
                                    var nodeIdViewModel = new OPCUAViewModel.NodeIdViewModel(typeReferences[i].NodeId, OPCItem.NodeIdViewModel.sessionViewModel);
                                    UFUAModel.DataType dataType = nodeIdViewModel.DataType.ToDataType();
                                    dataTypeName = dataType.ToString();
                                    arrayDimension = 0;
                                }
                                if (arrayDimension > 0)
                                {
                                    members = members + "lenght" + ":" + "Int32" + "#" + "0" + ";";
                                }
                                members = members + typeReferences[i].DisplayName.Text + ":" + dataTypeName + "#" + arrayDimension + ";";
                                skip = true;
                            }
                            if (!dictionary.ContainsKey(OPCItem.NodeIdViewModel.DisplayName.Text))
                                dictionary.Add(OPCItem.NodeIdViewModel.DisplayName.Text, members);
                            if (!StructList.Contains(OPCItem.NodeIdViewModel.DisplayName.Text))
                                StructList.Add(OPCItem.NodeIdViewModel.DisplayName.Text);
                        }
                    }
                    var m_encodings = OPCItem.NodeIdViewModel.sessionViewModel.Session.ReadAvailableEncodings((NodeId)OPCItem.NodeIdViewModel.nodeId);
                    if (m_encodings.Count != 0 && !skip)
                    {
                        ReferenceDescription encodings = m_encodings[0];
                        description = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDescription((NodeId)encodings.NodeId);
                        lock (thisLock)
                        {
                            if (!StructList.Contains(description.DisplayName))
                            {
                                #region 

                                #endregion
#if !NET_STANDARD
                                Opc.Ua.Client.DataDictionary datadictionary = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDictionary((NodeId)description.NodeId);
#else
                                Opc.Ua.Client.DataDictionary datadictionary = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDictionary((NodeId)description.NodeId).Result;
#endif
                                var schema = datadictionary.GetSchema((NodeId)description.NodeId);

                                System.Diagnostics.Debug.WriteLine("Struct {0}={1}", description.DisplayName, schema);

                                if (schema.Contains(description.DisplayName.ToString()))
                                {
                                    index = schema.IndexOf("<StructuredType Name=" + "\"" + description.DisplayName.ToString());
                                    if (index != -1)
                                    {
                                        schema = schema.Substring(index);
                                        checkSingleStructure = true;
                                    }
                                    else
                                        checkSingleStructure = false;
                                }
                                else
                                {
                                    checkSingleStructure = false;
                                }
                                members = SaveStructure(datadictionary, schema, (NodeId)description.NodeId, checkSingleStructure);
                                dictionary.Add(description.DisplayName, members);
                                StructList.Add(description.DisplayName);
                                isExtensionObject = true;
                                descriptionExtensionObject = description;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, String.Format(Properties.Resources.ErrorBadConnection, ex.Message), EventSeverity.High);
            }
            ExtensionObject extensionObject = (ExtensionObject)dataValue.Value;
            foreach (OpcClientDriverCommJob sub in s)
            {

                var j = sub;
                Tag tag = null;
                try
                {
                    List<object> lVal = new List<object>();
                    DriverErrorCodes derr = DriverErrorCodes.ErrorNoError;
                    for (int z = 0; z < j.TagsList.Count; z++)
                    {
                        tag = j.TagsList[z];
                        if (extensionObject != null)
                        {
                            byte[] actualReadValue = (byte[])extensionObject.Body;

                            if (description != null)
                                elementRead = ByteCount(description.DisplayName, tag.TagNode.Name, actualReadValue, 0);
                            else
                                elementRead = ByteCount(OPCItem.NodeIdViewModel.DisplayName.Text, tag.TagNode.Name, actualReadValue, 0);

                            System.Diagnostics.Debug.WriteLine("StructData z:{0}-{5} el:{1} num:{2} type:{3} uguali:{4} ",
                                z, position.Element, position.Number, position.Type, (position.Element == elementRead.Element &&
                                position.isArray == elementRead.isArray &&
                                position.Number == elementRead.Number &&
                                position.Type == elementRead.Type), tag.TagNode.Name);

                            byte[] valueToRead = null;
                            if (elementRead.Type == "CharArray" || elementRead.Type == "String")
                            {
                                valueToRead = new byte[elementRead.Number - excludeLengthByte];
                                for (int i = 0; i < elementRead.Number - excludeLengthByte; i++)
                                {
                                    valueToRead[i] = actualReadValue[position.Element - position.Number + excludeLengthByte + i];
                                }
                            }

                            else
                            {
                                valueToRead = new byte[elementRead.Number];
                                for (int i = 0; i < elementRead.Number; i++)
                                {
                                    valueToRead[i] = actualReadValue[elementRead.Element - elementRead.Number + i];
                                }
                            }

                            int dataTypeval = GetTagIdTypeToUFUAModelDataType(TagsList[0].TagNode);
                            //object data = null;
                            Array dataArray = null;
                            int size = 0;
                            //if (!position.isArray)
                            var data = byteConverterToRead(elementRead.Type, valueToRead);
                            if (position.isArray)
                            {
                                switch (elementRead.Type)
                                {
                                    case "UInt16":
                                        size = sizeof(UInt16);
                                        dataArray = Array.CreateInstance(typeof(UInt16), elementRead.Number / size);
                                        break;
                                    case "Int16":
                                        size = sizeof(Int16);
                                        dataArray = Array.CreateInstance(typeof(Int16), elementRead.Number / size);
                                        break;
                                    case "Float":
                                        size = sizeof(float);
                                        dataArray = Array.CreateInstance(typeof(float), elementRead.Number / size);
                                        break;
                                    case "Int32":
                                        size = sizeof(Int32);
                                        dataArray = Array.CreateInstance(typeof(Int32), elementRead.Number / size);
                                        break;
                                    case "UInt32":
                                        size = sizeof(UInt32);
                                        dataArray = Array.CreateInstance(typeof(UInt32), elementRead.Number / size);
                                        break;
                                    case "Int64":
                                        size = sizeof(UInt64);
                                        dataArray = Array.CreateInstance(typeof(UInt64), elementRead.Number / size);
                                        break;
                                    case "Double":
                                        size = sizeof(Double);
                                        dataArray = Array.CreateInstance(typeof(Double), elementRead.Number / size);
                                        break;
                                    case "UInt64":
                                        size = sizeof(UInt64);
                                        dataArray = Array.CreateInstance(typeof(UInt64), elementRead.Number / size);
                                        break;
                                    case "CharArray":
                                    case "String":
                                        size = sizeof(Char);
                                        dataArray = Array.CreateInstance(typeof(Char), elementRead.Number / size);
                                        break;
                                    case "Boolean":
                                        size = sizeof(Boolean);
                                        dataArray = Array.CreateInstance(typeof(Boolean), elementRead.Number / size);
                                        break;
                                    case "Byte":
                                        size = sizeof(Byte);
                                        dataArray = Array.CreateInstance(typeof(Byte), elementRead.Number / size);
                                        break;

                                }
                                for (int i = 0; i < (elementRead.Number / size); i++)

                                {
                                    byte[] tempReadValue = new byte[size];
                                    Array.Copy(valueToRead, i * size, tempReadValue, 0, size);
                                    dataArray.SetValue(byteConverterToRead(elementRead.Type, tempReadValue), i);
                                }
                                data = dataArray;
                            }

                            DataValue dataValueTemp = new DataValue(dataValue);
                            if (dataArray == null)
                                dataValueTemp.Value = data;
                            else
                                dataValueTemp.Value = dataArray;

                            //tag.LastValue = dataValueTemp.Value;
                            tag.Value.Value = Utils.Clone(dataValueTemp.Value);
                            tag.Value.StatusCode = dataValue.StatusCode;
                            lVal.Add(tag);
                            System.Diagnostics.Debug.WriteLine("{2} OPCClientDBG - DataValueChangedFromExtensionObject - next tag: {0} val:{3} z:{1}",
                                tag.TagNode.NodeId, z, Thread.CurrentThread.ManagedThreadId, tag.Value.Value);
                            if (StatusCode.IsBad(dataValue.StatusCode))
                                derr = (DriverErrorCodes)dataValue.StatusCode.Code;
                            if ((dataValue.Value == null) || (dataTypeval == -1) &&
                                (dataTypeval != (int)UFUAModel.DataType.String))
                            {
                                dataValue.StatusCode = StatusCodes.BadTypeMismatch;
                            }

                        }

                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = j, Values = lVal };
                    eJob.ErrorCode = derr;
                    foreach (var tg in lVal)
                    {
                        var tj = tg as Tag;
                        if (tj != null)
                            eJob.ChangedTags.Add(tj);
                    }
                    Station.OnJobExecuted(j, eJob);
                }
                catch (Exception ex)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = j };
                    eJob.ErrorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorCodeBadConnetion;
                    Station.OnJobExecuted(j, eJob);
                    Station.GetCommDriver().OnSystemEvent(null, String.Format(Properties.Resources.ErrorBadConnection, ex.Message), EventSeverity.High);
                }
            }

        }

        int GetTagIdTypeToUFUAModelDataType(DriverBaseInterfaces.TagDefinition t)
        {
            int returnValue = -1;
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        returnValue = (int)UFUAModel.DataType.Boolean;
                        break;
                    case (uint)BuiltInType.SByte:
                        returnValue = (int)UFUAModel.DataType.SByte;
                        break;
                    case (uint)BuiltInType.Byte:
                        returnValue = (int)UFUAModel.DataType.Byte;
                        break;
                    case (uint)BuiltInType.Int16:
                        returnValue = (int)UFUAModel.DataType.Int16;
                        break;
                    case (uint)BuiltInType.UInt16:
                        returnValue = (int)UFUAModel.DataType.UInt16;
                        break;
                    case (uint)BuiltInType.Float:
                        returnValue = (int)UFUAModel.DataType.Float;
                        break;
                    case (uint)BuiltInType.UInt32:
                        returnValue = (int)UFUAModel.DataType.UInt32;
                        break;
                    case (uint)BuiltInType.Int32:
                        returnValue = (int)UFUAModel.DataType.Int32;
                        break;
                    case (uint)BuiltInType.UInt64:
                        returnValue = (int)UFUAModel.DataType.UInt64;
                        break;
                    case (uint)BuiltInType.Int64:
                        returnValue = (int)UFUAModel.DataType.Int64;
                        break;
                    case (uint)BuiltInType.Double:
                        returnValue = (int)UFUAModel.DataType.Double;
                        break;
                    case (uint)BuiltInType.ByteString:
                    case (uint)BuiltInType.String:
                        returnValue = (int)UFUAModel.DataType.String;
                        break;
                    default:
                        returnValue = -1;
                        break;
                }
            }
            else if (t.DataType.IdType == IdType.String)
            {
                returnValue = (int)UFUAModel.DataType.String;
            }
            return returnValue;
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
                    default:
                        return 0;
                }
            }
            return 0;
        }
        new List<DriverBaseInterfaces.TagDefinition> GetSimpleTagList(DriverBaseInterfaces.TagDefinition t)
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
        void CheckJobValid()
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

            IsValid = true;
            InvalidReason = string.Empty;

            var ch = Station.GetChannel() as OpcClientDriverChannel;
            if (ch != null && !string.IsNullOrEmpty(ch.HostName))
            {
                //get host name for endpoint url
                var seu = OPCItem.EndpointUrl;
                int start = seu.IndexOf("://");
                if (start != -1)
                {
                    int end = seu.IndexOf(":", start + 1);//normal format
                    if (end == -1)
                        end = seu.IndexOf("/", start);//net.pipe
                    string snew = seu.Substring(0, start + 3);
                    snew += ch.HostName;
                    snew += seu.Substring(end);
                    OPCItem.EndpointUrl = snew;
                    OPCItem.HostName = ch.HostName;
                }
            }
        }
        #endregion

        #region Overrides
        public override uint GetMaxJobSize()
        {
            if (Station.GetCommDriver().AggregationLimit != 0)
                return Station.GetCommDriver().AggregationLimit;

            return 100;
        }
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return true;
        }

        // method not used
        public override void GetJobData(ref object jobData)
        {
            jobData = null;
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            lock (lockListObject)
            {
                for (int j = 0; j < TagsList.Count; j++)
                {
                    if (TagsList[j].SetTagValue(ref rec, (int)TagsList[j].ByteOffset))
                        changed.Add(TagsList[j]);
                }
            }
        }

        public uint SyncReadOpcValue(out DataValue dataValue)
        {
            uint result = StatusCodes.Good;

            dataValue = null;

            var sub = OPCItem.MonitoredItemViewModel.GetSubscriptionViewModelParent();

            ReadValueId valueId = new ReadValueId { NodeId = OPCItem.MonitoredItemViewModel.monitoredItem.ResolvedNodeId, AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null };
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection() { valueId };

            try
            {
                // read attributes.
                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                sub.subscription.Session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, nodesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                // check if node supports attribute.
                if (values[0].StatusCode == StatusCodes.BadAttributeIdInvalid)
                {
                    result = StatusCodes.Bad;
                }
                else
                {
                    dataValue = values[0];
                    int dataTypeval = GetTagIdTypeToUFUAModelDataType(TagsList[0].TagNode);
                    if ((dataTypeval != -1) && (dataTypeval != (int)UFUAModel.DataType.String))
                    {
                        dataValue.Value = UFUAModel.Extensions.DataTypeExtensions.ChangeType(dataValue.Value, (UFUAModel.DataType)dataTypeval, (int)TagsList[0].TagNode.ArrayDimension);
                    }
                }
            }
            catch (Exception ex)
            {
                result = StatusCodes.Bad;
                dataValue = null;
            }

            return result;
        }
        
        public override uint OnWriteTag(NodeId tagnodeid, ref object value, bool forceValue)
        {                       
            uint result = base.OnWriteTag(tagnodeid, ref value, forceValue);
            if (result == StatusCodes.Good)
            {
                // don't queue value for sync write
                if (!(SyncroExec || !Station.GetCommDriver().WriteAsync))
                    result = ((OpcClientDriverStation)Station).QueueTagToWrite(tagnodeid);
            }
            return result;
        }

        /// <summary>
        /// ExtensionObject data value type contains internally more values (prototypes) : before write value read from device and update values to write
        /// </summary>
        /// <param name="tagnodeids"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal DataValue ReadAndMergeExtensionObjectWithWriteValues(ref List<NodeId> tagnodeids, ref List<object> values)
        {
            Position elementWrite = new Position();

            ReferenceDescription description = descriptionExtensionObject;

            DataValue dataValue = OPCItem.NodeIdViewModel.sessionViewModel.Session.ReadValue((NodeId)OPCItem.NodeIdViewModel.nodeId);

            for (int index = 0; index < tagnodeids.Count; index++)
            {
                NodeId tagnodeid = tagnodeids[index];
                object value = values[index];

                Tag tag = null;// this.TagsList[0];
                var rt = TagsList.FindIndex(o => o.TagNode.NodeId == tagnodeid);
                if (rt != -1)
                    tag = TagsList[rt];

                ExtensionObject extensionObject = (ExtensionObject)dataValue.Value;
                byte[] actualReadValue = (byte[])extensionObject.Body;

                try
                {
                    elementWrite = ByteCount(description.DisplayName, tag.TagNode.Name, actualReadValue, 0);
                }
                catch
                {
                    elementWrite = ByteCount(OPCItem.NodeIdViewModel.DisplayName.Text, tag.TagNode.Name, actualReadValue, 0);
                }

                if (elementWrite.Number == 1)
                {
                    if (value is bool)
                    {
                        var boolValue = (bool)value;
                        actualReadValue[elementWrite.Element - elementWrite.Number] = boolValue ? (byte)1 : (byte)0;
                    }
                    else
                    {
                        actualReadValue[elementWrite.Element - elementWrite.Number] = (byte)value;
                    }
                }
                else
                {
                    byte[] bytevalue = null;
                    Array dataArray = null;
                    int size = 0;
                    if (!elementWrite.isArray)
                        bytevalue = byteConverterToWrite(elementWrite.Type, value);
                    else
                    {
                        bytevalue = new byte[elementWrite.Number];
                        switch (elementWrite.Type)
                        {
                            case "UInt16":
                                size = sizeof(UInt16);
                                dataArray = Array.CreateInstance(typeof(UInt16), elementWrite.Number / size);
                                break;
                            case "Int16":
                                size = sizeof(Int16);
                                dataArray = Array.CreateInstance(typeof(Int16), elementWrite.Number / size);
                                break;
                            case "Float":
                                size = sizeof(float);
                                dataArray = Array.CreateInstance(typeof(float), elementWrite.Number / size);
                                break;
                            case "Int32":
                                size = sizeof(Int32);
                                dataArray = Array.CreateInstance(typeof(Int32), elementWrite.Number / size);
                                break;
                            case "UInt32":
                                size = sizeof(UInt32);
                                dataArray = Array.CreateInstance(typeof(UInt32), elementWrite.Number / size);
                                break;
                            case "Int64":
                                size = sizeof(UInt64);
                                dataArray = Array.CreateInstance(typeof(UInt64), elementWrite.Number / size);
                                break;
                            case "Double":
                                size = sizeof(Double);
                                dataArray = Array.CreateInstance(typeof(Double), elementWrite.Number / size);
                                break;
                            case "UInt64":
                                size = sizeof(UInt64);
                                dataArray = Array.CreateInstance(typeof(UInt64), elementWrite.Number / size);
                                break;
                            case "CharArray":
                            case "String":
                                size = sizeof(Char);
                                dataArray = Array.CreateInstance(typeof(Char), elementWrite.Number / size);
                                break;
                            case "Boolean":
                                size = sizeof(Boolean);
                                dataArray = Array.CreateInstance(typeof(Boolean), elementWrite.Number / size);
                                break;
                            case "Byte":
                                size = sizeof(Byte);
                                dataArray = Array.CreateInstance(typeof(Byte), elementWrite.Number);
                                break;
                        }
                        var array = value as Array;
                        for (int i = 0; i < array.Length; i++)
                            dataArray.SetValue(array.GetValue(i), i);
                        int j = elementWrite.Element - elementWrite.Number; ;
                        bytevalue = actualReadValue;
                        for (int k = elementWrite.Element - elementWrite.Number; k < elementWrite.Element - elementWrite.Number + dataArray.Length; k++)
                        {
                            if (elementWrite.Type == "Boolean")
                            {
                                var boolValue = (bool)dataArray.GetValue(k - elementWrite.Element + elementWrite.Number);
                                bytevalue[k] = boolValue ? (byte)1 : (byte)0;
                            }
                            else
                            {
                                bytevalue = actualReadValue;
                                for (int i = 0; i < size; i++)
                                {
                                    bytevalue[j + i] = (byteConverterToWrite(position.Type, dataArray.GetValue(k - elementWrite.Element + elementWrite.Number)))[i];
                                }
                                j += size;
                            }
                        }


                    }

                    if (elementWrite.Type == "String" || elementWrite.Type == "CharArray")
                    {
                        var valueToRead = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            valueToRead[i] = actualReadValue[elementWrite.Element - elementWrite.Number + i + 1];
                        }
                        var data = byteConverterToRead("Int32", valueToRead);
                        int oldLength = Convert.ToInt32(data);

                        if (oldLength > bytevalue.Count())
                        {
                            //shorter then before
                            var byteToDelete = oldLength - bytevalue.Count();
                            byte[] arrayTowrite = new byte[actualReadValue.Count() - byteToDelete];
                            var lenghtToCopy = (elementWrite.Element - elementWrite.Number + 1);
                            Buffer.BlockCopy(actualReadValue, 0, arrayTowrite, 0, lenghtToCopy);

                            byte[] sLen = new byte[4];
                            InternalByteConverterToWrite("Int32", bytevalue.Count(), ref sLen);
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 1] = sLen[0];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 2] = sLen[1];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 3] = sLen[2];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 4] = sLen[3];
                            for (int i = 0; i < bytevalue.Count(); i++)
                            {
                                arrayTowrite[elementWrite.Element - elementWrite.Number + excludeLengthByte + i] = bytevalue[i];
                            }
                            lenghtToCopy = actualReadValue.Count() - (elementWrite.Element);
                            Buffer.BlockCopy(actualReadValue, elementWrite.Element, arrayTowrite, elementWrite.Element - elementWrite.Number + excludeLengthByte + bytevalue.Count(), lenghtToCopy);
                            extensionObject.Body = arrayTowrite;
                        }
                        else if (oldLength < bytevalue.Count())
                        {
                            //longer than before
                            var byteToAdd = bytevalue.Count() - oldLength;
                            byte[] arrayTowrite = new byte[actualReadValue.Count() + byteToAdd];
                            var lenghtToCopy = (elementWrite.Element - elementWrite.Number + 1);
                            Buffer.BlockCopy(actualReadValue, 0, arrayTowrite, 0, lenghtToCopy);

                            byte[] sLen = new byte[4];
                            InternalByteConverterToWrite("Int32", bytevalue.Count(), ref sLen);
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 1] = sLen[0];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 2] = sLen[1];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 3] = sLen[2];
                            arrayTowrite[elementWrite.Element - elementWrite.Number + 4] = sLen[3];
                            for (int i = 0; i < bytevalue.Count(); i++)
                            {
                                arrayTowrite[elementWrite.Element - elementWrite.Number + excludeLengthByte + i] = bytevalue[i];
                            }
                            lenghtToCopy = actualReadValue.Count() - elementWrite.Element;
                            Buffer.BlockCopy(actualReadValue, elementWrite.Element, arrayTowrite, elementWrite.Element - elementWrite.Number + excludeLengthByte + bytevalue.Count(), lenghtToCopy);
                            extensionObject.Body = arrayTowrite;
                        }
                        else
                        {
                            for (int i = 0; i < oldLength; i++)
                            {
                                actualReadValue[elementWrite.Element - elementWrite.Number + i + excludeLengthByte] = bytevalue[i];
                                extensionObject.Body = actualReadValue;
                            }
                        }
                    }
                    else
                    {
                        if (!elementWrite.isArray)
                        {
                            for (int i = 0; i < elementWrite.Number; i++)
                            {

                                actualReadValue[elementWrite.Element - elementWrite.Number + i] = bytevalue[i];
                                extensionObject.Body = actualReadValue;
                            }
                        }
                        else
                        {
                            for (int i = elementWrite.Element - elementWrite.Number; i < elementWrite.Element; i++)
                            {

                                actualReadValue[i] = bytevalue[i];
                            }
                            extensionObject.Body = actualReadValue;
                        }

                    }
                }

                dataValue.Value = extensionObject;
            }

            return dataValue;
        }

        public uint SyncWriteOpcValue(NodeId tagnodeid, ref object value)
        {
            if (OPCItem == null)
                return StatusCodes.BadNodeIdUnknown;

            if (OPCItem.MonitoredItemViewModel == null)
                return StatusCodes.BadInvalidState;

            //var tagIndex = TagsList.FindIndex(o => o.TagNode.NodeId == tagnodeid);
            //if (tagIndex != -1)
            //{
            //    lock (lockListObject)
            //        TagsList[tagIndex].LastValue = value;
            //}

            object cloneValue = null;

            if (isExtensionObject)
            {
                List<NodeId> tagnodeids = new List<NodeId>() { tagnodeid };
                List<object> values = new List<object>() { value };

                DataValue dataValue = ReadAndMergeExtensionObjectWithWriteValues(ref tagnodeids, ref values);

                cloneValue = Utils.Clone(dataValue);
            }
            else
            {
                cloneValue = Utils.Clone(value);
            }

            StatusCode result = StatusCodes.Good;
            try
            {
                OPCItem.MonitoredItemViewModel.WriteValue(cloneValue);
            }
            catch (ServiceResultException ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, OPCItem.HumanReadable, ex.Message), EventSeverity.High);
                return ex.StatusCode;
            }
            catch (Exception ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, OPCItem.HumanReadable, ex.Message), EventSeverity.High);
                return StatusCodes.BadUnexpectedError;
            }

            return StatusCodes.Good;
        }

        public override bool IsJobAggregable()
        {
            return false;
        }

        public string GetOPCItemEndpointUrl()
        {
            string result = string.Empty;
            try
            {
                result = OPCItem.EndpointUrl;
            } catch { }
            return result;
        }
        #endregion

        #region Properties
        OPCUAEntityReference opcItem;
        public OPCUAEntityReference OPCItem
        {
            get
            {
                return opcItem;
            }
            set { opcItem = value; }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return ret;// string.Format("{0}SA{1:00}", ret, StartAddress);
            }
        }

        internal bool IsInitialized
        {
            get { return bInitialized; }
        }
#endregion

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        public ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }

        public ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        private bool isExtensionObject = false;
        public bool IsExtensionObject
        {
            get { return isExtensionObject;  }
            set { isExtensionObject = value; }
        }
        #endregion
    }
}
