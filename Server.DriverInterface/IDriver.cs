using System;
using Opc.Ua;

namespace SimpleOpcFileServer
{
    public interface IDriver : IDisposable
    {
        string Key { get; }
        void AddItem(BaseDataVariableState variable, string configJson);
    }
}
