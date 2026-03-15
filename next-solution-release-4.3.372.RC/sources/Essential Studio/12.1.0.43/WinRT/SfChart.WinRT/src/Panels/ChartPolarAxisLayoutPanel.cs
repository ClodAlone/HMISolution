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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents ChartPolarAxisLayoutPanel
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartPolarAxisLayoutPanel : ILayoutCalculator
    {
        #region fields

        private Size desiredSize;
        private Panel panel;

        private double radius;
        private bool isRadiusCalculating;

        #endregion

        #region properties
        /// <summary>
        /// Gets or Sets the Chart area of the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfChart Area
        {
            get;
            set;
        }

        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Gets or Sets the polar axis of the Chart area.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisBase2D PolarAxis
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets the Cartesian axis of the Chart area.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisBase2D CartesianAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the radius of the panel
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (radius != value)
                {
                    radius = value;

                    if (!isRadiusCalculating 
                        && Area != null && Area.GridLinesLayout is ChartPolarGridLinesPanel)
                    {
                        CalculateSeriesRect(this.desiredSize);
                        (Area.GridLinesLayout as ChartPolarGridLinesPanel).UpdateElements();
                        (Area.GridLinesLayout as ChartPolarGridLinesPanel).Measure(this.desiredSize);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the desired sze of a panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Size DesiredSize
        {
            get { return desiredSize; }
        }
        /// <summary>
        /// Gets the Children count in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public List<UIElement> Children
        {
            get
            {
                if (panel != null)
                {
                    return panel.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get;
            set;
        }
        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartPolarLayoutPanel
        /// </summary>
        /// <param name="panel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChartPolarAxisLayoutPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException();

            this.panel = panel;
        }

        #endregion

        #region methods
        /// <summary>
        /// Measures the elements in the panel
        /// </summary>
        /// <param name="availableSize">AvailableSize of the panel</param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Size Measure(Size availableSize)
        {
            isRadiusCalculating = true;
            if (PolarAxis!=null)
            {
            PolarAxis.ComputeDesiredSize(availableSize);
#if NETFX_CORE || WPF || SILVERLIGHT_UNCOMMON
            PolarAxis.EnableScrollBar = false;
            }
            if (CartesianAxis!=null)
            {
            CartesianAxis.EnableScrollBar = false;
#endif
            CalculateSeriesRect(availableSize);
            CartesianAxis.ComputeDesiredSize(new Size(Area.SeriesClipRect.Width,
                                                      Math.Min(Area.SeriesClipRect.Width, Area.SeriesClipRect.Height) / 2));
            }
            isRadiusCalculating = false;
            desiredSize = availableSize;
            return availableSize;
        }

        public void DetachElements()
        {
            if (CartesianAxis != null)
            {
                if (CartesianAxis.GridLinesRecycler != null)
                    CartesianAxis.GridLinesRecycler.Clear();
                if (CartesianAxis.MinorGridLinesRecycler != null)
                    CartesianAxis.MinorGridLinesRecycler.Clear();
            }
            panel.Children.Clear();
            panel = null;
        }

        void CalculateSeriesRect(Size availableSize)
        {
            double width = Math.Max(availableSize.Width / 2 - Radius, 0);
            double height = Math.Max(availableSize.Height / 2 - Radius, 0);

            this.Area.AxisThickness = new Thickness(width, height, width, height);

            Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(0, 0), availableSize), this.Area.AxisThickness);

            this.Area.SeriesClipRect = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);

            Area.InternalCanvas.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, this.Area.SeriesClipRect.Width, this.Area.SeriesClipRect.Height)
            };
        }

        /// <summary>
        /// Arranges the elements in a panel
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange(Size finalSize)
        {
            double radius = Math.Max(0, this.Radius);
            Rect clientRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            if (PolarAxis!=null)
            {
            PolarAxis.ArrangeRect = clientRect;
            PolarAxis.Measure(new Size(clientRect.Width, clientRect.Height));
            PolarAxis.Arrange(clientRect);
            Canvas.SetLeft(PolarAxis, clientRect.Left);
            Canvas.SetTop(PolarAxis, clientRect.Top);
            }
            ChartAxis yAxis = this.CartesianAxis;

            if (yAxis != null)
            {
                Point center = ChartLayoutUtils.GetCenter(clientRect);

                if (!yAxis.OpposedPosition)
                {
                    CartesianAxis.ArrangeRect = new Rect(center.X - this.CartesianAxis.ComputedDesiredSize.Width, center.Y - radius, this.CartesianAxis.ComputedDesiredSize.Width, radius);
                    Rect rect = new Rect(this.CartesianAxis.ArrangeRect.Left, this.CartesianAxis.ArrangeRect.Top, this.CartesianAxis.ComputedDesiredSize.Width, this.CartesianAxis.ComputedDesiredSize.Height);
                    CartesianAxis.Measure(new Size(rect.Width, rect.Height));
                    CartesianAxis.Arrange(rect);
                    Canvas.SetLeft(CartesianAxis, CartesianAxis.ArrangeRect.Left);
                    Canvas.SetTop(CartesianAxis, CartesianAxis.ArrangeRect.Top);
                }
                else
                {
                    CartesianAxis.ArrangeRect = new Rect(center.X, center.Y - radius, this.CartesianAxis.ComputedDesiredSize.Width, radius);
                    Rect rect = new Rect(this.CartesianAxis.ArrangeRect.Left, this.CartesianAxis.ArrangeRect.Top, this.CartesianAxis.ComputedDesiredSize.Width, this.CartesianAxis.ComputedDesiredSize.Height);
                    CartesianAxis.Measure(new Size(rect.Left, rect.Height));
                    CartesianAxis.Arrange(rect);
                    Canvas.SetLeft(CartesianAxis, CartesianAxis.ArrangeRect.Left);
                    Canvas.SetTop(CartesianAxis, CartesianAxis.ArrangeRect.Top);
                }
            }

            double width = Math.Max(finalSize.Width / 2 - radius, 0);
            double height = Math.Max(finalSize.Height / 2 - radius, 0);

            //this.Area.AxisThickness = new Thickness(width, height, width, height);

            return finalSize;
        }

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void UpdateElements()
        {
            if (this.Area != null && this.Area.InternalPrimaryAxis!=null)
            {
                PolarAxis = this.Area.InternalPrimaryAxis as ChartAxisBase2D;
                PolarAxis.AxisLayoutPanel = this;
                CartesianAxis = this.Area.InternalSecondaryAxis as ChartAxisBase2D;
                if (!this.Children.Contains(PolarAxis))
                {
                    this.Children.Add(PolarAxis);
                    panel.Children.Add(PolarAxis);
                }

                if (!this.Children.Contains(CartesianAxis))
                {
                    this.Children.Add(CartesianAxis);
                    panel.Children.Add(CartesianAxis);
                }
            }
        }
        #endregion
    }
}
