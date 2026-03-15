using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ADEditor.Document;
using ADEditor.Controls;
using Utilities;
using Utilities.WPF;
using ADModel;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.Core;

namespace ADEditor
{

    public class pluginDesc
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String AssemblyName { get; set; }
        public Opc.Ua.NodeId NodeId { get; set; }
    }

    /// <summary>
    /// Interaction logic for ADEditorControl.xaml
    /// </summary>
    /// 
    public partial class ADEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        PluginList pluginList;
        NotificationList NotificationList;
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public ADEditorControl(ADEditorDocument doc)
        {
            InitializeComponent();
            Document = doc;
            generalSettings.Document = doc;

            Loaded += (o, e) =>
            {
                bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            //Document.CreateUndoRedoHelper(this);

            var listBaseAddresses = Document.GetConfiguration().BaseAddresses;
            TransportGrid.ItemsSource = listBaseAddresses;

            generalSettings.DataContext = Document.GetGeneralSettings();

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
                    Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
                }
                else if (e.NewSelectedItem == tabPluginList)
                {
                    if (pluginList == null)
                    {
                        using (new WaitCursor())
                        {
                            pluginList = new PluginList(Document);
                            inPluginGrid.Children.Add(pluginList);
                        }
                    }
                    pluginList.OnActivate();
                }
                else if (e.NewSelectedItem == tabNotifications)
                {
                    if (NotificationList == null)
                    {
                        using (new WaitCursor())
                        {
                            NotificationList = new NotificationList(Document, this);
                            inNotificationGrid.Children.Add(NotificationList);
                        }
                    }
                    NotificationList.OnActivate();
                }
            };
        }

        internal void OnActivate()
        {
            if (tabPluginList.IsSelected)
                pluginList.OnActivate();
            else if (tabNotifications.IsSelected)
                NotificationList.OnActivate();
            else
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
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
                bResult = Document.UowContext.TryCommitChanges(ADEditorDocument.logGeneral);
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
            if (tabPluginList.IsSelected)
                currentTabEditor = pluginList;
            else if (tabNotifications.IsSelected)
                currentTabEditor = NotificationList;

            if (currentTabEditor != null)
            {
                //var objects = Document.UowContext.GetObjectsToSave().OfType<DevExpress.Xpo.IXPSimpleObject>().ToList();
                var objects = Document.EditorManagerComponent.Workspace.ContextObjects;
                if (objects == null && Document.EditorManagerComponent.Workspace.ContextObject != null)
                    objects = new List<object>() { Document.EditorManagerComponent.Workspace.ContextObject };
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
                        Document.AddUndoAction(currentTabEditor, list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                }
            }
        }

        #endregion IEditableObject Members

        #region Properties

        ADEditorDocument _Document;
        [Browsable(false)]
        public ADEditorDocument Document
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
            var ret = Document.SaveToFile();
            if (!ret)
            {
                Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocWithoutClosure,
                    String.Format("{0} ({1})", Document.EditorManagerComponent.TypeTitle, Document.Parent.Title)));
            }
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
                if (tabPluginList.IsSelected)
                {
                    pluginList.DeleteSelectedItems();
                }
                else if (tabNotifications.IsSelected)
                {
                    NotificationList.DeleteSelectedItems();
                }
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((tabPluginList.IsSelected && pluginList.IsAnyItemSelected())
                || (tabNotifications.IsSelected && NotificationList.IsAnyItemSelected()));
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabPluginList.IsSelected)
                {
                    pluginList.CopySelectedToClipboard();
                    pluginList.DeleteSelectedItems();
                }
                else if (tabNotifications.IsSelected)
                {
                    NotificationList.CopySelectedToClipboard();
                    NotificationList.DeleteSelectedItems();
                }
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((tabPluginList.IsSelected && pluginList.IsAnyItemSelected())
                || (tabNotifications.IsSelected && NotificationList.IsAnyItemSelected()));
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabPluginList.IsSelected)
                    pluginList.CopySelectedToClipboard();
                else if (tabNotifications.IsSelected)
                    NotificationList.CopySelectedToClipboard();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabPluginList.IsSelected)
                e.CanExecute = pluginList.IsAnyItemSelected();
            else if (tabNotifications.IsSelected)
                e.CanExecute = NotificationList.IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);

                if (tabNotifications.IsSelected)
                    NotificationList.PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            Document.CopyWinClipboardToInMemoryData();
            if (tabNotifications.IsSelected)
                e.CanExecute = Document.ClipboardContainsNotifications() || Document.ClipboardContainsFolders();
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabPluginList.IsSelected)
                    pluginList.UndoAction();
                if (tabNotifications.IsSelected)
                    NotificationList.UndoAction();
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabPluginList.IsSelected)
                e.CanExecute = pluginList.IsAnyUndoActionAvailable();
            if (tabNotifications.IsSelected)
                e.CanExecute = NotificationList.IsAnyUndoActionAvailable();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabPluginList.IsSelected)
                    pluginList.RedoAction();
                if (tabNotifications.IsSelected)
                    NotificationList.RedoAction();
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabPluginList.IsSelected)
                e.CanExecute = pluginList.IsAnyRedoActionAvailable();
            if (tabNotifications.IsSelected)
                e.CanExecute = NotificationList.IsAnyRedoActionAvailable();
        }

        private void OnAddNewPlugin(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            AvailablePlugins selectplugin = new AvailablePlugins();

            GeneralDialogContent addPluginDialog = new GeneralDialogContent(selectplugin) 
            { 
                Owner = this.FindParent<Window>(),
                HelpLink = "AvailablePlugins"
            };
            if (addPluginDialog.ShowDialog() == true)
            {
                var plugindesc = selectplugin.GetSelectedPluginInfo();
                if(plugindesc != null)
                {
                    var list = (from plug in Document.GetGeneralSettings().ADPlugins where plug.AssemblyName == plugindesc.AssemblyName select plug).ToList();
                    if (list.Count == 0)
                    {
                        var plugin = Document.AddNewPlugin();
                        plugin.NodeId = Guid.NewGuid();
                        plugin.Name = plugindesc.Name;
                        plugin.AssemblyName = plugindesc.AssemblyName;
                        plugin.Description = plugindesc.Description;

                        Document.GetGeneralSettings().ADPlugins.Add(plugin);//restore row, commentata per case 9139

                        Document.AddUndoAction(pluginList, plugin, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                        pluginList.FlatGridRefresh();
                    }
                    else
                        MessageBox.Show(Properties.Resources.PluginAlreadyPresent, Properties.Resources.AddPlugin);
                }
                
                e.Handled = true;
            }
            
        }

        private void CanAddNewPlugin(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabPluginList);
        }

        private void OnRunTestPlugin(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabControlExt.SelectedItem = tabPluginList;
            pluginList.RunTest();
        }

        private void CanRunTestPlugin(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabPluginList && pluginList.IsAnyItemSelected());
        }

        private void OnEditPlugin(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            tabControlExt.SelectedItem = tabPluginList;
            pluginList.EditSelectedItem();
        }

        private void CanEditPlugin(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabPluginList && pluginList.IsAnyItemSelected());
        }

        private void OnAddNotification(object sender, ExecutedRoutedEventArgs e)
        {
			e.Handled = true;
            tabControlExt.SelectedItem = tabNotifications;
            var q = NotificationList.treeListControl.GetSelectedNodes().FirstOrDefault();
            ADFolder f = NotificationList.GetRootItem() as ADFolder;
            if (q != null && q.Tag != null)
                f = q.Tag as ADFolder;
            
            ADModel.ADNotification newnot = Document.AddNewNotification(f);
            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                NotificationList.AddNotification(newnot);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newnotification = new NewNotification(Document) { DataContext = newnot };
                GeneralDialogContent Dialog = new GeneralDialogContent(newnotification)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNotification"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (tabNotifications.IsSelected)
                    {
                        NotificationList.AddNotification(newnot);
                        /*Document.AddUndoAction(NotificationList, newnot, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                        NotificationList.AddTreeItem(newnot, f != null ? q : null).IsSelected = true;*/
                    }
                }
                else
                {
                    newnot.Delete();
                }
            }
        }
        private void CanAddNotification(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabNotifications);
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
                        Document.EditorManagerComponent.Workspace.IsBusy = true;
                        running = await Document.ServerCMSHelperAsync.WaitServerStarted(10000);
                        Document.EditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG

                        if (running || 
                            Document.EditorManagerComponent.UIInterface == null ||
                            Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStartServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerFailed.Replace("'newline'", Environment.NewLine),
                        ADServerInfo.ADServerInfo.GetServerName(), ex.Message));
                }
            }
            finally
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
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
                        Document.EditorManagerComponent.Workspace.IsBusy = true;
                        stopped = await Document.ServerCMSHelperAsync.WaitServerStopped(10000);
                        Document.EditorManagerComponent.Workspace.IsBusy = false;
#if !DEBUG
                        if (stopped ||
                            Document.EditorManagerComponent.UIInterface == null ||
                            Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.WaitingStopServer, CustomDialogIcons.Question) == CustomDialogResults.No)
                            break;
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StopServerFailed.Replace("'newline'", Environment.NewLine),
                        ADServerInfo.ADServerInfo.GetServerName(), ex.Message));
                }
            }
            finally
            {
                Document.EditorManagerComponent.Workspace.IsBusy = false;
            }
        }

        private void CanStopServer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = /*Document.ServerCMSHelperAsync.IsServerRunning || */Document.ServerCMSHelperAsync.IsServerStarted;
        }

        private void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ADModel.ADFolder folder = Document.AddNewFolder(NotificationList.GetSelectedFolder());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                NotificationList.AddFolder(folder);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newFolderControl = new NewFolder()
                {
                    DataContext = folder
                };
                GeneralDialogContent Dialog = new GeneralDialogContent(newFolderControl, GeneralDialogButtons.OkCancelButtons)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = ""
                };
                if (Dialog.ShowDialog() == true)
                {
                    NotificationList.AddFolder(folder);
                    /*Document.AddUndoAction(NotificationList, folder, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
                    NotificationList.AddTreeItem(folder, NotificationList.GetSelectedParentFolder(false, true)).IsSelected = true;*/
                }
                else
                {
                    folder.Delete();
                }
            }
            e.Handled = true;
        }

        private void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabNotifications);
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else if (tabPluginList.IsSelected)
                pluginList.EditSelectedItem();
            else if (tabNotifications.IsSelected)
                NotificationList.EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabPluginList.IsSelected)
                e.CanExecute = pluginList.IsAnyItemSelected();
            else if (tabNotifications.IsSelected)
                e.CanExecute = NotificationList.IsAnyItemSelected();
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.EditorManagerComponent.Workspace.ContextObject = null;

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
        }
        #endregion


        public void SelectNotificationTab()
        {
            tabNotifications.IsSelected = true;
        }

        private void TransportAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var bas = ADServerInfo.ADServerInfo.GetCurrentApplicationBaseAddresses();
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
                if (TransportGrid.SelectedItem is ADBaseAddress)
                {
                    var ba = TransportGrid.SelectedItem as ADBaseAddress;

                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var bas = ADServerInfo.ADServerInfo.GetCurrentApplicationBaseAddresses();
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
                            //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
                            uow.CommitChanges();
                            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(generalSettings.DataContext);
                            TransportGrid.ItemsSource = null;
                            TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                        }
                    }
                }
            }
        }

        private void TransportDelete_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (TransportGrid.SelectedItem != null)
            {
                var si = TransportGrid.SelectedItem as ADBaseAddress;
                if (si != null && (MessageBox.Show(string.Format(Properties.Resources.AskDeleteStation, si.Path),
                    Properties.Resources.CaptionDeleteBA, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    si.Delete();

                    //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);
                    TransportGrid.ItemsSource = null;
                    TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                }
            }
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
                if (Document.EditorManagerComponent.UIInterface != null)
                {
                    Document.EditorManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.ServiceManagerStartFailed.Replace("'newline'", Environment.NewLine), ex.Message));
                }
            }
        }

        private void CanServiceManager(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
