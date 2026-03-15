using System.Collections.Generic;
using System.Threading.Tasks;

namespace OPCUAAITool
{
    public class OpcVariableDto
    {
        public string DisplayName { get; set; }
        public string NodeId { get; set; }
        public object? DataType { get; set; }
    }

    public interface IOpcUaService
    {
        Task<IEnumerable<string>> GetNodeIdAsync(string name, string device);
        Task<IEnumerable<string>> GetVariableNameAsync(string nodeid);
        Task<string?> GetValueAsync(string nodeid);
        Task SetValueAsync(string nodeid, string value);
        Task ConnectToServerAsync();
        IEnumerable<OpcVariableDto> GetDiscoveredVariables();
    }
}