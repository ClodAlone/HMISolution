#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WINRT
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    internal class ResizeBehavior : DependencyObject
    {
        #region Constructor

        public ResizeBehavior()
        {
            _associateobj = AssociatedObject;
        }

        #endregion

        #region Enum Property

        #region ControlParts

        internal enum ControlParts
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
        }

        #endregion

        #endregion

        #region Private Fields

        bool isResizing;

        #endregion

        #region InternalFields

        internal FrameworkElement AssociatedObject { get; set; }
        internal FrameworkElement _associateobj;
        internal ScheduleHorizontalTimeSlotItemsControl timeSlot;
        internal ScheduleHorizontalTimeSlotControl horTimeSlot;
        internal SfSchedule sfSchedule;
        internal ControlParts resizePart;
        internal Storyboard EllipseAnimation1, EllipseAnimation2;
        internal double scrollBoundY1;
        internal double scrollBoundY2;
        internal double scrollBoundX1;
        internal double scrollBoundX2;
#if !WINRT
        internal double Y2;
        internal double X2;
#endif

        #endregion

        #region Dependency Properties

        #region Size Properties

        #region MinHeight

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register("MinHeight", typeof(Double), typeof(ResizeBehavior),
            new PropertyMetadata(15.0, OnSizeChanged));

        /// <summary>
        /// Set the minimum resize height
        /// </summary>
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        #endregion

        #region MaxHeight

        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register("MaxHeight", typeof(Double), typeof(ResizeBehavior),
            new PropertyMetadata(400.0, OnSizeChanged));

        /// <summary>
        /// Set the maximum resize height
        /// </summary>
        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        #endregion

        #region MinWidth

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof(Double), typeof(ResizeBehavior),
            new PropertyMetadata(15.0, OnSizeChanged));

        /// <summary>
        /// Set the minimum resize width
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        #endregion

        #region MaxWidth

        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register("MaxWidth", typeof(Double), typeof(ResizeBehavior),
            new PropertyMetadata(400.0, OnSizeChanged));

        /// <summary>
        /// Set the maximum resize width
        /// </summary>
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }



        private static void OnSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ResizeBehavior)obj;

            source.checkSizeValues();
        }
        #endregion

        #endregion

        #region Appareance Properties

        #region DragSpace

        public static readonly DependencyProperty DragSpaceProperty =
            DependencyProperty.Register("DragSpace", typeof(double), typeof(ResizeBehavior),
            new PropertyMetadata(15d, OnAppareanceElementChanged));

        /// <summary>
        /// Set the space from border for start resize mode
        /// </summary>
        public double DragSpace
        {
            get { return (double)GetValue(DragSpaceProperty); }
            set { SetValue(DragSpaceProperty, value); }
        }



        #endregion

        #region DragIndicatorsFill

        public static readonly DependencyProperty DragIndicatorsFillProperty =
            DependencyProperty.Register("DragIndicatorsFill", typeof(Brush), typeof(ResizeBehavior),
            new PropertyMetadata(new SolidColorBrush(Colors.Yellow), OnAppareanceElementChanged));

        /// <summary>
        /// Set the drags indicator BackGround
        /// </summary>
        public Brush DragIndicatorsFill
        {
            get { return (Brush)GetValue(DragIndicatorsFillProperty); }
            set { SetValue(DragIndicatorsFillProperty, value); }
        }

        #endregion

        #region DragIndicatorsOpacity

        public static readonly DependencyProperty DragIndicatorsOpacityProperty =
            DependencyProperty.Register("DragIndicatorsOpacity", typeof(double), typeof(ResizeBehavior),
            new PropertyMetadata(0.8, OnAppareanceElementChanged));

        /// <summary>
        /// Set the drags indicator Opacity
        /// </summary>
        public Double DragIndicatorsOpacity
        {
            get { return (Double)GetValue(DragIndicatorsOpacityProperty); }
            set { SetValue(DragIndicatorsOpacityProperty, value); }
        }



        private static void OnAppareanceElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

            var source = (ResizeBehavior)obj;

            if (source.AssociatedObject == null) return;

            source.setEllipse(source.resizePart);
        }
        #endregion

        #endregion

        #region Comportment

        #region IsLeftDraggable

        public static readonly DependencyProperty IsLeftDraggableProperty =
            DependencyProperty.Register("IsLeftDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the left part
        /// </summary>
        public bool IsLeftDraggable
        {
            get { return (bool)GetValue(IsLeftDraggableProperty); }
            set { SetValue(IsLeftDraggableProperty, value); }
        }

        #endregion

        #region IsRightDraggable

        public static readonly DependencyProperty IsRightDraggableProperty =
            DependencyProperty.Register("IsRightDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the right part
        /// </summary>
        public bool IsRightDraggable
        {
            get { return (bool)GetValue(IsRightDraggableProperty); }
            set { SetValue(IsRightDraggableProperty, value); }
        }

        #endregion

        #region IsTopDraggable

        public static readonly DependencyProperty IsTopDraggableProperty =
            DependencyProperty.Register("IsTopDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the top part
        /// </summary>
        public bool IsTopDraggable
        {
            get { return (bool)GetValue(IsTopDraggableProperty); }
            set { SetValue(IsTopDraggableProperty, value); }
        }

        #endregion

        #region IsBottomDraggable

        public static readonly DependencyProperty IsBottomDraggableProperty =
            DependencyProperty.Register("IsBottomDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the bottom part
        /// </summary>
        public bool IsBottomDraggable
        {
            get { return (bool)GetValue(IsBottomDraggableProperty); }
            set { SetValue(IsBottomDraggableProperty, value); }
        }

        #endregion

        #region IsTopLeftDraggable

        public static readonly DependencyProperty IsTopLeftDraggableProperty =
            DependencyProperty.Register("IsTopLeftDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the top left corner
        /// </summary>
        public bool IsTopLeftDraggable
        {
            get { return (bool)GetValue(IsTopLeftDraggableProperty); }
            set { SetValue(IsTopLeftDraggableProperty, value); }
        }

        #endregion

        #region IsTopRightDraggable

        public static readonly DependencyProperty IsTopRightDraggableProperty =
            DependencyProperty.Register("IsTopRightDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the top right corner
        /// </summary>
        public bool IsTopRightDraggable
        {
            get { return (bool)GetValue(IsTopRightDraggableProperty); }
            set { SetValue(IsTopRightDraggableProperty, value); }
        }

        #endregion

        #region IsBottomLeftDraggable

        public static readonly DependencyProperty IsBottomLeftDraggableProperty =
            DependencyProperty.Register("IsBottomLeftDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the bottom left corner
        /// </summary>
        public bool IsBottomLeftDraggable
        {
            get { return (bool)GetValue(IsBottomLeftDraggableProperty); }
            set { SetValue(IsBottomLeftDraggableProperty, value); }
        }

        #endregion

        #region IsBottomRightDraggable

        public static readonly DependencyProperty IsBottomRightDraggableProperty =
            DependencyProperty.Register("IsBottomRightDraggable", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true));

        /// <summary>
        /// Indicate if is possible resize dragging the bottom right corner
        /// </summary>
        public bool IsBottomRightDraggable
        {
            get { return (bool)GetValue(IsBottomRightDraggableProperty); }
            set { SetValue(IsBottomRightDraggableProperty, value); }
        }

        #endregion


        #region StayInParent

        public static readonly DependencyProperty StayInParentProperty =
            DependencyProperty.Register("StayInParent", typeof(bool), typeof(ResizeBehavior),
            new PropertyMetadata(true, StayInParentPropertyChanged));

        /// <summary>
        /// Indicate if the element stay or not into parent bounds
        /// </summary>
        public bool StayInParent
        {
            get { return (bool)GetValue(StayInParentProperty); }
            set { SetValue(StayInParentProperty, value); }
        }


        private static void StayInParentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var source = (ResizeBehavior)obj;

            source.checkParentBounds();
        }

        #endregion

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Espose this event because when size is changed by bahavior the FrameworkElement.SizeChanged event
        /// is not raised.
        /// </summary>
        public event EventHandler SizeChanged;

        private void raiseEvent(EventHandler e)
        {
            raiseEvent(e, EventArgs.Empty);
        }

        private void raiseEvent(EventHandler e, EventArgs args)
        {
            if (e != null) e(this, args);
        }

        #region Associated object events
#if WINRT
        void AssociatedObject_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isResizing)
            {
                var CurrentPositionInScroll = new Point();
                if (sfSchedule.ScheduleType == ScheduleType.Day || sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.WorkWeek)
                    CurrentPositionInScroll = e.GetCurrentPoint(sfSchedule.dayScrollViewer).Position;
                else if (sfSchedule.ScheduleType == ScheduleType.TimeLine)
                    CurrentPositionInScroll = e.GetCurrentPoint(sfSchedule.timelineScrollViewer).Position;
                resize(e.GetCurrentPoint(AssociatedObject).Position, e.GetCurrentPoint(AssociatedObject.FindParentElementOfType<Canvas>()).Position, CurrentPositionInScroll);

                #region DragDropTimer

                if (resizePart == ControlParts.Top)
                {
                    var g = VisualTreeHelper.GetParent(sender as Grid);
                    var scheduleDaysAppointmentViewControl = g as ScheduleDaysAppointmentViewControl;
                    if (scheduleDaysAppointmentViewControl != null)
                    {
                        var viewc = scheduleDaysAppointmentViewControl.schedule;
                        var viewcontrol = (viewc.flipviewselecteditem as FrameworkElement).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                        var selectedDate = viewc.GetCurrentDropLocation(viewcontrol, e);
                        var dayView = scheduleDaysAppointmentViewControl;
                        dayView.DragDropStartTime = selectedDate.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    }
                }
                else if (resizePart == ControlParts.Bottom)
                {
                    var g = VisualTreeHelper.GetParent(sender as Grid);
                    var scheduleDaysAppointmentViewControl = g as ScheduleDaysAppointmentViewControl;
                    if (scheduleDaysAppointmentViewControl != null)
                    {
                        var viewc = scheduleDaysAppointmentViewControl.schedule;
                        var viewcontrol = (viewc.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                        var selectedDate = viewc.GetCurrentDropLocation(viewcontrol, e);
                        var dayView = scheduleDaysAppointmentViewControl;
                        double DatetimeDiff = (scheduleDaysAppointmentViewControl.ActualHeight) / viewc.IntervalHeight;
                        DateTime currentDate = selectedDate;
                        DateTime endDate = currentDate.AddMinutes(viewc.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        dayView.DragDropEndTime = endDate.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    }
                }

                else if (resizePart == ControlParts.Left)
                {
                    var g = VisualTreeHelper.GetParent(sender as Grid);
                    var scheduleHorizontalAppointmentViewControl = g as ScheduleHorizontalAppointmentViewControl;
                    if (scheduleHorizontalAppointmentViewControl != null)
                    {
                        var viewc = scheduleHorizontalAppointmentViewControl.schedule;
                        var viewcontrol = (viewc.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                        var selectedDate = viewc.GetCurrentDropLocation(viewcontrol, e);
                        var dayView = scheduleHorizontalAppointmentViewControl;
                        dayView.DragDropStartTime = selectedDate.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    }
                }
                else if (resizePart == ControlParts.Right)
                {
                    var g = VisualTreeHelper.GetParent(sender as Grid);
                    var scheduleHorizontalAppointmentViewControl = g as ScheduleHorizontalAppointmentViewControl;
                    if (scheduleHorizontalAppointmentViewControl != null)
                    {
                        var viewc = scheduleHorizontalAppointmentViewControl.schedule;
                        var viewcontrol = (viewc.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                        var selectedDate = viewc.GetCurrentDropLocation(viewcontrol, e);
                        var dayView = scheduleHorizontalAppointmentViewControl;
                        var timelineView = viewcontrol.Content as ScheduleTimeLineView;
                        double DatetimeDiff;
                        if (viewc.isIntervalHeightset)
                        {
                            DatetimeDiff = (scheduleHorizontalAppointmentViewControl.ActualWidth) / (viewc.IntervalHeight);
                        }
                        else
                        {
                            DatetimeDiff = (scheduleHorizontalAppointmentViewControl.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth; // Since 80 is the default interval height for timeline view.
                        }
                        DateTime currentDate = selectedDate;
                        DateTime endDate = currentDate.AddMinutes(viewc.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        if (timelineView != null && timelineView.SelectedDates.Count > 1 && !timelineView.ShowNonWorkingHours &&
                            endDate.TimeOfDay.TotalMinutes > timelineView.WorkEndHour * 60)
                        {
                            TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, timelineView.WorkStartHour - timelineView.WorkEndHour, 0, 0);
                            endDate = endDate + nonWorkingHourTimeSpan;
                        }
                        dayView.DragDropEndTime = endDate.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    }
                }
                #endregion
            }
        }
#else
        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                var CurrentPositionInScroll = new Point();
                if (sfSchedule.ScheduleType == ScheduleType.Day || sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.WorkWeek)
                    CurrentPositionInScroll = e.GetPosition(sfSchedule.dayScrollViewer);
                else if (sfSchedule.ScheduleType == ScheduleType.TimeLine)
                    CurrentPositionInScroll = e.GetPosition(sfSchedule.timelineScrollViewer);
                resize(e.GetPosition(AssociatedObject), e.GetPosition(AssociatedObject.FindParentElementOfType<Canvas>()), CurrentPositionInScroll);

        #region DragDropTimer

                if (sender is Grid)
                {
                    var g = VisualTreeHelper.GetParent(sender as Grid);
                    if (resizePart == ControlParts.Top)
                    {
                        if (g is ScheduleDaysAppointmentViewControl)
                        {
                            var dayView = g as ScheduleDaysAppointmentViewControl;
                            var viewc = dayView.schedule;
                            ScheduleAppointment dropappointment = sfSchedule.SelectedAppointment;

                            TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                            double DayDiff = dropappointment.InternalEndTime.DayOfYear - dropappointment.InternalStartTime.DayOfYear;
                            var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                            double DatetimeDiff = (sfSchedule.dvc.ActualHeight) / viewc.IntervalHeight;
                            DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            if (selectedDate == EndDate)
                            {
                                EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                            }
                            if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                            {
                                EndDate = selectedDate.Add(datetimeDiff);
                                var scheduleDaysAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl;
                                if (scheduleDaysAppointmentViewControl != null)
                                {
                                    double height;
                                    if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                        selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                                    {
                                        TimeSpan timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                        height = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * viewc.IntervalHeight;
                                    }
                                    else
                                    {
                                        TimeSpan timeHeigth = EndDate - selectedDate;
                                        height = (timeHeigth.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * viewc.IntervalHeight;
                                    }
                                    scheduleDaysAppointmentViewControl.Height = height;
                                }
                            }
                            if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                            {
                                sfSchedule.AddResources(dropappointment);
                            }
                            DateTime daystarttime, dayendtime;
                            if (!sfSchedule.allDayFlag)
                            {
                                daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                                dayView.DragDropStartTime = daystarttime.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                                dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                            }

                        }
                    }
                    else if (resizePart == ControlParts.Bottom)
                    {
                        if (g is ScheduleDaysAppointmentViewControl)
                        {
                            var dayView = g as ScheduleDaysAppointmentViewControl;
                            var viewc = dayView.schedule;
                            ScheduleAppointment dropappointment = sfSchedule.SelectedAppointment;

                            TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                            double DayDiff = dropappointment.InternalEndTime.DayOfYear - dropappointment.InternalStartTime.DayOfYear;
                            var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                            double DatetimeDiff = (sfSchedule.dvc.ActualHeight) / viewc.IntervalHeight;
                            DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            if (selectedDate == EndDate)
                            {
                                EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                            }
                            if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                            {
                                EndDate = selectedDate.Add(datetimeDiff);
                                var scheduleDaysAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl;
                                if (scheduleDaysAppointmentViewControl != null)
                                {
                                    double height;
                                    if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                        selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                                    {
                                        TimeSpan timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                        height = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * viewc.IntervalHeight;
                                    }
                                    else
                                    {
                                        TimeSpan timeHeigth = EndDate - selectedDate;
                                        height = (timeHeigth.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * viewc.IntervalHeight;
                                    }
                                    scheduleDaysAppointmentViewControl.Height = height;
                                }
                            }
                            if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                            {
                                sfSchedule.AddResources(dropappointment);
                            }
                            DateTime daystarttime, dayendtime;
                            if (!sfSchedule.allDayFlag)
                            {
                                daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                                dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                                dayView.DragDropEndTime = dayendtime.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");

                            }

                        }
                    }
                    else if (resizePart == ControlParts.Left)
                    {
                        if (g is ScheduleHorizontalAppointmentViewControl)
                        {
                            var dayView = g as ScheduleHorizontalAppointmentViewControl;
                            var viewc = dayView.schedule;

                            var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                            if (dropappointment != null)
                            {
                                double DatetimeDiff = 0;
                                TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                                double DayDiff = dropappointment.InternalEndTime.DayOfYear - dropappointment.InternalStartTime.DayOfYear;
                                var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                                if (!(selectedDate == new DateTime()))
                                {
                                    if (DatetimeDiff.Equals(0))
                                    {
                                        if (sfSchedule.isIntervalHeightset)
                                        {
                                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (viewc.IntervalHeight);
                                        }
                                        else
                                        {
                                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                        }
                                    }
                                    DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                                    if (selectedDate == EndDate)
                                    {
                                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                                    }
                                    var selectedDatesCount = viewc.SelectedDates.Count;
                                    if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                                    {
                                        EndDate = selectedDate.Add(datetimeDiff);
                                        var scheduleHorizontalAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl;
                                        if (scheduleHorizontalAppointmentViewControl != null)
                                        {
                                            TimeSpan timeHeight;
                                            if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                                selectedDatesCount == 1 && selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                                            {
                                                timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                            }
                                            else
                                            {
                                                timeHeight = EndDate - selectedDate;
                                            }
                                            double intHeight = sfSchedule.isIntervalHeightset ? viewc.IntervalHeight : ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                            double width = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * intHeight;
                                            scheduleHorizontalAppointmentViewControl.Width = width;
                                            scheduleHorizontalAppointmentViewControl.AppWidth = width;
                                        }
                                    }

                                    DateTime starttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                                    DateTime endtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                                    dayView.DragDropStartTime = starttime.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                                }
                            }
                        }
                    }
                    else if (resizePart == ControlParts.Right)
                    {
                        if (g is ScheduleHorizontalAppointmentViewControl)
                        {
                            var dayView = g as ScheduleHorizontalAppointmentViewControl;
                            var viewc = dayView.schedule;
                            var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
#if WPF
                            var viewcontrol = (viewc.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
#else
                            var viewcontrol = (viewc.mainViewItem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems"); 
#endif
                            var timelineView = viewcontrol.Content as ScheduleTimeLineView;
                            if (dropappointment != null)
                            {
                                double DatetimeDiff = 0;
                                TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                                double DayDiff = dropappointment.InternalEndTime.DayOfYear - dropappointment.InternalStartTime.DayOfYear;
                                var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                                if (!(selectedDate == new DateTime()))
                                {
                                    if (DatetimeDiff.Equals(0))
                                    {
                                        if (sfSchedule.isIntervalHeightset)
                                        {
                                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (viewc.IntervalHeight);
                                        }
                                        else
                                        {
                                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                        }
                                    }
                                    DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                                    if (selectedDate == EndDate)
                                    {
                                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                                    }
                                    var selectedDatesCount = viewc.SelectedDates.Count;
                                    if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                                    {
                                        EndDate = selectedDate.Add(datetimeDiff);
                                        var scheduleHorizontalAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl;
                                        if (scheduleHorizontalAppointmentViewControl != null)
                                        {
                                            TimeSpan timeHeight;
                                            if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                                selectedDatesCount == 1 && selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                                            {
                                                timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                            }
                                            else
                                            {
                                                timeHeight = EndDate - selectedDate;
                                            }
                                            double intHeight = sfSchedule.isIntervalHeightset ? viewc.IntervalHeight : ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                            double width = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * intHeight;
                                            scheduleHorizontalAppointmentViewControl.Width = width;
                                            scheduleHorizontalAppointmentViewControl.AppWidth = width;
                                        }
                                    }

                                    DateTime starttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                                    DateTime endDate = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                                    if (timelineView != null && timelineView.SelectedDates.Count > 1 && !timelineView.ShowNonWorkingHours &&
                                        endDate.TimeOfDay.TotalMinutes > timelineView.WorkEndHour * 60)
                                    {
                                        TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, timelineView.WorkStartHour - timelineView.WorkEndHour, 0, 0);
                                        endDate = endDate + nonWorkingHourTimeSpan;
                                    }
                                    dayView.DragDropEndTime = endDate.ToString(viewc.TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                                }
                            }
                        }
                    }
                }
        #endregion
            }
        }
#endif

#if WINRT
        void AssociatedObject_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.ReleasePointerCapture(e.Pointer);
                isResizing = false;
            }
        }
#else
        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AssociatedObject != null)
                AssociatedObject.ReleaseMouseCapture();
            isResizing = false;
        }
#endif

#if WINRT
        void AssociatedObject_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.OriginalSource is Ellipse)
            {
                AssociatedObject.CapturePointer(e.Pointer);
                isResizing = true;
                var CurrentPositionInScroll = new Point();
                if (sfSchedule.ScheduleType == ScheduleType.Day || sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.WorkWeek)
                    CurrentPositionInScroll = e.GetCurrentPoint(sfSchedule.dayScrollViewer).Position;
                else if (sfSchedule.ScheduleType == ScheduleType.TimeLine)
                    CurrentPositionInScroll = e.GetCurrentPoint(sfSchedule.timelineScrollViewer).Position;
                resizePart = getResizePart(e.GetCurrentPoint(AssociatedObject).Position);
                if (resizePart == ControlParts.Bottom)
                {
                    scrollBoundY1 = CurrentPositionInScroll.Y + sfSchedule.dayScrollViewer.VerticalOffset - AssociatedObject.ActualHeight;
                }
                else if (resizePart == ControlParts.Top)
                {
                    scrollBoundY2 = CurrentPositionInScroll.Y + sfSchedule.dayScrollViewer.VerticalOffset + AssociatedObject.ActualHeight;
                }
                else if (resizePart == ControlParts.Left)
                {
                    scrollBoundX2 = CurrentPositionInScroll.X + sfSchedule.timelineScrollViewer.HorizontalOffset + AssociatedObject.ActualWidth;
                }
                else if (resizePart == ControlParts.Right)
                {
                    scrollBoundX1 = CurrentPositionInScroll.X + sfSchedule.timelineScrollViewer.HorizontalOffset - AssociatedObject.ActualWidth;
                }


            }
        }
#else
        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Ellipse)
            {
                AssociatedObject.CaptureMouse();
                isResizing = true;
                if (VisualTreeHelper.GetParent(AssociatedObject).GetType().Name == "ScheduleDaysAppointmentViewControl")
                    Y2 = AssociatedObject.ActualHeight + Canvas.GetTop(VisualTreeHelper.GetParent(AssociatedObject) as ScheduleDaysAppointmentViewControl);
                else if (VisualTreeHelper.GetParent(AssociatedObject).GetType().Name == "ScheduleHorizontalAppointmentViewControl")
                    X2 = AssociatedObject.ActualWidth + Canvas.GetLeft(VisualTreeHelper.GetParent(AssociatedObject) as ScheduleHorizontalAppointmentViewControl);
                resizePart = getResizePart(e.GetPosition(AssociatedObject));
                Point CurrentPositionInScroll = new Point();
                if (sfSchedule.ScheduleType == ScheduleType.Day || sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.WorkWeek)
                    CurrentPositionInScroll = e.GetPosition(sfSchedule.dayScrollViewer);
                else if (sfSchedule.ScheduleType == ScheduleType.TimeLine)
                    CurrentPositionInScroll = e.GetPosition(sfSchedule.timelineScrollViewer);
                resizePart = getResizePart(e.GetPosition(AssociatedObject));
                if (resizePart == ControlParts.Bottom)
                {
                    scrollBoundY1 = CurrentPositionInScroll.Y + sfSchedule.dayScrollViewer.VerticalOffset - AssociatedObject.ActualHeight;
                }
                else if (resizePart == ControlParts.Top)
                {
                    scrollBoundY2 = CurrentPositionInScroll.Y + sfSchedule.dayScrollViewer.VerticalOffset + AssociatedObject.ActualHeight;
                }
                else if (resizePart == ControlParts.Left)
                {
                    scrollBoundX2 = CurrentPositionInScroll.X + sfSchedule.timelineScrollViewer.HorizontalOffset + AssociatedObject.ActualWidth;
                }
                else if (resizePart == ControlParts.Right)
                {
                    scrollBoundX1 = CurrentPositionInScroll.X + sfSchedule.timelineScrollViewer.HorizontalOffset - AssociatedObject.ActualWidth;
                }
            }
        }
#endif
        #endregion

        #endregion

        #region Methods

        #region behaviour elements implementation

        internal void Attach(FrameworkElement ass_obj)
        {
            AssociatedObject = ass_obj;
            var Parentcanvas = AssociatedObject.FindParentElementOfType<Canvas>();
            var schedule = Parentcanvas.FindParentElementOfType<SfSchedule>();
            sfSchedule = schedule;
#if WINRT
            timeSlot = ((schedule.flipviewselecteditem as Grid).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems").Content as ScheduleDaysView).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            horTimeSlot = ((schedule.flipviewselecteditem as Grid).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems").Content as ScheduleTimeLineView).FindElementOfType<ScheduleHorizontalTimeSlotControl>();
#else
            var border = VisualTreeHelper.GetChild(schedule, 0) as Border;
            if (border != null)
            {
                var grid = border.FindName("PART_MainItem") as Grid;
                if (grid != null)
                {
                    var contentControl = schedule.currentSelectedItem;
                    if (contentControl != null)
                    {
                        timeSlot = (contentControl.Content as ScheduleDaysView).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
                        horTimeSlot = (contentControl.Content as ScheduleTimeLineView).FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                    }
                }
            }
#endif
            if (AssociatedObject != null && Parentcanvas != null)
            {
                var Dayapp = AssociatedObject.FindParentElementOfType<ScheduleDaysAppointmentViewControl>();
                var Monthapp = AssociatedObject.FindParentElementOfType<ScheduleMonthAppointmentViewControl>();
                if (Monthapp != null)
                {
                    MaxHeight = Parentcanvas.ActualHeight;
                    MaxWidth = Parentcanvas.ActualWidth;
                    MinHeight = AssociatedObject.ActualHeight;
                    MinWidth = Parentcanvas.ActualWidth / 7;
                    isResizing = false;
                    resizePart = ControlParts.None;
                    RemoveEllipse();
                    setEllipse(ControlParts.Left);
                    setEllipse(ControlParts.Right);
                }
                else if (Dayapp != null)
                {
                    MaxHeight = Parentcanvas.ActualHeight;
                    MaxWidth = Parentcanvas.ActualWidth;
                    MinHeight = 15;
                    MinWidth = Parentcanvas.ActualWidth / Dayapp.schedule.SelectedDates.Count;
                    isResizing = false;
                    resizePart = ControlParts.None;
                    RemoveEllipse();
                    setEllipse(ControlParts.Top);
                    setEllipse(ControlParts.Bottom);
                }
                else
                {
                    MaxHeight = Parentcanvas.ActualHeight;
                    MaxWidth = Parentcanvas.ActualWidth;
                    MinHeight = AssociatedObject.ActualHeight;
                    MinWidth = 15;
                    isResizing = false;
                    resizePart = ControlParts.None;
                    RemoveEllipse();
                    setEllipse(ControlParts.Left);
                    setEllipse(ControlParts.Right);
                }
                checkSizeValues();
                checkParentBounds();
#if WINRT
                AssociatedObject.PointerPressed += AssociatedObject_PointerPressed;
                AssociatedObject.PointerReleased += AssociatedObject_PointerReleased;
                AssociatedObject.PointerMoved += AssociatedObject_PointerMoved;
#else
                AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
                AssociatedObject.MouseMove += AssociatedObject_MouseMove;
#endif

            }
        }

        internal void Ellipse1IdealAnimation(UIElement ellipse)
        {
            EllipseAnimation1 = new Storyboard();
            #region DoubleAnimation1
#if WINRT
            var dbAniKeyFrame1 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(dbAniKeyFrame1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#else
            var dbAniKeyFrame1 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(dbAniKeyFrame1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
#endif
            Storyboard.SetTarget(dbAniKeyFrame1, ellipse);


            #region Easing for dbAniKeyFrame1
            var dbkeyframe = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() };
            #endregion
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe);
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe1);
            #endregion

            #region DoubleAnimation2
#if WINRT
            var dbAniKeyFrame2 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(dbAniKeyFrame2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#else
            var dbAniKeyFrame2 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(dbAniKeyFrame2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
#endif
            Storyboard.SetTarget(dbAniKeyFrame2, ellipse);


            #region Easing for dbAniKeyFrame2
            var dbkeyframe2 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe3 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe2);
            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe3);
            #endregion

            #region DoubleAnimation3
#if WINRT
            var dbAniKeyFrame3 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(dbAniKeyFrame3, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
#else
            var dbAniKeyFrame3 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(dbAniKeyFrame3, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
#endif
            Storyboard.SetTarget(dbAniKeyFrame3, ellipse);


            #region Easing for dbAniKeyFrame3
            var dbkeyframe4 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe5 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe4);
            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe5);

            #endregion

            #region DoubleAnimation4
#if WINRT
            var dbAniKeyFrame4 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(dbAniKeyFrame4, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
#else
            var dbAniKeyFrame4 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(dbAniKeyFrame4, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#endif
            Storyboard.SetTarget(dbAniKeyFrame4, ellipse);


            #region Easing for dbAniKeyFrame4
            var dbkeyframe6 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe7 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe6);
            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe7);
            #endregion

            #region DoubleAnimation5
#if WINRT
            var dbAniKeyFrame5 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTargetProperty(dbAniKeyFrame5, "(UIElement.Opacity)");
#else
            var dbAniKeyFrame5 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTargetProperty(dbAniKeyFrame5, new PropertyPath("(UIElement.Opacity)"));
#endif
            Storyboard.SetTarget(dbAniKeyFrame5, ellipse);


            #region Easing for dbAniKeyFrame5
            var dbkeyframe8 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe9 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame5.KeyFrames.Add(dbkeyframe8);
            dbAniKeyFrame5.KeyFrames.Add(dbkeyframe9);
            #endregion

            EllipseAnimation1.Children.Add(dbAniKeyFrame1);
            EllipseAnimation1.Children.Add(dbAniKeyFrame2);
            EllipseAnimation1.Children.Add(dbAniKeyFrame3);
            EllipseAnimation1.Children.Add(dbAniKeyFrame4);
            EllipseAnimation1.Children.Add(dbAniKeyFrame5);
        }

        internal void Ellipse2IdealAnimation(UIElement ellipse)
        {
            EllipseAnimation2 = new Storyboard();
            #region DoubleAnimation1
#if WINRT
            var dbAniKeyFrame1 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTarget(dbAniKeyFrame1, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
#else
            var dbAniKeyFrame1 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTarget(dbAniKeyFrame1, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame1, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleX)"));
#endif

            #region Easing for dbAniKeyFrame1
            var dbkeyframe = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() };
            #endregion
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe);
            dbAniKeyFrame1.KeyFrames.Add(dbkeyframe1);
            #endregion

            #region DoubleAnimation2
#if WINRT
            var dbAniKeyFrame2 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTarget(dbAniKeyFrame2, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
#else
            var dbAniKeyFrame2 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTarget(dbAniKeyFrame2, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame2, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.ScaleY)"));
#endif

            #region Easing for dbAniKeyFrame2
            var dbkeyframe2 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe3 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 3.6, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe2);
            dbAniKeyFrame2.KeyFrames.Add(dbkeyframe3);
            #endregion

            #region DoubleAnimation3
#if WINRT
            var dbAniKeyFrame3 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTarget(dbAniKeyFrame3, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame3, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
#else
            var dbAniKeyFrame3 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTarget(dbAniKeyFrame3, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame3, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
#endif

            #region Easing for dbAniKeyFrame3
            var dbkeyframe4 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe5 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe4);
            dbAniKeyFrame3.KeyFrames.Add(dbkeyframe5);

            #endregion

            #region DoubleAnimation4
#if WINRT
            var dbAniKeyFrame4 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTarget(dbAniKeyFrame4, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame4, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
#else
            var dbAniKeyFrame4 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTarget(dbAniKeyFrame4, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame4, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#endif

            #region Easing for dbAniKeyFrame4
            var dbkeyframe6 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe7 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe6);
            dbAniKeyFrame4.KeyFrames.Add(dbkeyframe7);
            #endregion

            #region DoubleAnimation5
#if WINRT
            var dbAniKeyFrame5 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = new RepeatBehavior { Type = RepeatBehaviorType.Forever } };
            Storyboard.SetTarget(dbAniKeyFrame5, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame5, "(UIElement.Opacity)");
#else
            var dbAniKeyFrame5 = new DoubleAnimationUsingKeyFrames { RepeatBehavior = RepeatBehavior.Forever };
            Storyboard.SetTarget(dbAniKeyFrame5, ellipse);
            Storyboard.SetTargetProperty(dbAniKeyFrame5, new PropertyPath("(UIElement.Opacity)"));
#endif

            #region Easing for dbAniKeyFrame5
            var dbkeyframe8 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 1 };
            var dbkeyframe9 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600)), Value = 0, EasingFunction = new CircleEase() };
            #endregion

            dbAniKeyFrame5.KeyFrames.Add(dbkeyframe8);
            dbAniKeyFrame5.KeyFrames.Add(dbkeyframe9);
            #endregion

            EllipseAnimation2.Children.Add(dbAniKeyFrame1);
            EllipseAnimation2.Children.Add(dbAniKeyFrame2);
            EllipseAnimation2.Children.Add(dbAniKeyFrame3);
            EllipseAnimation2.Children.Add(dbAniKeyFrame4);
            EllipseAnimation2.Children.Add(dbAniKeyFrame5);
        }

        internal void Detach()
        {
            RemoveEllipse();
#if WINRT
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed -= AssociatedObject_PointerPressed;
                AssociatedObject.PointerReleased -= AssociatedObject_PointerReleased;
                AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
            }
#else
            if (AssociatedObject != null)
            {
                AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
                AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            }
#endif
            AssociatedObject = null;

        }

        #endregion

        #region resizing functions

        ControlParts getResizePart(Point mousePosition)
        {
            var size = new Size(
                AssociatedObject.ActualWidth,
                AssociatedObject.ActualHeight);

            var center = new Point(size.Width / 2, size.Height / 2);

            if (mousePosition.X > center.X && mousePosition.X > size.Width - DragSpace &&
                mousePosition.Y < center.Y && mousePosition.Y < DragSpace &&
                IsTopRightDraggable)
                return ControlParts.RightTop;

            if (mousePosition.X > center.X && mousePosition.X > size.Width - DragSpace &&
                mousePosition.Y > center.Y && mousePosition.Y > size.Height - DragSpace &&
                IsBottomRightDraggable)
                return ControlParts.RightBottom;

            if (mousePosition.X < center.X && mousePosition.X < DragSpace &&
                mousePosition.Y < center.Y && mousePosition.Y < DragSpace &&
                IsTopLeftDraggable)
                return ControlParts.LeftTop;

            if (mousePosition.X < center.X && mousePosition.X < DragSpace &&
                mousePosition.Y > center.Y && mousePosition.Y > size.Height - DragSpace &&
                IsBottomLeftDraggable)
                return ControlParts.LeftBottom;

            if (mousePosition.X > center.X && mousePosition.X > size.Width - DragSpace &&
                IsRightDraggable)
                return ControlParts.Right;

            if (mousePosition.X < center.X && mousePosition.X < DragSpace &&
                IsLeftDraggable)
                return ControlParts.Left;

            if (mousePosition.Y < center.Y && mousePosition.Y < DragSpace &&
                IsTopDraggable)
                return ControlParts.Top;

            if (mousePosition.Y > center.Y && mousePosition.Y > size.Height - DragSpace &&
                IsBottomDraggable)
                return ControlParts.Bottom;

            return ControlParts.None;
        }

        void RemoveEllipse()
        {
            var parent = AssociatedObject as Grid;

            if (parent == null) return;
            double totalcount = parent.Children.Count;
            var ResizeRect = new List<FrameworkElement>();
            for (int i = 0; i < totalcount; i++)
            {
                var e = parent.Children[i] as FrameworkElement;

                if (e == null) continue;

                if (e.Name.Contains("__RESIZE_AUTO_GEN_RECTANGLE"))
                {
                    e.Visibility = Visibility.Collapsed;
                    ResizeRect.Add(e);
                }
            }
            foreach (FrameworkElement Rectangle in ResizeRect)
            {
                parent.Children.Remove(Rectangle);
            }

            EllipseAnimation1 = null;
            EllipseAnimation2 = null;


        }

        void setEllipse(ControlParts part)
        {
            var parent = AssociatedObject as Grid;

            if (parent == null) return;
            var hiddenellipse = new Ellipse { Stroke = new SolidColorBrush(Colors.Transparent), StrokeThickness = 3d, Fill = new SolidColorBrush(Colors.Transparent), Opacity = 1 };
            var elips = new Ellipse { Stroke = new SolidColorBrush(Colors.White), StrokeThickness = 3d, Fill = new SolidColorBrush(Colors.Black), Opacity = 1 };
#if WINRT
            var animationellips = new Ellipse
            {
                RenderTransformOrigin = new Point(0.5, 0.5),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1d,
                RenderTransform = new CompositeTransform()
            };
#else
            var animationellips = new Ellipse
            {
                RenderTransformOrigin = new Point(0.5, 0.5),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1d
            };
#endif
            elips.SetValue(Canvas.ZIndexProperty, (int)AssociatedObject.GetValue(Canvas.ZIndexProperty) + 1);
            switch (part)
            {
                case ControlParts.None:
                    elips = null;
                    break;

                case ControlParts.Top:
                    elips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_TOP" + GetHashCode();
                    animationellips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_TOP_ani" + GetHashCode();
                    hiddenellipse.Name = "__RESIZE_AUTO_GEN_RECTANGLE_TOP_hid" + GetHashCode();
                    elips.Width = 20;
                    animationellips.Width = 10;
                    elips.Margin = new Thickness(0, -10, 0, 0);
                    animationellips.Margin = new Thickness(0, -5, 0, 0);
                    elips.Height = 20;
                    animationellips.Height = 10;
                    elips.VerticalAlignment = animationellips.VerticalAlignment = VerticalAlignment.Top;
                    elips.SetValue(Grid.RowProperty, 0);
                    animationellips.SetValue(Grid.RowProperty, 0);

                    Ellipse1IdealAnimation(animationellips);

                    hiddenellipse.Width = 30;
                    hiddenellipse.Margin = new Thickness(0, -15, 0, 0);
                    hiddenellipse.Height = 30;
                    hiddenellipse.VerticalAlignment = VerticalAlignment.Top;
                    hiddenellipse.SetValue(Grid.RowProperty, 0);
                    break;

                case ControlParts.Bottom:
                    elips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_BOTTOM" + GetHashCode();
                    animationellips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_BOTTOM_ani" + GetHashCode();
                    hiddenellipse.Name = "__RESIZE_AUTO_GEN_RECTANGLE_BOTTOM_hid" + GetHashCode();
                    elips.Width = 20;
                    animationellips.Width = 10;
                    elips.Margin = new Thickness(0, 0, 0, -10);
                    animationellips.Margin = new Thickness(0, 0, 0, -5);
                    elips.Height = 20;
                    animationellips.Height = 10;
                    elips.VerticalAlignment = animationellips.VerticalAlignment = VerticalAlignment.Bottom;
                    elips.SetValue(Grid.RowProperty, 1);
                    animationellips.SetValue(Grid.RowProperty, 1);

                    Ellipse2IdealAnimation(animationellips);

                    hiddenellipse.Width = 30;
                    hiddenellipse.Margin = new Thickness(0, 0, 0, -15);
                    hiddenellipse.Height = 30;
                    hiddenellipse.VerticalAlignment = VerticalAlignment.Bottom;
                    hiddenellipse.SetValue(Grid.RowProperty, 1);
                    break;

                case ControlParts.Right:
                    elips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_RIGHT" + GetHashCode();
                    animationellips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_RIGHT_ani" + GetHashCode();
                    hiddenellipse.Name = "__RESIZE_AUTO_GEN_RECTANGLE_RIGHT_hid" + GetHashCode();
                    elips.Width = 20;
                    animationellips.Width = 10;
                    elips.Margin = new Thickness(0, 0, -10, 0);
                    animationellips.Margin = new Thickness(0, 0, -5, 0);
                    elips.Height = 20;
                    animationellips.Height = 10;
                    elips.HorizontalAlignment = animationellips.HorizontalAlignment = HorizontalAlignment.Right;
                    Grid.SetColumn(elips, 1);
                    Grid.SetColumn(animationellips, 1);

                    Ellipse2IdealAnimation(animationellips);

                    hiddenellipse.Width = 30;
                    hiddenellipse.Margin = new Thickness(0, 0, -15, 0);
                    hiddenellipse.Height = 30;
                    hiddenellipse.HorizontalAlignment = HorizontalAlignment.Right;
                    Grid.SetColumn(hiddenellipse, 1);
                    break;

                case ControlParts.Left:
                    elips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_LEFT" + GetHashCode();
                    animationellips.Name = "__RESIZE_AUTO_GEN_RECTANGLE_LEFT_ani" + GetHashCode();
                    hiddenellipse.Name = "__RESIZE_AUTO_GEN_RECTANGLE_LEFT_hid" + GetHashCode();
                    elips.Width = 20;
                    animationellips.Width = 10;
                    elips.Margin = new Thickness(-10, 0, 0, 0);
                    animationellips.Margin = new Thickness(-5, 0, 0, 0);
                    elips.Height = 20;
                    animationellips.Height = 10;
                    elips.HorizontalAlignment = animationellips.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(elips, 0);
                    Grid.SetColumn(animationellips, 0);

                    Ellipse1IdealAnimation(animationellips);

                    hiddenellipse.Width = 30;
                    hiddenellipse.Margin = new Thickness(-15, 0, 0, 0);
                    hiddenellipse.Height = 30;
                    hiddenellipse.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(hiddenellipse, 0);
                    break;

                case ControlParts.RightTop:
                    elips.Width = DragSpace;
                    elips.Height = DragSpace;
                    break;

                case ControlParts.RightBottom:
                    elips.Width = DragSpace;
                    elips.Height = DragSpace;
                    break;

                case ControlParts.LeftBottom:
                    elips.Width = DragSpace;
                    elips.Height = DragSpace;
                    break;

                case ControlParts.LeftTop:
                    elips.Width = DragSpace;
                    elips.Height = DragSpace;

                    break;
            }

            if (elips == null) return;
            parent.Children.Add(elips);
            parent.Children.Add(hiddenellipse);
            parent.Children.Add(animationellips);
        }

        void resize(Point insideMousePosition, Point parentMousePosition, Point currentPosition)
        {
            var m_parent = VisualTreeHelper.GetParent(AssociatedObject) as ScheduleMonthAppointmentViewControl;
            var d_parent = VisualTreeHelper.GetParent(AssociatedObject) as ScheduleDaysAppointmentViewControl;
            var t_parent = VisualTreeHelper.GetParent(AssociatedObject) as ScheduleHorizontalAppointmentViewControl;
            double newHeight = AssociatedObject.ActualHeight;
            double newWidth = AssociatedObject.ActualWidth;
            var newPosition = new Point();
            if (m_parent != null)
            {
                newPosition = new Point(
                (double)m_parent.GetValue(Canvas.LeftProperty),
                (double)m_parent.GetValue(Canvas.TopProperty));
            }
            else if (d_parent != null)
            {

                newPosition = new Point(
                    (double)d_parent.GetValue(Canvas.LeftProperty),
                    (double)d_parent.GetValue(Canvas.TopProperty));
            }
            else if (t_parent != null)
            {
                newPosition = new Point(
                             (double)t_parent.GetValue(Canvas.LeftProperty),
                             (double)t_parent.GetValue(Canvas.TopProperty));
            }


            switch (resizePart)
            {
                case ControlParts.RightTop:
                    newWidth = insideMousePosition.X;
                    newHeight -= parentMousePosition.Y - newPosition.Y;
                    newPosition.Y = parentMousePosition.Y;
                    break;

                case ControlParts.RightBottom:
                    newWidth = insideMousePosition.X;
                    newHeight = insideMousePosition.Y;
                    break;

                case ControlParts.LeftTop:
                    newWidth -= parentMousePosition.X - newPosition.X;
                    newPosition.X = parentMousePosition.X;
                    newHeight -= parentMousePosition.Y - newPosition.Y;
                    newPosition.Y = parentMousePosition.Y;
                    break;

                case ControlParts.LeftBottom:
                    newWidth -= parentMousePosition.X - newPosition.X;
                    newPosition.X = parentMousePosition.X;
                    newHeight = insideMousePosition.Y;
                    break;

                case ControlParts.Right:
                    if (!(sfSchedule.timelineScrollViewer.HorizontalOffset.Equals(sfSchedule.timelineScrollViewer.ExtentWidth - sfSchedule.timelineScrollViewer.ViewportWidth) && currentPosition.X > sfSchedule.timelineScrollViewer.ViewportWidth))
                    {
                        if ((sfSchedule.timelineScrollViewer.ExtentWidth > sfSchedule.timelineScrollViewer.HorizontalOffset + sfSchedule.timelineScrollViewer.ViewportWidth) && currentPosition.X > sfSchedule.timelineScrollViewer.ViewportWidth - sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
                            sfSchedule.isscrollmoveondragging = true;
#if WINRT
#if SyncfusionFramework4_5_11
                            sfSchedule.timelineScrollViewer.ChangeView(sfSchedule.timelineScrollViewer.HorizontalOffset + 10, null, null);
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset + 10);
#endif
                            newWidth = newWidth + 10;
                            newPosition.X = newPosition.X - 10;
#elif WPF
                        sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset + 1);
                        newWidth = newWidth + 1;
                        newPosition.X = newPosition.X - 1;
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset + 10);
                            newWidth = newWidth + 10;
                            newPosition.X = parentMousePosition.X - newWidth;
#endif
                        }
                        else if (sfSchedule.timelineScrollViewer.HorizontalOffset > scrollBoundX1 && currentPosition.X < sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
                            sfSchedule.isscrollmoveondragging = true;
#if WINRT
#if SyncfusionFramework4_5_11
                            sfSchedule.timelineScrollViewer.ChangeView(sfSchedule.timelineScrollViewer.HorizontalOffset - 10, null, null);
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset - 10);
#endif
                            newWidth = newWidth - 10;
                            newPosition.X = newPosition.X + 10;
#elif WPF
                        sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset - 1);
                        newWidth = newWidth - 1;
                        newPosition.X = newPosition.X + 1;
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset + 10);
                            newWidth = newWidth - 10;
                            newPosition.X = parentMousePosition.X + newWidth + 20;
#endif
                        }
                        else
                        {
                            sfSchedule.isScrollResizeEnabled = false;
                            newWidth = insideMousePosition.X;
                        }
                    }
                    break;

                case ControlParts.Left:
                    if (!(sfSchedule.timelineScrollViewer.HorizontalOffset.Equals(0) && currentPosition.X < 0))
                    {
                        if (sfSchedule.timelineScrollViewer.HorizontalOffset > 0 && currentPosition.X < sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
                            sfSchedule.isscrollmoveondragging = true;
#if SyncfusionFramework4_5_11 && WINRT
                            sfSchedule.timelineScrollViewer.ChangeView(sfSchedule.timelineScrollViewer.HorizontalOffset - 10, null, null);
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset - 10);
#endif
                            newWidth = newWidth + 10;
#if SILVERLIGHT
                            X2 = newWidth + newPosition.X;
#endif
                        }
                        else if ((scrollBoundX2 > sfSchedule.timelineScrollViewer.HorizontalOffset + sfSchedule.timelineScrollViewer.ViewportWidth) && currentPosition.X > sfSchedule.timelineScrollViewer.ViewportWidth - sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
                            sfSchedule.isscrollmoveondragging = true;
#if SyncfusionFramework4_5_11 && WINRT
                            sfSchedule.timelineScrollViewer.ChangeView(sfSchedule.timelineScrollViewer.HorizontalOffset + 10, null, null);
#else
                            sfSchedule.timelineScrollViewer.ScrollToHorizontalOffset(sfSchedule.timelineScrollViewer.HorizontalOffset + 10);
#endif
                            newWidth = newWidth - 10;
#if SILVERLIGHT
                            X2 = newWidth + newPosition.X;
#endif
                        }
                        else
                        {
                            sfSchedule.isScrollResizeEnabled = false;
#if SILVERLIGHT
                            newWidth = X2 - newPosition.X;
#else
                            newWidth += newPosition.X - parentMousePosition.X;
#endif
                            newPosition.X = parentMousePosition.X;
                        }
                    }
                    break;

                case ControlParts.Top:
                    if (!(sfSchedule.dayScrollViewer.VerticalOffset.Equals(0) && currentPosition.Y < 0))
                    {
                        if (sfSchedule.dayScrollViewer.VerticalOffset > 0 && currentPosition.Y < sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
#if SyncfusionFramework4_5_11 && WINRT
                            sfSchedule.dayScrollViewer.ChangeView(null, sfSchedule.dayScrollViewer.VerticalOffset - 10, null);
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset - 10);
#endif
                            newHeight = newHeight + 10;
#if SILVERLIGHT
                            Y2 = newHeight + newPosition.Y;
#endif
                        }
                        else if ((scrollBoundY2 > sfSchedule.dayScrollViewer.VerticalOffset + sfSchedule.dayScrollViewer.ViewportHeight) && currentPosition.Y > sfSchedule.dayScrollViewer.ViewportHeight - sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
#if SyncfusionFramework4_5_11 && WINRT
                            sfSchedule.dayScrollViewer.ChangeView(null, sfSchedule.dayScrollViewer.VerticalOffset + 10, null);
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset + 10);
#endif
                            newHeight = newHeight - 10;
#if SILVERLIGHT
                            Y2 = newHeight + newPosition.Y;
#endif
                        }

                        else
                        {
                            sfSchedule.isScrollResizeEnabled = false;
#if SILVERLIGHT
                            newHeight = Y2 - (newPosition.Y);
#else
                            newHeight -= parentMousePosition.Y - newPosition.Y;
#endif
                            newPosition.Y = parentMousePosition.Y;
                        }
                    }
                    break;
                case ControlParts.Bottom:
                    if (!(sfSchedule.dayScrollViewer.VerticalOffset.Equals(sfSchedule.dayScrollViewer.ExtentHeight - sfSchedule.dayScrollViewer.ViewportHeight) && currentPosition.Y > sfSchedule.dayScrollViewer.ViewportHeight))
                    {
                        if ((sfSchedule.dayScrollViewer.ExtentHeight > sfSchedule.dayScrollViewer.VerticalOffset + sfSchedule.dayScrollViewer.ViewportHeight) && currentPosition.Y > sfSchedule.dayScrollViewer.ViewportHeight - sfSchedule.IntervalHeight)
                        {

                            sfSchedule.isScrollResizeEnabled = true;
#if WINRT
#if SyncfusionFramework4_5_11
                            sfSchedule.dayScrollViewer.ChangeView(null, sfSchedule.dayScrollViewer.VerticalOffset + 10, null);
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset + 10);
#endif
                            newHeight = newHeight + 10;
                            newPosition.Y = newPosition.Y - 10;
#elif WPF
                        sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset + 1);
                        newHeight = newHeight + 1;
                        newPosition.Y = newPosition.Y - 1;
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset + 10);
                            newHeight = newHeight + 10;
                            newPosition.Y = parentMousePosition.Y - newHeight - 20;
#endif

                        }
                        else if (sfSchedule.dayScrollViewer.VerticalOffset > scrollBoundY1 && currentPosition.Y < sfSchedule.IntervalHeight)
                        {
                            sfSchedule.isScrollResizeEnabled = true;
#if WINRT
#if SyncfusionFramework4_5_11
                            sfSchedule.dayScrollViewer.ChangeView(null, sfSchedule.dayScrollViewer.VerticalOffset - 10, null);
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset - 10);
#endif
                            newHeight = newHeight - 10;
                            newPosition.Y = newPosition.Y + 10;
#elif WPF
                        sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset - 1);
                        newHeight = newHeight - 1;
                        newPosition.Y = newPosition.Y + 1;
#else
                            sfSchedule.dayScrollViewer.ScrollToVerticalOffset(sfSchedule.dayScrollViewer.VerticalOffset - 10);
                            newHeight = newHeight - 10;
                            newPosition.Y = parentMousePosition.Y - newHeight - 20;
#endif
                        }

                        else
                        {
                            sfSchedule.isScrollResizeEnabled = false;
                            newHeight = insideMousePosition.Y;
                        }
                    }
                    break;
            }

            bool has_changes = false;

            if (newWidth >= MinWidth && m_parent != null)
            {
                m_parent.Width = newWidth;
                m_parent.SetValue(Canvas.LeftProperty, newPosition.X);
                has_changes = true;
            }
            if (newWidth >= MinWidth && t_parent != null)
            {
                t_parent.Width = newWidth;
                t_parent.AppWidth = newWidth;
                t_parent.SetValue(Canvas.LeftProperty, newPosition.X);
                has_changes = true;
            }

            if (newHeight >= MinHeight && d_parent != null)
            {
                d_parent.Height = newHeight;

                d_parent.SetValue(Canvas.TopProperty, newPosition.Y);
                has_changes = true;
            }

            // call this to notify size change
            if (has_changes)
                raiseEvent(SizeChanged);

            checkParentBounds();

        }

        #endregion

        #region check functions

        void checkParentBounds()
        {
            if (!StayInParent || AssociatedObject == null) return;

            var parent = VisualTreeHelper.GetParent(AssociatedObject) as ScheduleMonthAppointmentViewControl;

            // check if parent exist and is a canvas
            if (parent == null) return;

            bool has_changes = false;

            try
            {
                if (Canvas.GetLeft(AssociatedObject) < 0)
                {
                    double prv_left = Canvas.GetLeft(AssociatedObject);
                    Canvas.SetLeft(AssociatedObject, 0);
                    AssociatedObject.Width += prv_left;
                    has_changes = true;
                }

                if (Canvas.GetTop(AssociatedObject) < 0)
                {
                    double prv_top = Canvas.GetTop(AssociatedObject);
                    Canvas.SetTop(AssociatedObject, 0);
                    AssociatedObject.Height += prv_top;
                    has_changes = true;
                }

                if (Canvas.GetLeft(AssociatedObject) + AssociatedObject.Width > parent.ActualWidth)
                {
                    parent.Width = AssociatedObject.ActualWidth - Canvas.GetLeft(parent);
                    has_changes = true;
                }


                if (Canvas.GetTop(AssociatedObject) + AssociatedObject.Height > parent.ActualHeight)
                {
                    AssociatedObject.Height = parent.ActualHeight - Canvas.GetTop(AssociatedObject);
                    has_changes = true;
                }

            }
            catch (Exception)
            {
                has_changes = false;
            }

            // call this to notify size change
            if (has_changes)
                raiseEvent(SizeChanged);

        }

        void checkSizeValues()
        {
            FrameworkElement element = AssociatedObject;

            if (element == null) return;

            bool has_changes = false;

            if (element.Width > MaxWidth)
            {
                element.Width = MaxWidth;
                has_changes = true;
            }

            if (element.Width < MinWidth)
            {
                element.Width = MinWidth;
                has_changes = true;
            }

            if (element.Height > MaxHeight)
            {
                element.Height = MaxHeight;
                has_changes = true;
            }

            if (element.Height < MinHeight)
            {
                element.Height = MinHeight;
                has_changes = true;
            }

            // call this to notify size change
            if (has_changes)
                raiseEvent(SizeChanged);
        }

        #endregion

        #endregion
    }
}
