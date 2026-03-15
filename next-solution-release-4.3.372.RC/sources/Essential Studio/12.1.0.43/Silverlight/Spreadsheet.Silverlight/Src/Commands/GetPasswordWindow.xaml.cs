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
    public partial class GetPasswordWindow : WindowControl, INotifyPropertyChanged
    {
        public GetPasswordWindow()
        {
            InitializeComponent();
            Icon = null;
            IconSize = new Size(0, 0);
            Description = SpreadsheetResourceWrapper.ProtectWorkbookDescriptionText;
            Title = SpreadsheetResourceWrapper.Password;
            //this.Loaded += (s, e) =>
            //{
            //    Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
            //};

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.Enter)
                OnOkButtonClick(this, new RoutedEventArgs());

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
        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                string[] split = _text.Split('\\');
                FileName = split[split.Count() - 1] + " is Protected";
                this.OnPropertyChanged("Text");
            }
        }

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                this.OnPropertyChanged("FileName");
            }
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

        internal bool CancelPassword;

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {

#if SILVERLIGHT
            AssociatedSpreadsheet.ExcelProperties.SetExcelEngine(AssociatedSpreadsheet.ExcelProperties.wbs,string.Empty, passwordBox.Password);
#endif
            Close();

         

        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            CancelPassword = true;
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
