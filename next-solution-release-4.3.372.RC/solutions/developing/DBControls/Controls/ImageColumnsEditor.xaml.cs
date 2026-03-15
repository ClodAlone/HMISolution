using System;
using System.Windows;
using System.Windows.Controls;
using CommonControls.PropertyDataTemplate;
using WPFUtilities.PropertyDataTemplate;
using DocumentManager.ComponentService;
using UFInterfaces;
using System.Collections.Generic;
using System.Windows.Input;
using Utilities;
using System.Linq;

namespace DBControls.Controls
{
    /// <summary>
    /// Interaction logic for ImageColumnsEditor.xaml
    /// </summary>
    public partial class ImageColumnsEditor : UserControl
    {
        #region DP
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(ImageColumnsEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            ImageColumnsEditor control = o as ImageColumnsEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImageColumnsEditor control = o as ImageColumnsEditor;
            if (control != null)
                control.OnWorkspaceChanged((IWorkspace)e.OldValue, (IWorkspace)e.NewValue);
        }

        protected virtual IWorkspace OnCoerceWorkspace(IWorkspace value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkspaceChanged(IWorkspace oldValue, IWorkspace newValue)
        {
            if (newValue != null && ListThresholdDt == null)
            {
                var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.FilterProperty, Properties.Resources.AllPictureFiles);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Images);
                factory.SetValue(SourceFilePropertyEditor.DefaultExtProperty, "ico");
                ListThresholdDt = new DataTemplate();
                ListThresholdDt.DataType = typeof(Uri);
                ListThresholdDt.VisualTree = factory;
            }
        }

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        public ImageThresholdCollection ImageThresholds {
            get;
            set;
        }

        #endregion
        #endregion

        #region Public Props
        public DataTemplate ListThresholdDt
        {
            get;
            set;
        }
        #endregion

        #region Declarations
        bool bLoaded;
        ImageThreshold lastSelectedItem;
        String columnName;
        #endregion

        #region Constructors
        public ImageColumnsEditor(String colName, List<ImageThreshold> loadingThres)
        {
            InitializeComponent();
            columnName = colName;
            ImageThresholds = new ImageThresholdCollection();
            foreach (var thr in loadingThres)
                ImageThresholds.Add(thr);

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
            };
        }
        #endregion

        #region Public Methods

        #endregion

        #region CommandBindings
        private void OnCommandCopy(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                CopyToClipboard();
            }
        }

        private void CopyToClipboard()
        {
            var collection = new ImageThresholdCollection(ListBoxThresholds.SelectedItems.Cast<ImageThreshold>().ToList());
            var dataObject = new DataObject();
            dataObject.SetData(typeof(string), collection.ToXml());
            Clipboard.SetDataObject(dataObject, true);
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            var itemsList = ListBoxThresholds.SelectedItems.Cast<ImageThreshold>().ToList();
            e.CanExecute = itemsList.Count > 0;
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            var itemsList = ListBoxThresholds.SelectedItems.Cast<ImageThreshold>().ToList();
            e.CanExecute = itemsList.Count > 0;
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                CopyToClipboard();
                DeleteLastSelectedItem();
            }
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                var dataObject = Clipboard.GetDataObject() as DataObject;

                var copiedItemString = dataObject.GetData(typeof(string)) as string;
                var copiedItems = ImageThresholdCollection.FromXml(copiedItemString);
                if (copiedItems != null)
                {
                    foreach (ImageThreshold item in copiedItems)
                    {
                        ImageThresholds.Add(item);
                    }
                }
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var dataObject = Clipboard.GetDataObject() as DataObject;
            if (dataObject != null)
            {
                var itemString = dataObject.GetData(typeof(string)) as String;
                e.CanExecute = itemString != null && ImageThresholdCollection.FromXml(itemString) != null;
            }
        }
        #endregion

        #region Private Methods

        void OnListBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!bLoaded)
                return;

            lastSelectedItem = ListBoxThresholds.SelectedItem as ImageThreshold;
        }

        void OnItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteLastSelectedItem();
        }

        void OnItemPreviewMouseDown(object sender, RoutedEventArgs e)
        {
            ((ListBoxItem)sender).IsSelected = true;
        }

        void btnAddThreshold_Click(object sender, RoutedEventArgs e)
        {
            ImageThresholds.Add(new ImageThreshold(columnName));
        }

        void btnDelThreshold_Click(object sender, RoutedEventArgs e)
        {
            DeleteLastSelectedItem();
        }

        void btnClearThreshold_Click(object sender, RoutedEventArgs e)
        {
            ImageThresholds.Clear();
        }

        void DeleteLastSelectedItem()
        {
            if (lastSelectedItem == null || !ImageThresholds.Contains(lastSelectedItem))
                return;

            ImageThresholds.Remove(lastSelectedItem);
        }

        #endregion
    }
}
