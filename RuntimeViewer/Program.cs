using RuntimeViewer.Components;
using RuntimeViewer.Shared.Services;

// Install crash reporter before anything else
var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
SharedModels.CrashReporter.Install(crashDir, "RuntimeViewer");

var builder = WebApplication.CreateBuilder(args);

// Enable static web assets in all environments (not just Development).
// Without this, CSS/JS from Razor Class Libraries return empty content
// when running the built exe directly (Production mode).
builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ProjectService>();
builder.Services.AddScoped<OpcRuntimeClient>();
builder.Services.AddScoped<CloudRuntimeClient>();
builder.Services.AddScoped<CommandService>();
builder.Services.AddScoped<RuntimeAuthService>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddSingleton<HdaReaderService>();
builder.Services.AddSingleton<EventLogReaderService>();
builder.Services.AddSingleton<NaturalLanguageQueryService>();
builder.Services.AddScoped<PushNotificationInterop>();
builder.Services.AddScoped<MultiSiteAggregator>();

var app = builder.Build();

// Parse CLI arguments
var isKiosk = args.Any(a => a.Equals("--kiosk", StringComparison.OrdinalIgnoreCase));
var configPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "nodes.json";
if (!Path.IsPathRooted(configPath))
{
    configPath = Path.GetFullPath(configPath);
}

var project = app.Services.GetRequiredService<ProjectService>();
project.IsKiosk = isKiosk;
var (success, message) = project.Load(configPath);
if (success)
{
    Console.WriteLine(message);
}
else
{
    Console.WriteLine($"Warning: {message}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(RuntimeViewer.Shared.Components.Routes).Assembly)
    .AddInteractiveServerRenderMode();

// ─── Web Push subscription API endpoints ───
var webPushConfig = project.Settings.AlarmNotification?.WebPush;
if (webPushConfig is { Enabled: true } &&
    !string.IsNullOrEmpty(webPushConfig.VapidPublicKey) &&
    !string.IsNullOrEmpty(webPushConfig.VapidPrivateKey))
{
    var pushProjectDir = Path.GetDirectoryName(Path.GetFullPath(configPath)) ?? AppContext.BaseDirectory;
    var pushStore = new SharedModels.WebPushSubscriptionStore(webPushConfig, pushProjectDir);

    app.MapGet("/api/push/vapid-key", () => Results.Ok(new { publicKey = webPushConfig.VapidPublicKey }));

    app.MapPost("/api/push/subscribe", (SharedModels.PushSubscriptionInfo sub) =>
    {
        if (string.IsNullOrEmpty(sub.Endpoint)) return Results.BadRequest("Endpoint required");
        var subs = pushStore.Load();
        if (!subs.Any(s => s.Endpoint == sub.Endpoint))
        {
            subs.Add(sub);
            pushStore.Save(subs);
        }
        return Results.Ok();
    });

    app.MapPost("/api/push/unsubscribe", (SharedModels.PushSubscriptionInfo sub) =>
    {
        if (string.IsNullOrEmpty(sub.Endpoint)) return Results.BadRequest("Endpoint required");
        var subs = pushStore.Load();
        subs.RemoveAll(s => s.Endpoint == sub.Endpoint);
        pushStore.Save(subs);
        return Results.Ok();
    });
}

app.Run();
