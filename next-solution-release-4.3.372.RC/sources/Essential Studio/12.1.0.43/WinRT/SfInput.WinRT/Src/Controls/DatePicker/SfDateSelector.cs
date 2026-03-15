// <copyright file="DateSelector.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a selectable object inside the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>.
    /// </summary>
    /// <remarks>
    /// DateSelector is a <see
    /// cref="N:Windows.UI.Xaml.Controls.Control">Control</see>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfDateSelector : Control,IDisposable
    {
        #region Variables

        private readonly DispatcherTimer _selectiontimer;

        private readonly DispatcherTimer _inputstringtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        private readonly DispatcherTimer _monthtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

        private readonly DispatcherTimer _datetimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

        private readonly DispatcherTimer _yeartimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

        private LoopingSelector _partMonth;

        private LoopingSelector _partDate;

        private LoopingSelector _partYear;

        private Grid _headergrid;

        private Grid _footergrid;

        private readonly List<char> _loopingSelectorOrder = new List<char>();

        internal Button PartDoneButton;

        internal Button PartCancelButton;

        private string inputstring;

        #endregion        

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        public SfDateSelector()
        {
            DefaultStyleKey = typeof(SfDateSelector);
            Loaded += DateSelector_Loaded;
            LayoutUpdated += SfDateSelector_LayoutUpdated;
            Unloaded += SfDateSelector_Unloaded;
            _selectiontimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.5)};
            _selectiontimer.Tick += selectiontimer_Tick;
            _monthtimer.Tick += TimerTick;
            _datetimer.Tick += TimerTick;
            _yeartimer.Tick += TimerTick;
            _inputstringtimer.Tick += inputstringtimer_Tick;
        }

        void SfDateSelector_LayoutUpdated(object sender, object e)
        {
            if (_partDate != null && _partMonth != null && _partDate != null)
            {
                if ((!_loopingSelectorOrder.Contains('m') && _partMonth.Visibility == Visibility.Visible) || (!_loopingSelectorOrder.Contains('d') && _partDate.Visibility == Visibility.Visible) || (!_loopingSelectorOrder.Contains('y') && _partYear.Visibility == Visibility.Visible))
                {
                    if (!_loopingSelectorOrder.Contains('m'))
                        _partMonth.Visibility = Visibility.Collapsed;
                    else
                    {
                        Grid.SetColumn(_partMonth, _loopingSelectorOrder.IndexOf('m'));
                        DisableItemsBeyondRange(_partMonth);
                    }

                    if (!_loopingSelectorOrder.Contains('d'))
                        _partDate.Visibility = Visibility.Collapsed;
                    else
                    {
                        Grid.SetColumn(_partDate, _loopingSelectorOrder.IndexOf('d'));
                        DisableItemsBeyondRange(_partDate);
                    }

                    if (!_loopingSelectorOrder.Contains('y'))
                        _partYear.Visibility = Visibility.Collapsed;
                    else
                    {
                        Grid.SetColumn(_partYear, _loopingSelectorOrder.IndexOf('y'));
                        DisableItemsBeyondRange(_partYear);
                    }
                }
                UpdateLoopingState();
            }
        }

        void SfDateSelector_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= SfDateSelector_Unloaded;
        }

        void inputstringtimer_Tick(object sender, object e)
        {
            _inputstringtimer.Stop();
            inputstring = string.Empty;
        }

        void DateSelector_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += AnimationPerFrameCallback;
            Validate();
            if (FormatString != null)
            {
                _loopingSelectorOrder.Clear();
                ResetVisibility();
                string formatString = FormatString.ToString().ToLower();
                if (formatString.Contains("d") || formatString.Contains("m") || formatString.Contains("y"))
                {
                    for (int i = 0; i < formatString.Length; i++)
                    {
                        if (formatString[i] == 'm' ||
                            formatString[i] == 'd' ||
                            formatString[i] == 'y')
                        {
                            char _char = formatString[i];
                            if (!_loopingSelectorOrder.Contains(_char))
                                _loopingSelectorOrder.Add(formatString[i]);
                        }
                    }
                    if (_partDate != null && _partMonth != null && _partDate != null)
                    {
                        if (!_loopingSelectorOrder.Contains('m'))
                            _partMonth.Visibility = Visibility.Collapsed;
                        else
                        {
                            Grid.SetColumn(_partMonth, _loopingSelectorOrder.IndexOf('m'));
                            DisableItemsBeyondRange(_partMonth);
                        }

                        if (!_loopingSelectorOrder.Contains('d'))
                            _partDate.Visibility = Visibility.Collapsed;
                        else
                        {
                            Grid.SetColumn(_partDate, _loopingSelectorOrder.IndexOf('d'));
                            DisableItemsBeyondRange(_partDate);
                        }

                        if (!_loopingSelectorOrder.Contains('y'))
                            _partYear.Visibility = Visibility.Collapsed;
                        else
                        {
                            Grid.SetColumn(_partYear, _loopingSelectorOrder.IndexOf('y'));
                            DisableItemsBeyondRange(_partYear);
                        }
                    }
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain m/d/y");
                }
            }
            UpdateItemSpacing();
            UpdateLoopingState();
        }

        private void UpdateLoopingState()
        {
            if (_partDate != null)
                _partDate.UpdateItemTemplate();
            if (_partMonth != null)
                _partMonth.UpdateItemTemplate();
            if (_partYear != null)
                _partYear.UpdateItemTemplate();
        }
        private void ExecuteTabKey()
        {
            if (_partMonth != null && _partMonth.IsExpanded)
                ExpandNext(_partMonth, _loopingSelectorOrder.IndexOf('m'));
            else if (_partDate != null && _partDate.IsExpanded)
                ExpandNext(_partDate, _loopingSelectorOrder.IndexOf('d'));
            else if (_partYear != null && _partYear.IsExpanded)
                ExpandNext(_partYear, _loopingSelectorOrder.IndexOf('y'));
            else if (ShowDoneButton && PartDoneButton.FocusState == FocusState.Keyboard)
            {
                PartDoneButton.IsTabStop = false;
                VisualStateManager.GoToState(PartDoneButton, "Normal", true);
                if (ShowCancelButton)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else
                    ExpandLoop(0);
            }
            else
            {
                if (ShowCancelButton && PartCancelButton.FocusState == FocusState.Keyboard)
                {
                    PartCancelButton.IsTabStop = false;
                    VisualStateManager.GoToState(PartCancelButton, "Normal", true);
                }
                ExpandLoop(0);
            }
        }

        private void ExecuteShiftTabKey()
        {
            if (_partMonth != null && _partMonth.IsExpanded)
                ExpandPrevious(_partMonth, _loopingSelectorOrder.IndexOf('m'));
            else if (_partDate != null && _partDate.IsExpanded)
                ExpandPrevious(_partDate, _loopingSelectorOrder.IndexOf('d'));
            else if (_partYear != null && _partYear.IsExpanded)
                ExpandPrevious(_partYear, _loopingSelectorOrder.IndexOf('y'));
            else if (ShowCancelButton && PartCancelButton.FocusState == FocusState.Keyboard)
            {
                PartCancelButton.IsTabStop = false;
                VisualStateManager.GoToState(PartCancelButton, "Normal", true);
                if (ShowDoneButton)
                {
                    PartDoneButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartDoneButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
            }
            else
            {
                if (ShowDoneButton && PartDoneButton.FocusState == FocusState.Keyboard)
                {
                    PartDoneButton.IsTabStop = false;
                    VisualStateManager.GoToState(PartDoneButton, "Normal", true);
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
                }
                else if (ShowCancelButton && PartCancelButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else if (ShowDoneButton && PartDoneButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartDoneButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartDoneButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
            }
        }

        private void ExpandPrevious(LoopingSelector selector, int index)
        {
            ExpandItemsBeforeCollapsing(selector);
            selector.IsExpanded = false;
            if (index - 1 < 0)
            {                
                if (ShowCancelButton && PartCancelButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else if (ShowDoneButton && PartDoneButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartDoneButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartDoneButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count-1);
            }
            else
                ExpandLoop(index - 1);
        }

        private void ExpandNext(LoopingSelector selector,int index)
        {
            ExpandItemsBeforeCollapsing(selector);
            selector.IsExpanded = false;
            if (index + 1 >= _loopingSelectorOrder.Count)
            {
                if (ShowDoneButton && PartDoneButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartDoneButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartDoneButton, "Focused", true);
                }
                else if (ShowCancelButton && PartCancelButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else
                    ExpandLoop(0);
            }
            else
                ExpandLoop(index + 1);
        }

        private void NavigateUp()
        {
            if (_partDate != null && _partDate.IsExpanded)
            {
                DisableItemsBeyondRange(_partDate);
                DateTime newdate = (_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddDays(-1);
                if (newdate.Month + 1 == (_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.Month)
                    newdate = ((_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddDays(-1).AddMonths(1));
                if (IsDateInRange(newdate))
                {
                    UpdateDayDataSource(new DateTimeWrapper(newdate));
                    _partMonth.DataSource.SelectedItem = _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = new DateTimeWrapper(newdate);
                }
            }
            else if (_partMonth != null && _partMonth.IsExpanded && IsDateInRange((_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(-1)))
            {
                DisableItemsBeyondRange(_partMonth);
                DateTime newdate = (_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(-1);
                if (newdate.Year + 1 == (_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.Year)
                    newdate = ((_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(-1).AddYears(1));
                if (IsDateInRange(newdate))
                {
                    UpdateMonthAndDayDataSource(new DateTimeWrapper(newdate));
                    _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partMonth.DataSource.SelectedItem;
                }
            }
            else if (_partYear != null && _partYear.IsExpanded && IsDateInRange((_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddYears(-1)))
            {
                DisableItemsBeyondRange(_partYear);
                UpdateDataSource(new DateTimeWrapper((_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddYears(-1)));
                _partMonth.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partYear.DataSource.SelectedItem;
            }
        }

        private void NavigateDown()
        {
            if (_partDate != null && _partDate.IsExpanded)
            {
                DisableItemsBeyondRange(_partDate);
                DateTime newdate = (_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddDays(1);
                if (newdate.Month - 1 == (_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.Month)
                    newdate = ((_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddDays(1).AddMonths(-1));
                if (IsDateInRange(newdate))
                {
                    UpdateDayDataSource(new DateTimeWrapper(newdate));
                    _partMonth.DataSource.SelectedItem = _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem;
                }
            }
            else if (_partMonth != null && _partMonth.IsExpanded && IsDateInRange((_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(1)))
            {
                DisableItemsBeyondRange(_partMonth);
                DateTime newdate = (_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(1);
                if (newdate.Year - 1 == (_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.Year)
                    newdate = ((_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMonths(1).AddYears(-1));
                if (IsDateInRange(newdate))
                {
                    UpdateMonthAndDayDataSource(new DateTimeWrapper(newdate));
                    _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partMonth.DataSource.SelectedItem;
                }
            }
            else if (_partYear != null && _partYear.IsExpanded && IsDateInRange((_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddYears(1)))
            {
                DisableItemsBeyondRange(_partYear);
                DateTime newdate = (_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddYears(1);
                if (IsDateInRange(newdate))
                {
                    UpdateDataSource(new DateTimeWrapper(newdate));
                    _partMonth.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partYear.DataSource.SelectedItem;
                }
            }
        }

        private void ExpandItemsBeforeCollapsing(LoopingSelector selector)
        {
            panel = GetVisualChild<Panel>(selector as DependencyObject);
            foreach (LoopingSelectorItem item in panel.Children)
                item._state = LoopingSelectorItem.State.Expanded;
        }

        void AnimationPerFrameCallback(object sender, object e)
        {
            if(_partYear!=null && _partYear.IsExpanded)
                DisableItemsBeyondRange(_partYear);
            if(_partMonth!=null && _partMonth.IsExpanded)
                DisableItemsBeyondRange(_partMonth);
            if(_partDate!=null && _partDate.IsExpanded)
                DisableItemsBeyondRange(_partDate);
        }

        #endregion       

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the style of the header of the data control field.
        /// </summary>
        /// <remarks>
        /// The HeaderStyle property governs the appearance of any text displayed in the
        /// header item
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(SfDateSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the datatemplate used to display the content of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> header.
        /// </summary>
        /// <remarks>
        /// Use the HeaderTemplate property to specify the custom content displayed for the
        /// header section of a date selector object
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.Header"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.HeaderStyle"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SfDateSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the style that apply for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/>.
        /// </summary>
        /// <remarks>
        /// Use to apply the style to the looping selector inside the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem"/>
        [ClassReference(IsReviewed = false)]
        public Style SelectorStyle
        {
            get { return (Style)GetValue(SelectorStyleProperty); }
            set { SetValue(SelectorStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorStyle.  This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty SelectorStyleProperty =
            DependencyProperty.Register("SelectorStyle", typeof(Style), typeof(SfDateSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the Minimum number of dates to be listed in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }
            set { SetValue(MinDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register("MinDate", typeof(DateTime), typeof(SfDateSelector), new PropertyMetadata(DateTime.MinValue));


        /// <summary>
        /// Gets or sets the Maximum number of dates to be listed in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
            DependencyProperty.Register("MaxDate", typeof(DateTime), typeof(SfDateSelector), new PropertyMetadata(DateTime.MaxValue));

        
        /// <summary>
        /// Gets or sets the currently selected date for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <remarks>
        /// Use to hold the currently selected date value.
        /// </remarks>
        /// <value>
        /// The default value is <see cref="P:System.DateTime.Now">DateTime.Now</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public object SelectedDateTime
        {
            get { return GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(object), typeof(SfDateSelector), new PropertyMetadata(DateTime.Now, OnSelectedDateTimeChanged));


        /// <summary>
        /// Gets or sets a value indicating whether show or hide the done button.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        /// <value>
        /// <c>true</c> the done button is show; otherwise done button not showing, <c>false</c>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.ShowCancelButton"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public bool ShowDoneButton
        {
            get { return (bool)GetValue(ShowDoneButtonProperty); }
            set { SetValue(ShowDoneButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowDoneButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDoneButtonProperty =
            DependencyProperty.Register("ShowDoneButton", typeof(bool), typeof(SfDateSelector), new PropertyMetadata(true));




        /// <summary>
        /// Gets or sets a value indicating whether show or hide the cancel button .
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        /// <value>
        /// <c>true</c> the cancel button is show; otherwise,cancel button not showing <c>false</c>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.ShowDoneButton"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public bool ShowCancelButton
        {
            get { return (bool)GetValue(ShowCancelButtonProperty); }
            set { SetValue(ShowCancelButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowCancelButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowCancelButtonProperty =
            DependencyProperty.Register("ShowCancelButton", typeof(bool), typeof(SfDateSelector), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfDateSelector), new PropertyMetadata(null,OnAccentBrushChanged));

        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        [ClassReference(IsReviewed = false)]
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(SfDateSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the template for day looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the day looping selector. The default value is null.
        /// </remarks>
        /// <value>
        /// The datatemplate of the day.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.MonthCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.YearCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate DayCellTemplate
        {
            get { return (DataTemplate)GetValue(DayCellTemplateProperty); }
            set { SetValue(DayCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayCellTemplateProperty =
            DependencyProperty.Register("DayCellTemplate", typeof(DataTemplate), typeof(SfDateSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template selector for day looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the day looping selector. The default value is null.
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.MonthCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.YearCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector DayCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DayCellTemplateSelectorProperty); }
            set { SetValue(DayCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayCellTemplateSelectorProperty =
            DependencyProperty.Register("DayCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfDateSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template for month looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the month looping selector. The default value is
        /// null.
        /// </remarks>
        /// <value>
        /// The datatemplate of the month.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.DayCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.YearCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate MonthCellTemplate
        {
            get { return (DataTemplate)GetValue(MonthCellTemplateProperty); }
            set { SetValue(MonthCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthCellTemplateProperty =
            DependencyProperty.Register("MonthCellTemplate", typeof(DataTemplate), typeof(SfDateSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the template selector for month looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the month looping selector. The default value is
        /// null.
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.DayCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.YearCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector MonthCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MonthCellTemplateSelectorProperty); }
            set { SetValue(MonthCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthcellSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthCellTemplateSelectorProperty =
            DependencyProperty.Register("MonthCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfDateSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template for year looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the year looping selector. The default value is
        /// null.
        /// </remarks>
        /// <value>
        /// The datatemplate of the year.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.MonthCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.DayCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate YearCellTemplate
        {
            get { return (DataTemplate)GetValue(YearCellTemplateProperty); }
            set { SetValue(YearCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearCellTemplateProperty =
            DependencyProperty.Register("YearCellTemplate", typeof(DataTemplate), typeof(SfDateSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template selector for year looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the year looping selector. The default value is
        /// null.
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.MonthCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.DayCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector YearCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(YearCellTemplateSelectorProperty); }
            set { SetValue(YearCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for YearCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YearCellTemplateSelectorProperty =
            DependencyProperty.Register("YearCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfDateSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the data used for the header of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.HeaderStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.HeaderTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>
        [ClassReference(IsReviewed = false)]
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(SfDateSelector), new PropertyMetadata(null));
      

        /// <summary>
        /// Getsor sets the Format of the string to be displayed in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        /// <value> The default value is m:d:y </value>
        [ClassReference(IsReviewed = false)]
        public object FormatString
        {
            get { return GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register("FormatString", typeof(object), typeof(SfDateSelector), new PropertyMetadata("m:d:y",OnFormatStringChanged));

        /// <summary>
        /// Gets or sets the SelectorItemHeight for the date selector items
        /// </summary>
        /// <value> The default value is 80 </value>
        public double SelectorItemHeight
        {
            get { return (double)GetValue(SelectorItemHeightProperty); }
            set { SetValue(SelectorItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectorItemHeightProperty =
            DependencyProperty.Register("SelectorItemHeight", typeof(double), typeof(SfDateSelector), new PropertyMetadata(80));


        /// <summary>
        /// Gets or sets the SelectorItemWidth for the date selector items
        /// </summary>
        /// <value> The default value is 80 </value>
        public double SelectorItemWidth
        {
            get { return (double)GetValue(SelectorItemWidthProperty); }
            set { SetValue(SelectorItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectorItemWidthProperty =
            DependencyProperty.Register("SelectorItemWidth", typeof(double), typeof(SfDateSelector), new PropertyMetadata(80));


        /// <summary>
        /// Gets or sets the SelectorItemSpacing for the date selector items
        /// </summary>
        /// <value> The default value is 4 </value>
        public double SelectorItemSpacing
        {
            get { return (double)GetValue(SelectorItemSpacingProperty); }
            set { SetValue(SelectorItemSpacingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorItemSpacing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectorItemSpacingProperty =
            DependencyProperty.Register("SelectorItemSpacing", typeof(double), typeof(SfDateSelector), new PropertyMetadata(4.0,new PropertyChangedCallback(OnSelectorItemSpacingChanged)));


        /// <summary>
        /// Gets or sets the SelectorItemCount for the date selector items
        /// </summary>
        /// <value> The default value is 0 </value>
        public int SelectorItemCount
        {
            get { return (int)GetValue(SelectorItemCountProperty); }
            set { SetValue(SelectorItemCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorItemCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectorItemCountProperty =
            DependencyProperty.Register("SelectorItemCount", typeof(int), typeof(SfDateSelector), new PropertyMetadata(0));

        #endregion

        #region Helper Methods
        
        private void Validate()
        {
            if (SelectedDateTime != null)
            {
                DateTime selecteddate;
                double mindiffvalue = 0, maxdiffvalue = 0;
                bool success = DateTime.TryParse(SelectedDateTime.ToString(), out selecteddate);
                if (selecteddate.Date < MinDate.Date || selecteddate.Date > MaxDate.Date)
                {
                    mindiffvalue = (MinDate.Date - selecteddate.Date).TotalDays;
                    maxdiffvalue = (selecteddate.Date - MaxDate).TotalDays;
                }
                if (mindiffvalue > maxdiffvalue)
                    SelectedDateTime = MinDate;
                else if (mindiffvalue < maxdiffvalue)
                    SelectedDateTime = MaxDate;
            }
        }

        private void SetYear(DateTime currentDate,string inputyear)
        {
            _partYear.UpdateItemTemplate();
            if (inputyear.Length < 4)
                inputyear=inputyear.PadLeft(4, '0');
            if (Convert.ToInt32(inputstring) == 0)
            {
                inputyear = "1";
                inputyear = inputyear.PadLeft(4, '0');
            }
            DateTime result=new DateTime(Convert.ToInt32(inputyear),currentDate.Month,currentDate.Day);
            result = GetValidDate(result);
            if (MaxDate.Year - MinDate.Year != 0)
            {
                UpdateDataSource(new DateTimeWrapper(result));
                SelectedDateTime = result;
                _partMonth.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partYear.DataSource.SelectedItem;
            }
        }

        private void SetMonth(DateTime currentDate, int inputMonth)
        {
            _partMonth.UpdateItemTemplate();
            if (inputMonth == 0 )
                inputMonth = 1;
            else if (inputMonth > 12)
                inputMonth = 12;
            DateTime result=new DateTime(currentDate.Year,Convert.ToInt32(inputMonth),currentDate.Day);
            result = GetValidDate(result);
            UpdateMonthAndDayDataSource(new DateTimeWrapper(result));
            SelectedDateTime = result;
            _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem = _partMonth.DataSource.SelectedItem;           
        }
        
        private void SetDate(DateTime currentDate, int inputDay)
        {
            _partDate.UpdateItemTemplate();
            if (inputDay == 0)
                inputDay = 1;
            else if (inputDay > (_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.LastDay().Day)
                inputDay = (_partDate.DataSource.SelectedItem as DateTimeWrapper).DateTime.LastDay().Day;
            DateTime result = new DateTime(currentDate.Year, currentDate.Month,inputDay);
            result = GetValidDate(result);
            UpdateDayDataSource(new DateTimeWrapper(result));
            SelectedDateTime = result;
            _partMonth.DataSource.SelectedItem = _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem;
        }

        private void ExpandLoop(int index)
        {
            if (_loopingSelectorOrder[index] == 'd')
                _partDate.IsExpanded = true;
            else if (_loopingSelectorOrder[index] == 'm')
                _partMonth.IsExpanded = true;
            else if (_loopingSelectorOrder[index] == 'y')
                _partYear.IsExpanded = true;
        }

        private DateTime GetValidDate(DateTime inputDate)
        {
            if (inputDate.Date < MinDate.Date)
                return MinDate.Date;
            else if (inputDate.Date > MaxDate.Date)
                return MaxDate.Date;
            else
                return inputDate;
        }

        private bool IsDateInRange(DateTime date)
        { 
            if(date.Date >= MinDate.Date && date.Date <= MaxDate.Date)
                return true;
            else
                return false;
        }

        private Panel panel;
        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        void DisableItemsBeyondRange(LoopingSelector selector)
        {
            panel = GetVisualChild<Panel>(selector as DependencyObject);
            foreach (LoopingSelectorItem item in panel.Children)
            {
                if (item._state == LoopingSelectorItem.State.Disabled)
                    item._state = LoopingSelectorItem.State.Normal;
            }
            if (selector.Name == "PART_Year")
            {
                foreach (LoopingSelectorItem item in panel.Children)
                {
                    if ((item.DataContext as DateTimeWrapper).DateTime.Date.Year < MinDate.Date.Year || (item.DataContext as DateTimeWrapper).DateTime.Date.Year > MaxDate.Date.Year)
                    {
                        item.SetState(LoopingSelectorItem.State.Disabled, false);
                    }
                }
            }
            else
            {
                foreach (LoopingSelectorItem item in panel.Children)
                {
                    if ((item.DataContext as DateTimeWrapper).DateTime.Date < MinDate.Date || (item.DataContext as DateTimeWrapper).DateTime.Date > MaxDate.Date)
                    {
                        item.SetState(LoopingSelectorItem.State.Disabled, false);
                    }
                }
            }
            selector.UpdateItemTemplate();
        }

        void selectiontimer_Tick(object sender, object e)
        {
            if(SelectedDateTime == null || (SelectedDateTime != null &&((DateTimeWrapper)_partDate.DataSource.SelectedItem).DateTime.Date >= MinDate.Date && ((DateTimeWrapper)_partDate.DataSource.SelectedItem).DateTime.Date <= MaxDate.Date && !(SelectedDateTime.Equals(((DateTimeWrapper)_partDate.DataSource.SelectedItem).DateTime))))
            SelectedDateTime = ((DateTimeWrapper)_partDate.DataSource.SelectedItem).DateTime;
        }

        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (sender == _partMonth)
            {
                DisableItemsBeyondRange(_partMonth);
                _monthtimer.Start();
            }

            if (sender == _partDate)
            {
                DisableItemsBeyondRange(_partDate);
                _datetimer.Start();
            }

            if (sender == _partYear)
            {
                DisableItemsBeyondRange(_partYear);
                _yeartimer.Start();
            }
        }

        void PART_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Cancel != null)
            {
                PartCancelButton.IsTabStop = false;
                Cancel(this, e);
            }
        }

        void PART_DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (Done != null)
            {
                PartDoneButton.IsTabStop = false;
                Done(this, e);
            }
        }

        internal void UpdateData()
        {
            DateTime datetime;
            if (SelectedDateTime == null || (SelectedDateTime != null && string.IsNullOrEmpty(SelectedDateTime.ToString())))
            {
                UpdateDatum(new DateTimeWrapper(DateTime.Now));
            }
            else if (DateTime.TryParse(SelectedDateTime.ToString(), out datetime))
            {
                UpdateDatum(new DateTimeWrapper(datetime));
            }
            else
            {
                throw new InvalidCastException("Input is not in DateTime format");
            }
        }

        internal void UpdateDatePart()
        {
            if (_partDate != null)
            {
                if (_partDate.DataSource == null)
                {
                    _partDate.DataSource = new DayDataSource();
                    _partDate.DataSource.SelectionChanged -= SelectionChanged;
                    _partDate.DataSource.SelectionChanged += SelectionChanged;
                }

                DateTimeWrapper wrapper;
                if (SelectedDateTime == null)
                {
                    wrapper = new DateTimeWrapper(DateTime.Now);
                    _partDate.DataSource.SelectedItem = wrapper;
                }
                else
                {
                    DateTime datetime;
                    if (DateTime.TryParse(SelectedDateTime.ToString(), out datetime))
                    {
                        wrapper = (new DateTimeWrapper(datetime));
                        _partDate.DataSource.SelectedItem = wrapper;
                    }
                }
            }
        }

        internal void CollapseAll()
        {
            if (_partMonth != null)
            {
                _partMonth.IsExpanded = false;
            }
            if (_partDate != null)
            {
                _partDate.IsExpanded = false;
            }
            if (_partYear != null)
            {
                _partYear.IsExpanded = false;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == _partMonth.DataSource)
            {
                if(_partDate != null && _partDate.DataSource != null && _partDate.DataSource.SelectedItem != _partMonth.DataSource.SelectedItem)
                {
                    if (_partYear != null && _partYear.DataSource != null && _partYear.DataSource.SelectedItem != _partMonth.DataSource.SelectedItem)
                    {
                        _partDate.DataSource.SelectedItem = _partYear.DataSource.SelectedItem = _partMonth.DataSource.SelectedItem;
                    }    
                }
            }

            if (_partYear != null && sender == _partYear.DataSource)
            {
                if (_partYear.DataSource != null && (_partDate != null && _partDate.DataSource != null && _partDate.DataSource.SelectedItem != _partYear.DataSource.SelectedItem))
                {
                    if (_partMonth != null && _partMonth.DataSource != null && _partMonth.DataSource.SelectedItem != _partYear.DataSource.SelectedItem)
                    {
                        if ((_partYear.DataSource.SelectedItem as DateTimeWrapper)!=null && (_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.Date < MinDate.Date)
                        {
                            UpdateDataSource(new DateTimeWrapper(MinDate.Date));
                        }
                        else if ((_partYear.DataSource.SelectedItem as DateTimeWrapper) != null && (_partYear.DataSource.SelectedItem as DateTimeWrapper).DateTime.Date > MaxDate.Date)
                        {
                            UpdateDataSource(new DateTimeWrapper(MaxDate.Date));
                        }
                        else
                            _partDate.DataSource.SelectedItem = _partMonth.DataSource.SelectedItem = _partYear.DataSource.SelectedItem;
                    }
                }
              
            }

            if (_partDate != null && sender == _partDate.DataSource)
            {
                if (_partDate.DataSource != null && (_partYear != null && _partYear.DataSource != null && _partYear.DataSource.SelectedItem != _partDate.DataSource.SelectedItem))
                {
                    if (_partMonth != null && _partMonth.DataSource != null && _partMonth.DataSource.SelectedItem != _partDate.DataSource.SelectedItem)
                    {
                        _partMonth.DataSource.SelectedItem = _partYear.DataSource.SelectedItem = _partDate.DataSource.SelectedItem; 
                    }
                }

            }
            _selectiontimer.Start();
        }


        private void TimerTick(object sender, object e)
        {
            if (sender == _monthtimer)
            {
                _monthtimer.Stop();
                if (_partMonth.IsExpanded && _partMonth.State == State.Expanded)
                {
                    _partMonth.IsExpanded = false;
                }
            }

            if (sender == _datetimer)
            {
                _datetimer.Stop();
                if (_partDate.IsExpanded && _partDate.State == State.Expanded)
                {
                    _partDate.IsExpanded = false;
                }
            }

            if (sender == _yeartimer)
            {
                _yeartimer.Stop();
                if (_partYear.IsExpanded && _partYear.State == State.Expanded)
                {
                    _partYear.IsExpanded = false;
                }
            }
        }

        private void SelectorManipulationStarted(object sender, RoutedEventArgs e)
        {
            if (sender == _partMonth)
            {
                _monthtimer.Stop();
            }

            if (sender == _partDate)
            {
                _datetimer.Stop();
            }

            if (sender == _partYear)
            {
                _yeartimer.Stop();
            }
        }

        private void SelectorManipulationCompleted(object sender, RoutedEventArgs e)
        {
            if (sender == _partMonth)
            {
                _monthtimer.Start();
            }

            if (sender == _partDate)
            {
                _datetimer.Start();
            }

            if (sender == _partYear)
            {
                _yeartimer.Start();
            }
        }

        private void MonthPointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partDate.IsExpanded && _partDate.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partDate);
                _partDate.IsExpanded = false;
            }
            if (_partYear.IsExpanded && _partYear.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partYear);
                _partYear.IsExpanded = false;
            }
            DisableItemsBeyondRange(_partMonth);
        }

        private void DatePointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partYear.IsExpanded && _partYear.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partYear);
                _partYear.IsExpanded = false;
            }
            if (_partMonth.IsExpanded && _partMonth.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partMonth);
                _partMonth.IsExpanded = false;
            }
            DisableItemsBeyondRange(_partDate);
        }

        private void YearPointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partDate.IsExpanded && _partDate.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partDate);
                _partDate.IsExpanded = false;
            }
            if (_partMonth.IsExpanded && _partMonth.State == State.Expanded)
            {
                ExpandItemsBeforeCollapsing(_partMonth);
                _partMonth.IsExpanded = false;
            }
            DisableItemsBeyondRange(_partYear);
        }

        private void UpdateDataSource(DateTimeWrapper data)
        {
            if (_partMonth != null)
                {
                    _partMonth.DataSource = new MonthDataSource() { SelectedItem = data };
                    _partMonth.DataSource.SelectionChanged -= SelectionChanged;
                    _partMonth.DataSource.SelectionChanged += SelectionChanged;
                }

                if (_partDate != null)
                {
                    _partDate.DataSource = new DayDataSource() { SelectedItem = data };
                    _partDate.DataSource.SelectionChanged -= SelectionChanged;
                    _partDate.DataSource.SelectionChanged += SelectionChanged;
                }

                if (_partYear != null)
                {
                    _partYear.DataSource = new YearDataSource() { SelectedItem = data };
                    _partYear.DataSource.SelectionChanged -= SelectionChanged;
                    _partYear.DataSource.SelectionChanged += SelectionChanged;
                }
        }

        private void UpdateDayDataSource(DateTimeWrapper data)
        {
            if (_partDate != null)
            {
                _partDate.DataSource = new DayDataSource() { SelectedItem = data };
                _partDate.DataSource.SelectionChanged -= SelectionChanged;
                _partDate.DataSource.SelectionChanged += SelectionChanged;
            }
        }

        private void UpdateMonthAndDayDataSource(DateTimeWrapper data)
        {
           if (_partMonth != null)
                {
                    _partMonth.DataSource = new MonthDataSource() { SelectedItem = data };
                    _partMonth.DataSource.SelectionChanged -= SelectionChanged;
                    _partMonth.DataSource.SelectionChanged += SelectionChanged;
                }

                if (_partDate != null)
                {
                    _partDate.DataSource = new DayDataSource() { SelectedItem = data };
                    _partDate.DataSource.SelectionChanged -= SelectionChanged;
                    _partDate.DataSource.SelectionChanged += SelectionChanged;
                }
        }

        private void UpdateDatum(DateTimeWrapper data)
        {
            if (_partMonth != null)
            {
                if (_partMonth.DataSource == null)
                {
                    _partMonth.DataSource = new MonthDataSource();
                    _partMonth.DataSource.SelectionChanged -= SelectionChanged;
                    _partMonth.DataSource.SelectionChanged += SelectionChanged;

                }
                _partMonth.DataSource.SelectedItem = data;
            }

            if (_partDate != null)
            {
                if (_partDate.DataSource == null)
                {
                    _partDate.DataSource = new DayDataSource();
                    _partDate.DataSource.SelectionChanged -= SelectionChanged;
                    _partDate.DataSource.SelectionChanged += SelectionChanged;
                }
                _partDate.DataSource.SelectedItem = data;
            }

            if (_partYear != null)
            {
                if (_partYear.DataSource == null)
                {
                    _partYear.DataSource = new YearDataSource();
                    _partYear.DataSource.SelectionChanged -= SelectionChanged;
                    _partYear.DataSource.SelectionChanged += SelectionChanged;
                }
                _partYear.DataSource.SelectedItem = data;
            }
        }

        #endregion        

        #region Override Methods

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partMonth = GetTemplateChild("PART_Month") as LoopingSelector;
            _partDate = GetTemplateChild("PART_Date") as LoopingSelector;
            _partYear = GetTemplateChild("PART_Year") as LoopingSelector;
            PartDoneButton = GetTemplateChild("PART_DoneButton") as Button;
            PartCancelButton = GetTemplateChild("PART_CancelButton") as Button;
            _headergrid = GetTemplateChild("HeaderGrid") as Grid;
            _footergrid = GetTemplateChild("FooterGrid") as Grid;
            
            if (PartDoneButton != null)
            {
                PartDoneButton.Click += PART_DoneButton_Click;
                PartDoneButton.GotFocus += PART_DoneButton_GotFocus;
            }

            if (PartCancelButton != null)
            {
                PartCancelButton.Click += PART_CancelButton_Click;
                PartCancelButton.GotFocus += PART_CancelButton_GotFocus;
            }

            if (_partMonth != null)
            {
                _partMonth.PointerPressed += MonthPointerPressed;
                _partMonth.PointerWheelChanged += MonthPointerPressed;
                _partMonth.ManipulationCompleted += SelectorManipulationCompleted;
                _partMonth.ManipulationStarted += SelectorManipulationStarted;
                _partMonth.PointerWheelChanged += OnPointerWheelChanged;
                _partMonth.GotFocus += PART_Month_GotFocus;
            }

            if (_partDate != null)
            {
                _partDate.PointerPressed += DatePointerPressed;
                _partDate.PointerWheelChanged += DatePointerPressed;
                _partDate.ManipulationCompleted += SelectorManipulationCompleted;
                _partDate.ManipulationStarted += SelectorManipulationStarted;
                _partDate.PointerWheelChanged += OnPointerWheelChanged;
                _partDate.GotFocus += PART_Date_GotFocus;
            }

            if (_partYear != null)
            {
                _partYear.PointerPressed += YearPointerPressed;
                _partYear.PointerWheelChanged += YearPointerPressed;
                _partYear.ManipulationCompleted += SelectorManipulationCompleted;
                _partYear.ManipulationStarted += SelectorManipulationStarted;
                _partYear.PointerWheelChanged += OnPointerWheelChanged;
                _partYear.GotFocus += PART_Year_GotFocus;
            }

            UpdateData();
        }

        /// <summary>
        /// Handles keydown event <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            bool shiftKey = false;
            var coreWindow = CoreWindow.GetForCurrentThread() as CoreWindow;
            var downState = CoreVirtualKeyStates.Down;
            shiftKey = (coreWindow.GetKeyState(Windows.System.VirtualKey.Shift) & downState) == downState;
            if (shiftKey && e.Key == Windows.System.VirtualKey.Tab)
                ExecuteShiftTabKey();
            else if (e.Key == Windows.System.VirtualKey.Tab)
                ExecuteTabKey();
            else if (e.Key == Windows.System.VirtualKey.Up)
                NavigateUp();
            else if (e.Key == Windows.System.VirtualKey.Down)
                NavigateDown();
            else if ((e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
                || (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9))
            {
                _inputstringtimer.Start();
                if (!string.IsNullOrEmpty(inputstring))
                {
                    if ((_partYear != null && _partYear.IsExpanded && inputstring.Length == 4))
                        inputstring = string.Empty;
                    else if (((_partMonth != null && _partMonth.IsExpanded) || (_partDate != null && _partDate.IsExpanded)) && inputstring.Length == 2)
                        inputstring = string.Empty;
                }
                if (e.Key.ToString().Contains("NumberPad"))
                    inputstring += e.Key.ToString().Replace("NumberPad", "");
                else if (e.Key.ToString().Contains("Number"))
                    inputstring += e.Key.ToString().Replace("Number", "");
                if (_partDate != null && _partDate.IsExpanded)
                    SetDate(((_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime), Convert.ToInt32(inputstring));
                else if (_partMonth != null && _partMonth.IsExpanded)
                    SetMonth(((_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime), Convert.ToInt32(inputstring));
                else if (_partYear != null && _partYear.IsExpanded)
                    SetYear(((_partMonth.DataSource.SelectedItem as DateTimeWrapper).DateTime), inputstring);
            }
            base.OnKeyDown(e);
        }

        void PART_CancelButton_GotFocus(object sender, RoutedEventArgs e)
        {
            _partDate.IsExpanded = _partMonth.IsExpanded = _partYear.IsExpanded = false;
        }

        void PART_DoneButton_GotFocus(object sender, RoutedEventArgs e)
        {
            _partDate.IsExpanded = _partMonth.IsExpanded = _partYear.IsExpanded = false;
        }


        void PART_Year_GotFocus(object sender, RoutedEventArgs e)
        {
            _partMonth.IsExpanded = _partDate.IsExpanded = false;
            _partYear.IsExpanded = true;
        }

        void PART_Date_GotFocus(object sender, RoutedEventArgs e)
        {
            _partMonth.IsExpanded = _partYear.IsExpanded = false;
            _partDate.IsExpanded = true;
        }

        void PART_Month_GotFocus(object sender, RoutedEventArgs e)
        {
            _partYear.IsExpanded = _partDate.IsExpanded = false;
            _partMonth.IsExpanded = true;
        }

        #endregion        

        #region Callback Methods

        private static void OnFormatStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfDateSelector;
            if (selector != null)
            {
                selector.OnFormatStringChanged(args);
            }
        }

        private static void OnAccentBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfDateSelector;
            if (selector != null && selector._partDate != null && selector._partMonth != null && selector._partYear != null)
            {
                LoopingSelectorItem loopingSelectorItem_Date = selector.GetVisualTreeItem(selector._partDate as DependencyObject) as LoopingSelectorItem;
                if (loopingSelectorItem_Date != null) loopingSelectorItem_Date.AccentBrush = selector.AccentBrush;

                LoopingSelectorItem loopingSelectorItem_Month = selector.GetVisualTreeItem(selector._partMonth as DependencyObject) as LoopingSelectorItem;
                if (loopingSelectorItem_Month != null) loopingSelectorItem_Month.AccentBrush = selector.AccentBrush;

                LoopingSelectorItem loopingSelectorItem_Year = selector.GetVisualTreeItem(selector._partYear as DependencyObject) as LoopingSelectorItem;
                if (loopingSelectorItem_Year != null) loopingSelectorItem_Year.AccentBrush = selector.AccentBrush;
                selector.UpdateLayout();
            }
        }

        private object GetVisualTreeItem(DependencyObject obj)
        {
            var item = obj;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(item); i++)
            {
                while (VisualTreeHelper.GetChild(item,i) != null && !(VisualTreeHelper.GetChild(item,i) is LoopingSelectorItem))
                {
                    item = VisualTreeHelper.GetChild(item,i);
                }
                if (VisualTreeHelper.GetChild(item,i) is LoopingSelectorItem)
                    return VisualTreeHelper.GetChild(item,i);
            }
            return item;
        }

        private void ResetVisibility()
        {
            if (_partDate != null)
                _partDate.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (_partMonth != null)
                _partMonth.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (_partYear != null)
                _partYear.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Occurs when the data used as FormatString has changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                string formatString = args.NewValue.ToString().ToLower();
                _loopingSelectorOrder.Clear();
                ResetVisibility();
                if (formatString.Contains("d") || formatString.Contains("m") || formatString.Contains("y"))
                {
                    for (int i = 0; i < formatString.Length; i++)
                    {
                        if (formatString[i] == 'm' ||
                            formatString[i] == 'd' ||
                            formatString[i] == 'y')
                        {
                            char _char = formatString[i];
                            if (!_loopingSelectorOrder.Contains(_char))
                                _loopingSelectorOrder.Add(formatString[i]);
                        }
                    }
                    if (_partDate != null && _partMonth != null && _partDate != null)
                    {
                        if (!_loopingSelectorOrder.Contains('m'))
                            _partMonth.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partMonth, _loopingSelectorOrder.IndexOf('m'));

                        if (!_loopingSelectorOrder.Contains('d'))
                            _partDate.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partDate, _loopingSelectorOrder.IndexOf('d'));

                        if (!_loopingSelectorOrder.Contains('y'))
                            _partYear.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partYear, _loopingSelectorOrder.IndexOf('y'));
                    }
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain m/d/y");
                }
            }
        }
        private static void OnSelectedDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfDateSelector;
            if (selector != null)
            {
                selector.Validate();
                selector.UpdateDatePart();
                if (selector.SelectedDateTimeChanged != null)
                {
                    selector.SelectedDateTimeChanged(sender, args);
                }

            }
        }

        private static void OnSelectorItemSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfDateSelector;
            selector.UpdateItemSpacing();
        }

        internal double UpdateSelectorHeight()
        {
            double selectorheight = 0;
            double itemmarginheight = 0;

            if (SelectorItemCount != 0)
            {
                foreach (var item in _loopingSelectorOrder)
                {
                    switch (item)
                    {
                        case 'm':
                            if (_partMonth != null)
                            {
                                var monthmarginheight = _partMonth.Margin.Top + _partMonth.Margin.Bottom;
                                itemmarginheight = itemmarginheight < monthmarginheight ? monthmarginheight : itemmarginheight;
                            }
                            break;
                        case 'd':
                            if (_partDate != null)
                            {
                                var datemarginheight = _partDate.Margin.Top + _partDate.Margin.Bottom;
                                itemmarginheight = itemmarginheight < datemarginheight ? datemarginheight : itemmarginheight;
                            }
                            break;
                        case 'y':
                            if (_partYear != null)
                            {
                                var yearmarginheight = _partYear.Margin.Top + _partYear.Margin.Bottom;
                                itemmarginheight = itemmarginheight < yearmarginheight ? yearmarginheight : itemmarginheight;
                            }
                            break;
                    }
                }
                for (int i = 0; i <= SelectorItemCount; i++)
                {
                    selectorheight = selectorheight + SelectorItemHeight + itemmarginheight;
                }
                if (_headergrid != null && _footergrid != null)
                    selectorheight = selectorheight + _headergrid.ActualHeight + _footergrid.ActualHeight;
            }
            return selectorheight;
        }

        private void UpdateItemSpacing()
        {
            for (int i = 0; i < _loopingSelectorOrder.Count; i++)
            {
                var item = _loopingSelectorOrder[i];
                switch (item)
                {
                    case 'm':
                        if (_partMonth != null)
                        {
                            _partMonth.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) : 
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partMonth.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                    case 'd':
                        if (_partDate != null)
                        {
                            _partDate.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partDate.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                    case 'y':
                        if (_partYear != null)
                        {
                            _partYear.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partYear.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when cancel button is clicked.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Cancel;

        /// <summary>
        /// Occurs when done is clicked.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Done;

        /// <summary>
        /// Occurs when selected date changed.
        /// </summary>
        /// <remarks>
        /// raised when selected date property is changed.
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DateSelector.SelectedDateTime"/>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SelectedDateTimeChanged;

        #endregion


        public void Dispose()
        {
            Loaded -= DateSelector_Loaded;
            LayoutUpdated -= SfDateSelector_LayoutUpdated;
            if(_selectiontimer!=null)
                _selectiontimer.Tick -= selectiontimer_Tick;
            if(_monthtimer !=null)
                _monthtimer.Tick -= TimerTick;
            if(_datetimer!=null)
                _datetimer.Tick -= TimerTick;
            if(_yeartimer!=null)
                _yeartimer.Tick -= TimerTick;
            if(_inputstringtimer!=null)
                _inputstringtimer.Tick -= inputstringtimer_Tick;

            CompositionTarget.Rendering -= AnimationPerFrameCallback;

            if (PartDoneButton != null)
            {
                PartDoneButton.Click -= PART_DoneButton_Click;
                PartDoneButton.GotFocus -= PART_DoneButton_GotFocus;
            }

            if (PartCancelButton != null)
            {
                PartCancelButton.Click -= PART_CancelButton_Click;
                PartCancelButton.GotFocus -= PART_CancelButton_GotFocus;
            }

            if (_partMonth != null)
            {
                _partMonth.PointerPressed -= MonthPointerPressed;
                _partMonth.PointerWheelChanged -= MonthPointerPressed;
                _partMonth.ManipulationCompleted -= SelectorManipulationCompleted;
                _partMonth.ManipulationStarted -= SelectorManipulationStarted;
                _partMonth.PointerWheelChanged -= OnPointerWheelChanged;
                _partMonth.GotFocus -= PART_Month_GotFocus;
                _partMonth.DataSource.SelectionChanged -= SelectionChanged;
                _partMonth = null;
            }

            if (_partDate != null)
            {
                _partDate.PointerPressed -= DatePointerPressed;
                _partDate.PointerWheelChanged -= DatePointerPressed;
                _partDate.ManipulationCompleted -= SelectorManipulationCompleted;
                _partDate.ManipulationStarted -= SelectorManipulationStarted;
                _partDate.PointerWheelChanged -= OnPointerWheelChanged;
                _partDate.GotFocus -= PART_Date_GotFocus;
                _partDate.DataSource.SelectionChanged -= SelectionChanged;
                _partDate = null;
            }

            if (_partYear != null)
            {
                _partYear.PointerPressed -= YearPointerPressed;
                _partYear.PointerWheelChanged -= YearPointerPressed;
                _partYear.ManipulationCompleted -= SelectorManipulationCompleted;
                _partYear.ManipulationStarted -= SelectorManipulationStarted;
                _partYear.PointerWheelChanged -= OnPointerWheelChanged;
                _partYear.GotFocus += PART_Year_GotFocus;
                _partYear.DataSource.SelectionChanged -= SelectionChanged;
                _partYear = null;
            }    

        }
    }
   
}
