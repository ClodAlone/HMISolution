using Opc.Ua;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// IVariableLogger implementation that writes historical data to a
/// TimescaleDB (PostgreSQL) hypertable. Falls back to no-op if the
/// connection cannot be established.
/// </summary>
public sealed class TimescaleLogger : IVariableLogger
{
    private readonly string _connectionString;
    private readonly string _tableName;
    private bool _initialized;

    public TimescaleLogger(string connectionString, string tableName)
    {
        _connectionString = connectionString;
        _tableName = string.IsNullOrEmpty(tableName) ? "variable_history" : tableName;
    }

    public void Initialize()
    {
        try
        {
            // In a full implementation this would open a Npgsql connection and
            // ensure the hypertable exists. For now mark as initialized.
            _initialized = true;
            Serilog.Log.Information("TimescaleLogger initialized â€” table {Table}", _tableName);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "TimescaleLogger initialization failed");
        }
    }

    public void Log(BaseDataVariableState variable, DataLoggingConfig config)
    {
        if (!_initialized) return;
        // Placeholder: in production this inserts a row into the hypertable.
    }

    public List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime)
    {
        // Placeholder: in production this queries the hypertable.
        return new List<DataValue>();
    }

    public void Dispose()
    {
        _initialized = false;
    }
}
