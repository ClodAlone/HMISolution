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
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportCode.xaml
    /// </summary>
    internal partial class ReportCode:UserControl
    {
        #region constructor

        public ReportCode()
        {
            InitializeComponent();
        }

        public ReportCode(string ReportCode)
        {
            InitializeComponent();
            Run run = new Run(ReportCode);
            Paragraph para = new Paragraph();
            para.Inlines.Add(run);
            Richtxt_CustomCode.Document.Blocks.Clear();
            Richtxt_CustomCode.Document.Blocks.Add(para);
        }
        
        #endregion
    }
}
