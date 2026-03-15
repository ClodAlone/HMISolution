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
      Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
 Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Blend;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Default;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Windows7;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
     Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.VS2010;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Metro;component/Editors/PercentTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(PercentTextBox), XamlResource = "/Syncfusion.Theming.Transparent;component/Editors/PercentTextBox.xaml")]
#endif
    public class PercentTextBox : EditorBase
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
        /// Event that is raised when <see cref="PercentageSymbol"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentageSymbolChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentEditMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentEditModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MinValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MaxValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentDecimalDigits"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentDecimalDigitsChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentDecimalSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentDecimalSeparatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentGroupSeparator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentGroupSeparatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="PercentGroupSizes"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PercentGroupSizesChanged;

        #endregion Events

        #region Members

        internal double? OldValue;
        internal double? mValue;
        internal bool? mValueChanged = true;
        internal bool mIsLoaded = false;
        internal string checktext = "";

        #endregion Members

        #region Constructor

#if WPF

        static PercentTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PercentTextBox), new FrameworkPropertyMetadata(typeof(PercentTextBox)));
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
        /// Initializes a new instance of the <see cref="PercentTextBox"/> class.
        /// </summary>
        public PercentTextBox()
        {
            pastecommand = new DelegateCommand<object>(_pastecommand, Canpaste);
            copycommand = new DelegateCommand<object>(_copycommmand, Canpaste);
            cutcommand = new DelegateCommand<object>(_cutcommmand, Canpaste);
#if WPF
            this.AddHandler(CommandManager.PreviewExecutedEvent,
new ExecutedRoutedEventHandler(CommandExecuted), true);
#endif
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(PercentTextBox));
            }
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(PercentTextBox);
#endif
            this.Loaded += IntegerTextbox_Loaded;
            this.SelectionChanged += new RoutedEventHandler(PercentTextBox_SelectionChanged);
        }

        private void _pastecommand(object parameter)
        {
            Paste();
        }

        private void _copycommmand(object parameter)
        {
            copy();
        }

        private void _cutcommmand(object parameter)
        {
            cut();
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
                    string oldtext = this.Text;
                    string copiedValue = string.Empty;
                    string afterseperator = string.Empty;
                    int seperatorindex = 0;
                    double oldval = 0;
                    if (!this.UseNullOption && this.PercentValue != null)
                        oldval = (double)this.PercentValue;
                    double val1;
                    int negFlag = 0;
                    NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
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
                                if (this.PercentEditMode == PercentEditMode.PercentMode)
                                    this.Text = val3.ToString("P", numberFormat);
                                if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                    this.Text = (val3 / 100).ToString("P", numberFormat);
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
                    else if (this.PercentValue == 0.0)
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
                    if (numberFormat != null)
                    {
                        if (this.Text.Contains(numberFormat.PercentSymbol))
                        {
                            for (int x = 0; x < this.Text.Length; x++)
                            {
                                if (this.Text[x].ToString() == numberFormat.PercentSymbol)
                                {
                                    this.Text = this.Text.Remove(x, 1);
                                }
                            }
                        }
                        if (this.Text.Contains(" "))
                        {
                            for (int x = 0; x < this.Text.Length; x++)
                            {
                                if (this.Text[x].ToString() == " ")
                                {
                                    this.Text = this.Text.Remove(x, 1);
                                }
                            }
                        }
                    }

                    if (negFlag == 1)
                        this.Text = "-" + this.Text;

                    double.TryParse(this.Text, out val1);
                    if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress) || val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                    {
                        if ((val1 > this.MaxValue) && (this.MaxValidation == MaxValidation.OnKeyPress))
                        {
                            if (this.MaxValueOnExceedMaxDigit)
                            {
                                val1 = this.MaxValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, val1);
                                if (this.PercentEditMode == PercentEditMode.PercentMode)
                                    this.Text = val1.ToString("P", numberFormat);
                                if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                    this.Text = (val1 / 100).ToString("P", numberFormat);
                            }
                            else
                            {
                                if (this.PercentEditMode == PercentEditMode.PercentMode)
                                    oldval = val1 / 100;
                                else if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                    oldval = val1;
                                if (PercentValue > this.MaxValue)
                                    oldval = this.MaxValue;
                                this.SetValue(false, oldval);
                                double val3 = oldval;
                                if (this.PercentEditMode == PercentEditMode.PercentMode)
                                    this.Text = val3.ToString("P", numberFormat);
                                if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                    this.Text = (val3 / 100).ToString("P", numberFormat);
                            }
                            int indexs = 0;
                            if (oldselection[oldselection.Length - 1].ToString() == numberFormat.PercentSymbol && oldselection[oldselection.Length - 2].ToString() == " ")
                                indexs = index1 + oldselection.Length - 2;
                            else if (oldselection[oldselection.Length - 1].ToString() == numberFormat.PercentSymbol)
                                indexs = index1 + oldselection.Length - 1;
                            else
                                indexs = index1 + oldselection.Length;
                            this.CaretIndex = indexs;
                        }
                        if (val1 < this.MinValue && (this.MinValidation == MinValidation.OnKeyPress))
                        {
                            if (this.MinValueOnExceedMinDigit)
                            {
                                val1 = this.MinValue;
                                this.CaretIndex = index1;
                                this.SetValue(false, val1);
                                if (this.PercentEditMode == PercentEditMode.PercentMode)
                                    this.Text = val1.ToString("P", numberFormat);
                                if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                    this.Text = (val1 / 100).ToString("P", numberFormat);
                            }
                            else
                            {
                                this.SetValue(false, oldval);
                                if (oldval != 0.0)
                                {
                                    double val3 = oldval;
                                    if (this.PercentEditMode == PercentEditMode.PercentMode)
                                        this.Text = val3.ToString("P", numberFormat);
                                    if (this.PercentEditMode == PercentEditMode.DoubleMode)
                                        this.Text = (val3 / 100).ToString("P", numberFormat);
                                }
                            }
                        }
                    }
                    else
                    {
                        this.SetValue(false, val1);
                        if (this.PercentEditMode == PercentEditMode.PercentMode)
                            this.Text = val1.ToString("P", numberFormat);
                        if (this.PercentEditMode == PercentEditMode.DoubleMode)
                            this.Text = (val1 / 100).ToString("P", numberFormat);
                        int indexs = 0;
                        if (oldselection[oldselection.Length - 1].ToString() == numberFormat.PercentSymbol && oldselection[oldselection.Length - 2].ToString() == " ")
                            indexs = index1 + oldselection.Length - 2;
                        else if (oldselection[oldselection.Length - 1].ToString() == numberFormat.PercentSymbol)
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

        private bool Canpaste(object parameter)
        {
            return true;
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

        private void PercentTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        #endregion Constructor

        #region overide

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        private ScrollViewer PART_ContentHost;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
#if WPF
            PART_ContentHost = this.GetTemplateChild("PART_ContentHost") as ScrollViewer;
#endif
#if SILVERLIGHT
            PART_ContentHost = this.GetTemplateChild("ContentElement") as ScrollViewer;
#endif
            base.OnApplyTemplate();
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
                PercentValueHandler.percentValueHandler.HandleUpKey(this);
            }
            else if (e.Delta < 0)
            {
                PercentValueHandler.percentValueHandler.HandleDownKey(this);
            }
        }

#if WPF

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = PercentValueHandler.percentValueHandler.HandleKeyDown(this, e);
            base.OnPreviewKeyDown(e);
        }

#endif

        /// <summary>
        /// Raises the <see cref="E:KeyDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.V)
                {
                    Paste();
                    e.Handled = true;
                }
                if (e.Key == Key.Z)
                {
                    if (IsUndoEnabled)
                    {
                        this.SetValue(true, this.OldValue);
                    }
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
                e.Handled = PercentValueHandler.percentValueHandler.HandleKeyDown(this, e);
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
                    PercentValueHandler.percentValueHandler.HandleDeleteKey(this);
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
            e.Handled = PercentValueHandler.percentValueHandler.MatchWithMask(this, e.Text);
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
        /// Raises the <see cref="E:LostFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if WPF
            if (!OnValidating(new CancelEventArgs(false)))
            {
                string validationerror = "";
                bool validationstatus = true;

                if (ValidationValue == this.PercentValue.ToString())
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
                        this.PercentValue = null;
#elif SyncfusionFramework4_0
                        this.SetCurrentValue(PercentValueProperty, null);
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
#if WPF
            if (mIsLoaded)
            {
                if (this.ValidationValue == this.PercentValue.ToString())
                    ValidationCompleted = true;
                else
                    ValidationCompleted = false;
            }
#endif
            double? Val = this.PercentValue;
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
                if (Val != this.PercentValue)
                {
                    this.PercentValue = Val;
                }
            }
            base.OnLostFocus(e);
            this.checktext = "";
        }

        /// <summary>
        /// Raises the <see cref="E:GotFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.EnableFocusColors && this.PART_ContentHost != null)
                this.PART_ContentHost.Background = this.FocusedBackground;
            base.OnGotFocus(e);
        }

        #endregion overide

        #region Internal Methods

        internal void FormatText()
        {
            if (this.PercentValue != null && !(double.IsNaN((double)this.PercentValue)))
            {
                NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
                if (this.PercentEditMode == PercentEditMode.DoubleMode)
                    this.MaskedText = (((double)mValue) / 100).ToString("P", numberFormat);
                else
                {
                    this.MaskedText = ((double)mValue).ToString("P", numberFormat);
                }
            }
            else
            {
                this.MaskedText = "";
            }
        }

        internal void SetValue(bool? IsReload, double? _Value)
        {
            if (IsReload == false)
            {
                mValueChanged = false;
                this.PercentValue = _Value;
                mValueChanged = true;
            }
            else if (IsReload == true)
            {
                var caretindex = this.CaretIndex;
                this.PercentValue = _Value;
                this.CaretIndex = caretindex;
            }
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

            if (PercentDecimalDigits >= 0)
                cultureInfo.NumberFormat.PercentDecimalDigits = this.PercentDecimalDigits;

            if (!PercentDecimalSeparator.Equals(string.Empty))
                cultureInfo.NumberFormat.PercentDecimalSeparator = this.PercentDecimalSeparator;

            if (!GroupSeperatorEnabled)
            {
                cultureInfo.NumberFormat.PercentGroupSeparator = string.Empty;
            }
            if (GroupSeperatorEnabled == true)
            {
#if WPF
                if (!PercentGroupSeparator.Equals(string.Empty))
                    cultureInfo.NumberFormat.PercentGroupSeparator = this.PercentGroupSeparator;
#else
                 if (!this.PercentGroupSeparator.Equals(string.Empty) && !(char.IsLetterOrDigit(this.PercentGroupSeparator, 0)) && this.PercentGroupSeparator.Length == 1)
                    cultureInfo.NumberFormat.PercentGroupSeparator = this.PercentGroupSeparator;
#endif
            }

#if SILVERLIGHT
            if (PercentGroupSizes != null)
                cultureInfo.NumberFormat.PercentGroupSizes = this.PercentGroupSizes;
#endif

#if WPF
            int count = this.PercentGroupSizes.Count;
            if (count > 0)
            {
                int[] ngs = new int[count];

                for (int i = 0; i < count; i++)
                {
                    ngs[i] = this.PercentGroupSizes[i];
                }
                cultureInfo.NumberFormat.PercentGroupSizes = ngs;
            }
#endif

            if (PercentNegativePattern >= 0)
                cultureInfo.NumberFormat.PercentNegativePattern = this.PercentNegativePattern;

            if (PercentPositivePattern >= 0)
                cultureInfo.NumberFormat.PercentPositivePattern = this.PercentPositivePattern;

            if (!PercentageSymbol.Equals(string.Empty))
                cultureInfo.NumberFormat.PercentSymbol = this.PercentageSymbol;

            return cultureInfo;
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var percentTextBox = (PercentTextBox)d;
            if (baseValue != null)
            {
                var value = (double?)baseValue;
                if (percentTextBox.mValueChanged == true)
                {
                    if (value > percentTextBox.MaxValue && !percentTextBox.ValidationOnLostFocus)
                    {
                        value = percentTextBox.MaxValue;
                    }
                    else if (value < percentTextBox.MinValue && !percentTextBox.ValidationOnLostFocus)
                    {
                        value = percentTextBox.MinValue;
                    }
                }
                if (value != null)
                {
                    percentTextBox.IsNegative = value < 0 ? true : false;
                    percentTextBox.IsZero = value == 0 ? true : false;
                    percentTextBox.IsNull = false;
                }
                return value;
            }
            else
            {
                if (percentTextBox.UseNullOption)
                {
                    percentTextBox.IsNull = true;
                    percentTextBox.IsNegative = false;
                    percentTextBox.IsZero = false;
                    return percentTextBox.NullValue;
                }
                else
                {
                    double value = 0L;
                    if (percentTextBox.mValueChanged == true)
                    {
                        if (value > percentTextBox.MaxValue)
                        {
                            value = percentTextBox.MaxValue;
                        }
                        if (value < percentTextBox.MinValue)
                        {
                            value = percentTextBox.MinValue;
                        }
                    }
                    percentTextBox.IsNegative = value < 0 ? true : false;
                    percentTextBox.IsZero = value == 0 ? true : false;
                    percentTextBox.IsNull = false;
                    return value;
                }
            }
        }

        private static object CoerceMinValue(DependencyObject d, object baseValue)
        {
            PercentTextBox percentTextBox = (PercentTextBox)d;
            if (percentTextBox.MinValue > percentTextBox.MaxValue)
            {
                return percentTextBox.MaxValue;
            }
            return baseValue;
        }

        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            PercentTextBox percentTextBox = (PercentTextBox)d;
            if (percentTextBox.MinValue > percentTextBox.MaxValue)
            {
                return percentTextBox.MinValue;
            }
            return baseValue;
        }

        #endregion Internal Methods

        /// <summary>
        /// Handles the Loaded event of the IntegerTextbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void IntegerTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            mIsLoaded = true;
            object tempObj = CoerceValue(this, PercentValue);
            double? tempVal = (double?)tempObj;
            if (this.IsNull == true)
            {
                this.WatermarkVisibility = Visibility.Visible;
            }
            if (tempVal != PercentValue)
            {
                PercentValue = tempVal;
            }
            else
            {
                FormatText();
            }
        }

        #region Properties

        /// <value>
        /// The boolean to enable or disable to validation on lost focus.
        /// </value>
        public bool ValidationOnLostFocus
        {
            get
            {
                return (bool)GetValue(ValidationOnLostFocusProperty);
            }

            set
            {
                SetValue(ValidationOnLostFocusProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ValidationOnLostFocus"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValidationOnLostFocusChanged;

#if WPF

        /// <summary>
        /// Identifies the <see cref="ValidationOnLostFocus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationOnLostFocusProperty =
            DependencyProperty.Register("ValidationOnLostFocus", typeof(bool), typeof(PercentTextBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValidationOnLostFocusChanged)));

#endif

#if SILVERLIGHT
        /// <summary>
        /// Identifies the <see cref="ValidationOnLostFocus"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidationOnLostFocusProperty =
           DependencyProperty.Register("ValidationOnLostFocus", typeof(bool), typeof(PercentTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnValidationOnLostFocusChanged)));

#endif

        /// <summary>
        ///
        /// </summary>
        public PercentEditMode PercentEditMode
        {
            get { return (PercentEditMode)GetValue(PercentEditModeProperty); }
            set { SetValue(PercentEditModeProperty, value); }
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
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty GroupSeperatorEnabledProperty =
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(PercentTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnPercentGroupSeparatorChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentEditModeProperty =
            DependencyProperty.Register("PercentEditMode", typeof(PercentEditMode), typeof(PercentTextBox), new PropertyMetadata(PercentEditMode.DoubleMode, OnPercentEditModeChanged));

        /// <summary>
        ///
        /// </summary>
        public double? PercentValue
        {
            get { return (double?)GetValue(PercentValueProperty); }
            set
            {
                SetValue(PercentValueProperty, value);
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

#if WPF

        // In WPF Percent Value property acts with Two Binding by default.
        public static readonly DependencyProperty PercentValueProperty =
      DependencyProperty.Register("PercentValue", typeof(double?), typeof(PercentTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(CoerceValue), false, UpdateSourceTrigger.LostFocus));

#endif

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentValueProperty =
     DependencyProperty.Register("PercentValue", typeof(double?), typeof(PercentTextBox), new PropertyMetadata(null,new PropertyChangedCallback(OnValueChanged)));
#endif

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(PercentTextBox), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(PercentTextBox), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        ///
        /// </summary>
        public int PercentDecimalDigits
        {
            get { return (int)GetValue(PercentDecimalDigitsProperty); }
            set { SetValue(PercentDecimalDigitsProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentDecimalDigitsProperty =
            DependencyProperty.Register("PercentDecimalDigits", typeof(int), typeof(PercentTextBox), new PropertyMetadata((int)-1, OnPercentDecimalDigitsChanged));

        /// <summary>
        ///
        /// </summary>
        public string PercentDecimalSeparator
        {
            get { return (string)GetValue(PercentDecimalSeparatorProperty); }
            set { SetValue(PercentDecimalSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentDecimalSeparatorProperty =
            DependencyProperty.Register("PercentDecimalSeparator", typeof(string), typeof(PercentTextBox), new PropertyMetadata(string.Empty, OnPercentDecimalSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        public string PercentGroupSeparator
        {
            get { return (string)GetValue(PercentGroupSeparatorProperty); }
            set { SetValue(PercentGroupSeparatorProperty, value); }
        }

#if WPF

        public static readonly DependencyProperty PercentGroupSeparatorProperty =
            DependencyProperty.Register("PercentGroupSeparator", typeof(string), typeof(PercentTextBox), new PropertyMetadata(string.Empty, OnPercentGroupSeparatorChanged, new CoerceValueCallback(CoercePercentGroupSeperator)));

#else

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentGroupSeparatorProperty =
            DependencyProperty.Register("PercentGroupSeparator", typeof(string), typeof(PercentTextBox), new PropertyMetadata(string.Empty, OnPercentGroupSeparatorChanged));
#endif

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
            DependencyProperty.Register("ScrollInterval", typeof(double), typeof(PercentTextBox), new PropertyMetadata(1.0));

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public int[] PercentGroupSizes
        {
            get { return (int[])GetValue(PercentGroupSizesProperty); }
            set { SetValue(PercentGroupSizesProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentGroupSizesProperty =
            DependencyProperty.Register("PercentGroupSizes", typeof(int[]), typeof(PercentTextBox), new PropertyMetadata(null, OnPercentGroupSizesChanged));
#endif

#if WPF

        public Int32Collection PercentGroupSizes
        {
            get { return (Int32Collection)GetValue(PercentGroupSizesProperty); }
            set { SetValue(PercentGroupSizesProperty, value); }
        }

        public static readonly DependencyProperty PercentGroupSizesProperty =
            DependencyProperty.Register("PercentGroupSizes", typeof(Int32Collection), typeof(PercentTextBox), new PropertyMetadata(new Int32Collection(), OnPercentGroupSizesChanged));

#endif

        /// <summary>
        ///
        /// </summary>
        public int PercentNegativePattern
        {
            get { return (int)GetValue(PercentNegativePatternProperty); }
            set { SetValue(PercentNegativePatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentNegativePatternProperty =
            DependencyProperty.Register("PercentNegativePattern", typeof(int), typeof(PercentTextBox), new PropertyMetadata((int)-1, OnPercentNegativePatternChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentNegativePatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentNegativePatternChanged(args);
            }
        }

        private static object CoercePercentGroupSeperator(DependencyObject d, object baseValue)
        {
            PercentTextBox percentTextBox = (PercentTextBox)d;
            NumberFormatInfo numberFormat = percentTextBox.GetCulture().NumberFormat;
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
                return numberFormat.PercentGroupSeparator;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentNegativePatternChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public int PercentPositivePattern
        {
            get { return (int)GetValue(PercentPositivePatternProperty); }
            set { SetValue(PercentPositivePatternProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentPositivePatternProperty =
            DependencyProperty.Register("PercentPositivePattern", typeof(int), typeof(PercentTextBox), new PropertyMetadata(-1, OnPercentPositivePatternChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentPositivePatternChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentPositivePatternChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentPositivePatternChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public string PercentageSymbol
        {
            get { return (string)GetValue(PercentageSymbolProperty); }
            set { SetValue(PercentageSymbolProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PercentageSymbolProperty =
            DependencyProperty.Register("PercentageSymbol", typeof(string), typeof(PercentTextBox), new PropertyMetadata(string.Empty, OnPercentageSymbolChanged));

        #endregion Properties

        #region PropertyChanged Callbacks

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
                p.OnPercentDecimalDigitsChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PercentDecimalDigitsChanged != null)
            {
                this.PercentDecimalDigitsChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.FormatText();
            }
        }

        /// <summary>
        /// Calls OnValidationOnLostFocusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        private static void OnValidationOnLostFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PercentTextBox instance = (PercentTextBox)d;
            instance.OnValidationOnLostFocusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see>
        ///     <cref>OnValidationOnLostFocusChanged</cref>
        /// </see>
        ///     event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValidationOnLostFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValidationOnLostFocusChanged != null)
            {
                ValidationOnLostFocusChanged(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentDecimalSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentDecimalSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentDecimalSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PercentDecimalSeparatorChanged != null)
            {
                this.PercentDecimalSeparatorChanged(this, args);
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
        public static void OnPercentGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentGroupSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentGroupSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object val = CoercePercentGroupSeperator(this, this.PercentGroupSeparator);
            if(this.PercentGroupSeparator!=val.ToString())
                this.PercentGroupSeparator=val.ToString();
#endif
            if (this.PercentGroupSeparatorChanged != null)
            {
                this.PercentGroupSeparatorChanged(this, args);
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
        public static void OnPercentGroupSizesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentGroupSizesChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentGroupSizesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PercentGroupSizesChanged != null)
            {
                this.PercentGroupSizesChanged(this, args);
            }
            if (mIsLoaded)
            {
                FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentageSymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentageSymbolChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentageSymbolChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PercentageSymbolChanged != null)
            {
                this.PercentageSymbolChanged(this, args);
            }
            if (mIsLoaded)
            {
                FormatText();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPercentEditModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox p = (PercentTextBox)obj;
            if (p != null)
            {
                p.OnPercentEditModeChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPercentEditModeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.PercentEditModeChanged != null)
            {
                this.PercentEditModeChanged(this, args);
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
            if ((PercentTextBox)obj != null)
                ((PercentTextBox)obj).OnValueChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object coerceValue = CoerceValue(this, this.PercentValue);
            if (this.PercentValue != (double?)coerceValue)
            {
                this.PercentValue = (double?)coerceValue;
                return;
            }
#endif
            if (this.PercentValue != null && !(double.IsNaN((double)this.PercentValue)))
            {
                this.IsNegative = this.PercentValue < 0 ? true : false;
                this.IsZero = this.PercentValue == 0 ? true : false;
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
            mValue = this.PercentValue;

            if (this.PercentValue != null)
                this.WatermarkVisibility = System.Windows.Visibility.Collapsed;

            if (PercentValueChanged != null)
                PercentValueChanged(this, args);
            if (this.PercentValue > this.MinValue && this.MinValidation == MinValidation.OnKeyPress)
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
            if ((PercentTextBox)obj != null)
            {
                ((PercentTextBox)obj).OnMinValueChanged(args);
            }
        }

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

            if (ValidationOnLostFocus == false)
            {
                if (this.PercentValue != this.ValidateValue(this.PercentValue))
                {
                    this.PercentValue = this.ValidateValue(this.PercentValue);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((PercentTextBox)obj != null)
            {
                ((PercentTextBox)obj).OnMaxValueChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaxValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MaxValueChanged != null)
            {
                this.MaxValueChanged(this, args);
            }

            if (this.MinValue > this.MaxValue)
                this.MinValue = this.MaxValue;

            if (ValidationOnLostFocus == false)
            {
                if (this.PercentValue != this.ValidateValue(this.PercentValue))
                {
                    this.PercentValue = this.ValidateValue(this.PercentValue);
                }
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
            DependencyProperty.Register("NullValue", typeof(double?), typeof(PercentTextBox), new PropertyMetadata(null, OnNullValueChanged));

        /// <summary>
        /// This Property used to allow multiple separator when PercentDecimalSeparator is set(^^)
        /// </summary>
#if WPF

        public bool AllowMultipleSymbol
        {
            get { return (bool)GetValue(AllowMultipleSymbolProperty); }
            set { SetValue(AllowMultipleSymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowMultipleSymbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowMultipleSymbolProperty =
            DependencyProperty.Register("AllowMultipleSymbol", typeof(bool), typeof(PercentTextBox), new PropertyMetadata(false));

#endif

        public static void OnNullValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((PercentTextBox)obj != null)
                ((PercentTextBox)obj).OnNullValueChanged(args);
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
            DependencyProperty.Register("ValueValidation", typeof(StringValidation), typeof(PercentTextBox), new PropertyMetadata(StringValidation.OnLostFocus));

        public InvalidInputBehavior InvalidValueBehavior
        {
            get { return (InvalidInputBehavior)GetValue(InvalidValueBehaviorProperty); }
            set { SetValue(InvalidValueBehaviorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InvalidValueBehaviorProperty =
            DependencyProperty.Register("InvalidValueBehavior", typeof(InvalidInputBehavior), typeof(PercentTextBox), new PropertyMetadata(InvalidInputBehavior.None, OnInvalidValueBehaviorChanged));

        public static void OnInvalidValueBehaviorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox m = (PercentTextBox)obj;
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
            DependencyProperty.Register("ValidationValue", typeof(string), typeof(PercentTextBox), new PropertyMetadata(string.Empty, OnValidationValueChanged));

        public static void OnValidationValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox m = (PercentTextBox)obj;
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
            DependencyProperty.Register("ValidationCompleted", typeof(bool), typeof(PercentTextBox), new PropertyMetadata(false, OnValidationCompletedPropertyChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationCompletedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PercentTextBox mask = (PercentTextBox)obj;
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