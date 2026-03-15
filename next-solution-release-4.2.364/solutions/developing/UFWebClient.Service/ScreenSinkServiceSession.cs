using System;
using System.Collections.Generic;
using System.Threading;
using DocumentManager.ComponentService;

namespace UFWebClient.Service
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
        internal Timer _demoModeLoop;
        internal DateTime lastDemoMode;
        internal int clickCounterInDemoMode;
        internal readonly List<Guid> listChanges = new List<Guid>();
        internal readonly List<ElementData> listStatusChanges = new List<ElementData>();
       
#if DEBUG
        static int globalCounter = 0;
        public int sessionid;
#endif
        internal ScreenSinkServiceSession(String theme, String clientId, bool disableStaticOptimization)
        {
#if DEBUG
            sessionid = ++globalCounter;
#endif
            ScreenSink = new WPFScreenSink.ScreenSink(theme, clientId, WPFScreenSink.ServerType.AppServer, disableStaticOptimization);
            mapIdToUIElements = new Dictionary<String, IList<Guid>>();
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
                return ScreenSink.DemoMode && lastDemoMode.AddSeconds(UFWebClientService.DemoModeCountDown) > DateTime.UtcNow;
            }
        }

        public void Dispose()
        {
            ScreenSink.Dispose();
        }
    }
}
