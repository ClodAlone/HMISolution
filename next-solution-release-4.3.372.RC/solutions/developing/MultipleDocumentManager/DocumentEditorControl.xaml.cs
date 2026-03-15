using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using MultipleDocumentManager.Document;

namespace MultipleDocumentManager
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
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
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
        }
        #endregion
    }
}
