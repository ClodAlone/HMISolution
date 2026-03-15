using System;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using UFInterfaces;
using Utilities;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using OPCUAViewModel.ContextMenuKey;
using System.Windows.Threading;
#endif
using System.IO;
using System.ComponentModel;
using System.Windows;

namespace OPCUAViewModel
{
    public class SystemEventStateViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public BaseEventState SystemEventState { get; protected set; }
        public INode nodeType { get; protected set; }

        private INode method;
#endregion

#region Constructor
        public SystemEventStateViewModel(BaseEventState cs, TreeViewItemViewModel parent, INode type)
            :base(parent, false)
        {
            if (cs == null)
                throw new ArgumentNullException("ConditionState");

            SystemEventState = cs;
            nodeType = type;

            Title = Source ?? String.Format("{0}", SystemEventState.SourceNode.Value);
        }
#endregion

#region Methods
        protected override void OnDispose()
        {
            base.OnDispose();
        }

        void CallClearAll()
        {
            var vm = Parent as MonitoredItemViewModel;
            vm.SystemEventStateList.Clear();
        }

        bool IsEnableCallClearAll
        {
            get
            {
                var vm = Parent as MonitoredItemViewModel;
                return vm.SystemEventStateList.Count > 0;
            }
        }

#endregion

#region Commands

        RelayCommand _clearAllCommand;
        public ICommand ClearAllCommand
        {
            get
            {
                if (_clearAllCommand == null)
                {
                    _clearAllCommand = new RelayCommand(
                        param => CallClearAll(),
                        param => IsEnableCallClearAll
                        );
                }
                return _clearAllCommand;
            }
        }

#endregion

#region Properties

        public String Source 
        {
            get 
            {
                if (SystemEventState.SourceName != null)
                    return Utils.Format("{0}", SystemEventState.SourceName.Value);
                return null;
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
                if (SystemEventState.Severity != null)
                    return SystemEventState.Severity.Value;
                return null;
            }
        }

        public DateTime? Time 
        {
            get 
            {
                if (SystemEventState.Time != null)
                    return SystemEventState.Time.Value;
                return null;
            }
        }

        public TimeZoneDataType LocalTime
        {
            get
            {
                if (SystemEventState.LocalTime != null)
                    return SystemEventState.LocalTime.Value;
                return null;
            }
        }

        public DateTime? ReceiveTime
        {
            get
            {
                if (SystemEventState.ReceiveTime != null)
                    return SystemEventState.ReceiveTime.Value;
                return null;
            }
        }

        public String Message 
        {
            get 
            {
                if (SystemEventState.Message != null)
                    return Utils.Format("{0}", SystemEventState.Message.Value);
                return null;
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
            get { return null; }
        }

        public ContextMenu contextMenu
        {
            get { return null; }
        }

        public Object Tooltip
        {
            get { return null; }
        }

        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        public object ContainedObject
        {
            get { return SystemEventState; }
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
