// <copyright file="NumericUpDown.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
#if !(WPF||SILVERLIGHT||WINDOWS_PHONE_7)
using Windows.System;
using Windows.UI.Core;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using Syncfusion.WP.Utils;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Syncfusion.WP.Controls.Input
#else
using Syncfusion.UI.Xaml.Utils;
using Syncfusion.UI.Xaml.Controls.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Windows.Input;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> control is a
    /// pair of spin buttons that the user can click to increment or decrement a
    /// values<font color="#000000">.</font>
    /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
    public class SfNumericUpDown : Control
#else
    public class SfNumericUpDown : Control, IDataValidator
#endif
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfNumericUpDown()
        {
            DefaultStyleKey = typeof(SfNumericUpDown);
        }

        #endregion

        #region Variables

        internal Grid outerGrid = null;

        internal Grid textGrid = null;

        internal SfUpDown upDown = null;

        internal bool focusUpDown = false;

        internal bool keyDown = false;

        internal SfNumericTextBox numericTextBox = null;

        internal decimal internalValue = 0;

        internal double internaldoubleValue = 0.0;

        private ICommand spinUpCommand;

        private ICommand spinDownCommand;

        #endregion                 

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the current value for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <remarks>
        /// The Default value is null.
        /// </remarks>
        /// <value>
        /// The value.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SfNumericUpDown), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Gets or sets the content displayed as a watermark in the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> when it is empty.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.WatermarkTemplate"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.WatermarkTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(SfNumericUpDown), new PropertyMetadata(null));


        /// <summary>
        /// <para>Gets or sets the <see cref="N:Windows.UI.Xaml.DataTemplate"/>
        /// used to display the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Watermark"/> of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.</para>
        /// </summary>
        /// <value>
        /// The Default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Watermark"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.WatermarkTemplateSelector"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(SfNumericUpDown), new PropertyMetadata(null));

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Provides a way to choose a <see
        /// cref="N:Windows.UI.Xaml.Controls.DataTemplateSelector"/> based on
        /// the data object and the data-bound <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Watermark"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.WatermarkTemplate"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplateSelector WatermarkTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(WatermarkTemplateSelectorProperty); }
            set { SetValue(WatermarkTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for WatermarkTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateSelectorProperty =
            DependencyProperty.Register("WatermarkTemplateSelector", typeof(DataTemplateSelector), typeof(SfNumericUpDown), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the data that is used as a format for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is N.
        /// </value>
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
            DependencyProperty.Register("FormatString", typeof(string), typeof(SfNumericUpDown), new PropertyMetadata("N", new PropertyChangedCallback(OnFormatStringChanged)));


        /// <summary>
        /// Gets or sets a value indicating whether the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> allow null values.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if allow null values; otherwise, <see langword="false"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowNull.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(SfNumericUpDown), new PropertyMetadata(false, OnAllowNullChanged));

        private static void OnAllowNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnAllowNullChanged(args);
            }
        }
        protected void OnAllowNullChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!AllowNull && Value == null)
            {
                Value = ReadLocalValue(MinimumProperty) != DependencyProperty.UnsetValue ? Minimum : 0.0;
            }
            else if(AllowNull && Value.Equals(0.0))
            {
                Value = null;
            }
        }


        /// <summary>
        /// Gets or sets the culture information associated with the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The Default value is <see cref="T:System.Globalization.CultureInfo"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Culture.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(SfNumericUpDown), new PropertyMetadata(CultureInfo.CurrentUICulture));




        /// <summary>
        /// <para>Gets or Sets the number of decimal digits associated with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/></para>
        /// </summary>
        /// <value>
        /// The Default value is <see cref="F:System.Int32.MaxValue">Int32.MaxValue</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public int MaximumNumberDecimalDigits
        {
            get { return (int)GetValue(MaximumNumberDecimalDigitsProperty); }
            set { SetValue(MaximumNumberDecimalDigitsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximumNumberDecimalDigits.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumNumberDecimalDigitsProperty =
            DependencyProperty.Register("MaximumNumberDecimalDigits", typeof(int), typeof(SfNumericUpDown), new PropertyMetadata(Int32.MaxValue));



        /// <summary>
        /// Gets or Sets the smallest possible value of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is <see cref="F:System.Double.MinValue"/>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Maximum"/>
        [ClassReference(IsReviewed = false)]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(SfNumericUpDown), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(OnMinimumChanged)));



        /// <summary>
        /// Gets or Sets the largest possible value of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is <see cref="F:System.Double.MaxValue"/>.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Minimum"/>
        [ClassReference(IsReviewed = false)]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SfNumericUpDown), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(OnMaximumChanged)));



        /// <summary>
        /// Gets or sets a smallest value to be added to or subtracted with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.LargeChange"/>
        [ClassReference(IsReviewed = false)]
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallChange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(SfNumericUpDown), new PropertyMetadata(1d, new PropertyChangedCallback(OnSmallChangeChanged)));




        /// <summary>
        /// Gets or sets a largest value to be added to or subtracted with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.SmallChange"/>
        [ClassReference(IsReviewed = false)]
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LargeChange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(SfNumericUpDown), new PropertyMetadata(1d, new PropertyChangedCallback(OnLargeChangeChanged)));


        /// <summary>
        /// Gets or sets a value indicating whether the values can be auto reversed.
        /// </summary>
        /// <value>
        /// <c>true</c> if items can be return to initial, once reached maximum; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool AutoReverse
        {
            get { return (bool)GetValue(AutoReverseProperty); }
            set { SetValue(AutoReverseProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoReverse.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoReverseProperty =
            DependencyProperty.Register("AutoReverse", typeof(bool), typeof(SfNumericUpDown), new PropertyMetadata(false));




        /// <summary>
        /// Gets or sets the spin buttons alignment.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SpinButtonsAlignment"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.EnableSpinAnimation"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.UpDownStyle"/>
        [ClassReference(IsReviewed = false)]
        public SpinButtonsAlignment SpinButtonsAlignment
        {
            get { return (SpinButtonsAlignment)GetValue(SpinButtonsAlignmentProperty); }
            set { SetValue(SpinButtonsAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SpinButtonsAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SpinButtonsAlignmentProperty =
            DependencyProperty.Register("SpinButtonsAlignment", typeof(SpinButtonsAlignment), typeof(SfNumericUpDown), new PropertyMetadata(SpinButtonsAlignment.Right, new PropertyChangedCallback(OnSpinButtonsAlignmentChanged)));



        /// <summary>
        /// Gets or sets the style for updown buttons.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.EnableSpinAnimation"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.SpinButtonsAlignment"/>
        [ClassReference(IsReviewed = false)]
        public Style UpDownStyle
        {
            get { return (Style)GetValue(UpDownStyleProperty); }
            set { SetValue(UpDownStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UpDownStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpDownStyleProperty =
            DependencyProperty.Register("UpDownStyle", typeof(Style), typeof(SfNumericUpDown), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfNumericUpDown), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the horizontal alignment of the contents of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is <see cref="N:Windows.UI.Xaml.TextAlignment"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(SfNumericUpDown), new PropertyMetadata(TextAlignment.Left));



        /// <summary>
        /// Gets or sets the Mode of Parsing
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.Parsers"/>
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Parsers.Double"/>
        /// </value>
        public Parsers ParsingMode
        {
            get { return (Parsers)GetValue(ParsingModeProperty); }
            set { SetValue(ParsingModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ParsingMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ParsingModeProperty =
            DependencyProperty.Register("ParsingMode", typeof(Parsers), typeof(SfNumericUpDown), new PropertyMetadata(Parsers.Double));


#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Returns a value whwn set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// The default value is true
        /// </remarks>
        public bool BlockCharactersOnTextInput
        {
            get { return (bool)GetValue(BlockCharactersOnTextInputProperty); }
            set { SetValue(BlockCharactersOnTextInputProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BlockCharactersOnTextInput.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BlockCharactersOnTextInputProperty =
            DependencyProperty.Register("BlockCharactersOnTextInput", typeof(bool), typeof(SfNumericUpDown), new PropertyMetadata(true));  
#endif



        #endregion

        #region Helper Methods

        /// <summary>
        /// This method calls when the decrement button is clicked and Decrements the value.
        /// </summary>
        /// <param name="change">The change.</param>
        private void DecrementValue(double change)
        {
            if (Value == null)
                Value = ReadLocalValue(MinimumProperty) != DependencyProperty.UnsetValue ? Minimum : 0.0;
            if (ParsingMode == Parsers.Decimal)
            {
                decimal.TryParse(Value.ToString(), out internalValue);
                decimal temp = internalValue - Convert.ToDecimal(change);
                if (Maximum.Equals(double.MaxValue))
                {
                    if (temp >= decimal.MinValue && temp <= decimal.MaxValue)
                    {
                        Value = temp;
                    }
                    else if (AutoReverse)
                    {
                        Value = Maximum;
                    }
                }
                else
                {
                    if (Value == null)
                        Value = Minimum;
                    decimal minValue, maxValue;
                    decimal.TryParse(Maximum.ToString(),NumberStyles.Any,Culture, out maxValue);
                    decimal.TryParse(Minimum.ToString(),NumberStyles.Any,Culture, out minValue);
                    if ((temp>= minValue || (minValue == 0 && Minimum == double.MinValue)) &&temp <= maxValue)
                    {
                        Value = temp;
                    }
                    else if (AutoReverse)
                    {
                        Value = Maximum;
                    }
                }
            }
            else
            {
                double temp = internaldoubleValue - change;
                if (temp >= Minimum && temp <= Maximum)
                {
                    if (MaximumNumberDecimalDigits != Int32.MaxValue)
                    {
                        double maxDecimalDigit = MaximumNumberDecimalDigits;
                        if (FormatString != "N")
                            maxDecimalDigit = MaximumNumberDecimalDigits + 2;
                        string formatString = "N" + maxDecimalDigit.ToString();
                        Value = temp.ToString(formatString, Culture.NumberFormat);
                    }
                    else
                        Value = temp;
                }
                else if (AutoReverse)
                {
                    Value = Maximum == double.MaxValue ? Minimum : Maximum; 
                }
            }
            FormatText();

        }




        /// <summary>
        /// This method calls when the increment button is clicked and Increments the value.
        /// </summary>
        /// <param name="change">The change.</param>
        private void IncrementValue(double change)
        {
            if (Value == null)
            {
                Value = ReadLocalValue(MinimumProperty) != DependencyProperty.UnsetValue ? Minimum : 0.0;
            }
            if (ParsingMode == Parsers.Decimal)
            {
                if (Maximum.Equals(double.MaxValue))
                    Maximum = Convert.ToDouble(decimal.MaxValue);
                decimal tempValue;
                decimal.TryParse(change.ToString(), out tempValue);
                decimal.TryParse(Value.ToString(), out internalValue);
                decimal temp =internalValue + tempValue;
                if (Convert.ToDouble(temp) <= Maximum && Convert.ToDouble(temp) >= Minimum)
                {
                    Value = temp;
                }
                else if (AutoReverse)
                {
                    Value = Minimum;
                }
            }
            else
            {
                double temp = internaldoubleValue + change;
                if (temp <= Maximum && temp >= Minimum)
                {
                    if (MaximumNumberDecimalDigits != Int32.MaxValue)
                    {
                        double maxDecimalDigit = MaximumNumberDecimalDigits;
                        if(FormatString!="N")
                            maxDecimalDigit=MaximumNumberDecimalDigits+2;
                        string formatString = "N" + maxDecimalDigit.ToString();
                        Value = temp.ToString(formatString, Culture.NumberFormat);
                    }
                    else
                        Value = temp;
                }


                else
                {
                    if (AutoReverse)

                        Value = Minimum == double.MinValue ? Maximum : Minimum;
                    else if (temp < Minimum)

                        Value = Minimum;
                    else if (temp == Maximum)
                        Value = Maximum;

                }

            }
            FormatText();
        }

        //Validates based on Minimum and Maximum value
        private void Validation()
        {
            if (Value != null)
            {
                if (ParsingMode == Parsers.Decimal)
                {
                    decimal maxValue, minValue;
                    decimal.TryParse(Maximum.ToString(), NumberStyles.Any, Culture, out maxValue);
                    decimal.TryParse(Minimum.ToString(), NumberStyles.Any, Culture, out minValue);
                    if (Maximum.Equals(double.MaxValue) || Minimum.Equals(double.MinValue))
                    {
                        if (internalValue > decimal.MaxValue || (Maximum != double.MaxValue && internalValue > maxValue))
                        {
                            Value = Convert.ToDecimal(Maximum);
                        }
                        else if (internalValue < decimal.MinValue || (Minimum!=double.MinValue && internalValue < minValue))
                        {
                            Value = Convert.ToDecimal(Minimum);
                        }
                    }
                    else
                    {
                        if (internalValue > maxValue)
                        {
                            Value = maxValue;
                        }
                        else if (internalValue < minValue)
                        {
                            Value = minValue;
                        }
                    }
                }
                else
                {
                    if (internaldoubleValue > Maximum)
                    {
                        Value = Maximum;
                    }
                    else if (internaldoubleValue < Minimum)
                    {
                        Value = Minimum;
                    }
                }
            }
        }
        private void FormatText()
        {
            if (numericTextBox != null)
            {
                if (focusUpDown)
                {
                    if (ParsingMode == Parsers.Decimal)
                        numericTextBox.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                    else if (ParsingMode == Parsers.Double)
                        numericTextBox.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (upDown != null)
                    {
                        upDown.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
#endif
                }
                else
                {
                    numericTextBox.SelectionStart = numericTextBox.Text.Length;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increments the associated performance counter by one and stores the result, as
        /// an atomic operation.
        /// </summary>
        /// <remarks>
        /// Returns void
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Decrement"/>
        [ClassReference(IsReviewed = false)]
        public void Increment()
        {
            focusUpDown = true;
            IncrementValue(SmallChange);
        }
        /// <summary>
        /// Decrements the associated performance counter by one and stores the result, as
        /// an atomic operation.
        /// </summary>
        /// <remarks>
        /// Returns void
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Increment"/>
        [ClassReference(IsReviewed = false)]
        public void Decrement()
        {
            focusUpDown = true;
            DecrementValue(SmallChange);
        }

        
        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            outerGrid = GetTemplateChild("PART_OuterGrid") as Grid;
            textGrid = GetTemplateChild("PART_TextGrid") as Grid;
            upDown = GetTemplateChild("PART_UpDown") as SfUpDown;
            numericTextBox = GetTemplateChild("PART_Content") as SfNumericTextBox;
            if (numericTextBox != null)
            {
                numericTextBox.LostFocus += numericTextBox_LostFocus;
            }
            base.OnApplyTemplate();
            Validation();

            if (!AllowNull && Value == null)
            {
                Value = ReadLocalValue(MinimumProperty) != DependencyProperty.UnsetValue ? Minimum : 0.0;
            }
            else if (AllowNull && Value!=null && (Value.Equals(0.0)||Value.Equals(Minimum)))
            {
                Value = null;
            }
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void numericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            keyDown = false;
        }
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
#else
        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
#endif
        {
            base.OnKeyDown(e);
            //CoreWindow for the active thread for using GetKeyState Method
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            var coreWindow = CoreWindow.GetForCurrentThread() as CoreWindow;

            var downState = CoreVirtualKeyStates.Down;

            //Specifies if Shift Key is pressed
            bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;

            if ((shiftKey && e.Key == VirtualKey.Tab))
            {
                keyDown = true;
            }
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if (e.Key == Key.Down)
#else
            if (e.Key == VirtualKey.Down)
#endif
            {
                DecrementValue(SmallChange);
                ValidateNumericTextBox();
                if (numericTextBox != null)
                    numericTextBox.Text = Value.ToString();
                e.Handled = true;
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            else if (e.Key == Key.Up)
#else
            else if (e.Key == VirtualKey.Up)
#endif
            {
                IncrementValue(SmallChange);
                ValidateNumericTextBox();
                if (numericTextBox != null)
                    numericTextBox.Text = Value.ToString();
                e.Handled = true;
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            else if (e.Key == Key.PageDown)
#else
            else if (e.Key == VirtualKey.PageDown)
#endif
            {
                DecrementValue(LargeChange);
                if (numericTextBox != null)
                    numericTextBox.Text = Value.ToString();
                e.Handled = true;
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            else if (e.Key == Key.PageUp)
#else
            else if (e.Key == VirtualKey.PageUp)
#endif
            {
                IncrementValue(LargeChange);
                if (numericTextBox != null)
                    numericTextBox.Text = Value.ToString();
                e.Handled = true;
            }
            else
                e.Handled = false;
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
#else
        protected override void OnKeyUp(KeyRoutedEventArgs e)
#endif
        {
            keyDown = false;
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (!keyDown)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                if (FocusManager.GetFocusedElement() != this)
                {
                    numericTextBox.Focus();
                }
#else
                if (FocusState != Windows.UI.Xaml.FocusState.Unfocused || FocusState == FocusState.Keyboard)
                {
                    numericTextBox.Focus(FocusState);
                }
#endif
            }
            focusUpDown = false;
            Validation();
            ValidateNumericTextBox();
        }

        private void ValidateNumericTextBox()
        {
            if (numericTextBox != null)
            {
                if (Value == null && AllowNull)
                    numericTextBox.Text = String.Empty;
                else
                {
                    string formatString;
                    if (MaximumNumberDecimalDigits != Int32.MaxValue)
                        formatString = "N" + MaximumNumberDecimalDigits.ToString();
                    else
                        formatString = FormatString;

                    if (ParsingMode == Parsers.Decimal)
                    {
                        if (MaximumNumberDecimalDigits != Int32.MaxValue  && internalValue.ToString().Contains(Culture.NumberFormat.NumberDecimalSeparator) && (internalValue.ToString().Length-internalValue.ToString().IndexOf(Culture.NumberFormat.NumberDecimalSeparator)) > MaximumNumberDecimalDigits)
                            numericTextBox.Text = internalValue.ToString(formatString, Culture.NumberFormat);
                        else
                            numericTextBox.Text = internalValue.ToString();
                    }
                    else if (ParsingMode == Parsers.Double)
                    {
                        if (MaximumNumberDecimalDigits != Int32.MaxValue && internaldoubleValue.ToString().Contains(Culture.NumberFormat.NumberDecimalSeparator) && (internaldoubleValue.ToString().Length-internaldoubleValue.ToString().IndexOf(Culture.NumberFormat.NumberDecimalSeparator)) > MaximumNumberDecimalDigits)
                            numericTextBox.Text = internaldoubleValue.ToString(formatString, Culture.NumberFormat);
                        else
                            numericTextBox.Text = internaldoubleValue.ToString();                 
                    }

                    numericTextBox.SelectionStart = numericTextBox.Text.Length;
                }
            }
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Validation();
            if (numericTextBox != null)
            {
                if (Value == null && AllowNull)
                    numericTextBox.Text = String.Empty;
                else
                {
                    if (ParsingMode == Parsers.Decimal)
                        numericTextBox.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                    else if (ParsingMode == Parsers.Double)
                        numericTextBox.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                }
            }
        }

        /// <summary>
        /// Occurs when the PointerWheel is changed
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
 	        base.OnMouseWheel(e);
            if (e.Delta > 0)
#else
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0)
#endif
            {
                Increment();
            }
            else
            {
                Decrement();
            }
        }
        #endregion

        #region Callback Methods

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> value is changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnValueChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown"/> value is changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Value != null)
            {
                bool isDouble;
                if (ParsingMode == Parsers.Decimal)
                {
                    isDouble = decimal.TryParse(Value.ToString(), out internalValue);
                }
                else
                {
                    isDouble = double.TryParse(Value.ToString(), out internaldoubleValue);
                }
                if (isDouble)
                {
                    Validation();
                    if (this.ValueChanged != null)
                    {
                        ValueChangedEventArgs valueArgs = new ValueChangedEventArgs() { NewValue = args.NewValue, OldValue = args.OldValue };
                        this.ValueChanged(this, valueArgs);
                    }
                }
                else
                {
                    throw new InvalidCastException("Input was not in the correct format");
                }
            }
            else
            {
                if (!AllowNull)
                    Value = ReadLocalValue(MinimumProperty) != DependencyProperty.UnsetValue && Minimum != double.MinValue ? Minimum : 0.0;
            }
        }

        /// <summary>
        /// Occurs when the Minimum value has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinimumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnMinimumChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the Minimum value has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
        {
            Validation();
            if (MinimumChanged != null)
            {
                MinimumChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the Maximum value has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaximumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnMaximumChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the Maximum value has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
        {
            Validation();
            if (MaximumChanged != null)
            {
                MaximumChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the SmallChange value has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSmallChangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnSmallChangeChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the SmallChange value has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSmallChangeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SmallChangeChanged != null)
            {
                SmallChangeChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when the LargeChange value has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnLargeChangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnLargeChangeChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the LargeChange value has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnLargeChangeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (LargeChangeChanged != null)
            {
                LargeChangeChanged(this, args);
            }
        }
        private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnFormatStringChanged(args);
            }
        }
        protected void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {

            if (Value == null && AllowNull)
                numericTextBox.Text = string.Empty;
        }


        /// <summary>
        /// Occurs when the SpinButtonsAlignment has changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSpinButtonsAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericUpDown)obj != null)
            {
                ((SfNumericUpDown)obj).OnSpinButtonsAlignmentChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the SpinButtonsAlignment has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnSpinButtonsAlignmentChanged(DependencyPropertyChangedEventArgs args)
        {
            if (numericTextBox != null)
            {
                if (SpinButtonsAlignment == SpinButtonsAlignment.Both)
                {
                        numericTextBox.TextAlignment = TextAlignment.Center;
                }
                else
                {
                    numericTextBox.TextAlignment = TextAlignment.Left;
                }
            }
        }


        #endregion

        #region Commands
        
        /// <summary>
        /// Enables the value to increment
        /// </summary>
        public ICommand SpinUpCommand
        {
            get
            {
                if (spinUpCommand == null)
                {
                    spinUpCommand = new DelegateCommand(param => Increment());
                }
                return spinUpCommand;
            }
        }       

        /// <summary>
        /// Enables the value to decrement
        /// </summary>
        public ICommand SpinDownCommand
        {
            get
            {
                if (spinDownCommand == null)
                {
                    spinDownCommand = new DelegateCommand(param => Decrement());
                }
                return spinDownCommand;
            }
        }
        #endregion  

        #region Events


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Value"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event ValueChangedEventHandler ValueChanged;


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Minimum"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback MinimumChanged;


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.Maximum"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback MaximumChanged;


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.SmallChange"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback SmallChangeChanged;


        /// <summary>
        /// Occurs when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericUpDown.LargeChange"/>changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event PropertyChangedCallback LargeChangeChanged;

        #endregion

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Validates the states
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.VisualStates"/>
        /// </summary>
        /// <param name="args"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Validate(ValidationEventArgs args)
        {
            if (args.HasError)
            {
                VisualStateManager.GoToState(this, "HasError", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoError", true);
            }
        }
#endif
    }
}
