using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;

namespace OPCUAViewModel
{
    static class DiscoveryURL
    {
        public static StringCollection m_DiscoveryUrls { get; private set; }

        static DiscoveryURL()
        {
            m_DiscoveryUrls = new StringCollection();
            m_DiscoveryUrls.Add(Properties.Settings.Default.DiscoveryURL1);
            m_DiscoveryUrls.Add(Properties.Settings.Default.DiscoveryURL2);
            m_DiscoveryUrls.Add(Properties.Settings.Default.DiscoveryURL3);
        }
    }
}
