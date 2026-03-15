using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using Utilities;
using DevExpress.Xpf.Editors;
using WPFUtilities;
using ScreenManager.Converters;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for AliasEditor.xaml
    /// </summary>
    /// 
    //
    public class KeyValuePair : INotifyPropertyChanged, ICloneable
    {
        #region Public Properties
        String value;
        public String Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        String key;
        public String Key
        {
            get { return key; }
            set
            {
                this.key = value;
                OnPropertyChanged("Key");
            }
        }
        #endregion

        #region Methods
        public object Clone()
        {
            return new KeyValuePair() {
                Key = key,
                Value = value
            };
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }

    public partial class AliasEditor : UserControl, INotifyPropertyChanged
    {
        BindingList<KeyValuePair> list;
        bool tagsListLoading = false;
        bool bEditing;
        bool bBlinking;
        //DataSinkInterface dataSinkInEditing;
        IUFUAEditorManager editor;
        IDocument doc;
        private string childSeparator = @"\\";
        #region Public Props
        #region TagsListLoading
        [Browsable(false)]
        public bool TagsListLoading
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return tagsListLoading;
            }
            set
            {
                if (tagsListLoading != value)
                {
                    tagsListLoading = value;
                    OnPropertyChanged("TagsListLoading");
                }
            }
        }
        #endregion
        #endregion
        public AliasEditor(Dictionary<string, string> map)
        {
            InitializeComponent();

            list = new BindingList<KeyValuePair>();
            foreach (var x in map.OrderBy(x => x.Key))
                list.Add(new KeyValuePair() { Key = x.Key, Value = x.Value });
            gridControl.ItemsSource = list;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (editor == null && OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                if (doc == null)
                    return;

                editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (editor != null)
                {
                    var converter = FindResource("AliasValueToHumanReadableConverter") as AliasValueToHumanReadableConverter;
                    if (converter != null)
                    {
                        converter.ufuaEditor = editor;
                        converter.document = doc;
                    }
                }
            }
        }

        internal Dictionary<string, string> GetMap()
        {
            var ret = new Dictionary<string, string>();
            foreach (var item in list)
            {
                //overwrites previous values
                ret[item.Key] = item.Value;
            }

            return ret;
        }

        void view_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                OnDelete(null, null);
            }
            if (e.Key == Key.Insert)
            {
                OnAdd(null, null);
            }
        }

        private void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex >= 0)
                list.RemoveAt(listIndex);
        }
        private void InsertRow(int rowHandle, KeyValuePair data)
        {
            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex < 0 || listIndex >= list.Count) listIndex = -1;
                list.Insert(listIndex + 1, data);
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            InsertRow(tableView.FocusedRowHandle, new KeyValuePair() { Key = "key", Value = "value" });
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            btnDelete.IsEnabled = TableView_GetSelectedRowIndex() >= 0;
        }

        private bool TableView_IsAnyItemSelected()
        {
            return TableView_GetSelectedRowIndex() >= 0;
        }

        private int TableView_GetSelectedRowIndex()
        {
            return gridControl.GetRowListIndex(tableView.FocusedRowHandle);
        }

        string CheckCreateNewKey(string oldKey)
        {
            int i = 0;
            var id = oldKey;
            while (IsDuplicateKey(id))
                id = String.Format("{0}{1}{2}", oldKey, Properties.Resources.NewKeySeparator, ++i);
            return id;
        }

        bool IsDuplicateKey(string k)
        {
            return (from o in list where o.Key == k select o).Any();
        }

        private string BuildTagPath(OPCUAEntityReference tag)
        {
            if(editor == null || doc == null)
                return null;

            var actualTagProject = editor.GetProjectDocument(doc, tag.AppName);

            string newPath = "";

            if (actualTagProject != null && actualTagProject.Parent != null)
            {
                newPath = actualTagProject.Title + childSeparator + tag.StringRepresentation;

                while (actualTagProject.Parent.Parent != null)
                {
                    newPath = actualTagProject.Parent.Title + childSeparator + newPath;

                    actualTagProject = actualTagProject.Parent;
                }

            }
            else
                newPath = tag.StringRepresentation;

            return newPath;
        }

        #region Tag ComboBox
        private void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (bBlinking)
                return;

            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo == null || combo.Text == null || !bEditing || editor == null)
            {
                bEditing = false;
                //dataSinkInEditing = null;
                return;
            }
            bEditing = false;
            var uriButton = GetDockElement<Button>(combo, "uriButton");
            if (uriButton == null)
                return;

            var names = combo.Text.Split('.');
            var childNames = combo.Text.Replace(childSeparator, "|").Split('|');
            if (names.Length > 1)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface(names[0]);
                if (datasync != null)
                {

                    var isMissing = datasync?.CheckVariable(combo.Text.Substring(names[0].Length + 1).Replace('\\', '&').Replace('/', '&'));
                    if (!isMissing.HasValue || isMissing.Value)
                    {
                        BlinkForegroundControl(combo);
                        uriButton.Tag = combo.Text;
                    }
                    else
                    {
                        var eaEntity = datasync.GetReference(combo.Text.Substring(names[0].Length + 1).Replace('\\', '&').Replace('/', '&'));
                        uriButton.Tag = String.Format("{0}.{1}", names[0], eaEntity.StringRepresentation);

                        var converter = FindResource("AliasValueToHumanReadableConverter") as IValueConverter;
                        if (converter != null)
                            combo.Text = converter.Convert(combo.Text, typeof(string), eaEntity, null) as String;
                    }
                }                
            }
            else if (childNames.Length > 1)
            {
                var tagName = childNames.Last();
                var xml = editor.GetTagEntityReference(doc, tagName, null, Project: combo.Text.Replace(childSeparator, "."));
                if (String.IsNullOrEmpty(xml))
                {
                    BlinkForegroundControl(combo);
                    uriButton.Tag = combo.Text;
                }
                else
                {
                    var tag = xml.FromXml<OPCUAEntityReference>();
                    uriButton.Tag = BuildTagPath(tag);

                    var converter = FindResource("AliasValueToHumanReadableConverter") as IValueConverter;
                    if (converter != null)
                        combo.Text = converter.Convert(combo.Text, typeof(string), null, null) as String;
                }
            }
            //else if (dataSinkInEditing != null)
            //{
            //    var isMissing = dataSinkInEditing.CheckVariable(combo.Text.Replace('\\', '&').Replace('/', '&'));
            //    if (isMissing)
            //    {
            //        BlinkForegroundControl(combo);
            //        uriButton.Tag = String.Format("{0}.{1}", dataSinkInEditing.DataSynkName, combo.Text);
            //    }
            //    else
            //    {
            //        var eaEntity = dataSinkInEditing.GetReference(combo.Text.Replace('\\', '&').Replace('/', '&'));
            //        uriButton.Tag = String.Format("{0}.{1}", eaEntity.AppName, eaEntity.StringRepresentation);

            //        var converter = FindResource("AliasValueToHumanReadableConverter") as IValueConverter;
            //        if (converter != null)
            //            combo.Text = converter.Convert(combo.Text, typeof(string), eaEntity, null) as String;
            //    }
            //}
            else
            {
                var split = combo.Text.Split(':');
                var instance = split[0];
                var name = split[0];
                if (split.Length > 1)
                    name = split[1];
                else
                    instance = null;

                var xml = editor.GetTagEntityReference(doc, name, instance);
                if (String.IsNullOrEmpty(xml))
                {
                    BlinkForegroundControl(combo);
                    uriButton.Tag = combo.Text;
                }
                else
                {
                    var tag = xml.FromXml<OPCUAEntityReference>();
                    uriButton.Tag = tag.StringRepresentation;
                    
                    var s = tag.StringRepresentationWithProject;
                    if (!String.IsNullOrEmpty(s))
                        combo.Text = s;
                }
            }
        }

        async void BlinkForegroundControl(Control control)
        {
            try
            {
                bBlinking = true;
                var oldColor = control.Foreground;
                control.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                control.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                control.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                control.Foreground = oldColor;
            }
            finally
            {
                bBlinking = false;
            }
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(sender as ComboBoxEdit, e.Key == Key.F5);
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bBlinking)
                return;

            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo == null)
                return;

            var selected = gridControl.SelectedItem as KeyValuePair;
            if (selected != null && combo.Text != selected.Value)
            {
                bEditing = true;
                //dataSinkInEditing = null;
                var varName = selected.Value;
                //var names = selected.Value.Split('.');
                //if (names.Length > 1)
                //{
                //    dataSinkInEditing = OPCUAEntityReference.GetDataSinkInterface(names[0]);
                //    if (dataSinkInEditing != null)
                //        varName = varName.Substring(names[0].Length + 1);
                //}
                combo.Text = varName;
            }

            combo.SelectAll();
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox(sender as ComboBoxEdit);
        }

        List<ComboBoxEdit> filledCombos = new List<ComboBoxEdit>();
        void FillComboBox(ComboBoxEdit uriLabel, bool bForceRefresh = false)
        {
            if (uriLabel == null || (filledCombos.Contains(uriLabel) && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            if (!filledCombos.Contains(uriLabel))
                filledCombos.Add(uriLabel);
            TagsListLoading = true;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                TagsListLoading = false;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
            //dataSinkInEditing = null;
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            
            var value = new OPCUAEntityReference(null);
            
            var uriLabel = GetDockElement<ComboBoxEdit>(button, "uriLabel");
            if (uriLabel == null)
                return;

            bEditing = false;
            if (value.Edit(sync: true, localserver: true))
            {
                if (value.HasValidValue)
                {                    
                    if (String.IsNullOrEmpty(value.EndpointUrl))
                        button.Tag = String.Format("{0}.{1}", value.AppName, value.StringRepresentation);
                    else if(editor != null && doc != null && editor.GetAplicationName(doc) != value.AppName)
                    {
                        button.Tag = BuildTagPath(value);
                    }
                    else
                    {
                        if (value.HumanReadable != null && 
                            value.HumanReadable.Contains(String.Format("({0})", SysVariables.SysNames.dataSynkName)) &&
                            SysVariables.ServerTags.Contains(value.StringRepresentation))
                        {
                            button.Tag = String.Format("{0}.{1}", SysVariables.SysNames.dataSynkName, value.StringRepresentation);
                        }
                        else
                            button.Tag = value.StringRepresentation;
                    }

                    var converter = FindResource("AliasValueToHumanReadableConverter") as IValueConverter;
                    if (converter != null)
                        uriLabel.Text = converter.Convert(button.Tag, typeof(string), value, null) as String;
                }
            }
        }

        T GetDockElement<T>(DependencyObject dependencyObject, string name) where T : FrameworkElement
        {
            var dockPanel = Utilities.WPF.DependencyObjectExtensions.FindFirstParent<DockPanel>(dependencyObject);
            return dockPanel?.FindName(name) as T;
        }
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
            if (TableView_IsAnyItemSelected())
            {
                var dataObject = new DataObject();
                var item = list[TableView_GetSelectedRowIndex()];
                var kvPair = item.Clone();
                dataObject.SetData(typeof(KeyValuePair), kvPair.ToXml());
                Clipboard.SetDataObject(dataObject, true);
            }
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TableView_IsAnyItemSelected();
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TableView_IsAnyItemSelected();
        }

        private void OnCommandCut(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                CopyToClipboard();
                DeleteRow(tableView.FocusedRowHandle);
            }
        }

        private void OnCommandPaste(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                var dataObject = Clipboard.GetDataObject() as DataObject;

                var copiedKvpString = dataObject.GetData(typeof(KeyValuePair)) as String;
                var copiedKvp = copiedKvpString.FromXml<KeyValuePair>();
                if (copiedKvp != null)
                {
                    copiedKvp.Key = CheckCreateNewKey(copiedKvp.Key);
                    InsertRow(tableView.FocusedRowHandle, copiedKvp);
                }
            }
        }

        private void CanCommandPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var dataObject = Clipboard.GetDataObject() as DataObject;
            if (dataObject != null)
            {
                var kvPairString = dataObject.GetData(typeof(KeyValuePair)) as String;
                e.CanExecute = kvPairString != null;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
