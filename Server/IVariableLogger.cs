using Opc.Ua;
using SharedModels;

namespace SimpleOpcFileServer
{
    public interface IVariableLogger : IDisposable
    {
        void Initialize();
        void Log(BaseDataVariableState variable, DataLoggingConfig config);
        List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime);
    }
}
