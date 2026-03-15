using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using OPCUAViewModel;
using ViewModelLib;
using UFInterfaces;
using UFUAModel.Extensions;
using System.Threading;
using System.Threading.Tasks;

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

        private static List<LocalizedText> StructList = new List<LocalizedText>();

        /// <summary>
        /// StructName nd StructMembers
        /// </summary>
        private static Dictionary <LocalizedText,string> dictionary = new Dictionary<LocalizedText,string>();
        /// <summary>
        /// EnumTypes
        /// </summary>
        private static List<string> enumList = new List<string>();

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
                OPCItem = new OPCUAEntityReference();
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
                if(OPCItem != null)
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
                if(observerSession != null)
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
                    if(arrayDimension == 0)
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
                    int lenght = (5 + (int)actualReadValue[position.Element]);
                    position.Number = lenght;
                    position.Element += lenght - 1;
                    break;
                default:
                    if (enumList.Contains(dataTypeName) || CheckEnumType(dataTypeName))
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
                    else if (runtime)
                    {
                        try
                        {
                            Position element_CB = new Position();
                            element_CB = ByteCount(dataTypeName, firstTagName, actualReadValue, position.Element);
                            return element_CB;
                        }
                        catch(Exception Ex)
                        {
                            if(!enumList.Contains(dataTypeName))
                                dataTypeName = "UInt32";
                            position.Element += 4;
                            position.Number = 4;
                        }
                        break;                        
                    }
                    else
                        break;
            }
            position.Type = dataTypeName;
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
            int Counter = 0;
            UInt32 arrayDimension = 0;
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
                if(DataTypeName.Contains("_x005C_"))
                    DataTypeName = DataTypeName.Replace("_x005C_", "\\");
                if (substring.Substring(index + 2, 11) == isArray)
                {
                    substring = substring.Substring(index + 15);
                    index = substring.IndexOf("\"");
                    nameOfTag = substring.Substring(0, index);
                    var attribute = OPCItem.NodeIdViewModel.NodeAttributes;
                    ExtensionObject actualValue = (ExtensionObject)attribute[13].Value;
                    byte[] actBody = (byte[])actualValue.Body;
                    element = CountingBytes(element, DataTypeName, actBody, false);
                    
                    byte[] valueToRead = new byte[4]; ;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Counter > 0)
                            valueToRead[i] = actBody[Counter - 4 + i];
                        else
                            valueToRead[i] = actBody[i];
                    }
                    arrayDimension = BitConverter.ToUInt32(valueToRead, 0);
                }
                Counter += (int)(element.Number * arrayDimension);
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
                    dictionary.Add(DataTypeName, submember = SaveStructure(datadictionary, xmlString, NodeId,checkSingleStructure, DataTypeName));
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
                enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                nodeBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[5].NodeId, OPCItem.NodeIdViewModel.sessionViewModel.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.DisplayName
                };

                enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                foreach (var r in enumeratedRefence)
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
                        enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(OPCItem.NodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                        if (enumeratedRefence != null)
                        {
                            if(!enumList.Contains(dataTypeName))
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
            if (tagName.IndexOf("/")!= -1)
            {
                firstTagName = firstTagName.Substring(firstTagName.IndexOf("/") + 1);
            }
            string members =string.Empty;
            exit = false;
            int index = -1;
            int lastIndex = 0;
            int commaIndex = 0;
            string memberName = string.Empty;
            string DataType = string.Empty;
            int arrayDimension;
            dictionary.TryGetValue(structName, out members);
            tagName = tagName.Substring(tagName.LastIndexOf(":") + 1);
                while (index != 0 && !exit)
                {
                    index = members.IndexOf(":") + 1;
                    if (index == 0)
                        continue;
                    lastIndex = members.IndexOf("#");
                    memberName = members.Substring(0, index - 1);
                    DataType = members.Substring(index, lastIndex - index);
                    commaIndex = members.IndexOf(";");
                    arrayDimension = Convert.ToInt16(members.Substring(lastIndex +1 , commaIndex - lastIndex - 1));
                    members = members.Substring(commaIndex + 1);
                    if (tagName == memberName)
                        exit = true;
                    position = CountingBytes(position, DataType, actualReadValue, true, firstTagName, arrayDimension, tagName);
                }
            //position.Type = DataType;
            return position;
        }

        internal byte[] byteConverterToWrite(string stringType, object data)
        {
            byte[] stringAnswer = new byte[position.Number];
            switch (stringType)
            {
                case "WChar":
                case "UInt16":
                    var ValueUint16 = Convert.ToUInt16(data);
                    stringAnswer = BitConverter.GetBytes(ValueUint16);
                    break;
                case "Int16":
                    var ValueInt16 = Convert.ToInt16(data);
                    stringAnswer = BitConverter.GetBytes(ValueInt16);
                    break;
                case "Float":
                    var ValueFloat = Convert.ToSingle(data);
                    stringAnswer = BitConverter.GetBytes(ValueFloat);
                    break;
                case "Int32":
                    var ValueInt32 = Convert.ToInt32(data);
                    stringAnswer = BitConverter.GetBytes(ValueInt32);
                    break;
                case "UInt32":
                    var ValueUInt32 = Convert.ToUInt32(data);
                    stringAnswer = BitConverter.GetBytes(ValueUInt32);
                    break;
                case "Int64":
                    var ValueInt64 = Convert.ToInt64(data);
                    stringAnswer = BitConverter.GetBytes(ValueInt64);
                    break;
                case "Double":
                    var ValueDouble = Convert.ToDouble(data);
                    stringAnswer = BitConverter.GetBytes(ValueDouble);
                    break;
                case "UInt64":
                    var ValueUInt64 = Convert.ToUInt64(data);
                    stringAnswer = BitConverter.GetBytes(ValueUInt64);
                    break;
                case "CharArray":
                    var ValueString = (string)data;
                    stringAnswer = System.Text.Encoding.Default.GetBytes(ValueString);
                    break;
                default:
                    if (enumList.Contains(stringType))
                    {
                        ValueUInt32 = Convert.ToUInt32(data);
                        stringAnswer = BitConverter.GetBytes(ValueUInt32);
                    }
                    break;

            }
            
            return stringAnswer;
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
        internal bool PrepareExecution(String sessionname, List<CommJob> listJob)
        {
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
                            }                               
                            else
                            {
                                List<OpcClientDriverCommJob> list = new List<OpcClientDriverCommJob>();
                                foreach(OpcClientDriverCommJob j in listJob)
                                {
                                    if (j.OPCItem == OPCItem)
                                        list.Add(j);
                                }
                                DataValueChangedFromExtensionObject(m.DataValue, OPCItem, list);
                            }
                        }
                            
                    });


                    if (monitoredItemViewModel.DataValue != null && !(monitoredItemViewModel.DataValue.Value is ExtensionObject))
                         DataValueChanged(monitoredItemViewModel.DataValue);
                    else
                    {
                        List<OpcClientDriverCommJob> list = new List<OpcClientDriverCommJob>();
                        foreach (OpcClientDriverCommJob j in listJob)
                        {
                            if (j.OPCItem == OPCItem)
                                list.Add(j);
                        }
                        if (monitoredItemViewModel.DataValue != null)
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
            
            try
            {

                OPCItem.Resolve(sessionname);
                var session = OPCItem.GetSession(sessionname);
                mystation.SetSessionSettings(sessionname);

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
            if (observer != null)
                observer.Dispose();
            if (observerMonitoredModel != null)
                observerMonitoredModel.Dispose();
            if (observerSession != null)
                observerSession.Dispose();

            observer = null;
            observerMonitoredModel = null;
            observerSession = null;

            bool bRet = false;
            if (OPCItem != null)
                OPCItem.SetInUse(this, false);

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
                
                TagsList[0].LastValue = dataValue.Value;
                
                ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = this, Values = dataValue };
                if (StatusCode.IsBad(dataValue.StatusCode))

                    eJob.ErrorCode = (DriverErrorCodes)dataValue.StatusCode.Code;
                if ((dataValue.Value == null) || (dataTypeval == -1) &&
                   (dataTypeval != (int)UFUAModel.DataType.String))
                {
                    dataValue.StatusCode = StatusCodes.BadTypeMismatch;
                }
                Station.OnJobExecuted(this, eJob);
            }    
        }
        public static object thisLock= new object();
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
            catch(Exception ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, string.Format(string.Format(Properties.Resources.ErrorReadingStruct,ex.Message), OPCItem.HumanReadable, ex.Message), EventSeverity.High);
            }

            Position elementRead = new Position();
            try
            {
                if(!StructList.Contains(description.DisplayName) || string.IsNullOrEmpty(description.DisplayName.Text))
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
                            dictionary.Add(OPCItem.NodeIdViewModel.DisplayName.Text, members);
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
		                        if(schema.Contains(description.DisplayName.ToString()))
		                        {
                                    index = schema.IndexOf("<StructuredType Name=" + "\"" + description.DisplayName.ToString());
                                    if(index!=-1)
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
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                Station.GetCommDriver().OnSystemEvent(null, String.Format(Properties.Resources.ErrorBadConnection, ex.Message), EventSeverity.High);
            }
            ExtensionObject extensionObject = (ExtensionObject)dataValue.Value;
            foreach (OpcClientDriverCommJob sub in s)
            {

                var j = sub;
                var tag = j.TagsList[0];
                try
                {
	                if (extensionObject != null)
	                {
	                    byte[] actualReadValue = (byte[])extensionObject.Body;
	                    System.Diagnostics.Debug.WriteLine("OPCClientDBG - DataValueChangedFromExtensionObject - next tag: {0}", tag.TagNode.Name);
	
	                    if (description != null)
	                        elementRead = ByteCount(description.DisplayName, tag.TagNode.Name, actualReadValue, 0);
	                    else
	                        elementRead = ByteCount(OPCItem.NodeIdViewModel.DisplayName.Text, tag.TagNode.Name, actualReadValue, 0);
	                    
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
	                        for(int i = 0;  i < (elementRead.Number/size); i++)

	                        {
	                            byte[] tempReadValue = new byte[size];
	                            Array.Copy(valueToRead, i * size, tempReadValue, 0, size);
	                            dataArray.SetValue(byteConverterToRead(elementRead.Type,tempReadValue), i);
	                        }
	                        data = dataArray;
	                    }
	
	                    DataValue dataValueTemp = new DataValue(dataValue);
	                    if (dataArray == null)
	                        dataValueTemp.Value = data;
	                    else
	                        dataValueTemp.Value = dataArray;

                        tag.LastValue = dataValueTemp.Value;

                        ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = j, Values = dataValueTemp };
	                    if (StatusCode.IsBad(dataValue.StatusCode))
	
	                        eJob.ErrorCode = (DriverErrorCodes)dataValue.StatusCode.Code;
	                    if ((dataValue.Value == null) || (dataTypeval == -1) &&
	                        (dataTypeval != (int)UFUAModel.DataType.String))
	                    {
	                        dataValue.StatusCode = StatusCodes.BadTypeMismatch;
	                    }

	                    Station.OnJobExecuted(j, eJob);

                    }
                }
                catch(Exception ex)
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
                        returnValue = (int) UFUAModel.DataType.Boolean;
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
            return returnValue ;
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
            if(ch != null && !string.IsNullOrEmpty(ch.HostName))
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

        public override void GetJobData(ref object jobData)
        {
            lock (lockListObject)
            {
                for (int i = 0; i < TagsList.Count; i++)
                {
                    uint dim = TagsList[i].Size;
                }
            }
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

        public override uint OnWriteTag(NodeId tagnodeid, ref object value)
        {
            bool checkSingleStructure = false;
            int index = 0;
            Position elementWrite = new Position();
            if (OPCItem == null)
                return StatusCodes.BadNodeIdUnknown;

            if (OPCItem.MonitoredItemViewModel == null)
            {
                return StatusCodes.BadInvalidState;
            }
            
            if(TagsList.Count > 0)
                TagsList[0].LastValue = value;

            var dataValue = OPCItem.NodeIdViewModel.sessionViewModel.Session.ReadValue((NodeId)OPCItem.NodeIdViewModel.nodeId);
            if (dataValue.Value is ExtensionObject)
            {
                var m_encodings = OPCItem.NodeIdViewModel.sessionViewModel.Session.ReadAvailableEncodings((NodeId)OPCItem.NodeIdViewModel.nodeId);
                ReferenceDescription encodings = m_encodings[0];
                ReferenceDescription description = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDescription((NodeId)encodings.NodeId);
                if (!StructList.Contains(description.DisplayName))
                {
#if !NET_STANDARD
                    Opc.Ua.Client.DataDictionary datadictionary = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDictionary((NodeId)description.NodeId);
#else
                    Opc.Ua.Client.DataDictionary datadictionary = OPCItem.NodeIdViewModel.sessionViewModel.Session.FindDataDictionary((NodeId)description.NodeId).Result;
#endif                    
                    var schema = datadictionary.GetSchema((NodeId)description.NodeId);
                    if (schema.Contains(description.DisplayName.ToString()))
                    {
                        index = schema.IndexOf("<StructuredType Name=" + "\"" + description.DisplayName.ToString());
                        schema = schema.Substring(index);
                        checkSingleStructure = true;
                    }
                    else
                    {
                        checkSingleStructure = false;
                    }
                    var members = SaveStructure(datadictionary, schema, (NodeId)description.NodeId, checkSingleStructure);
                    dictionary.Add(description.DisplayName, members);
                    StructList.Add(description.DisplayName);
                }
                var tag = this.TagsList[0];
                
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
                    if (!position.isArray)
                        bytevalue = byteConverterToWrite(position.Type, value);
                    else
                    {
                        bytevalue = new byte[elementWrite.Number];
                        switch (element.Type)
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
                        for(int i = 0; i < array.Length; i++)
                            dataArray.SetValue(array.GetValue(i), i);
                        int j = elementWrite.Element - elementWrite.Number; ;
                        for (int k = elementWrite.Element - elementWrite.Number; k < elementWrite.Element - elementWrite.Number + dataArray.Length ; k++)
                        {
                            if(elementWrite.Type == "Boolean")
                            {
                                var boolValue = (bool)dataArray.GetValue(j - elementWrite.Element + elementWrite.Number);
                                bytevalue[j] = boolValue ? (byte)1 : (byte)0;
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
                        if (position.Type == "string" || position.Type == "CharArray")
                    {
                        if (actualReadValue[position.Element - position.Number + 1] > bytevalue.Count())
                        {
                            var byteToDelete = actualReadValue[position.Element - position.Number + 1] - bytevalue.Count();
                            byte[] arrayTowrite = new byte[actualReadValue.Count() - byteToDelete];
                            var lenghtToCopy = (position.Element - position.Number);
                            Buffer.BlockCopy(actualReadValue,0,arrayTowrite,0,lenghtToCopy);
                            arrayTowrite[position.Element - position.Number + 1] = (byte)bytevalue.Count();
                            for (int i = 0; i < bytevalue.Count(); i++)
                            {
                                arrayTowrite[position.Element - position.Number + excludeLengthByte + i] = bytevalue[i];
                            }
                            lenghtToCopy = actualReadValue.Count() - (position.Element);
                            Buffer.BlockCopy(actualReadValue, position.Element, arrayTowrite, position.Element - position.Number + excludeLengthByte + bytevalue.Count(), lenghtToCopy);
                            extensionObject.Body = arrayTowrite;
                        }
                        else if (actualReadValue[position.Element - position.Number + 1] < bytevalue.Count())
                        {
                            var byteToAdd = bytevalue.Count() - actualReadValue[position.Element - position.Number + 1];
                            byte[] arrayTowrite = new byte[actualReadValue.Count() + byteToAdd];
                            var lenghtToCopy = (position.Element - position.Number);
                            Buffer.BlockCopy(actualReadValue, 0, arrayTowrite, 0, lenghtToCopy);
                            arrayTowrite[position.Element - position.Number + 1] = (byte)bytevalue.Count();
                            for (int i = 0; i < bytevalue.Count(); i++)
                            {
                                arrayTowrite[position.Element - position.Number + excludeLengthByte + i] = bytevalue[i];
                            }
                            lenghtToCopy = actualReadValue.Count() + byteToAdd -(position.Element + bytevalue.Count());
                            Buffer.BlockCopy(actualReadValue, position.Element, arrayTowrite, position.Element - position.Number + excludeLengthByte + bytevalue.Count(), lenghtToCopy);
                            extensionObject.Body = arrayTowrite;
                            extensionObject.Body = arrayTowrite;
                        }
                        else
                        {
                            for (int i = 0; i < elementWrite.Number; i++)
                            {
                                actualReadValue[elementWrite.Element - elementWrite.Number + i] = bytevalue[i];
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
                var cloneExtendedValue = Utils.Clone(dataValue);
                StatusCode extendedResult = StatusCodes.Good;
                try
                {
                    OPCItem.MonitoredItemViewModel.WriteValue(cloneExtendedValue);
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
            var cloneValue = Utils.Clone(value);
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

#endregion
    }
}
