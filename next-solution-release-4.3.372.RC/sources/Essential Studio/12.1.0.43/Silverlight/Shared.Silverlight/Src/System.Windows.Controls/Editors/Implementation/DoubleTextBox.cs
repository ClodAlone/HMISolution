#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

#if WPF

using Syncfusion.Licensing;
using System.Windows.Data;

#endif

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
#if WPF

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
 Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Blend;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Default;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Windows7;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.VS2010;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Metro;component/Editors/DoubleTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(DoubleTextBox), XamlResource = "/Syncfusion.Theming.Transparent;component/Editors/DoubleTextBox.xaml")]
#endif
    public class DoubleTextBox : EditorBase
    {
#if WPF

        public event PropertyChangedCallback ValidationCompletedChanged;

        public event PropertyChangedCallback InvalidValueBehaviorChanged;

        public event StringValidationCompletedEventHandler ValueValidationCompleted;

        public event PropertyChangedCallback ValidationValueChanged;

        public event CancelEventHandler Validating;

        public event EventHandler Validated;

#endif

        #region Events

        /// <summary>
        /// Delegate used to handle the ValueChanging event
        /// </summary>
        public delegate void ValueChangingEventHandler(object sender, ValueChangingEventArgs e);

        /// <summary>
        /// Event that is raised when <see cref="MinValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised before the <see cref="Value"/> property is changed.
        /// </summary>
        public event ValueChangingEventHandler ValueChanging;

        /// <summary>
        /// Event that is raised when <see cref="MaxValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberDecimalDigits"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberDecimalDigitsChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberDecimalSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberDecimalSeparatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberGroupSizes"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberGroupSizesChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberGroupSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberGroupSeparatorChanged;

        internal event PropertyChangedCallback IsExceedDecimalDigitsChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MinimumNumberDecimalDigitsChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MaximumNumberDecimalDigitsChanged;

        #endregion Events

        #region Members

        internal double? OldValue;
        internal double? mValue;
        internal bool? mValueChanged = true;
        internal bool mIsLoaded = false;
        internal int count = 1;
        internal bool negativeFlag = false;
        internal string checktext = "";
        internal int numberDecimalDigits;
        internal string lostfocusmasktext;

        #endregion Members

        #region Constructor

#if WPF

        static DoubleTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleTextBox), new FrameworkPropertyMetadata(typeof(DoubleTextBox)));
        }

#endif

        /// <summary>
        ///
        /// </summary>
        public ICommand pastecommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ICommand copycommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ICommand cutcommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public DoubleTextBox()
        {
            pastecommand = new DelegateCommand<object>(_pastecommand, Canpaste);
            copycommand = new DelegateCommand<object>(_copycommand, Canpaste);
            cutcommand = new DelegateCommand<object>(_cutcommand, Canpaste);
#if WPF
            this.AddHandler(CommandManager.PreviewExecutedEvent,
new ExecutedRoutedEventHandler(CommandExecuted), true);
#endif

#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(DoubleTextBox));
            }
#endif

#if SILVERLIGHT
            this.DefaultStyleKey = typeof(DoubleTextBox);
#endif
            this.Loaded += DoubleTextbox_Loaded;
        }

        #endregion Constructor

        private void _pastecommand(object parameter)
        {
            Paste();
        }

        private void _copycommand(object parameter)
        {
            copy();
        }

        private void _cutcommand(object parameter)
        {
            cut();
        }

        private bool Canpaste(object parameter)
        {
            return true;
        }

        #region overide

        private ScrollViewer PART_ContentHost;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.IsExceedDecimalDigits)
            {
                if (this.NumberDecimalDigits < 0)
                {
                    throw new InvalidOperationException("NumberDecimalDigits must be a postive value.current value -1");
                }
            }
#if WPF
            PART_ContentHost = this.GetTemplateChild("PART_ContentHost") as ScrollViewer;
#endif
#if SILVERLIGHT
            PART_ContentHost = this.GetTemplateChild("ContentElement") as ScrollViewer;
#endif
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseWheel"/> event occurs to provide handling for the event in a derived class without attaching a delegate.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (this.IsScrollingOnCircle)
            {
                if (e.Delta > 0)
                {
                    DoubleValueHandler.doubleValueHandler.HandleUpKey(this);
                }
                else if (e.Delta < 0)
                {
                    DoubleValueHandler.doubleValueHandler.HandleDownKey(this);
                }
            }
        }

#if WPF

        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Paste)
            {
                Paste();
                e.Handled = true;
            }
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Cut)
            {
                cut();
                e.Handled = true;
            }
        }

#endif

#if WPF

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = DoubleValueHandler.doubleValueHandler.HandleKeyDown(this, e);

            if (e.Key == Key.Z)
            {
                if (IsUndoEnabled)
                {
                    this.SetValue(true, this.OldValue);
                }
                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }

#endif

        /// <summary>
        /// Called when <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.V)
                {
                    Paste();
                    e.Handled = true;
                }
                

                if (e.Key == Key.X)
                {
                    cut();
                    e.Handled = true;
                }
            }
#if WPF
            else if (e.Key == Key.Return)
            {
                if (this.EnterToMoveNext)
                {
                    FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
                    TraversalRequest request = new TraversalRequest(focusDirection);
                    UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                    if (elementWithFocus != null)
                    {
                        elementWithFocus.MoveFocus(request);
                    }
                }
                e.Handled = true;
            }
#endif
            else
            {
                e.Handled = DoubleValueHandler.doubleValueHandler.HandleKeyDown(this, e);
            }
            base.OnKeyDown(e);
        }

        private void cut()
        {
            try
            {
                if (this.SelectionLength > 0)
                {
                    Clipboard.SetText(this.SelectedText);
                    this.count = 1;
                    DoubleValueHandler.doubleValueHandler.HandleDeleteKey(this);
                }
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            e.Handled = DoubleValueHandler.doubleValueHandler.MatchWithMask(this, e.Text);
            base.OnTextInput(e);
        }

        internal override void OnCultureChanged()
        {
            base.OnCultureChanged();
            if (this.mIsLoaded)
            {
                this.FormatText();
            }
        }

        internal override void OnNumberFormatChanged()
        {
            base.OnNumberFormatChanged();
            if (this.mIsLoaded)
            {
                this.FormatText();
            }
        }

#if WPF

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_validatingrResult)
            {
                e.Handled = true;
            }
            base.OnPreviewLostKeyboardFocus(e);
        }

#endif

        private bool _validatingrResult = false;

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if WPF
            if (!OnValidating(new CancelEventArgs(false)))
            {
                string validationerror = "";
                bool validationstatus = true;

                if (ValidationValue == this.Value.ToString())
                    validationstatus = true;
                else
                    validationstatus = false;

                string message = validationstatus ? "String validation succeeded" : "String validation failed";

                if (!validationstatus)
                {
                    if (InvalidValueBehavior == InvalidInputBehavior.DisplayErrorMessage)
                    {
                        MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, ValidationValue));
                        OnValidated(EventArgs.Empty);
                    }
                    else if (InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                    {
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, ValidationValue));
                        OnValidated(EventArgs.Empty);
#if WPF
#if SyncfusionFramework3_5
                        this.Value = null;
#elif SyncfusionFramework4_0
                        this.SetCurrentValue(ValueProperty, null);
#endif
#endif
                    }
                    else if (InvalidValueBehavior == InvalidInputBehavior.None)
                    {
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, ValidationValue));
                        OnValidated(EventArgs.Empty);
                    }
                }
                else
                {
                    OnValidated(EventArgs.Empty);
                }
            }
#endif
            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.Background;

            double? Val = this.Value;
#if WPF
            if (mIsLoaded)
            {
                if (this.ValidationValue == this.Value.ToString())
                    ValidationCompleted = true;
                else
                    ValidationCompleted = false;
            }

#endif
            if (Val != null)
            {
                if (Val > this.MaxValue)
                {
                    Val = this.MaxValue;
                }
                else if (mValue < this.MinValue)
                {
                    Val = this.MinValue;
                }
                if (Val != this.Value)
                {
                    bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = Val });
                    if (!cancelValueChanging)
                        this.Value = Val;
                }
            }
            if (this.MaskedText.Length >= 15 && this.MaxValidation == MaxValidation.OnLostFocus)
            {
                lostfocusmasktext = this.MaskedText;
                NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                double newvalue = double.Parse(this.MaskedText);
                this.MaskedText = newvalue.ToString("N", numberFormat);
                this.Value = newvalue;
            }
            base.OnLostFocus(e);
            this.checktext = "";
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.FocusedBackground;
            try
            {
                if (this.MaskedText.Length >= 15 && this.MaxValidation == MaxValidation.OnLostFocus)
                {
                    this.MaskedText = lostfocusmasktext;
                }
            }
            catch { }
            base.OnGotFocus(e);
        }

        #endregion overide

        #region Internal Methods

        internal void FormatText()
        {
            if (this.Value != null && !(double.IsNaN((double)this.Value)))
            {
                if (mValue == 0 && this.Value == null)
                {
                    if (this.UseNullOption)
                        this.SetValue(true, null);
                    else
                    {
                        this.SetValue(true, 0.0);
                        NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                        this.MaskedText = ((double)mValue).ToString("N", numberFormat);
                    }
                }
                else
                {
                    NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                    this.MaskedText = ((double)mValue).ToString("N", numberFormat);
                }
            }
            else
            {
                this.MaskedText = "";
            }
        }

        internal bool SetValue(bool? IsReload, double? _Value)
        {
            NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
            if (IsReload == false)
            {
                mValueChanged = false;

                bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = _Value });
                if (!cancelValueChanging)
                {
                    this.Value = _Value;
                    mValueChanged = true;
                    return true;
                }
                else
                {
                    if (this.Value != null)
                    {
                        double val = (double)this.Value;
                        this.MaskedText = val.ToString("N", numberFormat);
                    }
                    return true;
                }
            }
            else if (IsReload == true)
            {
                bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = _Value });
                if (!cancelValueChanging)
                {
                    var caretindex = this.CaretIndex;
                    this.Value = _Value;
                    this.CaretIndex = caretindex;
                    return true;
                }
                else
                {
                    if (this.Value != null)
                    {
                        double val = (double)this.Value;
                        this.MaskedText = val.ToString("N", numberFormat);
                    }
                    return true;
                }
            }
            return false;
        }

        internal bool TriggerValueChangingEvent(ValueChangingEventArgs args)
        {
            if (this.ValueChanging != null)
            {
                ValueChanging(this, args);
                return args.Cancel;
            }
            return false;
        }

        internal double? ValidateValue(double? Val)
        {
            if (Val != null)
            {
                if (Val > this.MaxValue)
                {
                    Val = this.MaxValue;
                }
                else if (mValue < this.MinValue)
                {
                    Val = this.MinValue;
                }
            }
            return Val;
        }

        internal CultureInfo GetCulture()
        {
            CultureInfo cultureInfo;
            if (Culture != null && Culture != CultureInfo.InvariantCulture)
                cultureInfo = this.Culture.Clone() as CultureInfo;
            else
                cultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;

            if (NumberFormat != null)
                cultureInfo.NumberFormat = NumberFormat;

            if (!GroupSeperatorEnabled)
            {
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
            }

            if (GroupSeperatorEnabled == true)
            {
#if WPF
                if (!NumberGroupSeparator.Equals(string.Empty) && cultureInfo.NumberFormat.NumberGroupSeparator != NumberGroupSeparator)
                    cultureInfo.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
#else
                if (!this.NumberGroupSeparator.Equals(string.Empty) && !(char.IsLetterOrDigit(this.NumberGroupSeparator, 0)) &&
                    this.NumberGroupSeparator.Length == 1 && (cultureInfo.NumberFormat.NumberDecimalSeparator != this.NumberDecimalSeparator ||
                    cultureInfo.NumberFormat.NumberGroupSeparator != NumberGroupSeparator))
                    cultureInfo.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
#endif
            }

            if (NumberDecimalDigits >= 0 && cultureInfo.NumberFormat.NumberDecimalDigits != this.NumberDecimalDigits)
                cultureInfo.NumberFormat.NumberDecimalDigits = this.NumberDecimalDigits;

            if (!NumberDecimalSeparator.Equals(string.Empty) && cultureInfo.NumberFormat.NumberDecimalSeparator != this.NumberDecimalSeparator)
                cultureInfo.NumberFormat.NumberDecimalSeparator = this.NumberDecimalSeparator;

#if SILVERLIGHT
            if (NumberGroupSizes != null)
                cultureInfo.NumberFormat.NumberGroupSizes = this.NumberGroupSizes;
#endif

#if WPF
            int count = this.NumberGroupSizes.Count;
            if (count > 0)
            {
                int[] ngs = new int[count];

                for (int i = 0; i < count; i++)
                {
                    ngs[i] = this.NumberGroupSizes[i];
                }
                cultureInfo.NumberFormat.NumberGroupSizes = ngs;
            }
#endif

            return cultureInfo;
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            DoubleTextBox doubleTextBox = (DoubleTextBox)d;
            if (baseValue != null)
            {
                double? value = (double?)baseValue;
                if (doubleTextBox.mValueChanged == true)
                {
                    if (value > doubleTextBox.MaxValue)
                    {
                        value = doubleTextBox.MaxValue;
                    }
                    else if (value < doubleTextBox.MinValue)
                    {
                        value = doubleTextBox.MinValue;
                    }
                }
                if (value != null)
                {
                    doubleTextBox.IsNegative = value < 0 ? true : false;
                    doubleTextBox.IsZero = value == 0 ? true : false;
                    doubleTextBox.IsNull = false;
                }
                if ((doubleTextBox.NumberDecimalDigits > 0 || doubleTextBox.MaximumNumberDecimalDigits > 0) || doubleTextBox.numberDecimalDigits > 0)
                {
                    int count = 0;
                    NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
                    if (baseValue.ToString().Contains(numberFormat.NumberDecimalSeparator))
                    {
                        int baseCount = CountDecimalDigits(baseValue.ToString(), d);
                        int maskCount = CountDecimalDigits(doubleTextBox.MaskedText, d);
                        if (!string.IsNullOrEmpty(doubleTextBox.MaskedText) && doubleTextBox.MaskedText != "DoubleTextBox")
                        {
                            DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                            if ((baseCount != 0 && baseValue.ToString()[baseValue.ToString().Length - 1] != '0' && doubleTextBox.MaskedText[doubleTextBox.MaskedText.Length - 1] != '0') || (baseCount > maskCount))
                            {
                                if (doubleTextBox.MaximumNumberDecimalDigits == -1 && doubleTextBox.MinimumNumberDecimalDigits == -1)
                                {
                                    baseCount = doubleTextBox.numberDecimalDigits;
                                }
                                else if (doubleTextBox.NumberDecimalDigits > 0 && doubleTextBox.MaximumNumberDecimalDigits >= 0 && doubleTextBox.MinimumNumberDecimalDigits == -1)
                                {
                                    if (baseCount >= doubleTextBox.MaximumNumberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.MaximumNumberDecimalDigits;
                                    }
                                    if (baseCount <= doubleTextBox.numberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.numberDecimalDigits;
                                    }
                                }
                                else if (doubleTextBox.NumberDecimalDigits > 0 && doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits == -1)
                                {
                                    if (baseCount >= doubleTextBox.numberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.numberDecimalDigits;
                                    }
                                    if (baseCount <= doubleTextBox.MinimumNumberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.MinimumNumberDecimalDigits;
                                    }
                                }
                                else if (doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits >= 0 && doubleTextBox.NumberDecimalDigits > 0)
                                {
                                    if (baseCount >= doubleTextBox.MaximumNumberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.MaximumNumberDecimalDigits;
                                    }
                                    if (baseCount <= doubleTextBox.MinimumNumberDecimalDigits)
                                    {
                                        baseCount = doubleTextBox.MinimumNumberDecimalDigits;
                                    }
                                }
                                doubleTextBox.NumberDecimalDigits = baseCount;
                            }
                            if (baseCount == 0)
                            {
                                if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                                {
                                    doubleTextBox.NumberDecimalDigits = 0;
                                }
                            }
                            DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                        }
                        else if ((doubleTextBox.MaskedText == "" || doubleTextBox.MaskedText == "DoubleTextBox") && baseValue != null && doubleTextBox.MaximumNumberDecimalDigits > doubleTextBox.NumberDecimalDigits)
                        {
                            DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                            count = CountDecimalDigits(baseValue.ToString(), d);
                            if (count >= doubleTextBox.MaximumNumberDecimalDigits)
                            {
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MaximumNumberDecimalDigits;
                            }
                            else if (count <= doubleTextBox.MinimumNumberDecimalDigits)
                            {
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            }
                            else
                            {
                                if (doubleTextBox.NumberDecimalDigits != doubleTextBox.numberDecimalDigits)
                                    doubleTextBox.NumberDecimalDigits = count;
                            }
                            DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                        }
                    }
                    else if (doubleTextBox.MaskedText.Contains(numberFormat.NumberDecimalSeparator))
                    {
                        DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                        int maskCount = CountDecimalDigits(doubleTextBox.MaskedText, d);
                        double result;
                        if (Double.TryParse(doubleTextBox.MaskedText, out result) && Convert.ToDouble(doubleTextBox.MaskedText) > value)
                        {
                            if (doubleTextBox.MinimumNumberDecimalDigits >= 0)
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.MinimumNumberDecimalDigits;
                            else
                                doubleTextBox.NumberDecimalDigits = doubleTextBox.numberDecimalDigits;
                        }
                        else
                            if (maskCount >= 0)
                            {
                                if (doubleTextBox.MaximumNumberDecimalDigits == -1 && doubleTextBox.MinimumNumberDecimalDigits == -1)
                                {
                                    maskCount = doubleTextBox.numberDecimalDigits;
                                }
                                else if (doubleTextBox.NumberDecimalDigits > 0 && doubleTextBox.MaximumNumberDecimalDigits >= 0 && doubleTextBox.MinimumNumberDecimalDigits == -1)
                                {
                                    if (maskCount >= doubleTextBox.MaximumNumberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.MaximumNumberDecimalDigits;
                                    }
                                    if (maskCount <= doubleTextBox.numberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.numberDecimalDigits;
                                    }
                                }
                                else if (doubleTextBox.NumberDecimalDigits > 0 && doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits == -1)
                                {
                                    if (maskCount >= doubleTextBox.numberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.numberDecimalDigits;
                                    }
                                    if (maskCount <= doubleTextBox.MinimumNumberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.MinimumNumberDecimalDigits;
                                    }
                                }
                                else if (doubleTextBox.MinimumNumberDecimalDigits >= 0 && doubleTextBox.MaximumNumberDecimalDigits >= 0 && doubleTextBox.NumberDecimalDigits > 0)
                                {
                                    if (maskCount >= doubleTextBox.MaximumNumberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.MaximumNumberDecimalDigits;
                                    }
                                    if (maskCount <= doubleTextBox.MinimumNumberDecimalDigits)
                                    {
                                        maskCount = doubleTextBox.MinimumNumberDecimalDigits;
                                    }
                                }
                                doubleTextBox.NumberDecimalDigits = maskCount;
                            }
                        DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                    }
                    double doublevalue = (double)value;

                    return Math.Round(doublevalue, doubleTextBox.NumberDecimalDigits);
                }
                else
                {
                    return value;
                }
            }
            else
            {
                if (doubleTextBox.UseNullOption)
                {
                    doubleTextBox.IsNull = true;
                    doubleTextBox.IsNegative = false;
                    doubleTextBox.IsZero = false;
                    return doubleTextBox.NullValue;
                }
                else
                {
                    double value = 0L;
                    if (doubleTextBox.mValueChanged == true)
                    {
                        if (value > doubleTextBox.MaxValue)
                        {
                            value = doubleTextBox.MaxValue;
                        }
                        if (value < doubleTextBox.MinValue)
                        {
                            value = doubleTextBox.MinValue;
                        }
                    }
                    doubleTextBox.IsNegative = value < 0 ? true : false;
                    doubleTextBox.IsZero = value == 0 ? true : false;
                    doubleTextBox.IsNull = false;
                    if (doubleTextBox.NumberDecimalDigits > 0)
                    {
                        double doublevalue = (double)value;
                        return Math.Round(doublevalue, doubleTextBox.numberDecimalDigits);
                    }
                    else
                        return value;
                }
            }
        }

        private static int CountDecimalDigits(string p, DependencyObject d)
        {
            if (!string.IsNullOrEmpty(p) && (d is DoubleTextBox))
            {
                int decimalCount = 0;
                DoubleTextBox doubleTextBox = (DoubleTextBox)d;
                NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
                for (int len = p.Length - 1; len >= 0; len--)
                {
                    if (numberFormat != null)
                    {
                        if (p[len].ToString() == numberFormat.NumberDecimalSeparator || p.Length.ToString() == doubleTextBox.NumberDecimalSeparator)
                        {
                            break;
                        }
                        else
                        {
                            decimalCount++;
                        }
                    }
                }
                return decimalCount;
            }
            else
                return 0;
        }

        private static object CoerceMinValue(DependencyObject d, object baseValue)
        {
            DoubleTextBox doubleTextBox = (DoubleTextBox)d;
            if (doubleTextBox.MinValue > doubleTextBox.MaxValue)
            {
                return doubleTextBox.MaxValue;
            }
            return baseValue;
        }

        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            DoubleTextBox doubleTextBox = (DoubleTextBox)d;
            if (doubleTextBox.MinValue > doubleTextBox.MaxValue)
            {
                return doubleTextBox.MinValue;
            }
            return baseValue;
        }

        #endregion Internal Methods

        private void copy()
        {
            try
            {
                Clipboard.SetText(this.SelectedText);
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

#if WPF

        private new void Paste()
#endif
#if SILVERLIGHT
         private void Paste()
#endif
        {
            if (this.IsReadOnly == false)
            {
                try
                {
                    double val1;
                    double oldval = 0;
                    if (!this.UseNullOption && this.Value != null)
                        oldval = (double)this.Value;
                    string oldselection = Clipboard.GetText();
                    int index1 = this.SelectionStart;
                    NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                    string copiedValue = string.Empty;
                    string afterseperator = string.Empty;
                    int seperatorindex = 0;
                    int negFlag = 0;
                    for (int i = 0; i < oldselection.Length; i++)
                    {
                        if (numberFormat != null)
                        {
                            if (numberFormat.NumberDecimalSeparator != null)
                            {
                                if (char.IsDigit(oldselection[i]) && i == seperatorindex)
                                {
                                    seperatorindex = i + 1;
                                    copiedValue += oldselection[i];
                                }
                                else if (oldselection[i].ToString() == numberFormat.NumberDecimalSeparator)
                                {
                                    seperatorindex = i;
                                }
                                else if (char.IsDigit(oldselection[i]))
                                {
                                    afterseperator += oldselection[i];
                                }
                                else
                                {
                                    if (i <= seperatorindex)
                                    {
                                        seperatorindex = i + 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (char.IsDigit(oldselection[i]) && i == seperatorindex)
                            {
                                seperatorindex = i + 1;
                                copiedValue += oldselection[i];
                            }
                        }
                    }

                    if (this.SelectionLength > 0)
                    {
                        if (oldselection == string.Empty)
                        {
                            if (this.OldValue != null)
                            {
                                this.SetValue(false, this.OldValue);
                                double val3 = (double)this.OldValue;
                                this.Text = val3.ToString("N", numberFormat);
                            }
                        }
                        else
                        {
                            if (this.SelectedText.Length == this.Text.Length)
                            {
                                if (oldselection[0] == '-')
                                {
                                    negFlag = 1;
                                }
                                else if (this.Culture.Name == "ar-SA" && oldselection[oldselection.Length - 1] == '-')
                                {
                                    negFlag = 1;
                                }
                            }
                            if (this.SelectedText.Length == this.Text.Length || this.Text.Contains(this.SelectedText) || this.SelectedText.Contains(numberFormat.NumberDecimalSeparator))
                            {
                                this.Text = this.Text.Replace(this.SelectedText, copiedValue);
                                if (numberFormat != null)
                                {
                                    if (numberFormat.NumberDecimalSeparator != null)
                                    {
                                        if (afterseperator != string.Empty && !this.Text.Contains(numberFormat.NumberDecimalSeparator))
                                        {
                                            this.Text = this.Text + numberFormat.NumberDecimalSeparator + afterseperator;
                                        }
                                        else if (this.Text.Contains(numberFormat.NumberDecimalSeparator))
                                        {
                                            for (int i = 0; i < this.Text.Length; i++)
                                            {
                                                if (this.Text[i].ToString() == numberFormat.NumberDecimalSeparator)
                                                {
                                                    seperatorindex = i;
                                                }
                                            }
                                            if (seperatorindex < this.Text.Length)
                                            {
                                                this.Text = this.Text.Insert(seperatorindex + 1, afterseperator);
                                            }
                                        }
                                    }
                                }
                                if (negFlag == 1)
                                    this.Text = "-" + this.Text;
                            }
                        }
                    }
                    else if (this.Value == 0.0)
                    {
                        this.Text = oldselection;
                    }

                    else
                    {
                        if (this.SelectionStart != this.Text.Length || this.Text.Length == 0)
                        {
                            if (this.Text.Length == 0)
                            {
                                if (oldselection[0] == '-')
                                {
                                    negFlag = 1;
                                }
                                else if (this.Culture.Name == "ar-SA" && oldselection[oldselection.Length - 1] == '-')
                                {
                                    negFlag = 1;
                                }
                            }
                            this.Text = this.Text.Insert(this.SelectionStart, copiedValue);
                            if (numberFormat != null)
                            {
                                if (numberFormat.NumberDecimalSeparator != null)
                                {
                                    if (afterseperator != string.Empty && !this.Text.Contains(numberFormat.NumberDecimalSeparator))
                                    {
                                        this.Text = this.Text + numberFormat.NumberDecimalSeparator + afterseperator;
                                    }
                                    else if (this.Text.Contains(numberFormat.NumberDecimalSeparator))
                                    {
                                        for (int i = 0; i < this.Text.Length; i++)
                                        {
                                            if (this.Text[i].ToString() == numberFormat.NumberDecimalSeparator)
                                            {
                                                seperatorindex = i;
                                            }
                                        }
                                        if (seperatorindex < this.Text.Length)
                                        {
                                            this.Text = this.Text.Insert(seperatorindex + 1, afterseperator);
                                        }
                                    }
                                }
                            }
                            if (negFlag == 1)
                                this.Text = "-" + this.Text;
                        }
                        else
                            return;
                    }
                    if (this.Text.Length >= 15 && this.MaxValidation == MaxValidation.OnLostFocus)
                    {
                        try
                        {
                            decimal newvalue = decimal.Parse(this.Text);
                            this.MaskedText = newvalue.ToString("N", numberFormat);
                        }
                        catch { }
                        return;
                    }
                    if (IsExceedDecimalDigits)
                    {
                        for (int len = this.Text.Length - 1; len >= 0; len--)
                        {
                            if (numberFormat != null)
                            {
                                if (this.Text[len].ToString() == numberFormat.NumberDecimalSeparator || this.Text.Length.ToString() == NumberDecimalSeparator)
                                {
                                    break;
                                }
                                else
                                {
                                    count++;
                                }
                            }
                        }
                        if (count >= MinimumNumberDecimalDigits && count < MaximumNumberDecimalDigits)
                        {
                            if (MaximumNumberDecimalDigits > 0)
                            {
                                if (MinimumNumberDecimalDigits > 0)
                                {
                                    DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                                    NumberDecimalDigits = count - 1;
                                    DoubleValueHandler.doubleValueHandler.AllowChange = true;
                                    DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                                }
                                else if (count <= numberDecimalDigits)
                                {
                                    NumberDecimalDigits = numberDecimalDigits;
                                }
                                else if (count <= MaximumNumberDecimalDigits)
                                {
                                    DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                                    NumberDecimalDigits = count;
                                    DoubleValueHandler.doubleValueHandler.AllowChange = true;
                                    DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                                }
                            }
                        }
                        else if (MaximumNumberDecimalDigits > 0)
                        {
                            DoubleValueHandler.doubleValueHandler.CanUpdate = true;
                            NumberDecimalDigits = MaximumNumberDecimalDigits;
                            DoubleValueHandler.doubleValueHandler.AllowChange = false;
                            DoubleValueHandler.doubleValueHandler.CanUpdate = false;
                        }
                        numberFormat = GetCulture().NumberFormat;
                    }
                    bool seperatorFlag = false;
                    bool groupFlag = false;
                    if (numberFormat.NumberDecimalSeparator != String.Empty && numberFormat.NumberDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    {
                        this.Text = this.Text.Replace(numberFormat.NumberDecimalSeparator, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                        seperatorFlag = true;
                    }
                    if (numberFormat.NumberGroupSeparator != String.Empty && numberFormat.NumberGroupSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator)
                    {
                        this.Text = this.Text.Replace(numberFormat.NumberGroupSeparator, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
                        groupFlag = true;
                    }
                    double.TryParse(this.Text, out val1);
                    if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress) || val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                    {
                        if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress))
                        {
                            if (this.MaxValueOnExceedMaxDigit)
                            {
                                val1 = this.MaxValue;
                                this.SetValue(false, val1);
                                if (seperatorFlag == true || groupFlag == true)
                                    this.MaskedText = val1.ToString("N", numberFormat);
                                this.Text = val1.ToString("N", numberFormat);
                                this.CaretIndex = index1;
                            }
                            else
                            {
                                if (oldval == 0 && this.UseNullOption)
                                {
                                    this.Value = null;
                                    this.Text = null;
                                }
                                else
                                {
                                    if (!double.IsNaN((double)oldval))
                                    {
                                        this.SetValue(false, oldval);
                                        double val3 = oldval;
                                        this.Text = val3.ToString("N", numberFormat);
                                    }
                                    else
                                    {
                                        this.Value = double.NaN;
                                        this.Text = "";
                                    }
                                }
                            }
                        }
                        else if (val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                        {
                            if (this.MinValueOnExceedMinDigit)
                            {
                                val1 = this.MinValue;
                                this.SetValue(false, val1);
                                if (seperatorFlag == true || groupFlag == true)
                                    this.MaskedText = val1.ToString("N", numberFormat);
                                this.Text = val1.ToString("N", numberFormat);
                                this.CaretIndex = index1;
                            }
                            else
                            {
                                if (oldval == 0 && this.UseNullOption)
                                {
                                    this.Value = null;
                                    this.Text = null;
                                }
                                else
                                {
                                    if (!double.IsNaN((double)oldval))
                                    {
                                        this.SetValue(false, oldval);
                                        double val3 = oldval;
                                        this.Text = val3.ToString("N", numberFormat);
                                    }
                                    else
                                    {
                                        this.Value = double.NaN;
                                        this.Text = "";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!double.IsNaN(val1))
                        {
                            this.SetValue(false, val1);
                            if (seperatorFlag == true || groupFlag == true)
                                this.MaskedText = val1.ToString("N", numberFormat);
                            if (this.Culture.Name == "vi-VN" && this.NumberGroupSeparator == string.Empty)
                            {
                                this.Text = val1.ToString("N", numberFormat).Replace(".", "");
                            }
                            else
                                this.Text = val1.ToString("N", numberFormat);
                            int indexs = 0;
                            if (this.Culture.Name != "ar-SA")
                                indexs = index1 + oldselection.Length;
                            this.CaretIndex = indexs;
                        }
                    }
                    seperatorFlag = false;
                    groupFlag = false;
                    negFlag = 0;
                }
                catch (COMException)
                {
                    //To handle COMException for another application accessing already opened Clipboard.
                }
            }
        }

        private void DoubleTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            mIsLoaded = true;
            object tempObj = CoerceValue(this, Value);
            double? tempVal = (double?)tempObj;
            if (this.IsNull == true)
            {
                this.WatermarkVisibility = Visibility.Visible;
            }
            if (tempVal != Value)
            {
                bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = tempVal });
                if (!cancelValueChanging)
                {
                    if (UseNullOption)
                    {
                        if (tempVal != null)
                            Value = tempVal;
                    }
                    else
                        Value = tempVal;
                }
            }
            else
            {
                FormatText();
            }
#if WPF
            if (this.TextSelectionOnFocus)
            {
                if (this.IsFocused == true)
                {
                    e.Handled = true;
                    this.Focus();
                    this.SelectAll();
                }
            }
#endif
        }

#if WPF

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (this.IsReadOnly == true)
            {
                e.Handled = true;
                base.OnContextMenuOpening(e);
            }
            else
            {
                base.OnContextMenuOpening(e);
            }
        }

#endif

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string NumberGroupSeparator
        {
            get { return (string)GetValue(NumberGroupSeparatorProperty); }
            set { SetValue(NumberGroupSeparatorProperty, value); }
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public int[] NumberGroupSizes
        {
            get { return (int[])GetValue(NumberGroupSizesProperty); }
            set { SetValue(NumberGroupSizesProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberGroupSizesProperty =
            DependencyProperty.Register("NumberGroupSizes", typeof(int[]), typeof(DoubleTextBox), new PropertyMetadata(null,OnNumberGroupSizesChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(DoubleTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));
#endif
#if WPF

        public Int32Collection NumberGroupSizes
        {
            get { return (Int32Collection)GetValue(NumberGroupSizesProperty); }
            set { SetValue(NumberGroupSizesProperty, value); }
        }

        public static readonly DependencyProperty NumberGroupSizesProperty =
            DependencyProperty.Register("NumberGroupSizes", typeof(Int32Collection), typeof(DoubleTextBox), new PropertyMetadata(new Int32Collection(), OnNumberGroupSizesChanged));

        // In WPF Value property acts with TwoWay Binding by default.
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(DoubleTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(CoerceValue), false, UpdateSourceTrigger.LostFocus));

#endif

        /// <summary>
        ///
        /// </summary>
        public int MinimumNumberDecimalDigits
        {
            get { return (int)GetValue(MinimumNumberDecimalDigitsProperty); }
            set { SetValue(MinimumNumberDecimalDigitsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimumNumberDecimalDigits.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumNumberDecimalDigitsProperty =
            DependencyProperty.Register("MinimumNumberDecimalDigits", typeof(int), typeof(DoubleTextBox), new PropertyMetadata(-1, new PropertyChangedCallback(OnMinimumNumberDecimalDigitsChanged)));

        /// <summary>
        ///
        /// </summary>
        public int MaximumNumberDecimalDigits
        {
            get { return (int)GetValue(MaximumNumberDecimalDigitsProperty); }
            set { SetValue(MaximumNumberDecimalDigitsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaximumNumberDecimalDigits.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumNumberDecimalDigitsProperty =
            DependencyProperty.Register("MaximumNumberDecimalDigits", typeof(int), typeof(DoubleTextBox), new PropertyMetadata(-1, new PropertyChangedCallback(OnMaximumNumberDecimalDigitsChanged)));

        /// <summary>
        ///
        /// </summary>
        public int NumberDecimalDigits
        {
            get { return (int)GetValue(NumberDecimalDigitsProperty); }
            set { SetValue(NumberDecimalDigitsProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string NumberDecimalSeparator
        {
            get { return (string)GetValue(NumberDecimalSeparatorProperty); }
            set { SetValue(NumberDecimalSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(DoubleTextBox), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(DoubleTextBox), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

#if WPF

        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(DoubleTextBox), new PropertyMetadata(string.Empty, OnNumberGroupSeparatorChanged, new CoerceValueCallback(CoerceNumberGroupSeperator)));

#else
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(DoubleTextBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberDecimalDigitsProperty =
            DependencyProperty.Register("NumberDecimalDigits", typeof(int), typeof(DoubleTextBox), new PropertyMetadata(-1, OnNumberDecimalDigitsChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberDecimalSeparatorProperty =
            DependencyProperty.Register("NumberDecimalSeparator", typeof(string), typeof(DoubleTextBox), new PropertyMetadata(string.Empty, OnNumberDecimalSeparatorChanged));

        #endregion Properties

        /// <summary>
        ///
        /// </summary>
        public bool GroupSeperatorEnabled
        {
            get { return (bool)GetValue(GroupSeperatorEnabledProperty); }
            set { SetValue(GroupSeperatorEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupSeperatorEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupSeperatorEnabledProperty =
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(DoubleTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));

        internal bool IsExceedDecimalDigits
        {
            get { return (bool)GetValue(IsExceedDecimalDigitsProperty); }
            set { SetValue(IsExceedDecimalDigitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExceedDecimalDigits.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsExceedDecimalDigitsProperty =
            DependencyProperty.Register("IsExceedDecimalDigits", typeof(bool), typeof(DoubleTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExceedDecimalDigits)));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsExceedDecimalDigits(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnIsExceedDecimalDigitsChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsExceedDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsExceedDecimalDigitsChanged != null)
                this.IsExceedDecimalDigitsChanged(this, args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaximumNumberDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnMaximumNumberDecimalDigitsChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaximumNumberDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MaximumNumberDecimalDigitsChanged != null)
                this.MaximumNumberDecimalDigitsChanged(this, args);
            if (this.MaximumNumberDecimalDigits > -1)
            {
                this.IsExceedDecimalDigits = true;
            }
            if ((this.MaximumNumberDecimalDigits < this.NumberDecimalDigits) && MaximumNumberDecimalDigits >= 0 && this.NumberDecimalDigits >= 0)
            {
                throw new InvalidOperationException("MaximumNumberDecimalDigits should not be lesser than NumberDecimalDigits");
            }
            if (MinimumNumberDecimalDigits >= 0 && MaximumNumberDecimalDigits >= 0)
            {
                if ((this.MaximumNumberDecimalDigits < this.NumberDecimalDigits) || (this.MaximumNumberDecimalDigits < this.MinimumNumberDecimalDigits))
                {
                    throw new InvalidOperationException("MaximumNumberDecimalDigits should not be lesser than NumberDecimalDigits or MinimumNumberDecimalDigits");
                }
                else if ((this.MinimumNumberDecimalDigits > this.MaximumNumberDecimalDigits))
                {
                    throw new InvalidOperationException("MinimumNumberDecimalDigits should not be greater than NumberDecimalDigits or MaximumNumberDecimalDigits");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinimumNumberDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnMinimumNumberDecimalDigitsChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMinimumNumberDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MinimumNumberDecimalDigitsChanged != null)
                this.MinimumNumberDecimalDigitsChanged(this, args);
            if (this.MinimumNumberDecimalDigits > -1)
            {
                this.IsExceedDecimalDigits = true;
            }
            if ((this.MinimumNumberDecimalDigits > this.NumberDecimalDigits) && MinimumNumberDecimalDigits >= 0 && this.NumberDecimalDigits >= 0)
                throw new InvalidOperationException("MinimumNumberDecimalDigits should not be greater than NumberDecimalDigits or MaximumNumberDecimalDigits");
            if (MinimumNumberDecimalDigits >= 0 && MaximumNumberDecimalDigits >= 0)
            {
                if ((this.MaximumNumberDecimalDigits < this.NumberDecimalDigits) || (this.MaximumNumberDecimalDigits < this.MinimumNumberDecimalDigits))
                {
                    throw new InvalidOperationException("MaximumNumberDecimalDigits should not be lesser than NumberDecimalDigits or MinimumNumberDecimalDigits");
                }
                else if ((this.MinimumNumberDecimalDigits > this.NumberDecimalDigits && this.NumberDecimalDigits >= 0) || (this.MinimumNumberDecimalDigits > this.MaximumNumberDecimalDigits))
                {
                    throw new InvalidOperationException("MinimumNumberDecimalDigits should not be greater than NumberDecimalDigits or MaximumNumberDecimalDigits");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public double ScrollInterval
        {
            get { return (double)GetValue(ScrollIntervalProperty); }
            set { SetValue(ScrollIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScrollInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollIntervalProperty =
            DependencyProperty.Register("ScrollInterval", typeof(double), typeof(DoubleTextBox), new PropertyMetadata(1.0));

        /// <summary>
        ///
        /// </summary>
        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(double), typeof(DoubleTextBox), new PropertyMetadata(1d));

        #region PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNumberGroupSizesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnNumberGroupSizesChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNumberGroupSizesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.NumberGroupSizesChanged != null)
            {
                this.NumberGroupSizesChanged(this, args);
            }

            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNumberDecimalSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnNumberDecimalSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNumberDecimalSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.NumberDecimalSeparatorChanged != null)
            {
                this.NumberDecimalSeparatorChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNumberDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox d = (DoubleTextBox)obj;
            if (d != null)
            {
                d.OnNumberDecimalDigitsChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNumberDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if ((this.NumberDecimalDigits < 0) && !mIsLoaded)
            {
                throw new InvalidOperationException("NumberDecimalDigits must be greater than zero");
            }
            else if((this.NumberDecimalDigits < 0) && mIsLoaded)
            {
                this.NumberDecimalDigits = 0;
            }
            if ((this.NumberDecimalDigits < this.MinimumNumberDecimalDigits && this.NumberDecimalDigits >= 0 && this.MinimumNumberDecimalDigits >= 0))
            {
                throw new InvalidOperationException("NumberDecimalDigits should not be lesser than MinimumNumberDecimalDigits");
            }
            if (MaximumNumberDecimalDigits >= 0)
            {
                if ((this.MaximumNumberDecimalDigits < this.NumberDecimalDigits) && this.NumberDecimalDigits >= 0)
                {
                    throw new InvalidOperationException("MaximumNumberDecimalDigits should not be lesser than NumberDecimalDigits");
                }
                else if ((this.MaximumNumberDecimalDigits < this.MinimumNumberDecimalDigits && this.MinimumNumberDecimalDigits >= 0))
                {
                    throw new InvalidOperationException("MaximumNumberDecimalDigits should not be lesser than NumberDecimalDigits");
                }
            }
            if (this.NumberDecimalDigitsChanged != null)
            {
                this.NumberDecimalDigitsChanged(this, args);
            }
            if (!DoubleValueHandler.doubleValueHandler.CanUpdate)
            {
                numberDecimalDigits = this.NumberDecimalDigits;
            }
            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DoubleTextBox)obj != null)
                ((DoubleTextBox)obj).OnValueChanged(args);
        }

#if WPF

        public override void OnUseNullOptionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!(bool)args.NewValue)
            {
                if (this.IsNull)
                {
                    this.Value = this.MinValue;
                    this.IsNull = false;
                }
            }
            base.OnUseNullOptionChanged(args);
        }

#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object coerceValue = CoerceValue(this, this.Value);
            if (this.Value != (double?)coerceValue)
            {
                this.Value = (double?)coerceValue;
                return;
            }
#endif
            if (this.Value != null && !(double.IsNaN((double)this.Value)))
            {
                this.IsNegative = this.Value < 0 ? true : false;
                this.IsZero = this.Value == 0 ? true : false;
                this.IsNull = false;
            }
            else
            {
                if (this.UseNullOption)
                {
                    this.IsNull = true;
                    this.IsNegative = false;
                    this.IsZero = false;
                }
            }

            OldValue = (double?)args.OldValue;
            mValue = this.Value;
            if (this.Value != null)
                this.WatermarkVisibility = System.Windows.Visibility.Collapsed;

            if (ValueChanged != null)
                ValueChanged(this, args);
            if (this.Value > this.MinValue && this.MinValidation == MinValidation.OnKeyPress)
            {
                this.checktext = "";
            }
            if (mIsLoaded)
            {
                if (mValueChanged == true)
                    FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DoubleTextBox)obj != null)
            {
                ((DoubleTextBox)obj).OnMinValueChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMinValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MinValueChanged != null)
                this.MinValueChanged(this, args);

            if (this.UseNullOption)
            {
                if (this.Value != null && !double.IsNaN((double)this.Value) && this.MinValue != 0)
                {
                    if (this.Value < this.MinValue)
                        this.Value = this.MinValue;
                }
            }

            if (this.Value != this.ValidateValue(this.Value))
            {
                double? tempVal = this.ValidateValue(this.Value);

                bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = tempVal });
                if (!cancelValueChanging)
                    this.Value = tempVal;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DoubleTextBox)obj != null)
            {
                ((DoubleTextBox)obj).OnMaxValueChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaxValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MaxValueChanged != null)
                this.MaxValueChanged(this, args);

            if (this.Value != this.ValidateValue(this.Value))
            {
                double? tempVal = this.ValidateValue(this.Value);

                bool cancelValueChanging = this.TriggerValueChangingEvent(new ValueChangingEventArgs() { OldValue = this.Value, NewValue = tempVal });
                if (!cancelValueChanging)

                    this.Value = tempVal;
            }
        }

        private static void OnNumberGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DoubleTextBox)obj != null)
                ((DoubleTextBox)obj).OnNumberGroupSeparatorChanged(args);
        }

        private static object CoerceNumberGroupSeperator(DependencyObject d, object baseValue)
        {
            DoubleTextBox doubleTextBox = (DoubleTextBox)d;
            NumberFormatInfo numberFormat = doubleTextBox.GetCulture().NumberFormat;
            if (!baseValue.Equals(string.Empty) && !(char.IsLetterOrDigit(baseValue.ToString(), 0)))
            {
                return baseValue;
            }
            else if (baseValue.Equals(string.Empty))
            {
                return baseValue;
            }
            else
            {
                return numberFormat.NumberGroupSeparator;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNumberGroupSeparatorChanged(DependencyPropertyChangedEventArgs e)
        {
#if SILVERLIGHT
            object val=CoerceNumberGroupSeperator(this,this.NumberGroupSeparator);
            if(this.NumberGroupSeparator!=val.ToString())
                this.NumberGroupSeparator=val.ToString();
#endif
            if (this.NumberGroupSeparatorChanged != null)
                this.NumberGroupSeparatorChanged(this, e);
            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        #endregion PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        public double? NullValue
        {
            get { return (double?)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NullValueProperty =
            DependencyProperty.Register("NullValue", typeof(double?), typeof(DoubleTextBox), new PropertyMetadata(null, OnNullValueChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNullValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((DoubleTextBox)obj != null)
                ((DoubleTextBox)obj).OnNullValueChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNullValueChanged(DependencyPropertyChangedEventArgs args)
        {
        }

#if WPF

        public StringValidation ValueValidation
        {
            get { return (StringValidation)GetValue(ValueValidationProperty); }
            set { SetValue(ValueValidationProperty, value); }
        }

        public static readonly DependencyProperty ValueValidationProperty =
            DependencyProperty.Register("ValueValidation", typeof(StringValidation), typeof(DoubleTextBox), new PropertyMetadata(StringValidation.OnLostFocus));

        public InvalidInputBehavior InvalidValueBehavior
        {
            get { return (InvalidInputBehavior)GetValue(InvalidValueBehaviorProperty); }
            set { SetValue(InvalidValueBehaviorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InvalidValueBehaviorProperty =
            DependencyProperty.Register("InvalidValueBehavior", typeof(InvalidInputBehavior), typeof(DoubleTextBox), new PropertyMetadata(InvalidInputBehavior.None, OnInvalidValueBehaviorChanged));

        public static void OnInvalidValueBehaviorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox m = (DoubleTextBox)obj;
            if (m != null)
            {
                m.OnInvalidValueBehaviorChanged(args);
            }
        }

        protected void OnInvalidValueBehaviorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.InvalidValueBehaviorChanged != null)
            {
                InvalidValueBehaviorChanged(this, args);
            }
        }

        public string ValidationValue
        {
            get { return (string)GetValue(ValidationValueProperty); }
            set { SetValue(ValidationValueProperty, value); }
        }

        public static readonly DependencyProperty ValidationValueProperty =
            DependencyProperty.Register("ValidationValue", typeof(string), typeof(DoubleTextBox), new PropertyMetadata(string.Empty, OnValidationValueChanged));

        public static void OnValidationValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox m = (DoubleTextBox)obj;
            if (m != null)
            {
                m.OnValidationValueChanged(args);
            }
        }

        protected void OnValidationValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ValidationValueChanged != null)
            {
                ValidationValueChanged(this, args);
            }
        }

        public bool ValidationCompleted
        {
            get { return (bool)GetValue(ValidationCompletedProperty); }
            set { SetValue(ValidationCompletedProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValidationCompletedProperty =
            DependencyProperty.Register("ValidationCompleted", typeof(bool), typeof(DoubleTextBox), new PropertyMetadata(false, OnValidationCompletedPropertyChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationCompletedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DoubleTextBox mask = (DoubleTextBox)obj;
            if (mask != null)
                mask.OnValidationCompletedPropertyChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValidationCompletedPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ValidationCompletedChanged != null)
            {
                this.ValidationCompletedChanged(this, args);
            }
        }

        internal void OnValueValidationCompleted(StringValidationEventArgs e)
        {
            if (ValueValidationCompleted != null)
            {
                ValueValidationCompleted(this, e);
            }
        }

        internal void OnValidated(EventArgs e)
        {
            if (Validated != null)
            {
                Validated(this, e);
            }
        }

        internal bool OnValidating(CancelEventArgs e)
        {
            if (Validating != null)
            {
                Validating(this, e);
                _validatingrResult = e.Cancel;
                return e.Cancel;
            }
            return false;
        }

#endif
    }

    /// <summary>
    ///
    /// </summary>
    public class ValueChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public ValueChangingEventArgs()
        { }

        // Summary:
        //     Gets the value of the property after the change.
        //
        // Returns:
        //     The property value after the change.
        /// <summary>
        ///
        /// </summary>
        public object NewValue { get; set; }

        //
        // Summary:
        //     Gets the value of the property before the change.
        //
        // Returns:
        //     The property value before the change.
        /// <summary>
        ///
        /// </summary>
        public object OldValue { get; set; }
    }
}