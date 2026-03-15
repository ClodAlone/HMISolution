using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using ScriptVariableValues;
using CommandManager.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;
using LogicManager.ComponentService;

namespace CommandManager
{
    [DataContract(Name = "RunLogicCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class RunLogicCommand : CommandManager
    {
        #region Properties

        [DataMember]
        Uri logicName;
        public Uri LogicName
        {
            get { return logicName; }
            set
            {
                if (value == logicName)
                    return;
                logicName = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("LogicName");
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
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("ExecutionMode");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        #endregion

        #region Overrides
#if !WINDOWS_UWP && !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
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
                if (LogicName == null)
                    return String.Empty;
                //return System.IO.Path.GetFileNameWithoutExtension(ScriptName.GetPathString());
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LogicName.GetPathString()), System.IO.Path.GetFileNameWithoutExtension(LogicName.GetPathString()));
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.RunLogicName;
            }
        }

#if !WINDOWS_UWP
        public override RemoteExecute RemoteExecute()
        {
            Execute();
            return null;
        }
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
            if (ExecutionMode != ExecutionMode.Shared)
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
        StartupContext startupContext;
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

            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            //IDocumentManager manager = CommandManagerComponent.uriRisolver.ResolveUri(logicName) as IDocumentManager;
            IDocumentManager manager = Parent?.GetService(typeof(ILogicManager)) as IDocumentManager;
            if (manager != null)
            {
#if !NET_STANDARD
                if (startupContext == null)
                    startupContext = new StartupContext();
                startupContext.Context = Entity;
#endif
                manager.Execute(logicName, Parent, ExecutionMode,
#if !NET_STANDARD
                    startupContext
#else
                    null
#endif
                    );
            }
        }
        #endregion
    }
}
