// <copyright file="Chart3DGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
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
    using System.Windows.Media.Media3D;
   
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Chart._3DChart"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Chart._3DChart;assembly=Syncfusion.Windows.Chart._3DChart"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class Chart3DGrid : Control
    {
        /// <summary>
        /// Initializes m_xAxis
        /// </summary>
        private ChartAxis m_xaxis;

        /// <summary>
        /// Initializes m_yAxis
        /// </summary>
        private ChartAxis m_yaxis;

        /// <summary>
        /// Initializes m_zaxis
        /// </summary>
        private ChartAxis m_zaxis;

        /// <summary>
        /// Initializes m_side
        /// </summary>
        private GridSide m_side;              

        /// <summary>
        /// Initializes static members of the Chart3DGrid class
        /// </summary>
        static Chart3DGrid()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(Chart3DGrid));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart3DGrid), new FrameworkPropertyMetadata(typeof(Chart3DGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the Chart3DGrid class
        /// </summary>
        /// <param name="xaxis">The xaxis value</param>
        /// <param name="yaxis">The yaxis value</param>
        public Chart3DGrid(ChartAxis xaxis, ChartAxis yaxis)
        {
            this.m_xaxis = xaxis;
            this.m_yaxis = yaxis;
        }

        /// <summary>
        /// Initializes a new instance of the Chart3DGrid class
        /// </summary>
        /// <param name="xaxis">The xaxis value</param>
        /// <param name="yaxis">The yaxis value</param>
        /// <param name="zaxis">The zaxis value</param>
        public Chart3DGrid(ChartAxis xaxis, ChartAxis yaxis, ChartAxis zaxis)
        {
            this.m_xaxis = xaxis;
            this.m_yaxis = yaxis;
            this.m_zaxis = zaxis;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>The side value.</value>
        public GridSide Side
        {
            get
            {
                return this.m_side;
            }

            set
            {
                this.m_side = value;
            }
        }

        /// <summary>
        /// OnRender method
        /// </summary>
        /// <param name="drawingContext">The DrawingContext</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect clientRect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            if (this.m_xaxis.Area != null)
            {
                this.DrawGridLines(drawingContext, clientRect);
            }

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Draws the grid lines.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="clientRect">The client rect.</param>
        private void DrawGridLines(DrawingContext drawingContext, Rect clientRect)
        {
            foreach (ChartAxis axis in this.m_xaxis.Area.Axes)
            {
                if (ChartArea.GetShowGridLines(axis))
                {

                    if (axis.Orientation == System.Windows.Controls.Orientation.Horizontal && !axis.depthaxisflag && ((this.Side == GridSide.Back) || (this.Side == GridSide.Bottom)))
                    {
                        if (this.Side == GridSide.Bottom)
                        {
                            ChartAxis xaxis = axis;

                            #region Render primary ticks grid lines
                            if (xaxis.Area.PrimarySeries != null && xaxis != null && ChartArea.GetShowGridLines(xaxis) && xaxis.Area.PrimarySeries.Type != ChartTypes.PointAndFigure)
                            {
                                Pen gridLinePen = ChartArea.GetGridLineStroke(xaxis);
                                //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                                //gridLinePen.Thickness = 3d;

                                for (int i = 0, ci = xaxis.TicksPoint.Count; i < ci; i++)
                                {
                                    double x = clientRect.Width * xaxis.ValueToCoefficient((double)xaxis.TicksPoint[i]);
                                    drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                }

                                drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.BottomLeft);
                                drawingContext.DrawLine(gridLinePen, clientRect.TopRight, clientRect.BottomRight);
                            }
                            #endregion

                            #region Render primary grid lines
                            if (xaxis != null && ChartArea.GetShowGridLines(xaxis))
                            {
                                Pen gridLinePen = ChartArea.GetGridLineStroke(xaxis);
                                //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                                //gridLinePen.EndLineCap = PenLineCap.Round;
                                //gridLinePen.LineJoin = PenLineJoin.Bevel;
                                //gridLinePen.StartLineCap = PenLineCap.Round;
                                //gridLinePen.Thickness = 2d;

                                for (int i = 0, ci = xaxis.VisibleLabels.Count; i < ci; i++)
                                {
                                    double x = clientRect.Width * xaxis.ValueToCoefficient(xaxis.VisibleLabels[i].Position);
                                    double y = clientRect.Height * xaxis.ValueToCoefficient(xaxis.VisibleLabels[i].Position);

                                    drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                    drawingContext.DrawLine(gridLinePen, new Point(0, y), new Point(clientRect.Width, y));
                                }

                                drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.BottomLeft);
                                drawingContext.DrawLine(gridLinePen, clientRect.TopRight, clientRect.BottomRight);
                            }

                            #endregion
                        }
                        else
                        {
                            ChartAxis xaxis = axis;

                            #region Render primary ticks grid lines
                            if (xaxis.Area.PrimarySeries != null && xaxis != null && ChartArea.GetShowGridLines(xaxis) && xaxis.Area.PrimarySeries.Type != ChartTypes.PointAndFigure)
                            {
                                Pen gridLinePen = ChartArea.GetGridLineStroke(xaxis);
                                //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                                //gridLinePen.Thickness = 3d;

                                for (int i = 0, ci = xaxis.TicksPoint.Count; i < ci; i++)
                                {
                                    double x = clientRect.Width * xaxis.ValueToCoefficient((double)xaxis.TicksPoint[i]);
                                    drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                }

                                drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.BottomLeft);
                                drawingContext.DrawLine(gridLinePen, clientRect.TopRight, clientRect.BottomRight);
                            }
                            #endregion

                            #region Render primary grid lines
                            if (xaxis != null && ChartArea.GetShowGridLines(xaxis))
                            {
                                Pen gridLinePen = ChartArea.GetGridLineStroke(xaxis);
                                //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                                //gridLinePen.EndLineCap = PenLineCap.Round;
                                //gridLinePen.LineJoin = PenLineJoin.Bevel;
                                //gridLinePen.StartLineCap = PenLineCap.Round;
                                //gridLinePen.Thickness = 2d;

                                for (int i = 0, ci = xaxis.VisibleLabels.Count; i < ci; i++)
                                {
                                    double x = clientRect.Width * xaxis.ValueToCoefficient(xaxis.VisibleLabels[i].Position);
                                    
                                    drawingContext.DrawLine(gridLinePen, new Point(x, 0), new Point(x, clientRect.Height));
                                  
                                }

                                drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.BottomLeft);
                                drawingContext.DrawLine(gridLinePen, clientRect.TopRight, clientRect.BottomRight);
                            }

                            #endregion
                        }
                    }
                    else if (axis.Orientation == System.Windows.Controls.Orientation.Vertical && ((this.Side == GridSide.Back) || (this.Side == GridSide.Left)))
                    {
                        ChartAxis yaxis = axis;
                        #region Render secondary ticks grid lines
                        if (yaxis.Area.PrimarySeries != null && yaxis != null && ChartArea.GetShowGridLines(yaxis) && yaxis.Area.PrimarySeries.Type != ChartTypes.PointAndFigure)
                        {
                            Pen gridLinePen = ChartArea.GetGridLineStroke(yaxis);
                            //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                            //gridLinePen.Thickness = 3d;

                            for (int i = 0, ci = yaxis.TicksPoint.Count; i < ci; i++)
                            {
                                double y = clientRect.Height * (1 - yaxis.ValueToCoefficient((double)yaxis.TicksPoint[i]));
                                drawingContext.DrawLine(gridLinePen, new Point(0, y), new Point(clientRect.Width, y));
                            }

                            drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.TopRight);
                            drawingContext.DrawLine(gridLinePen, clientRect.BottomLeft, clientRect.BottomRight);
                        }
                        #endregion

                        #region Render secondary grid lines
                        if (yaxis != null && ChartArea.GetShowGridLines(yaxis))
                        {
                            Pen gridLinePen = ChartArea.GetGridLineStroke(yaxis);
                            //gridLinePen.Brush = new SolidColorBrush(Colors.Black);
                            //gridLinePen.EndLineCap = PenLineCap.Round;
                            //gridLinePen.LineJoin = PenLineJoin.Bevel;
                            //gridLinePen.StartLineCap = PenLineCap.Triangle;
                            //gridLinePen.Thickness = 2d;

                            for (int i = 0, ci = yaxis.VisibleLabels.Count; i < ci; i++)
                            {
                                double y = clientRect.Height * (1 - yaxis.ValueToCoefficient(yaxis.VisibleLabels[i].Position));
                                drawingContext.DrawLine(gridLinePen, new Point(0, y), new Point(clientRect.Width, y));
                            }

                            drawingContext.DrawLine(gridLinePen, clientRect.TopLeft, clientRect.TopRight);
                            drawingContext.DrawLine(gridLinePen, clientRect.BottomLeft, clientRect.BottomRight);
                        }
                        #endregion
                    }
                }
            }
        }
    }
}
