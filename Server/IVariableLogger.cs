using Opc.Ua;
using SharedModels;

namespace SimpleOpcFileServer
{
    public interface IVariableLogger : IDisposable
    {
        void Initialize();
        void Log(BaseDataVariableState variable, DataLoggingConfig config);
        List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Returns the UTC timestamp of the most recent log entry, or null if the table is empty.
        /// Used by redundancy to detect history gaps after a restart.
        /// </summary>
        DateTime? GetLatestTimestamp();

        /// <summary>
        /// Read all log entries after the given UTC time. Used by redundancy gap-fill on startup.
        /// </summary>
        List<RedundancyService.LogEntry> ReadEntriesSince(DateTime sinceUtc, int maxRows = 100_000);
    }
}
