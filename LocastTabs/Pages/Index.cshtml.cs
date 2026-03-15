using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace LocastTabs.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _config;

    public IndexModel(IConfiguration config)
    {
        _config = config;
    }

    public List<ServerInfo> Servers { get; set; } = new();

    public void OnGet()
    {
        // Bind Servers section from configuration
        var servers = new List<ServerInfo>();
        _config.GetSection("Servers").Bind(servers);
        if (servers?.Count > 0)
        {
            Servers = servers;
        }
    }
}

public class ServerInfo
{
    public string? Name { get; set; }
    public string? Url { get; set; }
}
