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
using Syncfusion.Windows.Tools;


namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    /// <summary>
    /// Interaction logic for EncryptCommandWindow.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class EncryptCommandWindow : Window, INotifyPropertyChanged
    {
        public EncryptCommandWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            this.txtValue.Focus();
           // this.PreviewKeyDown += new KeyEventHandler(EncryptCommandWindow_PreviewKeyDown);
           // VisualStyle = VisualStyle.Windows7;
            Icon = null;
            txtValue.Password = this.Text;
           // IconSize = new Size(0, 0);
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
            AssociatedSpreadsheet.ExcelProperties.WorkBook.PasswordToOpen = txtValue.Password;
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
