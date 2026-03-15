using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using CommandManager.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using Utilities;

namespace CommandManager
{
    public class ChangeCultureRemoteExecute : RemoteExecute
    {

    }

    [DataContract(Name = "ChangeCultureCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class ChangeCultureCommand : CommandManager
    {
        #region Properties

        [DataMember]
        String cultureName;
        public String CultureName
        {
            get { return cultureName; }
#if !WINDOWS_UWP
            set
            {
                if (value == cultureName)
                    return;
                cultureName = value;
#if !NET_STANDARD
                OnPropertyChanged("CultureName");
                OnPropertyChanged("CommandSummary");
#endif
            }
#endif
        }

        [DataMember]
        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
#if !WINDOWS_UWP
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
#endif
        }

        #endregion

        #region Overrides
#if !WINDOWS_UWP && !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || propertyName == "ExecutionMode" || 
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
                return CultureName;
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.ChangeCultureName;
            }
        }

#if !WINDOWS_UWP
        public override RemoteExecute RemoteExecute()
        {
            return new ChangeCultureRemoteExecute() { executionMode = ExecutionMode, uri = new Uri(CultureName, UriKind.RelativeOrAbsolute), Parent = Parent };
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
            if (!String.IsNullOrEmpty(CultureName))
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

        public override bool IsUICommand()
        {
            return true;
        }

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

            var stringManager = Parent?.GetService(typeof(IStringEditorManager)) as IDocumentManager;
            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            //var listDocumentManagers = CommandManagerComponent.uriRisolver.GetListInstalledDocumentManagers();
            //var stringManager = (from c in listDocumentManagers where c.TypeScheme == "StringManager" select c).Single();
            if (stringManager != null)
                stringManager.Execute(String.IsNullOrEmpty(CultureName) ? null : new Uri(CultureName, UriKind.RelativeOrAbsolute), Parent, ExecutionMode, Entity);
        }
#endregion
    }
}