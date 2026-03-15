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

#if !WINRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7

namespace Syncfusion.WP.Controls.Input
#elif WPF
using Syncfusion.Licensing;
using System.Diagnostics;
namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a control for performing calculations with decimal numbers. <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/>
    /// </summary>
    [StyleTypedProperty(Property = "FunctionsPaneStyle", StyleTargetType = typeof(FunctionsPane))]
    public class SfCalculator : Control
    {
        private CalculatorFunctions previousFunction = CalculatorFunctions.None;

        private decimal previousValue;

        private bool lastfunctionexecuted = false;

        private FunctionsPane PART_Functions;

        private ExpressionList expressions = new ExpressionList();

        private bool percent = false;

        private decimal evaluatedValue;

        private string errorMessage=CalcConstatnts.CError;

        /// <summary>
        /// Used for handling events due to error messages.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public delegate void ErrorMessageEventHandler(Object Sender, ErrorDisplayArgs args);

        /// <summary>
        /// Invoked when an error message is displayed.
        /// </summary>
        public event ErrorMessageEventHandler ErrorMessageDisplayed;

        /// <summary>
        /// Initializes an instance for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control.
        /// </summary>
        public SfCalculator()
        {
#if WPF
            if (EnvironmentTestInput.IsSecurityGranted)
            {
                EnvironmentTestInput.StartValidateLicense(typeof(SfCalculator));
            }
#endif
            DefaultStyleKey = typeof(SfCalculator);
            Value = 0;
#if WINDOWS_PHONE || WINDOWS_PHONE_7
            DisplayText = FormatValue(Value);
#endif
        }

        /// <summary>
        /// Gets or sets the culture format for the decimal values.
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Culture.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(SfCalculator), new PropertyMetadata(new CultureInfo("en-US")));


        /// <summary>
        /// Gets or sets the string to be displayed.
        /// </summary>
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register("DisplayText", typeof(string), typeof(SfCalculator), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Gets or sets the Value that is calculated by the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control.
        /// </summary>
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            internal set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(SfCalculator), new PropertyMetadata(0.0m, OnValueChanged));



        /// <summary>
        /// Gets or sets the data to be stored in the memory.
        /// </summary>
        public decimal Memory
        {
            get { return (decimal)GetValue(MemoryProperty); }
            internal set { SetValue(MemoryProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Memory.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MemoryProperty =
            DependencyProperty.Register("Memory", typeof(decimal), typeof(SfCalculator), new PropertyMetadata(0.0m));


        /// <summary>
        /// Gets or sets the string for the expression <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.Expression"/>
        /// </summary>
        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Expression.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExpressionProperty =
            DependencyProperty.Register("Expression", typeof(string), typeof(SfCalculator), new PropertyMetadata(String.Empty));



        /// <summary>
        /// Gets or sets the style for the Function Pane <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.FunctionsPane"/>
        /// </summary>
        public Style FunctionsPaneStyle
        {
            get { return (Style)GetValue(FunctionsPaneStyleProperty); }
            set { SetValue(FunctionsPaneStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FunctionsPaneStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FunctionsPaneStyleProperty =
            DependencyProperty.Register("FunctionsPaneStyle", typeof(Style), typeof(SfCalculator), new PropertyMetadata(null));



        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calc = sender as SfCalculator;
            if (calc != null)
            {
                calc.DisplayText = calc.FormatValue(e.NewValue);
            }
        }

        private string FormatValue(object objvalue)
        {
            var value = (decimal)objvalue;
            string display;

            if (value % 1 == 0)
            {
                display = value.ToString("N0", Culture);
            }
            else
            {
                int length = value.ToString().Split( Culture.NumberFormat.NumberDecimalSeparator.ToCharArray()[0])[1].Length;
                display = value.ToString("N" + length.ToString(), Culture);
            }
            return display;
        }

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/> control.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Functions = GetTemplateChild("PART_Functions") as FunctionsPane;
            if (PART_Functions != null)
            {
                PART_Functions.CalculatorKeyDown += PartInputCalculatorKeyDown;
                PART_Functions.Function += PartFunctionsFunction;
            }
            base.OnApplyTemplate();
        }

        private void PartFunctionsFunction(object sender, FunctionEventArgs e)
        {
#if SILVERLIGHT
            this.Focus();
#endif
            try
            {
                if (e.Function != CalculatorFunctions.Return && expressions.ToString() == string.Empty)
                {
                    previousFunction = CalculatorFunctions.None;
                    previousValue = Value;
                }
                if (DisplayText != CalcConstatnts.CInvalid && DisplayText != errorMessage)
                    ExecuteFunction(e.Function);               
                else if (e.Function == CalculatorFunctions.Clear || e.Function == CalculatorFunctions.ClearEntry)
                    ExecuteFunction(e.Function);
            }
            catch (DivideByZeroException)
            {
                Reset();
                DisplayText = CalcConstatnts.CError;
                ErrorDisplayArgs args = new ErrorDisplayArgs("Cannot Divide by Zero", null);
                if (ErrorMessageDisplayed != null)
                    ErrorMessageDisplayed(this, args);
                if (args.NewErrorMessage != null)
                    DisplayText = args.NewErrorMessage;
                errorMessage = DisplayText;
            }
        }

        /// <summary>
        /// Resets all the values and expression.
        /// </summary>
        public void Reset()
        {
            expressions.Clear();
            Expression = String.Empty;
            previousFunction = CalculatorFunctions.None;
            previousValue = 0;
        }

        void PartInputCalculatorKeyDown(object sender, KeyInputEventArgs e)
        {
            lastfunctionexecuted = false;
            if (expressions.ReadyForOperand && DisplayText != errorMessage)
            {
                Value = 0;
                expressions.ReadyForOperand = false;
            }
#if SILVERLIGHT
            this.Focus();
#endif
            ParseInput(e.Key);
        }

#if !WINRT
        private void ParseInput(Key key)
#else
        private void ParseInput(VirtualKey key)
#endif
        {
            decimal value = 0;
            string previousdecimaldisplay = null;
            if (DisplayText != CalcConstatnts.CInvalid && DisplayText != errorMessage)
            {
                if (Value < Decimal.MaxValue)
                {
#if WINRT
                bool shiftKey = false;
                var coreWindow = CoreWindow.GetForCurrentThread() as CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                if (shiftKey)
                {
                    if (key.ToString().Equals("Number8"))
                        ExecuteFunction(CalculatorFunctions.Multiply);
                    else if ((int)key == 187)
                        ExecuteFunction(CalculatorFunctions.Add);                    
                }
                else if (key.ToString().Equals("Multiply"))
                    ExecuteFunction(CalculatorFunctions.Multiply);
                else if (key.ToString().Equals("Add"))
                    ExecuteFunction(CalculatorFunctions.Add); 
                else if ((int)key == 187 || key.ToString().Equals("Enter"))
                    ExecuteFunction(CalculatorFunctions.Return);
                else if ((int)key == 189||key.ToString().Equals("Subtract"))
                    ExecuteFunction(CalculatorFunctions.Subract);
                else if ((int)key == 191||key.ToString().Equals("Divide"))
                    ExecuteFunction(CalculatorFunctions.Divide);
                else if ((int)key == 8)
                    ExecuteFunction(CalculatorFunctions.Back);
                else
#endif
#if WPF
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (key.ToString().Equals("D8"))
                        ExecuteFunction(CalculatorFunctions.Multiply);
                    else if (key.ToString().Equals("OemPlus"))
                        ExecuteFunction(CalculatorFunctions.Add);
                }
                else if (key.ToString().Equals("Multiply"))
                    ExecuteFunction(CalculatorFunctions.Multiply);
                else if (key.ToString().Equals("Add"))
                    ExecuteFunction(CalculatorFunctions.Add); 
                else if (key.ToString().Equals("OemPlus") || key.ToString().Equals("Return"))
                    ExecuteFunction(CalculatorFunctions.Return);
                else if (key.ToString().Equals("OemMinus") || key.ToString().Equals("Subtract"))
                    ExecuteFunction(CalculatorFunctions.Subract);
                else if (key.ToString().Equals("OemQuestion") || key.ToString().Equals("Divide"))
                    ExecuteFunction(CalculatorFunctions.Divide);
                else if (key.ToString().Equals("Back"))
                    ExecuteFunction(CalculatorFunctions.Back);   
                else
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7||SILVERLIGHT
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                    {
                        if ((int)key == 56)
                            ExecuteFunction(CalculatorFunctions.Multiply);
                        else if ((int)key == 187)
                            ExecuteFunction(CalculatorFunctions.Add);
                    }
                else if (key.ToString().Equals("Multiply"))
                    ExecuteFunction(CalculatorFunctions.Multiply);
                else if (key.ToString().Equals("Add"))
                    ExecuteFunction(CalculatorFunctions.Add); 
                    else if ((int)key == 187 || key.ToString().Equals("Enter"))
                        ExecuteFunction(CalculatorFunctions.Return);
                    else if ((int)key == 189 || key.ToString().Equals("Subtract"))
                        ExecuteFunction(CalculatorFunctions.Subract);
                    else if ((int)key == 191 || key.ToString().Equals("Divide"))
                        ExecuteFunction(CalculatorFunctions.Divide);
                    else if (key.ToString().Equals("Back"))
                        ExecuteFunction(CalculatorFunctions.Back);
                    else
#endif
                {
                        //Check whether input is a number
#if WINDOWS_PHONE || WINDOWS_PHONE_7
                if (key.ToString().StartsWith("D") && !key.ToString().Equals("Decimal"))
                {
                    value = (int)key - 20;
#elif WPF
                    if (key.ToString().StartsWith("NumPad") || (key.ToString().StartsWith("D")) && !key.ToString().Equals("Decimal"))
                {
                    if (key.ToString().StartsWith("NumPad"))
                        value = (int)key - 74;
                    else
                        value = (int)key - 34;
#elif SILVERLIGHT
                        if ((key.ToString().StartsWith("NumPad") || (key.ToString().StartsWith("D")) || key.ToString().Equals("F1")) && !key.ToString().Equals("Decimal"))
                        {
                            if (key.ToString().StartsWith("NumPad"))
                                value = (int)key - 68;
                            else if(key.ToString().Equals("F1"))
                                value=8;
                            else
                                value = (int)key - 20;
#else
                    if (key.ToString().StartsWith("Number"))
                    {
                        if (key.ToString().Contains("NumberPad"))
                            value = (int)key - 96;
                        else
                            value = (int)key - 48;
#endif
                            if (previousFunction == CalculatorFunctions.SquareRoot || previousFunction == CalculatorFunctions.Reciproc)
                            {
                                Reset();
                                Value = 0;
                            }
                            if (expressions.Any() && (expressions.Last().ToString().Contains(CalcConstatnts.CSquareRoot.Substring(0, 4)) || expressions.Last().ToString().Contains(CalcConstatnts.CReciproc.Substring(0, 7))))
                            {
                                expressions.RemoveAt(expressions.Count - 1);
                                DisplayText = value.ToString();
                                Value = 0;
                            }
                            if (Value.ToString().Length < (decimal.MaxValue).ToString().Length - 1)
                            {
                                //check whether it has decimal seperator
                                if (DisplayText.Contains(Culture.NumberFormat.NumberDecimalSeparator))
                                {
                                    previousdecimaldisplay = DisplayText.Split(Culture.NumberFormat.NumberDecimalSeparator.ToCharArray())[1];
                                    decimal decimalvalue = Value % 1;
                                    //if there is valid decimal points
                                    if (decimalvalue > 0)
                                    {
                                        string valuestring = decimalvalue.ToString() + value.ToString();
                                        var newdecimalvalue = Decimal.Parse(valuestring, NumberStyles.Number);
#if WINDOWS_PHONE 
                                Value = Math.Truncate(Value) + newdecimalvalue;
#elif SILVERLIGHT|| WINDOWS_PHONE_7
                                        string integralPart = DisplayText.Split(Culture.NumberFormat.NumberDecimalSeparator.ToCharArray())[0];
                                            Value = decimal.Parse(integralPart) + newdecimalvalue;
#else
                                    Value = Math.Truncate(Value) + newdecimalvalue;
#endif

                                        DisplayText = FormatValue(Value);
                                    }
                                    else //if there is no decimal points, but decimal sign
                                    {
                                        decimal _value = value / 10;
                                        if (value == 0 || previousdecimaldisplay.Length > 0 && Convert.ToInt32(previousdecimaldisplay) == 0)
                                        {
                                            Value = Decimal.Parse(DisplayText + value, NumberStyles.Number, Culture);
                                            if (value == 0)
                                                previousdecimaldisplay = previousdecimaldisplay + "0";
                                        }
                                        else
                                            Value = (Value) + _value;
                                    }
                                }
                                else //no decimal sign, add whole number
                                {
                                    string valuestring = Value.ToString() + value.ToString();
                                    Value = Decimal.Parse(valuestring, NumberStyles.Number, Culture);
                                }
                            }
                            if (percent && expressions.Count >= 2)
                            {
                                expressions.Remove(expressions[expressions.Count - 1]);
                            }
                            if (expressions.Count == 1)
                            {
                                Reset();
                            }
                            percent = false;
                            if (DisplayText == Value.ToString() && expressions.Count == 0)
                                Reset();
                        }
                    }
                }

#if WPF
            if (key == Key.Decimal || (int)key == 190||key==Key.OemPeriod)
#elif !WINRT
                if (key == Key.Decimal || (int)key == 190)
#else
                if (key == VirtualKey.Decimal || (int)key == 190 || (int)key == 110)
#endif
                {
                    if (!DisplayText.Contains(Culture.NumberFormat.NumberDecimalSeparator))
                    {
                        DisplayText = FormatValue(Value) + Culture.NumberFormat.NumberDecimalSeparator;
                    }
                }


#if WINDOWS_PHONE || WINDOWS_PHONE_7
            if ((int)key == 20 && !string.IsNullOrEmpty(previousdecimaldisplay))
#elif SILVERLIGHT
                if (((int)key==20 || (int)key == 68) && !string.IsNullOrEmpty(previousdecimaldisplay))
#elif WPF
            if (((int)key==34 || (int)key == 74) && !string.IsNullOrEmpty(previousdecimaldisplay))
#else
            if ((int)key == 48 && !string.IsNullOrEmpty(previousdecimaldisplay))
#endif
                {
                    double roundedvalue = Math.Round(Convert.ToDouble(Culture.NumberFormat.NumberDecimalSeparator + previousdecimaldisplay), 2);
                    if (roundedvalue.Equals(0))
                        DisplayText = FormatValue(Value) + Culture.NumberFormat.NumberDecimalSeparator + previousdecimaldisplay;
                }
            }
        }

        /// <summary>
        /// Occurs when the key has been pressed
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
#if WPF
            if (e.Key.ToString().Equals("Return"))
#elif SILVERLIGHT
            if (e.Key.ToString().Equals("Enter"))
#endif
#elif WINRT
        protected override void OnKeyUp(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key.ToString().Equals("Enter"))
#endif
                ParseInput(e.Key);
#if SILVERLIGHT
            this.Focus();
#endif
            base.OnKeyUp(e);
        }

#if SILVERLIGHT
        /// <summary>
        /// Occurs when the focus is obtained through mouse.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Focus();
            base.OnMouseLeftButtonDown(e);
        }
#endif

        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
#if WPF
            if (!e.Key.ToString().Equals("Return") && expressions.ToString() == string.Empty)
            {
                previousFunction = CalculatorFunctions.None;
                previousValue = Value;
            }
            if (DisplayText != CalcConstatnts.CInvalid && DisplayText != errorMessage && !e.Key.ToString().Equals("Return"))
#elif SILVERLIGHT
            if (!e.Key.ToString().Equals("Enter") && expressions.ToString() == string.Empty)
            {
                previousFunction = CalculatorFunctions.None;
                previousValue = Value;
            }
            if (DisplayText != CalcConstatnts.CInvalid && DisplayText != errorMessage && !e.Key.ToString().Equals("Enter"))
#endif
            {
                if (expressions.ReadyForOperand)
                {
                    Value = 0;
                    expressions.ReadyForOperand = false;
                }
#if WINDOWS_PHONE || WINDOWS_PHONE_7||SILVERLIGHT
                if (e.PlatformKeyCode == 56 || e.PlatformKeyCode == 187 || e.PlatformKeyCode == 189 || e.PlatformKeyCode == 191 || e.PlatformKeyCode == 190)
                    ParseInput((Key)e.PlatformKeyCode);
                else
#endif
                    ParseInput(e.Key);
#else
        protected override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            lastfunctionexecuted = false;
            if (!e.Key.ToString().Equals("Enter") && expressions.ToString() == string.Empty)
            {
                previousFunction = CalculatorFunctions.None;
                previousValue = Value;
            }
            if (DisplayText != CalcConstatnts.CInvalid && DisplayText != errorMessage && !e.Key.ToString().Equals("Enter"))
            {
                if (expressions.ReadyForOperand && e.Key.ToString().StartsWith("Number"))
                {
                    Value = 0;
                    expressions.ReadyForOperand = false;
                }
                ParseInput(e.Key);
#endif
#if SILVERLIGHT
                this.Focus();
#endif
                base.OnKeyDown(e);
            }
        }

        private void ExecuteFunction(CalculatorFunctions function)
        {
            switch (function)
            {
                case CalculatorFunctions.Add:
                    Add(Value);
                    break;
                case CalculatorFunctions.Subract:
                    Subract(Value);
                    break;
                case CalculatorFunctions.Return:
                    Return(Value);
                    break;
                case CalculatorFunctions.Multiply:
                    Multiply(Value);
                    break;
                case CalculatorFunctions.Divide:
                    Divide(Value);
                    break;
                case CalculatorFunctions.SquareRoot:
                    SquareRoot(Value);
                    break;
                case CalculatorFunctions.Reciproc:
                    Reciproc(Value);
                    break;
                case CalculatorFunctions.Percentage:
                    Percentage(Value);
                    break;
                case CalculatorFunctions.Clear:
                    Clear();
                    break;
                case CalculatorFunctions.ClearEntry:
                    ClearEntry();
                    break;
                case CalculatorFunctions.Back:
                    Back();
                    break;
                case CalculatorFunctions.Memory:
                    EnterMemory(Value);
                    break;
                case CalculatorFunctions.MemoryAdd:
                    AddMemory(Value);
                    break;
                case CalculatorFunctions.MemoryClear:
                    ClearMemory();
                    break;
                case CalculatorFunctions.MemoryRecall:
                    RecaallMemory();
                    break;
                case CalculatorFunctions.MemorySubract:
                    SubractMemory(Value);
                    break;
                case CalculatorFunctions.Sign:
                    Sign(Value);
                    break;
            }
        }

        private decimal Evaluate(decimal value)
        {
            try
            {
                if (previousFunction == CalculatorFunctions.Add)
                {
                    return previousValue + value;
                }
                else if (previousFunction == CalculatorFunctions.Subract)
                {
                    if (Value == decimal.Parse(DisplayText) && expressions.Count == 0)
                        return value - previousValue;
                    else
                        return previousValue - value;
                }
                else if (previousFunction == CalculatorFunctions.Multiply)
                {
                    return previousValue * value;
                }
                else if (previousFunction == CalculatorFunctions.Divide)
                {
                    if (DisplayText == Value.ToString() && expressions.Count == 0)
                        return value / previousValue;
                    else
                        return previousValue / value;
                }
                else
                {
                    return value;
                }
            }
            catch (OverflowException)
            {
                Reset();
                DisplayText = CalcConstatnts.CError;
                ErrorDisplayArgs args = new ErrorDisplayArgs("Value exceeds the maximum length of the decimal type", null);
                if (ErrorMessageDisplayed != null)
                    ErrorMessageDisplayed(this, args);
                if (args.NewErrorMessage != null)
                    DisplayText = args.NewErrorMessage;
                errorMessage = DisplayText;
                return value;
            }
        }

        private void Sign(decimal value)
        {
            Value = -(value);
        }

        /// <summary>
        /// Validates the operators in the expression.
        /// </summary>
        public void CheckOperators()
        {
            if (lastfunctionexecuted && expressions.Count > 0)
            {
                expressions.RemoveAt(expressions.Count - 1);
                previousFunction = CalculatorFunctions.None;
            }
            else
            {
                ValidateExpression();
            }
            lastfunctionexecuted = true;
        }

        private void Add(decimal value)
        {
            //Evaluate the expression
            if (previousFunction == CalculatorFunctions.SquareRoot || previousFunction == CalculatorFunctions.Reciproc)
            {
                Value = previousValue;
            }
            else
            {
                CheckOperators();
                evaluatedValue = Evaluate(value);
                if (DisplayText != errorMessage)
                    Value = evaluatedValue;
            }

            if (DisplayText != errorMessage)
            {
                //Set expression string
                expressions.Add(CalcConstatnts.CAdd);
                Expression = expressions.ToString();


                //Set previous values.
                previousFunction = CalculatorFunctions.Add;
                previousValue = Value;
                TrailingZeros();
            }
            expressions.ReadyForOperand = true;
        }

        private void Subract(decimal value)
        {
            //Evaluate the expression
            if (previousFunction == CalculatorFunctions.SquareRoot || previousFunction == CalculatorFunctions.Reciproc)
            {
                Value = previousValue;
            }
            else
            {
                CheckOperators();           
                evaluatedValue = Evaluate(value);
                if (DisplayText != errorMessage)
                    Value = evaluatedValue;
            }

            if (DisplayText != errorMessage)
            {
                //Set expression string
                expressions.Add(CalcConstatnts.CMinus);
                Expression = expressions.ToString();

                //Set previous values.
                previousFunction = CalculatorFunctions.Subract;
                previousValue = Value;
                TrailingZeros();
            } 
            expressions.ReadyForOperand = true;
        }

        private void Multiply(decimal value)
        {
            //Evaluate the expression
            if (previousFunction == CalculatorFunctions.SquareRoot || previousFunction == CalculatorFunctions.Reciproc)
            {
                Value = previousValue;
            }
            else
            {
                CheckOperators();
                evaluatedValue = Evaluate(value);
                if (DisplayText != errorMessage)
                    Value = evaluatedValue;
            }

            if (DisplayText != errorMessage)
            {
                //Set expression string
                expressions.Add(CalcConstatnts.CMultiply);
                Expression = expressions.ToString();

                //Set previous values.
                previousFunction = CalculatorFunctions.Multiply;
                previousValue = Value;
                TrailingZeros();
            }
            expressions.ReadyForOperand = true;
        }

        private void Divide(decimal value)
        {
            //Evaluate the expression
            if (previousFunction == CalculatorFunctions.SquareRoot || previousFunction == CalculatorFunctions.Reciproc)
            {
                Value = previousValue;
            }
            else
            {
                CheckOperators();           
                evaluatedValue = Evaluate(value);
                if (DisplayText != errorMessage)
                    Value = evaluatedValue;
            }

            if (DisplayText != errorMessage)
            {
                //Set expression string
                expressions.Add(CalcConstatnts.CDivide);
                Expression = expressions.ToString();
                
                //Set previous values.
                previousFunction = CalculatorFunctions.Divide;
                previousValue = Value;
                TrailingZeros();
            }
            expressions.ReadyForOperand = true;
        }

        private void Percentage(decimal value)
        {
            //Evaluate value
            if (expressions.Count >= 2 && expressions[expressions.Count - 1].ToString() != Value.ToString() && DisplayText == previousValue.ToString())
            {
                expressions.Remove(expressions[expressions.Count - 1]);
                expressions.Remove(expressions[expressions.Count - 1]);
                previousFunction = CalculatorFunctions.None;
                if (expressions.Count == 0)
                    previousValue = 0;
            }
            if(expressions.Count==0 && previousFunction!=CalculatorFunctions.None)
                previousValue = value;
            if (expressions.Any() && (expressions.Last().ToString().Contains(CalcConstatnts.CReciproc.Substring(0, 7)) || expressions.Last().ToString().Contains(CalcConstatnts.CSquareRoot.Substring(0, 4)) || percent))
            {
                expressions.Remove(expressions[expressions.Count - 1]);
                previousFunction = CalculatorFunctions.Percentage;
            }
            if (expressions.Count == 0 && previousFunction == CalculatorFunctions.None)
                Value = 0;
            else
                Value = (value / 100) * previousValue;

            //Set expression string
            TrailingZeros();
            expressions.Add(DisplayText);
            if(expressions.Count>=2)
                previousFunction = CheckFunction(expressions[expressions.Count - 2].ToString());
            Expression = expressions.ToString();
            expressions.ReadyForOperand = true;
            percent = true;
        }

        private void Return(decimal value)
        {
           //Evaluate the expression          
            evaluatedValue = Evaluate(value);
            if (DisplayText != errorMessage)
                Value = evaluatedValue;

            TrailingZeros();
            //Reset the expression
            if (DisplayText == value.ToString())
                Reset();
            else
            {
                expressions.Clear();
                Expression = String.Empty;
            }
            if (DisplayText == errorMessage)
                expressions.ReadyForOperand = false;
            else
                expressions.ReadyForOperand = true;
        }

        private void SquareRoot(decimal value)
        {
            //Set expression string
            string strvalue;
            if (expressions.Count == 2 && expressions[expressions.Count - 1].ToString() != Value.ToString() && DisplayText == previousValue.ToString())
            {
                expressions.Remove(expressions[expressions.Count - 1]);
                expressions.Remove(expressions[expressions.Count - 1]);
                previousFunction = CalculatorFunctions.None;
            }
            if (expressions.Any() && (expressions.Last().ToString().Contains(CalcConstatnts.CReciproc.Substring(0, 7)) || expressions.Last().ToString().Contains(CalcConstatnts.CSquareRoot.Substring(0, 4))||percent))
            {
                strvalue = expressions.Last().ToString();
                expressions.RemoveAt(expressions.Count - 1);
            }
            else
                strvalue = value.ToString();
            expressions.Add(String.Format(CalcConstatnts.CSquareRoot, strvalue));
            Expression = expressions.ToString();

            //Evaluate the expression
            if (Value.ToString().Contains("-"))
                DisplayText = CalcConstatnts.CInvalid;
            else
                Value = (decimal)Math.Sqrt((double)value);

            //Set previous values.
            if (expressions.Count >= 2)
            {
                previousFunction = CheckFunction(expressions[expressions.Count - 2].ToString());
            }
            else
            {
                previousValue = Evaluate(Value);
                previousFunction = CalculatorFunctions.SquareRoot;
            }
            TrailingZeros();
        }

        private void Reciproc(decimal value)
        {
            //Set expression string
            string strvalue;
            if (expressions.Count == 2 && expressions[expressions.Count - 1].ToString() != Value.ToString() && DisplayText == previousValue.ToString())
            {
                expressions.Remove(expressions[expressions.Count - 1]);
                expressions.Remove(expressions[expressions.Count - 1]);
                previousFunction = CalculatorFunctions.None;
            }
            if (expressions.Any() && (expressions.Last().ToString().Contains(CalcConstatnts.CReciproc.Substring(0, 7)) || expressions.Last().ToString().Contains(CalcConstatnts.CSquareRoot.Substring(0, 4))||percent))
            {
                strvalue = expressions.Last().ToString();
                expressions.RemoveAt(expressions.Count - 1);
            }
            else
                strvalue = value.ToString();           
            expressions.Add(String.Format(CalcConstatnts.CReciproc, strvalue));
            Expression = expressions.ToString();

            //Evaluate the expression
            Value = 1 / value;

            //Set previous values.
            if (expressions.Count >= 2)
            {
                previousFunction = CheckFunction(expressions[expressions.Count - 2].ToString());
            }
            else
            {
                previousValue = Evaluate(Value);
                previousFunction = CalculatorFunctions.Reciproc;
            }
            TrailingZeros();
        }

        /// <summary>
        /// Removes all the trailing zeros in the decimal value.
        /// </summary>
        public void TrailingZeros()
        {
            if (DisplayText.Contains(Culture.NumberFormat.NumberDecimalSeparator))
            {
                   DisplayText = DisplayText.ToString().TrimEnd('0', Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator));
            }
        }

        /// <summary>
        /// Validates the expression
        /// </summary>
        public void ValidateExpression()
        {
            if (expressions.Any() && (expressions.Last().ToString().Contains(CalcConstatnts.CSquareRoot.Substring(0, 4)) || expressions.Last().ToString().Contains(CalcConstatnts.CReciproc.Substring(0, 7))))
            {
                expressions.RemoveAt(expressions.Count - 1);
            }
            if (!percent)
            {
                if (DisplayText.Contains(Culture.NumberFormat.NumberDecimalSeparator))
                    expressions.Add(Value.ToString().TrimEnd('0', '.'));
                else
                    expressions.Add(Value);
            }
            percent = false;
        }

        /// <summary>
        /// Identifies the function given as input
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public CalculatorFunctions CheckFunction(string s)
        {
            if (s.Equals(CalcConstatnts.CAdd))
                return CalculatorFunctions.Add;
            else if (s.Equals(CalcConstatnts.CMinus))
                return CalculatorFunctions.Subract;
            else if (s.Equals(CalcConstatnts.CMultiply))
                return CalculatorFunctions.Multiply;
            else if (s.Equals(CalcConstatnts.CDivide))
                return CalculatorFunctions.Divide;
            else if (s.Equals(CalcConstatnts.CReciproc))
                return CalculatorFunctions.Reciproc;
            else if (s.Equals(CalcConstatnts.CSquareRoot))
                return CalculatorFunctions.SquareRoot;
            else
                return CalculatorFunctions.None;
        }
        private void Clear()
        {
            Value = 0;
#if !WINRT
            DisplayText = FormatValue(Value);
#endif

            Reset();
        }

        private void ClearEntry()
        {
            if (DisplayText == CalcConstatnts.CInvalid || DisplayText == errorMessage)
                Reset();
            Value = 0;
#if !WINRT
            DisplayText = FormatValue(Value);
#endif

        }

        private void EnterMemory(decimal value)
        {
            Memory = value;
            expressions.ReadyForOperand = true;
            TrailingZeros();
        }

        private void AddMemory(decimal value)
        {
            Memory += value;
            expressions.ReadyForOperand = true;
            TrailingZeros();
        }

        private void SubractMemory(decimal value)
        {
            Memory -= value;
            expressions.ReadyForOperand = true;
            TrailingZeros();
        }

        private void RecaallMemory()
        {
            Value = Memory;
            expressions.ReadyForOperand = true;
            TrailingZeros();
        }

        private void ClearMemory()
        {
            Memory = 0;
            expressions.ReadyForOperand = true;
        }

        private void Back()
        {
            if (DisplayText.Length >= 1)
            {
                string display = DisplayText.Substring(0, DisplayText.Length - 1);
                if (String.IsNullOrEmpty(display))
                {
                    Value = 0;
                }
                else
                {
                    Decimal dec;                   
                    if (Decimal.TryParse(display, out dec))
                    {
                        if (DisplayText.Contains(Culture.NumberFormat.NumberDecimalSeparator) && DisplayText[DisplayText.Length - 1] != '.')
                        {
                            string[] displayArray = display.Split(Culture.NumberFormat.NumberDecimalSeparator.ToCharArray());
                            string previousdecimaldisplay = displayArray[1];
                            string integralPart = displayArray[0];
                            if (previousdecimaldisplay != string.Empty && decimal.Parse(previousdecimaldisplay) == 0)
                            {
                                Value = decimal.Parse(integralPart + Culture.NumberFormat.NumberDecimalSeparator + previousdecimaldisplay);
                                DisplayText = Value.ToString();
                            }
                            else
                            {
                                Value = Decimal.Parse(display);
#if !WINRT
                                DisplayText=Value.ToString();
#endif
                            }
                            if (previousdecimaldisplay == string.Empty)
                                DisplayText = Value.ToString() + Culture.NumberFormat.NumberDecimalSeparator;
                        }
                        else
                        {
                            Value = Decimal.Parse(display);
                            DisplayText = Value.ToString();
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Reprsents a class for the arrguments in Error Display.
    /// </summary>
    public class ErrorDisplayArgs : RoutedEventArgs
    {
        private string errorDetails, newErrorMessage;
        /// <summary>
        /// Initializes a new instance of the ErrorDisplayArgs class.
        /// </summary>
        /// <param name="_errorDetails"></param>
        /// <param name="_newErrorMessage"></param>
        public ErrorDisplayArgs(string _errorDetails, string _newErrorMessage)
        {
            errorDetails = _errorDetails;
            newErrorMessage = _newErrorMessage;
        }
        /// <summary>
        /// Gets the Error Details
        /// </summary>
        public string ErrorDetails { get { return errorDetails; } }
        /// <summary>
        /// Gets or sets the NewErrorMessage
        /// </summary>
        public string NewErrorMessage 
        { 
            get { return newErrorMessage; }
            set { newErrorMessage = value; }
        }
    }
}
