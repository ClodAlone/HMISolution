using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace WMIService
{
    public sealed class WMIService : IDisposable
    {
        public static WMIService GetInstance(string path)
        {
            return new WMIService(string.IsNullOrEmpty(path) ? "//./root/cimv2" : path);
        }
        //
        ManagementScope scopeCore;
        Dictionary<string, ManagementObjectCollection> queryCacheCore;
        bool connectedCore = false;
        WMIService(string path)
        {
            queryCacheCore = new Dictionary<string, ManagementObjectCollection>();
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Packet;
            this.scopeCore = new ManagementScope(path, options);
            try
            {
                Scope.Connect();
                connectedCore = Scope.IsConnected;
            }
            catch { connectedCore = false; }
        }
        public void Dispose()
        {
            connectedCore = false;
            if (queryCacheCore != null)
            {
                foreach (KeyValuePair<string, ManagementObjectCollection> pair in queryCacheCore)
                {
                    if (pair.Value != null) pair.Value.Dispose();
                }
                queryCacheCore.Clear();
                queryCacheCore = null;
            }
            scopeCore = null;
        }
        public bool Connected
        {
            get { return connectedCore; }
        }
        public ManagementScope Scope
        {
            get { return scopeCore; }
        }
        Dictionary<string, ManagementObjectCollection> QueryCache
        {
            get { return queryCacheCore; }
        }
        private ManagementObjectCollection GetManagementObjectCollection(string queryString)
        {
            ManagementObjectCollection result = null;
            ObjectQuery query = new ObjectQuery(queryString);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, query))
            {
                result = searcher.Get();
            }
            return result;
        }
        public ManagementObjectCollection GetObjectCollection(string queryString, bool allowQueryCaching)
        {
            ManagementObjectCollection result = null;
            if (allowQueryCaching) QueryCache.TryGetValue(queryString, out result);
            if (result == null)
            {
                result = GetManagementObjectCollection(queryString);
                if (allowQueryCaching)
                {
                    if (QueryCache.ContainsKey(queryString)) QueryCache[queryString] = result;
                    else QueryCache.Add(queryString, result);
                }
            }
            return result;
        }
        public ManagementObject[] GetObjects(string queryString, bool allowQueryCaching)
        {
            ManagementObject[] result = new ManagementObject[0];
            ManagementObjectCollection collection = GetObjectCollection(queryString, allowQueryCaching);
            if (collection != null && collection.Count > 0)
            {
                result = new ManagementObject[collection.Count];
                collection.CopyTo(result, 0);
            }
            return result;
        }
    }
}
