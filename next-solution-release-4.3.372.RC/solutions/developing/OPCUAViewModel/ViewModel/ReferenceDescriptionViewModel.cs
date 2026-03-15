using System;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using System.Windows.Input;
using Utilities;
using System.Collections.Generic;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
#endif
using System.Reflection;
using System.IO;
using System.Text;
using UFInterfaces;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Policy;

namespace OPCUAViewModel
{
    public class ReferenceDescriptionViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public ReferenceDescription referenceDescription { get; protected set; }
#endregion

#region Constructor
        public ReferenceDescriptionViewModel(ReferenceDescription rd, Browser b, TreeViewItemViewModel parent, ExpandedNodeId referType)
            : base(parent, true)
        {
            if (rd == null)
                throw new ArgumentNullException("ReferenceDescription");

            referenceDescription = rd;
            browser = b;
            Title = GetTargetText(referenceDescription);

            browser.MoreReferences += browser_MoreReferences;

            BrowseForTypeDef = referType;
        }
#endregion

#region Events
        void browser_MoreReferences(Browser sender, BrowserEventArgs e)
        {
            //BrowseExtracted(e.References);
            //e.References.Clear();
            
            if (CancelCurrentBrowsing)
            {
                e.Cancel = true;
            }
        }
#endregion

#region Methods
        protected override void OnDispose()
        {
            base.OnDispose();

            lock (lockObject)
            {
                if (_childrenVariables != null)
                {
                    foreach (var v in _childrenVariables)
                        v.Dispose();
                    _childrenVariables = null;
                }

                if (_nodeProperties != null)
                {
                    foreach (var v in _nodeProperties)
                        v.Dispose();
                    _nodeProperties = null;
                }

                if (_nodeReferences != null)
                {
                    foreach (var v in _nodeReferences)
                        v.Dispose();
                    _nodeReferences = null;
                }
            }

            if (browser != null)
                browser.MoreReferences -= browser_MoreReferences;
        }

        public static string GetTargetText(ReferenceDescription reference)
        {
            if (reference == null)
                return null;

            if (reference.DisplayName != null && !String.IsNullOrEmpty(reference.DisplayName.Text))
            {
                return reference.DisplayName.Text;
            }

            if (reference.BrowseName != null)
            {
                return reference.BrowseName.Name;
            }

            return null;
        }

        private Node FindParent(Node node)
        {
            IList<IReference> parents = node.ReferenceTable.Find(ReferenceTypeIds.Aggregates, true, true, browser.Session.TypeTree);
            //IList<IReference> parents = node.ReferenceTable.Find(ReferenceTypeIds.HasModelParent, false, false, browser.Session.TypeTree);
            if (parents.Count == 0)
                parents = node.ReferenceTable.Find(ReferenceTypeIds.Organizes, true, true, browser.Session.TypeTree);

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

                foreach (IReference parentReference in parents)
                {
                    Node parent = browser.Session.NodeCache.Find(parentReference.TargetId) as Node;
                    if (parent == null)
                        return null;

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

            return null;
        }

        private String GetParentPath()
        {
            RelativePath encodeBrowseName = new RelativePath(BrowseName); // we must use relative to encode special char like . etc
            if (Parent is ReferenceDescriptionViewModel)
            {
                ReferenceDescriptionViewModel parent = Parent as ReferenceDescriptionViewModel;
                //if (parent.NodeId != Objects.ObjectsFolder &&
                //    parent.NodeId != Objects.VariableTypesFolder &&
                //    parent.NodeId != Objects.RootFolder &&
                //    parent.NodeId != Objects.DataTypesFolder &&
                //    parent.NodeId != Objects.EventTypesFolder &&
                //    parent.NodeId != Objects.TypesFolder &&
                //    parent.NodeId != Objects.ViewsFolder)
                //{
                String parentPath = parent.GetParentPath();
                return String.IsNullOrEmpty(parentPath) ? 
                       String.Format("{0}", encodeBrowseName.Format(browser.Session.TypeTree)) : 
                       String.Format("{0}{1}", parentPath, encodeBrowseName.Format(browser.Session.TypeTree));
                //}
            }

            return String.Format("{0}", encodeBrowseName.Format(browser.Session.TypeTree));
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
                            INode datatype = browser.Session.NodeCache.Find(datatypeId);

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

                        if ((accessLevel & AccessLevels.CurrentRead) != 0)
                        {
                            bits.Append("Readable");
                        }

                        if ((accessLevel & AccessLevels.CurrentWrite) != 0)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("Writeable");
                        }

                        if ((accessLevel & AccessLevels.HistoryRead) != 0)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History");
                        }

                        if ((accessLevel & AccessLevels.HistoryWrite) != 0)
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

                        if ((notifier & EventNotifiers.SubscribeToEvents) != 0)
                        {
                            bits.Append("Subscribe");
                        }

                        if ((notifier & EventNotifiers.HistoryRead) != 0)
                        {
                            if (bits.Length > 0)
                            {
                                bits.Append(" | ");
                            }

                            bits.Append("History");
                        }

                        if ((notifier & EventNotifiers.HistoryWrite) != 0)
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
            NodeClass nodeClass = (NodeClass)referenceDescription.NodeClass;

            INode typeDefinition = browser.Session.NodeCache.Find(referenceDescription.TypeDefinition);

            switch (nodeClass)
            {
                case NodeClass.Object:
                    {
                        if (browser.Session.TypeTree.IsTypeOf(referenceDescription.TypeDefinition, ObjectTypes.FolderType))
                        {
                            return "Folder";
                        }

                        return "Object";
                    }

                case NodeClass.Variable:
                    {
                        if (browser.Session.TypeTree.IsTypeOf(referenceDescription.TypeDefinition, VariableTypes.PropertyType))
                        {
                            return "Property";
                        }

                        if (IsHistorizing && IsSubscribeToEvents)
                            return "VariableEH";
                        else if (IsHistorizing)
                            return "VariableH";
                        else if (IsSubscribeToEvents)
                            return "VariableE";
                        return "Variable";
                    }
            }

            return nodeClass.ToString();

        }

        private bool IsParentType(Node node)
        {
            NodeClass nodeClass = (NodeClass)node.NodeClass;

            // make sure the type definition is in the cache.
            INode typeDefinition = browser.Session.NodeCache.Find(node.TypeDefinitionId);

            switch (nodeClass)
            {
                case NodeClass.Object:
                    {
                        if (browser.Session.TypeTree.IsTypeOf(node.TypeDefinitionId, ObjectTypeIds.FolderType))
                            return false;
                        return true;
                    }

                case NodeClass.Variable:
                    {
                        if (browser.Session.TypeTree.IsTypeOf(node.TypeDefinitionId, VariableTypeIds.PropertyType))
                            return false;
                        return true;
                    }
            }

            return false;
        }


        public List<ReferenceDescriptionViewModel> BrowseForTypeDefinition(ExpandedNodeId typdef)
        {
            List<ReferenceDescriptionViewModel> list = new List<ReferenceDescriptionViewModel>();
            List<ReferenceDescriptionViewModel> recorsivelist = new List<ReferenceDescriptionViewModel>();

            try
            {
                Debug.WriteLine(String.Format("Browsing {0}", referenceDescription.DisplayName));


                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));

                List<ExpandedNodeId> listAddedNodes = new List<ExpandedNodeId>();
                // foreach (ReferenceDescription refer in references)
                Parallel.ForEach(references, refer =>
                    {
                        if (refer.NodeId != null && !listAddedNodes.Contains(refer.NodeId) && !refer.NodeId.IsAbsolute)
                        {
                            lock (listAddedNodes)
                            {
                                listAddedNodes.Add(refer.NodeId);

                                recorsivelist.Add(new ReferenceDescriptionViewModel(refer, browser, this, null));
                                if (refer.TypeDefinition == typdef)
                                {
                                    ReferenceDescriptionViewModel newReferenceDescriptionViewModel = new ReferenceDescriptionViewModel(refer, browser, this, null);
                                    list.Add(newReferenceDescriptionViewModel);
                                }
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
                Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);
            }
            finally
            {
            }

            // recorsivelist.ForEach(refer =>
            Parallel.ForEach(recorsivelist, refer =>
                {
                    List<ReferenceDescriptionViewModel> temp = refer.BrowseForTypeDefinition(typdef);
                    lock (list)
                    {
                        list.AddRange(temp);
                    }
                });

            recorsivelist.Clear();

            return list;
        }

        private void BrowseExtracted(ReferenceDescriptionCollection references)
        {
            using (var updater = new CollectionUpdater(Children))
            {
                List<ExpandedNodeId> listAddedNodes = new List<ExpandedNodeId>();
                foreach (ReferenceDescription refer in references)
                {
                    if (listAddedNodes.Contains(refer.NodeId))
                        continue;
                    listAddedNodes.Add(refer.NodeId);

                    if (BrowseForTypeDef != null &&
                        refer.TypeDefinition != BrowseForTypeDef &&
                        refer.TypeDefinition != ObjectTypeIds.FolderType &&
                        refer.TypeDefinition != ObjectTypeIds.BaseObjectType)
                        continue;

                    Children.Add(new ReferenceDescriptionViewModel(refer, browser, this, BrowseForTypeDef));
                    if ((refer.NodeClass & NodeClass.Variable) != 0)
                        ChildrenVariables.Add(new ReferenceDescriptionViewModel(refer, browser, this, BrowseForTypeDef));
                }
            }
        }

        public SafeObservableCollection<TreeViewItemViewModel> LoadParents()
        {
            SafeObservableCollection<TreeViewItemViewModel> ret = new SafeObservableCollection<TreeViewItemViewModel>();
            using (var updater = new CollectionUpdater(ret))
            {
                if (referenceDescription.NodeId == null || referenceDescription.NodeId.ServerIndex > 0)
                    return ret;
                CancelCurrentBrowsing = false;
                try
                {
                    browser.BrowseDirection = BrowseDirection.Inverse;
                    ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));

                    List<ExpandedNodeId> listAddedNodes = new List<ExpandedNodeId>();
                    foreach (ReferenceDescription refer in references)
                    {
                        if (listAddedNodes.Contains(refer.NodeId))
                            continue;
                        listAddedNodes.Add(refer.NodeId);

                        if (BrowseForTypeDef != null &&
                            refer.TypeDefinition != BrowseForTypeDef &&
                            refer.TypeDefinition != ObjectTypeIds.FolderType &&
                            refer.TypeDefinition != ObjectTypeIds.BaseObjectType)
                            continue;

                        ret.Add(new ReferenceDescriptionViewModel(refer, browser, this, BrowseForTypeDef));
                    }

                }
                catch (Exception ex)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
                    Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);
                }
                finally
                {
                    browser.BrowseDirection = BrowseDirection.Forward;
                }

                return ret;
            }
        }

        protected override void LoadChildren()
        {
            if (referenceDescription.NodeId == null || referenceDescription.NodeId.ServerIndex > 0)
                return;
            CancelCurrentBrowsing = false;
            try
            {
                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));
                BrowseExtracted(references);
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
                Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);
            }
        }

        private void ReadAttributes(Dictionary<uint, DataValue> attributes)
        {
            attributes.Clear();

            if (browser.Session.KeepAliveStopped)
                return;

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingAttributes);

            // build list of attributes to read.
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            foreach(var attributeId in Attributes.GetIdentifiers())
            {
                ReadValueId valueId = new ReadValueId 
                {
                    NodeId = ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris), 
                    AttributeId = attributeId, 
                    IndexRange = null, 
                    DataEncoding = null 
                };
                nodesToRead.Add(valueId);
            }

            // read attributes.
            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            browser.Session.Read(
                null,
                0, // MaxAge
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

                attributes.Add(nodesToRead[ii].AttributeId, values[ii]);

                // field.Name           = Attributes.GetBrowseName(nodesToRead[ii].AttributeId);
                if (diagnosticInfos != null && ii < diagnosticInfos.Count)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingReadingAttributesError, diagnosticInfos[ii].InnerStatusCode);
                }
            }
        }

        private void ReadProperties(SafeObservableCollection<TreeViewItemViewModel> properties)
        {
            properties.Clear();

            if (browser.Session.KeepAliveStopped)
                return;

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingProperties);

            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

            Browser b = new Browser(browser.Session) 
            { 
                BrowseDirection = BrowseDirection.Forward, 
                ReferenceTypeId = ReferenceTypeIds.HasProperty, 
                IncludeSubtypes = true, 
                NodeClassMask = (int)NodeClass.Variable, 
                ContinueUntilDone = true 
            };

            ReferenceDescriptionCollection references = b.Browse(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));

            foreach (ReferenceDescription reference in references)
            {
                ReadValueId valueId = new ReadValueId 
                {
                    NodeId = ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris), 
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

            browser.Session.Read(
                null,
                0, // MaxAge
                TimestampsToReturn.Neither,
                nodesToRead,
                out values,
                out diagnosticInfos);

            ClientBase.ValidateResponse(values, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            // update control.
            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                properties.Add(new ReferenceDescriptionViewModel(references[ii], browser, this, BrowseForTypeDef));

                if (diagnosticInfos != null && ii < diagnosticInfos.Count)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingReadingPropertiesError, diagnosticInfos[ii].InnerStatusCode);
                }
            }
        }

        private void AddReferences(SafeObservableCollection<TreeViewItemViewModel> references,
                                   NodeId referenceTypeId, BrowseDirection browseDirection)
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingReferences);

            // fetch the attributes for the reference type.
            INode referenceType = browser.Session.NodeCache.Find(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));

            if (referenceType == null)
                return;

            // browse for the references.
            Browser b = new Browser(browser.Session) 
            { 
                BrowseDirection = browseDirection, 
                ReferenceTypeId = referenceTypeId, 
                IncludeSubtypes = true, 
                NodeClassMask = 0, 
                ContinueUntilDone = true 
            };

            ReferenceDescriptionCollection r = b.Browse(ExpandedNodeId.ToNodeId(referenceDescription.NodeId, browser.Session.NamespaceUris));

            using (var updater = new CollectionUpdater(references))
            {
                // add results to list.
                foreach (ReferenceDescription reference in r)
                {
                    references.Add(new ReferenceDescriptionViewModel(reference, browser, this, BrowseForTypeDef));
                }
            }
        }

        private String RelativePathExtracted(bool bUseBrowseName)
        {
            Node node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

            if (node == null)
                return String.Empty;

            Node parent = FindParent(node);

            if (parent != null)
            {
                List<Node> parents = new List<Node>();
                parents.Add(parent);

                while (parent.NodeClass != NodeClass.ObjectType && parent.NodeClass != NodeClass.VariableType)
                {
                    parent = FindParent(parent);

                    if (parent == null)
                        break;

                    parents.Add(parent);
                }

                StringBuilder relativePath = new StringBuilder();

                if (bUseBrowseName)
                {
                    for (int ii = parents.Count - 1; ii >= 0; ii--)
                        relativePath.AppendFormat("/{0}", parents[ii].BrowseName);
                }
                else
                {
                    for (int ii = parents.Count - 1; ii >= 0; ii--)
                        relativePath.AppendFormat("/{0}", parents[ii].DisplayName);
                }

                return relativePath.ToString();
            }

            return String.Empty;
        }

        SessionViewModel GetSessionViewModelParent()
        {
            TreeViewItemViewModel parent = Parent;
            while (parent != null && !(parent is SessionViewModel))
                parent = parent.Parent;
            return parent as SessionViewModel;
        }

        public OPCUAEntityReference CreateEntityReference(ReferenceDescriptionViewModel StartingNode)
        {
            SessionViewModel model = GetSessionViewModelParent();
            if (model == null)
                throw new ArgumentException("Session not found");

            String hostName = model.HostName;

            //To avoid conflicts between tags with the same name, in the same project, running on different servers, hostName couldn't empty
            if (String.IsNullOrEmpty(hostName))
            {
                var uri = Utils.ParseUri(model.endPoint.EndpointUrl.ToString());
                if (uri != null)
                    hostName = uri.Host;
            }
                

            String appName = model.AppName;
            OPCUAEntityReference ret = null;
            if (StartingNode == null)
            {
                ret = new OPCUAEntityReference(hostName, appName, model.endPoint.EndpointUrl.ToString(),
                                                CompletePath, ExpandedNodeId.ToNodeId(NodeId, browser.Session.NamespaceUris), String.Format("{0} ({1})", OPCUAEntityReference.CompletePathToHumanReadable(CompletePath), appName),
                                                TypeDefinition, ReadablePath);
                // ret.UseSecurity = model.Session.Endpoint.SecurityMode != MessageSecurityMode.None;
            }
            else
            {
                int nIndex = CompletePath.IndexOf(StartingNode.CompletePath);
                if (nIndex < 0)
                    throw new ArgumentException("Relative Path could not be found");

                ret = new OPCUAEntityReference(hostName, appName, model.endPoint.EndpointUrl.ToString(),
                                                CompletePath.Substring(nIndex), StartingNode.CompletePath,
                                                ExpandedNodeId.ToNodeId(NodeId, browser.Session.NamespaceUris),
                                                ExpandedNodeId.ToNodeId(StartingNode.NodeId, browser.Session.NamespaceUris), String.Format("{0} ({1})", OPCUAEntityReference.CompletePathToHumanReadable(CompletePath), appName),
                                                TypeDefinition, TypeDefinitionString, ReadablePath.Substring(nIndex));
                // ret.UseSecurity = model.Session.Endpoint.SecurityMode != MessageSecurityMode.None;
            }

            ret.NodeIdViewModel = new NodeIdViewModel(NodeId, model);
            return ret;
        }

#endregion

#region Commands
        RelayCommand _browseCommand;
        public ICommand BrowseCommand
        {
            get
            {
                if (_browseCommand == null)
                {
                    _browseCommand = new RelayCommand(
                        param => LoadChildren(),
                        param => CanBrowse
                        );
                }
                return _browseCommand;
            }
        }

        bool CanBrowse
        {
            get { return referenceDescription != null &&
                         referenceDescription.NodeId != null && 
                         referenceDescription.NodeId.ServerIndex == 0; }
        }

#endregion

#region Properties

        NodeId parentId;
        public NodeId ParentId
        {
            get
            {
                if (parentId == null)
                {
                    Node node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

                    if (node != null)
                    {
                        Node parent = FindParent(node);
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
                if (parentedTypeId == null)
                {
                    Node node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

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

        public ExpandedNodeId ParentedTypeDefinition
        {
            get
            {
                Node node = browser.Session.NodeCache.Find(ParentedTypeId) as Node;
                if (node == null)
                    return null;
                return node.TypeDefinitionId;
            }
        }

        public String ParentedTypeDefinitionName
        {
            get
            {
                var node = browser.Session.NodeCache.Find(ParentedTypeId) as Node;
                if (node == null)
                    return null;
                node = browser.Session.NodeCache.Find(node.TypeDefinitionId) as Node;
                if (node == null)
                    return null;
                return node.DisplayName.ToString();
            }
        }

        public List<String> ParentedTypeDefinitionNameList
        {
            get
            {
                var node = browser.Session.NodeCache.Find(ParentedTypeId) as Node;
                if (node == null)
                    return null;
                var list = new List<Node>();
                while (node != null && node.TypeDefinitionId != ObjectIds.RootFolder && node.TypeDefinitionId != ObjectIds.ObjectsFolder)
                {
                    var typenode = browser.Session.NodeCache.Find(node.TypeDefinitionId) as Node;
                    if (typenode == null)
                        break;
                    list.Add(typenode);

                    Node parent = FindParent(node);
                    if (parent == null)
                        break;
                    while (!IsParentType(parent))
                    {
                        parent = FindParent(parent);
                        if (parent == null)
                            break;
                    }
                    if (parent == null)
                        break;
                    node = parent;
                }

                var ret = new List<String>();
                list.ForEach(n => { ret.Add(n.DisplayName.ToString()); });
                return ret;
            }
        }

        String readablePath;
        public String ReadablePath
        {
            get
            {
                if (String.IsNullOrEmpty(readablePath))
                {
                    var node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

                    if (node == null)
                        return String.Empty;

                    var parent = FindParent(node);

                    if (parent != null)
                    {
                        var parents = new List<Node>();
                        parents.Add(parent);

                        while (!IsParentType(parent))
                        {
                            parent = FindParent(parent);
                            if (parent == null)
                                break;
                            parents.Add(parent);
                        }

                        var readable = new StringBuilder();

                        for (int ii = parents.Count - 2; ii >= 0; ii--)
                        {
                            if (ii == parents.Count - 2)
                                readable.AppendFormat("{0}", parents[ii].DisplayName);
                            else
                                readable.AppendFormat("/{0}", parents[ii].DisplayName);
                        }
                        if (readable.Length > 0)
                            readable.AppendFormat("/{0}", node.BrowseName);
                        else
                            readable.AppendFormat("{0}", node.BrowseName);

                        readablePath = readable.ToString();
                    }
                    else
                    {
                        readablePath = string.Format("{0}", node.BrowseName);
                    }
                }

                return readablePath;
            }
        }

        String relativePath;
        public String RelativePath
        {
            get
            {
                if (String.IsNullOrEmpty(relativePath))
                {
                    var node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

                    if (node == null)
                        return String.Empty;

                    var parent = FindParent(node);

                    if (parent != null)
                    {
                        var parents = new List<Node>();
                        parents.Add(parent);

                        while (!IsParentType(parent))
                        {
                            parent = FindParent(parent);
                            if (parent == null)
                                break;
                            parents.Add(parent);
                        }

                        var relative = new StringBuilder();

                        for (int ii = parents.Count - 2; ii >= 0; ii--)
                        {
                            if (ii == parents.Count - 2)
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
                    
                    // relativePath = RelativePathExtracted(false);
                }

                return relativePath;
            }
        }

        String completePath;
        public String CompletePath
        {
            get
            {
                if (String.IsNullOrEmpty(completePath))
                {
                    var node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;

                    if (node == null)
                        return String.Empty;

                    /*
                    Node parent = FindParent(node);

                    if (parent != null)
                    {
                        List<Node> parents = new List<Node>();
                        parents.Add(parent);

                        while (parent != null)
                        {
                            parent = FindParent(parent);
                            if (parent != null)
                                parents.Add(parent);
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
                    */
                    completePath = GetParentPath();
                }

                return completePath;
            }
        }

        SafeObservableCollection<TreeViewItemViewModel> _childrenVariables;
        public SafeObservableCollection<TreeViewItemViewModel> ChildrenVariables
        {
            get
            {
                lock (lockObject)
                {
                    if (_childrenVariables == null)
                        _childrenVariables = new SafeObservableCollection<TreeViewItemViewModel>();
                }

                return _childrenVariables;
            }
        }

        SafeObservableCollection<TreeViewItemViewModel> _nodeProperties;
        public SafeObservableCollection<TreeViewItemViewModel> NodeProperties
        {
            get
            {
                lock (lockObject)
                {
                    if (_nodeProperties != null)
                        return _nodeProperties;

                    _nodeProperties = new SafeObservableCollection<TreeViewItemViewModel>();
                    try
                    {
                        ReadProperties(_nodeProperties);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                return _nodeProperties;
            }
        }

        SafeObservableCollection<TreeViewItemViewModel> _nodeReferences;
        public SafeObservableCollection<TreeViewItemViewModel> NodeReferences
        {
            get
            {
                lock (lockObject)
                {
                    if (_nodeReferences != null)
                        return _nodeReferences;

                    _nodeReferences = new SafeObservableCollection<TreeViewItemViewModel>();

                    AddReferences(_nodeReferences, ReferenceTypeIds.HasTypeDefinition, BrowseDirection.Forward);
                    AddReferences(_nodeReferences, ReferenceTypeIds.HasModellingRule, BrowseDirection.Forward);
                }

                return _nodeReferences;
            }
        }

        Dictionary<uint, DataValue> _nodeAttributes;
        public Dictionary<uint, DataValue> NodeAttributes
        {
            get
            {
                lock (lockObject)
                {
                    if (_nodeAttributes != null)
                        return _nodeAttributes;

                    _nodeAttributes = new Dictionary<uint, DataValue>();
                    try
                    {
                        ReadAttributes(_nodeAttributes);
                    }
                    catch (Exception ex)
                    {
                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
                        Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);
                    }
                }

                return _nodeAttributes;
            }
        }

        public class AttributeField
        {
            public String Name { get; set; }
            public String Value { get; set; }
            public String Status { get; set; }
        }

        public List<AttributeField> ReadableAttributesList
        {
            get
            {
                List<AttributeField> list = new List<AttributeField>();
                foreach (KeyValuePair<uint, DataValue> node in NodeAttributes)
                {
                    AttributeField af = new AttributeField()
                    {
                        Name = Attributes.GetBrowseName(node.Key),
                        Value = FormatAttributeValue(node.Key, node.Value.Value),
                        Status = String.Format("{0}", node.Value.StatusCode)
                    };
                    list.Add(af);
                }

                return list;
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

                return (accessLevel & AccessLevels.CurrentRead) != 0;
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

                return (accessLevel & AccessLevels.CurrentRead) != 0;
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

                return (accessLevel & AccessLevels.CurrentWrite) != 0;
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

                return (accessLevel & AccessLevels.CurrentWrite) != 0;
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
                return valueRank != null && (valueRank & ValueRanks.Scalar) != 0;
            }
        }

        public bool IsMethod
        {
            get
            {
                NodeClass nodeClass = (NodeClass)referenceDescription.NodeClass;

                // make sure the type definition is in the cache.
                INode typeDefinition = browser.Session.NodeCache.Find(referenceDescription.TypeDefinition);

                return nodeClass == NodeClass.Method;
            }
        }

        public bool IsEventNotifier
        {
            get
            {
                return IsSubscribeToEvents || IsEventHistoryRead || IsEventHistoryWrite;
            }
        }

        public bool IsSubscribeToEvents
        {
            get
            {
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.EventNotifier, out value))
                    return false;

                byte eventNotifier = Convert.ToByte(value.Value);
                return (eventNotifier & EventNotifiers.SubscribeToEvents) == EventNotifiers.SubscribeToEvents;
            }
        }

        public bool IsEventHistoryRead
        {
            get
            {
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
                return referenceDescription.NodeClass == NodeClass.ObjectType ||
                       referenceDescription.NodeClass == NodeClass.VariableType;
            }
        }

        public Browser browser { get; private set; }
        public bool CancelCurrentBrowsing { get; set; }

        public ExpandedNodeId _browseForTypeDef;
        public ExpandedNodeId BrowseForTypeDef
        {
            get
            {
                return _browseForTypeDef;
            }
            set
            {
                if (value == _browseForTypeDef)
                    return;

                _browseForTypeDef = value;
                OnPropertyChanged("BrowseForTypeDef");
            }
        }

        public QualifiedName BrowseName
        {
            get
            {
                return referenceDescription.BrowseName;
            }
            set
            {
                if (value == referenceDescription.BrowseName)
                    return;

                referenceDescription.BrowseName = value;
                OnPropertyChanged("BrowseName");
            }
        }

        public LocalizedText DisplayName
        {
            get
            {
                return referenceDescription.DisplayName;
            }
            set
            {
                if (value == referenceDescription.DisplayName)
                    return;

                referenceDescription.DisplayName = value;
                OnPropertyChanged("BrowseName");
            }
        }

        public bool IsForward
        {
            get
            {
                return referenceDescription.IsForward;
            }
            set
            {
                if (value == referenceDescription.IsForward)
                    return;

                referenceDescription.IsForward = value;
                OnPropertyChanged("IsForward");
            }
        }

        public NodeClass NodeClass
        {
            get
            {
                return referenceDescription.NodeClass;
            }
            set
            {
                if (value == referenceDescription.NodeClass)
                    return;

                referenceDescription.NodeClass = value;
                OnPropertyChanged("NodeClass");
            }
        }

        public ExpandedNodeId NodeId
        {
            get
            {
                return referenceDescription.NodeId;
                /*
                DataValue value;
                if (!NodeAttributes.TryGetValue(Attributes.NodeId, out value))
                    return null;
                return value.Value as NodeId;
                 * */
            }
            set
            {
                if (value == referenceDescription.NodeId)
                    return;

                referenceDescription.NodeId = value;
                OnPropertyChanged("NodeId");
            }
        }

        public NodeId ReferenceTypeId
        {
            get
            {
                return referenceDescription.ReferenceTypeId;
            }
            set
            {
                if (value == referenceDescription.ReferenceTypeId)
                    return;

                referenceDescription.ReferenceTypeId = value;
                OnPropertyChanged("ReferenceTypeId");
            }
        }

        public ExpandedNodeId TypeDefinition
        {
            get
            {
                // return referenceDescription.TypeDefinition;
                // return (NodeReferences[0] as ReferenceDescriptionViewModel).NodeId;
                Node node = browser.Session.NodeCache.Find(referenceDescription.NodeId) as Node;
                if (node == null)
                    return null;
                return node.TypeDefinitionId;
            }
            set
            {
                if (value == referenceDescription.TypeDefinition)
                    return;

                referenceDescription.TypeDefinition = value;
                OnPropertyChanged("TypeDefinition");
            }
        }

        public ExpandedNodeId TypeId
        {
            get
            {
                return referenceDescription.TypeId;
            }
        }

        public bool Unfiltered
        {
            get
            {
                return referenceDescription.Unfiltered;
            }
            set
            {
                if (value == referenceDescription.Unfiltered)
                    return;

                referenceDescription.Unfiltered = value;
                OnPropertyChanged("Unfiltered");
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

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/{1}.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location), GetTargetIcon());
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVM{GetTargetIcon()}", false);
                return bm;
#else
                return null;
#endif
            }
        }

        public Object ContainedObject
        {
            get
            {
                return referenceDescription;
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                return new OPCUAViewModel.UserControls.ReferenceDescriptionViewModel();
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource { get { return null; } }
        public ContextMenu contextMenu { get { return null; } }

        public object EntityParent
        {
            get { return Parent; }
        }

        public String TypeDefinitionString
        {
            get 
            {
                if (NodeReferences.Count == 0)
                    return null;

                return (NodeReferences[0] as ReferenceDescriptionViewModel).BrowseName.ToString(); 
            }
        }

#endregion
    }
}
