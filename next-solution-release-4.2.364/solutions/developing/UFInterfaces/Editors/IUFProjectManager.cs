using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;
using AppNameSettingService;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
#endif

namespace UFProjectManager.ComponentService
{
    public interface IUFProjectManager : IUFInterfaceBase
    {
#if !WINDOWS_UWP && !NET_STANDARD
        UserControl GetResourcePickerUserControl(IDocument parent, String filter);
        Uri GetResourcePickerUserControlUri(UserControl control);

        string GetConfigurationId(IDocument parent);
        string GetOriginalVersion(IDocument parent);
        string GetProjectVersion(IDocument parent);
        IEnumerable<String> GetResourceList(IDocument parent, String filter, bool getRelativePath = true);
        IDocumentManager GetResourceDocumentManager(IDocument parent, String filter);
        IEnumerable<String> GetActiveProjects();
        Dictionary<String, String> GetRenamedResources(IDocument parent);
        void ClearRenamedResourcesMap(IDocument parent, List<string> typeLabelList);
        string GetDefaultFileExt();
        void SetControllerDataActive(IDocument parent, Uri uri);
        void AddLogEntity(IDocument parent, string source, DateTime recordingTime, string message, System.Diagnostics.EventLogEntryType severity);
        void AddLogEntity(IDocument parent, string source, DateTime recordingTime, string message, string details, string comment, string userName, System.Diagnostics.EventLogEntryType severity);
        IAppNameSettings GetDefaultAppNameSettings(IDocument parent);
        Dictionary<String, IAppNameSettings> GetMapAppNameSettings(IDocument parent);
        void UpdateDefaultAppNameSettings(IDocument parent, IAppNameSettings settings);
        void ClearMapAppNameSettings(IDocument parent);
#endif
    }
}
