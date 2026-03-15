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

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    /// <summary>
    /// Interaction logic for ResizingHelpingLine.xaml
    /// </summary>
    internal partial class ResizingHelpingLine : UserControl
    {
        public ResizingHelpingLine()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                return this.InnerTextblock.Text;
            }
            set
            {
                this.InnerTextblock.Text = value;
            }
        }
        public double X1
        {
            get
            {
                return this.InnerLine.X1;
            }
            set
            {
                this.InnerLine.X1 = value;
            }
        }
        public double X2
        {
            get
            {
                return this.InnerLine.X2;
            }
            set
            {
                this.InnerLine.X2 = value;
            }
        }
        public double Y1
        {
            get
            {
                return this.InnerLine.Y1;
            }
            set
            {
                this.InnerLine.Y1 = value;
            }
        }
        public double Y2
        {
            get
            {
                return this.InnerLine.Y2;
            }
            set
            {
                this.InnerLine.Y2 = value;
            }
        }

    }
}
