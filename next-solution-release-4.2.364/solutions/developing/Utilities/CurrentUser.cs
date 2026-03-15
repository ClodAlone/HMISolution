using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace Utilities
{
    public class CurrentUser
    {
        public static string Name()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        public static bool IsAnAdministrator()
        {
            WindowsIdentity MyIdentity =
               WindowsIdentity.GetCurrent();
            WindowsPrincipal MyPrincipal =
               new WindowsPrincipal(MyIdentity);
            return MyPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsEqualTo(string userName, string domainName = null)
        {
            var domainUserName = String.Format("{0}\\{1}", domainName ?? System.Net.Dns.GetHostName(), userName);
            return String.Compare(CurrentUser.Name(), domainUserName, true) == 0;
        }

        static Dictionary<string, string[]> mapCacheGroups = new Dictionary<string, string[]>();
        public static string[] GetGroups(string identityName, bool bUseCache = false)
        {
            if (bUseCache)
            {
                lock (mapCacheGroups)
                {
                    if (mapCacheGroups.ContainsKey(identityName))
                        return mapCacheGroups[identityName];
                }
            }

            string[] ret = null;
            try
            {
                // extract the username and domain from the security token.
                string username = identityName;
                string domain = null;

                int index = username.IndexOf('\\');
                if (index != -1)
                {
                    domain = username.Substring(0, index);
                    username = username.Substring(index + 1);
                }

                if (domain != null)
                {
                    using (var pc = new PrincipalContext(ContextType.Domain, domain))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(pc, username);
                        if (user != null)
                        {
                            var groups = user.GetGroups();
                            ret = groups.Select(x => x.SamAccountName).ToArray();
                        }
                    }
                }
                else
                {
                    using (var pc = new PrincipalContext(ContextType.Machine))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(pc, username);
                        if (user != null)
                        {
                            var groups = user.GetGroups();
                            ret = groups.Select(x => x.SamAccountName).ToArray();
                        }
                    }
                }
            }
            catch
            { }

            if (ret != null)
            {
                lock (mapCacheGroups)
                {
                    if (mapCacheGroups.ContainsKey(identityName))
                        mapCacheGroups.Remove(identityName);
                    mapCacheGroups.Add(identityName, ret);
                }
            }

            return ret;
        }

        public static void CleanUserGroupsCache()
        {
            lock (mapCacheGroups)
            {
                mapCacheGroups.Clear();
            }
        }
    }
}
