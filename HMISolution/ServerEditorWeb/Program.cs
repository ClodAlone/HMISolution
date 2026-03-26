using System.Threading.RateLimiting;
using ServerEditorWeb.Components;
using ServerEditorWeb.Services;

// Install crash reporter before anything else
var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
SharedModels.CrashReporter.Install(crashDir, "Editor");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Rate limiting to protect web endpoints from abuse
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global fixed-window limiter per client IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 200,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 10
        });
    });
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
