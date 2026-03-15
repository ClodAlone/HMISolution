using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MembershipProviderSettings
{
    internal class UserNameComparer : IEqualityComparer<String>
    {
        #region Declarations
        public static readonly UserNameComparer OrdinalIgnoreCase = new UserNameComparer(StringComparison.OrdinalIgnoreCase);

        readonly StringComparison stringComparison;
        #endregion

        #region Constructors
        public UserNameComparer(StringComparison stringComparison)
        {
            this.stringComparison = stringComparison;
        }
        #endregion

        #region IEqualityComparer
        public bool Equals(String strA, String strB)
        {
            return String.Compare(strA, strB, stringComparison) == 0;
        }

        public int GetHashCode(String obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.GetHashCode();
            }
        }
        #endregion
    }
}
