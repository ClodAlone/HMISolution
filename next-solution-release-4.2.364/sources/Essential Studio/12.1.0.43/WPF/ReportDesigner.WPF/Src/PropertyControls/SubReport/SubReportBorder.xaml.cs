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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for SubReportBorder.xaml
    /// </summary>
    internal partial class SubReportBorder : UserControl
    {
        public SubReportBorder()
        {
            InitializeComponent();
            
            //DrawLine();
        }

        #region Public Properties

        public ExpressionComboBox BorderStyle
        {
            get
            {
                return this.cmb_BorderStyle;
            }
            set
            {
                this.cmb_BorderStyle = value;
            }
        }

        public ExpressionComboBox BorderWidth
        {
            get
            {
                return this.updwn_BorderWidth;
            }
            set
            {
                this.updwn_BorderWidth = value;
            }
        }

        public CustomUIEditorDropDown BorderColor
        {
            get
            {
                return this.cpkr_BorderColor;
            }
            set
            {
                this.cpkr_BorderColor = value;
            }
        }
         

        #endregion

        //private void DrawLine()
        //{
        //    Line line = new Line();
        //    line.Stroke = Brushes.Black;
        //    line.StrokeThickness = 1;
        //    //for top row drawing
        //    line.X1 = 20;
        //    line.Y1 = 20;
        //    line.X2 = 115;
        //    line.Y2 = 20;

        //    // for left row drawing
        //    line.X1 = 20;
        //    line.Y1 = 20;
        //    line.X2 = 20;
        //    line.Y2 = 76;

        //    // for bottom row drawing
        //    line.X1 = 20;
        //    line.Y1 = 76;
        //    line.X2 = 115;
        //    line.Y2 = 76;

        //    // for bottom row drawing
        //    line.X1 = 115;
        //    line.Y1 = 20;
        //    line.X2 = 115;
        //    line.Y2 = 76;
        //    Grid.SetColumn(line, 0);
        //    Grid.SetRow(line, 0);
        //    Grid.SetRowSpan(line, 3);
        //    Grid.SetColumnSpan(line, 3);
        //    this.grd_Preview.Children.Add(line);
        //}
    }
}
