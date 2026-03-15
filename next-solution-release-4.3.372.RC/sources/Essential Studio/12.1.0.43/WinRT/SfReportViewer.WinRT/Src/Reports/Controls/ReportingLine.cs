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
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingLine : ContentControl
    {
        Line line = null;

        internal LineModel LineModel { get; set; }

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        public ReportingLine(IReportItemModeler model)
        {
            this.Model = model;
            this.line = new Line();
            this.LineModel = this.Model as LineModel;
            this.IntializeLine();
            this.Content = this.line;
        }

        private void IntializeLine()
        {
            if (this.LineModel.Height == 0)
            {
                this.LineModel.Height = 1;
            }

            this.Height = this.LineModel.Height;
            this.Width = this.LineModel.Width;

            if (this.LineModel.PageInfo != null)
            {
                Canvas.SetLeft(this, this.LineModel.PageInfo.ActualLeft);
                Canvas.SetTop(this, this.LineModel.PageInfo.ActualTop);
            }
            else if (this.LineModel.PrintPageInfo != null)
            {
                Canvas.SetLeft(this, this.LineModel.PrintPageInfo.ActualLeft);
                Canvas.SetTop(this, this.LineModel.PrintPageInfo.ActualTop);
            }
            else if (this.LineModel.IsTablixChild)
            {
                Canvas.SetLeft(this, this.LineModel.Left);
                Canvas.SetTop(this, this.LineModel.Top);
            }
           

            this.line.X1 = this.LineModel.X1;
            this.line.Y1 = this.LineModel.Y1;
            this.line.X2 = this.LineModel.X2;
            this.line.Y2 = this.LineModel.Y2;
            this.line.Stroke = (Brush)new ReportingBrushConverter().ConvertFromString(this.LineModel.LineProperties.LineColor);

            if (this.LineModel.LineProperties != null && this.LineModel.LineProperties.LineWidth > this.LineModel.Height)
            {
                this.Height = this.LineModel.LineProperties.LineWidth;
                this.line.StrokeThickness = this.LineModel.LineProperties.LineWidth;
            }

            DoubleCollection dashesstyle = new DoubleCollection();

            if (this.LineModel.LineProperties.LineStyle == DOM.LineStyle.Dashed)
            {
                dashesstyle.Add(3);
                dashesstyle.Add(1);
                this.line.StrokeDashArray = dashesstyle;
            }
            else if (this.LineModel.LineProperties.LineStyle == DOM.LineStyle.Dotted)
            {
                dashesstyle.Add(1);
                dashesstyle.Add(1);
                this.line.StrokeDashArray = dashesstyle;
            }
        }
    }
}
