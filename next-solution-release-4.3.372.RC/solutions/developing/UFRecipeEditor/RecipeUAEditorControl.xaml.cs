using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Core;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;
using UIMsgBoxAlertService.ComponentService;
using log4net;
using UFRecipeSettings.UFRecipeModel;
using UFRecipeEditor.Controls;

namespace UFRecipeEditor
{
    /// <summary>
    /// Interaction logic for RecipeUAEditorControl.xaml
    /// </summary>
    public partial class RecipeUAEditorControl : UserControl, IEditableObject, IDisposable
    {

        #region Declarations
        static readonly ILog log = LogManager.GetLogger(Utilities.Properties.Resources.RecipeService);

        RedundancySettings redundancySettings;
        #endregion

        public RecipeUAEditorControl(RecipeUAServerDocument doc)
        {
            InitializeComponent();
            Document = doc;

            //Document.CreateUndoRedoHelper(this);

            var listBaseAddresses = Document.GetConfiguration().BaseAddresses;
            TransportGrid.ItemsSource = listBaseAddresses;

            generalSettings.DataContext = Document.GetConfiguration();

            tabControlExt.SelectionChanging += (o, e) =>
            {
                if (bDisposed || Document == null)
                    return;

                EndEdit();
            };

            tabControlExt.SelectionChanged += (o, e) =>
            {
                if (e.NewSelectedItem == tabGeneralSettings)
                {
                    RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
                }
                else if (e.NewSelectedItem == tabRedundancy)
                {
                    if (redundancySettings == null)
                    {
                        using (new WaitCursor())
                        {
                            redundancySettings = new RedundancySettings(Document);
                            redundancySettings.ClearValue(FrameworkElement.WidthProperty);
                            redundancySettings.ClearValue(FrameworkElement.HeightProperty);
                            redundancyGrid.Children.Add(redundancySettings);
                        }
                    }
                    redundancySettings.OnActivate();
                }
            };
        }

        internal void OnActivate()
        {
            if (tabRedundancy.IsSelected)
                redundancySettings.OnActivate();
            else
                RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
        }

        #region IEditableObject Members
        bool bEditingUow;
        public void BeginEdit()
        {
            if (!bEditingUow && Document.UowContext != null)
            {
                bEditingUow = true;
                Document.UowContext.BeforeFlushChanges += uowContext_BeforeFlushChanges;
            }
        }

        public void CancelEdit()
        {
            if (bEditingUow && Document.UowContext != null)
            {
                bEditingUow = false;
                Document.UowContext.RollbackTransaction();
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
            }
        }

        public void EndEdit()
        {
            bool bResult = true;
            if (bEditingUow && Document.UowContext != null)
            {
                bEditingUow = false;
                bResult = Document.UowContext.TryCommitChanges(log);
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
            }

            if (!bResult)
            {
                var uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(Properties.Resources.ErrorOnApplyingChanges);
            }
        }

        private void uowContext_BeforeFlushChanges(object sender, DevExpress.Xpo.SessionManipulationEventArgs e)
        {
            UserControl currentTabEditor = null;
            if (currentTabEditor != null)
            {
                //var objects = Document.UowContext.GetObjectsToSave().OfType<DevExpress.Xpo.IXPSimpleObject>().ToList();
                var objects = RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObjects;
                if (objects == null && RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject != null)
                    objects = new List<object>() { RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject };
                if (objects != null)
                {
                    var list = new List<DevExpress.Xpo.IXPSimpleObject>();
                    foreach (var obj in objects)
                    {
                        var parent = Document.UowContext.GetParentObject(obj);
                        if (parent != null)
                            list.Add(parent as DevExpress.Xpo.IXPSimpleObject);
                    }

                    if (list.Count > 0)
                        Document.AddUndoAction(currentTabEditor, list, Utilities.Xpo.UndoRedo.UndoRedoAction.Changed);
                }
            }
        }
        #endregion

        #region Properties

        RecipeUAServerDocument _Document;
        [Browsable(false)]
        public RecipeUAServerDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        #endregion

        #region Commands

        private void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Document.SaveToFile();
        }

        private void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.NeedsSave;
        }

        private void OnRemoveItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabRedundancy.IsSelected)
                    redundancySettings.DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabRedundancy.IsSelected && redundancySettings.IsAnyItemSelected();
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }
        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var ownerTypeName = Document.GetNextUndoOwner();
                if (ownerTypeName == typeof(RedundancySettings).Name)
                {
                    tabRedundancy.IsSelected = true;
                    redundancySettings.UndoAction();
                }
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.UndoContainsSomething(); ;
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                var ownerTypeName = Document.GetNextRedoOwner();
                if (ownerTypeName == typeof(RedundancySettings).Name)
                {
                    tabRedundancy.IsSelected = true;
                    redundancySettings.RedoAction();
                }
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document.RedoContainsSomething(); ;
        }

        private async void OnStartServer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            try
            {
                if (Document.StartServer(txtevent, Scroll, bSave: true, manually: true))
                {
                    bool running = false;
#if !DEBUG
                    while (!running)
#endif
                    {
                        RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = true;
                        running = await Document.ServerCMSHelperAsync.WaitServerStarted(10000);
                        RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG

                        if (running ||
                            RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface == null ||
                            RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStartServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface != null)
                {
                    RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                        RecipeUAServerInfo.RecipeUAServerInfo.GetServerName(), ex.Message));
                }
            }
            finally
            {
                RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void CanStartServer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Document.ServerCMSHelperAsync.IsServerRunning;
        }

        private async void OnStopServer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            try
            {
                if (Document.ServerCMSHelperAsync.StopServer())
                {
                    bool stopped = false;
#if !DEBUG
                    while (!stopped)
#endif
                    {
                        RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = true;
                        stopped = await Document.ServerCMSHelperAsync.WaitServerStopped(10000);
                        RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG
                        if (stopped ||
                            RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface == null ||
                            RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStopServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface != null)
                {
                    RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StopServerFailed.Replace("'newline'", Environment.NewLine),
                        RecipeUAServerInfo.RecipeUAServerInfo.GetServerName(), ex.Message));
                }
            }
            finally
            {
                RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void CanStopServer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = /*Document.ServerCMSHelperAsync.IsServerRunning || */Document.ServerCMSHelperAsync.IsServerStarted;
        }

        private void OnCertificateChecker(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabGeneralSettings.IsSelected = true;
            Document.CertificateChecker();
        }

        private void CanCertificateChecker(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnServiceManager(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                Document.ServiceManager();
            }
            catch (Exception ex)
            {
                if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface != null)
                {
                    RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ServiceManagerStartFailed.Replace("'newline'", Environment.NewLine), ex.Message));
                }
            }
        }

        private void CanServiceManager(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (RecipeEditorManagerComponent.recipeEditorManagerComponent.PropertyControl != null)
                RecipeEditorManagerComponent.recipeEditorManagerComponent.PropertyControl.Activate();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject = null;

            foreach (DXTabItem item in tabControlExt.Items)
            {
                if (item.Content is FrameworkElement)
                {
                    var fe = item.Content as FrameworkElement;
                    var list = (from p in fe.GetChildrenOfType<FrameworkElement>()
                                where p is IDisposable
                                select p as IDisposable).ToList();

                    list.ForEach((o) => o.Dispose());
                }

            }
            tabControlExt.Items.Clear();
            tabControlExt.Dispose();
        }
        #endregion

        private void TransportAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var bas = RecipeUAServerInfo.RecipeUAServerInfo.GetCurrentApplicationBaseAddresses();
            if (bas.Count == 0)
                return;
            var ba = Document.AddNewBaseAddress(Opc.Ua.Utils.ParseUri(bas.FirstOrDefault()).Scheme);
            var n = new UFUACommonControls.NewTransport(bas)
            {
                DataContext = ba
            };

            GeneralDialogContent newBaseAddressDialog = new GeneralDialogContent(n)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "AddTransport"
            };
            if (newBaseAddressDialog.ShowDialog() == true)
            {
                //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                TransportGrid.ItemsSource = null;
                TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                TransportGrid.SelectedItem = ba;
            }
            else
                ba.Delete();

            TransportGrid.ItemsSource = null;
            TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
        }

        private void TransportEdit_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (TransportGrid.SelectedItem != null)
            {
                RecipeUABaseAddress ba = null;

                if (TransportGrid.SelectedItem is RecipeUABaseAddress)
                {
                    var oba = TransportGrid.SelectedItem as RecipeUABaseAddress;
                    ba = Document.AddNewBaseAddress(Opc.Ua.Utils.UriSchemeNetPipe);
                    ba.Enabled = oba.Enabled;
                    ba.Port = oba.Port;
                    ba.Server = oba.Server;
                    ba.Transport = oba.Transport;
                    oba.Delete();
                }
                if (TransportGrid.SelectedItem is RecipeUABaseAddress)
                {
                    ba = TransportGrid.SelectedItem as RecipeUABaseAddress;
                }
                if(ba != null)
                {
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var bas = RecipeUAServerInfo.RecipeUAServerInfo.GetCurrentApplicationBaseAddresses();
                        var n = new UFUACommonControls.NewTransport(bas)
                        {
                            DataContext = uow.GetNestedObject(ba)
                        };
                        GeneralDialogContent Dialog = new GeneralDialogContent(n)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "EditTransport"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            //Document.AddUndoAction(this, si, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                            uow.CommitChanges();
                            RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
                            TransportGrid.ItemsSource = null;
                            TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                        }
                    }
                }
            }
        }

        private void TransportDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TransportGrid.SelectedItem != null)
            {
                var si = TransportGrid.SelectedItem as RecipeUABaseAddress;
                if (si != null)
                {
                    if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface == null ||
                        RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowYesNo(string.Format(Properties.Resources.AskDeleteEndpoint, si.Path), CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                    {
                        Document.GetConfiguration().BaseAddresses.Remove(si);
                        si.Delete();

                        //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);
                        TransportGrid.ItemsSource = null;
                        TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                    }
                }
            }
        }
    }
}
