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
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    /// <summary>
    /// Interaction logic for GetPasswordWindow.xaml
    /// </summary>
    /// 
    [DesignTimeVisible(false)]
    public partial class GetPasswordWindow : Window, INotifyPropertyChanged
    {
        
        public GetPasswordWindow()
        {
            InitializeComponent();
            Title = SpreadsheetResourceWrapper.Password;
            passwordBox.Focus();
            if (Application.Current != null)
            {
                mainWindow = Application.Current.MainWindow as Window;
                if (mainWindow != null)
                    this.Owner = mainWindow;
            }
            //this.Loaded += (s, e) =>
            //{
            //    Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, this.AssociatedSpreadsheet.VisualStyle);
               
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        internal Window mainWindow
        {
            get;
            private set;
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
              string[] split=_text.Split('\\');
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

        internal bool CancelPassword;

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {            
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
