using ServerSignalR.Hubs;
using ServerSignalR.Simulator;

var builder = WebApplication.CreateBuilder(args);

var numTags = Convert.ToUInt32(builder.Configuration["Simulator:numTags"]);
var pathDataFile = builder.Environment.WebRootPath + "\\Files\\DataValues.data";
var maximumReceiveMessageSizeKb = Convert.ToUInt32(builder.Configuration["Simulator:maximumReceiveMessageSizeKb"]);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR((o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = maximumReceiveMessageSizeKb * 1024; 

})).AddMessagePackProtocol();

var app = builder.Build();

Simulator.Init(numTags, pathDataFile);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<DataHub>("/dataHub");

app.Run();
