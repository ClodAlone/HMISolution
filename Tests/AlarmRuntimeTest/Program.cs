using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
#pragma warning disable CS0618

var appConfig = new ApplicationConfiguration
{
    ApplicationName = "Test", ApplicationUri = "urn:Test", ApplicationType = ApplicationType.Client,
    SecurityConfiguration = new SecurityConfiguration
    {
        ApplicationCertificate = new CertificateIdentifier { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(Path.GetTempPath(), "AlarmTestCerts", "own"), SubjectName = "CN=Test" },
        TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(Path.GetTempPath(), "AlarmTestCerts", "issuers") },
        TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(Path.GetTempPath(), "AlarmTestCerts", "trusted") },
        RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(Path.GetTempPath(), "AlarmTestCerts", "rejected") },
        AutoAcceptUntrustedCertificates = true
    },
    TransportConfigurations = new TransportConfigurationCollection(),
    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
};
await appConfig.Validate(ApplicationType.Client);
appConfig.CertificateValidator.CertificateValidation += (s, e) => e.Accept = true;
var endpoint = CoreClientUtils.SelectEndpoint(appConfig, "opc.tcp://localhost:14880/AlarmDemo", false, 15000);
var session = await Session.Create(appConfig, new ConfiguredEndpoint(null, endpoint, EndpointConfiguration.Create(appConfig)), false, "Test", 60000, null, null);
Console.WriteLine("Connected");
// Write all three above limits: Temp=100(>HH95), Pressure=10(>H8), Level=95(>H90)
var writes = new WriteValueCollection
{
    new WriteValue { NodeId = new NodeId("Alarm.Process.Temperature", 2), AttributeId = Attributes.Value, Value = new DataValue(new Variant("100")) },
    new WriteValue { NodeId = new NodeId("Alarm.Process.Pressure", 2), AttributeId = Attributes.Value, Value = new DataValue(new Variant("10")) },
    new WriteValue { NodeId = new NodeId("Alarm.Process.Level", 2), AttributeId = Attributes.Value, Value = new DataValue(new Variant("95")) }
};
session.Write(null, writes, out StatusCodeCollection wr, out DiagnosticInfoCollection wd);
for (int i = 0; i < wr.Count; i++) Console.WriteLine("Write[" + i + "]: " + wr[i]);
await Task.Delay(2000);

// Now do ConditionRefresh
var filter = new EventFilter();
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.EventId));
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.Retain));
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.SourceName));
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.NodeId));
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Message));
filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Severity));
var sub = new Subscription(session.DefaultSubscription) { PublishingInterval = 100 };
session.AddSubscription(sub); sub.Create();
var mi = new MonitoredItem(sub.DefaultItem) { StartNodeId = ObjectIds.Server, AttributeId = Attributes.EventNotifier, Filter = filter, QueueSize = 1000 };
var events = new List<EventFieldList>();
mi.Notification += (item, e) => { if (e.NotificationValue is EventFieldList efl) events.Add(efl); };
sub.AddItem(mi); sub.ApplyChanges();
session.Call(ObjectTypeIds.ConditionType, MethodIds.ConditionType_ConditionRefresh, new object[] { sub.Id });
await Task.Delay(2000);
int alarms = 0;
foreach (var evt in events)
{
    if (evt.EventFields.Count < 6) continue;
    var retain = evt.EventFields[1].Value;
    if (retain is not true) continue;
    alarms++;
    var src = evt.EventFields[2].Value;
    var cid = evt.EventFields[3].Value;
    var msg = evt.EventFields[4].Value;
    var sev = evt.EventFields[5].Value;
    Console.WriteLine("ALARM #" + alarms + ": src=" + src + " cid=" + cid + " sev=" + sev + " msg=" + msg);
}
Console.WriteLine("Total alarms: " + alarms);
sub.Delete(true); session.RemoveSubscription(sub); session.Close();
