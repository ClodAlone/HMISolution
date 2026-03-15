#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class ConditionalFormatWindow : WindowControl, INotifyPropertyChanged
    {
        public ConditionalFormatWindow()
        {
            InitializeComponent();
            txtValue.Focus();
            Loaded += ConditionalFormatWindowLoaded;
            Icon = null;
            IconSize = new Size(0, 0);
        }

        public void ConditionalFormatWindowLoaded(object sender, RoutedEventArgs e)
        {
            Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);

            if (Operator != ExcelComparisonOperator.Between && Operator!= ExcelComparisonOperator.NotBetween)
                txtValue.Width = 230;
            else
            {
                txtValue.Width = 90;
                txtValue2.Width = 90;
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
            if(string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                return;
            }
            if(Operator == ExcelComparisonOperator.Between || Operator == ExcelComparisonOperator.NotBetween)
                if (string.IsNullOrEmpty(value2) || string.IsNullOrWhiteSpace(value2))
                {
                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData, SpreadsheetResourceWrapper.MessageBoxCaption,
                                    MessageBoxButton.OK);
                    return;
                }
            IWorksheet currentWorksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (AssociatedSpreadsheet.GridProperties.CurrentCell.HasCurrentCell && currentWorksheet != null)
            {
                GridStyleInfo[] cellsInfo = null;
                if(AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    foreach (var item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
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
            if (style == Syncfusion_Spreadsheet_Silverlight.LightRedFillwithDarkRedText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 199, 206);
                condition1.FontColorRGB = Color.FromArgb(255, 156, 0, 6);
            }
            else if (style == Syncfusion_Spreadsheet_Silverlight.YellowFillwithDarkYellowText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 235, 156);
                condition1.FontColorRGB = Color.FromArgb(255, 156, 101, 0);
            }
            else if (style == Syncfusion_Spreadsheet_Silverlight.GreenFillwithDarkGreenText)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 198, 239, 206);
                condition1.FontColorRGB = Color.FromArgb(255, 0, 97, 0);
            }
            else if (style == Syncfusion_Spreadsheet_Silverlight.LightRedFill)
            {
                condition1.BackColorRGB = Color.FromArgb(255, 255, 199, 206);
            }
            else if (style == Syncfusion_Spreadsheet_Silverlight.RedText)
            {
                condition1.FontColorRGB = Color.FromArgb(255, 156, 0, 6);
            }
            else if (style == Syncfusion_Spreadsheet_Silverlight.RedBorder)
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
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
