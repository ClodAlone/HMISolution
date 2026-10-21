// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
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

public class UserAuthenticationIntegrationTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "opc_auth_" + Guid.NewGuid().ToString("N"));
    private ServiceProvider? _services;
    private TestServer? _server;
    private ApplicationConfiguration _config = null!;
    private ITelemetryContext _telemetry = null!;
    private string _endpoint = "";

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        _endpoint = $"opc.tcp://127.0.0.1:{port}/Authentication";
        _services = new ServiceCollection().BuildServiceProvider();
        _telemetry = new ServiceProviderTelemetryContext(_services);
        _config = new ApplicationConfiguration(_telemetry)
        {
            ApplicationName = "AuthenticationTest",
            ApplicationUri = "urn:localhost:AuthenticationTest",
            ApplicationType = ApplicationType.ClientAndServer,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_directory, "pki", "own"),
                    SubjectName = "CN=AuthenticationTest"
                },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "trusted") },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "issuers") },
                RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(_directory, "pki", "rejected") }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = [_endpoint],
                SecurityPolicies = [new ServerSecurityPolicy { SecurityMode = MessageSecurityMode.None, SecurityPolicyUri = SecurityPolicies.None }],
                UserTokenPolicies = [new UserTokenPolicy(UserTokenType.UserName)]
            },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 30000 }
        };
        await _config.ValidateAsync(ApplicationType.ClientAndServer);
        var application = new ApplicationInstance(_config, _telemetry);
        await application.CheckApplicationInstanceCertificatesAsync(false, 2048);
        _server = new TestServer(_telemetry, Path.Combine(_directory, "nodes.json"));
        await application.StartAsync(_server);
    }

    [Theory]
    [InlineData("hashed", "Test-Password-123")]
    [InlineData("legacy", "Legacy-Password-123")]
    [InlineData("unicode", "P\u00e4ssw\u00f6rd-\u5bc6\u78bc-123")]
    public async Task RuntimeClient_ConnectsWithValidCredentials(string username, string password)
    {
        using var client = new OpcRuntimeClient();
        await client.ConnectAsync(_endpoint, username, password);
        Assert.True(client.IsConnected, string.Join(Environment.NewLine, client.DiagnosticLog));
        Assert.Null(client.ErrorMessage);
    }

    [Fact]
    public async Task RuntimeClient_CanRetryAfterFailedLogin()
    {
        using var client = new OpcRuntimeClient();
        await client.ConnectAsync(_endpoint, "hashed", "incorrect");
        Assert.False(client.IsConnected);
        Assert.NotNull(client.ErrorMessage);

        await client.ConnectAsync(_endpoint, "hashed", "Test-Password-123");
        Assert.True(client.IsConnected, string.Join(Environment.NewLine, client.DiagnosticLog));
        Assert.Null(client.ErrorMessage);
    }

    [Theory]
    [InlineData("hashed", "incorrect")]
    [InlineData("hashed", "")]
    [InlineData("legacy", "incorrect")]
    [InlineData("missing", "Test-Password-123")]
    public async Task InvalidCredentials_AreRejected_AndValidRetrySucceeds(string username, string password)
    {
        var exception = await Assert.ThrowsAsync<ServiceResultException>(async () =>
        {
            using var session = await CreateSessionAsync(new UserIdentity(username, Encoding.UTF8.GetBytes(password)));
        });
        Assert.Equal(StatusCodes.BadUserAccessDenied, exception.StatusCode);

        using var retry = await CreateSessionAsync(new UserIdentity("hashed", Encoding.UTF8.GetBytes("Test-Password-123")));
        Assert.True(retry.Connected);
    }

    [Fact]
    public async Task AuthenticatedSession_CanChangeIdentity()
    {
        using var session = await CreateSessionAsync(new UserIdentity("hashed", Encoding.UTF8.GetBytes("Test-Password-123")));
        var sessionId = session.SessionId;
        await session.UpdateSessionAsync(new UserIdentity("legacy", Encoding.UTF8.GetBytes("Legacy-Password-123")),
            default, TestContext.Current.CancellationToken);
        Assert.True(session.Connected);
        Assert.Equal(sessionId, session.SessionId);
        Assert.Equal("legacy", session.Identity.DisplayName);
    }

    [Fact]
    public async Task AnonymousLogin_IsRejected_WhenNotAdvertised()
    {
        await Assert.ThrowsAsync<ServiceResultException>(async () =>
        {
            using var session = await CreateSessionAsync(new UserIdentity(new AnonymousIdentityToken()));
        });
    }

    private async Task<Opc.Ua.Client.ISession> CreateSessionAsync(UserIdentity identity)
    {
        var description = await CoreClientUtils.SelectEndpointAsync(_config, _endpoint, false, 10000,
            _telemetry, TestContext.Current.CancellationToken);
        Assert.NotNull(description);
        return await new DefaultSessionFactory(_telemetry).CreateAsync(_config,
            new ConfiguredEndpoint(null, description, EndpointConfiguration.Create(_config)), false,
            "AuthenticationTest", 30000, identity, default, TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
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
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
            => new(server, configuration, null, new TestNodeManager(server, configuration, configPath));
    }

    private sealed class TestNodeManager(IServerInternal server, ApplicationConfiguration configuration, string path)
        : SimpleFileServerNodeManager(server, configuration, path)
    {
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            var field = typeof(SimpleFileServerNodeManager).GetField("_users", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(field);
            var users = Assert.IsType<Dictionary<string, UserConfig>>(field.GetValue(this));
            users.Add("hashed", new UserConfig { Username = "hashed", PasswordHash = PasswordHasher.Hash("Test-Password-123") });
            users.Add("legacy", new UserConfig { Username = "legacy", Password = "Legacy-Password-123" });
            users.Add("unicode", new UserConfig { Username = "unicode", PasswordHash = PasswordHasher.Hash("P\u00e4ssw\u00f6rd-\u5bc6\u78bc-123") });
        }
    }
}
