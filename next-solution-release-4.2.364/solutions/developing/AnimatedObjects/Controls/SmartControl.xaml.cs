using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpf.Editors;
using StringManager.ComponentService;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace AnimatedObjects.Controls
{
    /// <summary>
    /// Interaction logic for SmartControl.xaml
    /// </summary>
    public partial class SmartControl : UserControl
    {
        #region DP
        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(SmartControl), new UIPropertyMetadata(null));
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
        readonly AnimatedObject bcontrol;
        AnimationItemList datalist;
        Window parentWindow;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(AnimatedObject c)
        {
            InitializeComponent();

            bcontrol = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(bcontrol);

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new AnimationItemList(bcontrol.AnimationList);
                datalist.ToList().ForEach(data => { if (data.NodeId == null) data.NodeId = Guid.NewGuid().ToString(); });
                gridControl.ItemsSource = datalist;
            };
        }
        #endregion

        #region Methods
        private void grid_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            e.Result = Comparer<int>.Default.Compare(e.ListSourceRowIndex1,
                e.ListSourceRowIndex2);

            e.Handled = true;
        }
        readonly List<ComboBoxEdit> listStretchFilled = new List<ComboBoxEdit>();
        public readonly List<String> stretches = Enum.GetNames(typeof(Stretch)).ToList();

        private void STRETCH_Editor_DropDownOpened(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listStretchFilled.Contains(combo))
                return;
            listStretchFilled.Add(combo);
            combo.ItemsSource = stretches;
        }
        private void ButtonImageClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (gridControl.SelectedItem as AnimationItem).BackImage = null;
                (gridControl.SelectedItem as AnimationItem).BackImageList.Clear();
                gridControl.RefreshData();
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }
        }
        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {

                UriImageListEditor urieditor = new UriImageListEditor((gridControl.SelectedItem as AnimationItem).BackImageList, Document);
                if (urieditor == null)
                    return;

                var Dialog = new GeneralDialogContent(urieditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.UriImageEditor,
                    HelpLink = "UriImageEditor"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (urieditor.DataContext != null && (urieditor.DataContext as BackImageItemList) != null)
                {
                    Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                    {
                        (gridControl.SelectedItem as AnimationItem).BackImageList = new BackImageItemList((urieditor.DataContext as BackImageItemList));
                        if ((gridControl.SelectedItem as AnimationItem).BackImageList.Count == 0)
                            (gridControl.SelectedItem as AnimationItem).BackImage = null;
                        else
                            (gridControl.SelectedItem as AnimationItem).BackImage = (gridControl.SelectedItem as AnimationItem).BackImageList[0].Value;

                        gridControl.RefreshData();
                        tableView.FocusedRowHandle = focusedrow;
                        ApplyChanges();
                    });
                }
            }
        }

        void OnBrushSelected(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void DlgButtonClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (gridControl.SelectedItem as AnimationItem).Text = string.Empty;
                gridControl.RefreshData(); 
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
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

                var Dialog = new GeneralDialogContent(stringEditor)
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
                    Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                    {
                        (gridControl.SelectedItem as AnimationItem).Text = new String((stringEditor.DataContext as String).ToArray());
                        gridControl.RefreshData();
                        tableView.FocusedRowHandle = focusedrow;
                        ApplyChanges();
                    });
                }
            }
        }

        readonly List<ComboBoxEdit> list = new List<ComboBoxEdit>();
        public readonly List<String> animations = Enum.GetNames(typeof(AnimationType)).ToList();
        private void Editor_DropDownOpened(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || list.Contains(combo))
                return;
            list.Add(combo);
            combo.ItemsSource = animations;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var _bcolor = RandomColor();
            //var _fcolor = new SolidColorBrush(Colors.White);
            var _fcolor = bcontrol is AnimatedText ? (bcontrol as AnimatedText).ControlForeground : (bcontrol as FullAnimatedText).Foreground;

            var list = (from c in datalist select c.Value).ToList().OrderByDescending(x => x);
            double _value = list.DefaultIfEmpty(-1).FirstOrDefault() + 1;

            //if (datalist.Count > 0)
            //    _value = (from c in datalist select c.Value).ToList().OrderByDescending(x => x).DefaultIfEmpty(0).LastOrDefault();

            while ((from m in datalist where m.Background == _bcolor select m).FirstOrDefault() != null)
            {
                _bcolor = RandomColor();
            }

            var newData = new AnimationItem(){ Value = _value, Background = _bcolor, Foreground = _fcolor, AnimationTime = 0.0, Animation = AnimationType.None,
                NodeId = Guid.NewGuid().ToString()
            };

            InsertRow(tableView.FocusedRowHandle, newData);

            //if (bcontrol != null)
            //    bcontrol.AnimationList = datalist;

            ApplyChanges();
        }

        Random randonGen = new Random();
        SolidColorBrush RandomColor()
        {
            SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb((byte)randonGen.Next(255), (byte)randonGen.Next(255), (byte)randonGen.Next(255)));
            return randomColor;
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
            if (listIndex >= 0)
            {
                //datalist.RemoveAt(listIndex);
                datalist.Remove((gridControl.SelectedItem as AnimationItem));
                ApplyChanges();
            }
        }

        private void InsertRow(int rowHandle, AnimationItem data)
        {
            //int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            //if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;

            //listIndex = -1;
            int listIndex = datalist.Count - 1;
            datalist.Insert(listIndex + 1, data);
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            //moveDown.IsEnabled = datalist != null && listIndex >= 0 && listIndex < datalist.Count - 1;
            //moveUp.IsEnabled = datalist != null && listIndex > 0;
            btnDelete.IsEnabled = listIndex >= 0;
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                bcontrol.AnimationList = datalist;
                bcontrol.UpdateBackImage();
            }
        }
        #endregion

        private void OnApplyChanges(object sender, EventArgs e)
        {
            int focusedrow = GetMasterFocusedRowHandle();
            gridControl.RefreshRow(focusedrow);
            ApplyChanges();
        }

        int GetMasterFocusedRowHandle()
        {
            var view = gridControl.View.FocusedView;
            return view == tableView ? tableView.FocusedRowHandle : (view.DataControl as GridControl).GetMasterRowHandle();
        }

        private void tableView_ValidateRow(object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e)
        {
            var _value = ((AnimationItem)e.Row).Value;
            var _animation = ((AnimationItem)e.Row).AnimationTime;
            e.IsValid = _value != Double.NaN && _animation != Double.NaN;
            ApplyChanges();
        }

        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void GridColumn_Validate(object sender, GridCellValidationEventArgs e)
        {
            bool _thvalue = (bool)((DataRowView)e.Row)["Value"];
            bool _anvalue = (bool)((DataRowView)e.Row)["AnimationTime"];
            if (_thvalue)
            {
                try
                {
                    double _value = Convert.ToDouble(e.Value);
                }
                catch
                {
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = string.Format(Properties.Resources.ValueError);
                }
            }
            if (_anvalue)
            {
                try
                {
                    double _value = Convert.ToDouble(e.Value);
                }
                catch
                {
                    e.IsValid = false;
                    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                    e.ErrorContent = string.Format(Properties.Resources.ValueError);
                }
            }
        }

        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!(sender as UIElement).IsKeyboardFocusWithin)
                return;

            ApplyChanges();
        }

        private void PART_Editor_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        void UpdateGrid()
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex<datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
                ApplyChanges();
            }
        }
    }
}
