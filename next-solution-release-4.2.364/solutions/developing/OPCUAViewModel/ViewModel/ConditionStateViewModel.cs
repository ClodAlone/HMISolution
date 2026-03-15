using System;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using UFInterfaces;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OPCUAViewModel.ContextMenuKey;
using System.Windows.Threading;
#endif
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace OPCUAViewModel
{
    public class ConditionStateViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public ConditionState conditionState { get; protected set; }
        public INode nodeType { get; protected set; }
#endregion

#region Constructor
        public ConditionStateViewModel(ConditionState cs, TreeViewItemViewModel parent, INode type)
            :base(parent, false)
        {
            if (cs == null)
                throw new ArgumentNullException("ConditionState");

            conditionState = cs;
            nodeType = type;

            
            Title = Source ?? Utils.Format("{0}", conditionState.SourceNode.Value);
        }
#endregion

#region Methods
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        //static PropertyInfo[] propertyInfos = typeof(ConditionStateViewModel).GetProperties(BindingFlags.Public |
        //                                                                                    BindingFlags.Instance |
        //                                                                                    BindingFlags.DeclaredOnly);
        public void UpdateConditionState(ConditionState condition, INode type)
        {
            conditionState = condition;
            nodeType = type;

            OnPropertyChanged("");
            //foreach (var propertyInfo in propertyInfos)
            //    OnPropertyChanged(propertyInfo.Name);
            // Array.ForEach(propertyInfos, propertyInfo => OnPropertyChanged(propertyInfo.Name));
        }

        void CallMethod(NodeId methodId, params Variant[] paramlist)
        {
            // build list of methods to call.
            CallMethodRequestCollection methodsToCall = new CallMethodRequestCollection();

            CallMethodRequest request = new CallMethodRequest();

            request.ObjectId = conditionState.NodeId;
            request.MethodId = methodId;
            request.Handle = this;

            foreach(var v in paramlist)
            {
                request.InputArguments.Add(v);
            }

            methodsToCall.Add(request);

            if (methodsToCall.Count == 0)
                return;

            // call the methods.
            CallMethodResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            var vm = Parent as MonitoredItemViewModel;
            vm.monitoredItem.Subscription.Session.Call(
                null,
                methodsToCall,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, methodsToCall);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, methodsToCall);

            for (int ii = 0; ii < results.Count; ii++)
            {
                if (StatusCode.IsBad(results[ii].StatusCode))
                {
                    ConditionLastError = Utils.Format("{0}", results[ii].StatusCode);
                }
            }
        }

        bool bCallingEnable;
        void CallEnable()
        {
            bCallingEnable = true;
            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    CallMethod(MethodIds.ConditionType_Enable);
                }
                catch (Exception ex)
                {
                    
                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingEnable = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
        }

        bool IsEnableCallEnable
        {
            get
            {
                if (!bCallingEnable && conditionState.EnabledState != null && conditionState.EnabledState.Id != null)
                    return !conditionState.EnabledState.Id.Value;
                return false;
            }
        }

        bool bCallingDisable;
        void CallDisable()
        {
            bCallingDisable = true;
            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    CallMethod(MethodIds.ConditionType_Disable);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingDisable = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
        }

        bool IsEnableCallDisable
        {
            get
            {
                if (!bCallingDisable && conditionState.EnabledState != null && conditionState.EnabledState.Id != null)
                    return conditionState.EnabledState.Id.Value;
                return false;
            }
        }

        bool bCallingAddComment;
        internal void CallAddComment()
        {
            //CallMethod(MethodIds.ConditionType_AddComment, conditionState.EventId.Value, UserTempComment);
            //UserTempComment = String.Empty;

            bCallingAddComment = true;
            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    CallMethod(MethodIds.ConditionType_AddComment, conditionState.EventId.Value, UserTempComment);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingAddComment = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
                UserTempComment = String.Empty;
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
            //UserTempComment = String.Empty;
        }

        internal bool IsEnableCallAddComment
        {
            get
            {
                return true;
            }
        }


        bool bCallingAcknowledge;
        public void CallAcknowledge(bool bSynchro = false)
        {
            bCallingAcknowledge = true;
            if (bSynchro)
            {
                try
                {
                    CallMethod(MethodIds.AcknowledgeableConditionType_Acknowledge, conditionState.EventId.Value, UserTempComment);
                }
                catch (Exception ex)
                {

                }
                bCallingAcknowledge = false;
                UserTempComment = String.Empty;
                return;
            }

            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    CallMethod(MethodIds.AcknowledgeableConditionType_Acknowledge, conditionState.EventId.Value, UserTempComment);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingAcknowledge = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
            UserTempComment = String.Empty;
        }

        public bool IsEnableCallAcknowledge
        {
            get
            {
                if (!bCallingAcknowledge && conditionState is AcknowledgeableConditionState)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.AckedState != null && ackcondition.AckedState.Id != null)
                        return !ackcondition.AckedState.Id.Value;
                }

                return false;
            }
        }

        bool bCallingConfirm;
        public void CallConfirm(bool bSynchro = false)
        {
            bCallingConfirm = true;
            if (bSynchro)
            {
                try
                {
                    CallMethod(MethodIds.AcknowledgeableConditionType_Confirm, conditionState.EventId.Value, UserTempComment);
                }
                catch (Exception ex)
                {

                }
                bCallingConfirm = false;
                UserTempComment = String.Empty;
                return;
            }

            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    CallMethod(MethodIds.AcknowledgeableConditionType_Confirm, conditionState.EventId.Value, UserTempComment);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingConfirm = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
            UserTempComment = String.Empty;
        }

        public bool IsEnableCallConfirm
        {
            get
            {
                if (!bCallingConfirm && conditionState is AcknowledgeableConditionState)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.ConfirmedState != null && ackcondition.ConfirmedState.Id != null)
                        return !ackcondition.ConfirmedState.Id.Value;
                }

                return false;
            }
        }

        void CallRespond()
        {
            CallMethod(MethodIds.DialogConditionType_Respond, DialogSelectedResponse);
            DialogSelectedResponse = -1;
        }

        bool IsEnableCallRespond
        {
            get
            {
                if (conditionState is DialogConditionState && DialogSelectedResponse != -1)
                    return true;

                return false;
            }
        }

        void Shelve(bool shelving, bool oneShot, double shelvingTime)
        {
            // build list of methods to call.
            CallMethodRequestCollection methodsToCall = new CallMethodRequestCollection();

            // check if the node supports shelving.
            var vm = Parent as MonitoredItemViewModel;
            BaseObjectState shelvingState = conditionState.FindChild(vm.monitoredItem.Subscription.Session.SystemContext, BrowseNames.ShelvingState) as BaseObjectState;
            if (shelvingState == null)
                return;

            CallMethodRequest request = new CallMethodRequest();

            request.ObjectId = shelvingState.NodeId;
            request.Handle = conditionState;

            // select the method to call.
            if (!shelving)
            {
                request.MethodId = MethodIds.ShelvedStateMachineType_Unshelve;
            }
            else
            {
                if (oneShot)
                {
                    request.MethodId = MethodIds.ShelvedStateMachineType_OneShotShelve;
                }
                else
                {
                    request.MethodId = MethodIds.ShelvedStateMachineType_TimedShelve;
                    request.InputArguments.Add(new Variant(shelvingTime));
                }
            }

            methodsToCall.Add(request);

            if (methodsToCall.Count == 0)
                return;

            // call the methods.
            CallMethodResultCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            vm.monitoredItem.Subscription.Session.Call(
                null,
                methodsToCall,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, methodsToCall);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, methodsToCall);

            for (int ii = 0; ii < results.Count; ii++)
            {
                if (StatusCode.IsBad(results[ii].StatusCode))
                {
                    ConditionLastError = Utils.Format("{0}", results[ii].StatusCode);
                }
            }
        }

        bool bCallingShelve;
        void CallShelve()
        {
            bCallingShelve = true;
            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    Shelve(true, ShelveOneShot, ShelvingTime);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingShelve = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
        }

        void CallUnshelve()
        {
            bCallingShelve = true;
            var task1 = Task.Factory.StartNew(delegate
            {
                try
                {
                    Shelve(false, ShelveOneShot, ShelvingTime);
                }
                catch (Exception ex)
                {

                }
            });
#if !WINDOWS_UWP
#if !NET_STANDARD
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            var task2 = task1.ContinueWith(ret =>
            {
                bCallingShelve = false;
#if !NET_STANDARD
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
            }, TaskScheduler.FromCurrentSynchronizationContext());
#endif
        }

        bool IsEnableCallShelve
        {
            get
            {
                if (bCallingShelve)
                    return false;
                var vm = Parent as MonitoredItemViewModel;
                if (vm == null || vm.monitoredItem.Subscription == null ||
                    vm.monitoredItem.Subscription.Session == null)
                    return false;
                BaseObjectState shelvingState = conditionState.FindChild(vm.monitoredItem.Subscription.Session.SystemContext, BrowseNames.ShelvingState) as BaseObjectState;
                return shelvingState != null;
            }
        }
        public bool IsActiveBranch()
        {
            if (conditionState.BranchId != null)
            {
                var branch = Utils.Format("{0}", conditionState.BranchId.Value);
                return branch.Equals("i=0");
            }
            return false;
        }


#endregion

#region Commands

        RelayCommand _enableCommand;
        public ICommand EnableCommand
        {
            get
            {
                if (_enableCommand == null)
                {
                    _enableCommand = new RelayCommand(
                        param => CallEnable(),
                        param => IsEnableCallEnable
                        );
                }
                return _enableCommand;
            }
        }

        RelayCommand _disableCommand;
        public ICommand DisableCommand
        {
            get
            {
                if (_disableCommand == null)
                {
                    _disableCommand = new RelayCommand(
                        param => CallDisable(),
                        param => IsEnableCallDisable
                        );
                }
                return _disableCommand;
            }
        }

        RelayCommand _addCommentCommand;
        public ICommand AddCommentCommand
        {
            get
            {
                if (_addCommentCommand == null)
                {
                    _addCommentCommand = new RelayCommand(
                        param => CallAddComment(),
                        param => IsEnableCallAddComment
                        );
                }
                return _addCommentCommand;
            }
        }
        RelayCommand _acknowledgeCommand;
        public ICommand AcknowledgeCommand
        {
            get
            {
                if (_acknowledgeCommand == null)
                {
                    _acknowledgeCommand = new RelayCommand(
                        param => CallAcknowledge(),
                        param => IsEnableCallAcknowledge
                        );
                }
                return _acknowledgeCommand;
            }
        }

        RelayCommand _confirmCommand;
        public ICommand ConfirmCommand
        {
            get
            {
                if (_confirmCommand == null)
                {
                    _confirmCommand = new RelayCommand(
                        param => CallConfirm(),
                        param => IsEnableCallConfirm
                        );
                }
                return _confirmCommand;
            }
        }

        RelayCommand _shelveCommand;
        public ICommand ShelveCommand
        {
            get
            {
                if (_shelveCommand == null)
                {
                    _shelveCommand = new RelayCommand(
                        param => CallShelve(),
                        param => IsEnableCallShelve
                        );
                }
                return _shelveCommand;
            }
        }

        RelayCommand _unshelveCommand;
        public ICommand UnshelveCommand
        {
            get
            {
                if (_unshelveCommand == null)
                {
                    _unshelveCommand = new RelayCommand(
                        param => CallUnshelve(),
                        param => IsEnableCallShelve
                        );
                }
                return _unshelveCommand;
            }
        }

#endregion

#region Properties

        public NodeId NodeId
        {
            get
            {
                return conditionState.NodeId;
            }
        }

        public String NodeIdString
        {
            get
            {
                return conditionState.NodeId.ToString();
            }
        }

        public String DialogConditionPrompt
        {
            get
            {
                if (conditionState is DialogConditionState)
                    return Utils.Format("{0}", BaseVariableState.GetValue((conditionState as DialogConditionState).Prompt));
                return String.Empty;
            }
        }

        // configure the buttons.
        LocalizedText[] DialogConditionResponses
        {
            get
            {
                if (conditionState is DialogConditionState)
                    return BaseVariableState.GetValue((conditionState as DialogConditionState).ResponseOptionSet);
                return null;
            }
        }


        bool _shelveOneShot;
        public bool ShelveOneShot
        {
            get
            {
                return _shelveOneShot;
            }
            set
            {
                if (value == _shelveOneShot)
                    return;
                _shelveOneShot = value;
                OnPropertyChanged("ShelveOneShot");
            }
        }

        double _shelvingTime;
        public double ShelvingTime
        {
            get
            {
                return _shelvingTime;
            }
            set
            {
                if (value == _shelvingTime)
                    return;
                _shelvingTime = value;
                OnPropertyChanged("ShelvingTime");
            }
        }

        int _dialogSelectedResponse = -1;
        public int DialogSelectedResponse
        {
            get
            {
                return _dialogSelectedResponse;
            }
            set
            {
                if (value == _dialogSelectedResponse)
                    return;
                _dialogSelectedResponse = value;
                OnPropertyChanged("DialogSelectedResponse");
            }
        }

        LocalizedText _userTempComment = String.Empty;
        public LocalizedText UserTempComment
        {
            get
            {
                return _userTempComment;
            }
            set
            {
                if (value == _userTempComment)
                    return;
                _userTempComment = value;
                OnPropertyChanged("UserTempComment");
            }
        }

        String _conditionLastError = String.Empty;
        public String ConditionLastError
        {
            get
            {
                return _conditionLastError;
            }
            set
            {
                if (value == _conditionLastError)
                    return;
                _conditionLastError = value;
                OnPropertyChanged("ConditionLastError");
            }
        }

        public String Source 
        {
            get 
            {
                if (conditionState.SourceName != null)
                    return Utils.Format("{0}", conditionState.SourceName.Value);
                return null;
            }
        }

        public String SourceNode
        {
            get
            {
                if (conditionState.SourceNode != null)
                    return Utils.Format("{0}", conditionState.SourceNode.Value);
                return null;
            }
        }
        public String Condition 
        {
            get 
            {
                if (conditionState.ConditionName != null)
                    return Utils.Format("{0}", conditionState.ConditionName.Value);
                return String.Empty;
            }
        }

        public String Branch 
        {
            get 
            {
                if (conditionState.BranchId != null)
                    return Utils.Format("{0}", conditionState.BranchId.Value);
                return String.Empty;
            }
        }

        public String BranchText
        {
            get
            {
                if (conditionState.BranchId != null)
                {
                    var branch = Utils.Format("{0}", conditionState.BranchId.Value);
                    return branch.Equals("i=0") ? Properties.Resource.BranchActive : Properties.Resource.BranchDisabled;
                }
                return String.Empty;
            }
        }

        public String Type 
        {
            get 
            {
                return Utils.Format("{0}", nodeType);
            }
        }

        public ushort? Severity 
        {
            get 
            {
                if (conditionState.Severity != null)
                    return conditionState.Severity.Value;
                return null;
            }
        }

        public String Quality
        {
            get
            {
                if (conditionState.Quality != null)
                    return String.Format("{0}", conditionState.Quality.Value);

                return String.Format("{0}", StatusCodes.Good);
            }
        }

        public DateTime? Time 
        {
            get 
            {
                if (conditionState.Time != null)
                    return conditionState.Time.Value;
                return null;
            }
        }

        public DateTime? EnabledTransitionTime
        {
            get
            {
                if (conditionState.EnabledState != null && conditionState.EnabledState.TransitionTime != null)
                    return conditionState.EnabledState.TransitionTime.Value;
                return null;
            }
        }

        public DateTime? ActiveTransitionTime
        {
            get
            {
                if (conditionState is AlarmConditionState)
                {
                    var alarmcondition = conditionState as AlarmConditionState;
                    if (alarmcondition.ActiveState != null && alarmcondition.ActiveState.TransitionTime != null)
                        return alarmcondition.ActiveState.TransitionTime.Value;
                }

                return null;
            }
        }      

        public DateTime? AckedTransitionTime
        {
            get
            {
                if (IsAcknowledgeableCondition && !NeedsAcknoledge)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.AckedState != null && ackcondition.AckedState.TransitionTime != null)
                        return ackcondition.AckedState.TransitionTime.Value;
                }
                
                return null;
            }
        }

        public DateTime? ConfirmedTransitionTime
        {
            get
            {
                if (IsAcknowledgeableCondition && !NeedsConfirm && !NeedsAcknoledge)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.ConfirmedState != null && ackcondition.ConfirmedState.TransitionTime != null)
                        return ackcondition.ConfirmedState.TransitionTime.Value;
                }

                return null;
            }
        }

        public DateTime? SuppressedTransitionTime
        {
            get
            {
                if (conditionState is AlarmConditionState)
                {
                    var alarmcondition = conditionState as AlarmConditionState;
                    if (alarmcondition.SuppressedState != null && alarmcondition.SuppressedState.TransitionTime != null)
                        return alarmcondition.SuppressedState.TransitionTime.Value;
                }

                return null;
            }
        }

        public DateTime? ShelvingTransitionTime
        {
            get
            {
                if (conditionState is AlarmConditionState && EnabledState.Contains(Opc.Ua.BrowseNames.TimedShelved))
                {
                    var alarmcondition = conditionState as AlarmConditionState;
                    if (alarmcondition.ShelvingState != null && alarmcondition.ShelvingState.LastTransition != null &&
                        alarmcondition.ShelvingState.LastTransition.TransitionTime != null)
                        return alarmcondition.ShelvingState.LastTransition.TransitionTime.Value;
                }

                return null;
            }
        }

        public TimeZoneDataType LocalTime
        {
            get
            {
                if (conditionState.LocalTime != null)
                    return conditionState.LocalTime.Value;
                return null;
            }
        }

        public DateTime? ReceiveTime
        {
            get
            {
                if (conditionState.ReceiveTime != null)
                    return conditionState.ReceiveTime.Value;
                return null;
            }
        }

        public String EnabledState 
        {
            get 
            {
                if (conditionState.EnabledState != null && conditionState.EnabledState.EffectiveDisplayName != null)
                    return Utils.Format("{0}", conditionState.EnabledState.EffectiveDisplayName.Value);
                return String.Empty;
            }
        }

        public String Message 
        {
            get 
            {
                if (conditionState.Message != null)
                    return Utils.Format("{0}", conditionState.Message.Value);
                return String.Empty;
            }
        }

       
        public String Comment 
        {
            get 
            {
                if (conditionState.Comment != null)
                    return Utils.Format("{0}", conditionState.Comment.Value);
                return String.Empty;
            }
        }

        public bool? Retain 
        {
            get 
            {
                if (conditionState.Retain != null)
                    return conditionState.Retain.Value;
                return null;
            }
        }

        public bool IsAcknowledgeableCondition
        {
            get
            {
                return conditionState is AcknowledgeableConditionState;
            }
        }
        
        public bool NeedsAcknoledge
        {
            get
            {
                if (conditionState is AcknowledgeableConditionState)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.AckedState != null && ackcondition.AckedState.Id != null)
                        return !ackcondition.AckedState.Id.Value;
                }

                return false;
            }
        }

        public bool NeedsConfirm
        {
            get
            {
                if (conditionState is AcknowledgeableConditionState)
                {
                    var ackcondition = conditionState as AcknowledgeableConditionState;
                    if (ackcondition.ConfirmedState != null && ackcondition.ConfirmedState.Id != null)
                        return !ackcondition.ConfirmedState.Id.Value;
                }

                return false;
            }
        }

#endregion

/*
#region Overrides

public override event PropertyChangedEventHandler PropertyChanged;

bool bDispatchedIdle;
protected override void OnPropertyChanged(string propertyName)
{
    base.OnPropertyChanged(propertyName);

    if (bObjectDisposed)
        return;

    PropertyChangedEventHandler handler = PropertyChanged;
    if (handler != null)
    {
        DependencyObject dispatcherObject = handler.Target as DependencyObject;
        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false && dispatcherObject.Dispatcher.Thread.IsAlive)
        {
            if (!bDispatchedIdle)
            {
                bDispatchedIdle = true;
                var disp = dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
                    {
                        // Dirty the commands registered with CommandManager,
                        // such as our Save command, so that they are queried
                        // to see if they can execute now.
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        bDispatchedIdle = false;
                    }));
                if (disp.Status == DispatcherOperationStatus.Aborted)
                    bDispatchedIdle = false;
            }
        }
        else
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }
}

#endregion
*/

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/connect_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMConnect", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                ConditionStateMenu wm = new ConditionStateMenu();
                wm.InitializeComponent();

                return wm["ContextMenuKey"] as ContextMenu;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                return new OPCUAViewModel.UserControls.ConditionStateViewModel();
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        public object ContainedObject
        {
            get { return conditionState; }
        }

        public object EntityParent
        {
            get { return Parent; }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

#endregion
    }
}
