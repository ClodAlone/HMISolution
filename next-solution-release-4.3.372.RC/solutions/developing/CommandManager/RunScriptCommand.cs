using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows.Controls;
using CommandManager.ComponentService;
using ScriptVariableValues;
using ScriptManager.ComponentService;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities;

namespace CommandManager
{
    [DataContract(Name = "RunScriptCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class RunScriptCommand : CommandManager
    {
        #region Properties

        [DataMember]
        Uri scriptName;
        public Uri ScriptName
        {
            get { return scriptName; }
            set
            {
                if (value == scriptName)
                    return;
                scriptName = value;
#if !NET_STANDARD
                OnPropertyChanged("ScriptName");
                OnPropertyChanged("CommandSummary");
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
                if (ScriptName == null)
                    return String.Empty;
                //return System.IO.Path.GetFileNameWithoutExtension(ScriptName.GetPathString());
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ScriptName.GetPathString()), System.IO.Path.GetFileNameWithoutExtension(ScriptName.GetPathString()));
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.RunScriptName;
            }
        }

        public override RemoteExecute RemoteExecute()
        {
            Execute();
            return null;
        }

#if !NET_STANDARD
        StartupContext startupContext;
#endif
        public override void Execute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            //IDocumentManager manager = CommandManagerComponent.uriRisolver.ResolveUri(scriptName) as IDocumentManager;
            IDocumentManager manager = Parent?.GetService(typeof(IScriptManager)) as IDocumentManager;
            if (manager != null)
            {
                if (startupContext == null)
                    startupContext = new StartupContext();
                startupContext.Context = Entity;
                startupContext.Parameter = Parameter;
                manager.Execute(scriptName, Parent, ExecutionMode, startupContext);
            }
#endif
        }
        #endregion
    }
}
