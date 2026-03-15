// <copyright file="TimePicker.cs" company="Syncfusion">
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
    /// Represents a control that allows the user to select a time by using a
    /// drop-down <see cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/> control
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfTimePicker : Control, IDataValidator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfTimePicker()
        {
            DefaultStyleKey = typeof(SfTimePicker);
            Loaded += TimePicker_Loaded;
        }

        void TimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            //if (_partTimeSelector != null)
            //    _partTimeSelector.FormatString = SelectorFormatString;
            if (ReadLocalValue(SelectorFormatStringProperty) == DependencyProperty.UnsetValue)
                SelectorFormatString = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            ValidatePopupPosition();
            CheckInlineEdit();
            if (!AllowNull && Value == null)
                Value = DateTime.Now;
        }

        #endregion

        #region Variables

        private RepeatButton _partDropDownButton;

        private SfTextBoxExt _partTextBoxExt;

        private Popup _partPopup;

        private FrameworkElement _partTimePickerPage;

        private SfTimeSelector _partTimeSelector;

        private SfTextBoxExt _partTextBlock;

        #endregion       

        #region Dependency Properties

        ///<summary>
        ///Gets or sets the input scope
        /// </summary>

        [ClassReference(IsReviewed = false)]


        public InputScopeNameValue InputScope
        {
            get { return (InputScopeNameValue)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputScope.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScopeNameValue), typeof(SfTimePicker), new PropertyMetadata(InputScopeNameValue.Default));

        /// <summary>
        /// Gets or sets the SelectorFormatString
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
            DependencyProperty.Register("SelectorFormatString", typeof(object), typeof(SfTimePicker), new PropertyMetadata("h:m:t", OnSelectorFormatStringChanged));
    

        /// <summary>
        /// Gets or sets the value of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> that hold the currently
        /// selected time.
        /// </summary>
        /// <value>
        /// The default value is <see cref="P:System.DateTime.Now">DateTime.Now</see>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.SetValueOnLostFocus"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
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
            DependencyProperty.Register("Value", typeof(object), typeof(SfTimePicker), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value><c>true</c> if this instance is drop down open; otherwise, <c>false</c>.</value>
        /// <summary>
        /// Gets or sets a value indicating whether the popup <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/> drop down is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.DropDownWidth"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.DropDownHeight"/>
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
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SfTimePicker), new PropertyMetadata(false,OnIsDropDownOpenChanged));



        /// <summary>
        /// Gets or sets the height of the drop down for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.DropDownWidth"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.IsDropDownOpen"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        [ClassReference(IsReviewed = false)]
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(SfTimePicker), new PropertyMetadata(0.0));


        /// <summary>
        /// Gets or sets the data that is used as a format for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
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
            DependencyProperty.Register("FormatString", typeof(string), typeof(SfTimePicker), new PropertyMetadata("hh:mm tt", OnFormatStringChanged));


        /// <summary>
        /// Gets or sets the style that apply for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>.
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
            DependencyProperty.Register("SelectorStyle", typeof(Style), typeof(SfTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether set <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.Value"/> on lost focus.
        /// </summary>
        /// <value>
        /// <c>true</c> if set value on lost focus; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        [ClassReference(IsReviewed = false)]
        public bool SetValueOnLostFocus
        {
            get { return (bool)GetValue(SetValueOnLostFocusProperty); }
            set { SetValue(SetValueOnLostFocusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SetValueOnLostFocus.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SetValueOnLostFocusProperty =
            DependencyProperty.Register("SetValueOnLostFocus", typeof(bool), typeof(SfTimePicker), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether AllowInlineEditing is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if set; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        public bool AllowInlineEditing
        {
            get { return (bool)GetValue(AllowInlineEditingProperty); }
            set { SetValue(AllowInlineEditingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanInlineEdit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowInlineEditingProperty =
            DependencyProperty.Register("AllowInlineEditing", typeof(bool), typeof(SfTimePicker), new PropertyMetadata(false, OnAllowInlineEditingChanged));

        /// <summary>
        /// Gets or sets the AllowNull for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfTimePicker"/> control.
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
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(SfTimePicker), new PropertyMetadata(false, OnAllowNullChanged));

        /// <summary>
        /// Gets or sets a value indicating whether ShowDropDownButton is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <value>
        /// <c>true</c> if set; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        public bool ShowDropDownButton
        {
            get { return (bool)GetValue(ShowDropDownButtonProperty); }
            set { SetValue(ShowDropDownButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowDropDownButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDropDownButtonProperty =
            DependencyProperty.Register("ShowDropDownButton", typeof(bool), typeof(SfTimePicker), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets the WaterMark property
        /// </summary>
        /// <value> 
        /// The default value is null
        /// </value>
        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(SfTimePicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a template for the WaterMark property
        /// </summary>
        /// <value> 
        /// The default value is null
        /// </value>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(SfTimePicker), new PropertyMetadata(null));

        


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
            DependencyProperty.Register("SelectorItemHeight", typeof(double), typeof(SfTimePicker), new PropertyMetadata(80));


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
            DependencyProperty.Register("SelectorItemWidth", typeof(double), typeof(SfTimePicker), new PropertyMetadata(80));


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
            DependencyProperty.Register("SelectorItemSpacing", typeof(double), typeof(SfTimePicker), new PropertyMetadata(4.0));


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
            DependencyProperty.Register("SelectorItemCount", typeof(int), typeof(SfTimePicker), new PropertyMetadata(0));

        #endregion

        #region Helper Methods

        void CheckInlineEdit()
        {
            if (AllowInlineEditing)
            {
                if (_partTextBoxExt != null)
                {
                    _partTextBoxExt.IsReadOnly = !AllowInlineEditing;
                    _partTextBoxExt.AllowPointerEvents = !AllowInlineEditing;
                    DateTime dateTime;
                    _partTextBoxExt.Text = Value != null && DateTime.TryParse(Value.ToString(), out dateTime) ? dateTime.ToString("hh:mm:ss:tt") : string.Empty;
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

        void PART_TextBoxExt_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!AllowInlineEditing)
                _partTextBoxExt.SelectionLength = 0;
        }

        void PART_TimeSelector_Select(object sender, RoutedEventArgs e)
        {
            if (_partTimeSelector != null)
            {
                Value = _partTimeSelector.SelectedTime;
                _partTimeSelector.CollapseAll();
                _partTimeSelector.UpdateData();
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
        }

        void PART_TextBoxExt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AllowInlineEditing)
            {
                DateTime datetime;
                var sfTextBoxExt = sender as SfTextBoxExt;
                Value = sfTextBoxExt != null && (DateTime.TryParse(sfTextBoxExt.Text, out datetime)) ? datetime : Value;
                if (Value != null)
                    _partTextBoxExt.Text = (DateTime.TryParse(Value.ToString(), out datetime)) ? datetime.ToString(FormatString) : string.Empty;
                else
                    _partTextBoxExt.Text = string.Empty;
            }
        }


        private void Cancelled(object sender, RoutedEventArgs e)
        {
            if (_partTimeSelector != null)
            {
                _partTimeSelector.SelectedTime = Value;
                _partTimeSelector.CollapseAll();
                _partTimeSelector.UpdateData();
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
        }

        private void OnStarted(object sender, object e)
        {
            if (_partTimeSelector != null)
            {
                _partTimeSelector.Focus(FocusState.Keyboard);
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
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                if (AllowInlineEditing)
                {
                    if (_partTextBoxExt != null)
                    {
                        if (AllowNull)
                            _partTextBoxExt.Text = string.Empty;
                        else
                            _partTextBoxExt.Text = DateTime.Now.ToString();
                        _partTimeSelector.SelectedTime = DateTime.Now;
                        _partTimeSelector.UpdateData();
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
                        _partTimeSelector.SelectedTime = DateTime.Now;
                        _partTimeSelector.UpdateData();
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
                        _partTimeSelector.SelectedTime = Value;
                        _partTimeSelector.UpdateData();
                    }
                }
                else
                {
                    if (_partTextBlock != null)
                    {
                        _partTextBlock.Text = datetime.ToString(FormatString);
                        _partTimeSelector.SelectedTime = Value;
                        _partTimeSelector.UpdateData();
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
            if (_partTimePickerPage != null)
            {
                _partTimePickerPage.RenderTransform = new CompositeTransform();
                _partTimePickerPage.RenderTransformOrigin = new Point(0.5, 0.5);

                Timeline timeline = BuildAnimation(from, to, TimeSpan.FromSeconds(0.1), new ExponentialEase { EasingMode = easingmode });
                Storyboard.SetTarget(timeline, _partTimePickerPage);
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

        private void DropDownOpen()
        {
            if (_partTimeSelector != null)
            {
                _partTimeSelector.SelectedTime = Value;
                _partTimeSelector.CollapseAll();
                _partTimeSelector.UpdateData();
            }
        }

        private void ValidatePopupPosition()
        {
            double top;
            double left = 0.0;
            var windowBounds = Window.Current.CoreWindow.Bounds;
            double selectorheight = _partTimeSelector != null ? _partTimeSelector.UpdateSelectorHeight() : 0;
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
                _partPopup.VerticalOffset = top;
                _partPopup.HorizontalOffset = left;
                if (_partTimeSelector != null)
                {
                    if (_partTimeSelector.ActualWidth + rect.X > windowBounds.Width)
                    {
                        left = -(_partTimeSelector.ActualWidth / 2 - ActualWidth / 2);
                        _partPopup.HorizontalOffset = -(rect.X + left) + left;
                    }
                }
                if (rect.X + left < windowBounds.X)
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
                if (rect.Y + top < windowBounds.Y)
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

                if (_partTimeSelector != null)
                {
                    _partTimeSelector.Focus(FocusState.Keyboard);
                }
            }
        }
        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes all te child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            _partTextBoxExt = GetTemplateChild("PART_TextBoxExt") as SfTextBoxExt;
            _partPopup = GetTemplateChild("PART_DropDown") as Popup;
            _partTimePickerPage = GetTemplateChild("PART_TimePickerPage") as FrameworkElement;
            _partTimeSelector = GetTemplateChild("PART_TimeSelector") as SfTimeSelector;
            _partDropDownButton = GetTemplateChild("PART_DropDownButton") as RepeatButton;
            _partTextBlock = GetTemplateChild("PART_TextBlock") as SfTextBoxExt;

            if (_partTextBoxExt != null)
            {
                FormatValue(Value);
                _partTextBoxExt.LostFocus += PART_TextBoxExt_LostFocus;
                _partTextBoxExt.SelectionChanged += PART_TextBoxExt_SelectionChanged;
            }

            if (_partTimeSelector != null)
            {
                Binding binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorFormatString");
                binding.Mode = BindingMode.TwoWay;
                _partTimeSelector.SetBinding(SfTimeSelector.FormatStringProperty, binding);


                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemHeight");
                binding.Mode = BindingMode.TwoWay;
                _partTimeSelector.SetBinding(SfTimeSelector.SelectorItemHeightProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemWidth");
                binding.Mode = BindingMode.TwoWay;
                _partTimeSelector.SetBinding(SfTimeSelector.SelectorItemWidthProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemSpacing");
                binding.Mode = BindingMode.TwoWay;
                _partTimeSelector.SetBinding(SfTimeSelector.SelectorItemSpacingProperty, binding);

                binding = new Binding();
                binding.Source = this;
                binding.Path = new PropertyPath("SelectorItemCount");
                binding.Mode = BindingMode.TwoWay;
                _partTimeSelector.SetBinding(SfTimeSelector.SelectorItemCountProperty, binding);
                
                _partTimeSelector.Cancel += Cancelled;
                _partTimeSelector.Select += PART_TimeSelector_Select;
                _partTimeSelector.LostFocus += PART_TimeSelector_LostFocus;
                _partTimeSelector.Loaded += PART_TimeSelector_Loaded;
                _partTimeSelector.KeyDown += PART_TimeSelector_KeyDown;
            }
            if (_partDropDownButton != null)
            {
                _partDropDownButton.Click += PART_DropDownButton_Click;
            }
            CheckInlineEdit();
            base.OnApplyTemplate();
        }

        void PART_TimeSelector_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                if (_partTimeSelector != null)
                {
                    _partTimeSelector.SelectedTime = Value;
                    _partTimeSelector.CollapseAll();
                    _partTimeSelector.UpdateData();
                }
                Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
            else if (e.Key == Windows.System.VirtualKey.Enter && IsDropDownOpen)
            {
                if (_partTimeSelector != null)
                {
                    Value = _partTimeSelector.SelectedTime;
                    _partTimeSelector.CollapseAll();
                    _partTimeSelector.UpdateData();
                }
                Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
        }

        void PART_TimeSelector_Loaded(object sender, RoutedEventArgs e)
        {
            if (_partTimeSelector.PartSelectButton != null)
                VisualStateManager.GoToState(_partTimeSelector.PartSelectButton, "Normal", false);
            if (_partTimeSelector.PartCancelButton != null)
                VisualStateManager.GoToState(_partTimeSelector.PartCancelButton, "Normal", false);
            _partTimeSelector.UpdateLayout();
            if (_partTimeSelector.ActualWidth < ActualWidth)
            {
                _partTimeSelector.MinWidth = ActualWidth;
            }
            ValidatePopupPosition();
        }

        void PART_TimeSelector_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(FocusManager.GetFocusedElement() is LoopingSelector))
            {
                if (!(FocusManager.GetFocusedElement() is SfTimeSelector))
                {
                    ClosePickerPage();
                }
            }
        }

        private void ClosePickerPage()
        {
            var element = FocusManager.GetFocusedElement() as FrameworkElement;
            if (element == _partTimeSelector.PartSelectButton ||
                element == _partTimeSelector.PartCancelButton)
            {
                return;
            }
            if (_partTimeSelector != null && IsDropDownOpen)
            {
                _partTimeSelector.CollapseAll();
                if (SetValueOnLostFocus)
                {
                    Value = _partTimeSelector.SelectedTime;
                }
                else
                {
                    _partTimeSelector.SelectedTime = Value;
                }
                _partTimeSelector.UpdateData();
            }
            Animate(1, 0, EasingMode.EaseIn, OnCompleted);
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
        /// Invoked when the pointer is released.
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
        /// Invoked when a key is pressed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space) 
            {
                if (!IsDropDownOpen && !AllowInlineEditing)
                {
                    e.Handled = true;
                    IsDropDownOpen = true;
                    Animate(0, 1, EasingMode.EaseOut, null);

                }
                else
                {
                    if (_partTimeSelector != null)
                    {
                        if (_partTextBoxExt != null && !string.IsNullOrEmpty(_partTextBoxExt.Text))
                            Value = _partTextBoxExt.Text.ToDateTime();
                        else
                            Value = _partTimeSelector.SelectedTime;
                        _partTimeSelector.CollapseAll();
                        _partTimeSelector.UpdateData();
                    }
                    Animate(1, 0, EasingMode.EaseIn, OnCompleted);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                if (_partTimeSelector != null)
                {
                    _partTimeSelector.SelectedTime = Value;
                    _partTimeSelector.CollapseAll();
                    _partTimeSelector.UpdateData();
                }
                Animate(1, 0, EasingMode.EaseIn, OnCompleted);
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Callback Methods


        private static void OnAllowInlineEditingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var  instance = obj as SfTimePicker;
            if (instance != null) instance.OnAllowInlineEditing(args);
        }

        /// <summary>
        /// Occurs when AllowInlineEditing is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        protected virtual void OnAllowInlineEditing(DependencyPropertyChangedEventArgs args)
        {
            CheckInlineEdit();
        }

        private static void OnSelectorFormatStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var selector = sender as SfTimePicker;
            if (selector != null)
            {
                selector.OnSelectorFormatStringChanged(args);
            }
        }

        /// <summary>
        /// Occurs when SelectorFormatString is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        protected virtual void OnSelectorFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                string format = args.NewValue.ToString().ToLower();
                if (format.Contains("h") || format.Contains("m") || format.Contains("t"))
                {
                    //if (_partTimeSelector != null)
                    //    _partTimeSelector.FormatString = args.NewValue;
                }
                else
                {
                    throw new InvalidCastException("Selector FormatString must contain h/m/t");
                }
            }
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var picker = sender as SfTimePicker;
            if (picker != null)
            {
                picker.FormatValue(picker.Value); 
                bool canexecute = args.NewValue != null ? args.NewValue.Equals(picker.Value) : picker.Value == null ? true : false;
                if (canexecute && picker.ValueChanged != null)
                {
                    picker.ValueChanged(sender, args);
                }
            }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfTimePicker;
            if (instance != null) instance.OnIsDropDownOpenChanged(args);
        }

        private static void OnAllowNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfTimePicker;
            if (instance != null && instance.Value == null && !(bool)args.NewValue)
                instance.Value = DateTime.Now;
        }
        /// <summary>
        /// Occurs when IsDropDownOpen is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (IsDropDownOpen)
                {
                    DropDownOpen();
                    Animate(0, 1, EasingMode.EaseOut, OnStarted);
                }
                else
                {
                    Animate(1, 0, EasingMode.EaseIn, OnCompleted);
                }
            }
        }

        private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = obj as SfTimePicker;
            if (instance != null) instance.OnFormatStringChanged(args);
        }

        /// <summary>
        /// Occurs when FormatString is set in <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> control.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TimeSelector"/>
        protected virtual void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            FormatValue(Value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the value for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TimePicker"/> changed.
        /// </summary>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.TimePicker.Value"/>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback ValueChanged;

        #endregion

        /// <summary>
        /// Validates the value
        /// </summary>
        /// <param name="args"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Validate(ValidationEventArgs args)
        {
            VisualStateManager.GoToState(this, args.HasError ? "HasError" : "NoError", true);
        }
    }
}
