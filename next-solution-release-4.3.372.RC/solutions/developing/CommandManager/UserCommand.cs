using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Controls;
using CommandManager.ComponentService;
using ScriptVariableValues;
using System.Windows.Threading;
using UFInterfaces.AuthenticationCredentialsProvider;
using Utilities.WPF;
using System.Web.Security;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Input;
using Opc.Ua;
using OPCUAViewModel;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UFUserEditor.ComponentService;
using System.ComponentModel;
using ViewModelLib;
using ExpressionManager;

namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum UserCommandType
    {
        Login,
        Logout,
        UnlockUser,
        UnlockAllUsers,
        Edit,
        ChangePassword,
    }


    [DataContract(Name = "UserCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class UserCommand : CommandManager
    {
        #region Declarations
#if !NET_STANDARD
        PropertyObserver<OPCUAEntityReference> observerItemViewModel;
        ExpressionEntity expressionEntity;
        bool bExpressionEntityDataContextSubscribed;
#endif

        bool bInitialized;
        #endregion

        #region Properties

        [DataMember]
        UserCommandType commandType;
        public UserCommandType CommandType
        {
            get { return commandType; }
            set
            {
                if (value == commandType)
                    return;
                commandType = value;
#if !NET_STANDARD
                OnPropertyChanged("CommandType");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("CommandType");
#endif
            }
        }

        #endregion

        #region Overrides
#if !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return CommandType == UserCommandType.UnlockUser;
                }

                return base[propertyName];
            }
        }
#endif

        public override String CommandSummary
        {
            get
            {
                return CommandType.ToString();
            }
        }


        public override String Name
        {
            get
            {
                return Properties.Resources.UserCommandName;
            }
        }

        bool isRemoteExecuting;
        public override RemoteExecute RemoteExecute()
        {
            if (CommandType != UserCommandType.UnlockUser &&
                CommandType != UserCommandType.UnlockAllUsers)
                return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };

            try
            {
                isRemoteExecuting = true;
                Execute();
            }
            catch (Exception exception)
            {
                return new RemoteExecute() { ex = exception };
            }
            finally
            {
                isRemoteExecuting = false;
            }

            return null;
        }

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            bInitialized = true;
            base.Init(entity, parent, sessionname);

#if !NET_STANDARD
            lockObject = new object();

            if (Parent != null)
            {
                rootParent = Parent;
                while (rootParent.Parent != null)
                    rootParent = rootParent.Parent;
            }

            userEditor = GetServiceUserEditor();
            if (service == null)
            {
                service = GetService();
                if (service != null)
                {
                    service.UserOnline += Service_UserOnline;
                    service.RefreshCurrentUser(rootParent.Title);
                }
            }

            if (!String.IsNullOrEmpty(Expression) || OpcuaEntityReference != null)
                sExpression = Expression;

            if (OpcuaEntityReference != null)
            {
                if (!String.IsNullOrEmpty(sExpression))
                {
                    observerItemViewModel = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        if (expressionEntity != null)
                        {
                            expressionEntity.Dispose();
                            expressionEntity = null;
                        }

                        if (n.MonitoredItemViewModel != null)
                        {
                            expressionEntity = ExpressionBucket.GetInstance(Parent).AddExpression(n.MonitoredItemViewModel, sExpression, sReverseExpression, mapCurrentParameteItems);
                        }
                    });
                }

                OpcuaEntityReference.Resolve(SessionName, parent);
                OpcuaEntityReference.SetInUse(Entity, true);
            }
            else if (!String.IsNullOrEmpty(sExpression))
            {
                if (Entity != null)
                {
                    FrameworkElement fe = null;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        fe = control.Content as FrameworkElement;
                    }

                    if (fe == null)
                        fe = Entity.ContainedObject as FrameworkElement;

                    if (fe != null)
                    {
                        var action = new Action(() =>
                        {
                            if (expressionEntity != null)
                            {
                                expressionEntity.Dispose();
                                expressionEntity = null;
                            }

                            var monitoredItemViewModel = fe.DataContext as MonitoredItemViewModel;
                            if (monitoredItemViewModel != null)
                            {
                                expressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(monitoredItemViewModel.ReferenceViewModel ?? monitoredItemViewModel, sExpression, sReverseExpression, mapCurrentParameteItems);
                            }
                        });

                        if (!bExpressionEntityDataContextSubscribed)
                        {
                            bExpressionEntityDataContextSubscribed = true;
                            fe.DataContextChanged += (s, e) => action();
                        }
                        if (fe.DataContext != null)
                            action();
                    }
                }
            }
#endif
            return true;
        }

        public override void Terminate()
        {
            if (!bInitialized)
                return;
            bInitialized = false;

#if !NET_STANDARD
            if (service != null)
            {
                service.UserOnline -= Service_UserOnline;
                service = null;
            }

            if (OpcuaEntityReference != null)
                OpcuaEntityReference.SetInUse(Entity, false);
            if (observerItemViewModel != null)
            {
                observerItemViewModel.Dispose();
                observerItemViewModel = null;
            }
            if (expressionEntity != null)
            {
                expressionEntity.Dispose();
                expressionEntity = null;
            }

            lock (lockObject)
            {
                if (subscribedModel != null)
                {
                    subscribedModel.PropertyChanged -= SubscribedModel_PropertyChanged;
                    subscribedModel = null;
                }

                if (dpUpdate != null &&
                    dpUpdate.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdate.Status != DispatcherOperationStatus.Completed)
                    dpUpdate.Abort();
            }
#endif

            base.Terminate();
        }

        public override void BlindExecute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            ExecuteOnUserInterface();
#endif
        }

        public override bool IsUICommand()
        {
            return true;
        }

        public override void Execute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            switch (CommandType)
            {
                case UserCommandType.Login:
                    {
                        if (service == null)
                            return;

                        var wnd = Control != null ? Control.FindParent<Window>() : null;
                        service.ValidateUsingCredentialsProvider(rootParent, rootParent.Title, owner: wnd);
                        break;
                    }
                case UserCommandType.Logout:
                    {
                        if (service == null)
                            return;
                        service.Logout(rootParent.Title);
                        break;
                    }
                case UserCommandType.Edit:
                    {
                        if (userEditor != null)
                        {
                            var userEditorControl = userEditor.GetRuntimeUserEditControl(rootParent);
                            if (userEditorControl == null)
                                throw new Exception(Properties.Resources.CannotEditUser);

                            userEditorControl.ClearValue(FrameworkElement.WidthProperty);
                            userEditorControl.ClearValue(FrameworkElement.HeightProperty);

                            var wnd = new GeneralDialogContent(userEditorControl)
                            {
                                DialogKeepContent = true,
                                Title = Properties.Resources.UserEditTitle,
                                HelpLink = "UserEditor"
                            };
                            var result = wnd.ShowDialog();
                            if (result == true)
                                userEditor.SaveAndRelease(rootParent);
                        }
                        break;
                    }
                case UserCommandType.ChangePassword:
                    {
                        if (userEditor != null)
                            userEditor.VerifyPasswordExpired(rootParent, currentUser, true);
                    }
                    break;
                case UserCommandType.UnlockUser:
                    {
                        if (!isRemoteExecuting && service == null)
                            return;

                        String userName = null;
                        var model = GetItemViewModel();
                        if (model != null)
                            userName = model.InvariantCultureValue;
                        else if (!isRemoteExecuting && OpcuaEntityReference == null)
                        {
                            Window owner = null;
                            if (Control != null)
                                owner = Control.FindParent<Window>();
                            if (owner == null)
                            {
                                var ie = Keyboard.FocusedElement as DependencyObject;
                                if (ie != null)
                                    owner = Window.GetWindow(ie);
                                if (owner == null)
                                    throw new Exception(Properties.Resources.UnlockUserCommandNotSupported);
                            }

                            var unlockUser = new UserControls.UnlockUser();

                            if (service == null)
                                unlockUser.DataContext = Membership.GetAllUsers();
                            else
                                unlockUser.DataContext = service.GetAllUsers(rootParent.Title);

                            unlockUser.ClearValue(FrameworkElement.WidthProperty);
                            unlockUser.ClearValue(FrameworkElement.HeightProperty);

                            var unlockDialog = new GeneralDialogContent(unlockUser, GeneralDialogButtons.OkCancelButtons)
                            {
                                Title = Properties.Resources.UnlockUserTitle,
                                Owner = owner
                            };

                            if (unlockDialog.ShowDialog() == true)
                                userName = unlockUser.cmbUser.Text;
                        }

                        if (!String.IsNullOrEmpty(userName))
                        {
                            if (isRemoteExecuting)
                            {
                                var user = Membership.GetUser(userName);
                                if (user == null || !user.UnlockUser())
                                    throw new Exception(String.Format(Properties.Resources.UnlockUserFailed, userName));
                            }
                            else
                            {
                                var result = service.UnlockUser(rootParent.Title, userName);
                                if (!result)
                                {
                                    var error = String.Format(Properties.Resources.UnlockUserFailed, userName);
                                    var ui = Parent?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                    if (ui != null)
                                        ui.ShowError(error);
#if !WINDOWS_UWP
                                    else
                                        System.Windows.MessageBox.Show(error, Name);
#endif
                                }
                            }
                        }
                        else if (isRemoteExecuting)
                            throw new Exception(Properties.Resources.CannotWriteOrReadTag);
                        break;
                    }
                case UserCommandType.UnlockAllUsers:
                    {
                        if (!isRemoteExecuting && service == null)
                            return;

                        if (isRemoteExecuting)
                        {
                            var users = Membership.GetAllUsers();
                            foreach (MembershipUser user in users)
                                user.UnlockUser();
                        }
                        else
                        {
                            using (new WaitCursor())
                            {
                                var users = service.GetAllUsers(rootParent.Title);
                                foreach (MembershipUser user in users)
                                    service.UnlockUser(rootParent.Title, user.UserName);
                            }
                        }
                        break;
                    }
            }
#endif
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute())
                return false;

#if !NET_STANDARD
            switch (CommandType)
            {
                case UserCommandType.Login:
                    {
                        return service != null;
                    }
                case UserCommandType.Logout:
                    {
                        return service != null && !String.IsNullOrEmpty(currentUser);
                    }
                case UserCommandType.UnlockUser:
                    {
                        var model = GetItemViewModel();
                        if (OpcuaEntityReference == null && model == null)
                            return true;

                        return model != null && !String.IsNullOrEmpty(model.InvariantCultureValue);
                    }
                case UserCommandType.UnlockAllUsers:
                    {
                        return true;
                    }
                case UserCommandType.Edit:
                    {
                        return userEditor != null;
                    }
                case UserCommandType.ChangePassword:
                    {
                        return userEditor != null && !String.IsNullOrEmpty(currentUser);
                    }
            }
#endif

            return false;
        }

#if !NET_STANDARD
        IDocument rootParent;
        String currentUser;
        private void Service_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUser = e.User;
        }

        IAuthenticationCredentialsProvider service;
        IAuthenticationCredentialsProvider GetService()
        {
            if (Parent == null)
                return null;
            var userEditor = GetServiceUserEditor();
            if (userEditor == null)
                return null;
            if (!userEditor.GetEnableUserManager(rootParent))
                return null;

            return Parent.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
        }

        IUFUserEditorManager userEditor;
        IUFUserEditorManager GetServiceUserEditor()
        {
            if (Parent == null)
                return null;
            var userEditor = Parent.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
            if (userEditor == null)
                return null;
            if (!userEditor.GetEnableUserManager(rootParent))
                return null;
            return userEditor;
        }
#endif
        #endregion

        #region Methods
#if !NET_STANDARD
        MonitoredItemViewModel subscribedModel;
        MonitoredItemViewModel GetItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
            if (OpcuaEntityReference == null)
            {
                if (Entity != null && Entity.ContainedObject is FrameworkElement)
                {
                    var ret = (Entity.ContainedObject as FrameworkElement).DataContext as MonitoredItemViewModel;
                    if (ret != null)
                        return ret;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        if (control.Content is FrameworkElement)
                            monitor = (control.Content as FrameworkElement).DataContext as MonitoredItemViewModel;
                    }
                }
            }
            else
                monitor = OpcuaEntityReference.MonitoredItemViewModel;
            if (expressionEntity != null)
                monitor = expressionEntity.TempVariable;
            if (monitor != null && !monitor.IsValid)
                return null;

            if (subscribedModel != monitor)
            {
                lock (lockObject)
                {
                    if (subscribedModel != null)
                        subscribedModel.PropertyChanged -= SubscribedModel_PropertyChanged;

                    subscribedModel = monitor;
                    if (subscribedModel != null)
                        subscribedModel.PropertyChanged += SubscribedModel_PropertyChanged;
                }
            }

            return monitor;
        }

        StatusCode lastStatusCode;
        DataValue lastDataValue;
        DispatcherOperation dpUpdate;
        object lockObject;
        void SubscribedModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                bool bForceDispatcherOperation = false;
                lock (lockObject)
                {
                    if (subscribedModel == null)
                        return;

                    bForceDispatcherOperation = lastDataValue == null ||
                        subscribedModel.DataValue != null && subscribedModel.DataValue.StatusCode != lastStatusCode;
                    lastDataValue = subscribedModel.DataValue;
                    if (subscribedModel.DataValue != null)
                        lastStatusCode = subscribedModel.DataValue.StatusCode;

                    if (dpUpdate == null || bForceDispatcherOperation ||
                        dpUpdate.Status == DispatcherOperationStatus.Completed ||
                        dpUpdate.Status == DispatcherOperationStatus.Aborted)
                    {
                        if (Control != null)
                            dpUpdate = Control.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                lock (lockObject)
                                {
                                    if (subscribedModel == null)
                                        return;
                                }

                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                            });
                    }
                }
            }
        }
#endif
        #endregion
    }
}
