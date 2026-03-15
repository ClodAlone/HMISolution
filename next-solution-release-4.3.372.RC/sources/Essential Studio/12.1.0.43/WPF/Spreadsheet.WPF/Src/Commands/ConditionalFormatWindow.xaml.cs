#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Spreadsheet;
using System.Drawing;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for ConditionalFormatWindow.xaml
    /// </summary>   
    [DesignTimeVisible(false)]
    public partial class ConditionalFormatWindow : Window, INotifyPropertyChanged
    {
        public ConditionalFormatWindow()
        {
            InitializeComponent();
            this.txtValue.Focus();
            Loaded += ConditionalFormatWindowLoaded;
            if (Application.Current != null)
            {
                mainWindow = Application.Current.MainWindow as Window;
                if (mainWindow != null)
                    this.Owner = mainWindow;
            }

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
                this.Close();
            if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        internal Window mainWindow
        {
            get;
            private set;
        }

        public void ConditionalFormatWindowLoaded(object sender, RoutedEventArgs e)
        {
            Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);

            if (Operator != ExcelComparisonOperator.Between && Operator != ExcelComparisonOperator.NotBetween)
                txtValue.Width = 237;
            else
            {
                txtValue.Width = 100;
                txtValue2.Width = 100;
            }

            switch (Operator)
            {
                case ExcelComparisonOperator.GreaterOrEqual:
                case ExcelComparisonOperator.Greater:
                    Description = SpreadsheetResourceWrapper.GreaterThanWindowDescription;
                    Title = SpreadsheetResourceWrapper.GreaterThanWindowTitle;
                    AdditionalTextboxVisibility = Visibility.Collapsed;
                    break;
                case ExcelComparisonOperator.LessOrEqual:
                case ExcelComparisonOperator.Less:
                    Description = SpreadsheetResourceWrapper.LessThanWindowDescription;
                    Title = SpreadsheetResourceWrapper.LessThanWindowTitle;
                    AdditionalTextboxVisibility = Visibility.Collapsed;
                    break;
                case ExcelComparisonOperator.Between:
                    Description = SpreadsheetResourceWrapper.BetweenWindowDescription;
                    Title = SpreadsheetResourceWrapper.BetweenWindowTitle;
                    AdditionalTextboxVisibility = Visibility.Visible;
                    break;
                case ExcelComparisonOperator.NotBetween:
                    Description = SpreadsheetResourceWrapper.NotBetweenWindowDescription;
                    Title = SpreadsheetResourceWrapper.NotBetweenWindowTitle;
                    AdditionalTextboxVisibility = Visibility.Visible;
                    break;
                case ExcelComparisonOperator.Equal:
                    Description = SpreadsheetResourceWrapper.EqualToWindowDescription;
                    Title = SpreadsheetResourceWrapper.EqualToWindowTitle;
                    AdditionalTextboxVisibility = Visibility.Collapsed;
                    break;
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private Visibility _additionalTextboxVisibility;
        public Visibility AdditionalTextboxVisibility
        {
            get { return _additionalTextboxVisibility; }
            set { _additionalTextboxVisibility = value; }
        }

        private ExcelComparisonOperator _operator;
        public ExcelComparisonOperator Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                OnPropertyChanged("Operator");
            }
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set { _associatedExcelEditor = value; }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            string value = txtValue.Text;
            string value2 = txtValue2.Text;
#if !SyncfusionFramework3_5
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
#else
            if(string.IsNullOrEmpty(value))
#endif
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                return;
            }
#if !SyncfusionFramework3_5
            if (Operator == ExcelComparisonOperator.Between || Operator == ExcelComparisonOperator.NotBetween)

                if (string.IsNullOrEmpty(value2) || string.IsNullOrWhiteSpace(value2))
#else
            if (Operator == ExcelComparisonOperator.Between || Operator == ExcelComparisonOperator.NotBetween)

                if (string.IsNullOrEmpty(value2))
#endif
                {
                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                    return;
                }
            IWorksheet currentWorksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (AssociatedSpreadsheet.GridProperties.CurrentCell.HasCurrentCell && currentWorksheet != null)
            {
                GridStyleInfo[] cellsInfo = null;
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                    {
                        string cell = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        ApplyConditionalFormat(currentWorksheet, cell);
                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    var rowColumnIndex = AssociatedSpreadsheet.GridProperties.CurrentCell.CellRowColumnIndex;
                    string cell = GridRangeInfo.GetAlphaLabel(rowColumnIndex.ColumnIndex) + rowColumnIndex.RowIndex;
                    ApplyConditionalFormat(currentWorksheet, cell);

                    GridRangeInfo rangeInfo = AssociatedSpreadsheet.GridProperties.CurrentCell.RangeInfo;
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo, Styles.StyleModifyType.Copy);

                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(
                        GridRangeInfo.Cell(rowColumnIndex.RowIndex, rowColumnIndex.ColumnIndex));
                }
            }
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            AssociatedSpreadsheet.GridProperties.CurrentCell.Deactivate();
            Close();
        }

        private void ApplyConditionalFormat(IWorksheet currentWorksheet, string cell)
        {
            string value = txtValue.Text;
            string value2 = txtValue2.Text;
            string style = comboCellStyle.SelectionBoxItem.ToString();
            IConditionalFormats condition = currentWorksheet.Range[cell].ConditionalFormats;
            IConditionalFormat condition1 = condition.AddCondition();
            if (Operator == ExcelComparisonOperator.Between || Operator == ExcelComparisonOperator.NotBetween)
            {
                condition1.FirstFormula = value;
                condition1.SecondFormula = value2;
            }
            else
                condition1.FirstFormula = value;
            condition1.Operator = Operator;
            condition1.FormatType = ExcelCFType.CellValue;
            if (style == Syncfusion_Speradsheet_Wpf.LightRedFillwithDarkRedText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 199, 206);
                condition1.FontColorRGB = Color.FromArgb(255, 156, 0, 6);
            }
            else if (style == Syncfusion_Speradsheet_Wpf.YellowFillwithDarkYellowText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 235, 156);
                condition1.FontColorRGB = Color.FromArgb(255, 156, 101, 0);
            }
            else if (style == Syncfusion_Speradsheet_Wpf.GreenFillwithDarkGreenText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 198, 239, 206);
                condition1.FontColorRGB = Color.FromArgb(255, 0, 97, 0);
            }
            else if (style == Syncfusion_Speradsheet_Wpf.LightRedFill)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 199, 206);
            }
            else if (style == Syncfusion_Speradsheet_Wpf.RedText)
            {
                condition1.FontColorRGB = Color.FromArgb(255, 156, 0, 6);
            }
            else if (style == Syncfusion_Speradsheet_Wpf.RedBorder)
            {
                condition1.LeftBorderStyle = ExcelLineStyle.Thin;
                condition1.LeftBorderColorRGB = Color.FromArgb(255, 156, 0, 6);
                condition1.RightBorderStyle = ExcelLineStyle.Thin;
                condition1.RightBorderColorRGB = Color.FromArgb(255, 156, 0, 6);
                condition1.TopBorderStyle = ExcelLineStyle.Thin;
                condition1.TopBorderColorRGB = Color.FromArgb(255, 156, 0, 6);
                condition1.BottomBorderStyle = ExcelLineStyle.Thin;
                condition1.BottomBorderColorRGB = Color.FromArgb(255, 156, 0, 6);
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

}
