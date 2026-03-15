using System;
using System.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

var app = new ApplicationInstance { ApplicationName = "BrowseTest", ApplicationType = ApplicationType.Client };
var config = new ApplicationConfiguration
{
    ApplicationName = "BrowseTest",
    ApplicationType = ApplicationType.Client,
    SecurityConfiguration = new SecurityConfiguration
    {
        ApplicationCertificate = new CertificateIdentifier { StoreType = CertificateStoreType.Directory, StorePath = "pki/own", SubjectName = "BrowseTest" },
        TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = "pki/trusted" },
        TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = "pki/issuer" },
        RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = "pki/rejected" }
    },
    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 30000 },
    TraceConfiguration = new TraceConfiguration()
};
await config.Validate(ApplicationType.Client);
config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = true; };
app.ApplicationConfiguration = config;
await app.CheckApplicationInstanceCertificates(false, 2048);

var dc = DiscoveryClient.Create(new Uri("opc.tcp://localhost:14840/SimpleOpcFileServer"));
var eps = dc.GetEndpoints(null);
dc.Dispose();
var ep = eps.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None) ?? eps.First();
var session = await Session.Create(config, new ConfiguredEndpoint(null, ep, EndpointConfiguration.Create(config)), false, "BrowseTest", 30000, new UserIdentity(new AnonymousIdentityToken()), null);

Console.WriteLine("Connected. Browsing Objects folder...");

void Browse(NodeId parentId, string indent)
{
    session.Browse(null, null, parentId, 0, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out var cp, out var refs);
    if (refs == null) return;
    foreach (var r in refs)
    {
        var nid = ExpandedNodeId.ToNodeId(r.NodeId, session.NamespaceUris);
        Console.WriteLine(indent + r.DisplayName + " [" + r.NodeClass + "] (" + nid + ")");
        if (r.NodeClass == NodeClass.Object && indent.Length < 12)
            Browse(nid, indent + "  ");
    }
}

Browse(ObjectIds.ObjectsFolder, "");
session.Close();
Console.WriteLine("Done.");
