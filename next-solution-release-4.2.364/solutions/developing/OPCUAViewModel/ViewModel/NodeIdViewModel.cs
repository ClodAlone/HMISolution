using System;
using System.Linq;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using System.Windows.Input;
using Utilities;
using System.Collections.Generic;
using System.Text;
using UFInterfaces;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Controls;
#endif
using System.IO;
using System.Threading;
#else
using System.Threading.Tasks;
#endif

namespace OPCUAViewModel
{
    public class NodeIdViewModel : TreeViewItemViewModel
    {
#region Members
        public ExpandedNodeId nodeId { get; protected set; }
#endregion

#region Constructor

        public NodeIdViewModel(ExpandedNodeId nid, SessionViewModel s)
            : base(null, false)
        {
            if (nid == null)
                throw new ArgumentNullException("NodeId");
            if (s == null)
                throw new ArgumentNullException("Session");

            nodeId = nid;
            // session = s.Session;
            sessionViewModel = s;
            // Title = GetTargetText(nid);
        }

#endregion

#region Methods
        protected override void OnDispose()
        {
            base.OnDispose();

            ClearCachedProperties();
        }

        internal void ReloadNodeProperties()
        {
            var nodeProperties = new SafeObservableCollection<NodeIdViewModel>();
            var mapProp = new Dictionary<string, NodeIdViewModel>();
            try
            {
                ReadProperties(nodeProperties, mapProp);
            }
            catch (Exception ex)
            { }

            lock (lockObject)
            {
                ClearCachedProperties();
                _nodeProperties = nodeProperties;
                mapProperties = mapProp;
            }

            OnPropertyChanged("NodeProperties");
        }

        void ClearCachedProperties()
        {
            lock (lockObject)
            {
                if (_nodeProperties != null)
                {
                    foreach (var v in _nodeProperties)
                        v.Dispose();
                    _nodeProperties = null;
                    mapProperties = null;
                }
            }
        }

#if WINDOWS_UWP
        async void EnsureNodeCache()
        {
            while (!sessionViewModel.DataTypeFetched)
                await Task.Delay(50);
        }
#else
        void EnsureNodeCache()
        {
            SpinWait.SpinUntil(() =>
            {
                return sessionViewModel.DataTypeFetched;
            }, 50);         

        }
#endif

        public String GetTargetText(ExpandedNodeId id)
        {
            if (id == null)
                return null;

            try
            {
                EnsureNodeCache();

                Node node = session.NodeCache.Find(nodeId) as Node;
                if (node == null)
                    return null;

                if (node.DisplayName != null && !String.IsNullOrEmpty(node.DisplayName.Text))
                {
                    return node.DisplayName.Text;
                }

                if (node.BrowseName != null)
                {
                    return node.BrowseName.Name;
                }
            }
            catch
            {

            }

            return null;
        }

        public String MakeRelative(ExpandedNodeId typeDefParent)
        {
            EnsureNodeCache();
            Node node = session.NodeCache.Find(nodeId) as Node;

            if (node == null)
                return String.Empty;

            if (parent == null)
                parent = FindParent(node);

            if (parent != null)
            {
                List<Node> parents = new List<Node>();
                parents.Add(parent);

                while (parent.NodeClass != NodeClass.ObjectType && parent.NodeClass != NodeClass.VariableType)
                {
                    parent = FindParent(parent);

                    if (parent == null)
                        break;

                    if (parent.NodeId == typeDefParent)
                    {
                        StringBuilder relativePath = new StringBuilder();

                        for (int ii = parents.Count - 1; ii >= 0; ii--)
                            relativePath.AppendFormat("/{0}", parents[ii].BrowseName);

                        return relativePath.ToString();
                    }

                    parents.Add(parent);
                }
            }

            return String.Empty;
        }

        public Node FindParent(Node node)
        {
            try
            {
                IList<IReference> parents = node.ReferenceTable.Find(ReferenceTypeIds.Aggregates, true, true, session.TypeTree);
                // IList<IReference> parents = node.ReferenceTable.Find(ReferenceTypeIds.HasModelParent, false, false, session.TypeTree);
                if (parents.Count == 0)
                    parents = node.ReferenceTable.Find(ReferenceTypeIds.Organizes, true, true, session.TypeTree);

                if (parents.Count > 0)
                {
                    NodeId modellingRule = node.ModellingRule;

                    bool followToType = false;

#if !NET_STANDARD
                    if (modellingRule == Objects.ModellingRule_MandatoryShared)
                    {
                        followToType = true;
                    }
#endif

                    EnsureNodeCache();
                    foreach (IReference parentReference in parents)
                    {
                        Node parent = session.NodeCache.Find(parentReference.TargetId) as Node;

                        if (followToType)
                        {
                            if (parent.NodeClass == NodeClass.VariableType || parent.NodeClass == NodeClass.ObjectType)
                            {
                                if (parent.NodeId == ObjectIds.ObjectsFolder ||
                                    parent.NodeId == ObjectIds.RootFolder)
                                    return null;
                                return parent;
                            }
                        }
                        else
                        {
                            if (parent.NodeId == ObjectIds.ObjectsFolder ||
                                parent.NodeId == ObjectIds.RootFolder)
                                return null;
                            return parent;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public class AttributeField
        {
            public uint attributeId { get; set; }
            public String Name { get; set; }
            public String Value { get; set; }
            public String Status { get; set; }
        }

        public List<AttributeField> GetReadableAttributesList()
        {
            List<AttributeField> list = new List<AttributeField>();
            foreach(KeyValuePair<uint, DataValue> node in NodeAttributes)
            {
                AttributeField af = new AttributeField()
                {
                    attributeId = node.Key,
                    Name = Attributes.GetBrowseName(node.Key),
                    Value = FormatAttributeValue(node.Key, node.Value.Value),
                    Status = String.Format("{0}", node.Value.StatusCode)
                };
                list.Add(af);
            }

            return list;
        }

        /// <summary>
        /// Checks if the datatype id is a built-in type.
        /// </summary>
        private static bool IsBuiltInType(ExpandedNodeId nodeId, out BuiltInType builtinType)
        {
            builtinType = BuiltInType.Null;

            if (nodeId.ServerIndex > 0 || nodeId.NamespaceIndex != 0 || nodeId.IdType != IdType.Numeric)
            {
                return false;
            }

            uint id = (uint)nodeId.Identifier;

            if (id > 0 && id <= DataTypes.BaseDataType)
            {
                builtinType = (BuiltInType)(int)id;
                return true;
            }

            switch (id)
            {
                case DataTypes.Enumeration: { builtinType = BuiltInType.Int32; break; }
                case DataTypes.Number: { builtinType = BuiltInType.Double; break; }
                case DataTypes.Integer: { builtinType = BuiltInType.Int64; break; }
                case DataTypes.UInteger: { builtinType = BuiltInType.UInt64; break; }

                default:
                    {
                        return false;
                    }
            }

            return true;
        }

        /// <summary>
        /// Finds the built-in type for the datatype.
        /// </summary>
        private BuiltInType GetBuiltInType(ExpandedNodeId datatypeId)
        {
            BuiltInType builtinType = BuiltInType.Variant;

            ExpandedNodeId subtypeId = datatypeId;

            EnsureNodeCache();
            while (!IsBuiltInType(subtypeId, out builtinType))
            {
                IList<INode> supertypeIds = session.NodeCache.Find(subtypeId, ReferenceTypeIds.HasSubtype, true, true);

                if (supertypeIds.Count == 0)
                {
                    break;
                }

                subtypeId = supertypeIds[0].NodeId;
            }

            return builtinType;
        }

        private string FormatAttributeValue(uint attributeId, object value)
        {
            switch (attributeId)
            {
                case Attributes.NodeClass:
                    {
                        if (value != null)
                        {
                            return String.Format("{0}", Enum.ToObject(typeof(NodeClass), value));
                        }

                        return "(null)";
                    }

                case Attributes.DataType:
                    {
                        NodeId datatypeId = value as NodeId;

                        if (datatypeId != null)
                        {
                            EnsureNodeCache();
                            INode datatype = session.NodeCache.Find(datatypeId);

                            return datatype != null ? String.Format("{0}", datatype.DisplayName.Text) : String.Format("{0}", datatypeId);
                        }

                        return String.Format("{0}", value);
                    }

                case Attributes.ValueRank:
                    {
                        int? valueRank = value as int?;

                        if (valueRank != null)
                        {
                            switch (valueRank.Value)
                            {
                                case ValueRanks.Scalar: return "Scalar";
                                case ValueRanks.OneDimension: return "OneDimension";
                                case ValueRanks.OneOrMoreDimensions: return "OneOrMoreDimensions";
                                case ValueRanks.Any: return "Any";

                                default:
                                    {
                                        return String.Format("{0}", valueRank.Value);
                                    }
                            }
                        }

                        return String.Format("{0}", value);
                    }

                case Attributes.MinimumSamplingInterval:
                    {
                        double? minimumSamplingInterval = value as double?;

                        if (minimumSamplingInterval != null)
                        {
                            if (minimumSamplingInterval.Value == MinimumSamplingIntervals.Indeterminate)
                            {
                                return "Indeterminate";
                            }

                            else if (minimumSamplingInterval.Value == MinimumSamplingIntervals.Continuous)
                            {
                                return "Continuous";
                            }

                            return String.Format("{0}", minimumSamplingInterval.Value);
                        }

                        return String.Format("{0}", value);
                    }

                case Attributes.AccessLevel:
                case Attributes.UserAccessLevel:
                    {
                        byte accessLevel = Convert.ToByte(value);

                        StringBuilder bits = new StringBuilder();

                        if ((accessLevel & AccessLevels.CurrentRead) == AccessLevels.CurrentRead)
                        {
                            bits.Append("Readable");
                        }

                        if ((accessLevel & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("Writeable");
                        }

                        if ((accessLevel & AccessLevels.HistoryRead) == AccessLevels.HistoryRead)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History");
                        }

                        if ((accessLevel & AccessLevels.HistoryWrite) == AccessLevels.HistoryWrite)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History Update");
                        }

                        if (bits.Length == 0)
                        {
                            bits.Append("No Access");
                        }

                        return String.Format("{0}", bits);
                    }

                case Attributes.EventNotifier:
                    {
                        byte notifier = Convert.ToByte(value);

                        StringBuilder bits = new StringBuilder();

                        if ((notifier & EventNotifiers.SubscribeToEvents) == EventNotifiers.SubscribeToEvents)
                        {
                            bits.Append("Subscribe");
                        }

                        if ((notifier & EventNotifiers.HistoryRead) == EventNotifiers.HistoryRead)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History");
                        }

                        if ((notifier & EventNotifiers.HistoryWrite) == EventNotifiers.HistoryWrite)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History Update");
                        }

                        if (bits.Length == 0)
                        {
                            bits.Append("No Access");
                        }

                        return String.Format("{0}", bits);
                    }

                default:
                    {
                        return String.Format("{0}", value);
                    }
            }
        }

        private string GetTargetIcon()
        {
            EnsureNodeCache();
            Node node = session.NodeCache.Find(nodeId) as Node;

            NodeClass nodeClass = (NodeClass)node.NodeClass;

            // make sure the type definition is in the cache.
            INode typeDefinition = session.NodeCache.Find(node.TypeDefinitionId);

            switch (nodeClass)
            {
                case NodeClass.Object:
                    {
                        if (session.TypeTree.IsTypeOf(node.TypeDefinitionId, ObjectTypeIds.FolderType))
                        {
                            return "foldr";
                        }

                        return "table";
                    }

                case NodeClass.Variable:
                    {
                        if (session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypeIds.PropertyType))
                        {
                            return "addbk";
                        }

                        return "analog";
                    }
            }

            return "effects";
        }

        private bool IsParentType(Node node)
        {
            NodeClass nodeClass = (NodeClass)node.NodeClass;

            // make sure the type definition is in the cache.
            EnsureNodeCache();
            INode typeDefinition = session.NodeCache.Find(node.TypeDefinitionId);

            switch (nodeClass)
            {
                case NodeClass.Object:
                    {
                        if (session.TypeTree.IsTypeOf(node.TypeDefinitionId, ObjectTypeIds.FolderType))
                            return false;
                        return true;
                    }

                case NodeClass.Variable:
                    {
                        if (session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypeIds.PropertyType))
                            return false;
                        return true;
                    }
            }

            return false;
        }

        private void ReadAttributes(Dictionary<uint, DataValue> attributes)
        {
            attributes.Clear();

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingAttributes);

            // build list of attributes to read.
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            Attributes.GetIdentifiers().ToList().ForEach(attributeId =>
            {
                ReadValueId valueId = new ReadValueId 
                {
                    NodeId = ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris), 
                    AttributeId = attributeId, 
                    IndexRange = null, 
                    DataEncoding = null 
                };
                nodesToRead.Add(valueId);
            });

            // read attributes.
            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                // check reading a valid node
                if (nodesToRead[ii].AttributeId == Attributes.NodeId && values[ii].Value == null)
                    throw new ServiceResultException(StatusCodes.BadUnexpectedError, "The nodeid is not valid and did not return valid attributes from the server.");


                // check if node supports attribute.
                if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                    continue;

                attributes.Add(nodesToRead[ii].AttributeId, values[ii]);

                // field.Name           = Attributes.GetBrowseName(nodesToRead[ii].AttributeId);
                if (diagnosticInfos != null && ii < diagnosticInfos.Count)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingReadingAttributesError, diagnosticInfos[ii].InnerStatusCode);
                }
            }
        }

        private bool ReadIsUserExecutable()
        {
            // build list of attributes to read.
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            ReadValueId valueId = new ReadValueId
            {
                NodeId = ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris),
                AttributeId = Attributes.UserExecutable,
                IndexRange = null,
                DataEncoding = null
            };
            nodesToRead.Add(valueId);

            // read attributes.
            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                // check if node supports attribute.
                if (values[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                    continue;

                byte value = Convert.ToByte(values[ii].Value);
                return value != 0;
            }

            return false;
        }

        ArgumentCollection GetArguments(QualifiedName browseName)
        {
            // find the method.
            EnsureNodeCache();
            MethodNode method = session.NodeCache.Find(nodeId) as MethodNode;

            if (method == null)
                return null;

            // fetch the argument list.
            VariableNode argumentsNode = session.NodeCache.Find(nodeId, ReferenceTypeIds.HasProperty, false, true, browseName) as VariableNode;

            if (argumentsNode == null)
                return null;

            // read the value from the server.
            DataValue value = session.ReadValue(argumentsNode.NodeId);

            ExtensionObject[] argumentsList = value.Value as ExtensionObject[];

            ArgumentCollection ret = new ArgumentCollection();
            if (argumentsList != null)
            {
                for (int ii = 0; ii < argumentsList.Length; ii++)
                {
                    ret.Add(argumentsList[ii].Body as Argument);
                }
            }

            return ret;
        }

        public VariantCollection CallMethod(params Variant[] paramlist)
        {
            return InternalCallMethod(ParentId, null, paramlist);
        }

        public VariantCollection CallMethod(RequestHeader requestHeader, params Variant[] paramlist)
        {
            return InternalCallMethod(ParentId, requestHeader, paramlist);
        }

        public VariantCollection CallMethod2(NodeId objectId, params Variant[] paramlist)
        {
            return InternalCallMethod(objectId, null, paramlist);
        }

        VariantCollection InternalCallMethod(NodeId objectId, RequestHeader requestHeader, params Variant[] paramlist)
        {
            if (session.KeepAliveStopped || !IsMethod || !IsMethodExecutable)
                return null;

            // build list of methods to call.
            CallMethodRequestCollection methodsToCall = new CallMethodRequestCollection();

            CallMethodRequest request = new CallMethodRequest
            {
                ObjectId = objectId != null ? ExpandedNodeId.ToNodeId(objectId, session.NamespaceUris) : null,
                MethodId = ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris),
                Handle = this
            };

            if (paramlist != null)
                paramlist.ToList().ForEach(v =>
                {
                    request.InputArguments.Add(v);
                });
            methodsToCall.Add(request);

            // call the methods.
            CallMethodResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            ResponseHeader responseHeader = session.Call(
                requestHeader,
                methodsToCall,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, methodsToCall);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, methodsToCall);

            VariantCollection vtcol = new VariantCollection();
            results.ForEach(res =>
            {
                if (StatusCode.IsBad(res.StatusCode))
                {
                    throw new ServiceResultException(new ServiceResult(res.StatusCode, 0, diagnosticInfos, responseHeader.StringTable));
                }

                vtcol.AddRange(res.OutputArguments);
            });

            return vtcol;
        }

        private void ReadProperties(SafeObservableCollection<NodeIdViewModel> properties, Dictionary<String, NodeIdViewModel> mapProperties)
        {
            using (var updater = new CollectionUpdater(properties))
            {
                properties.Clear();

                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingProperties);

                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

                Browser b = new Browser(session)
                {
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasProperty,
                    IncludeSubtypes = true,
                    NodeClassMask = (int)NodeClass.Variable,
                    ContinueUntilDone = true
                };

                ReferenceDescriptionCollection references = b.Browse(ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris));

                foreach (ReferenceDescription reference in references)
                {
                    ReadValueId valueId = new ReadValueId
                    {
                        NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris),
                        AttributeId = Attributes.Value,
                        IndexRange = null,
                        DataEncoding = null
                    };

                    nodesToRead.Add(valueId);
                }

                // check for empty list.
                if (nodesToRead.Count == 0)
                    return;

                // read values.
                DataValueCollection values;
                DiagnosticInfoCollection diagnosticInfos;

                session.Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    nodesToRead,
                    out values,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(values, nodesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                // update control.
                for (int ii = 0; ii < nodesToRead.Count; ii++)
                {
                    NodeIdViewModel nivm = new NodeIdViewModel(ExpandedNodeId.ToNodeId(references[ii].NodeId, session.NamespaceUris), sessionViewModel);
                    nivm.DataValue = values[ii];
                    properties.Add(nivm);

                    mapProperties.Add(references[ii].ToString(), nivm);

                    if (diagnosticInfos != null && ii < diagnosticInfos.Count)
                    {
                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingReadingPropertiesError, diagnosticInfos[ii].InnerStatusCode);
                    }
                }
            }
        }

#endregion

#region Properties

        string title;
        public override string Title
        {
            get
            {
                if (String.IsNullOrEmpty(title))
                    title = GetTargetText(nodeId);
                return title;
            }
        }

        /// <summary>
        /// The minimum severity for the events of interest.
        /// </summary>
        public EventSeverity Severity;

        /// <summary>
        /// The types for the events of interest.
        /// </summary>
        public IList<NodeId> EventTypes;

        /// <summary>
        /// Whether suppressed or shelved condition events are of interest.
        /// </summary>
        public bool IgnoreSuppressedOrShelved;

        /// <summary>
        /// The select clauses to use with the filter.
        /// </summary>
        public SimpleAttributeOperandCollection SelectClauses;

        private DataValue _dataValue;
        public DataValue DataValue
        {
            get
            {
                return _dataValue;
            }
            set
            {
                if (value == _dataValue)
                    return;

                _dataValue = value;

                OnPropertyChanged("DataValue");
            }
        }

        bool? bExist;
        public bool Exist
        {
            get
            {
                if (bExist.HasValue)
                    return bExist.Value;
                if (session == null)
                    return false;

                EnsureNodeCache();
                bExist = session.NodeCache.IsValid(nodeId);

                return bExist.Value;
            }
        }

        public bool IsVariable
        {
            get
            {
                if (session == null)
                    return false;

                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                if (node == null)
                    return false;
                return node.NodeClass == NodeClass.Variable;
            }
        }

        public bool IsObject
        {
            get
            {
                if (session == null)
                    return false;

                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                if (node == null)
                    return false;
                return node.NodeClass == NodeClass.Object;
            }
        }

        public bool IsMethod
        {
            get
            {
                if (session == null)
                    return false;

                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                if (node == null)
                    return false;
                return node.NodeClass == NodeClass.Method;
            }
        }

        public bool IsMethodExecutable
        {
            get
            {
                if (session == null)
                    return false;
//#if DEBUG
//                System.Diagnostics.Debug.WriteLine(String.Format("Checking if Method is executable {0}", nodeId));
//#endif
                if (!session.Connected)
                    return false;
                //MethodNode node = session.ReadNode(ExpandedNodeId.ToNodeId(nodeId, session.NamespaceUris)) as MethodNode;
                //return node != null && node.UserExecutable;
                return ReadIsUserExecutable();
            }
        }

        ArgumentCollection inputArgument;
        public ArgumentCollection InputArgument
        {
            get
            {
                if (inputArgument == null)
                {
                    inputArgument = GetArguments(Opc.Ua.BrowseNames.InputArguments);
                    if (inputArgument == null)
                        return null;
                    foreach (var arg in inputArgument)
                        arg.Value = Helpers.GetDefaultValue(arg.DataType, arg.ValueRank);
                }
                return inputArgument;
            }
        }

        public VariantCollection InputArgumentValues
        {
            get
            {
                var ret = new VariantCollection();
                if (InputArgument != null)
                {
                    foreach (var arg in InputArgument)
                        ret.Add(new Variant(arg.Value ?? Helpers.GetDefaultValue(arg.DataType, arg.ValueRank)));
                }
                return ret;
            }
        }

        public ArgumentCollection outpuArgument;
        public ArgumentCollection OutpuArgument
        {
            get
            {
                if (outpuArgument == null)
                    outpuArgument = GetArguments(Opc.Ua.BrowseNames.OutputArguments);
                return outpuArgument;
            }
        }

        public VariantCollection OutpuArgumentValues
        {
            get
            {
                var ret = new VariantCollection();
                foreach (var arg in OutpuArgument)
                    ret.Add(new Variant(arg.Value ?? Helpers.GetDefaultValue(arg.DataType, arg.ValueRank)));
                return ret;
            }
        }

        NodeId parentId;
        public NodeId ParentId
        {
            get
            {
                if (parentId == null)
                {
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(nodeId) as Node;

                    if (node != null)
                    {
                        if (parent == null)
                            parent = FindParent(node);
                        if (parent != null)
                            parentId = parent.NodeId;
                    }
                }

                return parentId;
            }
        }

        NodeId parentedTypeId;
        public NodeId ParentedTypeId
        {
            get
            {
                if (parentedTypeId == null && session != null)
                {
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(nodeId) as Node;

                    if (node != null)
                    {
                        Node parent = FindParent(node);
                        if (parent == null)
                            return null;
                        while (!IsParentType(parent))
                        {
                            var ret = FindParent(parent);
                            if (ret == null)
                                break;
                            parent = ret;
                        }

                        parentedTypeId = parent.NodeId;
                    }
                }

                return parentedTypeId;
            }
        }

        Node parent;
        List<Node> parents;
        String relativePath;
        public String RelativePath
        {
            get
            {
                if (String.IsNullOrEmpty(relativePath))
                {
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(nodeId) as Node;

                    if (node == null)
                        return String.Empty;

                    if (parent == null)
                        parent = FindParent(node);

                    if (parent != null)
                    {
                        if (parents == null)
                        {
                            parents = new List<Node>();
                            parents.Add(parent);

                            while (true)
                            {
                                parent = FindParent(parent);
                                if (parent == null)
                                    break;
                                parents.Add(parent);
                            }
                        }

                        Node parentObject = null;
                        for (int ii = parents.Count - 1; ii >= 0; ii--)
                        {
                            if (IsParentType(parents[ii]))
                            {
                                parentObject = parents[ii];
                                break;
                            }
                        }

                        bool bSkip = parentObject == null;
                        StringBuilder relative = new StringBuilder();
                        for (int ii = parents.Count - 1; ii >= 0; ii--)
                        {
                            if (!bSkip)
                            {
                                if (parentObject == parents[ii])
                                    bSkip = true;
                                continue;
                            }

                            // if (ii == parents.Count - 2)
                            if (relative.Length == 0)
                                relative.AppendFormat("{0}", parents[ii].BrowseName);
                            else
                                relative.AppendFormat("/{0}", parents[ii].BrowseName);
                        }
                        if (relative.Length > 0)
                            relative.AppendFormat("/{0}", node.BrowseName);
                        else
                            relative.AppendFormat("{0}", node.BrowseName);

                        relativePath = relative.ToString();
                    }
                }

                return relativePath;
            }
        }

        String readablePath;
        public String ReadablePath
        {
            get
            {
                if (String.IsNullOrEmpty(readablePath))
                {
                    try
                    {
                        EnsureNodeCache();
                        Node node = session.NodeCache.Find(nodeId) as Node;

                        if (node == null)
                            return String.Empty;

                        if (parent == null)
                            parent = FindParent(node);

                        if (parent != null)
                        {
                            if (parents == null)
                            {
                                parents = new List<Node>();
                                parents.Add(parent);

                                while (true)
                                {
                                    parent = FindParent(parent);
                                    if (parent == null)
                                        break;
                                    parents.Add(parent);
                                }
                            }

                            Node parentObject = null;
                            for (int ii = parents.Count - 1; ii >= 0; ii--)
                            {
                                if (IsParentType(parents[ii]))
                                {
                                    parentObject = parents[ii];
                                    break;
                                }
                            }

                            bool bSkip = parentObject == null;
                            StringBuilder readable = new StringBuilder();
                            for (int ii = parents.Count - 1; ii >= 0; ii--)
                            {
                                if (!bSkip)
                                {
                                    if (parentObject == parents[ii])
                                        bSkip = true;
                                    continue;
                                }
                                // if (ii == parents.Count - 2)
                                if (readable.Length == 0)
                                    readable.AppendFormat("{0}", parents[ii].DisplayName);
                                else
                                    readable.AppendFormat("/{0}", parents[ii].DisplayName);
                            }
                            if (readable.Length > 0)
                                readable.AppendFormat("/{0}", node.DisplayName);
                            else
                                readable.AppendFormat("{0}", node.DisplayName);

                            readablePath = readable.ToString();
                        }
                    }
                    catch { }
                }

                return readablePath;
            }
        }

        String completePath;
        public String CompletePath
        {
            get
            {
                if (String.IsNullOrEmpty(completePath))
                {
                    try
                    {
                        EnsureNodeCache();
                        Node node = session.NodeCache.Find(nodeId) as Node;

                        if (node == null)
                            return String.Empty;

                        if (parent == null)
                            parent = FindParent(node);

                        if (parent != null)
                        {
                            if (parents == null)
                            {
                                parents = new List<Node>();
                                parents.Add(parent);

                                while (parent != null)
                                {
                                    parent = FindParent(parent);
                                    if (parent != null)
                                        parents.Add(parent);
                                }
                            }

                            StringBuilder relative = new StringBuilder();

                            for (int ii = parents.Count - 1; ii >= 0; ii--)
                            {
                                if (ii == parents.Count - 1)
                                    relative.AppendFormat("{0}", parents[ii].BrowseName);
                                else
                                    relative.AppendFormat("/{0}", parents[ii].BrowseName);
                            }
                            relative.AppendFormat("/{0}", node.BrowseName);

                            completePath = relative.ToString();
                        }
                        else
                        {
                            StringBuilder relative = new StringBuilder();
                            relative.AppendFormat("{0}", node.BrowseName);
                            completePath = relative.ToString();
                        }
                    }
                    catch { }
                }

                return completePath;
            }
        }

        public String StartingAddress
        {
            get
            {
                return CompletePath.Replace("/" + RelativePath, "");
            }
        }


        Dictionary<String, NodeIdViewModel> mapProperties;
        SafeObservableCollection<NodeIdViewModel> _nodeProperties;
        public SafeObservableCollection<NodeIdViewModel> NodeProperties
        {
            get
            {
                lock (lockObject)
                {
                    if (_nodeProperties != null)
                        return _nodeProperties;

                    mapProperties = new Dictionary<string, NodeIdViewModel>();
                    _nodeProperties = new SafeObservableCollection<NodeIdViewModel>();
                    try
                    {
                        ReadProperties(_nodeProperties, mapProperties);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                return _nodeProperties;
            }
        }

        public Range Range
        {
            get
            {
                if (NodeProperties.Count > 0)
                {
                    NodeIdViewModel rangeId;
                    if (mapProperties.TryGetValue(Opc.Ua.BrowseNames.EURange, out rangeId))
                        return ExtensionObject.ToEncodeable(rangeId.DataValue.Value as ExtensionObject) as Range;
                }

                return null;
            }
        }

        public EUInformation EUInformation
        {
            get
            {
                if (NodeProperties.Count > 0)
                {
                    NodeIdViewModel rangeId;
                    if (mapProperties.TryGetValue(Opc.Ua.BrowseNames.EngineeringUnits, out rangeId))
                        return ExtensionObject.ToEncodeable(rangeId.DataValue.Value as ExtensionObject) as EUInformation;
                }

                return null;
            }
        }

        public LocalizedText[] EnumStrings
        {
            get
            {
                if (NodeProperties.Count > 0)
                {
                    NodeIdViewModel rangeId;
                    if (mapProperties.TryGetValue(Opc.Ua.BrowseNames.EnumStrings, out rangeId))
                        return rangeId.DataValue.Value as LocalizedText[];
                }

                return null;
            }
        }

        public LocalizedText TrueState
        {
            get
            {
                if (NodeProperties.Count > 0)
                {
                    NodeIdViewModel rangeId;
                    if (mapProperties.TryGetValue(Opc.Ua.BrowseNames.TrueState, out rangeId))
                        return rangeId.DataValue.Value as LocalizedText;
                }

                return null;
            }
        }

        public LocalizedText FalseState
        {
            get
            {
                if (NodeProperties.Count > 0)
                {
                    NodeIdViewModel rangeId;
                    if (mapProperties.TryGetValue(Opc.Ua.BrowseNames.FalseState, out rangeId))
                        return rangeId.DataValue.Value as LocalizedText;
                }

                return null;
            }
        }

        public bool IsEventNotifier
        {
            get
            {
                if (session == null)
                    return false;

                return IsSubscribeToEvents || IsEventHistoryRead || IsEventHistoryWrite;
            }
        }

        public bool IsSubscribeToEvents
        {
            get
            {
                try
                {
                    DataValue value;
                    if (!NodeAttributes.TryGetValue(Attributes.EventNotifier, out value))
                        return false;

                    byte eventNotifier = Convert.ToByte(value.Value);
                    return (eventNotifier & EventNotifiers.SubscribeToEvents) == EventNotifiers.SubscribeToEvents;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool IsEventHistoryRead
        {
            get
            {
                if (session == null)
                    return false;

                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.EventNotifier, out value))
                    return false;

                byte eventNotifier = Convert.ToByte(value.Value);
                return (eventNotifier & EventNotifiers.HistoryRead) == EventNotifiers.HistoryRead;
            }
        }

        public bool IsEventHistoryWrite
        {
            get
            {
                if (session == null)
                    return false;

                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.EventNotifier, out value))
                    return false;

                byte eventNotifier = Convert.ToByte(value.Value);
                return (eventNotifier & EventNotifiers.HistoryWrite) == EventNotifiers.HistoryWrite;
            }
        }

        public bool IsHistorizing
        {
            get
            {
                if (session == null)
                    return false;

                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.Historizing, out value))
                    return false;

                bool bHistorizing = Convert.ToBoolean(value.Value);
                return bHistorizing;
            }
        }

        public bool IsTypeDefinitionNode
        {
            get
            {
                if (session == null)
                    return false;

                return NodeClass == NodeClass.ObjectType ||
                       NodeClass == NodeClass.VariableType;
            }
        }

        public void InvalidateAttributes()
        {
            lock (lockObject)
            {
                _nodeAttributes = null;
            }
        }

        Dictionary<uint, DataValue> _nodeAttributes;
        public Dictionary<uint, DataValue> NodeAttributes
        {
            get
            {
                lock (lockObject)
                {
                    if (_nodeAttributes != null && _nodeAttributes.Count > 0)
                        return _nodeAttributes;

                    _nodeAttributes = new Dictionary<uint, DataValue>();
                    try
                    {
                        ReadAttributes(_nodeAttributes);
                    }
                    catch (Exception ex)
                    {
                        LastMessage = String.Format("{0} - {1} : {2}", Title, Properties.Resource.OPCUABrowsingReadingAttributesError, ex.Message);
                    }
                }

                return _nodeAttributes;
            }
        }

        public bool IsReadable
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.AccessLevel, out value))
                    return true;

                byte accessLevel = Convert.ToByte(value.Value);

                return (accessLevel & AccessLevels.CurrentRead) == AccessLevels.CurrentRead ||
                       (accessLevel & AccessLevels.CurrentReadOrWrite) == AccessLevels.CurrentReadOrWrite;
            }
        }

        public bool IsUserReadable
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.UserAccessLevel, out value))
                    return true;

                byte accessLevel = Convert.ToByte(value.Value);

                return (accessLevel & AccessLevels.CurrentRead) == AccessLevels.CurrentRead ||
                       (accessLevel & AccessLevels.CurrentReadOrWrite) == AccessLevels.CurrentReadOrWrite;
            }
        }

        public bool IsWritable
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.AccessLevel, out value))
                    return true;

                byte accessLevel = Convert.ToByte(value.Value);

                return (accessLevel & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite ||
                       (accessLevel & AccessLevels.CurrentReadOrWrite) == AccessLevels.CurrentReadOrWrite;
            }
        }

        public bool IsUserWritable
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.UserAccessLevel, out value))
                    return true;

                byte accessLevel = Convert.ToByte(value.Value);

                return (accessLevel & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite ||
                       (accessLevel & AccessLevels.CurrentReadOrWrite) == AccessLevels.CurrentReadOrWrite;
            }
        }

        public bool IsAnalogType
        {
            get 
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;

                // make sure the type definition is in the cache.
                INode typeDefinition = session.NodeCache.Find(node.TypeDefinitionId);

                return session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypes.AnalogItemType);
            }
        }

        public bool IsDiscreteItemType
        {
            get
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;

                // make sure the type definition is in the cache.
                INode typeDefinition = session.NodeCache.Find(node.TypeDefinitionId);

                return session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypes.DiscreteItemType);
            }
        }

        public bool IsTwoStateDiscreteType
        {
            get
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;

                // make sure the type definition is in the cache.
                INode typeDefinition = session.NodeCache.Find(node.TypeDefinitionId);

                return session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypes.TwoStateDiscreteType);
            }
        }

        public bool IsScalar
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.ValueRank, out value))
                    return false;

                int? valueRank = value.Value as int?;
                return valueRank != null && valueRank == ValueRanks.Scalar;
            }
        }

        public bool IsByteStringDataType
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.DataType, out value))
                    return false;
                NodeId datatypeId = value.Value as NodeId;
                if (datatypeId == null)
                    return false;

                BuiltInType builtinType = GetBuiltInType(datatypeId);
                return builtinType == BuiltInType.ByteString;
            }
        }

        public bool IsBoolean
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.DataType, out value))
                    return false;
                NodeId datatypeId = value.Value as NodeId;
                if (datatypeId == null)
                    return false;

                BuiltInType builtinType = GetBuiltInType(datatypeId);
                return builtinType == BuiltInType.Boolean;
            }
        }

        public bool IsString
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.DataType, out value))
                    return false;
                NodeId datatypeId = value.Value as NodeId;
                if (datatypeId == null)
                    return false;

                BuiltInType builtinType = GetBuiltInType(datatypeId);
                return builtinType == BuiltInType.String;
            }
        }

        public bool IsOneDimension
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.ValueRank, out value))
                    return false;

                int? valueRank = value.Value as int?;
                return valueRank != null && valueRank == ValueRanks.OneDimension;
            }
        }

        public bool IsTwoDimensions
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.ValueRank, out value))
                    return false;

                int? valueRank = value.Value as int?;
                return valueRank != null && valueRank == ValueRanks.TwoDimensions;
            }
        }

        public uint[] ArrayDimension
        {
            get
            {
                DataValue value;
                if (NodeAttributes.TryGetValue(Attributes.ArrayDimensions, out value))
                    return value.Value as uint[];

                return null;
            }
        }

        public BuiltInType DataType
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.DataType, out value))
                    return BuiltInType.Null;
                NodeId datatypeId = value.Value as NodeId;
                if (datatypeId == null)
                    return BuiltInType.Null;

                return GetBuiltInType(datatypeId);
            }
        }

        public Session session
        {
            get
            {
                return sessionViewModel.Session;
            }
        }

        public SessionViewModel sessionViewModel { get; private set; }


        QualifiedName browseName;
        public QualifiedName BrowseName
        {
            get
            {
                if (browseName != null)
                    return browseName;

                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                if (node != null)
                {
                    browseName = node.BrowseName;
                    return browseName;
                }
                browseName = new QualifiedName(String.Empty);
                return browseName;
            }
        }

        public LocalizedText DisplayName
        {
            get
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                return node.DisplayName;
            }
        }

        public LocalizedText Description
        {
            get
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                return node.Description;
            }
        }

        public NodeClass NodeClass
        {
            get
            {
                EnsureNodeCache();
                Node node = session.NodeCache.Find(nodeId) as Node;
                return node.NodeClass;
            }
        }


        public ExpandedNodeId TypeDefinition
        {
            get
            {
                try
                { 
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(nodeId) as Node;
                    if (node == null)
                        return null;
                    return node.TypeDefinitionId;
                }
                catch { }

                return null;
            }
        }

        public ExpandedNodeId ParentedTypeDefinition
        {
            get
            {
                try
                {
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(ParentedTypeId) as Node;
                    if (node == null)
                        return null;
                    return node.TypeDefinitionId;
                }
                catch { }

                return null;
            }
        }

        public String ParentedTypeDefinitionName
        {
            get
            {
                try
                {
                    EnsureNodeCache();
                    Node node = session.NodeCache.Find(ParentedTypeId) as Node;
                    if (node == null)
                        return null;
                    node = session.NodeCache.Find(node.TypeDefinitionId) as Node;
                    if (node == null)
                        return null;
                    return node.DisplayName.ToString();
                }
                catch { }

                return null;
            }
        }

        public List<String> ParentedTypeDefinitionNameList
        {
            get
            {
                try
                { 
                    EnsureNodeCache();
                    var node = session.NodeCache.Find(ParentedTypeId) as Node;
                    if (node == null)
                        return null;
                    var list = new List<Node>();
                    while (node != null && node.TypeDefinitionId != ObjectIds.RootFolder && node.TypeDefinitionId != ObjectIds.ObjectsFolder)
                    {
                        var typenode = session.NodeCache.Find(node.TypeDefinitionId) as Node;
                        if (typenode == null)
                            break;
                        list.Add(typenode);

                        Node p = FindParent(node);
                        if (p == null)
                            break;
                        while (!IsParentType(p))
                        {
                            p = FindParent(p);
                            if (p == null)
                                break;
                        }
                        if (p == null)
                            break;
                        node = p;
                    }

                    var ret = new List<String>();
                    list.ForEach(n => { ret.Add(n.BrowseName.ToString()); });
                    return ret;
                }
                catch { }

                return null;
            }
        }

        public List<ExpandedNodeId> ParentedTypeDefinitionIdList
        {
            get
            {
                try
                {
                    EnsureNodeCache();
                    var node = session.NodeCache.Find(ParentedTypeId) as Node;
                    if (node == null)
                        return null;
                    var list = new List<ExpandedNodeId>();
                    while (node != null && node.TypeDefinitionId != ObjectIds.RootFolder && node.TypeDefinitionId != ObjectIds.ObjectsFolder)
                    {
                        list.Add(node.TypeDefinitionId);

                        Node p = FindParent(node);
                        if (p == null)
                            break;
                        while (!IsParentType(p))
                        {
                            p = FindParent(p);
                            if (p == null)
                                break;
                        }
                        if (p == null)
                            break;
                        node = p;
                    }

                    return list;
                }
                catch { }

                return null;
            }
        }

        #endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations

        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif
    }
}
