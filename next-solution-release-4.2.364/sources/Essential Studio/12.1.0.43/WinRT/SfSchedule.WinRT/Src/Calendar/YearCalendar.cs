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
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Syncfusion.UI.Xaml.Primitives;
using Syncfusion.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Schedule
{
    public sealed class YearCalendar : Control
    {
        #region Constructor

        public YearCalendar()
        {
            this.DefaultStyleKey = typeof(YearCalendar);
            Loaded += YearCalendar_Loaded;
        }

        #endregion

        #region Private Fields

        Grid monthsGrid, yearsGrid;
        ContentPresenter headerPresenter;
        Button previousNavigationButton;
        Button nextNavigationButton;
        CalendarDayButton selectedMonthButton;
        CalendarDayButton selectedYearButton;
        CalendarDayButton todayButton;

        #endregion

        #region Dependency Properties

        #region DisplayYear
        public int DisplayYear
        {
            get { return (int)GetValue(DisplayYearProperty); }
            set { SetValue(DisplayYearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayYearProperty =
            DependencyProperty.Register("DisplayYear", typeof(int), typeof(YearCalendar), new PropertyMetadata(DateTime.Now.Date.Year, OnDisplayYearChanged));

        private static void OnDisplayYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is YearCalendar)
            {
                (d as YearCalendar).UpdateHeader();
            }
        }
        #endregion

        #region SelectedYear
        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof(int), typeof(YearCalendar), new PropertyMetadata(DateTime.Now.Date.Year, OnSelectedYearChanged));

        private static void OnSelectedYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is YearCalendar)
            {
                (d as YearCalendar).DisplayYear = (int)e.NewValue;
                (d as YearCalendar).UpdateNavigationButtonsMode();
            }
        }
        #endregion

        #region DisplayMonth
        public int DisplayMonth
        {
            get { return (int)GetValue(DisplayMonthProperty); }
            set { SetValue(DisplayMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMonth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMonthProperty =
            DependencyProperty.Register("DisplayMonth", typeof(int), typeof(YearCalendar), new PropertyMetadata(DateTime.Now.Date.Month));
        #endregion

        #region SelectedMonth
        public int SelectedMonth
        {
            get { return (int)GetValue(SelectedMonthProperty); }
            set { SetValue(SelectedMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMonth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedMonthProperty =
            DependencyProperty.Register("SelectedMonth", typeof(int), typeof(YearCalendar), new PropertyMetadata(DateTime.Now.Date.Month, OnSelectedMonthChanged));

        private static void OnSelectedMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is YearCalendar)
            {
                (d as YearCalendar).DisplayMonth = (int)e.NewValue;
            }
        }
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(YearCalendar), new PropertyMetadata(null));
        #endregion

        #region CellTemplate
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(YearCalendar), new PropertyMetadata(null));
        #endregion

        #region ShowTodayCell
        public bool ShowTodayCell
        {
            get { return (bool)GetValue(ShowTodayCellProperty); }
            set { SetValue(ShowTodayCellProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTodayCell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTodayCellProperty =
            DependencyProperty.Register("ShowTodayCell", typeof(bool), typeof(YearCalendar), new PropertyMetadata(true));
        #endregion

        #region TodayCellTemplate
        public DataTemplate TodayCellTemplate
        {
            get { return (DataTemplate)GetValue(TodayCellTemplateProperty); }
            set { SetValue(TodayCellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TodayCellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TodayCellTemplateProperty =
            DependencyProperty.Register("TodayCellTemplate", typeof(DataTemplate), typeof(YearCalendar), new PropertyMetadata(null));
        #endregion

        #region ShowGridLines
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(YearCalendar), new PropertyMetadata(true));
        #endregion

        #region GridLineStroke
        public Brush GridLineStroke
        {
            get { return (Brush)GetValue(GridLineStrokeProperty); }
            set { SetValue(GridLineStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLineStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridLineStrokeProperty =
            DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(YearCalendar), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region GridLineStrokeThickness
        public double GridLineStrokeThickness
        {
            get { return (double)GetValue(GridLineStrokeThicknessProperty); }
            set { SetValue(GridLineStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridLineStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridLineStrokeThicknessProperty =
            DependencyProperty.Register("GridLineStrokeThickness", typeof(double), typeof(YearCalendar), new PropertyMetadata(1d));
        #endregion

        #region ShowNavigationButtons
        public bool ShowNavigationButtons
        {
            get { return (bool)GetValue(ShowNavigationButtonsProperty); }
            set { SetValue(ShowNavigationButtonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNavigationButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNavigationButtonsProperty =
            DependencyProperty.Register("ShowNavigationButtons", typeof(bool), typeof(YearCalendar), new PropertyMetadata(true));
        #endregion

        #region PreviousNavigationButtonTemplate
        public ControlTemplate PreviousNavigationButtonTemplate
        {
            get { return (ControlTemplate)GetValue(PreviousNavigationButtonTemplateProperty); }
            set { SetValue(PreviousNavigationButtonTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousNavigationButtonTemplateProperty =
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(ControlTemplate), typeof(YearCalendar), new PropertyMetadata(null));
        #endregion

        #region NextNavigationButtonTemplate
        public ControlTemplate NextNavigationButtonTemplate
        {
            get { return (ControlTemplate)GetValue(NextNavigationButtonTemplateProperty); }
            set { SetValue(NextNavigationButtonTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(ControlTemplate), typeof(YearCalendar), new PropertyMetadata(null));
        #endregion

        #region YearHeader
        public object YearHeader
        {
            get { return (object)GetValue(YearHeaderProperty); }
            set { SetValue(YearHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YearHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YearHeaderProperty =
            DependencyProperty.Register("YearHeader", typeof(object), typeof(YearCalendar), new PropertyMetadata(DateTime.Now.Year));
        #endregion

        #region EnableMonthView
        internal bool EnableMonthView
        {
            get { return (bool)GetValue(EnableMonthViewProperty); }
            set { SetValue(EnableMonthViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableMonthView.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EnableMonthViewProperty =
            DependencyProperty.Register("EnableMonthView", typeof(bool), typeof(YearCalendar), new PropertyMetadata(true));
        #endregion

        #endregion

        #region Override Methods

        protected override void OnApplyTemplate()
        {
            headerPresenter = GetTemplateChild("PART_HeaderPresenter") as ContentPresenter;
            if (headerPresenter != null)
            {
                headerPresenter.PointerPressed += headerPresenter_PointerPressed;
            }
            previousNavigationButton = GetTemplateChild("PART_PreviousYearButton") as Button;
            if (previousNavigationButton != null)
                previousNavigationButton.Click += previousNavigationButton_Click;
            nextNavigationButton = GetTemplateChild("PART_NextYearButton") as Button;
            if (nextNavigationButton != null)
                nextNavigationButton.Click += nextNavigationButton_Click;
            todayButton = GetTemplateChild("PART_TodayButton") as CalendarDayButton;
            if (todayButton != null)
                todayButton.Click += todayButton_Click;
            monthsGrid = GetTemplateChild("PART_MonthsGrid") as Grid;
            if (monthsGrid != null)
            {
                foreach (CalendarDayButton calendarMonthButton in monthsGrid.Children)
                {
                    if (calendarMonthButton != null)
                    {
                        calendarMonthButton.Click += calendarMonthButton_Click;
                    }
                }
            }
            yearsGrid = GetTemplateChild("PART_YearsGrid") as Grid;
            if (yearsGrid != null)
            {
                foreach (CalendarDayButton calendarYearButton in yearsGrid.Children)
                {
                    if (calendarYearButton != null)
                    {
                        calendarYearButton.Click += calendarYearButton_Click;
                    }
                }
            }
            base.OnApplyTemplate();
        }

        #endregion

        #region Events

        void YearCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        void headerPresenter_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (headerPresenter != null)
            {
                EnableMonthView = false;
                headerPresenter.IsHitTestVisible = false;
            }
            UpdateHeader();
            UpdateYearView(SlideDirection.Down);
        }

        void calendarMonthButton_Click(object sender, RoutedEventArgs e)
        {
            var calendarMonthButton = (sender as CalendarDayButton);
            var month = calendarMonthButton.Content.ToString();
            SelectedMonth = GetMonth(month);
            SelectedMonthChangedEventArgs args = new SelectedMonthChangedEventArgs
            {
                Month = SelectedMonth
            };
            GetSelectedMonthChangedEvent(args);
            UpdateMonthButtonSelection(calendarMonthButton);
        }

        void calendarYearButton_Click(object sender, RoutedEventArgs e)
        {
            var calendarYearButton = (sender as CalendarDayButton);
            var year = (int)calendarYearButton.Content;
            SelectedYear = year;
            SelectedYearChangedEventArgs args = new SelectedYearChangedEventArgs
            {
                Year = SelectedYear
            };
            GetSelectedYearChangedEvent(args);
            UpdateYearButtonSelection(calendarYearButton);
            Refresh();
        }

        void previousNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnableMonthView)
            {
                if (SelectedYear > DateTime.MinValue.Year)
                {
                    SelectedYear--;
                }
                SelectedYearChangedEventArgs args = new SelectedYearChangedEventArgs
                {
                    Year = SelectedYear
                };
                GetSelectedYearChangedEvent(args);
                UpdateNavigationButtonsMode();
                if (monthsGrid != null)
                    UpdateMonthButtonSelection(null);
            }
            else
            {
                DisplayYear = GetStartYear(DisplayYear - 9);
                UpdateYearView(SlideDirection.Right);
                if (yearsGrid != null)
                    UpdateYearButtonSelection(null);
            }
        }

        void nextNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnableMonthView)
            {
                SelectedYear++;
                if (SelectedYear > DateTime.MaxValue.Year)
                    SelectedYear = DateTime.MaxValue.Year;
                SelectedYearChangedEventArgs args = new SelectedYearChangedEventArgs
                {
                    Year = SelectedYear
                };
                GetSelectedYearChangedEvent(args);
                UpdateNavigationButtonsMode();
                if (monthsGrid != null)
                    UpdateMonthButtonSelection(null);
            }
            else
            {
                DisplayYear = GetStartYear(DisplayYear + 10);
                UpdateYearView(SlideDirection.Left);
                if (yearsGrid != null)
                    UpdateYearButtonSelection(null);
            }
        }

        void todayButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedYear != DateTime.Now.Date.Year)
            {
                SelectedYear = DateTime.Now.Date.Year;
                SelectedYearChangedEventArgs yearArgs = new SelectedYearChangedEventArgs
                {
                    Year = SelectedYear
                };
                GetSelectedYearChangedEvent(yearArgs);
            }

            if (SelectedMonth != DateTime.Now.Date.Month)
            {
                SelectedMonth = DateTime.Now.Date.Month;
                SelectedMonthChangedEventArgs monthArgs = new SelectedMonthChangedEventArgs
                {
                    Month = SelectedMonth
                };
                GetSelectedMonthChangedEvent(monthArgs);
            }
            Refresh();
        }

        #endregion

        #region Implementation

        public void Refresh()
        {
            EnableMonthView = true;
            if (headerPresenter != null)
                headerPresenter.IsHitTestVisible = true;
            UpdateHeader();
            if (monthsGrid != null)
            {
                foreach (CalendarDayButton calendarMonthButton in monthsGrid.Children)
                {
                    if (calendarMonthButton != null)
                    {
                        calendarMonthButton.UpdateLayout();
                        TransitionContentControl transitionControl = calendarMonthButton.FindElementOfTypeWithName<TransitionContentControl>("PART_Content");
                        if (transitionControl != null && transitionControl.Transition is SlideTransition)
                            (transitionControl.Transition as SlideTransition).Direction = SlideDirection.Up;
                        if (GetMonth(calendarMonthButton.Content.ToString()) == DisplayMonth)
                        {
                            UpdateMonthButtonSelection(calendarMonthButton);
                        }
                    }
                }
            }
        }

        void UpdateHeader()
        {
            YearHeader = EnableMonthView ? DisplayYear.ToString() : ((GetStartYear(DisplayYear)).ToString() + " - " + GetEndYear(DisplayYear).ToString());
        }

        void UpdateYearView(SlideDirection slideDirection)
        {
            if (yearsGrid != null)
            {
                int startYear = GetStartYear(DisplayYear) - 1;

                foreach (CalendarDayButton calendarYearButton in yearsGrid.Children)
                {
                    if (calendarYearButton != null)
                    {
                        calendarYearButton.UpdateLayout();
                        if (startYear < DateTime.MinValue.Year || startYear > DateTime.MaxValue.Year)
                        {
                            calendarYearButton.Content = null;
                            calendarYearButton.IsEnabled = false;
                        }
                        else
                        {
                            TransitionContentControl transitionControl = calendarYearButton.FindElementOfTypeWithName<TransitionContentControl>("PART_Content");
                            if (transitionControl != null && transitionControl.Transition is SlideTransition)
                                (transitionControl.Transition as SlideTransition).Direction = slideDirection;
                            calendarYearButton.Content = startYear;
                            calendarYearButton.IsEnabled = true;
                            if (startYear == DisplayYear && slideDirection == SlideDirection.Down)
                            {
                                UpdateYearButtonSelection(calendarYearButton);
                            }
                        }
                        startYear++;
                    }
                }
            }
        }

        void UpdateNavigationButtonsMode()
        {
            if (previousNavigationButton != null)
                previousNavigationButton.IsEnabled = (SelectedYear > DateTime.MinValue.Year);
            if (nextNavigationButton != null)
                nextNavigationButton.IsEnabled = (SelectedYear < DateTime.MaxValue.Year);
        }

        void UpdateMonthButtonSelection(CalendarDayButton currentSelectedMonthButton)
        {
            if (selectedMonthButton != null)
                VisualStateManager.GoToState(selectedMonthButton, "Unselected", true);
            if (currentSelectedMonthButton != null)
            {
                VisualStateManager.GoToState(currentSelectedMonthButton, "Selected", true);
                selectedMonthButton = currentSelectedMonthButton;
            }
        }

        void UpdateYearButtonSelection(CalendarDayButton currentSelectedYearButton)
        {
            if (selectedYearButton != null)
                VisualStateManager.GoToState(selectedYearButton, "Unselected", true);
            if (currentSelectedYearButton != null)
            {
                VisualStateManager.GoToState(currentSelectedYearButton, "Selected", true);
                selectedYearButton = currentSelectedYearButton;
            }
        }

        int GetStartYear(int currentYear)
        {
            int startYear = (currentYear - (currentYear % 10));
            previousNavigationButton.IsEnabled = (startYear > DateTime.MinValue.Year);
            return startYear;
        }

        int GetEndYear(int startYear)
        {
            int endYear = (GetStartYear(DisplayYear) + 9);
            nextNavigationButton.IsEnabled = (endYear < DateTime.MaxValue.Year);
            return endYear;
        }

        int GetMonth(string month)
        {
            switch (month)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 1;
            }
        }

        #endregion

        #region Selected Month Changed Event

        public delegate void SelectedMonthChangedEventHandler(object sender, SelectedMonthChangedEventArgs e);

        public event SelectedMonthChangedEventHandler SelectedMonthChanged;

        /// <summary>
        /// Gets the Selected Month Changed event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.SelectedMonthChangedEventArgs"/> instance containing the event data.</param>
        internal void GetSelectedMonthChangedEvent(SelectedMonthChangedEventArgs e)
        {
            if (SelectedMonthChanged != null)
                SelectedMonthChanged(this, e);
        }

        #endregion

        #region Selected Year Changed Event

        public delegate void SelectedYearChangedEventHandler(object sender, SelectedYearChangedEventArgs e);

        public event SelectedYearChangedEventHandler SelectedYearChanged;

        /// <summary>
        /// Gets the Selected Year Changed event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.UI.Xaml.Schedule.SelectedYearChangedEventArgs"/> instance containing the event data.</param>
        internal void GetSelectedYearChangedEvent(SelectedYearChangedEventArgs e)
        {
            if (SelectedYearChanged != null)
                SelectedYearChanged(this, e);
        }

        #endregion
    }

    #region SelectedMonthChangedEventArgs

    public class SelectedMonthChangedEventArgs : EventArgs
    {
        #region CLR Properties

        #region Month

        public int Month { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #region SelectedYearChangedEventArgs

    public class SelectedYearChangedEventArgs : EventArgs
    {
        #region CLR Properties

        #region Year

        public int Year { get; set; }

        #endregion

        #endregion
    }

    #endregion
}
