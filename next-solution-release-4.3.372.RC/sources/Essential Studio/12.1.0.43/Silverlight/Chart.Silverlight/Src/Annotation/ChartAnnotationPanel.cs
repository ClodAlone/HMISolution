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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for Annotation Panel
    /// </summary>
    public class AnnotationPanel : Panel
    {
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = new Size(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
            foreach (FrameworkElement element in this.Children)
            {
                element.Measure(availableSize);
            }

            return availableSize;
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
            foreach (ContentPresenter presenter in this.Children)
            {
                ChartAnnotationLabel annotation = presenter.Tag as ChartAnnotationLabel;
                ChartSeriesAnnotation seriesannotation = presenter.Tag as ChartSeriesAnnotation;
                double left = annotation != null ? annotation.OffsetX : (seriesannotation != null ? seriesannotation.X + seriesannotation.OffsetX : 0);
                double top = annotation != null ? annotation.OffsetY : (seriesannotation != null ? seriesannotation.Y + seriesannotation.OffsetY : 0);

                //left = left < 0 ? 0 : (left + presenter.ActualWidth >= finalSize.Width ? left - (left + presenter.ActualWidth - finalSize.Width) : left);
                //top = top < 0 ? 0 : (top + presenter.ActualHeight >= finalSize.Height ? top - (top + presenter.ActualHeight - finalSize.Height) : top);

                if (seriesannotation != null)
                {
                    seriesannotation.SeriesAnnotationPanel = this;
                }
                else if (annotation != null)
                {
                    annotation.ChartAnnotationPanel = this;
                }


                presenter.Arrange(new Rect(left, top, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
            }

            return finalSize;
        }
    }
}
