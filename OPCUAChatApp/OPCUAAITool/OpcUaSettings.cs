namespace OPCUAAITool
{
    public class OpcUaAlias
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class OpcUaServerSettings
    {
        public string ServerUrl { get; set; }
        public List<OpcUaAlias> ListAlias { get; set; }
        public int TimeoutMs { get; set; }
    }

    public class OpcUaSettings
     {
        public String ApplicationName { get; set; } = "OPCUAAITool";
        public List<OpcUaServerSettings> ListServers { get; set; } = new List<OpcUaServerSettings>();
    }
}