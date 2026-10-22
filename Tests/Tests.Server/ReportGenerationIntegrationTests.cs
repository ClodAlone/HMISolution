using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua.Server;
using RuntimeViewer.Shared.Services;
using SharedModels;
using SimpleOpcFileServer;
using Xunit;

namespace Tests.Server;

public sealed class ReportGenerationIntegrationTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "report_tests_" + Guid.NewGuid().ToString("N"));
    private readonly OpcRuntimeClient _client = new();
    private TestServer? _server;
    private ServiceProvider? _services;

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var endpoint = $"opc.tcp://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/Reports";
        listener.Stop();
        _services = new ServiceCollection().BuildServiceProvider();
        var telemetry = new ServiceProviderTelemetryContext(_services);
        var config = new ApplicationConfiguration(telemetry)
        {
            ApplicationName = "ReportTest",
            ApplicationUri = "urn:localhost:ReportTest",
            ApplicationType = ApplicationType.ClientAndServer,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(_directory, "pki", "own"),
                    SubjectName = "CN=ReportTest"
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
        _server = new TestServer(telemetry, _directory);
        await application.StartAsync(_server);
        var description = await CoreClientUtils.SelectEndpointAsync(config, endpoint, false, 10000,
            telemetry, TestContext.Current.CancellationToken);
        var session = await new DefaultSessionFactory(telemetry).CreateAsync(config,
            new ConfiguredEndpoint(null, description!, EndpointConfiguration.Create(config)), false,
            "ReportTest", 30000, new UserIdentity(new AnonymousIdentityToken()), default);
        typeof(OpcRuntimeClient).GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(_client, session);
    }

    [Fact]
    public async Task Generate_ReturnsFullServerReportAndDeliversIdenticalHtmlToDisk()
    {
        var html = await _client.GenerateReportAsync("Operations");
        Assert.StartsWith("<!DOCTYPE html>", html);
        Assert.Contains("Operations test report", html);
        Assert.Contains("Process snapshot", html);
        Assert.Contains("42.5", html);
        Assert.Contains("m3/h", html);
        Assert.Contains("<svg", html);
        Assert.Equal(html, await File.ReadAllTextAsync(Path.Combine(_directory, "Operations.html"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Command_GeneratesOnEveryClickAndPublishesRealContent()
    {
        var js = new RecordingJs();
        var commands = new CommandService(_client, js);
        var generated = new List<(string Name, string Html)>();
        commands.ReportGenerated += (name, html) => generated.Add((name, html));
        var command = new SymbolCommand { Action = "GenerateReport", TargetReport = "Operations" };
        await commands.ExecuteAsync(command);
        await commands.ExecuteAsync(command);
        Assert.Equal(2, generated.Count);
        Assert.All(generated, item =>
        {
            Assert.Equal("Operations", item.Name);
            Assert.Contains("42.5", item.Html);
            Assert.StartsWith("<!DOCTYPE html>", item.Html);
        });
        Assert.Empty(js.Alerts);
    }

    [Theory]
    [InlineData("Missing")]
    [InlineData("Disabled")]
    public async Task UnavailableReport_FailsRatherThanReturningPlaceholder(string name)
    {
        await Assert.ThrowsAsync<ServiceResultException>(() => _client.GenerateReportAsync(name));
        Assert.False(File.Exists(Path.Combine(_directory, name + ".html")));
        Assert.Contains(_client.DiagnosticLog, entry => entry.Contains("REPORT ERROR: " + name));
    }

    [Fact]
    public async Task FailedCommand_NotifiesUserAndDoesNotPublishSuccess()
    {
        var js = new RecordingJs();
        var commands = new CommandService(_client, js);
        var published = false;
        commands.ReportGenerated += (_, _) => published = true;
        await commands.ExecuteAsync(new SymbolCommand { Action = "GenerateReport", TargetReport = "Missing" });
        Assert.False(published);
        Assert.Contains("Report generation failed", Assert.Single(js.Alerts));
    }

    [Fact]
    public async Task DisconnectedAndEmptyRequests_FailExplicitly()
    {
        using var disconnected = new OpcRuntimeClient();
        await Assert.ThrowsAsync<InvalidOperationException>(() => disconnected.GenerateReportAsync("Operations"));
        await Assert.ThrowsAsync<ArgumentException>(() => _client.GenerateReportAsync(" "));
    }

    [Fact]
    public async Task Viewer_GenerateButtonAndCommandsDisplayOnlyMatchingServerReports()
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton(_client)
            .AddSingleton<IJSRuntime, RecordingJs>().AddSingleton<CommandService>().BuildServiceProvider();
        await using var viewer = new ReportRenderer(services);
        await using var other = new ReportRenderer(services);
        await viewer.StartAsync("Operations");
        await other.StartAsync("OtherReport");
        await viewer.GenerateAsync();
        Assert.Contains("srcdoc=<!DOCTYPE html>", viewer.Html);
        Assert.Contains("42.5", viewer.Html);
        Assert.Contains("<iframe>", viewer.Html);
        Assert.Contains("sandbox=", viewer.Html);
        Assert.DoesNotContain("srcdoc=", other.Html);

        await using var commandViewer = new ReportRenderer(services);
        await commandViewer.StartAsync("Operations");
        Assert.DoesNotContain("srcdoc=", commandViewer.Html);
        await services.GetRequiredService<CommandService>().ExecuteAsync(
            new SymbolCommand { Action = "GenerateReport", TargetReport = "Operations" });
        await commandViewer.Dispatcher.InvokeAsync(() => { });
        Assert.Contains("srcdoc=<!DOCTYPE html>", commandViewer.Html);
        Assert.Contains("Process snapshot", commandViewer.Html);
        Assert.DoesNotContain("srcdoc=", other.Html);
    }

    [Theory]
    [InlineData("Missing")]
    [InlineData("")]
    public async Task Viewer_FailedGenerationDisplaysErrorWithoutPlaceholder(string name)
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton(_client)
            .AddSingleton<IJSRuntime, RecordingJs>().AddSingleton<CommandService>().BuildServiceProvider();
        await using var viewer = new ReportRenderer(services);
        await viewer.StartAsync(name);
        await viewer.GenerateAsync();
        Assert.Contains("Report generation failed:", viewer.Html);
        Assert.DoesNotContain("srcdoc=", viewer.Html);
        Assert.DoesNotContain("Last:", viewer.Html);
        Assert.DoesNotContain("generated client-side", viewer.Html);
    }

    [Theory]
    [InlineData("SimulateProcess")]
    [InlineData("SimulateEnergy")]
    public void ReportDemo_SimulationScriptsCompileWithServerGlobals(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "HMISolution.slnx")))
            directory = directory.Parent;
        Assert.NotNull(directory);
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory.FullName, "docker", "samples", "ReportDemo", "nodes.json")));
        var code = document.RootElement.GetProperty("Scripts").EnumerateArray()
            .Single(script => script.GetProperty("Name").GetString() == name).GetProperty("Code").GetString()!;
        var options = ScriptOptions.Default.AddReferences(typeof(SimpleFileServerNodeManager).Assembly)
            .AddImports("System", "System.Collections.Generic", "System.Linq");
        var diagnostics = CSharpScript.Create(code, options, typeof(ScriptGlobals)).Compile();
        Assert.Empty(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
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

    private sealed class TestServer(ITelemetryContext telemetry, string directory) : StandardServer(telemetry)
    {
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
            => new(server, configuration, null, new TestNodeManager(server, configuration, directory));
    }

    private sealed class TestNodeManager(IServerInternal server, ApplicationConfiguration configuration, string directory)
        : SimpleFileServerNodeManager(server, configuration, Path.Combine(directory, "nodes.json"))
    {
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            var ns = (ushort)SystemContext.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA");
            typeof(SimpleFileServerNodeManager).GetField("_namespaceIndex", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, ns);
            var variable = new BaseDataVariableState(null)
            {
                NodeId = new NodeId("Plant.Flow", ns), BrowseName = new QualifiedName("Flow", ns),
                DisplayName = new LocalizedText("Flow"), DataType = DataTypeIds.Double,
                ValueRank = ValueRanks.Scalar, Value = new Variant(42.5)
            };
            var variables = (IDictionary<string, BaseDataVariableState>)typeof(SimpleFileServerNodeManager)
                .GetField("_variables", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this)!;
            variables.Add("Plant.Flow", variable);
            AddPredefinedNode(SystemContext, variable);
            var reports = new ReportManager(this);
            reports.Initialize([
                new ReportConfig
                {
                    Name = "Operations", Title = "Operations test report",
                    Sections = [
                        new ReportSection { Type = "Header", Title = "Process snapshot" },
                        new ReportSection { Type = "Value", ValueVariablePath = "Plant.Flow", ValueUnit = "m3/h" },
                        new ReportSection { Type = "Chart", ChartVariablePaths = ["Plant.Flow"] }
                    ],
                    Delivery = new ReportDeliveryConfig { Method = "Disk", DiskPath = directory, FileNamePattern = "{ReportName}" }
                },
                new ReportConfig { Name = "Disabled", Enabled = false }
            ]);
            typeof(SimpleFileServerNodeManager).GetField("_reportManager", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, reports);
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out var references))
                externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
            typeof(SimpleFileServerNodeManager).GetMethod("CreateReportMethod", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(this, [references]);
        }
    }

    #pragma warning disable BL0006 // Exercise the report viewer's real button handler and render lifecycle.
    private sealed class ReportRenderer(IServiceProvider services) : Renderer(services, services.GetRequiredService<ILoggerFactory>())
    {
        private ReportViewerWidget? _widget;
        private int _root;
        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();
        public string Html { get; private set; } = "";
        public Task StartAsync(string reportName) => Dispatcher.InvokeAsync(async () =>
        {
            _widget = (ReportViewerWidget)InstantiateComponent(typeof(ReportViewerWidget));
            _root = AssignRootComponentId(_widget);
            await RenderRootComponentAsync(_root, ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ReportViewerWidget.ReportName)] = reportName
            }));
        });
        public Task GenerateAsync() => Dispatcher.InvokeAsync(async () =>
            await (Task)typeof(ReportViewerWidget).GetMethod("GenerateReport", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(_widget, null)!);
        protected override void HandleException(Exception exception) => throw new InvalidOperationException("Report widget render failed.", exception);
        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            var frames = GetCurrentRenderTreeFrames(_root);
            Html = string.Join(" ", frames.Array.Take(frames.Count).Select(frame => frame.FrameType switch
            {
                RenderTreeFrameType.Text => frame.TextContent,
                RenderTreeFrameType.Element => "<" + frame.ElementName + ">",
                RenderTreeFrameType.Attribute => frame.AttributeName + "=" + frame.AttributeValue,
                _ => ""
            }));
            return Task.CompletedTask;
        }
    }
#pragma warning restore BL0006

    private sealed class RecordingJs : IJSRuntime
    {
        public List<string> Alerts { get; } = [];
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Assert.Equal("alert", identifier);
            Alerts.Add(Assert.IsType<string>(Assert.Single(args!)));
            return ValueTask.FromResult(default(TValue)!);
        }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
