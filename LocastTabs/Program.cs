var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on the desired port by default (16091).
// This can still be overridden by environment variables (ASPNETCORE_URLS) or command-line args.
builder.WebHost.ConfigureKestrel(opts =>
{
    // Only add a default URL if none provided via configuration
    var urls = builder.Configuration["ASPNETCORE_URLS"]; // may be null
    if (string.IsNullOrEmpty(urls))
    {
        opts.ListenAnyIP(16091);
    }
});

// Enable running as a Windows Service or systemd service when deployed
if (OperatingSystem.IsWindows())
{
    builder.Host.UseWindowsService();
}
else if (OperatingSystem.IsLinux())
{
    builder.Host.UseSystemd();
}

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
