#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Styles;
    using Syncfusion.Windows.Shared;
    using System.Windows.Controls;
    using System.Threading;

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for date time edit control properties in a cell. 
    /// <para/>Each property in this sub object can be configured individually. Properties that
    /// have not been initialized will inherit default values from a base style.
    /// </summary>
    public class GridDateTimeEditStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridDateTimeEditStyleInfo defaultDateTimeEditInfo;

        /// <summary>
        /// Initalizes a new <see cref="GridDateTimeEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridDateTimeEditStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridDateTimeEditStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridDateTimeEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridDateTimeEditStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridDateTimeEditStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridDateTimeEditStyleInfoStore"/> that holds data for this <see cref="GridDateTimeEditStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridDateTimeEditStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridDateTimeEditStyleInfo(StyleInfoSubObjectIdentity identity, GridDateTimeEditStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridDateTimeEditStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridDateTimeEditStyleInfo()
            : base(new GridDateTimeEditStyleInfoStore())
        {
        }

        /// <summary>
        /// Returns a default <see cref="GridDateTimeEditStyleInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the validation info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// <para/>
        /// </remarks>
        public static GridDateTimeEditStyleInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultDateTimeEditInfo == null)
                {
                    defaultDateTimeEditInfo = new GridDateTimeEditStyleInfo();
                    defaultDateTimeEditInfo.AutoCorrectedHighlightDuration = 1d;
                    defaultDateTimeEditInfo.CorrectForeground = Brushes.Blue;
                    defaultDateTimeEditInfo.Cursor = Cursors.Arrow;
                    //defaultDateTimeEditInfo.CustomPattern = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    defaultDateTimeEditInfo.DateTimePattern = DateTimePattern.ShortDate;
                    defaultDateTimeEditInfo.IncorrectForeground = Brushes.Red;
                    defaultDateTimeEditInfo.IsAnimation = true;
                    defaultDateTimeEditInfo.IsAutoCorrect = false;
                    defaultDateTimeEditInfo.IsButtonPopUpEnabled = true;
                    defaultDateTimeEditInfo.IsCalendarEnabled = true;
                    defaultDateTimeEditInfo.IsEditable = true;
                    defaultDateTimeEditInfo.IsEmptyDateEnabled = true;
                    defaultDateTimeEditInfo.IsEnabledRepeatButton = true;
                    defaultDateTimeEditInfo.IsHoldMaxWidth = false;
                    defaultDateTimeEditInfo.IsPopupEnabled = true;
                    defaultDateTimeEditInfo.IsScrollingOnCircle = true;
                    defaultDateTimeEditInfo.IsVisibleRepeatButton = true;
                    defaultDateTimeEditInfo.IsWatchEnabled = true;
                    defaultDateTimeEditInfo.MaxDateTime = DateTime.MaxValue;
                    defaultDateTimeEditInfo.MinDateTime = DateTime.MinValue;
                    defaultDateTimeEditInfo.NoneDateText = "No date is selected";
                    defaultDateTimeEditInfo.RepeatButtonBackground = Brushes.LightSteelBlue;
                    defaultDateTimeEditInfo.RepeatButtonBorderBrush = Brushes.LightSteelBlue;
                    defaultDateTimeEditInfo.UncertainForeground = Brushes.Gray;
                    defaultDateTimeEditInfo.CanEdit = false;
                }

                return defaultDateTimeEditInfo;
            }
        }

        /// <summary>
        /// Gets or sets the auto corrected highlight duration.
        /// </summary>
        public double AutoCorrectedHighlightDuration
        {
            get
            {
                return (double)this.GetValue(GridDateTimeEditStyleInfoStore.AutoCorrectedHighlightDurationProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.AutoCorrectedHighlightDurationProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the AutoCorrectedHighlightDuration property is initialized.
        /// </summary>
        public bool HasAutoCorrectedHighlighDuration
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.AutoCorrectedHighlightDurationProperty);
            }
        }

        /// <summary>
        /// Gets or sets the correct foreground.
        /// </summary>
        public Brush CorrectForeground
        {
            get
            {
                return (Brush)this.GetValue(GridDateTimeEditStyleInfoStore.CorrectForegroundProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.CorrectForegroundProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the CorrectForeground property is initialized.
        /// </summary>
        public bool HasCorrectForeground
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.CorrectForegroundProperty);
            }
        }

        /// <summary>
        /// Gets or sets the cursor to display. The default value is null.
        /// </summary>
        public Cursor Cursor
        {
            get
            {
                return (Cursor)this.GetValue(GridDateTimeEditStyleInfoStore.CursorProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.CursorProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the Cursor property is initialized.
        /// </summary>
        public bool HasCursor
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.CursorProperty);
            }
        }

        /// <summary>
        /// Gets or sets a brush for marking incorrect foreground.
        /// </summary>
        public Brush IncorrectForeground
        {
            get
            {
                return (Brush)this.GetValue(GridDateTimeEditStyleInfoStore.IncorrectForegroundProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IncorrectForegroundProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the IncorrectForeground property is initialized.
        /// </summary>
        public bool HasIncorrectForeground
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IncorrectForegroundProperty);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for this date time edit control.
        /// </summary>
        public DateTime MaxDateTime
        {
            get
            {
                return (DateTime)this.GetValue(GridDateTimeEditStyleInfoStore.MaxDateTimeProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.MaxDateTimeProperty, value);
            }
        }

        public bool CanEdit
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.CanEditProperty);
            }
            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.CanEditProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the MaxDateTime property is initialized.
        /// </summary>
        public bool HasMaxDateTime
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.MaxDateTimeProperty);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for this date time edit control.
        /// </summary>
        public DateTime MinDateTime
        {
            get
            {
                return (DateTime)this.GetValue(GridDateTimeEditStyleInfoStore.MinDateTimeProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.MinDateTimeProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the MinDateTime property is initialized.
        /// </summary>
        public bool HasMinDateTime
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.MinDateTimeProperty);
            }
        }

        /// <summary>
        /// Gets or sets the popup delay.
        /// </summary>
        public TimeSpan PopupDelay
        {
            get
            {
                if (this.GetValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty) == null)
                {
                    return TimeSpan.MinValue;
                }
                return (TimeSpan)this.GetValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty, value);
            }
        }

        /// <summary>
        /// Specifies whether the PopupDelay property is initialized.
        /// </summary>
        public bool HasPopupDelay
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty);
            }
        }

        /// <summary>
        /// Gets or sets the brush for uncertain foreground.
        /// </summary>
        public Brush UncertainForeground
        {
            get
            {
                return (Brush)this.GetValue(GridDateTimeEditStyleInfoStore.UncertainForegroundProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.UncertainForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DateTimePattern for the GridStyleInfo. Set the
        /// CurrentThread.CurrentCulture if you are specifying any other CultureInfo.
        /// <para></para>
        /// <code lang="C#">            
        ///             var dateTimeStyleInfo = this.grid.Model[6, 1];
        ///             dateTimeStyleInfo.CultureInfo = new CultureInfo(&quot;vi-VN&quot;);
        ///             dateTimeStyleInfo.DateTimeEdit.DateTimePattern = Syncfusion.Windows.Shared.DateTimePattern.RFC1123;
        ///             dateTimeStyleInfo.CellValue = DateTime.Now;
        ///             dateTimeStyleInfo.CellType = &quot;DateTimeEdit&quot;;
        ///             Thread.CurrentThread.CurrentCulture = dateTimeStyleInfo.CultureInfo;</code>
        /// </summary>
        public DateTimePattern DateTimePattern
        {
            get
            {
                return (DateTimePattern)this.GetValue(GridDateTimeEditStyleInfoStore.DateTimePatternProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.DateTimePatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of DateTimePattern property.
        /// </summary>
        public void ResetDateTimePattern()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.DateTimePatternProperty);
        }

        /// <summary>
        /// Specifies whether the DateTimePattern property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeDateTimePattern()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.DateTimePatternProperty);
        }


        /// <summary>
        /// Specifies whether the DateTimePattern property is initialized.
        /// </summary>
        public bool HasDateTimePattern
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.DateTimePatternProperty);
            }
        }

        /// <summary>
        /// Gets or sets the custom pattern.
        /// </summary>
        public string CustomPattern
        {
            get
            {
                return (string)this.GetValue(GridDateTimeEditStyleInfoStore.CustomPatternProperty);
            }
            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.CustomPatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of CustomPattern property.
        /// </summary>
        public void ResetCustomPattern()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.CustomPatternProperty);
        }

        /// <summary>
        /// Specifies whether the CustomPattern property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeCustomPattern()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.CustomPatternProperty);
        }

        /// <summary>
        /// Specifies whether the CustomPattern property is initialized.
        /// </summary>
        public bool HasCustomPattern
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.CustomPatternProperty);
            }
        }

        /// <summary>
        /// Specifies whether the UncertainForeground property is initialized.
        /// </summary>
        public bool HasUncertainForeground
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.UncertainForegroundProperty);
            }
        }

        /// <summary>
        /// Specifies whether the MaxDate property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeMaxDate()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.MaxDateTimeProperty);
        }

        /// <summary>
        /// Specifies whether the MinDate property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeMinDate()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.MinDateTimeProperty);
        }

        #region IsEmptyDateEnabled
        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty date enabled.
        /// </summary>
        public bool IsEmptyDateEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsEmptyDateEnabledProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsEmptyDateEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsEmptyDateEnabled property.
        /// </summary>
        public void ResetIsEmptyDateEnabled()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsEmptyDateEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsEmptyDateEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsEmptyDateEnabled()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsEmptyDateEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsEmptyDateEnabled property is initialized.
        /// </summary>
        public bool HasIsEmptyDateEnabled
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsEmptyDateEnabledProperty);
            }
        }

        #endregion

        #region IsWatchEnabled
        /// <summary>
        /// Gets or sets a value indicating whether this instance is watch enabled.
        /// </summary>
        public bool IsWatchEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsWatchEnabledProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsWatchEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsWatchEnabled property.
        /// </summary>
        public void ResetIsWatchEnabled()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsWatchEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsWatchEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsWatchEnabled()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsWatchEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsWatchEnabled property is initialized.
        /// </summary>
        public bool HasIsWatchEnabled
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsWatchEnabledProperty);
            }
        }

        #endregion

        #region IsAnimation
        /// <summary>
        /// Gets or sets a value indicating whether this instance is animation.
        /// </summary>
        public bool IsAnimation
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsAnimationProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsAnimationProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsAnimation property.
        /// </summary>
        public void ResetIsAnimation()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsAnimationProperty);
        }

        /// <summary>
        /// Specifies whether the IsAnimation property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsAnimation()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsAnimationProperty);
        }

        /// <summary>
        /// Specifies whether the IsAnimation property is initialized.
        /// </summary>
        public bool HasIsAnimation
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsAnimationProperty);
            }
        }

        #endregion

        #region IsAutoCorrect
        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto correct.
        /// </summary>
        public bool IsAutoCorrect
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsAutoCorrectProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsAutoCorrectProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsAutoCorrect property.
        /// </summary>
        public void ResetIsAutoCorrect()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsAutoCorrectProperty);
        }

        /// <summary>
        /// Specifies whether the IsAutoCorrect property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsAutoCorrect()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsAutoCorrectProperty);
        }

        /// <summary>
        /// Specifies whether the IsAutoCorrect property is initialized.
        /// </summary>
        public bool HasIsAutoCorrect
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsAutoCorrectProperty);
            }
        }

        #endregion

        #region IsButtonPopUpEnabled
        /// <summary>
        /// Gets or sets a value indicating whether this instance is button pop up enabled.
        /// </summary>
        public bool IsButtonPopUpEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsButtonPopUpEnabledProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsButtonPopUpEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsButtonPopUpEnabled property.
        /// </summary>
        public void ResetIsButtonPopUpEnabled()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsButtonPopUpEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsButtonPopUpEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsButtonPopUpEnabled()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsButtonPopUpEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsButtonPopUpEnabled property is initialized.
        /// </summary>
        public bool HasIsButtonPopUpEnabled
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsButtonPopUpEnabledProperty);
            }
        }

        #endregion

        #region IsCalendarEnabled
        /// <summary>
        /// Gets or sets a value indicating whether this instance is calendar enabled.
        /// </summary>
        public bool IsCalendarEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsCalendarEnabledProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsCalendarEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsCalendarEnabled property.
        /// </summary>
        public void ResetIsCalendarEnabled()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsCalendarEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsCalendarEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsCalendarEnabled()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsCalendarEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsCalendarEnabled property is initialized.
        /// </summary>
        public bool HasIsCalendarEnabled
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsCalendarEnabledProperty);
            }
        }

        #endregion

        #region IsEditable
        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsEditableProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsEditableProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsEditable property.
        /// </summary>
        public void ResetIsEditable()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsEditableProperty);
        }

        /// <summary>
        /// Specifies whether the IsEditable property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsEditable()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsEditableProperty);
        }

        /// <summary>
        /// Specifies whether the IsEditable property is initialized.
        /// </summary>
        public bool HasIsEditable
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsEditableProperty);
            }
        }

        #endregion

        #region IsEnabledRepeatButton
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled repeat button.
        /// </summary>
        public bool IsEnabledRepeatButton
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsEnabledRepeatButtonProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsEnabledRepeatButtonProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsEnabledRepeatButton property.
        /// </summary>
        public void ResetIsEnabledRepeatButton()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsEnabledRepeatButtonProperty);
        }

        /// <summary>
        /// Specifies whether the IsEnabledRepeatButton property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsEnabledRepeatButton()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsEnabledRepeatButtonProperty);
        }

        /// <summary>
        /// Specifies whether the IsEnabledRepeatButton property is initialized.
        /// </summary>
        public bool HasIsEnabledRepeatButton
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsEnabledRepeatButtonProperty);
            }
        }

        #endregion

        #region IsHoldMaxWidth
        /// <summary>
        /// Gets or sets a value indicating whether this instance is hold max width.
        /// </summary>
        public bool IsHoldMaxWidth
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsHoldMaxWidthProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsHoldMaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsHoldMaxWidth property.
        /// </summary>
        public void ResetIsHoldMaxWidth()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsHoldMaxWidthProperty);
        }

        /// <summary>
        /// Specifies whether the IsHoldMaxWidth property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsHoldMaxWidth()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsHoldMaxWidthProperty);
        }

        /// <summary>
        /// Specifies whether the IsHoldMaxWidth property is initialized.
        /// </summary>
        public bool HasIsHoldMaxWidth
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsHoldMaxWidthProperty);
            }
        }

        #endregion

        #region IsPopupEnabled
        /// <summary>
        /// Gets or sets a value indicating whether this instance is popup enabled.
        /// </summary>
        public bool IsPopupEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsPopupEnabledProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsPopupEnabledProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsPopupEnabled property.
        /// </summary>
        public void ResetIsPopupEnabled()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsPopupEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsPopUpEnabled property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsPopupEnabled()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsPopupEnabledProperty);
        }

        /// <summary>
        /// Specifies whether the IsPopupEnabled property is initialized.
        /// </summary>
        public bool HasIsPopupEnabled
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsPopupEnabledProperty);
            }
        }

        #endregion

        #region IsScrollingOnCircle
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scrolling on circle.
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsScrollingOnCircleProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsScrollingOnCircleProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsScrollingOnCircle property.
        /// </summary>
        public void ResetIsScrollingOnCircle()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsScrollingOnCircle()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsScrollingOnCircleProperty);
        }

        /// <summary>
        /// Specifies whether the IsScrollingOnCircle property is initialized.
        /// </summary>
        public bool HasIsScrollingOnCircle
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsScrollingOnCircleProperty);
            }
        }
        
        #endregion

        #region IsVisibleRepeatButton
        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible repeat button.
        /// </summary>
        public bool IsVisibleRepeatButton
        {
            get
            {
                return (bool)this.GetValue(GridDateTimeEditStyleInfoStore.IsVisibleRepeatButtonProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.IsVisibleRepeatButtonProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of IsVisibleRepeatButton property.
        /// </summary>
        public void ResetIsVisibleRepeatButton()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IsVisibleRepeatButtonProperty);
        }

        /// <summary>
        /// Specifies whether the IsVisibleRepeatButton property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIsVisibleRepeatButton()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IsVisibleRepeatButtonProperty);
        }

        /// <summary>
        /// Specifies whether the IsVisibleRepeatButton property is initialized.
        /// </summary>
        public bool HasIsVisibleRepeatButton
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.IsVisibleRepeatButtonProperty);
            }
        }
        
        #endregion

        #region NoneDateText
        /// <summary>
        /// Gets or sets the none date text.
        /// </summary>
        public string NoneDateText
        {
            get
            {
                return (string)this.GetValue(GridDateTimeEditStyleInfoStore.NoneDateTextProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.NoneDateTextProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of NoneDateText property.
        /// </summary>
        public void ResetNoneDateText()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.NoneDateTextProperty);
        }

        /// <summary>
        /// Specifies whether the NoneDateText property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeNoneDateText()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.NoneDateTextProperty);
        }

        /// <summary>
        /// Specifies whether the NoneDateText property is initialized.
        /// </summary>
        public bool HasNoneDateText
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.NoneDateTextProperty);
            }
        }
        
        #endregion

        #region RepeatButtonBackground
        /// <summary>
        /// Gets or sets the background color of the repeat buttons.
        /// </summary>
        public Brush RepeatButtonBackground
        {
            get
            {
                return (Brush)this.GetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBackgroundProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of RepeatButtonBackground property.
        /// </summary>
        public void ResetRepeatButtonBackground()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBackgroundProperty);
        }

        /// <summary>
        /// Specifies whether the RepeatButtonBackground property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeRepeatButtonBackground()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.RepeatButtonBackgroundProperty);
        }

        /// <summary>
        /// Specifies whether the RepeatButtonBackground property is initialized.
        /// </summary>
        public bool HasRepeatButtonBackground
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.RepeatButtonBackgroundProperty);
            }
        }
        
        #endregion

        #region RepeatButtonBorderBrush

        /// <summary>
        /// Gets or sets the Border brush of the repeat buttons.
        /// </summary>
        public Brush RepeatButtonBorderBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBorderBrushProperty);
            }

            set
            {
                this.SetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the value of RepeatButtonBorderBrush property.
        /// </summary>
        public void ResetRepeatButtonBorderBrush()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.RepeatButtonBorderBrushProperty);
        }

        /// <summary>
        /// Specifies whether the RepeatButtonBorderBrush property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeRepeatButtonBorderBrush()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.RepeatButtonBorderBrushProperty);
        }

        /// <summary>
        /// Specifies whether the RepeatButtonBorderBrush property is initialized.
        /// </summary>
        public bool HasRepeatButtonBorderBrush
        {
            get
            {
                return this.HasValue(GridDateTimeEditStyleInfoStore.RepeatButtonBorderBrushProperty);
            }
        }
        
        #endregion

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>The copy of the current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridDateTimeEditStyleInfo(newOwner.CreateSubObjectIdentity(sip), (GridDateTimeEditStyleInfoStore)Store.Clone());
        }

        /// <summary>
        /// Resets the value of AutoCorrectedHighlighDuration property.
        /// </summary>
        public void ResetAutoCorrectedHighlighDuration()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.AutoCorrectedHighlightDurationProperty);
        }

        /// <summary>
        /// Resets the value of CorrectForeground property.
        /// </summary>
        public void ResetCorrectForeground()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.CorrectForegroundProperty);
        }

        /// <summary>
        /// Resets the value of Cursor property.
        /// </summary>
        public void ResetCursor()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.CursorProperty);
        }

        /// <summary>
        /// Resets the value of IncorrectForeground property.
        /// </summary>
        public void ResetIncorrectForeground()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.IncorrectForegroundProperty);
        }

        /// <summary>
        /// Resets the value of MaxDate property.
        /// </summary>
        public void ResetMaxDate()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.MaxDateTimeProperty);
        }

        /// <summary>
        /// Resets the value of MinDate property.
        /// </summary>
        public void ResetMinDate()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.MinDateTimeProperty);
        }

        /// <summary>
        /// Resets the value of PopupDelay property.
        /// </summary>
        public void ResetPopupDelay()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty);
        }

        /// <summary>
        /// Resets the value of UncertainForeground property.
        /// </summary>
        public void ResetUncertainForeground()
        {
            this.ResetValue(GridDateTimeEditStyleInfoStore.UncertainForegroundProperty);
        }

        /// <summary>
        /// Specifies whether the AutoCorrectedHighlighDuration property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeAutoCorrectedHighlighDuration()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.AutoCorrectedHighlightDurationProperty);
        }

        /// <summary>
        /// Specifies whether the CorrectForeground property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeCorrectForeground()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.CorrectForegroundProperty);
        }

        /// <summary>
        /// Specifies whether the Cursor property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeCursor()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.CursorProperty);
        }

        /// <summary>
        /// Specifies whether the IncorrectForeground property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeIncorrectForeground()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.IncorrectForegroundProperty);
        }

        /// <summary>
        /// Specifies whether the PopupDelay property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializePopupDelay()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.PopupDelayProperty);
        }

        /// <summary>
        /// Specifies whether the UncertainForeground property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        public bool ShouldSerializeUncertainForeground()
        {
            return this.HasValue(GridDateTimeEditStyleInfoStore.UncertainForegroundProperty);
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridDateTimeEditStyleInfo(identity, store as GridDateTimeEditStyleInfoStore);
            }

            return new GridDateTimeEditStyleInfo(identity);
        }

        /// <summary>
        /// Returns <see cref="GridDateTimeEditStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridDateTimeEditStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridDateTimeEditStyleInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable]
    [StaticDataField("sd")]
    public class GridDateTimeEditStyleInfoStore : StyleInfoStore
    {
        static GridDateTimeEditStyleInfoStore()
        {
        }

        private static StaticData sd = new StaticData(typeof(GridDateTimeEditStyleInfoStore), typeof(GridDateTimeEditStyleInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.AutoCorrectedHighlighDuration"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty AutoCorrectedHighlightDurationProperty = sd.CreateStyleInfoProperty(typeof(double), "AutoCorrectedHighlightDuration");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.CorrectForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty CorrectForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "CorrectForeground");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.Cursor"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty CursorProperty = sd.CreateStyleInfoProperty(typeof(Cursor), "Cursor");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IncorrectForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IncorrectForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "IncorrectForeground");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.MaxDate"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MaxDateTimeProperty = sd.CreateStyleInfoProperty(typeof(DateTime), "MaxDateTime");

        public static readonly StyleInfoProperty CanEditProperty = sd.CreateStyleInfoProperty(typeof(bool), "CanEdit");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.MinDate"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty MinDateTimeProperty = sd.CreateStyleInfoProperty(typeof(DateTime), "MinDateTime");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.PopupDelay"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty PopupDelayProperty = sd.CreateStyleInfoProperty(typeof(TimeSpan), "PopupDelay");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.UncertainForeground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty UncertainForegroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "UncertainForeground");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.DateTimePattern"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty DateTimePatternProperty = sd.CreateStyleInfoProperty(typeof(DateTimePattern), "DateTimePattern");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.CustomPattern"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty CustomPatternProperty = sd.CreateStyleInfoProperty(typeof(string), "CustomPattern");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsEmptyDateEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsEmptyDateEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsEmptyDateEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsWatchEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsWatchEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsWatchEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.CaretTemplate"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty CaretTemplateProperty = sd.CreateStyleInfoProperty(typeof(ControlTemplate), "CaretTemplate");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.CloseCalendarAction"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty CloseCalendarActionProperty = sd.CreateStyleInfoProperty(typeof(CloseCalendarAction), "CloseCalendarAction");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsAnimation"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsAnimationProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsAnimation");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsAutoCorrect"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsAutoCorrectProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsAutoCorrect");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsButtonPopUpEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsButtonPopUpEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsButtonPopUpEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsCalendarEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsCalendarEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsCalendarEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsEditable"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsEditableProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsEditable");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsEnabledRepeatButton"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsEnabledRepeatButtonProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsEnabledRepeatButton");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsHoldMaxWidth"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsHoldMaxWidthProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsHoldMaxWidth");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsPopupEnabled"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsPopupEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsPopupEnabled");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsScrollingOnCircle"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsScrollingOnCircleProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsScrollingOnCircle");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.IsVisibleRepeatButton"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty IsVisibleRepeatButtonProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsVisibleRepeatButton");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.NoneDateText"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty NoneDateTextProperty = sd.CreateStyleInfoProperty(typeof(string), "NoneDateText");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.RepeatButtonBackground"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty RepeatButtonBackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "RepeatButtonBackground");

        /// <summary>
        /// Provides information about the <see cref="GridDateTimeEditStyleInfo.RepeatButtonBorderBrus"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty RepeatButtonBorderBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "RepeatButtonBorderBrush");

        /// <overload>
        /// Initializes a new <see cref="GridDateTimeEditStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDateTimeEditStyleInfoStore"/>.
        /// </summary>
        public GridDateTimeEditStyleInfoStore()
        {
        }
#if !SyncfusionFramework4_0
        /// <summary>
        /// Initializes a new <see cref="GridDateTimeEditStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridDateTimeEditStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Creates a copy of the current object.</summary>
        /// <returns>A duplicate of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridDateTimeEditStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}