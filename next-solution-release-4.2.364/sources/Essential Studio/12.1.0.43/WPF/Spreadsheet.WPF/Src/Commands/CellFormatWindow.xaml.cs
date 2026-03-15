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
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for CellFormatWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class CellFormatWindow : Window, INotifyPropertyChanged
    {
         public CellFormatWindow()
         {
             InitializeComponent();
             Loaded += CellFormatWindowLoaded;
             this.DoubleTextBox.Focus();
             if (Application.Current != null)
             {
                 mainWindow = Application.Current.MainWindow as Window;
                 if (mainWindow != null)
                     this.Owner = mainWindow;
             }
             this.Loaded += (s, e) =>
             {
                 Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
             };
         }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.Enter)
            {
                RowHeightColumnWithChangeAction();
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
        

        void CellFormatWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (CellFormat == CellFormat.RowHeight)
            {
                Title = SpreadsheetResourceWrapper.RowHeight;
                Description = SpreadsheetResourceWrapper.RowHeightDescription;
            }
            else if (CellFormat == CellFormat.ColumnWidth)
            {
                Title = SpreadsheetResourceWrapper.ColumnWidth;
                Description = SpreadsheetResourceWrapper.ColumnWidthDescription;
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

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set
            {
                _associatedExcelEditor = value;
                OnPropertyChanged("AssociatedSpreadsheet");
            }
        }

        private CellFormat _cellFormat;
        internal CellFormat CellFormat
        {
            get { return _cellFormat; }
            set
            {
                _cellFormat = value;
                OnPropertyChanged("CellFormat");
            }
        }

         void RowHeightColumnWithChangeAction()
        {
            var model = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel;
            if (CellFormat == CellFormat.RowHeight)
            {
                double height = Value;
                var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("RowHeight");
                    for (int i = 0; i < upper; i++)
                    {
                        var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                        item = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.ExpandSelectedCellsRange(item);
                        int Top = item.Top;
                        int Bottom = item.Bottom;
                        if (item.RangeType == GridRangeInfoType.Cols)
                        {
                            Top = 1;
                            Bottom = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowCount-1;
                        }
                        if (height > 0)
                        {
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                for (int row = Top; row <= Bottom; row++)
                                {

                                    if (model.RowHeights[row] > 0)
                                    {
                                        var cmd = new SpreadsheetSetRowSizeCommand(model, row, row, model.RowHeights[row]);
                                        model.CommandStack.Push(cmd);
                                    }
                                    else
                                    {
                                        var cmd = new SpreadsheetSetRowHideCommand(model, row, row, true);
                                        model.CommandStack.Push(cmd);
                                    }
                                }
                            }
                            model.CommandStack.SuspendUndo = true;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(Top, Bottom, false);
                            worksheet.SetRowHeightInPixels(Top, (Bottom +1) - Top, height);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetRange(Top, Bottom, height);
                            model.CommandStack.SuspendUndo = false;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(Top, Bottom));
                        }
                        else
                        {
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                var cmd =new SpreadsheetSetRowHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, Top, Bottom, false);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(cmd);
                            }
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(Top, Bottom, true);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(Top, Bottom));
                        }
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    int row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                    if (height > 0)
                    {
                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            if (model.RowHeights[row] > 0)
                            {
                                var cmd = new SpreadsheetSetRowSizeCommand(model, row, row, model.RowHeights[row]);
                                model.CommandStack.Push(cmd);
                            }
                            else
                            {
                                var cmd = new SpreadsheetSetRowHideCommand(model, row, row, true);
                                model.CommandStack.Push(cmd);
                            }
                        }
                        model.CommandStack.SuspendUndo = true;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(row, row, false);
                        worksheet.SetRowHeightInPixels(row,height);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetRange(row, row, height);
                        model.CommandStack.SuspendUndo = false;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                    }
                    else
                    {
                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            var cmd =new SpreadsheetSetRowHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, row, row, false);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(cmd);
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(row, row, true);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                    }
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            }
            else if (CellFormat == CellFormat.ColumnWidth)
            {
                int width = (int)Value;
                var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("ColumnWidth");
                    for (int i = 0; i < upper; i++)
                    {
                        var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                        item = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.ExpandSelectedCellsRange(item);
                        int Left = item.Left;
                        int Right = item.Right;
                        if (item.RangeType == GridRangeInfoType.Rows)
                        {
                            Left = 1;
                            Right = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnCount-1;
                        }
                        if (width > 0)
                        {
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                for (int col = Left; col <= Right; col++)
                                {
                                    if (model.ColumnWidths[col] > 0)
                                    {
                                        var cmd = new SpreadsheetSetColumnSizeCommand(model, col, col,model.ColumnWidths[col]);
                                        model.CommandStack.Push(cmd);
                                    }
                                    else
                                    {
                                        var cmd = new SpreadsheetSetColumnHideCommand(model, col, col, true);
                                        model.CommandStack.Push(cmd);
                                    }
                                }
                            }
                            model.CommandStack.SuspendUndo = true;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(Left, Right, false);
                            worksheet.SetColumnWidthInPixels(Left, (Right + 1) - Left, width);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetRange(Left, Right, width);
                            model.CommandStack.SuspendUndo = false;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cols(Left, Right));
                        }
                        else
                        {
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                var cmd =new SpreadsheetSetColumnHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, Left, Right, false);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(cmd);
                            }
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(Left, Right, true);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cols(Left, Right));
                        }
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    int col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                    if (width > 0)
                    {
                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            if (model.ColumnWidths[col] > 0)
                            {
                                var cmd = new SpreadsheetSetColumnSizeCommand(model, col, col, model.ColumnWidths[col]);
                                model.CommandStack.Push(cmd);
                            }
                            else
                            {
                                var cmd = new SpreadsheetSetColumnHideCommand(model, col, col, true);
                                model.CommandStack.Push(cmd);
                            }
                        }
                        model.CommandStack.SuspendUndo = true;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(col, col, false);
                        worksheet.SetColumnWidthInPixels(col, width);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetRange(col, col, width);
                        model.CommandStack.SuspendUndo = false;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(col));
                    }
                    else
                    {
                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            var cmd =new SpreadsheetSetColumnHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, col, col, false);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(cmd);
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(col, col, true);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(col));
                    }
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            }
            Close();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            RowHeightColumnWithChangeAction();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Window CurrentWindow = Window.GetWindow(this);
            CurrentWindow.Close();
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
