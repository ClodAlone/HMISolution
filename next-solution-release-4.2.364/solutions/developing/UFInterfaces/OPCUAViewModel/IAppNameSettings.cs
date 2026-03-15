using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFInterfaces.Editors;
using UFUAEditor.ComponentService;

namespace AppNameSettingService
{
    public interface IAppNameSettings 
    {
        int RemoveDisabledItemAfterSecs { get; set; }
        int MaxCleanCount { get; set; }
        bool UseAlwaysSecureConnections { get; set; }
        bool UseSecurityWhenNotLocal { get; set; }
        int FastSamplingInterval { get; set; }
        int SlowSamplingInterval { get; set; }
        bool DisableWhenNotUsed { get; set; }
        int PublishingInterval { get; set; }
        bool UsePollingRead { get; set; }
        string AppNameRenamed { get; set; }
        string HostNameRenamed { get; set; }
        string EndpointRenamed { get; set; }
        bool AlwaysDiscoverEndpoint { get; set; }
        string BackupAppName { get; set; }
        string BackupHostName { get; set; }

        void SetOverriddenString(String s);
        bool HasOverriddenString();
    }
}
