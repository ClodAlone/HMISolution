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
using WPFUtilities;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using Utilities.Xpo.UndoRedo;
using UFInterfaces.Editors;

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
                //var driver = gridDataControl.SelectedItem as UFUAModel.UFUACommunicationDriver;
                if (Document.IsDriverNameUsed(driver.Name))
                {
                    using (new ResetCursor())
                    {
                        var ret = MessageBox.Show(String.Format(Properties.Resources.DriverUsed, driver.FriendlyName),
                                                driver.FriendlyName, MessageBoxButton.YesNo);
                        if (ret == MessageBoxResult.No)
                            continue;
                    }
                }

                listundo.Add(driver as IXPSimpleObject);
                driver.Delete();
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, UndoRedoAction.Removed);

            FlatGridRefresh();
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
