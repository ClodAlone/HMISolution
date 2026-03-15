#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;


#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class ChartTrendlinePanel :Canvas
    {
        #region fields

        private TrendlineBase trend;

        internal bool isarranged = false;

        #endregion

        #region Properties

        internal TrendlineBase Trend
        {
            get
            {
                return trend;
            }
            set
            {
                if (Trend != null)
                {
                    Trend.TrendlineSegments.CollectionChanged -= OnSegmentsCollectionChanged;
                      
                }

                trend = value;

                if (trend != null)
                {
                    trend.TrendlineSegments.CollectionChanged += OnSegmentsCollectionChanged;
                }
            }
        }

       
       

        #endregion

        #region ctor

        public ChartTrendlinePanel()
        {

        }

        #endregion

        #region methods
      
        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.CreateVisual(Size.Empty);
                    if (element != null)
                    {
                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }
               
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var segment = e.OldItems[0] as ChartSegment;
                if (segment.IsAddedToVisualTree)
                {
                    UIElement element = segment.GetRenderedVisual();
                    if (element != null && Children.Contains(element))
                    {
                        Children.Remove(element);
                        segment.IsAddedToVisualTree = false;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Children.Clear();
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                   UIElement element = segment.CreateSegmentVisual(Size.Empty);
                    
                    if (element != null)
                    {
                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }
            }
        }

        internal void Update(Size finalSize)
        {
                IChartTransformer chartTransformer = Trend.Series.CreateTransformer(finalSize, true);

                foreach (var segment in Trend.TrendlineSegments)
                {
                    segment.Update(chartTransformer);
                }
        }

#if WPF
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

            base.ArrangeOverride(finalSize);
            return finalSize;
        }

#endif

        #endregion
    }
}
