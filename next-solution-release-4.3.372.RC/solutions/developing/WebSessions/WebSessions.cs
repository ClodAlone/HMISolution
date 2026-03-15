using System;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace WebSessions
{
    public class WebSessions
    {
        #region Declarations
        readonly Semaphore activator;
        readonly long maxSessions = 0;
        readonly bool bFoundLicense;
        #endregion

        #region Constructors
        public WebSessions(bool checkLic = false)
        {
#if !DEBUG
            bFoundLicense = !MSZ.MSZView.CheckState(true);
            if (!checkLic || bFoundLicense)
                maxSessions = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ0HNh4VycfQXLY5Z6lNe+QBGr0uKoS0SZW/4j3jqDSg="/* WCL5 */);
#else
            bFoundLicense = true;
            maxSessions = 50;
#endif
            if (maxSessions > 0)
            {
                var semaphoreName = String.Format("Global\\{0}", Assembly.GetExecutingAssembly().FullName);
                try
                {
                    activator = Semaphore.OpenExisting(semaphoreName);
                }
                catch (NotSupportedException)
                {
                    activator = new Semaphore((int)maxSessions, (int)maxSessions);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    var semaphoreSecurity = new SemaphoreSecurity();
                    semaphoreSecurity.AddAccessRule(new SemaphoreAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        SemaphoreRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
#if !NET_STANDARD
                    bool createdNew = false;
                    activator = new Semaphore((int)maxSessions, (int)maxSessions, semaphoreName, out createdNew, semaphoreSecurity);
#else
                    bool createdNew = false;
                    activator = new Semaphore((int)maxSessions, (int)maxSessions, semaphoreName, out createdNew);
                    activator.SetAccessControl(semaphoreSecurity);
#endif
                }
            }
        }
        #endregion

        #region Public Method
        public bool Acquire()
        {
            try
            {
                if (activator != null)
                    return activator.WaitOne(1000);
            }
            catch (AbandonedMutexException ex)
            {
                return true;
            }

            return false;
        }

        public void Release()
        {
            if (activator != null)
                activator.Release();
        }
        #endregion

        #region Properties
        public long MaxSessions
        {
            get
            {
                return maxSessions;
            }
        }

        public bool FoundLicense
        {
            get
            {
                return bFoundLicense;
            }
        }
        #endregion
    }
}
