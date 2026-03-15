// <copyright file="TimeSelector.cs" company="Syncfusion">
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
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>.
    /// </summary>
    /// <remarks>
    /// TimeSelector is a <see
    /// cref="N:Windows.UI.Xaml.Controls.Control">Control</see>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfTimeSelector : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        public SfTimeSelector()
        {
            DefaultStyleKey = typeof(SfTimeSelector);
            _selectiontimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(0.5)};
            _selectiontimer.Tick += selectiontimer_Tick;
            _hourtimer.Tick += TimerTick;
            _minutetimer.Tick += TimerTick;
            _secondtimer.Tick += TimerTick;
            _meridiemtimer.Tick += TimerTick;
            Loaded += TimeSelector_Loaded;
            LayoutUpdated += SfTimeSelector_LayoutUpdated;
        }

        void SfTimeSelector_LayoutUpdated(object sender, object e)
        {
            if (_partHour != null && _partMinute != null && _partMeridiem != null)
            {
                if ((!_loopingSelectorOrder.Contains('h') && _partHour.Visibility == Visibility.Visible) || (!_loopingSelectorOrder.Contains('m') && _partMinute.Visibility == Visibility.Visible) || (!_loopingSelectorOrder.Contains('s') && _partsecond.Visibility == Visibility.Visible) || (!_loopingSelectorOrder.Contains('t') && _partMeridiem.Visibility == Visibility.Visible))
                {
                    if (!_loopingSelectorOrder.Contains('h'))
                        _partHour.Visibility = Visibility.Collapsed;
                    else
                        Grid.SetColumn(_partHour, _loopingSelectorOrder.IndexOf('h'));

                    if (!_loopingSelectorOrder.Contains('m'))
                        _partMinute.Visibility = Visibility.Collapsed;
                    else
                        Grid.SetColumn(_partMinute, _loopingSelectorOrder.IndexOf('m'));

                    if (!_loopingSelectorOrder.Contains('s'))
                        _partsecond.Visibility = Visibility.Collapsed;
                    else
                        Grid.SetColumn(_partsecond, _loopingSelectorOrder.IndexOf('s'));

                    if (!_loopingSelectorOrder.Contains('t') || DateTimeWrapper.IsTwentyFourHourtimeline)
                        _partMeridiem.Visibility = Visibility.Collapsed;
                    else
                        Grid.SetColumn(_partMeridiem, _loopingSelectorOrder.IndexOf('t'));
                }
                UpdateLoopingState();
            }
        }

        void inputstringtimer_Tick(object sender, object e)
        {
            _inputstringtimer.Stop();
            inputstring = string.Empty;
        }

        void TimeSelector_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormatString != null)
            {
                string formatString = FormatString.ToString().ToLower();
                _loopingSelectorOrder.Clear();
                ResetVisibility();
                if (formatString.Contains("h") || formatString.Contains("m") || formatString.Contains("t")||formatString.Contains("s"))
                {
                    for (int i = 0; i < formatString.Length; i++)
                    {
                        if (formatString[i] == 'h' ||
                            formatString[i] == 'm' ||
                            formatString[i] == 't' ||
                            formatString[i] == 's')
                        {
                            char _char = formatString[i];
                            if (!_loopingSelectorOrder.Contains(_char))
                                _loopingSelectorOrder.Add(formatString[i]);
                        }
                    }
                    if (_partHour != null && _partMinute != null && _partMeridiem != null)
                    {
                        if (!_loopingSelectorOrder.Contains('h'))
                            _partHour.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partHour, _loopingSelectorOrder.IndexOf('h'));

                        if (!_loopingSelectorOrder.Contains('m'))
                            _partMinute.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partMinute, _loopingSelectorOrder.IndexOf('m'));
                        
                        if (!_loopingSelectorOrder.Contains('s'))
                            _partsecond.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partsecond, _loopingSelectorOrder.IndexOf('s'));

                        if (!_loopingSelectorOrder.Contains('t')|| DateTimeWrapper.IsTwentyFourHourtimeline)
                            _partMeridiem.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partMeridiem, _loopingSelectorOrder.IndexOf('t'));
                    }
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain h/m/s/t");
                }
            }
            UpdateItemSpacing();
            UpdateLoopingState();
        }

        private void UpdateLoopingState()
        {
            if (_partHour != null)
                _partHour.UpdateItemTemplate();
            if (_partMeridiem != null)
                _partMeridiem.UpdateItemTemplate();
            if (_partMinute != null)
                _partMinute.UpdateItemTemplate();
            if (_partsecond != null)
                _partsecond.UpdateItemTemplate();
        }

        #endregion

        #region Variables

        private readonly List<char> _loopingSelectorOrder = new List<char>();

        private readonly DispatcherTimer _selectiontimer;

        private readonly DispatcherTimer _inputstringtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };

        private readonly DispatcherTimer _hourtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) }; 

        private readonly DispatcherTimer _minutetimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) }; 

        private readonly DispatcherTimer _meridiemtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

        private readonly DispatcherTimer _secondtimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) }; 

        private LoopingSelector _partHour; 

        private LoopingSelector _partMinute; 

        private LoopingSelector _partMeridiem;

        private LoopingSelector _partsecond;


        private Grid _headergrid;

        private Grid _footergrid;


        internal Button PartSelectButton;

        internal Button PartCancelButton;

        private string inputstring;

        #endregion              

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the FormatString for the control <see
        /// cref="N:Windows.UI.Xaml.Controls.Control"/>
        /// </summary>
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
            DependencyProperty.Register("FormatString", typeof(object), typeof(SfTimeSelector), new PropertyMetadata("h:m:t", OnFormatStringChanged));


        /// <summary>
        /// Gets or sets the currently selected time for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </summary>
        /// <remarks>
        /// Use to hold the currently selected time value.
        /// </remarks>
        /// <value>
        /// The default value is <see cref="P:System.DateTime.Now">DateTime.Now</see>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public object SelectedTime
        {
            get { return GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(object), typeof(SfTimeSelector), new PropertyMetadata(DateTime.Now, OnSelectedDateTimeChanged));



        /// <summary>
        /// Gets or sets the style of the header of the time control field.
        /// </summary>
        /// <remarks>
        /// The HeaderStyle property governs the appearance of any text displayed in the
        /// header item
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
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
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(SfTimeSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the datatemplate used to display the content of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/> header.
        /// </summary>
        /// <remarks>
        /// Use the HeaderTemplate property to specify the custom content displayed for the
        /// header section of a time selector object
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.Header"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HeaderStyle"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SfTimeSelector), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the style that apply for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/>.
        /// </summary>
        /// <remarks>
        /// Use to apply the style to the looping selector inside the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
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
            DependencyProperty.Register("SelectorStyle", typeof(Style), typeof(SfTimeSelector), new PropertyMetadata(null));

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
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.ShowCancelButton"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public bool ShowDoneButton
        {
            get { return (bool)GetValue(ShowDoneButtonProperty); }
            set { SetValue(ShowDoneButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowSelectButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDoneButtonProperty =
            DependencyProperty.Register("ShowDoneButton", typeof(bool), typeof(SfTimeSelector), new PropertyMetadata(true));



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
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.ShowDoneButton"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
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
            DependencyProperty.Register("ShowCancelButton", typeof(bool), typeof(SfTimeSelector), new PropertyMetadata(true));



        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfTimeSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
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
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(SfTimeSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the template for minute looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the Minute looping selector.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HourCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.MeridiemCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate MinuteCellTemplate
        {
            get { return (DataTemplate)GetValue(MinuteCellTemplateProperty); }
            set { SetValue(MinuteCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteCellTemplateProperty =
            DependencyProperty.Register("MinuteCellTemplate", typeof(DataTemplate), typeof(SfTimeSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for second looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the Second looping selector.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HourCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.MeridiemCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate SecondCellTemplate
        {
            get { return (DataTemplate)GetValue(SecondCellTemplateProperty); }
            set { SetValue(SecondCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondCellTemplateProperty =
            DependencyProperty.Register("SecondCellTemplate", typeof(DataTemplate), typeof(SfTimeSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the templateselector for second looping selector.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public DataTemplateSelector SecondCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SecondCellTemplateSelectorProperty); }
            set { SetValue(SecondCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondCellTemplateSelectorProperty =
            DependencyProperty.Register("SecondCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfTimeSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the templateselector for minute looping selector.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public DataTemplateSelector MinuteCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MinuteCellTemplateSelectorProperty); }
            set { SetValue(MinuteCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteCellTemplateSelectorProperty =
            DependencyProperty.Register("MinuteCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfTimeSelector), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the template for hour looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the hour looping selector.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.DayCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.MeridiemCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate HourCellTemplate
        {
            get { return (DataTemplate)GetValue(HourCellTemplateProperty); }
            set { SetValue(HourCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HourCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourCellTemplateProperty =
            DependencyProperty.Register("HourCellTemplate", typeof(DataTemplate), typeof(SfTimeSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the templateselector for hour looping selector.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public DataTemplateSelector HourCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HourCellTemplateSelectorProperty); }
            set { SetValue(HourCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HourCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourCellTemplateSelectorProperty =
            DependencyProperty.Register("HourCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfTimeSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the template for day looping selector.
        /// </summary>
        /// <remarks>
        /// Use as the datatemplate for the day looping selector.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HourCellTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.DayCellTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate MeridiemCellTemplate
        {
            get { return (DataTemplate)GetValue(MeridiemCellTemplateProperty); }
            set { SetValue(MeridiemCellTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MeridiemCellTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MeridiemCellTemplateProperty =
            DependencyProperty.Register("MeridiemCellTemplate", typeof(DataTemplate), typeof(SfTimeSelector), new PropertyMetadata(null));


        
        /// <summary>
        /// Gets or sets the templateselector for meridiem looping selector.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public DataTemplateSelector MeridiemCellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MeridiemCellTemplateSelectorProperty); }
            set { SetValue(MeridiemCellTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MeridiemCellTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MeridiemCellTemplateSelectorProperty =
            DependencyProperty.Register("MeridiemCellTemplateSelector", typeof(DataTemplateSelector), typeof(SfTimeSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the data used for the header of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HeaderStyle"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.HeaderTemplate"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>
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
            DependencyProperty.Register("Header", typeof(object), typeof(SfTimeSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the SelectorItemHeight for the time selector items
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
            DependencyProperty.Register("SelectorItemHeight", typeof(double), typeof(SfTimeSelector), new PropertyMetadata(80));


        /// <summary>
        /// Gets or sets the SelectorItemWidth for the time selector items
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
            DependencyProperty.Register("SelectorItemWidth", typeof(double), typeof(SfTimeSelector), new PropertyMetadata(80));


        /// <summary>
        /// Gets or sets the SelectorItemSpacing for the time selector items
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
            DependencyProperty.Register("SelectorItemSpacing", typeof(double), typeof(SfTimeSelector), new PropertyMetadata(4.0, new PropertyChangedCallback(OnSelectorItemSpacingChanged)));


        /// <summary>
        /// Gets or sets the SelectorItemCount for the time selector items
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
            DependencyProperty.Register("SelectorItemCount", typeof(int), typeof(SfTimeSelector), new PropertyMetadata(0));


        #endregion

        #region Helper Methods

        void selectiontimer_Tick(object sender, object e)
        {
            SelectedTime = ((DateTimeWrapper)_partMinute.DataSource.SelectedItem).DateTime;
        }

        void OnPointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender == _partHour)
            {
                _hourtimer.Start();
            }

            if (sender == _partMinute)
            {
                _minutetimer.Start();
            }

            if (sender == _partsecond)
            {
                _secondtimer.Start();
            }

            if (sender == _partMeridiem)
            {
                _meridiemtimer.Start();
            }
        }

        internal void UpdateData()
        {
            DateTime datetime;

            if (FormatString!=null && (FormatString.ToString().Contains("HH") || FormatString.ToString().Contains("H")))
                DateTimeWrapper.IsTwentyFourHourtimeline = true;
            else
                DateTimeWrapper.IsTwentyFourHourtimeline = false;
            if (SelectedTime == null || string.IsNullOrEmpty(SelectedTime.ToString()))
            {
                UpdateDatum(new DateTimeWrapper(DateTime.Now));
            }
            else if (DateTime.TryParse(SelectedTime.ToString(), out datetime))
            {
                UpdateDatum(new DateTimeWrapper(datetime));
            }
            else
            {
                throw new InvalidCastException("Input is not in DateTime format");
            }
        }

        private void PART_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
        }

        internal void CollapseAll()
        {
            if (_partHour != null)
            {
                _partHour.IsExpanded = false;
            }
            if (_partMinute != null)
            {
                _partMinute.IsExpanded = false;
            }

            if (_partsecond != null)
            {
               _partsecond.IsExpanded = false;
            }

            if (_partMeridiem != null)
            {
                _partMeridiem.IsExpanded = false;
            }
        }

        private void PART_SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Select != null)
            {
                Select(this, e);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == _partHour.DataSource)
            {
                if (_partMinute != null && _partMinute.DataSource != null && _partMinute.DataSource.SelectedItem != _partHour.DataSource.SelectedItem)
                {
                    _partMinute.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;
                }
                if (_partMeridiem != null && _partMeridiem.DataSource != null && _partMeridiem.DataSource.SelectedItem != _partHour.DataSource.SelectedItem)
                {
                    _partMeridiem.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;
                }
                if (_partsecond != null && (_partsecond != null && _partsecond.DataSource != null && _partHour.DataSource.SelectedItem != _partsecond.DataSource.SelectedItem))
                {
                    _partsecond.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;
                }

            }
            if (_partMinute != null && sender == _partMinute.DataSource)
            {
                if (_partMinute.DataSource != null && (_partHour != null && _partHour.DataSource != null && _partMinute.DataSource.SelectedItem != _partHour.DataSource.SelectedItem))
                {
                    _partHour.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
                }
                if (_partMinute.DataSource != null && (_partMeridiem != null && _partMeridiem.DataSource != null && _partMinute.DataSource.SelectedItem != _partMeridiem.DataSource.SelectedItem))
                {
                    _partMeridiem.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
                }
                if (_partMinute.DataSource != null && (_partsecond != null && _partsecond.DataSource != null && _partMinute.DataSource.SelectedItem != _partsecond.DataSource.SelectedItem))
                {
                    _partsecond.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
                }
            }

            if (_partsecond != null && sender == _partsecond.DataSource)
            {
                if (_partsecond.DataSource != null && (_partHour != null && _partHour.DataSource != null && _partMinute.DataSource.SelectedItem != _partsecond.DataSource.SelectedItem))
                {
                    _partHour.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;
                }
                if (_partsecond.DataSource != null && (_partMeridiem != null && _partMeridiem.DataSource != null && _partMeridiem.DataSource.SelectedItem != _partsecond.DataSource.SelectedItem))
                {
                    _partMeridiem.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;
                }
                if (_partsecond.DataSource != null && (_partMinute != null && _partMinute.DataSource != null && _partMinute.DataSource.SelectedItem != _partsecond.DataSource.SelectedItem))
                {
                    _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;
                }
            }

            if (_partMeridiem != null && sender == _partMeridiem.DataSource)
            {
                if (_partMeridiem.DataSource != null && (_partMinute != null && _partMinute.DataSource != null && _partMinute.DataSource.SelectedItem != _partMeridiem.DataSource.SelectedItem))
                {
                    _partMinute.DataSource.SelectedItem = _partMeridiem.DataSource.SelectedItem;
                }
                if (_partMeridiem.DataSource != null && (_partHour != null && _partHour.DataSource != null && _partHour.DataSource.SelectedItem != _partMeridiem.DataSource.SelectedItem))
                {
                    _partHour.DataSource.SelectedItem = _partMeridiem.DataSource.SelectedItem;
                }
            }
            _selectiontimer.Start();
        }


        private void TimerTick(object sender, object e)
        {
            if (sender == _hourtimer)
            {
                _hourtimer.Stop();
                if (_partHour.IsExpanded && _partHour.State == State.Expanded)
                {
                    _partHour.IsExpanded = false;
                }
            }

            if (sender == _minutetimer)
            {
                _minutetimer.Stop();
                if (_partMinute.IsExpanded && _partMinute.State == State.Expanded)
                {
                    _partMinute.IsExpanded = false;
                }
            }

            if (sender == _secondtimer)
            {
                _secondtimer.Stop();
                if (_partsecond.IsExpanded && _partsecond.State == State.Expanded)
                {
                    _partsecond.IsExpanded = false;
                }
            }

            if (sender == _meridiemtimer)
            {
                _meridiemtimer.Stop();
                if (_partMeridiem.IsExpanded && _partMeridiem.State == State.Expanded)
                {
                    _partMeridiem.IsExpanded = false;
                }
            }
        }

        private void SelectorManipulationStarted(object sender, RoutedEventArgs e)
        {
            if (sender == _partHour)
            {
                _hourtimer.Stop();
            }

            if (sender == _partMinute)
            {
                _minutetimer.Stop();
            }

            if (sender == _partsecond)
            {
             _secondtimer.Stop();
            }

            if (sender == _partMeridiem)
            {
                _meridiemtimer.Stop();
            }
        }

        private void SelectorManipulationCompleted(object sender, RoutedEventArgs e)
        {
            if (sender == _partHour)
            {
                _hourtimer.Start();
            }

            if (sender == _partMinute)
            {
                _minutetimer.Start();
            }

            if (sender == _partsecond)
            {
                _secondtimer.Start();
            }

            if (sender == _partMeridiem)
            {
                _meridiemtimer.Start();
            }
        }

        private void HourPointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partMinute.IsExpanded && _partMinute.State == State.Expanded)
            {
                _partMinute.IsExpanded = false;
            }
            if (_partMeridiem.IsExpanded && _partMeridiem.State == State.Expanded)
            {
                _partMeridiem.IsExpanded = false;
            }
            if (_partsecond.IsExpanded && _partsecond.State == State.Expanded)
            {
                _partsecond.IsExpanded = false;
            }

        }

        private void MinutePointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partMeridiem.IsExpanded && _partMeridiem.State == State.Expanded)
            {
                _partMeridiem.IsExpanded = false;
            }
            if (_partHour.IsExpanded && _partHour.State == State.Expanded)
            {
                _partHour.IsExpanded = false;
            }
            if (_partsecond.IsExpanded && _partsecond.State == State.Expanded)
            {
                _partsecond.IsExpanded = false;
            }
        }

        private void MeridiemPointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partMinute.IsExpanded && _partMinute.State == State.Expanded)
            {
                _partMinute.IsExpanded = false;
            }
            if (_partsecond.IsExpanded && _partsecond.State == State.Expanded)
            {
                _partsecond.IsExpanded = false;
            }
            if (_partHour.IsExpanded && _partHour.State == State.Expanded)
            {
                _partHour.IsExpanded = false;
            }
        }

        private void SecondPointerPressed(object sender, RoutedEventArgs e)
        {
            if (_partMinute.IsExpanded && _partMinute.State == State.Expanded)
            {
                _partMinute.IsExpanded = false;
            }
            if (_partMeridiem.IsExpanded && _partMeridiem.State == State.Expanded)
            {
                _partMeridiem.IsExpanded = false;
            }
            if (_partHour.IsExpanded && _partHour.State == State.Expanded)
            {
                _partHour.IsExpanded = false;
            }
        }

        private void UpdateDatum(DateTimeWrapper data)
        {
            if (_partHour != null)
            {
                if (_partHour.DataSource == null)
                {
                    if (FormatString!=null && (FormatString.ToString().Contains("HH") || FormatString.ToString().Contains("H")))
                        DateTimeWrapper.IsTwentyFourHourtimeline = true;
                    else
                        DateTimeWrapper.IsTwentyFourHourtimeline = false;
                    if (FormatString != null && (FormatString.ToString().Contains("HH") || FormatString.ToString().Contains("H")))
                        _partHour.DataSource = new TwentyFourHourDataSource() {SelectedItem = data };
                    else
                        _partHour.DataSource = new TwelveHourDataSource() { SelectedItem = data};
                    _partHour.DataSource.SelectionChanged += SelectionChanged;

                }
                _partHour.DataSource.SelectedItem = data;
            }

            if (_partMinute != null)
            {
                if (_partMinute.DataSource == null)
                {
                    _partMinute.DataSource = new MinuteDataSource();
                    _partMinute.DataSource.SelectionChanged += SelectionChanged;
                }
                _partMinute.DataSource.SelectedItem = data;
            }

            if (_partMeridiem != null)
            {
                if (_partMeridiem.DataSource == null)
                {
                    _partMeridiem.DataSource = new AmPmDataSource();
                    _partMeridiem.DataSource.SelectionChanged += SelectionChanged;
                }
                _partMeridiem.DataSource.SelectedItem = data;
            }


            if (_partsecond != null)
            {
                if (_partsecond.DataSource == null)
                {
                    _partsecond.DataSource = new SecondDataSource();
                    _partsecond.DataSource.SelectionChanged += SelectionChanged;
                }
                _partsecond.DataSource.SelectedItem = data;
            }

        }

        private void SetHour(TimeSpan currenttime, int inputhour)
        {
            if (!DateTimeWrapper.IsTwentyFourHourtimeline && inputhour > 12)
                inputhour = 12;
            else if (DateTimeWrapper.IsTwentyFourHourtimeline && inputhour > 24)
                inputhour = 0;
            TimeSpan result = new TimeSpan(Convert.ToInt32(inputhour),currenttime.Minutes,currenttime.Seconds);
            SelectedTime = result;
            _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;            
        }

        private void SetMinute(TimeSpan currenttime, int inputminute)
        {
            if (inputminute >= 60)
                inputminute = 0;
            TimeSpan result=new TimeSpan(currenttime.Hours,inputminute,currenttime.Seconds);
            SelectedTime = result;
            _partHour.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
        }

        private void SetSecond(TimeSpan currenttime, int inputsecond)
        {
            if (Convert.ToInt32(inputsecond) >= 60)
                inputsecond = 0;
            TimeSpan result = new TimeSpan(currenttime.Hours, currenttime.Minutes, inputsecond);
            SelectedTime = result;
            _partHour.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;            
        }

        private void ExpandLoop(int index)
        {
            if (_loopingSelectorOrder[index] == 'h')
                _partHour.IsExpanded = true;
            else if (_loopingSelectorOrder[index] == 'm')
                _partMinute.IsExpanded = true;
            else if (_loopingSelectorOrder[index] == 's')
                _partsecond.IsExpanded = true;
            else if (_loopingSelectorOrder[index] == 't')
                _partMeridiem.IsExpanded = true;
        }

        private void ExpandPrevious(LoopingSelector selector, int index)
        {
            selector.IsExpanded = false;
            if (index - 1 < 0)
            {
                if (ShowCancelButton && PartCancelButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else if (ShowDoneButton && PartSelectButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartSelectButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartSelectButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
            }
            else
                ExpandLoop(index - 1);
        }
       
        private void ExpandNext(LoopingSelector selector, int index)
        {
            selector.IsExpanded = false;
            if (index + 1 >= _loopingSelectorOrder.Count)
            {
                if (ShowDoneButton && PartSelectButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartSelectButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartSelectButton, "Focused", true);
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

        private void ExecuteTabKey()
        {
            if (_partHour != null && _partHour.IsExpanded)
                ExpandNext(_partHour, _loopingSelectorOrder.IndexOf('h'));
            else if (_partMinute != null && _partMinute.IsExpanded)
                ExpandNext(_partMinute, _loopingSelectorOrder.IndexOf('m'));
            else if (_partsecond != null && _partsecond.IsExpanded)
                ExpandNext(_partsecond, _loopingSelectorOrder.IndexOf('s'));
            else if (_partMeridiem != null && _partMeridiem.IsExpanded)
                ExpandNext(_partMeridiem, _loopingSelectorOrder.IndexOf('t'));
            else if (ShowDoneButton && PartSelectButton.FocusState == FocusState.Keyboard)
            {
                PartSelectButton.IsTabStop = false;
                VisualStateManager.GoToState(PartSelectButton, "Normal", true);
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
            if (_partHour != null && _partHour.IsExpanded)
                ExpandPrevious(_partHour, _loopingSelectorOrder.IndexOf('h'));
            else if (_partMinute != null && _partMinute.IsExpanded)
                ExpandPrevious(_partMinute, _loopingSelectorOrder.IndexOf('m'));
            else if (_partsecond != null && _partsecond.IsExpanded)
                ExpandPrevious(_partsecond, _loopingSelectorOrder.IndexOf('s'));
            else if (_partMeridiem != null && _partMeridiem.IsExpanded)
                ExpandPrevious(_partMeridiem, _loopingSelectorOrder.IndexOf('t'));
            else if (ShowCancelButton && PartCancelButton.FocusState == FocusState.Keyboard)
            {
                PartCancelButton.IsTabStop = false;
                VisualStateManager.GoToState(PartCancelButton, "Normal", true);
                if (ShowDoneButton)
                {
                    PartSelectButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartSelectButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
            }
            else
            {
                if (ShowDoneButton && PartSelectButton.FocusState == FocusState.Keyboard)
                {
                    PartSelectButton.IsTabStop = false;
                    VisualStateManager.GoToState(PartSelectButton, "Normal", true);
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
                }
                else if (ShowCancelButton && PartCancelButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartCancelButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartCancelButton, "Focused", true);
                }
                else if (ShowDoneButton && PartSelectButton.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
                {
                    PartSelectButton.IsTabStop = true;
                    VisualStateManager.GoToState(PartSelectButton, "Focused", true);
                }
                else
                    ExpandLoop(_loopingSelectorOrder.Count - 1);
            }
        }

        private void NavigateUp()
        {
            if (_partHour != null && _partHour.IsExpanded)
            {
                DateTime newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(-1);
                if (!DateTimeWrapper.IsTwentyFourHourtimeline)
                {
                    if ((_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "PM" && newtime.TimeOfDay.Hours == 11)
                        newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(11);
                    else if ((_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "AM" && newtime.TimeOfDay.Hours == 0)
                        newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(23);
                    else if ((_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "AM" && newtime.TimeOfDay.Hours == 23)
                        newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(11);
                }
                SelectedTime = newtime.TimeOfDay;
                _partsecond.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;
            }
            else if (_partMinute != null && _partMinute.IsExpanded)
            {
                DateTime newtime = (_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMinutes(-1);
                if (newtime.Hour + 1 == (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.Hour)
                    newtime = ((_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMinutes(-1).AddHours(1));
                SelectedTime = newtime.TimeOfDay;
                _partsecond.DataSource.SelectedItem = _partHour.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
            }
            else if (_partsecond != null && _partsecond.IsExpanded)
            {
                DateTime newtime = (_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddSeconds(-1);
                if (newtime.Minute + 1 == (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.Minute)
                    newtime = ((_partsecond.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddSeconds(-1).AddMinutes(1));
                SelectedTime = newtime.TimeOfDay;
                _partMinute.DataSource.SelectedItem = _partHour.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;
            }
            else if (!DateTimeWrapper.IsTwentyFourHourtimeline && _partMeridiem != null && _partMeridiem.IsExpanded)
            {
                if (_partHour != null && (_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "PM")
                {
                    DateTime newtime = (_partMeridiem.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(-12);
                    SelectedTime = newtime.TimeOfDay;
                    _partHour.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partMeridiem.DataSource.SelectedItem;
                }
            }
        }

        private void NavigateDown()
        {
            if (_partsecond != null && _partsecond.IsExpanded)
            {
                DateTime newtime = (_partsecond.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddSeconds(1);
                if (newtime.Minute - 1 == (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.Minute)
                    newtime = ((_partsecond.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddSeconds(1).AddMinutes(-1));
                SelectedTime = newtime.TimeOfDay;
                _partMinute.DataSource.SelectedItem = _partHour.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem;
            }
            else if (_partMinute != null && _partMinute.IsExpanded)
            {
                DateTime newtime = (_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMinutes(1);
                if (newtime.Hour - 1 == (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.Hour)
                    newtime = ((_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddMinutes(1).AddHours(-1));
                SelectedTime = newtime.TimeOfDay;
                _partHour.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem;
            }
            else if (_partHour != null && _partHour.IsExpanded)
            {
                DateTime newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(1);
                if (!DateTimeWrapper.IsTwentyFourHourtimeline)
                {
                    if ((_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "PM" && newtime.TimeOfDay.Hours == 0)
                        newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(-11);
                    else if ((_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "AM" && newtime.TimeOfDay.Hours == 12)
                        newtime = (_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(-11);
                }
                SelectedTime = newtime.TimeOfDay;
                _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partHour.DataSource.SelectedItem;
            }
            else if (!DateTimeWrapper.IsTwentyFourHourtimeline && _partMeridiem != null && _partMeridiem.IsExpanded)
            {
                if (_partHour != null && (_partHour.DataSource.SelectedItem as DateTimeWrapper).AmPmString == "AM")
                {
                    DateTime newtime = (_partMeridiem.DataSource.SelectedItem as DateTimeWrapper).DateTime.AddHours(12);
                    SelectedTime = newtime.TimeOfDay;
                    _partHour.DataSource.SelectedItem = _partMinute.DataSource.SelectedItem = _partsecond.DataSource.SelectedItem = _partMeridiem.DataSource.SelectedItem;
                }
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes all te child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partHour = GetTemplateChild("PART_Hour") as LoopingSelector;
            _partMinute = GetTemplateChild("PART_Minute") as LoopingSelector;
            _partMeridiem = GetTemplateChild("PART_Meridiem") as LoopingSelector;
            _partsecond = GetTemplateChild("PART_Second") as LoopingSelector;
            PartSelectButton = GetTemplateChild("PART_SelectButton") as Button;
            PartCancelButton = GetTemplateChild("PART_CancelButton") as Button;
            _headergrid = GetTemplateChild("HeaderGrid") as Grid;
            _footergrid = GetTemplateChild("FooterGrid") as Grid;

            if (_partHour != null)
            {
                _partHour.PointerPressed += HourPointerPressed;
                _partHour.PointerWheelChanged += HourPointerPressed;
                _partHour.ManipulationCompleted += SelectorManipulationCompleted;
                _partHour.ManipulationStarted += SelectorManipulationStarted;
                _partHour.PointerWheelChanged += OnPointerWheelChanged;
            }

            if (_partMinute != null)
            {
                _partMinute.PointerPressed += MinutePointerPressed;
                _partMinute.PointerWheelChanged += MinutePointerPressed;
                _partMinute.ManipulationCompleted += SelectorManipulationCompleted;
                _partMinute.ManipulationStarted += SelectorManipulationStarted;
                _partMinute.PointerWheelChanged += OnPointerWheelChanged;
            }

            if (_partsecond != null)
            {
                _partsecond.PointerPressed += SecondPointerPressed;
                _partsecond.PointerWheelChanged += SecondPointerPressed;
                _partsecond.ManipulationCompleted += SelectorManipulationCompleted;
                _partsecond.ManipulationStarted += SelectorManipulationStarted;
                _partsecond.PointerWheelChanged += OnPointerWheelChanged;
            }

            if (_partMeridiem != null)
            {
                _partMeridiem.PointerPressed += MeridiemPointerPressed;
                _partMeridiem.PointerWheelChanged += MeridiemPointerPressed;
                _partMeridiem.ManipulationCompleted += SelectorManipulationCompleted;
                _partMeridiem.ManipulationStarted += SelectorManipulationStarted;
                _partMeridiem.PointerWheelChanged += OnPointerWheelChanged;
            }

            if (PartSelectButton != null)
            {
                PartSelectButton.Click += PART_SelectButton_Click;
            }

            if (PartCancelButton != null)
            {
                PartCancelButton.Click += PART_CancelButton_Click;
            }

            UpdateData();
        }

        /// <summary>
        /// Handles keydown event <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/> control.
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
                if (!string.IsNullOrEmpty(inputstring) && inputstring.Length == 2)
                    inputstring = string.Empty;
                if (e.Key.ToString().Contains("NumberPad"))
                    inputstring += e.Key.ToString().Replace("NumberPad", "");
                else
                    inputstring += e.Key.ToString().Replace("Number", "");
                if (_partHour != null && _partHour.IsExpanded)
                    SetHour(((_partHour.DataSource.SelectedItem as DateTimeWrapper).DateTime.TimeOfDay), Convert.ToInt32(inputstring));
                else if (_partMinute != null && _partMinute.IsExpanded)
                    SetMinute(((_partMinute.DataSource.SelectedItem as DateTimeWrapper).DateTime.TimeOfDay), Convert.ToInt32(inputstring));
                else if (_partsecond != null && _partsecond.IsExpanded)
                    SetSecond(((_partsecond.DataSource.SelectedItem as DateTimeWrapper).DateTime.TimeOfDay), Convert.ToInt32(inputstring));
            }
            base.OnKeyDown(e);
        }
        #endregion               

        #region Callback Methods

        private static void OnFormatStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfTimeSelector;
            if (selector != null)
            {
                selector.OnFormatStringChanged(args);
            }
        }

        private void ResetVisibility()
        {
            if (_partHour != null)
                _partHour.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (_partMeridiem != null)
                _partMeridiem.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (_partMinute != null)
                _partMinute.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (_partsecond != null)
                _partsecond.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Occurs when the FormatString is changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                string formatString = args.NewValue.ToString().ToLower();
                _loopingSelectorOrder.Clear();
                ResetVisibility();
                if (formatString.Contains("h") || formatString.Contains("m") || formatString.Contains("t")||formatString.Contains("s"))
                {
                    for (int i = 0; i < formatString.Length; i++)
                    {
                        if (formatString[i] == 'h' ||
                            formatString[i] == 'm' ||
                            formatString[i] == 't' ||
                            formatString[i] == 's')
                        {
                            char _char = formatString[i];
                            if (!_loopingSelectorOrder.Contains(_char))
                                _loopingSelectorOrder.Add(formatString[i]);
                        }
                    }
                    if (_partHour != null && _partMinute != null && _partMeridiem != null && _partsecond!=null)
                    {
                        if (!_loopingSelectorOrder.Contains('h'))
                            _partHour.Visibility = Visibility.Collapsed;
                        else
                        {
                            _partHour.DataSource = null;
                            UpdateData();
                            Grid.SetColumn(_partHour, _loopingSelectorOrder.IndexOf('h'));                            
                        }

                        if (!_loopingSelectorOrder.Contains('m'))
                            _partMinute.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partMinute, _loopingSelectorOrder.IndexOf('m'));

                        if (!_loopingSelectorOrder.Contains('s'))
                            _partsecond.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partsecond, _loopingSelectorOrder.IndexOf('s'));

                        if (!_loopingSelectorOrder.Contains('t'))
                            _partMeridiem.Visibility = Visibility.Collapsed;
                        else
                            Grid.SetColumn(_partMeridiem, _loopingSelectorOrder.IndexOf('t'));
                    }
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain h/m/s/t");
                }
            }
        }

        private static void OnSelectedDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfTimeSelector;
            if (selector != null)
            {
                selector.UpdateData();

                if (selector.SelectedTimeChanged != null)
                {
                    selector.SelectedTimeChanged(sender, args);
                }

            }
        }



        private static void OnSelectorItemSpacingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfTimeSelector;
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
                        case 'h':
                            if (_partHour != null)
                            {
                                var hourmarginheight = _partHour.Margin.Top + _partHour.Margin.Bottom;
                                itemmarginheight = itemmarginheight < hourmarginheight ? hourmarginheight : itemmarginheight;
                            }
                            break;
                        case 'm':
                            if (_partMinute != null)
                            {
                                var minutemarginheight = _partMinute.Margin.Top + _partMinute.Margin.Bottom;
                                itemmarginheight = itemmarginheight < minutemarginheight ? minutemarginheight : itemmarginheight;
                            }
                            break;
                        case 't':
                            if (_partMeridiem != null)
                            {
                                var meridiemmarginheight = _partMeridiem.Margin.Top + _partMeridiem.Margin.Bottom;
                                itemmarginheight = itemmarginheight < meridiemmarginheight ? meridiemmarginheight : itemmarginheight;
                            }
                            break;
                        case 's':
                            if (_partsecond != null)
                            {
                                var secondmarginheight = _partsecond.Margin.Top + _partsecond.Margin.Bottom;
                                itemmarginheight = itemmarginheight < secondmarginheight ? secondmarginheight : itemmarginheight;
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
                    case 'h':
                        if (_partHour != null)
                        {
                            _partHour.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partHour.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                    case 'm':
                        if (_partMinute != null)
                        {
                            _partMinute.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partMinute.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                    case 't':
                        if (_partMeridiem != null)
                        {
                            _partMeridiem.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partMeridiem.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                    case 's':
                        if (_partsecond != null)
                        {
                            _partsecond.ItemMargin = (i == 0) ? new Thickness(4, 4, 0, 4) :
                                (i == _loopingSelectorOrder.Count - 1) ? new Thickness(0, 4, 4, 4) : new Thickness(0, 4, 0, 4);
                            if (i != _loopingSelectorOrder.Count - 1)
                                _partsecond.Margin = new Thickness(0, 0, SelectorItemSpacing, 0);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when selected time changed.
        /// </summary>
        /// <remarks>
        /// raised when selected time property is changed.
        /// </remarks>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimeSelector.SelectedTime"/>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SelectedTimeChanged;

        /// <summary>
        /// Occurs when cancel button is clicked.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Cancel;

        /// <summary>
        /// Occurs when done is clicked.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Select;

        #endregion

    }
}
