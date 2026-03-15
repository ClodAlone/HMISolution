using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using MSSchedulerSettings.Document;
using MSEditor.Controls;
using MSModel;
using MSSchedulerSettings.Controls;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Core;
using UIMsgBoxAlertService.ComponentService;
using log4net;

namespace MSEditor
{
    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    public partial class SchedulerEditorControl : UserControl, IEditableObject, IDisposable
    {

        #region Declarations
        //GeneralSettings generalSettings;
        EventList eventList;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public SchedulerEditorControl(SchedulerEditorDocument doc)
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
                else if (e.NewSelectedItem == tabEventList)
                {
                    if (eventList == null)
                    {
                        using (new WaitCursor())
                        {
                            eventList = new EventList(Document);
                            inPluginGrid.Children.Add(eventList);
                        }
                    }
                    eventList.OnActivate();
                }
            };
        }

        internal void OnActivate()
        {
            if (tabEventList.IsSelected && eventList != null)
                eventList.OnActivate();
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
            if (tabEventList.IsSelected)
                currentTabEditor = eventList;

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
        #endregion

        #region Properties

        SchedulerEditorDocument _Document;
        [Browsable(false)]
        public SchedulerEditorDocument Document
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
            if (tabEventList.IsSelected)
            {
                using (new WaitCursor())
                {
                    eventList.DeleteSelectedItems();
                }
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabEventList.IsSelected && eventList != null && eventList.IsAnyItemSelected());
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabEventList.IsSelected)
                {
                    eventList.CopySelectedToClipboard();
                    eventList.DeleteSelectedItems();
                }
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabEventList.IsSelected && eventList != null && eventList.IsAnyItemSelected();
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabEventList.IsSelected)
                    eventList.CopySelectedToClipboard();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabEventList.IsSelected)
                e.CanExecute = (eventList != null && eventList.IsAnyItemSelected());
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);
                if (tabEventList.IsSelected)
                    eventList.PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            Document.CopyWinClipboardToInMemoryData();
            if (tabEventList.IsSelected)
                e.CanExecute = Document.ClipboardContainsEvents() || Document.ClipboardContainsFolders(); ;
        }
        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabEventList.IsSelected)
                    eventList.UndoAction();
                /*else if (tabGeneralSettings.IsSelected)
                    generalSettings.UndoAction();*/
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabEventList.IsSelected)
                e.CanExecute = (eventList != null && eventList.IsAnyUndoActionAvailable());
            /*else if (tabGeneralSettings.IsSelected)
                e.CanExecute = generalSettings.IsAnyUndoActionAvailable();*/
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                if (tabEventList.IsSelected)
                    eventList.RedoAction();
                /*else if (tabGeneralSettings.IsSelected)
                    generalSettings.RedoAction();*/
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabEventList.IsSelected)
                e.CanExecute = (eventList != null && eventList.IsAnyRedoActionAvailable());
            /*else if (tabGeneralSettings.IsSelected)
                e.CanExecute = generalSettings.IsAnyRedoActionAvailable();*/
        }

        private void OnAddNewEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!tabEventList.IsSelected)
                tabEventList.IsSelected = true;
            MSScheduledAction newnot = Document.AddNewEvent(eventList.GetSelectedFolder());
            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                eventList.AddEvent(newnot);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newnotification = new NewEventControl(Document, styleName: ThemeImageHelper.GetTheme(Document)) { DataContext = newnot };
                GeneralDialogContent Dialog = new GeneralDialogContent(newnotification)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EventEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    if (tabEventList.IsSelected)
                    {
                        eventList.AddEvent(newnot);
                    }
                }
                else
                {
                    newnot.Delete();
                }
            }
        }

        private void CanAddNewEvent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabEventList);
        }

        private void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MSFolder folder = Document.AddNewFolder(eventList.GetSelectedFolder());

            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                eventList.AddFolder(folder);
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
                    eventList.AddFolder(folder);
                }
                else
                {
                    folder.Delete();
                }
            }
        }

        private void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tabControlExt.SelectedItem == tabEventList);
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
                        MSServerInfo.MSServerInfo.GetServerName(), ex.Message));
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
                        MSServerInfo.MSServerInfo.GetServerName(), ex.Message));
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

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else if (tabEventList.IsSelected)
                eventList.EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (tabEventList.IsSelected)
                e.CanExecute = eventList.IsAnyItemSelected();
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
            tabControlExt.Dispose();
        }
        #endregion

        internal void SelectNotificationTab()
        {
            tabEventList.IsSelected = true;
        }

        private void TransportAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var bas = MSServerInfo.MSServerInfo.GetCurrentApplicationBaseAddresses();
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
                MSBaseAddress ba = null;

                if (TransportGrid.SelectedItem is UFUAModel.UFUABaseAddress)
                {
                    var oba = TransportGrid.SelectedItem as UFUAModel.UFUABaseAddress;
                    ba = Document.AddNewBaseAddress(Opc.Ua.Utils.UriSchemeNetPipe);
                    ba.Enabled = oba.Enabled;
                    ba.Port = oba.Port;
                    ba.Server = oba.Server;
                    ba.Transport = oba.Transport;
                    oba.Delete();
                }
                if (TransportGrid.SelectedItem is MSBaseAddress)
                {
                    ba = TransportGrid.SelectedItem as MSBaseAddress;
                }
                if(ba != null)
                {
                    using (var uow = Document.BeginNestedUnitOfWork())
                    {
                        var bas = MSServerInfo.MSServerInfo.GetCurrentApplicationBaseAddresses();
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
            if (TransportGrid.SelectedItem != null)
            {
                var si = TransportGrid.SelectedItem as MSBaseAddress;
                var se = TransportGrid.SelectedItem as UFUAModel.UFUABaseAddress;
                if (si != null && (MessageBox.Show(string.Format(Properties.Resources.AskDeleteStation, si.Path),
                    Properties.Resources.CaptionDeleteBA, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    Document.GetConfiguration().BaseAddresses.Remove(si);
                    si.Delete();

                    //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);
                    TransportGrid.ItemsSource = null;
                    TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                }
                else if (se != null && (MessageBox.Show(string.Format(Properties.Resources.AskDeleteStation, se.Path),
                    Properties.Resources.CaptionDeleteBA, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    Document.GetConfiguration().BaseAddresses.Remove(se);
                    se.Delete();

                    //Document.AddUndoAction(this, ba, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);
                    TransportGrid.ItemsSource = null;
                    TransportGrid.ItemsSource = Document.GetConfiguration().BaseAddresses;
                }
            }
        }


    }
}
