// <copyright file="ChartPolarAreaGrid.cs" company="Syncfusion">
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
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    /// <summary>
    /// Represents the ChartPolarAreaGrid
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPolarAreaGrid : FrameworkElement
    {
        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartPolarAreaGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartPolarAreaGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChartPolarAreaGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the secondary axis.
        /// </summary>
        /// <value>The secondary axis.</value>
        public ChartAxis XAxis
        {
            get
            {
                return (ChartAxis)GetValue(XAxisProperty);
            }

            set
            {
                SetValue(XAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the primary axis.
        /// </summary>
        /// <value>The primary axis.</value>
        public ChartAxis YAxis
        {
            get
            {
                return (ChartAxis)GetValue(YAxisProperty);
            }

            set
            {
                SetValue(YAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            double bigRadius = Math.Min(this.ActualWidth, this.ActualHeight) / 2;
            Point center = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            #region Render background
            drawingContext.DrawEllipse(this.Background, null, center, bigRadius, bigRadius);
            #endregion

            #region Render secondary grid lines
            if (xAxis != null && ChartArea.GetShowGridLines(yAxis))
            {
                Pen pen = ChartArea.GetGridLineStroke(yAxis);
                ObservableCollection<ChartAxisLabel> labcoll = new ObservableCollection<ChartAxisLabel>();
                labcoll = xAxis.VisibleLabels;
                //yAxis
                if (!yAxis.Area.m_isRadar)
                {
                    foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                    {
                        double radius = bigRadius * yAxis.ValueToCoefficient(label.Position);
                        drawingContext.DrawEllipse(null, pen, center, radius, radius);
                    }
                }
                else if (yAxis.Area.m_isRadar )// && ChartRadarType.GetIsNetGridEnabled(yAxis.Area))
                {
                    # region RangeCalculationMode != AdjustAcrossChartTypes
                    if (xAxis.RangeCalculationMode != RangeCalculationMode.AdjustAcrossChartTypes)
                    {
                        foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                        {
                            double radius = bigRadius * yAxis.ValueToCoefficient(label.Position);
                            for (int i = 0; i < labcoll.Count; i++)
                            {

                            Vector vector = ChartTransform.ValueToVector(xAxis, labcoll[i].Position);
                            Vector vector2 = new Vector();
                            if ((i + 1) < labcoll.Count)
                            {
                                vector2 = ChartTransform.ValueToVector(xAxis, labcoll[i + 1].Position);
                            }
                            else
                            {
                                vector2 = ChartTransform.ValueToVector(xAxis, labcoll[0].Position);
                            }
                            Point connectPoint = center + radius * vector;
                            Point endPoint = center + radius * vector2;

                                drawingContext.DrawLine(yAxis.LineStroke, connectPoint, endPoint);
                            }
                        }
                    }
                    #endregion
                    #region  RangeCalculationMode == AdjustAcrossChartTypes
                    else
                    {
                        foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                        {
                            double radius = bigRadius * yAxis.ValueToCoefficient(label.Position);
                            if (labcoll.Count > 0)
                            {
                                Vector startingptvec = ChartTransform.ValueToVector(yAxis, yAxis.VisibleLabels[0].Position);
                                Vector firstsegendpt = ChartTransform.ValueToVector(xAxis, labcoll[0].Position);
                                Point p1 = center + radius * startingptvec;
                                Point p2 = center + radius * firstsegendpt;
                                drawingContext.DrawLine(yAxis.LineStroke, p1, p2);
                                for (int i = 0; i < labcoll.Count; i++)
                                {

                                    Vector vector = ChartTransform.ValueToVector(xAxis, labcoll[i].Position);
                                    Vector vector2 = new Vector();
                                    if ((i + 1) < labcoll.Count)
                                    {
                                        vector2 = ChartTransform.ValueToVector(xAxis, labcoll[i + 1].Position);
                                        Point connectPoint = center + radius * vector;
                                        Point endPoint = center + radius * vector2;

                                        drawingContext.DrawLine(yAxis.LineStroke, connectPoint, endPoint);
                                    }
                                }
                                Vector lastsegconnectpt = ChartTransform.ValueToVector(xAxis, labcoll[labcoll.Count -1].Position);
                                Point _p1 = center + radius * lastsegconnectpt ;
                                Point _p2 = center + radius * startingptvec;
                                drawingContext.DrawLine(yAxis.LineStroke, _p1, _p2);
                            }
                        }
                    }
                    #endregion
                }
                //else
                //{
                //    foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                //    {
                //        double radius = bigRadius * yAxis.ValueToCoefficient(label.Position);
                //        drawingContext.DrawEllipse(null, pen, center, radius, radius);
                //    }
                //}
            }
            #endregion

            #region Render primary grid lines
            if (xAxis != null && ChartArea.GetShowGridLines(xAxis))
            {
                Pen pen = ChartArea.GetGridLineStroke(xAxis);

                foreach (ChartAxisLabel label in xAxis.VisibleLabels)
                {
                    Vector vector = ChartTransform.ValueToVector(xAxis, label.Position);
                    drawingContext.DrawLine(pen, center, center + bigRadius * vector);
                }
            }
            #endregion

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxeschanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxeschanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartPolarAreaGrid grid = dObj as ChartPolarAreaGrid;

            if (grid != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxis).Changed -= new EventHandler(grid.OnAxeschanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxis).Changed += new EventHandler(grid.OnAxeschanged);
                }
            }
        }
        #endregion
    }
}