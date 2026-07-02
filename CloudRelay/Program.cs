// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using CloudRelay;

var builder = WebApplication.CreateBuilder(args);

// Read the shared API key from configuration (appsettings.json or env var)
var apiKey = builder.Configuration["CloudRelay:ApiKey"] ?? "";

builder.Services.AddSignalR();
builder.Services.AddSingleton(new RelayOptions { ApiKey = apiKey });

var app = builder.Build();

// Simple API-key middleware — rejects any request without a valid key
app.Use(async (context, next) =>
{
    var opts = context.RequestServices.GetRequiredService<RelayOptions>();
    if (!string.IsNullOrEmpty(opts.ApiKey))
    {
        var key = context.Request.Query["apiKey"].FirstOrDefault()
                  ?? context.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (key != opts.ApiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
    await next();
});

app.MapHub<RelayHub>("/relay");

app.MapGet("/health", () => Results.Ok("CloudRelay is running"));

app.Run();
