using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Extensions.AI;
using OPCUAAITool;
using OPCUAChatApp.Web.Components;
using OPCUAChatApp.Web.Services;
using OPCUAChatApp.Web.Services;
using OPCUAChatApp.Web.Services.Ingestion;
using OPCUAChatApp.Web.Services.Ingestion;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Bind OpcUaSettings and register
var opcSection = builder.Configuration.GetSection("OpcUaSettings");
var opcSettings = opcSection.Get<OpcUaSettings>() ?? new OpcUaSettings();
builder.Services.AddSingleton(opcSettings);

// Register OpcUaService and IOpcUaService
builder.Services.AddSingleton<IOpcUaService, OpcUaService>();

var openai = builder.AddOpenAIClient("openai");
openai.AddChatClient("gpt-4o-mini")
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment());
openai.AddEmbeddingGenerator("text-embedding-3-small");

// Register the AI Agent using the Agent Framework
builder.AddAIAgent("ChatAgent", (sp, key) =>
{
    // Get required services
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Configuring AI Agent with key '{Key}' for model '{Model}'", key, "gpt-4o-mini");

    var searchFunctions = sp.GetRequiredService<SearchFunctions>();
    var chatClient = sp.GetRequiredService<IChatClient>();

    // Create and configure the AI agent
    var aiAgent = chatClient.CreateAIAgent(
        name: key,
        instructions: "You can search documents or get variable values, first get the NodeId and then use to read or write variables...",
        description: "An AI agent that helps users reading or wrtiting runtime variable values.",
        tools:  [
                    AIFunctionFactory.Create(searchFunctions.SearchAsync),
                    AIFunctionFactory.Create(utilities.GetNodeId),
                    AIFunctionFactory.Create(utilities.GetVariableName),
                    AIFunctionFactory.Create(utilities.GetValue),
                    AIFunctionFactory.Create(utilities.SetValue)
                ]
        )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();

    return aiAgent;
});

var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteCollection<string, IngestedChunk>("data-chatapp20-chunks", vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedDocument>("data-chatapp20-documents", vectorStoreConnectionString);
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

// Register SearchFunctions for DI injection into the agent
builder.Services.AddSingleton<SearchFunctions>();

var app = builder.Build();

// Initialize utilities with settings
var settings = app.Services.GetRequiredService<OpcUaSettings>();
utilities.Initialize(app.Services);

// Try to resolve and connect the OPC UA service at startup (non-fatal)
try
{
    var opcService = app.Services.GetRequiredService<IOpcUaService>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    // Attempt connection asynchronously during startup
    await opcService.ConnectToServerAsync();
    logger.LogInformation("OPC UA service connected during startup.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to connect OPC UA service during startup. Continue starting the app; you can connect from the UI.");
}

// ... rest of the configuration ...
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // In development show detailed exception page
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// By default, we ingest PDF files from the /wwwroot/Data directory. You can ingest from
// other sources by implementing IIngestionSource.
// Important: ensure that any content you ingest is trusted, as it may be reflected back
// to users or could be a source of prompt injection risk.
await DataIngestor.IngestDataAsync(
    app.Services,
    new PDFDirectorySource(Path.Combine(builder.Environment.WebRootPath, "Data")));

app.Run();
