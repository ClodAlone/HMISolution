using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows.Controls;
using CommandManager.ComponentService;
using ScriptVariableValues;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using WPFUtilities;

namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum SystemCommandType
    {
        Quit,
        Logoff,
        Lock,
        Shutdown,
        Restart,
        RunApp
    }


    [DataContract(Name = "SystemCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class SystemCommand : CommandManager
    {
        #region Properties

        [DataMember]
        SystemCommandType commandType;
        public SystemCommandType CommandType
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

        [DataMember]
        String parameter;
        public String Parameter
        {
            get { return parameter; }
            set
            {
                if (value == parameter)
                    return;
                parameter = value;
#if !NET_STANDARD
                OnPropertyChanged("Parameter");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

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
#if !NET_STANDARD
                OnPropertyChanged("ExecutionMode");
                OnPropertyChanged("CommandSummary");
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
                if (propertyName == "Parameter")
                {
                    return CommandType == SystemCommandType.Restart ||
                        CommandType == SystemCommandType.RunApp ||
                        CommandType == SystemCommandType.Shutdown;
                }
                else if (propertyName == "ExecutionMode")
                {
                    return CommandType == SystemCommandType.RunApp;
                }
                else if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return false;
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
                return Properties.Resources.SystemCommandName;
            }
        }

#if !NET_STANDARD
        public override bool ExecuteOnTouchDown()
        {
            return false;
        }
#endif

        public override RemoteExecute RemoteExecute()
        {
            if (CommandType == SystemCommandType.Lock ||
                CommandType == SystemCommandType.Logoff ||
                CommandType == SystemCommandType.Quit ||
                CommandType == SystemCommandType.Restart ||
                CommandType == SystemCommandType.Shutdown)
                return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };

            Execute();
            return null;
        }

#if !NET_STANDARD
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        [DllImport("user32")]
        public static extern void LockWorkStation();
#endif

        public override void BlindExecute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif                
                )
                return;

#if !NET_STANDARD
            if (CommandType != SystemCommandType.Quit)
#endif
            {
                Execute();
            }
#if !NET_STANDARD
            else
            {
                ExecuteOnUserInterface();
            }
#endif
        }

#if !NET_STANDARD
        DispatcherTimer delay;
#endif
        public override void Execute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif                
                )
                return;

            switch (CommandType)
            {
                case SystemCommandType.Quit:
#if !NET_STANDARD
                    if (Control != null)
                    {
                        foreach (TouchDevice device in Control.TouchesOver)
                        {
                            if (device.IsActive)
                            {
                                if (delay == null)
                                {
                                    delay = new DispatcherTimer();
                                    delay.Interval = TimeSpan.FromMilliseconds(250);
                                    delay.Tick += (o, e) => 
                                    {
                                        executeDelayCommand = true;
                                        try
                                        {
                                            Execute();
                                        }
                                        finally
                                        {
                                            executeDelayCommand = false;
                                        }
                                    };
                                    delay.Start();
                                }
                                return;
                            }
                        }
                    }

                    try
                    {
                        WPFTabletSupport.DisableWPFTabletSupport();
                    }
                    catch { }

                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        using (var cursor = new WaitCursor())
                        {
                            Dispatcher.CurrentDispatcher.InvokeShutdown();
                        }
                    });
#endif
                    break;
                case SystemCommandType.Shutdown:
                    Process.Start("shutdown", String.Format("/s /t {0}", GetTimeout())); // the argument /s is to shut down the computer
                    break;
                case SystemCommandType.RunApp:
                    if (!String.IsNullOrEmpty(Parameter))
                    {
                        using (var cursor = new WaitCursor())
                        {
                            var path = String.Empty;
                            var parameter = Parameter;
                            if (parameter.StartsWith("\""))
                            {
                                path = parameter.Substring(1);
                                var index = path.IndexOf('\"');
                                if (index > 0)
                                {
                                    parameter = path.Substring(index + 1, path.Length - index - 1);
                                    if (parameter.StartsWith(" "))
                                        parameter = parameter.Substring(1, parameter.Length - 1);
                                    path = path.Substring(0, index);
                                }
                            }
                            else
                            {
                                path = parameter;
                                var index = path.IndexOf(' ');
                                if (index > 0)
                                {
                                    parameter = path.Substring(index + 1, path.Length - index - 1);
                                    path = path.Substring(0, index);
                                }
                                else
                                    parameter = String.Empty;
                            }

                            if (ExecutionMode == DocumentManager.ComponentService.ExecutionMode.Stop)
                            {
                                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                                foreach (var process in Process.GetProcessesByName(name))
                                {
                                    process.CloseMainWindow();
                                }
                            }
                            else
                            {
                                var process = Process.Start(path, parameter.Length > 0 ? parameter : null);
                                if (ExecutionMode == DocumentManager.ComponentService.ExecutionMode.Synchro)
                                {
                                    process.WaitForExit();
                                }
                                process.Close();
                            }
                        }
                    }
                    break;
                case SystemCommandType.Logoff:
#if !NET_STANDARD
                    ExitWindowsEx(0, 0);
#endif
                    break;
                case SystemCommandType.Lock:
#if !NET_STANDARD
                    LockWorkStation();
#endif
                    break;
                case SystemCommandType.Restart:
                    Process.Start("shutdown", String.Format("/r /t {0}", GetTimeout())); // the argument /r is to restart the computer
                    break;
            }
        }

        int GetTimeout()
        {
            int timeout = 0;
            if (!String.IsNullOrEmpty(Parameter))
            {
                try
                {
                    timeout = Convert.ToInt32(Parameter);
                }
                catch (Exception ex)
                {

                }
            }

            return timeout;
        }
        #endregion
    }
}
