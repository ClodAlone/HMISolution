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
using Syncfusion.XlsIO;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for UnhideWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class UnhideWindow : Window, INotifyPropertyChanged
    {
        public UnhideWindow()
        {
            InitializeComponent();
            Loaded += UnhideWindowLoaded;
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
        void UnhideWindowLoaded(object sender, RoutedEventArgs e)
        {
            Description = SpreadsheetResourceWrapper.UnhideSheetDescription;
            Title = SpreadsheetResourceWrapper.Unhide;
            HiddenSheet = new List<string>();
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
                OnPropertyChanged("HiddenSheet");
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
