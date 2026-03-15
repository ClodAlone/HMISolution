using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OPCUAAITool
{
    // Facade kept for backward compatibility with existing code that expects static methods on `utilities`.
    public static class utilities
    {
        static IServiceProvider? _services;

        public static void Initialize(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        static IOpcUaService GetService()
        {
            if (_services == null) throw new InvalidOperationException("utilities not initialized. Call utilities.Initialize(...) after app build.");
            var svc = (IOpcUaService?)_services.GetService(typeof(IOpcUaService));
            if (svc == null) throw new InvalidOperationException("IOpcUaService not registered.");
            return svc;
        }

        [Description("Get the NodId from variable and device name")]
        public static IEnumerable<string> GetNodeId(
            [Description("The variable name")] string name)
        {
            return GetService().GetNodeIdAsync(name, null).GetAwaiter().GetResult();
        }

        public static IEnumerable<string> GetVariableName(string nodeid)
        {
            return GetService().GetVariableNameAsync(nodeid).GetAwaiter().GetResult();
        }

        public static string GetValue(string nodeid)
        {
            return GetService().GetValueAsync(nodeid).GetAwaiter().GetResult() ?? string.Empty;
        }

        public static void SetValue(string nodeid, string value)
        {
            GetService().SetValueAsync(nodeid, value).GetAwaiter().GetResult();
        }

        public static System.Threading.Tasks.Task ConnectToServerAsync()
        {
            return GetService().ConnectToServerAsync();
        }

        // Helper to expose discovered variables for UI
        public static IEnumerable<OpcVariableDto> GetDiscoveredVariables()
        {
            return GetService().GetDiscoveredVariables();
        }
    }
}

