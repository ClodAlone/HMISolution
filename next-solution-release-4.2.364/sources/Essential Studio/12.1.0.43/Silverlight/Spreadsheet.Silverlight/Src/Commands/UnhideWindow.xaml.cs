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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class UnhideWindow : WindowControl, INotifyPropertyChanged 
    {
        public UnhideWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            Loaded += UnhideWindowLoaded;
        }

        void UnhideWindowLoaded(object sender, RoutedEventArgs e)
        {
            Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            Description = SpreadsheetResourceWrapper.UnhideSheetDescription;
            Title = SpreadsheetResourceWrapper.Unhide;
            HiddenSheet=new List<string>();
            var workbook = AssociatedSpreadsheet.ExcelProperties.WorkBook;
            foreach (IWorksheet sheet in workbook.Worksheets)
                if (sheet.Visibility == WorksheetVisibility.Hidden)
                    HiddenSheet.Add(sheet.Name);
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

        private List<string> _hiddenSheet;
        public List<string> HiddenSheet
        {
            get { return _hiddenSheet; }
            set
            {
                _hiddenSheet = value;
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

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in sheetListBox.SelectedItems)
            {
                AssociatedSpreadsheet.UnHideSheet(item.ToString());
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
