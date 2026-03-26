using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;
using Serilog;
using SimpleOpcFileServer;
using SharedModels;
using System.Text.Json;

try
{
    // Set current directory to AppContext.BaseDirectory to ensure relative paths work correctly when running as a Windows Service
    Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    string configPath = "nodes.json";
    if (args.Length > 0 && !args[0].StartsWith("-"))
    {
        configPath = args[0];
    }
    if (!Path.IsPathRooted(configPath))
    {
        configPath = Path.Combine(AppContext.BaseDirectory, configPath);
    }
    configPath = Path.GetFullPath(configPath);

    // Install crash reporter early
    var crashDir = Path.Combine(Path.GetDirectoryName(configPath) ?? AppContext.BaseDirectory, "crash_reports");
    SharedModels.CrashReporter.Install(crashDir, "Server");

    // Validate license
    var licenseFile = SharedModels.LicenseManager.FindLicenseFile(configPath);
    var licenseStatus = SharedModels.LicenseManager.Validate(licenseFile);
    Console.WriteLine($"License: {licenseStatus.Tier} — {licenseStatus.Message}");
    if (!string.IsNullOrEmpty(licenseStatus.LicensedTo))
        Console.WriteLine($"Licensed to: {licenseStatus.LicensedTo}");

    if (!File.Exists(configPath))
    {
        Console.WriteLine($"Warning: Configuration file {configPath} not found.");
    }
    string configName = Path.GetFileNameWithoutExtension(configPath);
    string? configDir = Path.GetDirectoryName(configPath);
    if (string.IsNullOrEmpty(configDir))
    {
        configDir = AppContext.BaseDirectory;
    }

    // Ensure log directory exists
    string logDir = Path.Combine(configDir, "Logs");
    if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

    string logPath = Path.Combine(logDir, $"log-{configName}-.txt");

    var builder = Host.CreateApplicationBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
        .CreateLogger();

    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services.AddSingleton(new ServerConfig { NodesConfigFile = configPath });
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = $"SimpleOpcFileServer_{configName}";
    });

    builder.Services.AddHostedService<OpcUaWorker>();
    builder.Services.AddSingleton<OpcUaServerApp>();
    builder.Services.AddSingleton<CameraStreamService>();
    builder.Services.AddHostedService<CameraHostedService>();

    var host = builder.Build();

    // Start diagnostics HTTP endpoint and configure crash email
    try
    {
        int diagPort = 14841;
        if (File.Exists(configPath))
        {
            var diagJson = File.ReadAllText(configPath);
            var diagModel = JsonSerializer.Deserialize(diagJson, ServerJsonContext.Default.NodeModel);
            if (diagModel?.Server?.DiagnosticsPort is > 0 and var port)
                diagPort = port;
            else if (diagModel?.Server?.DiagnosticsPort == 0)
                diagPort = 0;

            // Configure crash email notifications
            SharedModels.CrashReporter.ConfigureEmail(diagModel?.Server?.CrashEmail);

            // Configure diagnostics rate limiting
            SimpleOpcFileServer.DiagnosticsCollector.Instance.ConfigureRateLimit(diagModel?.Server?.RateLimit);
        }
        SimpleOpcFileServer.DiagnosticsCollector.Instance.Start(diagPort);
    }
    catch (Exception ex)
    {
        Log.Warning("Failed to initialize diagnostics: {Error}", ex.Message);
    }

    host.Run();
    SimpleOpcFileServer.DiagnosticsCollector.Instance.Dispose();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    SharedModels.CrashReporter.Report(ex, "Fatal");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}


// --- Classes ---

public class ServerConfig
{
    public string NodesConfigFile { get; set; } = "";
}

public class OpcUaWorker : BackgroundService
{
    private readonly OpcUaServerApp _serverApp;
    private readonly ILogger<OpcUaWorker> _logger;
    private readonly ServerConfig _serverConfig;

    public OpcUaWorker(OpcUaServerApp serverApp, ILogger<OpcUaWorker> logger, ServerConfig serverConfig)
    {
        _serverApp = serverApp;
        _logger = logger;
        _serverConfig = serverConfig;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Worker starting server...");
            _logger.LogInformation("Using config: {ConfigPath}", _serverConfig.NodesConfigFile);

            await _serverApp.StartAsync(_serverConfig.NodesConfigFile);

            _logger.LogInformation("Server started.");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Ignored, shutting down
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Server failed to start or run.");
            Environment.Exit(1);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping server...");
        _serverApp.Stop();
        return base.StopAsync(cancellationToken);
    }
}
public class OpcUaServerApp
{
    private OpcUaServer _server;

    public void Stop()
    {
        _server?.Stop();
    }

    public async Task StartAsync(string configPath = "nodes.json")
    {
        var endpointUrl = "opc.tcp://localhost:14840/SimpleOpcFileServer";
        var enableAnonymous = true;

        var pkiRoot = "%LocalApplicationData%/SimpleOpcFileServer/pki";

        try
        {
            if (File.Exists(configPath))
            {
                var json = await File.ReadAllTextAsync(configPath);
                var nodeModel = JsonSerializer.Deserialize(json, ServerJsonContext.Default.NodeModel);
                if (nodeModel?.Server != null)
                {
                    if (!string.IsNullOrWhiteSpace(nodeModel.Server.EndpointUrl))
                    {
                        endpointUrl = nodeModel.Server.EndpointUrl;
                    }

                    enableAnonymous = nodeModel.Server.EnableAnonymous;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning("Failed to load server settings from config: {Message}. Using defaults.", ex.Message);
        }

        var application = new ApplicationInstance
        {
            ApplicationName = "SimpleOpcFileServer",
            ApplicationType = ApplicationType.Server
        };

        var config = new ApplicationConfiguration
        {
            ApplicationName = application.ApplicationName,
            ApplicationUri = $"urn:{Utils.GetHostName()}:{application.ApplicationName}",
            ApplicationType = application.ApplicationType,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = $"{pkiRoot}/own",
                    SubjectName = null
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = $"{pkiRoot}/trusted"
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = $"{pkiRoot}/issuer"
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = $"{pkiRoot}/rejected"
                }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ServerConfiguration = new ServerConfiguration
            {
                BaseAddresses = new StringCollection { endpointUrl },
                SecurityPolicies = new ServerSecurityPolicyCollection
                {
                    new ServerSecurityPolicy
                    {
                        SecurityMode = MessageSecurityMode.None,
                        SecurityPolicyUri = SecurityPolicies.None
                    }
                },
                UserTokenPolicies = new UserTokenPolicyCollection()
            },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

        application.ApplicationConfiguration = config;

        // Ensure PKI directories exist
        try
        {
            Directory.CreateDirectory(Utils.ReplaceSpecialFolderNames(config.SecurityConfiguration.ApplicationCertificate.StorePath));
            Directory.CreateDirectory(Utils.ReplaceSpecialFolderNames(config.SecurityConfiguration.TrustedPeerCertificates.StorePath));
            Directory.CreateDirectory(Utils.ReplaceSpecialFolderNames(config.SecurityConfiguration.TrustedIssuerCertificates.StorePath));
            Directory.CreateDirectory(Utils.ReplaceSpecialFolderNames(config.SecurityConfiguration.RejectedCertificateStore.StorePath));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to create PKI directories.");
        }

        config.ServerConfiguration.UserTokenPolicies.Clear();
        if (enableAnonymous)
        {
            config.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy(UserTokenType.Anonymous));
        }
        config.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy(UserTokenType.UserName));

        await config.Validate(ApplicationType.Server);

        config.CertificateValidator.CertificateValidation += (s, e) =>
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
            }
        };

        try
        {
            await application.CheckApplicationInstanceCertificates(false, 2048);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Application certificate check failed; attempting to delete and regenerate invalid certificate.");

            try
            {
                var storePath = Utils.ReplaceSpecialFolderNames(config.SecurityConfiguration.ApplicationCertificate.StorePath);
                if (Directory.Exists(storePath))
                {
                    foreach (var file in Directory.EnumerateFiles(storePath))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }
            catch (Exception cleanupEx)
            {
                Log.Warning(cleanupEx, "Failed to cleanup certificate store.");
            }

            await application.CheckApplicationInstanceCertificates(false, 2048);
        }

        _server = new OpcUaServer(configPath);
        await application.Start(_server);
    }
}

public class OpcUaServer : StandardServer
{
    private readonly string _configPath;
    public OpcUaServer(string configPath) { _configPath = configPath; }
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        var nodeManagers = new List<INodeManager>();
        nodeManagers.Add(new SimpleFileServerNodeManager(server, configuration, _configPath));
        return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
    }
}
