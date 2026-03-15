#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using System;
#else
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif


namespace Syncfusion.UI.Xaml.Gauges
{
    #region LinearGaugeUniformPanel

    public class LinearGaugeUniformPanel : Panel
    {
        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int i = 0;
            double gap = finalSize.Width / (Children.Count - 1);
            LinearScale parentScale = null;
            bool isTicks = false;
            if (Children.Count > 0)
            {
                var contentPresenter = Children[0] as ContentPresenter;
                if (contentPresenter != null)
                {
                    if (contentPresenter.Content != null)
                    {
                        if (contentPresenter.Content is LinearScaleTick)
                        {
                            parentScale = (contentPresenter.Content as LinearScaleTick).ParentScale;
                            isTicks = true;
                        }
                        else if (contentPresenter.Content is LinearScaleLabel)
                        {
                            parentScale = (contentPresenter.Content as LinearScaleLabel).ParentScale;
                        }
                    }
                }
            }

            double interval = parentScale.interval;

            if (interval > 0 && !double.IsNaN(interval) && interval < (Math.Abs(parentScale.Minimum - parentScale.Maximum)) && !double.IsNaN(parentScale.ScaleBarWidth) && parentScale.Minimum >= 0)
            {
                //Ticks & Label position Fix while dynamically update the Maximum
                gap = ((interval - parentScale.Minimum) * (parentScale.ScaleBarWidth) / ((parentScale.Maximum - parentScale.Minimum))) / ((parentScale.MinorTicksPerInterval) + 1);
            }
            else if (interval > 0 && !double.IsNaN(interval) && interval < (Math.Abs(parentScale.Minimum - parentScale.Maximum)) && !double.IsNaN(parentScale.ScaleBarLength))
            {
                gap = parentScale.ScaleBarLength / ((parentScale.Maximum - parentScale.Minimum) / interval);
            }
               
          
            foreach (UIElement element in Children)
            {
                if (isTicks)
                {
                    if (i == 0)
                        element.Arrange(new Rect((i * gap) + element.DesiredSize.Width / 2, 0, finalSize.Width, finalSize.Height));
                    else if (i == Children.Count - 1)
                        element.Arrange(new Rect((i * gap) - element.DesiredSize.Width, 0, finalSize.Width, finalSize.Height));
                    else
                        element.Arrange(new Rect((i * gap), 0, finalSize.Width, finalSize.Height));
                }
                else
                {
                    if (parentScale != null && parentScale.ParentGauge != null)
                    {
                        if (parentScale.ParentGauge.Orientation == Orientation.Horizontal)
                        {
                            element.Arrange(parentScale.ScaleDirection == LinearScaleDirection.Forward
                                                ? (i != 0 ? new Rect((i * gap) - element.DesiredSize.Width / 2, 0, finalSize.Width, finalSize.Height) :
                                                            new Rect((i * gap), 0, finalSize.Width, finalSize.Height))
                                                : (i != 0 ? new Rect((i * gap) + element.DesiredSize.Width / 2, 0, finalSize.Width, finalSize.Height) :
                                                            new Rect((i * gap), 0, finalSize.Width, finalSize.Height)));
                        }
                        if (parentScale.ParentGauge.Orientation == Orientation.Vertical)
                        {
                            element.Arrange(parentScale.ScaleDirection == LinearScaleDirection.Forward
                                                ? new Rect((i*gap) - element.DesiredSize.Height/2, 0, finalSize.Width,
                                                           finalSize.Height)
                                                : new Rect((i*gap) + element.DesiredSize.Height/2, 0, finalSize.Width,
                                                           finalSize.Height));
                        }
                    }
                    else
                        element.Arrange(i != 0 ? new Rect((i * gap) + element.DesiredSize.Width / 2, 0, finalSize.Width, finalSize.Height) :
                                                  new Rect((i * gap), 0, finalSize.Width, finalSize.Height));
                }

                i++;
            }
            return finalSize;
        }

        #endregion
    }

    #endregion
}
