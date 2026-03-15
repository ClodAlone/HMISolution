using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using CommandManager.ComponentService;
using ScreenManager.ComponentService;
#endif
#if !WINDOWS_UWP
using System.ComponentModel;
#if !NET_STANDARD
using System.Windows.Controls;

#endif
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;


namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum eExecutionModeEx
    {
        Normal,
        ModalPopup,
        FramePopup,
        ContextPopup,
        Shared,
        Stop
    }
    public class OpenScreenCommandRemoteExecute : RemoteExecute
    {
        public Uri ParameterFile;
        public bool PreserveCurrentParameterFile;
        public bool OffsetRelativeToScreen;
    }

    [DataContract(Name = "OpenScreenCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OpenScreenCommand : CommandManager
    {
        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            requestedMonitor = -1;
            x = Double.NaN;
            y = Double.NaN;
            autoCloseSecs = 0;
        }

        #region Properties

        [DataMember]
        Uri screenName;
        public Uri ScreenName
        {
            get { return screenName; }
            set
            {
                if (value == screenName)
                    return;
                screenName = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ScreenName");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        Uri parameterFile;
        public Uri ParameterFile
        {
            get { return parameterFile; }
            set
            {
                if (value == parameterFile)
                    return;
                parameterFile = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ParameterFile");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        bool preserveCurrentParameterFile;
        public bool PreserveCurrentParameterFile
        {
            get { return preserveCurrentParameterFile; }
            set
            {
                if (value == preserveCurrentParameterFile)
                    return;
                preserveCurrentParameterFile = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("PreserveCurrentParameterFile");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("PreserveCurrentParameterFile");
#endif
            }
        }

        [DataMember]
        bool offsetRelativeToScreen;
        public bool OffsetRelativeToScreen
        {
            get { return offsetRelativeToScreen; }
            set
            {
                if (value == offsetRelativeToScreen)
                    return;
                offsetRelativeToScreen = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("OffsetRelativeToScreen");
#endif
            }
        }

        [DataMember]
        int requestedMonitor = -1;
        public int RequestedMonitor
        {
            get { return requestedMonitor; }
            set
            {
                if (value == requestedMonitor)
                    return;
                requestedMonitor = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("RequestedMonitor");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }


        eExecutionModeEx GetExecutionModeEx()
        {
            switch (executionMode)
            {
                case ExecutionMode.Normal:
                    return eExecutionModeEx.Normal;
                //return ExecutionModeEx.Normal;
                case ExecutionMode.Synchro:
                    if (!SynchroFrame && !SynchroPopup)
                        return eExecutionModeEx.ModalPopup;
                    if (SynchroPopup)
                        return eExecutionModeEx.ContextPopup;
                    if (SynchroFrame)
                        return eExecutionModeEx.FramePopup;
                    return eExecutionModeEx.Normal;
                case ExecutionMode.Shared:
                    return eExecutionModeEx.Normal;
                case ExecutionMode.Stop:
                    return eExecutionModeEx.Stop;
            }
            return eExecutionModeEx.Normal;
        }
        [DataMember]
        eExecutionModeEx? executionModeEx;
        public eExecutionModeEx? ExecutionModeEx
        {
            get
            {
                if (executionModeEx.HasValue)
                    return executionModeEx.Value;
                return GetExecutionModeEx();
            }
            set
            {
                if (executionModeEx == value)
                    return;
                executionModeEx = value;
                switch (executionModeEx)
                {
                    case eExecutionModeEx.Normal:
                        ExecutionMode = ExecutionMode.Normal;
                        break;
                    case eExecutionModeEx.ModalPopup:
                        ExecutionMode = ExecutionMode.Synchro;
                        SynchroPopup = false;
                        SynchroFrame = false;
                        break;
                    case eExecutionModeEx.FramePopup:
                        ExecutionMode = ExecutionMode.Synchro;
                        SynchroPopup = false;
                        SynchroFrame = true;
                        break;
                    case eExecutionModeEx.ContextPopup:
                        ExecutionMode = ExecutionMode.Synchro;
                        SynchroPopup = true;
                        SynchroFrame = false;
                        break;
                    case eExecutionModeEx.Shared:
                        ExecutionMode = ExecutionMode.Shared;
                        break;
                    case eExecutionModeEx.Stop:
                        ExecutionMode = ExecutionMode.Stop;
                        break;

                }
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ExecutionModeEx");
#endif
            }
        }

        [DataMember]
        ExecutionMode executionMode;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
            set
            {
                if (executionMode == value)
                    return;
                executionMode = value;

#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ExecutionMode");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("ExecutionMode");
#endif
            }
        }

        [DataMember]
        private bool synchroPopup;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool SynchroPopup
        {
            get { return synchroPopup; }
            set
            {
                if (synchroPopup == value)
                    return;
                synchroPopup = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("SynchroPopup");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("SynchroPopup");
#endif
            }
        }

        [DataMember]
        private bool synchroFrame;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool SynchroFrame
        {
            get { return synchroFrame; }
            set
            {
                if (synchroFrame == value)
                    return;
                synchroFrame = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("SynchroFrame");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("SynchroFrame");
#endif
            }
        }

        [DataMember]
        private bool isRelative;
        public bool IsRelative
        {
            get { return isRelative; }
            set
            {
                if (isRelative == value)
                    return;
                isRelative = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("IsRelative");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private double x = Double.NaN;
        public double X
        {
            get { return x; }
            set
            {
                if (x == value)
                    return;
                x = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("X");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private double y = Double.NaN;
        public double Y
        {
            get { return y; }
            set
            {
                if (y == value)
                    return;
                y = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Y");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private int autoCloseSecs = 0;
        public int AutoCloseSecs
        {
            get { return autoCloseSecs; }
            set
            {
                if (autoCloseSecs == value)
                    return;
                autoCloseSecs = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("AutoCloseSecs");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        #endregion

        #region Overrides

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "AutoCloseSecs")
            {
                if (AutoCloseSecs < 0)
                    return Properties.Resources.InvalidDelayCommandSecs;
            }

            return base.PerformValidation(propertyName);
        }

        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "ParameterFile")
                {
                    return !PreserveCurrentParameterFile && ExecutionMode != ExecutionMode.Stop;
                }
                else if (propertyName == "SynchroPopup")
                {
                    return ExecutionMode == ExecutionMode.Synchro && (!SynchroFrame || SynchroPopup);
                }
                else if (propertyName == "SynchroFrame" ||
                         propertyName == "IsRelative" ||
                         propertyName == "AutoCloseSecs")
                {
                    return ExecutionMode == ExecutionMode.Synchro && !SynchroPopup;
                }
                else if (propertyName == "X" || propertyName == "Y")
                {
                    return ExecutionMode == ExecutionMode.Synchro;
                }
                else if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return false;
                }
                else if(propertyName == "PreserveCurrentParameterFile")
                {
                    return executionMode != ExecutionMode.Stop;
                }
                else if(propertyName == "RequestedMonitor")
                {
                    return executionMode != ExecutionMode.Stop && (GetExecutionModeEx() != eExecutionModeEx.ContextPopup);
                }

                return base[propertyName];
            }
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.OpenScreenName;
            }
        }

        public override bool IsUICommand()
        {
            return true;
        }

#if !WINDOWS_UWP
        public override String CommandSummary
        {
            get
            {
                if (ScreenName == null)
                    return String.Empty;
                //return System.IO.Path.GetFileNameWithoutExtension(ScreenName.GetPathString());
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ScreenName.GetPathString()), System.IO.Path.GetFileNameWithoutExtension(ScreenName.GetPathString()));
            }
        }

        public override RemoteExecute RemoteExecute()
        {
            return new OpenScreenCommandRemoteExecute() { executionMode = ExecutionMode, uri = screenName, ParameterFile = ParameterFile, PreserveCurrentParameterFile = PreserveCurrentParameterFile, OffsetRelativeToScreen = OffsetRelativeToScreen, Parent = Parent };
        }
#endif
        public override void Execute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            
            if (Control != null)
            {
#if !WINDOWS_UWP
                Control.Dispatcher.BeginInvokeIfRequired(() =>
                {
                    OpenScreen();
                });
#else
                RunOnUIThread.RunIfRequired(() =>
                {
                    OpenScreen();
                });
#endif
            }
            else
                OpenScreen();
#endif
        }

#if !NET_STANDARD
        public override bool ExecuteOnTouchDown()
        {
            return false;
        }

        void OpenScreen()
        {
            var context = new Dictionary<String, Object>();
            context.Add("SynchroPopup", SynchroPopup);
            context.Add("SynchroFrame", SynchroFrame);
            context.Add("X", X);
            context.Add("Y", Y);
            context.Add("AutoCloseSecs", AutoCloseSecs);
            context.Add("IsRelative", IsRelative);
            context.Add("Entity", Entity);
            context.Add("OffsetRelativeToScreen", OffsetRelativeToScreen);

            if (PreserveCurrentParameterFile)
            {
                context.Add("ParameterFile", "*");
            }
            else if (ParameterFile != null)
            {
                var file = ParameterFile.GetPathString();
                if (!String.IsNullOrEmpty(file))
                    context.Add("ParameterFile", file);
            }

            if (RequestedMonitor >= 0)
                context.Add("Monitor", RequestedMonitor);

            var screenManager = Parent?.GetService(typeof(IScreenManager)) as IDocumentManager;
            if (screenManager != null)
            {
                if (screenName == null)
                {
                    //var listDocumentManagers = CommandManagerComponent.uriRisolver.GetListInstalledDocumentManagers();
                    //var screenManager = (from c in listDocumentManagers where c.TypeScheme == "ScreenManager" select c).Single();
                    screenManager.Execute(null, Parent, ExecutionMode, context);
                }
                else
                {
                    //IDocumentManager manager = CommandManagerComponent.uriRisolver.ResolveUri(screenName) as IDocumentManager;
                    screenManager.Execute(screenName, Parent, ExecutionMode, context);
                }
            }
        }
#endif
        #endregion
    }
}
