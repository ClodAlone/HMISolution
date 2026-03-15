/* Copyright (c) 1996-2017, OPC Foundation. All rights reserved.
/* Copyright (c) 1996-2017, OPC Foundation. All rights reserved.

   The source code in this file is covered under a dual-license scenario:
     - RCL: for OPC Foundation members in good-standing
     - GPL V2: everybody else

   RCL license terms accompanied with this source code. See http://opcfoundation.org/License/RCL/1.00/

   GNU General Public License as published by the Free Software Foundation;
   version 2 of the License are accompanied with this source code. See http://opcfoundation.org/License/GPLv2

   This source code is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/

using DevExpress.Xpo;
using log4net;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UFInterfaces;
using UFUAEditor.Document;
using UFUAModel;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities.ImportExportHelpers;

namespace UFUAEditor.Helpers
{    
    /// <summary>
    /// A set of nodes in an address space.
    /// </summary>
    public partial class UANodeSet: IDisposable
    {
        #region Constructors
        /// <summary>
        /// Creates an empty nodeset.
        /// </summary>
        public UANodeSet(UFUAServerDocument document)
        {
            this.document = document;
            if (document == null) throw new ArgumentNullException("ServerDocument");
            enumStrings = document.GetEnums();
            alarmThresholds = document.GetThresholdsList().ToList();
            workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
            uiMsgBox = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        public UANodeSet()
        {
        
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads a nodeset from a stream.
        /// </summary>
        /// <param name="istrm">The input stream.</param>
        /// <returns>The set of nodes</returns>
        public static UANodeSet Read(Stream istrm)
        {
            XmlTextReader reader = new XmlTextReader(istrm);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UANodeSet));
                return serializer.Deserialize(reader) as UANodeSet;
            }
            finally
            {
                reader.Close();
            }
        }
        /// <summary>
        /// Write a nodeset to a stream.
        /// </summary>
        /// <param name="istrm">The input stream.</param>
        public void Write(Stream istrm)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlWriter.Create(istrm, settings))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UANodeSet));
                    serializer.Serialize(writer, this);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(ex.Message);
                    writer.Close();
                    return;
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Imports a node from the set.
        /// </summary>
        public ImportExportResult Import(UFUAServerDocument document)
        {
            this.document = document;
            if (document == null) throw new ArgumentNullException("serverDocument");
            workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
            uiMsgBox = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

            InitNamespaceUris();
            if (Items.Count() > 0)
            {
                bCollectNodeIds = this.NamespaceUris != null && !this.NamespaceUris.Contains(ServerUri);
                existingObject = new Dictionary<string, List<UFUATag>>();
                existingAreaObject = new Dictionary<string, List<UFUAArea>>();
                existingAlarmSourceObject = new Dictionary<string, List<UFUAAlarmSource>>();
                existingAlarmSourceObjects = new Dictionary<string, UFUAAlarmSource>();
                existingAlarmDefinitionObject = new Dictionary<string, List<UFUAAlarmDefinition>>();
                computedVariableType = new List<string>();
                UAObject rootAlarmFolder = GetRootFolders(hasNotifier, true).First();
                List<UAObject> rootAlarmObjects = rootAlarmFolder != null ? GetRootObjects<UAObject>(rootAlarmFolder?.NodeId) : new List<UAObject>();
                if (rootAlarmObjects.Count() == 0)  //if no Alarm Objs have been find means the folder is not a root Alarm Folder
                    rootAlarmFolder = null;

                var rootNodesList = (from UANode node in Items
                                             where node is UAMethod || node is UAObject || node is UAVariable
                                             select node).ToList();

                if (addedObjects == null)
                    addedObjects = new List<IXPSimpleObject>();
                addedObjects.Clear();
                if (changedObjects == null)
                    changedObjects = new List<IXPSimpleObject>();
                changedObjects.Clear();

                workspace?.UpdateProgressState(0, 
                                                  rootNodesList.Count + rootAlarmObjects.Count,
                                                  $"{Properties.Resources.WorkInProgress}",
                                                  TaskbarItemProgressState.Normal);

                foreach (var obj in rootAlarmObjects)
                {
                    CreateArea(obj);
                    workspace?.IncrementProgressState();
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        break;
                }

                List<UAObject> rootFolders = GetRootFolders(hasComponent);
                foreach (var rootFolder in rootFolders)
                {
                    if (rootFolder == null || rootFolder == rootAlarmFolder)
                        continue;
                    string guidString = rootFolder.NodeId.Split(';')[1].Substring(2);
                    Guid guid = new Guid();
                    try
                    {
                        guid = Guid.ParseExact(guidString, "D");
                    }
                    catch { }

                    if (guid == UFUAServerInfo.Guids.RootTagsGuid)  //Movicon Address Space
                    {
                        List<UAMethod> rootUAMethods = GetRootObjects<UAMethod>(rootFolder?.NodeId);
                        List<UAObject> rootObjects = GetRootObjects<UAObject>(rootFolder?.NodeId);
                        List<UAVariable> rootTags = GetRootObjects<UAVariable>(rootFolder?.NodeId);

                        foreach (var obj in rootTags)
                        {
                            CreateTag(obj);
                            workspace?.IncrementProgressState();
                            if (dialogRetValue == CustomDialogResults.Cancel)
                                break;
                        }
                        foreach (var obj in rootUAMethods)
                        {
                            CreateMethod(obj);
                            workspace?.IncrementProgressState();
                            if (dialogRetValue == CustomDialogResults.Cancel)
                                break;
                        }
                        foreach (var obj in rootObjects)
                        {
                            CreateObject(obj);
                            workspace?.IncrementProgressState();
                            if (dialogRetValue == CustomDialogResults.Cancel)
                                break;
                        }
                    }
                    else
                    {
                        SetParentID(rootFolder);
                        if (rootFolder != null)
                            CreateObject(rootFolder);
                    }

                    if (dialogRetValue == CustomDialogResults.Cancel)
                        document.DropChanges();
                }

                

            }

            if (ErrorMessages.Count() > 0)
            {
                ErrorMessages.Distinct().ToList().ForEach(m => logGeneral.Error(m));
                return new ImportExportResult(ResultType.Successfully) { WasError = true, AddedObjects = addedObjects, ChangedObjects = changedObjects};
            }

            return new ImportExportResult(ResultType.Successfully) { AddedObjects = addedObjects, ChangedObjects = changedObjects };
        }

        /// <summary>
        /// From the imported address space returns the first level of the folders
        /// </summary>
        /// <param name="rootKind"></param>
        /// <param name="searchingAlarmFolder"></param>
        /// <returns>A list of root folders</returns>
        private List<UAObject> GetRootFolders(string rootKind, bool searchingAlarmFolder = false)
        {
            UAObject rootObject = null;
            List<UAObject> rootObjectList = new List<UAObject>();
            var rootNodeList = (from UANode node in Items
                                    where node is UAObject && (node as UAInstance).ParentNodeId == null &&
                                    node.References.Count() > 0
                                select node as UAObject).ToList();
            foreach (var item in rootNodeList)
            {
                rootObject = (from reference in item.References
                 where reference.ReferenceType == ExportAlias(organizes) &&
                 reference.Value == ExportAlias(objectsFolder)
                 select item).FirstOrDefault();
                if(rootObject != null)
                {
                    rootObject = (from reference in item.References
                     where //reference.ReferenceType == ExportAlias(rootKind) ||
                     reference.ReferenceType == ExportAlias(organizes) && reference.IsForward == false
                     select item).FirstOrDefault();
                    rootObjectList.Add(rootObject);
                    if(rootObject != null && searchingAlarmFolder)
                        break;
                }
            }

            return rootObjectList;
        }

        /// <summary>
        /// Assign, recursively, the parent ID to a node if it hasn't
        /// </summary>
        /// <param name="parent"></param>
        private void SetParentID(UANode parent)
        {
            foreach(var reference in parent.References)
            {
                if ((!(reference.ReferenceType == ExportAlias(hasComponent) && !reference.IsForward) &&
                    !(reference.ReferenceType == ExportAlias(organizes) && !reference.IsForward)) &&
                    reference.ReferenceType != ExportAlias(hasProperty))
                {
                    try
                    {
                        var rootnodeVariable = (from UANode node in Items
                                                where node is UAVariable &&
                                                (node as UAInstance).ParentNodeId == null &&
                                                node.NodeId == reference.Value
                                                select node as UAVariable).FirstOrDefault();
                        if (rootnodeVariable != null)
                        {
                            rootnodeVariable.ParentNodeId = parent.NodeId;
                            SetParentID(rootnodeVariable);
                            continue;
                        }
                        var rootnodeMethod = (from UANode node in Items
                                              where node is UAMethod &&
                                              (node as UAInstance).ParentNodeId == null &&
                                              node.NodeId == reference.Value
                                              select node as UAMethod).FirstOrDefault();
                        if (rootnodeMethod != null)
                        {
                            rootnodeMethod.ParentNodeId = parent.NodeId;
                            SetParentID(rootnodeMethod);
                            continue;
                        }
                        var rootnodeObject = (from UANode node in Items
                                              where node is UAObject &&
                                              (node as UAInstance).ParentNodeId == null &&
                                              node.NodeId == reference.Value
                                              select node as UAObject).FirstOrDefault();

                        if (rootnodeObject != null)
                        {
                            rootnodeObject.ParentNodeId = parent.NodeId;
                            SetParentID(rootnodeObject);
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Init root node item for export.
        /// </summary>
        public ImportExportResult Export()
        {
            if (document == null) throw new ArgumentNullException("serverDocument");

            InitNamespaceUris();


            //Defined in the Constructor
            this.xmlns = new XmlSerializerNamespaces();
            this.xmlns.Add(uaxPrefix, Namespaces.OpcUaXsd);


            this.NamespaceUris = new string[] { ReservedUri, $"{ApplicationUri}:{document.GetAplicationName()}", ServerUri, Diagnostics};
            this.Models = new ModelTableEntry[] {
                new ModelTableEntry() { ModelUri = ReservedUri},
                new ModelTableEntry() { ModelUri = ServerUri, Version = "1.02" }
            };
            //existingAlarmDefinitions = document.GetAlarmDefinitions().Distinct().ToDictionary(kvp => GetAlarmPath(kvp), kvp => kvp);
            ExportAlias();

            List<UANode> baseAlarmNodeList = CreateRootAlarmNodeList(UFUAServerInfo.UFUAServerInfo.GetAlarmRootName(), UFUAServerInfo.Guids.RootAlarmsGuid);
            foreach (var value in baseAlarmNodeList)
                Export(value);

            List<UANode> baseFolderNodeList = CreateRootNodeList(UFUAServerInfo.UFUAServerInfo.GetTagRootName(), UFUAServerInfo.Guids.RootTagsGuid);
            foreach (var value in baseFolderNodeList)
                Export(value);

            if (ErrorMessages.Count() > 0)
            {
                ErrorMessages.Distinct().ToList().ForEach(m => logGeneral.Error(m));
                return new ImportExportResult(ResultType.Successfully) { WasError = true };
            }

            return new ImportExportResult(ResultType.Successfully);
        }
        #endregion

        #region Private Methods

        private void ExportAlias()
        {
            AddAlias(BrowseNames.HasProperty, hasProperty);
            AddAlias(BrowseNames.HasComponent, hasComponent);
            AddAlias(BrowseNames.HasHistoricalConfiguration, hasHistoricalConfiguration);
            AddAlias(BrowseNames.HasCondition, hasCondition);
            AddAlias(BrowseNames.HasEventSource, hasEventSource);
            AddAlias(BrowseNames.HasNotifier, hasNotifier);

            AddAlias(BrowseNames.Organizes, organizes);
            AddAlias(BrowseNames.ObjectsFolder, objectsFolder);
            AddAlias(BrowseNames.FolderType, ExportAlias(folderType));
            AddAlias(BrowseNames.PropertyType, propertyType);
            AddAlias(BrowseNames.ServerType, serverType);
            AddAlias(BrowseNames.Server, server);
            AddAlias(BrowseNames.HasTypeDefinition.ToString(), hasTypeDefinition);

            AddAlias(BrowseNames.BaseDataVariableType, VariableTypeIds.BaseDataVariableType.ToString());
            AddAlias(BrowseNames.TwoStateDiscreteType, VariableTypeIds.TwoStateDiscreteType.ToString());
            AddAlias(BrowseNames.MultiStateDiscreteType, VariableTypeIds.MultiStateDiscreteType.ToString());
            AddAlias(BrowseNames.AnalogItemType, VariableTypeIds.AnalogItemType.ToString());
            AddAlias(BrowseNames.MethodNode, DataTypeIds.MethodNode.ToString());
            AddAlias(BrowseNames.LocalizedText.ToString(), DataTypeIds.LocalizedText.ToString());
            AddAlias(BrowseNames.Range.ToString(), DataTypeIds.Range.ToString());
            AddAlias(BrowseNames.EUInformation.ToString(), DataTypeIds.EUInformation.ToString());
            AddAlias(BrowseNames.HistoricalDataConfigurationType.ToString(), historicalDataConfigurationType);
            AddAlias(BrowseNames.AggregateConfigurationType.ToString(), ObjectTypeIds.AggregateConfigurationType.ToString());

            AddAlias(BrowseNames.ExclusiveLevelAlarmType.ToString(), ObjectTypeIds.ExclusiveLevelAlarmType.ToString());
            AddAlias(BrowseNames.NonExclusiveLevelAlarmType.ToString(), ObjectTypeIds.NonExclusiveLevelAlarmType.ToString());
            AddAlias(BrowseNames.ExclusiveDeviationAlarmType.ToString(), ObjectTypeIds.ExclusiveDeviationAlarmType.ToString());
            AddAlias(BrowseNames.NonExclusiveDeviationAlarmType.ToString(), ObjectTypeIds.NonExclusiveDeviationAlarmType.ToString());
            AddAlias(BrowseNames.ExclusiveRateOfChangeAlarmType.ToString(), ObjectTypeIds.ExclusiveRateOfChangeAlarmType.ToString());
            AddAlias(BrowseNames.NonExclusiveRateOfChangeAlarmType.ToString(), ObjectTypeIds.NonExclusiveRateOfChangeAlarmType.ToString());
            AddAlias(BrowseNames.TripAlarmType.ToString(), ObjectTypeIds.TripAlarmType.ToString());


            AddAlias(BrowseNames.Boolean.ToString(), DataTypeIds.Boolean.ToString());
            AddAlias(BrowseNames.SByte.ToString(), DataTypeIds.SByte.ToString());
            AddAlias(BrowseNames.Byte.ToString(), DataTypeIds.Byte.ToString());
            AddAlias(BrowseNames.Int16.ToString(), DataTypeIds.Int16.ToString());
            AddAlias(BrowseNames.UInt16.ToString(), DataTypeIds.UInt16.ToString());
            AddAlias(BrowseNames.Int32.ToString(), DataTypeIds.Int32.ToString());
            AddAlias(BrowseNames.UInt32.ToString(), DataTypeIds.UInt32.ToString());
            AddAlias(BrowseNames.Int64.ToString(), DataTypeIds.Int64.ToString());
            AddAlias(BrowseNames.UInt64.ToString(), DataTypeIds.UInt64.ToString());
            AddAlias(BrowseNames.Float.ToString(), DataTypeIds.Float.ToString());
            AddAlias(BrowseNames.Double.ToString(), DataTypeIds.Double.ToString());
            AddAlias(BrowseNames.String.ToString(), DataTypeIds.String.ToString());
            AddAlias(BrowseNames.Duration.ToString(), DataTypeIds.Duration.ToString());
            AddAlias(BrowseNames.ExceptionDeviationFormat.ToString(), DataTypeIds.ExceptionDeviationFormat.ToString());
            AddAlias(BrowseNames.UtcTime.ToString(), DataTypeIds.UtcTime.ToString());
        }

        private List<UANode> CreateRootAlarmNodeList(string displayName, Guid guid)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            UAObject uANode = new UAObject();
            uANode.NodeId = Export(FromGuidToNodeId(guid));
            uANode.BrowseName = new QualifiedName(displayName, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { displayName });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;
            nodeList.Add(uANode);
            List<Reference> m_references = new List<Reference>();

            var areas = document.GetAlarmAreas(null);
            if (areas.Count > 0)
            {
                workspace?.UpdateProgressState(0,
                                               areas.Count,
                                               $"{Properties.Resources.WorkInProgress}",
                                               TaskbarItemProgressState.Normal);

                foreach (var area in areas)
                {
                    var instance = CreateAreaNodeList(area, uANode);
                    if (instance != null)
                    {
                        nodeList.AddRange(instance);
                        m_references.Add(CreateReference(ExportAlias(organizes), Export(FromGuidToNodeId(area.NodeId))));
                        m_references.Add(CreateReference(ExportAlias(hasNotifier), Export(FromGuidToNodeId(area.NodeId))));
                    }
                    workspace?.IncrementProgressState();
                }
            }
            m_references.Add(CreateReference(ExportAlias(organizes), ExportAlias(objectsFolder), false));
            m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), ExportAlias(folderType)));
            uANode.References = m_references.ToArray();

            if (propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }

        private List<UANode> CreateRootNodeList(string displayName, Guid guid)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            UAObject uANode = new UAObject();
            uANode.NodeId = Export(FromGuidToNodeId(guid));
            uANode.BrowseName = new QualifiedName(displayName, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { displayName });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;
            nodeList.Add(uANode);
            List<Reference> m_references = new List<Reference>();

            var rootfolders = document.GetFolderCollection(null);
            var rootTags = document.GetTagCollection(null);

            if (rootfolders.Count > 0 || rootTags.Count > 0)
            {
                workspace?.UpdateProgressState(0,
                                               rootfolders.Count + rootTags.Count,
                                               $"{Properties.Resources.WorkInProgress}",
                                               TaskbarItemProgressState.Normal);
                foreach (var tag in rootTags)
                {
                    var instanceList = CreateTagNodeList(tag, uANode);
                    if (instanceList != null)
                    {
                        nodeList.AddRange(instanceList);
                        m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(tag.NodeId))));
                    }
                    workspace?.IncrementProgressState();
                }

                foreach (var folder in rootfolders)
                {
                    var instance = CreateFolderNodeList(folder, uANode);
                    if (instance != null)
                    {
                        nodeList.AddRange(instance);
                        m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(folder.NodeId))));
                    }
                    workspace?.IncrementProgressState();
                }
            }
            m_references.Add(CreateReference(ExportAlias(organizes), ExportAlias(objectsFolder),false));
            m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), ExportAlias(folderType)));
            uANode.References = m_references.ToArray();

            if (propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }

        private void InitNamespaceUris()
        {
            string serverConfiguration = UFUAServerInfo.UFUAServerInfo.GetServerConfigFile();
            var app = Utilities.ApplicationConfigurationHelper.LoadConfiguration(serverConfiguration);
            if (app != null)
                ApplicationUri = app.ApplicationUri;

            if (string.IsNullOrEmpty(ApplicationUri)) throw new ArgumentNullException("ApplicationUri");

            XmlElement extension = (from e in app?.Extensions where e.Name == "UAServerConfiguration" select e).FirstOrDefault();
            if (extension != null && extension.Attributes["xmlns"] != null)
                ServerUri = extension.Attributes["xmlns"].Value;

            if (string.IsNullOrEmpty(ServerUri)) throw new ArgumentNullException("ServerUri");

            ReservedUri = Namespaces.OpcUa;
            Diagnostics = $"{ReservedUri}/Diagnostics";


            trueState = new QualifiedName(BrowseNames.TrueState, NameSpaceIndex).ToString();
            falseState = new QualifiedName(BrowseNames.FalseState, NameSpaceIndex).ToString();

        }
        private void AddAlias(string alias, string nodeId)
        {
            int count = 1;

            if (this.Aliases != null)
            {
                for (int ii = 0; ii < this.Aliases.Length; ii++)
                {
                    if (this.Aliases[ii].Alias == alias)
                    {
                        this.Aliases[ii].Value = nodeId;
                        return;
                    }
                }

                count += this.Aliases.Length;
            }

            NodeIdAlias[] aliases = new NodeIdAlias[count];

            if (this.Aliases != null)
            {
                Array.Copy(this.Aliases, aliases, this.Aliases.Length);
            }

            aliases[count - 1] = new NodeIdAlias() { Alias = alias, Value = nodeId };
            this.Aliases = aliases;
        }
        private UFUAArea CheckExistingArea(UANode node, UFUAArea root = null)
        {
            if (node == null || string.IsNullOrEmpty(node.DisplayName?.FirstOrDefault()?.Value.ToString()))
                return null;
            UFUAArea area = null;
            string areaName = string.Empty;
            string nodeName = node.DisplayName?.FirstOrDefault()?.Value.ToString();
            areaName = UFUAModel.Helpers.NameValidator.EnsureValidName(nodeName);
            if (string.IsNullOrEmpty(areaName))
                areaName = document.NewAlarmAreaName(root);
            string rootName = UFUAServerInfo.UFUAServerInfo.GetAlarmRootName();
            string rootPath = root != null ? $"{rootName}/{root.GetRelativeName()}" : rootName;
            string objectFullName = $"{rootPath}/{nodeName}";
            List<string> listNames = new List<string>();

            if (!existingAreaObject.ContainsKey(rootPath))
            {
                existingAreaObject[rootPath] = document.GetAlarmAreas(root).ToList();
            }

            listNames = (from o in existingAreaObject[rootPath] select o.Name).ToList();
            if (listNames != null && listNames.Contains(areaName))
            {
                area = (from e in existingAreaObject[rootPath] where e.Name == nodeName select e).FirstOrDefault();
            }
            else
            {
                area = document.AddNewAlarmArea(root, areaName);
                addedObjects.Add(area);
            }

            if (area != null)
            {
                if (!existingAreaObject[rootPath].Contains(area))
                    existingAreaObject[rootPath].Add(area);
            }

            return area;
        }
        private UFUATag CheckExistingTag(UANode node, UFUAFolder root = null, UFUATagPrototype prototype = null)
        {
            if (node == null || string.IsNullOrEmpty(node.DisplayName?.FirstOrDefault()?.Value.ToString()))
                return null;
            UFUATag tag = null;
            string tagName = string.Empty;
            string nodeName = node.DisplayName?.FirstOrDefault()?.Value.ToString();
            tagName = UFUAModel.Helpers.NameValidator.EnsureValidName(nodeName);
            if (string.IsNullOrEmpty(tagName))
                tagName = document.NewTagName(root);
            string rootName = prototype == null ? "Root" : "Prototype " + prototype.Name;
            string rootPath = root != null ? $"{rootName}\\{root.GetFullName()}" : rootName;
            string objectFullName = $"{rootPath}\\{nodeName}";
            List<string> listNames = new List<string>();
            bool reNewTag = false;
            if (prototype == null)
            {
                if (!existingObject.ContainsKey(rootPath))
                {
                    existingObject[rootPath] = document.GetTagCollection(root).ToList();
                }

                listNames = (from o in existingObject[rootPath] select o.Name).ToList();
                if (listNames != null && listNames.Contains(tagName))
                {
                    tag = (from e in existingObject[rootPath] where e.Name == nodeName select e).FirstOrDefault();
                    if (tag != null)
                    {
                        if (dialogRetValue != CustomDialogResults.YesAll &&
                            dialogRetValue != CustomDialogResults.NoAll &&
                            dialogRetValue != CustomDialogResults.Cancel)
                        {
                            if (uiMsgBox != null)
                                dialogRetValue = uiMsgBox.ShowYesNoAllCancel(string.Format(Properties.Resources.ImportTagNameExist, objectFullName), CustomDialogIcons.Question);
                        }

                        if (dialogRetValue == CustomDialogResults.Cancel)
                            return null;
                    }
                }
                else
                {
                    tag = document.AddNewTag(root);
                    addedObjects.Add(tag);
                    reNewTag = true;
                }
            }
            else
            {
                if (!existingObject.ContainsKey(rootPath))
                {
                    existingObject[rootPath] = document.GetTagsList(prototype, root).ToList();
                }

                listNames = (from o in existingObject[rootPath] select o.Name).ToList();
                if (listNames != null && listNames.Contains(tagName))
                {
                    tag = (from e in existingObject[rootPath] where e.Name == nodeName select e).FirstOrDefault();
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        return null;
                }
                else
                {
                    tag = document.AddNewTag(root);
                    addedObjects.Add(tag);
                    prototype.Members.Add(tag);
                    reNewTag = true;
                }
            }

            if (tag != null)
            {
                if (dialogRetValue == CustomDialogResults.Yes || dialogRetValue == CustomDialogResults.YesAll)
                {
                    if (!reNewTag)
                    {
                        tag = document.AddNewTag(root);
                        addedObjects.Add(tag);
                    }
                    else
                        changedObjects.Add(tag);

                    tagName = document.NewImportedTagName(root, tagName, null, listNames, "{0}_{1}", prototype);
                }

                tag.Name = tagName;
                if (tagName != nodeName)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedTagNameChanged, $"{nodeName}", $"{tagName}"));

                tag.NodeIdToPublish = bCollectNodeIds ? node.NodeId : null;

                if (!existingObject[rootPath].Contains(tag))
                    existingObject[rootPath].Add(tag);
            }

            return tag;
        }

        private List<UANode> CreateTagNodeList(UFUAModel.UFUATag ufuaTag, UANode parentNode = null, bool isPrototypeMember = false)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            List<UANode> historianNodeList = new List<UANode>();
            UANode uANode = null;
            NodeId typeDefinitionId = null;
            List<NodeId> alarmDefinitionIds = new List<NodeId>();
            BuiltInType builtinType = BuiltInType.Null;
            NodeId dataTypeId = null;
            List<Reference> m_references = new List<Reference>();
            string parent = parentNode?.NodeId.ToString();
            GetDataType(ufuaTag.DataType, out builtinType, out dataTypeId);

            switch (ufuaTag.ModelType)
            {
                case UFUAModel.ModelType.Variable:
                    {
                        uANode =  new UAVariable();
                        if (!String.IsNullOrEmpty(parent))
                            (uANode as UAVariable).ParentNodeId = parent;
                        typeDefinitionId = VariableTypeIds.BaseDataVariableType;
                        break;
                    }

                case UFUAModel.ModelType.Digital:
                    {
                        uANode = new UAVariable();
                        if (!String.IsNullOrEmpty(parent))
                            (uANode as UAVariable).ParentNodeId = parent;
                        typeDefinitionId = VariableTypeIds.TwoStateDiscreteType;
                        break;
                    }
                case UFUAModel.ModelType.Enumerated:
                    {
                        uANode = new UAVariable();
                        if (parent != null)
                            (uANode as UAVariable).ParentNodeId = parent;
                        typeDefinitionId = VariableTypeIds.MultiStateDiscreteType;
                        break;
                    }

                case UFUAModel.ModelType.Analog:
                    {
                        uANode = new UAVariable();
                        if (!String.IsNullOrEmpty(parent))
                            (uANode as UAVariable).ParentNodeId = parent;
                        typeDefinitionId = VariableTypeIds.AnalogItemType;
                        break;
                    }

                case UFUAModel.ModelType.Method:
                    {
                        uANode = new UAMethod();
                        if (!String.IsNullOrEmpty(parent))
                            (uANode as UAMethod).ParentNodeId = parent;
                        typeDefinitionId = DataTypeIds.MethodNode;
                        break;
                    }

                case UFUAModel.ModelType.ObjectType:
                    {
                        uANode = new UAObject();
                        if (!String.IsNullOrEmpty(parent))
                            (uANode as UAObject).ParentNodeId = parent;
                        break;
                    }
            }

            uANode.NodeId = Export(FromGuidToNodeId(ufuaTag.NodeId));
            if (isPrototypeMember)
               uANode.NodeId = ConstructPropertiesId(parent, ufuaTag.Name);

            if (uANode is UAVariable)
            {
                UAVariable variable = (uANode as UAVariable);
                object value = null;
                if (!String.IsNullOrEmpty(ufuaTag.InitialValue))
                {
                    try
                    {
                        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                        value = ChangeType(ufuaTag.InitialValue, builtinType, ufuaTag.ArrayDimension, info);

                        if (value != null)
                            variable.InitialValue = GetValue(value);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                int valueRank;
                List<uint> arrayDimensions;
                if (ufuaTag.ArrayDimension == 0)
                {
                    valueRank = ValueRanks.Scalar;
                    arrayDimensions = null;
                }
                else
                {
                    valueRank = ValueRanks.OneDimension;
                    arrayDimensions = new List<uint>(1);
                    arrayDimensions.Add(ufuaTag.ArrayDimension);
                }
                byte accessLevel;
                byte userAccessLevel;
                switch (ufuaTag.AccessLevel)
                {
                    default:
                    case UFUAModel.AccessLevels.None:
                        accessLevel = userAccessLevel = Opc.Ua.AccessLevels.None;
                        break;
                    case UFUAModel.AccessLevels.CurrentRead:
                        accessLevel = userAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                        break;
                    case UFUAModel.AccessLevels.CurrentWrite:
                        accessLevel = userAccessLevel = Opc.Ua.AccessLevels.CurrentWrite;
                        break;
                    case UFUAModel.AccessLevels.CurrentReadOrWrite:
                        accessLevel = userAccessLevel = Opc.Ua.AccessLevels.CurrentReadOrWrite;
                        break;
                }
                variable.DataType = ExportAlias(dataTypeId);
                variable.ValueRank = valueRank;
                variable.ArrayDimensions = Export(arrayDimensions);
                variable.AccessLevel = accessLevel;
                if (ufuaTag.AuditTraceEnabled)
                {
                    if (ufuaTag.EnterCommentOnAudit)
                    {
                        var newAccessLevel = (int)variable.AccessLevel;
                        newAccessLevel &= ~Opc.Ua.AccessLevels.CurrentWrite;
                        variable.AccessLevel = variable.UserAccessLevel = (byte)newAccessLevel;
                    }
                }

                variable.UserAccessLevel = userAccessLevel;
                variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                variable.Historizing = false;

                if (ufuaTag.ModelType == UFUAModel.ModelType.Variable)
                {
                    m_references.AddRange(CreateVariableReferences(ufuaTag, uANode.NodeId, propertyNodeList));
                }
                else if (ufuaTag.ModelType == UFUAModel.ModelType.Analog)
                {
                    m_references.AddRange(CreateAnalogReferences(ufuaTag, uANode.NodeId, propertyNodeList));
                }
                else if (ufuaTag.ModelType == UFUAModel.ModelType.Enumerated)
                {
                    m_references.AddRange(CreateEnumReferences(ufuaTag, uANode.NodeId, propertyNodeList));
                }
                else if (ufuaTag.ModelType == UFUAModel.ModelType.Digital)
                {
                    m_references.AddRange(CreateDigitalReferences(ufuaTag, uANode.NodeId, propertyNodeList));
                }

                if(!string.IsNullOrEmpty(ufuaTag.HistorianSettings))
                {
                    m_references.AddRange(CreateHistoricalConfiguration(ufuaTag, uANode.NodeId, propertyNodeList));
                    variable.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    variable.Historizing = true;
                }

                if (ufuaTag.UFUAAlarmThresholds?.Count > 0)
                    m_references.AddRange(CreateAlarmCondition(ufuaTag, uANode.NodeId, propertyNodeList));

                uANode = variable;
            }
            //else if (uANode is UAMethod)
            //{
            //    m_references.AddRange(CreateMethodReferences(ufuaTag, uANode.NodeId, propertyNodeList));
            //}
            else if (uANode is UAObject)
            {
                if (ufuaTag.ModelType == UFUAModel.ModelType.ObjectType && !string.IsNullOrEmpty(ufuaTag.PrototypeName))
                {
                    typeDefinitionId = CreatePrototype(ufuaTag, propertyNodeList);
                    if(typeDefinitionId != null)
                    {
                        document.CreateSubPrototype(ufuaTag);
                        m_references.AddRange(CreatePrototypeReferences(ufuaTag, uANode, propertyNodeList));
                    }
                }
            }


            if (typeDefinitionId != null)
                m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), ExportAlias(typeDefinitionId)));

            if (parent != null)
            {
                m_references.Add(CreateReference(ExportAlias(hasComponent), parent.ToString(), false));
            }

            uANode.References = m_references.ToArray();
            uANode.BrowseName = new QualifiedName(ufuaTag.Name, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { ufuaTag.Name });
            uANode.Description = Export(new Opc.Ua.LocalizedText[] { ufuaTag.Description });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;

            nodeList.Add(uANode);
            if(propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }

        private void GetDataType(DataType? dataType, out BuiltInType builtinType, out NodeId dataTypeId)
        {
            builtinType = BuiltInType.Null;
            dataTypeId = null;

            switch (dataType)
            {
                case UFUAModel.DataType.Boolean: { builtinType = BuiltInType.Boolean; dataTypeId = DataTypeIds.Boolean; break; }
                case UFUAModel.DataType.SByte: { builtinType = BuiltInType.SByte; dataTypeId = DataTypeIds.SByte; break; }
                case UFUAModel.DataType.Byte: { builtinType = BuiltInType.Byte; dataTypeId = DataTypeIds.Byte; break; }
                case UFUAModel.DataType.Int16: { builtinType = BuiltInType.Int16; dataTypeId = DataTypeIds.Int16; break; }
                case UFUAModel.DataType.UInt16: { builtinType = BuiltInType.UInt16; dataTypeId = DataTypeIds.UInt16; break; }
                case UFUAModel.DataType.Int32: { builtinType = BuiltInType.Int32; dataTypeId = DataTypeIds.Int32; break; }
                case UFUAModel.DataType.UInt32: { builtinType = BuiltInType.UInt32; dataTypeId = DataTypeIds.UInt32; break; }
                case UFUAModel.DataType.Int64: { builtinType = BuiltInType.Int64; dataTypeId = DataTypeIds.Int64; break; }
                case UFUAModel.DataType.UInt64: { builtinType = BuiltInType.UInt64; dataTypeId = DataTypeIds.UInt64; break; }
                case UFUAModel.DataType.Float: { builtinType = BuiltInType.Float; dataTypeId = DataTypeIds.Float; break; }
                case UFUAModel.DataType.Double: { builtinType = BuiltInType.Double; dataTypeId = DataTypeIds.Double; break; }
                case UFUAModel.DataType.String: { builtinType = BuiltInType.String; dataTypeId = DataTypeIds.String; break; }
            }
        }

        private string CreatePrototype(UFUAModel.UFUATag tag, List<UANode> propertyNodeList)
        {
            if (tag == null)
                return null;
            
            var prototypeName = tag.PrototypeName;
            if (typePrototypeNodeIds.ContainsKey(prototypeName))
                return typePrototypeNodeIds[prototypeName];

            var prototype = document.GetPrototype(prototypeName);

            if (prototype != null)
            {
                UAObjectType prototypeNode = new UAObjectType();
                prototypeNode.NodeId = Export(FromGuidToNodeId(prototype.NodeId));
                prototypeNode.BrowseName = new QualifiedName(prototypeName, NameSpaceIndex).ToString();
                prototypeNode.DisplayName = Export(new Opc.Ua.LocalizedText[] { prototypeName });
                prototypeNode.Description = null;
                prototypeNode.WriteMask = 0;
                prototypeNode.UserWriteMask = 0;
                propertyNodeList.Add(prototypeNode);

                var rootfolders = new List<UFUAModel.UFUAFolder>(prototype.Folders);
                var rootTags = new List<UFUAModel.UFUATag>(prototype.Members);

                if (rootfolders.Count > 0 || rootTags.Count > 0)
                {
                    List<Reference> m_references = new List<Reference>();
                    List<UANode> list = new List<UANode>();
                    foreach (var _tag in rootTags)
                    {
                        var instanceList = CreateTagNodeList(_tag, prototypeNode);
                        if (instanceList != null)
                        {
                            list.AddRange(instanceList);
                            m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(_tag.NodeId))));
                        }
                    }

                    foreach (var folder in rootfolders)
                    {
                        var instanceList = CreateFolderNodeList(folder, prototypeNode);
                        if (instanceList != null)
                        {
                            list.AddRange(instanceList);
                            m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(folder.NodeId))));
                        }
                    }

                    prototypeNode.References = m_references.ToArray();
                    propertyNodeList.AddRange(list);
                }

                typePrototypeNodeIds[prototypeName] = prototypeNode.NodeId;
                return typePrototypeNodeIds[prototypeName];
            }
            else return null;
        }

        private string GetAlarmPath(UFUAAlarmDefinition alarm)
        {
            if (alarm != null)
                return $"{alarm.SourcePath}{UFUAArea.AreaSeparator}{alarm.Name}";
            return null;
        }

        private string GetAlarmPath(UFUAArea area)
        {
            if (area == null)
                return null;
            if (area.UFUAAreaAss != null)
                return $"{GetAlarmPath(area.UFUAAreaAss)}{UFUAArea.AreaSeparator}{area.Name}";
            else
                return area.Name;
        }

        List<UANode> CreateAreaNodeList(UFUAModel.UFUAArea area, UANode parentNode)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            UAObject uANode = new UAObject();
            List<Reference> m_references = new List<Reference>();
            uANode.NodeId = Export(FromGuidToNodeId(area.NodeId));
            uANode.BrowseName = new QualifiedName(area.Name, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { area.Name });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;
            
            IList<UFUAArea> rootAreas = document.GetAlarmAreas(area); 
            IList<UFUAAlarmSource> rootSources = area.UFUAAlarmSources.ToList();


            if (rootAreas.Count > 0 || rootSources.Count > 0)
            {
                foreach (var _area in rootAreas)
                {
                    var instanceList = CreateAreaNodeList(_area, uANode);
                    if (instanceList != null)
                    {
                        propertyNodeList.AddRange(instanceList);
                        m_references.Add(CreateReference(ExportAlias(organizes), Export(FromGuidToNodeId(_area.NodeId))));
                        m_references.Add(CreateReference(ExportAlias(hasNotifier), Export(FromGuidToNodeId(_area.NodeId))));
                    }
                }
                foreach (var source in rootSources)
                {
                    var instanceList = CreateSourceNodeList(source, uANode);
                    if (instanceList != null)
                    {
                        propertyNodeList.AddRange(instanceList);
                        var nodeId = Export(FromGuidToNodeId(source.NodeId));
                        m_references.Add(CreateReference(ExportAlias(hasEventSource), nodeId));
                        var definitions = source.UFUAAlarmDefinitions.ToList();
                        definitions.ForEach(d => typeAlarmNodeIds[d.NodeId.ToString()] = nodeId);
                    }
                }
            }

            if (parentNode != null && !string.IsNullOrEmpty(parentNode.NodeId))
            {
                m_references.Add(CreateReference(ExportAlias(organizes), parentNode.NodeId.ToString(), false));
                uANode.ParentNodeId = parentNode.NodeId.ToString();
            }
            m_references.Add(CreateReference(ExportAlias(organizes), ExportAlias(server), false));
            m_references.Add(CreateReference(ExportAlias(organizes), ExportAlias(folderType)));
            uANode.References = m_references.ToArray();
            nodeList.Add(uANode);
            if (propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }
        
        private List<UANode> CreateSourceNodeList(UFUAModel.UFUAAlarmSource source, UANode parentNode)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            UAObject uANode = new UAObject();
            List<Reference> m_references = new List<Reference>();
            uANode.NodeId = Export(FromGuidToNodeId(source.NodeId));
            uANode.BrowseName = new QualifiedName(source.Name, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { source.Name });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;

            if (parentNode != null && !string.IsNullOrEmpty(parentNode.NodeId))
            {
                m_references.Add(CreateReference(ExportAlias(hasEventSource), parentNode.NodeId.ToString(), false));
                uANode.ParentNodeId = parentNode.NodeId.ToString();
            }
            uANode.References = m_references.ToArray();
            nodeList.Add(uANode);
            if (propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }

        private List<Reference> CreatePrototypeReferences(UFUATag ufuaTag, UANode parent, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            if (ufuaTag == null)
                return m_references;
            List<UANode> list = new List<UANode>();
            var taglist = document.GetTagMemberCollection(null, ufuaTag.PrototypeName, ufuaTag.GetFullName());
            var folderList = document.GetFolderPrototypeCollection(null, ufuaTag.PrototypeName, ufuaTag.GetFullName());
            foreach (var tag in taglist)
            {
                var instanceList = CreateTagNodeList(tag, parent,true);
                if (instanceList != null)
                {
                    list.AddRange(instanceList);
                    m_references.Add(CreateReference(ExportAlias(hasComponent), ConstructPropertiesId(FromGuidToNodeId(ufuaTag.NodeId).ToString(),tag.Name)));
                }
            }
            foreach (var folder in folderList)
            {
                var instanceList = CreateFolderNodeList(folder, parent, true);
                if (instanceList != null)
                {
                    list.AddRange(instanceList);
                    m_references.Add(CreateReference(ExportAlias(hasComponent), ConstructPropertiesId(FromGuidToNodeId(ufuaTag.NodeId).ToString(), folder.Name)));
                }
            }
            propertyNodeList.AddRange(list);
            return m_references;
        }

        private List<Reference> CreateEnumReferences(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            var orderedEnumStrings = (from c in enumStrings
                                      where c.UFUATag != null &&
                                      c.UFUATag.Oid == ufuaTag.Oid
                                      orderby c.Oid
                                      select c).ToList();

            object value = null;
            if (orderedEnumStrings.Count > 0)
            {
                Opc.Ua.LocalizedText[] strings = new Opc.Ua.LocalizedText[orderedEnumStrings.Count];
                for (int ii = 0; ii < orderedEnumStrings.Count; ii++)
                {
                    strings[ii] = new Opc.Ua.LocalizedText(orderedEnumStrings[ii].Data, string.Empty, orderedEnumStrings[ii].Data, null);
                }
                value = strings;
            }

            UAVariable valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.LocalizedText), Opc.Ua.BrowseNames.EnumStrings, value);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            return m_references;
        }

        private UAObject AddHistorical(string parentId, UFUAModel.UFUAHistorianSettings historianSettings, List<UANode> propertyNodeList)
        {
            string browsable = Opc.Ua.BrowseNames.HAConfiguration;
            UAObject variable = AddObject(parentId, browsable, ExportAlias(historicalDataConfigurationType));
            variable.SymbolicName = historianSettings.Name;
            List<Reference> m_references = new List<Reference>();
            m_references.AddRange(variable.References);
            m_references.AddRange(CreateHistoricalComponents(variable.NodeId, historianSettings, variable, propertyNodeList));
            m_references.AddRange(CreateHistoricalReferences(variable.NodeId, historianSettings, variable, propertyNodeList));
            variable.References = m_references.ToArray();
            return variable;
        }

        private UAObject AddObject(string parentId, string browsable, string typeDefinition)
        {
            UAObject variable = new UAObject();
            List<Reference> m_references = new List<Reference>();
            variable.NodeId = ConstructPropertiesId(parentId, browsable);
            variable.ParentNodeId = parentId;
            variable.BrowseName = browsable;
            variable.DisplayName = Export(new Opc.Ua.LocalizedText[] { browsable });
            variable.Description = null;
            variable.WriteMask = 0;
            variable.UserWriteMask = 0;
            if(!string.IsNullOrEmpty(typeDefinition))
                m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), typeDefinition));
            m_references.Add(CreateReference(ExportAlias(hasProperty), parentId, false));
            variable.References = m_references.ToArray();
            return variable;
        }

        private UAObject AddCondition(string parentId, string browsable)
        {
            UAObject variable = new UAObject();
            List<Reference> m_references = new List<Reference>();
            variable.NodeId = ConstructPropertiesId(parentId, browsable);
            variable.ParentNodeId = parentId;
            variable.BrowseName = browsable;
            variable.DisplayName = Export(new Opc.Ua.LocalizedText[] { browsable });
            variable.Description = null;
            variable.WriteMask = 0;
            variable.UserWriteMask = 0;
            m_references.Add(CreateReference(ExportAlias(hasCondition), parentId, false));
            variable.References = m_references.ToArray();
            return variable;
        }
        private IEnumerable<Reference> CreateHistoricalComponents(string parentId, UFUAModel.UFUAHistorianSettings historianSettings, UAObject variable, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            UAObject objectProp = null;
            //components
            objectProp = AddObject(parentId, Opc.Ua.BrowseNames.AggregateConfiguration, ExportAlias(BrowseNames.AggregateConfigurationType));
            if(objectProp != null)
            {
                List<Reference> o_references = new List<Reference>();
                o_references.AddRange(objectProp.References);
                UAVariable valueProp = null;
                //component properties
                valueProp = AddProperty(objectProp.NodeId, ExportAlias(DataTypeIds.Boolean), Opc.Ua.BrowseNames.TreatUncertainAsBad, historianSettings.TreatUncertainAsBad, null);
                valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                propertyNodeList.Add(valueProp);
                o_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

                valueProp = AddProperty(objectProp.NodeId, ExportAlias(DataTypeIds.Byte), Opc.Ua.BrowseNames.PercentDataBad, historianSettings.PercentDataBad, null);
                valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                propertyNodeList.Add(valueProp);
                o_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

                valueProp = AddProperty(objectProp.NodeId, ExportAlias(DataTypeIds.Byte), Opc.Ua.BrowseNames.PercentDataGood, historianSettings.PercentDataGood, null);
                valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                propertyNodeList.Add(valueProp);
                o_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

                valueProp = AddProperty(objectProp.NodeId, ExportAlias(DataTypeIds.Boolean), Opc.Ua.BrowseNames.UseSlopedExtrapolation, historianSettings.UseSlopedExtrapolation, null);
                valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                propertyNodeList.Add(valueProp);
                o_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));


                objectProp.References = o_references.ToArray();
            }

            propertyNodeList.Add(objectProp);
            m_references.Add(CreateReference(ExportAlias(hasComponent), objectProp.NodeId));

            objectProp = AddObject(parentId, Opc.Ua.BrowseNames.AggregateFunctions, ExportAlias(BrowseNames.AggregateFunctionType));
            propertyNodeList.Add(objectProp);
            m_references.Add(CreateReference(ExportAlias(hasComponent), objectProp.NodeId));

            return m_references;
        }
        private IEnumerable<Reference> CreateHistoricalReferences(string parentId, UFUAModel.UFUAHistorianSettings historianSettings, UAObject variable, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            UAVariable valueProp = null;
            //properties
            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Boolean), Opc.Ua.BrowseNames.Stepped, historianSettings.Stepped, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.String), Opc.Ua.BrowseNames.Definition, null, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Duration), Opc.Ua.BrowseNames.MaxTimeInterval, historianSettings.MaxTimeInterval.TotalMilliseconds, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Duration), Opc.Ua.BrowseNames.MinTimeInterval, historianSettings.MinTimeInterval.TotalMilliseconds, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Double), Opc.Ua.BrowseNames.ExceptionDeviation, historianSettings.ExceptionDeviation, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.ExceptionDeviationFormat), Opc.Ua.BrowseNames.ExceptionDeviationFormat, (ExceptionDeviationFormat)historianSettings.ExceptionDeviationFormat, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.UtcTime), Opc.Ua.BrowseNames.StartOfArchive, DateTime.MinValue, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.UtcTime), Opc.Ua.BrowseNames.StartOfOnlineArchive, DateTime.MinValue, null);
            valueProp.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            return m_references;
        }

        private List<Reference> CreateHistoricalConfiguration(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            UAObject valueProp = null;
            UFUAModel.UFUAHistorianSettings historianSettings = document.GetHistoricalSettings(ufuaTag.HistorianSettings, false);
            valueProp = AddHistorical(parentId, historianSettings, propertyNodeList);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasHistoricalConfiguration), valueProp.NodeId));
            return m_references;
        }

        private List<Reference> CreateAlarmCondition(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();

            ufuaTag.UFUAAlarmThresholds.ToList().ForEach(threshold =>
            {
                if (threshold.UFUAAlarmDefinitionRef == null)
                    return;
                UAObject valueProp = null;
                valueProp = AddCondition(threshold, propertyNodeList);
                propertyNodeList.Add(valueProp);
                m_references.Add(CreateReference(ExportAlias(hasCondition), valueProp.NodeId));
            });

            return m_references;
        }

        private string GetThresholdBrowsableName(UFUAModel.UFUAAlarmThreshold alarmThreshold)
        {
            var suffix = new System.Text.StringBuilder(alarmThreshold.UFUATagAss.GetRelativePath(NameSpaceIndex).Replace("/", "."));
            suffix.Append(alarmThreshold.GetFriendlyExpressionString());
            suffix.AppendFormat(":{0}", alarmThreshold.UFUAAlarmDefinitionRef.Name);
            return suffix.ToString();
        }

        private string GetThresholdName(UFUAModel.UFUAAlarmThreshold alarmThreshold)
        {
            var suffix = new System.Text.StringBuilder(alarmThreshold.UFUATagAss.GetFullName().Replace("/", "."));
            suffix.Append(alarmThreshold.GetFriendlyExpressionString());
            suffix.AppendFormat(":{0}", alarmThreshold.UFUAAlarmDefinitionRef.Name);
            return suffix.ToString();
        }

        private UAObject AddCondition(UFUAModel.UFUAAlarmThreshold alarmThreshold, List<UANode> propertyNodeList)
        {
            var alarmDefinition = alarmThreshold.UFUAAlarmDefinitionRef;
            var parentId = GetAlarm(alarmDefinition);
            string browsable = GetThresholdBrowsableName(alarmThreshold);
            string objectType = ObjectTypeIds.TripAlarmType.ToString(); 
            UAObject variable = AddCondition(parentId, browsable);
            variable.DisplayName = Export(new Opc.Ua.LocalizedText[] { GetThresholdName(alarmThreshold) });
            variable.Description = Export(new Opc.Ua.LocalizedText[] { alarmThreshold.AlarmText });
            List<Reference> m_references = new List<Reference>();
            m_references.AddRange(variable.References);
            UAVariable valueProp = null;
            var accesslevel = Opc.Ua.AccessLevels.CurrentRead;
            bool addExclusiveProp = false;
            switch ((AlarmType)alarmDefinition.AlarmType.Value)
            {
                case AlarmType.ExclusiveLevel:
                    objectType = ObjectTypeIds.ExclusiveLevelAlarmType.ToString();
                    addExclusiveProp = true;
                    break;
                case AlarmType.NonExclusiveLevel:
                    addExclusiveProp = true;
                    break;
                case AlarmType.ExclusiveDeviation:
                    objectType = ObjectTypeIds.ExclusiveDeviationAlarmType.ToString();
                    addExclusiveProp = true;
                    break;
                case AlarmType.NonExclusiveDeviation:
                    objectType = ObjectTypeIds.NonExclusiveDeviationAlarmType.ToString();
                    addExclusiveProp = true;
                    break;
                case AlarmType.ExclusiveRateOfChange:
                    objectType = ObjectTypeIds.ExclusiveRateOfChangeAlarmType.ToString();
                    addExclusiveProp = true;
                    break;
                case AlarmType.NonExclusiveRateOfChange:
                    objectType = ObjectTypeIds.NonExclusiveRateOfChangeAlarmType.ToString();
                    addExclusiveProp = true;
                    break;
                case AlarmType.TripAlarm:
                    objectType = ObjectTypeIds.TripAlarmType.ToString();
                    break;
                default:
                    break;
            }

            if(addExclusiveProp)
            {
                #region ExclusiveLevel
                if(alarmDefinition.EnableHighHighLimit)
                {
                    valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.Double),
                        Opc.Ua.BrowseNames.HighHighLimit, alarmDefinition.HighHighLimit.Value, null);
                    valueProp.AccessLevel = valueProp.UserAccessLevel = alarmDefinition.EnableHighHighLimit ? accesslevel : Opc.Ua.AccessLevels.None;
                    m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
                    propertyNodeList.Add(valueProp);
                }

                if (alarmDefinition.EnableHighLimit)
                {
                    valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.Double),
                     Opc.Ua.BrowseNames.HighLimit,alarmDefinition.HighLimit.Value, null);
                    valueProp.AccessLevel = valueProp.UserAccessLevel = alarmDefinition.EnableHighHighLimit ? accesslevel : Opc.Ua.AccessLevels.None;
                    m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
                    propertyNodeList.Add(valueProp);
                }

                if (alarmDefinition.EnableLowLimit)
                {
                    valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.Double),
                    Opc.Ua.BrowseNames.LowLimit,alarmDefinition.LowLimit.Value, null);
                    valueProp.AccessLevel = valueProp.UserAccessLevel = alarmDefinition.EnableHighHighLimit ? accesslevel : Opc.Ua.AccessLevels.None;
                    m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
                    propertyNodeList.Add(valueProp);
                }

                if (alarmDefinition.EnableLowLowLimit)
                {
                    valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.Double),
                     Opc.Ua.BrowseNames.LowLowLimit,alarmDefinition.LowLowLimit.Value, null);
                    valueProp.AccessLevel = valueProp.UserAccessLevel = alarmDefinition.EnableHighHighLimit ? accesslevel : Opc.Ua.AccessLevels.None;
                    m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
                    propertyNodeList.Add(valueProp);
                }
                #endregion
            }

            //component properties
            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.Int16), Opc.Ua.BrowseNames.Severity, alarmDefinition.Severity, null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.String), Opc.Ua.BrowseNames.SourceName, alarmDefinition.SourcePath, null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.NodeId), Opc.Ua.BrowseNames.SourceNode, parentId, null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.NodeId), Opc.Ua.BrowseNames.EventType, ExportAlias(objectType), null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.NodeId), Opc.Ua.BrowseNames.ConditionType, alarmThreshold.AlarmType, null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            valueProp = AddProperty(variable.NodeId, ExportAlias(DataTypeIds.NodeId), Opc.Ua.BrowseNames.ConditionName, alarmDefinition.Name, null);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            propertyNodeList.Add(valueProp);

            variable.References = m_references.ToArray();
            return variable;
        }

        string GetAlarm(UFUAModel.UFUAAlarmDefinition alarm)
        {
            if (alarm == null)
                return null;

            var prototypeName = alarm.NodeId.ToString();
            if (typeAlarmNodeIds.ContainsKey(prototypeName))
                return typeAlarmNodeIds[prototypeName];
            else
                return null;
        }

        private List<Reference> CreateDigitalReferences(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            var orderedEnumStrings = (from c in enumStrings
                                      where c.UFUATag != null &&
                                      c.UFUATag.Oid == ufuaTag.Oid
                                      orderby c.Oid
                                      select c).ToList();
            object value = null;
            UAVariable valueProp = null;

            if (orderedEnumStrings.Count >= 2)
                value = new Opc.Ua.LocalizedText(orderedEnumStrings[1].Data, string.Empty, orderedEnumStrings[1].Data);

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.LocalizedText), Opc.Ua.BrowseNames.TrueState, value);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            if (orderedEnumStrings.Count >= 2)
                value = new Opc.Ua.LocalizedText(orderedEnumStrings[0].Data, string.Empty, orderedEnumStrings[0].Data);

            valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.LocalizedText), Opc.Ua.BrowseNames.FalseState, value);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            return m_references;
        }

        private UAVariable AddProperty(string parentId, string dataType, string browsable, object value, string description = null)
        {
            UAVariable variable = new UAVariable();
            variable.NodeId = ConstructPropertiesId(parentId, browsable);
            variable.ParentNodeId = parentId;
            variable.BrowseName = browsable;
            variable.DisplayName = Export(new Opc.Ua.LocalizedText[] { browsable });
            variable.Description = !string.IsNullOrEmpty(description) ? Export(new Opc.Ua.LocalizedText[] { description }) : null;
            variable.DataType = dataType;
            variable.ValueRank = ValueRanks.Scalar;
            variable.ArrayDimensions = null;
            variable.WriteMask = 0;
            variable.UserWriteMask = 0;
            variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
            variable.Historizing = false;
            if(value != null)
                variable.Value = GetValue(value);

            List<Reference> m_references = new List<Reference>();
            m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), ExportAlias(propertyType)));
            m_references.Add(CreateReference(ExportAlias(hasProperty), parentId, false));
            variable.References = m_references.ToArray();
            return variable;
        }

        private static string ConstructPropertiesId(string parentNodeId, string symbolicName)
        {
            String parent = parentNodeId.Replace(";g=", ";s=");
            StringBuilder buffer = new StringBuilder();
            buffer.Append(parent);

            // check if the parent is another component.
            int index = parentNodeId.IndexOf('?');

            if (index < 0)
            {
                buffer.Append('?');
            }
            else
            {
                buffer.Append('/');
            }


            buffer.Append(symbolicName);
            return buffer.ToString();
        }
        
        //private List<Reference> CreateMethodReferences(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        //{
        //    List<Reference> m_references = new List<Reference>();
        //    UAVariable valueProp = null;
        //    string hasProperty = new NodeId(ReferenceTypes.HasProperty).ToString();
        //    if (!string.IsNullOrEmpty(ufuaTag.DynamicSettings) ||
        //        !string.IsNullOrEmpty(ufuaTag.Description))
        //    {
        //        string value = !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
        //        valueProp = AddProperty(parentId, DataTypeIds.String.ToString(), Opc.Ua.BrowseNames.Definition, value);
        //        propertyNodeList.Add(valueProp);
        //        m_references.Add(CreateReference(hasProperty, valueProp.NodeId));
        //    }

        //    return m_references;
        //}

        private List<Reference> CreateAnalogReferences(UFUATag ufuaTag,string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();
            UAVariable valueProp = null;

            if (ufuaTag.DataType == UFUAModel.DataType.Float ||
                                                ufuaTag.DataType == UFUAModel.DataType.Double)
            {
                valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Double), Opc.Ua.BrowseNames.ValuePrecision, 6d);
                propertyNodeList.Add(valueProp);
                m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            }
            if (!string.IsNullOrEmpty(ufuaTag.DynamicSettings) ||
                !string.IsNullOrEmpty(ufuaTag.Description))
            {
                string value = !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.String), Opc.Ua.BrowseNames.Definition, value);
                propertyNodeList.Add(valueProp);
                m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            }

            var engineeringUnit = document.GetEngineeringUnit(ufuaTag.UFUAEngineeringUnit, false);

            valueProp = AddEURange(parentId, engineeringUnit);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddInstrumentRange(parentId, engineeringUnit);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            valueProp = AddEngineeringUnits(parentId, engineeringUnit);
            propertyNodeList.Add(valueProp);
            m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));

            return m_references;
        }

        private List<Reference> CreateVariableReferences(UFUATag ufuaTag, string parentId, List<UANode> propertyNodeList)
        {
            List<Reference> m_references = new List<Reference>();

            if (ufuaTag.DataType == UFUAModel.DataType.Float ||
                                                ufuaTag.DataType == UFUAModel.DataType.Double)
            {
                UAVariable valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.Double), Opc.Ua.BrowseNames.ValuePrecision, 6d);
                propertyNodeList.Add(valueProp);
                m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            }
            if (!string.IsNullOrEmpty(ufuaTag.DynamicSettings) ||
                !string.IsNullOrEmpty(ufuaTag.Description))
            {
                string value = !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                UAVariable valueProp = AddProperty(parentId, ExportAlias(DataTypeIds.String), Opc.Ua.BrowseNames.Definition, value);
                propertyNodeList.Add(valueProp);
                m_references.Add(CreateReference(ExportAlias(hasProperty), valueProp.NodeId));
            }

            return m_references;
        }

        private Reference CreateReference(string referenceType, string value, bool isForward = true)
        {
            var _ref  = new Reference() { ReferenceType = referenceType, Value = value};
            if (!isForward)
                _ref.IsForward = false;
            return _ref;
        }

        private XmlElement GetValue(object value)
        {
            try
            {

                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    CloseOutput = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Indent = true
                 };
                ServiceMessageContext messageContext = new ServiceMessageContext();
                messageContext.NamespaceUris = NameSpaceTable;

                StringBuilder destination = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(destination, settings))
                {
                    using (XmlEncoder encoder = new XmlEncoder(this.GetType(), writer, messageContext))
                    {
                        NamespaceTable namespaceUris = new NamespaceTable();

                        if (NamespaceUris != null)
                        {
                            for (int ii = 0; ii < NamespaceUris.Length; ii++)
                            {
                                namespaceUris.Append(NamespaceUris[ii]);
                            }
                        }

                        StringTable serverUris = new StringTable();

                        if (ServerUris != null)
                        {
                            for (int ii = 0; ii < ServerUris.Length; ii++)
                            {
                                serverUris.Append(ServerUris[ii]);
                            }
                        }
                        encoder.SetMappingTables(namespaceUris, serverUris);
                        Variant variant = new Variant(value);
                        encoder.WriteVariantContents(variant.Value, variant.TypeInfo);
                    }
                }

                XmlDocument document = new XmlDocument();
                document.InnerXml = destination.ToString();
                return document.DocumentElement.FirstChild as XmlElement;
            }
            catch (Exception ex)
            {
                ErrorMessages.Add(ex.Message);
                return null;
            }
        }

        private UAVariable AddEURange(string parentId, UFUAEngineeringUnit engineeringUnit)
        {
            object value = null;
            string desc = null;
            if (engineeringUnit != null)
            {
                value = new Range(engineeringUnit.EURangeHigh.Value, engineeringUnit.EURangeLow.Value);
                desc = engineeringUnit.Name;
            }
            return AddProperty(parentId, ExportAlias(DataTypeIds.Range), Opc.Ua.BrowseNames.EURange, value, desc);
        }

        private UAVariable AddEngineeringUnits(string parentId, UFUAEngineeringUnit engineeringUnit)
        {
            object value = null;
            string desc = null;
            if (engineeringUnit != null)
            {
                EUInformation info;
                if (!String.IsNullOrEmpty(engineeringUnit.UnitName))
                {
                    info = new EUInformation
                    {
                        UnitId = 0,
                        DisplayName = engineeringUnit.UnitName,
                        NamespaceUri = ServerUri
                    };
                }
                else
                {
                    info = new EUInformation
                    {
                        UnitId = -1
                    };
                }
                value = info;
                desc = engineeringUnit.Name;
            }
            return AddProperty(parentId, ExportAlias(DataTypeIds.EUInformation), Opc.Ua.BrowseNames.EngineeringUnits, value, desc);
        }

        private UAVariable AddInstrumentRange(string parentId, UFUAEngineeringUnit engineeringUnit)
        {
            object value = null;
            string desc = null;
            if (engineeringUnit != null)
            {
                value = new Range(engineeringUnit.InstrumentRangeHigh.Value, engineeringUnit.InstrumentRangeLow.Value);
                desc = engineeringUnit.Name;
            }
            return AddProperty(parentId, ExportAlias(DataTypeIds.Range), Opc.Ua.BrowseNames.InstrumentRange, value, desc);
        }
        
        private List<UANode> CreateFolderNodeList(UFUAModel.UFUAFolder ufuaFolder, UANode parentNode = null, 
                                                bool isPrototypeMember = false,string prototypeName = null, string tagOwnerPath = null)
        {
            List<UANode> nodeList = new List<UANode>();
            List<UANode> propertyNodeList = new List<UANode>();
            UAObject uANode = new UAObject();
            List<Reference> m_references = new List<Reference>(); 
            uANode.NodeId = Export(FromGuidToNodeId(ufuaFolder.NodeId));
            uANode.BrowseName = new QualifiedName(ufuaFolder.Name, NameSpaceIndex).ToString();
            uANode.DisplayName = Export(new Opc.Ua.LocalizedText[] { ufuaFolder.Name });
            uANode.WriteMask = 0;
            uANode.UserWriteMask = 0;

            IList<UFUATag> rootTags = new List<UFUATag>();
            IList<UFUAFolder> rootFolders = new List<UFUAFolder>();
            if(!isPrototypeMember)
            {
                rootFolders = document.GetFolderCollection(ufuaFolder);
                rootTags = document.GetTagCollection(ufuaFolder);
            }
            else
            {
                rootFolders = document.GetFolderPrototypeCollection(ufuaFolder, prototypeName, tagOwnerPath);
                rootTags = document.GetTagMemberCollection(ufuaFolder, prototypeName, tagOwnerPath);
            }

            if (rootFolders.Count > 0 || rootTags.Count > 0)
            {
                foreach (var tag in rootTags)
                {
                    var instanceList = CreateTagNodeList(tag, uANode, isPrototypeMember);
                    if (instanceList != null)
                    {
                        propertyNodeList.AddRange(instanceList);
                        m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(tag.NodeId))));
                    }
                }
                foreach (var folder in rootFolders)
                {
                    var instanceList = CreateFolderNodeList(folder, uANode, isPrototypeMember);
                    if (instanceList != null)
                    {
                        propertyNodeList.AddRange(instanceList);
                        m_references.Add(CreateReference(ExportAlias(hasComponent), Export(FromGuidToNodeId(folder.NodeId))));
                    }
                }
            }

            if (parentNode != null && !string.IsNullOrEmpty(parentNode.NodeId))
            {
                m_references.Add(CreateReference(ExportAlias(hasComponent), parentNode.NodeId.ToString(), false));
                uANode.ParentNodeId = parentNode.NodeId.ToString();
            }
            m_references.Add(CreateReference(ExportAlias(hasTypeDefinition), ExportAlias(folderType)));
            uANode.References = m_references.ToArray();
            nodeList.Add(uANode);
            if (propertyNodeList.Count > 0)
                nodeList.AddRange(propertyNodeList);

            return nodeList;
        }

        private void Export(UANode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            // add node to list.
            UANode[] nodes = null;

            int count = 1;

            if (this.Items == null)
            {
                nodes = new UANode[count];
            }
            else
            {
                count += this.Items.Length;
                nodes = new UANode[count];
                Array.Copy(this.Items, nodes, this.Items.Length);
            }

            nodes[count - 1] = node;
            this.Items = nodes;
        }

        private void CreateMethod(UAMethod node, UFUAModel.UFUAFolder root = null, UFUAModel.UFUATagPrototype prototypeModel = null)
        {
            UFUAModel.UFUATag tag = CheckExistingTag(node, root, prototypeModel);
            if (dialogRetValue == CustomDialogResults.Cancel || tag == null)
                return;

            tag.ModelType = ModelType.Method;
            tag.Description = node.Description?.FirstOrDefault()?.Value;
            if (root == null)
                tag.UFUATagPrototype = prototypeModel;
            document.EnsureValidNodeId(tag);
        }

        private void CreateObject(UAObject node, UFUAModel.UFUAFolder root = null, UFUAModel.UFUATagPrototype prototypeModel = null)
        {
            var refType = (from Reference reference in node.References
                           where reference.ReferenceType == ExportAlias(hasTypeDefinition)
                           select reference.Value).FirstOrDefault();
                                                      
                                                    //For the address space point of view, Base Object is equal to a folder
            if (refType == ExportAlias(folderType) || refType == ExportAlias(baseObjectType))
            {
                UFUAModel.UFUAFolder folder = CreateFolder(node, root, prototypeModel);

                List<UAMethod> rootUAMethods = GetRootObjects<UAMethod>(node.NodeId);
                List<UAObject> rootObjects = GetRootObjects<UAObject>(node.NodeId);
                List<UAVariable> rootTags = GetRootObjects<UAVariable>(node.NodeId);

                foreach (var obj in rootObjects)
                {
                    CreateObject(obj, folder, prototypeModel);
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        break;
                }
                foreach (var obj in rootTags)
                {
                    CreateTag(obj, folder, prototypeModel);
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        break;
                }
                foreach (var obj in rootUAMethods)
                {
                    CreateMethod(obj, folder, prototypeModel);
                    if (dialogRetValue == CustomDialogResults.Cancel)
                        break;
                }
            }
            else
                CreateTag(node, root, prototypeModel);
        }

        private UFUAModel.UFUAFolder CreateFolder(UAObject node, UFUAModel.UFUAFolder root = null, UFUAModel.UFUATagPrototype prototypeModel = null)
        {
            string folderName = UFUAModel.Helpers.NameValidator.EnsureValidName(node.DisplayName?.FirstOrDefault()?.Value);
            UFUAModel.UFUAFolder folder = null;
            if (prototypeModel != null)
            {
                folder = document.AddNewFolder(root);
                addedObjects.Add(folder);
            }
            else
            {
                folder = document.GetFolder(folderName, root);
                if (folder == null)
                {
                    folder = document.AddNewFolder(root);
                    addedObjects.Add(folder);
                }
            }

            folder.NodeIdToPublish = bCollectNodeIds ? node.NodeId : null;
            folder.Name = folderName;
            if (root == null)
                folder.UFUATagPrototype = prototypeModel;
            return folder;
        }

        private void CreateTag(UAObject node, UFUAModel.UFUAFolder root = null, UFUAModel.UFUATagPrototype prototypeModel = null)
        {
            if (dialogRetValue == CustomDialogResults.Cancel)
                return;

            var refType = (from Reference reference in node.References
                           where reference.ReferenceType == ExportAlias(hasTypeDefinition)
                           select reference.Value).FirstOrDefault();
            UAObjectType typedef = (from UANode p in Items
                                    where p is UAObjectType
                                    select p as UAObjectType).AsParallel().Where(p =>
                                    p.NodeId == refType).FirstOrDefault();
            if (typedef == null)
                return;

            UFUAModel.UFUATagPrototype prototype = CreatePrototype(typedef);
            if (prototype != null)
            {
                UFUAModel.UFUATag tag = CheckExistingTag(node, root);
                if (dialogRetValue == CustomDialogResults.Cancel || tag == null)
                    return;

                tag.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(node.DisplayName?.FirstOrDefault()?.Value);
                tag.NodeIdToPublish = bCollectNodeIds ? node.NodeId : null;
                tag.ModelType = ModelType.ObjectType;
                tag.PrototypeModel = prototype.Name;
                if (root == null)
                    tag.UFUATagPrototype = prototypeModel;
                document.EnsureValidNodeId(tag);
                document.CreateSubPrototype(tag);
            }
        }

        private UFUAModel.AccessLevels? GetAccessLevel(UAVariable node)
        {
            if (node.AccessLevel == Opc.Ua.AccessLevels.None)
                return UFUAModel.AccessLevels.None;
            else if (node.AccessLevel == Opc.Ua.AccessLevels.CurrentRead)
                return UFUAModel.AccessLevels.CurrentRead;
            else if (node.AccessLevel == Opc.Ua.AccessLevels.CurrentWrite)
                return UFUAModel.AccessLevels.CurrentWrite;
            else if (node.AccessLevel == Opc.Ua.AccessLevels.CurrentReadOrWrite)
                return UFUAModel.AccessLevels.CurrentReadOrWrite;
            else
                return UFUAModel.AccessLevels.None;
        }

        private UFUAHistorianSettings CreateHistorian(UAObject typedef)
        {
            string typedefName = typedef.SymbolicName;
            if (string.IsNullOrEmpty(typedefName))
                typedefName = document.NewHistoricalSettingsName(null, null, listHistoricalNames, "{0}_{1}");
            string historianName = UFUAModel.Helpers.NameValidator.EnsureValidName(typedefName);
            if (typeHistorianDefinitions.ContainsKey(historianName))
                return typeHistorianDefinitions[historianName];

            var historianprototype = document.GetHistoricalSettings(historianName);
            if (historianprototype != null)
            {
                if (dialogRetValue != CustomDialogResults.YesAll &&
                    dialogRetValue != CustomDialogResults.NoAll &&
                    dialogRetValue != CustomDialogResults.Cancel)
                {
                    if (uiMsgBox != null)
                        dialogRetValue = uiMsgBox.ShowYesNoAllCancel(string.Format(Properties.Resources.ImportPrototypeNameExist, typedefName), CustomDialogIcons.Question);
                }

                if (dialogRetValue == CustomDialogResults.Cancel)
                    return null;
            }

            if (historianprototype == null)
            {
                historianprototype = document.AddNewHistoricalSettings();
                addedObjects.Add(historianprototype);
            }
            else if (dialogRetValue == CustomDialogResults.No || dialogRetValue == CustomDialogResults.NoAll)
            {
                if (typeHistorianDefinitions != null)
                    typeHistorianDefinitions[typedefName] = historianprototype;

                return historianprototype;
            }
            else if (dialogRetValue == CustomDialogResults.Yes || dialogRetValue == CustomDialogResults.YesAll)
            {
                historianprototype = document.AddNewHistoricalSettings();
                addedObjects.Add(historianprototype);
                historianName = document.NewHistoricalSettingsName(historianName, null, listPrototypeNames, "{0}_{1}");
            }

            if (historianName != typedefName)
                logGeneral.Info(string.Format(Properties.Resources.ImportedHistoricalNameChanged, $"{typedefName}", $"{historianName}"));

            historianprototype.Name = historianName;

            if (typeHistorianDefinitions != null)
                typeHistorianDefinitions[typedefName] = historianprototype;

            UpdateHistorianSettings(historianprototype, typedef);

            return historianprototype;
        }

        private void UpdateHistorianSettings(UFUAHistorianSettings historianSettings, UAObject node)
        {
            if (historianSettings == null)
                return;

            var tagList = GetRootObjects<UAVariable>(node.NodeId);

            var Stepped = (from UAVariable v in tagList
                           where v.BrowseName.ToString() == Opc.Ua.BrowseNames.Stepped &&
                           v.DataType == BrowseNames.Boolean.ToString()
                              select v).FirstOrDefault();

            var Definition = (from UAVariable v in tagList
                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.Definition &&
                              v.DataType == BrowseNames.String.ToString()
                              select v).FirstOrDefault();

            var MaxTimeInterval = (from UAVariable v in tagList
                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.MaxTimeInterval &&
                              v.DataType == BrowseNames.Duration.ToString()
                              select v).FirstOrDefault();

            var MinTimeInterval = (from UAVariable v in tagList
                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.MinTimeInterval &&
                              v.DataType == BrowseNames.Duration.ToString()
                              select v).FirstOrDefault();

            var ExceptionDeviation = (from UAVariable v in tagList
                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.ExceptionDeviation &&
                              v.DataType == BrowseNames.Double.ToString()
                              select v).FirstOrDefault();

            var ExceptionDeviationFormat = (from UAVariable v in tagList
                                      where v.BrowseName.ToString() == Opc.Ua.BrowseNames.ExceptionDeviationFormat &&
                                      v.DataType == BrowseNames.ExceptionDeviationFormat.ToString()
                                      select v).FirstOrDefault();

            var StartOfArchive = (from UAVariable v in tagList
                                      where v.BrowseName.ToString() == Opc.Ua.BrowseNames.StartOfArchive &&
                                      v.DataType == BrowseNames.UtcTime.ToString()
                                      select v).FirstOrDefault();

            var StartOfOnlineArchive = (from UAVariable v in tagList
                                      where v.BrowseName.ToString() == Opc.Ua.BrowseNames.StartOfOnlineArchive &&
                                      v.DataType == BrowseNames.UtcTime.ToString()
                                      select v).FirstOrDefault();

            var objList = GetRootObjects<UAObject>(node.NodeId);
            var AggregateConfiguration = (from UAObject v in objList
                                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.AggregateConfiguration
                                              select v).FirstOrDefault();
            object value = null;
            if(Stepped != null)
            {
                value = GetXMLValue(Stepped.Value);
                if(value != null)
                    historianSettings.Stepped = (bool)value;
            }
            if (MaxTimeInterval != null)
            {
                value = GetXMLValue(MaxTimeInterval.Value);
                if (value != null)
                    historianSettings.MaxTimeInterval = TimeSpan.FromMilliseconds((double)value);
            }
            if (MinTimeInterval != null)
            {
                value = GetXMLValue(MinTimeInterval.Value);
                if (value != null)
                    historianSettings.MinTimeInterval = TimeSpan.FromMilliseconds((double)value);
            }
            if (ExceptionDeviation != null)
            {
                value = GetXMLValue(ExceptionDeviation.Value);
                if (value != null)
                    historianSettings.ExceptionDeviation = (double)value;
            }
            if (ExceptionDeviationFormat != null)
            {
                value = GetXMLValue(ExceptionDeviationFormat.Value);
                if (value != null)
                    historianSettings.ExceptionDeviationFormat = (DeviationType)value;
            }

            if (AggregateConfiguration != null)
            {
                tagList = GetRootObjects<UAVariable>(AggregateConfiguration.NodeId);

                var TreatUncertainAsBad = (from UAVariable v in tagList
                                           where v.BrowseName.ToString() == Opc.Ua.BrowseNames.TreatUncertainAsBad &&
                                           v.DataType == BrowseNames.Boolean.ToString()
                                           select v).FirstOrDefault();
                var PercentDataBad = (from UAVariable v in tagList
                                      where v.BrowseName.ToString() == Opc.Ua.BrowseNames.PercentDataBad &&
                                      v.DataType == BrowseNames.Byte.ToString()
                                      select v).FirstOrDefault();
                var PercentDataGood = (from UAVariable v in tagList
                                       where v.BrowseName.ToString() == Opc.Ua.BrowseNames.PercentDataGood &&
                                       v.DataType == BrowseNames.Byte.ToString()
                                       select v).FirstOrDefault();
                var UseSlopedExtrapolation = (from UAVariable v in tagList
                                              where v.BrowseName.ToString() == Opc.Ua.BrowseNames.UseSlopedExtrapolation &&
                                              v.DataType == BrowseNames.Boolean.ToString()
                                              select v).FirstOrDefault();


                if (TreatUncertainAsBad != null)
                {
                    value = GetXMLValue(TreatUncertainAsBad.Value);
                    if (value != null)
                        historianSettings.TreatUncertainAsBad = (bool)value;
                }
                if (PercentDataBad != null)
                {
                    value = GetXMLValue(PercentDataBad.Value);
                    if (value != null)
                        historianSettings.PercentDataBad = (Byte)value;
                }
                if (PercentDataGood != null)
                {
                    value = GetXMLValue(PercentDataGood.Value);
                    if (value != null)
                        historianSettings.PercentDataGood = (Byte)value;
                }
                if (UseSlopedExtrapolation != null)
                {
                    value = GetXMLValue(UseSlopedExtrapolation.Value);
                    if (value != null)
                        historianSettings.UseSlopedExtrapolation = (bool)value;
                }
            }
        }

        private UFUATagPrototype CreatePrototype(UAObjectType typedef)
        {
            string typedefName = typedef.DisplayName?.FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(typedefName))
                typedefName = document.NewImportedPrototypeName(null, null, listPrototypeNames, "{0}_{1}");
            string prototypeName = UFUAModel.Helpers.NameValidator.EnsureValidName(typedefName);
            if (typePrototypeDefinitions.ContainsKey(prototypeName))
                return typePrototypeDefinitions[prototypeName];

            var prototype = document.GetPrototype(prototypeName);
            if(prototype != null)
            {
                if (dialogRetValue != CustomDialogResults.YesAll &&
                    dialogRetValue != CustomDialogResults.NoAll &&
                    dialogRetValue != CustomDialogResults.Cancel)
                {
                    if (uiMsgBox != null)
                        dialogRetValue = uiMsgBox.ShowYesNoAllCancel(string.Format(Properties.Resources.ImportPrototypeNameExist, typedefName), CustomDialogIcons.Question);
                }

                if (dialogRetValue == CustomDialogResults.Cancel)
                    return null;
            }

            if (prototype == null)
            {
                prototype = document.AddNewPrototype();
                addedObjects.Add(prototype);
            }
            else if (dialogRetValue == CustomDialogResults.No || dialogRetValue == CustomDialogResults.NoAll)
            {
                if (typePrototypeDefinitions != null)
                    typePrototypeDefinitions[typedefName] = prototype;

                return prototype;
            }
            else if (dialogRetValue == CustomDialogResults.Yes || dialogRetValue == CustomDialogResults.YesAll)
            {
                prototype = document.AddNewPrototype();
                addedObjects.Add(prototype);
                prototypeName = document.NewImportedPrototypeName(prototypeName, null, listPrototypeNames, "{0}_{1}");
            }

            if (prototypeName != typedefName)
                logGeneral.Info(string.Format(Properties.Resources.ImportedPrototypeNameChanged, $"{typedefName}", $"{prototypeName}"));

            prototype.Name = prototypeName;
            prototype.NodeIdToPublish = bCollectNodeIds ? typedef.NodeId : null; 
            document.EnsureValidNodeId(prototype);

            if (typePrototypeDefinitions != null)
                typePrototypeDefinitions[typedefName] = prototype;

            CreatePrototypeMembersAndFolders(prototype, typedef);

            return prototype;
        }

        private void CreatePrototypeMembersAndFolders(UFUAModel.UFUATagPrototype tag, UAObjectType typedef)
        {
            List<UAMethod> rootUAMethods = GetRootObjects<UAMethod>(typedef.NodeId);
            List<UAObject> rootObjects = GetRootObjects<UAObject>(typedef.NodeId);
            List<UAVariable> rootTags = GetRootObjects<UAVariable>(typedef.NodeId);

            foreach (var obj in rootObjects)
            {
                CreateObject(obj, null, tag);
            }
            foreach (var obj in rootTags)
            {
                CreateTag(obj, null, tag);
            }
            foreach (var obj in rootUAMethods)
            {
                CreateMethod(obj, null, tag);
            }
        }

        private void CreateArea(UAObject node, UFUAModel.UFUAArea root = null)
        {
            UFUAModel.UFUAArea tag = CheckExistingArea(node, root);
            if (dialogRetValue == CustomDialogResults.Cancel || tag == null)
                return;

            var refAreas = (from Reference reference in node.References
                           where reference.ReferenceType == ExportAlias(hasNotifier)
                           select reference.Value).ToList();

            var refSources = (from Reference reference in node.References
                            where reference.ReferenceType == ExportAlias(hasEventSource)
                            select reference.Value).ToList();

            refAreas?.ForEach(area =>
            {
                UAObject alarmArea = GetObject<UAObject>(area);
                CreateArea(alarmArea, tag);
            });

            refSources?.ForEach(source =>
            {
                UAObject alarmSource = GetObject<UAObject>(source);
                CheckExistingAlarmSource(alarmSource, tag);
            });
        }

        private UFUAAlarmSource CheckExistingAlarmSource(UANode node, UFUAArea root = null)
        {
            if (node == null || string.IsNullOrEmpty(node.DisplayName?.FirstOrDefault()?.Value.ToString()))
                return null;
            UFUAAlarmSource source = null;
            string sourceName = string.Empty;
            string nodeName = node.DisplayName?.FirstOrDefault()?.Value.ToString();
            sourceName = UFUAModel.Helpers.NameValidator.EnsureValidName(nodeName);
            if (string.IsNullOrEmpty(sourceName))
                sourceName = document.NewAlarmSourceName(root);
            string rootName = UFUAServerInfo.UFUAServerInfo.GetAlarmRootName();
            string rootPath = root != null ? $"{rootName}/{root.GetRelativeName()}" : rootName;
            string objectFullName = $"{rootPath}/{nodeName}";
            List<string> listNames = new List<string>();

            if (!existingAlarmSourceObject.ContainsKey(rootPath))
            {
                existingAlarmSourceObject[rootPath] = document.GetSourceCollection(root).ToList();
            }

            listNames = (from o in existingAlarmSourceObject[rootPath] select o.Name).ToList();
            if (listNames != null && listNames.Contains(sourceName))
            {
                source = (from e in existingAlarmSourceObject[rootPath] where e.Name == nodeName select e).FirstOrDefault();
            }
            else
            {
                source = document.AddNewAlarmSource(root, sourceName);
                addedObjects.Add(source);
            }

            if (source != null)
            {
                if (!existingAlarmSourceObject[rootPath].Contains(source))
                    existingAlarmSourceObject[rootPath].Add(source);
                if (!existingAlarmSourceObjects.ContainsKey(objectFullName))
                    existingAlarmSourceObjects.Add(objectFullName, source);
            }

            return source;
        }
        private void CreateTag(UAVariable node, UFUAModel.UFUAFolder root = null, UFUAModel.UFUATagPrototype prototypeModel = null)
        {
            UFUAModel.UFUATag tag = CheckExistingTag(node, root, prototypeModel);
            if (dialogRetValue == CustomDialogResults.Cancel || tag == null)
                return;

            var refType = (from Reference reference in node.References
                           where reference.ReferenceType == ExportAlias(hasTypeDefinition)
                           select reference.Value).FirstOrDefault();

            var historicalId = (from Reference reference in node.References
                           where reference.ReferenceType == ExportAlias(hasHistoricalConfiguration)
                           select reference.Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(historicalId))
            {
                UAObject historian = GetObject<UAObject>(historicalId);
                if(historian != null)
                {
                    UFUAModel.UFUAHistorianSettings historianPrototype = CreateHistorian(historian);
                    string historicalName = historianPrototype?.Name;
                    if (!string.IsNullOrEmpty(historicalName))
                        tag.HistorianSettings = historicalName;
                }
            }

            if (computedVariableType.Contains(refType))
                tag.ModelType = ModelType.Variable;
            else
                tag.ModelType = GetModelType(refType);

            if(tag.ModelType == ModelType.Enumerated)
            {
                List<UAVariable> rootTags = GetRootObjects<UAVariable>(node.NodeId);
                var enums = (from UAVariable variable in rootTags
                             where variable.DataType == BrowseNames.LocalizedText.ToString()
                             select variable.Value).FirstOrDefault();
                Opc.Ua.LocalizedText[] strings = GetXMLValue(enums) as Opc.Ua.LocalizedText[];
                if(strings != null)
                    for (int ii = 0; ii < strings.Count(); ii++)
                    {
                        tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = strings[ii].Text });
                    }
            }
            else if (tag.ModelType == ModelType.Digital)
            {
                List<UAVariable> rootTags = GetRootObjects<UAVariable>(node.NodeId);
                var digital = (from UAVariable variable in rootTags
                                 where variable.DataType == BrowseNames.LocalizedText.ToString()
                                 && variable.BrowseName == falseState
                                 select variable.Value).FirstOrDefault();
                Opc.Ua.LocalizedText strings = GetXMLValue(digital) as Opc.Ua.LocalizedText;
                if (strings != null)
                    tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = strings.Text });


                digital = (from UAVariable variable in rootTags
                           where variable.DataType == BrowseNames.LocalizedText.ToString()
                           && variable.BrowseName == trueState
                           select variable.Value).FirstOrDefault();
                strings = GetXMLValue(digital) as Opc.Ua.LocalizedText;
                if (strings != null)
                    tag.EnumStrings.Add(new UFUAModel.UFUAEnumString(tag.Session) { Data = strings.Text });
            }
            else if(tag.ModelType == ModelType.ObjectType && IsBaseVariableTypeOf(refType))
            {
                tag.ModelType = ModelType.Variable;
                computedVariableType.Add(refType);
            }

            tag.DataType = GetDataType(node.DataType);
            tag.AccessLevel = GetAccessLevel(node);
            tag.Description = node.Description?.FirstOrDefault()?.Value;
            int dim;
            int.TryParse(node.ArrayDimensions,out dim);
            tag.ArrayDimension = (uint)dim;
            if (root == null)
                tag.UFUATagPrototype = prototypeModel;

            var value = GetXMLValue(node.InitialValue);
            BuiltInType builtinType = BuiltInType.Null;
            NodeId dataTypeId = null;
            GetDataType(tag.DataType, out builtinType, out dataTypeId);
            if (tag.ArrayDimension == 0)
            {
                tag.InitialValue = value?.ToString();
            }
            else if(value is Array && (value as Array).Length > 0)
            {
                var newValue = GetArrayValue(value as Array, dim);
                tag.InitialValue = newValue;
            }

            UpdateRanges(tag, node);
            UpdateThresholds(tag, node);
            document.EnsureValidNodeId(tag);
        }

        bool IsBaseVariableTypeOf(string refType)
        {
            var varType = (from UANode refNode in Items
                           where refNode is UAVariableType &&
                           (refNode as UAVariableType).NodeId == refType
                           select refNode as UAVariableType).FirstOrDefault();
            return varType != null;
        }

        string GetArrayValue(Array value, int arrayDimension)
        {
            StringBuilder stringBuilder = new StringBuilder("{");
            for (int i = 0; i < value.Length && i < arrayDimension; i++)
            {
                stringBuilder.Append($"{value.GetValue(i)} |");
            }
            return $"{stringBuilder.ToString().Substring(0, stringBuilder.Length - 2)}" + "}";
        }

        private string GetAlarmPath(UFUAAlarmThreshold alarm)
        {
            if (alarm.UFUAAlarmDefinitionRef != null)
                return $"{alarm.UFUAAlarmDefinitionRef.SourcePath}{UFUAArea.AreaSeparator}{alarm.UFUAAlarmDefinitionRef.Name}{UFUAArea.AreaSeparator}{alarm.AlarmText}";
            else
                return alarm.AlarmText;
        }

        AlarmType GetAlarmType(string alarmType)
        {
            switch (alarmType)
            {
                case BrowseNames.ExclusiveLevelAlarmType:
                    return AlarmType.ExclusiveLevel;
                case BrowseNames.NonExclusiveLevelAlarmType:
                    return AlarmType.NonExclusiveLevel;
                case BrowseNames.ExclusiveDeviationAlarmType:
                    return AlarmType.ExclusiveDeviation;
                case BrowseNames.NonExclusiveDeviationAlarmType:
                    return AlarmType.NonExclusiveDeviation;
                case BrowseNames.ExclusiveRateOfChangeAlarmType:
                    return AlarmType.ExclusiveRateOfChange;
                case BrowseNames.NonExclusiveRateOfChangeAlarmType:
                    return AlarmType.NonExclusiveRateOfChange;
                case BrowseNames.TripAlarmType:
                    return AlarmType.TripAlarm;
                default:
                    return AlarmType.TripAlarm;
                    break;
            }
        }
        double GetDoubleValue(UAVariable variable, double defaultValue)
        {
            try
            {
                return (double)GetXMLValue(variable.Value);
            }
            catch
            {
               return defaultValue;
            }
        }

        private void UpdateThresholds(UFUATag tag, UAVariable node)
        {
            Dictionary<string, UFUAModel.UFUAAlarmThreshold> thresholdList = new Dictionary<string, UFUAAlarmThreshold>();
            tag.UFUAAlarmThresholds?.ToList().ForEach(alarm =>
            {
                thresholdList.Add(GetAlarmPath(alarm), alarm);
            });

            var tagReferences = (from Reference reference in (node as UANode).References
                                   where reference.ReferenceType == ExportAlias(hasCondition)
                                   && reference.IsForward == true
                                   select reference.Value).ToList();
            tagReferences.ForEach(tagRef =>
            {
                var condition = GetObject<UAObject>(tagRef);
                if(condition != null)
                {
                    var condReferences = (from Reference reference in (condition as UANode).References
                                         where reference.ReferenceType == ExportAlias(hasProperty)
                                         && reference.IsForward == true
                                         select reference.Value).ToList();

                    UAVariable ConditionType = GetObject<UAVariable>(condReferences, BrowseNames.ConditionType);
                    UAVariable EventType = GetObject<UAVariable>(condReferences, BrowseNames.EventType);
                    UAVariable DefinitionName = GetObject<UAVariable>(condReferences, BrowseNames.ConditionName);
                    UAVariable SourceName = GetObject<UAVariable>(condReferences, BrowseNames.SourceName);
                    UAVariable Severity = GetObject<UAVariable>(condReferences, BrowseNames.Severity);

                    if (EventType == null || ConditionType == null || DefinitionName == null || SourceName == null || Severity == null)
                    {
                        ErrorMessages.Add(string.Format(Properties.Resources.ImportError, condition.DisplayName, Properties.Resources.ThresholdDetailsNotSufficient));
                        return;
                    }

                    AlarmType alarmType = GetAlarmType((string)GetXMLValue(EventType.Value));
                    ConditionType conditionType = (ConditionType)GetXMLValue(ConditionType.Value);
                    UAVariable HighHighLimit = GetObject<UAVariable>(condReferences, BrowseNames.HighHighLimit);
                    UAVariable HighLimit = GetObject<UAVariable>(condReferences, BrowseNames.HighLimit);
                    UAVariable LowLimit = GetObject<UAVariable>(condReferences, BrowseNames.LowLimit);
                    UAVariable LowLowLimit = GetObject<UAVariable>(condReferences, BrowseNames.LowLowLimit);

                    UFUAAlarmThreshold alarmthreshold = null;
                    try
                    {
                        string alarmText = condition.Description?.FirstOrDefault()?.Value;
                        string sourcePath = (string)GetXMLValue(SourceName.Value);
                        string definitionName = (string)GetXMLValue(DefinitionName.Value);
                        string conditionName = condition.DisplayName?.FirstOrDefault()?.Value;
                        string fullPath = $"{sourcePath}{UFUAArea.AreaSeparator}{conditionName}{UFUAArea.AreaSeparator}{alarmText}";
                        string tagPath = tag.GetRelativeName().Replace('/','.');
                        
                        string alarmName = conditionName.Contains($"{tagPath}") ? conditionName.Substring(($"{tagPath}{UFUAArea.AreaSeparator}").Length) : conditionName;
                        int expIndex = alarmName.LastIndexOf(':');
                        string alarmExpression = null;
                        if (expIndex >= 0)
                        {
                            alarmExpression = alarmName.Remove(expIndex);
                            alarmName = alarmName.Substring(expIndex + 1);
                        }

                        UFUAAlarmSource alarmSource;
                        UFUAAlarmDefinition definition;
                        if (thresholdList.ContainsKey(fullPath))
                        {
                            alarmthreshold = thresholdList[fullPath];
                        }
                        else
                        {
                            int index = sourcePath.LastIndexOf(UFUAArea.AreaSeparator);
                            string areaPath = index >= 0 ? sourcePath.Remove(index) : null;
                            string sourceName = index >= 0 ? sourcePath.Substring(index + 1) : null;
                            if (areaPath == null || sourceName == null)
                            {
                                ErrorMessages.Add(string.Format(Properties.Resources.ImportError, condition.DisplayName, Properties.Resources.ThresholdDetailsNotSufficient));
                                return;
                            }

                            string rootName = rootAlarmName;
                            if (existingAlarmSourceObjects.ContainsKey($"{rootAlarmName}{UFUAArea.AreaSeparator}{sourcePath}"))
                            {
                                alarmSource = existingAlarmSourceObjects[$"{rootAlarmName}{UFUAArea.AreaSeparator}{sourcePath}"];
                                definition = document.GetAlarmPrototype(definitionName, alarmSource);
                                if (definition == null)
                                {
                                    definition = document.AddNewAlarmPrototype(alarmSource, definitionName);
                                    addedObjects.Add(definition);
                                }
                            }
                            else
                            {
                                alarmSource = document.GetAlarmSource(sourcePath);
                                if (alarmSource != null)
                                    definition = document.GetAlarmPrototype(definitionName, alarmSource);
                                else
                                {
                                    UFUAArea area = document.AddNewAlarmArea(areaPath, tag.Session as UnitOfWork, null);
                                    alarmSource = document.AddNewAlarmSource(area, sourceName);
                                    definition = document.AddNewAlarmPrototype(alarmSource, definitionName);

                                    addedObjects.Add(area);
                                    addedObjects.Add(alarmSource);
                                    addedObjects.Add(definition);
                                }
                            }
                            if (definition != null)
                            {
                                alarmthreshold = document.AddNewAlarmThreshold(definition);
                                addedObjects.Add(alarmthreshold);
                            }
                        }
                        if (alarmthreshold != null)
                        {
                            definition = alarmthreshold.UFUAAlarmDefinitionRef;
                            alarmthreshold.AlarmText = alarmText;
                            alarmthreshold.Expression = alarmExpression;

                            if (EventType != null)
                                definition.AlarmType = alarmType;
                            if (HighHighLimit != null)
                            {
                                double highHighLimit = HighHighLimit != null ? GetDoubleValue(HighHighLimit, definition.HighHighLimit.Value) : 0;
                                definition.HighHighLimit = highHighLimit;
                            }
                            if (HighLimit != null)
                            {
                                double highLimit = HighLimit != null ? GetDoubleValue(HighLimit, definition.HighLimit.Value) : 0;
                                definition.HighLimit = highLimit;
                            }
                            if (LowLimit != null)
                            {
                                double lowLimit = LowLimit != null ? GetDoubleValue(LowLimit, definition.LowLimit.Value) : 0;
                                definition.LowLimit = lowLimit;
                            }
                            if (LowLowLimit != null)
                            {
                                double lowLowLimit = LowLowLimit != null ? GetDoubleValue(LowLowLimit, definition.LowLowLimit.Value) : 0;
                                definition.LowLowLimit = lowLowLimit;
                            }
                            if (ConditionType != null)
                                definition.ConditionType = conditionType;

                            changedObjects.Add(definition);
                            changedObjects.Add(alarmthreshold);
                            if (!tag.UFUAAlarmThresholds.Contains(alarmthreshold))
                                tag.UFUAAlarmThresholds.Add(alarmthreshold);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.Add(string.Format(Properties.Resources.ImportError, node.DisplayName, ex.Message));
                        return;
                    }
                }
            });

            thresholdList.Clear();
        }

        private void UpdateRanges(UFUATag tag, UAVariable node)
        {
            var tagList = (from UANode v in Items
                           where v is UAVariable && (node as UAInstance).ParentNodeId == node.NodeId
                           select node).ToList();

            var euRange = (from UAVariable v in tagList
                           where v.DisplayName.ToString() == Opc.Ua.BrowseNames.EURange &&
                           v.DataType == DataTypeIds.Range.ToString()
                           select v).FirstOrDefault();
            var instrumentRange = (from UAVariable v in tagList
                           where v.DisplayName.ToString() == Opc.Ua.BrowseNames.InstrumentRange &&
                           v.DataType == DataTypeIds.Range.ToString()
                           select v).FirstOrDefault();
            var engineeringUnit = (from UAVariable v in tagList
                           where v.DisplayName.ToString() == Opc.Ua.BrowseNames.EngineeringUnits &&
                           v.DataType == DataTypeIds.Range.ToString()
                           select v).FirstOrDefault();

            UFUAEngineeringUnit eunit = CreateEUnit(euRange, instrumentRange, engineeringUnit);
            if(eunit != null)
                tag.UFUAEngineeringUnit = eunit.Name;
        }

        private UFUAEngineeringUnit CreateEUnit(UAVariable euRange, UAVariable instrumentRange, UAVariable engineeringUnit)
        {
            if ((euRange == null || euRange.Value == null) &&
                (instrumentRange == null || instrumentRange.Value == null) &&
                (engineeringUnit == null || engineeringUnit.Value == null))
                return null;

            UFUAEngineeringUnit eUnit = null;

            string typedefName = euRange?.Description?.FirstOrDefault()?.Value;
            if(string.IsNullOrEmpty(typedefName))
                typedefName = instrumentRange?.Description?.FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(typedefName))
                typedefName = engineeringUnit?.Description?.FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(typedefName))
                typedefName = document.NewEngineeringUnitsName();

            string euName = UFUAModel.Helpers.NameValidator.EnsureValidName(typedefName);
            if (typeEUnitDefinitions.ContainsKey(euName))
                return typeEUnitDefinitions[euName];

            eUnit = document.GetEngineeringUnit(euName);
            if (eUnit != null)
            {
                if (dialogRetValue != CustomDialogResults.YesAll &&
                    dialogRetValue != CustomDialogResults.NoAll &&
                    dialogRetValue != CustomDialogResults.Cancel)
                {
                    if (uiMsgBox != null)
                        dialogRetValue = uiMsgBox.ShowYesNoAllCancel(string.Format(Properties.Resources.ImportEUnitExists, typedefName), CustomDialogIcons.Question);
                }

                if (dialogRetValue == CustomDialogResults.Cancel)
                    return null;
            }

            if (eUnit == null)
            {
                eUnit = document.AddNewEngineeringUnits();
                addedObjects.Add(eUnit);
            }
            else if (dialogRetValue == CustomDialogResults.No || dialogRetValue == CustomDialogResults.NoAll)
            {
                if (typeEUnitDefinitions != null)
                    typeEUnitDefinitions[typedefName] = eUnit;

                return eUnit;
            }
            else if (dialogRetValue == CustomDialogResults.Yes || dialogRetValue == CustomDialogResults.YesAll)
            {
                eUnit = document.AddNewEngineeringUnits();
                addedObjects.Add(eUnit);
                euName = document.NewEngineeringUnitsName(euName, null, listEUNames, "{0}_{1}");
            }

            if (euName != typedefName)
                logGeneral.Info(string.Format(Properties.Resources.ImportedPrototypeNameChanged, $"{typedefName}", $"{euName}"));

            eUnit.Name = euName;

            if(euRange != null && euRange.Value != null)
            {
                Range range = (Range)GetXMLValue(euRange.Value);
                if (range != null)
                {
                    eUnit.EURangeHigh = range.High;
                    eUnit.EURangeLow = range.Low;
                }
            }
            if (instrumentRange != null && instrumentRange.Value != null)
            {
                Range range = (Range)GetXMLValue(instrumentRange.Value);
                if (range != null)
                {
                    eUnit.InstrumentRangeHigh = range.High;
                    eUnit.InstrumentRangeLow = range.Low;
                }
            }

            if (engineeringUnit != null && engineeringUnit.Value != null)
            {
                EUInformation info = (EUInformation)GetXMLValue(engineeringUnit.Value);
            }

            if (typeEUnitDefinitions != null)
                typeEUnitDefinitions[typedefName] = eUnit;

            return eUnit;
        }

        private object GetXMLValue(XmlElement value)
        {
            if (value == null)
                return null;


            try
            {
                ServiceMessageContext messageContext = new ServiceMessageContext();
                messageContext.NamespaceUris = NameSpaceTable;
                string _uaxPrefix = value.GetPrefixOfNamespace(Namespaces.OpcUaXsd);
                if (string.IsNullOrEmpty(_uaxPrefix))
                    _uaxPrefix = uaxPrefix;
                XmlAttribute attribute = value.OwnerDocument.CreateAttribute(_uaxPrefix);
                attribute.Value = Namespaces.OpcUaXsd;
                value.Attributes.Append(attribute);

                using (XmlDecoder dencoder = new XmlDecoder(value, messageContext))
                {
                    NamespaceTable namespaceUris = new NamespaceTable();

                    if (NamespaceUris != null)
                    {
                        for (int ii = 0; ii < NamespaceUris.Length; ii++)
                        {
                            namespaceUris.Append(NamespaceUris[ii]);
                        }
                    }
                    StringTable serverUris = new StringTable();
                    if (ServerUris != null)
                    {
                        for (int ii = 0; ii < ServerUris.Length; ii++)
                        {
                            serverUris.Append(ServerUris[ii]);
                        }
                    }
                    dencoder.SetMappingTables(namespaceUris, serverUris);
                    TypeInfo typeInfo = null;
                    object ret = dencoder.ReadVariantContents(out typeInfo);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ErrorMessages.Add(string.Format(Properties.Resources.ImportDecodingError, ex.Message, $"{value.LocalName}: {value.InnerText}"));
                return null;
            }
        }

        private DataType? GetDataType(string refType)
        {
            if (refType == ExportAlias(DataTypeIds.Boolean))
                return DataType.Boolean;
            else if (refType == ExportAlias(DataTypeIds.SByte))
                return DataType.SByte;
            else if (refType == ExportAlias(DataTypeIds.Byte))
                return DataType.Byte;
            else if (refType == ExportAlias(DataTypeIds.Int16))
                return DataType.Int16;
            else if (refType == ExportAlias(DataTypeIds.UInt16))
                return DataType.UInt16;
            else if (refType == ExportAlias(DataTypeIds.Int32))
                return DataType.Int32;
            else if (refType == ExportAlias(DataTypeIds.UInt32))
                return DataType.UInt32;
            else if (refType == ExportAlias(DataTypeIds.Int64))
                return DataType.Int64;
            else if (refType == ExportAlias(DataTypeIds.UInt64))
                return DataType.UInt64;
            else if (refType == ExportAlias(DataTypeIds.Float))
                return DataType.Float;
            else if (refType == ExportAlias(DataTypeIds.Double))
                return DataType.Double;
            else if (refType == ExportAlias(DataTypeIds.String))
                return DataType.String;
            return null;
        }

        private ModelType? GetModelType(string refType)
        {
            if (refType == ExportAlias(VariableTypeIds.AnalogItemType))
                return ModelType.Analog;
            else if (refType == ExportAlias(VariableTypeIds.TwoStateDiscreteType))
                return ModelType.Digital;
            else if (refType == ExportAlias(VariableTypeIds.MultiStateDiscreteType))
                return ModelType.Enumerated;
            else if (refType == ExportAlias(DataTypeIds.MethodNode))
                return ModelType.Method;
            else if (refType == ExportAlias(VariableTypeIds.BaseDataVariableType))
                return ModelType.Variable;
            else
                return ModelType.ObjectType;
        }

        private List<T> GetRootObjects<T>(string nodeId = null) where T : class
        {
            Type nodetype = typeof(T);
            List<T> rootNodeList = (from UANode node in Items
                                    where node is T &&
                                    (node as UAInstance).ParentNodeId == nodeId
                                    //(nodeId == null || (nodeId != null && (node as UAInstance).ParentNodeId == nodeId))
                                    select node as T).ToList();
            return rootNodeList;
        }

        private T GetObject<T>(List<string> references, string browsable) where T : class
        {
            string nodeid = (from r in references where r.LastIndexOf(browsable) > 0 select r).FirstOrDefault();
            return GetObject<T>(nodeid);
        }

        private T GetObject<T>(string nodeId = null) where T : class
        {
            Type nodetype = typeof(T);
            T oNode = (from UANode node in Items
                                    where node is T &&
                                    (node as UAInstance).NodeId == nodeId
                                    select node as T).FirstOrDefault();
            return oNode;
        }

        /// <summary>
        /// Exports the array dimensions.
        /// </summary>
        private string Export(IList<uint> arrayDimensions)
        {
            if (arrayDimensions == null)
            {
                return String.Empty;
            }

            StringBuilder buffer = new StringBuilder();

            for (int ii = 0; ii < arrayDimensions.Count; ii++)
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(',');
                }

                buffer.Append(arrayDimensions[ii]);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Exports a namespace index.
        /// </summary>
        private ushort ExportNameSpaceIndex(ushort NameSpaceIndex, NamespaceTable namespaceUris)
        {
            // nothing special required for indexes 0.
            if (NameSpaceIndex < 1)
            {
                return NameSpaceIndex;
            }

            // return a bad value if parameters are bad.
            if (namespaceUris == null || namespaceUris.Count <= NameSpaceIndex)
            {
                return UInt16.MaxValue;
            }

            // find an existing index.
            int count = 1;
            string targetUri = namespaceUris.GetString(NameSpaceIndex);

            if (this.NamespaceUris != null)
            {
                for (int ii = 0; ii < this.NamespaceUris.Length; ii++)
                {
                    if (this.NamespaceUris[ii] == targetUri)
                    {
                        return (ushort)(ii + 1); // add 1 to adjust for the well-known URIs which are not stored.
                    }
                }

                count += this.NamespaceUris.Length;
            }

            // add a new entry.
            string[] uris = new string[count];

            if (this.NamespaceUris != null)
            {
                Array.Copy(this.NamespaceUris, uris, count - 1);
            }

            uris[count - 1] = targetUri;
            this.NamespaceUris = uris;

            // return the new index.
            return (ushort)count;
        }

        /// <summary>
        /// Exports a NodeId
        /// </summary>
        private string Export(Opc.Ua.NodeId source)
        {
            if (Opc.Ua.NodeId.IsNull(source))
            {
                return String.Empty;
            }
            return source.ToString();
        }

        /// <summary>
        /// Exports a NodeId as an alias.
        /// </summary>
        private string ExportAlias(Opc.Ua.NodeId source)
        {
            string nodeId = Export(source);

            if (!String.IsNullOrEmpty(nodeId))
            {
                if (this.Aliases != null)
                {
                    for (int ii = 0; ii < this.Aliases.Length; ii++)
                    {
                        if (this.Aliases[ii].Value == nodeId)
                        {
                            return this.Aliases[ii].Alias;
                        }
                    }
                }
            }

            return nodeId;
        }
        /// <summary>
        /// Exports a QualifiedName
        /// </summary>
        private string Export(Opc.Ua.QualifiedName source, NamespaceTable namespaceUris)
        {
            if (Opc.Ua.QualifiedName.IsNull(source))
            {
                return String.Empty;
            }

            if (source.NamespaceIndex > 0)
            {
                ushort NameSpaceIndex = ExportNameSpaceIndex(source.NamespaceIndex, namespaceUris);
                source = new Opc.Ua.QualifiedName(source.Name, NameSpaceIndex);
            }

            return source.ToString();
        }
        /// <summary>
        /// Exports localized text.
        /// </summary>
        private LocalizedText[] Export(Opc.Ua.LocalizedText[] input)
        {
            if (input == null)
            {
                return null;
            }

            List<LocalizedText> output = new List<LocalizedText>();

            for (int ii = 0; ii < input.Length; ii++)
            {
                if (input[ii] != null)
                {
                    LocalizedText text = new LocalizedText();
                    text.Locale = input[ii].Locale;
                    text.Value = input[ii].Text;
                    output.Add(text);
                }
            }

            return output.ToArray();
        }
        List<Reference> m_references;
        /// <summary>
        /// Adds a reference to target entity.
        /// </summary>
        /// <remarks>
        /// Will not add the reference if the browse name does not match the browse name filter.
        /// </remarks>
        private void Add(NodeId referenceTypeId, bool isInverse, NodeId targetId)
        {
            Reference _ref = new Reference() { IsForward = !isInverse, ReferenceType = referenceTypeId?.ToString(), Value = targetId?.ToString() };
            m_references.Add(_ref);
        }
        /// <summary>
        /// Returns true if the reference type is required.
        /// </summary>
        private bool IsRequired(NodeId referenceType)
        {
            if (NodeId.IsNull(referenceType))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Adds a reference to target entity.
        /// </summary>
        private void Add(Reference reference)
        {
            m_references.Add(reference);
        }
        /// <summary>
        /// Adds a reference to target entity.
        /// </summary>
        private void Add(NodeId referenceTypeId, NodeId referenceType)
        {
            Reference _ref = new Reference() { ReferenceType = referenceType.ToString(), Value = referenceTypeId.ToString() };
            m_references.Add(_ref);
        }

        /// <summary>
        /// Creates an encoder to save Variant values.
        /// </summary>
     
        private XmlDecoder CreateDecoder(XmlElement source)
        {
            ServiceMessageContext messageContext = new ServiceMessageContext();
            messageContext.NamespaceUris = NameSpaceTable;

            XmlDecoder decoder = new XmlDecoder(source, messageContext);

            NamespaceTable namespaceUris = new NamespaceTable();

            if (NamespaceUris != null)
            {
                for (int ii = 0; ii < NamespaceUris.Length; ii++)
                {
                    namespaceUris.Append(NamespaceUris[ii]);
                }
            }

            StringTable serverUris = new StringTable();

            if (ServerUris != null)
            {
                for (int ii = 0; ii < ServerUris.Length; ii++)
                {
                    serverUris.Append(ServerUris[ii]);
                }
            }

            decoder.SetMappingTables(namespaceUris, serverUris);

            return decoder;
        }

        static object ChangeType(Object v, BuiltInType builtinType, uint arraySizeOneDimension = 0, System.Globalization.NumberFormatInfo info = null)
        {
            object value = v;
            if (arraySizeOneDimension > 0)
            {
                if (value is Array)
                {
                    var values = value as Array;
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values.GetValue(ii), builtinType, 0, info), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    (value as String).Length > 2 && (value as String)[0] == '{' &&
                    (value as String)[(value as String).Length - 1] == '}')
                {
                    var values = (value as String).Substring(1, (value as String).Length - 2).Split(new string[] { " |" }, StringSplitOptions.None);
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < values.Length && ii < array.Length; ii++)
                    {
                        array.SetValue(ChangeType(values[ii], builtinType, 0, info), ii);
                    }

                    return array;
                }
                else if (value is String &&
                    builtinType == BuiltInType.Byte &&
                    (value as String).Length >= (arraySizeOneDimension * 2))
                {
                    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)arraySizeOneDimension);
                    for (int ii = 0; ii < array.Length; ii++)
                    {
                        array.SetValue(Byte.Parse((value as String).Substring(ii * 2, 2), System.Globalization.NumberStyles.HexNumber, new System.Globalization.NumberFormatInfo()), ii);
                    }

                    return array;
                }
                else
                    throw new InvalidCastException(String.Format("Cannot cast the value '{0}' to type {1}({2})", v, builtinType, arraySizeOneDimension));
            }

            try
            {
                if (value is String && builtinType != BuiltInType.String)
                {
                    if (String.Compare(value as String, "True", true) == 0)
                        v = 1;
                    else if (String.Compare(value as String, "False", true) == 0)
                        v = 0;
                    else if (info != null)
                    {
                        //System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                        v = Convert.ToDouble(v, info);
                    }
                    else
                    {
                        v = Convert.ToDouble(v);
                    }
                }
            }
            catch { }

            switch (builtinType)
            {
                case BuiltInType.Boolean: value = Convert.ToBoolean(v); break;
                case BuiltInType.SByte: value = Convert.ToSByte(v); break;
                case BuiltInType.Byte: value = Convert.ToByte(v); break;
                case BuiltInType.Int16: value = Convert.ToInt16(v); break;
                case BuiltInType.UInt16: value = Convert.ToUInt16(v); break;
                case BuiltInType.Int32: value = Convert.ToInt32(v); break;
                case BuiltInType.UInt32: value = Convert.ToUInt32(v); break;
                case BuiltInType.Int64: value = Convert.ToInt64(v); break;
                case BuiltInType.UInt64: value = Convert.ToUInt64(v); break;
                case BuiltInType.Float: value = Convert.ToSingle(v); break;
                case BuiltInType.Double: value = Convert.ToDouble(v); break;
            }

            return value;
        }

        private NodeId FromGuidToNodeId(Guid from)
        {
            return new NodeId(from, NameSpaceIndex);
        }

        private static NodeId ConstructIdForComponent(NodeState component, ushort NameSpaceIndex)
        {
            if (component == null)
            {
                return null;
            }

            // components must be instances with a parent.
            BaseInstanceState instance = component as BaseInstanceState;

            if (instance == null || instance.Parent == null)
            {
                return component.NodeId;
            }

            // parent must have a string identifier.
            string parentId = instance.Parent.NodeId.Identifier as string;

            if (parentId == null)
            {
                parentId = instance.Parent.NodeId.Identifier.ToString();
            }

            StringBuilder buffer = new StringBuilder();
            buffer.Append(parentId);

            // check if the parent is another component.
            int index = parentId.IndexOf('?');

            if (index < 0)
            {
                buffer.Append('?');
            }
            else
            {
                buffer.Append('/');
            }


            if (!NodeId.IsNull(component.NodeId) && component.NodeId.IdType == IdType.Guid)
                buffer.Append((Guid)component.NodeId.Identifier);
            else
                buffer.Append(component.SymbolicName);

            // return the node identifier.
            return new NodeId(buffer.ToString(), NameSpaceIndex);
        }
        private void SetStatusCode(NodeState baseObject, uint status, bool notify = true)
        {
            if (baseObject != null)
            {
                if (baseObject is BaseVariableState)
                {
                    var variable = baseObject as BaseVariableState;
                    variable.StatusCode = status;
                }

                var baseObjectType = baseObject as BaseObjectState;
                if (baseObjectType != null)
                {
                    var children = new List<BaseInstanceState>();
                    baseObjectType.GetChildren(null, children);
                    children.ForEach((child) => SetStatusCode(child, status, notify));
                }
            }
        }
        private void fillOrderedMemberList(UFUAModel.UFUATagPrototype typedef, Dictionary<string, int> w)
        {
            typedef.EnsureUniqueMembersOrderId();

            int id = 0;
            if (w.Values.Count > 0)
                id = w.Values.Max() + 1;
            var members = typedef.Members.OrderBy(o => o.MemberOrderId);
            foreach (var m in members)
            {
                var name = m.GetRelativeName();
                if (!w.ContainsKey(name))
                    w.Add(name/*m.NodeId.ToString()*/, id++);
            }
            var folders = typedef.Folders.OrderBy(o => o.MemberOrderId);
            foreach (var f in folders)
            {
                var name = f.GetRelativeName();
                if (!w.ContainsKey(name))
                {
                    w.Add(name/*m.NodeId.ToString()*/, id++);
                    fillOrderedMemberList(f, w);
                }
            }
        }
        private void fillOrderedMemberList(UFUAModel.UFUAFolder typedef, Dictionary<string, int> w)
        {
            int id = 0;
            if (w.Values.Count > 0)
                id = w.Values.Max() + 1;
            var members = typedef.UFUATags.OrderBy(o => o.MemberOrderId);
            foreach (var m in members)
            {
                var name = m.GetRelativeName();
                if (!w.ContainsKey(name))
                    w.Add(name/*m.NodeId.ToString()*/, id++);
            }
            var folders = typedef.UFUAFolders.OrderBy(o => o.MemberOrderId);
            foreach (var f in folders)
            {
                var name = f.GetRelativeName();
                if (!w.ContainsKey(name))
                {
                    w.Add(name/*m.NodeId.ToString()*/, id++);
                    fillOrderedMemberList(f, w);
                }
            }
        }
        private NodeId GetResolvedNodeId(UFUAModel.UFUATag ufuaTag, NodeId parent, bool bUseOldFormat = false)
        {
            NodeId nodeid;
            if (bUseOldFormat)
            {
                if (String.IsNullOrEmpty(ufuaTag.FolderPath))
                    nodeid = new NodeId(String.Format("{0}?{1}", parent.Identifier, ufuaTag.Name), NameSpaceIndex);
                else
                    nodeid = new NodeId(String.Format("{0}?{1}//{2}", parent.Identifier, ufuaTag.FolderPath, ufuaTag.Name), NameSpaceIndex);
            }
            else
            {
                if (String.IsNullOrEmpty(ufuaTag.FolderPath))
                    nodeid = new NodeId(String.Format("{0}?{1}", parent.Identifier, ufuaTag.NodeId), NameSpaceIndex);
                else
                    nodeid = new NodeId(String.Format("{0}?{1}/{2}", parent.Identifier, ufuaTag.FolderPath.Replace('\\', '/'), ufuaTag.NodeId), NameSpaceIndex);
            }

            return nodeid;
        }
        public void Dispose()
        {
            if (disposing)
                return;
            disposing = true;
            this.Items = null;
            if (mapStructToMemberOder != null)
                mapStructToMemberOder.Clear();
            if (mapNodeIdToDynamicSettings != null)
                mapNodeIdToDynamicSettings.Clear();
            if (enumStrings != null)
                enumStrings.Clear();
            if (alarmThresholds != null)
                alarmThresholds.Clear();
            if (checkRecursivePrototypes != null)
                checkRecursivePrototypes.Clear();
            if (typePrototypeDefinitions != null)
                typePrototypeDefinitions.Clear();
            
            if (m_references != null)
                m_references.Clear();
            if (existingObject != null)
                existingObject.Keys.ToList().ForEach(k => { existingObject[k].Clear(); });
            if (existingAlarmDefinitionObject != null)
                existingAlarmDefinitionObject.Keys.ToList().ForEach(k => { existingAlarmDefinitionObject[k].Clear(); });
            if (existingAlarmSourceObject != null)
                existingAlarmSourceObject.Keys.ToList().ForEach(k => { existingAlarmSourceObject[k].Clear(); });
            if (existingAreaObject != null)
                existingAreaObject.Keys.ToList().ForEach(k => { existingAreaObject[k].Clear(); });
        }
        #endregion

        #region Private Fields
        string trueState = string.Empty;
        string falseState = string.Empty;

        private bool disposing;
        private TypeTable TypeTree { get; }
        private readonly Dictionary<NodeId, Dictionary<string, int>> mapStructToMemberOder = new Dictionary<NodeId, Dictionary<string, int>>();
        private readonly Dictionary<NodeId, String> mapNodeIdToDynamicSettings = new Dictionary<NodeId, String>();
        static Opc.Ua.NamespaceTable nameSpaceTable = new Opc.Ua.NamespaceTable();
        static ushort NameSpaceIndex = (ushort)(nameSpaceTable.Count + 2 - 1);
        UFUAServerDocument document;
        /// <summary>
        /// Gets the default index for the node manager's namespace.
        /// </summary>
        private NamespaceTable NameSpaceTable
        {
            get { return nameSpaceTable; }
        }
        private string UFUAServerUri
        {
            get { return UFInterfaces.Constants.Namespaces.UriProgea; }
        }
        private List<UFUAModel.UFUAEnumString> enumStrings;
        private List<UFUAModel.UFUAAlarmThreshold> alarmThresholds;
        private readonly List<String> checkRecursivePrototypes = new List<String>();
        private readonly Dictionary<String, UFUAModel.UFUAHistorianSettings> typeHistorianDefinitions = new Dictionary<String, UFUAModel.UFUAHistorianSettings>();
        private readonly Dictionary<String, UFUAModel.UFUATagPrototype> typePrototypeDefinitions = new Dictionary<String, UFUAModel.UFUATagPrototype>();
        private readonly Dictionary<String, UFUAModel.UFUAEngineeringUnit> typeEUnitDefinitions = new Dictionary<String, UFUAModel.UFUAEngineeringUnit>();
        private readonly Dictionary<String, String> typePrototypeNodeIds = new Dictionary<String, String>();
        private readonly Dictionary<String, String> typeAlarmNodeIds = new Dictionary<String, String>();
        private object lockObj = new object();
        private string ServerUri;
        private string ApplicationUri;
        private string ReservedUri;
        private string Diagnostics;
        private string hasProperty = ReferenceTypeIds.HasProperty.ToString();
        private string hasComponent = ReferenceTypeIds.HasComponent.ToString();
        private string hasEventSource = ReferenceTypeIds.HasEventSource.ToString();
        private string hasNotifier = ReferenceTypeIds.HasNotifier.ToString();
        private string hasHistoricalConfiguration = ReferenceTypeIds.HasHistoricalConfiguration.ToString();
        private string hasCondition = ReferenceTypeIds.HasCondition.ToString();
        private string hasTypeDefinition = ReferenceTypeIds.HasTypeDefinition.ToString();
        private string organizes = ReferenceTypeIds.Organizes.ToString();
        private string objectsFolder = ObjectIds.ObjectsFolder.ToString();
        private string server = ObjectIds.Server.ToString();
        private string folderType = ObjectTypeIds.FolderType.ToString();
        private string serverType = ObjectTypeIds.ServerType.ToString();
        private string propertyType = VariableTypeIds.PropertyType.ToString();
        private string historicalDataConfigurationType = ObjectTypeIds.HistoricalDataConfigurationType.ToString();
        private string baseObjectType = ObjectTypeIds.BaseObjectType.ToString();
        private string uaxPrefix = Properties.Settings.Default.UaxPrefix;
        
        private IWorkspace workspace;
        private IUIMsgBoxAlertService uiMsgBox;
        private CustomDialogResults dialogRetValue;
        string rootAlarmName = UFUAServerInfo.UFUAServerInfo.GetAlarmRootName();
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.ImportExportLog);
        private List<string> computedVariableType;
        private Dictionary<string, List<UFUATag>> existingObject;
        private Dictionary<string, List<UFUAArea>> existingAreaObject;
        private Dictionary<string, List<UFUAAlarmSource>> existingAlarmSourceObject;
        private Dictionary<string, UFUAAlarmSource> existingAlarmSourceObjects;
        private Dictionary<string, List<UFUAAlarmDefinition>> existingAlarmDefinitionObject;
        private List<string> listPrototypeNames = new List<string>();
        private List<string> listHistoricalNames = new List<string>();
        private List<string> listEUNames = new List<string>();
        private bool bCollectNodeIds = false;
        private List<string> ErrorMessages = new List<string>();
        private List<IXPSimpleObject> addedObjects;
        private List<IXPSimpleObject> changedObjects;
        #endregion
    }
}
