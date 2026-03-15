using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace MembershipProviderSettings
{
    public static class MembershipProviderSettings
    {
        #region Declarations
        readonly static Dictionary<string, NameValueCollection> customSettings = new Dictionary<string, NameValueCollection>();

        static string MaxInvalidPasswordAttemptsName = "maxInvalidPasswordAttempts";
        static string UnlockableUsers = "unlockableUsers";
        #endregion

        #region Methods
        public static void SetMaxInvalidPasswordAttempts(string applicatioName, int? newValue)
        {
            if (String.IsNullOrEmpty(applicatioName))
                return;

            if (!customSettings.ContainsKey(applicatioName))
                customSettings.Add(applicatioName, new NameValueCollection());

            if (!newValue.HasValue)
                customSettings[applicatioName].Remove(MaxInvalidPasswordAttemptsName);
            else
                customSettings[applicatioName][MaxInvalidPasswordAttemptsName] = newValue.Value.ToString(CultureInfo.InvariantCulture);
        }

        public static int? GetMaxInvalidPasswordAttempts(string applicatioName)
        {
            if (!String.IsNullOrEmpty(applicatioName) && customSettings.ContainsKey(applicatioName))
            {
                var value = customSettings[applicatioName].Get(MaxInvalidPasswordAttemptsName);
                if (value != null)
                {
                    int maxInvalidPasswordAttempts;
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out maxInvalidPasswordAttempts))
                        return maxInvalidPasswordAttempts;
                }
            }

            return null;
        }

        public static void AddUnlockableUser(string applicatioName, string username)
        {
            if (String.IsNullOrEmpty(applicatioName))
                return;

            if (!customSettings.ContainsKey(applicatioName))
                customSettings.Add(applicatioName, new NameValueCollection());

            customSettings[applicatioName].Add(UnlockableUsers, username);
        }

        public static void ClearUnlockableUsers(string applicatioName)
        {
            if (String.IsNullOrEmpty(applicatioName))
                return;

            if (customSettings.ContainsKey(applicatioName))
                customSettings[applicatioName].Remove(UnlockableUsers);
        }

        public static bool IsUnlockableUser(string applicatioName, string username)
        {
            if (String.IsNullOrEmpty(applicatioName))
                return false;

            if (customSettings.ContainsKey(applicatioName))
            {
                var users = customSettings[applicatioName].GetValues(UnlockableUsers);
                return users != null && users.Contains(username, UserNameComparer.OrdinalIgnoreCase);
            }

            return false;
        }
        #endregion
    }
}
