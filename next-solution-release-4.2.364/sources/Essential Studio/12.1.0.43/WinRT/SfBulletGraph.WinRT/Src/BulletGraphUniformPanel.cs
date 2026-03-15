#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
#else
using System.Windows;
using System.Windows.Controls;

#endif

namespace Syncfusion.UI.Xaml.BulletGraph
{
    #region BulletGraphUniformPanel

    public class BulletGraphUniformPanel : Panel
    {
        public BulletGraphUniformPanel()
        {

        }
        #region Private Members
        SfBulletGraph ParentBulletGraph;
        bool isTicks;
        double maxLabelWidth, maxLabelHeight, minLabelWidth, minLabelHeight;
        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count > 0)
            {
                var contentPresenter = Children[0] as ContentPresenter;
                if (contentPresenter != null)
                {
                    if (contentPresenter.Content is BulletGraphTick)
                    {
                        ParentBulletGraph = (contentPresenter.Content as BulletGraphTick).ParentBulletGraph;
                        isTicks = true;
                    }
                    else if (contentPresenter.Content is BulletGraphLabel)
                    {
                        ParentBulletGraph = (contentPresenter.Content as BulletGraphLabel).ParentBulletGraph;
                        isTicks = false;
                    }
                }
            }
            int i = 0;
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                if (!isTicks)
                {
                    maxLabelWidth = Math.Max(maxLabelWidth, element.DesiredSize.Width);
                    maxLabelHeight = Math.Max(maxLabelHeight, element.DesiredSize.Height);
                    if (i == 0)
                    {
                        minLabelWidth = maxLabelWidth;
                        minLabelHeight = maxLabelHeight;
                    }

                    minLabelWidth = (!element.DesiredSize.Width.Equals(0)) ? Math.Min(minLabelWidth, element.DesiredSize.Width) : minLabelWidth;
                    minLabelHeight = (!element.DesiredSize.Height.Equals(0)) ? Math.Min(minLabelHeight, element.DesiredSize.Height) : minLabelHeight;
                }
                i++;
            }
//#if SILVERLIGHT && !WINDOWS_PHONE
//            if (!isTicks)
//            {
//                if (ParentBulletGraph != null)
//                {
//                    if (ParentBulletGraph.Orientation == Orientation.Horizontal)
//                    {
//                        ParentBulletGraph.QuantativeScaleWidth = ParentBulletGraph.RangesWidth + maxLabelWidth;
//                        ParentBulletGraph.LabelsMargin = new Thickness(-minLabelWidth, ParentBulletGraph.LabelsMargin.Top, ParentBulletGraph.LabelsMargin.Right, ParentBulletGraph.LabelsMargin.Bottom);
//                        ParentBulletGraph.TicksMargin = new Thickness(maxLabelWidth / 2, ParentBulletGraph.TicksMargin.Top, maxLabelWidth / 2, ParentBulletGraph.TicksMargin.Bottom);
//                        ParentBulletGraph.RangesMargin = new Thickness(maxLabelWidth / 2, ParentBulletGraph.RangesMargin.Top, maxLabelWidth / 2, ParentBulletGraph.RangesMargin.Bottom);
//                    }
//                    else
//                    {
//                        ParentBulletGraph.QuantativeScaleWidth = ParentBulletGraph.RangesWidth + maxLabelHeight;
//                        ParentBulletGraph.LabelsMargin = new Thickness(-minLabelHeight, ParentBulletGraph.LabelsMargin.Top, ParentBulletGraph.LabelsMargin.Right, ParentBulletGraph.LabelsMargin.Bottom);
//                        ParentBulletGraph.TicksMargin = new Thickness(maxLabelHeight / 2, ParentBulletGraph.TicksMargin.Top, maxLabelHeight / 2, ParentBulletGraph.TicksMargin.Bottom);
//                        ParentBulletGraph.RangesMargin = new Thickness(maxLabelHeight / 2, ParentBulletGraph.RangesMargin.Top, maxLabelHeight / 2, ParentBulletGraph.RangesMargin.Bottom);
//                    }
//                }
//            }
//#endif
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int i = 0;
            double gap = finalSize.Width / (Children.Count - 1);

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
                    if (ParentBulletGraph != null && ParentBulletGraph != null)
                    {
                        if (ParentBulletGraph.Orientation == Orientation.Horizontal)
                        {
                            element.Arrange(ParentBulletGraph.FlowDirection == BulletGraphFlowDirection.Forward
                                                ? new Rect((i * gap) - element.DesiredSize.Width / 2, 0, element.DesiredSize.Width, element.DesiredSize.Height)
                                                : new Rect((i * gap) + element.DesiredSize.Width / 2, 0, element.DesiredSize.Width, element.DesiredSize.Height));
                        }
                        if (ParentBulletGraph.Orientation == Orientation.Vertical)
                        {
                            element.Arrange(ParentBulletGraph.FlowDirection == BulletGraphFlowDirection.Forward
                                                ? new Rect((i * gap) - element.DesiredSize.Height / 2, 0, element.DesiredSize.Width, element.DesiredSize.Height)
                                                : new Rect((i * gap) + element.DesiredSize.Height / 2, 0, element.DesiredSize.Width, element.DesiredSize.Height));
                        }
                    }
                    else
                    {
                        element.Arrange(new Rect((i * gap), 0, element.DesiredSize.Width, element.DesiredSize.Height));
                    }
                }
                i++;
            }

            return finalSize;
        }

        #endregion
    }

    #endregion
}
