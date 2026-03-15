using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using CommandManager.ComponentService;
using ScreenManager.ComponentService;
using ScriptVariableValues;
using Utilities.WPF;
using System.Windows.Threading;
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
    [DataContract(Name = "Call3DCameraView", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class Call3DCameraView : CommandManager
    {
        #region Properties

        [DataMember]
        String call3DControl;
        public String Call3DControl
        {
            get { return call3DControl; }
            set
            {
                if (value == call3DControl)
                    return;
                call3DControl = value;
#if !NET_STANDARD
                OnPropertyChanged("Call3DControl");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        String viewName;
        public String ViewName
        {
            get { return viewName; }
            set
            {
                if (value == viewName)
                    return;
                viewName = value;
#if !NET_STANDARD
                OnPropertyChanged("ViewName");
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
                if (propertyName == "OpcuaEntityReference" ||
                    propertyName == "Expression")
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
                return ViewName;
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.Call3DCameraViewName;
            }
        }

        public override RemoteExecute RemoteExecute()
        {
            Execute();
            return null;
        }

        public override void BlindExecute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            ExecuteOnUserInterface();
#endif
        }

        public override void Execute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            var context = new Dictionary<String, Object>();
            context.Add("Entity", Entity);
            context.Add("Call3DControl", Call3DControl);
            context.Add("ViewName", ViewName);
            var screenManager = Parent?.GetService(typeof(IScreenManager)) as IDocumentManager;
            if (screenManager != null)
                screenManager.Execute(null, Parent, ExecutionMode.Normal, context);
#endif
        }
#endregion
    }
}
