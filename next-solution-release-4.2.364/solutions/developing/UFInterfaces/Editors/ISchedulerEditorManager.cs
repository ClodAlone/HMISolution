using System;
using System.Collections.Generic;
using UFInterfaces;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;
#if !NET_STANDARD
using PropertyControl.ComponentService;
using UIMsgBoxAlertService.ComponentService;
#endif

namespace MSSchedulerSettings.ComponentService
{
    public interface ISchedulerEditorManager : IUFInterfaceBase
    {
#if !NET_STANDARD
        IUIMsgBoxAlertService UIInterface { get; }
#endif
        IStringEditorManager StringEditor { get; }
#if !NET_STANDARD
        IWorkspace Workspace { get; }
#endif
        IUFUserEditorManager UserEditor { get; }
        String GetServerEntityReference(IDocument parent, bool bCheckEmpty = false);
        String GetSchedulerEntityReference(IDocument parent, String nodeId);
        IList<String> GetFlatEventsList(IDocument parent);
#if !NET_STANDARD
        IPropertyControl PropertyControl { get; }
#endif
    }
}
