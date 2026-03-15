using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using OPCUAViewModel;
using Utilities.WPF;
using Utilities;
using System.ComponentModel;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;

namespace AlarmWindow.Controls
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
        readonly BannerAlarmWindow bannerAlarmControl;
        readonly GridAlarmWindow gridAlarmControl;
        ThresholdList datalist;
        Window parentWindow;

        DevExpress.Xpf.Editors.PopupBaseEdit popupEdit;
        bool bMoreColorsOpened;

        bool bLoaded;
        #endregion

        #region Constructors
        public SmartControl(BannerAlarmWindow c)
        {
            InitializeComponent();

            bannerAlarmControl = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(bannerAlarmControl);

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new ThresholdList(bannerAlarmControl.Thresholds);
                datalist.OrderBy(t => t.ThresholdValue);
                gridControl.ItemsSource = datalist;
            };
        }

        public SmartControl(GridAlarmWindow c)
        {
            InitializeComponent();

            gridAlarmControl = c;
            DataContext = c;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(gridAlarmControl);

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                datalist = new ThresholdList(gridAlarmControl.Thresholds);
                datalist.OrderBy(t => t.ThresholdValue);
                gridControl.ItemsSource = datalist;
            };
        }
        #endregion

        #region Methods
        private void OnAdd(object sender, RoutedEventArgs e)
        {
            var _color = RandomColor();
            double _value = 0;
            if (datalist.Count > 0)
                _value = datalist.OrderByDescending(m => m.ThresholdValue).FirstOrDefault().ThresholdValue + 20;

            //var newData = new ThresholdSettings { ThresholdColor = _color, ThresholdForeColor = InvertColour(_color), ThresholdValue = _value };
            var newData = new ThresholdSettings { ThresholdColor = _color, ThresholdForeColor = Colors.Black, ThresholdValue = _value };
            datalist.Add(newData);
            ApplyChanges();
        }


        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        Color RandomColor()
        {
            Random randonGen = new Random();
            Color randomColor = Color.FromRgb((byte)randonGen.Next(255), (byte)randonGen.Next(255), (byte)randonGen.Next(255));

            while (datalist.Where(m => m.ThresholdColor == randomColor).FirstOrDefault() != null)
            {
                randomColor = Color.FromRgb((byte)randonGen.Next(255), (byte)randonGen.Next(255), (byte)randonGen.Next(255));
            }

            return randomColor;
        }

        Color InvertColour(Color ColourToInvert)
        {
            return Color.FromRgb((byte)~ColourToInvert.R, (byte)~ColourToInvert.G, (byte)~ColourToInvert.B);
        }

        private void DeleteRow(int rowHandle)
        {
            if (datalist.Count <= 1)
                return;

            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
            ApplyChanges();
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                if (bannerAlarmControl != null)
                    bannerAlarmControl.Thresholds = datalist;
                if (gridAlarmControl != null)
                    gridAlarmControl.Thresholds = datalist;
            }
        }
        #endregion

        private void PART_Editor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var popup = sender as DevExpress.Xpf.Editors.PopupBaseEdit;
            if (popup != null)
                popup.ShowPopup();
        }

        //private void popup_Closing(object sender, ClosingPopupEventArgs e)
        //{
        //    tableView.HideEditor();
        //}

        private void tableView_ShowingEditor(object sender, DevExpress.Xpf.Grid.ShowingEditorEventArgs e)
        {
            if (e.Column.Name == "Result") e.Cancel = true;
        }

        private void tableView_ValidateRow(object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e)
        {
            var _value = ((ThresholdSettings)e.Row).ThresholdValue;
            e.IsValid = _value != Double.NaN;
        }

        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }
        
        private void OnBrushSelected(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                ApplyChanges();
            }

            if (bMoreColorsOpened)
            {
                bMoreColorsOpened = false;
                popupEdit?.ClosePopup();
            }
        }

        private void OnMoreColorsClicked(object sender, RoutedEventArgs e)
        {
            bMoreColorsOpened = true;
        }

        private void Popup_Closing(object sender, DevExpress.Xpf.Editors.ClosingPopupEventArgs e)
        {
            if (bMoreColorsOpened)
            {
                popupEdit = sender as DevExpress.Xpf.Editors.PopupBaseEdit;
                e.Handled = e.Cancel = true;
            }
        }
    }
}
