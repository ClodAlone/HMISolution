// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua.Server;
using RuntimeViewer.Shared.Services;
using SharedModels;
using SimpleOpcFileServer;
using Xunit;

namespace Tests.Server;

public class AlarmActionsIntegrationTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "alarm_actions_" + Guid.NewGuid().ToString("N"));
    private readonly OpcRuntimeClient _client = new();
    private TestServer? _server;
    private ServiceProvider? _services;

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        var endpoint = $"opc.tcp://127.0.0.1:{port}/AlarmActions";
        _services = new ServiceCollection().BuildServiceProvider();
        var telemetry = new ServiceProviderTelemetryContext(_services);
        var config = new ApplicationConfiguration(telemetry)
        {
            ApplicationName = "AlarmActionsTest",
            ApplicationUri = "urn:localhost:AlarmActionsTest",
            ApplicationType = ApplicationType.ClientAndServer,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_directory, "pki", "own"),
                    SubjectName = "CN=AlarmActionsTest"
                },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "trusted") },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "issuers") },
                RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "rejected") }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = [endpoint],
                SecurityPolicies = [new ServerSecurityPolicy { SecurityMode = MessageSecurityMode.None, SecurityPolicyUri = SecurityPolicies.None }],
                UserTokenPolicies = [new UserTokenPolicy(UserTokenType.Anonymous)]
            },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 30000 }
        };
        await config.ValidateAsync(ApplicationType.ClientAndServer);
        var application = new ApplicationInstance(config, telemetry);
        await application.CheckApplicationInstanceCertificatesAsync(false, 2048);
        _server = new TestServer(telemetry, Path.Combine(_directory, "nodes.json"));
        await application.StartAsync(_server);
        var description = CoreClientUtils.SelectEndpoint(config, endpoint, false, 10000);
        var session = await new DefaultSessionFactory(telemetry).CreateAsync(config,
            new ConfiguredEndpoint(null, description, EndpointConfiguration.Create(config)), false,
            "AlarmActionsTest", 30000, new UserIdentity(new AnonymousIdentityToken()), default);
        var field = typeof(OpcRuntimeClient).GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        field.SetValue(_client, session);
    }

    [Theory]
    [InlineData(AlarmTriggerType.Limit)]
    [InlineData(AlarmTriggerType.Condition)]
    public async Task AckResetAndShelve_OperateOnlyOnSelectedCondition(AlarmTriggerType triggerType)
    {
        var manager = _server!.Manager!;
        var selected = manager.AddAlarm("Selected", triggerType);
        var other = manager.AddAlarm("Other", triggerType);
        manager.SetValue(selected, 85.123456789);
        manager.SetValue(other, 100);

        var rows = await _client.CollectDetailedAlarmsAsync();
        Assert.Equal(2, rows.Count);
        var alarm = Assert.Single(rows, a => a.SourceName == "Selected");
        Assert.False(alarm.ConditionId.IsNull);
        Assert.Equal(alarm.ConditionId.ToString(), alarm.Id);
        Assert.False(alarm.IsAcked);
        Assert.False(alarm.IsConfirmed);
        var originalMessage = alarm.Message;

        Assert.True(await _client.AcknowledgeAlarmAsync(alarm.ConditionId, alarm.EventId), _client.AlarmErrorMessage);
        rows = await _client.CollectDetailedAlarmsAsync();
        alarm = Assert.Single(rows, a => a.SourceName == "Selected");
        Assert.True(alarm.IsAcked);
        Assert.Equal(originalMessage, alarm.Message);
        Assert.False(Assert.Single(rows, a => a.SourceName == "Other").IsAcked);

        Assert.True(await _client.ConfirmAlarmAsync(alarm.ConditionId, alarm.EventId), _client.AlarmErrorMessage);
        rows = await _client.CollectDetailedAlarmsAsync();
        alarm = Assert.Single(rows, a => a.SourceName == "Selected");
        Assert.True(alarm.IsConfirmed);
        Assert.True(alarm.IsActive);
        Assert.False(Assert.Single(rows, a => a.SourceName == "Other").IsConfirmed);

        var serverAlarm = Assert.Single(manager.GetActiveAlarms(), a => a.NodeId == alarm.ConditionId);
        Assert.Equal(ReferenceTypeIds.HasComponent, serverAlarm.ShelvingState.ReferenceTypeId);
        Assert.Equal(new QualifiedName(BrowseNames.ShelvingState), serverAlarm.ShelvingState.BrowseName);
        Assert.True(await _client.ShelveAlarmAsync(alarm.ConditionId, 15), _client.AlarmErrorMessage);
        rows = await _client.CollectDetailedAlarmsAsync();
        alarm = Assert.Single(rows, a => a.SourceName == "Selected");
        Assert.True(alarm.IsShelved);
        Assert.True(serverAlarm.SuppressedOrShelved.Value);
        Assert.False(Assert.Single(rows, a => a.SourceName == "Other").IsShelved);
        Assert.True(await _client.UnshelveAlarmAsync(alarm.ConditionId), _client.AlarmErrorMessage);
        rows = await _client.CollectDetailedAlarmsAsync();
        Assert.False(Assert.Single(rows, a => a.SourceName == "Selected").IsShelved);
        Assert.False(serverAlarm.SuppressedOrShelved.Value);

        manager.SetValue(selected, 50);
        rows = await _client.CollectDetailedAlarmsAsync();
        Assert.Equal("Other", Assert.Single(rows).SourceName);
    }

    [Fact]
    public async Task BadEventIdAndInvalidDuration_AreReportedWithoutChangingAlarm()
    {
        var manager = _server!.Manager!;
        var source = manager.AddAlarm("Selected", AlarmTriggerType.Limit);
        manager.SetValue(source, 100);
        var alarm = Assert.Single(await _client.CollectDetailedAlarmsAsync());

        Assert.False(await _client.AcknowledgeAlarmAsync(alarm.ConditionId, Guid.NewGuid().ToByteArray()));
        Assert.Contains("BadEventIdUnknown", _client.AlarmErrorMessage);
        Assert.False(await _client.ShelveAlarmAsync(alarm.ConditionId, 0));
        Assert.Contains("positive", _client.AlarmErrorMessage);
        var unchanged = Assert.Single(await _client.CollectDetailedAlarmsAsync());
        Assert.False(unchanged.IsAcked);
        Assert.False(unchanged.IsShelved);

        using var viewer = new RuntimeViewer.Shared.Components.Viewer.AlarmListWidget();
        var opc = viewer.GetType().GetProperty("Opc", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(opc);
        opc.SetValue(viewer, _client);
        alarm.IsSelected = true;
        alarm.EventId = Guid.NewGuid().ToByteArray();
        SetField(viewer, "_alarms", new List<AlarmEntry> { alarm });
        SetField(viewer, "_initialLoadDone", true);
        await InvokeViewerAsync(viewer, "AckSelected");
        Assert.Contains("BadEventIdUnknown", GetField<string?>(viewer, "_errorMessage"));
        var refreshed = Assert.Single(GetField<List<AlarmEntry>>(viewer, "_alarms"));
        Assert.True(refreshed.IsSelected);
        Assert.False(refreshed.IsAcked);
    }

    [Fact]
    public async Task ViewerActions_PreserveSelectionAcrossRefreshAndProcessOnlySelectedRows()
    {
        var manager = _server!.Manager!;
        foreach (var name in new[] { "First", "Other", "Last" })
            manager.SetValue(manager.AddAlarm(name, AlarmTriggerType.Limit), 100);
        var rows = await _client.CollectDetailedAlarmsAsync();
        foreach (var row in rows)
            row.IsSelected = row.SourceName != "Other";
        rows.Reverse();
        var viewer = new RuntimeViewer.Shared.Components.Viewer.AlarmListWidget();
        var opc = viewer.GetType().GetProperty("Opc", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(opc);
        opc.SetValue(viewer, _client);
        SetField(viewer, "_alarms", rows);
        SetField(viewer, "_initialLoadDone", true);

        await InvokeViewerAsync(viewer, "RefreshAlarms");
        Assert.Equal(new[] { "First", "Last" }, GetField<List<AlarmEntry>>(viewer, "_alarms")
            .Where(a => a.IsSelected).Select(a => a.SourceName).OrderBy(n => n));
        await InvokeViewerAsync(viewer, "AckSelected");
        Assert.Null(GetField<string?>(viewer, "_errorMessage"));
        Assert.All(GetField<List<AlarmEntry>>(viewer, "_alarms"), a => Assert.Equal(a.SourceName != "Other", a.IsAcked));
        await InvokeViewerAsync(viewer, "ConfirmSelected");
        Assert.Null(GetField<string?>(viewer, "_errorMessage"));
        Assert.All(GetField<List<AlarmEntry>>(viewer, "_alarms"), a => Assert.Equal(a.SourceName != "Other", a.IsConfirmed));
        await InvokeViewerAsync(viewer, "ShelveSelected");
        Assert.Null(GetField<string?>(viewer, "_errorMessage"));
        Assert.All(GetField<List<AlarmEntry>>(viewer, "_alarms"), a => Assert.Equal(a.SourceName != "Other", a.IsShelved));
        await InvokeViewerAsync(viewer, "UnshelveSelected");
        Assert.Null(GetField<string?>(viewer, "_errorMessage"));
        Assert.All(GetField<List<AlarmEntry>>(viewer, "_alarms"), a => Assert.False(a.IsShelved));
        viewer.Dispose();
    }

    [Fact]
    public async Task TimedShelving_AutomaticallyUnshelvesWithoutChangingOtherAlarms()
    {
        var manager = _server!.Manager!;
        foreach (var name in new[] { "Selected", "Other" })
            manager.SetValue(manager.AddAlarm(name, AlarmTriggerType.Limit), 100);
        var selected = Assert.Single(manager.GetActiveAlarms(), a => a.SourceName.Value == "Selected");
        var call = typeof(OpcRuntimeClient).GetMethod("SessionCallAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(call);
        var task = Assert.IsAssignableFrom<Task<IList<object>>>(call.Invoke(_client,
            [selected.ShelvingState.NodeId, selected.ShelvingState.TimedShelve.NodeId, new object[] { 2000.0 }]));
        await task;
        Assert.Equal(ObjectIds.ShelvedStateMachineType_TimedShelved, selected.ShelvingState.CurrentState.Id.Value);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (selected.ShelvingState.CurrentState.Id.Value != ObjectIds.ShelvedStateMachineType_Unshelved)
            await Task.Delay(25, timeout.Token);

        var rows = await _client.CollectDetailedAlarmsAsync();
        Assert.Equal(2, rows.Count);
        Assert.All(rows, a => Assert.False(a.IsShelved));
        Assert.All(rows, a => Assert.True(a.IsActive));
    }

    private static async Task InvokeViewerAsync(object viewer, string name)
    {
        var method = viewer.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        await Assert.IsAssignableFrom<Task>(method.Invoke(viewer, null));
    }

    private static void SetField(object target, string name, object value)
    {
        var field = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        field.SetValue(target, value);
    }

    private static T GetField<T>(object target, string name)
    {
        var field = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        return (T)field.GetValue(target)!;
    }

    public async ValueTask DisposeAsync()
    {
        await _client.Disconnect();
        if (_server != null)
        {
            await _server.StopAsync();
            _server.Dispose();
        }
        _services?.Dispose();
        Directory.Delete(_directory, true);
    }

    private sealed class TestServer(ITelemetryContext telemetry, string configPath) : StandardServer(telemetry)
    {
        public TestNodeManager? Manager { get; private set; }
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Manager = new TestNodeManager(server, configuration, configPath);
            return new MasterNodeManager(server, configuration, null, Manager);
        }
    }

    private sealed class TestNodeManager(IServerInternal server, ApplicationConfiguration configuration, string path)
        : SimpleFileServerNodeManager(server, configuration, path)
    {
        public BaseDataVariableState AddAlarm(string name, AlarmTriggerType triggerType)
        {
            var ns = (ushort)SystemContext.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA");
            var parent = new BaseObjectState(null) { NodeId = new NodeId(name + ".Parent", ns) };
            var variable = new BaseDataVariableState(parent)
            {
                NodeId = new NodeId(name, ns), BrowseName = new QualifiedName(name, ns),
                DisplayName = new LocalizedText(name), DataType = DataTypeIds.Double,
                ValueRank = ValueRanks.Scalar, Value = 50.0
            };
            var cfg = new AlarmConfig
            {
                TriggerType = triggerType, HighLimit = 80, HighHighLimit = 95,
                LowLimit = 20, LowLowLimit = 5, Operator = ">", CompareValue = "80", Message = name + " alarm"
            };
            var create = typeof(SimpleFileServerNodeManager).GetMethod("CreateAlarmCondition", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(create);
            create.Invoke(this, [variable, cfg, name, parent, null]);
            return variable;
        }

        public void SetValue(BaseDataVariableState variable, double value)
        {
            variable.Value = value;
            variable.ClearChangeMasks(SystemContext, false);
        }
    }
}
