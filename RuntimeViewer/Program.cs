using RuntimeViewer.Components;
using RuntimeViewer.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
builder.Services.AddSingleton<DataExportService>();
builder.Services.AddScoped<PushNotificationInterop>();
builder.Services.AddScoped<MultiSiteAggregator>();

// ─── External Authentication (OAuth) ───
// Schemes are registered unconditionally; actual client IDs/secrets are read
// lazily from ProjectService via IPostConfigureOptions (project loaded before Run).
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/";
    options.Cookie.Name = "rv_auth";
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddGoogle(options => { options.ClientId = "unused"; options.ClientSecret = "unused"; })
.AddMicrosoftAccount(options => { options.ClientId = "unused"; options.ClientSecret = "unused"; })
.AddFacebook(options => { options.ClientId = "unused"; options.ClientSecret = "unused"; });

// Post-configure OAuth options from project settings (resolved lazily via DI)
builder.Services.AddSingleton<Microsoft.Extensions.Options.IPostConfigureOptions<Microsoft.AspNetCore.Authentication.Google.GoogleOptions>>(
    sp => new ExternalAuthPostConfigure<Microsoft.AspNetCore.Authentication.Google.GoogleOptions>(sp, "Google"));
builder.Services.AddSingleton<Microsoft.Extensions.Options.IPostConfigureOptions<Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountOptions>>(
    sp => new ExternalAuthPostConfigure<Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountOptions>(sp, "Microsoft"));
builder.Services.AddSingleton<Microsoft.Extensions.Options.IPostConfigureOptions<Microsoft.AspNetCore.Authentication.Facebook.FacebookOptions>>(
    sp => new ExternalAuthPostConfigure<Microsoft.AspNetCore.Authentication.Facebook.FacebookOptions>(sp, "Facebook"));

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

// ─── External Auth middleware & endpoints ───
app.UseAuthentication();

app.MapGet("/auth/login/{provider}", (string provider, HttpContext ctx) =>
{
    var scheme = provider switch
    {
        "Google" => Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme,
        "Microsoft" => Microsoft.AspNetCore.Authentication.MicrosoftAccount.MicrosoftAccountDefaults.AuthenticationScheme,
        "Facebook" => Microsoft.AspNetCore.Authentication.Facebook.FacebookDefaults.AuthenticationScheme,
        _ => null
    };
    if (scheme == null) return Results.BadRequest("Unknown provider");
    var props = new AuthenticationProperties { RedirectUri = "/auth/callback" };
    return Results.Challenge(props, [scheme]);
});

app.MapGet("/auth/callback", async (HttpContext ctx) =>
{
    var result = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    if (result?.Principal == null)
    {
        // Fall back — try each external scheme
        foreach (var scheme in new[] { "Google", "Microsoft", "Facebook" })
        {
            result = await ctx.AuthenticateAsync(scheme);
            if (result?.Principal != null)
            {
                // Sign in with the cookie scheme so the identity persists
                await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Principal);
                break;
            }
        }
    }
    ctx.Response.Redirect("/");
});

app.MapGet("/auth/user-info", (HttpContext ctx) =>
{
    if (ctx.User.Identity?.IsAuthenticated != true)
        return Results.Json(new { authenticated = false });

    var email = ctx.User.FindFirstValue(ClaimTypes.Email) ?? "";
    var name = ctx.User.FindFirstValue(ClaimTypes.Name) ?? email;
    var provider = ctx.User.Identity.AuthenticationType ?? "";
    return Results.Json(new { authenticated = true, email, name, provider });
});

app.MapGet("/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    ctx.Response.Redirect("/");
});

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

/// <summary>
/// Post-configures OAuth remote authentication options by reading the ClientId/Secret
/// from ProjectService settings. This avoids the need to know settings at registration time.
/// </summary>
sealed class ExternalAuthPostConfigure<TOptions> : Microsoft.Extensions.Options.IPostConfigureOptions<TOptions>
    where TOptions : Microsoft.AspNetCore.Authentication.OAuth.OAuthOptions
{
    private readonly IServiceProvider _sp;
    private readonly string _providerName;

    public ExternalAuthPostConfigure(IServiceProvider sp, string providerName)
    {
        _sp = sp;
        _providerName = providerName;
    }

    public void PostConfigure(string? name, TOptions options)
    {
        var project = _sp.GetService<RuntimeViewer.Shared.Services.ProjectService>();
        var prov = project?.Settings.ExternalAuth?.Providers?
            .FirstOrDefault(p => p.Name == _providerName && p.Enabled);
        if (prov != null && !string.IsNullOrEmpty(prov.ClientId))
        {
            options.ClientId = prov.ClientId;
            options.ClientSecret = prov.ClientSecret;
        }
    }
}
