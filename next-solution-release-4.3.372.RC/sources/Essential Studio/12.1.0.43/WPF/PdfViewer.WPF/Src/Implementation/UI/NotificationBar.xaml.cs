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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.PdfViewer.Base;

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// Interaction logic for NotificationBar.xaml
    /// </summary>
    public partial class NotificationBar : UserControl
    {
        internal PdfViewerExceptions exception = new PdfViewerExceptions();
        public NotificationBar()
        {
            InitializeComponent();
            LinearGradientBrush brush = new LinearGradientBrush(Color.FromRgb(207, 223, 244), Color.FromRgb(180, 203, 232), 90.0);
            this.Background = brush;
            //
            //label1
            //
            label1.HorizontalAlignment = HorizontalAlignment.Left;
            label1.FontSize = 15F;
            label1.FontFamily = new FontFamily("Segoe UI");
           

            button1.Visibility = Visibility.Visible;
            button1.BorderBrush = brush;
            button1.Background = Brushes.Transparent;
            button1.Click+=new RoutedEventHandler(button1_Click);
            
            LinkLabel.MouseEnter += new MouseEventHandler(LinkLabel_MouseEnter);
            this.Visibility = Visibility.Visible;
        }

        void LinkLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(exception.Exceptions.ToString());
            exception.Exceptions.Length = 0;
        }

        void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            exception.Exceptions.Length = 0;            
        }
    }
}
