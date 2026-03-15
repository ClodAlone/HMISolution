using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAViewModel
{
    public struct SessionTitleModel
    {
        #region Declarations
        string sessionName;
        string hostName;
        string appName;
        string endPoint;
        #endregion

        #region Constructors
        public SessionTitleModel(string sessionName, string appName) :
            this(sessionName, null, appName, null)
        { }

        public SessionTitleModel(string sessionName, string appName, string endPoint) : 
            this(sessionName, null, appName, endPoint)
        { }

        public SessionTitleModel(string sessionName, string hostName, string appName, string endPoint)
        {
            if (string.IsNullOrEmpty(sessionName))
                throw new ArgumentNullException("sessionName");
            if (string.IsNullOrEmpty(appName))
                // throw new ArgumentNullException("appName");
                appName = endPoint;

            this.sessionName = !String.IsNullOrEmpty(sessionName) ? sessionName : null;
            this.hostName = !String.IsNullOrEmpty(hostName) ? hostName : null;
            this.appName = !String.IsNullOrEmpty(appName) ? appName : null;
            this.endPoint = !String.IsNullOrEmpty(endPoint) ? endPoint : null;
        }
        #endregion

        #region Overrides
        public override bool Equals(Object value)
        {
            if (value is SessionTitleModel)
            {
                return Equals((SessionTitleModel)value);
            }
            return false;
        }

        public bool Equals(SessionTitleModel value)
        {
            return sessionName == value.SessionName &&
                appName == value.AppName &&
                hostName == value.hostName;
        }

        public static bool Equals(SessionTitleModel s1, SessionTitleModel s2)
        {
            return s1.Equals(s2);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(sessionName);

            if (!String.IsNullOrEmpty(hostName) &&
                String.Compare(hostName, Properties.Settings.Default.localhost, true) != 0 &&
                String.Compare(hostName, Properties.Settings.Default.localhostip, true) != 0)
                builder.AppendFormat("@{0}\\{1}", hostName, appName);
            else
                builder.AppendFormat("@{0}", appName);

            if (!String.IsNullOrEmpty(endPoint))
                builder.AppendFormat("-{0}", endPoint);

            return builder.ToString();
        }
        #endregion

        #region Operators
        public static bool operator ==(SessionTitleModel s1, SessionTitleModel s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(SessionTitleModel s1, SessionTitleModel s2)
        {
            return !s1.Equals(s2);
        }
        #endregion

        #region Methods
        
        #endregion

        #region Properties
        public string SessionName
        {
            get
            {
                return sessionName;
            }
        }

        public string HostName
        {
            get
            {
                return hostName;
            }
        }

        public string AppName
        {
            get
            {
                return appName;
            }
        }

        public string EndPoint
        {
            get
            {
                return endPoint;
            }
        }
        #endregion
    }
}
