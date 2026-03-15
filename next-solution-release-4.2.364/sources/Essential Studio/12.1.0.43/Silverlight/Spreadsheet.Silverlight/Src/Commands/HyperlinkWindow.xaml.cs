#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class HyperlinkWindow : WindowControl, INotifyPropertyChanged
    {
        public HyperlinkWindow()
        {
            InitializeComponent();
            this.Title = SpreadsheetResourceWrapper.InsertHyperlink;
            this.DisplayText.Focus();            
            Icon = null;           
            IconSize = new Size(0, 0);
            this.Loaded += (s, e) =>
            {
                
                Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
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
            get 
            {
                if (textToDisplay != null)
                    return textToDisplay;
                else
                    return string.Empty;
            }
            set
            {
                textToDisplay = value;
                this.OnPropertyChanged("TextToDisplay");
            }
        }

        private string address;
        public string Address
        {
            get 
            {
                if (address != null)
                    return address;
                else
                    return string.Empty;
            }
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
                if (Address != string.Empty)
                {
                    IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                    if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                    {
                        foreach (var item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
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

                        cellsInfo = new GridStyleInfo[] { };                       
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
                        AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.InvalidateCell(GridRangeInfo.Cell(row, col));
                    }
                    AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.InvalidateVisual(true);
                    AssociatedSpreadsheet.GridProperties.SpreadsheetGrid.CurrentCell.Deactivate();
                }
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
            {
                this.address = AddressText.Text;
                OnOkButtonClick(this, new RoutedEventArgs());
            }
            base.OnKeyDown(e);
        }


        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
