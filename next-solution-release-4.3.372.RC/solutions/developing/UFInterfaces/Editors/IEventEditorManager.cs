using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces;

namespace UFEventEditor.ComponentService
{
    public class EventEventArgs : EventArgs
    {
        public String Name;
        public String JsonCommand;
        public bool bExecuted;
    }

    public interface IEventEditorManager : IUFInterfaceBase
    {
        event EventHandler<EventEventArgs> FireUIEvent;
        void OnFireUIEvent(Object sender, EventEventArgs args);
    }
}

