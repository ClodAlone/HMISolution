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
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartSelectionBehavior enables the selection of segments in a Chart.
    /// </summary>
    /// <remarks>
    /// The selected segment can be displayed with a different color specified using SegmentSelectionBrush property available in corresponding series.
    /// ChartSelectionBehavior is applicable only to certain series such as <see cref="ColumnSeries"/>,<see cref="BarSeries"/>,
    /// <see cref="RangeColumnSeries"/>,<see cref="StackingBarSeries"/>,<see cref="StackingColumnSeries"/>,<see cref="ScatterSeries"/>,
    /// <see cref="BubbleSeries"/>,<see cref="PieSeries"/>.
    /// </remarks>
    public class ChartSelectionBehavior : ChartBehavior
    {
        private ChartSegment mouseUnderSegment;

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartSelectionBehavior()
        {

        }
#if WINDOWS_PHONE
        /// <summary>
        /// Called when OnMouse
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        /// <summary>
        /// Called when Pointer Released in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
            FrameworkElement element = e.OriginalSource as FrameworkElement;

            if (element != null && element.Tag != null)
            {
                ChartSegment segment = element.Tag as ChartSegment;
                if (segment != null && segment.Series is ISegmentSelectable && (segment.Series as ISegmentSelectable).SegmentSelectionBrush != null)
                {
                    if (segment == mouseUnderSegment)
                    {
                        mouseUnderSegment = segment;
                        if (mouseUnderSegment.Series.PreviousSelectedSegment != null)
                        {
                            mouseUnderSegment.Series.PreviousSelectedSegment.BindProperties();
                        }
                        segment.Series.PreviousSelectedSegment = segment;
                        OnSelectionChanged(mouseUnderSegment);
                    }
                }
            }
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when MouseLeftButtonDown in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
            FrameworkElement element = e.OriginalSource as FrameworkElement;
            if (element != null && element.Tag != null)
            {
                ChartSegment segment = element.Tag as ChartSegment;
                if (segment != null && segment.Series is ISegmentSelectable)
                {
                    mouseUnderSegment = segment;
                }
            }
        }
        /// <summary>
        /// Called when Selection changed
        /// </summary>
        /// <param name="segment"></param>
        protected virtual void OnSelectionChanged(ChartSegment segment)
        {
            if (segment != null)
            {
                Binding binding =new Binding();
                binding.Source = segment.Series;
                binding.Path = new PropertyPath("SegmentSelectionBrush");
                BindingOperations.SetBinding(segment, ChartSegment.InteriorProperty, binding);
                ChartSelectionChangedEventArgs eventArgs = new ChartSelectionChangedEventArgs();
                eventArgs.SelectedSegment = segment;
                eventArgs.SelectedSeries = segment.Series;
                this.ChartArea.OnSelectionChanged(eventArgs);
            }
        }
    }
}
