//-------------------------------------------------------------------------------------------------
// <copyright file="Line.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
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
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Internal;

namespace Syncfusion.RDL.Controls
{
    internal partial class ReportingLine
        : UserControl
    {
        bool isPrintMode = false;
        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {
                isPrintMode = value;
                if (isPrintMode)
                {
                    this.Margin = new Thickness(LineModel.PrintPageInfo.ActualLeft, LineModel.PrintPageInfo.ActualTop, 0, 0);
                }
                else
                {
                    this.Margin = new Thickness(LineModel.PageInfo.ActualLeft, LineModel.PageInfo.ActualTop, 0, 0);
                }
            }
        }

        internal LineModel LineModel { get; set; }

        internal LinePropertiesExpVal LineExpVal { get; set; }

        #region Members

        private System.Windows.Controls.Grid lineContainer = new System.Windows.Controls.Grid();
        private Line line = new Line();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingLine"/> class.
        /// </summary>
        public ReportingLine(IReportItemModeler pageContent)
        {
            this.LineModel = pageContent as LineModel;
            this.LineExpVal = LineModel.LineProperties;
            if (pageContent.PageInfo != null)
            {
                this.Margin = new Thickness(pageContent.FlowLayoutInfo.ActualLeft, pageContent.FlowLayoutInfo.ActualTop, 0, 0);
            }
            else if (pageContent.IsTablixChild)
            {
                this.Margin = new Thickness(pageContent.Left, pageContent.Top, 0, 0);
            } 
            string controlName = pageContent.Name;
            this.Name = controlName;
            this.Visibility = LineExpVal.Hidden ? Visibility.Collapsed : Visibility.Visible;

            if (pageContent.Height == 0)
            {
                pageContent.Height = 1;
            }

            if (pageContent.PageInfo != null)
            {
                this.Initialize(pageContent.PageInfo.ActualLeft, pageContent.PageInfo.ActualTop, pageContent.Width,pageContent.Height, LineExpVal.LineWidth, LineExpVal.LineColor);
            }
            else if (pageContent.IsTablixChild)
            {
                this.Initialize(pageContent.Left, pageContent.Top, pageContent.Width,pageContent.Height, LineExpVal.LineWidth, LineExpVal.LineColor);
            } 
            DoubleCollection dashesstyle = new DoubleCollection();

            if (this.LineModel.LineProperties.LineStyle==DOM.LineStyle.Dashed)
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

        private void Initialize(double left, double top, double width, double height, double lineThickness, string color)
        {
            Brush lineColor =(Brush)new ReportingBrushConverter().ConvertFromString(color);

            //// Left of the line.
            this.Left = left;

            //// Top of the line.
            this.Top = top;

            //// Width of the line control.
            this.Width = width;

            //// Height of the line control.
            this.Height = height<lineThickness?lineThickness:height;

            //// LineThinkness of the line control.
            this.LineThinkness = lineThickness;

            //// Color of the line control.
            this.LineColor = lineColor; 

            //// Adding the line control to the line container.
            lineContainer.Children.Add(line);

            // Setting default Horizontal alignment for the line container.
            lineContainer.HorizontalAlignment = HorizontalAlignment.Left;

            // Setting default vertical alignment for the line container.
            lineContainer.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            //// Adding the line container to the user control area.
            this.Content=lineContainer;

            this.InvalidateLine();
        }

        /// <summary>
        /// Invalidates the line.
        /// </summary>
        public void InvalidateLine()
        {
            this.line.X1 = this.LineModel.X1;
            this.line.Y1 = this.LineModel.Y1;
            this.line.X2 = this.LineModel.X2;
            this.line.Y2 = this.LineModel.Y2;
            this.line.Stroke = this.LineColor;
            this.line.StrokeThickness = this.LineThinkness;
        }

        
        #region Properties

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }


        /// <summary>
        /// Gets or sets the line thinkness.
        /// </summary>
        /// <value>The line thinkness.</value>
        public double LineThinkness
        {
            get { return (double)GetValue(LineThinknessProperty); }
            set { SetValue(LineThinknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        #endregion

        #region Dependency Properties
        
        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingLine.Left"/> dependency property.
        /// </summary>
        static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(ReportingLine), null);
        
        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingLine.Top"/> dependency property.
        /// </summary>
        static readonly DependencyProperty TopProperty =
            DependencyProperty.Register("Top", typeof(double), typeof(ReportingLine),null);
        
        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingLine.LineThinkness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineThinknessProperty =
            DependencyProperty.Register("LineThinkness", typeof(double), typeof(ReportingLine),null);
        
        /// <summary>
        /// Identifies the <see cref="Syncfusion.RDL.Controls.ReportingLine.LineColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(ReportingLine), null);

        #endregion
    }
}
