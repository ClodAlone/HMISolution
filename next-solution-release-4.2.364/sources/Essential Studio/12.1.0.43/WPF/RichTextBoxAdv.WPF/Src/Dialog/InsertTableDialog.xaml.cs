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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Interaction logic for InsertTableDialog.xaml
    /// </summary>
    public partial class InsertTableDialog : Window
    {
        RichTextBoxAdv richtextbox = null;

        public InsertTableDialog()
        {
            InitializeComponent();
        }

        public InsertTableDialog(RichTextBoxAdv richtext)
            : this()
        {
            richtextbox = richtext;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                if (e.Key == Key.Enter)
                {
                    Execute();
                }
                this.Close();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Execute();
            this.Close();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowIcon.SetNull(this);
        }

        private void Execute()
        {
            richtextbox.InsertTableInBlocks(Convert.ToInt16(row.Value), Convert.ToInt16(column.Value));
            richtextbox.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            richtextbox.Focus();
        }

    }
}
