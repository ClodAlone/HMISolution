#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    /// <summary>
    ///
    /// </summary>
    public class DateTimeBase : TextBox
    {
        #region Events

        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        /// <summary>
        /// Event that is raised when UnderlyingDateTime property is changed.
        /// </summary>
        public event PropertyChangedCallback UnderlyingDateTimeChanged;

        /// <summary>
        /// Event that is raised when IsScrollingOnCircle property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback IsScrollingOnCircleChanged;

        /// <summary>
        /// Event that is raised when CustomPattern property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomPatternChanged;

        /// <summary>
        /// Event that is raised when Pattern property is changed.
        /// </summary>
        public event PropertyChangedCallback PatternChanged;

#if WPF

        public event PropertyChangedCallback IsWatchEnabledChanged;

#endif

        //public event PropertyChangedCallback ShowNoDateTimeChanged;

        /// <summary>
        /// Event that is raised when IsEmptyDateEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback IsEmptyDateEnabledChanged;

        /// <summary>
        /// Event that is raised when IsButtonPopUpEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback IsButtonPopUpEnabledChanged;

        /// <summary>
        /// Event that is raised when IsCalendarEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback IsCalendarEnabledChanged;

        /// <summary>
        /// Event that is raised when IsVisibleRepeatButton property is changed.
        /// </summary>
        public event PropertyChangedCallback IsVisibleRepeatButtonChanged;

        /// <summary>
        /// Event that is raised when RepeatButtonBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback RepeatButtonBackgroundChanged;

        /// <summary>
        /// Event that is raised when RepeatButtonBorderBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback RepeatButtonBorderBrushChanged;

        /// <summary>
        /// Event that is raised when RepeatButtonBorderThickness property is changed
        /// </summary>
        public event PropertyChangedCallback RepeatButtonBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when UpRepeatButtonMargin property is changed
        /// </summary>
        public event PropertyChangedCallback UpRepeatButtonMarginChanged;

        /// <summary>
        /// Event that is raised when DownRepeatButtonMargin property is changed
        /// </summary>
        public event PropertyChangedCallback DownRepeatButtonMarginChanged;

        /// <summary>
        /// Event that is raised when UpRepeatButtonTemplate property is changed
        /// </summary>
        public event PropertyChangedCallback UpRepeatButtonTemplateChanged;

        /// <summary>
        /// Event that is raised when DownRepeatButtonTemplate property is changed
        /// </summary>
        public event PropertyChangedCallback DownRepeatButtonTemplateChanged;

        #endregion Events

        /// <summary>
        ///
        /// </summary>
        public DateTimeBase()
        {

        }

#if WPF

        /// <summary>
        /// Called whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = true;
            base.OnContextMenuOpening(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.DragDrop.DragEnter"/> attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Handled = true;
            base.OnDragEnter(e);
        }

#endif

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        [Obsolete("Use IsReadOnly Property")]
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        #region Events

        /// <summary>
        /// Event that is raised when CultureInfo property is changed.
        /// </summary>
        public event PropertyChangedCallback CultureInfoChanged;//CultureInfoChanged;

        #endregion Events

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public string CustomPattern
        {
            get { return (string)GetValue(CustomPatternProperty); }
            set { SetValue(CustomPatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return (CultureInfo)GetValue(CultureInfoProperty); }
            set { SetValue(CultureInfoProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTimeFormatInfo DateTimeFormat
        {
            get { return (DateTimeFormatInfo)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string NoneDateText
        {
            get { return (string)GetValue(NoneDateTextProperty); }
            set { SetValue(NoneDateTextProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get { return (bool)GetValue(IsScrollingOnCircleProperty); }
            set { SetValue(IsScrollingOnCircleProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTimePattern Pattern
        {
            get { return (DateTimePattern)GetValue(PatternProperty); }
            set { SetValue(PatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public string MaskedText
        {
            get { return (string)GetValue(MaskedTextProperty); }
            set
            {
                SetValue(TextProperty, value);
                SetValue(MaskedTextProperty, value);
            }
        }

        #endregion Properties

        #region DependencyProperties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is Can Edit the datetime.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set
            {
                SetValue(CanEditProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for CanEdit.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.Register("CanEdit", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CustomPatternProperty =
            DependencyProperty.Register("CustomPattern", typeof(string), typeof(DateTimeBase), new PropertyMetadata(string.Empty, OnCustomPatternChanged));

        /// <summary>
        /// Called when [custom pattern changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCustomPatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnCustomPatternChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CustomPatternChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnCustomPatternChanged(DependencyPropertyChangedEventArgs args)
        {
            this.BasePropertiesChanged();
            if (CustomPatternChanged != null)
            {
                CustomPatternChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(DateTimeFormatInfo), typeof(DateTimeBase), new PropertyMetadata(null, OnDateTimeFormatChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaskedTextProperty =
            DependencyProperty.Register("MaskedText", typeof(string), typeof(DateTimeBase), new PropertyMetadata(string.Empty));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CultureInfoProperty =
            DependencyProperty.Register("CultureInfo", typeof(CultureInfo), typeof(DateTimeBase), new PropertyMetadata(CultureInfo.CurrentCulture, new PropertyChangedCallback(OnCultureChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NoneDateTextProperty =
            DependencyProperty.Register("NoneDateText", typeof(string), typeof(DateTimeBase), new PropertyMetadata("No date is selected"));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsScrollingOnCircleProperty =
            DependencyProperty.Register("IsScrollingOnCircle", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true, OnIsScrollingOnCircleChanged));

        /// <summary>
        /// Called when [is scrolling on circle changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsScrollingOnCircleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsScrollingOnCircleChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsScrollingOnCircleChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsScrollingOnCircleChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsScrollingOnCircleChanged != null)
            {
                IsScrollingOnCircleChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IncorrectForegroundProperty =
            DependencyProperty.Register("IncorrectForeground", typeof(Brush), typeof(DateTimeBase), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PatternProperty =
            DependencyProperty.Register("Pattern", typeof(DateTimePattern), typeof(DateTimeBase), new PropertyMetadata(DateTimePattern.ShortDate, new PropertyChangedCallback(OnPatternChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsEmptyDateEnabledProperty =
            DependencyProperty.Register("IsEmptyDateEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false, OnIsEmptyDateEnabledChanged));

        /// <summary>
        /// Called when [is empty date enabled changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsEmptyDateEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsEmptyDateEnabledChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsEmptyDateEnabledChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsEmptyDateEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsEmptyDateEnabledChanged != null)
            {
                IsEmptyDateEnabledChanged(this, args);
            }
            this.BasePropertiesChanged();
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// Called when [is drop down open changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsDropDownOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;

            d.CheckPopUpStatus(d);

            if (d != null)
            {
                d.OnIsDropDownOpenChanged(args);
            }
        }

        /// <summary>
        /// Checks the pop up status.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void CheckPopUpStatus(DependencyObject obj)
        {
            if (obj is DateTimeEdit)
            {
                DateTimeEdit edit = obj as DateTimeEdit;

                if (this.IsDropDownOpen)
                    edit.OpenPopup();
                else
                    edit.ClosePopup();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsDropDownOpenChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs args)
        {
            IsPopupEnabled = IsDropDownOpen;

            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsEnabledRepeatButtonProperty =
            DependencyProperty.Register("IsEnabledRepeatButton", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsVisibleRepeatButtonProperty =
            DependencyProperty.Register("IsVisibleRepeatButton", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false, OnIsVisibleRepeatButtonChanged));

        /// <summary>
        /// Called when [is visible repeat button changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsVisibleRepeatButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsVisibleRepeatButtonChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsVisibleRepeatButtonChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsVisibleRepeatButtonChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsVisibleRepeatButtonChanged != null)
            {
                IsVisibleRepeatButtonChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsPopupEnabledProperty =
            DependencyProperty.Register("IsPopupEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsButtonPopUpEnabledProperty =
            DependencyProperty.Register("IsButtonPopUpEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true, OnIsButtonPopUpEnabledChanged));

        /// <summary>
        /// Called when [is button pop up enabled changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsButtonPopUpEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsButtonPopUpEnabledChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsButtonPopUpEnabledChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsButtonPopUpEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsButtonPopUpEnabledChanged != null)
            {
                IsButtonPopUpEnabledChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsCalendarEnabledProperty =
            DependencyProperty.Register("IsCalendarEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true, OnIsCalendarEnabledChanged));

        /// <summary>
        /// Called when [is calendar enabled changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsCalendarEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsCalendarEnabledChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsCalendarEnabledChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsCalendarEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsCalendarEnabledChanged != null)
            {
                IsCalendarEnabledChanged(this, args);
            }
        }

        #endregion DependencyProperties

        #region Obsolete

        /// <summary>
        /// Gets or sets the underlying date time.
        /// </summary>
        /// <value>The underlying date time.</value>
        // [Obsolete("Use DateTime Property")]
        internal DateTime? UnderlyingDateTime
        {
            get { return (DateTime?)GetValue(UnderlyingDateTimeProperty); }
            set { SetValue(UnderlyingDateTimeProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UnderlyingDateTimeProperty =
            DependencyProperty.Register("UnderlyingDateTime", typeof(DateTime?), typeof(DateTimeBase), new PropertyMetadata(null, OnUnderlyingDateTimeChanged));

        /// <summary>
        /// Called when [underlying date time changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnUnderlyingDateTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnUnderlyingDateTimeChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:UnderlyingDateTimeChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnUnderlyingDateTimeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (UnderlyingDateTimeChanged != null)
            {
                UnderlyingDateTimeChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the correct foreground.
        /// </summary>
        /// <value>The correct foreground.</value>
        [Obsolete("Use Foreground Property")]
        public Brush CorrectForeground
        {
            get { return (Brush)GetValue(CorrectForegroundProperty); }
            set
            {
                //SetValue(ForegroundProperty, value);
                SetValue(CorrectForegroundProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CorrectForegroundProperty =
            DependencyProperty.Register("CorrectForeground", typeof(Brush), typeof(DateTimeBase), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is culture right to left.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is culture right to left; otherwise, <c>false</c>.
        /// </value>
        public bool IsCultureRightToLeft
        {
            get { return (bool)GetValue(IsCultureRightToLeftProperty); }
            set
            {
                // SetValue(FlowDirectionProperty, value);
                SetValue(IsCultureRightToLeftProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hold max width.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hold max width; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsHoldMaxWidth
        {
            get { return (bool)GetValue(IsHoldMaxWidthProperty); }
            set { SetValue(IsHoldMaxWidthProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsHoldMaxWidthProperty =
            DependencyProperty.Register("IsHoldMaxWidth", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsCultureRightToLeftProperty =
            DependencyProperty.Register("IsCultureRightToLeft", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto correct.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is auto correct; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsAutoCorrect
        {
            get { return (bool)GetValue(IsAutoCorrectProperty); }
            set { SetValue(IsAutoCorrectProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsAutoCorrectProperty =
            DependencyProperty.Register("IsAutoCorrect", typeof(bool), typeof(DateTimeEdit), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the uncertain foreground.
        /// </summary>
        /// <value>The uncertain foreground.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush UncertainForeground
        {
            get { return (Brush)GetValue(UncertainForegroundProperty); }
            set { SetValue(UncertainForegroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UncertainForegroundProperty =
            DependencyProperty.Register("UncertainForeground", typeof(Brush), typeof(DateTimeBase), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        /// Gets or sets the date validation mode.
        /// </summary>
        /// <value>The date validation mode.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public DateValidationMode DateValidationMode
        {
            get { return (DateValidationMode)GetValue(DateValidationModeProperty); }
            set { SetValue(DateValidationModeProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DateValidationModeProperty =
            DependencyProperty.Register("DateValidationMode", typeof(DateValidationMode), typeof(DateTimeBase), new PropertyMetadata(DateValidationMode.Warning));

        /// <summary>
        /// Gets or sets the duration of the auto corrected higlight.
        /// </summary>
        /// <value>The duration of the auto corrected higlight.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public double AutoCorrectedHiglightDuration
        {
            get { return (double)GetValue(AutoCorrectedHiglightDurationProperty); }
            set { SetValue(AutoCorrectedHiglightDurationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty AutoCorrectedHiglightDurationProperty =
            DependencyProperty.Register("AutoCorrectedHiglightDuration", typeof(double), typeof(DateTimeBase), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is editable; otherwise, <c>false</c>.
        /// </value>
        // [Obsolete("Use IsReadonly Property")]
        internal bool IsEditable
        {
            get
            {
                return (bool)GetValue(IsEditableProperty);
            }
            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
             DependencyProperty.Register("IsEditable", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true, new PropertyChangedCallback(OnIsEditableChanged)));

        /// <summary>
        /// Called when [is editable changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeBase obj = (DateTimeBase)d;
            if (obj != null)
            {
                obj.OnIsEditableChangedHanlde(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsEditableChangedHanlde"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnIsEditableChangedHanlde(DependencyPropertyChangedEventArgs e)
        {
            if (IsEditable == true)
                IsReadOnly = false;
            else if (IsEditable == false)
                IsReadOnly = true;
        }

        /// <summary>
        /// Gets or sets the caret template.
        /// </summary>
        /// <value>The caret template.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public ControlTemplate CaretTemplate
        {
            get { return (ControlTemplate)GetValue(CaretTemplateProperty); }
            set { SetValue(CaretTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CaretTemplateProperty =
            DependencyProperty.Register("CaretTemplate", typeof(ControlTemplate), typeof(DateTimeBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is caret animation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is caret animation; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsCaretAnimation
        {
            get { return (bool)GetValue(IsCaretAnimationProperty); }
            set { SetValue(IsCaretAnimationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsCaretAnimationProperty =
            DependencyProperty.Register("IsCaretAnimation", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsAnimation
        {
            get { return (bool)GetValue(IsAnimationProperty); }
            set { SetValue(IsAnimationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the duration of the scroll.
        /// </summary>
        /// <value>The duration of the scroll.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public double ScrollDuration
        {
            get { return (double)GetValue(ScrollDurationProperty); }
            set { SetValue(ScrollDurationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ScrollDurationProperty =
            DependencyProperty.Register("ScrollDuration", typeof(double), typeof(DateTimeBase), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mask input enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mask input enabled; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsMaskInputEnabled
        {
            get { return (bool)GetValue(IsMaskInputEnabledProperty); }
            set { SetValue(IsMaskInputEnabledProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsMaskInputEnabledProperty =
            DependencyProperty.Register("IsMaskInputEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the incorrect foreground.
        /// </summary>
        /// <value>The incorrect foreground.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush IncorrectForeground
        {
            get { return (Brush)GetValue(IncorrectForegroundProperty); }
            set { SetValue(IncorrectForegroundProperty, value); }
        }

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        /// <summary>
        /// Gets or sets the repeat button border brush.
        /// </summary>
        /// <value>The repeat button border brush.</value>
        public Brush RepeatButtonBorderBrush
        {
            get { return (Brush)GetValue(RepeatButtonBorderBrushProperty); }
            set { SetValue(RepeatButtonBorderBrushProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty RepeatButtonBorderBrushProperty =
            DependencyProperty.Register("RepeatButtonBorderBrush", typeof(Brush), typeof(DateTimeBase), new PropertyMetadata(OnRepeatButtonBorderBrushChanged));

        /// <summary>
        /// Called when [repeat button border brush changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnRepeatButtonBorderBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnRepeatButtonBorderBrushChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RepeatButtonBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnRepeatButtonBorderBrushChanged(DependencyPropertyChangedEventArgs args)
        {
            if (RepeatButtonBorderBrushChanged != null)
            {
                RepeatButtonBorderBrushChanged(this, args);
            }
        }

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        /// <summary>
        /// Gets or sets the repeat button background.
        /// </summary>
        /// <value>The repeat button background.</value>
        public Brush RepeatButtonBackground
        {
            get { return (Brush)GetValue(RepeatButtonBackgroundProperty); }
            set { SetValue(RepeatButtonBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty RepeatButtonBackgroundProperty =
            DependencyProperty.Register("RepeatButtonBackground", typeof(Brush), typeof(DateTimeBase), new PropertyMetadata(OnRepeatButtonBackgroundChanged));

        /// <summary>
        /// Called when [repeat button background changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnRepeatButtonBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnRepeatButtonBackgroundChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RepeatButtonBackgroundChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnRepeatButtonBackgroundChanged(DependencyPropertyChangedEventArgs args)
        {
            if (RepeatButtonBackgroundChanged != null)
            {
                RepeatButtonBackgroundChanged(this, args);
            }
        }

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        /// <summary>
        /// Gets or sets the repeat button border thickness.
        /// </summary>
        /// <value>The repeat button border thickness.</value>
        public Thickness RepeatButtonBorderThickness
        {
            get { return (Thickness)GetValue(RepeatButtonBorderThicknessProperty); }
            set { SetValue(RepeatButtonBorderThicknessProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty RepeatButtonBorderThicknessProperty =
            DependencyProperty.Register("RepeatButtonBorderThickness", typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(OnRepeatButtonBorderThicknessChanged));

        /// <summary>
        /// Called when [repeat button border thickness changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnRepeatButtonBorderThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnRepeatButtonBorderThicknessChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RepeatButtonBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnRepeatButtonBorderThicknessChanged(DependencyPropertyChangedEventArgs args)
        {
            if (RepeatButtonBorderThicknessChanged != null)
            {
                RepeatButtonBorderThicknessChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets down repeat button content template.
        /// </summary>
        /// <value>Down repeat button content template.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public DataTemplate DownRepeatButtonContentTemplate
        {
            get { return (DataTemplate)GetValue(DownRepeatButtonContentTemplateProperty); }
            set { SetValue(DownRepeatButtonContentTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DownRepeatButtonContentTemplateProperty =
            DependencyProperty.Register("DownRepeatButtonContentTemplate", typeof(DataTemplate), typeof(DateTimeBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets up repeat button content template.
        /// </summary>
        /// <value>Up repeat button content template.</value>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public DataTemplate UpRepeatButtonContentTemplate
        {
            get { return (DataTemplate)GetValue(UpRepeatButtonContentTemplateProperty); }
            set { SetValue(UpRepeatButtonContentTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UpRepeatButtonContentTemplateProperty =
            DependencyProperty.Register("UpRepeatButtonContentTemplate", typeof(DataTemplate), typeof(DateTimeBase), new PropertyMetadata(null));

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        /// <summary>
        /// Gets or sets up repeat button margin.
        /// </summary>
        /// <value>Up repeat button margin.</value>
        public Thickness UpRepeatButtonMargin
        {
            get { return (Thickness)GetValue(UpRepeatButtonMarginProperty); }
            set { SetValue(UpRepeatButtonMarginProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UpRepeatButtonMarginProperty =
            DependencyProperty.Register("UpRepeatButtonMargin", typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(new Thickness(), OnUpRepeatButtonMarginChanged));

        /// <summary>
        /// Called when [up repeat button margin changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnUpRepeatButtonMarginChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnUpRepeatButtonMarginChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:UpRepeatButtonMarginChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnUpRepeatButtonMarginChanged(DependencyPropertyChangedEventArgs args)
        {
            if (UpRepeatButtonMarginChanged != null)
            {
                UpRepeatButtonMarginChanged(this, args);
            }
        }

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        /// <summary>
        /// Gets or sets down repeat button margin.
        /// </summary>
        /// <value>Down repeat button margin.</value>
        public Thickness DownRepeatButtonMargin
        {
            get { return (Thickness)GetValue(DownRepeatButtonMarginProperty); }
            set { SetValue(DownRepeatButtonMarginProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DownRepeatButtonMarginProperty =
            DependencyProperty.Register("DownRepeatButtonMargin", typeof(Thickness), typeof(DateTimeBase), new PropertyMetadata(new Thickness(), OnDownRepeatButtonMarginChanged));

        /// <summary>
        /// Called when [down repeat button margin changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDownRepeatButtonMarginChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnDownRepeatButtonMarginChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DownRepeatButtonMarginChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDownRepeatButtonMarginChanged(DependencyPropertyChangedEventArgs args)
        {
            if (DownRepeatButtonMarginChanged != null)
            {
                DownRepeatButtonMarginChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [select whole content].
        /// </summary>
        /// <value><c>true</c> if [select whole content]; otherwise, <c>false</c>.</value>
        public bool SelectWholeContent
        {
            get { return (bool)GetValue(SelectWholeContentProperty); }
            set
            {
                SetValue(SelectWholeContentProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectWholeContentProperty =
             DependencyProperty.Register("SelectWholeContent", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the popup delay.
        /// </summary>
        /// <value>The popup delay.</value>
        public TimeSpan PopupDelay
        {
            get { return (TimeSpan)GetValue(PopupDelayProperty); }
            set { SetValue(PopupDelayProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PopupDelayProperty =
            DependencyProperty.Register("PopupDelay", typeof(TimeSpan), typeof(DateTimeBase), new PropertyMetadata(new TimeSpan()));

        /// <summary>
        /// Gets or sets up repeat button template.
        /// </summary>
        /// <value>Up repeat button template.</value>
        public ControlTemplate UpRepeatButtonTemplate
        {
            get { return (ControlTemplate)GetValue(UpRepeatButtonTemplateProperty); }
            set { SetValue(UpRepeatButtonTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UpRepeatButtonTemplateProperty =
            DependencyProperty.Register("UpRepeatButtonTemplate", typeof(ControlTemplate), typeof(DateTimeBase), new PropertyMetadata(OnUpRepeatButtonTemplateChanged));

        /// <summary>
        /// Called when [up repeat button template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnUpRepeatButtonTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnUpRepeatButtonTemplateChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:UpRepeatButtonTemplateChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnUpRepeatButtonTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (UpRepeatButtonTemplateChanged != null)
            {
                UpRepeatButtonTemplateChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets down repeat button template.
        /// </summary>
        /// <value>Down repeat button template.</value>
        public ControlTemplate DownRepeatButtonTemplate
        {
            get { return (ControlTemplate)GetValue(DownRepeatButtonTemplateProperty); }
            set { SetValue(DownRepeatButtonTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DownRepeatButtonTemplateProperty =
            DependencyProperty.Register("DownRepeatButtonTemplate", typeof(ControlTemplate), typeof(DateTimeBase), new PropertyMetadata(OnDownRepeatButtonTemplateChanged));

        /// <summary>
        /// Called when [down repeat button template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDownRepeatButtonTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnDownRepeatButtonTemplateChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DownRepeatButtonTemplateChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDownRepeatButtonTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (DownRepeatButtonTemplateChanged != null)
            {
                DownRepeatButtonTemplateChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the drop down button template.
        /// </summary>
        /// <value>The drop down button template.</value>
        public ControlTemplate DropDownButtonTemplate
        {
            get { return (ControlTemplate)GetValue(DropDownButtonTemplateProperty); }
            set { SetValue(DropDownButtonTemplateProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DropDownButtonTemplateProperty =
            DependencyProperty.Register("DropDownButtonTemplate", typeof(ControlTemplate), typeof(DateTimeBase), new PropertyMetadata(OnDropDownButtonTemplateChanged));

        /// <summary>
        /// Called when [drop down button template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDropDownButtonTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnDropDownButtonTemplateChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DropDownButtonTemplateChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDropDownButtonTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        #endregion Obsolete

        #region PropertyChanged Callbacks

        /// <summary>
        /// Called when [culture changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCultureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DateTimeBase)obj != null)
            {
                ((DateTimeBase)obj).OnCultureChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CultureChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnCultureChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CultureInfoChanged != null)
                this.CultureInfoChanged(this, args);
            this.BasePropertiesChanged();
        }

        /// <summary>
        /// Called when [pattern changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnPatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DateTimeBase)obj != null)
            {
                ((DateTimeBase)obj).OnPatternChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PatternChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPatternChanged(DependencyPropertyChangedEventArgs args)
        {
            if (PatternChanged != null)
            {
                PatternChanged(this, args);
            }
            this.BasePropertiesChanged();
        }

        /// <summary>
        /// Called when [date time format changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDateTimeFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DateTimeBase)obj != null)
            {
                ((DateTimeBase)obj).OnDateTimeFormatChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DateTimeFormatChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDateTimeFormatChanged(DependencyPropertyChangedEventArgs args)
        {
            this.BasePropertiesChanged();
        }

        #endregion PropertyChanged Callbacks

        /// <summary>
        /// Bases the properties changed.
        /// </summary>
        protected virtual void BasePropertiesChanged()
        {
        }

        #region Popup

        /// <summary>
        /// Gets or sets the content element visibility.
        /// </summary>
        /// <value>The content element visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility ContentElementVisibility
        {
            get { return (Visibility)GetValue(ContentElementVisibilityProperty); }
            set { SetValue(ContentElementVisibilityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ContentElementVisibilityProperty =
            DependencyProperty.Register("ContentElementVisibility", typeof(Visibility), typeof(DateTimeBase), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the watermark visibility.
        /// </summary>
        /// <value>The watermark visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility WatermarkVisibility
        {
            get { return (Visibility)GetValue(WatermarkVisibilityProperty); }
            set
            {
                //value = CoerceWatermarkVisibility(this, value);
                SetValue(WatermarkVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Coerces the watermark visibility.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceWatermarkVisibility(DependencyObject d, object baseValue)
        {
            DateTimeBase editorBase = (DateTimeBase)d;

            if (editorBase.IsEmptyDateEnabled && (((Visibility)baseValue) == Visibility.Visible))
            {
                editorBase.ContentElementVisibility = Visibility.Collapsed;
                return Visibility.Visible;
            }
            else
            {
                editorBase.ContentElementVisibility = Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

#if WPF

        public static readonly DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(DateTimeBase), new PropertyMetadata(Visibility.Collapsed, OnWatermarkVisibilityChanged, new CoerceValueCallback(CoerceWatermarkVisibility)));

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(DateTimeBase), new PropertyMetadata(Visibility.Collapsed, OnWatermarkVisibilityChanged));
#endif

        /// <summary>
        /// Called when [watermark visibility changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnWatermarkVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnWatermarkVisibilityChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WatermarkVisibilityChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnWatermarkVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
            object coerceval = CoerceWatermarkVisibility(this, this.WatermarkVisibility);
            if (this.WatermarkVisibility != (Visibility)coerceval)
            {
                this.WatermarkVisibility = (Visibility)coerceval;
                return;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty date enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is empty date enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmptyDateEnabled
        {
            get { return (bool)GetValue(IsEmptyDateEnabledProperty); }
            set { SetValue(IsEmptyDateEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        //[Obsolete("Use IsPopupEnabled Property")]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled repeat button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled repeat button; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledRepeatButton
        {
            get { return (bool)GetValue(IsEnabledRepeatButtonProperty); }
            set { SetValue(IsEnabledRepeatButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is popup enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is popup enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPopupEnabled
        {
            get { return (bool)GetValue(IsPopupEnabledProperty); }
            set { SetValue(IsPopupEnabledProperty, value); }
        }

#if WPF

        /// <summary>
        /// Gets or sets a value indicating whether this instance is watch enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is watch enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsWatchEnabled
        {
            get { return (bool)GetValue(IsWatchEnabledProperty); }
            set { SetValue(IsWatchEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsWatchEnabledProperty =
            DependencyProperty.Register("IsWatchEnabled", typeof(bool), typeof(DateTimeBase), new PropertyMetadata(true, OnIsWatchEnabledChanged));

        /// <summary>
        /// Called when [is watch enabled changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsWatchEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTimeBase d = (DateTimeBase)obj;
            if (d != null)
            {
                d.OnIsWatchEnabledChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsWatchEnabledChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIsWatchEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsWatchEnabledChanged != null)
            {
                IsWatchEnabledChanged(this, args);
            }
        }

#endif

        /// <summary>
        /// Gets or sets a value indicating whether this instance is calendar enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is calendar enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCalendarEnabled
        {
            get { return (bool)GetValue(IsCalendarEnabledProperty); }
            set { SetValue(IsCalendarEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible repeat button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible repeat button; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisibleRepeatButton
        {
            get { return (bool)GetValue(IsVisibleRepeatButtonProperty); }
            set { SetValue(IsVisibleRepeatButtonProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is button pop up enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is button pop up enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsButtonPopUpEnabled
        {
            get { return (bool)GetValue(IsButtonPopUpEnabledProperty); }
            set { SetValue(IsButtonPopUpEnabledProperty, value); }
        }

        #endregion Popup

        #region Methods

        /// <summary>
        /// Gets the string pattern.
        /// </summary>
        /// <param name="dateTimeFormatInfo">The date time format info.</param>
        /// <param name="dateTimePattern">The date time pattern.</param>
        /// <param name="customPattern">The custom pattern.</param>
        /// <returns></returns>
        internal string GetStringPattern(DateTimeFormatInfo dateTimeFormatInfo, DateTimePattern dateTimePattern, string customPattern)
        {
            switch (dateTimePattern)
            {
                case DateTimePattern.ShortDate:
                    return dateTimeFormatInfo.ShortDatePattern;

                case DateTimePattern.LongDate:
                    return dateTimeFormatInfo.LongDatePattern;

                case DateTimePattern.ShortTime:
                    return dateTimeFormatInfo.ShortTimePattern;

                case DateTimePattern.LongTime:
                    return dateTimeFormatInfo.LongTimePattern;

                case DateTimePattern.FullDateTime:
                    return dateTimeFormatInfo.FullDateTimePattern;

                case DateTimePattern.MonthDay:
                    return dateTimeFormatInfo.MonthDayPattern;

                case DateTimePattern.RFC1123:
                    return dateTimeFormatInfo.RFC1123Pattern;

                case DateTimePattern.SortableDateTime:
                    return dateTimeFormatInfo.SortableDateTimePattern;

                case DateTimePattern.UniversalSortableDateTime:
                    return dateTimeFormatInfo.UniversalSortableDateTimePattern;

                case DateTimePattern.YearMonth:
                    return dateTimeFormatInfo.YearMonthPattern;

                case DateTimePattern.CustomPattern:
                    return customPattern;

                default:
                    throw new ArgumentOutOfRangeException("Enum implements incorrect.");
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// This enum classifies Date validation mode.
    /// </summary>
    public enum DateValidationMode
    {
        /// <summary>
        /// Specifies value representing that not possible to enter invalid date.
        /// </summary>
        Strict = 0,

        /// <summary>
        /// Specifies value representing that possible to enter invalide date with warning.
        /// </summary>
        Warning
    }

    /// <summary>
    ///
    /// </summary>
    public enum DateTimePattern
    {
        /// <summary>
        /// Chosen value indicates that customized by user DateTime
        /// pattern will be used.
        /// </summary>
        CustomPattern = 0,

        /// <summary>
        /// Chosen value indicates that this is standard ShortDate
        /// pattern.
        /// </summary>
        ShortDate,

        /// <summary>
        /// Chosen value indicates that this is standard LongDate
        /// pattern.
        /// </summary>
        LongDate,

        /// <summary>
        /// Chosen value indicates that this is standard ShortTime
        /// pattern.
        /// </summary>
        ShortTime,

        /// <summary>
        /// Chosen value indicates that this is standard LongTime
        /// pattern.
        /// </summary>
        LongTime,

        /// <summary>
        /// Chosen value indicates that this is standard FullDateTime
        /// pattern.
        /// </summary>
        FullDateTime,

        /// <summary>
        /// Chosen value indicates that this is standard MonthDay
        /// pattern.
        /// </summary>
        MonthDay,

        /// <summary>
        /// Chosen value indicates that this is standard RFC1123 pattern.
        /// </summary>
        RFC1123,

        /// <summary>
        /// Chosen value indicates that this is standard ShortDateTime
        /// pattern.
        /// </summary>
        SortableDateTime,

        /// <summary>
        /// Chosen value indicates that this is standard
        /// UniversalSortableDateTime pattern.
        /// </summary>
        UniversalSortableDateTime,

        /// <summary>
        /// Chosen value indicates that this is standard YearMonth
        /// pattern.
        /// </summary>
        YearMonth
    }
}