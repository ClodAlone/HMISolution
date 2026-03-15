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
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using Syncfusion.Windows.Controls.Grid;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    
    /// <summary>
    /// Interaction logic for FormatAsTableWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class FormatAsTableWindow : Window, INotifyPropertyChanged
    {
        public FormatAsTableWindow()
        {
            InitializeComponent();
            this.txtValue.Focus();
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

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        private TableBuiltInStyles _tableStyle;
        public TableBuiltInStyles TableStyle
        {
            get { return _tableStyle; }
            set
            {
                _tableStyle = value;
                OnPropertyChanged("TableStyle");
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
            bool canClose = false;
            if (Text.Contains(":"))
            {
                GridStyleInfo[] cellsInfo = new GridStyleInfo[] { };
                try
                {
                    IWorksheet currentWorksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[
                        AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                    Syncfusion.Windows.Controls.Grid.GridRangeInfo range = GridExcelHelper.ConvertExcelRangeToGridRange(Text);
                    IListObject table1 = currentWorksheet.ListObjects.Create("Table1", currentWorksheet[Text]);
                    table1.BuiltInTableStyle = TableStyle;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(range);
                    if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetTableFormatCommand tableFormatCmd = new SpreadsheetTableFormatCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, range, new Grid.GridStyleInfo[] { }, Styles.StyleModifyType.Copy, table1);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(tableFormatCmd);
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                    canClose = true;
                }
                catch (Exception)
                {
                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnterValidRange, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                    Show();
                }
            }
            else
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_EnterValidRange, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                Show();
            }
            if (canClose)
                Close();
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
