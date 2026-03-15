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
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    [DesignTimeVisible(false)]
    public partial class ProtectWorkbookWindow : WindowControl,INotifyPropertyChanged
    {
        public ProtectWorkbookWindow()
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
            get 
            {
                if (_text != null)
                    return _text;
                else
                    return string.Empty;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        private bool _isStructure = true;
        public bool IsStructure 
        {
            get { return _isStructure; }
            set
            {
                _isStructure = value;
                OnPropertyChanged("IsStructure");
            }
        }

        private bool _isWindows;
        public bool IsWindows
        {
            get { return _isWindows; }
            set
            {
                _isWindows = value;
                OnPropertyChanged("IsWindows");
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

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            IWorkbook workbook = AssociatedSpreadsheet.ExcelProperties.WorkBook;
            if (!workbook.IsWindowProtection)
                workbook.Protect(IsWindows, IsStructure, Text);
            this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected = true;
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
