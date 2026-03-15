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
      Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Blend;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Default;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Windows7;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.VS2010;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Metro;component/Editors/CurrencyTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
      Type = typeof(CurrencyTextBox), XamlResource = "/Syncfusion.Theming.Transparent;component/Editors/CurrencyTextBox.xaml")]
#endif
    public class CurrencyTextBox : EditorBase
    {
        #region Events

        /// <summary>
        /// Event that is raised when <see cref="MinValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencySymbolPosition"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencySymbolPositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MaxValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyDecimalDigits"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyDecimalDigitsChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyDecimalSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyDecimalSeparatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyGroupSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyGroupSeparatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyGroupSizes"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyGroupSizesChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyNegativePattern"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyNegativePatternChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencyPositivePattern"/>x property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencyPositivePatternChanged;

        /// <summary>
        /// Event that is raised when <see cref="CurrencySymbol"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CurrencySymbolChanged;

#if WPF

        public event PropertyChangedCallback ValidationCompletedChanged;

        public event PropertyChangedCallback InvalidValueBehaviorChanged;

        public event StringValidationCompletedEventHandler ValueValidationCompleted;

        public event PropertyChangedCallback ValidationValueChanged;

        public event CancelEventHandler Validating;

        public event EventHandler Validated;

#endif

        #endregion Events

        #region Members

        internal decimal? OldValue;
        internal decimal? mValue;
        internal bool? mValueChanged = true;
        internal bool mIsLoaded = false;
        internal string checktext = "";

        #endregion Members

        #region Constructor

#if WPF

        static CurrencyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CurrencyTextBox), new FrameworkPropertyMetadata(typeof(CurrencyTextBox)));
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
        public CurrencyTextBox()
        {
            pastecommand = new DelegateCommand<object>(_pastecommand, Canpaste);
            copycommand = new DelegateCommand<object>(_copycommand, Canpaste);
            cutcommand = new DelegateCommand<object>(_cutcommand, Canpaste);
#if WPF
            this.AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(CommandExecuted), true);
#endif
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(CurrencyTextBox));
            }
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(CurrencyTextBox);
#endif
            this.Loaded += IntegerTextbox_Loaded;
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
        private Border Focused_Border = null;

        /// <summary>
        ///
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
#if WPF
            PART_ContentHost = this.GetTemplateChild("PART_ContentHost") as ScrollViewer;
#endif
#if SILVERLIGHT
            PART_ContentHost = this.GetTemplateChild("ContentElement") as ScrollViewer;
#endif
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
                    string oldselection = Clipboard.GetText();
                    int index1 = this.SelectionStart;
                    NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                    string oldtext = this.Text;
                    CurrencyTextBox currencybox = this as CurrencyTextBox;
                    string copiedValue = string.Empty;
                    string afterseperator = string.Empty;
                    int seperatorindex = 0, currencySymbolcount = 0;
                    decimal oldval = 0;
                    if (!this.UseNullOption && this.Value != null)
                    {
                        oldval = (decimal)this.Value;
                    }
                    decimal val1;
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
                                        if (oldselection[i].ToString() == numberFormat.CurrencySymbol)
                                            currencySymbolcount++;
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
                                currencybox.SetValue(false, currencybox.OldValue);
                                decimal val3 = (decimal)currencybox.OldValue;
                                this.Text = val3.ToString("C", numberFormat);
                            }
                        }
                        else
                        {
                            if (this.SelectedText.Length == this.Text.Length)
                            {
                                if (oldselection[currencySymbolcount + 1] == '-')
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
                            }
                        }
                    }
                    else if (oldtext != string.Empty && double.Parse(oldtext.Replace(numberFormat.CurrencySymbol, "")) == 0.0)
                    {
                        this.Text = oldselection;
                    }
                    else
                    {
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
                    }
                    if (numberFormat != null)
                    {
                        if (this.Text.Contains(numberFormat.CurrencySymbol))
                        {
                            for (int x = 0; x < this.Text.Length; x++)
                            {
                                if (this.Text[x].ToString() == numberFormat.CurrencySymbol)
                                {
                                    this.Text = this.Text.Remove(x, 1);
                                }
                            }
                        }
                    }

                    if (negFlag == 1)
                        this.Text = "-" + this.Text;

                    decimal.TryParse(this.Text, out val1);
                    if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress) || val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                    {
                        if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress))
                        {
                            if (this.MaxValueOnExceedMaxDigit)
                            {
                                val1 = this.MaxValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, val1);
                                this.Text = val1.ToString("C", numberFormat);
                            }
                            else
                            {
                                this.SetValue(false, oldval);
                                decimal val3 = oldval;
                                this.Text = val3.ToString("C", numberFormat);
                            }
                        }
                        if (val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                        {
                            if (this.MinValueOnExceedMinDigit)
                            {
                                val1 = this.MinValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, val1);
                                this.Text = val1.ToString("C", numberFormat);
                            }
                            else
                            {
                                this.SetValue(false, oldval);
                                decimal val3 = oldval;
                                this.Text = val3.ToString("C", numberFormat);
                            }
                        }
                    }
                    else
                    {
                        this.SetValue(false, val1);
                        this.Text = val1.ToString("C", numberFormat);
                        int indexs = 0;
                        if (oldselection[oldselection.Length - 1].ToString() == numberFormat.CurrencySymbol)
                            indexs = index1 + oldselection.Length - 1;
                        else
                            indexs = index1 + oldselection.Length;
                        this.CaretIndex = indexs;
                    }
                }
                catch (COMException)
                {
                    //To handle COMException for another application accessing already opened Clipboard.
                }
            }
        }

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                CurrencyValueHandler.currencyValueHandler.HandleUpKey(this);
            }
            else if (e.Delta < 0)
            {
                CurrencyValueHandler.currencyValueHandler.HandleDownKey(this);
            }
        }

#if WPF

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = CurrencyValueHandler.currencyValueHandler.HandleKeyDown(this, e);
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
        ///
        /// </summary>
        /// <param name="e"></param>
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
                e.Handled = CurrencyValueHandler.currencyValueHandler.HandleKeyDown(this, e);
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
                    CurrencyValueHandler.currencyValueHandler.HandleDeleteKey(this);
                }
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            e.Handled = CurrencyValueHandler.currencyValueHandler.MatchWithMask(this, e.Text);
            IsValueChanged = false;
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
        ///
        /// </summary>
        /// <param name="e"></param>
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
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror,
                                                                                 ValidationValue));
                        OnValidated(EventArgs.Empty);
                    }
                    else if (InvalidValueBehavior == InvalidInputBehavior.ResetValue)
                    {
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror,
                                                                                 ValidationValue));
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
                        OnValueValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror,
                                                                                 ValidationValue));
                        OnValidated(EventArgs.Empty);
                    }
                }
            }
#endif

            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.Background;
            if (this.Focused_Border != null)
            {
                this.Focused_Border.Background = this.Background;
            }
#if WPF
            if (mIsLoaded)
            {
                decimal? temp = null;
                if (!string.IsNullOrEmpty(ValidationValue))
                {
                    temp = decimal.Parse(ValidationValue);
                    if (temp != null && temp == this.Value)
                        ValidationCompleted = true;
                    else
                        ValidationCompleted = false;
                }
            }
#endif
            this.checktext = "";
            decimal? Val = this.Value;
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
                    this.Value = Val;
                }
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Focused_Border != null)
            {
                Focused_Border.Background = FocusedBackground;
            }
            if (this.EnableFocusColors && this.PART_ContentHost != null)
            {
                this.PART_ContentHost.Background = this.FocusedBackground;
            }

            base.OnGotFocus(e);
        }

        #endregion overide

        #region Internal Methods

        internal void FormatText()
        {
            if (this.Value != null)
            {
                NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                if (NumberFormat == null)
                {
                    numberFormat.CurrencySymbol = this.CurrencySymbol;
                }
                this.Text = ((decimal)mValue).ToString("C", numberFormat);
                this.MaskedText = this.Text;
            }
            else
            {
                this.MaskedText = "";
            }
        }

        internal void SetValue(bool? IsReload, decimal? _Value)
        {
            if (IsReload == false)
            {
                mValueChanged = false;
                this.Value = _Value;
                mValueChanged = true;
                IsValueChanged = false;
            }
            else if (IsReload == true)
            {
                var caretindex = this.CaretIndex;
                this.Value = _Value;
                this.CaretIndex = caretindex;
            }
        }

        internal decimal? ValidateValue(decimal? Val)
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
            {
                cultureInfo.NumberFormat = NumberFormat;
                CurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol;
            }

            if ((MaxLength != 0 && this.CurrencyDecimalDigits < MaxLength && CurrencyDecimalDigits >= 0) || (MaxLength == 0 && CurrencyDecimalDigits >= 0))
                cultureInfo.NumberFormat.CurrencyDecimalDigits = this.CurrencyDecimalDigits;

            if (!CurrencyDecimalSeparator.Equals(string.Empty))
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = this.CurrencyDecimalSeparator;

            if (!GroupSeperatorEnabled)
            {
                cultureInfo.NumberFormat.CurrencyGroupSeparator = string.Empty;
            }
            if (GroupSeperatorEnabled == true)
            {
#if WPF
                if (!CurrencyGroupSeparator.Equals(string.Empty))
                    cultureInfo.NumberFormat.CurrencyGroupSeparator = this.CurrencyGroupSeparator;
#else
                if (!this.CurrencyGroupSeparator.Equals(string.Empty) && !(char.IsLetterOrDigit(this.CurrencyGroupSeparator, 0)) && this.CurrencyGroupSeparator.Length == 1)
                    cultureInfo.NumberFormat.CurrencyGroupSeparator = this.CurrencyGroupSeparator;
#endif
            }
#if WPF
            int count = this.CurrencyGroupSizes.Count;
            if (count > 0)
            {
                int[] ngs = new int[count];

                for (int i = 0; i < count; i++)
                {
                    ngs[i] = this.CurrencyGroupSizes[i];
                }
                cultureInfo.NumberFormat.CurrencyGroupSizes = ngs;
            }
#endif

#if SILVERLIGHT
              if (CurrencyGroupSizes != null)
                  cultureInfo.NumberFormat.CurrencyGroupSizes = this.CurrencyGroupSizes;
#endif
            if (this.CurrencyNegativePattern > -1 && this.CurrencyNegativePattern < 15)
            {
                cultureInfo.NumberFormat.CurrencyNegativePattern = this.CurrencyNegativePattern;
            }

            if (this.CurrencyPositivePattern > -1 && this.CurrencyPositivePattern < 4)
            {
                cultureInfo.NumberFormat.CurrencyPositivePattern = this.CurrencyPositivePattern;
            }

            if (this.CurrencySymbol != null && Culture == CultureInfo.CurrentCulture)
                cultureInfo.NumberFormat.CurrencySymbol = this.CurrencySymbol;

            if (this.CurrencySymbol != string.Empty && this.CurrencySymbol != CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol)
                cultureInfo.NumberFormat.CurrencySymbol = this.CurrencySymbol;

            if (!GroupSeperatorEnabled)
            {
                cultureInfo.NumberFormat.CurrencyGroupSeparator = string.Empty;
            }

            return cultureInfo;
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            CurrencyTextBox currencyTextBox = (CurrencyTextBox)d;
            if (baseValue != null)
            {
                decimal? value = (decimal?)baseValue;
                if (currencyTextBox.mValueChanged == true)
                {
                    if (value > currencyTextBox.MaxValue)
                    {
                        value = currencyTextBox.MaxValue;
                    }
                    else if (value < currencyTextBox.MinValue)
                    {
                        value = currencyTextBox.MinValue;
                    }
                }
                if (value != null)
                {
                    currencyTextBox.IsNegative = value < 0 ? true : false;
                    currencyTextBox.IsZero = value == 0 ? true : false;
                    currencyTextBox.IsNull = false;
                }
                return value;
            }
            else
            {
                if (currencyTextBox.UseNullOption)
                {
                    currencyTextBox.IsNull = true;
                    currencyTextBox.IsNegative = false;
                    currencyTextBox.IsZero = false;
                    return currencyTextBox.NullValue;
                }
                else
                {
                    decimal value = 0L;
                    if (currencyTextBox.mValueChanged == true)
                    {
                        if (value > currencyTextBox.MaxValue)
                        {
                            value = currencyTextBox.MaxValue;
                        }
                        if (value < currencyTextBox.MinValue)
                        {
                            value = currencyTextBox.MinValue;
                        }
                    }
                    currencyTextBox.IsNegative = value < 0 ? true : false;
                    currencyTextBox.IsZero = value == 0 ? true : false;
                    currencyTextBox.IsNull = false;
                    return value;
                }
            }
        }

        private static object CoerceMinValue(DependencyObject d, object baseValue)
        {
            CurrencyTextBox currencyTextBox = (CurrencyTextBox)d;
            if (currencyTextBox.MinValue > currencyTextBox.MaxValue)
            {
                return currencyTextBox.MaxValue;
            }
            return baseValue;
        }

        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            CurrencyTextBox currencyTextBox = (CurrencyTextBox)d;
            if (currencyTextBox.MinValue > currencyTextBox.MaxValue)
            {
                return currencyTextBox.MinValue;
            }
            return baseValue;
        }

        #endregion Internal Methods

        private void IntegerTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            mIsLoaded = true;
            object tempObj = CoerceValue(this, Value);
            decimal? tempVal = (decimal?)tempObj;
            if (this.IsNull == true)
            {
                this.WatermarkVisibility = Visibility.Visible;
            }
            if (tempVal != Value)
            {
                Value = tempVal;
            }
            else
            {
                FormatText();
            }
        }

        #region Properties

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(DecimalTypeConverter))]
#endif

        public decimal? Value
        {
            get { return (decimal?)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(DecimalTypeConverter))]
#endif

        public decimal MinValue
        {
            get { return (decimal)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(DecimalTypeConverter))]
#endif

        public decimal MaxValue
        {
            get { return (decimal)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

#if WPF

        // In WPF Value property acts with TwoWay binding by default.
        public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(decimal?), typeof(CurrencyTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(CoerceValue), false, UpdateSourceTrigger.LostFocus));

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(decimal?), typeof(CurrencyTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(CurrencyTextBox), new PropertyMetadata(decimal.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(CurrencyTextBox), new PropertyMetadata(decimal.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public int CurrencyDecimalDigits
        {
            get { return (int)GetValue(CurrencyDecimalDigitsProperty); }
            set { SetValue(CurrencyDecimalDigitsProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencyDecimalDigitsProperty =
            DependencyProperty.Register("CurrencyDecimalDigits", typeof(int), typeof(CurrencyTextBox), new PropertyMetadata((int)-1, OnCurrencyDecimalDigitsChanged));

        /// <summary>
        ///
        /// </summary>
        public string CurrencyDecimalSeparator
        {
            get { return (string)GetValue(CurrencyDecimalSeparatorProperty); }
            set { SetValue(CurrencyDecimalSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencyDecimalSeparatorProperty =
            DependencyProperty.Register("CurrencyDecimalSeparator", typeof(string), typeof(CurrencyTextBox), new PropertyMetadata(string.Empty, OnCurrencyDecimalSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        public string CurrencyGroupSeparator
        {
            get { return (string)GetValue(CurrencyGroupSeparatorProperty); }
            set { SetValue(CurrencyGroupSeparatorProperty, value); }
        }

#if WPF

        public static readonly DependencyProperty CurrencyGroupSeparatorProperty =
            DependencyProperty.Register("CurrencyGroupSeparator", typeof(string), typeof(CurrencyTextBox), new PropertyMetadata(string.Empty, OnCurrencyGroupSeparatorChanged, new CoerceValueCallback(CoerceCurrencyGroupSeperator)));

#else
        /// <summary>
        ///
        /// </summary>
         public static readonly DependencyProperty CurrencyGroupSeparatorProperty =
            DependencyProperty.Register("CurrencyGroupSeparator", typeof(string), typeof(CurrencyTextBox), new PropertyMetadata(string.Empty, OnCurrencyGroupSeparatorChanged));
#endif

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
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(CurrencyTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnCurrencyGroupSeparatorChanged)));

#if SILVERLIGHT

        /// <summary>
        ///
        /// </summary>
        public int[] CurrencyGroupSizes
        {
            get { return (int[])GetValue(CurrencyGroupSizesProperty); }
            set { SetValue(CurrencyGroupSizesProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencyGroupSizesProperty =
            DependencyProperty.Register("CurrencyGroupSizes", typeof(int[]), typeof(CurrencyTextBox), new PropertyMetadata(null, OnCurrencyGroupSizesChanged));
#endif

#if WPF

        public Int32Collection CurrencyGroupSizes
        {
            get { return (Int32Collection)GetValue(CurrencyGroupSizesProperty); }
            set { SetValue(CurrencyGroupSizesProperty, value); }
        }

        public static readonly DependencyProperty CurrencyGroupSizesProperty =
            DependencyProperty.Register("CurrencyGroupSizes", typeof(Int32Collection), typeof(CurrencyTextBox), new UIPropertyMetadata(new Int32Collection(), OnCurrencyGroupSizesChanged));

#endif

        /// <summary>
        ///
        /// </summary>
        public int CurrencyNegativePattern
        {
            get { return (int)GetValue(CurrencyNegativePatternProperty); }
            set { SetValue(CurrencyNegativePatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencyNegativePatternProperty =
            DependencyProperty.Register("CurrencyNegativePattern", typeof(int), typeof(CurrencyTextBox), new PropertyMetadata((int)-1, OnCurrencyNegativePatternChanged));

        /// <summary>
        ///
        /// </summary>
        public int CurrencyPositivePattern
        {
            get { return (int)GetValue(CurrencyPositivePatternProperty); }
            set { SetValue(CurrencyPositivePatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencyPositivePatternProperty =
            DependencyProperty.Register("CurrencyPositivePattern", typeof(int), typeof(CurrencyTextBox), new PropertyMetadata(-1, OnCurrencyPositivePatternChanged));

        /// <summary>
        ///
        /// </summary>
        public string CurrencySymbol
        {
            get { return (string)GetValue(CurrencySymbolProperty); }
            set { SetValue(CurrencySymbolProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencySymbolProperty =
            DependencyProperty.Register("CurrencySymbol", typeof(string), typeof(CurrencyTextBox), new PropertyMetadata(NumberFormatInfo.CurrentInfo.CurrencySymbol, OnCurrencySymbolChanged));

        /// <summary>
        ///
        /// </summary>
        public double ScrollInterval
        {
            get { return (double)GetValue(ScrollIntervalProperty); }
            set { SetValue(ScrollIntervalProperty, value); }
        }

        /// <summary>
        /// sing a DependencyProperty as the backing store for ScrollInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollIntervalProperty =
            DependencyProperty.Register("ScrollInterval", typeof(double), typeof(CurrencyTextBox), new PropertyMetadata(1.0));

        #endregion Properties

        #region PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
                c.OnCurrencyDecimalDigitsChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencyDecimalDigitsChanged != null)
            {
                this.CurrencyDecimalDigitsChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyDecimalSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
            {
                c.OnCurrencyDecimalSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyDecimalSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencyDecimalSeparatorChanged != null)
                this.CurrencyDecimalSeparatorChanged(this, args);
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyGroupSizesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
            {
                c.OnCurrencyGroupSizesChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyGroupSizesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencyGroupSizesChanged != null)
            {
                this.CurrencyGroupSizesChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

        private static object CoerceCurrencyGroupSeperator(DependencyObject d, object baseValue)
        {
            CurrencyTextBox currencyTextBox = (CurrencyTextBox)d;
            NumberFormatInfo numberFormat = currencyTextBox.GetCulture().NumberFormat;
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
                return numberFormat.CurrencyGroupSeparator;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
                c.OnCurrencyGroupSeparatorChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyGroupSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object val = CoerceCurrencyGroupSeperator(this, this.CurrencyGroupSeparator);
            if(this.CurrencyGroupSeparator!=val.ToString())
                this.CurrencyGroupSeparator=val.ToString();
#endif
            if (CurrencyGroupSeparatorChanged != null)
            {
                this.CurrencyGroupSeparatorChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyNegativePatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
                c.OnCurrencyNegativePatternChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyNegativePatternChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencyNegativePatternChanged != null)
            {
                this.CurrencyNegativePatternChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencyPositivePatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
            {
                c.OnCurrencyPositivePatternChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencyPositivePatternChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencyPositivePatternChanged != null)
            {
                this.CurrencyPositivePatternChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencySymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
            {
                c.OnCurrencySymbolChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencySymbolChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencySymbolChanged != null)
                this.CurrencySymbolChanged(this, args);
            if (mIsLoaded)
                this.FormatText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((CurrencyTextBox)obj != null)
                ((CurrencyTextBox)obj).OnValueChanged(args);
        }

        internal bool IsValueChanged = true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT

            object coerceValue = CoerceValue(this, this.Value);
            if (this.Value != (decimal?)coerceValue)
            {
                this.Value = (decimal?)coerceValue;
                return;
            }
#endif
            if (this.Value != null)
            {
                this.IsNegative = this.Value < 0 ? true : false;
                this.IsZero = this.Value == 0 ? true : false;
                this.IsNull = false;
                if (MaxLength != 0)
                {
                    if ((args.NewValue).ToString().Length > this.MaxLength && IsValueChanged)
                        CurrencyValueHandler.currencyValueHandler.MatchWithMask(this, this.Value.ToString());
                }
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

            OldValue = (decimal?)args.OldValue;
            mValue = this.Value;

            if (ValueChanged != null)
                ValueChanged(this, args);

            if (this.Value != null)
                this.WatermarkVisibility = System.Windows.Visibility.Collapsed;

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
            if ((CurrencyTextBox)obj != null)
            {
                ((CurrencyTextBox)obj).OnMinValueChanged(args);
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

            if (this.MaxValue < this.MinValue)
                this.MaxValue = this.MinValue;

            if (this.Value != this.ValidateValue(this.Value))
            {
                this.Value = this.ValidateValue(this.Value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((CurrencyTextBox)obj != null)
            {
                ((CurrencyTextBox)obj).OnMaxValueChanged(args);
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

            if (this.MinValue > this.MaxValue)
                this.MinValue = this.MaxValue;

            if (this.Value != this.ValidateValue(this.Value))
            {
                this.Value = this.ValidateValue(this.Value);
            }
        }

        #endregion PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public CurrencySymbolPosition CurrencySymbolPosition
        {
            get { return (CurrencySymbolPosition)GetValue(CurrencySymbolPositionProperty); }
            set { SetValue(CurrencySymbolPositionProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencySymbolPositionProperty =
            DependencyProperty.Register("CurrencySymbolPosition ", typeof(CurrencySymbolPosition), typeof(CurrencyTextBox), new PropertyMetadata(CurrencySymbolPosition.Left, OnCurrencySymbolPositionChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencySymbolPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox c = (CurrencyTextBox)obj;
            if (c != null)
                c.OnCurrencySymbolPositionChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencySymbolPositionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CurrencySymbolPositionChanged != null)
            {
                this.CurrencySymbolPositionChanged(this, args);
            }
            if (mIsLoaded)
                this.FormatText();
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(DecimalTypeConverter))]
#endif

        public decimal? NullValue
        {
            get { return (decimal?)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NullValueProperty =
            DependencyProperty.Register("NullValue", typeof(decimal?), typeof(CurrencyTextBox), new PropertyMetadata(null, OnNullValueChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNullValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((CurrencyTextBox)obj != null)
                ((CurrencyTextBox)obj).OnNullValueChanged(args);
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
            DependencyProperty.Register("ValueValidation", typeof(StringValidation), typeof(CurrencyTextBox), new PropertyMetadata(StringValidation.OnLostFocus));

        public InvalidInputBehavior InvalidValueBehavior
        {
            get { return (InvalidInputBehavior)GetValue(InvalidValueBehaviorProperty); }
            set { SetValue(InvalidValueBehaviorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InvalidValueBehaviorProperty =
            DependencyProperty.Register("InvalidValueBehavior", typeof(InvalidInputBehavior), typeof(CurrencyTextBox), new PropertyMetadata(InvalidInputBehavior.None, OnInvalidValueBehaviorChanged));

        public static void OnInvalidValueBehaviorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox m = (CurrencyTextBox)obj;
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
            DependencyProperty.Register("ValidationValue", typeof(string), typeof(CurrencyTextBox), new PropertyMetadata(string.Empty, OnValidationValueChanged));

        public static void OnValidationValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox m = (CurrencyTextBox)obj;
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
            DependencyProperty.Register("ValidationCompleted", typeof(bool), typeof(CurrencyTextBox), new PropertyMetadata(false, OnValidationCompletedPropertyChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationCompletedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CurrencyTextBox mask = (CurrencyTextBox)obj;
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
}