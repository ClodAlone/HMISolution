#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Controls;
using System.Windows;
#endif
#if !WPF
using System.Linq;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a uniform stack panel.
    /// </summary>
    public class UniformStackPanel : Panel
    {
        #region Contructor
        public UniformStackPanel()
        {
#if WINRT
            ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            ManipulationDelta += UniformStackPanel_ManipulationDelta;
#endif
        }
#if WINRT
        void UniformStackPanel_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                return;
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();
            ScheduleDaysView dayview = this.FindParentElementOfType<ScheduleDaysView>();
            if (Children.Count > 0 && Children[0].GetType() == typeof(ResourceHeaderItemsControl))
            {
                double delta = dayview.scrollviewer.HorizontalOffset + (e.Delta.Translation.X * -1);
                if (delta <= dayview.scrollviewer.ScrollableWidth)
                {
#if SyncfusionFramework4_5_11
                    dayview.scrollviewer.ChangeView(delta, null, null);
#else
                    dayview.scrollviewer.ScrollToHorizontalOffset(delta);
#endif
                    if (delta <= 0)
                    {
                        if (e.IsInertial)
                        {
                            e.Complete();
                        }
                        else
                        {
                            schedule.ScrollManipulationCompleted = true;
                        }
                    }
                }
                else
                {
                    if (e.IsInertial)
                    {
                        e.Complete();
                    }
                    else
                    {
                        schedule.ScrollManipulationCompleted = true;
                    }
                }

            }
            else if (Children.Count > 0 && Children[0].GetType() == typeof(CustomTextBlock) && Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
                {
                    schedule.ScrollManipulationCompleted = true;
                    if (e.IsInertial)
                    {
                        e.Complete();
                    }

                }
                else
                {
                    if (dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1) <= dayview.scrollviewer.ScrollableHeight && dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1) >= 0)
                    {
#if SyncfusionFramework4_5_11
                        dayview.scrollviewer.ChangeView(null, dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1), null);
#else
                        dayview.scrollviewer.ScrollToVerticalOffset(dayview.scrollviewer.VerticalOffset + (e.Delta.Translation.Y * -1));
#endif
                    }
                }
            }
            else if (Children.Count > 0 && Children[0].GetType() == typeof(TimeLineViewItemHeader))
            {
                if (Math.Abs(e.Delta.Translation.X) > Math.Abs(e.Delta.Translation.Y))
                {
                    schedule.ScrollManipulationCompleted = true;
                    if (e.IsInertial)
                    {
                        e.Complete();
                    }

                }
            }
        }
#endif
        #endregion

        #region Dependency properties

        #region Orientation

        /// <summary>
        /// Gets or sets the orientation by which child elements are stacked.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Event that is raised when <see cref="UniformStackPanel.Orientation"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OrientationChanged;

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformStackPanel), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (UniformStackPanel)d;
            instance.OnOrientationChanged(e);
        }

        /// <summary>
        /// Raises OrientationChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, e);
            }
        }

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count <= 0)
            {
                return new Size();
            }

#if WPF
            int visibleItemsCount = 0;
            foreach (UIElement child in Children)
            {
                if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                {
                    visibleItemsCount++;
                }
                else if (child.Visibility != Visibility.Collapsed)
                {
                    visibleItemsCount++;
                }
            }
#else
            int visibleItemsCount = Children.Count;
#endif

            Size itemSize = availableSize;
            var schedule = this.FindParentElementOfType<SfSchedule>();

            if (Orientation == Orientation.Horizontal)
            {
                if (!double.IsInfinity(itemSize.Width))
                {
                    if (Name == "timelinelabelspanel" && schedule != null && schedule.EnableAutoFormat && !schedule.isIntervalHeightset)
                        itemSize = availableSize;
                    else
                        itemSize.Width /= visibleItemsCount;
                }

                double maxHeight = 0.0, maxWidth = 0.0;

                foreach (UIElement item in Children)
                {
                    if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                    {
                        item.Measure(itemSize);
                        maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                        maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
                    }
                    else if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(itemSize);
                        maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                        maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
                    }
                }

                if (Name == "timelinelabelspanel" && schedule.EnableAutoFormat && !schedule.isIntervalHeightset)
                {
                    maxWidth = maxWidth < ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth ? ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth : maxWidth;
                }
                double mintotalwidth = maxWidth * (Children.Count);

                if (double.IsInfinity(itemSize.Width))
                {
                    itemSize.Width = mintotalwidth / visibleItemsCount;
                    itemSize.Height = maxHeight;

                    foreach (UIElement item in Children)
                    {
                        if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                        {
                            item.Measure(itemSize);
                        }
                        else if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(itemSize);
                        }
                    }
                }


                if (Name == "timelinelabelspanel" && schedule != null && schedule.EnableAutoFormat && !schedule.isIntervalHeightset)
                {
                    availableSize.Width = mintotalwidth;
                    var timeLineView = this.FindParentElementOfType<ScheduleTimeLineView>();
                    if (timeLineView != null && timeLineView.scheduleHorizontalTimeLineItemsControl != null)
                    {
                        timeLineView.scheduleHorizontalTimeLineItemsControl.Width = mintotalwidth;
                        if (timeLineView.NonAccessibleBlocks != null)
                            timeLineView.SetNonAccessibleBlocks();
                    }
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = maxWidth;
                }

                if (double.IsInfinity(availableSize.Height))
                {
                    availableSize.Height = maxHeight;
                }
            }
            else
            {
                if (!double.IsInfinity(itemSize.Height))
                {
                    itemSize.Height /= visibleItemsCount;
                }

                double maxWidth = 0.0;
                double height = 0.0;

                foreach (UIElement item in Children)
                {
                    if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                    {
                        item.Measure(itemSize);
                        height += double.IsInfinity(itemSize.Height) ? item.DesiredSize.Height : itemSize.Height;
                        maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                    }
                    else if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(itemSize);
                        height += double.IsInfinity(itemSize.Height) ? item.DesiredSize.Height : itemSize.Height;
                        maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                    }
                }

                if (double.IsInfinity(itemSize.Height))
                {
                    itemSize.Width = maxWidth;
                    itemSize.Height = height / visibleItemsCount;

                    foreach (UIElement item in Children)
                    {
                        if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                        {
                            item.Measure(itemSize);
                        }
                        else if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(itemSize);
                        }
                    }
                }

                if (double.IsInfinity(availableSize.Height))
                {
                    availableSize.Height = height;
                }

                if (double.IsInfinity(availableSize.Width))
                {
                    availableSize.Width = maxWidth;
                }
            }

            return availableSize;
        }

        /// <summary>
        /// Provides the behavior for the "arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count <= 0)
            {
                return finalSize;
            }

#if WPF
            int visibleItemsCount = 0;
            foreach (UIElement child in Children)
            {
                if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                {
                    visibleItemsCount++;
                }
                else if (child.Visibility != Visibility.Collapsed)
                {
                    visibleItemsCount++;
                }
            }
#else
            int visibleItemsCount = (Name == "timelineitemspanel" || Name == "timelinelabelspanel") ? Children.Count :
                                    Children.Count(child => child.Visibility != Visibility.Collapsed);
#endif

            if (Orientation == Orientation.Horizontal)
            {
                double itemWidth = finalSize.Width / visibleItemsCount;
                var itemRect = new Rect(0, 0, itemWidth, finalSize.Height);

                foreach (UIElement item in Children)
                {
                    if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                    {
                        item.Arrange(itemRect);
                        itemRect.X += itemWidth;
                    }
                    else if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Arrange(itemRect);
                        itemRect.X += itemWidth;
                    }
                }
            }
            else
            {
                double itemHeight = finalSize.Height / visibleItemsCount;
                var itemRect = new Rect(0, 0, finalSize.Width, itemHeight);
                foreach (UIElement item in Children)
                {
                    if (Name == "timelineitemspanel" || Name == "timelinelabelspanel")
                    {
                        item.Arrange(itemRect);
                        itemRect.Y += itemHeight;
                    }
                    else if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Arrange(itemRect);
                        itemRect.Y += itemHeight;
                    }
                }
            }

            return finalSize;
        }

        #endregion
    }
}
