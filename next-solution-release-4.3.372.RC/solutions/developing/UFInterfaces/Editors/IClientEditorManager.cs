using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;
using AppNameSettingService;

namespace ClientEditor.ComponentService
{
    public interface IClientEditorManager : IUFInterfaceBase
    {
        Dictionary<String, IAppNameSettings> GetAppNameSettings(IDocument document);
#if !NET_STANDARD
        Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(IDocument parent, String flat);
        event EventHandler<VariableEventArgs> CreatingVariable;
        event EventHandler<VariableEventArgs> VariableCreated;
#endif
    }
}
