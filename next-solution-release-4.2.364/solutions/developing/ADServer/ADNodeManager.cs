using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using Opc.Ua.Server;
using DevExpress.Xpo;
using System.Threading.Tasks;
using System.Threading;

namespace ADServer
{
    class ADNodeManager : CustomNodeManager2
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public ADNodeManager(ADUAServer uaserver, IServerInternal s, ApplicationConfiguration c)
        :
            base(s, c, Namespaces.ServerModel)
        {
            server = uaserver;
            if (!String.IsNullOrEmpty(server.ADConfiguration.NamespaceUri))
                SetNamespaces(server.ADConfiguration.NamespaceUri);

            if (!String.IsNullOrEmpty(server.ADConfiguration.AliasRoot))
                AliasRoot = server.ADConfiguration.AliasRoot;
            else
                AliasRoot = "AD";

            // SystemContext.SystemHandle = m_system = new UnderlyingSystem();
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            configuration = c.ParseExtension<ADServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration == null)
            {
                configuration = new ADServerConfiguration();
            }
        }
        #endregion

        #region INodeManager Members

        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            try
            {
                lock (Lock)
                {
                    IList<IReference> references = null;

                    if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                    {
                        externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                    }

                    using (var ufw = new UnitOfWork())
                    {
                        //using (new StopWatcherLogger(Properties.Resources.CreatingAddressSpace,
                        //                             Properties.Resources.CreatedAddressSpace))
                        {

                            basePluginFolder = CreateRootFolder("Plugins");

                            //var views = (from view in new XPQuery<UFUAModel.UFUAView>(ufw).AsParallel()
                            //             select view).ToList();
                            //foreach (var view in views)
                            //    CreateView(view);

                            var plugins = (from plugin in new XPQuery<ADModel.ADPlugin>(ufw).AsParallel()
                                           select plugin).ToList();

                            foreach (var plugin in plugins)
                            {
                                CreatePlugin(plugin, basePluginFolder.NodeId, basePluginFolder);
                            }
                            //var roots = (from folder in new XPQuery<UFUAModel.UFUAFolder>(ufw).AsParallel()
                            //             where folder.Name == AliasRoot && folder.UFUAFolderAss == null
                            //             select folder).ToList();

                            //if (folders.Count > 0 || roots.Count > 0)
                            //{
                                
                            //    foreach (var root in roots)
                            //    {
                            //        foreach (var tag in root.UFUATags)
                            //        {
                            //            CreateTag(tag, baseFolderTags.NodeId, baseFolderTags);
                            //        }
                            //    }

                                
                            //}
                        }

                        AddReverseReferences(externalReferences);
                    }
                }

                Logger.WriteToEventLog(Properties.Resources.LoggerSource, Properties.Resources.ServerStarted, System.Diagnostics.EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Error);
                Utils.Trace(e, "Unexpected error in Creating the address space");
            }
        }

        BaseObjectState CreatePlugin(ADModel.ADPlugin adPlugin, NodeId parent,
                                     BaseObjectState parentFolder = null,
                                     bool bAssignNodeId = false, bool creatingType = false,
                                     bool bAssignFolderId = false)
        {
            var folder = new FolderState(parentFolder) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(adPlugin.Name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            if (!bAssignFolderId)
                folder.NodeId = FromGuidToNodeId(adPlugin.NodeId);
            else
            {
                folder.NodeId = SystemContext.NodeIdFactory.New(SystemContext, folder);
                if (parentFolder != null)
                    parentFolder.AddChild(folder);
            }
            // if (!creatingType)
            folder.AddReference(ReferenceTypeIds.Organizes, true, parent);


            //**************************************
            // Plugin elements
            CreatePluginElements(folder.NodeId, folder);
            
            //*******************************************************
            // Plugin elements end
            //*******************************************************

            if (!bAssignNodeId)
            {
                AddPredefinedNode(SystemContext, folder);
                //mapNodeIdToNodeState.Add(folder.NodeId, folder);
            }


            return folder;
        }

        void CreatePluginElements(NodeId parent,
                                    NodeState parentFolder = null,
                                    bool bAssignNodeId = false, bool creatingType = false)
        {
            //**************************************
            // Plugin methods

            string name = "SendMessage";
            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(name, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = new NodeId(parent.Identifier.ToString() + "?" + name);
            methodBase.Create(
                            SystemContext,
                            methodBase.NodeId,
                            new QualifiedName(name, NamespaceIndex),
                            null,
                            bAssignNodeId);
            
            //In args
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(parent.Identifier.ToString() + "?" + name + "Input");
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument
            {
                Name = "MessageToSend",
                Description = "Message to send",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });
            methodBase.InputArguments.Value = args.ToArray();

            //Out args
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(parent.Identifier.ToString() + "?" + name + "Output");
            methodBase.OutputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            args.Clear();
            args.Add(new Argument
            {
                Name = "Outcome",
                Description = "Outcome of the send operation",
                DataType = DataTypeIds.Int16,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });
            methodBase.OutputArguments.Value = args.ToArray();

            if (!creatingType)
            {
                if (parentFolder == null)
                {
                    methodBase.AddReference(ReferenceTypeIds.Organizes, true, parent);
                }
                else
                {
                    if (parentFolder is BaseObjectState)
                        (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                    parentFolder.AddChild(methodBase);
                }
            }
            methodBase.OnCallMethod = OnMethodCall;

            
        }

        public ServiceResult OnMethodCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            /*var sourceName = String.Format("node {0} user {1}", method, GetUserName(context));
            var eventName = String.Format("Method Calling Input parameters {0}", inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.High, DateTime.UtcNow);

            if (mapNodeIdToInputArgs.Count > 0 && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                mapNodeIdToOutputArgs.Count > 0 && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            int i = 0;
            foreach (var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }

            var commDriver = GetCommunicationDriverFromNodeState(method);
            if (commDriver == null)
            {
                return StatusCodes.BadNodeIdInvalid;
            }
            return commDriver.OnMethodCall(new TagDefinition() { NodeId = method.NodeId },
                            inputArguments, outputArguments);*/
            return StatusCodes.BadNotImplemented;
        }
        BaseObjectState CreateRootFolder(String name)
        {
            var folder = new FolderState(null) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.NodeId = FromGuidToNodeId(Guid.NewGuid());
            folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            AddPredefinedNode(SystemContext, folder);
            //mapNodeIdToNodeState.Add(folder.NodeId, folder);

            return folder;
        }
        NodeId FromGuidToNodeId(Guid from)
        {
            return new NodeId(from, NamespaceIndex);
        }
        #endregion

        #region private fields
        readonly ADUAServer server;
        readonly ADServerConfiguration configuration;
        BaseObjectState basePluginFolder;
        BaseObjectState baseFolderTags;
        #endregion
    }
}
