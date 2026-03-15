using System;
using System.Collections.Generic;
using System.Threading;

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

        internal Timer _broadcastLoop;
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
            ScreenSink = new WPFScreenSink.ScreenSink(theme, clientId, WPFScreenSink.ServerType.HTML5, disableStaticOptimization);
            mapIdToUIElements = new Dictionary<String, IList<Guid>>();
        }

        public void Dispose()
        {
            ScreenSink.Dispose();
        }
    }
}
