using System;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using System.Windows.Input;
using System.Collections.Generic;
using Utilities;

namespace OPCUAViewModel
{
    public class BrowserViewModel : TreeViewItemViewModel
    {
        #region Members
        public Browser browser { get; protected set; }

        static SafeObservableCollection<BrowserViewModel> listActiveBrowsers = new SafeObservableCollection<BrowserViewModel>();
        #endregion

        #region Constructor
        public BrowserViewModel(Browser b, TreeViewItemViewModel parent)
            : base(parent, true)
        {
            if (b == null)
                throw new ArgumentNullException("Browser");

            browser = b;
            rootId = Objects.ObjectsFolder;

            if (b.Session != null)
                Title = String.Format("{0} {1}", Properties.Resource.OPCUABrowser, b.Session.SessionName);
            else
                Title = String.Format("{0} Unknown Session", Properties.Resource.OPCUABrowser);

            browser.MoreReferences += browser_MoreReferences;

            lock (listActiveBrowsers)
            {
                if (!listActiveBrowsers.Contains(this))
                    listActiveBrowsers.Add(this);
            }
        }
        #endregion

        #region Commands
        RelayCommand _openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(
                        param => LoadChildren(),
                        param => CanOpen
                        );
                }
                return _openCommand;
            }
        }

        bool CanOpen
        {
            get
            {
                return browser != null && browser.Session != null;
            }
        }
        #endregion

        #region Events
        void browser_MoreReferences(Browser sender, BrowserEventArgs e)
        {
            //BrowseExtracted(e.References);
            //e.References.Clear();
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

            lock (listActiveBrowsers)
            {
                if (listActiveBrowsers.Contains(this))
                    listActiveBrowsers.Remove(this);
            }

            if (browser == null)
                return;

            browser.MoreReferences -= browser_MoreReferences;
            browser = null;
        }

        public List<ReferenceDescriptionViewModel> BrowseForTypeDefinition(ExpandedNodeId typdef)
        {
            List<ReferenceDescriptionViewModel> list = new List<ReferenceDescriptionViewModel>();
            List<ReferenceDescriptionViewModel> recorsivelist = new List<ReferenceDescriptionViewModel>();
            {
                var oldBrowseDirection = browser.BrowseDirection;
                var oldReferenceTypeId = browser.ReferenceTypeId;
                browser.BrowseDirection = Opc.Ua.BrowseDirection.Inverse;
                browser.ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition;
                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(typdef, browser.Session.NamespaceUris));
                List<ExpandedNodeId> listAddedNodes = new List<ExpandedNodeId>();
                foreach (ReferenceDescription refer in references)
                {
                    if (refer.NodeId == null || listAddedNodes.Contains(refer.NodeId) || refer.NodeId.IsAbsolute)
                        continue;
                    listAddedNodes.Add(refer.NodeId);

                    recorsivelist.Add(new ReferenceDescriptionViewModel(refer, browser, this, null));
                    if (refer.TypeDefinition == typdef)
                    {
                        ReferenceDescriptionViewModel newReferenceDescriptionViewModel = new ReferenceDescriptionViewModel(refer, browser, this, null);
                        list.Add(newReferenceDescriptionViewModel);
                    }
                }

                browser.BrowseDirection = oldBrowseDirection;
                browser.ReferenceTypeId = oldReferenceTypeId;
            }

            if (list.Count > 0)
                return list;

            try
            {
                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris));

                List<ExpandedNodeId> listAddedNodes = new List<ExpandedNodeId>();
                foreach (ReferenceDescription refer in references)
                {
                    if (refer.NodeId == null || listAddedNodes.Contains(refer.NodeId) || refer.NodeId.IsAbsolute)
                        continue;
                    listAddedNodes.Add(refer.NodeId);

                    recorsivelist.Add(new ReferenceDescriptionViewModel(refer, browser, this, null));
                    if (refer.TypeDefinition == typdef)
                    {
                        ReferenceDescriptionViewModel newReferenceDescriptionViewModel = new ReferenceDescriptionViewModel(refer, browser, this, null);
                        list.Add(newReferenceDescriptionViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);

                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
            }
            finally
            {
            }

            recorsivelist.ForEach(refer =>
            {
                List<ReferenceDescriptionViewModel> temp = refer.BrowseForTypeDefinition(typdef);
                list.AddRange(temp);
            });

            recorsivelist.Clear();

            return list;
        }

        private void BrowseExtracted(ReferenceDescriptionCollection references)
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

        protected override void LoadChildren()
        {
            if (rootId == null || rootId.ServerIndex > 0)
                return;
            try
            {
                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris));
                BrowseExtracted(references);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, Properties.Resource.OPCUABrowsingError);

                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingError, ex);
                throw;
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
                ReadValueId valueId = new ReadValueId { NodeId = ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris), AttributeId = attributeId, IndexRange = null, DataEncoding = null };
                nodesToRead.Add(valueId);
            }

            // read attributes.
            DataValueCollection values;
            DiagnosticInfoCollection diagnosticInfos;

            browser.Session.Read(
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
            using (var updater = new CollectionUpdater(properties))
            {
                properties.Clear();

                if (browser.Session.KeepAliveStopped)
                    return;

                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingProperties);

                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

                Browser b = new Browser(browser.Session);

                browser.BrowseDirection = BrowseDirection.Forward;
                browser.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                browser.IncludeSubtypes = true;
                browser.NodeClassMask = (int)NodeClass.Variable;
                browser.ContinueUntilDone = true;

                ReferenceDescriptionCollection references = browser.Browse(ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris));

                foreach (ReferenceDescription reference in references)
                {
                    ReadValueId valueId = new ReadValueId { NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, browser.Session.NamespaceUris), AttributeId = Attributes.Value, IndexRange = null, DataEncoding = null };

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
                    properties.Add(new ReferenceDescriptionViewModel(references[ii], browser, this, BrowseForTypeDef));

                    if (diagnosticInfos != null && diagnosticInfos.Count < ii)
                    {
                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUABrowsingReadingPropertiesError, diagnosticInfos[ii].InnerStatusCode);
                    }
                }
            }
        }

        private void AddReferences(SafeObservableCollection<TreeViewItemViewModel> references, 
                                   NodeId referenceTypeId, BrowseDirection browseDirection)
        {
            using (var updater = new CollectionUpdater(references))
            {
                if (browser.Session.KeepAliveStopped)
                    return;

                LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUABrowsingReadingReferences);

                // fetch the attributes for the reference type.
                INode referenceType = browser.Session.NodeCache.Find(ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris));

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

                ReferenceDescriptionCollection r = browser.Browse(ExpandedNodeId.ToNodeId(rootId, browser.Session.NamespaceUris));

                // add results to list.
                foreach (ReferenceDescription reference in r)
                {
                    references.Add(new ReferenceDescriptionViewModel(reference, browser, this, BrowseForTypeDef));
                }
            }
        }

        public static ReferenceDescriptionCollection Browse(
#if !NET_STANDARD
            Session session, 
#else
            ISession session,
#endif
            BrowseDescription nodeToBrowse, bool throwOnError)
        {
            try
            {
                ReferenceDescriptionCollection references = new ReferenceDescriptionCollection();

                // construct browse request.
                BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
                nodesToBrowse.Add(nodeToBrowse);

                // start the browse operation.
                BrowseResultCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                session.Browse(
                    null,
                    null,
                    0,
                    nodesToBrowse,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToBrowse);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToBrowse);

                do
                {
                    // check for error.
                    if (StatusCode.IsBad(results[0].StatusCode))
                    {
                        throw new ServiceResultException(results[0].StatusCode);
                    }

                    // process results.
                    for (int ii = 0; ii < results[0].References.Count; ii++)
                    {
                        references.Add(results[0].References[ii]);
                    }

                    // check if all references have been fetched.
                    if (results[0].References.Count == 0 || results[0].ContinuationPoint == null)
                    {
                        break;
                    }

                    // continue browse operation.
                    ByteStringCollection continuationPoints = new ByteStringCollection();
                    continuationPoints.Add(results[0].ContinuationPoint);

                    session.BrowseNext(
                        null,
                        false,
                        continuationPoints,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, continuationPoints);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, continuationPoints);
                }
                while (true);

                //return complete list.
                return references;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        public static ReferenceDescriptionCollection BrowseSuperTypes(
#if !NET_STANDARD
            Session session, 
#else
            ISession session,
#endif
            NodeId nodeId, bool throwOnError)
        {
            ReferenceDescriptionCollection supertypes = new ReferenceDescriptionCollection();

            try
            {
                // find all of the children of the field.
                BrowseDescription nodeToBrowse = new BrowseDescription
                {
                    NodeId = nodeId,
                    BrowseDirection = BrowseDirection.Inverse,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = false, // more efficient to use IncludeSubtypes=False when possible.
                    NodeClassMask = 0, // the HasSubtype reference already restricts the targets to Types. 
                    ResultMask = (uint)BrowseResultMask.All
                };

                ReferenceDescriptionCollection references = Browse(session, nodeToBrowse, throwOnError);

                while (references != null && references.Count > 0)
                {
                    // should never be more than one supertype.
                    supertypes.Add(references[0]);

                    // only follow references within this server.
                    if (references[0].NodeId.ServerIndex > 0)
                    {
                        break;
                    }

                    // get the references for the next level up.
                    nodeToBrowse.NodeId = ExpandedNodeId.ToNodeId(references[0].NodeId, session.NamespaceUris);
                    references = Browse(session, nodeToBrowse, throwOnError);
                }

                // return complete list.
                return supertypes;
            }
            catch (Exception exception)
            {
                if (throwOnError)
                {
                    throw new ServiceResultException(exception, StatusCodes.BadUnexpectedError);
                }

                return null;
            }
        }

        public static void CollectFields(
#if !NET_STANDARD
            Session session, 
#else
            ISession session,
#endif
            NodeId nodeId, SimpleAttributeOperandCollection eventFields)
        {
            CollectFields(session, nodeId, eventFields, new List<NodeId>());
        }

        public static void CollectFields(
#if !NET_STANDARD
            Session session, 
#else
            ISession session,
#endif
            NodeId nodeId, 
            SimpleAttributeOperandCollection eventFields,
            List<NodeId> foundTypeIds)
        {
            // get the supertypes.
            ReferenceDescriptionCollection supertypes = BrowseSuperTypes(session, nodeId, false);

            if (supertypes == null)
                return;

            // process the types starting from the top of the tree.
            Dictionary<NodeId, QualifiedNameCollection> foundNodes = new Dictionary<NodeId, QualifiedNameCollection>();
            QualifiedNameCollection parentPath = new QualifiedNameCollection();

            for (int ii = supertypes.Count - 1; ii >= 0; ii--)
            {
                NodeId targetId = ExpandedNodeId.ToNodeId(supertypes[ii].NodeId, session.NamespaceUris);

                if (!foundTypeIds.Contains(targetId))
                {
                    foundTypeIds.Add(targetId);
                    CollectFields(session, targetId, parentPath, eventFields, foundNodes);
                }
            }

            if (!foundTypeIds.Contains(nodeId))
            {
                foundTypeIds.Add(nodeId);
                // collect the fields for the selected type.
                CollectFields(session, nodeId, parentPath, eventFields, foundNodes);
            }
        }

        static private void CollectFields(
#if !NET_STANDARD
            Session session,
#else
            ISession session,
#endif
            NodeId nodeId,
            QualifiedNameCollection parentPath,
            SimpleAttributeOperandCollection eventFields,
            Dictionary<NodeId, QualifiedNameCollection> foundNodes)
        {
            // find all of the children of the field.
            BrowseDescription nodeToBrowse = new BrowseDescription
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.Aggregates,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
                ResultMask = (uint)BrowseResultMask.All
            };

            ReferenceDescriptionCollection children = Browse(session, nodeToBrowse, false);

            if (children == null)
            {
                return;
            }

            // process the children.
            for (int ii = 0; ii < children.Count; ii++)
            {
                ReferenceDescription child = children[ii];

                if (child.NodeId.ServerIndex > 0)
                {
                    continue;
                }

                // construct browse path.
                QualifiedNameCollection browsePath = new QualifiedNameCollection(parentPath);
                browsePath.Add(child.BrowseName);

                // check if the browse path is already in the list.
                if (!ContainsPath(eventFields, browsePath))
                {
                    SimpleAttributeOperand field = new SimpleAttributeOperand
                    {
                        TypeDefinitionId = ObjectTypeIds.BaseEventType,
                        BrowsePath = browsePath,
                        AttributeId = (child.NodeClass == NodeClass.Variable) ? Attributes.Value : Attributes.NodeId
                    };

                    eventFields.Add(field);
                }

                // recusively find all of the children.
                NodeId targetId = ExpandedNodeId.ToNodeId(child.NodeId, session.NamespaceUris);

                // need to guard against loops.
                if (!foundNodes.ContainsKey(targetId))
                {
                    foundNodes.Add(targetId, browsePath);
                    CollectFields(session, ExpandedNodeId.ToNodeId(child.NodeId, session.NamespaceUris), browsePath, eventFields, foundNodes);
                }
            }
        }

        private static bool ContainsPath(SimpleAttributeOperandCollection selectClause, QualifiedNameCollection browsePath)
        {
            for (int ii = 0; ii < selectClause.Count; ii++)
            {
                SimpleAttributeOperand field = selectClause[ii];

                if (field.BrowsePath.Count != browsePath.Count)
                {
                    continue;
                }

                bool match = true;

                for (int jj = 0; jj < field.BrowsePath.Count; jj++)
                {
                    if (field.BrowsePath[jj] == browsePath[jj])
                        continue;

                    match = false;
                    break;
                }

                if (match)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Properties
        public ExpandedNodeId rootId { get; set; }

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
                    ReadAttributes(_nodeAttributes);
                }

                return _nodeAttributes;
            }
        }

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

        public bool BrowseForObjects
        {
            get
            {
                return rootId == Objects.ObjectsFolder;
            }
            set
            {
                if (!value && rootId != Objects.ObjectsFolder || value && rootId == Objects.ObjectsFolder)
                    return;

                rootId = value ? Objects.ObjectsFolder : Objects.RootFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForObjectTypes
        {
            get
            {
                return rootId == Objects.ObjectTypesFolder;
            }
            set
            {
                if (!value && rootId != Objects.ObjectTypesFolder || value && rootId == Objects.ObjectTypesFolder)
                    return;

                rootId = value ? Objects.ObjectTypesFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForVariableTypes
        {
            get
            {
                return rootId == Objects.VariableTypesFolder;
            }
            set
            {
                if (!value && rootId != Objects.VariableTypesFolder || value && rootId == Objects.VariableTypesFolder)
                    return;

                rootId = value ? Objects.VariableTypesFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForDataTypes
        {
            get
            {
                return rootId == Objects.DataTypesFolder;
            }
            set
            {
                if (!value && rootId != Objects.DataTypesFolder || value && rootId == Objects.DataTypesFolder)
                    return;

                rootId = value ? Objects.DataTypesFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForEventTypes
        {
            get
            {
                return rootId == Objects.EventTypesFolder;
            }
            set
            {
                if (!value && rootId != Objects.EventTypesFolder || value && rootId == Objects.EventTypesFolder)
                    return;

                rootId = value ? Objects.EventTypesFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForTypes
        {
            get
            {
                return rootId == Objects.TypesFolder;
            }
            set
            {
                if (!value && rootId != Objects.TypesFolder || value && rootId == Objects.TypesFolder)
                    return;

                rootId = value ? Objects.TypesFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public bool BrowseForViews
        {
            get
            {
                return rootId == Objects.ViewsFolder;
            }
            set
            {
                if (!value && rootId != Objects.ViewsFolder || value && rootId == Objects.ViewsFolder)
                    return;

                rootId = value ? Objects.ViewsFolder : Objects.ObjectsFolder;
                OnPropertyChanged("BrowseForObjects");
                OnPropertyChanged("BrowseForTypes");
                OnPropertyChanged("BrowseForEventTypes");
                OnPropertyChanged("BrowseForViews");
                OnPropertyChanged("BrowseForObjectTypes");
                OnPropertyChanged("BrowseForDataTypes");
                OnPropertyChanged("BrowseForVariableTypes");
            }
        }

        public ViewDescription ViewDescription 
        { 
            get
            {
                if (browser != null)
                    return browser.View;
                return null;
            }

            set
            {
                if (browser != null)
                    browser.View = value;
            }
        }

        public bool ContinueUntilDone
        {
            get
            {
                return browser.ContinueUntilDone;
            }
            set
            {
                if (value == browser.ContinueUntilDone)
                    return;

                browser.ContinueUntilDone = value;
                OnPropertyChanged("ContinueUntilDone");
            }
        }

        public BrowseDirection BrowseDirection
        {
            get
            {
                return browser.BrowseDirection;
            }
            set
            {
                if (value == browser.BrowseDirection)
                    return;

                browser.BrowseDirection = value;
                OnPropertyChanged("BrowseDirection");
            }
        }

        public bool IncludeSubtypes
        {
            get
            {
                return browser.IncludeSubtypes;
            }
            set
            {
                if (value == browser.IncludeSubtypes)
                    return;

                browser.IncludeSubtypes = value;
                OnPropertyChanged("IncludeSubtypes");
            }
        }

        public uint MaxReferencesReturned
        {
            get
            {
                return browser.MaxReferencesReturned;
            }
            set
            {
                if (value == browser.MaxReferencesReturned)
                    return;

                browser.MaxReferencesReturned = value;
                OnPropertyChanged("MaxReferencesReturned");
            }
        }

        public int NodeClassMask
        {
            get
            {
                return browser.NodeClassMask;
            }
            set
            {
                if (value == browser.NodeClassMask)
                    return;

                browser.NodeClassMask = value;
                OnPropertyChanged("NodeClassMask");
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
