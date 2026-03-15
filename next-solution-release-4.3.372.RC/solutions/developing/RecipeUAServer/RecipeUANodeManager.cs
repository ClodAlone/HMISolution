using System;
using System.Collections.Generic;
using System.Data;
using Opc.Ua;
using UFUAServerBase;
using UFRecipeSettings.UFRecipeModel;
using DriverBaseInterfaces;
using Utilities.Logger;
using System.Threading;
using System.Linq;
using UFRecipeExecuter;
using Utilities;
using System.Diagnostics;
#if !NET_STANDARD
using RedundancyService;
#endif

namespace RecipeUAServer
{
    public class RecipeUANodeManager : UANodeManager
    {

        #region Declarations
        readonly RecipeServer recipeServer;
        readonly Dictionary<String, BaseObjectState> mapFolders = new Dictionary<string, BaseObjectState>();
        DateTime lastReadEventsUTC = DateTime.MinValue;
        bool bSynchronizingRecipes;

        BaseObjectTypeState recipeBaseObjectType;
        #endregion

        #region Ctor
        public RecipeUANodeManager(UAServer uaserver, Opc.Ua.Server.IServerInternal s, Opc.Ua.ApplicationConfiguration c)
            : base(uaserver, s, c)
        {
            recipeServer = (this.server as RecipeUAServer)?.RecipeServer;
        }
        #endregion

        public void RecipeUANodeManager_SystemEvent(object sender, SystemEventArgs e)
        {
            e.logdestination = (int)LoggerDestination.RecipeService;
            UANodeManager_SystemEvent(sender, e);
        }

        #region Access Levels
        bool CanUserCallMethod(ISystemContext context, NodeState node)
        {
            if (context == SystemContext)
                return true;

            var level = GetUserAccessLevel(context, node);
            return (level & Opc.Ua.AccessLevels.CurrentReadOrWrite) == Opc.Ua.AccessLevels.CurrentReadOrWrite;
        }

        ServiceResult ValidateAuditTracePermission(ISystemContext context, NodeState node, string userComment)
        {
            if (mapNodeStateToAccessSettings.ContainsKey(node) &&
                mapNodeStateToAccessSettings[node].IsAuditTraceEnabled &&
                mapNodeStateToAccessSettings[node].IsCommentRequiredOnAudit)
            {
                var recipeUAServer = this.server as RecipeUAServer;
                if (recipeUAServer == null)
                    return StatusCodes.BadSecurityChecksFailed;
                
                if (String.IsNullOrWhiteSpace(userComment))
                    return StatusCodes.BadArgumentsMissing;

                var accessSettings = mapNodeStateToAccessSettings[node];
                var userName = GetUserName(context);
                if (recipeUAServer.IsUserManagerEnabled() && accessSettings.MinAccessLevelRequiredOnAudit > 0)
                {
                    int accessMask = 0;
                    int accessLevel = 0;
                    if (userName == null || !recipeUAServer.GetUserLevel(userName, ref accessMask, ref accessLevel))
                        return StatusCodes.BadIdentityTokenInvalid;

                    if (accessSettings.MinAccessLevelRequiredOnAudit > accessLevel)
                        return StatusCodes.BadUserAccessDenied;
                }

                bool bClearChangeMask = false;
                if (accessSettings.LastUserNameOnAudit != userName)
                {
                    accessSettings.LastUserNameOnAudit = userName;
                    var lastUserNameNodeId = new NodeId(String.Format("{0}?{1}", node.NodeId.Identifier, RecipeUAServerInfo.BrowserNames.LastUserNameOnAudit), NamespaceIndex);
                    if (mapNodeIdToNodeState.ContainsKey(lastUserNameNodeId))
                    {
                        mapNodeIdToNodeState[lastUserNameNodeId].UpdateChangeMasks(NodeStateChangeMasks.Value);
                        bClearChangeMask = true;
                    }
                }

                if (accessSettings.LastCommentOnAudit != userComment)
                {
                    accessSettings.LastCommentOnAudit = userComment;
                    var lastCommentNodeId = new NodeId(String.Format("{0}?{1}", node.NodeId.Identifier, RecipeUAServerInfo.BrowserNames.LastCommentOnAudit), NamespaceIndex);
                    if (mapNodeIdToNodeState.ContainsKey(lastCommentNodeId))
                    {
                        mapNodeIdToNodeState[lastCommentNodeId].UpdateChangeMasks(NodeStateChangeMasks.Value);
                        bClearChangeMask = true;
                    }
                }

                if (bClearChangeMask)
                    node.ClearChangeMasks(context, true);
            }

            return StatusCodes.Good;
        }
        #endregion

        #region Overrides
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            try
            {
                lock (Lock)
                {
                    var recipeUAServer = this.server as RecipeUAServer;
                    var recipeServer = recipeUAServer?.RecipeServer;
                    if (recipeServer == null || recipeUAServer == null)
                        throw new ArgumentNullException("RecipeServer cannot be null");

                    recipeServer.SystemEvent += RecipeUANodeManager_SystemEvent;
                    List<BaseObjectState> recipes = new List<BaseObjectState>();
                    using (new StopWatcherLogger(Utilities.Properties.Resources.RecipeService, LoggerDestination.Server,
                                                                            Properties.Resources.CreatingAddressSpace,
                                                                            Properties.Resources.CreatedAddressSpace))
                    {
                        var uris = recipeServer.GetWholeRecipeLists();
                        if (uris != null && uris.Count > 0)
                        {
                            baseFolderTags = CreateRootFolder(RecipeUAServerInfo.RecipeUAServerInfo.GetRecipesRootName(), RecipeUAServerInfo.Guids.RecipesRootGuid);
                            foreach (var uri in uris)
                            {
                                var recipe = CreateRecipe(uri);
                                if (recipe != null)
                                    recipes.Add(recipe);
                            }

                            AddPredefinedNode(SystemContext, baseFolderTags);
                            mapNodeIdToNodeState.Add(baseFolderTags.NodeId, baseFolderTags);
                        }

                        AddReverseReferences(externalReferences);
                    }

                    using (new StopWatcherLogger(Utilities.Properties.Resources.RecipeService, LoggerDestination.Server,
                                                                            Properties.Resources.InitializingRecipes,
                                                                            Properties.Resources.InitializedRecipes))
                    {
                        if (recipes.Count > 0)
                        {
                            foreach (var recipe in recipes)
                            {
                                var recipeUri = recipe.Handle as Uri;
                                if (recipeUri == null)
                                    continue;

                                var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
                                if (recipeExecuter != null)
                                {
                                    var statusNodeId = new NodeId(String.Format("{0}/{1}", recipe.NodeId.Identifier, RecipeUAServerInfo.BrowserNames.RecipeExecutionStatateName), NamespaceIndex);
                                    if (mapNodeIdToNodeState.ContainsKey(statusNodeId))
                                    {
                                        var variable = mapNodeIdToNodeState[statusNodeId] as BaseVariableState;
                                        UpdateStatusVariable(variable, (uint)recipeExecuter.ExecutionState);
                                        recipeExecuter.StateChanged += (s, e) =>
                                        {
                                            UpdateStatusVariable(variable, (uint)e.newState);
                                        };
                                    }
                                    DeviceAddressCheck(recipeExecuter);
                                }
                            }
                        }
                    }
#if !NET_STANDARD
                    #region Redundancy
                    if (recipeUAServer.IsRedundancyEnabled)
                    {
                        InitEventLogger();
                        
                        recipeUAServer.ActiveServerManager.EventConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.EventDefaultConnection);

                        recipeUAServer.ActiveServerManager.CallingMethod += (o, ev) =>
                        {
                            var handle = GetManagerHandle(SystemContext, ev.methodNodeId, null);
                            if (handle != null)
                            {
                                if (handle.Node is MethodState)
                                {
                                    var systemContext = SystemContext;
                                    var method = handle.Node as MethodState;
                                    var result = Call(systemContext, ev.methodRequest, method, ev.methodResult);
                                    ev.serviceResult = result;
                                }
                                else
                                    ev.serviceResult = StatusCodes.BadMethodInvalid;
                            }
                            else
                                ev.serviceResult = StatusCodes.BadNodeIdInvalid;
                        };

                        recipeUAServer.ActiveServerManager.ServerStateChanging += (o, ev) =>
                        {
                            ThreadPool.QueueUserWorkItem((ob) =>
                            {
                                if (ev.serverState == RedundancyService.RedundancyServerState.StartPolling)
                                {
                                    if (eventLogger != null)
                                    {
                                        eventLogger.StopSync();
                                    }
                                }
                                else if (ev.serverState == RedundancyService.RedundancyServerState.Activating)
                                {
                                    if (eventLogger != null)
                                    {
                                        eventLogger.Resume();
                                    }

                                    foreach (var recipe in recipes)
                                    {
                                        var recipeUri = recipe.Handle as Uri;
                                        if (recipeUri == null)
                                            continue;

                                        var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
                                        if (recipeExecuter != null)
                                        {
                                            recipeExecuter.Resume();
                                            var logItem = recipeExecuter.GetLastTransactionLogItem(recipe.BrowseName.Name);
                                            lastReadEventsUTC = logItem != null ? logItem.EventDateTimeUtc:DateTime.MinValue;
                                        }
                                    }
                                }
                                else if (ev.serverState == RedundancyService.RedundancyServerState.Deactivating)
                                {
                                    if (eventLogger != null)
                                    {
                                        eventLogger.Suspend();
                                    }

                                    foreach (var recipe in recipes)
                                    {
                                        var recipeUri = recipe.Handle as Uri;
                                        if (recipeUri == null)
                                            continue;

                                        var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
                                        if (recipeExecuter != null)
                                        {
                                            recipeExecuter.Suspend();
                                            var logItem = recipeExecuter.GetLastTransactionLogItem(recipe.BrowseName.Name);
                                            lastReadEventsUTC = logItem != null ? logItem.EventDateTimeUtc : DateTime.MinValue;
                                        }
                                        
                                    }
                                }
                            });
                        };

                        recipeUAServer.ActiveServerManager.SynchronizeHistoryData += (o, ev) =>
                        {
                            ev.IsExecuted = true;

                            if (ExitMode)
                                return;

                            if (ev.historySettings != null)
                            {
                                ThreadPool.QueueUserWorkItem((ob) =>
                                {
                                    if (!String.IsNullOrEmpty(ev.historySettings.EventDefaultConnection) &&
                                    !String.IsNullOrEmpty(server.UFUAConfiguration.EventDefaultConnection))
                                    {
                                        var destinationConn = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.EventDefaultConnection);
                                        eventLogger.SynchronizeHistoryData(ev.historySettings.EventDefaultConnection, destinationConn, ev.startTime, ev.endTime);
                                    }
                                });
                            }

                            if (ev.IsStarting)
                            {
                                foreach(var recipe in recipes)
                                {
                                    if (!SynchronizeRecipesDatabase(recipe))
                                        SynchronizeRecipes(recipe);
                                }
                            }
                            else if (!bSynchronizingRecipes && !recipeUAServer.ActiveServerManager.IsActiveServer)
                            {
                                bSynchronizingRecipes = true;
                                ThreadPool.QueueUserWorkItem((ob) =>
                                {
                                    try
                                    {
                                        foreach (var recipe in recipes)
                                        {
                                            if (!SynchronizeRecipesDatabase(recipe))
                                                SynchronizeRecipes(recipe);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                            String.Format(Properties.Resources.ErrorSynchronizingRecipesDataServer, ev.hostName, ex.Message),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.RecipeService);
                                    }
                                    finally
                                    {
                                        bSynchronizingRecipes = false;
                                    }
                                });
                            }
                        };
                    }


                    #endregion
#endif
                }
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService, e.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.RecipeService);
                Opc.Ua.Utils.Trace(e, "Unexpected error in Creating the address space");
                server.UpdateServerState(ServerState.Failed);
            }
        }

        public override void DeleteAddressSpace()
        {
            base.DeleteAddressSpace();

            lock (Lock)
            {
                mapFolders.Clear();
            }
        }

        protected override void Dispose(bool disposing)
        {
            var recipeServer = (this.server as RecipeUAServer)?.RecipeServer;
            if (recipeServer != null)
                recipeServer.SystemEvent -= RecipeUANodeManager_SystemEvent;

            base.Dispose(disposing);
        }
        #endregion

        #region Methods
#if !NET_STANDARD
        void SynchronizeRecipes(BaseObjectState recipe)
        {
            var recipeUAServer = this.server as RecipeUAServer;
            if (recipeUAServer != null && !recipeUAServer.ActiveServerManager.IsActiveServer)
            {
                var recipeUri = recipe.Handle as Uri;
                if (recipeUri == null)
                    return;

                var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
                if (recipeExecuter != null)
                {
                    CallMethodResult result = new CallMethodResult();
                    var methodNodeId = new NodeId(String.Format("{0}/{1}", recipe.NodeId.Identifier, RecipeUAServerInfo.MethodNames.RecipeReadTransactionLog), NamespaceIndex);
                    var methodToCall = new CallMethodRequest();
                    methodToCall.MethodId = ExpandedNodeId.ToNodeId(methodNodeId, SystemContext.NamespaceUris);
                    methodToCall.InputArguments.Add(lastReadEventsUTC);
                    var callMethod = new CallMethod() { methodNodeId = methodNodeId, methodRequest = methodToCall, methodResult = result };
                    var methodResult = recipeUAServer.ActiveServerManager.SendCallingMethod(callMethod);
                    if (ServiceResult.IsGood(methodResult.serviceResult))
                    {
                        if (methodResult.outputArguments != null && methodResult.outputArguments.Count == 6)
                        {
                            var oids = methodResult.outputArguments[0].Value as IList<Int32>;
                            var eventsType = methodResult.outputArguments[1].Value as IList<UInt16>;
                            var recipeName = methodResult.outputArguments[2].Value as IList<String>;
                            var xmlDataSets = methodResult.outputArguments[3].Value as IList<String>;
                            var eventsDateTime = methodResult.outputArguments[4].Value as IList<DateTime>;
                            var eventsDateTimeUTC = methodResult.outputArguments[5].Value as IList<DateTime>;

                            List<UFRecipeTransactionLogItem> itemsList = new List<UFRecipeTransactionLogItem>();
                            for(int i = 0; i < oids.Count(); i++)
                            {
                                itemsList.Add(new UFRecipeTransactionLogItem((TransactionLogEventType)eventsType[i], recipeName[i], xmlDataSets[i], eventsDateTime[i], eventsDateTimeUTC[i]));
                                itemsList = itemsList.OrderBy(x => x.EventDateTimeUtc).ToList();
                            }
                            if (oids.Count > 0)
                            {
                                try
                                {
                                    recipeExecuter.UpdateRedundancyData(itemsList);
                                    lastReadEventsUTC = itemsList.Last().EventDateTimeUtc;
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                        String.Format(Utilities.Properties.Resources.ErrorUpdatingRedundancyData, ex.Message), EventLogEntryType.Error);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Synchronize the recipes creating a database copy of the one stored in the active server
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns>A boolean represents if the method copied the database from another server or not</returns>
        bool SynchronizeRecipesDatabase(BaseObjectState recipe)
        {
            var recipeUAServer = this.server as RecipeUAServer;
            if (recipeUAServer != null && !recipeUAServer.ActiveServerManager.IsActiveServer)
            {
                
                var recipeUri = recipe.Handle as Uri;
                if (recipeUri == null)
                    return true;

                var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);

                //if the difference is less than TransactionLogMaxAge, the syncrhonization will be performed using the transaction log
                //in case of TransactionLogMaxAge=0, the synchronization must be performed always using the transaction log
                var lastItem = recipeExecuter.GetLastTransactionLogItem(recipe.BrowseName.Name);
                if (recipeUAServer.TransactionLogMaxAge == TimeSpan.Zero || lastItem == null ||
                    DateTime.UtcNow - lastItem.EventDateTimeUtc < recipeUAServer.TransactionLogMaxAge)
                {
                    if (lastItem != null)
                        lastReadEventsUTC = lastItem.EventDateTimeUtc;
                    return false;
                }

                if (recipeExecuter != null)
                {
                    CallMethodResult result = new CallMethodResult();
                    var methodNodeId = new NodeId(String.Format("{0}/{1}", recipe.NodeId.Identifier, RecipeUAServerInfo.MethodNames.RecipeReadDataName), NamespaceIndex);
                    var methodToCall = new CallMethodRequest();
                    methodToCall.MethodId = ExpandedNodeId.ToNodeId(methodNodeId, SystemContext.NamespaceUris);
                    methodToCall.InputArguments.Add(new Variant(DataRowState.Unchanged.ToString()));
                    methodToCall.InputArguments.Add(false);
                    var callMethod = new CallMethod() { methodNodeId = methodNodeId, methodRequest = methodToCall, methodResult = result };
                    var methodResult = recipeUAServer.ActiveServerManager.SendCallingMethod(callMethod);
                    if (ServiceResult.IsGood(methodResult.serviceResult))
                    {
                        String xmlDataSet = null;
                        if (methodResult.outputArguments != null && methodResult.outputArguments.Count > 0)
                            xmlDataSet = methodResult.outputArguments[0].ToString();
                        if (xmlDataSet != null)
                        {
                            String fillError = null;
                            using (var dataSet = recipeExecuter.CreateDataSet(fill: false, out fillError, bContinueOnError: false))
                            {
                                var xmlBuilder = new System.Text.StringBuilder();
                                System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings()
                                {
                                    ConformanceLevel = System.Xml.ConformanceLevel.Document,
                                    CloseInput = true,
                                    CheckCharacters = false
                                };

                                using (var stringReader = new System.IO.StringReader(xmlDataSet))
                                {
                                    using (var xmlReader = System.Xml.XmlReader.Create(stringReader, settings))
                                    {
                                        foreach (DataTable dataTable in dataSet.Tables)
                                            dataTable.BeginLoadData();

                                        // **********************************************************
                                        // Moving two node away is needed for reading all records
                                        xmlReader.MoveToContent();
                                        xmlReader.Read();
                                        // **********************************************************
                                        dataSet.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);
                                        UFRecipeExecuter.UFRecipeExecuter.ChangeRowsState(dataSet, DataRowState.Unchanged);

                                        foreach (DataTable dataTable in dataSet.Tables)
                                            dataTable.EndLoadData();
                                    }
                                }

                                recipeExecuter.MergeAndUpdateData(dataSet);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

#endif

        BaseObjectState CreateRecipe(Uri uri)
        {
            var recipeExecuter = recipeServer.GetRecipeExecuter(uri);
            if (recipeExecuter != null)
            {
                //var nodeId = FromGuidToNodeId(recipeExecuter.RecipeDocument.RecipeEntity.NodeId);
                //if (mapNodeIdToNodeState.ContainsKey(nodeId))
                //{
                //    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                //    String.Format(Properties.Resources.DuplicatedRecipeNodeId, nodeId, recipeExecuter.RecipeDocument.RecipeEntity.Name),
                //    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                //    return null;
                //}

                var path = recipeServer.GetRecipeFolderPath(uri);
                var parentFolder = CreateFolderPath(path, baseFolderTags);
                var recipe = CreateRecipe(recipeExecuter.RecipeDocument.RecipeEntity, parentFolder, uri);
                parentFolder.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(recipe);

                recipeExecuter.AuditTraceMessage += (s, e) =>
                {
                    String message = null;
                    if (e.LogEntryType == UFRecipeExecuter.AuditLogEntryType.Added)
                        message = String.Format(Properties.Resources.AuditTraceMessageAdded, e.RecipeIndex, recipe.DisplayName);
                    else if (e.LogEntryType == UFRecipeExecuter.AuditLogEntryType.Deleted)
                        message = String.Format(Properties.Resources.AuditTraceMessageDeleted, e.RecipeIndex, recipe.DisplayName);
                    else if (e.LogEntryType == UFRecipeExecuter.AuditLogEntryType.Modified)
                        message = String.Format(Properties.Resources.AuditTraceMessageModified, e.RecipeIndex, recipe.DisplayName);
                    else if (e.LogEntryType == UFRecipeExecuter.AuditLogEntryType.Activated)
                        message = String.Format(Properties.Resources.AuditTraceMessageActivated, e.RecipeIndex, recipe.DisplayName);

                    if (message != null)
                    {
                        var systemEvent = new SystemEvent();
                        systemEvent.evtype = ObjectTypeIds.AuditEventType;
                        systemEvent.details = e.Details;
                        systemEvent.username = e.UserName;
                        systemEvent.comment = e.UserComment;
                        RaiseSystemEvents(null, Utilities.Properties.Resources.RecipeService, message, EventSeverity.Medium, DateTime.UtcNow, systemEvent, addtosystemlog: true);
                    }
                };

                AddPredefinedNode(SystemContext, recipe);
                mapNodeIdToNodeState.Add(recipe.NodeId, recipe);

                return recipe;
            }

            return null;
        }

        BaseObjectState CreateRecipe(UFRecipeEntity recipeEntity, NodeState parentFolder, Uri recipeUri)
        {
            CreateRecipePrototype();

            BaseObjectState baseObject = new BaseObjectState(parentFolder);
            baseObject.Handle = recipeUri;
            baseObject.BrowseName = new QualifiedName(recipeEntity.RecipeName, NamespaceIndex);
            baseObject.DisplayName = baseObject.BrowseName.Name;
            baseObject.SymbolicName = baseObject.BrowseName.Name;
            baseObject.TypeDefinitionId = recipeBaseObjectType.NodeId;
            baseObject.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            //baseObject.NodeId = FromGuidToNodeId(recipeEntity.NodeId);
            baseObject.NodeId = New(SystemContext, baseObject);

            var accessSettings = new AccessSettings()
            {
                IsAuditTraceEnabled = recipeEntity.AuditTraceEnabled,
                IsCommentRequiredOnAudit = recipeEntity.EnterCommentOnAudit,
                IsPasswordRequiredOnAudit = recipeEntity.EnterPasswordOnAudit,
                MinAccessLevelRequiredOnAudit = recipeEntity.MinAccessLevelRequiredOnAudit
            };

            mapNodeStateToAccessSettings.Add(baseObject, accessSettings);

            var propIsAuditTraceEnabled = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.IsAuditTraceEnabled, accessSettings.IsAuditTraceEnabled);
            var propIsAuditCommentRequired = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.IsCommentRequiredOnAudit, accessSettings.IsCommentRequiredOnAudit);
            var propIsAuditPasswordRequired = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.IsPasswordRequiredOnAudit, accessSettings.IsPasswordRequiredOnAudit);
            var propIsAuditLevelRequired = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.MinAccessLevelRequiredOnAudit, accessSettings.MinAccessLevelRequiredOnAudit);
            var propAuditTraceUserName = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.LastUserNameOnAudit, accessSettings.LastUserNameOnAudit);
            var propAuditTraceComment = AddProperty(baseObject, RecipeUAServerInfo.BrowserNames.LastCommentOnAudit, accessSettings.LastCommentOnAudit);

            propIsAuditTraceEnabled.OnReadValue = (ISystemContext context,
                NodeState node,
                NumericRange indexRange,
                QualifiedName dataEncoding,
                ref object value,
                ref StatusCode statusCode,
                ref DateTime timestamp) =>
            {
                value = accessSettings.IsAuditTraceEnabled;
                return StatusCodes.Good;
            };

            propIsAuditCommentRequired.OnReadValue = (ISystemContext context,
               NodeState node,
               NumericRange indexRange,
               QualifiedName dataEncoding,
               ref object value,
               ref StatusCode statusCode,
               ref DateTime timestamp) =>
            {
                value = accessSettings.IsCommentRequiredOnAudit;
                return StatusCodes.Good;
            };

            propIsAuditPasswordRequired.OnReadValue = (ISystemContext context,
               NodeState node,
               NumericRange indexRange,
               QualifiedName dataEncoding,
               ref object value,
               ref StatusCode statusCode,
               ref DateTime timestamp) =>
            {
                value = accessSettings.IsPasswordRequiredOnAudit;
                return StatusCodes.Good;
            };

            propIsAuditLevelRequired.OnReadValue = (ISystemContext context,
               NodeState node,
               NumericRange indexRange,
               QualifiedName dataEncoding,
               ref object value,
               ref StatusCode statusCode,
               ref DateTime timestamp) =>
            {
                value = accessSettings.MinAccessLevelRequiredOnAudit;
                return StatusCodes.Good;
            };

            propAuditTraceUserName.OnReadValue = (ISystemContext context,
                NodeState node,
                NumericRange indexRange,
                QualifiedName dataEncoding,
                ref object value,
                ref StatusCode statusCode,
                ref DateTime timestamp) =>
            {
                value = accessSettings.LastUserNameOnAudit;
                return StatusCodes.Good;
            };
            mapNodeIdToNodeState.Add(propAuditTraceUserName.NodeId, propAuditTraceUserName);

            propAuditTraceComment.OnReadValue = (ISystemContext context,
                NodeState node,
                NumericRange indexRange,
                QualifiedName dataEncoding,
                ref object value,
                ref StatusCode statusCode,
                ref DateTime timestamp) =>
            {
                value = accessSettings.LastCommentOnAudit;
                return StatusCodes.Good;
            };
            mapNodeIdToNodeState.Add(propAuditTraceComment.NodeId, propAuditTraceComment);

            CreateStatusVariable(baseObject);
            CreateRecipeMethods(baseObject, recipeEntity.UserAccessLevel);

            return baseObject;
        }

        void CreateRecipePrototype()
        {
            if (recipeBaseObjectType != null)
                return;

            recipeBaseObjectType = new BaseObjectTypeState();

            recipeBaseObjectType.SuperTypeId = NodeId.Create(Opc.Ua.ObjectTypes.BaseObjectType, Opc.Ua.Namespaces.OpcUa, SystemContext.NamespaceUris);
            recipeBaseObjectType.WriteMask = AttributeWriteMask.None;
            recipeBaseObjectType.UserWriteMask = AttributeWriteMask.None;
            recipeBaseObjectType.IsAbstract = false;

            recipeBaseObjectType.NodeId = FromGuidToNodeId(RecipeUAServerInfo.Guids.RecipesPrototypeGuid);
            recipeBaseObjectType.BrowseName = new QualifiedName(RecipeUAServerInfo.BrowserNames.RecipePrototypeName, NamespaceIndex);
            recipeBaseObjectType.DisplayName = recipeBaseObjectType.BrowseName.Name;
            recipeBaseObjectType.SymbolicName = recipeBaseObjectType.BrowseName.Name;
            recipeBaseObjectType.Description = RecipeUAServerInfo.BrowserNames.RecipePrototypeDesc;

            CreateStatusVariable(recipeBaseObjectType);
            CreateRecipeMethods(recipeBaseObjectType, null);
            
            AddPredefinedNode(SystemContext, recipeBaseObjectType);
        }

        BaseObjectState CreateFolderPath(String path, BaseObjectState parentFolder)
        {
            if (String.IsNullOrEmpty(path))
                return parentFolder;
            else if (mapFolders.ContainsKey(path))
                return mapFolders[path];

            String stringPath = null;
            var folders = path.Split(System.IO.Path.DirectorySeparatorChar);
            foreach (var folder in folders)
            {
                if (stringPath == null)
                    stringPath = folder;
                else
                    stringPath = String.Format("{0}{1}{2}", stringPath, System.IO.Path.DirectorySeparatorChar, folder);
                if (mapFolders.ContainsKey(stringPath))
                {
                    parentFolder = mapFolders[stringPath];
                }
                else
                {
                    parentFolder = CreateFolder(folder, parentFolder);
                    mapFolders.Add(stringPath, parentFolder);
                }
            }

            return parentFolder;
        }

        BaseObjectState CreateFolder(String folderName, BaseObjectState parentFolder)
        {
            var folder = new FolderState(parentFolder) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(folderName, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.NodeId = New(SystemContext, folder);
            parentFolder.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            parentFolder.AddChild(folder);

            AddPredefinedNode(SystemContext, folder);
            mapNodeIdToNodeState.Add(folder.NodeId, folder);

            return folder;
        }

        PropertyState AddProperty(NodeState parent, String browseName, Object value)
        {
            var property = new PropertyState(parent)
            {
                ReferenceTypeId = ReferenceTypeIds.HasProperty,
                TypeDefinitionId = VariableTypeIds.PropertyType,
                SymbolicName = browseName,
                BrowseName = browseName,
                Value = value,
                Description = null,
                WriteMask = 0,
                UserWriteMask = 0,
                DataType = TypeInfo.GetDataTypeId(value),
                ValueRank = ValueRanks.Scalar,
                AccessLevel = Opc.Ua.AccessLevels.CurrentRead,
                UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead,
                MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
                Historizing = false
            };

            property.DisplayName = new LocalizedText(browseName, string.Empty, browseName);
            parent.AddChild(property);

            property.NodeId = ModelUtils.ConstructIdForComponent(property, NamespaceIndex);

            return property;
        }

        void CreateStatusVariable(NodeState parent)
        {
            var variable = new DataItemState(parent);
            variable.BrowseName = new QualifiedName(RecipeUAServerInfo.BrowserNames.RecipeExecutionStatateName, NamespaceIndex);
            variable.SymbolicName = variable.BrowseName.Name;
            variable.DisplayName = variable.BrowseName.Name;
            variable.Description = RecipeUAServerInfo.BrowserNames.RecipeExecutionStatateDesc;
            variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            variable.Historizing = false;
            variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            variable.NodeId = New(SystemContext, variable);

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            variable.Create(SystemContext, variable.NodeId, null, null, !isPrototype);

            variable.DataType = DataTypeIds.Boolean;
            variable.ValueRank = ValueRanks.OneDimension;
            List<uint> dimList = new List<uint>(1);
            dimList.Add(32);
            variable.ArrayDimensions = new ReadOnlyList<uint>(dimList);

            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            var array = TypeInfo.CreateArray(builtinType, 32);
            variable.Value = array;
            variable.Timestamp = DateTime.UtcNow;
            variable.StatusCode = StatusCodes.BadWaitingForInitialData;

            parent.AddChild(variable);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                mapNodeIdToNodeState.Add(variable.NodeId, variable);
            }
        }

        void UpdateStatusVariable(BaseVariableState variable, uint newValue)
        {
            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            var array = TypeInfo.CreateArray(builtinType, 32);

            int mask = 1;
            for (int ii = 0; ii < 32; ii++)
            {
                var bit = newValue & mask;
                array.SetValue(TypeInfo.Cast(bit, builtinType), ii);
                mask = mask << 1;
            }

            variable.Value = array;
            variable.Timestamp = DateTime.UtcNow;
            variable.StatusCode = StatusCodes.Good;
            variable.UpdateChangeMasks(NodeStateChangeMasks.Value);
            variable.ClearChangeMasks(SystemContext, true);
        }

        /// <summary>
        /// Check device address for the recipe and print in recipe log the configuration errors
        /// </summary>
        /// <param name="recipeExecuter">Recipe Executer associated to the recipe</param>
        void DeviceAddressCheck(UFRecipeExecuter.UFRecipeExecuter recipeExecuter)
        {
            if (!recipeExecuter.RecipeDocument.RecipeEntity.IsWritable() &&
                recipeExecuter.RecipeDocument.RecipeEntity.LinkType == LinkType.WrongLink)
                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidRecipeDeviceAddressForWrite,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);

            if (!recipeExecuter.RecipeDocument.RecipeEntity.IsReadable() &&
                recipeExecuter.RecipeDocument.RecipeEntity.LinkType == LinkType.WrongLink)
                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidRecipeDeviceAddressForRead,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);

            //Check Groups
            foreach (var group in recipeExecuter.RecipeDocument.RecipeEntity.Groups)
            {
                if (!group.IsWritable() && !String.IsNullOrEmpty(group.StartingAddress))
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidGroupDeviceAddressForWrite, group.GroupName,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);

                if (!group.IsReadable() && !String.IsNullOrEmpty(group.StartingAddress))
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidGroupDeviceAddressForRead, group.GroupName,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);
            }

            //Check DataValues
            foreach (var dataValueEntity in recipeExecuter.RecipeDocument.RecipeEntity.GetFlatDataValuesCollection())
            {
                if (!String.IsNullOrEmpty(dataValueEntity.StartingAddress) && !dataValueEntity.IsWritable())
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidDataValueDeviceAddressForWrite, dataValueEntity.DataValueName,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);

                if (!String.IsNullOrEmpty(dataValueEntity.StartingAddress) && !dataValueEntity.IsReadable())
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.InvalidDataValueDeviceAddressForRead, dataValueEntity.DataValueName,
                        recipeExecuter.RecipeDocument.RecipeEntity.Name), System.Diagnostics.EventLogEntryType.Error,
                        LoggerDestination.RecipeService);
            }
        }   

        void CreateRecipeMethods(NodeState parent, int? userAccessLevel)
        {
            CreateLoadFromDBMethod(parent, userAccessLevel);
            CreateSaveToDBMethod(parent, userAccessLevel);
            CreateDeleteFromDBMethod(parent, userAccessLevel);
            CreateWriteToPLCMethod(parent, userAccessLevel);
            CreateReadFromPLCMethod(parent, userAccessLevel);
            CreateExportToFileMethod(parent, userAccessLevel);
            CreateImportFromFileMethod(parent, userAccessLevel);
            CreateUpdateRecipeTagsMethod(parent, userAccessLevel);
            CreateReadDataMethod(parent, userAccessLevel);
            CreateWriteDataMethod(parent, userAccessLevel);
            CreateGetInDataServerValuesMethod(parent, userAccessLevel);
            CreateGetOutDataServerValuesMethod(parent, userAccessLevel);
#if !NET_STANDARD
            CreateReadTransactionLogMethod(parent, userAccessLevel);
#endif
        }

        void CreateLoadFromDBMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeLoadFromDBName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeLoadFromDBDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeLoadFromDBName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnLoadFromDB;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateSaveToDBMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeSaveToDBName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeSaveToDBDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeSaveToDBName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Guids",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnSaveToDB;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateDeleteFromDBMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeDeleteFromDBName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeDeleteFromDBDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeDeleteFromDBName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnDeleteFromDB;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateWriteToPLCMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeWriteToPLCName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeWriteToPLCDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeWriteToPLCName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Guids",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "WriteTimeout",
                DataType = DataTypeIds.Integer,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnWriteToPLC;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateReadFromPLCMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeReadFromPLCName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeReadFromPLCDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeReadFromPLCName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "ReadTimeout",
                DataType = DataTypeIds.Integer,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnReadFromPLC;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateExportToFileMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeExportToFileName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeExportToFileDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeExportToFileName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            //args.Add(new Argument()
            //{
            //    Name = "FilePath",
            //    DataType = DataTypeIds.String,
            //    ValueRank = ValueRanks.Scalar,
            //    ArrayDimensions = null
            //});

            //args.Add(new Argument()
            //{
            //    Name = "IsSynchro",
            //    DataType = DataTypeIds.Boolean,
            //    ValueRank = ValueRanks.Scalar,
            //    ArrayDimensions = null
            //});

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeExportToFileName, BrowseNames.OutputArguments), NamespaceIndex);

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "Guids",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnExportToFile;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateImportFromFileMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeImportFromFileName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeImportFromFileDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeImportFromFileName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "RecipeIndex",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            //args.Add(new Argument()
            //{
            //    Name = "FilePath",
            //    DataType = DataTypeIds.String,
            //    ValueRank = ValueRanks.Scalar,
            //    ArrayDimensions = null
            //});

            args.Add(new Argument()
            {
                Name = "Guids",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            args.Add(new Argument()
            {
                Name = "IsSynchro",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnImportFromFile;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateUpdateRecipeTagsMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeUpdateRecipeTagsName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeUpdateRecipeTagsDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnUpdateRecipeTags;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateReadDataMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeReadDataName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeReadDataDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeReadDataName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();

            args.Add(new Argument()
            {
                Name = "DataRowState",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "ContinueOnError",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeReadDataName, BrowseNames.OutputArguments), NamespaceIndex);

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "FillError",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "MapWrongValues",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnReadDataSet;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateWriteDataMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeWriteDataName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeWriteDataDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeWriteDataName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "DataRowState",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "RecipeId",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnWriteDataSet;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateGetInDataServerValuesMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeGetInDataServerValuesName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeGetInDataServerValuesDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeGetInDataServerValuesName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "RecipeId",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "ReadTimeout",
                DataType = DataTypeIds.Integer,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeGetInDataServerValuesName, BrowseNames.OutputArguments), NamespaceIndex);

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnGetInDataServerValues;
                var isExecutable = IsGetInDataServerValuesExecutable(methodBase);
                methodBase.UserExecutable = isExecutable;
                methodBase.Executable = isExecutable;
                if (isExecutable)
                {
                    methodBase.OnReadExecutable = IsGetInDataServerValuesExecutable;
                    methodBase.OnReadUserExecutable = IsGetInDataServerValuesExecutable;
                }
                else
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.DisableReadCommandMessage, instance.SymbolicName),
                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.RecipeService);
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }

        void CreateGetOutDataServerValuesMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeGetOutDataServerValuesName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeGetOutDataServerValuesDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeGetOutDataServerValuesName, BrowseNames.InputArguments), NamespaceIndex);

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "RecipeId",
                DataType = DataTypeIds.Guid,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "WriteTimeout",
                DataType = DataTypeIds.Integer,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnGetOutDataServerValues;
                var isExecutable = IsGetOutDataServerValuesExecutable(methodBase);
                methodBase.UserExecutable = isExecutable;
                methodBase.Executable = isExecutable;
                if (isExecutable)
                {
                    methodBase.OnReadExecutable = IsGetOutDataServerValuesExecutable;
                    methodBase.OnReadUserExecutable = IsGetOutDataServerValuesExecutable;
                }
                else
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        String.Format(Properties.Resources.DisableWriteCommandMessage, instance.SymbolicName),
                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.RecipeService);
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }
#if !NET_STANDARD
        void CreateReadTransactionLogMethod(NodeState parent, int? userAccessLevel)
        {
            var methodBase = new MethodState(parent);
            methodBase.BrowseName = new QualifiedName(RecipeUAServerInfo.MethodNames.RecipeReadTransactionLog, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.Description = RecipeUAServerInfo.MethodNames.RecipeReadTransactionLogDesc;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.NodeId = New(SystemContext, methodBase);
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            if (userAccessLevel != null && userAccessLevel.HasValue)
            {
                var accessSettings = new AccessSettings()
                {
                    UserAccessLevel = userAccessLevel.Value
                };
                mapNodeStateToAccessSettings.Add(methodBase, accessSettings);
            }

            // initialize the variable from the type model.
            var isPrototype = parent is BaseObjectTypeState;
            methodBase.Create(SystemContext, methodBase.NodeId, null, null, !isPrototype);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeReadTransactionLog, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "DateTime",
                DataType = DataTypeIds.DateTime,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });


            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);


            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", RecipeUAServerInfo.MethodNames.RecipeReadTransactionLog, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "OID",
                DataType = DataTypeIds.Int32,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventType",
                DataType = DataTypeIds.UInt16,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "RecipeName",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "XmlDataSet",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventDateTime",
                DataType = DataTypeIds.DateTime,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventDateTimeUtc",
                DataType = DataTypeIds.DateTime,
                ValueRank = ValueRanks.OneDimension,
                ArrayDimensions = null
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            parent.AddChild(methodBase);
            parent.AddChild(methodBase);
            if (parent is BaseObjectState)
            {
                var instance = parent as BaseObjectState;
                instance.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                methodBase.OnCallMethod = OnReadTransactionLog;
                mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
            }
        }
#endif
        #endregion

        #region Call Methods
        ServiceResult OnLoadFromDB(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[1]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Load,
                Index = recipeIndex,
                IsSynchro = isSynchro
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnSaveToDB(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            // keys
            var keys = inputArguments[1] as IList<Uuid>;
            if (keys == null)
                return StatusCodes.BadTypeMismatch;

            // values
            var values = inputArguments[2] as IList<Opc.Ua.Variant>;
            if (values == null)
                return StatusCodes.BadTypeMismatch;

            if (keys.Count != values.Count)
                return StatusCodes.BadInvalidArgument;

            var mapValues = new Dictionary<Guid, Opc.Ua.Variant>();
            for (int ii = 0; ii < keys.Count; ii++)
                mapValues.Add(keys[ii], values[ii]);

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[3]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            // userComment argument
            var userComment = inputArguments[4]?.ToString();
            //if (String.IsNullOrEmpty(userComment))
            //    return StatusCodes.BadInvalidArgument;

            var result = ValidateAuditTracePermission(context, method.Parent, userComment);
            if (StatusCode.IsBad(result.StatusCode))
                return result;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Save,
                Index = recipeIndex,
                Values = mapValues,
                IsSynchro = isSynchro,
                UserName = GetUserName(context),
                UserComment = userComment
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnDeleteFromDB(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[1]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            // userComment argument
            var userComment = inputArguments[2]?.ToString();
            //if (String.IsNullOrEmpty(userComment))
            //    return StatusCodes.BadInvalidArgument;

            var result = ValidateAuditTracePermission(context, method.Parent, userComment);
            if (StatusCode.IsBad(result.StatusCode))
                return result;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Remove,
                Index = recipeIndex,
                IsSynchro = isSynchro,
                UserName = GetUserName(context),
                UserComment = userComment
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnWriteToPLC(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            // keys pair values
            var keys = inputArguments[1] as IList<Uuid>;
            var values = inputArguments[2] as IList<DataValue>;
            var mapValues = new Dictionary<Guid, Opc.Ua.Variant>();
            if (keys != null && values != null && keys.Count == values.Count)
            {
                for (int ii = 0; ii < keys.Count; ii++)
                    mapValues.Add(keys[ii], values[ii]);
            }

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[3]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            // write timeout
            int writeTimeout = 0;
            if (!int.TryParse(inputArguments[4]?.ToString(), out writeTimeout))
                return StatusCodes.BadInvalidArgument;

            // userComment argument
            var userComment = inputArguments[5]?.ToString();
            //if (String.IsNullOrEmpty(userComment))
            //    return StatusCodes.BadInvalidArgument;

            var result = ValidateAuditTracePermission(context, method.Parent, userComment);
            if (StatusCode.IsBad(result.StatusCode))
                return result;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Activate,
                Index = recipeIndex,
                IsSynchro = isSynchro,
                Values = mapValues,
                Timeout = writeTimeout,
                UserName = GetUserName(context),
                UserComment = userComment
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnReadFromPLC(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[1]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            // read timeout
            int readTimeout = 0;
            if (!int.TryParse(inputArguments[2]?.ToString(), out readTimeout))
                return StatusCodes.BadInvalidArgument;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Read,
                Index = recipeIndex,
                IsSynchro = isSynchro,
                Timeout = readTimeout
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnExportToFile(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            ////recipe file
            //var filePath = inputArguments[1]?.ToString();
            //if (String.IsNullOrEmpty(filePath))
            //    return StatusCodes.BadInvalidArgument;

            //// is synchro
            //bool isSynchro = false;
            //if (!bool.TryParse(inputArguments[1]?.ToString(), out isSynchro))
            //    return StatusCodes.BadInvalidArgument;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Export,
                Index = recipeIndex,
                IsSynchro = true /* isSynchro */
            };

            outputArguments.Clear();
            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);
            else if (args.Values == null)
                return StatusCodes.BadArgumentsMissing;

            var guids = new UuidCollection();
            foreach (var guid in args.Values.Keys)
                guids.Add(new Uuid(guid));
            outputArguments.Add(guids);

            var values = new VariantCollection();
            foreach (var datavalue in args.Values.Values)
                values.Add(datavalue);
            outputArguments.Add(values);

            return StatusCodes.Good;
        }

        ServiceResult OnImportFromFile(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // recipe index
            var recipeIndex = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(recipeIndex))
                return StatusCodes.BadInvalidArgument;

            //// recipe file
            //var filePath = inputArguments[1]?.ToString();
            //if (String.IsNullOrEmpty(filePath))
            //    return StatusCodes.BadInvalidArgument;

            // keys
            var keys = inputArguments[1] as IList<Uuid>;
            if (keys == null)
                return StatusCodes.BadTypeMismatch;

            // values
            var values = inputArguments[2] as IList<Opc.Ua.Variant>;
            if (values == null)
                return StatusCodes.BadTypeMismatch;

            if (keys.Count != values.Count)
                return StatusCodes.BadInvalidArgument;

            var mapValues = new Dictionary<Guid, Opc.Ua.Variant>();
            for (int ii = 0; ii < keys.Count; ii++)
                mapValues.Add(keys[ii], values[ii]);

            // is synchro
            bool isSynchro = false;
            if (!bool.TryParse(inputArguments[3]?.ToString(), out isSynchro))
                return StatusCodes.BadInvalidArgument;

            var args = new UFRecipeExecutionContext.RecipeExecutionContext()
            {
                CommandType = UFRecipeExecutionContext.RecipeCommandType.Import,
                Index = recipeIndex,
                Values = mapValues,
                IsSynchro = isSynchro
            };

            recipeServer.Execute(recipeUri, DocumentManager.ComponentService.ExecutionMode.Normal, args);
            if (args.Exception != null)
                return new ServiceResult(args.Exception);

            return StatusCodes.Good;
        }

        ServiceResult OnUpdateRecipeTags(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            recipeExecuter.ForceUpdateRecipeTags();

            return StatusCodes.Good;
        }

        ServiceResult OnReadDataSet(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // DataRowState argument
            var rowState = DataRowState.Unchanged;
            Enum.TryParse<DataRowState>(inputArguments[0]?.ToString(), out rowState);

            // ContinueOnError argument
            bool continueOnError = false;
            if (!bool.TryParse(inputArguments[1]?.ToString(), out continueOnError))
                return StatusCodes.BadInvalidArgument;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            try
            {
                outputArguments.Clear();
                String fillError = null;
                Dictionary<string, List<Tuple<string, string>>> mapWrongValues = new Dictionary<string, List<Tuple<string, string>>>();
                using (var dataSet = recipeExecuter.CreateDataSet(fill: true, out fillError, ref mapWrongValues, bContinueOnError: continueOnError))
                {
                    var xmlDataSet = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    var dataChanges = dataSet.GetChanges(rowState);
                    if (dataChanges != null)
                    {
                        using (var xmlWriter = System.Xml.XmlWriter.Create(xmlDataSet, settings))
                        {
#if DEBUG
                            //dataChanges.WriteXmlSchema(@"C:\Temp\DataSetSchema_Read.xml");
                            //dataChanges.WriteXml(@"C:\Temp\DataSet_Read.xml", XmlWriteMode.IgnoreSchema);
#endif
                            dataChanges.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema);
                            outputArguments.Add(xmlDataSet.ToString());
                            outputArguments.Add(fillError ?? String.Empty);
                            outputArguments.Add(mapWrongValues.ToXml().ToString());
                        }
                    }
                    else if (!String.IsNullOrEmpty(fillError))
                        outputArguments.Add(fillError);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(ex);
            }

            return StatusCodes.Good;
        }

        ServiceResult OnWriteDataSet(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // XmlDataSet argument
            var xmlDataSet = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(xmlDataSet))
                return StatusCodes.BadInvalidArgument;

            // DataRowState argument
            var rowState = DataRowState.Unchanged;
            Enum.TryParse<DataRowState>(inputArguments[1]?.ToString(), out rowState);

            // RecipeId argument
            if (!(inputArguments[2] is Uuid))
                return StatusCodes.BadTypeMismatch;
            var recipeId = new Guid(((Uuid)inputArguments[2]).GuidString);

            // userComment argument
            var userComment = inputArguments[3]?.ToString();
            //if (String.IsNullOrEmpty(userComment))
            //    return StatusCodes.BadInvalidArgument;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            var result = ValidateAuditTracePermission(context, method.Parent, userComment);
            if (StatusCode.IsBad(result.StatusCode))
                return result;

            try
            {
                using (var dataSet = recipeExecuter.CreateDataSet(fill: false))
                {
                    System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings()
                    {
                        ConformanceLevel = System.Xml.ConformanceLevel.Document,
                        CloseInput = true,
                        CheckCharacters = false
                    };

                    using (var stringReader = new System.IO.StringReader(xmlDataSet))
                    {
                        using (var xmlReader = System.Xml.XmlReader.Create(stringReader, settings))
                        {
                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.BeginLoadData();

                            // **********************************************************
                            // Moving two node away is needed for reading all records
                            xmlReader.MoveToContent();
                            xmlReader.Read();
                            // **********************************************************
                            dataSet.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);
                            UFRecipeExecuter.UFRecipeExecuter.ChangeRowsState(dataSet, rowState);

                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.EndLoadData();

                            recipeExecuter.AcceptUpdateData(dataSet, recipeId, GetUserName(context), userComment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(ex);
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Check if values are redable from device
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="inputArguments"></param>
        /// <param name="outputArguments"></param>
        /// <returns></returns>
        ServiceResult OnGetInDataServerValues(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // XmlDataSet argument
            var xmlDataSet = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(xmlDataSet))
                return StatusCodes.BadInvalidArgument;

            // RecipeId argument
            if (!(inputArguments[1] is Uuid))
                return StatusCodes.BadTypeMismatch;
            var recipeId = new Guid(((Uuid)inputArguments[1]).GuidString);

            // read timeout
            int readTimeout = 0;
            if (!int.TryParse(inputArguments[2]?.ToString(), out readTimeout))
                return StatusCodes.BadInvalidArgument;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            try
            {
                using (var dataSet = recipeExecuter.CreateDataSet(fill: false))
                {
                    System.Xml.XmlReaderSettings readerSettings = new System.Xml.XmlReaderSettings()
                    {
                        ConformanceLevel = System.Xml.ConformanceLevel.Document,
                        CloseInput = true,
                        CheckCharacters = false
                    };

                    using (var stringReader = new System.IO.StringReader(xmlDataSet))
                    {
                        using (var xmlReader = System.Xml.XmlReader.Create(stringReader, readerSettings))
                        {
                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.BeginLoadData();

                            // **********************************************************
                            // Moving two node away is needed for reading all records
                            xmlReader.MoveToContent();
                            xmlReader.Read();
                            // **********************************************************
                            dataSet.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);
                            UFRecipeExecuter.UFRecipeExecuter.ChangeRowsState(dataSet, DataRowState.Unchanged);

                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.EndLoadData();

                            recipeExecuter.GetInDataServerValues(dataSet, recipeId, readTimeout, CancellationToken.None);
                        }
                    }

                    outputArguments.Clear();
                    var xmlBuilder = new System.Text.StringBuilder();
                    System.Xml.XmlWriterSettings writerSettings = new System.Xml.XmlWriterSettings()
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = false,
                        CloseOutput = true,
                        CheckCharacters = false
                    };

                    var dataChanges = dataSet.GetChanges(DataRowState.Modified);
                    if (dataChanges != null)
                    {
                        using (var xmlWriter = System.Xml.XmlWriter.Create(xmlBuilder, writerSettings))
                        {
                            dataChanges.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema);
                            outputArguments.Add(xmlBuilder.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(ex);
            }

            return StatusCodes.Good;
        }

        ServiceResult IsGetInDataServerValuesExecutable(
            ISystemContext context,
            NodeState node,
            ref bool value)
        {
            var method = node as MethodState;
            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            value = recipeExecuter.CanGetInDataServerValues();

            return ServiceResult.Good;
        }

        bool IsGetInDataServerValuesExecutable(NodeState node)
        {
            var method = node as MethodState;
            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return false;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return false;

            var ret = recipeExecuter.RecipeDocument.RecipeEntity.IsReadable();
            if (!ret)
            {
                ret = (from c in recipeExecuter.RecipeDocument.RecipeEntity.Groups
                       where c.IsReadable()
                       select c).FirstOrDefault() != null;
                if (!ret)
                    ret = (from c in recipeExecuter.RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                           where c.IsReadable() || c.IsTagIOReferenceValid()
                           select c).FirstOrDefault() != null;
            }

            return ret;
        }

        /// <summary>
        /// Check if values are writable on device
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="inputArguments"></param>
        /// <param name="outputArguments"></param>
        /// <returns></returns>
        ServiceResult OnGetOutDataServerValues(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            // XmlDataSet argument
            var xmlDataSet = inputArguments[0]?.ToString();
            if (String.IsNullOrEmpty(xmlDataSet))
                return StatusCodes.BadInvalidArgument;

            // RecipeId argument
            if (!(inputArguments[1] is Uuid))
                return StatusCodes.BadTypeMismatch;
            var recipeId = new Guid(((Uuid)inputArguments[1]).GuidString);

            // write timeout
            int writeTimeout = 0;
            if (!int.TryParse(inputArguments[2]?.ToString(), out writeTimeout))
                return StatusCodes.BadInvalidArgument;

            // userComment argument
            var userComment = inputArguments[3]?.ToString();
            //if (String.IsNullOrEmpty(userComment))
            //    return StatusCodes.BadInvalidArgument;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            var result = ValidateAuditTracePermission(context, method.Parent, userComment);
            if (StatusCode.IsBad(result.StatusCode))
                return result;

            try
            {
                using (var dataSet = recipeExecuter.CreateDataSet(fill: false))
                {
                    System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings()
                    {
                        ConformanceLevel = System.Xml.ConformanceLevel.Document,
                        CloseInput = true,
                        CheckCharacters = false
                    };

                    using (var stringReader = new System.IO.StringReader(xmlDataSet))
                    {
                        using (var xmlReader = System.Xml.XmlReader.Create(stringReader, settings))
                        {
                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.BeginLoadData();

                            // **********************************************************
                            // Moving two node away is needed for reading all records
                            xmlReader.MoveToContent();
                            xmlReader.Read();
                            // **********************************************************
                            dataSet.ReadXml(xmlReader, XmlReadMode.IgnoreSchema);
                            UFRecipeExecuter.UFRecipeExecuter.ChangeRowsState(dataSet, DataRowState.Unchanged);

                            foreach (DataTable dataTable in dataSet.Tables)
                                dataTable.EndLoadData();

                            recipeExecuter.GetOutDataServerValues(dataSet, recipeId, writeTimeout, CancellationToken.None, GetUserName(context), userComment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult(ex);
            }

            return StatusCodes.Good;
        }

        ServiceResult IsGetOutDataServerValuesExecutable(
            ISystemContext context,
            NodeState node,
            ref bool value)
        {
            var method = node as MethodState;
            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            value = recipeExecuter.CanGetOutDataServerValues();

            return ServiceResult.Good;
        }

        bool IsGetOutDataServerValuesExecutable(NodeState node)
        {
            var method = node as MethodState;
            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return false;

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return false;

            bool ret = recipeExecuter.RecipeDocument.RecipeEntity.IsWritable();
            if (!ret)
            {
                ret = (from c in recipeExecuter.RecipeDocument.RecipeEntity.Groups
                       where c.IsWritable()
                       select c).FirstOrDefault() != null;
                if (!ret)
                    ret = (from c in recipeExecuter.RecipeDocument.RecipeEntity.GetFlatDataValuesCollection()
                           where c.IsWritable() || c.IsTagIOReferenceValid()
                           select c).FirstOrDefault() != null;
            }

            return ret;
        }

#if !NET_STANDARD
        /// <summary>
        /// Parse and check input arguments and return output arguments for ReadTransactionLog OPC UA method
        /// </summary>
        /// <param name="context"></param>
        /// <param name="method"></param>
        /// <param name="inputArguments"></param>
        /// <param name="outputArguments"></param>
        /// <returns>Status code which describes the execution's service result</returns>
        ServiceResult OnReadTransactionLog(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (!CanUserCallMethod(context, method))
                return new ServiceResult(StatusCodes.BadUserAccessDenied, String.Format(Properties.Resources.BadUserAccessDenied, method.SymbolicName));

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            var recipeUri = method.Parent.Handle as Uri;
            if (recipeUri == null)
                return StatusCodes.BadMethodInvalid;

            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(inputArguments[0]);
                if (dateTime == null)
                    return StatusCodes.BadInvalidArgument;
            }
            catch
            {
                return StatusCodes.BadInvalidArgument;
            }

            var recipeExecuter = recipeServer.GetRecipeExecuter(recipeUri);
            if (recipeExecuter == null)
                return StatusCodes.BadMethodInvalid;

            var res = recipeExecuter.ReadTransactionLog(dateTime, method.Parent.BrowseName.Name);
            try
            {
                var oids = new List<Int32>();
                var eventsType = new List<UInt16>();
                var xmlDataSets = new List<string>();
                var recipeName = new List<string>();
                var eventsDateTime = new List<DateTime>();
                var eventsDateTimeUTC = new List<DateTime>();
                foreach (var value in res)
                {
                    oids.Add(value.Oid);
                    eventsType.Add((UInt16)value.EventType);
                    recipeName.Add(value.RecipeName);
                    xmlDataSets.Add(value.XmlDataSet);
                    eventsDateTime.Add(value.EventDateTime);
                    eventsDateTimeUTC.Add(value.EventDateTimeUtc);
                }

                outputArguments[0] = oids;
                outputArguments[1] = eventsType;
                outputArguments[2] = recipeName;
                outputArguments[3] = xmlDataSets;
                outputArguments[4] = eventsDateTime;
                outputArguments[5] = eventsDateTimeUTC;

                return StatusCodes.Good;
            }
            catch 
            {
                return StatusCodes.BadInternalError;
            }
            
        }
#endif
#endregion
    }
}
