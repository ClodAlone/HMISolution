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
using System.Reflection;

try
{
    // Display version banner
    var entryAsm = Assembly.GetEntryAssembly();
    var infoVer = entryAsm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    var version = infoVer?.Split('+')[0] ?? entryAsm?.GetName().Version?.ToString(3) ?? "1.0.0";
    var product = entryAsm?.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Server";
    var copyright = entryAsm?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";
    Console.WriteLine($"{product} - Server v{version}");
    if (!string.IsNullOrEmpty(copyright))
        Console.WriteLine(copyright);
    Console.WriteLine();

    // Set current directory to AppContext.BaseDirectory to ensure relative paths work correctly when running as a Windows Service
    Directory.SetCurrentDirectory(AppContext.BaseDirectory);

    string configPath = "nodes.json";
    if (args.Length > 0 && !args[0].StartsWith("-"))
    {
        configPath = args[0];
    }
    // Handle specific argument if passed via --NodesConfig or similar if needed.
    // For now simple argv check as previously discussed.

    if (!Path.IsPathRooted(configPath))
    {
        configPath = Path.Combine(AppContext.BaseDirectory, configPath);
    }
    configPath = Path.GetFullPath(configPath);

    // Install crash reporter early — before anything else can throw
    var crashDir = Path.Combine(Path.GetDirectoryName(configPath) ?? AppContext.BaseDirectory, "crash_reports");
    SharedModels.CrashReporter.Install(crashDir, "Server");

    // Validate license
    var licenseFile = SharedModels.LicenseManager.FindLicenseFile(configPath);
    var licenseStatus = SharedModels.LicenseManager.Validate(licenseFile);
    Console.WriteLine($"License: {licenseStatus.Tier} — {licenseStatus.Message}");
    if (!string.IsNullOrEmpty(licenseStatus.LicensedTo))
        Console.WriteLine($"Licensed to: {licenseStatus.LicensedTo}");
    if (licenseStatus.DemoStartedUtc.HasValue)
        Console.WriteLine($"Demo mode: full features for {licenseStatus.DemoGraceMinutes} minutes (started {licenseStatus.DemoStartedUtc.Value:HH:mm:ss} UTC)");

    if (!File.Exists(configPath))
    {
        // Fallback or warning
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
    builder.Services.AddSingleton<CameraRecordingService>();
    builder.Services.AddHostedService<CameraHostedService>();

    var host = builder.Build();

    // Start diagnostics HTTP endpoint and configure crash email.
    // Use streaming JSON reader to extract only the "Server" property
    // instead of deserializing the entire (potentially very large) config file.
    try
    {
        int diagPort = 14841;
        SharedModels.ServerSettings? serverSection = null;
        if (File.Exists(configPath))
        {
            using var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var jsonDoc = JsonDocument.Parse(fs);
            if (jsonDoc.RootElement.TryGetProperty("Server", out var serverEl))
            {
                serverSection = JsonSerializer.Deserialize<ServerSettings>(serverEl.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
        if (serverSection?.DiagnosticsPort is > 0 and var port)
            diagPort = port;
        else if (serverSection?.DiagnosticsPort == 0)
            diagPort = 0;

        // Configure crash email notifications
        SharedModels.CrashReporter.ConfigureEmail(serverSection?.CrashEmail);

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
        // Load settings from the project JSON and build the OPC UA configuration in code.
        // This avoids strict XML config schema/version issues.
        var endpointUrl = "opc.tcp://localhost:14840/SimpleOpcFileServer";
        var enableAnonymous = true;

        // Defaults for certificate stores.
        var pkiRoot = "%LocalApplicationData%/SimpleOpcFileServer/pki";

        try
        {
            if (File.Exists(configPath))
            {
                using var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var jsonDoc = await JsonDocument.ParseAsync(fs);
                if (jsonDoc.RootElement.TryGetProperty("Server", out var serverEl))
                {
                    var server = JsonSerializer.Deserialize<SharedModels.ServerSettings>(serverEl.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (server != null)
                    {
                        if (!string.IsNullOrWhiteSpace(server.EndpointUrl))
                        {
                            endpointUrl = server.EndpointUrl;
                        }

                        enableAnonymous = server.EnableAnonymous;

                        // When runtime login is disabled, anonymous OPC access must be
                        // allowed so the viewer can connect without user credentials.
                        if (!server.EnableRuntimeLogin)
                            enableAnonymous = true;
                    }
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
            TransportQuotas = new TransportQuotas
            {
                OperationTimeout = 15000,
                MaxMessageSize = 16 * 1024 * 1024,
                MaxBufferSize = 16 * 1024 * 1024,
                MaxStringLength = 4 * 1024 * 1024,
                MaxByteStringLength = 4 * 1024 * 1024
            },
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

        // Ensure PKI directories exist (the SDK may not create them in all cases).
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
        
        Log.Information("Validating OPC UA configuration...");
        await config.Validate(ApplicationType.Server);

        config.CertificateValidator.CertificateValidation += (s, e) =>
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
            }
        };

        Log.Information("Checking application certificates...");
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
        
        Log.Information("Starting OPC UA server...");
        _server = new OpcUaServer(configPath);
        await application.Start(_server);

        // The TCP transport is now open — clients can connect.
        // Complete the deferred node loading (JSON parse + address space population).
        Log.Information("OPC UA transport listening on {Endpoint}. Loading address space...", endpointUrl);
        _server.CompleteDeferredLoad();
        Log.Information("Address space loaded successfully.");
    }
}

/// <summary>
/// A custom OPC UA server that uses the SimpleFileServerNodeManager.
/// </summary>
public class OpcUaServer : StandardServer
{
    private readonly string _configPath;
    private SimpleFileServerNodeManager? _nodeManager;

    public OpcUaServer(string configPath)
    {
        _configPath = configPath;
    }

    /// <summary>
    /// Creates the master node manager for the server.
    /// </summary>
    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        _nodeManager = new SimpleFileServerNodeManager(server, configuration, _configPath);
        var nodeManagers = new List<INodeManager> { _nodeManager };
        return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
    }

    /// <summary>
    /// Completes deferred node loading after the TCP transport is already listening.
    /// </summary>
    public void CompleteDeferredLoad()
    {
        _nodeManager?.CompleteDeferredLoad();
    }
}

