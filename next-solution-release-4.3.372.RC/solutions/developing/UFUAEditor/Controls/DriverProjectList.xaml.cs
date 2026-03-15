using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using UFUAModel;
using HelpProvider.ComponentService;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using Utilities.Xpo.UndoRedo;
using UIMsgBoxAlertService.ComponentService;
using System.Reflection;
using DriverSettingsInterfaces;


namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for DriverProjectList.xaml
    /// </summary>
    public partial class DriverProjectList : UserControl, IDisposable
    {
        #region Declarations

        readonly UFUAServerDocument Document;
        readonly bool bShowOnlyCanImportTags;
        bool isPopup;
        private IUIMsgBoxAlertService uiMsgBox;
        #endregion

        #region Address Space

        public DriverProjectList(UFUAServerDocument doc, bool bPopup = false, bool bFilterCanImportTags = false)
        {
            InitializeComponent();
            Document = doc;
            isPopup = bPopup;
            bShowOnlyCanImportTags = bFilterCanImportTags;

            Document.CreateUndoRedoHelper();
            Document.ChangedDocument += Document_ChangedDocument;

            gridDataControl.SelectedItemChanged += (s, e) =>
            {
                e.Handled = true;
                {
                    if (gridDataControl.SelectedItems.Count == 0)
                    {
                        Document.EditorManagerComponent.Workspace.ContextObject = null;
                    }
                    else if (gridDataControl.SelectedItems.Count == 1)
                    {
                        Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(gridDataControl.SelectedItem);
                    }
                    else
                    {
                        Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(gridDataControl.SelectedItems);
                    }
                }
            };
            FlatGridRefresh();
            InitializeUIMessageBoxService();
        }

        void Document_ChangedDocument(object sender, ChangedDocumentEvent e)
        {
            if (e.Source == this)
                return;

            if (e.ChangedType == ChangedType.removed || e.ChangedType == ChangedType.added)
            {
                var changedObjects = (from object c in e.ChangedObjects.AsParallel()
                                      where c is UFUAModel.UFUACommunicationDriver
                                      select c).ToList();

                if (changedObjects.Count > 0)
                    FlatGridRefresh();
            }
        }

        internal void OnActivate()
        {
            if (dpFlatGridRefresh != null && dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
                FlatGridRefresh();

            if (gridDataControl.SelectedItems.Count == 0)
            {
                Document.EditorManagerComponent.Workspace.ContextObject = null;
            }
            else if (gridDataControl.SelectedItems.Count == 1)
            {
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(gridDataControl.SelectedItem);
            }
            else
            {
                Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(gridDataControl.SelectedItems);
            }
        }

        DispatcherOperation dpFlatGridRefresh;
        List<UFUACommunicationDriver> itemsToSelect = null;
        internal void FlatGridRefresh(List<UFUACommunicationDriver> selectItems = null)
        {
            itemsToSelect = selectItems;
            if (dpFlatGridRefresh == null || dpFlatGridRefresh.Status == DispatcherOperationStatus.Aborted)
            {
                var selected = gridDataControl.SelectedItem;
                gridDataControl.ItemsSource = null;
                var action = new Action(() =>
                {
                    using (new WaitCursor())
                    {
                        gridDataControl.ItemsSource = null;
                        var list = new List<UFUAModel.UFUACommunicationDriver>();
                        if (bShowOnlyCanImportTags)
                        {
                            list.AddRange((from drv in Document.GetDrivers().ToList().AsParallel()
                                           where drv.CanImportTags
                                           select drv));
                        }
                        else
                            list.AddRange(Document.GetDrivers());

                        gridDataControl.ItemsSource = list;
                        if (!list.Contains(selected))
                            selected = null;

                        if (itemsToSelect == null)
                            gridDataControl.SelectedItem = selected;
                        else
                            gridDataControl.SelectedItems = itemsToSelect;

                        var selectedHandles = gridDataControl.GetSelectedRowHandles();
                        if (selectedHandles.Count() > 0)
                            gridDataControl.View.FocusedRowHandle = selectedHandles.Last();
                    }
                });

                //if (gridDataControl.IsVisible)
                //    action();
                //else
                {
                    dpFlatGridRefresh = Dispatcher.BeginInvokeAsynchronouslyInRender(gridDataControl, () =>
                    {
                        if (bDisposed)
                            return;
                        dpFlatGridRefresh = null;

                        action();
                    });
                }
            }
        }

        private void InitializeUIMessageBoxService()
        {
            if (Document != null)
            {
                uiMsgBox = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            }
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (isPopup)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
            else
            {
                //if (Document.EditorManagerComponent.PropertyControl != null)
                //    Document.EditorManagerComponent.PropertyControl.Activate();
                //else
                EditSelectedItem();
            }
        }

        internal void EditSelectedItem()
        {
            var selected = gridDataControl.SelectedItem as UFUAModel.UFUACommunicationDriver;
            if (selected != null)
            {
                UFUAServerDocument.EditDriverSettings(selected, Document);
                FlatGridRefresh();
            }
        }

        internal void DeleteSelectedItems()
        {
            var items = new List<UFUAModel.UFUACommunicationDriver>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is UFUAModel.UFUACommunicationDriver)
                    items.Add(item as UFUAModel.UFUACommunicationDriver);
            }

            var listundo = new List<IXPSimpleObject>();
            foreach (var driver in items)
            {
                List<UFUATag> tags = new List<UFUATag>();
                if (Document.IsDriverNameUsed(driver.Name, ref tags))
                {
                    using (new ResetCursor())
                    {
                        var driverRemovalConfirmationDialogResult = CustomDialogResults.Yes;

                        if (uiMsgBox != null)
                        {
                            driverRemovalConfirmationDialogResult = 
                                uiMsgBox.ShowYesNo(
                                    String.Format(Properties.Resources.DriverUsed, driver.FriendlyName),
                                    CustomDialogIcons.Question,
                                    driver.FriendlyName);
                        }
                                
                        if (driverRemovalConfirmationDialogResult == CustomDialogResults.No)
                        {
                            continue;
                        }                          
                    }
                }

                var disableDriverAndKeepSettingsOrDeleteItResult = CustomDialogResults.Yes;

                if (uiMsgBox != null)
                {
                    disableDriverAndKeepSettingsOrDeleteItResult = 
                        uiMsgBox.ShowYesNo(
                            String.Format(Properties.Resources.DisableOrDeleteDriver, driver.FriendlyName),
                            CustomDialogIcons.Question,
                            driver.FriendlyName);
                }
                            
                if (disableDriverAndKeepSettingsOrDeleteItResult == CustomDialogResults.Yes)
                {
                    DisableDriver(tags, driver, listundo);
                }
                else
                {
                    DeleteDriversAndSettingsPermanently(tags, driver, listundo);
                }   
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

            FlatGridRefresh();
        }

        private ICommunicationDriverWpfEditing3 GetCommunicationDriverWpfEditing(UFUACommunicationDriver driver)
        {
            try
            {
                var uidll = UFUAServerInfo.UFUAServerInfo
                                        .GetDriversUIName(
                                            String.Format("{0}\\{1}",
                                            UFUAServerInfo.UFUAServerInfo.GetDriversFolder(),
                                            driver.AssemblyName));

                var driverAssemblyTypes = Assembly.LoadFile(uidll).GetTypes();

                var communicationDriverWpfEditing = driverAssemblyTypes
                    .Where(driverAssemblyType =>
                        driverAssemblyType.IsAbstract == false &&
                        typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(driverAssemblyType))
                    .Select(driverAssemblyType =>
                        (ICommunicationDriverWpfEditing)Activator.CreateInstance(driverAssemblyType))
                    .FirstOrDefault();

                var communicationDriverWpfEditing3 = communicationDriverWpfEditing as ICommunicationDriverWpfEditing3;
                
                return communicationDriverWpfEditing3;
            }
            catch
            {
                return null;
            }      
        }

        private void ShowDriverDisabledNotification(string driverFriendlyName, string infoMessage)
        {
            if (uiMsgBox != null)
            {
                uiMsgBox.ShowInformation(driverFriendlyName + ". " + infoMessage);
            }
            else
            {
                MessageBox.Show(infoMessage);
            }
        }

        private void DisableDriver(List<UFUATag> tags, UFUACommunicationDriver driver, List<IXPSimpleObject> listUndo)
        {
            try
            {
                var communicationDriverWpfEditing3 = GetCommunicationDriverWpfEditing(driver);

                if (communicationDriverWpfEditing3 == null) 
                {
                    var driverRemovalConfirmationDialogResult = CustomDialogResults.No;

                    if (uiMsgBox != null)
                    {
                        driverRemovalConfirmationDialogResult =
                            uiMsgBox.ShowYesNo(
                                String.Format(Properties.Resources.DriverCouldNotBeDisabled, driver.FriendlyName),
                                CustomDialogIcons.Question,
                                driver.FriendlyName);
                    }

                    if (driverRemovalConfirmationDialogResult == CustomDialogResults.Yes)
                    {
                        DeleteDriversAndSettingsPermanently(tags, driver, listUndo);
                    }
                    else
                    {
                        ShowDriverDisabledNotification(driver.FriendlyName, Properties.Resources.DriverWasNotDisabledInfo);
                    }

                    return;
                }

                var isDriverDisabled = communicationDriverWpfEditing3.DisableDriver(Document.ConnectionString, Document.Protected, Document.Id);

                if (isDriverDisabled)
                {
                    ShowDriverDisabledNotification(driver.FriendlyName, Properties.Resources.DriverWasDisabled);
                }
                else
                {
                    ShowDriverDisabledNotification(driver.FriendlyName, Properties.Resources.DriverWasNotDisabledWarning);
                }
            }
            catch (Exception ex)
            {
                if (uiMsgBox != null)
                {
                    uiMsgBox.ShowError(Properties.Resources.ErrorWhileDisablingDriver + " " + ex.Message);
                }
                else
                {
                    MessageBox.Show(ex.Message, Properties.Resources.ErrorWhileDisablingDriver);
                }
            }
        }

        private void DeleteDriversAndSettingsPermanently(List<UFUATag> tags, UFUACommunicationDriver driver, List<IXPSimpleObject> listUndo)
        {
            try
            {
                foreach (var tag in tags)
                {
                    tag.DynamicSettingsFlat.Remove(driver.FriendlyName);
                }

                listUndo.Add(driver);
                driver.Delete();

                UFUAServerDocument.UpdateDynamicSettingsAfterDriverCancellation(driver, tags);

                if (Document.FilePath != null) //In case of project saved in DB there are no files to delete  
                {
                    UFUAServerDocument.DeleteDriverSettings(driver, Document);
                }
            }
            catch(Exception ex)
            {
                if (uiMsgBox != null)
                {
                    uiMsgBox.ShowError(Properties.Resources.ErrorDeletingDriverSettingsFile + " " + ex.Message);
                }
                else
                {
                    MessageBox.Show(ex.Message, Properties.Resources.ErrorDeletingDriverSettingsFile);
                }
            }
        }

        internal void CopySelectedToClipboard()
        {
            var listdrivers = new List<UFUAModel.UFUACommunicationDriver>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is UFUAModel.UFUACommunicationDriver)
                    listdrivers.Add(item as UFUAModel.UFUACommunicationDriver);
            }

            Document.CleanClipbaord();
            Document.CopyDriversToClipbaord(listdrivers);
            Document.CheckClipbaord();
        }

        internal void PasteFromClipboard()
        {
            var listdrivers = Document.PasteClipboardDrivers();
            if (listdrivers.Count > 0)
            {
                FlatGridRefresh(listdrivers);

                // add list to undo manager
                var listundo = new List<IXPSimpleObject>();
                listdrivers.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                Document.AddUndoAction(this, listundo, UndoRedoAction.Added);
            }
        }

        internal UFUAModel.UFUACommunicationDriver GetSelectedItem()
        {
            return gridDataControl.SelectedItem as UFUAModel.UFUACommunicationDriver;
        }

        internal bool IsAnyItemSelected()
        {
            return gridDataControl.SelectedItem is UFUAModel.UFUACommunicationDriver;
        }

        #endregion
        
        #region Undo/Redo

        internal void UndoAction()
        {
            UndoRedoAction action;
            var list = Document.UndoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            list.Sources.ForEach(obj => { obj.Delete(); });
                            FlatGridRefresh();
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        internal void RedoAction()
        {
            UndoRedoAction action;
            var list = Document.RedoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            list.Sources.ForEach(obj => { obj.Delete(); });
                            FlatGridRefresh();
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.ChangedDocument -= Document_ChangedDocument;

            if (dpFlatGridRefresh != null && dpFlatGridRefresh.Status != DispatcherOperationStatus.Aborted &&
                dpFlatGridRefresh.Status != DispatcherOperationStatus.Completed)
            {
                dpFlatGridRefresh.Abort();
            }

            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void gridDataControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                //show help...
                if (gridDataControl.SelectedItem != null)
                {
                    UFUACommunicationDriver drv = gridDataControl.SelectedItem as UFUACommunicationDriver;

                    var helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                    if (helpProvider != null)
                        helpProvider.OpenDialogHelpPage(drv.Name, true, true);
                }
                e.Handled = true;
            }
        }
    }
}
