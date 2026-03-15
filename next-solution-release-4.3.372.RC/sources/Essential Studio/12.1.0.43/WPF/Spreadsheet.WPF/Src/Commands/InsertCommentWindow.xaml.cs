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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for InsertCommentWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class InsertCommentWindow : Window, INotifyPropertyChanged
    {
         public InsertCommentWindow()
         {
             InitializeComponent();
             this.CommentTextBox.Focus();
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

            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
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
            Close();
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