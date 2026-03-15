using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DocumentManager.ComponentService;
using Utilities.WPF;
using DevExpress.Xpf.Editors;
using WPFUtilities;
using StringManager.ComponentService;
using System.Windows.Input;
using Utilities;
using System.Collections.ObjectModel;

namespace StatesChartControl.Controls
{
    /// <summary>
    /// Interaction logic for PenValuesListEditor .xaml
    /// </summary>
    public partial class PenValuesListEditor : UserControl
    {
        #region DP
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(PenValuesListEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        readonly PenList focusedPenList;
        readonly ValueItemList datalist;
        #endregion

        #region Ctor
        public PenValuesListEditor(PenList focusedPenList, IDocument doc)
        {
            InitializeComponent();
            this.focusedPenList = focusedPenList;
            datalist = focusedPenList.Valuelist;

            gridControl.ItemsSource = datalist;
            DataContext = datalist;
            Document = doc;
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
            popupColorEditVNF.DataContext = this.focusedPenList;
        }
        #endregion

        readonly List<ComboBoxEdit> listStretchFilled = new List<ComboBoxEdit>();
        public readonly List<String> stretches = Enum.GetNames(typeof(Stretch)).ToList();

        #region Methods
        private void STRETCH_Editor_DropDownOpened(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listStretchFilled.Contains(combo))
                return;
            listStretchFilled.Add(combo);
            combo.ItemsSource = stretches;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var val = (from d in datalist.AsParallel() orderby d.dValue descending select d.dValue).ToList();
            var stringval = (val.DefaultIfEmpty(-1).FirstOrDefault() + 1).ToString();
            datalist.Add(new ValueItem() { Value = stringval });
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void DeleteRow(int rowHandle)
        {
            if (datalist.Count < 1)
                return;

            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0 && datalist.Count > listIndex)
            {
                datalist.RemoveAt(listIndex);
                ApplyChanges();
            }
        }

        private void InsertRow(int rowHandle, ValueItem data, bool bIsRealIndex = false)
        {
            var realIndex = rowHandle;
            if (!bIsRealIndex)
            {
                realIndex = gridControl.GetListIndexByRowHandle(rowHandle) + 1;
                if (realIndex < 0 || realIndex >= datalist.Count) realIndex = 0;
            }
            datalist.Insert(realIndex, data);
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex < 0 || listIndex >= datalist.Count - 1)
                return;
            datalist.Move(listIndex, listIndex + 1);
            gridControl.SelectedItem = datalist[listIndex + 1];
            ApplyChanges();
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex <= 0 || listIndex >= datalist.Count)
                return;
            datalist.Move(listIndex, listIndex - 1);
            gridControl.SelectedItem = datalist[listIndex - 1];
            ApplyChanges();
        }

        //private void ButtonVNF_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Document == null)
        //        return;

        //    Button senderbutton = (Button)sender;
        //    bool bResult;
        //    Brush _brush = smartControlHelper.TryChosingBrushWindow(this.FindParent<Window>(), sender as Button, Brushes.Yellow, out bResult);
        //    if (!bResult)
        //        return;
        //    senderbutton.Tag = _brush.Clone();

        //    _brush = null;
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    int focusedrow = tableView.FocusedRowHandle;
        //    int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
        //    if (listIndex >= 0)
        //    {
        //        if (Document == null)
        //            return;
        //        bool bResult;
        //        Brush _brush = smartControlHelper.TryChosingBrushWindow(this.FindParent<Window>(), sender as Button, Brushes.Yellow, out bResult);
        //        if(!bResult)
        //            return;
        //        (sender as Button).Tag = _brush.Clone();

        //        _brush = null;
        //        gridControl.RefreshRow(listIndex);

        //        tableView.FocusedRowHandle = focusedrow;
        //        ApplyChanges();
        //    }
        //}

        void ItemValue_Validate(object sender, ValidationEventArgs e)
        {
            e.IsValid = !datalist.CheckDuplicateItemsValue(e.Value.ToString());
            if (e.IsValid)
                return;
            e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
            e.ErrorContent = Properties.Resources.DuplicateItemValueError;
        }

        private void DlgButtonClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (datalist.ElementAt(listIndex).Label) = string.Empty;
                tableView.FocusedRowHandle = focusedrow;
            }
        }
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                Button button = (Button)sender;
                String value;
                value = (String)button.Tag;

                if (Document == null)
                    return;
                IStringEditorManager stringEditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringEditorManager == null)
                    return;
                var stringEditor = stringEditorManager.GetStringEditor(Document);

                if (stringEditor == null)
                    return;

                var Dialog = new Utilities.GeneralDialogContent(stringEditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectStringEditor,
                    HelpLink = "StringEditor"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (stringEditor.DataContext != null && (stringEditor.DataContext as String) != null)
                {
                    (datalist.ElementAt(listIndex).Label) = new String((stringEditor.DataContext as String).ToArray());
                    tableView.FocusedRowHandle = focusedrow;
                    ApplyChanges();
                }
            }
        }

        void ApplyChanges()
        {
            if (datalist.Count > 0)
                focusedPenList.FirstBackground = (datalist[0] as ValueItem).ControlBackground?.Clone();
            else
                focusedPenList.FirstBackground = Brushes.Transparent;
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
            var collection = new ValueItemList(gridControl.SelectedItems.Cast<ValueItem>().ToList());
            var dataObject = new DataObject();
            dataObject.SetData(typeof(string), collection.ToXml());
            Clipboard.SetDataObject(dataObject, true);
        }

        private void CanCommandCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            var itemsList = gridControl.SelectedItems.Cast<ValueItem>().ToList();
            e.CanExecute = itemsList.Count > 0;
        }

        private void CanCommandCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle) >= 0;
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

                var copiedItemString = dataObject.GetData(typeof(string)) as string;
                var copiedItems = ValueItemList.FromXml(copiedItemString);
                if (copiedItems != null)
                {
                    int focusedRow = Math.Max(-1, tableView.FocusedRowHandle);
                    foreach (ValueItem item in copiedItems)
                    {
                        focusedRow++;
                        InsertRow(focusedRow, item, true);
                    }
                    ApplyChanges();
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
                e.CanExecute = itemString != null && ValueItemList.FromXml(itemString) != null;
            }
        }
        #endregion
    }
}
