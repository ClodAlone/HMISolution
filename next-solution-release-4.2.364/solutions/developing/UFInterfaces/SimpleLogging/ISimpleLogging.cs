using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;

namespace Tracing.ComponentService
{
    public interface ISimpleLogging : IUFInterfaceBase
    {
        void AddItem(String source, String message, DateTime timestamp, int severity, Uri uri);
        void AddItem(String source, String message, DateTime timestamp);
        void AddItem(String source, String message);
    }
}
