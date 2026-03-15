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
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the panel where the series segments and adornments will be placed.
    /// </summary>
//    [ClassReference(IsReviewed = false)]
    public class ChartSeriesPanel : Canvas
    {
        #region fields

        private ChartSeries series;

        internal bool isarranged = false;

        #endregion

        #region Properties

        internal ChartSeries Series
        {
            get
            {
                return series;
            }
            set
            {
                if (series != null)
                {
                    series.Segments.CollectionChanged -= OnSegmentsCollectionChanged;
                }

                series = value;

                if (series != null)
                {
                    series.Segments.CollectionChanged += OnSegmentsCollectionChanged;
                    AddItems();
                }
            }
        }

        #endregion

        #region ctor

        public ChartSeriesPanel()
        {

        }

        #endregion

        #region methods

        private void OnSegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ChartSegment segment = e.NewItems[0] as ChartSegment;
                //segment.Interior = this.Series.GetInteriorColor(e.NewStartingIndex);
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
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ChartSegment segment = e.OldItems[0] as ChartSegment;
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
                for (int i = Children.Count-1; i >-1; i--)
                {
                    if (!(Children[i] is TrendlineBase))
                    {
                        Children.RemoveAt(i);
                    }
                }
              
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                ChartSegment segment = e.NewItems[0] as ChartSegment;
                if (!segment.IsAddedToVisualTree)
                {
                   UIElement element = segment.CreateSegmentVisual(Size.Empty);
                    
                    if (element != null)
                    {
                        Children.Add(element);
                        segment.IsAddedToVisualTree = true;
                    }
                }

                foreach (ChartSegment item in e.OldItems)
                {
                    var element = item.GetRenderedVisual();
                    if (Children.Contains(element))
                        Children.Remove(element);
                    item.IsAddedToVisualTree = false;
                }
            }
        }

        private void AddItems()
        {
            foreach (ChartSegment segment in Series.Segments)
            {
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
            bool canUpdate = !(Series is ISupportAxes);
            if (Series is ISupportAxes &&
                Series.ActualXAxis != null && Series.ActualYAxis != null)
            {
                canUpdate = true;
                if (Series.Area != null)
                {
                    Series.Area.ClearBuffer();
                }
            }          

            if (canUpdate)
            {
                IChartTransformer chartTransformer = Series.CreateTransformer(finalSize, true);
                if (Series is CircularSeriesBase)
                {
                    Rect rect = ChartLayoutUtils.Subtractthickness(new Rect(new Point(), finalSize), Series.Margin);
                    chartTransformer = Series.CreateTransformer(new Size(rect.Width, rect.Height), true);
                }
                foreach (ChartSegment segment in Series.Segments)
                {
                    segment.Update(chartTransformer);
                }

                if (Series.CanAnimate && Series.Segments.Count >0)
                {
                    Series.Animate();
                    Series.CanAnimate = false;
                }
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
