namespace ChatApp9.Web.Services;

public class OpcUaSettings
{
    public List<OpcUaServerConfig> Servers { get; set; } = new();
}

public class OpcUaServerConfig
{
    public string EndpointUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Aliases { get; set; } = new();
}
