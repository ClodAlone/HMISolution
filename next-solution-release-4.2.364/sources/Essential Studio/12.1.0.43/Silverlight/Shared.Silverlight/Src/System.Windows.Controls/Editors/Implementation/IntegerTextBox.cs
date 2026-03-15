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
using System.Security;
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
    /// <summary>
    /// 
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
 Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
 Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Blend;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Default;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Windows7;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.VS2010;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Metro;component/Editors/IntegerTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(IntegerTextBox), XamlResource = "/Syncfusion.Theming.Transparent;component/Editors/IntegerTextBox.xaml")]
#endif
    public class IntegerTextBox : EditorBase
    {
#if WPF
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback ValidationCompletedChanged;
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback InvalidValueBehaviorChanged;
        /// <summary>
        /// 
        /// </summary>
        public event StringValidationCompletedEventHandler ValueValidationCompleted;
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback ValidationValueChanged;
        /// <summary>
        /// 
        /// </summary>
        public event CancelEventHandler Validating;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Validated;

#endif

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="MinValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MaxValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberGroupSizes"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberGroupSizesChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberGroupSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NumberGroupSeparatorChanged;

        #endregion Events

        #region Members

        internal Int64? OldValue;
        internal Int64? mValue;
        internal bool? mValueChanged = true;
        internal bool mIsLoaded = false;
        internal int count = 1;
        internal string checktext = "";

        #endregion Members

        #region Constructor

#if WPF
        /// <summary>
        /// 
        /// </summary>
        static IntegerTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerTextBox), new FrameworkPropertyMetadata(typeof(IntegerTextBox)));
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
        public IntegerTextBox()
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
                EnvironmentTest.StartValidateLicense(typeof(IntegerTextBox));
            }
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(IntegerTextBox);
#endif
            this.Loaded += IntegerTextbox_Loaded;
        }

        #endregion Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _pastecommand(object parameter)
        {
            Paste();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _copycommand(object parameter)
        {
            copy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _cutcommand(object parameter)
        {
            cut();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool Canpaste(object parameter)
        {
            return true;
        }

        #region overide

        /// <summary>
        /// 
        /// </summary>
        private ScrollViewer PART_ContentHost;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 
        /// </summary>
        private void copy()
        {
            try
            {
                Clipboard.SetText(this.SelectedText);
            }
            catch (SecurityException)
            {
                //To handle Clipboard Security Exception
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
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
                    string text = Clipboard.GetText();

                    if (NumberGroupSeparator != "." && text.Contains("."))
                    {
                        int i = text.IndexOf(".");
                        text = text.Remove(i);
                    }
                    string oldselection = text;

                    int index1 = this.SelectionStart;
                    string type = this.GetType().ToString();
                    NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                    double oldval = 0;
                    if (!this.UseNullOption && this.Value != null)
                        oldval = (double)this.Value;
                    double val1;
                    int negFlag = 0;
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
                    for (int i = 0; i < oldselection.Length; i++)
                    {
                        if (char.IsDigit(oldselection[i]))
                        {
                        }
                        else
                        {
                            oldselection = oldselection.Remove(i, 1);
                            i--;
                        }
                    }
                    if (this.SelectionLength > 0)
                    {
                        if (oldselection == string.Empty)
                        {
                            if (this.OldValue != null)
                            {
                                this.SetValue(false, this.OldValue);
                                long val3 = (long)this.OldValue;
                                this.Text = val3.ToString("N", numberFormat);
                            }
                        }
                        else
                            this.Text = this.Text.Replace(this.SelectedText, oldselection);
                    }
                    else
                    {
                        this.Text = this.Text.Insert(this.SelectionStart, oldselection);
                    }

                    if (negFlag == 1)
                        this.Text = "-" + this.Text;

                    if (numberFormat.NumberDecimalSeparator != String.Empty && numberFormat.NumberDecimalSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    {
                        this.Text = this.Text.Replace(numberFormat.NumberDecimalSeparator, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    }
                    if (numberFormat.NumberGroupSeparator != String.Empty && numberFormat.NumberGroupSeparator != CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator)
                    {
                        this.Text = this.Text.Replace(numberFormat.NumberGroupSeparator, CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
                    }
                    double.TryParse(this.Text, out val1);
                    if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress) || val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                    {
                        if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress))
                        {
                            if (this.MaxValueOnExceedMaxDigit)
                            {
                                val1 = this.MaxValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, (long)val1);
                                this.Text = val1.ToString("N", numberFormat);
                            }
                            else
                            {
                                this.SetValue(false, (long)oldval);
                                double val3 = oldval;
                                this.Text = val3.ToString("N", numberFormat);
                            }
                        }
                        if (val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                        {
                            if (this.MinValueOnExceedMinDigit)
                            {
                                val1 = this.MinValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, (long)val1);
                                this.Text = val1.ToString("N", numberFormat);
                            }
                            else
                            {
                                this.SetValue(false, (long)oldval);
                                double val3 = oldval;
                                this.Text = val3.ToString("N", numberFormat);
                            }
                        }
                    }
                    else
                    {
                        this.SetValue(false, (long)val1);
                        if (negFlag == 1)
                        {
                            this.CaretIndex = index1 + negFlag + oldselection.Length;
                        }
                        else
                            this.CaretIndex = index1 + oldselection.Length;
                        this.Text = val1.ToString("N", numberFormat);
                    }
                }
                catch (SecurityException)
                {
                    //To handle Clipboard Security Exception
                }
                catch (COMException)
                {
                    //To handle COMException for another application accessing already opened Clipboard.
                }
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseWheel"/> event occurs to provide handling for the event in a derived class without attaching a delegate.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                IntegerValueHandler.integerValueHandler.HandleUpKey(this);
            }
            else if (e.Delta < 0)
            {
                IntegerValueHandler.integerValueHandler.HandleDownKey(this);
            }
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = IntegerValueHandler.integerValueHandler.HandleKeyDown(this, e);
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
                if (e.Key == Key.C)
                {
                    copy();
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
                e.Handled = IntegerValueHandler.integerValueHandler.HandleKeyDown(this, e);
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        private void cut()
        {
            if (this.SelectionLength > 0)
            {
                try
                {
                    Clipboard.SetText(this.SelectedText);
                    this.count = 1;
                    IntegerValueHandler.integerValueHandler.HandleDeleteKey(this);
                }
                catch (SecurityException)
                {
                    //To handle Clipboard Securtiy Exception
                }
                catch (COMException)
                {
                    //To handle COMException for another application accessing already opened Clipboard.
                }
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            e.Handled = IntegerValueHandler.integerValueHandler.MatchWithMask(this, e.Text);
            base.OnTextInput(e);
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void OnCultureChanged()
        {
            base.OnCultureChanged();
            if (this.mIsLoaded)
            {
                this.FormatText();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void OnNumberFormatChanged()
        {
            base.OnNumberFormatChanged();
            if (this.mIsLoaded)
            {
                this.FormatText();
            }
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_validatingrResult)
            {
                e.Handled = true;
            }
            base.OnPreviewLostKeyboardFocus(e);
        }

#endif
        /// <summary>
        /// 
        /// </summary>
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
            }
#endif
            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.Background;
            Int64? Val = this.Value;
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
            this.checktext = "";
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.FocusedBackground;
            base.OnGotFocus(e);
        }

        #endregion overide

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        internal void FormatText()
        {
            if (this.Value != null)
            {
                NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                this.MaskedText = ((Int64)mValue).ToString("N", numberFormat);
            }
            else
            {
                this.MaskedText = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsReload"></param>
        /// <param name="_Value"></param>
        internal void SetValue(bool? IsReload, long? _Value)
        {
            if (IsReload == false)
            {
                mValueChanged = false;
                this.Value = _Value;
                mValueChanged = true;
            }
            else if (IsReload == true)
            {
                var caretindex = this.CaretIndex;
                this.Value = _Value;
                this.CaretIndex = caretindex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        internal Int64? ValidateValue(Int64? Val)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal CultureInfo GetCulture()
        {
            CultureInfo cultureInfo;
            if (Culture != null && Culture != CultureInfo.InvariantCulture)
                cultureInfo = this.Culture.Clone() as CultureInfo;
            else
                cultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;

            if (NumberFormat != null)
                cultureInfo.NumberFormat = NumberFormat;

            cultureInfo.NumberFormat.NumberDecimalDigits = 0;
            if (!GroupSeperatorEnabled)
            {
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
            }

            if (GroupSeperatorEnabled == true)
            {
#if WPF
                if (!NumberGroupSeparator.Equals(string.Empty))
                    cultureInfo.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
#else
                 if (!this.NumberGroupSeparator.Equals(string.Empty) && !(char.IsLetterOrDigit(this.NumberGroupSeparator, 0)) && this.NumberGroupSeparator.Length == 1)
                    cultureInfo.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
#endif
            }

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
#if SILVERLIGHT
            if (GroupSeperatorEnabled == true)
            {
                //Regex rgxUrl = new Regex("[^a-zA-Z0-9]");
                //string checkspecialcharacters = rgxUrl.IsMatch()
                cultureInfo.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator[0].ToString();
            }
#endif
            return cultureInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            IntegerTextBox integerTextBox = (IntegerTextBox)d;
            if (baseValue != null)
            {
                Int64? value = (Int64?)baseValue;
                if (integerTextBox.mValueChanged == true)
                {
                    if (value > integerTextBox.MaxValue)
                    {
                        value = integerTextBox.MaxValue;
                    }
                    if (value < integerTextBox.MinValue)
                    {
                        value = integerTextBox.MinValue;
                    }
                }
                if (value != null)
                {
                    integerTextBox.IsNegative = value < 0 ? true : false;
                    integerTextBox.IsZero = value == 0 ? true : false;
                    integerTextBox.IsNull = false;
                }
                return value;
            }
            else
            {
                if (integerTextBox.UseNullOption)
                {
                    integerTextBox.IsNull = true;
                    integerTextBox.IsNegative = false;
                    integerTextBox.IsZero = false;
                    return integerTextBox.NullValue;
                }
                else
                {
                    Int64 value = 0L;
                    if (integerTextBox.mValueChanged == true)
                    {
                        if (value > integerTextBox.MaxValue)
                        {
                            value = integerTextBox.MaxValue;
                        }
                        if (value < integerTextBox.MinValue)
                        {
                            value = integerTextBox.MinValue;
                        }
                    }
                    integerTextBox.IsNegative = value < 0 ? true : false;
                    integerTextBox.IsZero = value == 0 ? true : false;
                    integerTextBox.IsNull = false;
                    return value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceMinValue(DependencyObject d, object baseValue)
        {
            IntegerTextBox integerTextBox = (IntegerTextBox)d;
            if (integerTextBox.MinValue > integerTextBox.MaxValue)
            {
                baseValue = integerTextBox.MaxValue;
            }
            return baseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            IntegerTextBox integerTextBox = (IntegerTextBox)d;
            if (integerTextBox.MinValue > integerTextBox.MaxValue)
            {
                return integerTextBox.MinValue;
            }
            return baseValue;
        }

        #endregion Internal Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntegerTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            mIsLoaded = true;
            object tempObj = CoerceValue(this, Value);
            Int64? tempVal = (Int64?)tempObj;
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
                this.mValue = this.Value;
                FormatText();
            }
        }

        #region Properties

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(IntTypeConverter))]
#endif
        /// <summary>
        /// 
        /// </summary>
        public Int64? Value
        {
            get { return (Int64?)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(IntTypeConverter))]
#endif
        /// <summary>
        /// 
        /// </summary>
        public Int64 MinValue
        {
            get { return (Int64)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(IntTypeConverter))]
#endif
        /// <summary>
        /// 
        /// </summary>
        public Int64 MaxValue
        {
            get { return (Int64)GetValue(MaxValueProperty); }
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

        /// <summary>
        ///
        /// </summary>
        public bool GroupSeperatorEnabled
        {
            get { return (bool)GetValue(GroupSeperatorEnabledProperty); }
            set { SetValue(GroupSeperatorEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupSeperatorEnabled.  This enables animation, styling, binding, etc...
#if WPF
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GroupSeperatorEnabledProperty =
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(IntegerTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty GroupSeperatorEnabledProperty =
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(IntegerTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));

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
            DependencyProperty.Register("NumberGroupSizes", typeof(int[]), typeof(IntegerTextBox), new PropertyMetadata(null,OnNumberGroupSizesChanged));

#endif
#if WPF
        /// <summary>
        /// 
        /// </summary>
        public Int32Collection NumberGroupSizes
        {
            get { return (Int32Collection)GetValue(NumberGroupSizesProperty); }
            set { SetValue(NumberGroupSizesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NumberGroupSizesProperty =
            DependencyProperty.Register("NumberGroupSizes", typeof(Int32Collection), typeof(IntegerTextBox), new PropertyMetadata(new Int32Collection(), OnNumberGroupSizesChanged));

#endif

        /// <summary>
        ///
        /// </summary>
        public double ProgressFactor
        {
            get
            {
                return (percentage / this.ActualWidth);
            }
        }

#if WPF

        /// <summary>
        /// In WPF Value property acts with TwoWay binding by default.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Int64?), typeof(IntegerTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(CoerceValue), false, UpdateSourceTrigger.LostFocus));

#endif

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Int64?), typeof(IntegerTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(Int64), typeof(IntegerTextBox), new PropertyMetadata(Int64.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(Int64), typeof(IntegerTextBox), new PropertyMetadata(Int64.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

#if WPF
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(IntegerTextBox), new PropertyMetadata(string.Empty, OnNumberGroupSeparatorChanged, new CoerceValueCallback(CoerceNumberGroupSeperator)));

#else
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(IntegerTextBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public int ScrollInterval
        {
            get { return (int)GetValue(ScrollIntervalProperty); }
            set { SetValue(ScrollIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScrollInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollIntervalProperty =
            DependencyProperty.Register("ScrollInterval", typeof(int), typeof(IntegerTextBox), new PropertyMetadata(1));

        #endregion Properties

        #region PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNumberGroupSizesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            IntegerTextBox i = (IntegerTextBox)obj;
            if (i != null)
            {
                if (i != null)
                {
                    i.OnNumberGroupSizesChanged(args);
                }
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
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((IntegerTextBox)obj != null)
                ((IntegerTextBox)obj).OnValueChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object coerceValue = CoerceValue(this, this.Value);
            if (this.Value != (Int64?)coerceValue)
            {
                this.Value = (Int64?)coerceValue;
                return;
            }
#endif
            if (this.Value != null)
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

            OldValue = (Int64?)args.OldValue;
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
            if ((IntegerTextBox)obj != null)
            {
                ((IntegerTextBox)obj).OnMinValueChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal double percentage;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMinValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MinValueChanged != null)
            {
                this.MinValueChanged(this, args);
            }

            if (this.MaxValue < this.MinValue)
                this.MaxValue = this.MinValue;

            if (this.UseNullOption)
            {
                if (this.Value != null && this.MinValue != 0)
                {
                    if (this.Value < this.MinValue)
                        this.Value = this.MinValue;
                }
            }

            if (this.Value != this.ValidateValue(this.Value))
            {
                this.Value = this.ValidateValue(this.Value);
            }
            if (this.Value != null)
                percentage = (double)(this.Value * (this.ActualWidth / this.MaxValue));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((IntegerTextBox)obj != null)
            {
                ((IntegerTextBox)obj).OnMaxValueChanged(args);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnNumberGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((IntegerTextBox)obj != null)
                ((IntegerTextBox)obj).OnNumberGroupSeparatorChanged(args);
        }

        private static object CoerceNumberGroupSeperator(DependencyObject d, object baseValue)
        {
            IntegerTextBox integerTextBox = (IntegerTextBox)d;
            NumberFormatInfo numberFormat = integerTextBox.GetCulture().NumberFormat;
            if (!baseValue.Equals(string.Empty) && !(char.IsLetterOrDigit(baseValue.ToString(), 0)))
            {
                return baseValue;
            }
            else if (baseValue.Equals(string.Empty))
            {
                return baseValue;
            }
            else
                return numberFormat.NumberGroupSeparator;
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
            {
                this.NumberGroupSeparatorChanged(this, e);
            }
            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        #endregion PropertyChanged Callbacks

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        [TypeConverter(typeof(IntTypeConverter))]
#endif
        /// <summary>
        /// 
        /// </summary>
        public Int64? NullValue
        {
            get { return (Int64?)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NullValueProperty =
            DependencyProperty.Register("NullValue", typeof(Int64?), typeof(IntegerTextBox), new PropertyMetadata(null, OnNullValueChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNullValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((IntegerTextBox)obj != null)
                ((IntegerTextBox)obj).OnNullValueChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNullValueChanged(DependencyPropertyChangedEventArgs args)
        {
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void OnValueValidationCompleted(StringValidationEventArgs e)
        {
            if (ValueValidationCompleted != null)
            {
                ValueValidationCompleted(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void OnValidated(EventArgs e)
        {
            if (Validated != null)
            {
                Validated(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        public StringValidation ValueValidation
        {
            get { return (StringValidation)GetValue(ValueValidationProperty); }
            set { SetValue(ValueValidationProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueValidationProperty =
            DependencyProperty.Register("ValueValidation", typeof(StringValidation), typeof(IntegerTextBox), new PropertyMetadata(StringValidation.OnLostFocus));

        /// <summary>
        /// 
        /// </summary>
        public InvalidInputBehavior InvalidValueBehavior
        {
            get { return (InvalidInputBehavior)GetValue(InvalidValueBehaviorProperty); }
            set { SetValue(InvalidValueBehaviorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InvalidValueBehaviorProperty =
            DependencyProperty.Register("InvalidValueBehavior", typeof(InvalidInputBehavior), typeof(IntegerTextBox), new PropertyMetadata(InvalidInputBehavior.None, OnInvalidValueBehaviorChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnInvalidValueBehaviorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            IntegerTextBox m = (IntegerTextBox)obj;
            if (m != null)
            {
                m.OnInvalidValueBehaviorChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnInvalidValueBehaviorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.InvalidValueBehaviorChanged != null)
            {
                InvalidValueBehaviorChanged(this, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValidationValue
        {
            get { return (string)GetValue(ValidationValueProperty); }
            set { SetValue(ValidationValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValidationValueProperty =
            DependencyProperty.Register("ValidationValue", typeof(string), typeof(IntegerTextBox), new PropertyMetadata(string.Empty, OnValidationValueChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            IntegerTextBox m = (IntegerTextBox)obj;
            if (m != null)
            {
                m.OnValidationValueChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnValidationValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ValidationValueChanged != null)
            {
                ValidationValueChanged(this, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ValidationCompleted
        {
            get { return (bool)GetValue(ValidationCompletedProperty); }
            set { SetValue(ValidationCompletedProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValidationCompletedProperty =
            DependencyProperty.Register("ValidationCompleted", typeof(bool), typeof(IntegerTextBox), new PropertyMetadata(false, OnValidationCompletedPropertyChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationCompletedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            IntegerTextBox mask = (IntegerTextBox)obj;
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

#endif
    }
}