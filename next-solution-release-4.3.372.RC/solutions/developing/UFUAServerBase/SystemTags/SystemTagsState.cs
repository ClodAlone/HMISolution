using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;

namespace UFUAServerBase.SystemTags
{
    internal class SystemTagsState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public SystemTagsState(NodeState parent, ISystemContext context, ushort namespaceindex)
            : base(parent)
        {
            Initialize(context);

            NodeId = new NodeId(UFUAServerInfo.Guids.SystemTagsGuid, namespaceindex);
            BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.SystemTagsName, namespaceindex);
            DisplayName = BrowseName.Name;
            SymbolicName = BrowseName.Name;
            Description = new LocalizedText(UFUAServerInfo.BrowserNames.SystemTagsDesc, String.Empty, UFUAServerInfo.BrowserNames.SystemTagsDesc);
            ReferenceTypeId = null;
            TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            EventNotifier = EventNotifiers.None;

            CreateSystemTagChilds(context, namespaceindex);
            CreateAlarmsChilds(context, namespaceindex);
            CreateMessagesChilds(context, namespaceindex);
            CreateHistorianChilds(context, namespaceindex);
            CreateEventLoggerChilds(context, namespaceindex);
            CreateRedundancyChilds(context, namespaceindex);
        }
        #endregion

        #region Private Methods

        void CreateAlarmsChilds(ISystemContext context, ushort namespaceindex)
        {
            alarmsBaseObject = new BaseObjectState(this);
            alarmsBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.AlarmsName, namespaceindex);
            alarmsBaseObject.DisplayName = alarmsBaseObject.BrowseName.Name;
            alarmsBaseObject.SymbolicName = alarmsBaseObject.BrowseName.Name;
            alarmsBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            alarmsBaseObject.ReferenceTypeId = null;
            alarmsBaseObject.NodeId = ModelUtils.ConstructIdForComponent(alarmsBaseObject, namespaceindex);
            alarmsBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);

            // Alarms system variables
            alarmsSoundState = new SystemTagsMember<bool>(true, true, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsSoundStateName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsSoundStateDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsSoundStateDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite
            };

            alarmsSoundState.Create(
                context,
                alarmsSoundState.NodeId,
                new QualifiedName(alarmsSoundState.SymbolicName, namespaceindex),
                null,
                true);

            alarmsSoundBuzzing = new SystemTagsMember<bool>(false, false, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsSoundBuzzing.Create(
                context,
                alarmsSoundBuzzing.NodeId,
                new QualifiedName(alarmsSoundBuzzing.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumEnabled = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumEnabledName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumEnabledDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumEnabledDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumEnabled.Create(
                context,
                alarmsNumEnabled.NodeId,
                new QualifiedName(alarmsNumEnabled.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumActiveOn = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumActiveOnName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumActiveOnDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumActiveOnDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumActiveOn.Create(
                context,
                alarmsNumActiveOn.NodeId,
                new QualifiedName(alarmsNumActiveOn.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumActiveOff = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumActiveOffName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumActiveOffDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumActiveOffDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumActiveOff.Create(
                context,
                alarmsNumActiveOff.NodeId,
                new QualifiedName(alarmsNumActiveOff.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumActiveOnOff = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumActiveOnOffName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumActiveOnOffDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumActiveOnOffDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumActiveOnOff.Create(
                context,
                alarmsNumActiveOnOff.NodeId,
                new QualifiedName(alarmsNumActiveOnOff.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumShelved = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumShelvedName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumShelvedDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumShelvedDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumShelved.Create(
                context,
                alarmsNumShelved.NodeId,
                new QualifiedName(alarmsNumShelved.SymbolicName, namespaceindex),
                null,
                true);

            alarmsNumNotAck = new SystemTagsMember<long>(false, 0, alarmsBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.AlarmsNumNotAckName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.AlarmsNumNotAckDesc, String.Empty, UFUAServerInfo.BrowserNames.AlarmsNumNotAckDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            alarmsNumNotAck.Create(
                context,
                alarmsNumNotAck.NodeId,
                new QualifiedName(alarmsNumNotAck.SymbolicName, namespaceindex),
                null,
                true);

            runtimeAlarmSettingsUpdateMethod = new MethodState(this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RuntimeAlarmSettingsUpdateMethodName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RuntimeAlarmSettingsUpdateMethodDesc, String.Empty, UFUAServerInfo.BrowserNames.RuntimeAlarmSettingsUpdateMethodDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                UserExecutable = true,
                Executable = true
            };

            runtimeAlarmSettingsUpdateMethod.Create(
                context,
                runtimeAlarmSettingsUpdateMethod.NodeId,
                new QualifiedName(runtimeAlarmSettingsUpdateMethod.SymbolicName, namespaceindex),
                null,
                true);

            
        }

        void CreateMessagesChilds(ISystemContext context, ushort namespaceindex)
        {
            messagesBaseObject = new BaseObjectState(this);
            messagesBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.MessagesName, namespaceindex);
            messagesBaseObject.DisplayName = messagesBaseObject.BrowseName.Name;
            messagesBaseObject.SymbolicName = messagesBaseObject.BrowseName.Name;
            messagesBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            messagesBaseObject.ReferenceTypeId = null;
            messagesBaseObject.NodeId = ModelUtils.ConstructIdForComponent(messagesBaseObject, namespaceindex);
            messagesBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);

            // Messages system variables
            messagesNumEnabled = new SystemTagsMember<long>(false, 0, messagesBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.MessagesNumEnabledName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.MessagesNumEnabledDesc, String.Empty, UFUAServerInfo.BrowserNames.MessagesNumEnabledDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            messagesNumEnabled.Create(
                context,
                messagesNumEnabled.NodeId,
                new QualifiedName(messagesNumEnabled.SymbolicName, namespaceindex),
                null,
                true);

            messagesNumActiveOn = new SystemTagsMember<long>(false, 0, messagesBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.MessagesNumActiveOnName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.MessagesNumActiveOnDesc, String.Empty, UFUAServerInfo.BrowserNames.MessagesNumActiveOnDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            messagesNumActiveOn.Create(
                context,
                messagesNumActiveOn.NodeId,
                new QualifiedName(messagesNumActiveOn.SymbolicName, namespaceindex),
                null,
                true);

            messagesNumShelved = new SystemTagsMember<long>(false, 0, messagesBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.MessagesNumShelvedName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.MessagesNumShelvedDesc, String.Empty, UFUAServerInfo.BrowserNames.MessagesNumShelvedDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            messagesNumShelved.Create(
                context,
                messagesNumShelved.NodeId,
                new QualifiedName(messagesNumShelved.SymbolicName, namespaceindex),
                null,
                true);
        }

        void CreateSystemTagChilds(ISystemContext context, ushort namespaceindex)
        {
            systemTagBaseObject = new BaseObjectState(this);
            systemTagBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.SystemTagName, namespaceindex);
            systemTagBaseObject.DisplayName = systemTagBaseObject.BrowseName.Name;
            systemTagBaseObject.SymbolicName = systemTagBaseObject.BrowseName.Name;
            systemTagBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            systemTagBaseObject.ReferenceTypeId = null;
            systemTagBaseObject.NodeId = ModelUtils.ConstructIdForComponent(systemTagBaseObject, namespaceindex);
            systemTagBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);

            // Dynamic Tags system variables
            dynamicTagCount = new SystemTagsMember<long>(true, 0, systemTagBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DynamicTagCountName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DynamicTagCountDesc, String.Empty, UFUAServerInfo.BrowserNames.DynamicTagCountDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            dynamicTagCount.Create(
                context,
                dynamicTagCount.NodeId,
                new QualifiedName(dynamicTagCount.SymbolicName, namespaceindex),
                null,
                true);

            // RecordingInError system variables
            recordingInError = new SystemTagsMember<bool>(false, false, systemTagBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RecordingInErrorName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RecordingInErrorDesc, String.Empty, UFUAServerInfo.BrowserNames.RecordingInErrorDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            recordingInError.Create(
                context,
                recordingInError.NodeId,
                new QualifiedName(recordingInError.SymbolicName, namespaceindex),
                null,
                true);

            // LicenseSerialNumber system variables
            licenseSerialNumber = new SystemTagsMember<string>(false, null, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.LicenseSerialNumberName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.LicenseSerialNumberDesc, String.Empty, UFUAServerInfo.BrowserNames.LicenseSerialNumberDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            licenseSerialNumber.Create(
                context,
                licenseSerialNumber.NodeId,
                new QualifiedName(licenseSerialNumber.SymbolicName, namespaceindex),
                null,
                true);

            // WebHMI Active sessions system variables
            activeSessionsWebHMI = new SystemTagsMember<long>(false, 0, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.ActiveSessionsWebHMIName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.ActiveSessionsWebHMIDesc, String.Empty, UFUAServerInfo.BrowserNames.ActiveSessionsWebHMIDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite
            };

            activeSessionsWebHMI.Create(
                context,
                activeSessionsWebHMI.NodeId,
                new QualifiedName(activeSessionsWebHMI.SymbolicName, namespaceindex),
                null,
                true);

            // WebClient HTML5 Active sessions system variables
            activeSessionsWebClient = new SystemTagsMember<long>(false, 0, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.ActiveSessionsWebClientName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.ActiveSessionsWebClientDesc, String.Empty, UFUAServerInfo.BrowserNames.ActiveSessionsWebClientDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite
            };

            activeSessionsWebClient.Create(
                context,
                activeSessionsWebClient.NodeId,
                new QualifiedName(activeSessionsWebClient.SymbolicName, namespaceindex),
                null,
                true);

            // WebApps Active sessions system variables
            activeSessionsWebApps = new SystemTagsMember<long>(false, 0, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.ActiveSessionsWebAppsName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.ActiveSessionsWebAppsDesc, String.Empty, UFUAServerInfo.BrowserNames.ActiveSessionsWebAppsDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite
            };

            activeSessionsWebApps.Create(
                context,
                activeSessionsWebApps.NodeId,
                new QualifiedName(activeSessionsWebApps.SymbolicName, namespaceindex),
                null,
                true);
        }

        void CreateHistorianChilds(ISystemContext context, ushort namespaceindex)
        {
            historianBaseObject = new BaseObjectState(this);
            historianBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.HistorianName, namespaceindex);
            historianBaseObject.DisplayName = historianBaseObject.BrowseName.Name;
            historianBaseObject.SymbolicName = historianBaseObject.BrowseName.Name;
            historianBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            historianBaseObject.ReferenceTypeId = null;
            historianBaseObject.NodeId = ModelUtils.ConstructIdForComponent(historianBaseObject, namespaceindex);
            historianBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);

            // Historical system variables
            recordHistoricalEntriesPending = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RecordEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RecordEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.RecordEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            recordHistoricalEntriesPending.Create(
                context,
                recordHistoricalEntriesPending.NodeId,
                new QualifiedName(recordHistoricalEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            recordHistoricalEntriesRunning = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RecordEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RecordEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.RecordEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            recordHistoricalEntriesRunning.Create(
                context,
                recordHistoricalEntriesRunning.NodeId,
                new QualifiedName(recordHistoricalEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            failsHistoricalEntriesPending = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FailsEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FailsEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.FailsEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            failsHistoricalEntriesPending.Create(
                context,
                failsHistoricalEntriesPending.NodeId,
                new QualifiedName(failsHistoricalEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            failsHistoricalEntriesRunning = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FailsEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FailsEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.FailsEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            failsHistoricalEntriesRunning.Create(
                context,
                failsHistoricalEntriesRunning.NodeId,
                new QualifiedName(failsHistoricalEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            deleteHistoricalEntriesPending = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DeleteEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DeleteEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.DeleteEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            deleteHistoricalEntriesPending.Create(
                context,
                deleteHistoricalEntriesPending.NodeId,
                new QualifiedName(deleteHistoricalEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            deleteHistoricalEntriesRunning = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DeleteEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DeleteEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.DeleteEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            deleteHistoricalEntriesRunning.Create(
                context,
                deleteHistoricalEntriesRunning.NodeId,
                new QualifiedName(deleteHistoricalEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            flushHistoricalEntriesPending = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FlushEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FlushEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.FlushEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            flushHistoricalEntriesPending.Create(
                context,
                flushHistoricalEntriesPending.NodeId,
                new QualifiedName(flushHistoricalEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            flushHistoricalEntriesRunning = new SystemTagsMember<long>(false, 0, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FlushEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FlushEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.FlushEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            flushHistoricalEntriesRunning.Create(
                context,
                flushHistoricalEntriesRunning.NodeId,
                new QualifiedName(flushHistoricalEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            dischargingHistoricalEntriesMode = new SystemTagsMember<bool>(false, false, historianBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DischargingEntriesName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DischargingEntriesDesc, String.Empty, UFUAServerInfo.BrowserNames.DischargingEntriesDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            dischargingHistoricalEntriesMode.Create(
                context,
                dischargingHistoricalEntriesMode.NodeId,
                new QualifiedName(dischargingHistoricalEntriesMode.SymbolicName, namespaceindex),
                null,
                true);
        }

        void CreateEventLoggerChilds(ISystemContext context, ushort namespaceindex)
        {
            eventLoggerBaseObject = new BaseObjectState(this);
            eventLoggerBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.EventLoggerName, namespaceindex);
            eventLoggerBaseObject.DisplayName = eventLoggerBaseObject.BrowseName.Name;
            eventLoggerBaseObject.SymbolicName = eventLoggerBaseObject.BrowseName.Name;
            eventLoggerBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            eventLoggerBaseObject.ReferenceTypeId = null;
            eventLoggerBaseObject.NodeId = ModelUtils.ConstructIdForComponent(eventLoggerBaseObject, namespaceindex);
            eventLoggerBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);

            // Event system variables
            recordEventEntriesPending = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RecordEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RecordEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.RecordEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            recordEventEntriesPending.Create(
                context,
                recordEventEntriesPending.NodeId,
                new QualifiedName(recordEventEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            recordEventEntriesRunning = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RecordEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RecordEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.RecordEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            recordEventEntriesRunning.Create(
                context,
                recordEventEntriesRunning.NodeId,
                new QualifiedName(recordEventEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            failsEventEntriesPending = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FailsEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FailsEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.FailsEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            failsEventEntriesPending.Create(
                context,
                failsEventEntriesPending.NodeId,
                new QualifiedName(failsEventEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            failsEventEntriesRunning = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FailsEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FailsEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.FailsEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            failsEventEntriesRunning.Create(
                context,
                failsEventEntriesRunning.NodeId,
                new QualifiedName(failsEventEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            deleteEventEntriesPending = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DeleteEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DeleteEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.DeleteEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            deleteEventEntriesPending.Create(
                context,
                deleteEventEntriesPending.NodeId,
                new QualifiedName(deleteEventEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            deleteEventEntriesRunning = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DeleteEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DeleteEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.DeleteEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            deleteEventEntriesRunning.Create(
                context,
                deleteEventEntriesRunning.NodeId,
                new QualifiedName(deleteEventEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            flushEventEntriesPending = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FlushEntriesPendingName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FlushEntriesPendingDesc, String.Empty, UFUAServerInfo.BrowserNames.FlushEntriesPendingDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            flushEventEntriesPending.Create(
                context,
                flushEventEntriesPending.NodeId,
                new QualifiedName(flushEventEntriesPending.SymbolicName, namespaceindex),
                null,
                true);

            flushEventEntriesRunning = new SystemTagsMember<long>(false, 0, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.FlushEntriesRunningName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.FlushEntriesRunningDesc, String.Empty, UFUAServerInfo.BrowserNames.FlushEntriesRunningDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            flushEventEntriesRunning.Create(
                context,
                flushEventEntriesRunning.NodeId,
                new QualifiedName(flushEventEntriesRunning.SymbolicName, namespaceindex),
                null,
                true);

            dischargingEventEntriesMode = new SystemTagsMember<bool>(false, false, eventLoggerBaseObject)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.DischargingEntriesName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.DischargingEntriesDesc, String.Empty, UFUAServerInfo.BrowserNames.DischargingEntriesDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            dischargingEventEntriesMode.Create(
                context,
                dischargingEventEntriesMode.NodeId,
                new QualifiedName(dischargingEventEntriesMode.SymbolicName, namespaceindex),
                null,
                true);
        }

        void CreateRedundancyChilds(ISystemContext context, ushort namespaceindex)
        {
            redundancyBaseObject = new BaseObjectState(this);
            redundancyBaseObject.BrowseName = new QualifiedName(UFUAServerInfo.BrowserNames.RedundancyName, namespaceindex);
            redundancyBaseObject.DisplayName = redundancyBaseObject.BrowseName.Name;
            redundancyBaseObject.SymbolicName = redundancyBaseObject.BrowseName.Name;
            redundancyBaseObject.TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            redundancyBaseObject.ReferenceTypeId = null;
            redundancyBaseObject.NodeId = ModelUtils.ConstructIdForComponent(redundancyBaseObject, namespaceindex);
            redundancyBaseObject.AddReference(ReferenceTypeIds.Organizes, true, NodeId);
           
            redundancyActiveServerState = new SystemTagsMember<bool>(false, false, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RedundancyActiveServerStateName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RedundancyActiveServerStateDesc, String.Empty, UFUAServerInfo.BrowserNames.RedundancyActiveServerStateDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            redundancyActiveServerState.Create(
                context,
                redundancyActiveServerState.NodeId,
                new QualifiedName(redundancyActiveServerState.SymbolicName, namespaceindex),
                null,
                true);

            redundancyActiveServerHostName = new SystemTagsMember<string>(false, null, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RedundancyActiveServerHostNameName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RedundancyActiveServerHostNameDesc, String.Empty, UFUAServerInfo.BrowserNames.RedundancyActiveServerHostNameDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            redundancyActiveServerHostName.Create(
                context,
                redundancyActiveServerHostName.NodeId,
                new QualifiedName(redundancyActiveServerHostName.SymbolicName, namespaceindex),
                null,
                true);

            redundancyArrayAliveServerHostName = new SystemTagsMember<string[]>(false, null, this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RedundancyArrayAliveServerHostNameName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RedundancyArrayAliveServerHostNameDesc, String.Empty, UFUAServerInfo.BrowserNames.RedundancyArrayAliveServerHostNameDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead
            };

            redundancyArrayAliveServerHostName.Create(
                context,
                redundancyArrayAliveServerHostName.NodeId,
                new QualifiedName(redundancyArrayAliveServerHostName.SymbolicName, namespaceindex),
                null,
                true);

            redundancySwitchActiveServerMethod = new MethodState(this)
            {
                SymbolicName = UFUAServerInfo.BrowserNames.RedundancySwitchActiveServerMethodName,
                Description = new LocalizedText(UFUAServerInfo.BrowserNames.RedundancySwitchActiveServerMethodDesc, String.Empty, UFUAServerInfo.BrowserNames.RedundancySwitchActiveServerMethodDesc),
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                UserExecutable = true,
                Executable = true
            };

            redundancySwitchActiveServerMethod.Create(
                context,
                redundancySwitchActiveServerMethod.NodeId,
                new QualifiedName(redundancySwitchActiveServerMethod.SymbolicName, namespaceindex),
                null,
                true);

        }
        #endregion

        #region Public Methods
        public void SetInUse(ISystemContext context, NodeState node, double samplingIntervall)
        {
            var systemTag = node as SystemTagsMember<long>;
            if (systemTag != null && systemTag.MinimumSamplingInterval > 0)
            {
                systemTag.SetSampling(samplingIntervall);
            }
        }

        public void SetNotInUse(ISystemContext context, NodeState node)
        {
            SetInUse(context, node, 0.0);
        }

        public List<NodeId> GetRedundancyPrivateNodeIds()
        {
            if (redundancyPrivateNodeIds == null)
            {
                redundancyPrivateNodeIds = new List<NodeId>()
                {
                    redundancyBaseObject.NodeId,
                    RedundancyActiveServerState.NodeId,
                    RedundancyActiveServerHostName.NodeId,
                    RedundancyArrayAliveServerHostName.NodeId,
                    AlarmsSoundState.NodeId
                };
            }

            return redundancyPrivateNodeIds;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Allow to enable or disable the alarms beeper in the server.
        /// </summary>
        public SystemTagsMember<bool> AlarmsSoundState
        {
            get
            {
                return alarmsSoundState;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsSoundState, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsSoundState = value;
            }
        }

        /// <summary>
        /// Beeper are playing in the server.
        /// </summary>
        public SystemTagsMember<bool> AlarmsSoundBuzzing
        {
            get
            {
                return alarmsSoundBuzzing;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsSoundBuzzing, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsSoundBuzzing = value;
            }
        }

        /// <summary>
        /// Number of enabled alarms in the server.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumEnabled
        {
            get
            {
                return alarmsNumEnabled;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumEnabled, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumEnabled = value;
            }
        }

        /// <summary>
        /// Number of active alarms in the server with 'On' state.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumActiveOn
        {
            get
            {
                return alarmsNumActiveOn;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumActiveOn, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumActiveOn = value;
            }
        }

        /// <summary>
        /// Number of active alarms in the server with 'Off' state.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumActiveOff
        {
            get
            {
                return alarmsNumActiveOff;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumActiveOff, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumActiveOff = value;
            }
        }

        /// <summary>
        /// Number of active alarms in the server with 'On' or 'Off' state.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumActiveOnOff
        {
            get
            {
                return alarmsNumActiveOnOff;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumActiveOnOff, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumActiveOnOff = value;
            }
        }

        /// <summary>
        /// Number of shelved alarms in the server.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumShelved
        {
            get
            {
                return alarmsNumShelved;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumShelved, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumShelved = value;
            }
        }

        /// <summary>
        /// Allow to force the switch from active server to next server.
        /// </summary>
        public MethodState RuntimeAlarmSettingsUpdateMethod
        {
            get
            {
                return runtimeAlarmSettingsUpdateMethod;
            }

            set
            {
                if (!Object.ReferenceEquals(runtimeAlarmSettingsUpdateMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                runtimeAlarmSettingsUpdateMethod = value;
            }
        }

        /// <summary>
        /// Number of active alarms in the server with 'Not Acknowledge' state.
        /// </summary>
        public SystemTagsMember<long> AlarmsNumNotAck
        {
            get
            {
                return alarmsNumNotAck;
            }
            set
            {
                if (!Object.ReferenceEquals(alarmsNumNotAck, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                alarmsNumNotAck = value;
            }
        }

        /// <summary>
        /// Number of enabled messages in the server.
        /// </summary>
        public SystemTagsMember<long> MessagesNumEnabled
        {
            get
            {
                return messagesNumEnabled;
            }
            set
            {
                if (!Object.ReferenceEquals(messagesNumEnabled, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                messagesNumEnabled = value;
            }
        }

        /// <summary>
        /// Number of active messages in the server with 'On' state.
        /// </summary>
        public SystemTagsMember<long> MessagesNumActiveOn
        {
            get
            {
                return messagesNumActiveOn;
            }
            set
            {
                if (!Object.ReferenceEquals(messagesNumActiveOn, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                messagesNumActiveOn = value;
            }
        }

        /// <summary>
        /// Number of shelved messages in the server.
        /// </summary>
        public SystemTagsMember<long> MessagesNumShelved
        {
            get
            {
                return messagesNumShelved;
            }
            set
            {
                if (!Object.ReferenceEquals(messagesNumShelved, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                messagesNumShelved = value;
            }
        }

        /// <summary>
        /// Number of entries are waiting in the historical cache for inserting in the database.
        /// </summary>
        public SystemTagsMember<long> RecordHistoricalEntriesPending
        {
            get
            {
                return recordHistoricalEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(recordHistoricalEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                recordHistoricalEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of entries that historical worked thread is processing for inserting in the database. 
        /// </summary>
        public SystemTagsMember<long> RecordHistoricalEntriesRunning
        {
            get
            {
                return recordHistoricalEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(recordHistoricalEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                recordHistoricalEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of fails entries are waiting in the historical cache for retrying.
        /// </summary>
        public SystemTagsMember<long> FailsHistoricalEntriesPending
        {
            get
            {
                return failsHistoricalEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(failsHistoricalEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                failsHistoricalEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of fails entries that historical worked thread is processing for retrying.
        /// </summary>
        public SystemTagsMember<long> FailsHistoricalEntriesRunning
        {
            get
            {
                return failsHistoricalEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(failsHistoricalEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                failsHistoricalEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of delete entries are waiting in the historical cache for deleting from database.
        /// </summary>
        public SystemTagsMember<long> DeleteHistoricalEntriesPending
        {
            get
            {
                return deleteHistoricalEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(deleteHistoricalEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                deleteHistoricalEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of delte entries that historical worked thread is processing for deleting from database.
        /// </summary>
        public SystemTagsMember<long> DeleteHistoricalEntriesRunning
        {
            get
            {
                return deleteHistoricalEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(deleteHistoricalEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                deleteHistoricalEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of entries are waiting in the historical cache for flushing safely to xml file.
        /// </summary>
        public SystemTagsMember<long> FlushHistoricalEntriesPending
        {
            get
            {
                return flushHistoricalEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(flushHistoricalEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                flushHistoricalEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of entries that historical worked thread is processing for flushing safely to xml file.
        /// </summary>
        public SystemTagsMember<long> FlushHistoricalEntriesRunning
        {
            get
            {
                return flushHistoricalEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(flushHistoricalEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                flushHistoricalEntriesRunning = value;
            }
        }

        /// <summary>
        /// The historical cache is full and new entries will be lost.
        /// </summary>
        public SystemTagsMember<bool> DischargingHistoricalEntriesMode
        {
            get
            {
                return dischargingHistoricalEntriesMode;
            }
            set
            {
                if (!Object.ReferenceEquals(dischargingHistoricalEntriesMode, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                dischargingHistoricalEntriesMode = value;
            }
        }

        /// <summary>
        /// Number of entries are waiting in the event cache for inserting in the database.
        /// </summary>
        public SystemTagsMember<long> RecordEventEntriesPending
        {
            get
            {
                return recordEventEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(recordEventEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                recordEventEntriesPending = value;
            }
        }

        /// <summary>
        //// Number of entries that event worked thread is processing for inserting in the database. 
        /// </summary>
        public SystemTagsMember<long> RecordEventEntriesRunning
        {
            get
            {
                return recordEventEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(recordEventEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                recordEventEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of fails entries are waiting in the event cache for retrying.
        /// </summary>
        public SystemTagsMember<long> FailsEventEntriesPending
        {
            get
            {
                return failsEventEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(failsEventEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                failsEventEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of fails entries that event worked thred is processing for retrying.
        /// </summary>
        public SystemTagsMember<long> FailsEventEntriesRunning
        {
            get
            {
                return failsEventEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(failsEventEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                failsEventEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of delete entries are waiting in the event cache for deleting from database.
        /// </summary>
        public SystemTagsMember<long> DeleteEventEntriesPending
        {
            get
            {
                return deleteEventEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(deleteEventEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                deleteEventEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of delte entries that event worked thread is processing for deleting from database.
        /// </summary>
        public SystemTagsMember<long> DeleteEventEntriesRunning
        {
            get
            {
                return deleteEventEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(deleteEventEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                deleteEventEntriesRunning = value;
            }
        }

        /// <summary>
        /// Number of entries are waiting in the event cache for flushing safely to xml file.
        /// </summary>
        public SystemTagsMember<long> FlushEventEntriesPending
        {
            get
            {
                return flushEventEntriesPending;
            }
            set
            {
                if (!Object.ReferenceEquals(flushEventEntriesPending, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                flushEventEntriesPending = value;
            }
        }

        /// <summary>
        /// Number of entries that event worked thread is processing for flushing safely to xml file.
        /// </summary>
        public SystemTagsMember<long> FlushEventEntriesRunning
        {
            get
            {
                return flushEventEntriesRunning;
            }
            set
            {
                if (!Object.ReferenceEquals(flushEventEntriesRunning, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                flushEventEntriesRunning = value;
            }
        }

        /// <summary>
        /// The event flushing cache is full and new entries will be lost.
        /// </summary>
        public SystemTagsMember<bool> DischargingEventEntriesMode
        {
            get
            {
                return dischargingEventEntriesMode;
            }
            set
            {
                if (!Object.ReferenceEquals(dischargingEventEntriesMode, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                dischargingEventEntriesMode = value;
            }
        }

        /// <summary>
        /// Allow to read the redundancy server state.
        /// </summary>
        public SystemTagsMember<bool> RedundancyActiveServerState
        {
            get
            {
                return redundancyActiveServerState;
            }

            set
            {
                if (!Object.ReferenceEquals(redundancyActiveServerState, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                redundancyActiveServerState = value;
            }
        }

        /// <summary>
        /// Allow to read the redundancy server active host name.
        /// </summary>
        public SystemTagsMember<string> RedundancyActiveServerHostName
        {
            get
            {
                return redundancyActiveServerHostName;
            }

            set
            {
                if (!Object.ReferenceEquals(redundancyActiveServerHostName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                redundancyActiveServerHostName = value;
            }
        }

        /// <summary>
        /// Allow to read the redundancy array server alive host names.
        /// </summary>
        public SystemTagsMember<string[]> RedundancyArrayAliveServerHostName
        {
            get
            {
                return redundancyArrayAliveServerHostName;
            }

            set
            {
                if (!Object.ReferenceEquals(redundancyArrayAliveServerHostName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                redundancyArrayAliveServerHostName = value;
            }
        }

#if !NET_STANDARD
        /// <summary>
        /// Allow to force the switch from active server to next server.
        /// </summary>
        public MethodState RedundancySwitchActiveServerMethod
        {
            get
            {
                return redundancySwitchActiveServerMethod;
            }

            set
            {
                if (!Object.ReferenceEquals(redundancySwitchActiveServerMethod, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                redundancySwitchActiveServerMethod = value;
            }
        }
#endif

        /// <summary>
        /// Allow to read the dynamic tag count.
        /// </summary>
        public SystemTagsMember<long> DynamicTagCount
        {
            get
            {
                return dynamicTagCount;
            }

            set
            {
                if (!Object.ReferenceEquals(dynamicTagCount, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                dynamicTagCount = value;
            }
        }

        /// <summary>
        /// Allow to read the recording in error state.
        /// </summary>
        public SystemTagsMember<bool> RecordingInError
        {
            get
            {
                return recordingInError;
            }

            set
            {
                if (!Object.ReferenceEquals(recordingInError, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                recordingInError = value;
            }
        }

        /// <summary>
        /// Allow to read the license serial number.
        /// </summary>
        public SystemTagsMember<string> LicenseSerialNumber
        {
            get
            {
                return licenseSerialNumber;
            }

            set
            {
                if (!Object.ReferenceEquals(licenseSerialNumber, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                licenseSerialNumber = value;
            }
        }

        /// <summary>
        /// Allow to read the active WebHMI session count.
        /// </summary>
        public SystemTagsMember<long> ActiveSessionsWebHMI
        {
            get
            {
                return activeSessionsWebHMI;
            }

            set
            {
                if (!Object.ReferenceEquals(activeSessionsWebHMI, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                activeSessionsWebHMI = value;
            }
        }

        /// <summary>
        /// Allow to read the active WebClient session count.
        /// </summary>
        public SystemTagsMember<long> ActiveSessionsWebClient
        {
            get
            {
                return activeSessionsWebClient;
            }

            set
            {
                if (!Object.ReferenceEquals(activeSessionsWebClient, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                activeSessionsWebClient = value;
            }
        }

        /// <summary>
        /// Allow to read the active WebHMI session count.
        /// </summary>
        public SystemTagsMember<long> ActiveSessionsWebApps
        {
            get
            {
                return activeSessionsWebApps;
            }

            set
            {
                if (!Object.ReferenceEquals(activeSessionsWebApps, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                activeSessionsWebApps = value;
            }
        }

        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (systemTagBaseObject != null)
            {
                if (!children.Contains(systemTagBaseObject))
                    children.Add(systemTagBaseObject);

                if (dynamicTagCount != null && 
                    systemTagBaseObject.FindChild(context, dynamicTagCount.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(dynamicTagCount);
                }

                if (recordingInError != null &&
                    systemTagBaseObject.FindChild(context, recordingInError.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(recordingInError);
                }

                if (licenseSerialNumber != null &&
                    systemTagBaseObject.FindChild(context, licenseSerialNumber.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(licenseSerialNumber);
                }

                if (activeSessionsWebHMI != null &&
                    systemTagBaseObject.FindChild(context, activeSessionsWebHMI.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(activeSessionsWebHMI);
                }

                if (activeSessionsWebClient != null &&
                    systemTagBaseObject.FindChild(context, activeSessionsWebClient.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(activeSessionsWebClient);
                }

                if (activeSessionsWebApps != null &&
                    systemTagBaseObject.FindChild(context, activeSessionsWebApps.BrowseName) == null)
                {
                    systemTagBaseObject.AddChild(activeSessionsWebApps);
                }
            }

            if (alarmsBaseObject != null)
            {
                if (!children.Contains(alarmsBaseObject))
                    children.Add(alarmsBaseObject);
                
                if (alarmsSoundState != null &&
                    alarmsBaseObject.FindChild(context, alarmsSoundState.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsSoundState);
                }
                if (alarmsSoundBuzzing != null &&
                    alarmsBaseObject.FindChild(context, alarmsSoundBuzzing.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsSoundBuzzing);
                }
                if (alarmsNumEnabled != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumEnabled.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumEnabled);
                }
                if (alarmsNumActiveOn != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumActiveOn.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumActiveOn);
                }
                if (alarmsNumActiveOff != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumActiveOff.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumActiveOff);
                }
                if (alarmsNumActiveOnOff != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumActiveOnOff.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumActiveOnOff);
                }
                if (alarmsNumShelved != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumShelved.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumShelved);
                }
                if (alarmsNumNotAck != null &&
                    alarmsBaseObject.FindChild(context, alarmsNumNotAck.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(alarmsNumNotAck);
                }
                if (runtimeAlarmSettingsUpdateMethod != null &&
                    alarmsBaseObject.FindChild(context, runtimeAlarmSettingsUpdateMethod.BrowseName) == null)
                {
                    alarmsBaseObject.AddChild(runtimeAlarmSettingsUpdateMethod);
                }
            }

            if (messagesBaseObject != null)
            {
                if (!children.Contains(messagesBaseObject))
                    children.Add(messagesBaseObject);

                if (messagesNumEnabled != null &&
                    messagesBaseObject.FindChild(context, messagesNumEnabled.BrowseName) == null)
                {
                    messagesBaseObject.AddChild(messagesNumEnabled);
                }
                if (messagesNumActiveOn != null &&
                    messagesBaseObject.FindChild(context, messagesNumActiveOn.BrowseName) == null)
                {
                    messagesBaseObject.AddChild(messagesNumActiveOn);
                }
                if (messagesNumShelved != null &&
                    messagesBaseObject.FindChild(context, messagesNumShelved.BrowseName) == null)
                {
                    messagesBaseObject.AddChild(messagesNumShelved);
                }
            }

            if (historianBaseObject != null)
            {
                if (!children.Contains(historianBaseObject))
                    children.Add(historianBaseObject);

                if (recordHistoricalEntriesPending != null &&
                    historianBaseObject.FindChild(context, recordHistoricalEntriesPending.BrowseName) == null)
                {
                    historianBaseObject.AddChild(recordHistoricalEntriesPending);
                }
                if (recordHistoricalEntriesRunning != null &&
                    historianBaseObject.FindChild(context, recordHistoricalEntriesRunning.BrowseName) == null)
                {
                    historianBaseObject.AddChild(recordHistoricalEntriesRunning);
                }
                if (failsHistoricalEntriesPending != null &&
                    historianBaseObject.FindChild(context, failsHistoricalEntriesPending.BrowseName) == null)
                {
                    historianBaseObject.AddChild(failsHistoricalEntriesPending);
                }
                if (failsHistoricalEntriesRunning != null &&
                    historianBaseObject.FindChild(context, failsHistoricalEntriesRunning.BrowseName) == null)
                {
                    historianBaseObject.AddChild(failsHistoricalEntriesRunning);
                }
                if (deleteHistoricalEntriesPending != null &&
                    historianBaseObject.FindChild(context, deleteHistoricalEntriesPending.BrowseName) == null)
                {
                    historianBaseObject.AddChild(deleteHistoricalEntriesPending);
                }
                if (deleteHistoricalEntriesRunning != null &&
                    historianBaseObject.FindChild(context, deleteHistoricalEntriesRunning.BrowseName) == null)
                {
                    historianBaseObject.AddChild(deleteHistoricalEntriesRunning);
                }
                if (flushHistoricalEntriesPending != null && 
                    historianBaseObject.FindChild(context, flushHistoricalEntriesPending.BrowseName) == null)
                {
                    historianBaseObject.AddChild(flushHistoricalEntriesPending);
                }
                if (flushHistoricalEntriesRunning != null &&
                    historianBaseObject.FindChild(context, flushHistoricalEntriesRunning.BrowseName) == null)
                {
                    historianBaseObject.AddChild(flushHistoricalEntriesRunning);
                }
                if (dischargingHistoricalEntriesMode != null &&
                    historianBaseObject.FindChild(context, dischargingHistoricalEntriesMode.BrowseName) == null)
                {
                    historianBaseObject.AddChild(dischargingHistoricalEntriesMode);
                }
            }

            if (eventLoggerBaseObject != null)
            {
                if (!children.Contains(eventLoggerBaseObject))
                    children.Add(eventLoggerBaseObject);

                if (recordEventEntriesPending != null &&
                    eventLoggerBaseObject.FindChild(context, recordEventEntriesPending.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(recordEventEntriesPending);
                }
                if (recordEventEntriesRunning != null &&
                    eventLoggerBaseObject.FindChild(context, recordEventEntriesRunning.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(recordEventEntriesRunning);
                }
                if (failsEventEntriesPending != null &&
                    eventLoggerBaseObject.FindChild(context, failsEventEntriesPending.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(failsEventEntriesPending);
                }
                if (failsEventEntriesRunning != null &&
                    eventLoggerBaseObject.FindChild(context, failsEventEntriesRunning.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(failsEventEntriesRunning);
                }
                if (deleteEventEntriesPending != null &&
                    eventLoggerBaseObject.FindChild(context, deleteEventEntriesPending.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(deleteEventEntriesPending);
                }
                if (deleteEventEntriesRunning != null &&
                    eventLoggerBaseObject.FindChild(context, deleteEventEntriesRunning.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(deleteEventEntriesRunning);
                }
                if (flushEventEntriesPending != null &&
                    eventLoggerBaseObject.FindChild(context, flushEventEntriesPending.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(flushEventEntriesPending);
                }
                if (flushEventEntriesRunning != null &&
                    eventLoggerBaseObject.FindChild(context, flushEventEntriesRunning.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(flushEventEntriesRunning);
                }
                if (dischargingEventEntriesMode != null &&
                    eventLoggerBaseObject.FindChild(context, dischargingEventEntriesMode.BrowseName) == null)
                {
                    eventLoggerBaseObject.AddChild(dischargingEventEntriesMode);
                }
            }

            if (redundancyBaseObject != null)
            {
                if (!children.Contains(redundancyBaseObject))
                    children.Add(redundancyBaseObject);

                if (redundancyActiveServerState != null &&
                    redundancyBaseObject.FindChild(context, redundancyActiveServerState.BrowseName) == null)
                {
                    redundancyBaseObject.AddChild(redundancyActiveServerState);
                }
                if (redundancyActiveServerHostName != null &&
                    redundancyBaseObject.FindChild(context, redundancyActiveServerHostName.BrowseName) == null)
                {
                    redundancyBaseObject.AddChild(redundancyActiveServerHostName);
                }
                if (redundancyArrayAliveServerHostName != null && 
                    redundancyBaseObject.FindChild(context, redundancyArrayAliveServerHostName.BrowseName) == null)
                {
                    redundancyBaseObject.AddChild(redundancyArrayAliveServerHostName);
                }
                if (redundancySwitchActiveServerMethod != null &&
                    redundancyBaseObject.FindChild(context, redundancySwitchActiveServerMethod.BrowseName) == null)
                {
                    redundancyBaseObject.AddChild(redundancySwitchActiveServerMethod);
                }
            }

            base.GetChildren(context, children);
        }
        #endregion

        #region Private Fields
        List<NodeId> redundancyPrivateNodeIds;

        BaseObjectState alarmsBaseObject;
        SystemTagsMember<bool> alarmsSoundState;
        SystemTagsMember<bool> alarmsSoundBuzzing;
        SystemTagsMember<long> alarmsNumEnabled;
        SystemTagsMember<long> alarmsNumActiveOn;
        SystemTagsMember<long> alarmsNumActiveOff;
        SystemTagsMember<long> alarmsNumActiveOnOff;
        SystemTagsMember<long> alarmsNumShelved;
        SystemTagsMember<long> alarmsNumNotAck;
        MethodState runtimeAlarmSettingsUpdateMethod;

        BaseObjectState messagesBaseObject;
        SystemTagsMember<long> messagesNumEnabled;
        SystemTagsMember<long> messagesNumActiveOn;
        SystemTagsMember<long> messagesNumShelved;

        BaseObjectState historianBaseObject;
        SystemTagsMember<long> recordHistoricalEntriesPending;
        SystemTagsMember<long> recordHistoricalEntriesRunning;
        SystemTagsMember<long> failsHistoricalEntriesPending;
        SystemTagsMember<long> failsHistoricalEntriesRunning;
        SystemTagsMember<long> deleteHistoricalEntriesPending;
        SystemTagsMember<long> deleteHistoricalEntriesRunning;
        SystemTagsMember<long> flushHistoricalEntriesPending;
        SystemTagsMember<long> flushHistoricalEntriesRunning;
        SystemTagsMember<bool> dischargingHistoricalEntriesMode;

        BaseObjectState eventLoggerBaseObject;
        SystemTagsMember<long> recordEventEntriesPending;
        SystemTagsMember<long> recordEventEntriesRunning;
        SystemTagsMember<long> failsEventEntriesPending;
        SystemTagsMember<long> failsEventEntriesRunning;
        SystemTagsMember<long> deleteEventEntriesPending;
        SystemTagsMember<long> deleteEventEntriesRunning;
        SystemTagsMember<long> flushEventEntriesPending;
        SystemTagsMember<long> flushEventEntriesRunning;
        SystemTagsMember<bool> dischargingEventEntriesMode;

        BaseObjectState redundancyBaseObject;
		SystemTagsMember<bool> redundancyActiveServerState;
        SystemTagsMember<string> redundancyActiveServerHostName;
        SystemTagsMember<string[]> redundancyArrayAliveServerHostName;
        MethodState redundancySwitchActiveServerMethod;

        BaseObjectState systemTagBaseObject;
        SystemTagsMember<long> dynamicTagCount;
        SystemTagsMember<bool> recordingInError;
        SystemTagsMember<string> licenseSerialNumber;
        SystemTagsMember<long> activeSessionsWebHMI;
        SystemTagsMember<long> activeSessionsWebClient;
        SystemTagsMember<long> activeSessionsWebApps;

        #endregion
    }
}
