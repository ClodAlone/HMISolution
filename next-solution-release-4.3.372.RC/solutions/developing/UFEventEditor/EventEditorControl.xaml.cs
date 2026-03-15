using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using UFEventEditor.Document;
using UFEventEditor.Controls;
using Utilities;
using Utilities.WPF;
using UFEventModel;
using UFEventEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.Grid;

namespace UFEventEditor
{

    public class pluginDesc
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String AssemblyName { get; set; }
        public Opc.Ua.NodeId NodeId { get; set; }
    }

    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    /// 
    public partial class EventEditorControl : UserControl, IEditableObject, IDisposable
    {
        #region Declarations
        EventList eventList;
        readonly EventEditorManagerComponent EditorComponent;
        #endregion

        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public EventEditorControl(EventEditorManagerComponent ed, EventEditorDocument doc)
        {
            InitializeComponent();
            Document = doc;
            EditorComponent = ed;

            Loaded += (o, e) =>
            {
                bLoaded = true;
                eventList.OnActivate();
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            Document.CreateUndoRedoHelper(this);
            eventList = new EventList(EditorComponent, Document, this);
            inEventsGrid.Children.Add(eventList);
        }

        internal void OnActivate()
        {
                eventList.OnActivate();
        }

        public void ActivateCommandExplorer()
        {
            if (EditorComponent.CommandExplorer == null)
                return;

            EditorComponent.CommandExplorer.Activate();
        }

        #region IEditableObject Members
        bool bEditingUow;
        public void BeginEdit()
        {
            if (bEditingUow)
                return;

            if (Document.UowContext != null)
            {
                bEditingUow = true;
                Document.UowContext.BeforeFlushChanges += uowContext_BeforeFlushChanges;
            }
        }

        public void CancelEdit()
        {
            if (!bEditingUow)
                return;

            try
            {
                if (Document.UowContext != null)
                {
                    Document.UowContext.RollbackTransaction();
                    Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                }
            }
            finally
            {
                bEditingUow = false;
            }
        }

        public void EndEdit()
        {
            if (!bEditingUow)
                return;

            bool bResult = true;
            try
            {

                if (Document.UowContext != null)
                {
                    bResult = Document.UowContext.TryCommitChanges(EventEditorDocument.log);
                    Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                }
            }
            finally
            {
                bEditingUow = false;
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
            UserControl currentTabEditor = eventList;
           
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

        EventEditorDocument _Document;
        [Browsable(false)]
        public EventEditorDocument Document
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
                eventList.DeleteSelectedItems();
            }
        }

        private void CanRemoveItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (eventList.treeListControl.SelectedItem != null);
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                eventList.CopySelectedToClipboard();
                eventList.DeleteSelectedItems();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (eventList.IsAnyItemSelected());
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                eventList.CopySelectedToClipboard();
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = eventList.IsAnyItemSelected();
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);
                eventList.PasteFromClipboard();
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            Document.CopyWinClipboardToInMemoryData();
            e.CanExecute = Document.ClipboardContainsEvents() || Document.ClipboardContainsFolders();
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                eventList.UndoAction();
            }
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = eventList.IsAnyUndoActionAvailable();
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                eventList.RedoAction();
            }
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = eventList.IsAnyRedoActionAvailable();
        }
        private void OnAddNewEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeListNode q = eventList.treeListControl.GetSelectedNodes().FirstOrDefault();
            UFEventFolder f = eventList.itemRoot.Tag as UFEventFolder;
            if (q != null && q.Tag != null)
                f = q.Tag as UFEventFolder;

            UFEventObject neweve = Document.AddNewEvent(f);
            if (Document.EditorManagerComponent.PropertyControl != null)
            {
                eventList.AddEvent(neweve);
                Document.EditorManagerComponent.PropertyControl.Activate();
            }
            else
            {
                var newevent = new NewEvent(EditorComponent, Document) { DataContext = neweve };
                GeneralDialogContent Dialog = new GeneralDialogContent(newevent)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "AddNewEvent"
                };
                if (Dialog.ShowDialog() == true)
                {
                    EditorComponent.CommandExplorer.PropagateChanges(newevent.CommandCtrl.Content as UserControl);
                    eventList.AddEvent(neweve);
                }
                else
                {
                    neweve.Delete();
                }
            }
            e.Handled = true;
        }

        private void CanAddNewEvent(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            UFEventFolder folder = Document.AddNewFolder(eventList.GetSelectedFolder());

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
            e.Handled = true;
        }

        private void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Document.EditorManagerComponent.PropertyControl != null)
                Document.EditorManagerComponent.PropertyControl.Activate();
            else
                eventList.EditSelectedItem();
        }

        private void CanCommandProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = eventList.IsAnyItemSelected();
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
        }
        #endregion
    }
}
