using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUserModel
{
    [DeferredDeletion(false)]
    public class UFPasswordHistory : XPObject
    {
        public UFPasswordHistory(Session session)
            : base(session)
        { }

        #region Properties
        private string _Password;
        [ValueConverter(typeof(EncryptedValueConverter))]
        [Size(SizeAttribute.Unlimited)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
                this.RaisePropertyChangedEvent("PasswordConfirm");
            }
        }

        private DateTime _LastChangedTime;
        public DateTime LastChangedTime
        {
            get
            {
                return _LastChangedTime;
            }
            set
            {
                SetPropertyValue("LastChangedTime", ref _LastChangedTime, value);
            }
        }

        private UFUser _UFUserAss;
        [Association("UFUser-UFPasswordHistory")]
        public UFUser UFUserAss
        {
            get
            {
                return _UFUserAss;
            }
            set
            {
                SetPropertyValue("UFUserAss", ref _UFUserAss, value);
            }
        }
        #endregion
    }
}
