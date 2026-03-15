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
#if !NET_STANDARD
using System.Windows.Controls;
using System.ComponentModel;
#endif
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;
using System.Windows;

namespace CommandManager
{
    [DataContract(Name = "OpenMapCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OpenMapCommand : CommandManager
    {
        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
        }

        #region Properties


        [DataMember]
        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
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

#if !NET_STANDARD        
        [DataMember]
        private Rect zoomTo;
        public Rect ZoomTo
        {
            get { return zoomTo; }
            set
            {
                if (zoomTo == value)
                    return;
                zoomTo = value;
#if !WINDOWS_UWP
                OnPropertyChanged("ZoomTo");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }
#endif

        [DataMember]
        private bool enableZoomingScrolling = true;
        public bool EnableZoomingScrolling
        {
            get { return enableZoomingScrolling; }
            set
            {
                if (enableZoomingScrolling == value)
                    return;
                enableZoomingScrolling = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("EnableZoomingScrolling");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private bool showMiniMap = true;
        public bool ShowMiniMap
        {
            get { return showMiniMap; }
            set
            {
                if (showMiniMap == value)
                    return;
                showMiniMap = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ShowMiniMap");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        private bool showNextButton = true;
        public bool ShowNextButton
        {
            get { return showNextButton; }
            set
            {
                if (showNextButton == value)
                    return;
                showNextButton = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ShowNextButton");
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
            return base.PerformValidation(propertyName);
        }

        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "X" || propertyName == "Y" || propertyName == "IsRelative")
                {
                    return ExecutionMode == ExecutionMode.Synchro;
                }
                else if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.OpenMapName;
            }
        }

#if !WINDOWS_UWP
        public override String CommandSummary
        {
            get
            {
                return String.Empty;
            }
        }

        public override RemoteExecute RemoteExecute()
        {
            return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };
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
#else
                RunOnUIThread.RunIfRequired(() =>
#endif
                {
                    OpenMap();
                });
            }
            else
                OpenMap();
#endif
        }

#if !NET_STANDARD
        public override bool ExecuteOnTouchDown()
        {
            return false;
        }

        void OpenMap()
        {
            var screenManager = Parent?.GetService(typeof(IScreenManager)) as IScreenManager;
            if (screenManager != null)
                screenManager.OpenMap(Parent, ExecutionMode, X, Y, IsRelative, 
                    Entity, ZoomTo, EnableZoomingScrolling, ShowMiniMap, ShowNextButton);
        }
#endif

        public override bool IsUICommand()
        {
            return true;
        }

        #endregion
    }
}
