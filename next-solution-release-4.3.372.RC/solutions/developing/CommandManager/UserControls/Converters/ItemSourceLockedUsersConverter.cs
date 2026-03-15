using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Data;

namespace CommandManager.UserControls.Converters
{
    public class ItemSourceLockedUsersConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MembershipUserCollection)
            {
                var users = new MembershipUser[(value as MembershipUserCollection).Count];
                (value as MembershipUserCollection).CopyTo(users, 0);
                return (from user in users.AsParallel()
                        where user.IsLockedOut
                        select user).ToList();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
