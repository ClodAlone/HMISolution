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
using System.ComponentModel;

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// Interaction logic for PageNumberToolTip.xaml
    /// </summary>
    public partial class PageNumberToolTip : UserControl
    {
        public PageNumberToolTip()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private string _content;

        public string USContent
        {
            get { return _content; }
            set { _content = value; }
        }

        internal void UpdatePageNumber(String pageNumber)
        {
            PageDisplay.Content = pageNumber;
        }
    }
}
