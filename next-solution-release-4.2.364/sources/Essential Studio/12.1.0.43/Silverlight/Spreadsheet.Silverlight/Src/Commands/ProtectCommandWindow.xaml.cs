#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Grid.Converter;
using System;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class ProtectCommandWindow : WindowControl,INotifyPropertyChanged
    {
        public ProtectCommandWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            txtValue.Focus();
            this.Loaded += (s, e) =>
            {
                Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            };
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
