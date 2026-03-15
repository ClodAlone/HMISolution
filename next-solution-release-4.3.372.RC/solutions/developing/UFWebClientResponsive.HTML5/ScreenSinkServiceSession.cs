using System;
using System.Collections.Generic;
using System.Threading;
using DocumentManager.ComponentService;
using UFUAServerInfo;
using WPFScreenSink;

namespace UFWebClient.HTML5
{
    public class ScreenSinkServiceSession : IDisposable
    {
        public readonly WPFScreenSink.ScreenSink ScreenSink;
        public bool bIsMobile;
        public readonly Dictionary<String, IList<Guid>> mapIdToUIElements;
        public readonly Object lockObject = new Object();
        public Uri currentPage;
        public Uri defaultPage;
        public Dictionary<IDocument, UFProjectManager.UFProjectDocument> projectDocuments;

        internal Timer _broadcastLoop;
        internal DateTime lastDemoMode;
        internal int clickCounterInDemoMode;
        internal readonly List<Guid> listChanges = new List<Guid>();
        internal readonly List<ElementData> listStatusChanges = new List<ElementData>();
        
        static long licenseInUseCounter = 0;
        
        internal static ScreenSinkSessionCounter SessionCounter;
#if DEBUG
        static int globalCounter = 0;
        public int sessionid;
#endif
        static ScreenSinkServiceSession()
        {
            SessionCounter = new ScreenSinkSessionCounter(Global.UFUAEditorComponent, Global.projectDocument, Global.ClientSessionName, Global.ActiveSessionCountVariableName, BrowserNames.ActiveSessionsWebClientName, () => GetLicenseInUseCounter(), Properties.Resources.WebClientHTML5Log);
        }

        internal ScreenSinkServiceSession(String theme, String clientId, bool disableStaticOptimization, bool bPopup)
        {
#if DEBUG
            sessionid = ++globalCounter;
#endif
            ScreenSink = new ScreenSink(theme, clientId, ServerType.HTML5, disableStaticOptimization, bPopup);
            mapIdToUIElements = new Dictionary<String, IList<Guid>>();

            if (ScreenSink.UseLicense)
                Interlocked.Increment(ref licenseInUseCounter);
            SessionCounter.UpdateActiveSessionCount();
        }

        public void Dispose()
        {
            if (ScreenSink.UseLicense)
                Interlocked.Decrement(ref licenseInUseCounter);
            SessionCounter.UpdateActiveSessionCount();
            ScreenSink.Dispose();
        }

        public static long GetLicenseInUseCounter()
        {
            return Interlocked.Read(ref licenseInUseCounter);
        }

        internal bool InDemoMode
        {
            get
            {
                return ScreenSink.DemoMode;
            }
        }

        internal bool InDemoModeCountDown
        {
            get
            {
                return ScreenSink.DemoMode && lastDemoMode.AddSeconds(Global.DemoModeCountDown) > DateTime.UtcNow;
            }
        }
    }
}
