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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for ProtectCommandWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]    
    public partial class ProtectCommandWindow : Window,INotifyPropertyChanged
    {
        
        public ProtectCommandWindow()
        {
            InitializeComponent();
            this.txtValue.Focus();
           // this.PreviewKeyDown += new KeyEventHandler(ProtectCommandWindow_PreviewKeyDown);
           
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
            if(e.Key==Key.Enter)
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

       

        private string _descriptionText;
        public string DescriptionText
        {
            get { return _descriptionText; }
            set
            {
                _descriptionText = value;
                OnPropertyChanged("DescriptionText");
            }
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set { _associatedExcelEditor = value; }
        }

        private string _SheetName;
        public string SheetName
        {
            get { return _SheetName; }
            set { _SheetName = value; }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            IWorksheet worksheet;
            if (string.IsNullOrEmpty(SheetName))
                worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            else
                worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[SheetName];

            if (worksheet != null && worksheet.IsPasswordProtected)
                this.AssociatedSpreadsheet.UnProtectSheet(worksheet.Name, txtValue.Password);
            else
            {
                if (worksheet != null)
                    this.AssociatedSpreadsheet.ProtectSheet(worksheet.Name, txtValue.Password);
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
