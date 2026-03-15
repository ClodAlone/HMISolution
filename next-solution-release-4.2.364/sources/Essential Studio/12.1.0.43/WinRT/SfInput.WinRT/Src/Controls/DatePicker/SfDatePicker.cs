// <copyright file="DatePicker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using Syncfusion.UI.Xaml.Controls.Data;
using System;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that allows the user to select a date by using a drop-down
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfDatePicker : Control, IDataValidator,IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Input">Syncfusion.UI.Xaml.Controls.Input
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfDatePicker()
        {
            DefaultStyleKey = typeof(SfDatePicker);
            Loaded += DatePicker_Loaded;
            Unloaded += SfDatePicker_Unloaded;
        }

        void SfDatePicker_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= SfDatePicker_Unloaded;
        }

        #endregion

        #region Variables

        private RepeatButton _partDropDownButton;

        private SfTextBoxExt _partTextBoxExt;

        private Popup _partPopup;

        private FrameworkElement _partDatePickerPage;

        private SfDateSelector _partDateSelector;

        private SfTextBoxExt _partTextBlock;


        #endregion       

        #region Dependency Properties

         ///<summary>
        ///Gets or sets the input scope
        /// </summary>

        //[ClassReference(IsReviewed=false)]


        public InputScopeNameValue InputScope
        {
            get { return (InputScopeNameValue)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputScope.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScopeNameValue), typeof(SfDatePicker), new PropertyMetadata(InputScopeNameValue.Default));

        /// <summary>
        /// Gets or sets the value of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> that hold the currently
        /// selected date.
        /// </summary>
        /// <value>
        /// The default value is <see cref="P:System.DateTime.Now">DateTime.Now</see>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.SetValueOnLostFocus"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        [ClassReference(IsReviewed = false)]
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDateTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SfDatePicker), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Gets or sets the Minimum number of dates to be listed in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> control.
        /// </summary>
        /// <value> The default value is  DateTime.MinValue</value>
        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }
            set { SetValue(MinDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register("MinDate", typeof(DateTime), typeof(SfDatePicker), new PropertyMetadata(DateTime.MinValue, OnMinDateChanged));



        /// <summary>
        /// Gets or sets the Maximum number of dates to be listed in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> control.
        /// </summary>
        /// <value> The default value is  DateTime.MaxValue</value>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
            DependencyProperty.Register("MaxDate", typeof(DateTime), typeof(SfDatePicker), new PropertyMetadata(DateTime.MaxValue, OnMaxDateChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the popup <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> drop down is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.DropDownHeight"/>
        [ClassReference(IsReviewed = false)]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SfDatePicker), new PropertyMetadata(false,OnIsDropDownOpenChanged));




        /// <summary>
        /// Gets or sets the height of the drop down for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>.
        /// </summary>
        /// <value>
        /// The default value is is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.IsDropDownOpen"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        [ClassReference(IsReviewed = false)]
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectorStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(SfDatePicker), new PropertyMetadata(0.0));



        /// <summary>
        /// Gets or sets the style that apply for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null
        /// </value>
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
            DependencyProperty.Register("SelectorStyle", typeof(Style), typeof(SfDatePicker), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value indicating whether set <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.Value"/> on lost focus.
        /// </summary>
        /// <value>
        /// <c>true</c> if set value on lost focus; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.Value"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        [ClassReference(IsReviewed = false)]
        public bool SetValueOnLostFocus
        {
            get { return (bool)GetValue(SetValueOnLostFocusProperty); }
            set { SetValue(SetValueOnLostFocusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SetValueInLostFocus.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SetValueOnLostFocusProperty =
            DependencyProperty.Register("SetValueOnLostFocus", typeof(bool), typeof(SfDatePicker), new PropertyMetadata(false));



        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>.
        /// </summary>
        /// <value>ba
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfDatePicker), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the data that is used as a format for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/>
        [ClassReference(IsReviewed = false)]
        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FormatString.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register("FormatString", typeof(string), typeof(SfDatePicker), new PropertyMetadata("d",OnFormatStringChanged));


        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool AllowInlineEditing
        {
            get { return (bool)GetValue(AllowInlineEditingProperty); }
            set { SetValue(AllowInlineEditingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowInlineEditing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowInlineEditingProperty =
            DependencyProperty.Register("AllowInlineEditing", typeof(bool), typeof(SfDatePicker), new PropertyMetadata(false, OnAllowInlineEditingChanged));

        /// <summary>
        /// Gets or sets the SelectorFormatString for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateSelector"/> control.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object SelectorFormatString
        {
            get { return GetValue(SelectorFormatStringProperty); }
            set { SetValue(SelectorFormatStringProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ValueFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectorFormatStringProperty =
            DependencyProperty.Register("SelectorFormatString", typeof(object), typeof(SfDatePicker), new PropertyMetadata("m:d:y",OnSelectorFormatStringChanged));

        /// <summary>
        /// Gets or sets the AllowNull for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDatePicker"/> control.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }
        ///<summary>
        /// Using a DependencyProperty as the backing store for AllowNull.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(SfDatePicker), new PropertyMetadata(false,OnAllowNullChanged));


        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>

        public bool ShowDropDownButton
        {
            get { return (bool)GetValue(ShowDropDownButtonProperty); }
            set { SetValue(ShowDropDownButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowDropDownButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDropDownButtonProperty =
            DependencyProperty.Register("ShowDropDownButton", typeof(bool), typeof(SfDatePicker), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the data used as WateMark
        /// </summary>
        /// <value> The default value is null </value>
        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(SfDatePicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the data used as WateMark
        /// </summary>
        /// <value> The default value is null </value>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(SfDatePicker), new PropertyMetadata(null));

        

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
            DependencyProperty.Register("SelectorItemHeight", typeof(double), typeof(SfDatePicker), new PropertyMetadata(80));


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
            DependencyProperty.Register("SelectorItemWidth", typeof(double), typeof(SfDatePicker), new PropertyMetadata(80));


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
            DependencyProperty.Register("SelectorItemSpacing", typeof(double), typeof(SfDatePicker), new PropertyMetadata(4.0));


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
            DependencyProperty.Register("SelectorItemCount", typeof(int), typeof(SfDatePicker), new PropertyMetadata(0));



        #endregion

        #region Helper Methods

        void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Validate();
            //if (_partDateSelector != null)
            //    _partDateSelector.FormatString = SelectorFormatString;
            if (ReadLocalValue(SelectorFormatStringProperty) == DependencyProperty.UnsetValue)
                SelectorFormatString = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            ValidatePopupPosition();
            CheckInlineEdit();
           
        }

        private void Validate()
        {
            if (Value != null)
            {
                DateTime selecteddate;
                double mindiffvalue = 0, maxdiffvalue = 0;
                bool success = DateTime.TryParse(Value.ToString(), out selecteddate);
                if (selecteddate.Date < MinDate.Date || selecteddate.Date > MaxDate.Date)
                {
                    mindiffvalue = (MinDate.Date - selecteddate.Date).TotalDays;
                    maxdiffvalue = (selecteddate.Date - MaxDate).TotalDays;
                }
                if (mindiffvalue > maxdiffvalue)
                    Value = MinDate;
                else if (mindiffvalue < maxdiffvalue)
                    Value = MaxDate;
            }
        }

        void CheckInlineEdit()
        {
            if (AllowInlineEditing)
            {
                if (_partTextBoxExt != null)
                {
                    _partTextBoxExt.IsReadOnly = !AllowInlineEditing;
                    _partTextBoxExt.AllowPointerEvents = !AllowInlineEditing;
                    DateTime dateTime;
                    _partTextBoxExt.Text = Value != null && DateTime.TryParse(Value.ToString(), out dateTime) ? dateTime.ToString("MM/dd/yyyy") : string.Empty;
                }
            }
            else
            {
                if (_partTextBlock != null)
                {
                    DateTime dateTime;
                    _partTextBlock.Text = Value != null && DateTime.TryParse(Value.ToString(), out dateTime) ? dateTime.ToString(FormatString) : string.Empty;
                }

            }
        }

        private void ValidatePopupPosition()
        {
            double top;
            double left = 0.0;
            var windowBounds = Window.Current.CoreWindow.Bounds;
            double selectorheight = _partDateSelector != null ? _partDateSelector.UpdateSelectorHeight() : 0;
            if (selectorheight != 0)
                DropDownHeight = selectorheight;
            if (DropDownHeight > windowBounds.Height)
                DropDownHeight = windowBounds.Height;
            top = -((DropDownHeight / 2) - (ActualHeight / 2));
            GeneralTransform pickerTransform = TransformToVisual(Window.Current.Content);
            Point point1 = pickerTransform.TransformPoint(new Point());
            var rect = new Rect(point1.X, point1.Y, ActualWidth, ActualHeight);
            if (_partPopup != null)
            {
                if (_partDateSelector != null)
                {
                    if (_partDateSelector.ActualWidth + rect.X > windowBounds.Width)
                    {
                        left = -(_partDateSelector.ActualWidth / 2 - ActualWidth / 2);
                        _partPopup.HorizontalOffset = -(rect.X + left) + left;
                    }
                }
                if (rect.X + left < 0)
                {
                    _partPopup.HorizontalOffset = -(rect.X + left) + left;
                }
                else if (rect.Right - left > windowBounds.Width)
                {
                    _partPopup.HorizontalOffset = 2 * left + (windowBounds.Width - rect.Right);
                }
                else
                {
                    _partPopup.HorizontalOffset = left;
                }
                if (rect.Y + top < 0)
                {
                    _partPopup.VerticalOffset = -(rect.Y + top) + top;
                }
                else if (rect.Y - top > windowBounds.Bottom)
                {
                    _partPopup.VerticalOffset = 2 * top + ((windowBounds.Height - rect.Bottom));
                }
                else
                {
                    _partPopup.VerticalOffset = top;
                }

                if (_partDateSelector != null)
                {
                    _partDateSelector.Focus(FocusState.Keyboard);
                }
            }
        }

        private void DropDownOpen()
        {
            if (_partDateSelector != null)
            {
                _partDateSelector.SelectedDateTime = Value;
                _partDateSelector.CollapseAll();
                _partDateSelector.UpdateData();
            }

        }

        void PART_DateSelector_Cancel(object sender, RoutedEventArgs e)
        {
            if (_partDateSelector != null)
            {
                _partDateSelector.SelectedDateTime = Value;
                _partDateSelector.CollapseAll();
                _partDateSelector.UpdateData();
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
        }

        void PART_DateSelector_Done(object sender, RoutedEventArgs e)
        {
            if (_partDateSelector != null)
            {
                Value = _partDateSelector.SelectedDateTime;
                _partDateSelector.CollapseAll();
                _partDateSelector.UpdateData();
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
        }

        private void PART_TextBoxExt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AllowInlineEditing)
            {
                DateTime datetime;
                var sfTextBoxExt = sender as SfTextBoxExt;
                bool success=(DateTime.TryParseExact(sfTextBoxExt.Text, this.FormatString, System.Globalization.CultureInfo.CurrentUICulture, System.Globalization.DateTimeStyles.None, out datetime));
                Value = (sfTextBoxExt != null && success && datetime.Date >= MinDate.Date && datetime.Date <= MaxDate.Date)
                            ? datetime
                            : Value;
                if (Value != null)
                    _partTextBoxExt.Text = (DateTime.TryParse(Value.ToString(), out datetime))
                                               ? datetime.ToString(FormatString)
                                               : string.Empty;
                else
                    _partTextBoxExt.Text = string.Empty;
            }
        }

        void ClosePickerPage()
        {
            var element = FocusManager.GetFocusedElement() as FrameworkElement;
            if (element == _partDateSelector.PartDoneButton ||
                element == _partDateSelector.PartCancelButton)
            {
                return;
            }
            if (_partDateSelector != null && IsDropDownOpen)
            {
                _partDateSelector.CollapseAll();
                if (SetValueOnLostFocus)
                {
                    Value = _partDateSelector.SelectedDateTime;
                }
                else
                {
                    _partDateSelector.SelectedDateTime = Value;
                    _partDateSelector.CollapseAll();
                    _partDateSelector.UpdateData();
                }
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
        }

        private void OnStarted(object sender, object e)
        {
            if (_partDateSelector != null)
            {
                _partDateSelector.Focus(FocusState.Keyboard);
            }
        }

        private void OnCompleted(object sender, object e)
        {
            IsDropDownOpen = false;
            Focus(FocusState.Keyboard);
        }

        private void FormatValue(object value)
        {
            DateTime datetime;
            if (value == null || (value != null && string.IsNullOrEmpty(value.ToString())))
            {
                if (AllowInlineEditing)
                {
                    if (_partTextBoxExt != null)
                    {
                        if (AllowNull)
                            _partTextBoxExt.Text = string.Empty;
                        else
                            _partTextBoxExt.Text = DateTime.Now.ToString();
                        _partDateSelector.SelectedDateTime = DateTime.Now;
                        _partDateSelector.UpdateData();
                    }
                }
                else
                {
                    if (_partTextBlock != null)
                    {
                        if (AllowNull)
                            _partTextBlock.Text = string.Empty;
                        else
                            _partTextBlock.Text = DateTime.Now.ToString();
                        _partDateSelector.SelectedDateTime = DateTime.Now;
                        _partDateSelector.UpdateData();
                    }
                }
                if (!AllowNull)
                    Value = DateTime.Now;
            }
            else if (DateTime.TryParse(value.ToString(), out datetime))
            {
                if (AllowInlineEditing)
                {
                    if (_partTextBoxExt != null)
                    {
                        _partTextBoxExt.Text = datetime.ToString(FormatString);
                        _partDateSelector.SelectedDateTime = Value;
                        _partDateSelector.UpdateData();
                    }
                }
                else
                {
                    if (_partTextBlock != null)
                    {
                        _partTextBlock.Text = datetime.ToString(FormatString);
                        _partDateSelector.SelectedDateTime = Value;
                        _partDateSelector.UpdateData();
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Input is not in DateTime format");
            }
        }

        internal void Animate(double from, double to, EasingMode easingmode, EventHandler<object> completed)
        {
            if (_partDatePickerPage != null)
            {
                _partDatePickerPage.RenderTransform = new CompositeTransform();
                _partDatePickerPage.RenderTransformOrigin = new Point(0.5, 0.5);

                Timeline timeline = BuildAnimation(from, to, TimeSpan.FromSeconds(0.1), new ExponentialEase { EasingMode = easingmode });
                Storyboard.SetTarget(timeline, _partDatePickerPage);
                Storyboard.SetTargetProperty(timeline, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

                var story = new Storyboard();
                story.Children.Add(timeline);
                if (completed != null)
                {
                    story.Completed += completed;
                }
                story.Begin();
            }
        }

        private Timeline BuildAnimation(double from, double to, TimeSpan duration, EasingFunctionBase easingfunction)
        {
            var timeline = new DoubleAnimationUsingKeyFrames();
            var frame1 = new EasingDoubleKeyFrame {Value = @from, KeyTime = TimeSpan.FromSeconds(0)};
            var frame2 = new EasingDoubleKeyFrame {Value = to, KeyTime = duration, EasingFunction = easingfunction};
            timeline.KeyFrames.Add(frame1);
            timeline.KeyFrames.Add(frame2);
            return timeline;
        }

        void PART_TextBoxExt_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(!AllowInlineEditing)
                _partTextBoxExt.SelectionLength = 0;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            _partTextBoxExt = GetTemplateChild("PART_TextBoxExt") as SfTextBoxExt;
            _partPopup = GetTemplateChild("PART_DropDown") as Popup;
            _partDatePickerPage = GetTemplateChild("PART_DatePickerPage") as FrameworkElement;
            _partDateSelector = GetTemplateChild("PART_DateSelector") as SfDateSelector;
            _partDropDownButton = GetTemplateChild("PART_DropDownButton") as RepeatButton;
            _partTextBlock = GetTemplateChild("PART_TextBlock") as SfTextBoxExt;

            if (_partTextBoxExt != null)
            {
                FormatValue(Value);
                _partTextBoxExt.LostFocus += PART_TextBoxExt_LostFocus;
                _partTextBoxExt.SelectionChanged += PART_TextBoxExt_SelectionChanged;
            }

            if (_partDateSelector != null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorFormatString");
                binding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.FormatStringProperty, binding);
                Binding minvaluebinding = new Binding();
                minvaluebinding.Source = this;
                minvaluebinding.Path = new PropertyPath("MinDate");
                minvaluebinding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.MinDateProperty, minvaluebinding);
                Binding maxvaluebinding = new Binding();
                maxvaluebinding.Source = this;
                maxvaluebinding.Path = new PropertyPath("MaxDate");
                maxvaluebinding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.MaxDateProperty, maxvaluebinding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemHeight");
                binding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.SelectorItemHeightProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemWidth");
                binding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.SelectorItemWidthProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemSpacing");
                binding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.SelectorItemSpacingProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemCount");
                binding.Mode = BindingMode.TwoWay;
                _partDateSelector.SetBinding(SfDateSelector.SelectorItemCountProperty, binding);
                
                _partDateSelector.Done += PART_DateSelector_Done;
                _partDateSelector.Cancel += PART_DateSelector_Cancel;
                _partDateSelector.LostFocus += PART_DateSelector_LostFocus;
                _partDateSelector.Loaded += PART_DateSelector_Loaded;
                _partDateSelector.KeyDown += PART_DateSelector_KeyDown;
            }
            if (_partDropDownButton != null)
            {
                _partDropDownButton.Click += PART_DropDownButton_Click;
            }
            CheckInlineEdit();
            base.OnApplyTemplate();
        }

        void PART_DateSelector_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                if (_partDateSelector != null)
                {
                    _partDateSelector.SelectedDateTime = Value;
                    _partDateSelector.CollapseAll();
                    _partDateSelector.UpdateData();
                }
                Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
            else if (e.Key == Windows.System.VirtualKey.Enter && IsDropDownOpen)
            {
                if (_partDateSelector != null)
                {
                    Value = _partDateSelector.SelectedDateTime;
                    _partDateSelector.CollapseAll();
                    _partDateSelector.UpdateData();
                }
                Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
        }

        void PART_DateSelector_Loaded(object sender, RoutedEventArgs e)
        {
            if (_partDateSelector.PartDoneButton != null)
                VisualStateManager.GoToState(_partDateSelector.PartDoneButton, "Normal", false);
            if (_partDateSelector.PartDoneButton != null)
                VisualStateManager.GoToState(_partDateSelector.PartCancelButton, "Normal", false);
            _partDateSelector.UpdateLayout();
            _partDateSelector.MinWidth = ActualWidth;
            ValidatePopupPosition();
        }

        void PART_DateSelector_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(FocusManager.GetFocusedElement() is LoopingSelector))
            {
                if ((FocusManager.GetFocusedElement() != this._partDateSelector))
                {
                    ClosePickerPage();
                }
            }
        }

        void PART_DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllowInlineEditing)
            {
                DateTime datetime;
                Value = (DateTime.TryParse(_partTextBoxExt.Text, out datetime)) ? datetime : Value;
                if (Value != null)
                    _partTextBoxExt.Text = (DateTime.TryParse(Value.ToString(), out datetime)) ? datetime.ToString(FormatString) : string.Empty;
                else
                    _partTextBoxExt.Text = string.Empty;
            }
            if (_partTextBoxExt != null)
            {
                Value = _partTextBoxExt.Text;
            }
            if (_partPopup != null)
            {
                IsDropDownOpen = true;
                Animate(0, 1, EasingMode.EaseOut, null);
            }
        }

       
        /// <summary>
        /// Occurs when the pointer is released
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (_partPopup != null && !AllowInlineEditing)
            {
                e.Handled = true;
                IsDropDownOpen = true;
                Animate(0, 1, EasingMode.EaseOut, null);
            }
        }      

        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key== Windows.System.VirtualKey.Space)
            {
                if (!IsDropDownOpen && !AllowInlineEditing)
                {
                    e.Handled = true;
                    IsDropDownOpen = true;
                    Animate(0, 1, EasingMode.EaseOut, null);

                }
                else
                {
                    if (_partDateSelector != null)
                    {
                        if (_partTextBoxExt != null && !string.IsNullOrEmpty(_partTextBoxExt.Text))
                            Value = _partTextBoxExt.Text.ToDateTime();
                        else
                            Value = _partDateSelector.SelectedDateTime;
                        Validate();
                        _partDateSelector.CollapseAll();
                        _partDateSelector.UpdateData();
                    }
                    Animate(1, 0, EasingMode.EaseIn, OnCompleted);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Escape) 
            {
                if (_partDateSelector != null)
                {
                    _partDateSelector.SelectedDateTime = Value;
                    _partDateSelector.CollapseAll();
                    _partDateSelector.UpdateData();
                }
               Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Callback Methods

        private static void OnSelectorFormatStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfDatePicker;
            if (selector != null)
            {
                selector.OnSelectorFormatStringChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the data used as SelectorFormatString has changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnSelectorFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                string format = args.NewValue.ToString().ToLower();
                if (format.Contains("m") || format.Contains("d") || format.Contains("y"))
                {
                    //if (_partDateSelector != null)
                    //    _partDateSelector.FormatString = args.NewValue;
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain m/d/y");
                }
            }
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as SfDatePicker;
            if (picker != null)
            {
                picker.FormatValue(args.NewValue);
                bool canexecute = args.NewValue != null ? args.NewValue.Equals(picker.Value) : picker.Value == null ? true : false;
                if (canexecute && picker.ValueChanged != null)
                {
                    picker.ValueChanged(sender, args);
                }
                if(picker!=null && picker.Value!=null)
                picker.Validate();
            }
        }

        private static void OnMaxDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as SfDatePicker;
            if (picker != null)
                picker.Validate();
        }

        private static void OnMinDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as SfDatePicker;
            if (picker != null)
                picker.Validate();
        }

        private static void OnAllowInlineEditingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfDatePicker;
            if (instance != null) instance.OnAllowInlineEditing(args);
        }

        /// <summary>
        /// Occurs when the instance of AllowInlineEditing is set true.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnAllowInlineEditing(DependencyPropertyChangedEventArgs args)
        {
            CheckInlineEdit();
        }
        private static void OnIsDropDownOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfDatePicker;
            if (instance != null) instance.OnIsDropDownOpenChanged(args);
        }

        private static void OnAllowNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfDatePicker;
            if (instance != null && instance.Value==null && !(bool)args.NewValue)
                instance.Value = DateTime.Now;
        }

        /// <summary>
        /// Occurs when the instance of IsDropDownOpen is set true.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (IsDropDownOpen)
                {
                    DropDownOpen();
                    Animate(0, 1, EasingMode.EaseOut,OnStarted);
                }
                else
                {
                    Animate(1, 0, EasingMode.EaseIn, OnCompleted);
                }
            }
        }

        private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfDatePicker;
            if (instance != null) instance.OnFormatStringChanged(args);
        }

        /// <summary>
        ///  Occurs when the data used as FormatString has changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            FormatValue(Value);
        }


        #endregion

        #region Events

        /// <summary>
        /// Occurs when the value for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DatePicker"/> changed.
        /// </summary>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DatePicker.Value"/>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback ValueChanged;

        #endregion


        /// <summary>
        /// /// Validates the states
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.VisualStates"/>
        /// </summary>
        /// <param name="args"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Validate(ValidationEventArgs args)
        {
            VisualStateManager.GoToState(this, args.HasError ? "HasError" : "NoError", true);
        }

        public void Dispose()
        {
            Loaded -= DatePicker_Loaded;
            if (_partTextBoxExt != null)
            {
                _partTextBoxExt.LostFocus -= PART_TextBoxExt_LostFocus;
                _partTextBoxExt.SelectionChanged -= PART_TextBoxExt_SelectionChanged;
            }
            if(_partDateSelector!=null)
            {
                _partDateSelector.Done -= PART_DateSelector_Done;
                _partDateSelector.Cancel -= PART_DateSelector_Cancel;
                _partDateSelector.LostFocus -= PART_DateSelector_LostFocus;
                _partDateSelector.Loaded -= PART_DateSelector_Loaded;
                _partDateSelector.KeyDown -= PART_DateSelector_KeyDown;
                _partDateSelector.Dispose();
                _partDateSelector = null;
            }
            if (_partDropDownButton != null)
            {
                _partDropDownButton.Click -= PART_DropDownButton_Click;
            }
            
            
        }
    }
}
