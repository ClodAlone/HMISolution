// <copyright file="CalendarDependencyProperties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// CalendarEdit partial class which holds dependency properties.
    /// </summary>
    public partial class CalendarEdit
    {
        #region DP getters and setters

        /// <summary>
        /// Gets or sets the calendar object.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Calendar"/>
        /// </value>
        /// <seealso cref="Calendar"/>
        public Calendar Calendar
        {
            get
            {
                return (Calendar)GetValue(CalendarProperty);
            }

            set
            {
                SetValue(CalendarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the culture of the control.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CultureInfo"/>
        /// </value>
        /// <seealso cref="CultureInfo"/>
        /// <example>
        /// <code>
        ///    //Create a new instance of the CalendarEdit
        ///    CalendarEdit calendarEdit = new CalendarEdit();
        ///    //Initialize the calendar in the German culture
        ///    calendarEdit.Culture = new System.Globalization.CultureInfo("de-DE");
        ///  <para></para>
        /// Result:
        ///    Calendar will be displayed in the German culture.
        ///  </code>
        /// </example>
        [TypeConverter(typeof(CultureInfoTypeConverter)), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)GetValue(CultureProperty);
            }

            set
            {
                SetValue(CultureProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarStyle"/>
        /// Default value is CalendarStyle.Standard.
        /// </value>
        /// <seealso cref="CalendarStyle"/>
        public CalendarStyle CalendarStyle
        {
            get
            {
                return (CalendarStyle)GetValue(CalendarStyleProperty);
            }

            set
            {
                SetValue(CalendarStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// Default value is DateTime.Now.Date.
        /// </value>
        /// <seealso cref="DateTime"/>
        public DateTime Date
        {
            get
            {
                return (DateTime)GetValue(DateProperty);
            }

            set
            {
                SetValue(DateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of selected dates.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DatesCollection"/>
        /// </value>
        /// <seealso cref="DatesCollection"/>
        public DatesCollection SelectedDates
        {
            get
            {
                return (DatesCollection)GetValue(SelectedDatesProperty);
            }

            set
            {
                if (AllowSelection)
                {
                    SetValue(SelectedDatesProperty, value);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public BlackDatesCollection BlackoutDates
        {
            get
            {
                return (BlackDatesCollection)GetValue(BlackDatesProperty);
            }
            set
            {
                SetValue(BlackDatesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection border brush of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush SelectionBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBorderBrushProperty);
            }

            set
            {
                SetValue(SelectionBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush InValidDateBorderBrush
        {
            get
            {
                return (Brush)GetValue(InValidDateBorderBrushProperty);
            }

            set
            {
                SetValue(InValidDateBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush InValidDateForeGround
        {
            get
            {
                return (Brush)GetValue(InValidDateForeGroundProperty);
            }

            set
            {
                SetValue(InValidDateForeGroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush InValidDateBackground
        {
            get
            {
                return (Brush)GetValue(InValidDateBackgroundProperty);
            }

            set
            {
                SetValue(InValidDateBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush InValidDateCrossBackground
        {
            get
            {
                return (Brush)GetValue(InValidDateCrossBackgroundProperty);
            }

            set
            {
                SetValue(InValidDateCrossBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection foreground of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>The selection foreground.</value>
        public Brush SelectionForeground
        {
            get
            {
                return (Brush)GetValue(SelectionForegroundProperty);
            }

            set
            {
                SetValue(SelectionForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection BorderBrush for the WeekNumbers
        /// </summary>
        public Brush WeekNumberSelectionBorderBrush
        {
            get
            {
                return (Brush)GetValue(WeekNumberSelectionBorderBrushProperty);
            }

            set
            {
                SetValue(WeekNumberSelectionBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection Border Thickness for the WeekNumbers
        /// </summary>
        public Thickness WeekNumberSelectionBorderThickness
        {
            get
            {
                return (Thickness)GetValue(WeekNumberSelectionBorderThicknessProperty);
            }

            set
            {
                SetValue(WeekNumberSelectionBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number border thickness.
        /// </summary>
        /// <value>The week number border thickness.</value>
        public Thickness WeekNumberBorderThickness
        {
            get
            {
                return (Thickness)GetValue(WeekNumberBorderThicknessProperty);
            }

            set
            {
                SetValue(WeekNumberBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection Border CornerRadius for the WeekNumbers
        /// </summary>
        public CornerRadius WeekNumberSelectionBorderCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(WeekNumberSelectionBorderCornerRadiusProperty);
            }

            set
            {
                SetValue(WeekNumberSelectionBorderCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background for the WeekNumbers
        /// </summary>
        public Brush WeekNumberBackground
        {
            get
            {
                return (Brush)GetValue(WeekNumberBackgroundProperty);
            }

            set
            {
                SetValue(WeekNumberBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush MouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(MouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(MouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush NotCurrentMonthForeground
        {
            get
            {
                return (Brush)GetValue(NotCurrentMonthForegroundProperty);
            }

            set
            {
                SetValue(NotCurrentMonthForegroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush MouseOverForeground
        {
            get
            {
                return (Brush)GetValue(MouseOverForegroundProperty);
            }

            set
            {
                SetValue(MouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }

            set
            {
                SetValue(MouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush SelectedDayCellBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectedDayCellBorderBrushProperty);
            }

            set
            {
                SetValue(SelectedDayCellBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush SelectedDayCellBackground
        {
            get
            {
                return (Brush)GetValue(SelectedDayCellBackgroundProperty);
            }

            set
            {
                SetValue(SelectedDayCellBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush SelectedDayCellHoverBackground
        {
            get
            {
                return (Brush)GetValue(SelectedDayCellHoverBackgroundProperty);
            }

            set
            {
                SetValue(SelectedDayCellHoverBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush TodayCellBorderBrush
        {
            get
            {
                return (Brush)GetValue(TodayCellBorderBrushProperty);
            }

            set
            {
                SetValue(TodayCellBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush TodayCellBackground
        {
            get
            {
                return (Brush)GetValue(TodayCellBackgroundProperty);
            }

            set
            {
                SetValue(TodayCellBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush TodayCellForeground
        {
            get
            {
                return (Brush)GetValue(TodayCellForegroundProperty);
            }

            set
            {
                SetValue(TodayCellForegroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush TodayCellSelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(TodayCellSelectedBorderBrushProperty);
            }

            set
            {
                SetValue(TodayCellSelectedBorderBrushProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush TodayCellSelectedBackground
        {
            get
            {
                return (Brush)GetValue(TodayCellSelectedBackgroundProperty);
            }

            set
            {
                SetValue(TodayCellSelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Brush SelectedDayCellForeground
        {
            get
            {
                return (Brush)GetValue(SelectedDayCellForegroundProperty);
            }

            set
            {
                SetValue(SelectedDayCellForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground for the WeekNumbers
        /// </summary>
        public Brush WeekNumberForeground
        {
            get
            {
                return (Brush)GetValue(WeekNumberForegroundProperty);
            }

            set
            {
                SetValue(WeekNumberForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number border brush.
        /// </summary>
        /// <value>The week number border brush.</value>
        public Brush WeekNumberBorderBrush
        {
            get
            {
                return (Brush)GetValue(WeekNumberBorderBrushProperty);
            }

            set
            {
                SetValue(WeekNumberBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number hover border brush.
        /// </summary>
        /// <value>The week number hover border brush.</value>
        public Brush WeekNumberHoverBorderBrush
        {
            get
            {
                return (Brush)GetValue(WeekNumberHoverBorderBrushProperty);
            }

            set
            {
                SetValue(WeekNumberHoverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number selection background.
        /// </summary>
        /// <value>The week number selection background.</value>
        public Brush WeekNumberSelectionBackground
        {
            get
            {
                return (Brush)GetValue(WeekNumberSelectionBackgroundProperty);
            }

            set
            {
                SetValue(WeekNumberSelectionBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number hover background.
        /// </summary>
        /// <value>The week number hover background.</value>
        public Brush WeekNumberHoverBackground
        {
            get
            {
                return (Brush)GetValue(WeekNumberHoverBackgroundProperty);
            }

            set
            {
                SetValue(WeekNumberHoverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number hover foreground.
        /// </summary>
        /// <value>The week number hover foreground.</value>
        public Brush WeekNumberHoverForeground
        {
            get
            {
                return (Brush)GetValue(WeekNumberHoverForegroundProperty);
            }

            set
            {
                SetValue(WeekNumberHoverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number selection foreground.
        /// </summary>
        /// <value>The week number selection foreground.</value>
        public Brush WeekNumberSelectionForeground
        {
            get
            {
                return (Brush)GetValue(WeekNumberSelectionForegroundProperty);
            }

            set
            {
                SetValue(WeekNumberSelectionForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection border corner radius of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Default radius is 5.
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius SelectionBorderCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(SelectionBorderCornerRadiusProperty);
            }

            set
            {
                SetValue(SelectionBorderCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week number corner radius.
        /// </summary>
        /// <value>The week number corner radius.</value>
        public CornerRadius WeekNumberCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(WeekNumberCornerRadiusProperty);
            }

            set
            {
                SetValue(WeekNumberCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether date selection
        /// is allowed. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool AllowSelection
        {
            get
            {
                return (bool)GetValue(AllowSelectionProperty);
            }

            set
            {
                SetValue(AllowSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiply date selection
        /// is allowed. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool AllowMultiplySelection
        {
            get
            {
                return (bool)GetValue(AllowMultiplySelectionProperty);
            }

            set
            {
                SetValue(AllowMultiplySelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is today button clicked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is today button clicked; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool IsTodayButtonClicked
        {
            get
            {
                return (bool)GetValue(IsTodayButtonClickedProperty);
            }

            set
            {
                SetValue(IsTodayButtonClickedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether day names
        /// should be abbreviated. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsDayNamesAbbreviated
        {
            get
            {
                return (bool)GetValue(IsDayNamesAbbreviatedProperty);
            }

            set
            {
                SetValue(IsDayNamesAbbreviatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether month names
        /// should be abbreviated. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsMonthNameAbbreviated
        {
            get
            {
                return (bool)GetValue(IsMonthNameAbbreviatedProperty);
            }

            set
            {
                SetValue(IsMonthNameAbbreviatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether week numbers
        /// should be shown. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsShowWeekNumbers
        {
            get
            {
                return (bool)GetValue(IsShowWeekNumbersProperty);
            }

            set
            {
                SetValue(IsShowWeekNumbersProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether week numbers Grid
        /// should be shown. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsShowWeekNumbersGrid
        {
            get
            {
                return (bool)GetValue(IsShowWeekNumbersGridProperty);
            }

            set
            {
                SetValue(IsShowWeekNumbersGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether year editing should be enabled.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsAllowYearSelection
        {
            get
            {
                return (bool)GetValue(IsAllowYearSelectionProperty);
            }

            set
            {
                SetValue(IsAllowYearSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether previous month days are visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowPreviousMonthDays
        {
            get
            {
                return (bool)GetValue(ShowPreviousMonthDaysProperty);
            }

            set
            {
                SetValue(ShowPreviousMonthDaysProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether next month days are visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowNextMonthDays
        {
            get
            {
                return (bool)GetValue(ShowNextMonthDaysProperty);
            }

            set
            {
                SetValue(ShowNextMonthDaysProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a selection range mode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SelectionRangeMode"/>
        /// Default value is SelectionRangeMode.CurrentMonth.
        /// </value>
        /// <seealso cref="SelectionRangeMode"/>
        public SelectionRangeMode SelectionRangeMode
        {
            get
            {
                return (SelectionRangeMode)GetValue(SelectionRangeModeProperty);
            }

            set
            {
                SetValue(SelectionRangeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a month changing animation time.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 300.
        /// </value>
        /// <seealso cref="int"/>
        public int FrameMovingTime
        {
            get
            {
                return (int)GetValue(FrameMovingTimeProperty);
            }

            set
            {
                SetValue(FrameMovingTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a calendar mode changing animation time.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 300.
        /// </value>
        /// <seealso cref="int"/>
        public int ChangeModeTime
        {
            get
            {
                return (int)GetValue(ChangeModeTimeProperty);
            }

            set
            {
                SetValue(ChangeModeTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the month change direction.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AnimationDirection"/>
        /// </value>
        /// <seealso cref="AnimationDirection"/>
        public AnimationDirection MonthChangeDirection
        {
            get
            {
                return (AnimationDirection)GetValue(MonthChangeDirectionProperty);
            }

            set
            {
                SetValue(MonthChangeDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day name cells content template.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate DayNameCellsDataTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DayNameCellsDataTemplateProperty);
            }

            set
            {
                SetValue(DayNameCellsDataTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the previous scroll button content template.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public ControlTemplate PreviousScrollButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(PreviousScrollButtonTemplateProperty);
            }

            set
            {
                SetValue(PreviousScrollButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the next scroll button content template.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public ControlTemplate NextScrollButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(NextScrollButtonTemplateProperty);
            }

            set
            {
                SetValue(NextScrollButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day cells content template.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate DayCellsDataTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DayCellsDataTemplateProperty);
            }

            set
            {
                SetValue(DayCellsDataTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day cells style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="Style"/>
        public Style DayCellsStyle
        {
            get
            {
                return (Style)GetValue(DayCellsStyleProperty);
            }

            set
            {
                SetValue(DayCellsStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day name cells style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="Style"/>
        public Style DayNameCellsStyle
        {
            get
            {
                return (Style)GetValue(DayNameCellsStyleProperty);
            }

            set
            {
                SetValue(DayNameCellsStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day cells data template selector.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplateSelector"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplateSelector"/>
        public DataTemplateSelector DayCellsDataTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(DayCellsDataTemplateSelectorProperty);
            }

            set
            {
                SetValue(DayCellsDataTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day name cells data template selector.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplateSelector"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="DataTemplateSelector"/>
        public DataTemplateSelector DayNameCellsDataTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(DayNameCellsDataTemplateSelectorProperty);
            }

            set
            {
                SetValue(DayNameCellsDataTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day cells content template according to
        /// its date. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplatesDictionary"/>
        /// </value>
        /// <seealso cref="DataTemplatesDictionary"/>
        public DataTemplatesDictionary DateDataTemplates
        {
            get
            {
                return (DataTemplatesDictionary)GetValue(DateDataTemplatesProperty);
            }

            set
            {
                SetValue(DateDataTemplatesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day cells style according to its date.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="StylesDictionary"/>
        /// </value>
        /// <seealso cref="StylesDictionary"/>
        public StylesDictionary DateStyles
        {
            get
            {
                return (StylesDictionary)GetValue(DateStylesProperty);
            }

            set
            {
                SetValue(DateStylesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of DisableDateSelection property.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DisableDateSelection"/>
        /// Default value of DisableDateSelection is false.
        /// </value>
        /// <seealso cref="DisableDateSelection"/>
        public bool DisableDateSelection
        {
            get
            {
                return (bool)GetValue(DisableDateSelectionProperty);
            }

            set
            {
                SetValue(DisableDateSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to scroll to the selected date.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ScrollToDateEnabled
        {
            get
            {
                return (bool)GetValue(ScrollToDateEnabledProperty);
            }

            set
            {
                SetValue(ScrollToDateEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day names grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DayNamesGrid"/>
        /// </value>
        /// <seealso cref="DayNamesGrid"/>
        protected internal DayNamesGrid DayNamesGrid
        {
            get
            {
                return (DayNamesGrid)GetValue(DayNamesGridProperty);
            }

            set
            {
                SetValue(DayNamesGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the week numbers grid.
        /// </summary>
        /// <value>The week numbers grid.</value>
        protected internal WeekNumbersGrid WeekNumbersGrid
        {
            get
            {
                return (WeekNumbersGrid)GetValue(WeekNumbersGridProperty);
            }

            set
            {
                SetValue(WeekNumbersGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a day grid for the current month.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DayGrid"/>
        /// </value>
        /// <seealso cref="DayGrid"/>
        protected internal DayGrid CurrentDayGrid
        {
            get
            {
                return (DayGrid)GetValue(CurrentDayGridProperty);
            }

            set
            {
                SetValue(CurrentDayGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a day grid for the next changed month.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DayGrid"/>
        /// </value>
        /// <seealso cref="DayGrid"/>
        protected internal DayGrid FollowingDayGrid
        {
            get
            {
                return (DayGrid)GetValue(FollowingDayGridProperty);
            }

            set
            {
                SetValue(FollowingDayGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the month grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MonthGrid"/>
        /// </value>
        /// <seealso cref="MonthGrid"/>
        protected internal MonthGrid CurrentMonthGrid
        {
            get
            {
                return (MonthGrid)GetValue(CurrentMonthGridProperty);
            }

            set
            {
                SetValue(CurrentMonthGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the year grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="YearGrid"/>
        /// </value>
        /// <seealso cref="YearGrid"/>
        protected internal YearGrid CurrentYearGrid
        {
            get
            {
                return (YearGrid)GetValue(CurrentYearGridProperty);
            }

            set
            {
                SetValue(CurrentYearGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the year range grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="YearRangeGrid"/>
        /// </value>
        /// <seealso cref="YearRangeGrid"/>
        protected internal YearRangeGrid CurrentYearRangeGrid
        {
            get
            {
                return (YearRangeGrid)GetValue(CurrentYearRangeGridProperty);
            }

            set
            {
                SetValue(CurrentYearRangeGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets current year week number grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="WeekNumberGridPanel"/>
        /// </value>
        /// <seealso cref="WeekNumberGridPanel"/>
        protected internal WeekNumberGridPanel CurrentWeekNumbersGrid
        {
            get
            {
                return (WeekNumberGridPanel)GetValue(CurrentWeekNumbersGridProperty);
            }

            set
            {
                SetValue(CurrentWeekNumbersGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets following year week number grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="WeekNumberGridPanel"/>
        /// </value>
        /// <seealso cref="WeekNumberGridPanel"/>
        protected internal WeekNumberGridPanel FollowingWeekNumbersGrid
        {
            get
            {
                return (WeekNumberGridPanel)GetValue(FollowingWeekNumbersGridProperty);
            }

            set
            {
                SetValue(FollowingWeekNumbersGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the following month grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MonthGrid"/>
        /// </value>
        /// <seealso cref="MonthGrid"/>
        protected internal MonthGrid FollowingMonthGrid
        {
            get
            {
                return (MonthGrid)GetValue(FollowingMonthGridProperty);
            }

            set
            {
                SetValue(FollowingMonthGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the following year grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="YearGrid"/>
        /// </value>
        /// <seealso cref="YearGrid"/>
        protected internal YearGrid FollowingYearGrid
        {
            get
            {
                return (YearGrid)GetValue(FollowingYearGridProperty);
            }

            set
            {
                SetValue(FollowingYearGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the following year range grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="YearRangeGrid"/>
        /// </value>
        /// <seealso cref="YearRangeGrid"/>
        protected internal YearRangeGrid FollowingYearRangeGrid
        {
            get
            {
                return (YearRangeGrid)GetValue(FollowingYearRangeGridProperty);
            }

            set
            {
                SetValue(FollowingYearRangeGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinDate.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MinDate"/>
        /// </value>
        /// <seealso cref="MinDate"/>
        public DateTime MinDate
        {
            get
            {
                return (DateTime)GetValue(CalendarEdit.MinDateProperty);
            }
            set
            {
                SetValue(CalendarEdit.MinDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MaxDate.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MaxDate"/>
        /// </value>
        /// <seealso cref="MaxDate"/>
        public DateTime MaxDate
        {
            get
            {
                return (DateTime)GetValue(CalendarEdit.MaxDateProperty);
            }
            set
            {
                SetValue(CalendarEdit.MaxDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date visible settings.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisibleDate"/>
        /// </value>
        /// <seealso cref="VisibleDate"/>
        protected internal VisibleDate VisibleData
        {
            get
            {
                return (VisibleDate)GetValue(VisibleDataProperty);
            }

            set
            {
                SetValue(VisibleDataProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header foreground of the calendar.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush HeaderForeground
        {
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }

            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header background of the calendar.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets the today date.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.Empty.
        /// </value>
        /// <seealso cref="string"/>
        public string TodayDate
        {
            get
            {
                return (string)GetValue(TodayDateProperty);
            }

            private set
            {
                SetValue(TodayDatePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether today bar is visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool TodayRowIsVisible
        {
            get
            {
                return (bool)GetValue(TodayRowIsVisibleProperty);
            }

            set
            {
                SetValue(TodayRowIsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [min max hidden].
        /// </summary>
        /// <value><c>true</c> if [min max hidden]; otherwise, <c>false</c>.</value>
        public bool MinMaxHidden
        {
            get
            {
                return (bool)GetValue(MinMaxHiddenProperty);
            }

            set
            {
                SetValue(MinMaxHiddenProperty, value);
            }
        }

        #endregion DP getters and setters

        #region private static

        /// <summary>
        /// Invoked whenever the <see cref="Calendar"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCalendarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnCalendarChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCalendarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnCalendarStyleChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Culture"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnCultureChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Date"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDateChanged(e);
            instance.FireMonthChanged();
        }

        private void FireMonthChanged()
        {
            if (this.MonthChanged != null)
            {
                this.MonthChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowSelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAllowSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnAllowSelectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowMultiplySelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAllowMultiplySelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnAllowMultiplySelectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsMonthNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsMonthNameAbbreviatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsMonthNameAbbreviatedChanged(e);
        }

        /// <summary>
        /// Called when [is today button clicked changed].
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsTodayButtonClickedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsTodayButtonClickedChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionRangeMode"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectionRangeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectionRangeModeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectionBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectionBorderBrushChanged(e);
        }

        private static void OnInValidDateBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnInValidDateBorderBrushChanged(e);
        }

        private void OnInValidDateBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void OnInValidDatForeGroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnInValidDatForeGroundChanged(e);
        }

        private void OnInValidDatForeGroundChanged(DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void OnInValidDateBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnInValidDateBackgroundChanged(e);
        }

        private void OnInValidDateBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void InValidDateCrossBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.InValidDateCrossBackgroundChanged(e);
        }

        private void InValidDateCrossBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private static void OnMouseOverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnMouseOverBorderBrushChanged(e);
        }

        private static void OnMouseOverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnMouseOverForegroundChanged(e);
        }

        private static void OnMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnMouseOverBackgroundChanged(e);
        }

        private static void OnSelectedDayCellBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectedDayCellBorderBrushChanged(e);
        }

        private static void OnSelectedDayCellHoverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectedDayCellHoverBackgroundChanged(e);
        }

        private static void OnTodayCellBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayCellBorderBrushChanged(e);
        }

        private static void OnTodayCellForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayCellForegroundChanged(e);
        }

        private static void OnTodayCellBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayCellBackgroundChanged(e);
        }

        private static void OnTodayCellSelectedBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayCellSelectedBorderBrushChanged(e);
        }

        private static void OnTodayCellSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayCellSelectedBackgroundChanged(e);
        }

        private static void OnNotCurrentMonthForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnNotCurrentMonthForegroundChanged(e);
        }

        private static void OnSelectedDayCellBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectedDayCellBackgroundChanged(e);
        }

        private static void OnSelectedDayCellForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectedDayCellForegroundChanged(e);
        }

        /// <summary>
        /// Called when [selection foreground changed].
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectionForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectionForegroundChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="WeekNumberSelectionBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWeekNumberSelectionBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberSelectionBorderBrushChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="WeekNumberSelectionBorderThickness"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWeekNumberSelectionBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberSelectionBorderThicknessChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="WeekNumberSelectionBorderCornerRadius"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWeekNumberSelectionBorderCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberSelectionBorderCornerRadiusChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="WeekNumberBackground"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWeekNumberBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberBackgroundChanged(e);
        }

        /// <summary>
        /// Called when [week number hover background changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberHoverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberHoverBackgroundChanged(e);
        }

        /// <summary>
        /// Called when [week number hover border brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberHoverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberHoverBorderBrushChanged(e);
        }

        /// <summary>
        /// Called when [week number selection background changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberSelectionBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberSelectionBackgroundChanged(e);
        }

        /// <summary>
        /// Called when [week number border brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberBorderBrushChanged(e);
        }

        /// <summary>
        /// Called when [week number border thickness changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberBorderThicknessChanged(e);
        }

        /// <summary>
        /// Called when [week number corner radius changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberCornerRadiusChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="WeekNumberForeground"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWeekNumberForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberForegroundChanged(e);
        }

        /// <summary>
        /// Called when [week number selection foreground changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberSelectionForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberSelectionForegroundChanged(e);
        }

        /// <summary>
        /// Called when [week number hover foreground changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekNumberHoverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnWeekNumberHoverForegroundChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionBorderCornerRadius"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectionBorderCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnSelectionBorderCornerRadiusChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="FrameMovingTime"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnFrameMovingTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnFrameMovingTimeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ChangeModeTime"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnChangeModeTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnChangeModeTimeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="MonthChangeDirection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnMonthChangeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnMonthChangeDirectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsDayNamesAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsDayNamesAbbreviatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsDayNamesAbbreviatedChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsDataTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayNameCellsDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayNameCellsDataTemplateChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="PreviousScrollButtonTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnPreviousScrollButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnPreviousScrollButtonTemplateChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="NextScrollButtonTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnNextScrollButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnNextScrollButtonTemplateChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsDataTemplate"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayCellsDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayCellsDataTemplateChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsStyle"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayCellsStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayCellsStyleChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsStyle"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayNameCellsStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayNameCellsStyleChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsDataTemplateSelector"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayCellsDataTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayCellsDataTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsDataTemplateSelector"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDayNameCellsDataTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDayNameCellsDataTemplateSelectorChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DateDataTemplates"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDateDataTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDateDataTemplatesChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsShowWeekNumbers"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsShowWeekNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsShowWeekNumbersChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsShowWeekNumbersGrid"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsShowWeekNumbersGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsShowWeekNumbersGridChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsAllowYearSelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsAllowYearSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnIsAllowYearSelectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ShowPreviousMonthDays"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowPreviousMonthDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnShowPreviousMonthDaysChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ShowNextMonthDays"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowNextMonthDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnShowNextMonthDaysChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="TodayRowIsVisible"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnTodayRowIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnTodayRowIsVisibleChanged(e);
        }

        /// <summary>
        /// Called when [min max hidden changed].
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinMaxHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnMinMaxHiddenChanged(e);
        }

        /// <summary>
        /// Called when [MinDate changed].
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit ce = (CalendarEdit)d;
            ce.MinDate = (DateTime)e.NewValue;
            ce.CoerceValue(VisibleDataProperty);
        }

        /// <summary>
        /// Called when [coerce MinDate].
        /// </summary>
        /// <param name="sender">The dependency property.</param>
        /// <param name="data">The value.</param>
        /// <returns>Coerce object.</returns>
        private static object OnCoerceMinDateProperty(DependencyObject sender, object data)
        {
            CalendarEdit ce = (CalendarEdit)sender;
            ce.CoerceValue(VisibleDataProperty);
            return data;
        }

        /// <summary>
        /// Called when [MaxDate changed].
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit ce = (CalendarEdit)d;
            ce.MaxDate = (DateTime)e.NewValue;
            ce.CoerceValue(VisibleDataProperty);
        }

        /// <summary>
        /// Called when [coerce MaxDate].
        /// </summary>
        /// <param name="sender">The dependency property.</param>
        /// <param name="data">The value.</param>
        /// <returns>Coerce object.</returns>
        private static object OnCoerceMaxDateProperty(DependencyObject sender, object data)
        {
            CalendarEdit ce = (CalendarEdit)sender;
            ce.CoerceValue(VisibleDataProperty);
            return data;
        }

        /// <summary>
        /// Called when [coerce visible data].
        /// </summary>
        /// <param name="d">The dependency property.</param>
        /// <param name="value">The value.</param>
        /// <returns>Coerce object.</returns>
        private static object OnCoerceVisibleData(DependencyObject d, object value)
        {
            CalendarEdit calendar = d as CalendarEdit;
            return calendar.OnCoerceVisibleData(value);
        }

        /// <summary>
        /// Invoked whenever the <see cref="VisibleData"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisibleDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnVisibleDataChanged(e);
        }

        private static void OnVisualModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnVisualModeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ScrollToDateEnabled"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnScrollToDateEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnScrollToDateEnabledChanged(e);
        }

        /// <summary>
        /// Called when [date styles changed].
        /// </summary>
        /// <param name="d">The d object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDateStylesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDateStylesChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DisableDateSelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDisableDateSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDisableDateSelectionChanged(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        private static void OnDatesCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarEdit instance = (CalendarEdit)d;
            instance.OnDatesCollectionChanged(e);
        }

        private static void OnBlackDatesCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //CalendarEdit instance = (CalendarEdit)d;
            //instance.OnBlackDatesCollectionChanged(e);
        }

        /// <summary>
        /// Coerces the value of <see cref="Date"/> property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnCoerceDate(DependencyObject d, object value)
        {
            CalendarEdit instance = (CalendarEdit)d;
            DateTime date = (DateTime)value;
            DateTime newDate = instance.OnCoerceDate(date);

            if (date != newDate)
            {
                return newDate;
            }
            else
            {
                return value;
            }
        }

        #endregion private static

        #region protected virtual

        /// <summary>
        /// Raises the <see cref="IsShowWeekNumbersChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsShowWeekNumbersChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsShowWeekNumbersChanged != null)
            {
                IsShowWeekNumbersChanged(this, e);
            }

            if (m_mainGrid == null && m_weekNumbersContainer == null)
            {
                m_mainGrid = GetMainGrid();
                m_weekNumbersContainer = GetWeekNumbersContainer();
            }

            if (m_mainGrid != null && m_weekNumbersContainer != null)
            {
                if ((bool)e.NewValue && VisualMode == CalendarVisualMode.Days)
                {
                    ShowWeekNumbersContainer();
                }
                else
                {
                    HideWeekNumbersContainer();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="IsShowWeekNumbersGridChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsShowWeekNumbersGridChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsShowWeekNumbersGridChanged != null)
            {
                IsShowWeekNumbersGridChanged(this, e);
            }

            if (wcurrentweekNumbersContainer != null && wfollowweekNumbersContainer != null)
            {
                if (IsShowWeekNumbersGrid == true)
                {
                    if ((bool)e.NewValue == true && VisualMode == CalendarVisualMode.WeekNumbers)
                    {
                        ShowWeekNumbersForYearContainer();
                    }

                    if ((bool)e.NewValue == true && VisualMode == CalendarVisualMode.Days)
                    {
                        HideWeekNumbersForYearContainer();
                    }
                }

                if ((bool)e.NewValue == false && VisualMode == CalendarVisualMode.WeekNumbers)
                {
                    ChangeVisualMode(ChangeVisualModeDirection.Up);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="IsAllowYearSelectionChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsAllowYearSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAllowYearSelectionChanged != null)
            {
                IsAllowYearSelectionChanged(this, e);
            }

            if (m_yearUpDownPanel == null)
            {
                ApplyYearEditingTemplate();
                ApplyEditMonthTemplate();
            }
            else
            {
                if ((bool)e.NewValue)
                {
                    AddMonthButtonsEvents();
                }
                else
                {
                    DeleteMonthButtonsEvents();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ShowPreviousMonthDaysChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowPreviousMonthDaysChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowPreviousMonthDaysChanged != null)
            {
                ShowPreviousMonthDaysChanged(this, e);
            }

            InitVisibleDayGrid();
        }

        /// <summary>
        /// Raises the <see cref="ShowNextMonthDaysChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowNextMonthDaysChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowNextMonthDaysChanged != null)
            {
                ShowNextMonthDaysChanged(this, e);
            }

            InitVisibleDayGrid();
        }

        /// <summary>
        /// Raises the <see cref="TodayRowIsVisibleChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTodayRowIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TodayRowIsVisibleChanged != null)
            {
                TodayRowIsVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MinMaxHiddenChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMinMaxHiddenChanged(DependencyPropertyChangedEventArgs e)
        {
            //ScrollToDate();

            CurrentDayGrid.Initialize(VisibleData, Culture, Calendar);
            FollowingDayGrid.Initialize(VisibleData, Culture, Calendar);

            if (MinMaxHiddenChanged != null)
            {
                MinMaxHiddenChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [coerce visible data].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Visible Date.</returns>
        protected virtual VisibleDate OnCoerceVisibleData(object obj)
        {
            VisibleDate visData = (VisibleDate)obj;
            if (MinMaxHidden)
            {
                int minYear = Calendar.GetYear(MinDate);
                int minMonth = Calendar.GetMonth(MinDate);
                int minDay = Calendar.GetDayOfMonth(MinDate);

                if (new Date(visData.VisibleYear, visData.VisibleMonth, 31) < new Date(minYear, minMonth, MinDate.Day))
                {
                    DateTime minDateTime = new DateTime(MinDate.Year, MinDate.Month, MinDate.Day);
                    int year = Calendar.GetYear(minDateTime);
                    int month = Calendar.GetMonth(minDateTime);
                    int day = Calendar.GetDayOfMonth(minDateTime);

                    return new VisibleDate(year, month, day);
                }
                else if (new Date(visData.VisibleYear, visData.VisibleMonth, 1) > new Date(MaxDate.Year, MaxDate.Month, MaxDate.Day))
                {
                    DateTime maxDateTime = new DateTime(MaxDate.Year, MaxDate.Month, MaxDate.Day);
                    int year = Calendar.GetYear(maxDateTime);
                    int month = Calendar.GetMonth(maxDateTime);
                    int day = Calendar.GetDayOfMonth(maxDateTime);

                    return new VisibleDate(year, month, day);
                }
            }

            else
            {
                int minYear = Calendar.GetYear(miDate);
                int minMonth = Calendar.GetMonth(miDate);
                int minDay = Calendar.GetDayOfMonth(miDate);

                //if (miDate.ToString() == "1/1/01" && MinDate.ToString() != "4/30/1900")
                //{
                //}
                //else
                //{
                //    if(MinDate.ToString()=="4/30/1900")
                //    {
                //        miDate=MinDate;
                //    }
                //    }

                if (new Date(visData.VisibleYear, visData.VisibleMonth, 31) < new Date(minYear, minMonth, miDate.Day))
                {
                    DateTime minDateTime = new DateTime(miDate.Year, miDate.Month, miDate.Day);
                    int year = Calendar.GetYear(minDateTime);
                    int month = Calendar.GetMonth(minDateTime);
                    int day = Calendar.GetDayOfMonth(minDateTime);
                    return new VisibleDate(year, month, day);
                }
                else if (new Date(visData.VisibleYear, visData.VisibleMonth, 1) > new Date(mxDate.Year, mxDate.Month, mxDate.Day))
                {
                    DateTime maxDateTime = new DateTime(mxDate.Year, mxDate.Month, mxDate.Day);
                    int year = Calendar.GetYear(maxDateTime);
                    int month = Calendar.GetMonth(maxDateTime);
                    int day = Calendar.GetDayOfMonth(maxDateTime);
                    return new VisibleDate(year, month, day);
                }
            }

            return visData;
        }

        /// <summary>
        /// Invoked whenever the <see cref="VisibleData"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnVisibleDataChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisibleDataChanged != null)
            {
                VisibleDataChanged(this, e);
            }

            DateTimeFormatInfo format = Culture.DateTimeFormat;
            if (VisualMode == CalendarVisualMode.Days)
            {
                DayNamesGrid.SetDayNames(format);
            }

            if (VisualModeInfo.NewMode != VisualModeInfo.OldMode)
            {
                FindCurrentGrid(VisualModeInfo.NewMode).Initialize(VisibleData, Culture, Calendar);
                FindCurrentGrid(VisualModeInfo.OldMode).Initialize(VisibleData, Culture, Calendar);
            }
            else
            {
                VisibleDataToMinSupportedDate(format);
                FindCurrentGrid(VisualMode).Initialize(VisibleData, Culture, Calendar);
            }

            if (m_monthButton1 != null)
            {
                m_monthButton1.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
            }

            if (VisualMode == CalendarVisualMode.Days && FollowingDayGrid.WeekNumbers != null)
            {
                WeekNumbersGrid.SetWeekNumbers(FollowingDayGrid.WeekNumbers);
                UpdateWeekNumbersContainer();
            }

            InitializePopup();
            NavigateButtonVerify();
        }

        /// <summary>
        /// Invoked whenever the <see cref="ScrollToDateEnabled"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnScrollToDateEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScrollToDateEnabledChanged != null)
            {
                ScrollToDateEnabledChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVisualModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualModeChanged != null)
            {
                VisualModeChanged(this, e);
            }
            Rectangle rect1 = this.GetTemplateChild("rect") as Rectangle;
            if (rect1 != null)
            {
                if (VisualMode != CalendarVisualMode.Days)
                {
                    rect1.Visibility = Visibility.Collapsed;
                }
                if (VisualMode == CalendarVisualMode.Days)
                {
                    rect1.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DateStylesChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDateStylesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateStylesChanged != null)
            {
                DateStylesChanged(this, e);
            }

            if (e.OldValue != null)
            {
                ((StylesDictionary)e.OldValue).CollectionChanged -= new NotifyCollectionChangedEventHandler(DateStyles_OnPropertyChanged);
            }

            ((StylesDictionary)e.NewValue).CollectionChanged += new NotifyCollectionChangedEventHandler(DateStyles_OnPropertyChanged);

            if (IsInitializeComplete)
            {
                InitilizeDayCellStyles(CurrentDayGrid);
                InitilizeDayCellStyles(FollowingDayGrid);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="DisableDateSelection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDisableDateSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DisableDateSelectionChanged != null)
            {
                this.DisableDateSelectionChanged(this, e);
            }
            if (this.DisableDateSelection == true)
            {
                DateTimeFormatInfo format = Culture.DateTimeFormat;
                this.DayNamesGrid.Visibility = Visibility.Hidden;
                this.CurrentDayGrid.Visibility = Visibility.Hidden;
                this.CurrentMonthGrid.Visibility = Visibility.Visible;
                this.FollowingMonthGrid.Visibility = Visibility.Hidden;
                this.WeekNumbersGrid.Visibility = Visibility.Hidden;
                this.VisualMode = CalendarVisualMode.Months;
                this.VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Months);
                this.TodayRowIsVisible = false;
                this.IsWeekCellClicked = false;
                VisibleDataToMinSupportedDate(format);
                FindCurrentGrid(VisualMode).Initialize(VisibleData, Culture, Calendar);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="Calendar"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnCalendarChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CalendarChanged != null)
            {
                CalendarChanged(this, e);
            }

            if (e.NewValue == null)
            {
                ClearValue(CalendarProperty);
            }

            if (Date == MinDate)
            {
                MinDate = Calendar.MinSupportedDateTime;
                Date = Calendar.MinSupportedDateTime;
            }

            if (Date == miDate)
            {
                miDate = Calendar.MinSupportedDateTime;
                Date = Calendar.MinSupportedDateTime;
            }
            if (MinDate < Calendar.MinSupportedDateTime)
            {
                MinDate = Calendar.MinSupportedDateTime;
            }

            if (MaxDate > Calendar.MaxSupportedDateTime)
            {
                MaxDate = Calendar.MaxSupportedDateTime;
            }

            if (miDate < Calendar.MinSupportedDateTime)
            {
                miDate = Calendar.MinSupportedDateTime;
            }

            if (mxDate > Calendar.MaxSupportedDateTime)
            {
                mxDate = Calendar.MaxSupportedDateTime;
            }

            if (IsInitializeComplete)
            {
                CoerceVisibleData(Calendar);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnCalendarStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CalendarStyleChanged != null)
            {
                CalendarStyleChanged(this, e);
            }

            if ((CalendarStyle)e.NewValue == CalendarStyle.Standard)
            {
                if (VisualMode != CalendarVisualMode.Days)
                {
                    m_bsuspendEventFire = true;

                    OnDateChanged(new DependencyPropertyChangedEventArgs());

                    m_bsuspendEventFire = false;
                }

                if (IsAllowYearSelection)
                {
                    AddMonthButtonsEvents();
                }
            }
            else
            {
                DeleteMonthButtonsEvents();
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="Culture"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnCultureChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CultureChanged != null)
            {
                CultureChanged(this, e);
            }

            CultureInfo newCulture = (CultureInfo)e.NewValue;

            if (newCulture != null && newCulture.IsNeutralCulture)
            {
                Culture = new CultureInfo(newCulture.LCID + 1024);
            }

            UpdateMinDate(e);
            CoerceVisibleData(Calendar);

            TodayDate = DateTime.Now.ToString("D", Culture.DateTimeFormat);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Date"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateChanged != null && !m_bsuspendEventFire && Invalidateflag)
            {
                DateChanged(this, e);
            }

            if (IsInitializeComplete)
            {
                if (VisualMode == CalendarVisualMode.Days)
                {
                    CurrentDayGrid.SetIsDate(Calendar);
                    FollowingDayGrid.SetIsDate(Calendar);

                    if (Keyboard.Modifiers != ModifierKeys.Shift)
                    {
                        if (m_shiftDateChangeEnabled)
                        {
                            m_shiftDate = Date;
                        }
                    }

                    if (!m_dateSetManual)
                    {
                        if (ScrollToDateEnabled)
                        {
                            ScrollToDate();
                        }
                    }

                    m_dateSetManual = false;
                }
                else
                {
                    if (VisualMode == CalendarVisualMode.Months)
                    {
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days));
                    }

                    if (VisualMode == CalendarVisualMode.Years)
                    {
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.Years, CalendarVisualMode.Months));
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days));
                    }

                    if (VisualMode == CalendarVisualMode.YearsRange)
                    {
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.YearsRange, CalendarVisualMode.Years));
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.Years, CalendarVisualMode.Months));
                        m_visualModeQueue.Enqueue(new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days));
                    }

                    ScrollToDate();
                }
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowSelection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnAllowSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowSelectionChanged != null)
            {
                AllowSelectionChanged(this, e);
            }

            SelectedDates.AllowInsert = (bool)e.NewValue;
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionRangeMode"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnSelectionRangeModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectionRangeModeChanged != null)
            {
                SelectionRangeModeChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="SelectionBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush or a Brush inherited type.</exception>
        protected virtual void OnSelectionBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectionBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectionBorderBrushChanged != null)
            {
                SelectionBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SelectionForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectionForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectionForeground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectionForegroundChanged != null)
            {
                SelectionForegroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseOverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("MouseOverBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (MouseOverBorderBrushChanged != null)
            {
                MouseOverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("MouseOverBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (MouseOverBackgroundChanged != null)
            {
                MouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMouseOverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("MouseOverForeground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (MouseOverForegroundChanged != null)
            {
                MouseOverForegroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDayCellBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectedDayCellBorderBrushChanged != null)
            {
                SelectedDayCellBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDayCellHoverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellHoverBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectedDayCellHoverBackgroundChanged != null)
            {
                SelectedDayCellHoverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTodayCellBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellHoverBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (TodayCellBorderBrushChanged != null)
            {
                TodayCellBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTodayCellForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("TodayCellForeground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (TodayCellForegroundChanged != null)
            {
                TodayCellForegroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTodayCellBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("TodayCellBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (TodayCellBackgroundChanged != null)
            {
                TodayCellBackgroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTodayCellSelectedBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("TodayCellSelectedBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (TodayCellSelectedBorderBrushChanged != null)
            {
                TodayCellSelectedBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTodayCellSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("TodayCellSelectedBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (TodayCellSelectedBackgroundChanged != null)
            {
                TodayCellSelectedBackgroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNotCurrentMonthForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (NotCurrentMonthForegroundChanged != null)
            {
                NotCurrentMonthForegroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDayCellBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectedDayCellBackgroundChanged != null)
            {
                SelectedDayCellBackgroundChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDayCellForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush newBrush = e.NewValue as Brush;
            if (newBrush == null)
            {
                throw new ArgumentException("SelectedDayCellForeground property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (SelectedDayCellForegroundChanged != null)
            {
                SelectedDayCellForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="WeekNumberSelectionBorderBrush"/> is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush or a Brush inherited type.</exception>
        protected virtual void OnWeekNumberSelectionBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBrush = e.NewValue as Brush;
            if (weekBrush == null)
            {
                throw new ArgumentException("WeekNumberSelectionBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (this.WeekNumberSelectionBorderBrushChanged != null)
            {
                this.WeekNumberSelectionBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBrush = e.NewValue as Brush;
            if (weekBrush == null)
            {
                throw new ArgumentException("WeekNumberBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }

            if (this.WeekNumberBorderBrushChanged != null)
            {
                this.WeekNumberBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="WeekNumberSelectionBorderThickness"/> is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Thickness type.</exception>
        protected virtual void OnWeekNumberSelectionBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness weekThickness = (Thickness)e.NewValue;

            if (weekThickness == null)
            {
                throw new ArgumentException("WeekNumberSelectionBorderThickness property can be assigned only by a Thickness type value.");
            }

            if (this.WeekNumberSelectionBorderThicknessChanged != null)
            {
                this.WeekNumberSelectionBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            Thickness weekThickness = (Thickness)e.NewValue;

            if (weekThickness == null)
            {
                throw new ArgumentException("WeekNumberSelectionBorderThickness property can be assigned only by a Thickness type value.");
            }

            if (this.WeekNumberBorderThicknessChanged != null)
            {
                this.WeekNumberBorderThicknessChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="WeekNumberSelectionBorderCornerRadius"/> is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a CornerRadius type.</exception>
        protected virtual void OnWeekNumberSelectionBorderCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            CornerRadius weekCornerRadius = (CornerRadius)e.NewValue;

            if (weekCornerRadius == null)
            {
                throw new ArgumentException("WeekNumberSelectionBorderCornerRadius property can be assigned only by a CornerRadius type value.");
            }

            if (this.WeekNumberSelectionBorderCornerRadiusChanged != null)
            {
                this.WeekNumberSelectionBorderCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWeekNumberCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            CornerRadius weekCornerRadius = (CornerRadius)e.NewValue;

            if (weekCornerRadius == null)
            {
                throw new ArgumentException("WeekNumberSelectionBorderCornerRadius property can be assigned only by a CornerRadius type value.");
            }

            if (this.WeekNumberCornerRadiusChanged != null)
            {
                this.WeekNumberCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="WeekNumberBackground"/> is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush type.</exception>
        protected virtual void OnWeekNumberBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBackground = e.NewValue as Brush;

            if (weekBackground == null)
            {
                throw new ArgumentException("WeekNumberBackground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberBackgroundChanged != null)
            {
                this.WeekNumberBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberSelectionBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberSelectionBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBackground = e.NewValue as Brush;

            if (weekBackground == null)
            {
                throw new ArgumentException("WeekNumberBackground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberSelectionBackgroundChanged != null)
            {
                this.WeekNumberSelectionBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberHoverBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberHoverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBackground = e.NewValue as Brush;

            if (weekBackground == null)
            {
                throw new ArgumentException("WeekNumberBackground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberHoverBackgroundChanged != null)
            {
                this.WeekNumberHoverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberHoverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberHoverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekBackground = e.NewValue as Brush;

            if (weekBackground == null)
            {
                throw new ArgumentException("WeekNumberBackground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberHoverBorderBrushChanged != null)
            {
                this.WeekNumberHoverBorderBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="WeekNumberForeground"/> is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush type.</exception>
        protected virtual void OnWeekNumberForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekForeground = e.NewValue as Brush;

            if (weekForeground == null)
            {
                throw new ArgumentException("WeekNumberForeground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberForegroundChanged != null)
            {
                this.WeekNumberForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberSelectionForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberSelectionForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekForeground = e.NewValue as Brush;

            if (weekForeground == null)
            {
                throw new ArgumentException("WeekNumberForeground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberSelectionForegroundChanged != null)
            {
                this.WeekNumberSelectionForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WeekNumberHoverForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnWeekNumberHoverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Brush weekForeground = e.NewValue as Brush;

            if (weekForeground == null)
            {
                throw new ArgumentException("WeekNumberForeground property can be assigned only by a Brush type value.");
            }

            if (this.WeekNumberHoverForegroundChanged != null)
            {
                this.WeekNumberHoverForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionBorderCornerRadius"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnSelectionBorderCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectionBorderCornerRadiusChanged != null)
            {
                SelectionBorderCornerRadiusChanged(this, e);
            }

            CornerRadius newRadius = (CornerRadius)e.NewValue;
            CurrentDayGrid.SelectionBorder.CornerRadius = newRadius;
            FollowingDayGrid.SelectionBorder.CornerRadius = newRadius;
        }

        /// <summary>
        /// Invoked whenever the <see cref="FrameMovingTime"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnFrameMovingTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameMovingTimeChanged != null)
            {
                FrameMovingTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises the <see cref="ChangeModeTimeChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnChangeModeTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ChangeModeTimeChanged != null)
            {
                ChangeModeTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="MonthChangeDirection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnMonthChangeDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MonthChangeDirectionChanged != null)
            {
                MonthChangeDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsMonthNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnIsMonthNameAbbreviatedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMonthNameAbbreviatedChanged != null)
            {
                IsMonthNameAbbreviatedChanged(this, e);
            }

            if (m_monthButton1 != null)
            {
                m_monthButton1.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsTodayButtonClickedChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsTodayButtonClickedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsTodayButtonClickedChanged != null)
            {
                IsTodayButtonClickedChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowMultiplySelection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnAllowMultiplySelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowMultiplySelectionChanged != null)
            {
                AllowMultiplySelectionChanged(this, e);
            }

            SelectedDates.Clear();

            if (AllowSelection)
            {
                SelectedDates.Add(Date);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsDayNamesAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnIsDayNamesAbbreviatedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDayNamesAbbreviatedChanged != null)
            {
                IsDayNamesAbbreviatedChanged(this, e);
            }

            DayNamesGrid.SetDayNames(Culture.DateTimeFormat);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsDataTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayNameCellsDataTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayNameCellsDataTemplateChanged != null)
            {
                DayNameCellsDataTemplateChanged(this, e);
            }

            DataTemplate template = (DataTemplate)e.NewValue;
            DayNamesGrid.UpdateTemplateAndSelector(template, DayNameCellsDataTemplateSelector);
        }

        /// <summary>
        /// Invoked whenever the <see cref="NextScrollButtonTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnNextScrollButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (NextScrollButtonTemplateChanged != null)
            {
                NextScrollButtonTemplateChanged(this, e);
            }

            if (m_nextButton != null)
            {
                ControlTemplate template = (ControlTemplate)e.NewValue;
                m_nextButton.UpdateCellTemplate(template);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="PreviousScrollButtonTemplate"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnPreviousScrollButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PreviousScrollButtonTemplateChanged != null)
            {
                PreviousScrollButtonTemplateChanged(this, e);
            }

            if (m_prevButton != null)
            {
                ControlTemplate template = (ControlTemplate)e.NewValue;
                m_prevButton.UpdateCellTemplate(template);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsDataTemplate"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayCellsDataTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayCellsDataTemplateChanged != null)
            {
                DayCellsDataTemplateChanged(this, e);
            }

            DataTemplate template = (DataTemplate)e.NewValue;
            DataTemplateSelector selector = DayCellsDataTemplateSelector;
            DataTemplatesDictionary collection = DateDataTemplates;
            CurrentDayGrid.UpdateTemplateAndSelector(template, selector, collection);
            FollowingDayGrid.UpdateTemplateAndSelector(template, selector, collection);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsStyle"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayCellsStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayCellsStyleChanged != null)
            {
                DayCellsStyleChanged(this, e);
            }

            Style style = (Style)e.NewValue;
            CurrentDayGrid.UpdateStyles(style, DateStyles);
            FollowingDayGrid.UpdateStyles(style, DateStyles);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsStyle"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayNameCellsStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayNameCellsStyleChanged != null)
            {
                DayNameCellsStyleChanged(this, e);
            }

            Style style = (Style)e.NewValue;
            DayNamesGrid.UpdateStyles(style);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayCellsDataTemplateSelector"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayCellsDataTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayCellsDataTemplateSelectorChanged != null)
            {
                DayCellsDataTemplateSelectorChanged(this, e);
            }

            DataTemplate template = DayCellsDataTemplate;
            DataTemplateSelector selector = (DataTemplateSelector)e.NewValue;
            DataTemplatesDictionary collection = DateDataTemplates;
            CurrentDayGrid.UpdateTemplateAndSelector(template, selector, collection);
            FollowingDayGrid.UpdateTemplateAndSelector(template, selector, collection);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DayNameCellsDataTemplateSelector"/> property is
        /// changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDayNameCellsDataTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayNameCellsDataTemplateSelectorChanged != null)
            {
                DayNameCellsDataTemplateSelectorChanged(this, e);
            }

            DataTemplateSelector selector = (DataTemplateSelector)e.NewValue;
            DayNamesGrid.UpdateTemplateAndSelector(DayNameCellsDataTemplate, selector);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DateDataTemplates"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing
        /// the event data.</param>
        protected virtual void OnDateDataTemplatesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateDataTemplatesChanged != null)
            {
                DateDataTemplatesChanged(this, e);
            }

            if (e.OldValue != null)
            {
                ((DataTemplatesDictionary)e.OldValue).CollectionChanged -= new NotifyCollectionChangedEventHandler(DateDataTemplates_OnPropertyChanged);
            }

            ((DataTemplatesDictionary)e.NewValue).CollectionChanged += new NotifyCollectionChangedEventHandler(DateDataTemplates_OnPropertyChanged);

            if (IsInitializeComplete)
            {
                InitilizeDayCellTemplates(CurrentDayGrid);
                InitilizeDayCellTemplates(FollowingDayGrid);
            }
        }

        /// <summary>
        /// Coerces the <see cref="Date"/> property.
        /// </summary>
        /// <param name="date">Value that should be checked.</param>
        /// <returns>Checked value.</returns>
        protected virtual DateTime OnCoerceDate(DateTime date)
        {
            if (date > MaxDate)
            {
                return MaxDate;
            }

            if (date < MinDate)
            {
                return MinDate;
            }

            return date;
        }

        #endregion protected virtual

        #region private

        /// <summary>
        /// Invoked whenever month storyboard animation is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> containing the event
        /// data.</param>
        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            m_monthButton2.Visibility = Visibility.Hidden;
            CurrentDayGrid.Visibility = Visibility.Hidden;
            CurrentDayGrid.SelectionBorder.Visibility = Visibility.Visible;
            FollowingDayGrid.SelectionBorder.Visibility = Visibility.Visible;
            CurrentDayGrid.ClearValue(DayGrid.FocusVisualStyleProperty);
            FollowingDayGrid.ClearValue(DayGrid.FocusVisualStyleProperty);
            if (CurrentDayGrid.IsFocused || FollowingDayGrid.IsFocused)
            {
                FollowingDayGrid.Focus();
            }

            AdornerLayer adorner = AdornerLayer.GetAdornerLayer(FollowingDayGrid);

            if (adorner != null)
            {
                adorner.Update();
            }

            if (m_iscrollCounter != 0)
            {
                if (m_iscrollCounter > 0)
                {
                    BeginMoving(MoveDirection.Next, m_iscrollCounter);
                }
                else
                {
                    BeginMoving(MoveDirection.Prev, m_iscrollCounter);
                }

                m_iscrollCounter = 0;
            }

            if (m_postScrollNeed)
            {
                m_postScrollNeed = false;
                ScrollToDate();
            }
        }

        /// <summary>
        /// Updates MinDate to the new culture new date. Also updates Calendar to the new culture calendar.
        /// </summary>
        /// <param name="e">Event args.</param>
        private void UpdateMinDate(DependencyPropertyChangedEventArgs e)
        {
            CultureInfo newCulture = (CultureInfo)e.NewValue;
            CultureInfo oldCulture = (CultureInfo)e.OldValue;

            if (newCulture != null)
            {
                if (newCulture.Name == "ar-SA")
                    Calendar = oldCulture.Calendar;
                else
                    Calendar = newCulture.Calendar;

                m_oldMinDate = oldCulture.Calendar.MinSupportedDateTime;
                m_newMinDate = newCulture.Calendar.MinSupportedDateTime;
                //miDate = newCulture.Calendar.MinSupportedDateTime;
                //mxDate = newCulture.Calendar.MaxSupportedDateTime;

                if (MinDate == m_oldMinDate)
                {
                    MinDate = m_newMinDate;
                }
                if (miDate == m_oldMinDate)
                {
                    miDate = m_newMinDate;
                }
            }
        }

        /// <summary>
        /// Corrects VisibleData property to min Date if Date is min.
        /// </summary>
        /// <param name="format">The Date time format info.</param>
        private void VisibleDataToMinSupportedDate(DateTimeFormatInfo format)
        {
            DateTime minSupported = format.Calendar.MinSupportedDateTime;

            if (Date == minSupported)
            {
                VisibleData = new VisibleDate(minSupported.Year, minSupported.Month, minSupported.Day);
            }
        }

        /// <summary>
        /// Initializes popup window date values.
        /// </summary>
        private void InitializePopup()
        {
            if (m_popup != null)
            {
                m_popup.Format = Culture.DateTimeFormat;
                m_popup.CurrentDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, VisibleData.VisibleDay);
                if (MinMaxHidden)
                {
                    m_popup.MinDate = new Date(MinDate, Calendar);
                    m_popup.MaxDate = new Date(MaxDate, Calendar);
                }
                else
                {
                    m_popup.MinDate = new Date(miDate, Calendar);
                    m_popup.MaxDate = new Date(mxDate, Calendar);
                }
            }
        }

        /// <summary>
        /// Initializes visible day grid.
        /// </summary>
        private void InitVisibleDayGrid()
        {
            if (FollowingDayGrid.Visibility == Visibility.Visible)
            {
                FollowingDayGrid.Initialize(VisibleData, Culture, Calendar);
            }
            else
            {
                CurrentDayGrid.Initialize(VisibleData, Culture, Calendar);
            }
        }

        #endregion private

        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="Calendar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarProperty =
            DependencyProperty.Register("Calendar", typeof(Calendar), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCalendarChanged)));

        /// <summary>
        /// Identifies the <see cref="IsTodayButtonClicked"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayButtonClickedProperty =
            DependencyProperty.Register("IsTodayButtonClicked", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsTodayButtonClickedChanged)));

        /// <summary>
        /// Identifies the <see cref="Culture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(CalendarEdit), new FrameworkPropertyMetadata(new CultureInfo(Thread.CurrentThread.CurrentCulture.LCID), new PropertyChangedCallback(OnCultureChanged)));

        /// <summary>
        /// Identifies the <see cref="CalendarStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarStyleProperty =
            DependencyProperty.Register("CalendarStyle", typeof(CalendarStyle), typeof(CalendarEdit), new FrameworkPropertyMetadata(CalendarStyle.Standard, new PropertyChangedCallback(OnCalendarStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="Date"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarEdit), new FrameworkPropertyMetadata(DateTime.Now.Date, new PropertyChangedCallback(OnDateChanged), new CoerceValueCallback(OnCoerceDate)));

        /// <summary>
        /// Identifies the <see cref="SelectedDates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(DatesCollection), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDatesCollectionChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty BlackDatesProperty = DependencyProperty.Register("BlackDates", typeof(BlackDatesCollection), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBlackDatesCollectionChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.Register("MouseOverBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMouseOverBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMouseOverForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDayCellBorderBrushProperty =
            DependencyProperty.Register("SelectedDayCellBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedDayCellBorderBrushChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TodayCellBorderBrushProperty =
          DependencyProperty.Register("TodayCellBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTodayCellBorderBrushChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TodayCellForegroundProperty =
         DependencyProperty.Register("TodayCellForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTodayCellForegroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TodayCellBackgroundProperty =
        DependencyProperty.Register("TodayCellBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTodayCellBackgroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TodayCellSelectedBorderBrushProperty =
        DependencyProperty.Register("TodayCellSelectedBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTodayCellSelectedBorderBrushChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TodayCellSelectedBackgroundProperty =
        DependencyProperty.Register("TodayCellSelectedBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTodayCellSelectedBackgroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedDayCellHoverBackgroundProperty =
           DependencyProperty.Register("SelectedDayCellHoverBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedDayCellHoverBackgroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NotCurrentMonthForegroundProperty =
            DependencyProperty.Register("NotCurrentMonthForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnNotCurrentMonthForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDayCellBackgroundProperty =
            DependencyProperty.Register("SelectedDayCellBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedDayCellBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDayCellForegroundProperty =
            DependencyProperty.Register("SelectedDayCellForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedDayCellForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderBrushProperty =
            DependencyProperty.Register("SelectionBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectionBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectionForegroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InValidDateBorderBrushProperty =
           DependencyProperty.Register("InValidDateBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnInValidDateBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InValidDateForeGroundProperty =
            DependencyProperty.Register("InValidDateForeGround", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnInValidDatForeGroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InValidDateBackgroundProperty =
           DependencyProperty.Register("InValidDateBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnInValidDateBackgroundChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InValidDateCrossBackgroundProperty =
          DependencyProperty.Register("InValidDateCrossBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(InValidDateCrossBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberSelectionBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberSelectionBorderBrushProperty =
            DependencyProperty.Register("WeekNumberSelectionBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberSelectionBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberSelectionBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberSelectionBorderThicknessProperty =
            DependencyProperty.Register("WeekNumberSelectionBorderThickness", typeof(Thickness), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberSelectionBorderThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberSelectionBorderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberSelectionBorderCornerRadiusProperty =
            DependencyProperty.Register("WeekNumberSelectionBorderCornerRadius", typeof(CornerRadius), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberSelectionBorderCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberBackgroundProperty =
            DependencyProperty.Register("WeekNumberBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberSelectionBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberSelectionBackgroundProperty =
            DependencyProperty.Register("WeekNumberSelectionBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberSelectionBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberHoverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberHoverBackgroundProperty =
            DependencyProperty.Register("WeekNumberHoverBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberHoverBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberHoverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberHoverBorderBrushProperty =
            DependencyProperty.Register("WeekNumberHoverBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberHoverBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberHoverForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberHoverForegroundProperty =
            DependencyProperty.Register("WeekNumberHoverForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberHoverForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberSelectionForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberSelectionForegroundProperty =
            DependencyProperty.Register("WeekNumberSelectionForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberSelectionForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberBorderBrushProperty =
            DependencyProperty.Register("WeekNumberBorderBrush", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberForegroundProperty =
            DependencyProperty.Register("WeekNumberForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberCornerRadiusProperty =
            DependencyProperty.Register("WeekNumberCornerRadius", typeof(CornerRadius), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="WeekNumberBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberBorderThicknessProperty =
           DependencyProperty.Register("WeekNumberBorderThickness", typeof(Thickness), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnWeekNumberBorderThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionBorderCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderCornerRadiusProperty =
           DependencyProperty.Register("SelectionBorderCornerRadius", typeof(CornerRadius), typeof(CalendarEdit), new FrameworkPropertyMetadata(new CornerRadius(5), new PropertyChangedCallback(OnSelectionBorderCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectionProperty =
            DependencyProperty.Register("AllowSelection", typeof(bool), typeof(CalendarEdit), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowSelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowMultiplySelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMultiplySelectionProperty =
            DependencyProperty.Register("AllowMultiplySelection", typeof(bool), typeof(CalendarEdit), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowMultiplySelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="IsDayNamesAbbreviated"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDayNamesAbbreviatedProperty =
            DependencyProperty.Register("IsDayNamesAbbreviated", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsDayNamesAbbreviatedChanged)));

        /// <summary>
        /// Identifies the <see cref="IsMonthNameAbbreviated"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMonthNameAbbreviatedProperty =
            DependencyProperty.Register("IsMonthNameAbbreviated", typeof(bool), typeof(CalendarEdit), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsMonthNameAbbreviatedChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionRangeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionRangeModeProperty =
            DependencyProperty.Register("SelectionRangeMode", typeof(SelectionRangeMode), typeof(CalendarEdit), new UIPropertyMetadata(SelectionRangeMode.CurrentMonth, new PropertyChangedCallback(OnSelectionRangeModeChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameMovingTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameMovingTimeProperty =
            DependencyProperty.Register(
            "FrameMovingTime",
            typeof(int),
            typeof(CalendarEdit),
            new UIPropertyMetadata(300, new PropertyChangedCallback(OnFrameMovingTimeChanged)),
            new ValidateValueCallback(ValidateFrameMovingTime));

        /// <summary>
        /// Identifies the <see cref="ChangeModeTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeModeTimeProperty =
            DependencyProperty.Register("ChangeModeTime", typeof(int), typeof(CalendarEdit), new FrameworkPropertyMetadata(300, new PropertyChangedCallback(OnChangeModeTimeChanged)));

        /// <summary>
        /// Identifies the <see cref="MonthChangeDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthChangeDirectionProperty =
            DependencyProperty.Register("MonthChangeDirection", typeof(AnimationDirection), typeof(CalendarEdit), new UIPropertyMetadata(new PropertyChangedCallback(OnMonthChangeDirectionChanged)));

        /// <summary>
        /// Identifies the <see cref="DayNameCellsDataTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameCellsDataTemplateProperty =
            DependencyProperty.Register("DayNameCellsDataTemplate", typeof(DataTemplate), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayNameCellsDataTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="NextScrollButtonTemplateProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextScrollButtonTemplateProperty =
            DependencyProperty.Register("NextScrollButtonTemplate", typeof(ControlTemplate), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNextScrollButtonTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="PreviousScrollButtonTemplateProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousScrollButtonTemplateProperty =
            DependencyProperty.Register("PreviousScrollButtonTemplate", typeof(ControlTemplate), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPreviousScrollButtonTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="DayCellsDataTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayCellsDataTemplateProperty =
            DependencyProperty.Register("DayCellsDataTemplate", typeof(DataTemplate), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayCellsDataTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="DayCellsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayCellsStyleProperty =
            DependencyProperty.Register("DayCellsStyle", typeof(Style), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayCellsStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="DayNameCellsStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameCellsStyleProperty =
            DependencyProperty.Register("DayNameCellsStyle", typeof(Style), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayNameCellsStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="DayCellsDataTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayCellsDataTemplateSelectorProperty =
            DependencyProperty.Register("DayCellsDataTemplateSelector", typeof(DataTemplateSelector), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayCellsDataTemplateSelectorChanged)));

        /// <summary>
        /// Identifies the <see cref="DayNameCellsDataTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayNameCellsDataTemplateSelectorProperty =
            DependencyProperty.Register("DayNameCellsDataTemplateSelector", typeof(DataTemplateSelector), typeof(CalendarEdit), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDayNameCellsDataTemplateSelectorChanged)));

        /// <summary>
        /// Identifies the <see cref="DateDataTemplates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateDataTemplatesProperty =
            DependencyProperty.Register("DateDataTemplates", typeof(DataTemplatesDictionary), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDateDataTemplatesChanged)));

        /// <summary>
        /// Identifies the <see cref="DateStyles"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateStylesProperty =
            DependencyProperty.Register("DateStyles", typeof(StylesDictionary), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDateStylesChanged)));

        /// <summary>
        /// Identifies the <see cref="DisableDateSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisableDateSelectionProperty =
            DependencyProperty.Register("DisableDateSelection", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisableDateSelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="ScrollToDateEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollToDateEnabledProperty =
            DependencyProperty.Register("ScrollToDateEnabled", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnScrollToDateEnabledChanged)));

        /// <summary>
        /// Identifies the <see cref="DayNamesGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty DayNamesGridProperty =
            DependencyProperty.Register("DayNamesGrid", typeof(DayNamesGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="WeekNumbersGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty WeekNumbersGridProperty =
             DependencyProperty.Register("WeekNumbersGrid", typeof(WeekNumbersGrid), typeof(CalendarEdit));

        /// <summary>
        ///  Identifies the <see cref="CurrentDayGrid"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentDayGridProperty =
            DependencyProperty.Register("CurrentDayGrid", typeof(DayGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="FollowingDayGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty FollowingDayGridProperty =
            DependencyProperty.Register("FollowingDayGrid", typeof(DayGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="CurrentMonthGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty CurrentMonthGridProperty =
            DependencyProperty.Register("CurrentMonthGrid", typeof(MonthGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="CurrentYearGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty CurrentYearGridProperty =
            DependencyProperty.Register("CurrentYearGrid", typeof(YearGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="CurrentYearRangeGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty CurrentYearRangeGridProperty =
            DependencyProperty.Register("CurrentYearRangeGrid", typeof(YearRangeGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="FollowingMonthGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty FollowingMonthGridProperty =
            DependencyProperty.Register("FollowingMonthGrid", typeof(MonthGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="FollowingYearGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty FollowingYearGridProperty =
            DependencyProperty.Register("FollowingYearGrid", typeof(YearGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="FollowingYearRangeGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty FollowingYearRangeGridProperty =
            DependencyProperty.Register("FollowingYearRangeGrid", typeof(YearRangeGrid), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="CurrentWeekNumbersGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty CurrentWeekNumbersGridProperty =
            DependencyProperty.Register("CurrentWeekNumbersGrid", typeof(WeekNumberGridPanel), typeof(CalendarEdit));

        /// <summary>
        /// Identifies the <see cref="FollowingWeekNumbersGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty FollowingWeekNumbersGridProperty =
            DependencyProperty.Register("FollowingWeekNumbersGrid", typeof(WeekNumberGridPanel), typeof(CalendarEdit));

        /// <summary>
        ///
        /// </summary>
        protected internal static readonly DependencyProperty VisualModeProperty =
          DependencyProperty.Register("VisualMode", typeof(CalendarVisualMode), typeof(CalendarEdit), new FrameworkPropertyMetadata(CalendarVisualMode.Days, new PropertyChangedCallback(OnVisualModeChanged)));

        /// <summary>
        /// Identifies the <see cref="MinDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register("MinDate", typeof(DateTime), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMinDateChanged), new CoerceValueCallback(OnCoerceMinDateProperty)));

        /// <summary>
        /// Identifies the <see cref="MaxDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
            DependencyProperty.Register("MaxDate", typeof(DateTime), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMaxDateChanged), new CoerceValueCallback(OnCoerceMaxDateProperty)));

        /// <summary>
        /// Identifies the <see cref="VisibleData"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty VisibleDataProperty =
            DependencyProperty.Register("VisibleData", typeof(VisibleDate), typeof(CalendarEdit), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisibleDataChanged), new CoerceValueCallback(OnCoerceVisibleData)));

        /// <summary>
        /// Identifies the <see cref="HeaderForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
           DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(CalendarEdit), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="TodayDate"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey TodayDatePropertyKey =
            DependencyProperty.RegisterReadOnly("TodayDate", typeof(string), typeof(CalendarEdit), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="TodayDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TodayDateProperty = TodayDatePropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the <see cref="TodayRowIsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TodayRowIsVisibleProperty =
            DependencyProperty.Register("TodayRowIsVisible", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnTodayRowIsVisibleChanged)));

        /// <summary>
        /// Identifies the <see cref="MinMaxHidden"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinMaxHiddenProperty =
            DependencyProperty.Register("MinMaxHidden", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnMinMaxHiddenChanged)));

        /// <summary>
        /// Identifies the <see cref="IsShowWeekNumbers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowWeekNumbersProperty =
            DependencyProperty.Register("IsShowWeekNumbers", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsShowWeekNumbersChanged)));

        /// <summary>
        /// Identifies the <see cref="IsShowWeekNumbersGrid"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowWeekNumbersGridProperty =
            DependencyProperty.Register("IsShowWeekNumbersGrid", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsShowWeekNumbersGridChanged)));

        /// <summary>
        /// Identifies the <see cref="IsAllowYearSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAllowYearSelectionProperty =
            DependencyProperty.Register("IsAllowYearSelection", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAllowYearSelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowPreviousMonthDays"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPreviousMonthDaysProperty =
            DependencyProperty.Register("ShowPreviousMonthDays", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowPreviousMonthDaysChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowNextMonthDays"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowNextMonthDaysProperty =
            DependencyProperty.Register("ShowNextMonthDays", typeof(bool), typeof(CalendarEdit), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowNextMonthDaysChanged)));

        #endregion Dependency properties
    }
}