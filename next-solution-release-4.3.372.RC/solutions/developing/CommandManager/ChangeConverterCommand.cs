using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using CommandManager.ComponentService;
using UnitConverterManager.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
#endif
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using System.ComponentModel;
using Utilities;

namespace CommandManager
{
    public class ChangeConverterRemoteExecute : RemoteExecute
    {

    }

    [DataContract(Name = "ChangeConverterCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class ChangeConverterCommand : CommandManager
    {
        #region Properties

        [DataMember]
        String converterName;
        public String ConverterName
        {
            get { return converterName; }
#if !WINDOWS_UWP
            set
            {
                if (value == converterName)
                    return;
                converterName = value;
#if !NET_STANDARD
                OnPropertyChanged("ConverterName");
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

        #region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "ExecutionMode")
            {
                if (ExecutionMode == ExecutionMode.Synchro || ExecutionMode == ExecutionMode.Shared)
                    return Properties.Resources.InvalidOption;
            }

            return base.PerformValidation(propertyName);
        }
#endif
        #endregion

        #region Overrides
#if !WINDOWS_UWP && !NET_STANDARD
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
                return ConverterName;
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.ChangeConverterName;
            }
        }

#if !WINDOWS_UWP
        public override RemoteExecute RemoteExecute()
        {
            return new ChangeConverterRemoteExecute() { executionMode = ExecutionMode, uri = new Uri(ConverterName, UriKind.RelativeOrAbsolute), Parent = Parent };
        }
#endif

        public override void BlindExecute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            if (!String.IsNullOrEmpty(ConverterName))
            {
                Execute();
            }
            else
            {
                ExecuteOnUserInterface();
            }
#endif
        }

        public override void Execute()
        {
#if !NET_STANDARD
            if (IsAccessDenied() || !CanExecute() || !CanExecuteDelayCommand())
                return;

            var unitConverterManager = Parent?.GetService(typeof(IUnitConverterEditorManager)) as IDocumentManager;
            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            //var listDocumentManagers = CommandManagerComponent.uriRisolver.GetListInstalledDocumentManagers();
            //var stringManager = (from c in listDocumentManagers where c.TypeScheme == "StringManager" select c).Single();
            if (unitConverterManager != null)
                unitConverterManager.Execute(String.IsNullOrEmpty(ConverterName) ? null : new Uri(ConverterName, UriKind.RelativeOrAbsolute), Parent, ExecutionMode, Entity);
#endif
        }
        #endregion
    }
}
