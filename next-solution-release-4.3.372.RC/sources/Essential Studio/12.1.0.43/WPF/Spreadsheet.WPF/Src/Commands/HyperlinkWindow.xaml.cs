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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for HyperlinkWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class HyperlinkWindow : Window, INotifyPropertyChanged
    {
        public HyperlinkWindow()
        {
            InitializeComponent();
            this.Title = SpreadsheetResourceWrapper.InsertHyperlink;
            this.DisplayTextBox.Focus();
            if (Application.Current != null)
            {
                mainWindow = Application.Current.MainWindow as Window;
                if (mainWindow != null)
                    this.Owner = mainWindow;
            }
            this.Loaded += (s, e) =>
            {
                Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
                if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle.CellValue != null)
                {
                    this.TextToDisplay = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.CellValue.ToString();
                }
                if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle.CellValue2 != null)
                {
                    this.Address = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.CellValue2.ToString();
                    //For numbers the horizontal alignment values are stored in the CellValue2
                    if (this.Address == "HAlignRight" || this.Address == "HAlignLeft" || this.Address == "HAlignCenter")
                        this.Address = string.Empty;
                }
                else
                    this.Address = string.Empty;
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (e.Key == Key.Enter)
            {
                this.address = AddressText.Text;
                OnOkButtonClick(this, new RoutedEventArgs());
            }

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
        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set
            {
                _associatedExcelEditor = value;
            }
        }

        private string textToDisplay;
        public string TextToDisplay
        {
            get { return textToDisplay; }
            set
            {
                textToDisplay = value;
                this.OnPropertyChanged("TextToDisplay");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                this.OnPropertyChanged("Address");
            }
        }

        string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                GridStyleInfo[] cellsInfo;
                IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                    {
                        string cell = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);

                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);

                        IHyperLink hyperlink = sheet.HyperLinks.Add(sheet.Range[cell]);
                        if (Regex.IsMatch(address, pattern))
                            hyperlink.Type = ExcelHyperLinkType.Url;
                        else
                            hyperlink.Type = ExcelHyperLinkType.Workbook;
                        hyperlink.Address = address;
                        if (string.IsNullOrEmpty(textToDisplay))
                            hyperlink.TextToDisplay = sheet.Name;
                        else
                            hyperlink.TextToDisplay = textToDisplay;
                        for (int row = item.Top; row <= item.Bottom; row++)
                        {
                            for (int col = item.Left; col <= item.Right; col++)
                            {
                                var range = sheet.Range[row, col];
                                var style = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel[row, col];
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                                ExcelGridModelImportExtensions.CopyHyperlinkToCell(AssociatedSpreadsheet.ExcelProperties.WorkBook, sheet, range, style);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                            }
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual();
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                    var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                    string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cell(row, col));
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(GridRangeInfo.Cell(row, col), cellsInfo, Styles.StyleModifyType.Copy);
                    IHyperLink hyperlink = sheet.HyperLinks.Add(sheet.Range[cell]);
                    if (Regex.IsMatch(address, pattern))
                        hyperlink.Type = ExcelHyperLinkType.Url;
                    else
                        hyperlink.Type = ExcelHyperLinkType.Workbook;
                    hyperlink.Address = address;
                    if (string.IsNullOrEmpty(textToDisplay))
                        hyperlink.TextToDisplay = sheet.Name + "!" + cell;
                    else
                        hyperlink.TextToDisplay = textToDisplay;
                    var range = sheet.Range[row, col];
                    var style = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel[row, col];
                    AssociatedSpreadsheet.GridProperties.CurrentCellStyle = style;                    
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                    ExcelGridModelImportExtensions.CopyHyperlinkToCell(AssociatedSpreadsheet.ExcelProperties.WorkBook, sheet, range, style);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                    AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.InvalidateCell(GridRangeInfo.Cell(row,col));
                }
                AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.InvalidateVisual(true);
                AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.CurrentCell.Deactivate();
            }
            catch (Exception)
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnteredInvalidData);
            }

            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
