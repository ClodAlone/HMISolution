using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using CommandManager.ComponentService;
using System.Windows.Threading;
using UIMsgBoxAlertService.ComponentService;
#endif
using DocumentManager.ComponentService;
using UFRecipeEditor.ComponentService;
using System.Runtime.Serialization;
using UFInterfaces;
using ViewModelLib;
using OPCUAViewModel;
using Utilities;
using Utilities.Commands;
using Opc.Ua;
using UFRecipeExecutionContext;
using UFInterfaces.AuditTrace;

namespace CommandManager
{
    [DataContract(Name = "RecipeCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class RecipeCommand : CommandManager, ICheckUserCallable
    {
        #region Properties

        [DataMember]
        Uri recipeName;
        public Uri RecipeName
        {
            get { return recipeName; }
            set
            {
                if (value == recipeName)
                    return;
                recipeName = value;
#if !NET_STANDARD
                OnPropertyChanged("RecipeName");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        UFRecipeExecutionContext.RecipeCommandType commandType;
        public UFRecipeExecutionContext.RecipeCommandType CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                if (commandType == value)
                    return;
                commandType = value;
#if !NET_STANDARD
                OnPropertyChanged("CommandType");
                OnPropertyVisiblityChanged("CommandType");
#endif
            }
        }

        [DataMember]
        bool synchronous;
        public bool Synchronous
        {
            get { return synchronous; }
            set
            {
                if (synchronous == value)
                    return;
                synchronous = value;
#if !NET_STANDARD
                OnPropertyChanged("Synchronous");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        int syncTimeout = 10000;
        public int SyncTimeout
        {
            get
            {
                return syncTimeout;
            }
            set
            {
                if (syncTimeout == value)
                    return;
                syncTimeout = value;
#if !NET_STANDARD
                OnPropertyChanged("SyncTimeout");
#endif
            }
        }

        #endregion

        #region Overrides
        IAuditTrace auditTrace;
        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            if (base.Init(entity, parent, sessionname))
            {
                IDocumentManager manager = Parent?.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
                if (manager != null && recipeName != null)
                {
                    manager.Execute(recipeName, Parent, DocumentManager.ComponentService.ExecutionMode.Shared, sessionname);
                    auditTrace = ((IRecipeEditorManager)manager).GetAuditTraceInterface(recipeName, Parent);
                    if (auditTrace != null && !auditTrace.IsAuditPropertiesFetched)
                        auditTrace.AuditPropertiesFetched += AuditTrace_AuditPropertiesFetched;
                    else
                        Utilities.Commands.CheckUserCallable.GetInstance().Add(this);
                    return true;
                }
            }

            return false;
        }

        void AuditTrace_AuditPropertiesFetched(object sender, EventArgs e)
        {
            auditTrace.AuditPropertiesFetched -= AuditTrace_AuditPropertiesFetched;
            Utilities.Commands.CheckUserCallable.GetInstance().Add(this);
        }
#if !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return false;
                }
                else if (propertyName == "SyncTimeout")
                {
                    return CommandType == UFRecipeExecutionContext.RecipeCommandType.Read ||
                        CommandType == UFRecipeExecutionContext.RecipeCommandType.Activate;
                }

                return base[propertyName];
            }
        }

        public override bool ExecuteOnTouchDown()
        {
            return false;
        }
#endif
        public override String CommandSummary
        {
            get
            {
                if (RecipeName == null)
                    return String.Empty;
                //                return System.IO.Path.GetFileNameWithoutExtension(RecipeName.GetPathString());
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(RecipeName.GetPathString()), System.IO.Path.GetFileNameWithoutExtension(RecipeName.GetPathString()));
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.RecipeCommandName;
            }
        }

        #region ICheckUserCallable
        public void CheckUserCallable()
        {
            try
            {
                IRecipeEditorManager manager = Parent?.GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                if (manager != null)
                {
                    var bCan = manager.IsReady(recipeName, (int)commandType, Parent);
                    if (bCan != bCanExecute)
                    {
                        bCanExecute = bCan;
#if !WINDOWS_UWP && !NET_STANDARD
                        if (Control != null)
                        {
                            Control.Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                            });
                        }
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (bCanExecute)
                {
                    bCanExecute = false;
#if !WINDOWS_UWP && !NET_STANDARD
                    if (Control != null)
                    {
                        Control.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        });
                    }
#endif
                }
                else
                    bCanExecute = false;
            }
        }
        #endregion

        public override RemoteExecute RemoteExecute()
        {
            if (CommandType == RecipeCommandType.Show ||
                CommandType == RecipeCommandType.Export ||
                CommandType == RecipeCommandType.Import)
            {
                return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };
            }

            Execute();
            return null;
        }

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
            if (CommandType != RecipeCommandType.Show)
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

            string filepath = null;
            if (CommandType == RecipeCommandType.Export)
            {
#if !NET_STANDARD
                filepath = GetExportFilePath();
#endif
                if (filepath == null)
                    return;
            }
            else if (CommandType == RecipeCommandType.Import)
            {
#if !NET_STANDARD
                filepath = GetImportFilePath();
#endif
                if (filepath == null)
                    return;
            }

            IDocumentManager manager = Parent?.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
            // IDocumentManager manager = CommandManagerComponent.uriRisolver.ResolveUri(recipeName) as IDocumentManager;
            if (manager != null)
                manager.Execute(recipeName, Parent, DocumentManager.ComponentService.ExecutionMode.Normal,
                                new UFRecipeExecutionContext.RecipeExecutionContext()
                                {
                                    CommandType = CommandType,
                                    Timeout = SyncTimeout,
                                    FilePathName = filepath,
                                    IsSynchro = Synchronous
#if !NET_STANDARD
                                    , Control = Control
#endif
                                });
        }

        bool bCanExecute;
        public override bool CanExecute()
        {
            if (!base.CanExecute())
                return false;

            return bCanExecute;
        }

        public override void Terminate()
        {
            if (auditTrace != null)
                auditTrace.AuditPropertiesFetched -= AuditTrace_AuditPropertiesFetched;
            Utilities.Commands.CheckUserCallable.GetInstance().Remove(this);

            if (Parent != null && recipeName != null)
            {
                IDocumentManager manager = Parent.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
                if (manager != null)
                    manager.Terminate(recipeName, Parent);
            }

            base.Terminate();
        }
#endregion

#region Private Methods
#if !NET_STANDARD
        string GetExportFilePath()
        {
            Ookii.Dialogs.Wpf.VistaSaveFileDialog dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
            dialog.Title = Properties.Resources.LayoutImportRecipeCommandTitle;
            dialog.OverwritePrompt = false;
            dialog.ValidateNames = true;
            dialog.AddExtension = true;
            dialog.DefaultExt = "csv";
            dialog.Filter = Properties.Resources.CSVFilter;
            if (dialog.ShowDialog() != true)
                return null;

            string file = dialog.FileName;
            if (String.IsNullOrEmpty(file))
                return null;

            if (System.IO.File.Exists(file))
            {
                bool searchForNewName = false;
                IUIMsgBoxAlertService ui = null;
                if (Parent != null)
                    ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (ui != null)
                {
                    var ret = ui.ShowYesNoCancel(Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);
                    if (ret == CustomDialogResults.Cancel)
                        return null;
                    else if (ret == CustomDialogResults.No)
                        searchForNewName = true;
                }
                else
                {
                    var ret = System.Windows.MessageBox.Show(Properties.Resources.ExportFileWarning, Properties.Resources.LayoutImportRecipeCommandTitle, System.Windows.MessageBoxButton.YesNoCancel);
                    if (ret == System.Windows.MessageBoxResult.Cancel)
                        return null;
                    else if (ret == System.Windows.MessageBoxResult.No)
                        searchForNewName = true;
                }

                if (searchForNewName)
                {
                    int i = 1;
                    while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()))))
                    {
                        i++;
                    }
                    file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()));
                }
            }

            return file;
        }

        string GetImportFilePath()
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.CheckFileExists = false;
            dialog.Title = Properties.Resources.LayoutImportRecipeCommandTitle;
            dialog.ValidateNames = true;
            dialog.Filter = Properties.Resources.CSVFilter;
            if (dialog.ShowDialog() != true)
                return null;

            string file = dialog.FileName;
            if (String.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                IUIMsgBoxAlertService ui = null;
                if (Parent != null)
                    ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (ui != null)
                {
                    ui.ShowWarning(Properties.Resources.ImportFileWarning);
                }
                else
                {
                    System.Windows.MessageBox.Show(Properties.Resources.ImportFileWarning, Properties.Resources.LayoutImportRecipeCommandTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }

                return null;
            }

            return file;
        }
#endif
#endregion
    }
}
