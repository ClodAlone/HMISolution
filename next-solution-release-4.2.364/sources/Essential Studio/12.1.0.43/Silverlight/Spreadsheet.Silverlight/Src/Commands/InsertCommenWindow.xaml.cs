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
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class InsertCommentWindow : WindowControl, INotifyPropertyChanged
    {
        public InsertCommentWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            CommentBox.Focus();
            this.Loaded += (s, e) =>
            {
                Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
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

        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                this.OnPropertyChanged("Author");
            }
        }

        private string text;
        public string Comment
        {
            get { return text; }
            set
            {
                text = value;
                this.OnPropertyChanged("Comment");
            }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            //if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
            //{
            //    foreach (var range in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
            //    {
            //        string cells = range.ConvertGridRangeToExcelRange();
            //        comments[cells].Text = Comment;
            //    }
            //}
            //else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            //{
            //    var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
            //    var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
            //    string cells = GridRangeInfo.GetAlphaLabel(col) + row;
            //    comments[cells].Text = Comment;
            //}
            var excelStyle = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName][AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex, AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex];
            if (excelStyle != null)
            {
                GridStyleInfo[] cellsInfo;

                excelStyle.AddComment().Text = Comment;
                GridRangeInfo range = excelStyle.ConvertExcelRangeToGridRange();
                cellsInfo = this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(range);
                this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(range, cellsInfo, Styles.StyleModifyType.Copy);
                this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex, AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex));
                this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual();
                this.AssociatedSpreadsheet.GridProperties.RefreshCurrentStyle();
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
                Comment = CommentBox.Text;
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
