/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Server;
using DevExpress.Xpo;
using System.Threading.Tasks;
using DriverBaseInterfaces;
using System.Threading;
using UFUAHistorian;
using UFUAAlarm;
using UFUATagLogger;
using System.Collections;
using System.Diagnostics;
using UFUAModel;
using UFUAServerBase.SystemTags;
using UFUAHistorianModel;
using Utilities.Logger;
using Utilities.Converters;
using Opc.Ua.Utilities;
using DevExpress.Xpo.DB;
using XpoHelpers;
using System.IO;
using DataReader.Helpers;
#if !NET_STANDARD
using RedundancyService;
using System.Speech.Synthesis;
#endif

namespace UFUAServerBase
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class UANodeManager : CustomNodeManager2 // CustomNodeManager2
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public UANodeManager(UAServer uaserver, IServerInternal s, ApplicationConfiguration c)
        :
            base(s, c, Namespaces.ServerModel)
        {
            server = uaserver;
            server.ServerStateChangeEvent += server_ServerStateChangeEvent;
            if (server.IsUserManagerEnabled())
                server.SessionChangedEvent += server_SessionChangedEvent;
            if (!String.IsNullOrEmpty(server.UFUAConfiguration.NamespaceUri))
                SetNamespaces(server.UFUAConfiguration.NamespaceUri);

            if (!String.IsNullOrEmpty(server.UFUAConfiguration.AliasRoot))
                AliasRoot = server.UFUAConfiguration.AliasRoot;
            else
                AliasRoot = UFUAServerInfo.UFUAServerInfo.GetDefaultAliasRootName();

            // SystemContext.SystemHandle = m_system = new UnderlyingSystem();
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            configuration = c.ParseExtension<UAServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (configuration == null)
            {
                configuration = new UAServerConfiguration();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {  
            if (disposing)
            {
                foreach (var driver in server.CommDrivers.Keys)
                {
                    if (server.CommDrivers[driver] is IDisposable)
                        (server.CommDrivers[driver] as IDisposable).Dispose();
                }

#if !NET_STANDARD
                if (synthesizer != null)
                {
                    synthesizer.Dispose();
                    synthesizer = null;
                }
#endif
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        /// <remarks>
        /// This method is called by the NodeState.Create() method which initializes a Node from
        /// the type model. During initialization a number of child nodes are created and need to 
        /// have NodeIds assigned to them. This implementation constructs NodeIds by constructing
        /// strings. Other implementations could assign unique integers or Guids and save the new
        /// Node in a dictionary for later lookup.
        /// </remarks>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            var ret = ModelUtils.ConstructIdForComponent(node, NamespaceIndex);
            /////////////////////////////////////////////////////////////////////
            // http://support.progea.com/Products/default.asp?8162#58969
            if (ret != null)
            {
                lock (Lock)
                {
                    if (PredefinedNodes != null && !PredefinedNodes.ContainsKey(ret))
                    {
                        PredefinedNodes[ret] = node;
                    }
                }
            }
            ////////////////////////////////////////////////////////////////////
            return ret;
        }
        #endregion

        #region NodeManager Utils

        IDataLayer CreateDataLayerAlarmRuntimeSettings(out IDisposable[] objectsToDisposeOnDisconnect, bool retry = true)
        {
            string conn = null;
            try
            {
                conn = XpoHelper.GetConnectionString(server.ActiveConnectionString, "", UFUAServerInfo.Properties.Settings.Default.RuntimeAlrFileName, UFUAServerInfo.Properties.Settings.Default.DefaultFileExtAlr);
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dict.GetDataStoreSchema(typeof(TagPersistence).Assembly);
                return XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect);
                /* http://www.devexpress.com/Support/Center/Question/Details/Q535243
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                var store = XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                dict.GetDataStoreSchema(typeof(TagPersistence).Assembly);
                return new ThreadSafeDataLayer(dict, store);
                */
            }
            
            catch (Exception e)
            {
                string file = XpoHelper.GetDataSourceFilePath(conn);
                if (file != null && File.Exists(file))
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        if (retry)
                            return CreateDataLayerAlarmRuntimeSettings(out objectsToDisposeOnDisconnect, false);
                        File.Delete(file);
                    }
                    catch { }
                }

                var message = String.Format(Properties.Resources.ErrorCreatingAlarmRuntimeDataLayer, UFUAServerInfo.Properties.Settings.Default.RuntimeAlrFileName, e.InnerException.Message);
                var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                Console.WriteLine(logMessage);

                Logger.GetDestinationLog(LoggerDestination.Server).Error(message);
            }

            objectsToDisposeOnDisconnect = new IDisposable[0];

            return null;
        }

        struct alrRunData
        {
            public NodeId nodeId;
            public string text;
        }

        List<alrRunData> GetAllAlrRunData()
        {
            var runSettingList = new List<alrRunData>();
            Parallel.ForEach(mapNodeIdToAlarmStatus.Keys, nodeId =>
            {
                Parallel.ForEach(mapNodeIdToAlarmStatus[nodeId], setting =>
                {
                    var alrRunData = new alrRunData() { nodeId = setting.nodeId.ToString(), text = setting.Message };
                    lock (runSettingList)
                    {
                        runSettingList.Add(alrRunData);
                    }
                });
            });

            return runSettingList;
        }

        bool SaveAlarmRuntimeChange()
        {
            IDisposable[] objectsToDisposeOnDisconnect;
            var idl = CreateDataLayerAlarmRuntimeSettings(out objectsToDisposeOnDisconnect);
            if (idl == null)
                return false;

            try
            {
                using (var ufw = new UnitOfWork(idl))
                {
                    try
                    {
                        ////////////////////////////////////////////////////////////////////////////
                        // delete all first
                        (from p in new XPQuery<AlarmRuntimeSetting>(ufw, true).AsParallel()
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////
                        var runSettingList = GetAllAlrRunData();
                        foreach (var p in runSettingList)
                        {
                            var alrSett = new AlarmRuntimeSetting(ufw) { AlarmStatusNodeId = p.nodeId, AlarmText = p.text };
                        }
                        ufw.CommitChanges();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            finally
            {
                idl.Dispose();
                foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                    obj.Dispose();
            }

            return true;
        }

        List<alrRunData> LoadAlarmRuntimeChange()
        {
            IDisposable[] objectsToDisposeOnDisconnect;
            var idl = CreateDataLayerAlarmRuntimeSettings(out objectsToDisposeOnDisconnect);
            if (idl == null)
                return null;

            var changesList = new List<alrRunData>();
            try
            {
                using (var ufw = new UnitOfWork(idl))
                {
                    var chList = (from c in new XPQuery<AlarmRuntimeSetting>(ufw) select c).ToList();
                    if (chList.Count > 0)
                    {
                        Parallel.ForEach (chList, setting =>
                        {
                            var alr = new alrRunData() { nodeId = setting.AlarmStatusNodeId, text = setting.AlarmText };
                            lock (changesList)
                            {
                                changesList.Add(alr);
                            }
                        });
                    }
                }
            }
            finally
            {
                idl.Dispose();
                foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                    obj.Dispose();
            }

            return changesList;
        }

        ServiceResult UpdateAlarmRuntimeChanges()
        {
            var alrChanges = LoadAlarmRuntimeChange();

            if (alrChanges != null && alrChanges.Count > 0)
            {
                var errors = new List<String>();
                var mapSourceToUpdate = new Dictionary<SourceState, List<AlarmStatus>>();
                Parallel.ForEach(alrChanges, ch =>
                {
                    NodeId tagNodeid = null;
                    //AlarmStatus nodeid
                    if (mapAlarmStatusNodeIdToVariableNodeId.ContainsKey(ch.nodeId))
                        tagNodeid = mapAlarmStatusNodeIdToVariableNodeId[ch.nodeId];

                    if (tagNodeid != null)
                    {
                        AlarmStatus alrStatus = null;
                        if (mapNodeIdToAlarmStatus.ContainsKey(tagNodeid))
                        {
                            alrStatus = (from a in mapNodeIdToAlarmStatus[tagNodeid] where a.nodeId == ch.nodeId select a).FirstOrDefault();

                            if (alrStatus != null)
                            {
                                alrStatus.Message = ch.text;
                                if (mapAlarmStatusToSourceState.ContainsKey(alrStatus))
                                {
                                    var sourceState = mapAlarmStatusToSourceState[alrStatus];
                                    lock (errors)
                                    {
                                        if (!mapSourceToUpdate.ContainsKey(sourceState))
                                            mapSourceToUpdate[sourceState] = new List<AlarmStatus>();
                                        mapSourceToUpdate[sourceState].Add(alrStatus);
                                    }
                                }
                            }
                            else
                            {
                                lock (errors)
                                {
                                    errors.Add(string.Format(Properties.Resources.RuntimeAlarmErrorAlarmNotFound, ch.nodeId.ToString()));//Alarm not found
                                }
                            }
                        }
                        else
                        {
                            lock (errors)
                            {
                                errors.Add(string.Format(Properties.Resources.RuntimeAlarmErrorTagNotFound, ch.nodeId.ToString()));//Tag not found for alarm
                            }
                        }
                    }
                    else
                    {
                        lock (errors)
                        {
                            errors.Add(string.Format(Properties.Resources.RuntimeAlarmErrorNodeId, ch.nodeId.ToString()));//Alarm NodeId incorrect
                        }
                    }
                });

                if (mapSourceToUpdate.Count > 0)
                {
                    Parallel.ForEach(mapSourceToUpdate.Keys, source =>
                    {
                        source.UpdateAlarmStatus(mapSourceToUpdate[source], false);
                    });
                }

                if (errors.Count > 0)
                {
                    Parallel.ForEach(errors, err =>
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                err,
                                System.Diagnostics.EventLogEntryType.Error,
                                LoggerDestination.Server);
                    });
                }

                return (errors.Count == 0 ? StatusCodes.Good : StatusCodes.Bad);
            }

            return Opc.Ua.StatusCodes.BadNoData;
        }


        protected NodeId FromGuidToNodeId(Guid from)
        {
            return new NodeId(from, NamespaceIndex);
        }

        bool IsRootNode(NodeState node)
        {
            return node == baseFolderTags || 
                    node == baseFolderAlarms || 
                    node == baseFolderDrivers ||
                    node == baseFolderDiagnosticDrivers;
        }

        ViewState CreateView(UFUAModel.UFUAView ufuaView)
        {
            var nodeId = FromGuidToNodeId(ufuaView.NodeId);
            if (mapNodeIdToNodeState.ContainsKey(nodeId))
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    String.Format(Properties.Resources.DuplicatedViewNodeId, nodeId, ufuaView.Name),
                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                return null;
            }

            var view = new ViewState();

            view.BrowseName = new QualifiedName(ufuaView.Name, NamespaceIndex);
            view.DisplayName = view.BrowseName.Name;
            view.SymbolicName = view.BrowseName.Name;

            view.NodeId = nodeId;
            view.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ViewsFolder);

            AddPredefinedNode(SystemContext, view);
            mapNodeIdToNodeState.Add(view.NodeId, view);

            mapNodeIdToView.Add(view.NodeId, new List<NodeId>());

            return view;
        }

        void AssignTagToView(UFUAModel.UFUATag ufuaTag, BaseInstanceState node)
        {
            foreach (var view in ufuaTag.UFUAViews)
            {
                var nodeid = FromGuidToNodeId(view.NodeId);
                if (!mapNodeIdToView.ContainsKey(nodeid))
                    continue;
                mapNodeIdToView[nodeid].Add(node.NodeId);

                AssignChildrenToView(nodeid, node);
            }
        }

        void AssignChildrenToView(NodeId nodeid, BaseInstanceState node)
        {
            if (!mapNodeIdToView.ContainsKey(nodeid))
                return;

            var children = new List<BaseInstanceState>();
            node.GetChildren(SystemContext, children);
            children.ForEach((child) =>
            {
                mapNodeIdToView[nodeid].Add(child.NodeId);
                AssignChildrenToView(nodeid, child);
            });
        }

        void AssignNodeToUpdateList(List<NodeId> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            nodes.ForEach((nodeid) =>
            {
                AssignNodeToUpdateList(nodeid);
            });
        }

        void AssignNodeToUpdateList(NodeId nodeid)
        {
            var state = mapNodeIdToNodeState[nodeid];
            AssignNodeToUpdateList(state);
        }

        void AssignNodeToUpdateList(NodeState state)
        {
            lock (listNodeToUpdate)
            {
                if (!listNodeToUpdate.Contains(state))
                    listNodeToUpdate.Add(state);

                if (nodeStateUpdater == null)
                {
                    var dueTime = bNodeStateUpdaterReady ? TimeSpan.FromMilliseconds(500) : TimeSpan.FromMilliseconds(-1);
                    nodeStateUpdater = new Timer((o) =>
                    {
                        var pending = new List<NodeState>();
                        lock (listNodeToUpdate)
                        {
                            pending.AddRange(listNodeToUpdate);
                            listNodeToUpdate.Clear();

                            if (nodeStateUpdater != null)
                            {
                                nodeStateUpdater.Dispose();
                                nodeStateUpdater = null;
                            }
                        }

                        pending.ForEach((node) => 
                        {
                            node.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            node.ClearChangeMasks(SystemContext, true);
                        });
                    }, this, dueTime, TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        void AssignNodeToAlwaysInUseList(NodeId nodeid)
        {
            if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                return;

            var state = mapNodeIdToNodeState[nodeid];
            AssignNodeToAlwaysInUseList(state);
        }

        void AssignNodeToAlwaysInUseList(NodeState nodeState)
        {
            if (mapAlwaysNodeIdInUse.ContainsKey(nodeState.NodeId))
                return;

            var instance = nodeState as BaseInstanceState;
            while (instance != null)
            {
                if (mapNodeIdToDynamicSettings.ContainsKey(instance.NodeId))
                {
                    if (!mapAlwaysNodeIdInUse.ContainsKey(instance.NodeId))
                        mapAlwaysNodeIdInUse.Add(instance.NodeId, new List<NodeId>());
                    mapAlwaysNodeIdInUse[instance.NodeId].Add(nodeState.NodeId);
                    break;
                }

                instance = instance.Parent as BaseInstanceState;
            }
        }

#if !NET_STANDARD
        #region Script Manager
        public virtual ServiceResult OnMethodCallScript(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.MethodCallEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
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

            if (!mapNodeIdMethodToScriptManager.ContainsKey(method.NodeId) ||
                !mapNodeIdMethodToSubName.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdUnknown;

            object[] array = new Object[inputArguments.Count];
            for (i = 0; i < inputArguments.Count; ++i)
                array[i] = inputArguments[i];
            if (mapNodeIdMethodToScriptManager[method.NodeId].MethodCall(mapNodeIdMethodToSubName[method.NodeId], array))
                return StatusCodes.Good;

            return StatusCodes.BadNoEntryExists;
        }

        Dictionary<NodeId, ScriptManager> mapScriptManagers;
        internal bool EnableScriptRemoteDebugging(Guid id, bool bEnable)
        {
            if (mapScriptManagers == null)
                return false;
            var nodeid = FromGuidToNodeId(id);
            if (!mapScriptManagers.ContainsKey(nodeid))
                return false;
            mapScriptManagers[nodeid].EnableRemoteDebugging(bEnable);
            return true;
        }

        internal List<String> SynchronizeScriptRemoteDebugging(Guid id, List<String> data)
        {
            if (mapScriptManagers == null)
                return null;
            var nodeid = FromGuidToNodeId(id);
            if (!mapScriptManagers.ContainsKey(nodeid))
                return null;
            mapScriptManagers[nodeid].DebugSynchronize(data);
            return mapScriptManagers[nodeid].GetRemoteDebuggingData();
        }

        internal void StartScripts()
        {
            if (mapScriptManagers == null)
                return;
            ScriptManager.StartEngine();
            mapScriptManagers.Values.ToList().ForEach(manager => manager.Start());
        }

        void InitializeScriptCode(String code, String[] listProcedures, int[] breakpoints, bool separateThread,
                                  BaseInstanceState instance)
        {
            if (String.IsNullOrEmpty(code))
                return;
            if (mapScriptManagers == null)
                mapScriptManagers = new Dictionary<NodeId, ScriptManager>();
            if (mapScriptManagers.ContainsKey(instance.NodeId))
                return;
            ScriptManager scriptManager;
            if (separateThread)
                scriptManager = new ScriptManagerSeparateThread(instance.BrowseName.Name, instance,
                                server as StandardServer, SystemContext, breakpoints, mapNameToNodeState, this);
            else
                scriptManager = new ScriptManager(instance.BrowseName.Name, instance,
                                server as StandardServer, SystemContext, breakpoints, mapNameToNodeState, this);
            scriptManager.InUseTag += (o, e) =>
            {
                var node = o as NodeState;
                if (node != null)
                {
                    var commDriver = GetCommunicationDriversFromNodeStateDictionary(node);
                    if (commDriver != null && commDriver.Count > 0)
                    {
                        foreach (var driver in commDriver)
                        {
                            InUseTagChanged(new InUseTagChangedArg()
                            {
                                Driver = driver,
                                NodeId = node.NodeId,
                                SamplingInterval = server.UFUAConfiguration.DefaultDataIOSamplingInterval,
                                bInUse = true,
                                TimeStamp = DateTime.UtcNow
                            });
                        }
                    }
                }
            };

            scriptManager.InitScriptEngine(code);
            mapScriptManagers.Add(instance.NodeId, scriptManager);
            if (listProcedures != null)
            {
                var mapSubs = ScriptManager.GetSubList(listProcedures.ToList());
                foreach (var sub in mapSubs.Keys)
                {
                    if (!sub.StartsWith("Method"))
                        continue;

                    var methodBase = new MethodState(instance);
                    methodBase.BrowseName = new QualifiedName(sub, NamespaceIndex);
                    methodBase.SymbolicName = methodBase.BrowseName.Name;
                    methodBase.DisplayName = methodBase.BrowseName.Name;
                    methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                    methodBase.UserExecutable = true;
                    methodBase.Executable = true;
                    //if (parentFolder == null || parentFolder is FolderState)
                    //    methodBase.NodeId = FromGuidToNodeId(ufuaTag.NodeId);
                    //else
                    //    methodBase.NodeId = New(SystemContext, methodBase);
                    // set the symbolic name and reference types.
                    // initialize the variable from the type model.
                    methodBase.Create(
                        SystemContext,
                        methodBase.NodeId,
                        new QualifiedName(sub, NamespaceIndex),
                        null,
                        true);

                    var mapParameters = mapSubs[sub];
                    if (mapParameters.Count > 0)
                    {
                        // add input arguments.
                        methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
                        methodBase.InputArguments.NodeId = New(SystemContext, methodBase.InputArguments);
                        methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
                        methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
                        methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                        methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                        methodBase.InputArguments.DataType = DataTypeIds.Argument;
                        methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

                        List<Argument> args = new List<Argument>();
                        foreach (var parName in mapParameters.Keys)
                        {
                            var arg = new Argument { Name = parName };
                            arg.DataType = mapParameters[parName];

                            arg.ValueRank = ValueRanks.Scalar;
                            arg.ArrayDimensions = null;

                            args.Add(arg);
                        }
                        methodBase.InputArguments.Value = args.ToArray();
                        mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);
                    }

                    // methodBase.AddReference(ReferenceTypeIds.Organizes, true, instance.NodeId);
                    // AddPredefinedNode(SystemContext, methodBase);
                    // mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
                    if (instance is BaseObjectState)
                        (instance as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                    instance.AddChild(methodBase);

                    mapNodeIdMethodToScriptManager.Add(methodBase.NodeId, scriptManager);
                    mapNodeIdMethodToSubName.Add(methodBase.NodeId, sub);
                    // set up method handlers. 
                    methodBase.OnCallMethod = OnMethodCallScript;
                }
            }
        }
        #endregion
#endif

        void fillOrderedMemberList(UFUATagPrototype typedef, Dictionary<string, int> w)
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
        void fillOrderedMemberList(UFUAFolder typedef, Dictionary<string, int> w)
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

        NodeId GetResolvedNodeId(UFUAModel.UFUATag ufuaTag, NodeId parent, bool bUseOldFormat = false)
        {
            NodeId nodeid;
            if (bUseOldFormat)
            {
                if (String.IsNullOrEmpty(ufuaTag.FolderPath))
                    nodeid = new NodeId(String.Format("{0}?{1}", parent.Identifier, ufuaTag.Name), NamespaceIndex);
                else
                    nodeid = new NodeId(String.Format("{0}?{1}//{2}", parent.Identifier, ufuaTag.FolderPath, ufuaTag.Name), NamespaceIndex);
            }
            else
            {
                if (String.IsNullOrEmpty(ufuaTag.FolderPath))
                    nodeid = new NodeId(String.Format("{0}?{1}", parent.Identifier, ufuaTag.NodeId), NamespaceIndex);
                else
                    nodeid = new NodeId(String.Format("{0}?{1}/{2}", parent.Identifier, ufuaTag.FolderPath.Replace('\\', '/'), ufuaTag.NodeId), NamespaceIndex);
            }

            return nodeid;
        }

        void UpdateMapInstanceState(BaseInstanceState state)
        {
            var tagesNodeId = FromGuidToNodeId(UFUAServerInfo.Guids.RootTagsGuid);
            var name = state.DisplayName.ToString();
            var parent = state.Parent;
            while (parent != null)
            {
                if (parent.NodeId == ObjectIds.ObjectsFolder || parent.NodeId == tagesNodeId)
                    break;
                name = String.Format("{0}\\{1}", parent.DisplayName, name);
                if (parent is BaseInstanceState)
                    parent = (parent as BaseInstanceState).Parent;
                else
                    parent = null;
            }
            if (!mapNameToNodeState.ContainsKey(name))
                mapNameToNodeState.Add(name, state);
        }

        readonly List<String> checkRecursivePrototypes = new List<String>();
        BaseInstanceState CreateTag(UFUAModel.UFUATag ufuaTag, NodeId parent,
                                    NodeState parentFolder = null,
                                    bool bAssignNodeId = false, bool creatingType = false,
                                    bool forceRetentive = false, bool bInsideObject = false, bool forceInUse = false)
        {
            BaseDataVariableState variable = null;
            if (!creatingType)
            {
                var nodeId = FromGuidToNodeId(ufuaTag.NodeId);
                if (mapNodeIdToNodeState.ContainsKey(nodeId))
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        String.Format(Properties.Resources.DuplicatedTagNodeId, nodeId, ufuaTag.GetFullName()),
                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    return null;
                }
            }

            switch (ufuaTag.ModelType)
            {
                case UFUAModel.ModelType.Variable:
                    {
                        variable = new DataItemState(parentFolder);
                        break;
                    }

                case UFUAModel.ModelType.Digital:
                    {
                        variable = new TwoStateDiscreteState(parentFolder);
                        break;
                    }
                case UFUAModel.ModelType.Enumerated:
                    {
                        variable = new MultiStateDiscreteState(parentFolder);
                        break;
                    }

                case UFUAModel.ModelType.Method:
                    {
                        var methodBase = new MethodState(parentFolder);
                        methodBase.BrowseName = new QualifiedName(ufuaTag.Name, NamespaceIndex);
                        methodBase.SymbolicName = methodBase.BrowseName.Name;
                        methodBase.DisplayName = methodBase.BrowseName.Name;
                        methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                        methodBase.UserExecutable = true;
                        methodBase.Executable = true;
                        //if (parentFolder == null || parentFolder is FolderState)
                        //    methodBase.NodeId = FromGuidToNodeId(ufuaTag.NodeId);
                        //else
                        //    methodBase.NodeId = New(SystemContext, methodBase);
                        // set the symbolic name and reference types.
                        if (!bAssignNodeId)
                            methodBase.NodeId = FromGuidToNodeId(ufuaTag.NodeId);
                        // initialize the variable from the type model.
                        methodBase.Create(
                            SystemContext,
                            methodBase.NodeId,
                            new QualifiedName(ufuaTag.Name, NamespaceIndex),
                            null,
                            bAssignNodeId);

                        //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                        {
                            var typedefinputs = (from types in tagPrototypes.AsParallel()
                                                 where types.Name == (ufuaTag.Name + "Input") && types.UFUATagOwner == null
                                                 select types).ToList();
                            if (typedefinputs.Count == 1)
                            {
                                var typedef = typedefinputs[0];

                                // add input arguments.
                                methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
                                methodBase.InputArguments.NodeId = FromGuidToNodeId(typedef.NodeId);
                                methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
                                methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
                                methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                                methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                                methodBase.InputArguments.DataType = DataTypeIds.Argument;
                                methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

                                List<Argument> args = new List<Argument>();
                                var ordMembers = (from mem in typedef.Members.AsParallel() orderby mem.Oid select mem).ToList();
                                foreach (var member in ordMembers)
                                {
                                    var arg = new Argument { Name = member.Name, Description = member.Description };
                                    switch (member.DataType)
                                    {
                                        case UFUAModel.DataType.Boolean: { arg.DataType = DataTypeIds.Boolean; break; }
                                        case UFUAModel.DataType.SByte: { arg.DataType = DataTypeIds.SByte; break; }
                                        case UFUAModel.DataType.Byte: { arg.DataType = DataTypeIds.Byte; break; }
                                        case UFUAModel.DataType.Int16: { arg.DataType = DataTypeIds.Int16; break; }
                                        case UFUAModel.DataType.UInt16: { arg.DataType = DataTypeIds.UInt16; break; }
                                        case UFUAModel.DataType.Int32: { arg.DataType = DataTypeIds.Int32; break; }
                                        case UFUAModel.DataType.UInt32: { arg.DataType = DataTypeIds.UInt32; break; }
                                        case UFUAModel.DataType.Int64: { arg.DataType = DataTypeIds.Int64; break; }
                                        case UFUAModel.DataType.UInt64: { arg.DataType = DataTypeIds.UInt64; break; }
                                        case UFUAModel.DataType.Float: { arg.DataType = DataTypeIds.Float; break; }
                                        case UFUAModel.DataType.Double: { arg.DataType = DataTypeIds.Double; break; }
                                        case UFUAModel.DataType.String: { arg.DataType = DataTypeIds.String; break; }
                                    }
                                    if (member.ArrayDimension == 0)
                                    {
                                        arg.ValueRank = ValueRanks.Scalar;
                                        arg.ArrayDimensions = null;
                                    }
                                    else
                                    {
                                        arg.ValueRank = ValueRanks.OneDimension;
                                        List<uint> dimList = new List<uint>(1);
                                        dimList.Add(member.ArrayDimension);
                                        arg.ArrayDimensions = new UInt32Collection(dimList);
                                    }

                                    args.Add(arg);
                                }
                                methodBase.InputArguments.Value = args.ToArray();
                                mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);
                            }

                            var typedefoutputs = (from types in tagPrototypes.AsParallel()
                                                  where types.Name == (ufuaTag.Name + "Output") && types.UFUATagOwner == null
                                                  select types).ToList();
                            if (typedefoutputs.Count == 1)
                            {
                                var typedef = typedefoutputs[0];

                                // add output arguments.
                                methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
                                methodBase.OutputArguments.NodeId = FromGuidToNodeId(typedef.NodeId);
                                methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;//OutputArguments;
                                methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
                                methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                                methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                                methodBase.OutputArguments.DataType = DataTypeIds.Argument;
                                methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

                                List<Argument> args = new List<Argument>();
                                var ordMembers = (from mem in typedef.Members.AsParallel() orderby mem.Oid select mem).ToList();
                                foreach (var member in ordMembers)
                                {
                                    var arg = new Argument { Name = member.Name, Description = member.Description };
                                    switch (member.DataType)
                                    {
                                        case UFUAModel.DataType.Boolean: { arg.DataType = DataTypeIds.Boolean; break; }
                                        case UFUAModel.DataType.SByte: { arg.DataType = DataTypeIds.SByte; break; }
                                        case UFUAModel.DataType.Byte: { arg.DataType = DataTypeIds.Byte; break; }
                                        case UFUAModel.DataType.Int16: { arg.DataType = DataTypeIds.Int16; break; }
                                        case UFUAModel.DataType.UInt16: { arg.DataType = DataTypeIds.UInt16; break; }
                                        case UFUAModel.DataType.Int32: { arg.DataType = DataTypeIds.Int32; break; }
                                        case UFUAModel.DataType.UInt32: { arg.DataType = DataTypeIds.UInt32; break; }
                                        case UFUAModel.DataType.Int64: { arg.DataType = DataTypeIds.Int64; break; }
                                        case UFUAModel.DataType.UInt64: { arg.DataType = DataTypeIds.UInt64; break; }
                                        case UFUAModel.DataType.Float: { arg.DataType = DataTypeIds.Float; break; }
                                        case UFUAModel.DataType.Double: { arg.DataType = DataTypeIds.Double; break; }
                                        case UFUAModel.DataType.String: { arg.DataType = DataTypeIds.String; break; }
                                    }
                                    if (member.ArrayDimension == 0)
                                    {
                                        arg.ValueRank = ValueRanks.Scalar;
                                        arg.ArrayDimensions = null;
                                    }
                                    else
                                    {
                                        arg.ValueRank = ValueRanks.OneDimension;
                                        List<uint> dimList = new List<uint>(1);
                                        dimList.Add(member.ArrayDimension);
                                        arg.ArrayDimensions = new UInt32Collection(dimList);
                                    }

                                    args.Add(arg);
                                }
                                methodBase.OutputArguments.Value = args.ToArray();
                                mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);
                            }
                        }

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

                        // set up method handlers. 
                        methodBase.OnCallMethod = OnMethodCall;
                        if (!creatingType)
                            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

                        if (!ufuaTag.ExcludeDynamicSettings && !String.IsNullOrEmpty(ufuaTag.DynamicSettings) && !NodeId.IsNull(methodBase.NodeId))
                            mapNodeIdToDynamicSettings.Add(methodBase.NodeId, ufuaTag.DynamicSettings);

                        AssignTagToView(ufuaTag, methodBase);
                        return methodBase;
                    }

                case UFUAModel.ModelType.ObjectType:
                    {
                        NodeId nodeId = null;
                        if (parentFolder == null || IsRootNode(parentFolder) || !bInsideObject)
                            nodeId = FromGuidToNodeId(ufuaTag.NodeId);

                        if (checkRecursivePrototypes.Contains(ufuaTag.PrototypeName))
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                String.Format(Properties.Resources.PrototypeModelIsRecursive, ufuaTag.PrototypeName, ufuaTag.Name),
                                System.Diagnostics.EventLogEntryType.Warning,
                                LoggerDestination.Server);
                            return null;
                        }

                        BaseObjectState newbaseObject = null;
                        try
                        {
                            checkRecursivePrototypes.Add(ufuaTag.PrototypeName);
                            newbaseObject = CreateObject(ufuaTag, parentFolder, nodeId, creatingType, forceRetentive);
                        }
                        finally
                        {
                            checkRecursivePrototypes.Remove(ufuaTag.PrototypeName);
                        }

                        if (newbaseObject == null)
                            return null;

                        if (parentFolder == null)
                        {
                            newbaseObject.AddReference(ReferenceTypeIds.Organizes, true, parent);
                        }
                        else
                        {
                            if (parentFolder is BaseObjectState)
                                (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                            parentFolder.AddChild(newbaseObject);
                        }

                        if (!creatingType)
                        {
                            AddPredefinedNode(SystemContext, newbaseObject);
                            mapNodeIdToNodeState.Add(newbaseObject.NodeId, newbaseObject);
                            UpdateMapInstanceState(newbaseObject);
                            if (!ufuaTag.IsPrototypeMember && nodeId == null)
                                mapNodeIdToNodeState.Add(FromGuidToNodeId(ufuaTag.NodeId), newbaseObject);

                            //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                            {
                                var prototypeName = ufuaTag.PrototypeName;
                                var typedeflist = (from p in tagPrototypes.AsParallel()
                                                   where p.Name == prototypeName && p.UFUATagOwner == null
                                                   select p).ToList();
                                if (typedeflist.Count > 0)
                                {
                                    var typedef = typedeflist[0];
                                    Dictionary<int, XPObject> whole = new Dictionary<int, XPObject>();

                                    if (!mapStructToMemberOder.ContainsKey(FromGuidToNodeId(typedef.NodeId)))
                                    {
                                        mapStructToMemberOder.Add(FromGuidToNodeId(typedef.NodeId), new Dictionary<string, int>());

                                        fillOrderedMemberList(typedef, mapStructToMemberOder[FromGuidToNodeId(typedef.NodeId)]);
                                    }
                                }
                            }

                            if (!forceRetentive && !ufuaTag.IsRetentive && !ufuaTag.ExcludeDynamicSettings && !String.IsNullOrEmpty(ufuaTag.DynamicSettings))
                                SetStatusCode(newbaseObject.NodeId, StatusCodes.UncertainInitialValue, notify: false);
                        }

                        AssignTagToView(ufuaTag, newbaseObject);
                        return newbaseObject;
                    }

                case UFUAModel.ModelType.Analog:
                    {
                        variable = new AnalogItemState(parentFolder);
                        break;
                    }
            }

            // set the symbolic name and reference types.
            //if (!bAssignNodeId)
            variable.NodeId = FromGuidToNodeId(ufuaTag.NodeId);
            variable.SymbolicName = ufuaTag.Name;
            variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // initialize the variable from the type model.
            variable.Create(
                SystemContext,
                variable.NodeId,
                new QualifiedName(ufuaTag.Name, NamespaceIndex),
                null,
                bAssignNodeId);

            if (!creatingType && bInsideObject && !NodeId.IsNull(parent))
            {
                NodeId nodeid = GetResolvedNodeId(ufuaTag, parent, bUseOldFormat: true);
                if (!mapCacheNodeIdString.ContainsKey(nodeid))
                    mapCacheNodeIdString.Add(nodeid, nodeid.ToString());
                if (!mapNodeIdMemberNewFormat.ContainsKey(mapCacheNodeIdString[nodeid]))
                    mapNodeIdMemberNewFormat.Add(mapCacheNodeIdString[nodeid], variable.NodeId);
            }

            #region Add Properties
            if (ufuaTag.ModelType == UFUAModel.ModelType.Enumerated)
            {
                var node = variable as MultiStateDiscreteState;
                var orderedEnumStrings = (from c in enumStrings
                                          where c.UFUATag != null && 
                                          c.UFUATag.Oid == ufuaTag.Oid
                                          orderby c.Oid
                                          select c).ToList();
                if (orderedEnumStrings.Count > 0)
                {
                    node.EnumStrings = new PropertyState<LocalizedText[]>(node);
                    node.EnumStrings.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.EnumStrings.ModellingRuleId = null;
                    node.EnumStrings.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.EnumStrings.SymbolicName = Opc.Ua.BrowseNames.EnumStrings;
                    node.EnumStrings.BrowseName = Opc.Ua.BrowseNames.EnumStrings;
                    node.EnumStrings.DisplayName = Opc.Ua.BrowseNames.EnumStrings;
                    node.EnumStrings.Description = null;
                    node.EnumStrings.WriteMask = 0;
                    node.EnumStrings.UserWriteMask = 0;
                    node.EnumStrings.DataType = DataTypeIds.LocalizedText;
                    node.EnumStrings.ValueRank = ValueRanks.Scalar;
                    node.EnumStrings.ArrayDimensions = null;
                    node.EnumStrings.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.EnumStrings.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.EnumStrings.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.EnumStrings.Historizing = false;
                    node.EnumStrings.NodeId = ModelUtils.ConstructIdForComponent(node.EnumStrings, NamespaceIndex);
                    node.EnumStrings.OnReadValue = OnReadLocalizedTextValue;

                    LocalizedText[] strings = new LocalizedText[orderedEnumStrings.Count];
                    
                    for (int ii = 0; ii < orderedEnumStrings.Count; ii++)
                    {
                        strings[ii] = new LocalizedText(orderedEnumStrings[ii].Data, string.Empty, orderedEnumStrings[ii].Data, null);
                    }

                    node.EnumStrings.Value = strings;
                    node.EnumStrings.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else if (node.EnumStrings != null)
                    node.EnumStrings.NodeId = ModelUtils.ConstructIdForComponent(node.EnumStrings, NamespaceIndex);
                if ((!ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings)) ||
                    !string.IsNullOrEmpty(ufuaTag.Description))
                {
                    node.Definition = new PropertyState<String>(node);
                    node.Definition.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.Definition.ModellingRuleId = null;
                    node.Definition.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.Definition.SymbolicName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.BrowseName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.DisplayName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.Description = null;
                    node.Definition.WriteMask = 0;
                    node.Definition.UserWriteMask = 0;
                    node.Definition.DataType = DataTypeIds.String;
                    node.Definition.ValueRank = ValueRanks.Scalar;
                    node.Definition.ArrayDimensions = null;
                    node.Definition.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.Definition.Historizing = false;
                    node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                    node.Definition.Value = !ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                    node.Definition.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else
                {
                    if (node.Definition != null)
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                }
                if (node.ValuePrecision != null)
                    node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
            }
            else if (ufuaTag.ModelType == UFUAModel.ModelType.Digital)
            {
                var node = variable as TwoStateDiscreteState;
                var orderedEnumStrings = (from c in enumStrings
                                          where c.UFUATag != null && 
                                          c.UFUATag.Oid == ufuaTag.Oid
                                          orderby c.Oid
                                          select c).ToList();
                if (orderedEnumStrings.Count >= 2)
                {
                    node.TrueState = new PropertyState<LocalizedText>(node);
                    node.TrueState.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.TrueState.ModellingRuleId = null;
                    node.TrueState.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.TrueState.SymbolicName = Opc.Ua.BrowseNames.TrueState;
                    node.TrueState.BrowseName = Opc.Ua.BrowseNames.TrueState;
                    node.TrueState.DisplayName = Opc.Ua.BrowseNames.TrueState;
                    node.TrueState.Description = null;
                    node.TrueState.WriteMask = 0;
                    node.TrueState.UserWriteMask = 0;
                    node.TrueState.DataType = DataTypeIds.LocalizedText;
                    node.TrueState.ValueRank = ValueRanks.Scalar;
                    node.TrueState.ArrayDimensions = null;
                    node.TrueState.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.TrueState.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.TrueState.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.TrueState.Historizing = false;
                    node.TrueState.NodeId = ModelUtils.ConstructIdForComponent(node.TrueState, NamespaceIndex);
                    node.TrueState.Value = new LocalizedText(orderedEnumStrings[1].Data, string.Empty, orderedEnumStrings[1].Data);
                    node.TrueState.Timestamp = ufuaTag.SettingsTimeStamp;
                    node.TrueState.OnReadValue = OnReadLocalizedTextValue;

                    node.FalseState = new PropertyState<LocalizedText>(node);
                    node.FalseState.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.FalseState.ModellingRuleId = null;
                    node.FalseState.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.FalseState.SymbolicName = Opc.Ua.BrowseNames.FalseState;
                    node.FalseState.BrowseName = Opc.Ua.BrowseNames.FalseState;
                    node.FalseState.DisplayName = Opc.Ua.BrowseNames.FalseState;
                    node.FalseState.Description = null;
                    node.FalseState.WriteMask = 0;
                    node.FalseState.UserWriteMask = 0;
                    node.FalseState.DataType = DataTypeIds.LocalizedText;
                    node.FalseState.ValueRank = ValueRanks.Scalar;
                    node.FalseState.ArrayDimensions = null;
                    node.FalseState.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.FalseState.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.FalseState.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.FalseState.Historizing = false;
                    node.FalseState.NodeId = ModelUtils.ConstructIdForComponent(node.FalseState, NamespaceIndex);
                    node.FalseState.Value = new LocalizedText(orderedEnumStrings[0].Data, string.Empty, orderedEnumStrings[0].Data);
                    node.FalseState.Timestamp = ufuaTag.SettingsTimeStamp;
                    node.FalseState.OnReadValue = OnReadLocalizedTextValue;
                }
                else
                {
                    if (node.TrueState != null)
                        node.TrueState.NodeId = ModelUtils.ConstructIdForComponent(node.TrueState, NamespaceIndex);
                    if (node.FalseState != null)
                        node.FalseState.NodeId = ModelUtils.ConstructIdForComponent(node.FalseState, NamespaceIndex);
                }
                if ((!ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings)) ||
                    !string.IsNullOrEmpty(ufuaTag.Description))
                {
                    node.Definition = new PropertyState<String>(node);
                    node.Definition.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.Definition.ModellingRuleId = null;
                    node.Definition.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.Definition.SymbolicName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.BrowseName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.DisplayName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.Description = null;
                    node.Definition.WriteMask = 0;
                    node.Definition.UserWriteMask = 0;
                    node.Definition.DataType = DataTypeIds.String;
                    node.Definition.ValueRank = ValueRanks.Scalar;
                    node.Definition.ArrayDimensions = null;
                    node.Definition.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.Definition.Historizing = false;
                    node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                    node.Definition.Value = !ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                    node.Definition.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else
                {
                    if (node.Definition != null)
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                }
                if (node.ValuePrecision != null)
                    node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
            }
            else if (ufuaTag.ModelType == UFUAModel.ModelType.Variable)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // By Maurizio Zaniboni - Progea Srl.
                // Inserted this source lines for passing the CTT unit test : Data Access->Data Access DataItems->010.js.
                if (ufuaTag.DataType == UFUAModel.DataType.Float ||
                    ufuaTag.DataType == UFUAModel.DataType.Double)
                {
                    var node = variable as DataItemState;

                    node.ValuePrecision = new PropertyState<double>(node);
                    node.ValuePrecision.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.ValuePrecision.ModellingRuleId = null;
                    node.ValuePrecision.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.ValuePrecision.SymbolicName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.BrowseName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.DisplayName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.Description = null;
                    node.ValuePrecision.WriteMask = 0;
                    node.ValuePrecision.UserWriteMask = 0;
                    node.ValuePrecision.DataType = DataTypeIds.Double;
                    node.ValuePrecision.ValueRank = ValueRanks.Scalar;
                    node.ValuePrecision.ArrayDimensions = null;
                    node.ValuePrecision.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.ValuePrecision.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.ValuePrecision.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.ValuePrecision.Historizing = false;
                    node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
                    node.ValuePrecision.Value = 6;
                    node.ValuePrecision.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else
                {
                    var node = variable as DataItemState;
                    if (node.ValuePrecision != null)
                        node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if ((!ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings)) ||
                    !string.IsNullOrEmpty(ufuaTag.Description))
                {
                    var node = variable as DataItemState;

                    node.Definition = new PropertyState<String>(node);
                    node.Definition.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.Definition.ModellingRuleId = null;
                    node.Definition.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.Definition.SymbolicName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.BrowseName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.DisplayName = Opc.Ua.BrowseNames.Definition;
                    node.Definition.Description = null;
                    node.Definition.WriteMask = 0;
                    node.Definition.UserWriteMask = 0;
                    node.Definition.DataType = DataTypeIds.String;
                    node.Definition.ValueRank = ValueRanks.Scalar;
                    node.Definition.ArrayDimensions = null;
                    node.Definition.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.Definition.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.Definition.Historizing = false;
                    node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                    node.Definition.Value = !ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                    node.Definition.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else
                {
                    var node = variable as DataItemState;
                    if (node.Definition != null)
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                }
            }
            else if (ufuaTag.ModelType == UFUAModel.ModelType.Analog)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // By Maurizio Zaniboni - Progea Srl.
                // Inserted this source lines for passing the CTT unit test : Data Access->Data Access DataItems->010.js.
                if (ufuaTag.DataType == UFUAModel.DataType.Float ||
                    ufuaTag.DataType == UFUAModel.DataType.Double)
                {
                    var node = variable as DataItemState;

                    node.ValuePrecision = new PropertyState<double>(node);
                    node.ValuePrecision.ReferenceTypeId = ReferenceTypes.HasProperty;
                    node.ValuePrecision.ModellingRuleId = null;
                    node.ValuePrecision.TypeDefinitionId = VariableTypeIds.PropertyType;
                    node.ValuePrecision.SymbolicName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.BrowseName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.DisplayName = Opc.Ua.BrowseNames.ValuePrecision;
                    node.ValuePrecision.Description = null;
                    node.ValuePrecision.WriteMask = 0;
                    node.ValuePrecision.UserWriteMask = 0;
                    node.ValuePrecision.DataType = DataTypeIds.Double;
                    node.ValuePrecision.ValueRank = ValueRanks.Scalar;
                    node.ValuePrecision.ArrayDimensions = null;
                    node.ValuePrecision.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.ValuePrecision.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    node.ValuePrecision.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                    node.ValuePrecision.Historizing = false;
                    node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
                    node.ValuePrecision.Value = 6;
                    node.ValuePrecision.Timestamp = ufuaTag.SettingsTimeStamp;
                }
                else
                {
                    var node = variable as DataItemState;
                    if (node.ValuePrecision != null)
                        node.ValuePrecision.NodeId = ModelUtils.ConstructIdForComponent(node.ValuePrecision, NamespaceIndex);
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (!String.IsNullOrEmpty(ufuaTag.UFUAEngineeringUnit))
                {
                    var node = variable as AnalogItemState;

                    if ((!ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings)) ||
                        !string.IsNullOrEmpty(ufuaTag.Description))
                    {
                        node.Definition = new PropertyState<String>(node);
                        node.Definition.ReferenceTypeId = ReferenceTypes.HasProperty;
                        node.Definition.ModellingRuleId = null;
                        node.Definition.TypeDefinitionId = VariableTypeIds.PropertyType;
                        node.Definition.SymbolicName = Opc.Ua.BrowseNames.Definition;
                        node.Definition.BrowseName = Opc.Ua.BrowseNames.Definition;
                        node.Definition.DisplayName = Opc.Ua.BrowseNames.Definition;
                        node.Definition.Description = null;
                        node.Definition.WriteMask = 0;
                        node.Definition.UserWriteMask = 0;
                        node.Definition.DataType = DataTypeIds.String;
                        node.Definition.ValueRank = ValueRanks.Scalar;
                        node.Definition.ArrayDimensions = null;
                        node.Definition.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                        node.Definition.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                        node.Definition.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                        node.Definition.Historizing = false;
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                        node.Definition.Value = !ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                        node.Definition.Timestamp = ufuaTag.SettingsTimeStamp;
                    }
                    else if (node.Definition != null)
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);

                    //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    {
                        var engineeringUnitList = (from engunit in engineeringUnits.AsParallel()
                                                   where engunit.Name == ufuaTag.UFUAEngineeringUnit
                                                   select engunit).ToList();

                        if (engineeringUnitList.Count > 0)
                        {
                            var engineeringUnit = engineeringUnitList[0];

                            node.EURange = new PropertyState<Range>(node);
                            node.EURange.ReferenceTypeId = ReferenceTypes.HasProperty;
                            node.EURange.ModellingRuleId = null;
                            node.EURange.TypeDefinitionId = VariableTypeIds.PropertyType;
                            node.EURange.SymbolicName = Opc.Ua.BrowseNames.EURange;
                            node.EURange.BrowseName = Opc.Ua.BrowseNames.EURange;
                            node.EURange.DisplayName = Opc.Ua.BrowseNames.EURange;
                            node.EURange.Description = null;
                            node.EURange.WriteMask = 0;
                            node.EURange.UserWriteMask = 0;
                            node.EURange.DataType = DataTypeIds.Range;
                            node.EURange.ValueRank = ValueRanks.Scalar;
                            node.EURange.ArrayDimensions = null;
                            node.EURange.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                            node.EURange.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                            node.EURange.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                            node.EURange.Historizing = false;
                            node.EURange.NodeId = ModelUtils.ConstructIdForComponent(node.EURange, NamespaceIndex);
                            node.EURange.Value = new Range(engineeringUnit.EURangeHigh.Value, engineeringUnit.EURangeLow.Value);
                            node.EURange.Timestamp = ufuaTag.SettingsTimeStamp;

                            if (engineeringUnit.IsValidInstrumentRange())
                            {
                                node.InstrumentRange = new PropertyState<Range>(node);
                                node.InstrumentRange.ReferenceTypeId = ReferenceTypes.HasProperty;
                                node.InstrumentRange.ModellingRuleId = null;
                                node.InstrumentRange.TypeDefinitionId = VariableTypeIds.PropertyType;
                                node.InstrumentRange.SymbolicName = Opc.Ua.BrowseNames.InstrumentRange;
                                node.InstrumentRange.BrowseName = Opc.Ua.BrowseNames.InstrumentRange;
                                node.InstrumentRange.DisplayName = Opc.Ua.BrowseNames.InstrumentRange;
                                node.InstrumentRange.Description = null;
                                node.InstrumentRange.WriteMask = 0;
                                node.InstrumentRange.UserWriteMask = 0;
                                node.InstrumentRange.DataType = DataTypeIds.Range;
                                node.InstrumentRange.ValueRank = ValueRanks.Scalar;
                                node.InstrumentRange.ArrayDimensions = null;
                                node.InstrumentRange.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                                node.InstrumentRange.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                                node.InstrumentRange.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                                node.InstrumentRange.Historizing = false;
                                node.InstrumentRange.NodeId = ModelUtils.ConstructIdForComponent(node.InstrumentRange, NamespaceIndex);
                                node.InstrumentRange.Value = new Range(engineeringUnit.InstrumentRangeHigh.Value, engineeringUnit.InstrumentRangeLow.Value);
                                node.InstrumentRange.Timestamp = ufuaTag.SettingsTimeStamp;
                            }
                            else if (node.InstrumentRange != null)
                                node.InstrumentRange.NodeId = ModelUtils.ConstructIdForComponent(node.InstrumentRange, NamespaceIndex);

                            // by Maurizio Zaniboni - Progea Srl
                            // Commented the bellow line for passing the CTT unit test : Data Access->Data Access AnalogItemType->015.js
                            //if (!String.IsNullOrEmpty(engineeringUnit.UnitName))
                            {
                                node.EngineeringUnits = new PropertyState<EUInformation>(node);
                                node.EngineeringUnits.ReferenceTypeId = ReferenceTypes.HasProperty;
                                node.EngineeringUnits.ModellingRuleId = null;
                                node.EngineeringUnits.TypeDefinitionId = VariableTypeIds.PropertyType;
                                node.EngineeringUnits.SymbolicName = Opc.Ua.BrowseNames.EngineeringUnits;
                                node.EngineeringUnits.BrowseName = Opc.Ua.BrowseNames.EngineeringUnits;
                                node.EngineeringUnits.DisplayName = Opc.Ua.BrowseNames.EngineeringUnits;
                                node.EngineeringUnits.Description = null;
                                node.EngineeringUnits.WriteMask = 0;
                                node.EngineeringUnits.UserWriteMask = 0;
                                node.EngineeringUnits.DataType = DataTypeIds.Range;
                                node.EngineeringUnits.ValueRank = ValueRanks.Scalar;
                                node.EngineeringUnits.ArrayDimensions = null;
                                node.EngineeringUnits.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                                node.EngineeringUnits.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                                node.EngineeringUnits.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                                node.EngineeringUnits.Historizing = false;
                                node.EngineeringUnits.NodeId = ModelUtils.ConstructIdForComponent(node.EngineeringUnits, NamespaceIndex);

                                /////////////////////////////////////////////////////////////////////////////////////////////
                                // by Maurizio Zaniboni - Progea Srl
                                // Changed for passing the CTT unit test : Data Access->Data Access AnalogItemType->015.js
                                EUInformation info;
                                if (!String.IsNullOrEmpty(engineeringUnit.UnitName))
                                {
                                    info = new EUInformation
                                    {
                                        UnitId = 0,
                                        DisplayName = engineeringUnit.UnitName,
                                        NamespaceUri = Namespaces.ServerModel
                                    };
                                }
                                else
                                {
                                    info = new EUInformation
                                    {
                                        UnitId = -1
                                    };
                                }
                                /////////////////////////////////////////////////////////////////////////////////////////////

                                node.EngineeringUnits.Value = info;
                                node.EngineeringUnits.Timestamp = ufuaTag.SettingsTimeStamp;
                            }
                        }
                        else
                        {
                            if (node.Definition != null)
                                node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                            if (node.EURange != null)
                                node.EURange.NodeId = ModelUtils.ConstructIdForComponent(node.EURange, NamespaceIndex);
                            if (node.InstrumentRange != null)
                                node.InstrumentRange.NodeId = ModelUtils.ConstructIdForComponent(node.InstrumentRange, NamespaceIndex);
                            if (node.EngineeringUnits != null)
                                node.EngineeringUnits.NodeId = ModelUtils.ConstructIdForComponent(node.EngineeringUnits, NamespaceIndex);

                            Logger.WriteToEventLog(Properties.Resources.LoggerSource, 
                                String.Format(Properties.Resources.EngineeringUnitNotFound, ufuaTag.UFUAEngineeringUnit, ufuaTag.GetRelativeName()), 
                                System.Diagnostics.EventLogEntryType.Warning, 
                                LoggerDestination.Server);
                        }

                        if (engineeringUnitList.Count > 1)
                        {
                            if (duplicatedUnitsNames == null)
                                duplicatedUnitsNames = new List<String>();
                            if (!duplicatedUnitsNames.Contains(engineeringUnitList[0].Name))
                            {
                                duplicatedUnitsNames.Add(engineeringUnitList[0].Name);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.DuplicatedEngineeringUnits, engineeringUnitList[0].Name),
                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                            }
                        }
                    }
                }
                else
                {
                    var node = variable as AnalogItemState;
                    if (node.Definition != null)
                        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                    if (node.EURange != null)
                        node.EURange.NodeId = ModelUtils.ConstructIdForComponent(node.EURange, NamespaceIndex);
                    if (node.InstrumentRange != null)
                        node.InstrumentRange.NodeId = ModelUtils.ConstructIdForComponent(node.InstrumentRange, NamespaceIndex);
                    if (node.EngineeringUnits != null)
                        node.EngineeringUnits.NodeId = ModelUtils.ConstructIdForComponent(node.EngineeringUnits, NamespaceIndex);
                }
            }
            #endregion

            if (parentFolder == null)
            {
                variable.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(variable);
            }

            variable.Description = ufuaTag.Description;

            switch (ufuaTag.DataType)
            {
                case UFUAModel.DataType.Boolean: { variable.DataType = DataTypeIds.Boolean; break; }
                case UFUAModel.DataType.SByte: { variable.DataType = DataTypeIds.SByte; break; }
                case UFUAModel.DataType.Byte: { variable.DataType = DataTypeIds.Byte; break; }
                case UFUAModel.DataType.Int16: { variable.DataType = DataTypeIds.Int16; break; }
                case UFUAModel.DataType.UInt16: { variable.DataType = DataTypeIds.UInt16; break; }
                case UFUAModel.DataType.Int32: { variable.DataType = DataTypeIds.Int32; break; }
                case UFUAModel.DataType.UInt32: { variable.DataType = DataTypeIds.UInt32; break; }
                case UFUAModel.DataType.Int64: { variable.DataType = DataTypeIds.Int64; break; }
                case UFUAModel.DataType.UInt64: { variable.DataType = DataTypeIds.UInt64; break; }
                case UFUAModel.DataType.Float: { variable.DataType = DataTypeIds.Float; break; }
                case UFUAModel.DataType.Double: { variable.DataType = DataTypeIds.Double; break; }
                case UFUAModel.DataType.String: { variable.DataType = DataTypeIds.String; break; }
            }

            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            if (!String.IsNullOrEmpty(ufuaTag.InitialValue))
            {
                try
                {
                    System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                    variable.Value = ChangeType(ufuaTag.InitialValue, builtinType, ufuaTag.ArrayDimension, info);
                }
                catch (Exception ex) 
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        String.Format(Properties.Resources.FailedToSetInitalValueTag, ufuaTag.InitialValue, ufuaTag.GetRelativeName(), ex.Message), 
                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                }
            }
            else if (ufuaTag.ArrayDimension == 0)
            {
                if (builtinType == BuiltInType.String)
                    variable.Value = String.Empty;
                else
                    variable.Value = TypeInfo.GetDefaultValue(builtinType);
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // By Maurizio Zaniboni - Progea Srl.
                // Changed this source lines for passing the CTT unit test : Attribute Services->Attribute Read->024.js.
                //var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)ufuaTag.ArrayDimension);
                //variable.Value = new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, CastArrayElement));
                var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)ufuaTag.ArrayDimension);
                if (array != null && builtinType == BuiltInType.String)
                {
                    for (int ii = 0; ii < array.Length; ii++)
                        array.SetValue(String.Empty, ii);
                }
                variable.Value = array;
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            variable.Timestamp = DateTime.UtcNow;

            if (ufuaTag.ArrayDimension == 0)
            {
                variable.ValueRank = ValueRanks.Scalar;
                variable.ArrayDimensions = null;
                if (Properties.Settings.Default.AddOptionalProperties)
                {
                    var propAllowNulls = AddProperty(variable, Opc.Ua.BrowseNames.AllowNulls, false);
                    propAllowNulls.DataType = DataTypes.Boolean;
                    propAllowNulls.OnReadValue =
                        (ISystemContext context,
                        NodeState node,
                        NumericRange indexRange,
                        QualifiedName dataEncoding,
                        ref object value,
                        ref StatusCode statusCode,
                        ref DateTime timestamp) =>
                    {
                        if (!CanUserReadNodeState(context, variable))
                        {
                            return StatusCodes.BadUserAccessDenied;
                        }

                        value = false;
                        return StatusCodes.Good;
                    };
                }
            }
            else
            {
                variable.ValueRank = ValueRanks.OneDimension;
                List<uint> dimList = new List<uint>(1);
                dimList.Add(ufuaTag.ArrayDimension);
                variable.ArrayDimensions = new ReadOnlyList<uint>(dimList);
            }

            if (forceRetentive || (!ufuaTag.IsPrototypeMember && ufuaTag.IsRetentive))
            {
                if (!creatingType)
                {
                    InitTagLogger();

                    var entity = new UFUATagLogEntity()
                    {
                        NodeId = variable.NodeId.ToString(),
                        TagName = ModelUtils.ConstructNameForComponent(variable),
                        Value = new DataValue(),
                        builtinType = builtinType,
                        arraySizeOneDimension = ufuaTag.ArrayDimension
                    };

                    var bFound = tagLogger.GetDataValue(entity);
                    if (bFound)
                    {
                        variable.Value = entity.Value.Value;
                        variable.Timestamp = entity.Value.SourceTimestamp;
                        variable.StatusCode = entity.Value.StatusCode;
                    }

                    mapNodeIdToTagLogEntities[variable.NodeId] = entity;
                    variable.StateChanged += OnNodeStateChangedTagLogger;

                    if (!bFound)
                        AssignNodeToUpdateList(variable);
                }
            }
            else if (!ufuaTag.ExcludeDynamicSettings && !String.IsNullOrEmpty(ufuaTag.DynamicSettings))
            {
                variable.StatusCode = StatusCodes.UncertainInitialValue;
            }

            if (ufuaTag.EnableStatistics && !ufuaTag.IsPrototypeMember && !creatingType)
            {
                isAnyTagStatisticsEnabled = true;

                InitTagLogger();

                if (!mapNodeIdToTagLogEntities.ContainsKey(variable.NodeId))
                {
                    var e = new UFUATagLogEntity()
                    {
                        NodeId = variable.NodeId.ToString(),
                        TagName = ModelUtils.ConstructNameForComponent(variable),
                        Value = new DataValue(),
                        builtinType = builtinType,
                        arraySizeOneDimension = ufuaTag.ArrayDimension
                    };

                    mapNodeIdToTagLogEntities[variable.NodeId] = e;
                    variable.StateChanged += OnNodeStateChangedTagLogger;

                    if (!tagLogger.GetDataValue(e))
                        AssignNodeToUpdateList(variable);
                }

                var entity = mapNodeIdToTagLogEntities[variable.NodeId];
                entity.enableStatistics = true;
                var propMin = AddProperty(variable, StatDef.StatisticsDefinitions.statParams[StatDef.StatProps.Min], entity.Min);
                var propMax = AddProperty(variable, StatDef.StatisticsDefinitions.statParams[StatDef.StatProps.Max], entity.Max);
                var propAverage = AddProperty(variable, StatDef.StatisticsDefinitions.statParams[StatDef.StatProps.Average], entity.Average);
                var propNumUpdates = AddProperty(variable, StatDef.StatisticsDefinitions.statParams[StatDef.StatProps.NumUpdates], entity.CountUpdates);
                var propTotalTimeOn = AddProperty(variable, StatDef.StatisticsDefinitions.statParams[StatDef.StatProps.TotalTimeOn], entity.TotalTimeOn.ToString());
                propTotalTimeOn.DataType = DataTypes.String;

                entity.StatisticUpdated += (o, e) =>
                {
                    propMin.UpdateChangeMasks(NodeStateChangeMasks.Value);
                    propMax.UpdateChangeMasks(NodeStateChangeMasks.Value);
                    propNumUpdates.UpdateChangeMasks(NodeStateChangeMasks.Value);
                    propAverage.UpdateChangeMasks(NodeStateChangeMasks.Value);
                    propTotalTimeOn.UpdateChangeMasks(NodeStateChangeMasks.Value);
                    propMin.ClearChangeMasks(SystemContext, true);
                    propMax.ClearChangeMasks(SystemContext, true);
                    propNumUpdates.ClearChangeMasks(SystemContext, true);
                    propAverage.ClearChangeMasks(SystemContext, true);
                    propTotalTimeOn.ClearChangeMasks(SystemContext, true);
                };

                propMin.OnReadValue = (ISystemContext context,
                                       NodeState node,
                                       NumericRange indexRange,
                                       QualifiedName dataEncoding,
                                       ref object value,
                                       ref StatusCode statusCode,
                                       ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = entity.Min;
                    return StatusCodes.Good;
                };

                propMax.OnReadValue = (ISystemContext context,
                                       NodeState node,
                                       NumericRange indexRange,
                                       QualifiedName dataEncoding,
                                       ref object value,
                                       ref StatusCode statusCode,
                                       ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = entity.Max;
                    return StatusCodes.Good;
                };

                propNumUpdates.OnReadValue = (ISystemContext context,
                                       NodeState node,
                                       NumericRange indexRange,
                                       QualifiedName dataEncoding,
                                       ref object value,
                                       ref StatusCode statusCode,
                                       ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = entity.CountUpdates;
                    return StatusCodes.Good;
                };

                propAverage.OnReadValue = (ISystemContext context,
                                       NodeState node,
                                       NumericRange indexRange,
                                       QualifiedName dataEncoding,
                                       ref object value,
                                       ref StatusCode statusCode,
                                       ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = entity.Average;
                    return StatusCodes.Good;
                };

                propTotalTimeOn.OnReadValue = (ISystemContext context,
                                       NodeState node,
                                       NumericRange indexRange,
                                       QualifiedName dataEncoding,
                                       ref object value,
                                       ref StatusCode statusCode,
                                       ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = entity.TotalTimeOn.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                    return StatusCodes.Good;
                };
            }

            switch (ufuaTag.AccessLevel)
            {
                default:
                case UFUAModel.AccessLevels.None:
                    variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.None;
                    break;
                case UFUAModel.AccessLevels.CurrentRead:
                    variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                    break;
                case UFUAModel.AccessLevels.CurrentWrite:
                    variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentWrite;
                    break;
                case UFUAModel.AccessLevels.CurrentReadOrWrite:
                    variable.AccessLevel = variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentReadOrWrite;
                    break;
            }

            var accessSettings = new AccessSettings()
            {
                UserAccessLevel = ufuaTag.UserAccessLevel,
                UserReadAccessMask = ufuaTag.UserReadAccessMask,
                UserWriteAccessMask = ufuaTag.UserWriteAccessMask
            };
            mapNodeStateToAccessSettings.Add(variable, accessSettings);

            int samplingIntervals = -1;
            switch(ufuaTag.SamplingInterval)
            {
                case SamplingIntervals.Slow:
                    samplingIntervals = server.UFUAConfiguration.SlowDataIOSamplingInterval.Value; 
                    break;
                case SamplingIntervals.Medium:
                    samplingIntervals = server.UFUAConfiguration.MediumDataIOSamplingInterval.Value;
                    break;
                case SamplingIntervals.Fast:
                    samplingIntervals = server.UFUAConfiguration.FastDataIOSamplingInterval.Value;
                    break;
            }
            mapNodeIdToSamplingIntervals.Add(variable.NodeId, samplingIntervals);

            if (ufuaTag.AuditTraceEnabled && /*!ufuaTag.IsPrototypeMember && */!creatingType)
            {
                accessSettings.IsAuditTraceEnabled = ufuaTag.AuditTraceEnabled;
                accessSettings.IsCommentRequiredOnAudit = ufuaTag.EnterCommentOnAudit;
                accessSettings.IsPasswordRequiredOnAudit = ufuaTag.EnterPasswordOnAudit;
                accessSettings.MinAccessLevelRequiredOnAudit = ufuaTag.MinAccessLevelRequiredOnAudit;

                // remove 'CurrentWrite' access level with audit trace comment enabled
                bool originalCurrentWrite = (variable.AccessLevel & Opc.Ua.AccessLevels.CurrentWrite) == Opc.Ua.AccessLevels.CurrentWrite;
                if (accessSettings.IsCommentRequiredOnAudit)
                {
                    isAnyAuditTraceEnabled = true;
                    var newAccessLevel = (int)variable.AccessLevel;
                    newAccessLevel &= ~Opc.Ua.AccessLevels.CurrentWrite;
                    variable.AccessLevel = variable.UserAccessLevel = (byte)newAccessLevel;
                }

#if !DEBUG 
                if (initHist)
#endif
                {
                    InitHistorian(server.UFUAConfiguration.EnableEventDataProtection);

                    if (historianLogger != null)
                    {
                        variable.StateChanged += OnNodeStateChangedAudit;

                        var action = new Action(() =>
                        {
                            if (isHistoricalLoggerDisabled)
                                variable.StateChanged -= OnNodeStateChangedAudit;
                            else
                                CreateHistorianAudit(variable, ufuaTag.MaxAuditAge.Value);
                        });
                        listPendingInitTask.Add(action);
                    }
                }

                var propIsAuditTraceEnabled = AddProperty(variable, UFUAServerInfo.BrowserNames.IsAuditTraceEnabled, accessSettings.IsCommentRequiredOnAudit);
                var propAuditTraceUserName = AddProperty(variable, UFUAServerInfo.BrowserNames.LastUserNameOnAudit, accessSettings.LastUserNameOnAudit);
                var propAuditTraceComment = AddProperty(variable, UFUAServerInfo.BrowserNames.LastCommentOnAudit, accessSettings.LastCommentOnAudit);

                propIsAuditTraceEnabled.OnReadValue = (ISystemContext context,
                    NodeState node,
                    NumericRange indexRange,
                    QualifiedName dataEncoding,
                    ref object value,
                    ref StatusCode statusCode,
                    ref DateTime timestamp) =>
                {
                    //if (!CanUserReadNodeState(context, variable))
                    //{
                    //    return StatusCodes.BadUserAccessDenied;
                    //}

                    value = originalCurrentWrite && accessSettings.IsCommentRequiredOnAudit;
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
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = accessSettings.LastUserNameOnAudit;
                    return StatusCodes.Good;
                };

                propAuditTraceComment.OnReadValue = (ISystemContext context,
                    NodeState node,
                    NumericRange indexRange,
                    QualifiedName dataEncoding,
                    ref object value,
                    ref StatusCode statusCode,
                    ref DateTime timestamp) =>
                {
                    if (!CanUserReadNodeState(context, variable))
                    {
                        return StatusCodes.BadUserAccessDenied;
                    }

                    value = accessSettings.LastCommentOnAudit;
                    return StatusCodes.Good;
                };
            }
            
            variable.OnReadUserAccessLevel = OnReadUserAccessLevel;
#if !NET_STANDARD
            variable.OnReadUserAccessLevelContext = OnReadUserAccessLevelContext;
#endif

            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            variable.Historizing = false;

            variable.OnReadValue = OnReadTagValue;
            variable.OnWriteValue = OnWriteTagValue;
            if (!creatingType)
            {
                mapNodeIdToNodeState.Add(variable.NodeId, variable);
                UpdateMapInstanceState(variable);
            }

            if (!ufuaTag.ExcludeDynamicSettings && !String.IsNullOrEmpty(ufuaTag.DynamicSettings) && !NodeId.IsNull(variable.NodeId))
                mapNodeIdToDynamicSettings.Add(variable.NodeId, ufuaTag.DynamicSettings);

            AssignTagToView(ufuaTag, variable);

            if (!creatingType)
            {
                if (ufuaTag.AlwaysInUse.HasValue && ufuaTag.AlwaysInUse.Value || Properties.Settings.Default.AlwaysInUseAllVariables ||
                    (ufuaTag.UseShared.HasValue && ufuaTag.UseShared.Value && forceInUse))
                    AssignNodeToAlwaysInUseList(variable);

                if (!String.IsNullOrEmpty(ufuaTag.HistorianSettings)
#if !DEBUG 
                && initHist
#endif
)
                {
                    //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    {
                        var historianSettings = (from hissett in historians.AsParallel()
                                                 where hissett.Name == ufuaTag.HistorianSettings && hissett.Enabled == true
                                                 select hissett).ToList();// .Single();
                        if (historianSettings.Count > 0)
                        {
                            var checkUserIdentity = (from settings in historianSettings.AsParallel()
                                                     where settings.EnableDataProtection
                                                     select settings).ToList().Count > 0;

                            InitHistorian(checkUserIdentity);

                            if (historianLogger != null)
                            {
                                variable.StateChanged += OnNodeStateChangedHistorian;

                                var action = new Action(() =>
                                {
                                    if (isHistoricalLoggerDisabled)
                                        variable.StateChanged -= OnNodeStateChangedHistorian;
                                    else
                                        CreateTagHistorian(ModelUtils.ConstructNameForComponent(variable), variable.NodeId.ToString(),
                                            variable, historianSettings[0], creatingType);
                                });
                                listPendingInitTask.Add(action);
                            }
                        }

                        if (historianSettings.Count > 1)
                        {
                            if (duplicatedHistoricalNames == null)
                                duplicatedHistoricalNames = new List<String>();
                            if (!duplicatedHistoricalNames.Contains(historianSettings[0].Name))
                            {
                                duplicatedHistoricalNames.Add(historianSettings[0].Name);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.DuplicatedHistoricalPrototypeName, historianSettings[0].Name),
                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                            }
                        }
                    }
                }

                // don't load thresholds with an invalid alarm defintion reference or who were disabled by the engineering tool
                var listEnabledThresholds = (from c in alarmThresholds
                                             where c.Enabled == true &&
                                             c.UFUATagAss != null &&
                                             c.UFUATagAss.Oid == ufuaTag.Oid &&
                                             c.UFUAAlarmDefinitionRef != null &&
                                             c.UFUAAlarmDefinitionRef.UFUAAlarmDefinitions != null
                                             select c).ToList();

                if (listEnabledThresholds.Count > 0)
                {
#if !DEBUG
                if (AlarmItemsCount < maxAlarmItems)
                {
#endif
                    var analogVariable = variable as AnalogItemState;
                    if (!creatingType)
                    {
                        variable.StateChanged += OnNodeStateChangedAlarm;
                    }

                    foreach (var alarmThreshold in listEnabledThresholds)
                    {
#if !DEBUG
                        if (++AlarmItemsCount > maxAlarmItems)
                        {
                            if (!maxAlarmReached)
                            {
                                Logger.GetDestinationLog(LoggerDestination.License).Warn(string.Format(Properties.Resources.LicenseMaxAlarmReached, maxAlarmItems));
                                maxAlarmReached = true;
                            }
                            break;
                        }
#endif
                        var alarmDefinition = alarmThreshold.UFUAAlarmDefinitionRef;
                        //if (alarmDefinition == null)
                        //    continue;

                        BaseInstanceState alarmInstanceState = variable;
                        if (alarmDefinition.StatisticData != StatDef.StatProps.None)
                        {
                            var statName = StatDef.StatisticsDefinitions.statParams[alarmDefinition.StatisticData];

                            var children = new List<BaseInstanceState>();
                            variable.GetChildren(SystemContext, children);
                            var statisticData = (from child in children.AsParallel()
                                                 where child.BrowseName != null &&
                                                 child.BrowseName.Name == statName
                                                 select child).FirstOrDefault();
                            if (statisticData != null)
                            {
                                alarmInstanceState = statisticData;
                                if (!creatingType)
                                {
                                    if (!mapNodeIdToNodeState.ContainsKey(alarmInstanceState.NodeId))
                                        mapNodeIdToNodeState.Add(alarmInstanceState.NodeId, alarmInstanceState);
                                    alarmInstanceState.StateChanged += OnNodeStateChangedAlarm;
                                    updatedChangeMasksNodeStates.Add(alarmInstanceState);
                                }
                            }
                        }

                        var nodeidSource = FromGuidToNodeId(alarmDefinition.UFUAAlarmDefinitions.NodeId);
                        var nodeidReference = AlarmStatus.ConstructNodeId(ufuaTag.NodeId, parent, alarmDefinition, NamespaceIndex, alarmThreshold.Expression);
                        var alarmName = new System.Text.StringBuilder();
                        alarmName.AppendFormat("{0}:{1}", ModelUtils.ConstructNameForComponent(variable), alarmDefinition.Name);
                        alarmName.Append(alarmThreshold.GetFriendlyExpressionString());

                        var folder = mapNodeIdToSourceState[nodeidSource];
                        var aliasTags = (from c in alarmThreshold.AliasTags orderby c.Oid ascending select c.TagEntity).ToList();
                        var action = new Action(() =>
                        {
                            ExpressionValueConverter converter = null;
                            if (alarmDefinition.StatisticData == StatDef.StatProps.TotalTimeOn && alarmDefinition.AlarmType != AlarmType.TripAlarm)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.AlarmThresholdUnsupportedStaticData, alarmThreshold.Name, alarmDefinition.StatisticData),
                                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                return;
                            }
                            else if (!String.IsNullOrEmpty(alarmThreshold.Expression))
                            {
                                if (alarmDefinition.StatisticData != StatDef.StatProps.None)
                                {
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.AlarmThresholdExpressionNotSupportedOnStatistic, alarmThreshold.Name),
                                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    return;
                                }
                                else
                                {
                                    converter = new ExpressionValueConverter(alarmThreshold.Expression);
                                    converter.ThrowExceptions = true;
                                    converter.ParseFormula();
                                    var error = converter.GetParserError();
                                    if (!String.IsNullOrEmpty(error))
                                    {
                                        converter.Dispose();
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdExpressionError, error, alarmThreshold.Name),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                        return;
                                    }
                                }
                            }

                            var startMessageName = ModelUtils.ConstructNameForComponent(alarmInstanceState, '/', 
                                FromGuidToNodeId(UFUAServerInfo.Guids.RootTagsGuid));
                            var settings = new AlarmStatus()
                            {
                                nodeId = nodeidReference,
                                Name = alarmName.ToString(),
                                Message = alarmThreshold.GetAlarmMessage(startMessageName),
                                type = alarmDefinition.AlarmType.Value,
                                TripCondition = (TripCondition)alarmDefinition.ConditionType,
                                ActivationLowValue = alarmDefinition.ActivationLowValue.Value,
                                ActivationValue = alarmDefinition.ActivationValue.Value,
                                ExceptionDeviationFormat = (ExceptionDeviationFormat)alarmDefinition.DeviationType,
                                Limits = new double?[4],
                                LimitsQuality = new uint?[4],
                                range = analogVariable != null && analogVariable.EURange != null ? analogVariable.EURange.Value : null,
                                instrumentRange = analogVariable != null && analogVariable.InstrumentRange != null ? analogVariable.InstrumentRange.Value : null,
                                Severity = (EventSeverity)alarmDefinition.Severity,
                                QualityGoodOnly = alarmThreshold.IsQualityGoodEnabled,
                                BeepEnabled = alarmThreshold.IsBeepEnabled,
                                SoundFile = alarmDefinition.SoundFile,
                                RepeatSoundContinuously = alarmDefinition.RepeatSoundContinuously,
                                UseTimeStamp = alarmDefinition.UseTimeStamp,
                                SaveEventsLog = alarmDefinition.SaveEventsLog.Value,
                                SaveBranches = alarmDefinition.SaveBranches,
                                SupportAck = alarmDefinition.SupportAck.Value,
                                SupportReset = alarmDefinition.SupportReset.Value,
                                Expression = alarmThreshold.Expression,
                                TimeUnit = alarmDefinition.TimeUnit.TotalMilliseconds,
                                DelayTimeOn = alarmDefinition.DelayTimeOn.TotalMilliseconds,
                                DelayTimeOff = alarmDefinition.DelayTimeOff.TotalMilliseconds,
                                TagAliasLastValues = aliasTags.Count > 0 ? new DataValue[aliasTags.Count] : null,
                                isTotalTimeOn = alarmDefinition.StatisticData == StatDef.StatProps.TotalTimeOn,
                                StatisticsData = mapNodeIdToTagLogEntities.ContainsKey(variable.NodeId) && mapNodeIdToTagLogEntities[variable.NodeId].enableStatistics ? mapNodeIdToTagLogEntities[variable.NodeId] : null,
                                CreateAlrMemberAckedState = alarmDefinition.CreateAlrMemberAckedState.Value,
                                CreateAlrMemberConfirmedState = alarmDefinition.CreateAlrMemberConfirmedState.Value,
                                CreateAlrMemberConfirm = alarmDefinition.CreateAlrMemberConfirm.Value,
                                CreateAlrMemberSuppressedState = alarmDefinition.CreateAlrMemberSuppressedState.Value,
                                CreateAlrMemberShelvingState = alarmDefinition.CreateAlrMemberShelvingState.Value,
                                CreateAlrMemberEnabledState = alarmDefinition.CreateAlrMemberEnabledState.Value,
                                CreateAlrMemberActiveState = alarmDefinition.CreateAlrMemberActiveState.Value,
                                CreateAlrMemberQuality = alarmDefinition.CreateAlrMemberQuality.Value
#if NET_STANDARD
                                ,CreateAlrMemberLocalTime = alarmDefinition.CreateAlrMemberLocalTime.Value
#endif
                            };

                            settings.Limits[0] = alarmDefinition.EnableHighHighLimit ? alarmDefinition.HighHighLimit : null;
                            settings.Limits[1] = alarmDefinition.EnableHighLimit ? alarmDefinition.HighLimit : null;
                            settings.Limits[2] = alarmDefinition.EnableLowLimit ? alarmDefinition.LowLimit : null;
                            settings.Limits[3] = alarmDefinition.EnableLowLowLimit ? alarmDefinition.LowLowLimit : null;

                            lock (Lock)
                            {
                                var duplicate = (from c in mapAlarmStatusToSourceState.Keys/*.AsParallel()*/ where c.nodeId == nodeidReference select c.nodeId).ToList().Count > 0;
                                //if (mapNodeIdToEventLogEntities.ContainsKey(nodeidReference))
                                if (duplicate)
                                {
                                    string msg = String.Format(Properties.Resources.DuplicatedAlarmThreshold, alarmName, ufuaTag.GetRelativeName());
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, msg, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                    return;
                                }

                                if (!mapNodeIdToAlarmStatus.ContainsKey(alarmInstanceState.NodeId))
                                {
                                    mapNodeIdToAlarmStatus[alarmInstanceState.NodeId] = new List<AlarmStatus>();
                                    AssignNodeToAlwaysInUseList(variable);
                                }
                                mapNodeIdToAlarmStatus[alarmInstanceState.NodeId].Add(settings);
                                if (!mapAlarmStatusNodeIdToVariableNodeId.ContainsKey(settings.nodeId))
                                    mapAlarmStatusNodeIdToVariableNodeId.Add(settings.nodeId, variable.NodeId);
                                mapAlarmStatusToSourceState[settings] = folder;
                                if (converter != null)
                                    mapAlarmStatusToExpressionValueConverter[settings] = converter;

                                if (!alarmThreshold.EnableTag.IsEmpty())
                                {
                                    var nodeid = alarmThreshold.EnableTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidEnableTag, alarmThreshold.Name, alarmThreshold.EnableTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapEnableStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapEnableStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapEnableStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.ActivationLowValueTag.IsEmpty())
                                {
                                    settings.ActivationLowValueQuality = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.ActivationLowValueTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidActivationTag, alarmThreshold.Name, alarmThreshold.ActivationLowValueTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapActivationLowStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapActivationLowStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapActivationLowStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.ActivationValueTag.IsEmpty())
                                {
                                    settings.ActivationValueQuality = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.ActivationValueTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidActivationTag, alarmThreshold.Name, alarmThreshold.ActivationValueTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapActivationStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapActivationStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapActivationStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.HighHighLimitTag.IsEmpty())
                                {
                                    settings.LimitsQuality[0] = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.HighHighLimitTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidLimitTag, alarmThreshold.Name, alarmThreshold.HighHighLimitTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapHighHighStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapHighHighStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapHighHighStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.HighLimitTag.IsEmpty())
                                {
                                    settings.LimitsQuality[1] = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.HighLimitTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidLimitTag, alarmThreshold.Name, alarmThreshold.HighLimitTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapHighStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapHighStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapHighStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.LowLimitTag.IsEmpty())
                                {
                                    settings.LimitsQuality[2] = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.LowLimitTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidLimitTag, alarmThreshold.Name, alarmThreshold.LowLimitTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapLowStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapLowStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapLowStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                if (!alarmThreshold.LowLowLimitTag.IsEmpty())
                                {
                                    settings.LimitsQuality[3] = StatusCodes.UncertainInitialValue;
                                    var nodeid = alarmThreshold.LowLowLimitTag.ResolveNodeId(NamespaceIndex);
                                    if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.AlarmThresholdInvalidLimitTag, alarmThreshold.Name, alarmThreshold.LowLowLimitTag),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                    else
                                    {
                                        if (!mapLowLowStateToAlarmStatus.ContainsKey(nodeid))
                                        {
                                            mapLowLowStateToAlarmStatus[nodeid] = new List<AlarmStatus>();
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                        mapLowLowStateToAlarmStatus[nodeid].Add(settings);
                                    }
                                }

                                for (int ii = 0; ii < aliasTags.Count; ii++)
                                {
                                    if (!aliasTags[ii].IsEmpty())
                                    {
                                        var nodeid = aliasTags[ii].ResolveNodeId(NamespaceIndex);
                                        if (!mapNodeIdToNodeState.ContainsKey(nodeid))
                                        {
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                String.Format(Properties.Resources.AlarmThresholdInvalidAliasTag, alarmThreshold.Name, aliasTags[ii]),
                                                System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                        }
                                        else
                                        {
                                            if (!mapAlarmStatusToAliasPosition.ContainsKey(settings))
                                                mapAlarmStatusToAliasPosition[settings] = new Dictionary<NodeId, int>();
                                            mapAlarmStatusToAliasPosition[settings][nodeid] = ii;
                                            AssignNodeToAlwaysInUseList(nodeid);
                                        }
                                    }
                                }

                                //if (!mapNodeIdToEventLogEntities.ContainsKey(nodeidReference))
                                //{
                                //    mapNodeIdToEventLogEntities[nodeidReference] = new UFUAEventLogEntity()
                                //    {
                                //        EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                                //        MaxAge = server.UFUAConfiguration.EventMaxAge
                                //    };
                                //}
                            }

                            var alarm = folder.GetOrCreateAlarmConditionState(settings, conditionaName: alarmThreshold.GetUniqueConditionName());
                            lock (Lock)
                            {
                                if (!alarm.ReferenceExists(ReferenceTypeIds.HasCondition, true, variable.NodeId))
                                    alarm.AddReference(ReferenceTypeIds.HasCondition, true, variable.NodeId);
                            }

                            //if (parentFolder != null)
                            //    parentFolder.AddChild(alarm);

                            folder.LoadAlarmStatus(settings, alarmThreshold.SettingsTimeStamp);
                        });
                        listPendingInitTask.Add(action);

                        //folder.UpdateAlarmStatus(settings, new DataValue
                        //{
                        //    Value = variable.Value,
                        //    ServerTimestamp = DateTime.UtcNow,
                        //    SourceTimestamp = DateTime.MinValue,
                        //    StatusCode = StatusCodes.Good
                        //});
                    }
#if !DEBUG
                }
                else if (!maxAlarmReached)
                {
                    Logger.GetDestinationLog(LoggerDestination.License).Warn(string.Format(Properties.Resources.LicenseMaxAlarmReached, maxAlarmItems));
                    maxAlarmReached = true;
                }
#endif

                }
            }

#if !NET_STANDARD
#if !CONNEXT
            if (!String.IsNullOrEmpty(ufuaTag.ScriptCode))
            {
                InitializeScriptCode(ufuaTag.ScriptCode, ufuaTag.GetListProcedures(), ufuaTag.Breakpoints, ufuaTag.UseSeparateThreadScriptExecuter, variable);
            }
#endif
            if (!creatingType && server.IsRedundancyEnabled && !ufuaTag.IsRedundancyEnabled.Value)
            {
                redundancyPrivateNodeIds.Add(variable.NodeId);
            }
#endif

            return variable;
        }

        PropertyState AddProperty(BaseVariableState variable, String browseName, Object value)
        {
            var property = new PropertyState(variable)
            {
                ReferenceTypeId = ReferenceTypeIds.HasProperty,
                TypeDefinitionId = VariableTypeIds.PropertyType,
                SymbolicName = browseName,
                BrowseName = browseName,
                Value = value,
                Description = null,
                WriteMask = 0,
                UserWriteMask = 0,
                DataType = variable.DataType,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = Opc.Ua.AccessLevels.CurrentRead,
                UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead,
                MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
                Historizing = false
            };

            property.DisplayName = new LocalizedText(browseName, string.Empty, browseName);
            variable.AddChild(property);

            property.NodeId = ModelUtils.ConstructIdForComponent(property, NamespaceIndex);

            return property;
        }

        /*
        BaseInstanceState CreateDiagnosticObject(String driverName, NodeId parent, NodeState parentFolder = null)
        {
            string tagname = string.Format("{0}Statistics", driverName);
            NodeId nodeId = null;
            if (parentFolder == null || parentFolder is FolderState)
                nodeId = new NodeId(tagname, NamespaceIndex);

            var newbaseObject = new BaseObjectState(parentFolder);//CreateObject(tagname, "DriverDiagnosticPrototype", parentFolder, nodeId);
            newbaseObject.BrowseName = new QualifiedName(tagname, NamespaceIndex);
            newbaseObject.DisplayName = newbaseObject.BrowseName.Name;
            newbaseObject.SymbolicName = newbaseObject.BrowseName.Name;

            //newbaseObject.TypeDefinitionId = superTypeId;
            newbaseObject.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            if (nodeId == null)
                newbaseObject.NodeId = New(SystemContext, newbaseObject);
            else
                newbaseObject.NodeId = nodeId;

            //create any folders
            //CreateFolder(folder, baseObject.NodeId, baseObject, true, false, true, forceRetentive);

            //create tags
            //CreateTag(tag, baseObject.NodeId, baseObject, true, false, forceRetentive);
            {
                string dname = "Diagno01";
                BaseDataVariableState variable = new DataItemState(newbaseObject);

                variable.SymbolicName = dname;
                variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;

                variable.Create(
                SystemContext,
                variable.NodeId,
                new QualifiedName(dname, NamespaceIndex),
                null,
                true);

                //else if (ufuaTag.ModelType == UFUAModel.ModelType.Variable)
                //{
                //    if ((!ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings)) ||
                //        !string.IsNullOrEmpty(ufuaTag.Description))
                //    {
                //        var node = variable as DataItemState;
                //
                //        node.Definition = new PropertyState<String>(node);
                //        node.Definition.ReferenceTypeId = ReferenceTypes.HasProperty;
                //       node.Definition.ModellingRuleId = null;
                //        node.Definition.TypeDefinitionId = VariableTypeIds.PropertyType;
                //        node.Definition.SymbolicName = Opc.Ua.BrowseNames.Definition;
                //        node.Definition.BrowseName = Opc.Ua.BrowseNames.Definition;
                //        node.Definition.DisplayName = Opc.Ua.BrowseNames.Definition;
                //        node.Definition.Description = null;
                //        node.Definition.WriteMask = 0;
                //        node.Definition.UserWriteMask = 0;
                //        node.Definition.DataType = DataTypeIds.String;
                //        node.Definition.ValueRank = ValueRanks.Scalar;
                //        node.Definition.ArrayDimensions = null;
                //        node.Definition.AccessLevel = AccessLevels.CurrentRead;
                //        node.Definition.UserAccessLevel = AccessLevels.CurrentRead;
                //        node.Definition.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
                //        node.Definition.Historizing = false;
                //        node.Definition.NodeId = ModelUtils.ConstructIdForComponent(node.Definition, NamespaceIndex);
                //        node.Definition.Value = !ufuaTag.ExcludeDynamicSettings && !ufuaTag.ExcludeDynamicSettings && !string.IsNullOrEmpty(ufuaTag.DynamicSettings) ? ufuaTag.DynamicSettings : ufuaTag.Description;
                //        node.Definition.Timestamp = ufuaTag.SettingsTimeStamp;
                //    }
                //}

                //if (newbaseObject == null)
                //{
                //    variable.AddReference(ReferenceTypeIds.Organizes, true, parent);
                //}
                //else
                {
                    if (newbaseObject is BaseObjectState)
                        (newbaseObject as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                    newbaseObject.AddChild(variable);
                }

                variable.Description = "Diagno01 description";


                //switch (ufuaTag.DataType)
                //{
                //    case UFUAModel.DataType.Boolean: { variable.DataType = DataTypeIds.Boolean; break; }
                //    case UFUAModel.DataType.SByte: { variable.DataType = DataTypeIds.SByte; break; }
                //    case UFUAModel.DataType.Byte: { variable.DataType = DataTypeIds.Byte; break; }
                //    case UFUAModel.DataType.Int16: { variable.DataType = DataTypeIds.Int16; break; }
                //    case UFUAModel.DataType.UInt16: { 
                variable.DataType = DataTypeIds.UInt16;// break; }
                //    case UFUAModel.DataType.Int32: { variable.DataType = DataTypeIds.Int32; break; }
                //    case UFUAModel.DataType.UInt32: { variable.DataType = DataTypeIds.UInt32; break; }
                //    case UFUAModel.DataType.Int64: { variable.DataType = DataTypeIds.Int64; break; }
                //    case UFUAModel.DataType.UInt64: { variable.DataType = DataTypeIds.UInt64; break; }
                //    case UFUAModel.DataType.Float: { variable.DataType = DataTypeIds.Float; break; }
                //    case UFUAModel.DataType.Double: { variable.DataType = DataTypeIds.Double; break; }
                //    case UFUAModel.DataType.String: { variable.DataType = DataTypeIds.String; break; }
                //}

                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                //if (!String.IsNullOrEmpty(ufuaTag.InitialValue))
                //{
                //    try
                //    {
                //        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                //        variable.Value = ChangeType(ufuaTag.InitialValue, builtinType, ufuaTag.ArrayDimension, info);
                //    }
                //    catch (Exception ex) 
                //    {
                //        Logger.WriteToEventLog(Properties.Resources.LoggerSource, ex.ToString(), System.Diagnostics.EventLogEntryType.Warning);
                //        Utils.Trace(ex, String.Format("Unexpected error setting the initial value '{0}' for tag '{1}\\{2}'", ufuaTag.InitialValue, ufuaTag.FolderPath, ufuaTag.Name));
                //    }
                //}
                //else if (ufuaTag.ArrayDimension == 0)
                variable.Value = TypeInfo.GetDefaultValue(builtinType);
                //else
                //{
                //    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)ufuaTag.ArrayDimension);
                //    variable.Value = new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, CastArrayElement));
                //}
                variable.Timestamp = DateTime.UtcNow;

                //if (ufuaTag.ArrayDimension == 0)
                //{
                variable.ValueRank = ValueRanks.Scalar;
                variable.ArrayDimensions = null;
                //}
                //else
                //{
                //    variable.ValueRank = ValueRanks.OneDimension;
                //    List<uint> dimList = new List<uint>(1);
                //    dimList.Add(ufuaTag.ArrayDimension);
                //    variable.ArrayDimensions = new ReadOnlyList<uint>(dimList);
                //}

                //if (ufuaTag.IsWriteable)
                //{
                //    variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
                //    variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                //}
                //else
                //{
                variable.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
                //}
                variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
                variable.Historizing = false;

                mapNodeIdToNodeState.Add(variable.NodeId, variable);
                UpdateMapInstanceState(variable);
            }

            if (newbaseObject == null)
                return null;


            if (parentFolder == null)
            {
                newbaseObject.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(newbaseObject);
            }

            AddPredefinedNode(SystemContext, newbaseObject);
            mapNodeIdToNodeState.Add(newbaseObject.NodeId, newbaseObject);
            UpdateMapInstanceState(newbaseObject);

            return newbaseObject;
        }
        */

        BaseInstanceState CreateDriverDiagnosticObject(String driverName, NodeId parent, NodeState parentFolder = null)
        {
            server.CommDrivers[driverName].SetDiagnosticFolder(parent.Identifier.ToString(), NamespaceIndex);

            var newbaseObject = AddDiagFolder(string.Format("{0}Statistics", server.CommDrivers[driverName].GetDriverName()),
               parent, parentFolder);
            if (newbaseObject == null)
                return null;

            foreach (StatisicTag DiagVar in server.CommDrivers[driverName].GetDriverDiagVar())
                AddDiagVar(newbaseObject, DiagVar);

            var channelbaseObject = AddDiagFolder("Channels",newbaseObject.NodeId, newbaseObject);
            if (channelbaseObject == null)
                return null;
            foreach (string channelName in server.CommDrivers[driverName].GetChannelsNames())
            {
                var channeObject = AddDiagFolder(channelName, channelbaseObject.NodeId, channelbaseObject);
                if (channeObject == null)
                    return null;
                foreach (StatisicTag DiagVar in server.CommDrivers[driverName].GetChannelDiagVar())
                    AddDiagVar(channeObject, DiagVar);
            }

            var stationbaseObject = AddDiagFolder("Stations",newbaseObject.NodeId, newbaseObject);
            if (stationbaseObject == null)
                return null;
            foreach (string stationName in server.CommDrivers[driverName].GetStationsNames())
            {
                var stationObject = AddDiagFolder(stationName, stationbaseObject.NodeId, stationbaseObject);
                if (stationObject == null)
                    return null;
                foreach (StatisicTag DiagVar in server.CommDrivers[driverName].GetStationDiagVar())
                    AddDiagVar(stationObject, DiagVar);
            }

            var commandbaseObject = AddDiagFolder("Command",newbaseObject.NodeId, newbaseObject);
            CreateDiagnosticMethod("ResumeDiagnostic", OnResumeDiagnostic, driverName, commandbaseObject.NodeId, commandbaseObject);
            CreateDiagnosticMethod("SuspendDiagnostic", OnSuspendDiagnostic, driverName, commandbaseObject.NodeId, commandbaseObject);
            CreateDiagnosticMethod("ResetDiagnostic", OnResetDiagnostic, driverName, commandbaseObject.NodeId, commandbaseObject);

            return newbaseObject;
        }

        private BaseObjectState AddDiagFolder(string newFolderName, NodeId parent, NodeState parentFolder = null)
        {
            var newFolderObject = new FolderState(parentFolder);//CreateObject(tagname, "DriverDiagnosticPrototype", parentFolder, nodeId);
            if (newFolderObject == null)
                return null;
            newFolderObject.BrowseName = new QualifiedName(newFolderName, NamespaceIndex);
            newFolderObject.DisplayName = newFolderObject.BrowseName.Name;
            newFolderObject.SymbolicName = newFolderObject.BrowseName.Name;

            //newbaseObject.TypeDefinitionId = superTypeId;
            newFolderObject.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            newFolderObject.TypeDefinitionId = ObjectTypeIds.FolderType;
            newFolderObject.NodeId = New(SystemContext, newFolderObject);


            if (parentFolder == null)
            {
                newFolderObject.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(newFolderObject);
            }
            AddPredefinedNode(SystemContext, newFolderObject);
            mapNodeIdToNodeState.Add(newFolderObject.NodeId, newFolderObject);
            return newFolderObject;
        }

        BaseInstanceState CreateDiagnosticMethod(String Name, GenericMethodCalledEventHandler OnMethodCall, String driverName, NodeId parent, NodeState parentFolder = null)
        {

            var methodName = String.Format("{0}", Name);
            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = New(SystemContext, methodBase);

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, false);

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

            // set up method handlers. 
            methodBase.OnCallMethod = OnMethodCall;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

            AddCommunicationDriverToNodeStateDictionary(methodBase, driverName);

            return methodBase;
        }


        public void AddDiagVar(BaseObjectState newbaseObject, StatisicTag diagVar)
        {
            BaseDataVariableState variable = new DataItemState(newbaseObject);

            variable.SymbolicName = diagVar.Name;
            variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            variable.Create(
            SystemContext,
            variable.NodeId,
            new QualifiedName(diagVar.Name, NamespaceIndex),
            null,
            true);

            {
                if (newbaseObject is BaseObjectState)
                    (newbaseObject as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                newbaseObject.AddChild(variable);
            }

            variable.Description = diagVar.Description;


            switch (diagVar.DataType)
            {
                case UFUAModel.DataType.Boolean: { variable.DataType = DataTypeIds.Boolean; break; }
                case UFUAModel.DataType.SByte: { variable.DataType = DataTypeIds.SByte; break; }
                case UFUAModel.DataType.Byte: { variable.DataType = DataTypeIds.Byte; break; }
                case UFUAModel.DataType.Int16: { variable.DataType = DataTypeIds.Int16; break; }
                case UFUAModel.DataType.UInt16: { variable.DataType = DataTypeIds.UInt16; break; }
                case UFUAModel.DataType.Int32: { variable.DataType = DataTypeIds.Int32; break; }
                case UFUAModel.DataType.UInt32: { variable.DataType = DataTypeIds.UInt32; break; }
                case UFUAModel.DataType.Int64: { variable.DataType = DataTypeIds.Int64; break; }
                case UFUAModel.DataType.UInt64: { variable.DataType = DataTypeIds.UInt64; break; }
                case UFUAModel.DataType.Float: { variable.DataType = DataTypeIds.Float; break; }
                case UFUAModel.DataType.Double: { variable.DataType = DataTypeIds.Double; break; }
                case UFUAModel.DataType.String: { variable.DataType = DataTypeIds.String; break; }
            }

            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            variable.Value = ChangeType(variable.Value, builtinType);
            //if (!String.IsNullOrEmpty(ufuaTag.InitialValue))
            //{
            //    try
            //    {
            //        System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
            //        variable.Value = ChangeType(ufuaTag.InitialValue, builtinType, ufuaTag.ArrayDimension, info);
            //    }
            //    catch (Exception ex) 
            //    {
            //        Logger.WriteToEventLog(Properties.Resources.LoggerSource, ex.ToString(), System.Diagnostics.EventLogEntryType.Warning);
            //        Utils.Trace(ex, String.Format("Unexpected error setting the initial value '{0}' for tag '{1}\\{2}'", ufuaTag.InitialValue, ufuaTag.FolderPath, ufuaTag.Name));
            //    }
            //}
            //else if (ufuaTag.ArrayDimension == 0)
            variable.Value = TypeInfo.GetDefaultValue(builtinType);
            //else
            //{
            //    var array = Opc.Ua.TypeInfo.CreateArray(builtinType, (int)ufuaTag.ArrayDimension);
            //    variable.Value = new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, CastArrayElement));
            //}
            variable.Timestamp = DateTime.UtcNow;

            //if (ufuaTag.ArrayDimension == 0)
            //{
            variable.ValueRank = ValueRanks.Scalar;
            variable.ArrayDimensions = null;
            //}
            //else
            //{
            //    variable.ValueRank = ValueRanks.OneDimension;
            //    List<uint> dimList = new List<uint>(1);
            //    dimList.Add(ufuaTag.ArrayDimension);
            //    variable.ArrayDimensions = new ReadOnlyList<uint>(dimList);
            //}

            //if (ufuaTag.IsWriteable)
            //{
            //    variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            //    variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            //}
            //else
            //{
            variable.AccessLevel = Opc.Ua.AccessLevels.CurrentRead;
            variable.UserAccessLevel = Opc.Ua.AccessLevels.CurrentRead;
            //}
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            variable.Historizing = false;

            mapNodeIdToNodeState.Add(variable.NodeId, variable);
            UpdateMapInstanceState(variable);
        }
        


        BaseInstanceState CreateReadValuesMethod(String driverName, NodeId parent, NodeState parentFolder = null)
        {
            var methodName = String.Format("{0}.ReadValues", driverName);
            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = new NodeId(methodName, NamespaceIndex);

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, false);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "StartingAddress",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

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

            // set up method handlers. 
            methodBase.OnCallMethod = OnReadValues;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

            AddCommunicationDriverToNodeStateDictionary(methodBase, driverName);

            return methodBase;
        }

        BaseInstanceState CreateWriteValuesMethod(String driverName, NodeId parent, NodeState parentFolder = null)
        {
            var methodName = String.Format("{0}.WriteValues", driverName);
            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = new NodeId(methodName, NamespaceIndex);

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, false);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "StartingAddress",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Values",
                DataType = DataTypeIds.BaseDataType,
                ValueRank = ValueRanks.OneDimension
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            /*
            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "Result",
                Description = new LocalizedText("Operation Result", String.Empty, "Operation Result"),
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });
            */

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

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

            // set up method handlers. 
            methodBase.OnCallMethod = OnWriteValues;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

            AddCommunicationDriverToNodeStateDictionary(methodBase, driverName);

            return methodBase;
        }

        static object CastArrayElement(object source, BuiltInType srcType, BuiltInType dstType)
        {
            return ChangeType(source, dstType);
        }

        static object ChangeType(Object v, BuiltInType builtinType, uint arraySizeOneDimension = 0, System.Globalization.NumberFormatInfo info = null, bool skipNullValues = false)
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
                        array.SetValue(ChangeType(values.GetValue(ii), builtinType, 0, info, skipNullValues), ii);
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
                        array.SetValue(ChangeType(values[ii], builtinType, 0, info, skipNullValues), ii);
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

            if (!skipNullValues || v != null)
            {
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
            }

            return value;
        }

        public ServiceResult OnReadUserAccessLevel(ISystemContext context, NodeState node, ref byte value)
        {
            var token = context.UserIdentity as UserIdentity;
            value = GetUserAccessLevel(token, node);
            return ServiceResult.Good;
        }

        public ServiceResult OnReadUserAccessLevelContext(IOperationContext context, NodeState node, ref byte value)
        {
            var token = context.UserIdentity as UserIdentity;
            value = GetUserAccessLevel(token, node);
            return ServiceResult.Good;
        }

        protected byte GetUserAccessLevel(ISystemContext context, NodeState node)
        {
            var token = context.UserIdentity as UserIdentity;
            return GetUserAccessLevel(token, node);
        }

        byte GetUserAccessLevel(UserIdentity token, NodeState node)
        {
            if (!server.IsUserManagerEnabled())
            {
                if (node is BaseVariableState && (node as BaseVariableState).Historizing)
                    return Opc.Ua.AccessLevels.CurrentReadOrWrite | Opc.Ua.AccessLevels.HistoryReadOrWrite;
                else
                    return Opc.Ua.AccessLevels.CurrentReadOrWrite;
            }

            var level = Opc.Ua.AccessLevels.None;
            if (mapNodeStateToAccessSettings.ContainsKey(node))
            {
                var settings = mapNodeStateToAccessSettings[node];
                level = server.MatchUserReadAccessLevel(token, settings.UserAccessLevel, settings.UserReadAccessMask,
                    settings.UserWriteAccessMask);
            }

            if (node is BaseVariableState && (node as BaseVariableState).Historizing)
            {
                if ((level & Opc.Ua.AccessLevels.CurrentRead) == Opc.Ua.AccessLevels.CurrentRead)
                    level |= Opc.Ua.AccessLevels.HistoryRead;
                if ((level & Opc.Ua.AccessLevels.CurrentWrite) == Opc.Ua.AccessLevels.CurrentWrite)
                    level |= Opc.Ua.AccessLevels.HistoryWrite;
            }

            return level;
        }

        bool CanUserReadNodeState(ISystemContext context, NodeState node)
        {
            if (context == SystemContext)
                return true;

            var level = GetUserAccessLevel(context, node);
            return (level & Opc.Ua.AccessLevels.CurrentRead) == Opc.Ua.AccessLevels.CurrentRead ||
                (level & Opc.Ua.AccessLevels.CurrentReadOrWrite) == Opc.Ua.AccessLevels.CurrentReadOrWrite;
        }

        bool CanUserWriteNodeState(ISystemContext context, NodeState node)
        {
            if (context == SystemContext)
                return true;

            var level = GetUserAccessLevel(context, node);
            return (level & Opc.Ua.AccessLevels.CurrentWrite) == Opc.Ua.AccessLevels.CurrentWrite ||
                (level & Opc.Ua.AccessLevels.CurrentReadOrWrite) == Opc.Ua.AccessLevels.CurrentReadOrWrite;
        }

        bool IsAnyAuditTraceEnabled()
        {
            return isAnyAuditTraceEnabled;
        }

        bool IsAnyTagStatisticsEnabled()
        {
            return isAnyTagStatisticsEnabled;
        }

        /// <summary>
        /// Used to receive notifications when the value attribute is read or written.
        /// </summary>
        public ServiceResult OnReadLocalizedTextValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (!CanUserReadNodeState(context, node))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            if (value is LocalizedText)
            {
                var text = value as LocalizedText;
                value = Server.ResourceManager.Translate(context.PreferredLocales, text);
            }
            else if (value is LocalizedText[])
            {
                var texts = value as LocalizedText[];
                for (int ii = 0; ii < texts.Length; ii++)
                    texts[ii] = Server.ResourceManager.Translate(context.PreferredLocales, texts[ii]);
            }

            // apply the index range and encoding.
            ServiceResult result = BaseVariableState.ApplyIndexRangeAndDataEncoding(context, indexRange, dataEncoding, ref value);

            if (ServiceResult.IsBad(result))
            {
                return result;
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Used to receive notifications when the value attribute is read or written.
        /// </summary>
        public ServiceResult OnReadTagValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (!CanUserReadNodeState(context, node))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            var dataVariable = node as BaseDataVariableState;
            if (dataVariable != null && dataVariable.ValueRank == ValueRanks.Scalar && value == null)
            {
                BuiltInType builtinType = TypeInfo.GetBuiltInType(dataVariable.DataType, Server.TypeTree);
                if (builtinType == BuiltInType.String)
                    value = String.Empty;
                else
                    value = TypeInfo.GetDefaultValue(builtinType);
            }

            // apply the index range and encoding.
            ServiceResult result = BaseVariableState.ApplyIndexRangeAndDataEncoding(context, indexRange, dataEncoding, ref value);

            if (ServiceResult.IsBad(result))
            {
                return result;
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Used to receive notifications when the value attribute is read or written.
        /// </summary>
        public ServiceResult OnWriteTagValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            if (!CanUserWriteNodeState(context, node))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, node, GetUserName(context));
            var eventName = String.Format(Properties.Resources.WriteTagValueEventLog, value);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            var dataVariable = node as BaseDataVariableState;
            if (dataVariable != null && dataVariable.ValueRank == ValueRanks.Scalar && value == null)
                return StatusCodes.BadTypeMismatch;

            // apply the index range.
            var variable = node as BaseVariableState;
            if (variable != null && indexRange != NumericRange.Empty)
            {
                object target = Opc.Ua.Utils.Clone(variable.Value);
                TypeInfo dstTypeInfo = TypeInfo.Construct(target);
                if (dstTypeInfo.BuiltInType == BuiltInType.ByteString)
                    dstTypeInfo = new TypeInfo(BuiltInType.Byte, ValueRanks.OneDimension);

                if (dstTypeInfo.ValueRank == ValueRanks.Scalar &&
                    indexRange.Dimensions == 1 && indexRange.Count != 0)
                {
                    if (dstTypeInfo.BuiltInType == BuiltInType.Byte)
                    {
                        if (indexRange.Begin > 7 || indexRange.End > 7)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToByte(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (byte)((byte)1 << indexRange.Begin);
                            else
                                sValue &= (byte)(~((byte)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (byte)((byte)1 << index);
                                else
                                    sValue &= (byte)(~((byte)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.SByte)
                    {
                        if (indexRange.Begin > 7 || indexRange.End > 7)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToSByte(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (sbyte)((sbyte)1 << indexRange.Begin);
                            else
                                sValue &= (sbyte)(~((sbyte)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (sbyte)((sbyte)1 << index);
                                else
                                    sValue &= (sbyte)(~((sbyte)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int16)
                    {
                        if (indexRange.Begin > 15 || indexRange.End > 15)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt16(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (short)((short)1 << indexRange.Begin);
                            else
                                sValue &= (short)(~((short)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (short)((short)1 << index);
                                else
                                    sValue &= (short)(~((short)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt16)
                    {
                        if (indexRange.Begin > 15 || indexRange.End > 15)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt16(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (ushort)((ushort)1 << indexRange.Begin);
                            else
                                sValue &= (ushort)(~((ushort)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (ushort)((ushort)1 << index);
                                else
                                    sValue &= (ushort)(~((ushort)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int32)
                    {
                        if (indexRange.Begin > 31 || indexRange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt32(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= 1 << indexRange.Begin;
                            else
                                sValue &= ~(1 << indexRange.Begin);
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= 1 << index;
                                else
                                    sValue &= ~(1 << index);
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt32)
                    {
                        if (indexRange.Begin > 31 || indexRange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt32(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (uint)((uint)1 << indexRange.Begin);
                            else
                                sValue &= (uint)(~((uint)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (uint)((uint)1 << index);
                                else
                                    sValue &= (uint)(~((uint)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int64 || dstTypeInfo.BuiltInType == BuiltInType.Integer)
                    {
                        if (indexRange.Begin > 63 || indexRange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt64(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << indexRange.Begin);
                            else
                                sValue &= (long)(~((long)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt64 || dstTypeInfo.BuiltInType == BuiltInType.UInteger)
                    {
                        if (indexRange.Begin > 63 || indexRange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt64(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (ulong)((ulong)1 << indexRange.Begin);
                            else
                                sValue &= (ulong)(~((ulong)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (ulong)((ulong)1 << index);
                                else
                                    sValue &= (ulong)(~((ulong)1 << index));
                            }
                        }

                        value = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Float)
                    {
                        if (indexRange.Begin > 31 || indexRange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = (long)Convert.ToSingle(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << indexRange.Begin);
                            else
                                sValue &= (long)(~((long)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        value = Opc.Ua.TypeInfo.Cast(sValue, dstTypeInfo.BuiltInType);
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Double || dstTypeInfo.BuiltInType == BuiltInType.Number)
                    {
                        if (indexRange.Begin > 63 || indexRange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = (long)Convert.ToDouble(target);
                        if (indexRange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << indexRange.Begin);
                            else
                                sValue &= (long)(~((long)1 << indexRange.Begin));
                        }
                        else
                        {
                            for (int index = indexRange.Begin; index <= indexRange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        value = Opc.Ua.TypeInfo.Cast(sValue, dstTypeInfo.BuiltInType);
                    }
                    else
                    {
                        var result = indexRange.UpdateRange(ref target, value);

                        if (ServiceResult.IsBad(result))
                        {
                            return result;
                        }

                        value = target;
                    }
                }
                else if (dstTypeInfo.ValueRank == ValueRanks.OneDimension &&
                    indexRange.Dimensions == 2 && indexRange.SubRanges[0].Count == 1 && indexRange.SubRanges[1].Count != 0)
                {
                    Array targetArray = target as Array;
                    // check for invalid target.
                    if (targetArray == null)
                    {
                        return StatusCodes.BadIndexRangeInvalid;
                    }

                    if (indexRange.Begin >= targetArray.Length)
                    {
                        return StatusCodes.BadIndexRangeNoData;
                    }

                    var subrange = indexRange.SubRanges[1];
                    var element = targetArray.GetValue(indexRange.Begin);
                    object source = null;
                    
                    if (dstTypeInfo.BuiltInType == BuiltInType.Byte)
                    {
                        if (subrange.Begin > 7 || subrange.End > 7)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToByte(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (byte)((byte)1 << subrange.Begin);
                            else
                                sValue &= (byte)(~((byte)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (byte)((byte)1 << index);
                                else
                                    sValue &= (byte)(~((byte)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.SByte)
                    {
                        if (subrange.Begin > 7 || subrange.End > 7)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToSByte(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (sbyte)((sbyte)1 << subrange.Begin);
                            else
                                sValue &= (sbyte)(~((sbyte)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (sbyte)((sbyte)1 << index);
                                else
                                    sValue &= (sbyte)(~((sbyte)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int16)
                    {
                        if (subrange.Begin > 15 || subrange.End > 15)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt16(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (short)((short)1 << subrange.Begin);
                            else
                                sValue &= (short)(~((short)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (short)((short)1 << index);
                                else
                                    sValue &= (short)(~((short)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt16)
                    {
                        if (subrange.Begin > 15 || subrange.End > 15)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt16(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (ushort)((ushort)1 << subrange.Begin);
                            else
                                sValue &= (ushort)(~((ushort)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (ushort)((ushort)1 << index);
                                else
                                    sValue &= (ushort)(~((ushort)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int32)
                    {
                        if (subrange.Begin > 31 || subrange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt32(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= 1 << subrange.Begin;
                            else
                                sValue &= ~(1 << subrange.Begin);
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= 1 << index;
                                else
                                    sValue &= ~(1 << index);
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt32)
                    {
                        if (subrange.Begin > 31 || subrange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt32(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (uint)((uint)1 << subrange.Begin);
                            else
                                sValue &= (uint)(~((uint)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (uint)((uint)1 << index);
                                else
                                    sValue &= (uint)(~((uint)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Int64 || dstTypeInfo.BuiltInType == BuiltInType.Integer)
                    {
                        if (subrange.Begin > 63 || subrange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToInt64(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << subrange.Begin);
                            else
                                sValue &= (long)(~((long)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.UInt64 || dstTypeInfo.BuiltInType == BuiltInType.UInteger)
                    {
                        if (subrange.Begin > 63 || subrange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = Convert.ToUInt64(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (ulong)((ulong)1 << subrange.Begin);
                            else
                                sValue &= (ulong)(~((ulong)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (ulong)((ulong)1 << index);
                                else
                                    sValue &= (ulong)(~((ulong)1 << index));
                            }
                        }

                        source = sValue;
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Float)
                    {
                        if (subrange.Begin > 31 || subrange.End > 31)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = (long)Convert.ToSingle(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << subrange.Begin);
                            else
                                sValue &= (long)(~((long)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        source = Opc.Ua.TypeInfo.Cast(sValue, dstTypeInfo.BuiltInType);
                    }
                    else if (dstTypeInfo.BuiltInType == BuiltInType.Double || dstTypeInfo.BuiltInType == BuiltInType.Number)
                    {
                        if (subrange.Begin > 63 || subrange.End > 63)
                            return StatusCodes.BadIndexRangeInvalid;

                        bool bValue = Convert.ToBoolean(value);
                        var sValue = (long)Convert.ToDouble(element);
                        if (subrange.Count == 1)
                        {
                            if (bValue)
                                sValue |= (long)((long)1 << subrange.Begin);
                            else
                                sValue &= (long)(~((long)1 << subrange.Begin));
                        }
                        else
                        {
                            for (int index = subrange.Begin; index <= subrange.End; ++index)
                            {
                                if (bValue)
                                    sValue |= (long)((long)1 << index);
                                else
                                    sValue &= (long)(~((long)1 << index));
                            }
                        }

                        source = Opc.Ua.TypeInfo.Cast(sValue, dstTypeInfo.BuiltInType);
                    }

                    if (source != null)
                    {
                        var array = Opc.Ua.TypeInfo.CreateArray(dstTypeInfo.BuiltInType, 1);
                        array.SetValue(source, 0);
                        value = array;
                    }

                    var result = indexRange.SubRanges[0].UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    value = target;
                }
                else
                {
                    var result = indexRange.UpdateRange(ref target, value);

                    if (ServiceResult.IsBad(result))
                    {
                        return result;
                    }

                    value = target;
                }
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // By Maurizio Zaniboni - Progea Srl.
            // Inserted this source lines for passing the CTT unit test : Attribute Services->Attribute Write Values->Err-008.js.
            if (variable != null)
            {
                TypeInfo typeInfo = TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable);

                if (typeInfo == null || typeInfo == TypeInfo.Unknown)
                {
                    return new ServiceResult(StatusCodes.BadTypeMismatch);
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // By Maurizio Zaniboni - Progea Srl.
            // Moved here for passing the CTT unit test : Data Access->Data Access AnalogItemType->020.js.
            //value normalization must occurr...
            //to raw
            object scaledValue = value;
            var candidate = node as AnalogItemState;
            if (candidate != null && candidate.InstrumentRange != null &&
                candidate.InstrumentRange.Value != null)
            {
                if (candidate.DataType.IdType == IdType.Numeric)
                {
                    uint dimension = 0;
                    if (candidate.ValueRank != ValueRanks.Scalar && candidate.ArrayDimensions != null)
                        dimension = candidate.ArrayDimensions[0];
                    uint nType = (uint)candidate.DataType.Identifier;
                    if (nType != Opc.Ua.DataTypes.String && nType != Opc.Ua.DataTypes.Boolean)
                        scaledValue = ScaleValue(value, nType, dimension, candidate.EURange.Value, candidate.InstrumentRange.Value, true);

                    if (scaledValue == null)
                        return StatusCodes.BadOutOfRange;
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !NET_STANDARD
            if (server.IsRedundancyEnabled)
            {
                if (redundancyPrivateNodeIds.Contains(node.NodeId))
                    return new ServiceResult(StatusCodes.Good);

                if (variable == null || !Utils.IsEqual(variable.Value, value) || 
                    (timestamp != DateTime.MinValue && variable.Timestamp != timestamp) || 
                    variable.StatusCode != statusCode)
                {
                    if (timestamp == DateTime.MinValue)
                        timestamp = DateTime.UtcNow;

                    var collection = new WrappedDataValueCollection();
                    var wrappedValue = new WrappedDataValue(new Variant(scaledValue), statusCode, timestamp);
                    collection.Add(wrappedValue);
                    if (!server.ActiveServerManager.IsActiveServer)
                    {
                        return server.ActiveServerManager.SendWritingData(new ChangedTags() { NodeId = node.NodeId.ToString(), DataValues = collection });
                    }
                    else
                    {
                        var changedTags = new ChangedTags() { NodeId = node.NodeId.ToString(), DataValues = collection };
                        if (mapNodeIdToTagLogEntities.ContainsKey(node.NodeId) &&
                            mapNodeIdToTagLogEntities[node.NodeId].enableStatistics)
                        {
                            var entry = mapNodeIdToTagLogEntities[node.NodeId];
                            entry.UpdateStatistics(new DataValue(new Variant(value), statusCode, timestamp));
                            changedTags.Statistics = new StatisticsData()
                            {
                                Min = entry.min,
                                Max = entry.max,
                                TotAverage = entry.totAverage,
                                CountUpdates = entry.countUpdates,
                                TotalTimeOn = entry.totalTimeOn,
                                LastTotalTimeOn = entry.lastTotalTimeOn
                            };
                        }

                        try
                        {
                            server.ActiveServerManager.SendChangedTags(changedTags);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
#endif

            uint serviceResult = StatusCodes.Good;
            var commDriver = GetCommunicationDriversFromNodeStateDictionary(node);
            if (commDriver != null && commDriver.Count > 0)
            {
                if (variable != null && (StatusCode.IsBad(variable.StatusCode) || StatusCode.IsUncertain(variable.StatusCode)))
                    statusCode = StatusCodes.UncertainSubstituteValue;

                foreach (var driver in commDriver)
                {
                    uint ret = driver.OnWriteTag(new TagDefinition() { NodeId = node.NodeId }, ref scaledValue, ref statusCode, ref timestamp);
                    if (StatusCode.IsNotGood(ret))
                        serviceResult = ret;
                }
            }

            return new ServiceResult(serviceResult);
        }

        static bool IsConvertibleToDouble(DataValue value)
        {
            if (value.Value == null)
                return false;

            return IsConvertibleToDouble(value.WrappedValue.TypeInfo.BuiltInType);
        }

        static bool IsConvertibleToDouble(NodeState nodestate)
        {
            BuiltInType builtinType = BuiltInType.Null;

            var variable = nodestate as BaseVariableState;
            if (variable != null && variable.ValueRank == ValueRanks.Scalar)
                builtinType = TypeInfo.GetBuiltInType(variable.DataType);

            return IsConvertibleToDouble(builtinType);
        }

        static bool IsConvertibleToDouble(BuiltInType builtInType)
        {
            return TypeInfo.IsNumericType(builtInType) || builtInType == BuiltInType.Boolean;
        }

        static double GetDoubleVal(object val, uint datatype)
        {
            return Convert.ToDouble(val);
        }

        static object GetObjectVal(double conv, uint datatype)
        {
            try
            {
                switch (datatype)
                {
                    case (uint)Opc.Ua.DataTypes.SByte:
                        return Convert.ToSByte(conv);
                    case (uint)Opc.Ua.DataTypes.Byte:
                        return Convert.ToByte(conv);
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        return Convert.ToUInt16(conv);
                    case (uint)Opc.Ua.DataTypes.Double:
                        return conv;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        return Convert.ToInt16(conv);
                    case (uint)Opc.Ua.DataTypes.Int32:
                        return Convert.ToInt32(conv);
                    case (uint)Opc.Ua.DataTypes.Int64:
                        return Convert.ToInt64(conv);
                    case (uint)Opc.Ua.DataTypes.Float:
                        return Convert.ToSingle(conv);
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        return Convert.ToUInt32(conv);
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        return Convert.ToUInt64(conv);
                }
            }
            catch (OverflowException)
            {
                return null;
            }

            return null;
        }

        static object ScaleValue(object val, uint datatype, uint dimension, Range EUrange, Range InstRange, bool toraw)
        {
            // By Maurizio Zaniboni - Progrea Srl.
            // Used "Utils.Clone" for passing the CTT unit test : Data Access->Data Access PercentDeadBand->009.js
            object clone = Utils.Clone(val);
            switch (datatype)
            {
                case (uint)Opc.Ua.DataTypes.String:
                case (uint)Opc.Ua.DataTypes.Boolean:
                    break;
                case (uint)Opc.Ua.DataTypes.SByte:
                case (uint)Opc.Ua.DataTypes.Byte:
                case (uint)Opc.Ua.DataTypes.UInt16:
                case (uint)Opc.Ua.DataTypes.Double:
                case (uint)Opc.Ua.DataTypes.Int16:
                case (uint)Opc.Ua.DataTypes.Int32:
                case (uint)Opc.Ua.DataTypes.Int64:
                case (uint)Opc.Ua.DataTypes.Float:
                case (uint)Opc.Ua.DataTypes.UInt32:
                case (uint)Opc.Ua.DataTypes.UInt64:
                    if (dimension == 0)
                    {
                        var dval = GetDoubleVal(clone, datatype);
                        if (toraw)
                        {
                            if (dval >= EUrange.Low && dval <= EUrange.High)
                            {
                                double conv = ((InstRange.High - InstRange.Low) * (dval - EUrange.Low) / (EUrange.High - EUrange.Low)) + InstRange.Low;
                                return GetObjectVal(conv, datatype);
                            }
                            else
                                return null;
                        }
                        else
                        {
                            if (dval >= InstRange.Low && dval <= InstRange.High)
                            {
                                double conv = ((EUrange.High - EUrange.Low) * (dval - InstRange.Low) / (InstRange.High - InstRange.Low)) + EUrange.Low;
                                return GetObjectVal(conv, datatype);
                            }
                            else
                                return null;
                        }
                    }
                    else
                    {
                        var aVal = clone as Array;
                        if (aVal != null)
                        {
                            for (int i = 0; i < aVal.Length; i++)
                            {
                                var dval = GetDoubleVal(aVal.GetValue(i), datatype);
                                if (toraw)
                                {
                                    if (dval >= EUrange.Low && dval <= EUrange.High)
                                    {
                                        double conv = (((InstRange.High - InstRange.Low) / (EUrange.High - EUrange.Low)) * (dval - EUrange.Low)) + InstRange.Low;
                                        aVal.SetValue(GetObjectVal(conv, datatype), i);
                                    }
                                    else
                                        aVal.SetValue(null, i);
                                }
                                else
                                {
                                    if (dval >= InstRange.Low && dval <= InstRange.High)
                                    {
                                        double conv = (((EUrange.High - EUrange.Low) / (InstRange.High - InstRange.Low)) * (dval - InstRange.Low)) + EUrange.Low;
                                        aVal.SetValue(GetObjectVal(conv, datatype), i);
                                    }
                                    else
                                        aVal.SetValue(null, i);
                                }
                            }
                        }
                        return aVal;
                    }
                default:
                    break;
            }

            return clone;
        }

        static String ResolveValue(NodeState node, DataValue datavalue, bool checkIsArray = false)
        {
            try
            {
                if (node is MultiStateDiscreteState)
                {
                    var variable = node as MultiStateDiscreteState;
                    if (variable.EnumStrings != null)
                    {
                        if (checkIsArray && datavalue.Value is Array)
                        {
                            var array = datavalue.Value as Array;
                            var values = Array.CreateInstance(typeof(String), array.Length);
                            for (int ii = 0; ii < array.Length; ii++)
                            {
                                uint dvalue = Convert.ToUInt32(array.GetValue(ii));
                                values.SetValue(variable.EnumStrings.Value[dvalue].Key, ii);
                            }
                            return new Variant(values).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            uint dvalue = Convert.ToUInt32(datavalue.Value);
                            if (variable.EnumStrings != null)
                                return variable.EnumStrings.Value[dvalue].Key;
                        }
                    }
                }
                else if (node is TwoStateDiscreteState)
                {
                    var variable = node as TwoStateDiscreteState;
                    if (variable.TrueState != null)
                    {
                        if (checkIsArray && datavalue.Value is Array)
                        {
                            var array = datavalue.Value as Array;
                            var values = Array.CreateInstance(typeof(String), array.Length);
                            for (int ii = 0; ii < array.Length; ii++)
                            {
                                bool bvalue = Convert.ToBoolean(array.GetValue(ii));
                                values.SetValue(bvalue ? variable.TrueState.Value.Key : variable.FalseState.Value.Key, ii);
                            }
                            return new Variant(values).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            bool bvalue = Convert.ToBoolean(datavalue.Value);
                            return bvalue ? variable.TrueState.Value.Key : variable.FalseState.Value.Key;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return String.Format("{0}", datavalue.WrappedValue);
        }

        public void OnNodeStateChangedTagLogger(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                var value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                var error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var entry = mapNodeIdToTagLogEntities[node.NodeId];
                entry.Value = value;

                if (
#if !NET_STANDARD
                    !server.IsRedundancyEnabled &&
#endif
                    entry.enableStatistics && ServiceResult.IsGood(error))
                    entry.UpdateStatistics(value);

                tagLogger.AddLogEntity(entry); 
            }
        }

        #region Data Logger StateChanged Event Handler
        public void OnNodeEnableRecordingChangedDataLogger(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                if (node is BaseVariableState) 
                {
                    DataValue value = new DataValue
                    {
                        Value = null,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.MinValue,
                        StatusCode = StatusCodes.Good
                    };

                    ServiceResult error = node.ReadAttribute(
                            context,
                            Attributes.Value,
                            NumericRange.Empty,
                            null,
                            value);

                    //if (ServiceResult.IsBad(error))
                    //{
                    //    value = null;
                    //}

                    // Retreive the data logger settings and the column entity by node state.
                    var listdlsettings = (from c in mapNodeIdToDataLoggerSettings[node.NodeId].AsParallel()
                                          where c.EnableRecordingTag != null && c.EnableRecordingTag.ResolveNodeId(NamespaceIndex) == node.NodeId
                                          select c).ToList();
                    Parallel.ForEach(listdlsettings, dlsettings => 
                    {
                        dataLogger.EnableDataLoggerRecording(dlsettings.Name, Convert.ToDouble(value.Value) != 0.0);
                    });
                }
            }
        }

        public void OnNodeRecordingChangedDataLogger(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                if (node is BaseVariableState) 
                {
                    DataValue value = new DataValue
                    {
                        Value = null,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.MinValue,
                        StatusCode = StatusCodes.Good
                    };

                    ServiceResult error = node.ReadAttribute(
                            context,
                            Attributes.Value,
                            NumericRange.Empty,
                            null,
                            value);

                    //if (ServiceResult.IsBad(error))
                    //{
                    //    value = null;
                    //}

                    if (Convert.ToDouble(value.Value) != 0.0)
                    {
#if !NET_STANDARD
                        if (!server.IsRedundancyEnabled || server.ActiveServerManager.IsActiveServer)
#endif
                        {
                            // Retreive the data logger settings and the column entity by node state.
                            var listdlsettings = (from c in mapNodeIdToDataLoggerSettings[node.NodeId].AsParallel()
                                                  where c.RecordingTag != null && c.RecordingTag.ResolveNodeId(NamespaceIndex) == node.NodeId
                                                  select c).ToList();
                            Parallel.ForEach(listdlsettings, dlsettings => 
                            {
                                var dlentity = mapDataLoggerNameToDataLoggerEntity[dlsettings.Name].CreateSnapshot();

                                dlentity.recordingTime = DateTime.UtcNow;
                                dlentity.recordingType = DataLoggerModel.DataLoggerRecordingType.OnCommand;
                                dlentity.userName = GetUserName(context);
                                dataLogger.AddDataLoggerEntry(dlentity);
                            });
                        }

                        //ThreadPool.QueueUserWorkItem((o) =>
                        {
                            var variable = node as BaseVariableState;
                            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                            DataValue writevalue = new DataValue
                            {
                                Value = TypeInfo.GetDefaultValue(builtinType),
                                ServerTimestamp = DateTime.MinValue,
                                SourceTimestamp = DateTime.MinValue,
                                StatusCode = StatusCodes.Good
                            };
                            var result = node.WriteAttribute(
                                context,
                                Attributes.Value,
                                NumericRange.Empty,
                                writevalue);
                            if (ServiceResult.IsGood(result))
                            {
                                node.ClearChangeMasks(SystemContext, true);
                            }
                        }//);
                    }
                }
            }
        }

        public void OnNodeResettingChangedDataLogger(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                if (node is BaseVariableState)
                {
                    DataValue value = new DataValue
                    {
                        Value = null,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.MinValue,
                        StatusCode = StatusCodes.Good
                    };

                    ServiceResult error = node.ReadAttribute(
                            context,
                            Attributes.Value,
                            NumericRange.Empty,
                            null,
                            value);

                    //if (ServiceResult.IsBad(error))
                    //{
                    //    value = null;
                    //}

                    if (Convert.ToDouble(value.Value) != 0.0)
                    {
#if !NET_STANDARD
                        if (!server.IsRedundancyEnabled || server.ActiveServerManager.IsActiveServer)
#endif
                        {
                            // Retreive the data logger settings and the column entity by node state.
                            var listdlsettings = (from c in mapNodeIdToDataLoggerSettings[node.NodeId].AsParallel()
                                                  where c.ResettingTag != null && c.ResettingTag.ResolveNodeId(NamespaceIndex) == node.NodeId
                                                  select c).ToList();
                            Parallel.ForEach(listdlsettings, dlsettings => 
                            {
                                dataLogger.ResetDataLoggerEntries(dlsettings.Name);
                            });
                        }

                        //ThreadPool.QueueUserWorkItem((o) =>
                        {
                            var variable = node as BaseVariableState;
                            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                            DataValue writevalue = new DataValue
                            {
                                Value = TypeInfo.GetDefaultValue(builtinType),
                                ServerTimestamp = DateTime.MinValue,
                                SourceTimestamp = DateTime.MinValue,
                                StatusCode = StatusCodes.Good
                            };
                            var result = node.WriteAttribute(
                                context,
                                Attributes.Value,
                                NumericRange.Empty,
                                writevalue);
                            if (ServiceResult.IsGood(result))
                            {
                                node.ClearChangeMasks(SystemContext, true);
                            }
                        }//);
                    }
                }
            }
        }

        public void OnNodeSateChangedDataLogger(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);
                
                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                // Retreive the data logger settings and the column entity by node state.
                var listdlsettings = mapNodeIdToDataLoggerSettings[node.NodeId];
                Parallel.ForEach(listdlsettings, dlsettings => 
                {
                    List<String> columnNamesChanged = null;
                    var dlentity = mapDataLoggerNameToDataLoggerEntity[dlsettings.Name];
                    if (dlentity.listDataColumnEntities != null)
                    {
                        columnNamesChanged = new List<String>();
                        for (int ii = 0; ii < dlentity.listDataColumnEntities.Count; ii++)
                        {
                            // Update values to column entity.
                            var colentity = dlentity.listDataColumnEntities[ii];
                            try
                            {
                                if (colentity.columnValue.UpdateValue(node.NodeId, value) && !colentity.skipDataChange)
                                    columnNamesChanged.Add(colentity.columnName);
                                if (colentity.userColumnEnabled)
                                    colentity.userName = GetUserName(context);
                                if (colentity.stringValueColumnEnabled && colentity.columnValue.Exists(node.NodeId))
                                    colentity.stringValue = ResolveValue(node, value, checkIsArray: true);
                            }
                            catch (DataLoggerManager.ExpressionValueConverterException ex)
                            {
                                string msg = String.Format(Properties.Resources.DataLoggerColumnExpressionError, ex.InnerException.Message, dlentity.dataLoggerName, colentity.columnName);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, msg, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                continue;
                            }
                        }
                    }
                    
                    dataLogger.UpdateDataLoggerEntry(dlentity, columnNamesChanged.ToArray());
                });
            }
        }
        #endregion

        public void OnNodeStateChangedAlarm(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsStarted && server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}
                if (!mapNodeIdToAlarmStatus.ContainsKey(node.NodeId))
                    return;

                var listSettings = mapNodeIdToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (mapAlarmStatusToAliasPosition.ContainsKey(settings))
                    {
                        var alias = mapAlarmStatusToAliasPosition[settings];
                        foreach (var nodeId in alias.Keys)
                        {
                            if (mapNodeIdToNodeState.ContainsKey(nodeId))
                            { 
                                var aliasNode = mapNodeIdToNodeState[nodeId];
                                DataValue aliasValue = new DataValue
                                {
                                    Value = null,
                                    ServerTimestamp = DateTime.UtcNow,
                                    SourceTimestamp = DateTime.MinValue,
                                    StatusCode = StatusCodes.Good
                                };

                                aliasNode.ReadAttribute(
                                    context,
                                    Attributes.Value,
                                    NumericRange.Empty,
                                    null,
                                    aliasValue);

                                var aliasPosition = alias[nodeId];
                                settings.TagAliasLastValues[aliasPosition] = aliasValue;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(settings.Expression))
                    {
                        if (mapAlarmStatusToExpressionValueConverter.ContainsKey(settings))
                        {
                            var expressor = mapAlarmStatusToExpressionValueConverter[settings];
                            DataValue newValue = new DataValue(value);
                            try
                            {
                                var exprValue = expressor.Convert(value.Value, typeof(double), null, null);
                                newValue.WrappedValue = new Variant(Convert.ToDouble(exprValue));
                            }
                            catch (Exception ex)
                            {
                                string msg = String.Format(Properties.Resources.AlarmThresholdExpressionError, ex.Message, settings.Name);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, msg, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                newValue = null;
                            }

                            if (newValue != null)
                            {
                                var sourceState = mapAlarmStatusToSourceState[settings];
                                sourceState.UpdateAlarmStatus(settings, newValue);
                            }
                        }
                    }
                    else
                    {
                        var sourceState = mapAlarmStatusToSourceState[settings];
                        sourceState.UpdateAlarmStatus(settings, value);
                    }
                });
            }
        }

        public void OnNodeStateChangedAlarmEnabled(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapEnableStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    var sourceState = mapAlarmStatusToSourceState[settings];
                    sourceState.EnableAlarmStatus(settings, value);

                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedLowActivationValue(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapActivationLowStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.ActivationLowValue = Convert.ToDouble(value.Value);
                    settings.ActivationLowValueQuality = value.StatusCode.Code;

                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedActivationValue(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapActivationStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.ActivationValue = Convert.ToDouble(value.Value);
                    settings.ActivationValueQuality = value.StatusCode.Code;

                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedHighHighLimit(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapHighHighStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.Limits[0] = Convert.ToDouble(value.Value);
                    settings.LimitsQuality[0] = value.StatusCode.Code;

                        // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedHighLimit(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapHighStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.Limits[1] = Convert.ToDouble(value.Value);
                    settings.LimitsQuality[1] = value.StatusCode.Code;


                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedLowLimit(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapLowStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.Limits[2] = Convert.ToDouble(value.Value);
                    settings.LimitsQuality[2] = value.StatusCode.Code;

                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnNodeStateChangedLowLowLimit(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer)
                return;
#endif

            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var listSettings = mapLowLowStateToAlarmStatus[node.NodeId];
                Parallel.ForEach(listSettings, settings =>
                {
                    if (IsConvertibleToDouble(value))
                        settings.Limits[3] = Convert.ToDouble(value.Value);
                    settings.LimitsQuality[3] = value.StatusCode.Code;

                    // Force the update of all node states related to alarm status who has been enabled
                    if ((settings.state & AlarmState.Enabled) != 0)
                    {
                        var listNodeId = (from c in mapNodeIdToAlarmStatus where c.Value.Contains(settings) select c.Key).ToList();
                        AssignNodeToUpdateList(listNodeId);
                    }
                });
            }
        }

        public void OnSourceStateChangedAlarm(object sender, UpdatedStatusEventArgs e)
        {
            if (ExitMode
#if !NET_STANDARD
                || server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer
#endif
                )
                return;

            //UFUAEventLogEntity logEntry;
            //lock (Lock)
            //{
            //    if (!mapNodeIdToEventLogEntities.ContainsKey(e.NodeId))
            //        return;

            //    InitEventLogger();
            //    logEntry = mapNodeIdToEventLogEntities[e.NodeId];
            //}

            InitEventLogger();

            if (eventLogger != null)
            {
                UFUAEventLogEntity logEntry = new UFUAEventLogEntity()
                {
                    EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                    MaxAge = server.UFUAConfiguration.EventMaxAge.Value
                };

                logEntry.EventId = e.Condition.EventId != null ? new Guid(e.Condition.EventId.Value) : Guid.NewGuid();
                logEntry.EventType = e.Condition.EventType != null ? e.Condition.EventType.Value : null;
                logEntry.SourceNode = e.Condition.SourceNode != null ? e.Condition.SourceNode.Value.ToString() : null;
                logEntry.SourceName = e.Condition.SourceName != null ? e.Condition.SourceName.Value : String.Empty;
                logEntry.DurationTime = e.Duration;
                if (e.UseTimeStamp && e.Condition.ActiveState.TransitionTime != null)
                    logEntry.EventDateTime = e.Condition.ActiveState.TransitionTime.Value;
                else
                    logEntry.EventDateTime = e.Condition.Time != null ? e.Condition.Time.Value : DateTime.UtcNow;
                logEntry.EventMessage = e.Condition.Message != null ? e.Condition.Message.Value.Text : String.Empty;
                if (e.Condition.Message.Value.TranslationInfo != null &&
                    e.Condition.Message.Value.TranslationInfo.Args != null &&
                    e.Condition.Message.Value.TranslationInfo.Args.Length > 0)
                {
                    var values = new string[e.Condition.Message.Value.TranslationInfo.Args.Length];
                    for (int ii = 0; ii < e.Condition.Message.Value.TranslationInfo.Args.Length; ii++)
                        values[ii] = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", e.Condition.Message.Value.TranslationInfo.Args[ii]);
                    logEntry.EventDetails = String.Format("{0}", new Opc.Ua.Variant(values));
                }
                else
                    logEntry.EventDetails = e.Condition.ConditionName != null ? e.Condition.ConditionName.Value : String.Empty;
                logEntry.EventComment = e.EventComment;
                logEntry.EventState = e.Condition.EnabledState != null &&
                                                e.Condition.EnabledState.EffectiveDisplayName != null ?
                                                e.Condition.EnabledState.EffectiveDisplayName.Value.Text : String.Empty;
                //logEntry.EventUniqueId      = e.NodeId.ToString();
                logEntry.EventOccurence = e.Occurence;
                logEntry.EventSequence = e.Sequence;
                logEntry.Severity = e.Condition.Severity != null ? e.Condition.Severity.Value : ushort.MinValue;
                logEntry.UserName = e.EventUserName != null && e.EventUserName != "Anonymous" ? e.EventUserName : null;

                //if (logEntry.Severity == 0)
                //    logEntry.EventType = ObjectTypeIds.SystemEventType;

                eventLogger.AddLogEntity(logEntry);
            }
        }

        private void OnSourceAlarmsStatusChanged(object sender, ChangedAlarmsArgs e)
        {
#if !NET_STANDARD
            if (server.IsRedundancyEnabled)
            {
                if (server.ActiveServerManager.IsActiveServer)
                {
                    try
                    {
                        server.ActiveServerManager.SendChangedAlarms(new ChangedAlarms() { NodeId = e.nodeId.ToString(), AlarmsStatus = new WrappedAlarmStatusCollection(e.alarmsStatus) });
                    }
                    catch
                    { }
                }
                /* Inactive server don't need to update alarms because the ack, rst, comment, etc. methods are executed only on active server.
                else
                {
                    ServiceResult retserver = server.ActiveServerManager.SendUpdatingAlarms(new ChangedAlarms() { NodeId = e.nodeId, AlarmsStatus = e.alarmsStatus.ToList() });
                    //if (!ServiceResult.IsGood(retserver))
                    //{
                    //    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    //                        String.Format(Properties.Resources.ErrorRedundancyServer, retserver),
                    //                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                    //}
                }
                */
            }
#endif
        }

        long numBeep = 0;
        private void OnAlarmStateChanged(object sender, AlarmStateChangedArgs e)
        {
            if (e.oldState == e.newState)
                return;

            long numShelved = 0;
            long numEnabled = 0;
            long numActiveOn = 0;
            long numActiveOff = 0;
            long numActiveOnOff = 0;
            long numNotAck = 0;
            long numShelvedWithSoundOn = 0;

            if ((e.oldState & AlarmState.Shelved) == 0 && (e.newState & AlarmState.Shelved) != 0)
                numShelved = 1;
            else if ((e.oldState & AlarmState.Shelved) != 0 && (e.newState & AlarmState.Shelved) == 0)
                numShelved = -1;

            if ((e.oldState & AlarmState.Enabled) == 0 && (e.newState & AlarmState.Enabled) != 0)
                numEnabled = 1;
            else if ((e.oldState & AlarmState.Enabled) != 0 && (e.newState & AlarmState.Enabled) == 0)
                numEnabled = -1;

            if ((e.oldState & AlarmState.Shelved) != 0 && (e.newState & AlarmState.Shelved) != 0)
                numShelvedWithSoundOn = 0;
            else if ((e.newState & AlarmState.Enabled) != 0 && (e.newState & AlarmState.Confirmed) != 0)
                numShelvedWithSoundOn = numShelved;

            if (numEnabled != 0)
            {
                if ((e.newState & AlarmState.Deleted) == 0)
                {
                    if ((e.newState & AlarmState.Active) != 0)
                        numActiveOn = numActiveOnOff = numEnabled;
                    if ((e.newState & AlarmState.Active) == 0)
                        numActiveOff = numActiveOnOff = numEnabled;
                    if ((e.newState & AlarmState.Acknowledged) == 0)
                        numNotAck = numEnabled;
                }
            }
            else if (e.oldState == AlarmState.Undefined)
            {
                if ((e.newState & AlarmState.Enabled) != 0 && (e.newState & AlarmState.Deleted) == 0)
                {
                    if ((e.newState & AlarmState.Active) != 0)
                        numActiveOn = numActiveOnOff = 1;
                    if ((e.newState & AlarmState.Active) == 0)
                        numActiveOff = numActiveOnOff = 1;
                    if ((e.newState & AlarmState.Acknowledged) == 0)
                        numNotAck = 1;
                }
            }
            else if ((e.newState & AlarmState.Deleted) == 0)
            {
                if ((e.oldState & AlarmState.Active) == 0 && (e.newState & AlarmState.Active) != 0)
                {
                    numActiveOn = 1;

                    if ((e.oldState & AlarmState.Deleted) == 0)
                        numActiveOff = -1;
                    else
                        numActiveOnOff = 1;
                }
                else if ((e.oldState & AlarmState.Active) != 0 && (e.newState & AlarmState.Active) == 0)
                {
                    numActiveOff = 1;

                    if ((e.oldState & AlarmState.Deleted) == 0)
                        numActiveOn = -1;
                    else
                        numActiveOnOff = 1;
                }

                if ((e.oldState & AlarmState.Acknowledged) != 0 && (e.newState & AlarmState.Acknowledged) == 0 && (e.newState & AlarmState.Shelved) == 0)
                    numNotAck = 1;
                else if ((e.oldState & AlarmState.Acknowledged) == 0 && (e.newState & AlarmState.Acknowledged) != 0 && (e.newState & AlarmState.Shelved) == 0)
                    numNotAck = -1;
            }
            else if ((e.oldState & AlarmState.Deleted) == 0 && (e.newState & AlarmState.Deleted) != 0)
            {
                if ((e.oldState & AlarmState.Active) != 0)
                    numActiveOn = numActiveOnOff = -1;
                if ((e.oldState & AlarmState.Active) == 0)
                    numActiveOff = numActiveOnOff = -1;
                if ((e.oldState & AlarmState.Acknowledged) == 0)
                    numNotAck = -1;
            }

            if (e.canPlay)
            {
                long prevBeep = numBeep;

                if (e.isMessage)
                {
                    numBeep += numActiveOn;
                }
                else if (e.canAck)
                {
                    numBeep += numNotAck;
                }
                else if (e.canReset)
                {
                    numBeep += numActiveOnOff;
                }
                else if (!e.canAck && !e.canReset)
                {
                    numBeep += numActiveOn;
                }

                numBeep -= numShelvedWithSoundOn;

                isBeeping = numBeep > 0;
            }

            if (numShelved != 0 || numEnabled != 0 || numActiveOn != 0 || 
                numActiveOff != 0 || numActiveOnOff != 0 || numNotAck != 0)
            {
                var source = (SourceState)sender;
                bool bPendingChanges = true;
                lock (lockSystemCounters)
                {
                    if (e.isMessage)
                    {
                        if (mapMessageCounters == null)
                            mapMessageCounters = new Dictionary<NodeId, UpdateCounters>();
                        if (!mapMessageCounters.ContainsKey(source.NodeId))
                            mapMessageCounters.Add(source.NodeId, 
                                new UpdateCounters(source, numShelved, numEnabled, numActiveOn, numActiveOff, numActiveOnOff, numNotAck));
                        else
                        {
                            mapMessageCounters[source.NodeId].Add(numShelved, numEnabled, numActiveOn, numActiveOff, numActiveOnOff, numNotAck);
                            if (mapMessageCounters[source.NodeId].IsEmpty)
                            {
                                bPendingChanges = false;
                                mapMessageCounters.Remove(source.NodeId);
                            }
                        }
                    }
                    else
                    {
                        if (mapAlarmCounters == null)
                            mapAlarmCounters = new Dictionary<NodeId, UpdateCounters>();
                        if (!mapAlarmCounters.ContainsKey(source.NodeId))
                            mapAlarmCounters.Add(source.NodeId, 
                                new UpdateCounters(source, numShelved, numEnabled, numActiveOn, numActiveOff, numActiveOnOff, numNotAck));
                        else
                        {
                            mapAlarmCounters[source.NodeId].Add(numShelved, numEnabled, numActiveOn, numActiveOff, numActiveOnOff, numNotAck);
                            if (mapAlarmCounters[source.NodeId].IsEmpty)
                            {
                                bPendingChanges = false;
                                mapAlarmCounters.Remove(source.NodeId);
                            }
                        }
                    }
                }

                if (bPendingChanges)
                    ElaborateSystemCounters();
            }
        }

        void ElaborateSystemCounters()
        {
            lock (lockSystemCounters)
            {
                if (!bSystemCountersUpdating && systemCountersUpdater == null)
                {
                    systemCountersUpdater = new Timer((o) =>
                    {
                        var pendingAlarms = new List<UpdateCounters>();
                        var pendingMessages = new List<UpdateCounters>();
                        lock (lockSystemCounters)
                        {
                            if (mapAlarmCounters != null)
                            {
                                pendingAlarms.AddRange(mapAlarmCounters.Values);
                                mapAlarmCounters.Clear();
                            }
                            if (mapMessageCounters != null)
                            {
                                pendingMessages.AddRange(mapMessageCounters.Values);
                                mapMessageCounters.Clear();
                            }

                            bSystemCountersUpdating = true;
                            if (systemCountersUpdater != null)
                            {
                                systemCountersUpdater.Dispose();
                                systemCountersUpdater = null;
                            }
                        }

                        using (new Utilities.ChangeThreadPriority(ThreadPriority.Lowest))
                        {
                            if (systemTags != null)
                            {
                                var isBuzzing = systemTags.AlarmsSoundState.Value && isBeeping;
                                if (systemTags.AlarmsSoundBuzzing.Value != isBuzzing)
                                {
                                    systemTags.AlarmsSoundBuzzing.Value = isBuzzing;
                                    systemTags.AlarmsSoundBuzzing.Timestamp = DateTime.UtcNow;
                                    AssignNodeToUpdateList(systemTags.AlarmsSoundBuzzing);
                                }
                            }

                            pendingAlarms.ForEach((counter) =>
                            {
                                if (systemTags != null)
                                {
                                    if (counter.NumEnabled != 0)
                                    {
                                        systemTags.AlarmsNumEnabled.Value += counter.NumEnabled;
                                        systemTags.AlarmsNumEnabled.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumEnabled);
                                    }
                                    if (counter.NumShelved != 0)
                                    {
                                        systemTags.AlarmsNumShelved.Value += counter.NumShelved;
                                        systemTags.AlarmsNumShelved.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumShelved);
                                    }
                                    if (counter.NumActiveOn != 0)
                                    {
                                        systemTags.AlarmsNumActiveOn.Value += counter.NumActiveOn;
                                        systemTags.AlarmsNumActiveOn.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumActiveOn);
                                    }
                                    if (counter.NumActiveOff != 0)
                                    {
                                        systemTags.AlarmsNumActiveOff.Value += counter.NumActiveOff;
                                        systemTags.AlarmsNumActiveOff.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumActiveOff);
                                    }
                                    if (counter.NumActiveOnOff != 0)
                                    {
                                        systemTags.AlarmsNumActiveOnOff.Value += counter.NumActiveOnOff;
                                        systemTags.AlarmsNumActiveOnOff.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumActiveOnOff);
                                    }
                                    if (counter.NumNotAck != 0)
                                    {
                                        systemTags.AlarmsNumNotAck.Value += counter.NumNotAck;
                                        systemTags.AlarmsNumNotAck.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.AlarmsNumNotAck);
                                    }
                                }

                                var parent = counter.Source.Parent;
                                while (parent != null && parent is Model.AreaState)
                                {
                                    var nodeid = parent.NodeId;
                                    parent = (parent as Model.AreaState).Parent;
                                    if (!mapAreaNodeIdToSettings.ContainsKey(nodeid))
                                        continue;

                                    var settings = mapAreaNodeIdToSettings[nodeid];
                                    if (counter.NumShelved != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumShelvedStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumShelvedStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumShelvedStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumShelved;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumEnabled != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumEnabledStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumEnabledStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumEnabledStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumEnabled;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumActiveOn != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumActiveOnStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumActiveOnStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumActiveOnStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumActiveOn;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumActiveOff != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumActiveOffStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumActiveOffStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumActiveOffStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumActiveOff;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumActiveOnOff != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumActiveOnOffStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumActiveOnOffStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumActiveOnOffStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumActiveOnOff;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumNotAck != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.AlarmsNumNotAckStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.AlarmsNumNotAckStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.AlarmsNumNotAckStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumNotAck;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                }
                            });

                            pendingMessages.ForEach((counter) =>
                            {
                                if (systemTags != null)
                                {
                                    if (counter.NumShelved != 0)
                                    {
                                        systemTags.MessagesNumShelved.Value += counter.NumShelved;
                                        systemTags.MessagesNumShelved.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.MessagesNumShelved);
                                    }
                                    if (counter.NumEnabled != 0)
                                    {
                                        systemTags.MessagesNumEnabled.Value += counter.NumEnabled;
                                        systemTags.MessagesNumEnabled.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.MessagesNumEnabled);
                                    }
                                    if (counter.NumActiveOn != 0)
                                    {
                                        systemTags.MessagesNumActiveOn.Value += counter.NumActiveOn;
                                        systemTags.MessagesNumActiveOn.Timestamp = DateTime.UtcNow;
                                        AssignNodeToUpdateList(systemTags.MessagesNumActiveOn);
                                    }
                                }

                                var parent = counter.Source.Parent;
                                while (parent != null && parent is Model.AreaState)
                                {
                                    var nodeid = parent.NodeId;
                                    parent = (parent as Model.AreaState).Parent;
                                    if (!mapAreaNodeIdToSettings.ContainsKey(nodeid))
                                        continue;

                                    var settings = mapAreaNodeIdToSettings[nodeid];
                                    if (counter.NumShelved != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.MessagesNumShelvedStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.MessagesNumShelvedStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.MessagesNumShelvedStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumShelved;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumEnabled != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.MessagesNumEnabledStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.MessagesNumEnabledStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.MessagesNumEnabledStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumEnabled;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    if (counter.NumActiveOn != 0)
                                    {
                                        BaseVariableState variable = null;
                                        if (!NodeId.IsNull(settings.MessagesNumActiveOnStateNodeId) &&
                                            mapNodeIdToNodeState.ContainsKey(settings.MessagesNumActiveOnStateNodeId))
                                            variable = mapNodeIdToNodeState[settings.MessagesNumActiveOnStateNodeId] as BaseVariableState;
                                        if (variable != null)
                                        {
                                            try
                                            {
                                                long currentValue = Convert.ToInt64(variable.Value);
                                                currentValue += counter.NumActiveOn;
                                                BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
                                                variable.Value = ChangeType(currentValue, builtinType);
                                                AssignNodeToUpdateList(variable);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                }
                            });
                        }

                        lock (lockSystemCounters)
                        {
                            bSystemCountersUpdating = false;
                            if ((mapAlarmCounters != null && mapAlarmCounters.Count > 0) ||
                                (mapMessageCounters != null && mapMessageCounters.Count > 0))
                            {
                                ElaborateSystemCounters();
                            }
                        }
                    }, this, TimeSpan.FromMilliseconds(Properties.Settings.Default.DelayUpdateSystemTags), TimeSpan.FromMilliseconds(-1));
                }
            }
        }

        public void OnEnabledNodeStateChangedHistorian(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var isEnabled = Convert.ToDouble(value.Value) != 0.0;
                var nodeIds = mapEnabledHistorianLogEntities[node.NodeId];
                Parallel.ForEach(nodeIds, nodeId =>
                {
                    if (isEnabled)
                        historianLogger.Resume(nodeId);
                    else
                        historianLogger.Suspend(nodeId);
                });
            }
        }

        public void OnNodeStateChangedHistorian(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue 
                { 
                    Value = null, 
                    ServerTimestamp = DateTime.UtcNow, 
                    SourceTimestamp = DateTime.MinValue, 
                    StatusCode = StatusCodes.Good 
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                if (!mapNodeIdToHistorianLogEntities.ContainsKey(node.NodeId))
                    return;

                var entry = mapNodeIdToHistorianLogEntities[node.NodeId];

                //if (!entry.OnlyGood || StatusCode.IsGood(value.StatusCode))
                entry.valueBefore = new DataValue(entry.value);
                entry.value = value;
                entry.User = GetUserName(context);
                entry.Reason = null;
                entry.resolvedValue = ResolveValue(node, value);
                entry.resolvedValueBefore = ResolveValue(node, entry.valueBefore);
                
                historianLogger.AddLogEntity(entry);
            }
        }

        public void OnNodeStateChangedAudit(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue
                {
                    Value = null,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = StatusCodes.Good
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                if (!mapNodeIdToAuditLogEntities.ContainsKey(node.NodeId) || 
                    !mapNodeStateToAccessSettings.ContainsKey(node))
                    return;

                var entry = mapNodeIdToAuditLogEntities[node.NodeId];
                var accessSettings = mapNodeStateToAccessSettings[node];

                //if (!entry.OnlyGood || StatusCode.IsGood(value.StatusCode))
                entry.valueBefore = new DataValue(entry.value);
                entry.value = value;
                entry.resolvedValue = ResolveValue(node, value);
                entry.resolvedValueBefore = ResolveValue(node, entry.valueBefore);
                
                if (context.UserIdentity == null)
                {
                    // Internal server StateChanged event.
                    entry.Reason = null;
                    entry.User = null;
                }
                else if (!accessSettings.IsCommentRequiredOnAudit)
                {
                    entry.User = GetUserName(context);
                    entry.Reason = null;
                }
                
                historianLogger.AddLogEntity(entry);
            }
        }

        public void OnAlarmsSoundStateChanged(
            ISystemContext context,
            NodeState node,
            NodeStateChangeMasks changes)
        {
            if ((changes & NodeStateChangeMasks.Value) != 0)
            {
                DataValue value = new DataValue 
                { 
                    Value = null, 
                    ServerTimestamp = DateTime.UtcNow, 
                    SourceTimestamp = DateTime.MinValue, 
                    StatusCode = StatusCodes.Good 
                };

                ServiceResult error = node.ReadAttribute(
                        context,
                        Attributes.Value,
                        NumericRange.Empty,
                        null,
                        value);

                //if (ServiceResult.IsBad(error))
                //{
                //    value = null;
                //}

                var isBuzzing = (bool)value.Value && isBeeping;
                if (systemTags.AlarmsSoundBuzzing.Value != isBuzzing)
                {
                    systemTags.AlarmsSoundBuzzing.Value = isBuzzing;
                    systemTags.AlarmsSoundBuzzing.Timestamp = DateTime.UtcNow;
                    AssignNodeToUpdateList(systemTags.AlarmsSoundBuzzing);
                }
            }
        }

        public virtual ServiceResult OnResumeDiagnostic(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.ResumeDiagnosticEventLog);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow, null, true);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            return commDriver[0].OnResumeDiagnostic();
        }

        public virtual ServiceResult OnSuspendDiagnostic(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.SuspendDiagnosticEventLog);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow, null, true);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            return commDriver[0].OnSuspendDiagnostic();
        }
        public virtual ServiceResult OnResetDiagnostic(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.ResetDiagnosticEventLog);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow, null, true);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            return commDriver[0].OnResetDiagnostic();
        }

        public virtual ServiceResult OnMethodCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.MethodCallEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            int i = 0;
            foreach(var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            uint serviceResult = StatusCodes.Good;
            foreach (var driver in commDriver)
            {
                uint ret = commDriver[0].OnMethodCall(new TagDefinition() { NodeId = method.NodeId }, inputArguments, outputArguments);
                if (StatusCode.IsNotGood(ret))
                    serviceResult = ret;
            }

            return new ServiceResult(serviceResult);
        }

        public virtual ServiceResult OnReadValues(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.ReadValuesEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            /*
            int i = 0;
            foreach (var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }
            */

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            var startingAddress = inputArguments[0].ToString();
            var datavalues = new List<object>();
            if (inputArguments[1] is Variant[])
            {
                foreach (var value in (inputArguments[1] as Variant[]))
                    datavalues.Add(value);
            }

            var result = commDriver[0].OnReadValues(startingAddress, datavalues);

            if (result == (uint)ServiceResult.Good)
            {
                outputArguments.Clear();
                foreach (var value in datavalues)
                    outputArguments.Add(value);
            }
            
            return result;
        }

        public virtual ServiceResult OnWriteValues(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.WriteValuesEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            /*
            int i = 0;
            foreach (var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }
            */

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(method);
            if (commDriver == null || commDriver.Count == 0)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            var startingAddress = inputArguments[0].ToString();
            var datavalues = new List<object>();
            if (inputArguments[1] is Variant[])
            {
                foreach (var value in (inputArguments[1] as Variant[]))
                    datavalues.Add(value);
            }

            return commDriver[0].OnWriteValues(inputArguments[0].ToString(), datavalues);
        }

        public virtual ServiceResult OnRuntimeAlarmSettingsUpdate(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = Properties.Resources.RedundancyCallSwitchOfActiveServer;
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (systemTags == null || systemTags.RuntimeAlarmSettingsUpdateMethod != method)
                return StatusCodes.BadMethodInvalid;

            return UpdateAlarmRuntimeChanges();
        }

#if !NET_STANDARD
        public virtual ServiceResult OnRedundancySwitchActiveServer(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = Properties.Resources.RedundancyCallSwitchOfActiveServer;
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (systemTags == null || systemTags.RedundancySwitchActiveServerMethod != method)
                return StatusCodes.BadMethodInvalid;

            if (!server.IsRedundancyEnabled)
                return StatusCodes.BadNotSupported;

            return server.ActiveServerManager.SwitchActiveServer();
        }
#endif

        public virtual ServiceResult OnAddEventLog(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
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

            object[] array = new Object[inputArguments.Count];
            for (i = 0; i < inputArguments.Count; ++i)
                array[i] = inputArguments[i];

            var source = array[0].ToString();
            if (string.IsNullOrWhiteSpace(source))
                return StatusCodes.BadInvalidArgument;

            var message = array[1].ToString();
            if (string.IsNullOrWhiteSpace(message))
                return StatusCodes.BadInvalidArgument;

            var details = array[2].ToString();
            //if (string.IsNullOrWhiteSpace(details))
            //    return StatusCodes.BadInvalidArgument;

            var comment = array[3].ToString();
            //if (string.IsNullOrWhiteSpace(comment))
            //    return StatusCodes.BadInvalidArgument;

            var userName = array[4].ToString();
            //if (string.IsNullOrWhiteSpace(comment))
            //    return StatusCodes.BadInvalidArgument;

            DateTime date;
            if(!DateTime.TryParse(array[5].ToString(), out date))
                return StatusCodes.BadInvalidArgument;

            EventSeverity severity;
            if (!EventSeverity.TryParse(array[6].ToString(), out severity))
                return StatusCodes.BadInvalidArgument;

            var systemEvent = new SystemEvent();
            systemEvent.evtype = ObjectTypeIds.AuditEventType;
            systemEvent.details = details ?? String.Empty;
            systemEvent.comment = comment ?? String.Empty;
            systemEvent.username = userName ?? String.Empty;
            RaiseSystemEvents(null, source, message, severity, date, systemEvent, addtosystemlog: true);

            //outputArguments.Clear();

            return StatusCodes.Good;
        }
        public virtual ServiceResult OnWriteAuditValue(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.MethodCallEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
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

            object[] array = new Object[inputArguments.Count];
            for (i = 0; i < inputArguments.Count; ++i)
                array[i] = inputArguments[i];

            var nodeId = array[0] as NodeId;
            if (!mapNodeIdToNodeState.ContainsKey(nodeId) || 
                !mapNodeStateToAccessSettings.ContainsKey(mapNodeIdToNodeState[nodeId]))
                return StatusCodes.BadNodeIdUnknown;

            var variable = mapNodeIdToNodeState[nodeId] as BaseVariableState;
            var accessSettings = mapNodeStateToAccessSettings[variable];

            if (variable == null || !accessSettings.IsAuditTraceEnabled || !accessSettings.IsCommentRequiredOnAudit)
                return StatusCodes.BadNodeIdRejected;
            
            var comment = array[2].ToString();

            if (string.IsNullOrWhiteSpace(comment))
                return StatusCodes.BadInvalidArgument;

            var userName = array[3].ToString();
            var password = array[4].ToString();

            outputArguments.Clear();

            if (server.IsUserManagerEnabled())
            {
                var tokenId = array[5].ToString();
                var token = AuditTrace.AuditTraceToken.RestoreToken(tokenId);
                if (token == null)
                    return StatusCodes.BadInvalidArgument;

                if (String.IsNullOrEmpty(userName) && token.StateId == AuditTrace.AuditState.None)
                    token.StateId = AuditTrace.AuditState.WaitingForUser;

                server.VerifyPassword(userName, password);

                token.UserName = userName;
                token.Password = password;

                int accessMask = 0;
                int accessLevel = 0;
                if (accessSettings.MinAccessLevelRequiredOnAudit > 0)
                {
                    if (!server.GetUserLevel(userName, ref accessMask, ref accessLevel))
                        return StatusCodes.BadIdentityTokenRejected;
                }

                if (token.StateId == AuditTrace.AuditState.None)
                {
                    if (accessSettings.IsPasswordRequiredOnAudit)
                    {
                        token.StateId = AuditTrace.AuditState.WaitingForPassword;
                    }
                    else if (accessSettings.MinAccessLevelRequiredOnAudit > 0 &&
                        accessSettings.MinAccessLevelRequiredOnAudit > accessLevel)
                    {
                        token.StateId = AuditTrace.AuditState.WaitingForLevel;
                    }
                }
                else if (token.StateId == AuditTrace.AuditState.WaitingForUser)
                {
                    if (accessSettings.MinAccessLevelRequiredOnAudit > 0 &&
                        accessSettings.MinAccessLevelRequiredOnAudit > accessLevel)
                    {
                        token.StateId = AuditTrace.AuditState.WaitingForLevel;
                    }
                    else
                        token.StateId = AuditTrace.AuditState.Completed;
                }
                else if (token.StateId == AuditTrace.AuditState.WaitingForPassword)
                {
                    if (token.UserName != userName || token.Password != password)
                        return StatusCodes.BadIdentityTokenRejected;

                    if (accessSettings.MinAccessLevelRequiredOnAudit > 0 &&
                        accessSettings.MinAccessLevelRequiredOnAudit > accessLevel)
                    {
                        token.StateId = AuditTrace.AuditState.WaitingForLevel;
                    }
                    else
                        token.StateId = AuditTrace.AuditState.Completed;
                }
                else if (token.StateId == AuditTrace.AuditState.WaitingForLevel)
                {
                    if (accessSettings.MinAccessLevelRequiredOnAudit > accessLevel)
                        return StatusCodes.BadIdentityTokenRejected;

                    token.StateId = AuditTrace.AuditState.Completed;
                }
                
                if (token.StateId == AuditTrace.AuditState.WaitingForPassword)
                {
                    outputArguments.Add(token.UserName);
                    outputArguments.Add(0);

                    return StatusCodes.GoodCallAgain;
                }
                else if (token.StateId == AuditTrace.AuditState.WaitingForLevel)
                {
                    outputArguments.Add(String.Empty);
                    outputArguments.Add(accessSettings.MinAccessLevelRequiredOnAudit);

                    return StatusCodes.GoodCallAgain;
                }

                AuditTrace.AuditTraceToken.RemoveToken(tokenId);
            }

            if (variable.OnWriteValue != null)
            {
                var sValue = array[1].ToString();
                uint dimension = 0;
                if (variable.ValueRank != ValueRanks.Scalar && variable.ArrayDimensions != null)
                    dimension = variable.ArrayDimensions[0];
                System.Globalization.NumberFormatInfo info = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
                var value = ChangeType(sValue, TypeInfo.GetBuiltInType(variable.DataType), dimension, info);
                var statusCode = variable.StatusCode;
                var timeStamp = variable.Timestamp;
                var result = variable.OnWriteValue(
                    context,
                    variable,
                    NumericRange.Empty,
                    null,
                    ref value,
                    ref statusCode,
                    ref timeStamp);

                if (ServiceResult.IsBad(result))
                {
                    return result;
                }

                variable.Value = value;
                variable.StatusCode = statusCode;
                variable.Timestamp = timeStamp;

                string signature = null;
                if (server.IsUserManagerEnabled() && !String.IsNullOrEmpty(userName))
                    signature = server.GetUserSignature(userName);

                if (String.IsNullOrEmpty(signature))
                {
                    if (!String.IsNullOrEmpty(userName))
                        signature = userName;
                    else if (context.UserIdentity != null)
                        signature = context.UserIdentity.DisplayName;
                }

                if (mapNodeIdToAuditLogEntities.ContainsKey(variable.NodeId))
                {
                    var entry = mapNodeIdToAuditLogEntities[variable.NodeId];
                    entry.Reason = comment;
                    entry.User = signature;
                }

                if (accessSettings.LastCommentOnAudit != comment ||
                    accessSettings.LastUserNameOnAudit != signature)
                {
                    accessSettings.LastUserNameOnAudit = signature;
                    accessSettings.LastCommentOnAudit = comment;

                    var children = new List<BaseInstanceState>();
                    variable.GetChildren(SystemContext, children);
                    var props = (from child in children.AsParallel()
                                    where child.BrowseName != null &&
                                    (child.BrowseName.Name == UFUAServerInfo.BrowserNames.LastUserNameOnAudit ||
                                    child.BrowseName.Name == UFUAServerInfo.BrowserNames.LastCommentOnAudit)
                                    select child).ToList();
                    props.ForEach(prop => prop.UpdateChangeMasks(NodeStateChangeMasks.Value));
                }

                //OnNodeStateChangedAudit(context, variable, NodeStateChangeMasks.Value);

                variable.ClearChangeMasks(context, true);

                return result;
            }

            return StatusCodes.BadNoEntryExists;
        }

        public virtual ServiceResult OnResetStatistics(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithUserName, method, GetUserName(context));
            var eventName = String.Format(Properties.Resources.MethodCallEventLog, inputArguments);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
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

            object[] array = new Object[inputArguments.Count];
            for (i = 0; i < inputArguments.Count; ++i)
                array[i] = inputArguments[i];

            var nodeId = array[0] as NodeId;
            if (!mapNodeIdToTagLogEntities.ContainsKey(nodeId))
                return StatusCodes.BadNodeIdUnknown;

            var entry = mapNodeIdToTagLogEntities[nodeId];
            if (!entry.enableStatistics)
                return StatusCodes.BadNodeIdInvalid;

            entry.ResetStatistics();

            if (tagLogger != null)
                tagLogger.AddLogEntity(entry);

#if !NET_STANDARD
            if (server.IsRedundancyEnabled &&
                server.ActiveServerManager.IsActiveServer &&
                !redundancyPrivateNodeIds.Contains(nodeId))
            {
                var collection = new WrappedDataValueCollection();
                var changedTags = new ChangedTags() { NodeId = nodeId.ToString(), DataValues = collection };
                changedTags.Statistics = new StatisticsData()
                {
                    Min = entry.min,
                    Max = entry.max,
                    TotAverage = entry.totAverage,
                    CountUpdates = entry.countUpdates,
                    TotalTimeOn = entry.totalTimeOn,
                    LastTotalTimeOn = entry.lastTotalTimeOn
                };

                try
                {
                    server.ActiveServerManager.SendChangedTags(changedTags);
                }
                catch (Exception ex)
                {

                }
            }
#endif

            //if (mapNodeIdToNodeState.ContainsKey(nodeId))
            //{
            //    var node = mapNodeIdToNodeState[nodeId];
            //    node.UpdateChangeMasks(NodeStateChangeMasks.Value);
            //    node.ClearChangeMasks(context, true);
            //}

            return StatusCodes.Good;
        }

        void AddCommunicationDriverToNodeStateDictionary(NodeState nodeState, string driverName)
        {
            if (!server.CommDrivers.ContainsKey(driverName))
                return;

            var commDriver = server.CommDrivers[driverName];
            if (!mapNodeStateToCommDriver.ContainsKey(nodeState))
                mapNodeStateToCommDriver.Add(nodeState, new List<ICommunicationDriver>());
            if (!mapNodeStateToCommDriver[nodeState].Contains(commDriver))
                mapNodeStateToCommDriver[nodeState].Add(commDriver);
        }

        IList<ICommunicationDriver> GetCommunicationDriversFromNodeStateDictionary(NodeState node)
        {
            if (mapNodeStateToCommDriver.ContainsKey(node))
                return mapNodeStateToCommDriver[node];
            else if (node is BaseInstanceState)
            {
                var instance = node as BaseInstanceState;
                if (instance.Parent != null)
                    return GetCommunicationDriversFromNodeStateDictionary(instance.Parent);
            }

            return null;
        }

        void CreateHistorian(UFUAModel.UFUAHistorianSettings UFUAHistorianSettings, BaseDataVariableState variable)
        {
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate;
            variable.Historizing = true;
            variable.AccessLevel |= Opc.Ua.AccessLevels.HistoryReadOrWrite;
            variable.UserAccessLevel |= Opc.Ua.AccessLevels.HistoryReadOrWrite;
            /* OPC IOP WorkShop : we will work on annotations later when we come back
            var annotations = new PropertyState<Annotation>(variable)
            {
                ReferenceTypeId = ReferenceTypeIds.HasProperty,
                TypeDefinitionId = VariableTypeIds.PropertyType,
                SymbolicName = BrowseNames.Annotations,
                BrowseName = BrowseNames.Annotations,
                Description = null,
                WriteMask = 0,
                UserWriteMask = 0,
                DataType = DataTypeIds.Annotation,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.HistoryReadOrWrite,
                UserAccessLevel = AccessLevels.HistoryReadOrWrite,
                MinimumSamplingInterval = MinimumSamplingIntervals.Indeterminate,
                Historizing = false
            };
            
            annotations.DisplayName = new LocalizedText(annotations.BrowseName.Name, string.Empty, annotations.BrowseName.Name);
            variable.AddChild(annotations);

            annotations.NodeId = ModelUtils.ConstructIdForComponent(annotations, NamespaceIndex);
            */

            var configuration = new HistoricalDataConfigurationState(variable);
            configuration.MaxTimeInterval = new PropertyState<double>(configuration);
            configuration.MinTimeInterval = new PropertyState<double>(configuration);

            configuration.Create(
                SystemContext,
                null,
                Opc.Ua.BrowseNames.HAConfiguration,
                null,
                true);

            configuration.SymbolicName = BrowseNames.HAConfiguration;
            configuration.ReferenceTypeId = ReferenceTypeIds.HasHistoricalConfiguration;

            configuration.MinTimeInterval.Value = UFUAHistorianSettings.MinTimeInterval.TotalMilliseconds;
            configuration.MaxTimeInterval.Value = UFUAHistorianSettings.MaxTimeInterval.TotalMilliseconds;
            configuration.Stepped.Value = UFUAHistorianSettings.Stepped;

            configuration.AggregateConfiguration.PercentDataGood.Value = UFUAHistorianSettings.PercentDataGood;
            configuration.AggregateConfiguration.PercentDataBad.Value = UFUAHistorianSettings.PercentDataBad;
            configuration.AggregateConfiguration.UseSlopedExtrapolation.Value = UFUAHistorianSettings.UseSlopedExtrapolation;
            configuration.AggregateConfiguration.TreatUncertainAsBad.Value = UFUAHistorianSettings.TreatUncertainAsBad;

            lock (Lock)
            {
                mapNodeIdToAggregateConfigurations.Add(variable.NodeId.ToString(), configuration.AggregateConfiguration);
            }

            variable.AddChild(configuration);
        }
        
        readonly Dictionary<String, BaseObjectTypeState> typeDefinitions = new Dictionary<String, BaseObjectTypeState>();
        protected BaseObjectState CreateObject(UFUATag ufuaTag, NodeState parentFolder, NodeId nodeId, bool creatingType = false, bool forceRetentive = false)
        {
            var ufw = ufuaTag.Session;
            //using (UnitOfWork ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var prototypeName = ufuaTag.PrototypeName;
                var typedeflist = (from p in tagPrototypes.AsParallel()
                                    where p.Name == prototypeName && p.UFUATagOwner == null
                                    select p).ToList();
                UFUAModel.UFUATagPrototype typedef = typedeflist.Count > 0 ? typedeflist[0] : null;

                NodeId superTypeId = NodeId.Null;
                if (typedef != null)
                {
                    lock (Lock)
                    {
                        if (!typeDefinitions.ContainsKey(typedef.Name))
                        {
                            BaseObjectTypeState typeDefObject = new BaseObjectTypeState();

                            typeDefObject.SuperTypeId = NodeId.Create(Opc.Ua.ObjectTypes.BaseObjectType, Opc.Ua.Namespaces.OpcUa, SystemContext.NamespaceUris);
                            typeDefObject.WriteMask = AttributeWriteMask.None;
                            typeDefObject.UserWriteMask = AttributeWriteMask.None;
                            typeDefObject.IsAbstract = false;

                            typeDefObject.NodeId = FromGuidToNodeId(typedef.NodeId);
                            typeDefObject.BrowseName = new QualifiedName(typedef.Name, NamespaceIndex);
                            typeDefObject.DisplayName = typeDefObject.BrowseName.Name;
                            typeDefObject.SymbolicName = typeDefObject.BrowseName.Name;
                            typeDefObject.Description = typedef.Description;

                            var sortedFolders = (from c in typedef.Folders.AsParallel()
                                                 orderby c.Oid
                                                 select c).ToList();

                            var usedNames = new List<String>();
                            foreach (var folder in sortedFolders)
                            {
                                if (usedNames.Contains(folder.Name))
                                {
                                    var oldName = folder.Name;
                                    var newName = oldName;
                                    
                                    bool exist = true;
                                    while (exist)
                                    {
                                        newName = String.Format("{0}1", newName);
                                        exist = (from c in sortedFolders.AsParallel()
                                                 where c.Name == newName
                                                 select c).SingleOrDefault() != null;
                                        if (!exist)
                                            folder.Name = newName;
                                    }

                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.DuplicatedFolderNameOnPrototype, oldName, typedef.Name, newName),
                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                }

                                usedNames.Add(folder.Name);
                                BaseObjectState ba = CreateFolder(ufuaFolder: folder, parent: typeDefObject.NodeId, parentFolder: null,
                                    bAssignNodeId: false, creatingType: true, bAssignFolderId: false, bInsideObject: true);
                                if (ba != null)
                                    typeDefObject.AddChild(ba);
                            }

                            var sortedTags = (from c in typedef.Members.AsParallel()
                                              orderby c.Oid
                                              select c).ToList();

                            usedNames.Clear();
                            foreach (var tag in sortedTags)
                            {
                                if (usedNames.Contains(tag.Name))
                                {
                                    var oldName = tag.Name;
                                    var newName = oldName;

                                    bool exist = true;
                                    while (exist)
                                    {
                                        newName = String.Format("{0}1", newName);
                                        exist = (from c in sortedTags.AsParallel()
                                                 where c.Name == newName
                                                 select c).SingleOrDefault() != null;
                                        if (!exist)
                                            tag.Name = newName;
                                    }
                                    
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.DuplicatedMemberNameOnPrototype, oldName, typedef.Name, newName),
                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                }

                                usedNames.Add(tag.Name);
                                BaseInstanceState ba = CreateTag(ufuaTag: tag, parent: typeDefObject.NodeId, parentFolder: null,
                                    bAssignNodeId: false, creatingType: true, bInsideObject: true);
                                if (ba != null)
                                    typeDefObject.AddChild(ba);
                            }

                            typeDefinitions.Add(typedef.Name, typeDefObject);

                            AddPredefinedNode(SystemContext, typeDefObject);
                        }

                        superTypeId = typeDefinitions[typedef.Name].NodeId;
                    }
                }

                if (creatingType)
                    return null;

                BaseObjectState baseObject = new BaseObjectState(parentFolder);
                baseObject.BrowseName = new QualifiedName(ufuaTag.Name, NamespaceIndex);
                baseObject.DisplayName = baseObject.BrowseName.Name;
                baseObject.SymbolicName = baseObject.BrowseName.Name;
                baseObject.TypeDefinitionId = superTypeId;
                baseObject.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                if (nodeId == null)
                    baseObject.NodeId = New(SystemContext, baseObject);
                else
                    baseObject.NodeId = nodeId;

                if (!ufuaTag.ExcludeDynamicSettings && !String.IsNullOrEmpty(ufuaTag.DynamicSettings) && !NodeId.IsNull(baseObject.NodeId))
                    mapNodeIdToDynamicSettings.Add(baseObject.NodeId, ufuaTag.DynamicSettings);

                if (typedef != null)
                {
                    Dictionary<Guid, UFUATag> subMembers = null;
                    if (ufuaTag.SubPrototypeMembers != null && ufuaTag.SubPrototypeMembers.Count > 0)
                    {
                        var prototype = ufuaTag.SubPrototypeMembers[0];
                        subMembers = (from c in prototype.GetTagMembers().AsParallel() 
                                      where !c.UseShared.Value select c).ToDictionary(c => c.NodeId, c => c);
                    }

                    if (typedef.Folders.Count > 0)
                    {
                        var sortedFoldersType = (from c in typedef.Folders.AsParallel()
                                                 orderby c.Oid
                                                 select c).ToList();
                        foreach (var folder in sortedFoldersType)
                        {
                            CreateFolder(ufuaFolder: folder, parent: baseObject.NodeId, parentFolder: baseObject,
                                bAssignNodeId: true, creatingType: false, bAssignFolderId: true,
                                forceRetentive: ufuaTag.IsRetentive || forceRetentive, bInsideObject: true);
                        }
                    }

                    if (typedef.Members.Count > 0)
                    {
                        var sortedTagsType = (from c in typedef.Members.AsParallel()
                                              orderby c.Oid
                                              select c).ToList();
                        foreach (var stag in sortedTagsType)
                        {
                            var tag = stag;
                            if (subMembers != null && subMembers.ContainsKey(tag.NodeId))
                            {
                                UFUAModel.Helpers.KeepReadOnlyProperties.CopyReadOnlyProperties(stag, subMembers[tag.NodeId]);
                                tag = subMembers[tag.NodeId];
                            }
                            CreateTag(ufuaTag: tag, parent: baseObject.NodeId, parentFolder: baseObject,
                                bAssignNodeId: true, creatingType: false, forceRetentive: ufuaTag.IsRetentive || forceRetentive, bInsideObject: true,
                                forceInUse: (ufuaTag.AlwaysInUse.HasValue ? ufuaTag.AlwaysInUse.Value : false));
                        }
                    }
                }
                else
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        String.Format(Properties.Resources.PrototypeModelNotFound, ufuaTag.PrototypeName, ufuaTag.Name),
                        System.Diagnostics.EventLogEntryType.Warning,
                        LoggerDestination.Server);
                }

                return baseObject;
            }
        }

        BaseObjectState CreateServerFolder(String name, Guid guid)
        {
            var serverObject = server.ServerInstance.DiagnosticsNodeManager.FindPredefinedNode(ObjectIds.Server, typeof(ServerObjectState));
            var folder = new FolderState(serverObject) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.NodeId = FromGuidToNodeId(guid);
            if (serverObject == null)
                folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.Server);
            else
                serverObject.AddChild(folder);

            return folder;
        }

        protected BaseObjectState CreateRootFolder(String name, Guid guid)
        {
            var folder = new FolderState(null) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.NodeId = FromGuidToNodeId(guid);
            folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

            return folder;
        }

        protected virtual BaseObjectState CreateFolder(UFUAModel.UFUAFolder ufuaFolder, NodeId parent, 
                                     BaseObjectState parentFolder = null,
                                     bool bAssignNodeId = false, bool creatingType = false,
                                     bool bAssignFolderId = false, bool forceRetentive = false, bool bInsideObject = false)
        {
            if (!creatingType)
            {
                var nodeId = FromGuidToNodeId(ufuaFolder.NodeId);
                if (mapNodeIdToNodeState.ContainsKey(nodeId))
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        String.Format(Properties.Resources.DuplicatedFolderNodeId, nodeId, ufuaFolder.GetFullName()),
                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    return null;
                }
            }

            var folder = new FolderState(parentFolder) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(ufuaFolder.Name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            if (!bAssignFolderId)
                folder.NodeId = FromGuidToNodeId(ufuaFolder.NodeId);
            else
                folder.NodeId = SystemContext.NodeIdFactory.New(SystemContext, folder);

            if (parentFolder == null)
            {
                folder.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(folder);
            }

            var usedNames = new List<String>();
            foreach (var ufuainFolder in ufuaFolder.UFUAFolders)
            {
                if (creatingType)
                {
                    if (usedNames.Contains(ufuainFolder.Name))
                    {
                        var oldName = ufuainFolder.Name;
                        var newName = oldName;

                        bool exist = true;
                        while (exist)
                        {
                            newName = String.Format("{0}1", newName);
                            exist = (from c in ufuaFolder.UFUAFolders.AsParallel()
                                     where c.Name == newName
                                     select c).SingleOrDefault() != null;
                            if (!exist)
                                ufuainFolder.Name = newName;
                        }

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            String.Format(Properties.Resources.DuplicatedFolderNameOnPrototype, oldName, ufuainFolder.PrototypeReference.Name, newName),
                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }

                    usedNames.Add(ufuainFolder.Name);
                }

                CreateFolder(ufuainFolder, folder.NodeId, folder, 
                    bAssignNodeId, creatingType, bAssignFolderId, 
                    forceRetentive, bInsideObject);
            }

            usedNames.Clear();
            foreach (var ufuatag in ufuaFolder.UFUATags)
            {
                if (creatingType)
                {
                    if (usedNames.Contains(ufuatag.Name))
                    {
                        var oldName = ufuatag.Name;
                        var newName = oldName;

                        bool exist = true;
                        while (exist)
                        {
                            newName = String.Format("{0}1", newName);
                            exist = (from c in ufuaFolder.UFUATags.AsParallel()
                                     where c.Name == newName
                                     select c).SingleOrDefault() != null;
                            if (!exist)
                                ufuatag.Name = newName;
                        }

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            String.Format(Properties.Resources.DuplicatedMemberNameOnPrototype, oldName, ufuatag.PrototypeReference.Name, newName),
                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }

                    usedNames.Add(ufuatag.Name);
                }

                CreateTag(ufuatag, folder.NodeId, folder, 
                    bAssignNodeId, creatingType, 
                    forceRetentive, bInsideObject);
            }

            if (!creatingType)
            {
                AddPredefinedNode(SystemContext, folder);
                mapNodeIdToNodeState.Add(folder.NodeId, folder);
            }

            return folder;
        }

        BaseObjectState CreateSourceState(UFUAModel.UFUAAlarmSource ufuaFolder, NodeId parent, 
                                            BaseObjectState parentFolder)
        {
            var nodeid = FromGuidToNodeId(ufuaFolder.NodeId);
            if (mapNodeIdToNodeState.ContainsKey(nodeid))
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    String.Format(Properties.Resources.DuplicatedSourceNodeId, nodeid, ufuaFolder.Name),
                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                return null;
            }

            var folder = new SourceState(parentFolder, ufuaFolder.Name, ufuaFolder.GetRelativeName(), nodeid, SystemContext);

            // HasEventSource and HasNotifier control the propagation of event notifications so
            // they are not like other references. These calls set up a link between the source
            // and area that will cause events produced by the source to be automatically 
            // propagated to the area.
            folder.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, true, parentFolder);
            parentFolder.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, false, folder);
            folder.EventNotifier |= EventNotifiers.SubscribeToEvents | EventNotifiers.HistoryRead;

            AddPredefinedNode(SystemContext, folder);
            mapNodeIdToNodeState.Add(folder.NodeId, folder);

            folder.Init(server.RetentiveConnectionString, server.UFUAConfiguration.MaxHistoryAlarmsBranches.Value);
            folder.UpdatedStatusEvent += OnSourceStateChangedAlarm;
            folder.AlarmsStatusChanged += OnSourceAlarmsStatusChanged;
            folder.AlarmStateChanged += OnAlarmStateChanged;

            mapNodeIdToSourceState.Add(nodeid, folder);
            if (!mapAreaNodeIdToSources.ContainsKey(parent))
            {
                mapAreaNodeIdToSources[parent] = new List<NodeId>();
            }
            mapAreaNodeIdToSources[parent].Add(nodeid);

            return folder;
        }

        BaseObjectState CreateArea(UFUAModel.UFUAArea ufuaFolder, NodeId parent, 
                                   BaseObjectState parentFolder = null)
        {
            var nodeId = FromGuidToNodeId(ufuaFolder.NodeId);
            if (mapNodeIdToNodeState.ContainsKey(nodeId))
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    String.Format(Properties.Resources.DuplicatedAreaNodeId, nodeId, ufuaFolder.Name),
                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                return null;
            }

            var folder = new Model.AreaState(parentFolder);
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.NodeId = nodeId;
            folder.BrowseName = new QualifiedName(ufuaFolder.Name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.ReferenceTypeId = ReferenceTypeIds.HasNotifier;
            folder.EventNotifier = EventNotifiers.SubscribeToEvents | EventNotifiers.HistoryRead;
            if (parentFolder != null)
                parentFolder.AddChild(folder);
            folder.AddReference(ReferenceTypeIds.Organizes, true, parent);

            foreach (var ufuainFolder in ufuaFolder.UFUAAreas)
            {
                CreateArea(ufuainFolder, folder.NodeId, folder);
            }

            foreach (var ufuatag in ufuaFolder.UFUAAlarmSources)
            {
                CreateSourceState(ufuatag, folder.NodeId, folder);
            }

            AddPredefinedNode(SystemContext, folder);
            mapNodeIdToNodeState.Add(folder.NodeId, folder);

            if (!ufuaFolder.AlarmsNumEnabledTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumEnabledTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumEnabledStateNodeId = nodeid;
            }
            if (!ufuaFolder.AlarmsNumActiveOnTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumActiveOnTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumActiveOnStateNodeId = nodeid;
            }
            if (!ufuaFolder.AlarmsNumActiveOffTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumActiveOffTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumActiveOffStateNodeId = nodeid;
            }
            if (!ufuaFolder.AlarmsNumActiveOnOffTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumActiveOnOffTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumActiveOnOffStateNodeId = nodeid;
            }
            if (!ufuaFolder.AlarmsNumShelvedTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumShelvedTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumShelvedStateNodeId = nodeid;
            }
            if (!ufuaFolder.AlarmsNumNotAckTag.IsEmpty())
            {
                var nodeid = ufuaFolder.AlarmsNumNotAckTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].AlarmsNumNotAckStateNodeId = nodeid;
            }
            if (!ufuaFolder.MessagesNumEnabledTag.IsEmpty())
            {
                var nodeid = ufuaFolder.MessagesNumEnabledTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].MessagesNumEnabledStateNodeId = nodeid;
            }
            if (!ufuaFolder.MessagesNumActiveOnTag.IsEmpty())
            {
                var nodeid = ufuaFolder.MessagesNumActiveOnTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].MessagesNumActiveOnStateNodeId = nodeid;
            }
            if (!ufuaFolder.MessagesNumShelvedTag.IsEmpty())
            {
                var nodeid = ufuaFolder.MessagesNumShelvedTag.ResolveNodeId(NamespaceIndex);
                if (!mapAreaNodeIdToSettings.ContainsKey(folder.NodeId))
                    mapAreaNodeIdToSettings[folder.NodeId] = new AreaSettings(ufuaFolder.Name);
                mapAreaNodeIdToSettings[folder.NodeId].MessagesNumShelvedStateNodeId = nodeid;
            }

            return folder;
        }

        void CreateAddEventLogMethods()
        {
            var methodName = UFUAServerInfo.BrowserNames.AddEventLog;
            var methodBase = new MethodState(baseFolderTags);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, true);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "Source",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventMessage",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventDetails",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "EventComment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserName",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "RecordingTime",
                DataType = DataTypeIds.DateTime,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Severity",
                DataType = DataTypeIds.Int32,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });
           

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            //methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            //methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            //methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            //methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            //methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            //methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            //methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            //methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            //args = new List<Argument>();
            //args.Add(new Argument()
            //{
            //    Name = "message",
            //    DataType = DataTypeIds.String,
            //    ValueRank = ValueRanks.Scalar,
            //    ArrayDimensions = null
            //});

            //methodBase.OutputArguments.Value = args.ToArray();
            //mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            baseFolderTags.AddChild(methodBase);

            // set up method handlers. 
            methodBase.OnCallMethod = OnAddEventLog;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
        }

        void CreateAuditMethods()
        {
            var methodName = UFUAServerInfo.BrowserNames.WriteAuditValue;
            var methodBase = new MethodState(baseFolderTags);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, true);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "NodeId",
                DataType = DataTypeIds.NodeId,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Value",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Comment",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            //args.Add(new Argument()
            //{
            //    Name = "ElectronicSignature",
            //    DataType = DataTypeIds.String,
            //    ValueRank = ValueRanks.Scalar,
            //    ArrayDimensions = null
            //});

            args.Add(new Argument()
            {
                Name = "UserName",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "Password",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "TokenId",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "UserRoleRequired",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            args.Add(new Argument()
            {
                Name = "UserLevelRequired",
                DataType = DataTypeIds.Int32,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            baseFolderTags.AddChild(methodBase);

            // set up method handlers. 
            methodBase.OnCallMethod = OnWriteAuditValue;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
        }

        void CreateStatisticMethods()
        {
            var methodName = UFUAServerInfo.BrowserNames.ResetStatistics;
            var methodBase = new MethodState(baseFolderTags);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, true);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "NodeId",
                DataType = DataTypeIds.NodeId,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            //methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            //methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            //methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            //methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            //methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            //methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            //methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            //methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            //methodBase.OutputArguments.Value = args.ToArray();
            //mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            baseFolderTags.AddChild(methodBase);

            // set up method handlers. 
            methodBase.OnCallMethod = OnResetStatistics;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);
        }

        #endregion

        #region INodeManager Members
#if !DEBUG
        private bool initHist = false;
        private long maxAlarmItems = 0;
        private long AlarmItemsCount = 0;
        private bool maxAlarmReached = false;
#endif
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
                    updatedChangeMasksNodeStates = new List<NodeState>();

#if !NET_STANDARD
                    if (server.UFUAConfiguration.SpeechEnabled)
                    {
                        if (synthesizer == null)
                        {
                            synthesizer = new SpeechSynthesizer();
                            try
                            {
                                if (!String.IsNullOrEmpty(server.UFUAConfiguration.SpeechVoiceName))
                                {
                                    try
                                    {
                                        synthesizer.SelectVoice(server.UFUAConfiguration.SpeechVoiceName);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.GetDestinationLog(LoggerDestination.Server).Error(String.Format(Properties.Resources.SpeechCannotSelectVoice, server.UFUAConfiguration.SpeechVoiceName, ex.Message));
                                    }
                                }
                                synthesizer.SetOutputToDefaultAudioDevice();
                            }
                            catch (Exception ex)
                            {
                                Logger.GetDestinationLog(LoggerDestination.Server).Error(String.Format(Properties.Resources.SpeechErrorCannotBeInitialized, ex.Message));
                                synthesizer.Dispose();
                                synthesizer = null;
                            }
                        }
                    }
#endif
                    IList<IReference> references = null;

                    if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                    {
                        externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                    }

                    using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                    {
                        using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.CreatingAlarms,
                                                        Properties.Resources.CreatedAlarms))
                        {
                            var folders = (from folder in new XPQuery<UFUAModel.UFUAArea>(ufw).AsParallel()
                                           where folder.UFUAAreaAss == null
                                           select folder).ToList();

                            if (folders.Count > 0)
                            {
                                baseFolderAlarms = CreateRootFolder(UFUAServerInfo.UFUAServerInfo.GetAlarmRootName(), UFUAServerInfo.Guids.RootAlarmsGuid);

                                int current = 0;
                                int total = folders.Count;
                                var dt = DateTime.UtcNow.AddMilliseconds(2000);
                                foreach (var folder in folders)
                                {
                                    if (++current < total && dt <= DateTime.UtcNow)
                                    {
                                        dt = DateTime.UtcNow.AddMilliseconds(2000);
                                        Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                            Properties.Resources.CreatingAlarms, (current * 100) / total));
                                    }

                                    var area = CreateArea(folder, baseFolderAlarms.NodeId, baseFolderAlarms);
                                    if (area == null)
                                        continue;
                                    AddRootNotifier(area);

                                    // add an organizes reference from the ObjectsFolder to the area.
                                    references.Add(new NodeStateReference(ReferenceTypeIds.HasNotifier, false, area.NodeId));
                                }

                                AddPredefinedNode(SystemContext, baseFolderAlarms);
                                mapNodeIdToNodeState.Add(baseFolderAlarms.NodeId, baseFolderAlarms);
                            }
                        }

                        using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.CreatingDriverMethods,
                                                        Properties.Resources.CreatedDriverMethods))
                        {
                            if (server.CommDrivers.Keys.Count > 0)
                            {
                                baseFolderDrivers = CreateRootFolder(UFUAServerInfo.UFUAServerInfo.GetDriverRootName(), UFUAServerInfo.Guids.RootDriversGuid);

                                int current = 0;
                                int total = server.CommDrivers.Count;
                                var dt = DateTime.UtcNow.AddMilliseconds(2000);
                                foreach (var driver in server.CommDrivers.Keys)
                                {
                                    if (++current < total && dt <= DateTime.UtcNow)
                                    {
                                        dt = DateTime.UtcNow.AddMilliseconds(2000);
                                        Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                            Properties.Resources.CreatingDriverMethods, (current * 100) / total));
                                    }

                                    CreateWriteValuesMethod(driver, baseFolderDrivers.NodeId, baseFolderDrivers);
                                    CreateReadValuesMethod(driver, baseFolderDrivers.NodeId, baseFolderDrivers);
                                }

                                AddPredefinedNode(SystemContext, baseFolderDrivers);
                                mapNodeIdToNodeState.Add(baseFolderDrivers.NodeId, baseFolderDrivers);
                            }
                        }

                        using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.CreatingDiagnostic,
                                                        Properties.Resources.CreatedDiagnostic))
                        {
                            if (server.CommDrivers.Keys.Count > 0)
                            {
                                baseFolderDiagnosticDrivers = CreateServerFolder("DiagnosticInformations", UFUAServerInfo.Guids.RootDiagnosticGuid);

                                int current = 0;
                                int total = server.CommDrivers.Count;
                                var dt = DateTime.UtcNow.AddMilliseconds(2000);
                                foreach (var driver in server.CommDrivers.Keys)
                                {
                                    if (++current < total && dt <= DateTime.UtcNow)
                                    {
                                        dt = DateTime.UtcNow.AddMilliseconds(2000);
                                        Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                            Properties.Resources.CreatingDiagnostic, (current * 100) / total));
                                    }

                                    CreateDriverDiagnosticObject(driver, baseFolderDiagnosticDrivers.NodeId, baseFolderDiagnosticDrivers);
                                }

                                AddPredefinedNode(SystemContext, baseFolderDiagnosticDrivers);
                                mapNodeIdToNodeState.Add(baseFolderDiagnosticDrivers.NodeId, baseFolderDiagnosticDrivers);
                            }
                        }
#if !DEBUG 
                        initHist = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxIRk/KJdyld2yhb2z/b0ClA=="/* DLR */);
                        if(!initHist)
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                Properties.Resources.DataloggerLicenseNotFound,
                                EventLogEntryType.Warning, LoggerDestination.License);
                        
                        maxAlarmItems = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNLOWKl78+jQgKGUlcim5yQ=="/* ALR */);
                        AlarmItemsCount = 0;
#endif
                        using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.CreatingAddressSpace,
                                                        Properties.Resources.CreatedAddressSpace))
                        {
                            enumStrings = (from c in new XPQuery<UFUAModel.UFUAEnumString>(ufw).AsParallel()
                                           select c).ToList();

                            alarmDefinitions = (from c in new XPQuery<UFUAModel.UFUAAlarmDefinition>(ufw).AsParallel()
                                                select c).ToList();

                            alarmThresholds = (from c in new XPQuery<UFUAModel.UFUAAlarmThreshold>(ufw).AsParallel()
                                               select c).ToList();

                            tagPrototypes = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(ufw).AsParallel()
                                             select p).ToList();

                            engineeringUnits = (from engunit in new XPQuery<UFUAModel.UFUAEngineeringUnit>(ufw).AsParallel()
                                                select engunit).ToList();

                            historians = (from hissett in new XPQuery<UFUAModel.UFUAHistorianSettings>(ufw).AsParallel()
                                          select hissett).ToList();

                            views = (from view in new XPQuery<UFUAModel.UFUAView>(ufw).AsParallel()
                                     select view).ToList();
                            foreach (var view in views)
                                CreateView(view);

                            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(ufw).AsParallel()
                                           where folder.Name != AliasRoot && folder.UFUAFolderAss == null 
                                           && folder.UFUATagPrototype == null
                                           select folder).ToList();

                            var roots = (from folder in new XPQuery<UFUAModel.UFUAFolder>(ufw).AsParallel()
                                         where folder.Name == AliasRoot && folder.UFUAFolderAss == null
                                         && folder.UFUATagPrototype == null
                                         select folder).ToList();
                            if (roots.Count > 0)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.InvalidRootFolderName, AliasRoot),
                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                            }

                            var rootTags = (from tag in new XPQuery<UFUAModel.UFUATag>(ufw).AsParallel()
                                            where tag.UFUAFolder == null && tag.UFUATagPrototype == null
                                            select tag).ToList();

                            
                            if (folders.Count > 0 || rootTags.Count > 0)
                            {
                                baseFolderTags = CreateRootFolder(UFUAServerInfo.UFUAServerInfo.GetTagRootName(), UFUAServerInfo.Guids.RootTagsGuid);

                                int current = 0;
                                int total = folders.Count + rootTags.Count;
                                var dt = DateTime.UtcNow.AddMilliseconds(2000);
                                foreach (var tag in rootTags)
                                {
                                    if (++current < total && dt <= DateTime.UtcNow)
                                    {
                                        dt = DateTime.UtcNow.AddMilliseconds(2000);
                                        Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                            Properties.Resources.CreatingAddressSpace, (current * 100) / total));
                                    }

                                    CreateTag(tag, baseFolderTags.NodeId, baseFolderTags);
                                }

                                foreach (var folder in folders)
                                {
                                    if (++current < total && dt <= DateTime.UtcNow)
                                    {
                                        dt = DateTime.UtcNow.AddMilliseconds(2000);
                                        Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                            Properties.Resources.CreatingAddressSpace, (current * 100) / total));
                                    }

                                    CreateFolder(folder, baseFolderTags.NodeId, baseFolderTags);
                                }

                                if (IsAnyAuditTraceEnabled())
                                {
                                    CreateAuditMethods();
                                }

                                if (IsAnyTagStatisticsEnabled())
                                {
                                    CreateStatisticMethods();
                                }

                                CreateAddEventLogMethods();

                                AddPredefinedNode(SystemContext, baseFolderTags);
                                mapNodeIdToNodeState.Add(baseFolderTags.NodeId, baseFolderTags);
                            }
                        }
#if !DEBUG 
                        if(initHist)
#endif
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                            Properties.Resources.InitializingDataLoggers,
                                                            Properties.Resources.InitializedDataLoggers))
                        { 
                            // Retreive valid data loggers from XPObject session.
                            var enabledDataloggers = (from c in new XPQuery<DataLoggerModel.DataLoggerSettings>(ufw).AsParallel()
                                               where c.Enable.Value
                                               select c).ToList();

                            var invalidDataloggers = (from c in enabledDataloggers/*.AsParallel()*/ // AsParallerl cannot be used in this case for avoing thread-safe exception when "c.IsValid" method is called.
                                                      where !c.IsValid
                                                      select c).ToList();

                            var validDataloggers = (from c in enabledDataloggers.AsParallel()
                                                    where !invalidDataloggers.Contains(c)
                                                    select c).ToList();

                            if (validDataloggers.Count > 0)
                            {
                                var checkUserIdentity = (from settings in validDataloggers.AsParallel()
                                                         where settings.EnableDataProtection
                                                         select settings).ToList().Count > 0;

                                InitDataLogger(checkUserIdentity);

                                if (dataLogger != null)
                                {
                                    var listEnableTags = new List<NodeId>();
                                    var listRecordingTags = new List<NodeId>();
                                    var listResetTags = new List<NodeId>();
                                    var listColumnsTags = new List<NodeId>();

                                    int current = 0;
                                    int total = validDataloggers.Count;
                                    var dt = DateTime.UtcNow.AddMilliseconds(2000);
                                    foreach (var dlsettings in validDataloggers)
                                    {
                                        if (++current < total && dt <= DateTime.UtcNow)
                                        {
                                            dt = DateTime.UtcNow.AddMilliseconds(2000);
                                            Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                                Properties.Resources.InitializingDataLoggers, (current * 100) / total));
                                        }

                                        // Retreive valid columns from data logger settings.
                                        var columns = (from c in dlsettings.Columns.AsParallel()
                                                       where c.IsValid && mapNodeIdToNodeState.ContainsKey(c.ColumnTag.ResolveNodeId(NamespaceIndex))
                                                       select c).ToList();

                                        if (columns.Count > 0)
                                        {
                                            var usedNames = new List<String>();
                                            var mapguids = new Dictionary<Guid, String>();
                                            foreach (var column in columns)
                                            {
                                                if (!mapguids.ContainsKey(column.ColumnTag.Guid))
                                                    mapguids.Add(column.ColumnTag.Guid, column.Name);

                                                if (usedNames.Contains(column.Name))
                                                {
                                                    var oldName = column.Name;
                                                    var newName = oldName;

                                                    bool exist = true;
                                                    while (exist)
                                                    {
                                                        newName = String.Format("{0}1", newName);
                                                        exist = (from c in columns.AsParallel()
                                                                 where c.Name == newName
                                                                 select c).SingleOrDefault() != null;
                                                        if (!exist)
                                                            column.ColumnName = newName;
                                                    }

                                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                        String.Format(Properties.Resources.DuplicatedDataLoggerColumnName, oldName, dlsettings.Name, newName),
                                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                                }

                                                usedNames.Add(column.Name);
                                            }

                                            var maptags = (from tag in new XPQuery<UFUAModel.UFUATag>(ufw).AsParallel()
                                                           where mapguids.ContainsKey(tag.NodeId) && !tag.IsSubPrototypeMember
                                                           select tag).GroupBy(tag => tag.NodeId, tag => tag).AsParallel().ToDictionary(tag => tag.Key, tag => tag.First());

                                            Parallel.ForEach(columns, col =>
                                            {
                                                if (maptags.ContainsKey(col.ColumnTag.Guid))
                                                    col.UFUATagReference = maptags[col.ColumnTag.Guid];
                                            });

                                            if (mapDataLoggerNameToDataLoggerEntity.ContainsKey(dlsettings.Name))
                                            {
                                                var oldName = dlsettings.Name;
                                                var newName = oldName;
                                                
                                                bool exist = true;
                                                while (exist)
                                                {
                                                    newName = String.Format("{0}1", newName);
                                                    exist = (from c in validDataloggers.AsParallel()
                                                             where c.Name == newName
                                                             select c).SingleOrDefault() != null;
                                                    if (!exist)
                                                        dlsettings.Name = newName;
                                                }

                                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    String.Format(Properties.Resources.DuplicatedDataLoggerName, oldName, newName),
                                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                            }

                                            bool bInitialized = false;
                                            try
                                            {
                                                bInitialized = dataLogger.Init(dlsettings);
                                            }
                                            catch (Exception ex)
                                            {
                                                var message = String.Format(
                                                    Properties.Resources.DataLoggerErrorOnInitializing,
                                                    dlsettings.Name, ex.Message);
                                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                            }

                                            if (bInitialized)
                                            {
                                                var dlentity = new DataLoggerManager.DataLoggerEntity()
                                                {
                                                    dataLoggerName = dlsettings.Name,
                                                    recordingTime = DateTime.MinValue,
                                                    listDataColumnEntities = new List<DataLoggerManager.DataColumEntity>(columns.Count)
                                                };
                                                mapDataLoggerNameToDataLoggerEntity.Add(dlsettings.Name, dlentity);

                                                bool bInvalidTag;
                                                if (dlsettings.EnableRecordingTag != null && !dlsettings.EnableRecordingTag.IsEmpty())
                                                {
                                                    bInvalidTag = false;
                                                    if (mapNodeIdToNodeState.ContainsKey(dlsettings.EnableRecordingTag.ResolveNodeId(NamespaceIndex)))
                                                    {
                                                        var nodestate = mapNodeIdToNodeState[dlsettings.EnableRecordingTag.ResolveNodeId(NamespaceIndex)];
                                                        if (IsConvertibleToDouble(nodestate))
                                                        {
                                                            dataLogger.EnableDataLoggerRecording(dlsettings.Name, false);
                                                            if (!mapNodeIdToDataLoggerSettings.ContainsKey(nodestate.NodeId))
                                                                mapNodeIdToDataLoggerSettings.Add(nodestate.NodeId, new List<DataLoggerModel.DataLoggerSettings>());
                                                            if (!mapNodeIdToDataLoggerSettings[nodestate.NodeId].Contains(dlsettings))
                                                                mapNodeIdToDataLoggerSettings[nodestate.NodeId].Add(dlsettings);
                                                            if (!listEnableTags.Contains(nodestate.NodeId))
                                                            {
                                                                listEnableTags.Add(nodestate.NodeId);
                                                                nodestate.StateChanged += OnNodeEnableRecordingChangedDataLogger;
                                                                AssignNodeToAlwaysInUseList(nodestate);
                                                            }
                                                            nodestate.UpdateChangeMasks(NodeStateChangeMasks.Value);
                                                            updatedChangeMasksNodeStates.Add(nodestate);
                                                        }
                                                        else
                                                            bInvalidTag = true;
                                                    }
                                                    else
                                                        bInvalidTag = true;

                                                    if (bInvalidTag)
                                                    {
                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.DataLoggerInvalidEnableRecordingTag, dlsettings.Name, dlsettings.EnableRecordingTag),
                                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                    }
                                                }

                                                if (dlsettings.RecordingTag != null && !dlsettings.RecordingTag.IsEmpty())
                                                {
                                                    bInvalidTag = false;
                                                    if (mapNodeIdToNodeState.ContainsKey(dlsettings.RecordingTag.ResolveNodeId(NamespaceIndex)))
                                                    {
                                                        var nodestate = mapNodeIdToNodeState[dlsettings.RecordingTag.ResolveNodeId(NamespaceIndex)];
                                                        if (IsConvertibleToDouble(nodestate))
                                                        {
                                                            if (!mapNodeIdToDataLoggerSettings.ContainsKey(nodestate.NodeId))
                                                                mapNodeIdToDataLoggerSettings.Add(nodestate.NodeId, new List<DataLoggerModel.DataLoggerSettings>());
                                                            if (!mapNodeIdToDataLoggerSettings[nodestate.NodeId].Contains(dlsettings))
                                                                mapNodeIdToDataLoggerSettings[nodestate.NodeId].Add(dlsettings);
                                                            if (!listRecordingTags.Contains(nodestate.NodeId))
                                                            {
                                                                listRecordingTags.Add(nodestate.NodeId);
                                                                nodestate.StateChanged += OnNodeRecordingChangedDataLogger;
                                                                AssignNodeToAlwaysInUseList(nodestate);
                                                            }
                                                        }
                                                        else
                                                            bInvalidTag = true;
                                                    }
                                                    else
                                                        bInvalidTag = true;

                                                    if (bInvalidTag)
                                                    {
                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.DataLoggerInvalidRecordingTag, dlsettings.Name, dlsettings.RecordingTag),
                                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                    }
                                                }

                                                if (dlsettings.ResettingTag != null && !dlsettings.ResettingTag.IsEmpty())
                                                {
                                                    bInvalidTag = false;
                                                    if (mapNodeIdToNodeState.ContainsKey(dlsettings.ResettingTag.ResolveNodeId(NamespaceIndex)))
                                                    {
                                                        var nodestate = mapNodeIdToNodeState[dlsettings.ResettingTag.ResolveNodeId(NamespaceIndex)];
                                                        if (IsConvertibleToDouble(nodestate))
                                                        {
                                                            if (!mapNodeIdToDataLoggerSettings.ContainsKey(nodestate.NodeId))
                                                                mapNodeIdToDataLoggerSettings.Add(nodestate.NodeId, new List<DataLoggerModel.DataLoggerSettings>());
                                                            if (!mapNodeIdToDataLoggerSettings[nodestate.NodeId].Contains(dlsettings))
                                                                mapNodeIdToDataLoggerSettings[nodestate.NodeId].Add(dlsettings);
                                                            if (!listResetTags.Contains(nodestate.NodeId))
                                                            {
                                                                listResetTags.Add(nodestate.NodeId);
                                                                nodestate.StateChanged += OnNodeResettingChangedDataLogger;
                                                                AssignNodeToAlwaysInUseList(nodestate);
                                                            }
                                                        }
                                                        else
                                                            bInvalidTag = true;
                                                    }
                                                    else
                                                        bInvalidTag = true;


                                                    if (bInvalidTag)
                                                    {
                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.DataLoggerInvalidResettingTag, dlsettings.Name, dlsettings.ResettingTag),
                                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                    }
                                                }

                                                foreach (var col in columns)
                                                {
                                                    if (col.UFUATagReference == null)
                                                    {
                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.DataLoggerInvalidColumnTag, dlsettings.Name, col.ColumnTag, col.Name),
                                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                        continue;
                                                    }

                                                    var nodeIds = new List<NodeId>();
                                                    ExpressionValueConverter converter = null;
                                                    if (DataLoggerModel.Helpers.DataLoggerSettingsHelper.IsObjectTypeValue(col))
                                                    {
                                                        var prototypeName = col.UFUATagReference.PrototypeName;
                                                        var typedef = (from p in tagPrototypes.AsParallel()
                                                                       where p.Name == prototypeName && p.UFUATagOwner == null
                                                                       select p).FirstOrDefault();
                                                        if (typedef != null)
                                                        {
                                                            var members = typedef.GetTagMembers(bSorted: true);
                                                            foreach (var member in members)
                                                            {
                                                                NodeId nodeId = GetResolvedNodeId(member, col.UFUATagReference.NodeId);
                                                                if (mapNodeIdToNodeState.ContainsKey(nodeId))
                                                                {
                                                                    var nodestate = mapNodeIdToNodeState[nodeId];
                                                                    if (!mapNodeIdToDataLoggerSettings.ContainsKey(nodestate.NodeId))
                                                                        mapNodeIdToDataLoggerSettings.Add(nodestate.NodeId, new List<DataLoggerModel.DataLoggerSettings>());
                                                                    if (!mapNodeIdToDataLoggerSettings[nodestate.NodeId].Contains(dlsettings))
                                                                        mapNodeIdToDataLoggerSettings[nodestate.NodeId].Add(dlsettings);
                                                                    if (!listColumnsTags.Contains(nodestate.NodeId))
                                                                    {
                                                                        listColumnsTags.Add(nodestate.NodeId);
                                                                        nodestate.StateChanged += OnNodeSateChangedDataLogger;
                                                                        AssignNodeToAlwaysInUseList(nodestate);
                                                                    }
                                                                    nodestate.UpdateChangeMasks(NodeStateChangeMasks.Value);
                                                                    updatedChangeMasksNodeStates.Add(nodestate);
                                                                    nodeIds.Add(nodeId);
                                                                }
                                                                else
                                                                {
                                                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                    String.Format(Properties.Resources.DataLoggerInvalidNestedColumnTag, col.Name, dlsettings.Name, member.Name),
                                                                    System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                                }
                                                            }
                                                        }

                                                        if (typedef == null || nodeIds.Count == 0)
                                                        {
                                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                String.Format(Properties.Resources.DataLoggerInvalidColumnTag, dlsettings.Name, col.ColumnTag, col.Name),
                                                                System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!String.IsNullOrEmpty(col.Expression))
                                                        {
                                                            converter = new ExpressionValueConverter(col.Expression);
                                                            converter.ThrowExceptions = true;
                                                            converter.ParseFormula();
                                                            var error = converter.GetParserError();
                                                            if (!String.IsNullOrEmpty(error))
                                                            {
                                                                converter.Dispose();
                                                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                    String.Format(Properties.Resources.DataLoggerColumnExpressionError, error, dlsettings.Name, col.ColumnName),
                                                                    System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                                continue;
                                                            }
                                                        }

                                                        var nodeId = col.ColumnTag.ResolveNodeId(NamespaceIndex);
                                                        var nodestate = mapNodeIdToNodeState[nodeId];
                                                        if (!mapNodeIdToDataLoggerSettings.ContainsKey(nodestate.NodeId))
                                                            mapNodeIdToDataLoggerSettings.Add(nodestate.NodeId, new List<DataLoggerModel.DataLoggerSettings>());
                                                        if (!mapNodeIdToDataLoggerSettings[nodestate.NodeId].Contains(dlsettings))
                                                            mapNodeIdToDataLoggerSettings[nodestate.NodeId].Add(dlsettings);
                                                        if (!listColumnsTags.Contains(nodestate.NodeId))
                                                        {
                                                            listColumnsTags.Add(nodestate.NodeId);
                                                            nodestate.StateChanged += OnNodeSateChangedDataLogger;
                                                            AssignNodeToAlwaysInUseList(nodestate);
                                                        }
                                                        nodestate.UpdateChangeMasks(NodeStateChangeMasks.Value);
                                                        updatedChangeMasksNodeStates.Add(nodestate);
                                                        nodeIds.Add(nodeId);
                                                    }

                                                    // Is a valid datalogger column and can be handled at runtime.
                                                    dlentity.listDataColumnEntities.Add(new DataLoggerManager.DataColumEntity()
                                                    {
                                                        nodeId = col.ColumnTag != null ? col.ColumnTag.ResolveNodeId(NamespaceIndex) : NodeId.Null,
                                                        columnName = col.ColumnName,
                                                        columnValue = new DataLoggerManager.DataColumnValue(nodeIds, converter),
                                                        skipDataChange = col.SkipDataChange,
                                                        sourceTimeStampColumnEnabled = col.AddSourceTimeStampColumn,
                                                        serverTimeStampColumnEnabled = col.AddServerTimeStampColumn,
                                                        statusCodeColumnEnabled = col.AddStatusCodeColumn,
                                                        userColumnEnabled = col.AddUserColumn,
                                                        stringValueColumnEnabled = col.AddStringValueColumn,
                                                        expression = col.Expression
                                                    });
                                                }

                                                var invalidColumns = (from c in dlsettings.Columns.AsParallel()
                                                                      where !columns.Contains(c) && (c.ColumnTag == null || c.ColumnTag.Guid != TagEntityReference.Reserved.Guid)
                                                                      select c).ToList();

                                                if (invalidColumns.Count > 0)
                                                {
                                                    invalidColumns.ForEach((col) =>
                                                    {
                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.DataLoggerInvalidColumnTag, dlsettings.Name, col.ColumnTag ?? UFUAModel.TagEntityReference.Empty, col.Name),
                                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                                    });
                                                }

#if !NET_STANDARD
                                                if (server.IsRedundancyEnabled)
                                                {
                                                    var connection = XpoHelpers.XpoHelper.NormalizeConnectionString(dlsettings.ConnectionSettings.Connection, false);
                                                    if (server.ActiveServerManager.MapDataLoggerConnection == null)
                                                        server.ActiveServerManager.MapDataLoggerConnection = new Dictionary<String, DataConnectionParameters>();
                                                    if (!server.ActiveServerManager.MapDataLoggerConnection.ContainsKey(dlsettings.Name))
                                                        server.ActiveServerManager.MapDataLoggerConnection.Add(dlsettings.Name, new DataConnectionParameters()
                                                        {
                                                            DataSourceName = dlsettings.ConnectionSettings.DataSourceName,
                                                            DataProvider = dlsettings.ConnectionSettings.DataProvider,
                                                            Connection = connection
                                                        });
                                                }
#endif
                                            }
                                            else
                                            {
                                                var message = String.Format(
                                                    Properties.Resources.DataLoggerFailedToInitialize.Replace("-newline-", Environment.NewLine),
                                                    dlsettings.Name);
                                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                            }
                                        }
                                    }
                                }
                            }

                            if (invalidDataloggers.Count > 0)
                            {
                                invalidDataloggers.ForEach((dlsettings) =>
                                {
                                    string message = Properties.Resources.DataLoggerInvalid;
                                    if (dlsettings.Columns.Count == 0)
                                    {
                                        message = String.Format(Properties.Resources.DataLoggerMissingColumn, dlsettings.Name);
                                    }
                                    else if (!dlsettings.RecordOnDataChange && (dlsettings.RecordingTag == null || dlsettings.RecordingTag.IsEmpty()))
                                    {
                                        message = String.Format(Properties.Resources.DataLoggerMissingRecordingTag, dlsettings.Name);
                                    }
                                    else if (dlsettings.RecordOnDataChange && dlsettings.RecordingTimeInterval == TimeSpan.Zero)
                                    {
                                        message = String.Format(Properties.Resources.DataLoggerMissingRecordingTimeInterval, dlsettings.Name);
                                    }
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                });
                            }
                        }

                        using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                        Properties.Resources.CreatingSystemTags,
                                                        Properties.Resources.CreatedSystemTags))
                        {
                            // get the server object.
                            //ServerObjectState serverObject = (ServerObjectState)server.CurrentInstance.DiagnosticsNodeManager.FindPredefinedNode(
                            //    ObjectIds.Server,
                            //    typeof(ServerObjectState));

                            systemTags = new SystemTagsState(/*serverObject*/null, SystemContext, NamespaceIndex);
                            systemTags.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                            //baseObject.TypeDefinitionId = superTypeId;
                            //baseObject.ReferenceTypeId = ReferenceTypeIds.HasComponent;

                            // Handle the AlarmsSoundState state changed event
                            systemTags.AlarmsSoundState.StateChanged += OnAlarmsSoundStateChanged;

                            systemTags.RuntimeAlarmSettingsUpdateMethod.OnCallMethod = OnRuntimeAlarmSettingsUpdate;
                            systemTags.RuntimeAlarmSettingsUpdateMethod.UserExecutable = true;
                            systemTags.RuntimeAlarmSettingsUpdateMethod.ClearChangeMasks(SystemContext, true);

#if !NET_STANDARD
                            systemTags.RedundancySwitchActiveServerMethod.OnCallMethod = OnRedundancySwitchActiveServer;
                            systemTags.RedundancySwitchActiveServerMethod.UserExecutable = server.IsRedundancyEnabled;
                            systemTags.RedundancySwitchActiveServerMethod.ClearChangeMasks(SystemContext, true);
#endif

#if !DEBUG
                            var serial = MSZ.MSZView.GetSerialInfo(shortVersion: true); 
                            systemTags.LicenseSerialNumber.Value = serial;
                            systemTags.LicenseSerialNumber.Timestamp = DateTime.UtcNow;
                            systemTags.LicenseSerialNumber.ClearChangeMasks(SystemContext, true);
                            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
                            {
                                var newSerial = MSZ.MSZView.GetSerialInfo(shortVersion: true);
                                if (systemTags.LicenseSerialNumber.Value != newSerial)
                                {
                                    systemTags.LicenseSerialNumber.Value = newSerial;
                                    systemTags.LicenseSerialNumber.Timestamp = DateTime.UtcNow;
                                    systemTags.LicenseSerialNumber.ClearChangeMasks(SystemContext, true);
                                }
                            };
#else
                            systemTags.LicenseSerialNumber.Value = "0";
                            systemTags.LicenseSerialNumber.Timestamp = DateTime.UtcNow;
                            systemTags.LicenseSerialNumber.ClearChangeMasks(SystemContext, true);
#endif

                            AddPredefinedNode(SystemContext, systemTags);
                            mapNodeIdToNodeState.Add(systemTags.NodeId, systemTags);
                        }
                    }
                }

                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                Properties.Resources.ProcessingPendingTask,
                                                Properties.Resources.ProcessedPendingTask))
                {
                    if (listPendingInitTask.Count() > 0)
                    {
                        long current = 0;
                        int total = listPendingInitTask.Count;
                        var listTask = new List<Task>();
                        listPendingInitTask.ForEach(action =>
                        {
                            var task1 = Task.Factory.StartNew(action);
                            listTask.Add(task1);
                            task1.ContinueWith((o) =>
                            {
                                Interlocked.Increment(ref current);
                            });
                        });

                        while (!Task.WaitAll(listTask.ToArray(), 2000))
                        {
                            var progress = Interlocked.Read(ref current);
                            if (progress < total)
                            {
                                Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                    Properties.Resources.ProcessingPendingTask, (progress * 100) / total));
                            }
                        };

                        listPendingInitTask.Clear();
                    }
                }

                lock (listNodeToUpdate)
                {
                    if (nodeStateUpdater != null)
                        nodeStateUpdater.Change(0, System.Threading.Timeout.Infinite);
                    bNodeStateUpdaterReady = true;
                }

                UpdateAlarmRuntimeChanges();

                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                Properties.Resources.InitializingAlarms,
                                                Properties.Resources.InitializedAlarms))
                {
                    lock (Lock)
                    {
                        foreach (var key in mapNodeIdToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var baseVariable = mapNodeIdToNodeState[key] as NodeState;
                            baseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(baseVariable);
                        }

                        foreach (var key in mapEnableStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var enableBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            enableBaseVariable.StateChanged += OnNodeStateChangedAlarmEnabled;
                            enableBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(enableBaseVariable);
                        }

                        foreach (var key in mapActivationLowStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var activationBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            activationBaseVariable.StateChanged += OnNodeStateChangedLowActivationValue;
                            activationBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(activationBaseVariable);
                        }

                        foreach (var key in mapActivationStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var activationBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            activationBaseVariable.StateChanged += OnNodeStateChangedActivationValue;
                            activationBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(activationBaseVariable);
                        }

                        foreach (var key in mapHighHighStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var highHighBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            highHighBaseVariable.StateChanged += OnNodeStateChangedHighHighLimit;
                            highHighBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(highHighBaseVariable);
                        }

                        foreach (var key in mapHighStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var highBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            highBaseVariable.StateChanged += OnNodeStateChangedHighLimit;
                            highBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(highBaseVariable);
                        }

                        foreach (var key in mapLowStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var lowBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            lowBaseVariable.StateChanged += OnNodeStateChangedLowLimit;
                            lowBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(lowBaseVariable);
                        }

                        foreach (var key in mapLowLowStateToAlarmStatus.Keys)
                        {
                            if (!mapNodeIdToNodeState.ContainsKey(key))
                                continue;

                            var lowLowBaseVariable = mapNodeIdToNodeState[key] as BaseDataVariableState;
                            lowLowBaseVariable.StateChanged += OnNodeStateChangedLowLowLimit;
                            lowLowBaseVariable.UpdateChangeMasks(NodeStateChangeMasks.Value);
                            updatedChangeMasksNodeStates.Add(lowLowBaseVariable);
                        }

                        foreach (var settings in mapAreaNodeIdToSettings.Values)
                        {
                            settings.GetStateNodeIds().ForEach((nodeId) => 
                            {
                                if (!mapNodeIdToNodeState.ContainsKey(nodeId))
                                {
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.AlarmAreaInvalidTag, settings.AreaName, nodeId),
                                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                }
                            });
                        }
                    }
                }

                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                Properties.Resources.UpdatingNodeState,
                                                Properties.Resources.UpdatedNodeState))
                {
                    updatedChangeMasksNodeStates.ForEach((node) => node.ClearChangeMasks(SystemContext, true));
                }

                if (dataLogger != null)
                    dataLogger.Start();

                using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                {
                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                    Properties.Resources.CreatingDynamics,
                                                    Properties.Resources.CreatedDynamics))
                    {
                        var dyntags = (from tag in new XPQuery<UFUAModel.UFUATag>(ufw).AsParallel()
                                       where !tag.ExcludeDynamicSettings && !String.IsNullOrEmpty(tag.DynamicSettings) &&
                                       ((tag.IsSubPrototypeMember && !tag.UseShared.Value) 
                                       || tag.IsPrototypeMember == false)
                                       select tag).ToDictionary(tag => tag, tag => tag.GetDynamicSettings());

                        bool bArrayOneSize = false;
#if !DEBUG
                        bArrayOneSize = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx3ItKbxy9EMJoUOWvtWCzGQ=="/* AR1 */);
#endif
                        totalCount = UFUAModel.Helpers.DynamicTagsCounter.GetDynamicTagsCounter(ufw, bArrayOneSize, dyntags.Keys.ToList());
                        systemTags.DynamicTagCount.Value = totalCount;
                        systemTags.DynamicTagCount.ClearChangeMasks(SystemContext, true);

#if !DEBUG
                            long maxConcurrentItems = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxxJm6Pl2n3HEE5KHmXWpASw=="/* STG */);
                            if (totalCount > maxConcurrentItems)
                            {
#if !NET_STANDARD
                                if (Environment.UserInteractive)
                                    System.Windows.Forms.MessageBox.Show(String.Format(Properties.Resources.LicenseTagExceeded, totalCount, maxConcurrentItems));
#endif
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, 
                                    String.Format(Properties.Resources.LicenseTagExceeded, totalCount, maxConcurrentItems), 
                                    EventLogEntryType.Warning, LoggerDestination.License);

                                System.Environment.Exit(-11);
                            }
#endif
                        int current = 0;
                        int total = server.CommDrivers.Count;
                        var dt = DateTime.UtcNow.AddMilliseconds(2000);
                        foreach (var driver in server.CommDrivers.Keys)
                        {
                            if (++current < total && dt <= DateTime.UtcNow)
                            {
                                dt = DateTime.UtcNow.AddMilliseconds(2000);
                                Console.WriteLine(String.Format("{0} - {1} ({2}%)", DateTime.Now,
                                    Properties.Resources.CreatingDynamics, (current * 100) / total));
                            }

                            var drivermodeltags = new Dictionary<UFUATag, String>();
                            string drName = string.Format("{0}.", driver.ToLower());
                            Parallel.ForEach(dyntags.Keys.ToList(), tag =>
                            {
                                foreach (var dyn in tag.GetDynamicSettings())
                                {
                                    if (dyn.ToLower().StartsWith(drName))
                                    {
                                        lock (drivermodeltags)
                                        {
                                            // constraint of only one dynamic address with same driver.
                                            if (drivermodeltags.ContainsKey(tag))
                                            {
                                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    String.Format(Properties.Resources.ErrorDuplicatedDriverDynamicSettings, tag.Name, dyn, tag.NodeId),
                                                    System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                            }
                                            else
                                                drivermodeltags[tag] = dyn;
                                        }

                                        lock (dyntags)
                                        {
                                            if (dyntags.ContainsKey(tag))
                                                dyntags[tag].Remove(dyn);
                                            if (dyntags[tag].Count == 0)
                                                dyntags.Remove(tag);
                                        }
                                    }
                                }
                            });

                            var drivertags = new List<TagDefinition>();
                            Parallel.ForEach(drivermodeltags.Keys, drivertag =>
                            {
                                NodeId nodeid = null;
                                if (drivertag.IsSubPrototypeMember)
                                    nodeid = drivertag.GetResolvedNodeId(NamespaceIndex);
                                else
                                    nodeid = FromGuidToNodeId(drivertag.NodeId);

                                var tagdef = new TagDefinition()
                                {
                                    NodeId = nodeid,
                                    DynamicSettings = drivermodeltags[drivertag],
                                    ArrayDimension = drivertag.ArrayDimension,
                                    SamplingInterval = ((mapNodeIdToSamplingIntervals.ContainsKey(nodeid) && mapNodeIdToSamplingIntervals[nodeid] != -1) ?
                                    mapNodeIdToSamplingIntervals[nodeid] :
                                    server.UFUAConfiguration.DefaultDataIOSamplingInterval)
#if DEBUG
                                    ,
                                    Name = (drivertag.FolderPath.Length == 0 ? drivertag.Name : string.Format("{0}\\{1}", drivertag.FolderPath, drivertag. Name))
#endif
                                };

                                lock (drivertags)
                                {
                                    if (mapNodeIdToNodeState.ContainsKey(tagdef.NodeId))
                                        drivertags.Add(tagdef);
                                    else
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.ErrorMissingDriverNodeIdForDrivers, tagdef.NodeId, driver),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                    }
                                }
                            });

                            var inuse = new List<TagDefinition>();
                            Parallel.ForEach(drivermodeltags.Keys, drivertag =>
                            {
                                NodeId nodeid = null;
                                if (drivertag.IsSubPrototypeMember)
                                    nodeid = drivertag.GetResolvedNodeId(NamespaceIndex);
                                else
                                    nodeid = FromGuidToNodeId(drivertag.NodeId);

                                if (mapAlwaysNodeIdInUse.ContainsKey(nodeid))
                                {
                                    mapAlwaysNodeIdInUse[nodeid].ForEach((id) =>
                                    {
                                        var tagdef = new TagDefinition()
                                        {
                                            NodeId = id,
                                            SamplingInterval = ((mapNodeIdToSamplingIntervals.ContainsKey(id) && mapNodeIdToSamplingIntervals[id] != -1) ?
                                            mapNodeIdToSamplingIntervals[id] :
                                            server.UFUAConfiguration.DefaultDataIOSamplingInterval)
                                        };

                                        lock (inuse)
                                        {
                                            inuse.Add(tagdef);
                                        }
                                    });
                                }
                            });

                            var tagstoAdd = new List<TagDefinition>();
                            for (int i = 0; i < drivertags.Count; i++)
                            {
                                var tagType = drivertags[i];
                                SetStatusCode(tagType.NodeId, StatusCodes.BadWaitingForInitialData);

                                if (mapNodeIdToNodeState[tagType.NodeId] is BaseDataVariableState)
                                {
                                    tagType.DataType = (mapNodeIdToNodeState[tagType.NodeId] as BaseDataVariableState).DataType;
                                    tagType.InitialValue = (mapNodeIdToNodeState[tagType.NodeId] as BaseDataVariableState).Value;
                                }
                                else if (mapNodeIdToNodeState[tagType.NodeId] is BaseObjectState)
                                {
                                    BaseObjectState baseObjectType = mapNodeIdToNodeState[tagType.NodeId] as BaseObjectState;
                                    tagType.DataType = baseObjectType.TypeDefinitionId;
                                }
                                else if (mapNodeIdToNodeState[tagType.NodeId] is MethodState)
                                {
                                    tagType.DataType = (mapNodeIdToNodeState[tagType.NodeId] as MethodState).ReferenceTypeId;
                                }

                                var baseObject = mapNodeIdToNodeState[tagType.NodeId];

                                AddCommunicationDriverToNodeStateDictionary(baseObject, driver);
                                tagstoAdd.Add(tagType);
                            }
                            server.CommDrivers[driver].TagChanging += UANodeManager_TagChanging;
                            server.CommDrivers[driver].TagChanged += UANodeManager_TagChanged;
                            server.CommDrivers[driver].SystemEvent += UANodeManager_SystemEvent;
                            server.CommDrivers[driver].AudiEvent += UANodeManager_AudiEvent;
                            server.CommDrivers[driver].TagPrototypeQuery += UANodeManager_TagPrototypeQuery;
                            var commDriverEx = server.CommDrivers[driver] as ICommunicationDriver2;
                            if (commDriverEx != null)
                                commDriverEx.StateChanged += UANodeManager_StateChanged;
                            server.CommDrivers[driver].AddDynamics(tagstoAdd);
                            

                            var listObservingNodes = server.CommDrivers[driver].GetObservingNodes();
                            listObservingNodes.ForEach(nodeidToObserve =>
                                {
                                    if (mapNodeIdToNodeState.ContainsKey(nodeidToObserve) && mapNodeIdToNodeState[nodeidToObserve] is BaseDataVariableState)
                                    {
                                        var state = mapNodeIdToNodeState[nodeidToObserve];
                                        state.StateChanged += (ISystemContext context, NodeState node, NodeStateChangeMasks changes) =>
                                        {
                                            if ((changes & NodeStateChangeMasks.Value) != 0)
                                            {
                                                DataValue value = new DataValue 
                                                { 
                                                    Value = null, 
                                                    ServerTimestamp = DateTime.UtcNow, 
                                                    SourceTimestamp = DateTime.MinValue, 
                                                    StatusCode = StatusCodes.Good 
                                                };

                                                ServiceResult error = node.ReadAttribute(
                                                        context,
                                                        Attributes.Value,
                                                        NumericRange.Empty,
                                                        null,
                                                        value);

                                                server.CommDrivers[driver].UpdateObservedTag(nodeidToObserve, value);
                                            }
                                        };
                                        state.UpdateChangeMasks(NodeStateChangeMasks.Value);
                                        state.ClearChangeMasks(SystemContext, true);
                                    }
                                    else
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.ErrorMissingDriverNodeIdForDrivers, nodeidToObserve, driver),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                                });

                            //server.CommDrivers[driver].Startup();

                            if (inuse.Count > 0)
                            {
                                Parallel.ForEach(inuse, tag =>
                                {
                                    lock (mapNodeIdToInUseInfo)
                                    {
                                        mapNodeIdToInUseInfo[tag.NodeId] = new UANodeInUseInfo
                                        {
                                            Occurrences = 1,
                                            SamplingInterval = tag.SamplingInterval,
                                            SamplingIntervalCollection = new Dictionary<int, double>()
                                        };
                                        mapNodeIdToInUseInfo[tag.NodeId].SamplingIntervalCollection.Add(-1, tag.SamplingInterval);
                                    }
                                });

                                server.CommDrivers[driver].InUseDynamics(inuse, true);
                            }
                        }

                        foreach (var driver in server.CommDriversDisabled.Keys)
                        {
                            string drName = string.Format("{0}.", driver.ToLower());
                            Parallel.ForEach(dyntags.Keys.ToList(), tag =>
                            {
                                foreach (var dyn in tag.GetDynamicSettings())
                                {
                                    if (dyn.ToLower().StartsWith(drName))
                                    {
                                        bool bSetQuality = false;
                                        lock (dyntags)
                                        {
                                            if (dyntags.ContainsKey(tag))
                                                dyntags[tag].Remove(dyn);
                                            if (dyntags[tag].Count == 0)
                                                bSetQuality = dyntags.Remove(tag);
                                        }

                                        if (bSetQuality)
                                        {
                                            NodeId nodeid = null;
                                            if (tag.IsSubPrototypeMember)
                                                nodeid = tag.GetResolvedNodeId(NamespaceIndex);
                                            else
                                                nodeid = FromGuidToNodeId(tag.NodeId);

                                            SetStatusCode(nodeid, StatusCodes.UncertainLastUsableValue);
                                        }
                                    }
                                }
                            });
                        }

                        foreach (var driver in server.CommDriversFaulted.Keys)
                        {
                            RaiseSystemEvents(null, driver, String.Format(Properties.Resources.InitDriverMissingLicense, driver), EventSeverity.High, DateTime.UtcNow, addtosystemlog: true);
                        }

                        if (dyntags.Keys.Count > 0)
                        {
                            dyntags.Keys.ToList().ForEach(drivertag =>
                            {
                                NodeId nodeid = null;
                                if (drivertag.IsSubPrototypeMember)
                                    nodeid = drivertag.GetResolvedNodeId(NamespaceIndex);
                                else
                                    nodeid = FromGuidToNodeId(drivertag.NodeId);

                                SetStatusCode(nodeid, StatusCodes.BadResourceUnavailable);

                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.ErrorMissingDriverDynamicSettings, drivertag.Name, String.Join(";", dyntags[drivertag]), nodeid),
                                    System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                            });
                        }

                        mapAlwaysNodeIdInUse.Clear();
                    }
                }

                lock (Lock)
                {
                    AddReverseReferences(externalReferences);
                }
#if !NET_STANDARD
#region Redundancy
                if (server.IsRedundancyEnabled)
                {
                    InitEventLogger();

                    redundancyPrivateNodeIds.AddRange(systemTags.GetRedundancyPrivateNodeIds());

                    server.ActiveServerManager.HistorianConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.HistorianDefaultConnection);
                    server.ActiveServerManager.EventConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.EventDefaultConnection);

                    systemTags.RedundancySwitchActiveServerMethod.UserExecutable = server.ActiveServerManager.IsSwitchExecutable;
                    systemTags.RedundancySwitchActiveServerMethod.ClearChangeMasks(SystemContext, true);

                    server.ActiveServerManager.TagsChanged += (o, e) =>
                        {
                            Parallel.ForEach(e, changedTags =>
                            {
                                UANodeManager_TagChanged(this, new ChangedTagArgs() { NodeId = changedTags.NodeId, DataValues = changedTags.DataValues });

                                if (mapNodeIdToTagLogEntities.ContainsKey(changedTags.NodeId) &&
                                    mapNodeIdToTagLogEntities[changedTags.NodeId].enableStatistics)
                                {
                                    var entry = mapNodeIdToTagLogEntities[changedTags.NodeId];
                                    entry.min = changedTags.Min;
                                    entry.max = changedTags.Max;
                                    entry.totAverage = changedTags.TotAverage;
                                    entry.countUpdates = changedTags.CountUpdates;
                                    entry.totalTimeOn = changedTags.TotalTimeOn;
                                    entry.lastTotalTimeOn = changedTags.LastTotalTimeOn;

                                    entry.OnStatisticUpdated();
                                }

                                //ThreadPool.QueueUserWorkItem((ob) =>
                                {
                                    if (changedTags.DataValues.Count > 0)
                                    {
                                        var value = changedTags.DataValues[changedTags.DataValues.Count - 1].Value;
                                        var status = changedTags.DataValues[changedTags.DataValues.Count - 1].StatusCode;
                                        var timestamp = changedTags.DataValues[changedTags.DataValues.Count - 1].ServerTimestamp;
                                        var handle = GetManagerHandle(SystemContext, changedTags.NodeId, null);
                                        if (handle != null)
                                        {
                                            var commDriver = GetCommunicationDriversFromNodeStateDictionary(handle.Node);
                                            if (commDriver != null && commDriver.Count > 0)
                                            {
                                                foreach (var driver in commDriver)
                                                    driver.OnWriteTag(new TagDefinition() { NodeId = changedTags.NodeId }, ref value, ref status, ref timestamp);
                                            }
                                        }
                                    }
                                }//);
                            });
                        };
                    server.ActiveServerManager.AlarmsChanged += (o, e) =>
                    {
                        ThreadPool.QueueUserWorkItem((ob) =>
                            {
                                if (mapNodeIdToSourceState.ContainsKey(e.nodeId))
                                {
                                    var sourceState = mapNodeIdToSourceState[e.nodeId];
                                    sourceState.UpdateAlarmStatus(e.alarmsStatus, false);
                                }
                            });
                    };

                    if (server.ActiveServerManager.IsActiveServer)
                    {
                        systemTags.RedundancyActiveServerState.Value = true;
                        systemTags.RedundancyActiveServerState.ClearChangeMasks(SystemContext, true);
                    }
                    server.ActiveServerManager.ActiveServer += (o, ev) =>
                    {
                        systemTags.RedundancyActiveServerState.Value = true;
                        systemTags.RedundancyActiveServerState.ClearChangeMasks(SystemContext, true);
                    };
                    server.ActiveServerManager.InactiveServer += (o, ev) =>
                    {
                        systemTags.RedundancyActiveServerState.Value = false;
                        systemTags.RedundancyActiveServerState.ClearChangeMasks(SystemContext, true);
                    };
                    server.ActiveServerManager.ActiveServerChanged += (o, ev) =>
                    {
                        systemTags.RedundancyActiveServerHostName.Value = ev.activeServer;
                        systemTags.RedundancyActiveServerHostName.ClearChangeMasks(SystemContext, true);

                        var switchExecutable = server.ActiveServerManager.IsSwitchExecutable;
                        if (switchExecutable != systemTags.RedundancySwitchActiveServerMethod.UserExecutable)
                        {
                            systemTags.RedundancySwitchActiveServerMethod.UserExecutable = switchExecutable;
                            systemTags.RedundancySwitchActiveServerMethod.ClearChangeMasks(SystemContext, true);
                        }
                    };
                    server.ActiveServerManager.AliveServerListChanged += (o, ev) =>
                    {
                        systemTags.RedundancyArrayAliveServerHostName.Value = ev.aliveServers;
                        systemTags.RedundancyArrayAliveServerHostName.ClearChangeMasks(SystemContext, true);

                        var switchExecutable = server.ActiveServerManager.IsSwitchExecutable && ev.aliveServers.Length == 2;
                        if (switchExecutable != systemTags.RedundancySwitchActiveServerMethod.UserExecutable)
                        {
                            systemTags.RedundancySwitchActiveServerMethod.UserExecutable = switchExecutable;
                            systemTags.RedundancySwitchActiveServerMethod.ClearChangeMasks(SystemContext, true);
                        }
                    };
                    server.ActiveServerManager.WritingValue += (o, ev) =>
                    {
                        var handle = GetManagerHandle(SystemContext, ev.NodeId, null);
                        if (handle != null)
                        {
                            if (ev.DataValues.Count > 0)
                            {
                                var value = ev.DataValues[ev.DataValues.Count - 1];
                                var val = value.Value;
                                var stamp = value.SourceTimestamp;
                                var status = value.StatusCode;

                                if (value.Value != null)
                                {
                                    NodeState nodeState = GetNodeStateFromNodeId(ev.NodeId);
                                    var variable = nodeState as AnalogItemState;
                                    if (variable != null && variable.InstrumentRange != null &&
                                        variable.InstrumentRange.Value != null)
                                    {
                                        if (variable.DataType.IdType == IdType.Numeric)
                                        {
                                            uint dimension = 0;
                                            if (variable.ValueRank != ValueRanks.Scalar && variable.ArrayDimensions != null)
                                                dimension = variable.ArrayDimensions[0];
                                            uint nType = (uint)variable.DataType.Identifier;
                                            if (nType != Opc.Ua.DataTypes.String && nType != Opc.Ua.DataTypes.Boolean)
                                            {
                                                val = ScaleValue(value.Value, nType, dimension, variable.EURange.Value, variable.InstrumentRange.Value, false);
                                            }
                                        }
                                    }
                                }

                                ev.Status = (uint)OnWriteTagValue(SystemContext, handle.Node, NumericRange.Empty, null, ref val, ref status, ref stamp);
                                if (ev.Status == StatusCodes.Good)
                                {
                                    UANodeManager_TagChanged(this, new ChangedTagArgs() { NodeId = ev.NodeId, DataValues = ev.DataValues });
                                }
                            }
                            else
                                ev.Status = StatusCodes.BadNoData;
                        }
                        else
                            ev.Status = StatusCodes.BadNodeIdInvalid;
                    };
                    server.ActiveServerManager.UpdatingAlarms += (o, ev) => 
                    {
                        if (mapNodeIdToSourceState.ContainsKey(ev.nodeId))
                        {
                            var sourceState = mapNodeIdToSourceState[ev.nodeId];
                            sourceState.UpdateAlarmStatus(ev.alarmsStatus);
                        }
                    };
                    server.ActiveServerManager.CallingMethod += (o, ev) =>
                    {
                        var handle = GetManagerHandle(SystemContext, ev.methodNodeId, null);
                        if (handle != null)
                        {
                            if (handle.Node is MethodState)
                            {
                                var systemContext = SystemContext;
                                if ((handle.Node.BrowseName == new QualifiedName(UFUAServerInfo.BrowserNames.WriteAuditValue, NamespaceIndex)) || 
                                    (handle.Node.BrowseName == new QualifiedName(UFUAServerInfo.BrowserNames.AddEventLog, NamespaceIndex)))
                                {
                                    systemContext = SystemContext.Copy();
                                    systemContext.UserIdentity = new UserIdentity();
                                }
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
                    server.ActiveServerManager.GettingAllLiveData += (o, ev) =>
                    {
                        ev.liveData = GetAllValue();
                    };
                    server.ActiveServerManager.GettingAllAlarmsStatus += (o, ev) =>
                    {
                        ev.alarmsStatus = GetAllAlarmsStatus();
                    };
                    server.ActiveServerManager.SynchronizeAllLiveData += (o, ev) =>
                    {
                        if (ev.liveData == null)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                   String.Format(Properties.Resources.ErrorSynchronizingLiveDataServer, ev.hostName),
                                                   System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        }
                        else
                        {
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                            Properties.Resources.SynchronizingLiveDataServer,
                                                            Properties.Resources.SynchronizedLiveDataServer, 
                                                            ev.hostName))
                            {
                                SetAllValue(ev.liveData);
                            }
                        }
                    };
                    server.ActiveServerManager.SynchronizeAllAlarmsStatus += (o, ev) =>
                    {
                        if (ev.alarmsStatus == null)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                   String.Format(Properties.Resources.ErrorSynchronizingAlarmsStatusServer, ev.hostName),
                                                   System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        }
                        else
                        {
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server, 
                                                            Properties.Resources.SynchronizingAlarmsStatusServer,
                                                            Properties.Resources.SynchronizedAlarmsStatusServer, 
                                                            ev.hostName))
                            {
                                var validList = (from c in ev.alarmsStatus.Keys.AsParallel() where mapNodeIdToSourceState.ContainsKey(c) select c).ToList();
                                Parallel.ForEach(validList, key =>
                                {
                                    mapNodeIdToSourceState[key].UpdateAlarmStatus(ev.alarmsStatus[key], false);
                                });
                            }
                        }
                    };
                    server.ActiveServerManager.ServerStateChanging += (o, ev) =>
                    {
                        ThreadPool.QueueUserWorkItem((ob) =>
                            {
                                if (ev.serverState == RedundancyService.RedundancyServerState.StartPolling)
                                {
                                    if (historianLogger != null)
                                    {
                                        historianLogger.StopSync();
                                    }

                                    if (eventLogger != null)
                                    {
                                        eventLogger.StopSync();
                                    }

                                    if (dataLogger != null)
                                    {
                                        dataLogger.StopSync();
                                    }
                                }
                                else if (ev.serverState == RedundancyService.RedundancyServerState.Activating)
                                {
                                    if (historianLogger != null)
                                    {
                                        historianLogger.Resume();
                                    }

                                    if (eventLogger != null)
                                    {
                                        eventLogger.Resume();
                                    }

                                    if (dataLogger != null)
                                    {
                                        dataLogger.Resume();
                                    }
                                }
                                else if (ev.serverState == RedundancyService.RedundancyServerState.Deactivating)
                                {
                                    if (historianLogger != null)
                                    {
                                        historianLogger.Suspend();
                                    }

                                    if (eventLogger != null)
                                    {
                                        eventLogger.Suspend();
                                    }

                                    if (dataLogger != null)
                                    {
                                        dataLogger.Suspend();
                                    }
                                }
                            });
                    };
                    server.ActiveServerManager.SynchronizeHistoryData += (o, ev) =>
                    {
                        ev.IsExecuted = true;

                        if (ev.historySettings == null)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                   String.Format(Properties.Resources.ErrorSynchronizingHistoryDataServer, ev.hostName),
                                                   System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        }

                        if (ExitMode || ev.historySettings == null)
                            return;

                        ThreadPool.QueueUserWorkItem((ob) =>
                            {
                                if (historianLogger != null)
                                {
                                    if (ev.historySettings.HistorianCustomConnection != null)
                                    {
                                        foreach (var key in ev.historySettings.HistorianCustomConnection.Keys)
                                        {
                                            var connection = ev.historySettings.HistorianCustomConnection[key];
                                            if (server.ActiveServerManager.MapHistorianCustomConnection != null &&
                                                server.ActiveServerManager.MapHistorianCustomConnection.ContainsKey(key))
                                            {
                                                var destinationConn = XpoHelpers.XpoHelper.NormalizeConnectionString(server.ActiveServerManager.MapHistorianCustomConnection[key]);
                                                historianLogger.SynchronizeHistoryData(connection, destinationConn, ev.startTime, ev.endTime);
                                            }
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(ev.historySettings.HistorianDefaultConnection) &&
                                        !String.IsNullOrEmpty(server.UFUAConfiguration.HistorianDefaultConnection))
                                    {
                                        var destinationConn = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.HistorianDefaultConnection);
                                        historianLogger.SynchronizeHistoryData(ev.historySettings.HistorianDefaultConnection, destinationConn, ev.startTime, ev.endTime);
                                    }
                                }

                                if (!String.IsNullOrEmpty(ev.historySettings.EventDefaultConnection) &&
                                    !String.IsNullOrEmpty(server.UFUAConfiguration.EventDefaultConnection))
                                {
                                    var destinationConn = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.EventDefaultConnection);
                                    eventLogger.SynchronizeHistoryData(ev.historySettings.EventDefaultConnection, destinationConn, ev.startTime, ev.endTime);
                                }

                                if (dataLogger != null)
                                {
                                    if (ev.historySettings.DataLoggerConnection != null)
                                    {
                                        foreach (var key in ev.historySettings.DataLoggerConnection.Keys)
                                        {
                                            var parameters = ev.historySettings.DataLoggerConnection[key];
                                            if (server.ActiveServerManager.MapDataLoggerConnection != null &&
                                                server.ActiveServerManager.MapDataLoggerConnection.ContainsKey(key))
                                            {
                                                var source = new DataReader.DataReaderModel()
                                                {
                                                    DataSourceName = parameters.DataSourceName,
                                                    DataProvider = parameters.DataProvider,
                                                    Connection = parameters.Connection
                                                };
                                                dataLogger.SynchronizeHistoryData(key, source, ev.startTime, ev.endTime);
                                            }
                                        }
                                    }
                                }
                            });
                    };
                }
#endregion
#endif
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, Properties.Resources.ServerStarted, System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                Utils.Trace(e, "Unexpected error in Creating the address space");
                server.UpdateServerState(ServerState.Failed);
            }
            finally
            {
                lock (Lock)
                {
                    if (enumStrings != null)
                        enumStrings.Clear();
                    if (alarmDefinitions != null)
                        alarmDefinitions.Clear();
                    if (alarmThresholds != null)
                        alarmThresholds.Clear();
                    if (tagPrototypes != null)
                        tagPrototypes.Clear();
                    if (engineeringUnits != null)
                        engineeringUnits.Clear();
                    if (historians != null)
                        historians.Clear();
                    if (views != null)
                        views.Clear();
                    if (updatedChangeMasksNodeStates != null)
                        updatedChangeMasksNodeStates.Clear();
                    if (duplicatedHistoricalNames != null)
                        duplicatedHistoricalNames.Clear();
                    if (duplicatedUnitsNames != null)
                        duplicatedUnitsNames.Clear();
                }
            }
        }

        private void SetStatusCode(NodeId tagID, uint status, bool notify = true)
        {
            NodeState baseObject = null;
            lock (mapNodeIdToNodeState)
            {
                if (mapNodeIdToNodeState.ContainsKey(tagID))
                    baseObject = mapNodeIdToNodeState[tagID];
            }

            if (baseObject != null)
            {
                if (baseObject is BaseVariableState)
                {
                    var variable = baseObject as BaseVariableState;
                    variable.StatusCode = status;
                }

                if (notify)
                    baseObject.ClearChangeMasks(SystemContext, true);

                var baseObjectType = baseObject as BaseObjectState;
                if (baseObjectType != null)
                {
                    var children = new List<BaseInstanceState>();
                    baseObjectType.GetChildren(SystemContext, children);
                    children.ForEach((child) => SetStatusCode(child.NodeId, status, notify));
                }
            }
        }

#if !NET_STANDARD
        void SetAllValue(Dictionary<NodeId, LiveDataValue> values)
        {
            IDictionary<NodeId,NodeState> operationCache = new NodeIdDictionary<NodeState>();
            Parallel.ForEach(values.Keys, key =>
            {
                var handle = GetManagerHandle(SystemContext, key, operationCache);
                if (handle != null)
                {
                    if (handle.Node is BaseVariableState)
                    {
                        var variable = handle.Node as BaseVariableState;
                        variable.StatusCode = values[key].DataValue.StatusCode;
                        variable.Timestamp = values[key].DataValue.SourceTimestamp;
                        variable.Value = values[key].DataValue.DataValue.Value;

                        if (values[key].Statistics != null && 
                            mapNodeIdToTagLogEntities.ContainsKey(key) &&
                            mapNodeIdToTagLogEntities[key].enableStatistics)
                        {
                            var entry = mapNodeIdToTagLogEntities[key];
                            entry.min = values[key].Statistics.Min;
                            entry.max = values[key].Statistics.Max;
                            entry.totAverage = values[key].Statistics.TotAverage;
                            entry.countUpdates = values[key].Statistics.CountUpdates;
                            entry.totalTimeOn = values[key].Statistics.TotalTimeOn;
                            entry.lastTotalTimeOn = values[key].Statistics.LastTotalTimeOn;

                            entry.OnStatisticUpdated();
                        }

                        var commDriver = GetCommunicationDriversFromNodeStateDictionary(handle.Node);
                        if (commDriver != null && commDriver.Count > 0)
                        {
                            var tagvalue = variable.Value;
                            var tagstatus = variable.StatusCode;
                            var tagtime = variable.Timestamp;
                            foreach (var driver in commDriver)
                                driver.OnWriteTag(new TagDefinition() { NodeId = key }, ref tagvalue, ref tagstatus, ref tagtime);
                        }

                        variable.ClearChangeMasks(SystemContext, true);
                    }
                }
            });
        }

        Dictionary<NodeId, LiveDataValue> GetAllValue()
        {
            Dictionary<NodeId, NodeState> localmapNodeIdToNodeState = null;
            lock (mapNodeIdToNodeState)
            {
                localmapNodeIdToNodeState = new Dictionary<NodeId, NodeState>(mapNodeIdToNodeState);
            }

            var ret = new Dictionary<NodeId, LiveDataValue>();
            Parallel.ForEach(localmapNodeIdToNodeState.Keys, key =>
            {
                if (localmapNodeIdToNodeState[key] is BaseVariableState)
                {
                    var variable = localmapNodeIdToNodeState[key] as BaseVariableState;
                    LiveDataValue liveData = null;
                    lock (ret)
                    {
                        if (!ret.ContainsKey(key) && !redundancyPrivateNodeIds.Contains(key))
                        {
                            liveData = new LiveDataValue();
                            ret.Add(key, liveData);
                        }
                    }

                    if (liveData != null)
                    {
                        liveData.DataValue = new WrappedDataValue(variable.WrappedValue, variable.StatusCode, variable.Timestamp);
                        if (mapNodeIdToTagLogEntities.ContainsKey(key) && 
                            mapNodeIdToTagLogEntities[key].enableStatistics)
                        {
                            liveData.Statistics = new StatisticsData();
                            liveData.Statistics.Min = mapNodeIdToTagLogEntities[key].min;
                            liveData.Statistics.Max = mapNodeIdToTagLogEntities[key].max;
                            liveData.Statistics.TotAverage = mapNodeIdToTagLogEntities[key].totAverage;
                            liveData.Statistics.CountUpdates = mapNodeIdToTagLogEntities[key].countUpdates;
                            liveData.Statistics.TotalTimeOn = mapNodeIdToTagLogEntities[key].totalTimeOn;
                            liveData.Statistics.LastTotalTimeOn = mapNodeIdToTagLogEntities[key].lastTotalTimeOn;
                        }
                    }
                }

                var baseObjectType = localmapNodeIdToNodeState[key] as BaseObjectState;
                if (baseObjectType != null && !redundancyPrivateNodeIds.Contains(baseObjectType.NodeId))
                {
                    var children = new List<BaseInstanceState>();
                    baseObjectType.GetChildren(SystemContext, children);
                    List<BaseVariableState> list;
                    lock (ret)
                    {
                        list = (from c in children.OfType<BaseVariableState>()
                                where !ret.ContainsKey(c.NodeId) &&
                                !redundancyPrivateNodeIds.Contains(c.NodeId)
                                select c).ToList();
                    }

                    foreach (var child in list)
                    {
                        var liveData = new LiveDataValue();
                        liveData.DataValue = new WrappedDataValue(child.WrappedValue, child.StatusCode, child.Timestamp);
                        if (mapNodeIdToTagLogEntities.ContainsKey(child.NodeId) &&
                            mapNodeIdToTagLogEntities[child.NodeId].enableStatistics)
                        {
                            liveData.Statistics = new StatisticsData();
                            liveData.Statistics.Min = mapNodeIdToTagLogEntities[child.NodeId].min;
                            liveData.Statistics.Max = mapNodeIdToTagLogEntities[child.NodeId].max;
                            liveData.Statistics.TotAverage = mapNodeIdToTagLogEntities[child.NodeId].totAverage;
                            liveData.Statistics.CountUpdates = mapNodeIdToTagLogEntities[child.NodeId].countUpdates;
                            liveData.Statistics.TotalTimeOn = mapNodeIdToTagLogEntities[child.NodeId].totalTimeOn;
                            liveData.Statistics.LastTotalTimeOn = mapNodeIdToTagLogEntities[child.NodeId].lastTotalTimeOn;
                        }

                        lock (ret)
                        {
                            if (!ret.ContainsKey(child.NodeId))
                                ret.Add(child.NodeId, liveData);
                        }
                    }
                }
            });

            return ret;
        }

        Dictionary<NodeId, List<AlarmStatus>> GetAllAlarmsStatus()
        {
            var mapSource = new Dictionary<NodeId, List<AlarmStatus>>();
            Parallel.ForEach(mapAlarmStatusToSourceState.Keys, settings =>
            {
                var sourceState = mapAlarmStatusToSourceState[settings];
                sourceState.UpdateEventId(settings);
                var branches = settings.GetBranches();
                branches.ForEach((branch) => sourceState.UpdateEventId(branch));
                lock (mapSource)
                {
                    if (!mapSource.ContainsKey(sourceState.NodeId))
                        mapSource.Add(sourceState.NodeId, new List<AlarmStatus>());
                    mapSource[sourceState.NodeId].Add(settings);
                    foreach (var branch in branches)
                        mapSource[sourceState.NodeId].Add(branch);
                }
            });

            return mapSource;
        }
#endif

        void InitTagLogger()
        {
            lock (lockLogger)
            {
                if (tagLogger != null)
                    return;

                tagLogger = new UFUATagLogger.UFUATagLogger(server.RetentiveConnectionString);
            }
        }

        protected void InitEventLogger()
        {
            lock (lockLogger)
            {
                if (server.UFUAConfiguration.EnableEventDataProtection && !server.IsRunningAsCFR21IdentityUser)
                {
#if !NET_STANDARD
                    if (eventLogger != null)
                    {
                        eventLogger.Dispose();
                        eventLogger.RecyclingEvent -= eventLogger_RecyclingEvent;
                        eventLogger.FlushedEvent -= eventLogger_FlushedEvent;
                        eventLogger.FlushedDataSafely -= eventLogger_FlushedDataSafely;
                        eventLogger.ErrorFlushingData -= eventLogger_ErrorFlushingData;
                        eventLogger.StatisticDataChanged -= eventLogger_StatisticDataChanged;
                        eventLogger = null;
                    }

                    if (!isHistoricalLoggerDisabled)
                    {
                        isHistoricalLoggerDisabled = true;
                        var message = string.Format(Properties.Resources.RunningAsInvalidIdentityUser, UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting());
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, message,
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        server.OnBalloonEvent(message, System.Windows.Forms.ToolTipIcon.Error);
                    }
                    return;
#else
                    if (!isEventLogDataProtectionNotSupported)
                    {
                        isEventLogDataProtectionNotSupported = true;
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, Properties.Resources.EventDataProtectionNotSupported,
                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }
#endif
                }

                if (eventLogger != null || isHistoricalLoggerDisabled)
                    return;

                eventLogger = new UFUAEventLogger(new UFUAHistorianConfiguration()
                {
                    DefaultSettings = server.UFUAConfiguration.EventDefaultConnection,
                    SafelySettings = server.SafeDataConnectionString,
                    RedundancyServerId =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.ArrayPosition :
#endif
                        -1,
                    RedundancyHistoryThreadPool =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.HistoryThreadPool :
#endif
                        0,
                    RedundancyMaxSyncEntities = server.UFUAConfiguration.RedundancyMaxSyncEntities.Value,
                    MaxConcurrentAccess = server.UFUAConfiguration.MaxConcurrentHistoricalAccess.Value,
                    ErrorTimeInterval = TimeSpan.FromSeconds(server.UFUAConfiguration.WaitHistoryRetryTime.Value),
                    defaultMaxAge = server.UFUAConfiguration.EventMaxAge.Value,
                    MaxErrorBeforeFlush = server.UFUAConfiguration.MaxHistoryErrorBeforeFlush.Value,
                    MaxErrorCacheSize = server.UFUAConfiguration.MaxHistoryErrorCacheSize.Value,
                    MaxDeletingEntities = server.UFUAConfiguration.MaxHistoryDeletingEntities.Value,
                    MinPendingEntities = server.UFUAConfiguration.MinHistoryPendingEntities.Value,
                    MaxPendingEntities = server.UFUAConfiguration.MaxHistoryPendingEntities.Value,
                    MaxDeleteProcess = server.UFUAConfiguration.MaxHistoryDeleteProcess.Value,
                    MaxRestoreProcess = server.UFUAConfiguration.MaxHistoryRestoreProcess.Value,
                    MaxTotalSafelyFilesSize = server.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize.Value,
                    EnableEventDataProtection = server.UFUAConfiguration.EnableEventDataProtection
                });

                eventLogger.RecyclingEvent += eventLogger_RecyclingEvent;
                eventLogger.FlushedEvent += eventLogger_FlushedEvent;
                eventLogger.FlushedDataSafely += eventLogger_FlushedDataSafely;
                eventLogger.ErrorFlushingData += eventLogger_ErrorFlushingData;
                eventLogger.StatisticDataChanged += eventLogger_StatisticDataChanged;
            }
        }

        void InitHistorian(bool checkIdentityUser = false)
        {
            lock (lockLogger)
            {
                if (checkIdentityUser && !server.IsRunningAsCFR21IdentityUser)
                {
#if !NET_STANDARD
                    if (historianLogger != null)
                    {
                        historianLogger.Dispose();
                        historianLogger.RecyclingData -= historianLogger_RecyclingData;
                        historianLogger.FlushedData -= historianLogger_FlushedData;
                        historianLogger.FlushedDataSafely -= historianLogger_FlushedDataSafely;
                        historianLogger.ErrorFlushingData -= historianLogger_ErrorFlushingData;
                        historianLogger.StatisticDataChanged -= historianLogger_StatisticDataChanged;
                        historianLogger = null;
                    }

                    if (!isHistoricalLoggerDisabled)
                    {
                        isHistoricalLoggerDisabled = true;
                        var message = string.Format(Properties.Resources.RunningAsInvalidIdentityUser, UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting());
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, message,
                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        server.OnBalloonEvent(message, System.Windows.Forms.ToolTipIcon.Error);
                    }
                    return;
#else
                    if (!isHistorianDataProtectionNotSupported)
                    {
                        isHistorianDataProtectionNotSupported = true;
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, Properties.Resources.HistorianDataProtectionNotSupported,
                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }
#endif
                }

                if (historianLogger != null || isHistoricalLoggerDisabled)
                    return;

                historianLogger = new UFUAHistorianLogger(new UFUAHistorianConfiguration()
                {
                    DefaultSettings = server.UFUAConfiguration.HistorianDefaultConnection,
                    SafelySettings = server.SafeDataConnectionString,
                    RedundancyServerId =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.ArrayPosition :
#endif
                        -1,
                    RedundancyHistoryThreadPool =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.HistoryThreadPool :
#endif
                        0,
                    RedundancyMaxSyncEntities = server.UFUAConfiguration.RedundancyMaxSyncEntities.Value,
                    MaxConcurrentAccess = server.UFUAConfiguration.MaxConcurrentHistoricalAccess.Value,
                    ErrorTimeInterval = TimeSpan.FromSeconds(server.UFUAConfiguration.WaitHistoryRetryTime.Value),
                    MaxErrorBeforeFlush = server.UFUAConfiguration.MaxHistoryErrorBeforeFlush.Value,
                    MaxErrorCacheSize = server.UFUAConfiguration.MaxHistoryErrorCacheSize.Value,
                    MaxDeletingEntities = server.UFUAConfiguration.MaxHistoryDeletingEntities.Value,
                    MinPendingEntities = server.UFUAConfiguration.MinHistoryPendingEntities.Value,
                    MaxPendingEntities = server.UFUAConfiguration.MaxHistoryPendingEntities.Value,
                    MaxDeleteProcess = server.UFUAConfiguration.MaxHistoryDeleteProcess.Value,
                    MaxRestoreProcess = server.UFUAConfiguration.MaxHistoryRestoreProcess.Value,
                    MaxTotalSafelyFilesSize = server.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize.Value
                });

                historianLogger.RecyclingData += historianLogger_RecyclingData;
                historianLogger.FlushedData += historianLogger_FlushedData;
                historianLogger.FlushedDataSafely += historianLogger_FlushedDataSafely;
                historianLogger.ErrorFlushingData += historianLogger_ErrorFlushingData;
                historianLogger.StatisticDataChanged += historianLogger_StatisticDataChanged;
            }
        }

        void InitDataLogger(bool checkIdentityUser = false)
        {
            lock (lockLogger)
            {
                if (checkIdentityUser && !server.IsRunningAsCFR21IdentityUser)
                {
#if !NET_STANDARD
                    if (dataLogger != null)
                    {
                        dataLogger.Dispose();
                        dataLogger.FlushedDataSafely -= dataLogger_FlushedDataSafely;
                        dataLogger.ErrorStateChanged -= dataLogger_ErrorStateChanged;
                        dataLogger = null;
                    }

                    if (!isHistoricalLoggerDisabled)
                    {
                        isHistoricalLoggerDisabled = true;
                        var message = string.Format(Properties.Resources.RunningAsInvalidIdentityUser, UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting());
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, message,
                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        server.OnBalloonEvent(message, System.Windows.Forms.ToolTipIcon.Error);
                    }
                    return;
#else
                    if (!isDataLoggerDataProtectionNotSupported)
                    {
                        isDataLoggerDataProtectionNotSupported = true;
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource, Properties.Resources.DataLoggerDataProtectionNotSupported,
                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                    }
#endif
                }

                if (dataLogger != null || isHistoricalLoggerDisabled)
                    return;

                dataLogger = new DataLoggerManager.DataLogger(new DataLoggerManager.DataLoggerConfiguration()
                {
                    xpoDataConnectionString = server.UFUAConfiguration.HistorianDefaultConnection,
                    xpoSafeDataConnectionString = server.SafeDataConnectionString,
                    projectRootFolder = server.ProjectRootFolder,
                    redundancyServerId =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.ArrayPosition :
#endif
                        -1,
                    redundancyHistoryThreadPool =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.ActiveServerManager.HistoryThreadPool :
#endif
                        -1,
                    redundancyMaxSyncRecords =
#if !NET_STANDARD
                        server.IsRedundancyEnabled ? server.UFUAConfiguration.RedundancyMaxSyncEntities.Value :
#endif
                        -1,
                    MaxRestoreProcess = server.UFUAConfiguration.MaxHistoryRestoreProcess.Value,
                    MaxTotalSafelyFilesSize = server.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize.Value
                });

                dataLogger.FlushedDataSafely += dataLogger_FlushedDataSafely;
                dataLogger.ErrorStateChanged += dataLogger_ErrorStateChanged;
            }
        }

        private void CreateTagHistorian(String name, NodeId nodeid, NodeState nodeState,
                                    UFUAModel.UFUAHistorianSettings historianSettings, 
                                    bool bOnlyDefinition = false)
        {
            var baseVariable = nodeState as BaseVariableState;
            var analogVariable = nodeState as AnalogItemState;

            CreateHistorian(historianSettings, nodeState as BaseDataVariableState);

            if (!bOnlyDefinition)
            {
                uint dimension = 0;
                if (baseVariable.ValueRank != ValueRanks.Scalar && baseVariable.ArrayDimensions != null)
                    dimension = baseVariable.ArrayDimensions[0];

                var connectionString = server.UFUAConfiguration.HistorianDefaultConnection;
                if (!String.IsNullOrWhiteSpace(historianSettings.ConnectionSettings))
                {
                    connectionString = XpoHelper.NormalizeConnectionString(historianSettings.ConnectionSettings, server.ProjectRootFolder);
                    if (server.IsRunningAsCFR21IdentityUser)
                        connectionString = XpoHelper.EnsureTrustedConnectionStrings(connectionString);
                }

                var historianEntry = new UFUAHistorianLogEntity()
                {
                    NodeId = nodeid.ToString(),
                    Name = name,
                    Description = baseVariable.Description != null ? baseVariable.Description.Text : null,
                    HistoricalName = historianSettings.Name,
                    MaxAge = historianSettings.MaxAge.Value,
                    ExceptionDeviation = historianSettings.ExceptionDeviation,
                    HistoricalConnection = connectionString,
                    DataType = baseVariable.DataType,
                    ExceptionDeviationFormat = (ExceptionDeviationFormat)historianSettings.ExceptionDeviationFormat,
                    range = analogVariable != null && analogVariable.EURange != null ? analogVariable.EURange.Value : null,
                    instrumentRange = analogVariable != null && analogVariable.InstrumentRange != null ? analogVariable.InstrumentRange.Value : null,
                    MinTimeInterval = historianSettings.MinTimeInterval,
                    MaxTimeInterval = historianSettings.MaxTimeInterval,
                    ErrorTimeInterval = TimeSpan.FromSeconds(historianSettings.WaitRetryTime.Value),
                    MaxErrorBeforeFlush = historianSettings.MaxErrorBeforeFlush.Value,
                    MaxErrorCacheSize = historianSettings.MaxErrorCacheSize.Value,
                    Stepped = historianSettings.Stepped,
                    OnlyGood = historianSettings.RecordOnlyOnQualityGood,
                    Enabled = historianSettings.Enabled.Value,
                    EnableDataProtection = historianSettings.EnableDataProtection,
                    ArrayDimension = dimension,
                    // runtime members
                    value = new DataValue() 
                    { 
                        Value = baseVariable.Value, 
                        StatusCode = baseVariable.StatusCode,
                        SourceTimestamp = baseVariable.Timestamp, 
                        ServerTimestamp = DateTime.UtcNow,
                    },
                    valueBefore = new DataValue()
                    {
                        Value = baseVariable.Value,
                        StatusCode = baseVariable.StatusCode,
                        SourceTimestamp = baseVariable.Timestamp,
                        ServerTimestamp = DateTime.UtcNow,
                    }
                };

                historianEntry.resolvedValue = ResolveValue(nodeState, historianEntry.value);
                historianEntry.resolvedValueBefore = ResolveValue(nodeState, historianEntry.valueBefore);

                if (!String.IsNullOrWhiteSpace(historianSettings.TableName))
                    historianEntry.HistoricalConnection = UFUAHistorianModel.Helpers.HistorianHelper.SetTableName<UFUAAuditDataItem>(historianEntry.HistoricalConnection, historianSettings.TableName);

                lock (Lock)
                {
                    mapNodeIdToHistorianLogEntities.Add(nodeid, historianEntry);
                    historianLogger.Init(historianEntry);
                    AssignNodeToAlwaysInUseList(nodeid);
#if !NET_STANDARD
                    if (server.IsRedundancyEnabled)
                    {
                        if (!String.IsNullOrWhiteSpace(historianSettings.ConnectionSettings) || !String.IsNullOrWhiteSpace(historianSettings.TableName))
                        {
                            var defaultSettings = server.UFUAConfiguration.HistorianDefaultConnection;
                            if (defaultSettings != null)
                                defaultSettings = XpoHelpers.XpoHelper.NormalizeConnectionString(defaultSettings);
                            String settings = null;
                            if (String.IsNullOrWhiteSpace(historianSettings.ConnectionSettings))
                                settings = defaultSettings;
                            else if (!String.IsNullOrWhiteSpace(historianSettings.ConnectionSettings))
                                settings = XpoHelpers.XpoHelper.NormalizeConnectionString(XpoHelper.NormalizeConnectionString(historianSettings.ConnectionSettings, server.ProjectRootFolder));
                            if (settings != null && !String.IsNullOrWhiteSpace(historianSettings.TableName))
                                settings = UFUAHistorianModel.Helpers.HistorianHelper.SetTableName<UFUAAuditDataItem>(settings, historianSettings.TableName);
                            if (settings != null && settings != defaultSettings)
                            {
                                if (server.ActiveServerManager.MapHistorianCustomConnection == null)
                                    server.ActiveServerManager.MapHistorianCustomConnection = new Dictionary<NodeId, String>();
                                var key = new NodeId(String.IsNullOrWhiteSpace(historianSettings.TableName) ? historianEntry.HistoricalName.ToLower() : historianSettings.TableName.ToLower(), NamespaceIndex);
                                if (!server.ActiveServerManager.MapHistorianCustomConnection.ContainsKey(key) &&
                                    !server.ActiveServerManager.MapHistorianCustomConnection.ContainsValue(settings))
                                    server.ActiveServerManager.MapHistorianCustomConnection.Add(key, settings);
                            }
                        }
                    }
#endif

                    if (historianSettings.EnableTag != null && !historianSettings.EnableTag.IsEmpty())
                    {
                        bool bInvalid = false;
                        if (mapNodeIdToNodeState.ContainsKey(historianSettings.EnableTag.ResolveNodeId(NamespaceIndex)))
                        {
                            var nodestate = mapNodeIdToNodeState[historianSettings.EnableTag.ResolveNodeId(NamespaceIndex)];
                            if (IsConvertibleToDouble(nodestate))
                            {
                                if (!mapEnabledHistorianLogEntities.ContainsKey(nodestate.NodeId))
                                {
                                    mapEnabledHistorianLogEntities.Add(nodestate.NodeId, new List<String>());
                                    nodestate.StateChanged += OnEnabledNodeStateChangedHistorian;
                                }
                                mapEnabledHistorianLogEntities[nodestate.NodeId].Add(historianEntry.NodeId);
                                nodestate.UpdateChangeMasks(NodeStateChangeMasks.Value);
                                updatedChangeMasksNodeStates.Add(nodestate);
                            }
                            else
                                bInvalid = true;
                        }
                        else
                            bInvalid = true;

                        if (bInvalid)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                String.Format(Properties.Resources.HistorianInvalidEnabledTag, historianSettings.Name, historianSettings.EnableTag),
                                System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                        }
                    }
                }
            }
        }

        private void CreateHistorianAudit(NodeState nodeState, TimeSpan maxAge)
        {
            var baseVariable = nodeState as BaseVariableState;

            uint dimension = 0;
            if (baseVariable.ValueRank != ValueRanks.Scalar && baseVariable.ArrayDimensions != null)
                dimension = baseVariable.ArrayDimensions[0];

            var auditConnectionString = server.UFUAConfiguration.AuditTraceDefaultConnection;
            if (string.IsNullOrEmpty(auditConnectionString))
                auditConnectionString = server.UFUAConfiguration.HistorianDefaultConnection;
            var historianEntry = new UFUAHistorianLogEntity()
            {
                NodeId = new NodeId(String.Format("{0}#{1}", nodeState.NodeId.Identifier, UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix()), NamespaceIndex).ToString(),
                Name = ModelUtils.ConstructNameForComponent(nodeState),
                Description = baseVariable.Description != null ? baseVariable.Description.Text : null,
                HistoricalName = String.Format("{0}#{1}", server.UFUAConfiguration.ApplicationName, UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix()),
                MaxAge = maxAge,
                HistoricalConnection = auditConnectionString,
                DataType = baseVariable.DataType,
                ExceptionDeviationFormat = ExceptionDeviationFormat.AbsoluteValue,
                ErrorTimeInterval = TimeSpan.FromSeconds(UFUAModel.UFUAHistorianSettings.defaultWaitRetryTime),
                MaxErrorBeforeFlush = UFUAModel.UFUAHistorianSettings.defaultMaxErrorBeforeFlush,
                MaxErrorCacheSize = UFUAModel.UFUAHistorianSettings.defaultMaxErrorCacheSize,
                Enabled = true,
                EnableDataProtection = server.UFUAConfiguration.EnableEventDataProtection,
                ArrayDimension = dimension,
                // runtime members
                value = new DataValue()
                {
                    Value = baseVariable.Value,
                    StatusCode = baseVariable.StatusCode,
                    SourceTimestamp = baseVariable.Timestamp,
                    ServerTimestamp = DateTime.UtcNow,
                },
                valueBefore = new DataValue()
                {
                    Value = baseVariable.Value,
                    StatusCode = baseVariable.StatusCode,
                    SourceTimestamp = baseVariable.Timestamp,
                    ServerTimestamp = DateTime.UtcNow,
                }
            };

            historianEntry.resolvedValue = ResolveValue(nodeState, historianEntry.value);
            historianEntry.resolvedValueBefore = ResolveValue(nodeState, historianEntry.valueBefore);

#if !NET_STANDARD
            if (server.IsRedundancyEnabled)
            {
                if (!String.IsNullOrWhiteSpace(server.UFUAConfiguration.AuditTraceDefaultConnection))
                {
                    var settings = XpoHelpers.XpoHelper.NormalizeConnectionString(server.UFUAConfiguration.AuditTraceDefaultConnection);
                    var defaultSettings = server.UFUAConfiguration.HistorianDefaultConnection;
                    if (defaultSettings != null)
                        defaultSettings = XpoHelpers.XpoHelper.NormalizeConnectionString(defaultSettings);
                    if (settings != defaultSettings)
                    {
                        if (server.ActiveServerManager.MapHistorianCustomConnection == null)
                            server.ActiveServerManager.MapHistorianCustomConnection = new Dictionary<NodeId, String>();
                        var key = new NodeId(historianEntry.HistoricalName.ToLower(), NamespaceIndex);
                        if (!server.ActiveServerManager.MapHistorianCustomConnection.ContainsKey(key) &&
                            !server.ActiveServerManager.MapHistorianCustomConnection.ContainsValue(settings))
                            server.ActiveServerManager.MapHistorianCustomConnection.Add(key, settings);
                    }
                }
            }
#endif

            lock (Lock)
            {
                mapNodeIdToAuditLogEntities.Add(nodeState.NodeId, historianEntry);
                historianLogger.Init(historianEntry);
                AssignNodeToAlwaysInUseList(nodeState.NodeId);
            }
        }

#if !NET_STANDARD
        bool lastOverFlowState;
        Queue<ChangedTagArgs> RedundancyPendingChangedTags;
        Thread RedundancyChangedTagsThread;
        ManualResetEvent RedundancyChangedTagsEvent;
        void RedundancyChangedTag(ChangedTagArgs e)
        {
            lock (lockThreads)
            {
                if (RedundancyChangedTagsThread == null)
                {
                    RedundancyPendingChangedTags = new Queue<ChangedTagArgs>();
                    RedundancyChangedTagsEvent = new ManualResetEvent(false);
                    RedundancyChangedTagsThread = new Thread((o) =>
                    {
                        while (!ExitMode)
                        {
                            if (!ExitMode)
                            {
                                RedundancyChangedTagsEvent.WaitOne();
                            }
                            List<ChangedTagArgs> list;
                            lock (lockThreads)
                            {
                                //Console.WriteLine(String.Format("RedundancyPendingChangedTags = {0}", RedundancyPendingChangedTags.Count));
                                list = RedundancyPendingChangedTags.ToList();
                                RedundancyPendingChangedTags.Clear();
                                RedundancyChangedTagsEvent.Reset();
                            }

                            bool bOverflow = false;
                            var changedTags = new Dictionary<NodeId, ChangedTags>();
                            list.ForEach((item) =>
                            {
                                int removed = 0;
                                int taked = Math.Max(1, server.UFUAConfiguration.RedundancyMaxPendingTransmitMessages.Value);
                                int skiped = Math.Max(0, item.DataValues.Count - taked);
                                
                                if (!changedTags.ContainsKey(item.NodeId))
                                {
                                    changedTags.Add(item.NodeId, new ChangedTags() { NodeId = item.NodeId.ToString(), DataValues = new WrappedDataValueCollection(item.DataValues.Skip(skiped).Take(taked)) });
                                }
                                else
                                {
                                    changedTags[item.NodeId].DataValues.AddRange(item.DataValues);
                                    removed = Math.Max(0, changedTags[item.NodeId].DataValues.Count - server.UFUAConfiguration.RedundancyMaxPendingTransmitMessages.Value);
                                    changedTags[item.NodeId].DataValues.RemoveRange(0, removed);
                                }

                                if (mapNodeIdToTagLogEntities.ContainsKey(item.NodeId) &&
                                    mapNodeIdToTagLogEntities[item.NodeId].enableStatistics)
                                {
                                    var entry = mapNodeIdToTagLogEntities[item.NodeId];
                                    if (changedTags[item.NodeId].Statistics == null)
                                        changedTags[item.NodeId].Statistics = new StatisticsData();
                                    changedTags[item.NodeId].Statistics.Min = entry.min;
                                    changedTags[item.NodeId].Statistics.Max = entry.max;
                                    changedTags[item.NodeId].Statistics.TotAverage = entry.totAverage;
                                    changedTags[item.NodeId].Statistics.CountUpdates = entry.countUpdates;
                                    changedTags[item.NodeId].Statistics.TotalTimeOn = entry.totalTimeOn;
                                    changedTags[item.NodeId].Statistics.LastTotalTimeOn = entry.lastTotalTimeOn;
                                }

                                if (skiped > 0 || removed > 0)
                                    bOverflow = true;
                            });

                            if (bOverflow && !lastOverFlowState)
                            {
                                string msg = String.Format(Properties.Resources.RedundancyTooManyChangeValuesToSend, 
                                    server.UFUAConfiguration.RedundancyMaxPendingTransmitMessages.Value);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    msg,System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Redundancy);
                            }
                            lastOverFlowState = bOverflow;

                            try
                            {
                                server.ActiveServerManager.SendChangedTags(changedTags.Values.ToList());
                            }
                            catch (Exception ex)
                            { }
                        }
                    });
                    RedundancyChangedTagsThread.Start();
                }

                RedundancyPendingChangedTags.Enqueue(e);
                Debug.WriteLine("RedundancyPendingChangedTags.Count = {0}", RedundancyPendingChangedTags.Count);
                RedundancyChangedTagsEvent.Set();
            }
        }
#endif

        Queue<ChangedTagArgs> pendingChangedTags;
        Thread ChangedTagsThread;
        ManualResetEvent ChangedTagsEvent;
        bool ExitMode;
        UFUATagLogger.UFUATagLogger tagLogger;
        DataLoggerManager.DataLogger dataLogger;
        internal UFUAHistorianLogger historianLogger;
        protected UFUAEventLogger eventLogger;

        void ExitPendingThreads()
        {
            ExitMode = true;
            Thread changedTagsThread = ChangedTagsThread;
            Thread inUseTagsThread = InUseTagsThread;
#if !NET_STANDARD
            Thread redundancyChangedTagsThread = RedundancyChangedTagsThread;
#endif
            lock (lockThreads)
            {
                if (ChangedTagsEvent != null)
                    ChangedTagsEvent.Set();
                if (InUseTagsEvent != null)
                    InUseTagsEvent.Set();
#if !NET_STANDARD
                if (RedundancyChangedTagsEvent != null)
                    RedundancyChangedTagsEvent.Set();
#endif
            }

            if (changedTagsThread != null)
                changedTagsThread.Join();
            if (inUseTagsThread != null)
                inUseTagsThread.Join();
#if !NET_STANDARD
            if (redundancyChangedTagsThread != null)
                redundancyChangedTagsThread.Join();
#endif
        }

        void historianLogger_RecyclingData(object sender, RecyclingDataArgs e)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithName, e.nodeId, e.name);
            var eventName = String.Format(Properties.Resources.RecyclingDataEventLog, e.count);
            RaiseSystemEvents(e.nodeId, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);
        }

        void historianLogger_FlushedData(object sender, FlushedDataArgs e)
        {
            bool isInError = false;
            lock (recordingInErrorConnections)
            {
                var key = String.Format("{0}{1}", historianFaultFormat, e.connection);
                if (recordingInErrorConnections.Contains(key))
                    recordingInErrorConnections.Remove(key);
                isInError = recordingInErrorConnections.Count > 0;
            }

            var eventName = String.Format(Properties.Resources.FlushedDataEventLog, e.count);
            RaiseSystemEvents(null, "Historian", eventName, EventSeverity.Low, DateTime.UtcNow);

            if (systemTags != null && systemTags.RecordingInError.Value != isInError)
            {
                systemTags.RecordingInError.Value = isInError;
                AssignNodeToUpdateList(systemTags.RecordingInError);

                if (isInError)
                    server.OnAlertEvent(Properties.Resources.RecordingDataInError.Replace("--newline--", Environment.NewLine));
            }
        }

        void historianLogger_FlushedDataSafely(object sender, FlushedDataSafelyArgs e)
        {
            var sourceName = String.Format(Properties.Resources.SourceNameLog, e.Name);
            string eventName;
            if (e.Result)
                eventName = String.Format(Properties.Resources.FlushedDataSafelyOK, e.Connection, e.Count);
            else
                eventName = String.Format(Properties.Resources.FlushedDataSafelyError, e.Connection, e.Count);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.High, DateTime.UtcNow);
        }

        void historianLogger_ErrorFlushingData(object sender, ErrorFlushingDataArgs e)
        {
            bool isInError = false;
            bool isEnteringInError = false;
            lock (recordingInErrorConnections)
            {
                var key = String.Format("{0}{1}", historianFaultFormat, e.connection);
                if (!recordingInErrorConnections.Contains(key))
                {
                    recordingInErrorConnections.Add(key);
                    isEnteringInError = true;
                }
                isInError = recordingInErrorConnections.Count > 0;
            }

            var message = String.Format(Properties.Resources.ErrorFlushingData, XpoHelper.GetConnectionStringWithoutPassword(e.connection), e.exception);
            RaiseSystemEvents(null, "Historian", message, EventSeverity.High, DateTime.UtcNow, null, isEnteringInError);

            if (systemTags != null && systemTags.RecordingInError.Value != isInError)
            {
                systemTags.RecordingInError.Value = isInError;
                AssignNodeToUpdateList(systemTags.RecordingInError);

                if (isInError)
                    server.OnAlertEvent(Properties.Resources.RecordingDataInError.Replace("--newline--", Environment.NewLine));
            }

            if (isEnteringInError)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                server.OnBalloonEvent(Properties.Resources.HistorianDataErrorOccured, System.Windows.Forms.ToolTipIcon.Error);
            }
        }

        void historianLogger_StatisticDataChanged(object sender, StatisticDataArgs e)
        {
            if (systemTags == null)
                return;

            if (e.oldData.RecordEntriesPending != e.newData.RecordEntriesPending)
            {
                systemTags.RecordHistoricalEntriesPending.Value = e.newData.RecordEntriesPending;
                systemTags.RecordHistoricalEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.RecordHistoricalEntriesPending);
            }
            if (e.oldData.RecordEntriesRunning != e.newData.RecordEntriesRunning)
            {
                systemTags.RecordHistoricalEntriesRunning.Value = e.newData.RecordEntriesRunning;
                systemTags.RecordHistoricalEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.RecordHistoricalEntriesRunning);
            }
            if (e.oldData.FailsEntriesPending != e.newData.FailsEntriesPending)
            {
                systemTags.FailsHistoricalEntriesPending.Value = e.newData.FailsEntriesPending;
                systemTags.FailsHistoricalEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FailsHistoricalEntriesPending);
            }
            if (e.oldData.FailsEntriesRunning != e.newData.FailsEntriesRunning)
            {
                systemTags.FailsHistoricalEntriesRunning.Value = e.newData.FailsEntriesRunning;
                systemTags.FailsHistoricalEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FailsHistoricalEntriesRunning);
            }
            if (e.oldData.DeleteEntriesPending != e.newData.DeleteEntriesPending)
            {
                systemTags.DeleteHistoricalEntriesPending.Value = e.newData.DeleteEntriesPending;
                systemTags.DeleteHistoricalEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DeleteHistoricalEntriesPending);
            }
            if (e.oldData.DeleteEntriesRunning != e.newData.DeleteEntriesRunning)
            {
                systemTags.DeleteHistoricalEntriesRunning.Value = e.newData.DeleteEntriesRunning;
                systemTags.DeleteHistoricalEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DeleteHistoricalEntriesRunning);
            }
            if (e.oldData.FlushEntriesPending != e.newData.FlushEntriesPending)
            {
                systemTags.FlushHistoricalEntriesPending.Value = e.newData.FlushEntriesPending;
                systemTags.FlushHistoricalEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FlushHistoricalEntriesPending);
            }
            if (e.oldData.FlushEntriesRunning != e.newData.FlushEntriesRunning)
            {
                systemTags.FlushHistoricalEntriesRunning.Value = e.newData.FlushEntriesRunning;
                systemTags.FlushHistoricalEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FlushHistoricalEntriesRunning);
            }
            if (e.oldData.DischargingEntriesMode != e.newData.DischargingEntriesMode)
            {
                systemTags.DischargingHistoricalEntriesMode.Value = e.newData.DischargingEntriesMode;
                systemTags.DischargingHistoricalEntriesMode.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DischargingHistoricalEntriesMode);
            }
        }

        void eventLogger_RecyclingEvent(object sender, RecyclingDataArgs e)
        {
            var sourceName = String.Format(Properties.Resources.SourceNodeLogWithName, e.nodeId, e.name);
            var eventName = String.Format(Properties.Resources.RecyclingDataEventLog, e.count);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.Low, DateTime.UtcNow);
        }

        void eventLogger_FlushedEvent(object sender, FlushedDataArgs e)
        {
            bool isInError = false;
            lock (recordingInErrorConnections)
            {
                var key = String.Format("{0}{1}", eventLoggerFaultFormat, e.connection);
                if (recordingInErrorConnections.Contains(key))
                    recordingInErrorConnections.Remove(key);
                isInError = recordingInErrorConnections.Count > 0;
            }

            var eventName = String.Format(Properties.Resources.FlushedDataEventLog, e.count);
            RaiseSystemEvents(null, "EventLogger", eventName, EventSeverity.Low, DateTime.UtcNow);

            if (systemTags != null && systemTags.RecordingInError.Value != isInError)
            {
                systemTags.RecordingInError.Value = isInError;
                AssignNodeToUpdateList(systemTags.RecordingInError);

                if (isInError)
                    server.OnAlertEvent(Properties.Resources.RecordingDataInError.Replace("--newline--", Environment.NewLine));
            }
        }

        void eventLogger_FlushedDataSafely(object sender, FlushedDataSafelyArgs e)
        {
            var sourceName = String.Format(Properties.Resources.SourceNameLog, e.Name);
            string eventName;
            if (e.Result)
                eventName = String.Format(Properties.Resources.FlushedDataSafelyOK, e.Connection, e.Count);
            else
                eventName = String.Format(Properties.Resources.FlushedDataSafelyError, e.Connection, e.Count);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.High, DateTime.UtcNow);
        }

        void eventLogger_ErrorFlushingData(object sender, ErrorFlushingDataArgs e)
        {
            bool isInError = false;
            bool isEnteringInError = false;
            lock (recordingInErrorConnections)
            {
                var key = String.Format("{0}{1}", eventLoggerFaultFormat, e.connection);
                if (!recordingInErrorConnections.Contains(key))
                {
                    recordingInErrorConnections.Add(key);
                    isEnteringInError = true;
                }
                isInError = recordingInErrorConnections.Count > 0;
            }

            var message = String.Format(Properties.Resources.ErrorFlushingData, e.connection, e.exception);
            RaiseSystemEvents(null, "EventLogger", message, EventSeverity.High, DateTime.UtcNow, null, isEnteringInError);

            if (systemTags != null && systemTags.RecordingInError.Value != isInError)
            {
                systemTags.RecordingInError.Value = isInError;
                AssignNodeToUpdateList(systemTags.RecordingInError);

                if (isInError)
                    server.OnAlertEvent(Properties.Resources.RecordingDataInError.Replace("--newline--", Environment.NewLine));
            }

            if (isEnteringInError)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, message, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                server.OnBalloonEvent(Properties.Resources.EventLogErrorOccured, System.Windows.Forms.ToolTipIcon.Error);
            }
        }

        void eventLogger_StatisticDataChanged(object sender, StatisticDataArgs e)
        {
            if (systemTags == null)
                return;

            if (e.oldData.RecordEntriesPending != e.newData.RecordEntriesPending)
            {
                systemTags.RecordEventEntriesPending.Value = e.newData.RecordEntriesPending;
                systemTags.RecordEventEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.RecordEventEntriesPending);
            }
            if (e.oldData.RecordEntriesRunning != e.newData.RecordEntriesRunning)
            {
                systemTags.RecordEventEntriesRunning.Value = e.newData.RecordEntriesRunning;
                systemTags.RecordEventEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.RecordEventEntriesRunning);
            }
            if (e.oldData.FailsEntriesPending != e.newData.FailsEntriesPending)
            {
                systemTags.FailsEventEntriesPending.Value = e.newData.FailsEntriesPending;
                systemTags.FailsEventEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FailsEventEntriesPending);
            }
            if (e.oldData.FailsEntriesRunning != e.newData.FailsEntriesRunning)
            {
                systemTags.FailsEventEntriesRunning.Value = e.newData.FailsEntriesRunning;
                systemTags.FailsEventEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FailsEventEntriesRunning);
            }
            if (e.oldData.DeleteEntriesPending != e.newData.DeleteEntriesPending)
            {
                systemTags.DeleteEventEntriesPending.Value = e.newData.DeleteEntriesPending;
                systemTags.DeleteEventEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DeleteEventEntriesPending);
            }
            if (e.oldData.DeleteEntriesRunning != e.newData.DeleteEntriesRunning)
            {
                systemTags.DeleteEventEntriesRunning.Value = e.newData.DeleteEntriesRunning;
                systemTags.DeleteEventEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DeleteEventEntriesRunning);
            }
            if (e.oldData.FlushEntriesPending != e.newData.FlushEntriesPending)
            {
                systemTags.FlushEventEntriesPending.Value = e.newData.FlushEntriesPending;
                systemTags.FlushEventEntriesPending.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FlushEventEntriesPending);
            }
            if (e.oldData.FlushEntriesRunning != e.newData.FlushEntriesRunning)
            {
                systemTags.FlushEventEntriesRunning.Value = e.newData.FlushEntriesRunning;
                systemTags.FlushEventEntriesRunning.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.FlushEventEntriesRunning);
            }
            if (e.oldData.DischargingEntriesMode != e.newData.DischargingEntriesMode)
            {
                systemTags.DischargingEventEntriesMode.Value = e.newData.DischargingEntriesMode;
                systemTags.DischargingEventEntriesMode.Timestamp = DateTime.UtcNow;
                AssignNodeToUpdateList(systemTags.DischargingEventEntriesMode);
            }
        }

        void dataLogger_FlushedDataSafely(object sender, DataLoggerManager.FlushedDataSafelyEventArgs e)
        {
            var sourceName = String.Format(Properties.Resources.SourceNameLog, e.DataLoggerName);
            string eventName;
            if (String.IsNullOrEmpty(e.ErrorMessage))
                eventName = String.Format(Properties.Resources.FlushedDataSafelyOK, e.FilePath, e.RecordsCounter);
            else
                eventName = String.Format(Properties.Resources.FlushedDataSafelyOK, e.FilePath, e.RecordsCounter);
            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.High, DateTime.UtcNow);
        }

        void dataLogger_ErrorStateChanged(object sender, DataLoggerManager.ErrorStateChangedEventArgs e)
        {
            bool isInError = false;
            bool isEnteringInError = false;
            lock (recordingInErrorConnections)
            {
                var key = String.Format("{0}{1}", dataLoggerFaultFormat, e.DataLoggerName);
                if (e.bErrorState && !recordingInErrorConnections.Contains(key))
                {
                    recordingInErrorConnections.Add(key);
                    isEnteringInError = true;
                }
                else if (!e.bErrorState && recordingInErrorConnections.Contains(key))
                    recordingInErrorConnections.Remove(key);
                isInError = recordingInErrorConnections.Count > 0;
            }

            var sourceName = String.Format(Properties.Resources.SourceNameLog, e.DataLoggerName);
            RaiseSystemEvents(null, sourceName, e.ErrorMessage, e.bErrorState ? EventSeverity.High : EventSeverity.Low, DateTime.UtcNow, null, isEnteringInError);

            if (systemTags != null && systemTags.RecordingInError.Value != isInError)
            {
                systemTags.RecordingInError.Value = isInError;
                AssignNodeToUpdateList(systemTags.RecordingInError);

                if (isInError)
                    server.OnAlertEvent(Properties.Resources.RecordingDataInError.Replace("--newline--", Environment.NewLine));
            }

            if (isEnteringInError)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ErrorMessage, System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                server.OnBalloonEvent(Properties.Resources.DataLoggerErrorOccured, System.Windows.Forms.ToolTipIcon.Error);
            }
        }
        
        void UANodeManager_TagPrototypeQuery(object sender, TagPrototypeArgs e)
        {
            lock (Lock)
            {
                e.listTags = new List<TagDefinition>();
                NodeId protonode = e.sourceNode;
                List<TagDefinition> lastlista = new List<TagDefinition>();
                
                var baseType = mapNodeIdToNodeState[protonode];
                NodeId prototypeId = NodeId.Null;
                
                if(baseType is BaseObjectState)
                {
                    prototypeId = (baseType as BaseObjectState).TypeDefinitionId;
                }

                List<BaseInstanceState> children = new List<BaseInstanceState>();
                baseType.GetChildren(SystemContext, children);

                lastlista = GetFlatTagDefinitionList(e.driverName, children, prototypeId);

                for (int i = 0; i < lastlista.Count; i++)
                    e.listTags.Add(lastlista[i]);
            }
        }

        List<TagDefinition> GetFlatTagDefinitionList(String driverName, List<BaseInstanceState> children, NodeId prototypeId, String structname = null)
        {
            List<TagDefinition> newlist = new List<TagDefinition>();
            List<TagDefinition> lista = GetFirstOrderSortedList(driverName, children, prototypeId);
            int addorder = 0;
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].DataType.IdType == IdType.Guid)
                {

                    BaseInstanceState child = children.Find((o) => { return o.NodeId == lista[i].NodeId; });
                    if (child != null)
                    {
                        List<BaseInstanceState> subchildren = new List<BaseInstanceState>();
                        child.GetChildren(SystemContext, subchildren);

                        var templista = GetFlatTagDefinitionList(driverName, subchildren, child.TypeDefinitionId, child.BrowseName.Name);
                        for (int j = 0; j < (templista.Count); j++)
                        {
                            TagDefinition td = templista[j];
                            if (structname != null)
                                td.Name = string.Format("{0}:{1}/{2}", td.NodeId.NamespaceIndex, structname, td.Name);
                            td.MemberOrder += lista[i].MemberOrder + addorder;
                            newlist.Add(td);
                        }
                        addorder += /*lista[i].MemberOrder + */templista[templista.Count - 1].MemberOrder;
                    }
                }
                else
                {
                    TagDefinition td = lista[i];
                    if (structname != null)
                        td.Name = string.Format("{0}:{1}/{2}", td.NodeId.NamespaceIndex, structname, td.Name);
                    td.MemberOrder += addorder;
                    newlist.Add(td);
                }
            }
            return newlist;
        }

        static int CompareMemberOrder(TagDefinition x, TagDefinition y)
        {
            if (x.MemberOrder == y.MemberOrder)
                return 0;
            else if (x.MemberOrder > y.MemberOrder)
                return 1;
            else
                return -1;
        }

        List<TagDefinition> GetFirstOrderSortedList(String driverName, List<BaseInstanceState> children, NodeId prototypeId, int offset = 0, String foldername = null, String searchname = null)
        {
            List<TagDefinition> lista = new List<TagDefinition>();

            if (children.Count == 0)
                return lista;

            Dictionary<int, BaseInstanceState> orderedChildren = new Dictionary<int, BaseInstanceState>();
            List<int> orderedKeys = new List<int>();
            for (int ii = 0; ii < children.Count; ii++)
            {
                BaseInstanceState child = children[ii];
                string node = child.BrowseName.Name;
                if (searchname != null)
                    node = string.Format("{0}/{1}", searchname, node);
                int o = -1;
                if (mapStructToMemberOder.ContainsKey(prototypeId) && mapStructToMemberOder[prototypeId].ContainsKey(node))
                    o = mapStructToMemberOder[prototypeId][node];
                if (o > -1 && !orderedChildren.ContainsKey(o))
                    orderedChildren.Add(o, child);
                orderedKeys = orderedChildren.Keys.ToList();
                orderedKeys.Sort();
            }
            for (int ii = 0; ii < orderedKeys.Count; ii++)
            {
                BaseInstanceState child = orderedChildren[orderedKeys[ii]];
                String dynsettings = String.Empty;
                if (mapNodeIdToDynamicSettings.ContainsKey(child.NodeId))
                {
                    var listValidDynSettings = new List<String>();
                    var drvName = String.Format("{0}.", driverName.ToLower());
                    var dynamics = mapNodeIdToDynamicSettings[child.NodeId].Split(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator());
                    foreach (var settings in dynamics)
                    {
                        if (settings.ToLower().StartsWith(drvName))
                            listValidDynSettings.Add(settings);
                    }

                    if (listValidDynSettings.Count > 0)
                        dynsettings = String.Join(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator().ToString(), listValidDynSettings);
                }                

                if (child is BaseVariableState)
                {
                    var td = new TagDefinition()
                    {
                        NodeId = child.NodeId,
                        DynamicSettings = dynsettings,
                        DataType = (child as BaseVariableState).DataType,
                        ArrayDimension = (uint)((child as BaseVariableState).ArrayDimensions != null &&
                        (child as BaseVariableState).ArrayDimensions.Count > 0 ?
                        (child as BaseVariableState).ArrayDimensions[0] : 0),
                        MemberOrder = lista.Count + offset,
                        InitialValue = (child as BaseVariableState).Value,
                        Name = string.Format("{0}:{1}", child.BrowseName.NamespaceIndex, child.BrowseName.Name)
                    };
                    if (foldername != null)
                        td.Name = string.Format("{0}/{1}", foldername, td.Name);
                    lista.Add(td);
                    
                }
                else if (child is BaseObjectState)
                {
                    if ((child as BaseObjectState).TypeDefinitionId.IdType == IdType.Numeric && (uint)(child as BaseObjectState).TypeDefinitionId.Identifier == ObjectTypes.FolderType)
                    {
                        List<BaseInstanceState> subchildren = new List<BaseInstanceState>();
                        child.GetChildren(SystemContext, subchildren);
                        if (subchildren.Count > 0)
                        {
                            string fname = string.Format("{0}:{1}", child.BrowseName.NamespaceIndex, child.BrowseName.Name);
                            string sname = child.BrowseName.Name;
                            if (searchname != null)
                                sname = string.Format("{0}/{1}", searchname, sname);
                            if(foldername != null)
                                fname = string.Format("{0}/{1}", foldername, fname);
                            List<TagDefinition> templista = new List<TagDefinition>();
                            templista = GetFirstOrderSortedList(driverName, subchildren, prototypeId, lista.Count + offset, fname, sname);
                            lista.AddRange(templista);
                        }
                    }
                    else if (String.IsNullOrEmpty(dynsettings))
                    {
                        List<BaseInstanceState> subchildren = new List<BaseInstanceState>();
                        child.GetChildren(SystemContext, subchildren);
                        if (subchildren.Count > 0)
                        {
                            string fname = string.Format("{0}:{1}", child.BrowseName.NamespaceIndex, child.BrowseName.Name);
                            if (foldername != null)
                                fname = string.Format("{0}/{1}", foldername, fname);
                            List<TagDefinition> templista = new List<TagDefinition>();

                            templista = GetFirstOrderSortedList(driverName, subchildren, child.TypeDefinitionId, lista.Count + offset);
                            for (int k = 0; k < templista.Count; k++)
                            {
                                var td = new TagDefinition()
                                {
                                    NodeId = templista[k].NodeId,
                                    DynamicSettings = templista[k].DynamicSettings,
                                    DataType = templista[k].DataType,
                                    ArrayDimension = templista[k].ArrayDimension,
                                    MemberOrder = lista.Count + offset,
                                    InitialValue = templista[k].InitialValue,
                                    Name = templista[k].Name
                                };
                                if(!string.IsNullOrEmpty(fname))
                                    td.Name = string.Format("{0}/{1}", fname, td.Name);
                                lista.Add(td);
                            }
                        }
                    }
                }
                else if (child is MethodState)
                {
                    MethodState m = child as MethodState;
                    var td = new TagDefinition()
                    {
                        NodeId = child.NodeId,
                        DynamicSettings = dynsettings,
                        DataType = m.ReferenceTypeId,
                        MemberOrder = lista.Count + offset,
                        Name = string.Format("{0}:{1}", child.BrowseName.NamespaceIndex, child.BrowseName.Name)
                    };
                    if (foldername != null)
                        td.Name = string.Format("{0}/{1}", foldername, td.Name);
                    lista.Add(td);
                }
            }

            lista.Sort(CompareMemberOrder);
            return lista;
        }

        void UANodeManager_TagChanged(object sender, ChangedTagArgs e)
        {
            lock (lockThreads)
            {
                if (ChangedTagsThread == null)
                {
                    pendingChangedTags = new Queue<ChangedTagArgs>();
                    ChangedTagsEvent = new ManualResetEvent(false);
                    ChangedTagsThread = new Thread((o) =>
                        {
                            while (!ExitMode)
                            {
                                if (!ExitMode)
                                {
                                    ChangedTagsEvent.WaitOne();
                                }
                                List<ChangedTagArgs> list;
                                lock (lockThreads) 
                                {
                                    list = pendingChangedTags.ToList();
                                    pendingChangedTags.Clear();
                                    ChangedTagsEvent.Reset();
                                }

                                list.ForEach((item) =>
                                {
                                    NodeState baseObject = GetNodeStateFromNodeId(item.NodeId);
                                    if (baseObject != null)
                                    {
                                        item.DataValues.ForEach(value =>
                                            {
                                                //value notmalization must occurr...
                                                if (baseObject is BaseVariableState)
                                                {
                                                    var variable = baseObject as BaseVariableState;
                                                    if (item.DisableQualityUpdate == false)
                                                        variable.StatusCode = value.StatusCode;
                                                 

                                                    if (value.SourceTimestamp != DateTime.MinValue)
                                                        variable.Timestamp = value.SourceTimestamp;

                                                    if (value.Value != null || (value.Value ==null && item.AllowNullValues))
                                                    {
                                                        if (!String.IsNullOrEmpty(item.driverName))
                                                        {
                                                            var commDriver = GetCommunicationDriversFromNodeStateDictionary(baseObject);
                                                            if (commDriver != null && commDriver.Count > 0)
                                                            {
                                                                foreach (var driver in commDriver)
                                                                {
								    // force driver name to lowercase for back compatibility with old Movicon project
                                                                    if (driver.GetDriverName().ToLower() == item.driverName.ToLower())
                                                                        continue;

                                                                    var refValue = value.Value;
                                                                    var refStatus = value.StatusCode;
                                                                    var refTime = value.ServerTimestamp;
                                                                    driver.OnWriteTag(new TagDefinition() { NodeId = baseObject.NodeId }, ref refValue, ref refStatus, ref refTime);
                                                                }
                                                            }
                                                        }

                                                        //to scaled
                                                        var candidate = variable as AnalogItemState;
                                                        if (value.Value != null && candidate != null && candidate.InstrumentRange != null &&
                                                            candidate.InstrumentRange.Value != null)
                                                        {
                                                            if (candidate.DataType.IdType == IdType.Numeric)
                                                            {
                                                                uint dimension = 0;
                                                                if (candidate.ValueRank != ValueRanks.Scalar && candidate.ArrayDimensions != null)
                                                                    dimension = candidate.ArrayDimensions[0];
                                                                uint nType = (uint)candidate.DataType.Identifier;
                                                                if (nType != Opc.Ua.DataTypes.String && nType != Opc.Ua.DataTypes.Boolean)
                                                                    value.Value = ScaleValue(value.Value, nType, dimension, candidate.EURange.Value, candidate.InstrumentRange.Value, false);
                                                                if (value.Value == null)
                                                                    variable.StatusCode = StatusCodes.BadOutOfRange;
                                                            }
                                                        }
                                                        if (value.Value != null || (value.Value == null && item.AllowNullValues))
                                                            variable.Value = value.Value;
                                                    }

#if !NET_STANDARD
                                                    if (!String.IsNullOrEmpty(item.driverName) && server.IsRedundancyEnabled && server.ActiveServerManager.IsActiveServer)
                                                    {
                                                        if (mapNodeIdToTagLogEntities.ContainsKey(variable.NodeId) &&
                                                            mapNodeIdToTagLogEntities[variable.NodeId].enableStatistics)
                                                        {
                                                            var entry = mapNodeIdToTagLogEntities[variable.NodeId];
                                                            entry.UpdateStatistics(value);
                                                        }
                                                    }
#endif
                                                }
                                                else if (baseObject is BaseObjectState)
                                                {
                                                    var obj = baseObject as BaseObjectState;
                                                    UpdateBaseObjects(obj, item.NodeId, value);
                                                }
                                            });
                                        baseObject.ClearChangeMasks(SystemContext, true);
                                    }
                                });
                            }
                        });
                    ChangedTagsThread.Start();
                }

                pendingChangedTags.Enqueue(new ChangedTagArgs(e));
                ChangedTagsEvent.Set();
            }

#if !NET_STANDARD
            if (server.IsRedundancyEnabled && server.ActiveServerManager.IsActiveServer && !redundancyPrivateNodeIds.Contains(e.NodeId))
                RedundancyChangedTag(new ChangedTagArgs(e));
#endif
        }

        bool UpdateBaseObjects(BaseObjectState obj, NodeId node, DataValue value)
        { 
            List<BaseInstanceState> children = new List<BaseInstanceState>();
            obj.GetChildren(SystemContext, children);
                                                        
            var variable = (from v in children where v.NodeId == node select v).ToList();
            if (variable != null && variable.Count > 0)
            {
                if (variable[0] is BaseVariableState)
                {
                    var var = variable[0] as BaseVariableState;
                    var.StatusCode = value.StatusCode;
                    if (value.SourceTimestamp != DateTime.MinValue)
                        var.Timestamp = value.SourceTimestamp;
                    if (value.Value != null)
                        var.Value = value.Value;

                    return obj.SetChildValue(SystemContext, variable[0].BrowseName, var.Value, false);
                }

                //return obj.SetChildValue(SystemContext, variable[0].BrowseName, value.Value, false);
            }
            else if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    BaseObjectState b = child as BaseObjectState;
                    if (b != null)
                        /*return*/
                        if (UpdateBaseObjects(b, node, value))
                            return true;
                }
            }

            return false;
        }

        NodeState GetNodeStateFromNodeId(NodeId nodeId)
        {
            lock (mapNodeIdToNodeState)
            {
                NodeId n = nodeId;
                if (!mapNodeIdToNodeState.ContainsKey(n))
                {
                    if (nodeId.IdType == IdType.String)
                    {
                        string s = nodeId.ToString();
                        int p = s.IndexOf("?");
                        if (p != -1)
                        {
                            s = s.Substring(0, p);
                            n = new NodeId(s);
                        }

                        if (!mapNodeIdToNodeState.ContainsKey(n))
                        {
                            s = s.Replace(";s=", ";g=");
                            n = new NodeId(s);
                        }
                    }
                }
                if (mapNodeIdToNodeState.ContainsKey(n))
                    return mapNodeIdToNodeState[n];
            }

            return null;
        }

        void UANodeManager_TagChanging(object sender, ChangedTagArgs e)
        {
        }

        protected void UANodeManager_AudiEvent(object sender, AuditEventArgs e)
        {
            RaiseAuditEvents(e.sourceNode, e.sourceName, e.EventName, e.severity, e.time, e.status, true);
        }

        protected void UANodeManager_SystemEvent(object sender, SystemEventArgs e)
        {
            /* 
            if (synthesizer != null)
                synthesizer.SpeakAsync(e.EventName);
            */

            //var eventString = String.Format(Properties.Resources.ConsoleSystemEvent, e.time.ToLocalTime(), e.EventName);
            //Console.WriteLine(eventString);
            var info = new SystemEvent()
            {
                evtype = e.eventtype,
                details = e.details,
                comment = e.comment,
                state = e.state,
                //uniqueid = e.uniqueid,
                username = e.username,
                logentry = e.EventName,
                logdestination = e.logdestination
            };
            RaiseSystemEvents(e.sourceNode, e.sourceName, e.EventName, e.severity, e.time, info, true);
            //server.UpdateServerState(e.CommunicationFault ? ServerState.CommunicationFault : ServerState.Running);

        }

        protected void UANodeManager_StateChanged(object sender, ComunicationStateArgs e)
        {
            if (e.NewState == ComunicationState.Fault)
                server.UpdateServerState(ServerState.CommunicationFault);
            else
            {
                var driver = (from c in server.CommDrivers.Values.AsParallel()
                              where c is ICommunicationDriver2 &&
                              (c as ICommunicationDriver2).GetDriverState() == ComunicationState.Fault
                              select c as ICommunicationDriver2).FirstOrDefault();

                server.UpdateServerState(driver != null ? ServerState.CommunicationFault : ServerState.Running);
            }
        }

        void server_ServerStateChangeEvent(object sender, ServerStateChangedEventArgs e)
        {
            var eventName = String.Format(Properties.Resources.ServerStateChangeEventLog, e.OldState, e.NewState);

            var sourcename = String.Format("{0} ({1})", e.ServerName, System.Net.Dns.GetHostName());
            RaiseSystemEvents(null, sourcename, eventName, 
                (e.NewState == ServerState.CommunicationFault || e.NewState == ServerState.Failed) ? EventSeverity.High : EventSeverity.Low, DateTime.UtcNow, null, true);
        }

        private void server_SessionChangedEvent(object sender, SessionStateEventArgs e)
        {
            if (ExitMode
#if !NET_STANDARD
                || server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer
#endif
                )
                return;

            var anonymous = UserTokenType.Anonymous.ToString();
            if (e.OldUserNameIndentity == e.NewUserNameIndentity ||
                (e.OldUserNameIndentity == null || e.OldUserNameIndentity == anonymous) &&
                e.NewUserNameIndentity == anonymous)
                return;

            InitEventLogger();

            if (eventLogger != null)
            {
                UFUAEventLogEntity logEntry = new UFUAEventLogEntity()
                {
                    EventId = Guid.NewGuid(),
                    EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                    MaxAge = server.UFUAConfiguration.EventMaxAge.Value
                };

                if (e.SessionState == SessionEventReason.Activated)
                {
                    if (e.OldUserNameIndentity != anonymous && e.NewUserNameIndentity != anonymous)
                    {
                        logEntry.EventMessage = String.Format(Properties.Resources.EventLogSessionChangedMessage, e.OldUserNameIndentity, e.NewUserNameIndentity);
                        logEntry.EventState = Properties.Resources.EventLogSessionActivatedState;
                        logEntry.UserName = e.NewUserNameIndentity;
                    }
                    else if (e.OldUserNameIndentity != anonymous && e.NewUserNameIndentity == anonymous)
                    {
                        logEntry.EventMessage = String.Format(Properties.Resources.EventLogSessionClosedMessage, e.OldUserNameIndentity);
                        logEntry.EventState = Properties.Resources.EventLogSessionClosedState;
                        logEntry.UserName = e.OldUserNameIndentity;
                    }
                    else
                    {
                        logEntry.EventMessage = String.Format(Properties.Resources.EventLogSessionActivatedMessage, e.NewUserNameIndentity);
                        logEntry.EventState = Properties.Resources.EventLogSessionActivatedState;
                        logEntry.UserName = e.NewUserNameIndentity;
                    }
                }
                else if (e.SessionState == SessionEventReason.Closing)
                {
                    logEntry.EventMessage = String.Format(Properties.Resources.EventLogSessionClosedMessage, e.OldUserNameIndentity);
                    logEntry.EventState = Properties.Resources.EventLogSessionClosedState;
                    logEntry.UserName = e.OldUserNameIndentity;
                }
                else
                {
                    logEntry.EventMessage = String.Format(Properties.Resources.EventLogSessionChangedMessage, e.OldUserNameIndentity, e.NewUserNameIndentity);
                    logEntry.EventState = Properties.Resources.EventLogSessionActivatedState;
                    logEntry.UserName = e.NewUserNameIndentity;
                }

                logEntry.EventType = ObjectTypeIds.AuditSessionEventType;
                logEntry.SourceNode = e.ClientIdentity;
                logEntry.SourceName = e.ClientName;
                logEntry.EventDateTime = DateTime.UtcNow;
                logEntry.EventDetails = String.Format(Properties.Resources.EventLogSessionIdentity, e.SessionIdentity, e.SessionName);
                logEntry.EventComment = null;
                //logEntry.EventUniqueId = (systeminfo == null ? String.Empty : systeminfo.uniqueid);
                logEntry.EventOccurence = ulong.MinValue;
                logEntry.EventSequence = ulong.MinValue;
                logEntry.Severity = 0;

                eventLogger.AddLogEntity(logEntry);
            }
        }

#region Views

        /// <summary>
        /// Checks if the node is in the view.
        /// </summary>
        protected override bool IsNodeInView(ServerSystemContext context, ContinuationPoint continuationPoint, NodeState node)
        {
            if (continuationPoint.View != null)
            {
                lock (Lock)
                {
                    if (mapNodeIdToView.ContainsKey(continuationPoint.View.ViewId))
                    {
                        if (node == baseFolderAlarms ||
                            node == baseFolderTags ||
                            node == baseFolderDrivers ||
                            node == baseFolderDiagnosticDrivers)
                            return true;
                        else if (mapNodeIdToView[continuationPoint.View.ViewId].Contains(node.NodeId))
                            return true;
                        else
                        {
                            var children = new List<BaseInstanceState>();
                            node.GetChildren(SystemContext, children);
                            foreach (var child in children)
                            {
                                if (IsNodeInView(SystemContext, continuationPoint, child))
                                    return true;
                            }
                        }
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the reference is in the view.
        /// </summary>
        protected override bool IsReferenceInView(ServerSystemContext context, ContinuationPoint continuationPoint, IReference reference)
        {
            if (continuationPoint.View != null)
            {
                // guard against absolute node ids.
                if (reference.TargetId.IsAbsolute)
                {
                    return true;
                }

                // find the node.
                NodeState node = FindPredefinedNode((NodeId)reference.TargetId, typeof(NodeState));

                if (node != null)
                {
                    return IsNodeInView(context, continuationPoint, node);
                }
            }

            return true;
        }

#endregion
        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                        Properties.Resources.StoppingDrivers,
                                                        Properties.Resources.StoppedDrivers))
                {
                    foreach (var driver in server.CommDrivers.Keys)
                    {
                        var commDriverEx = server.CommDrivers[driver] as ICommunicationDriver2;
                        if (commDriverEx != null)
                            commDriverEx.StateChanged -= UANodeManager_StateChanged;
                        server.CommDrivers[driver].Terminate();
                        server.CommDrivers[driver].TagChanging -= UANodeManager_TagChanging;
                        server.CommDrivers[driver].TagChanged -= UANodeManager_TagChanged;
                        server.CommDrivers[driver].SystemEvent -= UANodeManager_SystemEvent;
                        server.CommDrivers[driver].AudiEvent -= UANodeManager_AudiEvent;
                        server.CommDrivers[driver].TagPrototypeQuery -= UANodeManager_TagPrototypeQuery;                        
                    }
                }
            }

            AutoResetEvent waitHandle = null;
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                       Properties.Resources.StoppingPendingTask,
                                                       Properties.Resources.StoppedPendingTask))
            {
                ExitPendingThreads();

                lock (listNodeToUpdate)
                {
                    if (nodeStateUpdater != null)
                    {
                        waitHandle = new AutoResetEvent(false);
                        nodeStateUpdater.Change(0, System.Threading.Timeout.Infinite);
                        nodeStateUpdater.Dispose(waitHandle);
                        nodeStateUpdater = null;
                    }
                }

                if (waitHandle != null)
                {
                    waitHandle.WaitOne();
                    waitHandle.Dispose();
                    waitHandle = null;
                }
            }

#if !NET_STANDARD
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                            Properties.Resources.StoppingScripts,
                                            Properties.Resources.StoppedScripts))
            {
                List<ScriptManager> scriptManagers = null;
                lock (Lock)
                {
                    if (mapScriptManagers != null)
                        scriptManagers = mapScriptManagers.Values.ToList();
                }

                if (scriptManagers != null)
                {
                    foreach (var scriptManager in scriptManagers)
                        scriptManager.RequestTerminate();

                    foreach (var scriptManager in scriptManagers)
                        scriptManager.Terminate();

                    foreach (var scriptManager in scriptManagers)
                        scriptManager.Dispose();
                }
            }
#endif
            //lock (Lock)
            {
                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                Properties.Resources.StoppingAlarms,
                                                Properties.Resources.StoppedAlarms))
                {
                    lock (Lock)
                    {
                        foreach (var source in mapNodeIdToSourceState.Values)
                        {
                            source.UpdatedStatusEvent -= OnSourceStateChangedAlarm;
                            source.AlarmsStatusChanged -= OnSourceAlarmsStatusChanged;
                            source.AlarmStateChanged -= OnAlarmStateChanged;
                            source.Dispose();
                        }
                    }

                    SourceState.ExitPendingThreads();
                    AuditTrace.AuditTraceToken.ClearAllTokens();

                    lock (lockSystemCounters)
                    {
                        if (systemCountersUpdater != null)
                        {
                            waitHandle = new AutoResetEvent(false);
                            systemCountersUpdater.Change(0, System.Threading.Timeout.Infinite);
                            systemCountersUpdater.Dispose(waitHandle);
                            systemCountersUpdater = null;
                        }
                    }

                    lock (Lock)
                    {
                        mapAlarmStatusToSourceState.Clear();
                        mapAlarmStatusToExpressionValueConverter.Clear();
                        mapNodeIdToAlarmStatus.Clear();
                        mapAlarmStatusNodeIdToVariableNodeId.Clear();
                        mapEnableStateToAlarmStatus.Clear();
                        mapActivationLowStateToAlarmStatus.Clear();
                        mapActivationStateToAlarmStatus.Clear();
                        mapHighHighStateToAlarmStatus.Clear();
                        mapHighStateToAlarmStatus.Clear();
                        mapLowStateToAlarmStatus.Clear();
                        mapLowLowStateToAlarmStatus.Clear();
                        mapAlarmStatusToAliasPosition.Clear();
                        mapNodeStateToAccessSettings.Clear();
                        mapNodeIdToSamplingIntervals.Clear();
                        mapNodeIdToNodeState.Clear();
                        mapNodeIdMemberNewFormat.Clear();
                        mapCacheNodeIdString.Clear();
                        mapNodeIdToInUseInfo.Clear();
                        mapNodeIdToInputArgs.Clear();
                        mapNodeIdToOutputArgs.Clear();
                        mapNodeIdToHistorianLogEntities.Clear();
                        mapNodeIdToAuditLogEntities.Clear();
                        mapEnabledHistorianLogEntities.Clear();
                        mapNodeIdToDataLoggerSettings.Clear();
                        mapDataLoggerNameToDataLoggerEntity.Clear();
                        //mapNodeIdToEventLogEntities.Clear();
                        mapAreaNodeIdToSources.Clear();
                        mapAreaNodeIdToSettings.Clear();
                        mapNodeIdToTagLogEntities.Clear();
                        mapNodeIdToAggregateConfigurations.Clear();
                        mapNodeIdToDynamicSettings.Clear();
                        mapNodeIdToView.Clear();
#if !NET_STANDARD
                        mapNodeIdMethodToScriptManager.Clear();
                        mapNodeIdMethodToSubName.Clear();
#endif
                        mapNodeIdToSourceState.Clear();
                    }
                }

                if (waitHandle != null)
                {
                    waitHandle.WaitOne();
                    waitHandle.Dispose();
                    waitHandle = null;
                }
            }

            lock (lockLogger)
            {
                using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                                Properties.Resources.StoppingHistoricalData,
                                                Properties.Resources.StoppedHistoricalData))
                {
                    if (historianLogger != null)
                    {
                        historianLogger.Dispose();
                        historianLogger.RecyclingData -= historianLogger_RecyclingData;
                        historianLogger.FlushedData -= historianLogger_FlushedData;
                        historianLogger.FlushedDataSafely -= historianLogger_FlushedDataSafely;
                        historianLogger.ErrorFlushingData -= historianLogger_ErrorFlushingData;
                        historianLogger.StatisticDataChanged -= historianLogger_StatisticDataChanged;
                        //historianLogger = null;
                    }

                    if (eventLogger != null)
                    {
                        eventLogger.Dispose();
                        eventLogger.RecyclingEvent -= eventLogger_RecyclingEvent;
                        eventLogger.FlushedEvent -= eventLogger_FlushedEvent;
                        eventLogger.FlushedDataSafely -= eventLogger_FlushedDataSafely;
                        eventLogger.ErrorFlushingData -= eventLogger_ErrorFlushingData;
                        eventLogger.StatisticDataChanged -= eventLogger_StatisticDataChanged;
                        //eventLogger = null;
                    }

                    if (tagLogger != null)
                    {
                        tagLogger.Dispose();
                        //tagLogger = null;
                    }

                    if (dataLogger != null)
                    {
                        dataLogger.Dispose();
                        dataLogger.FlushedDataSafely -= dataLogger_FlushedDataSafely;
                        dataLogger.ErrorStateChanged -= dataLogger_ErrorStateChanged;
                        //dataLogger = null;
                    }
                }
            }
        }

        protected static string GetUserName(ISystemContext context)
        {
            if (context.UserIdentity != null)
            {
                return context.UserIdentity.DisplayName;
            }

            return null;
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace.
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                var nodeid = nodeId;
                if (!mapCacheNodeIdString.ContainsKey(nodeid))
                    mapCacheNodeIdString.Add(nodeid, nodeid.ToString());

                if (mapNodeIdMemberNewFormat.ContainsKey(mapCacheNodeIdString[nodeId]))
                {
                    nodeid = mapNodeIdMemberNewFormat[mapCacheNodeIdString[nodeId]];
                }

                // check for check for nodes that are being currently monitored.
                MonitoredNode2 monitoredNode = null;

                if (MonitoredNodes.TryGetValue(nodeid, out monitoredNode))
                {
                    NodeHandle handle = new NodeHandle { NodeId = nodeid, Validated = true, Node = monitoredNode.Node };

                    return handle;
                }

                // if (nodeId.IdType != IdType.String)
                {
                    NodeState node = null;

                    if (PredefinedNodes.TryGetValue(nodeid, out node))
                    {
                        NodeHandle handle = new NodeHandle { NodeId = nodeid, Node = node, Validated = true };

                        return handle;
                    }
                }

                // By Maurizio Zaniboni - Progea Srl.
                // Commented for passing the CTT unit test : View Services->View Translate BrowsePath->Err-002.js.
                // parse the identifier.
                //ParsedNodeId parsedNodeId = ParsedNodeId.Parse(nodeid);

                //if (parsedNodeId != null)
                //{
                //    NodeHandle handle = new NodeHandle { NodeId = nodeid, Validated = false, Node = null, ParsedNodeId = parsedNodeId };

                //    return handle;
                //}
                
                return null;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
            ServerSystemContext context,
            NodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            NodeState target = null;

            // check if already in the cache.
            if (cache != null)
            {
                if (cache.TryGetValue(handle.NodeId, out target))
                {
                    // nulls mean a NodeId which was previously found to be invalid has been referenced again.
                    if (target == null)
                    {
                        return null;
                    }

                    handle.Node = target;
                    handle.Validated = true;
                    return handle.Node;
                }

                target = null;
            }

            try
            {
                // check if the node id has been parsed.
                if (handle.ParsedNodeId == null)
                {
                    return null;
                }

                NodeState root = null;
                if (handle.RootId == null || !mapNodeIdToNodeState.TryGetValue(handle.RootId, out root))
                {
                    return null;
                }

                // all done if no components to validate.
                if (String.IsNullOrEmpty(handle.ComponentPath))
                {
                    handle.Validated = true;
                    handle.Node = target = root;
                    return handle.Node;
                }

                // validate component.
                NodeState component = root.FindChildBySymbolicName(context, handle.ComponentPath);

                // component does not exist.
                if (component == null)
                {
                    return null;
                }

                // found a valid component.
                handle.Validated = true;
                handle.Node = target = component;
                return handle.Node;
            }
            finally
            {
                // store the node in the cache to optimize subsequent lookups.
                if (cache != null)
                {
                    cache.Add(handle.NodeId, target);
                }
            }
        }
#endregion

#region Overridden Methods
        /// <summary>
        /// Writes the value for the specified attributes.
        /// </summary>
        public override void Write(
            OperationContext context,
            IList<WriteValue> nodesToWrite,
            IList<ServiceResult> errors)
        {
            if (nodesToWrite != null && nodesToWrite.Count > 0)
            {
                for (int i = 0; i < nodesToWrite.Count; i++)
                { 
                    WriteValue nodeToWrite = nodesToWrite[i];
                    NodeState baseObject = null;
                    if (mapNodeIdToNodeState.ContainsKey(nodeToWrite.NodeId))
                        baseObject = mapNodeIdToNodeState[nodeToWrite.NodeId];
                    if (baseObject != null)
                    {
                        var candidate = baseObject as AnalogItemState;
                        if (candidate != null && candidate.EURange != null &&
                            candidate.EURange.Value != null)
                        {
                            // By Maurizio Zaniboni - Progea Srl
                            // Added "TypeInfo.IsNumericType(builtInType)" condition for passing CTT unit test :
                            // Data Access->Data Access AnalogItemType->Err-001.js
                            var dataTypeId = TypeInfo.GetDataTypeId(nodeToWrite.Value.Value);
                            var builtInType = TypeInfo.GetBuiltInType(dataTypeId);
                            if (candidate.DataType.IdType == IdType.Numeric && TypeInfo.IsNumericType(builtInType))
                            {
                                uint dimension = 0;
                                if (candidate.ValueRank != ValueRanks.Scalar && candidate.ArrayDimensions != null)
                                    dimension = candidate.ArrayDimensions[0];
                                double dval = 0.0;
                                Array aVal = null;
                                uint nType = (uint)candidate.DataType.Identifier;
                                if (nType != Opc.Ua.DataTypes.String && nType != Opc.Ua.DataTypes.Boolean)
                                {
                                    uint datatype = (uint)candidate.DataType.Identifier;
                                    if (dimension == 0)
                                    {
                                        dval = GetDoubleVal(nodeToWrite.Value.Value, datatype);
                                        if (dval < candidate.EURange.Value.Low || dval > candidate.EURange.Value.High)
                                        {
                                            errors[i] = StatusCodes.BadOutOfRange;
                                            nodesToWrite[i].Processed = true;
                                        }
                                    }
                                    else
                                    {
                                         aVal = nodeToWrite.Value.Value as Array;
                                         for (int ii = 0; ii < aVal.Length; ii++)
                                         {
                                             dval = GetDoubleVal(aVal.GetValue(ii), datatype);
                                             if (dval < candidate.EURange.Value.Low || dval > candidate.EURange.Value.High)
                                             {
                                                 nodesToWrite[i].Processed = true;
                                                 errors[i] = StatusCodes.BadOutOfRange;
                                                 break;
                                             }
                                         }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            base.Write(context, nodesToWrite, errors);
        }

        /// <summary>
        /// Called after creating a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemCreated(ServerSystemContext context, NodeHandle handle, MonitoredItem monitoredItem)
        {
            NodeState block = handle.Node/*.GetHierarchyRoot() steve 020511*/ as NodeState;
            if (block == null)//steve 020511
                return;

            uint occurences;
            double sampling;
            double newsampling;

            lock (mapNodeIdToInUseInfo)
            {
                if (!mapNodeIdToInUseInfo.ContainsKey(block.NodeId))
                {
                    mapNodeIdToInUseInfo[block.NodeId] = new UANodeInUseInfo
                    {
                        Occurrences = 0,
                        SamplingInterval = monitoredItem.SamplingInterval,
                        SamplingIntervalCollection = new Dictionary<int, double>()
                    };
                }
                if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.ContainsKey((int)monitoredItem.Id))
                    mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection[(int)monitoredItem.Id] = monitoredItem.SamplingInterval;
                else
                    mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Add((int)monitoredItem.Id, monitoredItem.SamplingInterval);

                var InUseInfo = mapNodeIdToInUseInfo[block.NodeId];
                sampling = InUseInfo.SamplingInterval;
                occurences = InUseInfo.Occurrences += 1;

                if (mapNodeIdToSamplingIntervals.ContainsKey(block.NodeId) && mapNodeIdToSamplingIntervals[block.NodeId] != -1)
                    newsampling = mapNodeIdToSamplingIntervals[block.NodeId];
                else
                {
                    if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Count > 0)
                        newsampling = mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Values.Min();
                    else
                        newsampling = server.UFUAConfiguration.DefaultDataIOSamplingInterval;
                }

                InUseInfo.SamplingInterval = newsampling; //monitoredItem.SamplingInterval;
                mapNodeIdToInUseInfo[block.NodeId] = InUseInfo;
            }

            Debug.Print("Receiving create on Monitored Item {0}: Occurences = {1}, Sampling Interval {2}", block.NodeId, occurences, monitoredItem.SamplingInterval);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(handle.Node);
            if (commDriver != null && commDriver.Count > 0 && (occurences == 1 || sampling != newsampling/*monitoredItem.SamplingInterval*/))
            {
                foreach (var driver in commDriver)
                {
                    InUseTagChanged(new InUseTagChangedArg()
                    {
                        Driver = driver,
                        NodeId = block.NodeId,
                        SamplingInterval = newsampling,
                        bInUse = occurences > 0,
                        TimeStamp = DateTime.UtcNow
                    });
                }
            }

            if (systemTags != null)
            {
                if (occurences > 0)
                    systemTags.SetInUse(SystemContext, block, newsampling);
                else
                    systemTags.SetNotInUse(SystemContext, block);
            }
        }

#if !NET_STANDARD
        /// <summary>
        /// Calls a method on an object.
        /// </summary>
        protected override ServiceResult Call(
            ISystemContext context,
            CallMethodRequest methodToCall,
            MethodState method,
            CallMethodResult result)
        {
            if (!server.IsRedundancyEnabled || server.ActiveServerManager.IsActiveServer)
                return base.Call(context, methodToCall, method, result);

            var callMethod = new CallMethod() { methodNodeId = method.NodeId, methodRequest = methodToCall, methodResult = result };
            var serviceResult = server.ActiveServerManager.SendCallingMethod(callMethod);
            result = callMethod.methodResult;
            return serviceResult;
        }
#endif

        Queue<InUseTagChangedArg> pendingInUseTags;
        Thread InUseTagsThread;
        ManualResetEvent InUseTagsEvent;
        void InUseTagChanged(InUseTagChangedArg inUseChanged)
        {
            lock (Lock)
            {
                if (InUseTagsThread == null)
                {
                    pendingInUseTags = new Queue<InUseTagChangedArg>();
                    InUseTagsEvent = new ManualResetEvent(false);
                    InUseTagsThread = new Thread((o) =>
                        {
                            while (!ExitMode)
                            {
                                if (!ExitMode)
                                {
                                    InUseTagsEvent.WaitOne();
                                }
                                List<InUseTagChangedArg> list;
                                lock (Lock) 
                                {
                                    list = pendingInUseTags.ToList();
                                    pendingInUseTags.Clear();
                                    InUseTagsEvent.Reset();
                                }

                                // Aggregates changes using ICommunicationDriver member.
                                var aggregatedComDriverList = new List<List<InUseTagChangedArg>>();
                                while (list.Count > 0)
                                {
                                    aggregatedComDriverList.Add((from c in list.AsParallel() where c.Driver == list[0].Driver select c).ToList());
                                    var match = list[0].Driver;
                                    list.RemoveAll(c => c.Driver == match);
                                }
                                
                                Parallel.ForEach(aggregatedComDriverList, listchanges =>
                                {
                                    ICommunicationDriver driver = listchanges[0].Driver;

                                    // Create InUse and NotInUse list and take the more recent item for each NodeId.
                                    var listInUse = new List<InUseTagChangedArg>();
                                    var listNotInUse = new List<InUseTagChangedArg>();
                                    while (listchanges.Count > 0)
                                    {
                                        var first = (from c in listchanges/*.AsParallel()*/
                                                     where c.NodeId == listchanges[0].NodeId
                                                     orderby c.TimeStamp descending 
                                                     select c).First();
                                        if (first.bInUse)
                                            listInUse.Add(first);
                                        else
                                            listNotInUse.Add(first);

                                        var match = listchanges[0].NodeId;
                                        listchanges.RemoveAll(c => c.NodeId == match);
                                    }

                                    var tags = new List<TagDefinition>();
                                    // Process InUse changes on ICommunicationDriver.
                                    if (listInUse.Count > 0)
                                    {
                                        listInUse.ForEach((e) =>
                                        {
                                            tags.Add(new TagDefinition()
                                            {
                                                NodeId = e.NodeId,
                                                SamplingInterval = e.SamplingInterval
                                            });
                                        });
                                        driver.InUseDynamics(tags, true);
                                    }

                                    tags.Clear();
                                    // Process NotInUse changes on ICommunicationDriver.
                                    if (listNotInUse.Count > 0)
                                    {
                                        listNotInUse.ForEach((e) =>
                                        {
                                            tags.Add(new TagDefinition()
                                            {
                                                NodeId = e.NodeId,
                                                SamplingInterval = e.SamplingInterval
                                            });
                                        });
                                        driver.InUseDynamics(tags, false);
                                    }
                                });
                            }
                        });
                    InUseTagsThread.Start();
                }

                pendingInUseTags.Enqueue(inUseChanged);
                InUseTagsEvent.Set();
            }
        }

        /// <summary>
        /// Called after modifying a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemModified(ServerSystemContext context, NodeHandle handle, MonitoredItem monitoredItem)
        {
            // overridden by the sub-class. (this is the sub-class!)
            NodeState block = handle.Node as NodeState;
            if (block == null)
                return;
            uint occurences;
            double sampling;
            double newsampling;

            lock (mapNodeIdToInUseInfo)
            {
                if (!mapNodeIdToInUseInfo.ContainsKey(block.NodeId))
                {
                    mapNodeIdToInUseInfo[block.NodeId] = new UANodeInUseInfo
                    {
                        Occurrences = 0,
                        SamplingInterval = monitoredItem.SamplingInterval,
                        SamplingIntervalCollection = new Dictionary<int, double>()
                    };
                }
                if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.ContainsKey((int)monitoredItem.Id))
                    mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection[(int)monitoredItem.Id] = monitoredItem.SamplingInterval;
                else
                    mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Add((int)monitoredItem.Id, monitoredItem.SamplingInterval);

                var InUseInfo = mapNodeIdToInUseInfo[block.NodeId];
                sampling = InUseInfo.SamplingInterval;
                occurences = InUseInfo.Occurrences;

                if (mapNodeIdToSamplingIntervals.ContainsKey(block.NodeId) && mapNodeIdToSamplingIntervals[block.NodeId] != -1)
                    newsampling = mapNodeIdToSamplingIntervals[block.NodeId];
                else
                {
                    if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Count > 0)
                        newsampling = mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Values.Min();
                    else
                        newsampling = server.UFUAConfiguration.DefaultDataIOSamplingInterval;
                }
                

                InUseInfo.SamplingInterval = newsampling;// monitoredItem.SamplingInterval;
                mapNodeIdToInUseInfo[block.NodeId] = InUseInfo;
            }

            Debug.Print("Receiving modify on Monitored Item {0}: Occurences = {1}, Sampling Interval {2}", block.NodeId, occurences, monitoredItem.SamplingInterval);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(handle.Node);
            if (commDriver != null && commDriver.Count > 0 && (occurences == 0 || sampling != newsampling/*monitoredItem.SamplingInterval*/))
            {
                foreach (var driver in commDriver)
                {
                    InUseTagChanged(new InUseTagChangedArg()
                    {
                        Driver = driver,
                        NodeId = block.NodeId,
                        SamplingInterval = newsampling,
                        bInUse = occurences > 0,
                        TimeStamp = DateTime.UtcNow
                    });
                }
            }

            if (systemTags != null)
            {
                if (occurences > 0)
                    systemTags.SetInUse(SystemContext, block, newsampling);
                else
                    systemTags.SetNotInUse(SystemContext, block);
            }
        }

        /// <summary>
        /// Called after deleting a MonitoredItem.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle for the node.</param>
        /// <param name="monitoredItem">The monitored item.</param>
        protected override void OnMonitoredItemDeleted(ServerSystemContext context, NodeHandle handle, MonitoredItem monitoredItem)
        {
            NodeState block = handle.Node/*.GetHierarchyRoot() steve 020511*/ as NodeState;
            if (block == null)//steve 020511
                return;

            uint occurences;
            double sampling;
            double newsampling;

            lock (mapNodeIdToInUseInfo)
            {
                if (!mapNodeIdToInUseInfo.ContainsKey(block.NodeId))
                {
                    mapNodeIdToInUseInfo[block.NodeId] = new UANodeInUseInfo
                    {
                        Occurrences = 0,
                        SamplingInterval = monitoredItem.SamplingInterval,
                        SamplingIntervalCollection = new Dictionary<int, double>()
                    };
                }
                if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.ContainsKey((int)monitoredItem.Id))
                    mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Remove((int)monitoredItem.Id);

                var InUseInfo = mapNodeIdToInUseInfo[block.NodeId];
                sampling = InUseInfo.SamplingInterval;
                occurences = InUseInfo.Occurrences;
                if (occurences > 0)
                    occurences = InUseInfo.Occurrences -= 1;

                if (mapNodeIdToSamplingIntervals.ContainsKey(block.NodeId) && mapNodeIdToSamplingIntervals[block.NodeId] != -1)
                    newsampling = mapNodeIdToSamplingIntervals[block.NodeId];
                else
                {
                    if (mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Count > 0)
                        newsampling = mapNodeIdToInUseInfo[block.NodeId].SamplingIntervalCollection.Values.Min();
                    else
                        newsampling = server.UFUAConfiguration.DefaultDataIOSamplingInterval;
                }

                InUseInfo.SamplingInterval = newsampling;// monitoredItem.SamplingInterval;
                mapNodeIdToInUseInfo[block.NodeId] = InUseInfo;
            }

            Debug.Print("Receiving delete on Monitored Item {0}: Occurences = {1}, Sampling Interval {2}", block.NodeId, occurences, monitoredItem.SamplingInterval);

            var commDriver = GetCommunicationDriversFromNodeStateDictionary(handle.Node);
            if (commDriver != null && commDriver.Count > 0 && (occurences == 0 || sampling != newsampling/*monitoredItem.SamplingInterval*/))
            {
                foreach (var driver in commDriver)
                {
                    InUseTagChanged(new InUseTagChangedArg()
                    {
                        Driver = driver,
                        NodeId = block.NodeId,
                        SamplingInterval = newsampling,
                        bInUse = occurences > 0,
                        TimeStamp = DateTime.UtcNow
                    });
                }
            }

            if (systemTags != null)
            {
                if (occurences > 0)
                    systemTags.SetInUse(SystemContext, block, newsampling);
                else
                    systemTags.SetNotInUse(SystemContext, block);
            }
        }

        /// <summary>
        /// Revises an aggregate filter (may require knowledge of the variable being used). 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="samplingInterval">The sampling interval for the monitored item.</param>
        /// <param name="queueSize">The queue size for the monitored item.</param>
        /// <param name="filterToUse">The filter to revise.</param>
        /// <returns>Good if the filter is acceptable.</returns>
        protected override StatusCode ReviseAggregateFilter(
            ServerSystemContext context,
            NodeHandle handle,
            double samplingInterval,
            uint queueSize,
            ServerAggregateFilter filterToUse)
        {
            // use the sampling interval to limit the processing interval.
            if (filterToUse.ProcessingInterval < samplingInterval)
            {
                filterToUse.ProcessingInterval = samplingInterval;
            }

            if (!mapNodeIdToHistorianLogEntities.ContainsKey(handle.NodeId))
                return StatusCodes.BadAggregateInvalidInputs;

            var entity = mapNodeIdToHistorianLogEntities[handle.NodeId];

            //if (entity == null)
            //{
            //    // no historial data so must start in the future.
            //    while (filterToUse.StartTime < DateTime.UtcNow)
            //    {
            //        filterToUse.StartTime = filterToUse.StartTime.AddMilliseconds(filterToUse.ProcessingInterval);
            //    }

            //    // use suitable defaults for values which are are not archived items.
            //    filterToUse.AggregateConfiguration.UseServerCapabilitiesDefaults = false;
            //    filterToUse.AggregateConfiguration.UseSlopedExtrapolation = false;
            //    filterToUse.AggregateConfiguration.TreatUncertainAsBad = false;
            //    filterToUse.AggregateConfiguration.PercentDataBad = 100;
            //    filterToUse.AggregateConfiguration.PercentDataGood = 100;
            //    filterToUse.Stepped = true;
            //}
            //else
            {
                // use the archive acquisition sampling interval to limit the processing interval.
                if (filterToUse.ProcessingInterval < entity.MinTimeInterval.TotalMilliseconds)
                {
                    filterToUse.ProcessingInterval = entity.MinTimeInterval.TotalMilliseconds;
                }

                // ensure the buffer does not get overfilled.
                while (filterToUse.StartTime.AddMilliseconds(queueSize * filterToUse.ProcessingInterval) < DateTime.UtcNow)
                {
                    filterToUse.StartTime = filterToUse.StartTime.AddMilliseconds(filterToUse.ProcessingInterval);
                }

                filterToUse.Stepped = entity.Stepped;

                // revise the configration.
                ReviseAggregateConfiguration(context, entity, filterToUse.AggregateConfiguration);
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Revises the aggregate configuration.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        /// <param name="configurationToUse"></param>
        private void ReviseAggregateConfiguration(
            ServerSystemContext context,
            UFUAHistorianLogEntity item,
            AggregateConfiguration configurationToUse)
        {
            // set configuration from defaults.
            if (configurationToUse.UseServerCapabilitiesDefaults || !mapNodeIdToAggregateConfigurations.ContainsKey(item.NodeId))
            {
                AggregateConfiguration configuration = Server.AggregateManager.GetDefaultConfiguration(null);
                configurationToUse.UseSlopedExtrapolation = configuration.UseSlopedExtrapolation;
                configurationToUse.TreatUncertainAsBad = configuration.TreatUncertainAsBad;
                configurationToUse.PercentDataBad = configuration.PercentDataBad;
                configurationToUse.PercentDataGood = configuration.PercentDataGood;
            }
            else
            {
                configurationToUse.PercentDataBad = mapNodeIdToAggregateConfigurations[item.NodeId].PercentDataBad.Value;
                configurationToUse.PercentDataGood = mapNodeIdToAggregateConfigurations[item.NodeId].PercentDataGood.Value;
                configurationToUse.TreatUncertainAsBad = mapNodeIdToAggregateConfigurations[item.NodeId].TreatUncertainAsBad.Value;
                configurationToUse.UseSlopedExtrapolation = mapNodeIdToAggregateConfigurations[item.NodeId].UseSlopedExtrapolation.Value;
            }

            // override configuration when it does not make sense for the item.
            configurationToUse.UseServerCapabilitiesDefaults = false;

            if (item.Stepped)
            {
                configurationToUse.UseSlopedExtrapolation = false;
            }
        }

        #endregion

        #region Simple Events
        internal protected virtual void RaiseSystemEvents(NodeId sourceNode/*ObjectIds.Server*/, String sourceName, String EventName, 
                               EventSeverity severity, DateTime time, SystemEvent systeminfo = null, bool addtosystemlog = false, bool logOnFile = true)
        {
            if (ExitMode
#if !NET_STANDARD
                || server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer
#endif
                )
                return;

            try
            {
                if (NodeId.IsNull(sourceNode))
                    sourceNode = ObjectIds.Server;

                if (addtosystemlog)
                {
                    //UFUAEventLogEntity logEntry;
                    //lock (Lock)
                    //{
                    //    InitEventLogger();

                    //    if (!mapNodeIdToEventLogEntities.ContainsKey(sourceNode))
                    //    {
                    //        mapNodeIdToEventLogEntities[sourceNode] = new UFUAEventLogEntity()
                    //        {
                    //            EventId = Guid.NewGuid(),
                    //            EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                    //            MaxAge = server.UFUAConfiguration.EventMaxAge
                    //        };
                    //    }

                    //    logEntry = mapNodeIdToEventLogEntities[sourceNode];
                    //}

                    InitEventLogger();

                    if (eventLogger != null)
                    {
                        UFUAEventLogEntity logEntry = new UFUAEventLogEntity()
                        {
                            EventId = Guid.NewGuid(),
                            EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                            MaxAge = server.UFUAConfiguration.EventMaxAge.Value
                        };

                        logEntry.EventType = (systeminfo == null ? ObjectTypeIds.SystemEventType : systeminfo.evtype);
                        logEntry.SourceNode = sourceNode.ToString();
                        logEntry.SourceName = sourceName;
                        logEntry.EventDateTime = time;
                        logEntry.EventMessage = EventName;
                        logEntry.EventDetails = (systeminfo == null ? String.Empty : systeminfo.details);
                        logEntry.EventComment = (systeminfo == null ? String.Empty : systeminfo.comment);
                        logEntry.EventState = (systeminfo == null ? String.Empty : systeminfo.state);
                        //logEntry.EventUniqueId = (systeminfo == null ? String.Empty : systeminfo.uniqueid);
                        logEntry.EventOccurence = ulong.MinValue;
                        logEntry.EventSequence = ulong.MinValue;
                        logEntry.Severity = (ushort)severity;
                        logEntry.UserName = (systeminfo == null ? String.Empty : systeminfo.username);

                        eventLogger.AddLogEntity(logEntry);
                        if (logOnFile)
                            AddLogEntry((EventSeverity)logEntry.Severity, logEntry.SourceName, logEntry.EventMessage, systeminfo == null ? -1 : systeminfo.logdestination);
                    }
                }

                if (Server.IsRunning)
                {
                    SystemEventState e = new SystemEventState(null);

                    e.Initialize(
                        SystemContext,
                        null,
                        severity,
                        new LocalizedText(EventName, string.Empty, EventName));

                    e.SetChildValue(SystemContext, BrowseNames.SourceNode, sourceNode, false);
                    e.SetChildValue(SystemContext, BrowseNames.SourceName, sourceName, false);
                    e.Time.Value = time;

                    Server.ReportEvent(e);
                }
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                Utils.Trace(e, "Unexpected error in OnRaiseSystemEvents");
            }
        }

        void RaiseAuditEvents(NodeId sourceNode/*ObjectIds.Server*/, String sourceName, String EventName,
                              EventSeverity severity, DateTime time, bool status, bool addtosystemlog = false)
        {
            if (ExitMode
#if !NET_STANDARD
                || server.IsRedundancyEnabled && !server.ActiveServerManager.IsActiveServer
#endif
                )
                return;

            try
            {
                if (NodeId.IsNull(sourceNode))
                    sourceNode = ObjectIds.Server;

                if (addtosystemlog)
                {
                    //UFUAEventLogEntity logEntry;
                    //lock (Lock)
                    //{
                    //    InitEventLogger();

                    //    if (!mapNodeIdToEventLogEntities.ContainsKey(sourceNode))
                    //    {
                    //        mapNodeIdToEventLogEntities[sourceNode] = new UFUAEventLogEntity()
                    //        {
                    //            EventId = Guid.NewGuid(),
                    //            EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                    //            MaxAge = server.UFUAConfiguration.EventMaxAge
                    //        };
                    //    }

                    //    logEntry = mapNodeIdToEventLogEntities[sourceNode];
                    //}

                    InitEventLogger();

                    if (eventLogger != null)
                    {
                        UFUAEventLogEntity logEntry = new UFUAEventLogEntity()
                        {
                            EventId = Guid.NewGuid(),
                            EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                            MaxAge = server.UFUAConfiguration.EventMaxAge.Value
                        };

                        logEntry.EventType = ObjectTypeIds.AuditEventType;
                        logEntry.SourceNode = sourceNode.ToString();
                        logEntry.SourceName = sourceName;
                        logEntry.EventDateTime = time;
                        logEntry.EventMessage = EventName;
                        logEntry.EventDetails = String.Empty;
                        logEntry.EventComment = String.Empty;
                        logEntry.EventState = String.Empty;
                        //logEntry.EventUniqueId = String.Empty;
                        logEntry.EventOccurence = ulong.MinValue;
                        logEntry.EventSequence = ulong.MinValue;
                        logEntry.Severity = (ushort)severity;
                        logEntry.UserName = String.Empty;

                        eventLogger.AddLogEntity(logEntry);
                        AddLogEntry((EventSeverity)logEntry.Severity, logEntry.SourceName, logEntry.EventMessage);
                    }
                }

                if (Server.IsRunning)
                {
                    AuditEventState ae = new AuditEventState(null);

                    ae.Initialize(
                        SystemContext,
                        null,
                        severity,
                        new LocalizedText(EventName, string.Empty, EventName),
                        status,
                        time);

                    ae.SetChildValue(SystemContext, BrowseNames.SourceNode, sourceNode, false);
                    ae.SetChildValue(SystemContext, BrowseNames.SourceName, sourceName, false);

                    Server.ReportEvent(ae);
                }
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                Utils.Trace(e, "Unexpected error in OnRaiseSystemEvents");
            }
        }

        void AddLogEntry(EventSeverity eventSeverity, string source, string message, int logDestination = -1)
        {
            EventLogEntryType entryType = EventLogEntryType.Information;
            switch (eventSeverity)
            {
                case EventSeverity.Max:
                case EventSeverity.High:
                    entryType = EventLogEntryType.Error;
                    break;
                case EventSeverity.MediumHigh:
                    entryType = EventLogEntryType.Warning;
                    break;
                case EventSeverity.Medium:
                case EventSeverity.MediumLow:
                case EventSeverity.Low:
                case EventSeverity.Min:
                    entryType = EventLogEntryType.Information;
                    break;
                default:
                    break;
            }
            LoggerDestination loggerDestination = LoggerDestination.Server;
            if (logDestination != -1)
                loggerDestination = (LoggerDestination)logDestination;
            Logger.WriteToEventLog(source,
                                    message,
                                    entryType,
                                    loggerDestination);
        }

        #endregion

        #region Historian Functions

        #region HistoricalEvents
        /// <summary>
        /// Reads history events.
        /// </summary>
        protected override void HistoryReadEvents(
            ServerSystemContext context,
            ReadEventDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToProcess.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                HistoryReadValueId nodeToRead = nodesToRead[handle.Index];
                HistoryReadResult result = results[handle.Index];

                HistoryEventReadRequest request = null;

                // validate node.
                NodeState source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                if (!CanUserReadNodeState(context, source))
                {
                    errors[handle.Index] = StatusCodes.BadUserAccessDenied;
                    continue;
                }

                // load an exising request.
                if (nodeToRead.ContinuationPoint != null)
                {
                    request = LoadEventContinuationPoint(context, nodeToRead.ContinuationPoint);

                    if (request == null)
                    {
                        errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
                        continue;
                    }
                }

                // create a new request.
                else
                {
                    request = CreateEventReadRequest(
                        context,
                        details,
                        handle,
                        nodeToRead);
                }

                // process values until the max is reached.
                HistoryEvent data = new HistoryEvent();

                while (request.NumValuesPerNode == 0 || data.Events.Count < request.NumValuesPerNode)
                {
                    if (request.Events.Count == 0)
                    {
                        break;
                    }

                    BaseEventState e = null;

                    if (request.TimeFlowsBackward)
                    {
                        e = request.Events.Last.Value;
                        request.Events.RemoveLast();
                    }
                    else
                    {
                        e = request.Events.First.Value;
                        request.Events.RemoveFirst();
                    }

                    data.Events.Add(GetEventFields(request, e));
                }

                errors[handle.Index] = ServiceResult.Good;

                // check if a continuation point is requred.
                if (request.Events.Count > 0)
                {
                    // only set if both end time and start time are specified.
                    if (details.StartTime != DateTime.MinValue && details.EndTime != DateTime.MinValue)
                    {
                        result.ContinuationPoint = SaveEventContinuationPoint(context, request);
                    }
                }

                // check if no data returned.
                else
                {
                    errors[handle.Index] = StatusCodes.GoodNoData;
                }

                // return the data.
                result.HistoryData = new ExtensionObject(data);
            }
        }
#endregion

#region HistoricalAccess
        /// <summary>
        /// Reads the raw data for an item.
        /// </summary>
        protected override void HistoryReadRawModified(
            ServerSystemContext context,
            ReadRawModifiedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                HistoryReadValueId nodeToRead = nodesToRead[handle.Index];
                HistoryReadResult result = results[handle.Index];

                HistoryReadRequest request = null;

                // validate node.
                NodeState source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                if (!CanUserReadNodeState(context, source))
                {
                    errors[handle.Index] = StatusCodes.BadUserAccessDenied;
                    continue;
                }

                // load an exising request.
                if (nodeToRead.ContinuationPoint != null)
                {
                    request = LoadHistoryContinuationPoint(context, nodeToRead.ContinuationPoint);

                    if (request == null)
                    {
                        errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
                        continue;
                    }
                }

                // create a new request.
                else
                {
                    request = CreateHistoryReadRequest(
                        context,
                        details,
                        handle,
                        nodeToRead);
                }

                // process values until the max is reached.
                HistoryData data = new HistoryData();

                while (request.NumValuesPerNode == 0 || data.DataValues.Count < request.NumValuesPerNode)
                {
                    if (request.Values.Count == 0)
                    {
                        break;
                    }

                    DataValue value = request.Values.First.Value;
                    request.Values.RemoveFirst();
                    data.DataValues.Add(value);
                }

                errors[handle.Index] = ServiceResult.Good;

                // check if a continuation point is requred.
                if (request.Values.Count > 0)
                {
                    // only set if both end time and start time are specified.
                    if (details.StartTime != DateTime.MinValue && details.EndTime != DateTime.MinValue)
                    {
                        result.ContinuationPoint = SaveHistoryContinuationPoint(context, request);
                    }
                }

                // check if no data returned.
                else
                {
                    errors[handle.Index] = StatusCodes.GoodNoData;
                }

                // return the data.
                result.HistoryData = new ExtensionObject(data);
            }
        }

        /// <summary>
        /// Reads the processed data for an item.
        /// </summary>
        protected override void HistoryReadProcessed(
            ServerSystemContext context,
            ReadProcessedDetails details,
            TimestampsToReturn timestampsToReturn,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToRead.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                HistoryReadValueId nodeToRead = nodesToRead[handle.Index];
                HistoryReadResult result = results[handle.Index];

                HistoryReadRequest request = null;

                // validate node.
                NodeState source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                if (!CanUserReadNodeState(context, source))
                {
                    errors[handle.Index] = StatusCodes.BadUserAccessDenied;
                    continue;
                }

                // load an exising request.
                if (nodeToRead.ContinuationPoint != null)
                {
                    request = LoadHistoryContinuationPoint(context, nodeToRead.ContinuationPoint);

                    if (request == null)
                    {
                        errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
                        continue;
                    }
                }

                // create a new request.
                else
                {
                    // validate aggregate type.
                    if (details.AggregateType.Count <= ii || !Server.AggregateManager.IsSupported(details.AggregateType[ii]))
                    {
                        errors[handle.Index] = StatusCodes.BadAggregateNotSupported;
                        continue;
                    }

                    request = CreateHistoryReadRequest(
                        context,
                        details,
                        handle,
                        nodeToRead,
                        details.AggregateType[ii]);
                }

                // process values until the max is reached.
                HistoryData data = new HistoryData();

                while (request.NumValuesPerNode == 0 || data.DataValues.Count < request.NumValuesPerNode)
                {
                    if (request.Values.Count == 0)
                    {
                        break;
                    }

                    DataValue value = request.Values.First.Value;
                    request.Values.RemoveFirst();
                    data.DataValues.Add(value);
                }

                errors[handle.Index] = ServiceResult.Good;

                // check if a continuation point is requred.
                if (request.Values.Count > 0)
                {
                    result.ContinuationPoint = SaveHistoryContinuationPoint(context, request);
                }

                // check if no data returned.
                else
                {
                    errors[handle.Index] = StatusCodes.GoodNoData;
                }

                // return the data.
                result.HistoryData = new ExtensionObject(data);
            }
        }

        /// <summary>
        /// Updates the data history for one or more nodes.
        /// </summary>
        protected override void HistoryUpdateData(
            ServerSystemContext context,
            IList<UpdateDataDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToProcess.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                UpdateDataDetails nodeToUpdate = nodesToUpdate[handle.Index];
                HistoryUpdateResult result = results[handle.Index];

                // validate node.
                NodeState source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                if (!CanUserWriteNodeState(context, source))
                {
                    errors[handle.Index] = StatusCodes.BadUserAccessDenied;
                    continue;
                }

                if (!mapNodeIdToHistorianLogEntities.ContainsKey(source.NodeId))
                    continue;

                var entity = mapNodeIdToHistorianLogEntities[source.NodeId];

                // process each item.
                for (int jj = 0; jj < nodeToUpdate.UpdateValues.Count; jj++)
                {
                    entity.value = nodeToUpdate.UpdateValues[jj];
                    entity.User = GetUserName(context);
                    StatusCode error = historianLogger.UpdateHistory(entity, nodeToUpdate.PerformInsertReplace);
                    result.OperationResults.Add(error);
                }

                errors[handle.Index] = ServiceResult.Good;
            }
        }

        /// <summary>
        /// Deletes the data history for one or more nodes.
        /// </summary>
        protected override void HistoryDeleteAtTime(
            ServerSystemContext context,
            IList<DeleteAtTimeDetails> nodesToUpdate,
            IList<HistoryUpdateResult> results,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToProcess.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                DeleteAtTimeDetails nodeToUpdate = nodesToUpdate[handle.Index];
                HistoryUpdateResult result = results[handle.Index];

                // validate node.
                NodeState source = ValidateNode(context, handle, cache);

                if (source == null)
                {
                    continue;
                }

                if (!CanUserWriteNodeState(context, source))
                {
                    errors[handle.Index] = StatusCodes.BadUserAccessDenied;
                    continue;
                }

                if (!mapNodeIdToHistorianLogEntities.ContainsKey(source.NodeId))
                    continue;

                var entity = mapNodeIdToHistorianLogEntities[source.NodeId];

                // process each item.
                for (int jj = 0; jj < nodeToUpdate.ReqTimes.Count; jj++)
                {
                    entity.User = GetUserName(context);
                    StatusCode error = historianLogger.DeleteHistory(entity, nodeToUpdate.ReqTimes[jj]);
                    result.OperationResults.Add(error);
                }

                errors[handle.Index] = ServiceResult.Good;
            }
        }

        /// <summary>
        /// Releases the history continuation point.
        /// </summary>
        protected override void HistoryReleaseContinuationPoints(
            ServerSystemContext context,
            IList<HistoryReadValueId> nodesToRead,
            IList<ServiceResult> errors,
            List<NodeHandle> nodesToProcess,
            IDictionary<NodeId, NodeState> cache)
        {
            for (int ii = 0; ii < nodesToProcess.Count; ii++)
            {
                NodeHandle handle = nodesToProcess[ii];
                HistoryReadValueId nodeToRead = nodesToRead[handle.Index];

                // find the continuation point.
                var historyRequest = LoadHistoryContinuationPoint(context, nodeToRead.ContinuationPoint);
                var eventRequest = LoadEventContinuationPoint(context, nodeToRead.ContinuationPoint);

                if (historyRequest == null && eventRequest == null)
                {
                    errors[handle.Index] = StatusCodes.BadContinuationPointInvalid;
                    continue;
                }

                // all done.
                errors[handle.Index] = StatusCodes.Good;
            }
        }

#endregion

#endregion

#region HistoricalEvents Helpers

        /// <summary>
        /// Creates a new event request.
        /// </summary>
        private HistoryEventReadRequest CreateEventReadRequest(
            ServerSystemContext context,
            ReadEventDetails details,
            NodeHandle handle,
            HistoryReadValueId nodeToRead)
        {
            FilterContext filterContext = new FilterContext(context.NamespaceUris, context.TypeTable, context.PreferredLocales);
            // LinkedList<BaseEventState> events = new LinkedList<BaseEventState>();

            bool sizeLimited = (details.StartTime == DateTime.MinValue || details.EndTime == DateTime.MinValue);
            //bool applyIndexRangeOrEncoding = (nodeToRead.ParsedIndexRange != NumericRange.Empty || !QualifiedName.IsNull(nodeToRead.DataEncoding));
            bool timeFlowsBackward = (details.StartTime == DateTime.MinValue) || (details.EndTime != DateTime.MinValue && details.EndTime < details.StartTime);

            List<NodeId> nodeIds = new List<NodeId>();
            nodeIds.Add(handle.NodeId);
            lock (Lock)
            {
                if (mapAreaNodeIdToSources.ContainsKey(handle.NodeId))
                {
                    nodeIds.AddRange(mapAreaNodeIdToSources[handle.NodeId]);
                }
            }

            dynamic entity;
            if (nodeIds.Count == 1)
            {
                entity = new UFUAEventLogEntity()
                {
                    EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                    MaxAge = server.UFUAConfiguration.EventMaxAge.Value,
                    SourceNode = handle.NodeId.ToString(),
                };
            }
            else
            {
                entity = new List<UFUAEventLogEntity>(nodeIds.Count);
                nodeIds.ForEach((nodeid) =>
                { 
                    entity.Add(new UFUAEventLogEntity()
                    {
                        EventConnection = server.UFUAConfiguration.EventDefaultConnection,
                        MaxAge = server.UFUAConfiguration.EventMaxAge.Value,
                        SourceNode = nodeid.ToString(),
                    });
                });
            }

            var events = new LinkedList<BaseEventState>();

            if (eventLogger != null)
            {
                // read history. 
                var view = eventLogger.ReadHistory(entity, details.StartTime, details.EndTime, sizeLimited ? (int)details.NumValuesPerNode : 0);

                int startBound = -1;
                int endBound = -1;
                int ii = (timeFlowsBackward) ? view.Count - 1 : 0;

                while (ii >= 0 && ii < view.Count)
                {
                    try
                    {
                        DateTime timestamp = view[ii].EventDateTimeUtc;

                        // check if looking for start of data.
                        if (events.Count == 0)
                        {
                            if (timeFlowsBackward)
                            {
                                if ((details.StartTime != DateTime.MinValue && timestamp >= details.StartTime) || (details.StartTime == DateTime.MinValue && timestamp >= details.EndTime))
                                {
                                    startBound = ii;
                                    continue;
                                }
                            }
                            else
                            {
                                if (timestamp <= details.StartTime)
                                {
                                    startBound = ii;
                                    continue;
                                }
                            }
                        }

                        // check if absolute max values specified.
                        if (sizeLimited)
                        {
                            if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < events.Count)
                            {
                                break;
                            }
                        }

                        // check for end bound.
                        if (details.EndTime != DateTime.MinValue && timestamp >= details.EndTime)
                        {
                            if (timeFlowsBackward)
                            {
                                if (timestamp <= details.EndTime)
                                {
                                    endBound = ii;
                                    break;
                                }
                            }
                            else
                            {
                                if (timestamp >= details.EndTime)
                                {
                                    endBound = ii;
                                    break;
                                }
                            }
                        }

                        var rowToEventField = RowToEventField(context, NamespaceIndex, view[ii]);
                        if (details.Filter.WhereClause != null && details.Filter.WhereClause.Elements.Count > 0)
                        {
                            if (!details.Filter.WhereClause.Evaluate(filterContext, rowToEventField))
                            {
                                continue;
                            }
                        }

                        // add event.
                        events.AddLast(rowToEventField);
                    }
                    finally
                    {
                        if (timeFlowsBackward)
                        {
                            ii--;
                        }
                        else
                        {
                            ii++;
                        }
                    }
                }
            }

            return new HistoryEventReadRequest() 
            { 
                Events = events, 
                TimeFlowsBackward = details.StartTime == DateTime.MinValue || (details.EndTime != DateTime.MinValue && details.EndTime < details.StartTime),
                NumValuesPerNode = details.NumValuesPerNode, 
                Filter = details.Filter,
                FilterContext = filterContext
            };
        }

        /// <summary>
        /// Fetches the requested event fields from the event.
        /// </summary>
        private HistoryEventFieldList GetEventFields(HistoryEventReadRequest request, IFilterTarget instance)
        {
            // fetch the event fields.
            HistoryEventFieldList fields = new HistoryEventFieldList();

            foreach (SimpleAttributeOperand clause in request.Filter.SelectClauses)
            {
                // get the value of the attribute (apply localization).
                object value = instance.GetAttributeValue(
                    request.FilterContext,
                    clause.TypeDefinitionId,
                    clause.BrowsePath,
                    clause.AttributeId,
                    clause.ParsedIndexRange);

                // add the value to the list of event fields.
                if (value != null)
                {
                    // translate any localized text.
                    LocalizedText text = value as LocalizedText;

                    if (text != null)
                    {
                        value = Server.ResourceManager.Translate(request.FilterContext.PreferredLocales, text);
                    }

                    // add value.
                    fields.EventFields.Add(new Variant(value));
                }

                // add a dummy entry for missing values.
                else
                {
                    fields.EventFields.Add(Variant.Null);
                }
            }

            return fields;
        }

        private static BaseEventState RowToEventField(ISystemContext SystemContext, ushort namespaceIndex,
            UFUAAuditLogItem row)
        {
            // construct the event.
            var e = new BaseEventState(null);

            e.Initialize(
                SystemContext,
                null,
                (EventSeverity)row.Severity, row.EventMessage);

            // override event id and time.                
            e.EventId.Value = row.EventId.ToByteArray();
            e.Time.Value = row.EventDateTimeUtc;
            e.ReceiveTime.Value = e.Time.Value;
            e.LocalTime = new PropertyState<TimeZoneDataType>(e);
            e.LocalTime.Value = Utils.GetTimeZoneInfo();

            e.SetChildValue(SystemContext, Opc.Ua.BrowseNames.SourceName, row.SourceName, false);
            e.SetChildValue(SystemContext, Opc.Ua.BrowseNames.SourceNode, new NodeId(row.SourceNode, namespaceIndex), false);
            e.SetChildValue(SystemContext, Opc.Ua.BrowseNames.LocalTime, row.EventDateTime, false);

            return e;
        }

        private class HistoryEventReadRequest
        {
            public byte[] ContinuationPoint;
            public LinkedList<BaseEventState> Events;
            public bool TimeFlowsBackward;
            public uint NumValuesPerNode;
            public EventFilter Filter;
            public FilterContext FilterContext;
        }

        /// <summary>
        /// Loads a event continuation point.
        /// </summary>
        private static HistoryEventReadRequest LoadEventContinuationPoint(
            ServerSystemContext context,
            byte[] continuationPoint)
        {
            Opc.Ua.Server.Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            var request = session.RestoreHistoryContinuationPoint(continuationPoint) as HistoryEventReadRequest;

            if (request == null)
            {
                return null;
            }

            return request;
        }

        /// <summary>
        /// Saves a event continuation point.
        /// </summary>
        private static byte[] SaveEventContinuationPoint(
            ServerSystemContext context,
            HistoryEventReadRequest request)
        {
            Opc.Ua.Server.Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            Guid id = Guid.NewGuid();
            session.SaveHistoryContinuationPoint(id, request);
            request.ContinuationPoint = id.ToByteArray();
            return request.ContinuationPoint;
        }

#endregion
        
#region HistoricalAccess Helpers

        /// <summary>
        /// Creates a new history request.
        /// </summary>
        private HistoryReadRequest CreateHistoryReadRequest(
            ServerSystemContext context,
            ReadRawModifiedDetails details,
            NodeHandle handle,
            HistoryReadValueId nodeToRead)
        {
            bool sizeLimited = (details.StartTime == DateTime.MinValue || details.EndTime == DateTime.MinValue);
            bool applyIndexRangeOrEncoding = (nodeToRead.ParsedIndexRange != NumericRange.Empty || !QualifiedName.IsNull(nodeToRead.DataEncoding));
            bool returnBounds = !details.IsReadModified && details.ReturnBounds;
            bool timeFlowsBackward = (details.StartTime == DateTime.MinValue) || (details.EndTime != DateTime.MinValue && details.EndTime < details.StartTime);

            var entity = mapNodeIdToHistorianLogEntities[handle.NodeId];

            LinkedList<DataValue> values = new LinkedList<DataValue>();

            // read history. 
            var view = historianLogger.ReadHistory(entity, details.StartTime, details.EndTime, details.IsReadModified, sizeLimited ? (int)details.NumValuesPerNode : 0);

            int startBound = -1;
            int endBound = -1;
            int ii = (timeFlowsBackward) ? view.Count - 1 : 0;

            while (ii >= 0 && ii < view.Count)
            {
                try
                {
                    DateTime timestamp = view[ii].SourceTimeStamp;

                    // check if looking for start of data.
                    if (values.Count == 0)
                    {
                        if (timeFlowsBackward)
                        {
                            if ((details.StartTime != DateTime.MinValue && timestamp >= details.StartTime) || (details.StartTime == DateTime.MinValue && timestamp >= details.EndTime))
                            {
                                startBound = ii;
                                continue;
                            }
                        }
                        else
                        {
                            if (timestamp <= details.StartTime)
                            {
                                startBound = ii;
                                continue;
                            }
                        }
                    }

                    // check if absolute max values specified.
                    if (sizeLimited)
                    {
                        if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                        {
                            break;
                        }
                    }

                    // check for end bound.
                    if (details.EndTime != DateTime.MinValue && timestamp >= details.EndTime)
                    {
                        if (timeFlowsBackward)
                        {
                            if (timestamp <= details.EndTime)
                            {
                                endBound = ii;
                                break;
                            }
                        }
                        else
                        {
                            if (timestamp >= details.EndTime)
                            {
                                endBound = ii;
                                break;
                            }
                        }
                    }

                    // check if the start bound needs to be returned.
                    if (returnBounds && values.Count == 0 && details.StartTime != DateTime.MinValue)
                    {
                        // add start bound.
                        if (startBound == -1)
                        {
                            values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.StartTime, details.StartTime));
                        }
                        else
                        {
                            values.AddLast(RowToDataValue(context, nodeToRead, entity.DataType, entity.ArrayDimension, view[startBound], applyIndexRangeOrEncoding));
                        }

                        // check if absolute max values specified.
                        if (sizeLimited)
                        {
                            if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                            {
                                break;
                            }
                        }
                    }

                    // add value.
                    values.AddLast(RowToDataValue(context, nodeToRead, entity.DataType, entity.ArrayDimension, view[ii], applyIndexRangeOrEncoding));
                }
                finally
                {
                    if (timeFlowsBackward)
                    {
                        ii--;
                    }
                    else
                    {
                        ii++;
                    }
                }
            }

            // add late bound.
            while (returnBounds && details.EndTime != DateTime.MinValue)
            {
                // add start bound.
                if (values.Count == 0)
                {
                    if (startBound == -1)
                    {
                        values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.StartTime, details.StartTime));
                    }
                    else
                    {
                        values.AddLast(RowToDataValue(context, nodeToRead, entity.DataType, entity.ArrayDimension, view[startBound], applyIndexRangeOrEncoding));
                    }
                }

                // check if absolute max values specified.
                if (sizeLimited)
                {
                    if (details.NumValuesPerNode > 0 && details.NumValuesPerNode < values.Count)
                    {
                        break;
                    }
                }

                // add end bound.
                if (endBound == -1)
                {
                    values.AddLast(new DataValue(Variant.Null, StatusCodes.BadBoundNotFound, details.EndTime, details.EndTime));
                }
                else
                {
                    values.AddLast(RowToDataValue(context, nodeToRead, entity.DataType, entity.ArrayDimension, view[endBound], applyIndexRangeOrEncoding));
                }

                break;
            }

            return new HistoryReadRequest() { Values = values, NumValuesPerNode = details.NumValuesPerNode, Filter = null };
        }

        /// <summary>
        /// Creates a new history request.
        /// </summary>
        private HistoryReadRequest CreateHistoryReadRequest(
            ServerSystemContext context,
            ReadProcessedDetails details,
            NodeHandle handle,
            HistoryReadValueId nodeToRead,
            NodeId aggregateId)
        {
            bool applyIndexRangeOrEncoding = (nodeToRead.ParsedIndexRange != NumericRange.Empty || !QualifiedName.IsNull(nodeToRead.DataEncoding));
            bool timeFlowsBackward = (details.EndTime < details.StartTime);

            var entity = mapNodeIdToHistorianLogEntities[handle.NodeId];

            LinkedList<DataValue> values = new LinkedList<DataValue>();

            // read history. 
            var view = historianLogger.ReadHistory(entity, details.StartTime, details.EndTime, false);

            int ii = (timeFlowsBackward) ? view.Count - 1 : 0;

            // choose the aggregate configuration.
#if !NET_STANDARD
            AggregateConfiguration configuration = (AggregateConfiguration)details.AggregateConfiguration.Clone();
#else
            AggregateConfiguration configuration = new AggregateConfiguration()
            {
                PercentDataBad = details.AggregateConfiguration.PercentDataBad,
                PercentDataGood = details.AggregateConfiguration.PercentDataGood,
                TreatUncertainAsBad = details.AggregateConfiguration.TreatUncertainAsBad,
                UseServerCapabilitiesDefaults = details.AggregateConfiguration.UseServerCapabilitiesDefaults,
                UseSlopedExtrapolation = details.AggregateConfiguration.UseSlopedExtrapolation
            };
#endif
            ReviseAggregateConfiguration(context, entity, configuration);

            // create the aggregate calculator.
            IAggregateCalculator calculator = Server.AggregateManager.CreateCalculator(
                aggregateId,
                details.StartTime,
                details.EndTime,
                details.ProcessingInterval,
                entity.Stepped,
                configuration);

            while (ii >= 0 && ii < view.Count)
            {
                try
                {

                    DataValue value = RowToDataValue(context, nodeToRead, entity.DataType, entity.ArrayDimension, view[ii], applyIndexRangeOrEncoding);
                    calculator.QueueRawValue(value);

                    // queue any processed values.
                    QueueProcessedValues(
                        context,
                        calculator,
                        nodeToRead.ParsedIndexRange,
                        nodeToRead.DataEncoding,
                        applyIndexRangeOrEncoding,
                        false,
                        values);
                }
                finally
                {
                    if (timeFlowsBackward)
                    {
                        ii--;
                    }
                    else
                    {
                        ii++;
                    }
                }
            }

            // queue any processed values beyond the end of the data.
            QueueProcessedValues(
                context,
                calculator,
                nodeToRead.ParsedIndexRange,
                nodeToRead.DataEncoding,
                applyIndexRangeOrEncoding,
                true,
                values);

            return new HistoryReadRequest { Values = values, NumValuesPerNode = 0, Filter = null };
        }

        /// <summary>
        /// Extracts and queues any processed values.
        /// </summary>
        private static void QueueProcessedValues(
            ServerSystemContext context,
            IAggregateCalculator calculator,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            bool applyIndexRangeOrEncoding,
            bool returnPartial,
            LinkedList<DataValue> values)
        {
            DataValue proccessedValue = calculator.GetProcessedValue(returnPartial);

            while (proccessedValue != null)
            {
                // apply any index range or encoding.
                if (applyIndexRangeOrEncoding)
                {
                    object rawValue = proccessedValue.Value;
                    ServiceResult result = BaseVariableState.ApplyIndexRangeAndDataEncoding(context, indexRange, dataEncoding, ref rawValue);

                    if (ServiceResult.IsBad(result))
                    {
                        proccessedValue.Value = rawValue;
                    }
                    else
                    {
                        proccessedValue.Value = null;
                        proccessedValue.StatusCode = result.StatusCode;
                    }
                }

                // queue the result.
                values.AddLast(proccessedValue);
                proccessedValue = calculator.GetProcessedValue(returnPartial);
            }
        }

        private DataValue RowToDataValue(
            ServerSystemContext context,
            HistoryReadValueId nodeToRead,
            NodeId DataType,
            uint Dimension,
            UFUAAuditDataItem row,
            bool applyIndexRangeOrEncoding)
        {
            Object objectvalue = null;
            BuiltInType builtinType = TypeInfo.GetBuiltInType(DataType, Server.TypeTree);
            try
            {
                if (row.dValue != null)
                {
                    objectvalue = ChangeType(row.dValue, builtinType, skipNullValues: true);
                }
                else
                {
                    objectvalue = ChangeType(row.Value, builtinType, Dimension, skipNullValues: true);
                }
            }
            catch (Exception ex)
            {              
            }

            DataValue value = new DataValue(new Variant(objectvalue), row.StatusCode, row.SourceTimeStamp, row.ServerTimeStamp);

            // apply any index range or encoding.
            if (applyIndexRangeOrEncoding)
            {
                object rawValue = value.Value;
                ServiceResult result = BaseVariableState.ApplyIndexRangeAndDataEncoding(context, nodeToRead.ParsedIndexRange, nodeToRead.DataEncoding, ref rawValue);

                if (ServiceResult.IsBad(result))
                {
                    value.Value = rawValue;
                }
                else
                {
                    value.Value = null;
                    value.StatusCode = result.StatusCode;
                }
            }

            return value;
        }

        /// <summary>
        /// Stores a read history request.
        /// </summary>
        private class HistoryReadRequest
        {
            public byte[] ContinuationPoint;
            public LinkedList<DataValue> Values;
            public uint NumValuesPerNode;
            public AggregateFilter Filter;
        }

        /// <summary>
        /// Loads a history continuation point.
        /// </summary>
        private static HistoryReadRequest LoadHistoryContinuationPoint(
            ServerSystemContext context,
            byte[] continuationPoint)
        {
            Opc.Ua.Server.Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            HistoryReadRequest request = session.RestoreHistoryContinuationPoint(continuationPoint) as HistoryReadRequest;

            if (request == null)
            {
                return null;
            }

            return request;
        }

        /// <summary>
        /// Saves a history continuation point.
        /// </summary>
        private static byte[] SaveHistoryContinuationPoint(
            ServerSystemContext context,
            HistoryReadRequest request)
        {
            Opc.Ua.Server.Session session = context.OperationContext.Session;

            if (session == null)
            {
                return null;
            }

            Guid id = Guid.NewGuid();
            session.SaveHistoryContinuationPoint(id, request);
            request.ContinuationPoint = id.ToByteArray();
            return request.ContinuationPoint;
        }
#endregion

#region Private Fields
        


        readonly object lockLogger = new object();
        readonly object lockThreads = new object();
        protected BaseObjectState baseFolderTags;
        BaseObjectState baseFolderAlarms;
        BaseObjectState baseFolderDrivers;
        BaseObjectState baseFolderDiagnosticDrivers;
        SystemTagsState systemTags;

        List<UFUAEnumString> enumStrings;
        List<UFUAAlarmDefinition> alarmDefinitions;
        List<UFUAAlarmThreshold> alarmThresholds;
        List<UFUATagPrototype> tagPrototypes;
        List<UFUAEngineeringUnit> engineeringUnits;
        List<UFUAHistorianSettings> historians;
        List<UFUAView> views;
        List<NodeState> updatedChangeMasksNodeStates;
        List<String> duplicatedHistoricalNames;
        List<String> duplicatedUnitsNames;

#if !NET_STANDARD
        [Obsolete]
        protected SpeechSynthesizer synthesizer;
#endif

        readonly UAServerConfiguration configuration;
        protected readonly UAServer server;
        protected readonly Dictionary<NodeId, NodeState> mapNodeIdToNodeState = new Dictionary<NodeId, NodeState>();
        protected readonly Dictionary<NodeId, int> mapNodeIdToSamplingIntervals = new Dictionary<NodeId, int>();
        protected readonly Dictionary<String, BaseInstanceState> mapNameToNodeState = new Dictionary<String, BaseInstanceState>();
        protected readonly Dictionary<NodeState, AccessSettings> mapNodeStateToAccessSettings = new Dictionary<NodeState, AccessSettings>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapNodeIdToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, NodeId> mapAlarmStatusNodeIdToVariableNodeId = new Dictionary<NodeId, NodeId>();
        readonly Dictionary<AlarmStatus, SourceState> mapAlarmStatusToSourceState = new Dictionary<AlarmStatus, SourceState>();
        readonly Dictionary<AlarmStatus, ExpressionValueConverter> mapAlarmStatusToExpressionValueConverter = new Dictionary<AlarmStatus, ExpressionValueConverter>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapEnableStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapActivationLowStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapActivationStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapHighHighStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapHighStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapLowStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<NodeId, List<AlarmStatus>> mapLowLowStateToAlarmStatus = new Dictionary<NodeId, List<AlarmStatus>>();
        readonly Dictionary<AlarmStatus, Dictionary<NodeId, int>> mapAlarmStatusToAliasPosition = new Dictionary<AlarmStatus, Dictionary<NodeId, int>>();
        protected readonly Dictionary<NodeId, Argument[]> mapNodeIdToInputArgs = new Dictionary<NodeId, Argument[]>();
        protected readonly Dictionary<NodeId, Argument[]> mapNodeIdToOutputArgs = new Dictionary<NodeId, Argument[]>();
        readonly Dictionary<NodeId, UFUAHistorianLogEntity> mapNodeIdToHistorianLogEntities = new Dictionary<NodeId, UFUAHistorianLogEntity>();
        readonly Dictionary<NodeId, UFUAHistorianLogEntity> mapNodeIdToAuditLogEntities = new Dictionary<NodeId, UFUAHistorianLogEntity>();
        readonly Dictionary<NodeId, List<String>> mapEnabledHistorianLogEntities = new Dictionary<NodeId, List<String>>();
        //readonly Dictionary<NodeId, UFUAEventLogEntity> mapNodeIdToEventLogEntities = new Dictionary<NodeId, UFUAEventLogEntity>();
        readonly Dictionary<NodeId, List<NodeId>> mapAreaNodeIdToSources = new Dictionary<NodeId, List<NodeId>>();
        readonly Dictionary<NodeId, AreaSettings> mapAreaNodeIdToSettings = new Dictionary<NodeId, AreaSettings>();
        readonly Dictionary<NodeId, UFUATagLogEntity> mapNodeIdToTagLogEntities = new Dictionary<NodeId, UFUATagLogEntity>();
        readonly Dictionary<NodeId, List<DataLoggerModel.DataLoggerSettings>> mapNodeIdToDataLoggerSettings = new Dictionary<NodeId, List<DataLoggerModel.DataLoggerSettings>>();
        readonly Dictionary<String, DataLoggerManager.DataLoggerEntity> mapDataLoggerNameToDataLoggerEntity = new Dictionary<String, DataLoggerManager.DataLoggerEntity>();
        readonly Dictionary<String, AggregateConfigurationState> mapNodeIdToAggregateConfigurations = new Dictionary<String, AggregateConfigurationState>();
        readonly Dictionary<NodeId, String> mapNodeIdToDynamicSettings = new Dictionary<NodeId, String>();
        readonly Dictionary<NodeId, List<NodeId>> mapNodeIdToView = new Dictionary<NodeId, List<NodeId>>();
        readonly Dictionary<NodeId, SourceState> mapNodeIdToSourceState = new Dictionary<NodeId, SourceState>();
        readonly Dictionary<NodeId, UANodeInUseInfo> mapNodeIdToInUseInfo = new Dictionary<NodeId, UANodeInUseInfo>();
#if !NET_STANDARD
        readonly Dictionary<NodeId, ScriptManager> mapNodeIdMethodToScriptManager = new Dictionary<NodeId, ScriptManager>();
        readonly Dictionary<NodeId, String> mapNodeIdMethodToSubName = new Dictionary<NodeId, String>();
#endif
        readonly Dictionary<NodeId, Dictionary<string, int>> mapStructToMemberOder = new Dictionary<NodeId, Dictionary<string, int>>();
        readonly Dictionary<String, NodeId> mapNodeIdMemberNewFormat = new Dictionary<String, NodeId>();
        readonly Dictionary<NodeId, String> mapCacheNodeIdString = new Dictionary<NodeId, String>();
        readonly Dictionary<NodeState, List<ICommunicationDriver>> mapNodeStateToCommDriver = new Dictionary<NodeState, List<ICommunicationDriver>>();
        readonly Dictionary<NodeId, List<NodeId>> mapAlwaysNodeIdInUse = new Dictionary<NodeId, List<NodeId>>();

        static readonly String eventLoggerFaultFormat = "EventLogger-@-";
        static readonly String historianFaultFormat = "Historian-@-";
        static readonly String dataLoggerFaultFormat = "DataLogger-@-";
        readonly List<String> recordingInErrorConnections = new List<String>();

        readonly List<Action> listPendingInitTask = new List<Action>();
        readonly List<NodeState> listNodeToUpdate = new List<NodeState>();
        readonly List<NodeId> redundancyPrivateNodeIds = new List<NodeId>();

        Timer nodeStateUpdater;
        bool bNodeStateUpdaterReady;

        readonly object lockSystemCounters = new object();
        Dictionary<NodeId, UpdateCounters> mapAlarmCounters;
        Dictionary<NodeId, UpdateCounters> mapMessageCounters;
        Timer systemCountersUpdater;
        bool bSystemCountersUpdating;
        bool isBeeping;

        bool isAnyTagStatisticsEnabled;
        bool isAnyAuditTraceEnabled;

        bool isHistoricalLoggerDisabled;
        internal uint totalCount { get; private set; }
#if NET_STANDARD
        bool isHistorianDataProtectionNotSupported;
        bool isEventLogDataProtectionNotSupported;
        bool isDataLoggerDataProtectionNotSupported;
#endif
#endregion
    }
}
