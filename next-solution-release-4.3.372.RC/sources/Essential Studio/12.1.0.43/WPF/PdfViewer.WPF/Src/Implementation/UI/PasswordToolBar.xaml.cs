#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.PdfViewer
{
    public partial class PasswordToolBar : Window
    {
        internal string Password = string.Empty;
        public PasswordToolBar()
        {
            InitializeComponent();
        }

        void cancelButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        void okButton_Click(object sender, RoutedEventArgs e)
        {
            Password = passwordTextBox.Password;
            this.DialogResult = true;
            this.Close();
        }
    }
}
