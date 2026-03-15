#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for GroupbyWindow.xaml
    /// </summary>
    
    [DesignTimeVisible(false)]
    public partial class GroupbyWindow : WindowControl
    {
        public GroupbyWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
           
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
                if(NeedToGroup)
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Group(ExcelGroupBy.ByRows);
                else
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByRows);

                if (this.AssociatedSpreadsheet.OutlineRowCount != (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel)
                    this.AssociatedSpreadsheet.OutlineRowCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel;
                else
                    this.AssociatedSpreadsheet.RefreshRowGroupPanel();
            }
            else if (gridRange.IsCols)
            {
                if(NeedToGroup)
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Group(ExcelGroupBy.ByColumns);
                else
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByColumns);

                if (this.AssociatedSpreadsheet.OutlineColumnCount != (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel)
                    this.AssociatedSpreadsheet.OutlineColumnCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel;
                else
                    this.AssociatedSpreadsheet.RefreshColumnGroupPanel();
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
