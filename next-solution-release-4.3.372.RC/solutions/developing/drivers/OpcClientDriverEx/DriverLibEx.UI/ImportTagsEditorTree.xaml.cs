using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using UFUAModel.Extensions;
using OPCUAViewModelService.ComponentService;
using Opc.Ua;
using Opc.Ua.Client;
using DevExpress.Xpf.Core;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;
using UFInterfaces.Editors;
using System.ComponentModel;
using Utilities.WPF;
using DriverCodeBaseEx;

namespace OpcClientDriver.UI
{
    struct Position
    {
        internal int Element;
        internal int Number;
        internal string Type;
    }

    struct AddressSpaceObj
    {
        public NodeId nodeId;
        public NodeId parentNodeId;
        public NodeClass nodeClass;
        public BuiltInType dataType;
        public ExpandedNodeId typeDefinition;
        public bool IECStruct;
        public OPCUAViewModel.OPCUAEntityReference eRef;
    }

    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {        
        private static List<string> enumList = new List<string>();
        ImportDataModelOpcClientDriver importDataModel;
        bool alreadyLoaded = false;        
        List<string> ProtopttipesList;
        bool errorOnImport;        
        Position position;
        public GetStationName readStationName;
        private string DataTypeName;
        BaseImportTree baseImportTree;
        string DriverName;
        public ImportTagsEditorTree()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    importDataModel = null;
                    return;
                }
                alreadyLoaded = true;

                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                Assembly a = Assembly.GetAssembly(this.GetType());
                DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 200
                });                
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 150
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "ArrayDim",
                    bindingName = "TagArrayDim",
                    colWidth = 90
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "EndPoint",
                    bindingName = "TagEndPoint",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Relative Path",
                    bindingName = "TagRelativePath",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "NodeId",
                    bindingName = "TagNodeId",
                    colWidth = 150
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "AppName",
                    bindingName = "TagAppName",
                    colWidth = 200
                });
                
                baseImportTree = new BaseImportTree(DriverName, lista[0],
                   (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleTitle(false);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                #region Server browse controls
                var stationList = baseImportTree.GetStationSettingsList();

                CmbStationBS.ItemsSource = (from s in stationList orderby s.Value.Name select s.Value.Name).ToList();
                CmbStationBS.SelectedIndex = 0;
                #endregion

                DataContext = this;
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return new ImportDataTreeItemControlOpcClientDriver(tag);
        }

        AddressSpaceObj getASObj(OPCUAViewModel.NodeIdViewModel nIVM, List<AddressSpaceObj> lASObj, NodeId parent = null)
        {
            AddressSpaceObj a = new AddressSpaceObj();
            a.nodeId = ExpandedNodeId.ToNodeId(nIVM.nodeId, nIVM.session.NamespaceUris);
            a.nodeClass = nIVM.NodeClass;
            a.dataType = nIVM.DataType;
            a.IECStruct = IsIECStructure(nIVM, nIVM.sessionViewModel);
            a.parentNodeId = parent;
            a.typeDefinition = nIVM.TypeDefinition;
            lASObj.Add(a);
            if (nIVM.IsObject || nIVM.DataType == BuiltInType.ExtensionObject)
            {
                BrowseDescription nodeToBrowse = new BrowseDescription
                {
                    NodeId = a.nodeId,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method),
                    ResultMask = (uint)BrowseResultMask.All
                };

                ReferenceDescriptionCollection references = OPCUAViewModel.BrowserViewModel.Browse(nIVM.session, nodeToBrowse, false);
                if (references != null)
                {
                    foreach (ReferenceDescription refer in references)
                    {
                        if (refer.ReferenceTypeId == ReferenceTypeIds.HasEventSource || refer.ReferenceTypeId == ReferenceTypeIds.HasNotifier)
                            continue;
                        var nodeIdViewModel = new OPCUAViewModel.NodeIdViewModel(refer.NodeId, nIVM.sessionViewModel);

                        getASObj(nodeIdViewModel, lASObj, a.nodeId);
                    }
                }
            }
            return a;
        }
        public void ImportSelectedTags()
        {
            string importfolder = ReadFolderName();
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string stationName = ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            List<ImportTag> taglist = new List<ImportTag>();
            List<string> ImportPrototypeName = new List<string>();
            List<ImportPrototype> protolist = new List<ImportPrototype>();
            errorOnImport = false;

            if (list.Count > 0 && TabFile.IsSelected)
            {
                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        foreach (var elemList in list)
                        {
                            progressBar.ImportNewTag(elemList.Name);
                            progressBar.ThrowIfCancellationRequested();

                            ImportData elem = elemList as ImportData;
                            if (elem != null)
                            {
                                bool isStructure = elem.Children.Count > 0;
                                ImportDataOpcClientDriver single = elem as ImportDataOpcClientDriver;
                                OpcClientDriverDynTagSettings sp = new OpcClientDriverDynTagSettings();

                                sp.AppName = single.AppName;
                                sp.EndpointUrl = single.Endpointurl;
                                sp.ItemName = single.Name;
                                sp.RelativePath = single.RelativePath;
                                sp.ResolvedNodeId = single.NodeId;
                                sp.StationName = stationName;
                                single.DynAddress = sp.ToString();

                                ImportTag tagtoimport = new ImportTag()
                                {
                                    Name = single.Name,
                                    DataType = (DataType)single.TagTypeInt,
                                    DynSettings = single.DynAddress,
                                    Folder = importfolder,
                                    ModelType = UFUAModel.ModelType.Variable,
                                    Description = single.Description,
                                    BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                    BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                };
                                if (single.ArrayDimension > 0)
                                    tagtoimport.ArrayDimension = (uint)single.ArrayDimension;

                                taglist.Add(tagtoimport);
                            }
                        }

                        DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }
            else if (TabBrowser.IsSelected && browserOpcUa.Content is FrameworkElement)
            {
                // Set the number of tags that will be imported
                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        ProtopttipesList = new List<string>();
                        var editor = browserOpcUa.Content as FrameworkElement;
                        var popupBrowser = editor as ISelectEntityReference;
                        if (popupBrowser.SelectedReference is OPCUAViewModel.OPCUAEntityReferenceList)
                        {
                            var opcuaEntityReferenceList = popupBrowser.SelectedReference as OPCUAViewModel.OPCUAEntityReferenceList;
                            var datatypelist = editor.DataContext as OPCUAViewModel.TypeDeclaration;

                            OPCUAViewModel.OPCUAEntityReferenceList defList = new OPCUAViewModel.OPCUAEntityReferenceList();
                            List<AddressSpaceObj> lASObj = new List<AddressSpaceObj>();
                            foreach (var eR in opcuaEntityReferenceList)
                            {
                                try
                                {

                                    var o = getASObj(eR.NodeIdViewModel, lASObj);
                                    //use list to build tag and structures
                                    var lsIEC = (from p in lASObj where p.IECStruct && p.parentNodeId != null select p).ToList();
                                    if (lsIEC.Count > 0)
                                    {
                                        //all the item in the list, as single selected  entity reference..
                                        var l = (from i in lASObj select i.eRef).ToList();
                                        defList.AddRange(l);
                                    }
                                    else
                                        defList.Add(eR);
                                    lASObj.Clear();
                                }
                                catch (Exception ex)
                                {
                                    errorOnImport = true;
                                }
                            }

                            foreach (var entityReference in defList)
                            {
                                progressBar.ImportNewTag(entityReference.HumanReadable);
                                progressBar.ThrowIfCancellationRequested();

                                if (entityReference.NodeIdViewModel == null ||
                                    entityReference.NodeIdViewModel.TypeDefinition == ObjectTypeIds.FolderType)
                                    continue;

                                string prototypeName = null; 
                                prototypeName = PrototypeName(entityReference, taglist, protolist, false, importfolder);

                                if (prototypeName == string.Empty)
                                {
                                    prototypeName = PrototypeName(entityReference, taglist, protolist, true, importfolder);
                                    //if (prototypeName == string.Empty)
                                    //    continue;
                                }
                                if (prototypeName == "ImportDone")
                                    break;
                                // Create and prepare tag to import
                                var tag = PrepareTag(entityReference.NodeIdViewModel, prototypeName, relativePath: entityReference.RelativePath);
                                if (tag == null)
                                    continue;

                                tag.BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags;
                                tag.BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink;

                                OpcClientDriverDynTagSettings sp = null;
                                if (entityReference.AppName != string.Empty)
                                {
                                    // Create and assign dynamic settings
                                    sp = new OpcClientDriverDynTagSettings()
                                    {
                                        AppName = entityReference.AppName,
                                        EndpointUrl = entityReference.EndpointUrl,
                                        ItemName = entityReference.HumanReadableNoProject,
                                        RelativePath = entityReference.RelativePath,
                                        ResolvedNodeId = entityReference.ResolvedNodeId,
                                        StationName = stationName
                                    };
                                }
                                else
                                {
                                    StringTable applicationNameTable = entityReference.NodeIdViewModel.session.ServerUris;
                                    string appName = string.Empty;
                                    if (applicationNameTable.Count > 0)
                                    {
                                        var endPointWithAppName = applicationNameTable.GetString(0);
                                        var indexPos = endPointWithAppName.LastIndexOf(":") + 1;
                                        appName = endPointWithAppName.Substring(indexPos);
                                    }
                                    sp = new OpcClientDriverDynTagSettings()
                                    {
                                        AppName = appName,
                                        EndpointUrl = entityReference.EndpointUrl,
                                        ItemName = entityReference.HumanReadableNoProject,
                                        RelativePath = entityReference.RelativePath,
                                        ResolvedNodeId = entityReference.ResolvedNodeId,
                                        StationName = stationName
                                    };
                                }
                                tag.DynSettings = sp.ToString();
                                if (tag.DynSettings.Contains(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator().ToString())) ;
                                    tag.DynSettings = tag.DynSettings.Replace(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator().ToString(), "");
                                if (importfolder != stationName)
                                {
                                    tag.Folder = importfolder;
                                }
                                else
                                {
                                    if (tag.Folder != null)
                                        tag.Folder = String.Format("{0}/{1}", stationName, tag.Folder);
                                    else
                                        tag.Folder = stationName;
                                }
                                taglist.Add(tag);
                            
                                // Create and prepare prototypes to import
                                var prototypes = PreparePrototypes(entityReference.NodeIdViewModel, membername: tag.Name);
                                if (prototypes != null)
                                { 
                                    for (int i = 0; i< prototypes.Count; i++)
                                    {
                                        if (prototypes[i].Elements.Count > 0)
                                        {
                                            foreach (var t in prototypes)
                                            {
                                                if (ImportPrototypeName.Contains(t.Name))
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    protolist.Add(t);
                                                    ImportPrototypeName.Add(t.Name);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var att = entityReference.NodeIdViewModel.GetReadableAttributesList();
                                                DataTypeName = (from q in entityReference.NodeIdViewModel.GetReadableAttributesList()
                                                                where q.attributeId == Attributes.DataType
                                                                select q.Value).FirstOrDefault();
                                                prototypeName = DataTypeName;
                                                tag.PrototypeModel = prototypeName;
                                                var DataTypeFormStruct = PreparePrototypesFromStructDatatype(entityReference.NodeIdViewModel, entityReference.NodeIdViewModel.sessionViewModel, prototypeName);

                                                if (DataTypeFormStruct != null)
                                                {
                                                    if (entityReference.NodeIdViewModel != null && entityReference.NodeIdViewModel.ArrayDimension != null && entityReference.NodeIdViewModel.ArrayDimension.Count() > 0)
                                                    {
                                                        MessageBox.Show(string.Format(Properties.Resources.WarningIEC61131_3ArrayOfStructsNotSupported, entityReference.NodeIdViewModel.DisplayName), Properties.Resources.ImportMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                                                        taglist.Remove(tag);
                                                    }
                                                    else
                                                    {     
                                                        sp.Parse(tag.DynSettings);
                                                        sp.Struct_IEC61131_3 = true;
                                                        tag.DynSettings = sp.ToString();
                                                        var pl = (from p in protolist where p.Name == prototypeName select p).ToList();
                                                        if (pl.Count == 0)
                                                        {
                                                            protolist.AddRange(DataTypeFormStruct);
                                                        }
                                                    }
                                                }

                                            }
                                            catch(Exception ex)
                                            {
                                                CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidExceptioPrototypeImport, DriverName, tag.NodeId.Identifier),System.Diagnostics.EventLogEntryType.Error);
                                                errorOnImport = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    

                        foreach(var p in protolist)
                        {
                            foreach (var e in p.Elements)
                            {
                                if (!string.IsNullOrEmpty(e.PrototypeModel))
                                    if (e.PrototypeModel.Contains("\\"))
                                        e.PrototypeModel = UFUAModel.Helpers.NameValidator.EnsureValidName(e.PrototypeModel);
                            }
                        }
                        DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
                        ProtopttipesList.Clear();
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }                    
            if (errorOnImport)
            {
                MessageBox.Show(Properties.Resources.ErrorOnImporting, Properties.Resources.ImportMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        bool IsIECStructure(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, OPCUAViewModel.SessionViewModel session)
        {
            bool bRet = false;
            if (nodeIdViewModel.IsObject || nodeIdViewModel.DataType == BuiltInType.ExtensionObject)
            {
                // browse for the references.
                BrowseDescription nodeToBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.BrowseName
                };
                ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
                if (typeReferences == null || typeReferences.Count == 0)
                    bRet = true;
            }
            return bRet;
        }
        List<ImportPrototype> PreparePrototypesFromStructDatatype(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, OPCUAViewModel.SessionViewModel session, string prototypeName, string parentFolder = null)
        {
            if (!nodeIdViewModel.IsObject && nodeIdViewModel.DataType != BuiltInType.ExtensionObject)
                return null;
            
            var m_encodings = session.Session.ReadAvailableEncodings((NodeId)nodeIdViewModel.nodeId);
            ReferenceDescription encodings = m_encodings[0];
            ReferenceDescription description = session.Session.FindDataDescription((NodeId)encodings.NodeId);
            DataDictionary datadictionary = session.Session.FindDataDictionary((NodeId)description.NodeId);           

            var schema = datadictionary.GetSchema((NodeId)description.NodeId);
            if (schema == null)
            {
                CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidFindPrototypeImport, DriverName, (NodeId)description.NodeId.Identifier.ToString()), System.Diagnostics.EventLogEntryType.Error);
                errorOnImport = true;
                return null;
            }

            var nodeId = (NodeId)description.NodeId;
            var prototypes = new List<ImportPrototype>();
            var prototype = PreparePrototypeFromStructDatatType(nodeIdViewModel, datadictionary, schema, prototypeName, nodeId);
            
            if (prototype != null)
            {
                prototypes.Add(prototype);
                for (int k = 0; k < prototypes.Count; k++)
                {
                    foreach (var member in prototypes[k].Elements)
                    {
                        if (member.ModelType != ModelType.ObjectType)
                            continue;
                        using (var memberViewModel = new OPCUAViewModel.NodeIdViewModel(member.NodeId, nodeIdViewModel.sessionViewModel))
                        {
                            var tempnodeId = nodeId.ToString();
                            int indexs = 0;
                            if(nodeId.IdType == IdType.String)
                                indexs = tempnodeId.LastIndexOf("s=") + 2;
                            else if(nodeId.IdType == IdType.Guid)
                                indexs = tempnodeId.LastIndexOf("g=") + 2;
                            var NodeId = tempnodeId.Substring(0, indexs) + member.PrototypeModel;
                            var xmlString = datadictionary.GetSchema(NodeId);
                            if (xmlString == null)
                            {
                                CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidFindPrototypeImport, DriverName, (NodeId)description.NodeId.Identifier.ToString()), System.Diagnostics.EventLogEntryType.Error);
                                errorOnImport = true;
                                return null;
                            }
                            var subprototypes = PreparePrototypesFromStructDatatype(memberViewModel, datadictionary, xmlString, member.PrototypeModel, NodeId);
                            if (subprototypes != null)
                            {
                                prototypes.AddRange(subprototypes);
                            }
                        }
                    }
                }
            }
            return prototypes;
        }
        List<ImportPrototype> PreparePrototypesFromStructDatatype(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, DataDictionary dataDictionary, string schema, string prototypeName, NodeId nodeId, string parentFolder = null)
        {
            var prototypes = new List<ImportPrototype>();
            var prototype = PreparePrototypeFromStructDatatType(nodeIdViewModel, dataDictionary, schema, prototypeName, nodeId);
            prototypes.Add(prototype);

            return prototypes;
        }
        ImportPrototype PreparePrototypeFromStructDatatType(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, DataDictionary datadictionary, string schema, string prototypeName, NodeId nodeId)
        {
            var prototype = new ImportPrototype();
            prototype.Name = prototypeName;
            prototype.Elements = new List<ImportTag>();
            var members = PrepareMembersFromStructDataType(nodeIdViewModel, datadictionary, schema, nodeId, prototypeName);
            prototype.Elements.AddRange(members);
            
            return prototype;
        }
        internal bool CheckEnumType(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, string dataTypeName)
        {
            BrowseDescription nodeBrowse = new BrowseDescription
            {
                NodeId = ExpandedNodeId.ToNodeId(Objects.DataTypesFolder, nodeIdViewModel.sessionViewModel.NamespaceUris),
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ResultMask = (uint)BrowseResultMask.DisplayName
            };
            ReferenceDescriptionCollection enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);

            if (enumeratedRefence.Count > 0)
            {
                nodeBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[0].NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.DisplayName
                };
                enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                nodeBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[5].NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.BrowseName
                };

                enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                foreach (var r in enumeratedRefence)
                {
                    if (r.BrowseName.Name == dataTypeName)
                    {
                        nodeBrowse = new BrowseDescription
                        {
                            NodeId = ExpandedNodeId.ToNodeId(r.NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                            BrowseDirection = BrowseDirection.Forward,
                            ReferenceTypeId = ReferenceTypeIds.HasProperty,
                            IncludeSubtypes = true,
                            NodeClassMask = 0,
                            ResultMask = (uint)BrowseResultMask.DisplayName
                        };
                        enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                        if (enumeratedRefence != null && enumeratedRefence.Count > 0)
                        {
                            enumList.Add(dataTypeName);
                            return true;
                        }
                    }
                }

            }
            return false;
        }
        internal Position ByteCount(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, List<ImportTag> members, byte[] actualReadValue)
        {
            string dataType = string.Empty;
            foreach (ImportTag t in members)
            {
                dataType = t.DataType.ToString();
                switch (dataType)
                {
                    case "SByte":
                    case "Boolean":
                    case "Char":
                    case "Byte":
                        if(t.ArrayDimension == 0)
                        {
                            position.Element += 1;
                            position.Number = 1;
                        }
                        else
                        {
                            position.Element += (int)(1 * t.ArrayDimension) + 4;
                            position.Number = (int)(1 * t.ArrayDimension);
                        }
                        break;
                    case "WChar":
                    case "UInt16":
                    case "Int16":
                        if (t.ArrayDimension == 0)
                        {
                            position.Element += 2;
                            position.Number = 2;
                        }
                        else
                        {
                            position.Element += (int)(2 * t.ArrayDimension) + 4;
                            position.Number = (int)(2 * t.ArrayDimension);
                        }
                        break;
                    case "Float":
                    case "Int32":
                    case "UInt32":
                        if (t.ArrayDimension == 0)
                        {
                            position.Element += 4;
                            position.Number = 4;
                        }
                        else
                        {
                            position.Element += (int)(4 * t.ArrayDimension) + 4;
                            position.Number = (int)(4 * t.ArrayDimension);
                        }
                        break;
                    case "Int64":
                    case "Double":
                    case "Date":
                    case "Time_Of_Day":
                    case "DateTime":
                    case "UInt64":
                        if (t.ArrayDimension == 0)
                        {
                            position.Element += 8;
                            position.Number = 8;
                        }
                        else
                        {
                            position.Element += (int)(8 * t.ArrayDimension) + 4;
                            position.Number = (int)(8 * t.ArrayDimension);
                        }
                        break;
                    case "CharArray":
                    case "String":
                        int lenght = (5 + (int)actualReadValue[position.Element]);
                        position.Number = lenght;
                        position.Element += lenght - 1;
                        break;
                    default:
                        if (enumList.Contains(dataType) || CheckEnumType(nodeIdViewModel, dataType))
                        {
                            if (t.ArrayDimension == 0)
                            {
                                position.Element += 4;
                                position.Number = 4;
                            }
                            else
                            {
                                position.Element += (int)(4 * t.ArrayDimension) + 4;
                                position.Number = (int)(4 * t.ArrayDimension);
                            }
                            break;
                        }
                        CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidDataTypeImporter, DriverName, t.NodeId.Identifier),System.Diagnostics.EventLogEntryType.Error);
                        errorOnImport = true;
                        break;
                }

            }
            position.Type = dataType;
            return position;
        }

        List<ImportTag> PrepareMembersFromStructDataType(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, DataDictionary datadictionary, string substring, NodeId nodeId, string prototypeName)
        {
            ImportTag newTag = new ImportTag();
            int index = -1;
            int dataTypeNameIndex = -1;
            string dataTypeName = string.Empty;
            string tagName = string.Empty;
            string nameOfTag = string.Empty;
            string isArray = "LengthField";
            int tagNameIndex = 0;
            var members = new List<ImportTag>();
            bool checkSingleStructure = false;
            UInt32 arrayDimension = 0;
            ImportTag tag = new ImportTag((NodeId)nodeIdViewModel.nodeId);
            if (prototypeName.Contains("\\"))
                prototypeName = prototypeName.Replace("\\", "_x005C_");
            while (tagNameIndex != -1)
            {
                if(substring.Contains(prototypeName))
                {
                    index = substring.IndexOf("<StructuredType Name=" + "\"" + prototypeName);
                    if(index != -1)
                        substring = substring.Substring(index);
                    checkSingleStructure = true;
                }

                //if (checkSingleStructure)
                //    if (substring.IndexOf("TypeName=\"q") < substring.IndexOf("/"))
                //    {
                //        substring = substring.Substring(substring.IndexOf("Field Name=") + 12);
                //    }
                tagNameIndex = substring.IndexOf("Field Name=") + 12;
                if (tagNameIndex < 12)
                {
                    tagNameIndex = -1;
                    continue;
                }
                substring = substring.Substring(tagNameIndex);
                index = substring.IndexOf("\"");
                tagName = substring.Substring(0, index);
                substring = substring.Substring(index);
                if (checkSingleStructure)
                {
                    if (substring.IndexOf("TypeName=\\\"")< substring.IndexOf("TypeName=\"q") && substring.IndexOf("TypeName=\"q") > substring.IndexOf("/>"))
                        dataTypeNameIndex = substring.IndexOf("TypeName=\\\"") + 13;
                    else
                        dataTypeNameIndex = substring.IndexOf("TypeName=\"q") + 13;
                }
                else
                    dataTypeNameIndex = substring.IndexOf("TypeName=\"q") + 13;
                substring = substring.Substring(dataTypeNameIndex);
                index = substring.IndexOf("\"");
                dataTypeName = substring.Substring(0, index);
                if (substring.Substring(index + 2, 11) == isArray)
                {
                    substring = substring.Substring(index + 15);
                    index = substring.IndexOf("\"");
                    nameOfTag = substring.Substring(0, index);
                    var attribute = nodeIdViewModel.NodeAttributes;
                    ExtensionObject actualValue = (ExtensionObject)attribute[13].Value;
                    byte[] actBody = (byte[])actualValue.Body;
                    position.Element = 0;
                    position.Number = 0;
                    var element = ByteCount(nodeIdViewModel,members,actBody);
                    byte[] valueToRead = new byte[element.Number]; ;
                    for (int i = 0; i < element.Number; i++)
                    {

                        valueToRead[i] = actBody[element.Element - element.Number + i];
                    }
                    arrayDimension = BitConverter.ToUInt32(valueToRead, 0);
                }   
                if (dataTypeName == ("DateTime"))
                    continue;
                newTag = PrepareTagFromStructDataType(nodeIdViewModel, tagName, dataTypeName, datadictionary, nodeId, arrayDimension);

                if (arrayDimension != 0)
                    members.RemoveAt(members.Count - 1);
                arrayDimension = 0;
                if (newTag != null)
                    members.Add(newTag);
                if(checkSingleStructure)
                {
                    if (substring.IndexOf("</StructuredType") < substring.IndexOf("<Field Name"))
                        break;
                }
            }
            return members;
        }

        ImportTag PrepareTagFromStructDataType(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, string TagName, string DataTypeName, DataDictionary datadictionary, NodeId nodeId, UInt32 arrayDimension = 0)
        {
            ImportTag tag = new ImportTag(nodeId);
            DataValue enumStrings = null;
            switch (DataTypeName)
            {
                case "Boolean":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Boolean;
                    if(arrayDimension!=0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Byte":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Byte;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Double":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Double;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Float":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Float;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Int16":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Int16;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Int32":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Int32;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "Int64":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.Int64;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "UInt16":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.UInt16;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "UInt32":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.UInt32;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "UInt64":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.UInt64;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "SByte":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.SByte;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "CharArray":
                case "String":
                    tag.ModelType = ModelType.Analog;
                    tag.DataType = DataType.String;
                    if (arrayDimension != 0)
                        tag.ArrayDimension = arrayDimension;
                    tag.Name = TagName;
                    break;
                case "DateTime":
                case "Date":
                case "Time":
                case "Time_Of_Day":
                    CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidDataTypeImporter, DriverName, tag.NodeId.Identifier), System.Diagnostics.EventLogEntryType.Error);
                    errorOnImport = true;
                    break;
                default:
                    try
                    {
                        enumStrings = importEnumType(nodeIdViewModel, true, DataTypeName);
                        LocalizedText[] tempLocText = null;
                        string[] tempStringArray = null;
                        if (enumStrings != null)
                        {
                            if ((LocalizedText[])enumStrings.Value != null)
                            {
                                tag.DataType = DataType.UInt32;
                                tag.ModelType = UFUAModel.ModelType.Enumerated;
                                tempLocText = (LocalizedText[])enumStrings.Value;
                                tempStringArray = new string[tempLocText.Length];
                                for (int i = 0; i < tempLocText.Length; i++)
                                    tempStringArray[i] = tempLocText[i].Text;
                                tag.EnumsString = tempStringArray;
                                enumStrings = null;
                            }
                            else
                            {
                                tag.ModelType = ModelType.Analog;
                                tag.DataType = DataType.UInt32;
                            }

                        }
                    }
                    catch(Exception Ex)
                    {
                        tag.ModelType = ModelType.Analog;
                        tag.DataType = DataType.UInt32;
                    }
                    break;
            }
            if (tag.Name == null && DataTypeName != "DateTime" && nodeId != null && !string.IsNullOrEmpty(DataTypeName))
            {
                var tempnodeId = nodeId.ToString();
                int indexs = 0;
                if (nodeId.IdType == IdType.String)
                    indexs = tempnodeId.LastIndexOf("s=") + 2;
                else if (nodeId.IdType == IdType.String)
                    indexs = tempnodeId.LastIndexOf("g=") + 2;
                if(DataTypeName.Contains("_x005C_"))
                    DataTypeName = DataTypeName.Replace("_x005C_","\\");
                if (DataTypeName.StartsWith(":"))
                    DataTypeName = DataTypeName.Substring(1);
                var NodeId = tempnodeId.Substring(0, indexs) + DataTypeName;
                var xmlString = datadictionary.GetSchema(NodeId);
                if(!string.IsNullOrEmpty(xmlString))
                {
                    if (tag.ModelType != UFUAModel.ModelType.Enumerated)
                        tag.ModelType = ModelType.ObjectType;
                    tag.PrototypeModel = DataTypeName;
                }
                tag.Name = TagName;
            }
            return tag;
        }

        private string PrototypeName(OPCUAViewModel.OPCUAEntityReference entityReference, List<ImportTag> taglist, List<ImportPrototype> protolist, bool ForceReading, string importfolder)
            {
            string stationName = ReadStationName();
            string prototypeName;
            string name = string.Empty;
            if (entityReference.NodeIdViewModel.IsObject || ForceReading)
            {
                // browse for the references.
                BrowseDescription nodeToBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(entityReference.NodeIdViewModel.nodeId, entityReference.NodeIdViewModel.session.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.BrowseName
                };
                ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(entityReference.NodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
                if (typeReferences == null || typeReferences.Count == 0 || ForceReading)
                {
                    BrowseDescription Browse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(entityReference.NodeIdViewModel.nodeId, entityReference.NodeIdViewModel.session.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasComponent,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.BrowseName
                    };
                    ReferenceDescriptionCollection typeReference = OPCUAViewModel.BrowserViewModel.Browse(entityReference.NodeIdViewModel.sessionViewModel.Session, Browse, false);
                    if (typeReference == null || typeReference.Count == 0)
                        return string.Empty;
                    OPCUAViewModel.NodeIdViewModel[] Node = new OPCUAViewModel.NodeIdViewModel[typeReference.Count];
                    ImportTag[] tags = new ImportTag[typeReference.Count];
                    OpcClientDriverDynTagSettings[] sps = new OpcClientDriverDynTagSettings[typeReference.Count];
                    for (int i = 0; i < typeReference.Count; i++)
                    {
                        Node[i] = new OPCUAViewModel.NodeIdViewModel(typeReference[i].NodeId, entityReference.NodeIdViewModel.sessionViewModel);
                        if (entityReference.Name != null)
                            name = entityReference.Name;
                        else
                            name = entityReference.ReadablePath;
                        tags[i] = PrepareTag(Node[i], null, name);
                        if (tags[i] == null)
                            continue;
                        sps[i] = new OpcClientDriverDynTagSettings()
                        {
                            AppName = entityReference.AppName,
                            EndpointUrl = entityReference.EndpointUrl,
                            ItemName = (entityReference.HumanReadableNoProject + "\\" + tags[i].Name),
                            RelativePath = (entityReference.RelativePath + "/" + entityReference.NodeIdViewModel.nodeId.NamespaceIndex + ":" + tags[i].Name),
                            ResolvedNodeId = (entityReference.ResolvedNodeId + "?" + tags[i].Name),
                            StationName = stationName
                        };
                        tags[i].DynSettings = sps[i].ToString();
                        if (importfolder != stationName)
                            tags[i].Folder = importfolder;
                        else
                            tags[i].Folder = String.Format("{0}/{1}", stationName, entityReference.NodeIdViewModel.BrowseName.Name);
                        taglist.Add(tags[i]);
                    }
                    DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
                    return prototypeName = "ImportDone";
                };
                prototypeName = UFUAModel.Helpers.NameValidator.EnsureValidName(typeReferences[0].BrowseName.Name);
                return prototypeName;
            }
            else if (entityReference.NodeIdViewModel.IsVariable)
            {
                BrowseDescription variableToBrowse = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(entityReference.NodeIdViewModel.nodeId, entityReference.NodeIdViewModel.session.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.DisplayName
                };
                ReferenceDescriptionCollection typeReference = OPCUAViewModel.BrowserViewModel.Browse(entityReference.NodeIdViewModel.sessionViewModel.Session, variableToBrowse, false);
                BrowseDescription variableToBrowse2 = new BrowseDescription
                {
                    NodeId = ExpandedNodeId.ToNodeId(entityReference.NodeIdViewModel.nodeId, entityReference.NodeIdViewModel.session.NamespaceUris),
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.References,
                    IncludeSubtypes = true,
                    NodeClassMask = 0,
                    ResultMask = (uint)BrowseResultMask.BrowseName
                };
                ReferenceDescriptionCollection typeReference2 = OPCUAViewModel.BrowserViewModel.Browse(entityReference.NodeIdViewModel.sessionViewModel.Session, variableToBrowse2, false);
                if (typeReference == null || typeReference.Count == 0)
                    return string.Empty;
                return prototypeName = UFUAModel.Helpers.NameValidator.EnsureValidName(typeReference[0].BrowseName.Name);
            }
            else
                return string.Empty;
        }

        List<ImportPrototype> PreparePrototypes(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, string parentFolder = null, string membername = null)
        {
            if (!nodeIdViewModel.IsObject && nodeIdViewModel.DataType != BuiltInType.ExtensionObject)
                return null;

            // browse for the references.
            BrowseDescription nodeToBrowse = new BrowseDescription
            {
                NodeId = ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris),
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                IncludeSubtypes = true,
                NodeClassMask = 0,
                ResultMask = (uint)BrowseResultMask.BrowseName
            };
            ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
            if (typeReferences == null || typeReferences.Count == 0)
                return null;

            var prototypes = new List<ImportPrototype>();
            var prototype = PreparePrototype(nodeIdViewModel, typeReferences[0].BrowseName.Name, parentFolder, membername);
            if (prototype != null)
            {
                prototypes.Add(prototype);
                foreach (var member in prototype.Elements)
                {
                    if (member.ModelType != ModelType.ObjectType)
                        continue;

                    using (var memberViewModel = new OPCUAViewModel.NodeIdViewModel(member.NodeId, nodeIdViewModel.sessionViewModel))
                    {
                        //parentFolder = String.Format("{0}/", member.Name);
                        if (!String.IsNullOrEmpty(member.Folder))
                            parentFolder = String.Format("{0}/{1}/", member.Folder, member.Name);
                        var subprototypes = PreparePrototypes(memberViewModel, parentFolder, member.Name);
                        if (subprototypes != null)
                            prototypes.AddRange(subprototypes);
                    }
                }
            }
            
            return prototypes;
        }

        ImportPrototype PreparePrototype(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, String prototypeName, String parentFolder = null, string membername = null)
        {
            if (!nodeIdViewModel.IsObject && nodeIdViewModel.DataType != BuiltInType.ExtensionObject)
                return null;

            var prototype = new ImportPrototype();
            prototype.Name = prototypeName;
            prototype.Elements = new List<ImportTag>();

            parentFolder = String.Empty; // structure/prototype folder always start from root (empty)
            var members = PrepareMembers(nodeIdViewModel.sessionViewModel, ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris), parentFolder, membername);
            prototype.Elements.AddRange(members);

            return prototype;
        }

        List<ImportTag> PrepareMembers(OPCUAViewModel.SessionViewModel sessionViewModel, NodeId nodeId, string parentFolder, string membername = null)
        {
            var members = new List<ImportTag>();
            //var browser = sessionViewModel.CreateBrowser();
            // find all of the children of the field.
            BrowseDescription nodeToBrowse = new BrowseDescription
            {
                NodeId = nodeId,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method),
                ResultMask = (uint)BrowseResultMask.All
            };

            ReferenceDescriptionCollection references = OPCUAViewModel.BrowserViewModel.Browse(sessionViewModel.Session, nodeToBrowse, false);
            if (references != null)
            {
                foreach (ReferenceDescription refer in references)
                {
                    if (refer.ReferenceTypeId != ReferenceTypeIds.HasEventSource && refer.ReferenceTypeId != ReferenceTypeIds.HasNotifier)
                    {
                        if (refer.NodeId == null || refer.NodeId.IsAbsolute)
                            continue;

                        if (refer.TypeDefinition == ObjectTypeIds.FolderType)
                        {
                            //parentFolder += string.Format("\\{0}:{1}", refer.NodeId.NamespaceIndex, refer.BrowseName.Name);
                            string thisFolder = string.Format("{0}\\{1}", parentFolder, refer.BrowseName.Name);                            
                            members.AddRange(PrepareMembers(sessionViewModel, ExpandedNodeId.ToNodeId(refer.NodeId, sessionViewModel.NamespaceUris), thisFolder));
                            continue;                            
                        }

                        using (var nodeIdViewModel = new OPCUAViewModel.NodeIdViewModel(refer.NodeId, sessionViewModel))
                        {
                            string prototypeModel = null;
                            if (nodeIdViewModel.IsObject /*&& nodeIdViewModel.DataType != BuiltInType.ExtensionObject*/)
                            {
                                nodeToBrowse = new BrowseDescription
                                {
                                    NodeId = ExpandedNodeId.ToNodeId(refer.NodeId, sessionViewModel.NamespaceUris),
                                    BrowseDirection = BrowseDirection.Forward,
                                    ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                                    IncludeSubtypes = true,
                                    NodeClassMask = 0,
                                    ResultMask = (uint)BrowseResultMask.BrowseName
                                };

                                ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(sessionViewModel.Session, nodeToBrowse, false);
                                if (typeReferences == null || typeReferences.Count == 0)
                                    continue;

                                prototypeModel = typeReferences[0].BrowseName.Name;
                            }

                        //if (nodeIdViewModel.IsObject && nodeIdViewModel.DataType != BuiltInType.ExtensionObject)
                        //{
                        //    errorOnImport = true;
                        //    continue;
                        //}
                        var tag = PrepareTag(nodeIdViewModel, prototypeModel, parentFolder, membername);
                        if (tag != null) 
                            members.Add(tag);
                        }
                    }
                    else
                        continue;
                }
            }
            else
            {
                nodeToBrowse = new BrowseDescription
                {
                    NodeId = nodeId,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HasDescription,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method),
                    ResultMask = (uint)BrowseResultMask.All
                };

                references = OPCUAViewModel.BrowserViewModel.Browse(sessionViewModel.Session, nodeToBrowse, false);
            }
            return members;
        }

        DataValue importEnumType(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, bool isMemberOfAStructure, string dataTypeName = null)
        {
            ReferenceDescriptionCollection enumeratedRefence = null;
            DataValue enumStrings = new DataValue();
            BrowseDescription nodeBrowse = null;
            Dictionary<uint, DataValue> attribute = null;
            KeyValuePair<uint, DataValue> eightAttribute;
            Node readNode = null;
            try
            {
                if (!isMemberOfAStructure)
                {
                    attribute = nodeIdViewModel.NodeAttributes;
                    if (attribute.Count >= 8)
                    {
                        eightAttribute = attribute.ElementAt(8);
                        readNode = nodeIdViewModel.session.ReadNode((NodeId)eightAttribute.Value.Value);
                        nodeBrowse = new BrowseDescription
                        {
                            NodeId = ExpandedNodeId.ToNodeId(readNode.NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                            BrowseDirection = BrowseDirection.Forward,
                            ReferenceTypeId = ReferenceTypeIds.HasProperty,
                            IncludeSubtypes = true,
                            NodeClassMask = 0,
                            ResultMask = (uint)BrowseResultMask.DisplayName
                        };

                        enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                        if (enumeratedRefence != null && enumeratedRefence.Count > 0)
                            enumStrings = nodeIdViewModel.sessionViewModel.Session.ReadValue((NodeId)enumeratedRefence[0].NodeId);
                        else
                            return null;
                    }
                }
                else
                {
                    nodeBrowse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(Objects.DataTypesFolder, nodeIdViewModel.sessionViewModel.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.Organizes,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.DisplayName
                    };

                    enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                    nodeBrowse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[0].NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.DisplayName
                    };

                    enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                    nodeBrowse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(enumeratedRefence[5].NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.BrowseName
                    };

                    enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                    foreach (var r in enumeratedRefence)
                    {
                        if (r.BrowseName.Name/*DisplayName*/ == dataTypeName)
                        {
                            nodeBrowse = new BrowseDescription
                            {
                                NodeId = ExpandedNodeId.ToNodeId(r.NodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                                BrowseDirection = BrowseDirection.Forward,
                                ReferenceTypeId = ReferenceTypeIds.HasProperty,
                                IncludeSubtypes = true,
                                NodeClassMask = 0,
                                ResultMask = (uint)BrowseResultMask.DisplayName
                            };

                            enumeratedRefence = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeBrowse, false);
                            if (enumeratedRefence != null && enumeratedRefence.Count > 0)
                                enumStrings = nodeIdViewModel.sessionViewModel.Session.ReadValue((NodeId)enumeratedRefence[0].NodeId);
                            else return null;
                        }
                    }
                }
            }
            catch(Exception Ex)
            {
                enumStrings = null;
            }
            return enumStrings;
        }

        ImportTag PrepareTag(OPCUAViewModel.NodeIdViewModel nodeIdViewModel, string prototypeModel = null, string parentFolder = null, string membername = null, string relativePath = null)
        {

            try
            {
                var DataTypeTag = nodeIdViewModel.DataType;
                var nodeId = ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris);
                ImportTag tag = new ImportTag(nodeId);
                DataValue enumStrings = null;
                var tagName = nodeIdViewModel.BrowseName.Name;

                int id;
                if (int.TryParse(UFUAModel.Helpers.NameValidator.RemoveInvalidCharacters(tagName), out id))
                {
                    if (!string.IsNullOrWhiteSpace(relativePath))
                    {
                        string[] nameSplit = relativePath.Split(new string[] { $"/{nodeIdViewModel.BrowseName.NamespaceIndex}:" }, StringSplitOptions.None);
                        if (nameSplit.Length > 1)
                            tagName = String.Format($"{nameSplit[nameSplit.Length - 2]}_{nameSplit[nameSplit.Length - 1]}");
                    }
                }
                //tag.Name = nodeIdViewModel.DisplayName.Text;
                // add to tag name station name
                tag.Name = AddStationName(tagName);

                if (nodeIdViewModel.DataType != Opc.Ua.BuiltInType.Null && nodeIdViewModel.DataType != BuiltInType.ExtensionObject && nodeIdViewModel.DataType != BuiltInType.Variant)
                {
                    if (nodeIdViewModel.DataType != BuiltInType.DateTime && nodeIdViewModel.DataType != BuiltInType.LocalizedText)
                        tag.DataType = nodeIdViewModel.DataType.ToDataType();
                    else if (nodeIdViewModel.DataType == BuiltInType.DateTime)
                    {
                        CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidDataTypeImporter, DriverName, nodeId.Identifier), System.Diagnostics.EventLogEntryType.Error);
                        errorOnImport = true;
                        return null;
                    }
                    else if (nodeIdViewModel.DataType == BuiltInType.LocalizedText)
                    {
                        CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidDataTypeImporter, DriverName, nodeId.Identifier), System.Diagnostics.EventLogEntryType.Error);
                        errorOnImport = true;
                        return null;
                    }
                }
                tag.Description = nodeIdViewModel.Description != null ? nodeIdViewModel.Description.Text : null;
                if (nodeIdViewModel.IsOneDimension)
                {
                    var arrayDimension = nodeIdViewModel.ArrayDimension;
                    if (arrayDimension != null)
                        tag.ArrayDimension = nodeIdViewModel.ArrayDimension[0];
                }
                if (nodeIdViewModel.DataType == BuiltInType.Int32)
                    enumStrings = importEnumType(nodeIdViewModel, false);
                
                
                if(enumStrings!= null &&
                    !(enumStrings.Value is LocalizedText[])) 
                {
                    //type not allowed
                    CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidEnumTagImport, DriverName, nodeId.Identifier), System.Diagnostics.EventLogEntryType.Error);
                    errorOnImport = true;
                    return null;
                }

                if (nodeIdViewModel.IsDiscreteItemType && !nodeIdViewModel.IsTwoStateDiscreteType || enumStrings!= null)
                {
                    tag.ModelType = UFUAModel.ModelType.Enumerated;
                    var nodeToBrowse = new BrowseDescription
                    {
                        NodeId = ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.sessionViewModel.NamespaceUris),
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasProperty,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ResultMask = (uint)BrowseResultMask.DisplayName
                    };

                    ReferenceDescriptionCollection typeReferences = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, nodeToBrowse, false);
                    DataValue value = null;
                    LocalizedText[] tempLocText = null;
                    string[] tempStringArray = null;
                    if (typeReferences != null && typeReferences.Count != 0)
                    {
                        for (int j=0; j< typeReferences.Count; j++)
                        {
                            value = nodeIdViewModel.sessionViewModel.Session.ReadValue((NodeId)typeReferences[j].NodeId);
                            if((value.Value as LocalizedText[]) != null)
                            {
                                tempLocText = (LocalizedText[])value.Value;
                                tempStringArray = new string[tempLocText.Length];
                                for (int i = 0; i < tempLocText.Length; i++)
                                    tempStringArray[i] = tempLocText[i].Text;
                                tag.EnumsString = tempStringArray;
                                break;
                            }
                            else if ((value.Value as string[]) != null)
                            {
                                tempStringArray = (string[])value.Value;
                                tag.EnumsString = tempStringArray;
                                break;
                            }
                        }
                    }
                    else if (enumStrings!= null)
                    {
                        tempLocText = (LocalizedText[])enumStrings.Value;
                        tempStringArray = new string[tempLocText.Length];
                        for (int i = 0; i < tempLocText.Length; i++)
                        {
                            tempStringArray[i] = tempLocText[i].Text;
                        }
                            tag.EnumsString = tempStringArray;
                    }
                }
                else if (nodeIdViewModel.IsTwoStateDiscreteType && !nodeIdViewModel.IsDiscreteItemType)
                {
                    tag.ModelType = UFUAModel.ModelType.Digital;
                }
                else if (nodeIdViewModel.IsMethod)
                {
                    tag.ModelType = UFUAModel.ModelType.Method;
                }
                else if (nodeIdViewModel.IsObject || nodeIdViewModel.DataType == BuiltInType.ExtensionObject)
                {
                    if (tag.ArrayDimension == 0)// if to be removed if you want to implement the array of prototype
                    {
                        tag.ModelType = UFUAModel.ModelType.ObjectType;
                        if (prototypeModel != null)
                            tag.PrototypeModel = prototypeModel;
                        else
                        {
                            BrowseDescription variableToBrowse = new BrowseDescription
                            {
                                NodeId = ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris),
                                BrowseDirection = BrowseDirection.Forward,
                                ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition,
                                IncludeSubtypes = true,
                                NodeClassMask = 0,
                                ResultMask = (uint)BrowseResultMask.BrowseName
                            };
                            ReferenceDescriptionCollection typeReference = OPCUAViewModel.BrowserViewModel.Browse(nodeIdViewModel.sessionViewModel.Session, variableToBrowse, false);
                            tag.PrototypeModel = typeReference[0].BrowseName.Name;
                        }
                    }
                    else
                    {
                        CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidTagImport, DriverName , nodeId.Identifier), System.Diagnostics.EventLogEntryType.Error);
                        errorOnImport = true;
                        return null;
                    }


                }
                else if (nodeIdViewModel.IsAnalogType)
                    tag.ModelType = UFUAModel.ModelType.Analog;
                else
                    tag.ModelType = UFUAModel.ModelType.Variable;
                string rp = nodeIdViewModel.RelativePath;
                if (!String.IsNullOrEmpty(rp))
                {
                    string ph = string.Format("{0}:",nodeIdViewModel.BrowseName.NamespaceIndex);
                    rp = rp.Replace(ph, "");
                    //var ns = nodeIdViewModel.DisplayName.Text;
                    // add to tag name station name
                    var ns = AddStationName(nodeIdViewModel.BrowseName.Name);
                    var index = rp.LastIndexOf(ns);
                    string folder = String.Empty;
                    if(index > 0)
                    {
                        folder = rp.Substring(0, index);
                    }
                    else
                    {
                        folder = rp;
                    }
                    if (membername != null)
                    {
                        int n = folder.IndexOf(membername);
                        if (n != -1)
                            folder = folder.Substring(n + membername.Length);
                    }
                    System.Diagnostics.Debug.WriteLine("-- DEBUG -- PrepareTag 1 folder {0} data {1} name {2} proto {3}", folder, tag.DataType, tag.Name, tag.PrototypeModel);
                    index = folder.IndexOf('/');
                    if (index == 0)
                        folder = folder.Substring(1);

                    index = folder.LastIndexOf('/');
                    bool delete = false;
                    if ((!string.IsNullOrEmpty(prototypeModel) || nodeIdViewModel.DataType != BuiltInType.ExtensionObject || nodeIdViewModel.DataType != BuiltInType.LocalizedText || nodeIdViewModel.DataType != BuiltInType.Variant || nodeIdViewModel.DataType != BuiltInType.DateTime) && (tag.NodeId.ToString().Contains(".") || (tag.ModelType == UFUAModel.ModelType.ObjectType)))
                    {
                        //Filter the path to delete the paths of the previous structures
                        string folderTmp = folder;
                        for (int i = 0; i < ProtopttipesList.Count; i++)
                        {
                            var indexTmp = folderTmp.IndexOf(ProtopttipesList[i]);
                            if (indexTmp == -1 && nodeIdViewModel.IsVariable)
                            {
                                var indexTemp = folder.LastIndexOf("/");
                                if (indexTemp != -1)
                                {
                                    string folderTmpLimited = folder.Substring(0, indexTemp);
                                    indexTemp = folderTmpLimited.LastIndexOf("/");
                                    if (indexTemp != -1)
                                    {
                                        for (int j = 0; j < ProtopttipesList.Count; j++)
                                        {
                                            int lastIndex = ProtopttipesList[j].Length - 1;
                                            var path = ProtopttipesList[j].Substring(0, lastIndex);
                                            var IndexPrototype = path.LastIndexOf("/") + 1;
                                            int lenght = path.Length - IndexPrototype;
                                            string NameOfLastLevelProt = string.Empty;
                                            if (lenght != 0  || !(tag.ModelType == UFUAModel.ModelType.ObjectType) || tag.Folder == null)
                                                NameOfLastLevelProt = path.Substring(IndexPrototype, lenght);
                                            else
                                            {
                                                NameOfLastLevelProt = path;
                                               break;
                                            }
                                            int firstIndex = folderTmpLimited.Length - NameOfLastLevelProt.Length;
                                            lenght = folderTmpLimited.Length - firstIndex;
                                            string NameOfLastLevelTag = folderTmpLimited.Substring(firstIndex, lenght);
                                            if (!string.IsNullOrEmpty(NameOfLastLevelProt) && (NameOfLastLevelProt == NameOfLastLevelTag || folderTmpLimited.Contains(NameOfLastLevelProt)))
                                            {
                                                delete = true;
                                                index = 0;
                                                folderTmp = NameOfLastLevelProt;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (indexTmp != -1)
                                folderTmp = folderTmp.Remove(indexTmp, ProtopttipesList[i].Length);
                        }

                        //Check and isolate if the path contains a real folder 
                        var index1 = folderTmp.LastIndexOf('/');
                        string realFolder = string.Empty;
                        index = -1;
                        if ((index1 > 0) && (folderTmp.Length > index1))
                        {
                            realFolder = folderTmp.Substring(0, index1);
                            index = index1;
                        }
                        //Populate the map with the paths attributed to the structures
                        if (!ProtopttipesList.Contains(folderTmp + "/") && !delete && tag.ModelType == UFUAModel.ModelType.ObjectType)
                            ProtopttipesList.Add(folderTmp + "/");

                        folder = realFolder;

                    }

                    if (index > 0)
                        folder = folder.Substring(0, index);
                    else
                        folder = null;

                    if (folder != null && parentFolder != null)
                    {
                        index = folder.IndexOf(parentFolder);
                        if (index == 0)
                            folder = folder.Substring(parentFolder.Length);

                        //Control the folder if contain the struct folder, if true, the struct folders are removed the in the path
                        string folderTmp = folder + "/";
                        for (int i = 0; i < ProtopttipesList.Count; i++)
                        {
                            var indexTmp = folderTmp.IndexOf(ProtopttipesList[i]);
                            if (indexTmp != -1)
                                folderTmp = folderTmp.Remove(indexTmp, ProtopttipesList[i].Length);
                        }

                        //Control the last character if it is a slash because it would add a folder to the tree
                        if (folderTmp.Length > 0)
                        {
                            if (folderTmp.Substring(folderTmp.Length - 1) == "/")
                                folderTmp = folderTmp.Remove(folderTmp.Length - 1);
                        }
                        folder = folderTmp;
                    }
                    tag.Folder = folder;
                }

                if (string.IsNullOrEmpty(tag.Folder))
                    tag.Folder = parentFolder;
                return tag;
            }
            catch (Exception ex)
            {
                CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidExceptioPrototypeImport, DriverName, ExpandedNodeId.ToNodeId(nodeIdViewModel.nodeId, nodeIdViewModel.session.NamespaceUris).Identifier), System.Diagnostics.EventLogEntryType.Error);
                errorOnImport = true;
                return null;
            }
        }

        private ImportDataModel LoadFile(string file)
        {
            string stationName = ReadStationName();

            importDataModel = new ImportDataModelOpcClientDriver(readStationName);

            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                if(!line.StartsWith("//"))
                                {
                                    row = line.Split(',');
                                    parsedData.Add(row);
                                }
                            }
                        }
                        bool refresh = false;
                        /*
                         * File structure: name, endpointurl, relativepath, nodeid, appname
                         */

                        OpcClientDriverDynTagSettings p = new OpcClientDriverDynTagSettings();
                        string dynamicaddress;
                        string name;
                        string type;
                        string endpointurl;
                        string relativepath;
                        string nodeid;
                        string appname;
                        int MovType;
                        uint varsize;
                        string arrDim;
                        foreach (var s in parsedData)
                        {
                            MovType = -1;
                            varsize = 0;
                            dynamicaddress = string.Empty;
                            name = string.Empty;
                            type = string.Empty;
                            endpointurl = string.Empty;
                            relativepath = string.Empty;
                            nodeid = string.Empty;
                            appname = string.Empty;
                            arrDim = string.Empty;

                            if (s.Length >=5)
                            {
                                p.StationName = stationName;
                                name = s[0];
                                type = s[1];
                                arrDim = s[2];
                                endpointurl = s[3];
                                relativepath = s[4];
                                if (s.Length > 5)
                                    nodeid = s[5];
                                if (s.Length > 6)
                                    appname = s[6];

                                MovType = GetMoviconTypeId(type, ref varsize);
                                if (MovType != -1)
                                {
                                    refresh = true;
                                    var IVar = importDataModel.addImportData();
                                    IVar.Name = name;
                                    ((ImportDataOpcClientDriver)IVar).Endpointurl = endpointurl;
                                    ((ImportDataOpcClientDriver)IVar).NodeId = nodeid;
                                    ((ImportDataOpcClientDriver)IVar).AppName = appname;
                                    ((ImportDataOpcClientDriver)IVar).TagTypeInt = MovType;
                                    IVar.Select = false;
                                    ((ImportDataOpcClientDriver)IVar).RelativePath = relativepath;
                                    IVar.ArrayDimension = 0;
                                    if(!string.IsNullOrEmpty(arrDim))
                                    {
                                        try
                                        {
                                            IVar.ArrayDimension = Convert.ToUInt32(arrDim);
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    IVar.szType = (IVar.ArrayDimension != 0 ? string.Format("{0}[{1}]", type, IVar.ArrayDimension) : type);

                                    AddTreeItem(IVar);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);                    
                    return null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }

            return importDataModel;
        }

        public int GetMoviconTypeId(string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            if (Type.Contains("BOOLEAN"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("SBYTE"))
            {
                nType = (int)DataType.SByte;
                VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                nType = (int)DataType.Byte;
                VarSize = 1;
            }
            else if (Type.Contains("UINT16"))
            {
                nType = (int)DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("INT16"))
            {
                nType = (int)DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("UINT32"))
            {
                nType = (int)DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("INT32"))
            {
                nType = (int)DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("UINT64"))
            {
                nType = (int)DataType.UInt64;
                VarSize = 8;
            }
            else if (Type.Contains("INT64"))
            {
                nType = (int)DataType.Int64;
                VarSize = 8;
            }
            else if (Type.Contains("FLOAT"))
            {
                nType = (int)DataType.Float;
                VarSize = 4;
            }
            else if (Type.Contains("DOUBLE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Contains("STRING"))
            {
                nType = (int)DataType.String;
                VarSize = 1;
            }
            return (nType);

        }
  
        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = -1;               
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = parent.Id;                
                parent.Children.Add(tag);
            }
        }

        /// <summary>
        /// "Override localy" base class function ReadFolderName
        /// </summary>
        /// <returns></returns>
        private string ReadFolderName()
        {
            if (TabBrowser.IsSelected)
                return DKPFolderCommandVariable.uriLabel.Text.Length != 0 ? DKPFolderCommandVariable.uriLabel.Text : CmbStationBS.Text;
            else
                return baseImportTree.ReadFolderName();
        }

        /// <summary>
        /// "Override localy" base class function ReadStationName
        /// </summary>
        /// <returns></returns>
        private String ReadStationName()
        {
            if (TabBrowser.IsSelected)
            {
                //return CmbStation.Text --> will not be updated immediatly when OnChangedEvent being raised
                if (CmbStationBS.SelectedValue == null)
                    return string.Empty;
                else
                    return CmbStationBS.SelectedValue.ToString();
            } else
            {
                return baseImportTree.ReadStationName();
            }
        }


        private string AddStationName(string tagName)
        {
            string stationaName = string.Empty;

            if (TabBrowser.IsSelected)
            {
                if (AddStationNameBS.IsChecked == true)
                    stationaName = CmbStationBS.Text;
            }
            stationaName += "_";

            tagName = (stationaName != "_" ?
                        stationaName + tagName : tagName);

            return tagName;
        }


        #region IDisposable Members

        public void Dispose()
        {
            //if (idl != null)
            //    idl.Dispose();
            using (new WaitCursor())
            {
                if (importDataModel != null)
                {
                    importDataModel.Dispose();
                    importDataModel = null;
                }

                if (baseImportTree != null)
                {
                    baseImportTree.LoadImportFile -= LoadFile;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }

        #endregion

        private void TabControlExt_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            using (new WaitCursor())
            {
                if ((e.NewSelectedItem as DXTabItem) != null && (e.NewSelectedItem as DXTabItem)  == TabBrowser)
                {
                    if (browserOpcUa.Content == null)
                    {
                        var editor = OPCUAViewModelComponent.opcuaBrowserService.MultiSelectionEditor;

                        editor.ClearValue(FrameworkElement.WidthProperty);
                        editor.ClearValue(FrameworkElement.HeightProperty);
                        

                        browserOpcUa.Content = editor;
                    }
                }
            }
        }

        private void CmbStationBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void UpdateBS_Click(object sender, RoutedEventArgs e)
        {

            ImportSelectedTags();

            // close import windows
            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }

        private void TabControlExt_SelectionChanging(object sender, TabControlSelectionChangingEventArgs e)
        {
            if((e.NewSelectedItem as DXTabItem)!=null && (e.NewSelectedItem as DXTabItem) == TabBrowser)
            {
                string stationName = ReadStationName();
                if (string.IsNullOrEmpty(stationName))
                {
                    MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportEnterStationName, DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                    e.Cancel = true;
                    return;
                }
            }
        }
    }

    public class ImportDataOpcClientDriver : ImportData, IDisposable
    {
        public ImportDataOpcClientDriver(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelOpcClientDriver = inDataModel as ImportDataModelOpcClientDriver;
        }
        private ImportDataModelOpcClientDriver dataModelOpcClientDriver { get; set; }

        private int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        
        private string _Endpointurl;
        public string Endpointurl
        {
            get { return _Endpointurl; }
            set { _Endpointurl = value; }
        }
        private string _AppName;
        public string AppName
        {
            get { return _AppName; }
            set { _AppName = value; }
        }
        
        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }
        private string _RelativePath;
        public string RelativePath
        {
            get { return _RelativePath; }
            set { _RelativePath = value; }
        }
        private string _NodeId;
        public string NodeId
        {
            get { return _NodeId; }
            set { _NodeId = value; }
        }
        
        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if(Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}
        public override DataType IconTagType
        {
            get {
                if (_TagTypeInt == -1)
                    return DataType.Boolean;
                else
                    return (DataType)_TagTypeInt; 
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataOpcClientDriver;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    };

    public class ImportDataModelOpcClientDriver : ImportDataModel, IDisposable
    {
        public ImportDataModelOpcClientDriver(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataOpcClientDriver importData = new ImportDataOpcClientDriver(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataOpcClientDriver els7 = el as ImportDataOpcClientDriver;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
        #endregion
    }

}
