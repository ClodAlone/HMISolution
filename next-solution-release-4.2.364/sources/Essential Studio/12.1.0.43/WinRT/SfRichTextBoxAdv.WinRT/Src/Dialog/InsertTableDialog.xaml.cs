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
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.UI.Xaml.Controls
{
    public partial class InsertTableDialog : WindowControl
    {
        RichTextBoxAdv richtextbox = null;

        public InsertTableDialog(RichTextBoxAdv richtext):this()
        {
            richtextbox = richtext;
            SkinManager.SetVisualStyle(this, Windows.Controls.Theming.VisualStyle.Office2010Blue);
        }

        public InsertTableDialog()
        {
            InitializeComponent();
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

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Execute();
            this.Close();
        }

        private void Execute()
        {
            richtextbox.InsertTableCommand.Execute(new int[] { Convert.ToInt16(rowUpDown.Value), Convert.ToInt16(columnUpDown.Value) });
            richtextbox.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            richtextbox.Focus();
        }


    }
}
