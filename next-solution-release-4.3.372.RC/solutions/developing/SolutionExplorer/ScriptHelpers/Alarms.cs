using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using UFInterfaces;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUAEditor.ComponentService;
using Utilities;
using ViewModelLib;

namespace UFProjectManager.ScriptHelpers
{
    public class Alarms : IDisposable, IEntityReference
    {
        readonly IUFUAEditorManager ufuaEditor;
        readonly UFProjectDocument Document;
        readonly IAuthenticationCredentialsProvider credentialsProviderReference;
        OPCUAEntityReference serverReference;
        bool bSubscribed;
        PropertyObserver<OPCUAEntityReference> observer;
        SafeObservableCollection<ConditionStateViewModel> conditionStateList;
        private UserIdentity userIdentity;

        public Alarms(UFProjectDocument p, IUFUAEditorManager d, IAuthenticationCredentialsProvider c)
        {
            Document = p;
            ufuaEditor = d;
            credentialsProviderReference = c;
            var server = ufuaEditor.GetServerEntityReference(Document);
            if (server != null)
            {
                serverReference = server.FromXml<OPCUAEntityReference>();
                Subscribe();
            }

            if (credentialsProviderReference != null)
            {
                credentialsProviderReference.UserOnline += AuthenticationCredentialsProvider_UserOnline;
                credentialsProviderReference.RefreshCurrentUser(Document.Title);
            }  
        }

        void Subscribe()
        {
            if (!IsValid)
                return;

            observer = new PropertyObserver<OPCUAEntityReference>(serverReference);
            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                conditionStateList = n.MonitoredItemViewModel.ConditionStateList;
                SetUserIdentity();
            });

            serverReference.Resolve(SessionName);
            serverReference.SetInUse(this, true);
            bSubscribed = true;
        }

        void Unsubscribe()
        {
            if (!bSubscribed)
                return;
            serverReference.SetInUse(this, false);
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }
        }

        String sessionName = "ScriptAlarmSession";
        public String SessionName
        {
            get
            {
                return sessionName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                var newsessionName = String.Format("{0}-{1}", Thread.CurrentThread.ManagedThreadId, value);
                if (sessionName != newsessionName)
                {
                    Unsubscribe();
                    serverReference = new OPCUAEntityReference(serverReference);
                    sessionName = newsessionName;
                    Subscribe();
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return serverReference != null && serverReference.IsValid;
            }
        }

        public bool IsReady
        {
            get
            {
                return IsValid && serverReference.MonitoredItemViewModel != null && conditionStateList != null;
            }
        }

        public void AckAllAlarms()
        {
            if (!IsReady)
                return;
            if (serverReference.MonitoredItemViewModel.ConditionAcknowledgeAllCommand.CanExecute(null))
                serverReference.MonitoredItemViewModel.ConditionAcknowledgeAllCommand.Execute(null);
        }

        public void ConfirmAllAlarms()
        {
            if (!IsReady)
                return;
            if (serverReference.MonitoredItemViewModel.ConditionConfirmAllCommand.CanExecute(null))
                serverReference.MonitoredItemViewModel.ConditionConfirmAllCommand.Execute(null);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            if (credentialsProviderReference != null)
                credentialsProviderReference.UserOnline -= AuthenticationCredentialsProvider_UserOnline;

            Unsubscribe();
        }

        private void SetUserIdentity()
        {
            try
            {
                if (userIdentity == null)
                    RealTimeConnectionManagerViewModel.SetUserIdentity(SessionName, null, new StringCollection());
                else
                    RealTimeConnectionManagerViewModel.SetUserIdentity(SessionName, userIdentity, new StringCollection());
            }
            catch (Exception ex)
            {   }
        }

        public void SetUserIdentity(UserIdentity userIdentity)
        {
            this.userIdentity = userIdentity;
            SetUserIdentity();
        }

        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs e)
        {
            userIdentity = new UserIdentity(e.User, e.Password);
        }
        #region EnityReference

        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Media.ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get { return serverReference; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get { return Document; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get { return Properties.Resources.AlarmsScript; }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        #endregion
    }
}
