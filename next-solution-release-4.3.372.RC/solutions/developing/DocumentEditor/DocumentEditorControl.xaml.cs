using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using DocumentEditor.Document;
using Utilities;
using UIMsgBoxAlertService.ComponentService;

namespace DocumentEditor
{
    /// <summary>
    /// Interaction logic for DocumentEditorControl.xaml
    /// </summary>
    public partial class DocumentEditorControl : UserControl, IEditableObject, IDisposable
    {
        bool bLoaded;
        public bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        public DocumentEditorControl(DocumentEditorDocument doc)
        {
            Loaded += (o, e) =>
            {
                bLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                bLoaded = false;
            };

            InitializeComponent();
            Document = doc;

            Document.CreateUndoRedoHelper(this);
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            if (Document.UowContext != null)
            {
                if (Document.UowContext.TrackingChanges)
                    Document.UowContext.RollbackTransaction();

                Document.UowContext.BeginTrackingChanges();
                Document.UowContext.BeforeFlushChanges += uowContext_BeforeFlushChanges;
            }
        }

        public void CancelEdit()
        {
            if (Document.UowContext != null)
            {
                Document.UowContext.RollbackTransaction();
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                //Document.UowContext = null;
            }
        }

        public void EndEdit()
        {
            bool bResult = true;
            if (Document.UowContext != null)
            {
                bResult = Document.UowContext.TryCommitChanges(DocumentEditorDocument.log);
                Document.UowContext.BeforeFlushChanges -= uowContext_BeforeFlushChanges;
                //Document.UowContext = null;
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
                    Document.AddUndoAction(this, list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed);
            }
        }

        #endregion IEditableObject Members

        #region Properties

        DocumentEditorDocument _Document;
        [Browsable(false)]
        public DocumentEditorDocument Document
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

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyInMemoryDataToWinClipboard();
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                Document.CopyWinClipboardToInMemoryData(true);
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            Document.CopyWinClipboardToInMemoryData();
        }

        private void OnCommandUndo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanCommandUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnCommandRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void CanCommandRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void OnMyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("MyCommand Executed");
        }

        private void CanMyCommand(object sender, CanExecuteRoutedEventArgs e)
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
        }
        #endregion
    }
}
