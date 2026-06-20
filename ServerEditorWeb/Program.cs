using ServerEditorWeb.Components;
using ServerEditorWeb.Services;
using System.Threading.RateLimiting;

// Install crash reporter before anything else
var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
SharedModels.CrashReporter.Install(crashDir, "Editor");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Keep circuits alive longer while paused at breakpoints
            options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(30);
            options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(10);
        }
    });

if (builder.Environment.IsDevelopment())
{
    // Increase SignalR timeouts so the connection survives breakpoint pauses
    builder.Services.AddSignalR(hubOptions =>
    {
        hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(10);
        hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(5);
        hubOptions.HandshakeTimeout = TimeSpan.FromMinutes(2);
    });
}

builder.Services.AddSingleton<NodeEditorService>();
builder.Services.AddSingleton<ServerProcessService>();
builder.Services.AddSingleton<RuntimeViewerProcessService>();
builder.Services.AddSingleton<DockerServiceManager>();
builder.Services.AddSingleton<ServerDiagnosticsClient>();
builder.Services.AddScoped<GitService>();
builder.Services.AddSingleton<ProjectDiffService>();
builder.Services.AddSingleton<AiService>();
builder.Services.AddSingleton<SyntaxCheckService>();
builder.Services.AddSingleton<ScriptCompletionService>();
builder.Services.AddSingleton<CodeSnippetService>();  // ← NEW: Code snippet management
builder.Services.AddSingleton<PropertyGridService>();
builder.Services.AddSingleton<DockLayoutService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<SymbolLibraryService>();
builder.Services.AddSingleton<ClipboardService>();
builder.Services.AddSingleton<UndoRedoService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<OpcClientService>();
builder.Services.AddSingleton<CrossReferenceService>();
builder.Services.AddSingleton<WatchTableService>();
builder.Services.AddSingleton<ProjectValidationService>();
builder.Services.AddSingleton<LicenseService>();
builder.Services.AddSingleton<HelpService>();
builder.Services.AddSingleton<EditorLocalizationService>();
builder.Services.AddSingleton<DriverTestService>();
builder.Services.AddSingleton<CertificateService>();
builder.Services.AddSingleton<BackupService>();
builder.Services.AddSingleton<ScriptDebugService>();
builder.Services.AddSingleton<FindReplaceService>();
builder.Services.AddSingleton<LiveTagService>();
builder.Services.AddSingleton<ProtocolTrafficService>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 200,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 10
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseRateLimiter();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Lightweight API for Desktop shell to query unsaved state
app.MapGet("/api/has-unsaved-changes", (ServerEditorWeb.Services.NodeEditorService editor) =>
    Results.Json(new { hasUnsavedChanges = editor.AnyUnsavedChanges }));

app.Run();
