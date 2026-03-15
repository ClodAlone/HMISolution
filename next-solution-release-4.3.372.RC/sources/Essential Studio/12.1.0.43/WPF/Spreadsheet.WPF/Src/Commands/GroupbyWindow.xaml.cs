#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for GroupbyWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class GroupbyWindow : Window
    {
        public GroupbyWindow()
        {
            InitializeComponent();

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

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();

            GridRangeInfo gridRange = this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[0];
            string excelRange;

            if (Rows)
                gridRange = GridRangeInfo.Rows(gridRange.Top > 1 ? gridRange.Top : 1, gridRange.Bottom);
            else
                gridRange = GridRangeInfo.Cols(gridRange.Left > 1 ? gridRange.Left : 1, gridRange.Right);

            excelRange = gridRange.ConvertGridRangeToExcelRange(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);

            if (gridRange.IsRows)
            {
                if (NeedToGroup)
                {
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Group(ExcelGroupBy.ByRows);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand groupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                                                     this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByRows, false);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(groupCommand);
                    }
                }
                else
                {
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByRows);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand ungroupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                                                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByRows, true);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(ungroupCommand);
                    }
                }
                this.AssociatedSpreadsheet.OutlineRowCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel;

            }
            else if (gridRange.IsCols)
            {
                if (NeedToGroup)
                {
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Group(ExcelGroupBy.ByColumns);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand groupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                                                     this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByColumns, false);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(groupCommand);
                    }
                }
                else
                {
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByColumns);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand ungroupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                                                        this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByColumns, true);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(ungroupCommand);
                    }
                }
                this.AssociatedSpreadsheet.OutlineColumnCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel;
            }

        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
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
       
        private bool rows=true;
        public bool Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        private bool columns;
        public bool Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private bool needToGroup;
        public bool NeedToGroup
        {
            get { return needToGroup; }
            set { needToGroup = value; }
        }

    }
}
