// <copyright file="DiagramViewGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the diagram grid.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class DiagramViewGrid : FrameworkElement
    {
        #region Class fields

        /// <summary>
        /// Represents the DiagramView.
        /// </summary>
        private DiagramView dview;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramViewGrid"/> class.
        /// </summary>
        public DiagramViewGrid()
        {
            this.Loaded += new RoutedEventHandler(DiagramViewGrid_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the DiagramViewGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DiagramViewGrid_Loaded(object sender, RoutedEventArgs e)
        {
            dview = this.TemplatedParent as DiagramView;
            this.InvalidateVisual();
        }

        #endregion
        

        //internal DiagramPage Page
        //{
        //    get { return (DiagramPage)GetValue(PageProperty); }
        //    set { SetValue(PageProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        //internal static readonly DependencyProperty PageProperty =
        //    DependencyProperty.Register("Page", typeof(DiagramPage), typeof(DiagramViewGrid));



        #region Methods

        /// <summary>
        /// Renders the DiagramViewGrid.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawGrid(drawingContext);
        }
        
        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="context">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        private void DrawGrid(DrawingContext context)
        {
            if (dview != null && dview.Page!=null)
            {
                double l = ((Matrix)dview.Page.TransformToVisual(dview.Scrollviewer).GetValue(MatrixTransform.MatrixProperty)).OffsetX;
                double t = ((Matrix)dview.Page.TransformToVisual(dview.Scrollviewer).GetValue(MatrixTransform.MatrixProperty)).OffsetY;
                double x = l + (dview.Page as DiagramPage).PxGridHorizontalOffset - (l % (dview.Page as DiagramPage).PxGridHorizontalOffset);
                double y = t + (dview.Page as DiagramPage).PxGridVerticalOffset - (t % (dview.Page as DiagramPage).PxGridVerticalOffset);

                Rect fill = new Rect(-(x < 0 ? 0 : x), -(y < 0 ? 0 : y), ActualWidth + (x < 0 ? -x : x) + (dview.Page as DiagramPage).PxGridHorizontalOffset, ActualHeight + (y < 0 ? -y : y) + (dview.Page as DiagramPage).PxGridVerticalOffset);

                double hlinescount = fill.Width / (dview.Page as DiagramPage).PxGridVerticalOffset;
                double vlinescount = fill.Height / (dview.Page as DiagramPage).PxGridHorizontalOffset;
                Point p = fill.TopLeft;

                if (dview.ShowVerticalGridLine)
                {
                    for (int i = 0; i < hlinescount; i++)
                    {
                        context.DrawLine(dview.VerticalGridLineStyle, p, new Point(p.X, fill.Bottom));
                        p = new Point(p.X + (dview.Page as DiagramPage).PxGridVerticalOffset, fill.Top);
                    }
                }

                p = fill.TopLeft;

                if (dview.ShowHorizontalGridLine)
                {
                    for (int i = 0; i < vlinescount; i++)
                    {
                        context.DrawLine(dview.HorizontalGridLineStyle, p, new Point(fill.Right, p.Y));
                        p = new Point(fill.Left, p.Y + (dview.Page as DiagramPage).PxGridHorizontalOffset);
                    }
                }
            }
        }
        #endregion
    }
}

