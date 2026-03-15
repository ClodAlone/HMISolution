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
using Syncfusion.Windows.Reports.Designer.Editors;
using Syncfusion.Windows.Reports.Designer.Controls;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for LineStyle.xaml
    /// </summary>
    internal partial class LineStyle : UserControl
    {
        internal string LineColor { get; set; }
        internal string LineThickness { get; set; }
        public LineStyle()
        {
            InitializeComponent();
            this.LineColor = null;
            this.LineThickness = "1";          
        }

        public CustomUIEditorDropDown LineColors
        {
            get
            {
                return this.cpkr_Font;
            }

            set
            {
                this.cpkr_Font = value;
            }
        }

    }
}
