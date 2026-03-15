// <copyright file="ChartPolarAxisLabelsPanel.cs" company="Syncfusion">
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

    /// <summary>
    /// Represents ChartPolarAxisLabelsPanel class
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartPolarAxisLabelsPanel : Panel
    {
        #region Members
        /// <summary>
        /// Declares m_maxLabelsSize
        /// </summary>
        private Size m_maxLabelsSize = new Size();
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Axis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
          DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartPolarAxisLabelsPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnAxisChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
          ChartCartesianAxisLabelsPanel.PositionProperty.AddOwner(typeof(ChartPolarAxisLabelsPanel));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(ChartPolarAxisLabelsPanel), new UIPropertyMetadata(0d));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Center.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(ChartPolarAxisLabelsPanel));
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis value.</value>
        public ChartAxis Axis
        {
            get
            {
                return (ChartAxis)GetValue(AxisProperty);
            }

            set
            {
                SetValue(AxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public Point Center
        {
            get
            {
                return (Point)GetValue(CenterProperty);
            }

            set
            {
                SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPolarAxisLabelsPanel"/> class.
        /// </summary>
        public ChartPolarAxisLabelsPanel()
        {
            base.ClipToBounds = false;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            m_maxLabelsSize = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                element.Measure(availableSize);

                m_maxLabelsSize.Width = Math.Max(element.DesiredSize.Width, m_maxLabelsSize.Width);
                m_maxLabelsSize.Height = Math.Max(element.DesiredSize.Height, m_maxLabelsSize.Height);
            }

            double dx = availableSize.Width - 2 * m_maxLabelsSize.Width;
            double dy = availableSize.Height - 2 * m_maxLabelsSize.Height;

            this.Radius = 0.5 * Math.Min(dx, dy) - Math.Max(Axis.TickSize, 0);
            this.Center = (Point)ChartLayoutUtils.GetCenter(availableSize);

            return ChartLayoutUtils.CheckSize(availableSize);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double dx = finalSize.Width - 2 * m_maxLabelsSize.Width;
            double dy = finalSize.Height - 2 * m_maxLabelsSize.Height;

            this.Radius = Math.Max(0, 0.5 * Math.Min(dx, dy) - Math.Max(Axis.TickSize, 0));
            this.Center = (Point)ChartLayoutUtils.GetCenter(finalSize);

            this.DoLayout(finalSize);

            return finalSize;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point center = this.Center;
            double radius = this.Radius;

            foreach (ChartAxisLabel label in Axis.VisibleLabels)
            {
                Vector vector = ChartTransform.ValueToVector(this.Axis, label.Position);
                Point connectPoint = center + radius * vector;
                Point endPoint = connectPoint + Axis.TickSize * vector;

                drawingContext.DrawLine(Axis.LineStroke, connectPoint, endPoint);
            }
            //if (!Axis.Area.m_isRadar)
            //{
               // drawingContext.DrawEllipse(null, Axis.LineStroke, Center, Radius, Radius);
           // }
            //else if (Axis.Area.m_isRadar && !ChartRadarType.GetIsNetGridEnabled(Axis.Area))
            //{
            //    drawingContext.DrawEllipse(null, Axis.LineStroke, Center, Radius, Radius);
            //}

            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Does the layout.
        /// </summary>
        /// <param name="size">The size value.</param>
        /// <returns>Returns the size</returns>
        private Size DoLayout(Size size)
        {
            double aroundRadius = this.Radius + Math.Max(Axis.TickSize, 0);

            foreach (FrameworkElement element in this.InternalChildren)
            {
                double pos = (double)element.GetValue(ChartCartesianAxisLabelsPanel.PositionProperty);
                double coef = this.Axis.ValueToCoefficient(pos);

                Vector vector = ChartTransform.ValueToVector(this.Axis, pos);
                Point connectPoint = this.Center + aroundRadius * vector;
                var labelwidth = ((element as UIElement).DesiredSize.Width)/2;
                if (coef == 0.25d)
                {
                    connectPoint.X -= element.DesiredSize.Width;
                    connectPoint.Y -= element.DesiredSize.Height / 2;
                }
                else if (coef == 0.5d)
                {
                    connectPoint.X -= element.DesiredSize.Width / 2;
                }
                else if (coef == 0.75d)
                {
                    connectPoint.Y -= element.DesiredSize.Height / 2;
                }
                else if (0 <= coef && coef < 0.25d)
                {
                    if (Math.Abs((this.Center.X) - (connectPoint.X)) < labelwidth)
                        connectPoint.X -= element.DesiredSize.Width / 2;
                    else
                        connectPoint.X -= element.DesiredSize.Width;
                    connectPoint.Y -= element.DesiredSize.Height;
                }
                else if (0.25d < coef && coef < 0.5d)
                {
                    connectPoint.X -= element.DesiredSize.Width;
                }
                else if (0.75d < coef && coef <= 1d)
                {
                    connectPoint.Y -= element.DesiredSize.Height;
                }

                element.Arrange(new Rect(connectPoint, element.DesiredSize));
            }

            return size;
        }

        /// <summary>
        /// Called when axis is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Called when axis is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxisChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartPolarAxisLabelsPanel axisElement = dObj as ChartPolarAxisLabelsPanel;

            if (axisElement != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxis).Changed -= new EventHandler(axisElement.OnAxisChanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxis).Changed += new EventHandler(axisElement.OnAxisChanged);
                }
            }
        }
        #endregion
    }
}
