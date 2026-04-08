using ServerEditorWeb.Components;
using ServerEditorWeb.Services;
using System.Threading.RateLimiting;

// Install crash reporter before anything else
var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
SharedModels.CrashReporter.Install(crashDir, "Editor");

var builder = WebApplication.CreateBuilder(args);

// Enable static web assets in all environments (not just Development).
// Without this, CSS/JS from Razor Class Libraries return empty content
// when running the built exe directly (Production mode).
builder.WebHost.UseStaticWebAssets();

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

builder.Services.AddSingleton<NodeEditorService>();
builder.Services.AddSingleton<ServerProcessService>();
builder.Services.AddSingleton<RuntimeViewerProcessService>();
builder.Services.AddSingleton<ServerDiagnosticsClient>();
builder.Services.AddScoped<GitService>();
builder.Services.AddSingleton<AiService>();
builder.Services.AddSingleton<SyntaxCheckService>();
builder.Services.AddSingleton<DataLoggingReaderService>();
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

app.Run();
