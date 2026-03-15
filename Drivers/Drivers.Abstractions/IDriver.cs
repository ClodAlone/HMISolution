using System;
using Opc.Ua;

namespace SimpleOpcFileServer
{
    public interface IDriver : IDisposable
    {
        string Key { get; }
        void AddItem(BaseDataVariableState variable, string configJson);

        /// <summary>
        /// Raised when the driver encounters an error (connection failure, read/write error, etc.).
        /// Parameters: source (e.g. device key or variable name), error message.
        /// </summary>
        event Action<string, string>? OnError;

        /// <summary>
        /// Raised after a complete poll/receive cycle.
        /// Parameter: elapsed time in milliseconds.
        /// </summary>
        event Action<double>? OnCycleCompleted;
    }
}
