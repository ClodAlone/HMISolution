#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7

using Syncfusion.WP.Utils;
using System.Windows;
using System.Windows.Media;
using Syncfusion.WP.Primitives;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Syncfusion.WP.Controls.Input

#else
using Windows.Devices.Input;
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a control for displaying the dates in the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalendar"/> control.
    /// </summary>
    public sealed class CalendarView : Control
    {
        private const int CEndOfWeek = 6;

        internal const int CWeekDays = 7;

        private const int CTotalCells = 42;

        private const int CTotalRows = 6;

        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarView"/> class.
        /// </summary>
        public CalendarView()
        {
            DefaultStyleKey = typeof(CalendarView);
            this.Loaded += CalendarViewLoaded;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            timer=new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.8);
            timer.Tick += timer_Tick;

            Selection_Timer = new DispatcherTimer();
            Selection_Timer.Interval = TimeSpan.FromSeconds(0.3);
            Selection_Timer.Tick += Selection_Timer_Tick;

            popup=new Popup();
            popup.IsOpen = true;
            popup.HorizontalOffset = 0;
            popup.VerticalOffset = 0;
#endif
        }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        void Selection_Timer_Tick(object sender, object e)
        {
            Selection_Timer.Stop();
            parentCalendar.ReleasePointerCaptures();
            if (IsLongPress)
            {
                IsLongPress = false;
                timer.Stop();
                if (parentCalendar.SelectionStartDate != null && parentCalendar.SelectionEndDate != null)
                {
                    if (!parentCalendar.ContainsDateinSelection((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate))
                    {
                        parentCalendar.SelectedDates.Add(new DateRange((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    else
                    {
                        parentCalendar.SelectedDates.Remove(parentCalendar.ContainsDate((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    ValidateSelectionStates((DateTime)parentCalendar.SelectionStartDate);
                }
                popup.Child = null;
                parentCalendar.SelectionStartDate = null;
                parentCalendar.SelectionEndDate = null;
                
            }
        }


        void timer_Tick(object sender, object e)
        {
            timer.Stop();
            if (!(parentCalendar!=null && parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate((DateTime)currentButton.Content)) && parentCalendar.IsValidateMinMax(currentButton.Content.ToDateTime()) && !parentCalendar.IsManipulated)
            {  
                IsLongPress = true;
                if (parentCalendar.SelectionStartDate == null)
                    parentCalendar.SelectionStartDate = currentButton.Content;
                currentButton.Part_Circle.Height = currentButton.ActualWidth / 2;
                currentButton.Part_Circle.Width = currentButton.ActualWidth / 2;
                currentButton.UpdateLongPressState(true);
            }
        }
#endif
        internal void CalendarViewLoaded(object sender, RoutedEventArgs e)
        {
            if (parentCalendar != null)
            {
                OnRender(parentCalendar.displayDate, true);
            }
            this.Loaded -= CalendarViewLoaded;
            this.LayoutUpdated += CalendarView_LayoutUpdated;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))      
            if (parentCalendar.SelectionMode == SelectionMode.Multiple)
            {
                foreach (DateRange date in parentCalendar.SelectedDates)
                {
                    ValidateSelectionStates((DateTime)date.StartDate);
                }
            }
#endif
        }

        void CalendarView_LayoutUpdated(object sender, object e)
        {
            if (parentCalendar != null && parentCalendar.SelectedDate != null)
            {
                if (parentCalendar.SelectionMode == SelectionMode.Single)
                {
                    if (ValidateSelectionStates(DateTime.Parse(parentCalendar.SelectedDate.ToString())))
                    {
                        this.LayoutUpdated -= CalendarView_LayoutUpdated;
                    }
                }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                else if (parentCalendar.SelectionMode == SelectionMode.Multiple)
                {
                    foreach (DateRange dateRange in parentCalendar.SelectedDates)
                    {
                        if (ValidateSelectionStates(dateRange.StartDate))
                            this.LayoutUpdated -= CalendarView_LayoutUpdated;
                    }
                }
#endif
            }
            else
            {
                this.LayoutUpdated -= CalendarView_LayoutUpdated;
            }

            if (parentCalendar != null && ValidateActiveStates(parentCalendar.displayDate))
            {
                this.LayoutUpdated -= CalendarView_LayoutUpdated;
            }
            else
            {
                this.LayoutUpdated -= CalendarView_LayoutUpdated;
            }
        }

        internal Grid PART_LayoutRoot;

        internal SfCalendar parentCalendar;

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal DispatcherTimer timer;

        internal DispatcherTimer Selection_Timer;

        internal bool IsLongPress = false;

        internal CalendarDayButton currentButton = null;

        internal Windows.UI.Xaml.Controls.Primitives.Popup popup = null;
#endif
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.CalendarView"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_LayoutRoot = GetTemplateChild("PART_LayoutRoot") as Grid;
        }

        internal void OnRender(DateTime dateTime, bool redraw)
        {
            if (parentCalendar != null && PART_LayoutRoot != null)
            {
                if (redraw)
                    ClearCellData();
                DrawPreviousMonthCells(dateTime, redraw);
                DrawCurrentMonthCells(dateTime, redraw);
                DrawNextMonthCells(dateTime, redraw);
            }
        }

        internal void ClearCellData()
        {
            foreach (var element in PART_LayoutRoot.Children.ToList())
            {
                if (element.GetType() != typeof(Rectangle))
                {
                    PART_LayoutRoot.Children.Remove(element);
                }
            }
        }

        internal bool ValidateSelectionStates(ICollection<DateTime> dateTimes)
        {
            foreach (var element in PART_LayoutRoot.Children)
            {
                if (element.GetType() != typeof(Rectangle) && !(parentCalendar!=null && parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate((DateTime)parentCalendar.SelectedDate)))
                {
                    var button = element as CalendarDayButton;
                    if (button != null)
                    {
                        if (button.Content != null)
                        {
                            var _dateTime = (DateTime)button.Content;
                            if (dateTimes.Contains(_dateTime))
                            {
                                if ((DateTime)parentCalendar.SelectedDate == _dateTime)
                                {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                    button.Focus(Windows.UI.Xaml.FocusState.Programmatic);
#endif
                                }
                                if (!button.UpdateSelectionState(true))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (!button.UpdateSelectionState(false))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal bool IsDateInRange(DateTime datetime)
        {
            foreach (var dateRange in parentCalendar.SelectedDates)
            {
                if (dateRange.StartDate <=dateRange.EndDate)
                {
                    if (dateRange.StartDate <= datetime && dateRange.EndDate >= datetime)
                        return true;
                }
                else
                {
                    if (dateRange.StartDate >= datetime && dateRange.EndDate <= datetime)
                        return true;
                }
            }
            return false;
        }

        internal bool IsDateSelectedInRange(DateTime datetime)
        {
            if (parentCalendar.SelectionStartDate != null && parentCalendar.SelectionEndDate != null)
            {
                if ((DateTime)parentCalendar.SelectionStartDate <= (DateTime)parentCalendar.SelectionEndDate)
                {
                    if ((DateTime)parentCalendar.SelectionStartDate <= datetime && (DateTime)parentCalendar.SelectionEndDate >= datetime)
                        return true;
                }
                else
                {
                    if ((DateTime)parentCalendar.SelectionStartDate >= datetime && (DateTime)parentCalendar.SelectionEndDate <= datetime)
                        return true;
                }
            }
            return false;
        }
#endif
        internal bool ValidateSelectionStates(DateTime dateTime)
        {
            foreach (var element in PART_LayoutRoot.Children)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                 if (element.GetType() != typeof(Rectangle))
#else
                if (element.GetType() != typeof(Rectangle)&& !IsLongPress)
#endif
                {
                    var button = element as CalendarDayButton;
                    if (button != null)
                    {
                        var _dateTime = (DateTime)button.Content;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                        if ((_dateTime == dateTime && parentCalendar.SelectionMode != SelectionMode.None))
#else
                        if (parentCalendar.ValidateDate(dateTime) && (_dateTime == dateTime && parentCalendar.SelectionMode!=SelectionMode.None && parentCalendar.SelectedDates.Count==0) ||(parentCalendar.SelectionMode==SelectionMode.Multiple && parentCalendar.ContainsDateinStartSelection(_dateTime)||IsDateInRange(_dateTime)))
#endif
                        {
                            button.IsTabStop = true;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                            button.Focus();
#else
                            button.Focus(Windows.UI.Xaml.FocusState.Programmatic);
#endif
                                if (!button.UpdateSelectionState(true))
                                {
                                    return false;
                                }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))                          
                           if (parentCalendar.SelectionMode == SelectionMode.Multiple && parentCalendar.SelectedDates.Count>0)
                            {
                                if (!button.UpdateMultiSelectionState(true)) 
                                    {
                                        return false;
                                    }
                            }
#endif
                        }
                        else
                        {
                            button.IsTabStop = false;
                            if (parentCalendar.SelectionMode == SelectionMode.Single)
                            {
                                if (!button.UpdateSelectionState(false))
                                {
                                    return false;
                                }
                            }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                            else if (parentCalendar.SelectionMode == SelectionMode.Multiple)
                            {
                                if (!button.UpdateMultiSelectionState(false))
                                {
                                    return false;
                                }
                            }
#endif
                        }
                    }
                }
            }
            return true;
        }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        internal bool UpdateSelectionStates(bool isUnloading=false)
        {
            foreach (var element in PART_LayoutRoot.Children)
            {
                if (element.GetType() != typeof (Rectangle) && (IsLongPress || isUnloading))
                {
                    var button = element as CalendarDayButton;
                    if (button != null)
                    {
                        var _dateTime = (DateTime) button.Content;
                        if (IsDateSelectedInRange(_dateTime))
                            button.UpdateMultiSelectionState(true);
                        else if(!IsDateInRange(_dateTime))
                            button.UpdateMultiSelectionState(false);
                    }
                }
            }
            return true;
        }
#endif
        internal bool ValidateActiveStates(DateTime currentdate)
        {
            bool flag = true;

            foreach (var element in PART_LayoutRoot.Children.ToList())
            {
                if (element.GetType() != typeof(Rectangle))
                {
                    var button = element as CalendarDayButton;
                    if (button != null)
                    {
                        var datetime = (DateTime)button.Content;
                        if (datetime.Month == DateTime.Now.Month && datetime.Day == DateTime.Now.Day && datetime.Year == DateTime.Now.Year)
                        {
                            button.IsTabStop = true;
                            if (!VisualStateManager.GoToState(button, "Today", true))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            button.IsTabStop = false;
                        }
                        //if (datetime.Month == parentCalendar.displayDate.Month && datetime.Year == parentCalendar.displayDate.Year)
                        //{
                        //    if (datetime.Month != DateTime.Now.Month && datetime.Day != DateTime.Now.Day && datetime.Year != DateTime.Now.Year)
                        //    {
                        //        if (datetime.Day == 1 && parentCalendar.SelectedDate == null) 
                        //        {
                        //             button.IsTabStop = true;
                        //             VisualStateManager.GoToState(button, "Today", true);
                        //        }
                        //    }
                        //}
                        if (datetime.Month == currentdate.Month)
                        {
                            if (!button.UpdateActiveVisualState(true))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!button.UpdateActiveVisualState(false))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return flag;
        }

        internal void DrawNextMonthCells(DateTime dateTime, bool redraw)
        {
            var nextMonth = dateTime.AddMonths(1).FirstDay();
            int firstday;
             if(parentCalendar.FirstDayofWeek==DayOfWeek.Sunday)
                firstday= (int)dateTime.FirstDay().StartOfWeek(parentCalendar.Culture.DateTimeFormat.FirstDayOfWeek);
            else
                firstday = (int)dateTime.FirstDay().StartOfWeek(parentCalendar.FirstDayofWeek);
            int cureentmonthdays = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            int occupiedcells = cureentmonthdays + firstday;
            int remainingcells = CTotalCells - occupiedcells;
            DateTime nextmonthlastdate = nextMonth.AddDays(remainingcells);
            int row = CTotalRows - (int)Math.Ceiling(remainingcells / 7.0);

            foreach (var _dateTime in nextMonth.EachDayInMonth())
            {
                if (_dateTime < nextmonthlastdate)
                {
                    int day;
                    if(parentCalendar.FirstDayofWeek==DayOfWeek.Sunday)
                       day= (int)_dateTime.StartOfWeek(parentCalendar.Culture.DateTimeFormat.FirstDayOfWeek);
                    else
                        day = (int)_dateTime.StartOfWeek(parentCalendar.FirstDayofWeek);

                    if (redraw)
                    {
                        var dayButton = new CalendarDayButton {Content = _dateTime};
                        dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                        ? parentCalendar.CellTemplate
                                                        : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                             dayButton);

                        dayButton.Click += UpdateNextMonthSelection;

                        dayButton.UpdateActiveVisualState(false);

                        if (PART_LayoutRoot != null)
                        {
                            PART_LayoutRoot.Children.Add(dayButton);

                            Grid.SetColumn(dayButton, day * 2);
                            Grid.SetRow(dayButton, row * 2);
                        }

                        UpdateDatesView(_dateTime, dayButton);
                        dayButton.UpdateLayout();
                        ValidateActiveStates(parentCalendar.displayDate);
                    }
                    else
                    {
                        var dayButton = PART_LayoutRoot.ElementAt(row * 2, day * 2) as CalendarDayButton;
                        if (dayButton != null)
                        {
                            dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                      ? parentCalendar.CellTemplate
                                                      : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                           dayButton);
                            dayButton.Click -= UpdatePreviousMonthSelection;
                            dayButton.Click -= UpdateNextMonthSelection;
                            dayButton.Click -= UpdateCurrentMonthSelection;
                            dayButton.Click += UpdateNextMonthSelection;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                            if(parentCalendar.SelectionMode==SelectionMode.Multiple)
                            {
                                dayButton.PointerEntered -= dayButton_PointerEntered;
                                dayButton.PointerMoved -= dayButton_PointerMoved;
                                parentCalendar.PointerMoved -= parentCalendar_PointerMoved;
                                parentCalendar.PointerCaptureLost -= parentCalendar_PointerCaptureLost;
                                parentCalendar.PointerReleased -= parentCalendar_PointerReleased;
                                dayButton.PointerEntered += dayButton_PointerEntered;
                                dayButton.PointerMoved += dayButton_PointerMoved;
                                parentCalendar.PointerMoved += parentCalendar_PointerMoved;
                                parentCalendar.PointerCaptureLost += parentCalendar_PointerCaptureLost;
                                parentCalendar.PointerReleased += parentCalendar_PointerReleased;
                            }
#endif
                            dayButton.Content = _dateTime;
                            dayButton.UpdateActiveVisualState(false);

                            UpdateDatesView(_dateTime, dayButton);
                        }
                    }

                    if (day == CEndOfWeek)
                    {
                        row++;
                    }
                }
            }
        }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        void parentCalendar_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.OriginalSource is CalendarDayButton && IsLongPress && !parentCalendar.IsManipulated)
            {
                IsLongPress = false;
                timer.Stop();
                var button = e.OriginalSource as CalendarDayButton;
                if(parentCalendar.SelectionEndDate==null)
                    parentCalendar.SelectionEndDate = button.Content;
                    if (parentCalendar.SelectionStartDate != null && parentCalendar.SelectionEndDate!=null && !(parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content))&& parentCalendar.IsValidateMinMax(button.Content.ToDateTime()))
                    {
                        if (!parentCalendar.ContainsDateinSelection((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate))
                        {
                            parentCalendar.SelectedDates.Add(new DateRange((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                        }
                        else
                        {
                            parentCalendar.SelectedDates.Remove(parentCalendar.ContainsDate((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                        }
                        Selection_Timer.Stop();
						ValidateSelectionStates((DateTime)parentCalendar.SelectionStartDate);
                    }
                    popup.Child = null;
                    parentCalendar.SelectionStartDate = null;
                    parentCalendar.SelectionEndDate = null;
            }
        }

        void dayButton_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            timer.Stop();
            if(parentCalendar.SelectionStartDate!=null && parentCalendar.SelectionEndDate!=null && parentCalendar.SelectionStartDate==parentCalendar.SelectionEndDate)
            IsLongPress = false;
        }

        void dayButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(parentCalendar).Properties.IsLeftButtonPressed && !IsLongPress && !timer.IsEnabled && e.GetCurrentPoint(parentCalendar).PointerDevice.PointerDeviceType==PointerDeviceType.Touch)
            {
                timer.Start();
                currentButton = (sender as CalendarDayButton);
                parentCalendar.SelectionStartDate = currentButton.Content;
                mPreviousPoint = e.GetCurrentPoint(Window.Current.Content);
            }
            (sender as CalendarDayButton).IsPointerReleased = false;
        }
#endif
       
        internal void DrawPreviousMonthCells(DateTime dateTime, bool redraw)
        {
            int firstday;
              if(parentCalendar.FirstDayofWeek==DayOfWeek.Sunday)
                 firstday = (int)dateTime.FirstDay().StartOfWeek(parentCalendar.Culture.DateTimeFormat.FirstDayOfWeek);
             else
                firstday= (int)dateTime.FirstDay().StartOfWeek(parentCalendar.FirstDayofWeek);
            DateTime previousMonth = dateTime.AddMonths(-1).FirstDay();
            int days = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            int previousmonthstartdate = days - firstday;
            DateTime previousmonthstartdatetime = previousMonth.AddDays(previousmonthstartdate);

            var row = 0;

            foreach (var _dateTime in previousMonth.EachDayInMonth())
            {
                if (_dateTime >= previousmonthstartdatetime)
                {
                    int day;
                    if (parentCalendar.FirstDayofWeek == DayOfWeek.Sunday)
                        day = (int)_dateTime.StartOfWeek(parentCalendar.Culture.DateTimeFormat.FirstDayOfWeek);
                    else
                        day= (int)_dateTime.StartOfWeek(parentCalendar.FirstDayofWeek);

                    if (redraw)
                    {
                        var dayButton = new CalendarDayButton { Content = _dateTime };
                        dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                        ? parentCalendar.CellTemplate
                                                        : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                             dayButton);

                        dayButton.Click += UpdatePreviousMonthSelection;

                        dayButton.UpdateActiveVisualState(false);

                        if (PART_LayoutRoot != null)
                        {
                            PART_LayoutRoot.Children.Add(dayButton);

                            Grid.SetColumn(dayButton, day * 2);
                            Grid.SetRow(dayButton, row * 2);
                        }

                        UpdateDatesView(_dateTime, dayButton);
                        dayButton.UpdateLayout();
                        ValidateActiveStates(parentCalendar.displayDate);
                    }
                    else
                    {
                        var dayButton = PART_LayoutRoot.ElementAt(row * 2, day * 2) as CalendarDayButton;

                        if (dayButton != null)
                        {
                            dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                      ? parentCalendar.CellTemplate
                                                      : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                           dayButton);
                            dayButton.Click -= UpdatePreviousMonthSelection;
                            dayButton.Click -= UpdateNextMonthSelection;
                            dayButton.Click -= UpdateCurrentMonthSelection;
                            dayButton.Click += UpdatePreviousMonthSelection;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))                            
                            if(parentCalendar.SelectionMode==SelectionMode.Multiple)
                            {
                                dayButton.PointerEntered -= dayButton_PointerEntered;
                                dayButton.PointerMoved -= dayButton_PointerMoved;
                                parentCalendar.PointerMoved -= parentCalendar_PointerMoved;
                                parentCalendar.PointerCaptureLost -= parentCalendar_PointerCaptureLost;
                                parentCalendar.PointerReleased -= parentCalendar_PointerReleased;
                                parentCalendar.PointerExited -= parentCalendar_PointerExited;
                                dayButton.PointerEntered += dayButton_PointerEntered;
                                dayButton.PointerMoved += dayButton_PointerMoved;
                                parentCalendar.PointerMoved += parentCalendar_PointerMoved;
                                parentCalendar.PointerCaptureLost += parentCalendar_PointerCaptureLost;
                                parentCalendar.PointerReleased +=parentCalendar_PointerReleased;
                                parentCalendar.PointerExited += parentCalendar_PointerExited;
                            }
#endif
                            dayButton.Content = _dateTime;
                            dayButton.UpdateActiveVisualState(false);

                            UpdateDatesView(_dateTime, dayButton);
                        }
                    }

                    if (day == CEndOfWeek)
                    {
                        row++;
                    }
                }
            }
        }

        internal void DrawCurrentMonthCells(DateTime dateTime, bool redraw)
        {
            int row = 0;

            foreach (var _dateTime in dateTime.EachDayInMonth())
            {
                int day;
                if(parentCalendar.FirstDayofWeek==DayOfWeek.Sunday)
                    day = (int)_dateTime.StartOfWeek(parentCalendar.Culture.DateTimeFormat.FirstDayOfWeek);
                else
                    day = (int)_dateTime.StartOfWeek(parentCalendar.FirstDayofWeek);

                if (redraw)
                {
                    var dayButton = new CalendarDayButton { Content = _dateTime };
                    dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                    ? parentCalendar.CellTemplate
                                                    : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                         dayButton);

                    dayButton.Click += UpdateCurrentMonthSelection;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                    if (parentCalendar.SelectionMode == SelectionMode.Multiple)
                    {
                        dayButton.PointerEntered +=dayButton_PointerEntered;
                        dayButton.PointerMoved += dayButton_PointerMoved;
                        parentCalendar.PointerMoved += parentCalendar_PointerMoved;
                        parentCalendar.PointerCaptureLost +=parentCalendar_PointerCaptureLost;
                        parentCalendar.PointerReleased += parentCalendar_PointerReleased;
						 parentCalendar.PointerExited -= parentCalendar_PointerExited;
                        dayButton.PointerCaptureLost+=dayButton_PointerCaptureLost;
                        parentCalendar.PointerExited += parentCalendar_PointerExited;
                    }
#endif
                    dayButton.UpdateActiveVisualState(true);

                    if (PART_LayoutRoot != null)
                    {
                        PART_LayoutRoot.Children.Add(dayButton);

                        Grid.SetColumn(dayButton, day * 2);
                        Grid.SetRow(dayButton, row * 2);
                    }

                    UpdateDatesView(_dateTime, dayButton);
                }
                else
                {
                    var dayButton = PART_LayoutRoot.ElementAt(row * 2, day * 2) as CalendarDayButton;

                    if (dayButton != null)
                    {
                        dayButton.ContentTemplate = parentCalendar.CellTemplateSelector == null
                                                   ? parentCalendar.CellTemplate
                                                   : parentCalendar.CellTemplateSelector.SelectTemplate(_dateTime,
                                                                                                        dayButton);
                        dayButton.Click -= UpdatePreviousMonthSelection;
                        dayButton.Click -= UpdateNextMonthSelection;
                        dayButton.Click -= UpdateCurrentMonthSelection;
                        dayButton.Click += UpdateCurrentMonthSelection;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                        if(parentCalendar.SelectionMode==SelectionMode.Multiple)
                        {
                            dayButton.PointerMoved -= dayButton_PointerMoved;
                            dayButton.PointerCaptureLost -= dayButton_PointerCaptureLost;
                            dayButton.PointerEntered -= dayButton_PointerEntered;
                            parentCalendar.PointerMoved -= parentCalendar_PointerMoved;
                            parentCalendar.PointerReleased -= parentCalendar_PointerReleased;
                            parentCalendar.PointerCaptureLost -= parentCalendar_PointerCaptureLost;
                            parentCalendar.PointerExited+=parentCalendar_PointerExited;
                            dayButton.PointerMoved += dayButton_PointerMoved;
                            dayButton.PointerEntered += dayButton_PointerEntered;
                            dayButton.PointerCaptureLost += dayButton_PointerCaptureLost;
                            parentCalendar.PointerMoved +=parentCalendar_PointerMoved;
                            parentCalendar.PointerReleased+=parentCalendar_PointerReleased;
                            parentCalendar.PointerCaptureLost +=parentCalendar_PointerCaptureLost;
                            parentCalendar.PointerExited+=parentCalendar_PointerExited;
                        }
#endif
                        dayButton.Content = _dateTime;
                        dayButton.UpdateActiveVisualState(true);
                        dayButton.IsDateBlocked = false;

                        UpdateDatesView(_dateTime, dayButton);
                    }
                }

                if (day == CEndOfWeek)
                {
                    row++;
                }
            }
        }

#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
        void parentCalendar_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var control = GetParentItem(e.OriginalSource as DependencyObject);
            if (control is CalendarDayButton && IsLongPress && !parentCalendar.IsManipulated)
            {
                var button = control as CalendarDayButton;
                if (parentCalendar.SelectionEndDate == null)
                    parentCalendar.SelectionEndDate = button.Content;
                if (parentCalendar.SelectionStartDate != null && parentCalendar.SelectionEndDate != null && !(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)) && parentCalendar.IsValidateMinMax(button.Content.ToDateTime()))
                {
                    if (!parentCalendar.ContainsDateinSelection((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate))
                    {
                        parentCalendar.SelectedDates.Add(new DateRange((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    else
                    {
                        parentCalendar.SelectedDates.Remove(parentCalendar.ContainsDate((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    Selection_Timer.Stop();
                    ValidateSelectionStates((DateTime)parentCalendar.SelectionStartDate);
                }
                popup.Child = null;
                parentCalendar.SelectionStartDate = null;
                parentCalendar.SelectionEndDate = null;
                IsLongPress = false;
            }
            
            popup.Child = null;

            timer.Stop();
        }


        void parentCalendar_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var control = GetParentItem(e.OriginalSource as DependencyObject);
            if(control is CalendarDayButton && IsLongPress && !parentCalendar.IsManipulated)
            {
                var button = control as CalendarDayButton;
                if (parentCalendar.SelectionEndDate == null)
                    parentCalendar.SelectionEndDate = button.Content;
                if (parentCalendar.SelectionStartDate != null && parentCalendar.SelectionEndDate != null && !(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)) && parentCalendar.IsValidateMinMax(button.Content.ToDateTime()))
                {
                    if (!parentCalendar.ContainsDateinSelection((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate))
                    {
                        parentCalendar.SelectedDates.Add(new DateRange((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    else
                    {
                        parentCalendar.SelectedDates.Remove(parentCalendar.ContainsDate((DateTime)parentCalendar.SelectionStartDate, (DateTime)parentCalendar.SelectionEndDate));
                    }
                    Selection_Timer.Stop();
                    ValidateSelectionStates((DateTime)parentCalendar.SelectionStartDate);
                }
                popup.Child = null;
                parentCalendar.SelectionStartDate = null;
                parentCalendar.SelectionEndDate = null;
            }
            IsLongPress = false;
            popup.Child = null;
            
            timer.Stop();
        }

        private Windows.UI.Input.PointerPoint mPreviousPoint = null;
        void parentCalendar_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (IsLongPress && e.GetCurrentPoint(parentCalendar).Properties.IsLeftButtonPressed)
            {
                var parent = GetParentItem(e.OriginalSource as DependencyObject);
                if (parent is CalendarDayButton && !parentCalendar.IsManipulated)
                {
                    var button = parent as CalendarDayButton;
                    if (parentCalendar.SelectionStartDate == null)
                        parentCalendar.SelectionStartDate = currentButton.Content;
				    if(e.GetCurrentPoint(parentCalendar).PointerDevice.PointerDeviceType==PointerDeviceType.Touch)
                         Selection_Timer.Start();					
					
					parentCalendar.SelectionEndDate = button.Content;
                    if (currentButton != button && !(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)) && parentCalendar.IsValidateMinMax(button.Content.ToDateTime()))
                    {
                        
                        UpdateSelectionStates();
                        Windows.UI.Input.PointerPoint currentpoint = e.GetCurrentPoint(Window.Current.Content);

                        if (mPreviousPoint != null)
                        {
                            Line line = new Line()
                            {
                                X1 = mPreviousPoint.Position.X,
                                Y1 = mPreviousPoint.Position.Y,
                                X2 = currentpoint.Position.X,
                                Y2 = currentpoint.Position.Y,
                                StrokeThickness = 2,
                                Stroke = button.Foreground
                            };
                            popup.IsOpen = true;
                            popup.Child = line;
                        }
                    }             
                  }
            }
            else
            {
                popup.IsOpen = false;
                IsLongPress = false;
            }
            
        }

      private object GetParentItem(DependencyObject obj)
        {
            var item = obj;
            while (Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(item) != null && !(Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(item) is CalendarDayButton))
            {
               item = Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(item);
            }
            if (Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(item) is CalendarDayButton)
                return Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(item);
            return item;
        }

        void dayButton_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (parentCalendar != null)
            {
                var button = sender as CalendarDayButton;
                if (e.GetCurrentPoint(parentCalendar).Properties.IsLeftButtonPressed && button != null &&
                    !(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)) && parentCalendar.IsValidateMinMax(button.Content.ToDateTime()))
                {
                    if (!timer.IsEnabled && !IsLongPress)
                    {
						currentButton = button;
                        parentCalendar.SelectionStartDate = currentButton.Content;
                        button.Part_Circle.Height = button.ActualWidth / 5;
                        button.Part_Circle.Width = button.ActualWidth / 5;
                        mPreviousPoint = e.GetCurrentPoint(Window.Current.Content);
						if(e.GetCurrentPoint(parentCalendar).PointerDevice.PointerDeviceType==PointerDeviceType.Mouse)
						{
                            if (!(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)currentButton.Content)) && parentCalendar.IsValidateMinMax(currentButton.Content.ToDateTime()) && !parentCalendar.IsManipulated)
							{  
								IsLongPress = true;
								if (parentCalendar.SelectionStartDate == null)
									parentCalendar.SelectionStartDate = currentButton.Content;
								currentButton.Part_Circle.Height = currentButton.ActualWidth / 2;
								currentButton.Part_Circle.Width = currentButton.ActualWidth / 2;
							}
						}
						else
							timer.Start();
                    }
                    
                    if(timer.IsEnabled && IsLongPress)timer.Stop();
                }
            }
            if (!e.GetCurrentPoint(parentCalendar).Properties.IsLeftButtonPressed)
            {
                popup.Child = null;
                popup.IsOpen = false;
            }
        }

#endif
        private void UpdateCurrentMonthSelection(object sender, RoutedEventArgs e)
        {
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            if (parentCalendar != null && !parentCalendar.IsManipulated)
#else
            if (parentCalendar != null)
#endif
            {
                dynamic prevdateTime;
                var button = sender as CalendarDayButton;
                if (button != null && parentCalendar.SelectionMode != SelectionMode.None && !(parentCalendar.BlackOutDates!=null &&parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)))
                {
                    prevdateTime = parentCalendar.SelectedDate;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
				    if(parentCalendar.SelectionMode==SelectionMode.Multiple)	
                    UpdateMonthSelection(button);
#endif
                    parentCalendar.SelectedDate = button.Content;
                }
            }
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
            else if (parentCalendar.IsManipulated)
                parentCalendar.IsManipulated = false;
#endif
        }
        
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))

        internal void SetSelection()
        {
            if (parentCalendar.SelectionStartDate != null)
            {
                if (!parentCalendar.ContainsDateinStartSelection((DateTime)parentCalendar.SelectionStartDate))
                {
                    parentCalendar.SelectedDates.Add(new DateRange((DateTime)parentCalendar.SelectionStartDate));
                }
                else
                {
                    parentCalendar.SelectedDates.Remove(parentCalendar.ContainsDate((DateTime)parentCalendar.SelectionStartDate));
                }
                ValidateSelectionStates((DateTime)parentCalendar.SelectionStartDate);
            }
        }
        
        void UpdateMonthSelection(CalendarDayButton button)
        {
            if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down)
            {
                parentCalendar.SelectionStartDate = button.Content;
                if (parentCalendar.SelectionStartDate != null)
                {
                    SetSelection();
                }
            }
            else
            {
                if (parentCalendar.SelectionMode == SelectionMode.Multiple &&(!IsLongPress && !button.IsPointerReleased))
                {
                    parentCalendar.SelectedDates.Clear();
                    parentCalendar.SelectionStartDate = null;
                    parentCalendar.SelectionEndDate = null;
                    popup.Child = null;
                }
            }
        }
#endif

        private void UpdateNextMonthSelection(object sender, RoutedEventArgs e)
        {
            if (parentCalendar != null)
            {
                dynamic prevdateTime;
                var button = sender as CalendarDayButton;
                if (button != null && parentCalendar.SelectionMode != SelectionMode.None && !(parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)))
                {
                    prevdateTime = parentCalendar.SelectedDate;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                    if(parentCalendar.SelectionMode==SelectionMode.Multiple)
                    UpdateMonthSelection(button);
#endif
                    parentCalendar.SelectedDate = button.Content;
                  //  parentCalendar.DisplayDate = parentCalendar.displayDate.AddMonths(1);
                }
            }
        }

        private void UpdatePreviousMonthSelection(object sender, RoutedEventArgs e)
        {
            if (parentCalendar != null)
            {
                dynamic prevdateTime;
                var button = sender as CalendarDayButton;
                if (button != null && parentCalendar.SelectionMode != SelectionMode.None && !(parentCalendar.BlackOutDates != null && parentCalendar.BlackOutDates.ContainsDate((DateTime)button.Content)))
                {
                    prevdateTime = parentCalendar.SelectedDate;
#if (!(WINDOWS_PHONE||WINDOWS_PHONE_7))
                    if(parentCalendar.SelectionMode==SelectionMode.Multiple)
                    UpdateMonthSelection(button);
#endif
                    parentCalendar.SelectedDate = button.Content;
               //     parentCalendar.DisplayDate = parentCalendar.displayDate.AddMonths(-1);
                }
            }
        }

        private void UpdateMinMaxDates(DateTime _dateTime,CalendarDayButton dayButton)
        {
            Object VisibleMinDate,VisibleMaxDate;

            VisibleMinDate=parentCalendar.VisibleMinDate;
            VisibleMaxDate= parentCalendar.VisibleMaxDate;

            if (VisibleMinDate == null && VisibleMaxDate != null || VisibleMaxDate == null && VisibleMinDate != null || VisibleMinDate != null && VisibleMaxDate != null)
            {
                if (!parentCalendar.ValidateDate(_dateTime))
                {
                    dayButton.IsDateBlocked = true;
                    dayButton.ContentTemplate = parentCalendar.DisabledCellTemplate;
                }
                else if (parentCalendar.BlackOutDates != null && !parentCalendar.BlackOutDates.ContainsDate(_dateTime) && (parentCalendar.VisibleDates.Count == 0 || parentCalendar.VisibleDates.ContainsDate(_dateTime)))
                {
                    dayButton.IsDateBlocked = false;
                }
            }
        }
    
        private void UpdateVisibleDates(DateTime _dateTime,CalendarDayButton dayButton)
        {
            if (((parentCalendar.VisibleMinDate == null && parentCalendar.VisibleMaxDate == null)|| parentCalendar.ValidateDate(_dateTime))
                && (parentCalendar.BlackOutDates!=null && (parentCalendar.BlackOutDates.Count == 0 || !parentCalendar.BlackOutDates.ContainsDate(_dateTime))) &&(parentCalendar.VisibleDates!=null && parentCalendar.VisibleDates.Count>0))
            {
                if (parentCalendar.VisibleDates.ContainsDate(_dateTime))
                {
                    dayButton.IsDateBlocked = false;                    
                }
                else
                {
                    dayButton.IsDateBlocked = true;
                    dayButton.ContentTemplate = parentCalendar.DisabledCellTemplate;
                }
            }
        }
   
        private void UpdateBlockedDates(DateTime _dateTime,CalendarDayButton dayButton)
        {
            if (parentCalendar!=null && parentCalendar.BlackOutDates!=null && parentCalendar.BlackOutDates.ContainsDate(_dateTime))
            {
                dayButton.IsDateBlocked = true;
                dayButton.ContentTemplate = parentCalendar.BlackOutCellTemplate;
            }
        }

        private void UpdateDatesView(DateTime _dateTime, CalendarDayButton dayButton)
        {
            UpdateBlockedDates(_dateTime, dayButton);
            UpdateVisibleDates(_dateTime, dayButton);
            UpdateMinMaxDates(_dateTime, dayButton);
        }
    }
}
