// <copyright file="ChartCartesianPanel.cs" company="Syncfusion">
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
    using System.Linq;

    /// <summary>
    /// Represents panel for cartesian coordinate system. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartCartesianPanel : Panel, IDisposable
    {
        #region Constants
        /// <summary>
        /// Initializes c_axesLengthCoef
        /// </summary>
        private const double C_axesLengthCoef = 0.8d;
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_axesThickness
        /// </summary>
        private Thickness m_axesThickness = new Thickness();
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the AxesThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesThicknessProperty =
            DependencyProperty.Register("AxesThickness", typeof(Thickness), typeof(ChartCartesianPanel), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the HorizontalScrollBarHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarHeightProperty =
            DependencyProperty.Register("HorizontalScrollBarHeight", typeof(double), typeof(ChartCartesianPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the VerticalScrollBarWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarWidthProperty =
            DependencyProperty.Register("VerticalScrollBarWidth", typeof(double), typeof(ChartCartesianPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the height of the horizontal scroll bar. This is a dependency property.
        /// </summary>
        /// <value>The height of the horizontal scroll bar.</value>
        public double HorizontalScrollBarHeight
        {
            get { return (double)GetValue(HorizontalScrollBarHeightProperty); }
            set { SetValue(HorizontalScrollBarHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the vertical scroll bar. This is a dependency property.
        /// </summary>
        /// <value>The width of the vertical scroll bar.</value>
        public double VerticalScrollBarWidth
        {
            get { return (double)GetValue(VerticalScrollBarWidthProperty); }
            set { SetValue(VerticalScrollBarWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axes thickness. This is a dependency property.
        /// </summary>
        /// <value>The axes thickness.</value>
        public Thickness AxesThickness
        {
            get { return (Thickness)GetValue(AxesThicknessProperty); }
            set { SetValue(AxesThicknessProperty, value); }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Thickness thickness = new Thickness();
            Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(finalSize), this.m_axesThickness);
            foreach (UIElement element in base.InternalChildren)
            {
                if (element != null)
                {
                    Size desiredSize = element.DesiredSize;
                    ChartAxis content = (element as ContentPresenter).Content as ChartAxis;
                    try
                    {
                        if (content != null)
                        {
                            if (content.Orientation == Orientation.Horizontal)
                            {
                                if (content.OpposedPosition)
                                {
                                    element.Arrange(new Rect(rect.X, thickness.Top, rect.Width - this.VerticalScrollBarWidth, desiredSize.Height));
                                    thickness.Top += desiredSize.Height;
                                }
                                else
                                {
                                    element.Arrange(new Rect(rect.X, (finalSize.Height - thickness.Bottom) - desiredSize.Height, rect.Width - this.VerticalScrollBarWidth, desiredSize.Height));
                                    thickness.Bottom += desiredSize.Height == 0 ? (element as FrameworkElement).ActualHeight : desiredSize.Height;
                                }

                                if ((((content != null) && (content.Area != null)) && content.Area.IsSync && content.Area.PrimaryChartArea != null))
                                {
                                    if (content.Area.PrimaryAxis == content)
                                    {
                                        if (content.Area.index == ((SyncChartAreas)content.Area.ChartAreaParent).Areas.Count - 1)
                                        {
                                            //content.axisHeight = element.DesiredSize.Height;
                                            content.axisHeight = (element.DesiredSize.Height < (element as FrameworkElement).ActualHeight ? (element as FrameworkElement).ActualHeight : element.DesiredSize.Height);// element.DesiredSize.Height;
                                        }
                                    }
                                }
                            }
                            else if (content.OpposedPosition)
                            {
                                element.Arrange(new Rect((finalSize.Width - thickness.Right) - desiredSize.Width, rect.Y, desiredSize.Width, rect.Height - this.HorizontalScrollBarHeight));
                                thickness.Right += desiredSize.Width;
                            }
                            else
                            {
                                element.Arrange(new Rect(thickness.Left, rect.Y, desiredSize.Width, rect.Height - this.HorizontalScrollBarHeight));
                                thickness.Left += desiredSize.Width;
                            }
                            this.AxesThickness = this.m_axesThickness;

                            if (content.Area.IsSync && (content.Area.PrimaryChartArea != null))
                            {
                                double left = double.MinValue, right = double.MinValue;
                                left = (from d in content.Area.ChartAreaParent.Areas select d.AxesThickness.Left).Max();
                                right = (from d in content.Area.ChartAreaParent.Areas select d.AxesThickness.Right).Max();

                                foreach (ChartArea area in content.Area.ChartAreaParent.Areas)
                                {
                                    Thickness tempThickness = new Thickness(0);
                                    if (area.AxesThickness.Left < left)
                                    {
                                        tempThickness = new Thickness(left - area.AxesThickness.Left, 0, 0, 0);
                                    }

                                    if (area.AxesThickness.Right < right)
                                    {
                                        tempThickness = new Thickness(tempThickness.Left, 0, right - area.AxesThickness.Right, 0);
                                    }

                                    int index = content.Area.ChartAreaParent.Areas.IndexOf(area);
                                    int last = content.Area.ChartAreaParent.Areas.Count - 1;
                                    if (index != 0 && index != last)
                                    {
                                        //if (content.Area.ChartAreaParent.isCustomPanel == true)
                                        //{
                                        //    area.Margin = new Thickness(tempThickness.Left, 0 - area.AxesThickness.Top , tempThickness.Right, 0 - area.AxesThickness.Bottom );
                                        //}
                                        //else
                                        //{
                                            area.Margin = new Thickness(tempThickness.Left, 0 - area.AxesThickness.Top , tempThickness.Right, 0 - area.AxesThickness.Bottom );
                                        //}
                                    }
                                    else if (index == 0)
                                    {
                                        //if (content.Area.ChartAreaParent.isCustomPanel == true)
                                        //{
                                        //    area.Margin = new Thickness(tempThickness.Left, 0, tempThickness.Right, 0 - area.AxesThickness.Bottom );
                                        //}
                                        //else
                                        //{
                                            area.Margin = new Thickness(tempThickness.Left, 0, tempThickness.Right, 0 - area.AxesThickness.Bottom );

                                        //}
                                    }
                                    else if (index == last)
                                    {
                                        //if (content.Area.ChartAreaParent.isCustomPanel == true)
                                        //{
                                        //    area.Margin = new Thickness(tempThickness.Left, 0 - area.AxesThickness.Top, tempThickness.Right, 0);
                                        //}
                                        //else
                                        //{
                                            area.Margin = new Thickness(tempThickness.Left, 0 - area.AxesThickness.Top , tempThickness.Right, 0);
                                        //}
                                    }
                                }
                            }

                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Width and height must be non-negative.");
                    }
                }
            }
            return finalSize;


        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(0.8 * availableSize.Width, 0.8 * availableSize.Height);
            this.m_axesThickness = new Thickness(0.0);
            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(size);
                ChartAxis content = (element as ContentPresenter).Content as ChartAxis;
                if (content != null)
                {
                    if (content.Orientation == Orientation.Horizontal)
                    {
                        if (content.OpposedPosition)
                        {
                            this.m_axesThickness.Top += element.DesiredSize.Height;
                        }
                        else
                        {
                            this.m_axesThickness.Bottom += element.DesiredSize.Height;
                        }
                    }
                    else if (content.OpposedPosition)
                    {
                        this.m_axesThickness.Right += element.DesiredSize.Width;
                        if ((((content != null) && (content.Area != null)) && content.Area.IsSync) && (content.Area.PrimaryChartArea.areaThickness.Right < this.m_axesThickness.Right))
                        {
                            content.Area.PrimaryChartArea.areaThickness.Right = this.m_axesThickness.Right;
                        }
                    }
                    else
                    {
                        this.m_axesThickness.Left += element.DesiredSize.Width;
                        if (content.Area.PrimaryChartArea != null)
                        {
                            if ((((content != null) && (content.Area != null)) && content.Area.IsSync) && (content.Area.PrimaryChartArea.areaThickness.Left < this.m_axesThickness.Left))
                            {
                                content.Area.PrimaryChartArea.areaThickness.Left = this.m_axesThickness.Left;
                            }
                        }
                    }
                }
                this.AxesThickness = this.m_axesThickness;
            }
            return ChartLayoutUtils.CheckSize(availableSize);

        }

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"/> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"/> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"/> that was removed from the collection.</param>
        /// <seealso cref="ChartCartesianPanel"/>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded != null)
            {
                ChartAxis newAxis = (visualAdded as ContentPresenter).DataContext as ChartAxis;
                if (newAxis != null)
                {
                    newAxis.Changed += new EventHandler(OnChildAxisChanged);
                }
            }

            if (visualRemoved != null)
            {
                ChartAxis oldAxis = (visualRemoved as ContentPresenter).DataContext as ChartAxis;
                if (oldAxis != null)
                {
                    oldAxis.Changed -= new EventHandler(OnChildAxisChanged);
                }
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// Called when child axis changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnChildAxisChanged(object sender, EventArgs e)
        {
            AxisChangedEventArgs axisEventArgs = e as AxisChangedEventArgs;
            if (axisEventArgs != null)
            {
                if (axisEventArgs.DependencyPropertyEventArgs.Property == ChartAxis.OpposedPositionProperty || axisEventArgs.DependencyPropertyEventArgs.Property == ChartAxis.HeaderPositionProperty)
                {
                    this.InvalidateMeasure();
                }
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Resources.Clear();
            this.Resources = null;
        }

        #endregion
    }
}