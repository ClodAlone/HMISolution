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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class UnprotectWorkbookWindow : WindowControl, INotifyPropertyChanged
    {
        public UnprotectWorkbookWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            this.Loaded += (s, e) =>
            {
                Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());
            base.OnKeyDown(e);
        }

        private SpreadsheetControl _associatedExcelEditor;
        public SpreadsheetControl AssociatedSpreadsheet
        {
            get { return _associatedExcelEditor; }
            set { _associatedExcelEditor = value; }
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

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IWorkbook workbook = AssociatedSpreadsheet.ExcelProperties.WorkBook;
                workbook.Unprotect(txtValue.Password);
                this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected = false;
            }
            catch (Exception)
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_InCorrectPassword, SpreadsheetResourceWrapper.ValidationMessage_Error, MessageBoxButton.OK);
            }
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
