#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartScaleBreakPanel
    /// </summary>
    public class ChartScaleBreakPanel : Panel
    {
      

        #region Members
        internal ChartAxis m_axis = null;
        #endregion

        #region Implementation

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in this.Children)
            {
                ContentPresenter scaleBreakPresenter = element as ContentPresenter;
                ChartScaleBreak scaleBreak = scaleBreakPresenter.Tag as ChartScaleBreak;
                scaleBreak.scaleBreakPanel = this;
                this.m_axis = scaleBreak.m_axis;

                ItemsPresenter presenter = VisualTreeHelper.GetParent(this) as ItemsPresenter;
                ChartScaleBreakPresenter scaleBreakItemsControl = VisualTreeHelper.GetParent(presenter) as ChartScaleBreakPresenter;

                ChartAxis xAxis = scaleBreakItemsControl.XAxis;
                ChartAxis yAxis = scaleBreakItemsControl.YAxis;
                ChartArea area = scaleBreakItemsControl.Area;
                IChartTransformer transformer = null;
                Point from, to;
                Path path = new Path();

                if (area != null)
                    transformer = ChartTransform.CreateCartesian(new Rect(new Point(0, 0), availableSize), area.PrimarySeries);

                if (m_axis != null && yAxis != null && m_axis == yAxis && transformer != null)
                {
                    if (m_axis.m_enableBreaks && m_axis.BreaksMode != ScaleBreaksModes.None)
                    {
                        if (!area.PrimarySeries.IsRotated(area.PrimarySeries.Type))
                        {
                            from = transformer.TransformToVisible(xAxis.VisibleRange.Start, scaleBreak.m_breakRange.End, area.PrimarySeries);
                            to = transformer.TransformToVisible(xAxis.VisibleRange.End, scaleBreak.m_breakRange.End, area.PrimarySeries);
                        }
                        else
                        {
                            from = transformer.TransformToVisible(scaleBreak.m_breakRange.End, xAxis.VisibleRange.Start, area.PrimarySeries);
                            to = transformer.TransformToVisible(scaleBreak.m_breakRange.End, xAxis.VisibleRange.End, area.PrimarySeries);
                        }
                        path = scaleBreak.DrawBreakLine(scaleBreak, availableSize, from, to);
                    }
                    else
                    {
                        this.Children.Clear();
                    }
                }
                scaleBreakPresenter.Content = path;

                element.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);

        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in this.Children)
            {
                element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return base.ArrangeOverride(finalSize);
        }

        #endregion
    }

   
}
