using System;
using Opc.Ua;

namespace SimpleOpcFileServer
{
    public interface IDriver : IDisposable
    {
        string Key { get; }
        void AddItem(BaseDataVariableState variable, string configJson);

        /// <summary>
        /// Starts the driver (polling timer, connections, etc.).
        /// Called after all items have been added to allow bulk loading without
        /// the driver competing for resources during address-space creation.
        /// Default implementation is a no-op for drivers that auto-start.
        /// </summary>
        void Start() { }

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
