using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace AspNetSqlProvider
{
    public class ConfigurableSqlMembershipProvider : SqlMembershipProvider
    {
        #region Overrides
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                var value = MembershipProviderSettings.MembershipProviderSettings.GetMaxInvalidPasswordAttempts(ApplicationName);
                if (value.HasValue)
                    return value.Value;

                return base.MaxInvalidPasswordAttempts;
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            if (MembershipProviderSettings.MembershipProviderSettings.IsUnlockableUser(ApplicationName, username))
                UnlockUser(username);

            return base.ValidateUser(username, password);
        }
        #endregion
    }
}
