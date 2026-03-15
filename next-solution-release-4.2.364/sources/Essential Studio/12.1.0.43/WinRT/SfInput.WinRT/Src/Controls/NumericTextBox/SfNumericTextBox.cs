// <copyright file="NumericTextBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if !(WPF||SILVERLIGHT||WINDOWS_PHONE_7)
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;

using Windows.System;

using Windows.UI.Input;
using Windows.UI.Popups;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Controls.Input;

namespace Syncfusion.WP.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Runtime.InteropServices;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/> control. The
    /// NumericTextBox control allows the user to enter only numeric values in an
    /// application. This control has additional functionality that is not found in the
    /// standard Windows 8 text box control, including Watermark support.
    /// </summary>
    /// <remarks>
    /// The NumericTextBox is inherited from <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/> control.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfNumericTextBox : SfTextBoxExt
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfNumericTextBox()
        {
            DefaultStyleKey = typeof(SfNumericTextBox);
#if WINRT
            mapSeparator = new DecimalSeparatorCollection(BlockCharactersOnTextInput);
#endif
            this.Loaded += NumericTextBox_Loaded;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            this.ContextMenuOpening += NumericTextBox_ContextMenuOpening;
#endif
            this.TextChanged += NumericTextBox_TextChanged;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            this.resourceWrapper = new ResourceWrapper();
#endif
        }

        void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Culture != null)
            {
                if (!AllowNull && Text == string.Empty)
                {
                    IntermediateValue = 0;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (FocusManager.GetFocusedElement()!=this)
#else
                    if (FocusState == Windows.UI.Xaml.FocusState.Unfocused)
#endif
                    {
                        if (ParsingMode == Parsers.Decimal)
                            Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                        else
                            Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                    }
                    else
                    {
                        if(ParsingMode == Parsers.Decimal)                        
                            Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        else
                            Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);                       
                    }
                }
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if (FocusManager.GetFocusedElement()!=this)
#else
            if (FocusState == Windows.UI.Xaml.FocusState.Unfocused)
#endif
            {
                if (ParsingMode == Parsers.Decimal)
                {   
                    decimal intVal = internalValue;
                    if (!decimal.TryParse(Text, out intVal) && Convert.ToDecimal(Value) == 0)
                        Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                }
                else
                {
                    double intVal = internaldoubleValue;
                    if (!double.TryParse(Text, out intVal) && Convert.ToDouble(Value) == 0.0)
                        Text = intVal.ToString(FormatString, Culture.NumberFormat);
                }
            }       
        }

        #endregion

        #region Variables

#if WINRT

        internal VirtualKey[] specialkeys = { VirtualKey.Number0, VirtualKey.Number1, VirtualKey.Number2, VirtualKey.Number3, VirtualKey.Number4, 
                                                  VirtualKey.Number5, VirtualKey.Number6, VirtualKey.Number7, VirtualKey.Number8, VirtualKey.Number9, 
                                                  VirtualKey.NumberPad0, VirtualKey.NumberPad1, VirtualKey.NumberPad2, VirtualKey.NumberPad3, VirtualKey.NumberPad4, 
                                                  VirtualKey.NumberPad5, VirtualKey.NumberPad6, VirtualKey.NumberPad7, VirtualKey.NumberPad8, VirtualKey.NumberPad9,
                                            VirtualKey.Subtract, VirtualKey.Shift, VirtualKey.Decimal, VirtualKey.PageUp, VirtualKey.PageDown,
                                                VirtualKey.Up,VirtualKey.Down, VirtualKey.Enter, VirtualKey.Escape};
#endif
        internal string EnteredValue = "0";

        internal int selectionStart = 0;

        internal int selectionLength = 0;

        internal string selectedText = "";

        internal String MaskedText = "";

        internal String enteredtext = "";

        internal bool updateSelectionStart = false;

        internal Button deleteButton = null;

        internal double internaldoubleValue = 0;

        internal decimal internalValue = 0;

        internal Point mousePosition = new Point(0, 0);

        internal double popupHeight = 0.0;

        internal bool isFocused = false;

        internal bool isNegative = false;

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        internal ResourceWrapper resourceWrapper;
        internal ScrollViewer contentelement = null;
        internal PopupMenu popupMenu = null;
#endif
        //Identifies the Old Value
        internal decimal? OldValue = 0;

        internal double? OldDoubleValue = 0;

        //Format String used for internal purpose
        internal String InternalFormatString = "N0";
#if WINRT
        //Separator instance used for mapping with DecimalSeparator
        internal static DecimalSeparatorCollection mapSeparator;
#else
        //Separator instance used for mapping with DecimalSeparator
        internal DecimalSeparatorCollection mapSeparator = new DecimalSeparatorCollection(true);
#endif

        internal bool isSeparator = false;

        internal int groupSeparatorCountbefore = 0;

        internal int groupSeparatorCountafter = 0;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        internal UICommand CutCommand;

        internal UICommand CopyCommand;

        internal UICommand PasteCommand;

        internal UICommand UndoCommand;

        internal UICommand RedoCommand;

        internal UICommand SelectAllCommand;
#endif
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        /// <value>
        /// The default value is the zero.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/>
        [ClassReference(IsReviewed = false)]
        internal Object IntermediateValue
        {
            get { return (object)GetValue(IntermediateValueProperty); }
            set { SetValue(IntermediateValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntermediateValueProperty =
            DependencyProperty.Register("IntermediateValue", typeof(object), typeof(SfNumericTextBox), new PropertyMetadata(0.0, new PropertyChangedCallback(OnIntermediateValueChanged)));

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/> control value.
        /// </summary>
        public Object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                if (ValueChangedMode == ValueChange.OnLostFocus && FocusManager.GetFocusedElement() == this)
#else
                if (ValueChangedMode == ValueChange.OnLostFocus && (FocusState==FocusState.Keyboard || FocusState==FocusState.Pointer||FocusState==FocusState.Programmatic))
#endif
                    SetValue(IntermediateValueProperty,value);
                else
                    SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SfNumericTextBox), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Gets or sets the Value changed mode
        /// </summary>
        /// <value>
        /// The default value is <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.ValueChange.OnKeyFocus"/>
        /// </value>
        public ValueChange ValueChangedMode
        {
            get { return (ValueChange)GetValue(ValueChangedModeProperty); }
            set { SetValue(ValueChangedModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueChangedModeProperty =
            DependencyProperty.Register("ValueChangedMode", typeof(object), typeof(SfNumericTextBox), new PropertyMetadata(ValueChange.OnKeyFocus));


        /// <summary>
        /// Gets or sets the data that is used as a format for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/>.
        /// </summary>
        /// <value>
        /// The format string.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.TextBoxExt"/>
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
            DependencyProperty.Register("FormatString", typeof(string), typeof(SfNumericTextBox), new PropertyMetadata("N", new PropertyChangedCallback(OnFormatStringChanged)));


        /// <summary>
        /// Gets or sets the culture information associated with the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/>.
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
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(SfNumericTextBox), new PropertyMetadata(CultureInfo.CurrentUICulture, new PropertyChangedCallback(OnFormatStringChanged)));

        /// <summary>
        /// Gets or Sets the Percent DisplayMode associated with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/>
        /// </summary>
        /// <value>
        /// The Default value is <see cref="Syncfusion.UI.Xaml.Controls.Input.PercentDisplayMode">PercentDisplayMode.Compute</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public PercentDisplayMode PercentDisplayMode
        {
            get { return (PercentDisplayMode)GetValue(PercentDisplayModeProperty); }
            set { SetValue(PercentDisplayModeProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for PercentDisplayMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PercentDisplayModeProperty =
            DependencyProperty.Register("PercentDisplayMode", typeof(PercentDisplayMode), typeof(SfNumericTextBox), new PropertyMetadata(PercentDisplayMode.Compute,OnPercentDisplayModeChanged));

        
        /// <summary>
        /// Gets or Sets the number of decimal digits associated with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/>
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
            DependencyProperty.Register("MaximumNumberDecimalDigits", typeof(int), typeof(SfNumericTextBox), new PropertyMetadata(Int32.MaxValue, new PropertyChangedCallback(OnMaximumNumberDecimalDigitsChanged)));



        /// <summary>
        /// Gets or sets a value indicating whether the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/> allow null values.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if allow null value; otherwise, <see langword="false"/>.
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
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(SfNumericTextBox), new PropertyMetadata(false,OnAllowNullChanged));


        /// <summary>
        /// Gets or sets the Parsing mode
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
        /// Using a DependencyProperty as the backing store for BlockCharactersOnTextInput.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ParsingModeProperty =
            DependencyProperty.Register("ParsingMode", typeof(Parsers), typeof(SfNumericTextBox), new PropertyMetadata(Parsers.Double));


        /// <summary>
        /// Returns a value if set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
#if WINRT
        public bool BlockCharactersOnTextInput
        {
            get { return (bool)GetValue(BlockCharactersOnTextInputProperty); }
            set { SetValue(BlockCharactersOnTextInputProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BlockCharactersOnTextInput.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BlockCharactersOnTextInputProperty =
            DependencyProperty.Register("BlockCharactersOnTextInput", typeof(bool), typeof(SfNumericTextBox), new PropertyMetadata(true,OnBlockCharactersOnTextInputChanged));

        private static void OnBlockCharactersOnTextInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            mapSeparator = new DecimalSeparatorCollection((bool) e.NewValue);
        }    

#endif     
            
        #endregion

        #region Helper Methods

        void NumericTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (Culture != null && IntermediateValue != null)
            {
                if (Text == MaskedText)
                {
                    int decimallength = GetDecimalLength(MaskedText);
                    InternalFormatString = "N" + decimallength.ToString();
                    if (ParsingMode == Parsers.Decimal)
                    {
                        bool isDouble = decimal.TryParse(IntermediateValue.ToString(), out internalValue);                        
                        MaskedText = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        MaskedText = RemoveSeparator(MaskedText);
                        if (ReadLocalValue(MaximumNumberDecimalDigitsProperty) != DependencyProperty.UnsetValue && MaximumNumberDecimalDigits >= 0 && MaximumNumberDecimalDigits < 100)
                        {
                            string tempFormatString = "N" + MaximumNumberDecimalDigits;
                            decimal tempvalue = decimal.Parse(internalValue.ToString(tempFormatString, Culture.NumberFormat));
                            this.Text = tempvalue.ToString(FormatString, Culture.NumberFormat);
                        }
                        else
                            this.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                    }
                    else
                    {
                        MaskedText = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                        MaskedText = RemoveSeparator(MaskedText);
                        if (ReadLocalValue(MaximumNumberDecimalDigitsProperty) != DependencyProperty.UnsetValue && MaximumNumberDecimalDigits >= 0 && MaximumNumberDecimalDigits < 100)
                        {
                            string tempFormatString = "N" + MaximumNumberDecimalDigits;
                            double tempvalue = double.Parse(internaldoubleValue.ToString(tempFormatString, Culture.NumberFormat));
                            this.Text = tempvalue.ToString(FormatString, Culture.NumberFormat);
                        }
                        else
                            this.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                    }
                    
                    this.SelectionStart = Text.Length;         
                }
                else if (MaskedText != Text && FormatString!="P" && FormatString!="C")
                {
                    ValueFromText();

                }
            }
            if (AllowNull && Value!=null && Value.Equals(0.0) )
            {
                Value = null;
            }
        }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private async void NumericTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (BlockCharactersOnTextInput)
            {
                e.Handled = true;
                SfNumericTextBox numericTextBox = sender as SfNumericTextBox;

                var menu = new PopupMenu();

                if (numericTextBox.SelectionLength > 0)
                {
                    menu.Commands.Add(CutCommand);
                    menu.Commands.Add(CopyCommand);
                }

                DataPackageView content = Clipboard.GetContent() as DataPackageView;
                if (content.AvailableFormats.Count > 0 && content.AvailableFormats.Contains(StandardDataFormats.Text))
                {
                    if (content.GetTextAsync() != null)
                        menu.Commands.Add(PasteCommand);
                }
                if (popupMenu != null)
                {
                    if (!popupMenu.Commands.Contains(RedoCommand))
                    {
                        menu.Commands.Add(UndoCommand);
                    }
                    else
                    {
                        menu.Commands.Add(RedoCommand);
                    }
                }
                else
                    menu.Commands.Add(UndoCommand);

                if (SelectedText.Length == numericTextBox.Text.Length)
                {
                    menu.Commands.Remove(SelectAllCommand);
                }
                else
                {
                    menu.Commands.Add(SelectAllCommand);
                }
                popupHeight = (menu.Commands.Count + 1) * 40;
                Rect rect = GetTextboxSelectionRect(numericTextBox);
                var chosenCommand = await menu.ShowForSelectionAsync(rect);
                String selectedText = "";
                var dataPackage = new DataPackage();
                if (chosenCommand != null)
                {
                    switch ((int)chosenCommand.Id)
                    {
                        // Cut Command
                        case 1:
                            selectionStart = SelectionStart;
                            selectedText = numericTextBox.SelectedText;
                            dataPackage.SetText(selectedText);
                            Clipboard.SetContent(dataPackage);
                            numericTextBox.MaskedText = numericTextBox.Text.Remove(selectionStart, SelectionLength);
                            numericTextBox.MaskedText = RemoveSeparator(numericTextBox.MaskedText);
                            SetInternalFormatString(Culture.NumberFormat.NumberDecimalSeparator);
                            if (numericTextBox.MaskedText == string.Empty)
                            {
                                if (AllowNull)
                                {
                                    IntermediateValue = null;
                                    numericTextBox.Text = String.Empty;
                                }
                                else
                                {
                                    IntermediateValue = 0;
                                    if (numericTextBox.FocusState != FocusState.Unfocused)
                                    {
                                        if (ParsingMode == Parsers.Decimal)
                                            numericTextBox.Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                        else
                                            numericTextBox.Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                                    }
                                    else
                                    {
                                        if (ParsingMode == Parsers.Decimal)
                                            numericTextBox.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                                        else
                                            numericTextBox.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                                    }
                                }
                            }
                            else
                            {
                                if (ParsingMode == Parsers.Decimal)
                                {
                                    IntermediateValue = decimal.Parse(numericTextBox.MaskedText);
                                    Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                }
                                else
                                {
                                    IntermediateValue = double.Parse(numericTextBox.MaskedText);
                                    Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                                }

                            }
                            SelectionStart = selectionStart;
                            break;

                        // Copy Command
                        case 2:
                            selectedText = numericTextBox.SelectedText;
                            dataPackage.SetText(selectedText);
                            Clipboard.SetContent(dataPackage);
                            break;

                        // Paste Command
                        case 3:
                            selectionStart = SelectionStart;
                            selectionLength = SelectionLength;
                            selectedText = await Paste(numericTextBox, content);
                            break;

                        // Undo Command
                        case 4:
                            selectionStart = SelectionStart;
                            if (ParsingMode == Parsers.Decimal)
                                this.IntermediateValue = OldValue;
                            else
                                this.IntermediateValue = OldDoubleValue;
                            SelectionStart = selectionStart;
                            menu.Commands.Remove(UndoCommand);
                            menu.Commands.Add(RedoCommand);
                            break;

                        // Redo Command
                        case 5:
                            selectionStart = SelectionStart;
                            if (ParsingMode == Parsers.Decimal)
                                this.IntermediateValue = OldValue;
                            else
                                this.IntermediateValue = OldDoubleValue;
                            SelectionStart = selectionStart;
                            menu.Commands.Remove(RedoCommand);
                            menu.Commands.Add(UndoCommand);
                            break;

                        // Select All command
                        case 6:
                            SelectionStart = 0;
                            SelectionLength = numericTextBox.Text.Length;
                            break;
                    }
                }
                popupMenu = menu;
            }
        }

#if SyncfusionFramework4_5_1
        private async new Task<string> Paste(SfNumericTextBox numericTextBox, DataPackageView content)
#else
        private async Task<string> Paste(SfNumericTextBox numericTextBox, DataPackageView content)
#endif
#else 
#if !WINDOWS_PHONE_7
       private string Paste(SfNumericTextBox numericTextBox, string content)
#endif
#endif
#if !WINDOWS_PHONE_7
        {
#if WINDOWS_PHONE
            if (content !=null && Culture != null)
#else
            if (content.AvailableFormats.Count > 0 && Culture != null)
#endif
            {
                selectedText = numericTextBox.SelectedText;
                selectionStart = SelectionStart;
#if WINDOWS_PHONE
                String clipboardText =content;
#else
                String clipboardText = await content.GetTextAsync();
#endif
                String parsedText = RemoveCharacter(clipboardText);
                if (selectionLength > 0)
                {
                    MaskedText = Text.Remove(selectionStart, selectionLength);
                    MaskedText = MaskedText.Insert(selectionStart, parsedText);
                }
                else
                {
                    MaskedText = Text.Insert(selectionStart, parsedText);
                }
                String originalText = RemoveSeparator(MaskedText);
                if (originalText.Length > MaxLength && MaxLength > 0)
                {
                    if (originalText.Contains(Culture.NumberFormat.NumberDecimalSeparator))
                    {
                        MaskedText = originalText.Substring(0, MaxLength+ (Culture.NumberFormat.NumberDecimalSeparator.Length));
                    }
                    else
                    {
                        MaskedText = originalText.Substring(0, MaxLength);
                    }
                }
                int currentindex = MaskedText.IndexOf(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                int decimallength = 0;
                if (currentindex > 0)
                {
                    decimallength = GetDecimalLength(MaskedText);
                }
                InternalFormatString = "N" + decimallength.ToString();
                int len = Text.Length;
                if (MaskedText != string.Empty)
                {
                    if (ParsingMode == Parsers.Decimal)
                    {
                        if (!MaskedText.Contains(Culture.NumberFormat.NumberDecimalSeparator) && MaskedText.Length > decimal.MaxValue.ToString().Length)
                        {
                            MaskedText = MaskedText.Substring(0, decimal.MaxValue.ToString().Length);
                        }
                        IntermediateValue = decimal.Parse(MaskedText);
                        Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                    }
                    else
                    {
                        IntermediateValue = double.Parse(MaskedText);
                        Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                    }     
                }
                else
                {
                    if (AllowNull)
                    {
                        IntermediateValue = null;
                        Text = String.Empty;
                    }
                    else
                    {
                        IntermediateValue = 0;
#if !(WINDOWS_PHONE)
                        if (this.FocusState != FocusState.Unfocused)
#endif
                        {
                            if (ParsingMode == Parsers.Decimal)
                                Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                            else
                                Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                        }
#if !(WINDOWS_PHONE)
                        else
                        {
                            if(ParsingMode == Parsers.Decimal)
                                Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                            else
                                Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                        }
#endif
                    }
                }
                if (selectedText == string.Empty)
                    SelectionStart = selectionStart + Math.Abs(Text.Length - len);
                else
                    SelectionStart = selectionStart + parsedText.Length;
            }
            return selectedText;
        }
#endif
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        // returns a rect for selected text
        // if no text is selected, returns caret location
        // textbox should not be empty
        private Rect GetTextboxSelectionRect(SfNumericTextBox textbox)
        {
            GeneralTransform buttonTransform = textbox.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            Rect rect = new Rect(point.X + mousePosition.X, 0, 0, point.Y + mousePosition.Y);
            double windowHeight = Window.Current.CoreWindow.Bounds.Height;
            if (rect.Height + popupHeight > windowHeight)
            {
                rect = new Rect(point.X + mousePosition.X, 0, 0, Math.Abs(windowHeight - popupHeight));
            }
            return rect;
        }
#endif

        //Retrieves Value from Text if Text Property is set directly
        private void ValueFromText()
        {
            if (Culture != null)
            {
                MaskedText = RemoveSeparator(Text);
                SetInternalFormatString(Culture.NumberFormat.NumberDecimalSeparator);
                if (MaskedText != String.Empty)
                {
                    if (ParsingMode == Parsers.Decimal)
                    {
                        bool isdecimal = decimal.TryParse(MaskedText, out internalValue);
                        if (isdecimal)
                        {
                            IntermediateValue = internalValue;
                            Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                        }
                    }
                    else
                    {
                        bool isdouble = double.TryParse(MaskedText, out internaldoubleValue);
                        if (isdouble)
                        {
                            IntermediateValue = internaldoubleValue;
                            Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                        }
                    }
                }
                else
                {
                    if (AllowNull)
                    {
                        IntermediateValue = null;
                        Text = String.Empty;
                    }
                    else
                    {
                        IntermediateValue = 0;
                        if(ParsingMode == Parsers.Decimal)
                            Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                        else
                            Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                    }
                }
            }
        }

        void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllowNull)
            {
                IntermediateValue = null;
            }
            else
            {
                IntermediateValue = 0;
                InternalFormatString = "N0";
                if (Culture != null)
                {
                    if(ParsingMode == Parsers.Decimal)
                        Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                    else
                        Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                }
            }
        }

        //Performed when Backspace Key is pressed
        private void HandleBackspaceKey(String numberDecimalSeparator, String numberGroupSeparator)
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if(SelectionStart > 0)
            selectionStart = SelectionStart -1;
#else
            selectionStart = SelectionStart;
#endif
            isSeparator = false;
            groupSeparatorCountbefore = 0;
            groupSeparatorCountafter = 0;            
            groupSeparatorCountbefore = GetGroupSeparatorCount(MaskedText, numberGroupSeparator);
            if (MaskedText.Length > selectionStart && selectionStart > 0 && SelectionLength <= 1)
            {
                if (MaskedText[selectionStart].ToString() == numberDecimalSeparator || selectedText.Contains(numberDecimalSeparator))
            {
                    isSeparator = true;
                    Text = Text.Insert(selectionStart, numberDecimalSeparator);
                }
                else if (MaskedText[selectionStart].ToString() == numberGroupSeparator)
                {
                    isSeparator = true;
                    Text = Text.Insert(selectionStart, numberGroupSeparator);
                }
            }
            if (Text.StartsWith(numberGroupSeparator))
            {
                Text = Text.Remove(0, 1);
            }
            else if (Text.StartsWith(numberDecimalSeparator))
            {
                if (AllowNull)
                {
                    if (Text.Length == 1)
                    {
                        Text = String.Empty;
                        IntermediateValue = null;
                    }
                    else
                    {
                        Text = Text.Insert(0, "0");
                    }
                }
                else
                {
                    Text = Text.Insert(0, "0");
                }
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            else
            {
                if (SelectionLength > 0)
                {
                    selectionStart++;
                    if (SelectedText.Length == Text.Length ||(selectionStart==1 && SelectedText[0].ToString()!=numberGroupSeparator&&SelectedText[0].ToString()!=numberDecimalSeparator))
                        selectionStart = 0;
                    Text = Text.Remove(selectionStart, SelectionLength);
                }
                else if(SelectionStart > 0)
                    Text = Text.Remove(selectionStart, 1);
            }
#endif
            if (Culture != null)
            {
                if (Text == String.Empty || (Text.Length == 1 && Text.Contains(Culture.NumberFormat.NegativeSign)))
                {
                    if (AllowNull)
                    {
                        IntermediateValue = null;
                        Text = String.Empty;
                    }
                    else
                    {
                        IntermediateValue = 0;
                        if(ParsingMode == Parsers.Decimal)
                            Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        else
                            Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                        selectionStart = 0;
                    }
                }
            }
            SetValue(numberDecimalSeparator);
            int index = MaskedText.IndexOf(numberDecimalSeparator);
            groupSeparatorCountafter = GetGroupSeparatorCount(MaskedText, numberGroupSeparator);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if ((index <= 0 || selectionStart-1 <= index) && !isSeparator && groupSeparatorCountbefore > groupSeparatorCountafter && selectionStart > 0)
#else
            if ((index <= 0 || selectionStart <= index) && !isSeparator && groupSeparatorCountbefore > groupSeparatorCountafter && selectionStart > 0)
#endif
            {
                selectionStart--;
            }
            SelectionStart = selectionStart;
        }


        //Performed when Delete Key is pressed
        private void HandleDeleteKey(String numberDecimalSeparator, String numberGroupSeparator)
        {
            selectionStart = SelectionStart;
            isSeparator = false;
            groupSeparatorCountbefore = 0 ;
            groupSeparatorCountafter = 0;
            groupSeparatorCountbefore = GetGroupSeparatorCount(MaskedText, numberGroupSeparator);
            if (MaskedText.Length > selectionStart)
            {
                if (MaskedText[selectionStart].ToString() == numberDecimalSeparator || selectedText.Contains(numberDecimalSeparator))
                {
                    isSeparator = true;
                    Text = Text.Insert(selectionStart, numberDecimalSeparator);
                    selectionStart++;
                }
                else if (MaskedText[selectionStart].ToString() == numberGroupSeparator)
                {
                    isSeparator = true;
                    Text = Text.Insert(selectionStart, numberGroupSeparator);
                    selectionStart++;
                }
            }
            if (Text.StartsWith(numberGroupSeparator))
            {
                Text = Text.Remove(0, 1);
            }
            else if (Text.StartsWith(numberDecimalSeparator))
            {
                if (AllowNull)
                {
                    if (Text.Length == 1)
                    {
                        Text = String.Empty;
                        IntermediateValue = null;
                    }
                    else
                    {
                        Text = Text.Insert(0, "0"); 
                        selectionStart++;
                    }
                }
                else
                {
                    Text = Text.Insert(0, "0");
                    selectionStart++;
                }
            }
            if (Culture != null)
            {
                if (Text == String.Empty)
                {
                    if (AllowNull)
                    {
                        IntermediateValue = null;
                        Text = String.Empty;
                    }
                    else
                    {
                        IntermediateValue = 0;
                        if(ParsingMode == Parsers.Decimal)
                            Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        else
                            Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                    }
                }
            }
            SetValue(numberDecimalSeparator);
            groupSeparatorCountafter = GetGroupSeparatorCount(MaskedText, numberGroupSeparator);
            int index = MaskedText.IndexOf(numberDecimalSeparator);
            if ((index <= 0 || selectionStart <= index) && !isSeparator && groupSeparatorCountbefore > groupSeparatorCountafter && selectionStart > 0)
            {
                selectionStart--;
            }
            SelectionStart = selectionStart;
        }

        private int GetGroupSeparatorCount(String text, String numberGroupSeparator)
        {
            int groupSeparatorCount = 0;
            foreach (char character in text)
            {
                if (character.ToString().Equals(numberGroupSeparator))
                {
                    groupSeparatorCount++;
                }
            }
            return groupSeparatorCount;
        }

        private void SetValue(String numberDecimalSeparator)
        {
            int index = Text.IndexOf(numberDecimalSeparator);
            if (index > 0)
            {
                int decimallength = GetDecimalLength(Text);
                InternalFormatString = "N" + decimallength.ToString();
            }
            else
            {
                InternalFormatString = "N0";
            }

            if (Text == String.Empty)
            {
                if (AllowNull)
                {
                    IntermediateValue = null;
                }
                else
                {
                    IntermediateValue = 0;
                }
            }
            else
            {
                MaskedText = RemoveSeparator(Text);
                if(ParsingMode == Parsers.Decimal)
                {
                    this.IntermediateValue = !MaskedText.Equals(string.Empty) ? decimal.Parse(MaskedText) : 0;
                if (AllowNull && IntermediateValue != null && (internalValue == 0))
                {
                    IntermediateValue = null;
                    Text = String.Empty;
                }
                }
                else
                {
                    this.IntermediateValue = !MaskedText.Equals(string.Empty) ? double.Parse(MaskedText) : 0;
                if (AllowNull && IntermediateValue != null && (internaldoubleValue == 0))
                {
                    IntermediateValue = null;
                    Text = String.Empty;
                }
                }
            }
        }


        //Get the length of the decimal digits
        private int GetDecimalLength(String text)
        {
            int index = text.IndexOf(Culture.NumberFormat.NumberDecimalSeparator);
            int decimallength = text.Length - index - 1;

            if (index > 0 && decimallength > MaximumNumberDecimalDigits)
                decimallength = MaximumNumberDecimalDigits;

            return decimallength;
        }

        private void MatchWithMask(String enteredText, String numberDecimalSeparator, String negativesign, String numberGroupSeparator)
        {
            groupSeparatorCountbefore = 0;
            groupSeparatorCountafter = 0;
            groupSeparatorCountbefore = GetGroupSeparatorCount(Text, numberGroupSeparator);
            String maskedText = "";
            String originalText = RemoveCharacter(Text);
            if (originalText.Length < MaxLength || MaxLength <= 0)
            {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if(BlockCharactersOnTextInput && enteredText == negativesign)
#else
                if(enteredText == negativesign)
#endif
                {
                    if (internaldoubleValue == 0)
                    {
                        isNegative = !isNegative;
                    }
                    else
                        isNegative = false;
                    selectionStart = SelectionStart;
                    if (ParsingMode == Parsers.Decimal)
                    {
                        if (internalValue != 0)
                        {
                            IntermediateValue = internalValue * -1;
                        }
                    }
                    else
                    {
                        if (internaldoubleValue != 0)
                        {
                            IntermediateValue = internaldoubleValue * -1;
                        }
                    }
                    if (Text.Contains(negativesign))
                    {
                        selectionStart++;
                    }
                    else
                    {
                        selectionStart--;
                    }
                    if (selectionStart >= 0)
                        SelectionStart = selectionStart;
                }
                else
                {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (BlockCharactersOnTextInput && ((internalValue == 0 && ParsingMode == Parsers.Decimal) || (internaldoubleValue == 0 && ParsingMode== Parsers.Double)) && !Text.Contains(numberDecimalSeparator))
#else
                        if (((internalValue == 0 && ParsingMode == Parsers.Decimal) || (internaldoubleValue == 0 && ParsingMode== Parsers.Double)) && !Text.Contains(numberDecimalSeparator))
#endif
                    {
                        if (enteredText == numberDecimalSeparator && MaximumNumberDecimalDigits > 0)
                        {
                            if (!this.MaskedText.Contains(numberDecimalSeparator))
                            {
                                selectionStart = MaskedText.Length;
                                this.MaskedText = this.MaskedText.Insert(selectionStart, enteredText);
                                this.Text = MaskedText;
                                selectionStart++;
                                SelectionStart = selectionStart;
                            }
                        }
                        else if (enteredText == numberDecimalSeparator && MaximumNumberDecimalDigits <= 0)
                        {
                            return;
                        }
                        else
                        {
                            if (isNegative)
                            {
                                if (this.ParsingMode == Parsers.Decimal)
                                {
                                    IntermediateValue = decimal.Parse(EnteredValue) * -1;
                                }
                                else if (this.ParsingMode == Parsers.Double)
                                {
                                    IntermediateValue = double.Parse(EnteredValue) * -1;
                                }
                                isNegative = false;
                                this.SelectionStart++;
                            }
                            else
                            {
                                if (this.ParsingMode == Parsers.Decimal)
                                {
                                    IntermediateValue = decimal.Parse(EnteredValue);
                                }
                                else if (this.ParsingMode == Parsers.Double)
                                {
                                    IntermediateValue = double.Parse(EnteredValue);
                                }
                            }
                            SetInternalFormatString(numberDecimalSeparator);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                            if (BlockCharactersOnTextInput)
#endif
                            {
                                if (ParsingMode == Parsers.Decimal)
                                    this.MaskedText = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                else
                                    this.MaskedText = internaldoubleValue.ToString(InternalFormatString,Culture.NumberFormat);
                            }
                            this.SelectionStart++;
                        }
                    }
                    else
                    {
                        if (Text.StartsWith(numberDecimalSeparator))
                        {
                            this.MaskedText = this.Text;
                        }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                        else if(BlockCharactersOnTextInput)
#else
                            else
#endif
                        {
                            if (ParsingMode == Parsers.Decimal)
                                this.MaskedText = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                            else
                                this.MaskedText = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                        }

                        string unMaskedText = RemoveCharacter(MaskedText);
                        if (MaskedText.Length == SelectionStart)
                        {
                            updateSelectionStart = true;
                            selectionStart = unMaskedText.Length;
                        }
                        else
                        {
                            updateSelectionStart = false;
                            selectionStart = this.SelectionStart;
                        }

                        if (SelectionLength > 0)
                        {
                            if (!SelectedText.Contains(numberDecimalSeparator))
                            {
                                if (SelectionStart < MaskedText.Length && MaskedText.Contains(SelectedText))
                                {
                                    this.MaskedText = this.MaskedText.Remove(SelectionStart, SelectedText.Length);
                                    this.MaskedText = this.MaskedText.Insert(SelectionStart, enteredtext);
                                }
                            }
                            else
                            {
                                String extractedText = "";
                                String decimalText = "";
                                int index = MaskedText.IndexOf(numberDecimalSeparator);
                                if (SelectionStart + SelectionLength != MaskedText.Length)
                                {
                                    for (int i = SelectionStart; i < SelectionStart + SelectionLength; i++)
                                    {
                                        if (i > index)
                                        {
                                            MaskedText = MaskedText.Remove(i, 1);
                                            MaskedText = MaskedText.Insert(i, "0");
                                        }
                                        else if (i < index)
                                        {
                                            extractedText = extractedText + MaskedText[i];
                                        }
                                    }
                                    if (extractedText.Length > 0)
                                        this.MaskedText = this.MaskedText.Replace(extractedText, enteredText);
                                    decimalText = MaskedText.Substring(index + 1);
                                    if (decimalText.Length > 0)
                                    {
                                        int decimalValue = GetDecimalLength(decimalText);
                                        if (decimalValue > 0)
                                        {
                                            InternalFormatString = "N" + decimalValue.ToString().Length;
                                        }
                                        else
                                            InternalFormatString = "N0";
                                    }
                                }
                                else
                                {
                                    this.MaskedText = this.MaskedText.Replace(SelectedText, enteredText);
                                }
                            }

                        }
                        else
                        {
                            if (enteredText == numberDecimalSeparator && MaximumNumberDecimalDigits > 0)
                            {
                                if (!this.MaskedText.Contains(numberDecimalSeparator))
                                {
                                    selectionStart = MaskedText.Length;
                                    this.MaskedText = this.MaskedText.Insert(selectionStart, enteredText);
                                }
                            }
                            else if (enteredText == numberDecimalSeparator && MaximumNumberDecimalDigits <= 0)
                            {
                                return;
                            }
                            else
                            {
                                if (MaskedText.Length < SelectionStart)
                                    MaskedText = Text;

                                int index = MaskedText.IndexOf(numberDecimalSeparator);
                                if (MaskedText.Contains(numberDecimalSeparator) && SelectionStart == MaskedText.Length && index > 0)
                                {
                                    int decimallength = GetDecimalLength(MaskedText);
                                    if (decimallength >= MaximumNumberDecimalDigits)
                                    {
                                        SelectionStart = MaskedText.Length;
                                        return;
                                    }
                                }
                                if (this.MaskedText.Contains(numberDecimalSeparator) && !MaskedText.EndsWith(numberDecimalSeparator) && Text.EndsWith(numberDecimalSeparator)
                                    || (index > 0 && MaskedText.Length > SelectionStart && SelectionStart > index))
                                {
                                    this.MaskedText = this.MaskedText.Remove(SelectionStart, MaskedText[SelectionStart].ToString().Length);
                                    this.MaskedText = this.MaskedText.Insert(SelectionStart, enteredText);
                                }
                                else
                                {
                                    this.MaskedText = this.MaskedText.Insert(SelectionStart, enteredText);
                                }
                            }
                            if (enteredText == numberDecimalSeparator)
                            {
                                updateSelectionStart = false;
                                selectionStart = MaskedText.Length;
                            }
                        }
                        if (!updateSelectionStart)
                        {
                            if (MaskedText.Contains(numberDecimalSeparator) && enteredText == numberDecimalSeparator)
                            {
                                selectionStart = MaskedText.IndexOf(numberDecimalSeparator) + 1;
                            }
                            else
                            {
                                selectionStart++;
                            }
                        }

                        if (SelectionStart == MaskedText.Length - 1 && MaskedText.Contains(numberDecimalSeparator))
                        {
                            updateSelectionStart = false;
                            if (MaskedText.StartsWith(numberDecimalSeparator))
                                selectionStart = MaskedText.Length + 1;
                            else
                                selectionStart = MaskedText.Length;
                        }
                        SetInternalFormatString(numberDecimalSeparator);
                        this.MaskedText = RemoveSeparator(MaskedText);
                        maskedText = this.MaskedText;
#if WINRT
                        if (ParsingMode == Parsers.Decimal && BlockCharactersOnTextInput)
#else
                            if (ParsingMode == Parsers.Decimal)
#endif
                        {
                            if (maskedText.Length > decimal.MaxValue.ToString().Length && !maskedText.Contains(numberDecimalSeparator))
                                return;
                            decimal temp = internalValue;
                            if (maskedText.StartsWith(numberDecimalSeparator) && maskedText.Length == 1)
                            {
                                IntermediateValue = null;
                                Text = MaskedText;
                            }
                            else
                            {
                                decimal newValue = decimal.Parse(maskedText);
                                if (!newValue.Equals(decimal.Parse(IntermediateValue.ToString())))
                                    this.IntermediateValue = decimal.Parse(maskedText);
                            }
                            if (internalValue == temp && !MaskedText.EndsWith(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator))
                            {

                                Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);

                            }
                        }
#if WINRT
                        else if(ParsingMode == Parsers.Double && BlockCharactersOnTextInput)
#else
                            else if(ParsingMode == Parsers.Double)
#endif
                        {
                            if (maskedText.Length > decimal.MaxValue.ToString().Length && !maskedText.Contains(numberDecimalSeparator))
                                return;
                            double temp = internaldoubleValue;
                            if (maskedText.StartsWith(numberDecimalSeparator) && maskedText.Length == 1)
                            {
                                IntermediateValue = null;
                                Text = MaskedText;
                            }
                            else
                            {
                                if (ParsingMode == Parsers.Double)
                                {
                                    double newValue = double.Parse(maskedText);
                                    object parsedValue = IntermediateValue != null ? double.Parse(IntermediateValue.ToString()) : IntermediateValue;
                                    if (!newValue.Equals(parsedValue))
                                    {
                                        if (isNegative)
                                        {
                                            this.IntermediateValue = newValue * -1;
                                            selectionStart++;
                                            isNegative = false;
                                        }
                                        else
                                        {
                                            this.IntermediateValue = newValue;
                                        }
                                    }
                                }
                                else
                                {
                                    decimal newValue = decimal.Parse(maskedText);
                                    if (!newValue.Equals(decimal.Parse(IntermediateValue.ToString())))
                                        if (isNegative)
                                        {
                                            this.IntermediateValue = newValue * -1;
                                            selectionStart++;
                                            isNegative = false;
                                        }
                                        else
                                        {
                                            this.IntermediateValue = newValue;
                                        }
                                }
                            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                            if (Value != null && MaskedText != string.Empty && Value.ToString() != maskedText)
                            {
                                bool isdouble = double.TryParse(maskedText, out internaldoubleValue);
                                Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                            }
#endif
                            if (internaldoubleValue == temp && !MaskedText.EndsWith(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator))
                            {

                                Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);

                            }
                        }
                        MaskedText = maskedText;
                        if (isNegative)
                        {
                            if (ParsingMode == Parsers.Decimal)
                            {
                                if (internalValue != 0)
                                {
                                    IntermediateValue = internalValue * -1;
                                }
                            }
                            else
                            {
                                if (internaldoubleValue != 0)
                                {
                                    IntermediateValue = internaldoubleValue * -1;
                                }
                            } 
                            isNegative = false;
                        }
                        //Updates SelectionStart
                        if (updateSelectionStart)
                        {
                            Int64 tempValue;
                            bool isInt = Int64.TryParse(MaskedText, out tempValue);
                            maskedText = tempValue.ToString(InternalFormatString, Culture.NumberFormat);
                            if (maskedText.Length < MaskedText.Length)
                            {
                                MaskedText = RemoveSeparator(MaskedText);
                                if (ParsingMode == Parsers.Decimal)
                                {
                                    decimal newValue = decimal.Parse(MaskedText);
                                    maskedText = newValue.ToString(InternalFormatString, Culture.NumberFormat);
                                }
                                else
                                {
                                    double newValue = double.Parse(MaskedText);
                                    maskedText = newValue.ToString(InternalFormatString, Culture.NumberFormat);
                                }
                            }
                            int j = -1;
                            int i1 = 0;

                            int selectionstart_temp = 0;
                            int selectionlength_temp = 0;

                            bool textflag = false;
                            int textpos = 0;
                            for (i1 = 0; i1 < maskedText.Length; )
                            {
                                if (char.IsDigit(maskedText[i1]))
                                {
                                    j++;
                                }

                                if (j == selectionStart)
                                {
                                    selectionstart_temp = i1;
                                    textflag = true;
                                }

                                if (textflag == true && char.IsDigit(maskedText[i1]))
                                {
                                    textpos++;
                                }

                                if (textflag == true && textpos == enteredText.Length)
                                {
                                    if (Char.IsDigit(maskedText[i1]))
                                        selectionlength_temp = i1 + 1;
                                    else
                                        selectionlength_temp = i1;
                                    break;
                                }

                                i1++;
                            }
                            this.SelectionStart = selectionlength_temp;
                        }
                        else
                        {
                            int index = Text.IndexOf(numberDecimalSeparator);
                            groupSeparatorCountafter = GetGroupSeparatorCount(Text, numberGroupSeparator);
                            if ((index <= 0 || index > selectionStart) && groupSeparatorCountafter > groupSeparatorCountbefore)
                            {
                                selectionStart++;
                            }
                            this.SelectionStart = selectionStart;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        //Sets Internal FormatString for internal purpose
        private void SetInternalFormatString(String numberDecimalSeparator)
        {
            if (this.MaskedText.Contains(numberDecimalSeparator))
            {
                String tempFormattedString;
                if(ParsingMode == Parsers.Decimal)
                 tempFormattedString = internalValue.ToString("N0", Culture.NumberFormat);
                else
                    tempFormattedString = internaldoubleValue.ToString("N0", Culture.NumberFormat);
                if (MaskedText.EndsWith(numberDecimalSeparator))
                {
                    InternalFormatString = "N1";
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (!BlockCharactersOnTextInput)
                    {
                        Text = MaskedText;
                    }
                    else
#endif
                        this.Text = tempFormattedString + enteredtext;

                    if (enteredtext == numberDecimalSeparator)
                    {
                        selectionStart = Text.Length;
                        updateSelectionStart = false;
                    }

                }
                else
                {
                    int index = MaskedText.IndexOf(numberDecimalSeparator);
                    int decimallength = GetDecimalLength(MaskedText);
                    InternalFormatString = "N" + decimallength.ToString();
                }
            }
            else
            {
                InternalFormatString = "N0";
            }
        }

        //Removing Number Group Separator

        private String RemoveSeparator(String text)
        {
            bool isInserted = false;
            int i = 0;
            foreach (char character in text)
            {
                if (!char.IsNumber(character) && character.ToString() != Culture.NumberFormat.NumberDecimalSeparator)
                {
                    if (text.Contains(character.ToString()))
                    {
                        text = text.Remove(text.IndexOf(character), 1);
                        if (text.Length > 0 && character.ToString() == Culture.NumberFormat.NegativeSign && i == 0 && !text.Contains(Culture.NumberFormat.NegativeSign))
                        {
                            text = text.Insert(i, CultureInfo.CurrentUICulture.NumberFormat.NegativeSign);
                        }
                        i--;
                    }
                }
                //Removing Separator if Text contains more than one NumberDecimalSeparator
                if (character.ToString() == Culture.NumberFormat.NumberDecimalSeparator)
                {
                    if (!isInserted)
                    {
                        int index = text.IndexOf(character);
                        if (index >= 0)
                        {
                            text = text.Remove(index, 1);
                            if (MaximumNumberDecimalDigits > 0 && index >= 0)
                            {
                                text = text.Insert(index, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                isInserted = true;
                            }

                        }
                    }
                    else
                    {
                        text = text.Remove(i, 1);
                        i--;
                    }

                }
                i++;
            }
            return text;
        }

        //Removing Unwanted characters

        private String RemoveCharacter(String text)
        {
            foreach (char character in text)
            {
                if (!char.IsNumber(character))
                {
                    text = text.Remove(text.IndexOf(character), 1);
                }
            }
            return text;
        }

        //Checks whether FormatString=="P" and PercentDisplayMode is Value
        private bool CheckPercentDisplayMode()
        {
            if (FormatString!=null && FormatString.ToUpper().Equals("P") && PercentDisplayMode == PercentDisplayMode.Value)
                return true;
            else
                return false;

        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            deleteButton = GetTemplateChild("DeleteButton") as Button;
            if (deleteButton != null)
                deleteButton.Click += deleteButton_Click;
            GeneralTransform tranform = this.TransformToVisual(null);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            Point pp = tranform.Transform(new Point(0, 0));
#else
            Point pp = tranform.TransformPoint(new Point(0, 0));
            if (resourceWrapper != null)
            {
                CutCommand = new UICommand(resourceWrapper.Cut, null, 1);
                CopyCommand = new UICommand(resourceWrapper.Copy, null, 2);
                PasteCommand = new UICommand(resourceWrapper.Paste, null, 3);
                UndoCommand = new UICommand(resourceWrapper.Undo, null, 4);
                RedoCommand = new UICommand(resourceWrapper.Redo, null, 5);
                SelectAllCommand = new UICommand(resourceWrapper.SelectAll, null, 6);
            }
#endif
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            mousePosition = e.GetPosition(this);
            base.OnMouseLeftButtonDown(e);
        }
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            PointerPoint currentPoint = e.GetCurrentPoint(this);
            mousePosition = currentPoint.Position;
            base.OnPointerPressed(e);
        }
#endif
#if WINRT
        //[DllImport("user32.dll")]
        //private static extern long GetKeyboardLayoutName(
        //System.Text.StringBuilder pwszKLID);
#endif
        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE_7||WINDOWS_PHONE
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
#else
        protected async override void OnKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
#endif
        {
            if (Culture != null)
            {
                String numberDecimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;
                String numberGroupSeparator = Culture.NumberFormat.NumberGroupSeparator;
                String negativeSign = Culture.NumberFormat.NegativeSign;
                String pressedText = e.Key.ToString();
                bool shiftKey = false;

#if! (WINDOWS_PHONE||WINDOWS_PHONE_7)
                        //CoreWindow for the active thread for using GetKeyState Method
                        var coreWindow = CoreWindow.GetForCurrentThread() as CoreWindow;

                        var downState = CoreVirtualKeyStates.Down;

                        //Specifies if Alt Key is pressed
                        bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;

                        //Specifies if Control Key is pressed
                        bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;

                        //Specifies if Shift Key is pressed
                        shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;

                        //Specifies no modifiers is pressed
                        bool noModifiers = !menuKey && !controlKey && !shiftKey;
#else
                    if (e.Key == Key.Unknown)
                    {
                        pressedText = e.PlatformKeyCode.ToString();
                    }
#endif


                            int initialSelectionStart = SelectionStart;
                            MaskedText = this.Text;
                            selectionStart = SelectionStart;
                            selectionLength = SelectionLength;
                            selectedText = SelectedText;
                            if (!IsReadOnly)
                            {
#if WINDOWS_PHONE_7
                                if (e.Key != Key.Back)
#endif
                                    base.OnKeyDown(e);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                if (!BlockCharactersOnTextInput && !specialkeys.Contains(e.Key) && (!mapSeparator.separatorList.ContainsKey(pressedText)
                                    || (!mapSeparator.separatorList[pressedText].Equals(numberDecimalSeparator) && !mapSeparator.separatorList[pressedText].Equals(negativeSign)))
                                    && (!mapSeparator.shiftseparatorList.ContainsKey(pressedText)
                                    || (!mapSeparator.shiftseparatorList[pressedText].Equals(numberDecimalSeparator) && !mapSeparator.shiftseparatorList[pressedText].Equals(negativeSign))))
                                            e.Handled = true;
#endif
                                if (shiftKey && mapSeparator.shiftseparatorList.ContainsKey(pressedText))
                                {
                                    if (mapSeparator.shiftseparatorList[pressedText].Equals(numberDecimalSeparator))
                                    {
                                        enteredtext = numberDecimalSeparator;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                        if (BlockCharactersOnTextInput || (!BlockCharactersOnTextInput && MaskedText.Contains(numberDecimalSeparator)))
#endif
                                        {
                                            MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign,
                                                          numberGroupSeparator); 
                                            e.Handled = true;
                                        }
                                    }
                                    else if (mapSeparator.shiftseparatorList[pressedText].Equals(negativeSign))
                                    {
                                        enteredtext = negativeSign;
                                        MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign, numberGroupSeparator);
                                    }
                                    else
                                        e.Handled = true;
                                }
#if (WINDOWS_PHONE||WINDOWS_PHONE_7)
                                else if (!shiftKey && mapSeparator.separatorList.ContainsKey(pressedText))
#else
                                else if (!shiftKey && mapSeparator.separatorList.ContainsKey(pressedText) &&
                                    (BlockCharactersOnTextInput|| (!BlockCharactersOnTextInput && (e.Key!=VirtualKey.Number1&&e.Key!=VirtualKey.Number5&&e.Key!=VirtualKey.Number8))))
#endif
                                {
                                    if (mapSeparator.separatorList[pressedText].Equals(numberDecimalSeparator))
                                    {
                                        enteredtext = numberDecimalSeparator;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                        if (BlockCharactersOnTextInput || (!BlockCharactersOnTextInput && MaskedText.Contains(numberDecimalSeparator)))
#endif
                                        {
                                            MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign,
                                                          numberGroupSeparator);
                                            e.Handled = true;
                                        }
                                    }
                                    else if (mapSeparator.separatorList[pressedText].Equals(negativeSign))
                                    {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                        if (BlockCharactersOnTextInput)
#endif
                                        {
                                            enteredtext = negativeSign;
                                            MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign,
                                                          numberGroupSeparator);
                                            e.Handled = true;
                                        }
                                    }
									else
                                        e.Handled = true;
                                }
#if WINRT
                                if (!BlockCharactersOnTextInput && e.Key == VirtualKey.Decimal)
                                {
                                    enteredtext = numberDecimalSeparator;
                                    MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign, numberGroupSeparator);
                                    e.Handled = true;
                                }
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    if (((e.Key == Key.Subtract) || (e.Key == Key.Decimal) || (e.Key >= Key.D0) && (e.Key <= Key.D9) || (e.Key >= Key.NumPad0) && (e.Key <= Key.NumPad9)))
                    {
                        if (e.Key == Key.Subtract)
                            enteredtext = negativeSign;
                        else if (e.Key == Key.Decimal)
#else
                                if (BlockCharactersOnTextInput && noModifiers && ((e.Key == VirtualKey.Subtract) || (e.Key == VirtualKey.Decimal) || (e.Key >= VirtualKey.Number0) && (e.Key <= VirtualKey.Number9) || (e.Key >= VirtualKey.NumberPad0) && (e.Key <= VirtualKey.NumberPad9)))
                                {
                                    if (e.Key == VirtualKey.Subtract)
                                        enteredtext = negativeSign;
                                    else if (e.Key == VirtualKey.Decimal)
#endif
                                        enteredtext = numberDecimalSeparator;
                                    else
                                    {
                                        if (pressedText.Length > 0)
                                        {
                                            enteredtext = pressedText[pressedText.Length - 1].ToString();
                                            EnteredValue = enteredtext;
                                        }
                                    }
                                    MatchWithMask(enteredtext, numberDecimalSeparator, negativeSign, numberGroupSeparator);

                                    e.Handled = true;
                                }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                else if (controlKey)
                                {
                                    if (e.Key == VirtualKey.C || e.Key == VirtualKey.V || e.Key == VirtualKey.X)
                                    {
                                        int index = MaskedText.IndexOf(numberDecimalSeparator);
                                        selectionStart = SelectionStart;
                                        MaskedText = RemoveSeparator(Text);
                                        if (index > 0 && ((index - MaskedText.Length) - 1 > MaximumNumberDecimalDigits) && selectedText == String.Empty)
                                        {
                                            MaskedText = IntermediateValue.ToString();
                                        }
                                        int len = Text.Length;
                                        DataPackageView content = Clipboard.GetContent() as DataPackageView;
                                        String clipboardText = "";
                                        if (content.AvailableFormats.Count > 0 && content.AvailableFormats.Contains(StandardDataFormats.Text))
                                        {
                                            if (content.GetTextAsync() != null)
                                            {
                                                clipboardText = await content.GetTextAsync();
                                            }
                                        }

                                        String parsedText = RemoveCharacter(clipboardText);
                                        if (e.Key == VirtualKey.V)
                                        {
                                            if (content.AvailableFormats.Count > 0 && content.AvailableFormats.Contains(StandardDataFormats.Text))
                                            {
                                                if (ParsingMode == Parsers.Decimal)
                                                    Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                                else
                                                    Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                                                SelectionStart = initialSelectionStart;
                                                selectedText = await Paste(this, content);
                                            }
                                        }
                                        else
                                        {
                                            SetInternalFormatString(numberDecimalSeparator);
                                        }
                                        if (MaskedText != string.Empty)
                                        {
                                            if (ParsingMode == Parsers.Decimal)
                                            {
                                                if (!MaskedText.Contains(Culture.NumberFormat.NumberDecimalSeparator) && MaskedText.Length > decimal.MaxValue.ToString().Length)
                                                {
                                                    MaskedText = MaskedText.Substring(0, decimal.MaxValue.ToString().Length);
                                                }
                                                IntermediateValue = decimal.Parse(MaskedText);
                                                Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                            }
                                            else
                                            {
                                                IntermediateValue = double.Parse(MaskedText);
                                                Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                                            }
                                        }
                                        else
                                        {
                                            if (AllowNull)
                                            {
                                                IntermediateValue = null;
                                                Text = String.Empty;
                                            }
                                            else
                                            {
                                                IntermediateValue = 0;
                                                if (this.FocusState != FocusState.Unfocused)
                                                {
                                                    if (ParsingMode == Parsers.Decimal)
                                                        Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                                                    else
                                                        Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                                                }
                                                else
                                                {
                                                    if (ParsingMode == Parsers.Decimal)
                                                        Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                                                    else
                                                        Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                                                }
                                            }
                                        }
                                        if (selectedText == string.Empty)
                                            selectionStart = selectionStart + Math.Abs(Text.Length - len);
                                        SelectionStart = selectionStart;
                                        e.Handled = false;
                                    }
                                    else if (e.Key == VirtualKey.Z || e.Key == VirtualKey.Y)
                                    {
                                        if (ParsingMode == Parsers.Decimal)
                                            this.IntermediateValue = OldValue;
                                        else
                                            this.IntermediateValue = OldDoubleValue;
                                        SelectionStart = selectionStart;
                                        e.Handled = false;
                                    }
                                    else
                                    {
                                        e.Handled = true;
                                    }
                                }
                                else if (shiftKey)
                                {
                                    if (e.Key == VirtualKey.Tab)
                                        e.Handled = false;
                                    else if (e.Key == VirtualKey.Back)
                                    {
                                        HandleBackspaceKey(numberDecimalSeparator, numberGroupSeparator);
                                    }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                    else if (BlockCharactersOnTextInput)
#else
                                        else
#endif
                                        e.Handled = true;
                                }
                                else if (menuKey)
                                {
                                    if (e.Key == VirtualKey.F4)
                                        e.Handled = false;
                                }
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key == Key.Tab)
#else
                                else if (e.Key == VirtualKey.Tab)
#endif
                                {
                                    e.Handled = false;
                                }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key == Key.Back)
#else
                                else if (e.Key == VirtualKey.Back)
#endif
                                {
                                    HandleBackspaceKey(numberDecimalSeparator, numberGroupSeparator);
                                }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key == Key.Delete)
#else
                                else if (e.Key == VirtualKey.Delete)
#endif
                                {
                                    HandleDeleteKey(numberDecimalSeparator, numberGroupSeparator);
                                }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                    else if (e.Key != Key.PageDown && e.Key != Key.PageUp && e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Home && e.Key != Key.End && e.Key != Key.Escape && e.Key != Key.Enter && !(e.Key >= Key.F1 && e.Key <= Key.F12))
#else
                                else if (BlockCharactersOnTextInput && e.Key != VirtualKey.PageDown && e.Key != VirtualKey.PageUp && e.Key != VirtualKey.Up && e.Key != VirtualKey.Down && e.Key != VirtualKey.Left && e.Key != VirtualKey.Right && e.Key != VirtualKey.Home && e.Key != VirtualKey.End && e.Key != VirtualKey.Enter && e.Key != VirtualKey.Escape && !(e.Key >= VirtualKey.F1 && e.Key <= VirtualKey.F12))
#endif
                                    e.Handled = true;

                            }
                            else
                            {
                                e.Handled = false;
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
            if (!IsReadOnly && Culture != null)
            {
                decimal tempValue = internalValue;
                double tempdoubleValue = internaldoubleValue;
                if(CheckPercentDisplayMode())
                {
                    if (ParsingMode == Parsers.Decimal)
                        tempValue = Convert.ToDecimal((Convert.ToDouble(internalValue.ToString("N", Culture.NumberFormat)) / 100));
                    else
                        tempdoubleValue = Convert.ToDouble(internaldoubleValue.ToString("N", Culture.NumberFormat))/100 ;
                }
                if (Text != String.Empty)
                {
                    MaskedText = Text;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (!BlockCharactersOnTextInput && MaskedText.Contains(Culture.NumberFormat.NegativeSign))
                    {
                        int count = 0;
                        for (int i = 0; i < MaskedText.Length; i++)
                        {
                            if (MaskedText[i].ToString().Equals(Culture.NumberFormat.NegativeSign))
                                count++;
                        }
                        isNegative = count % 2 == 0 ? false : true;
                    }
#endif
                    MaskedText = RemoveSeparator(MaskedText);
                    if (MaskedText.StartsWith(Culture.NumberFormat.NumberDecimalSeparator) && MaskedText.Length == 1)
                    {
                        InternalFormatString = "N0";
                        if (AllowNull)
                        {
                            IntermediateValue = null;
                            Text = String.Empty;
                        }
                        else
                        {
                            if (ParsingMode == Parsers.Decimal)
                            {
                                IntermediateValue = 0;
                                this.Text = CheckPercentDisplayMode()? tempValue.ToString(FormatString,Culture.NumberFormat): internalValue.ToString(FormatString, Culture.NumberFormat);
                            }
                            else
                            {
                                IntermediateValue = 0;
                                this.Text = CheckPercentDisplayMode() ? tempdoubleValue.ToString(FormatString, Culture.NumberFormat) : internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                            }
                        }
                    }
                    else
                    {
                        if (AllowNull && ((String.IsNullOrEmpty(MaskedText)
                            || String.IsNullOrWhiteSpace(MaskedText) || (MaskedText == "0" && IntermediateValue.ToString()!="0"))) &&
                            (IntermediateValue != null && IntermediateValue.ToString() == "0" && MaskedText.Length <= 1))
                        {
                            IntermediateValue = null;
                            Text = String.Empty;
                        }
                        else
                        {
                            if (ParsingMode == Parsers.Decimal)
                            {
                                if (MaskedText != string.Empty)
                                {
                                    if (MaskedText.Length > decimal.MaxValue.ToString().Length)
                                        MaskedText = MaskedText.Remove(decimal.MaxValue.ToString().Length, MaskedText.Length - decimal.MaxValue.ToString().Length);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                    if (isNegative)
                                        IntermediateValue = decimal.Parse(MaskedText)*-1;
                                    else
#endif
                                        IntermediateValue = decimal.Parse(MaskedText);
                                    this.Text = CheckPercentDisplayMode() ? tempValue.ToString(FormatString, Culture.NumberFormat) : internalValue.ToString(FormatString, Culture.NumberFormat);
                                }
                            }
                            else
                            {
                                if (MaskedText != string.Empty)
                                {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                                    if (isNegative)
                                        IntermediateValue = double.Parse(MaskedText)*-1;
                                    else
#endif
                                        IntermediateValue = double.Parse(MaskedText);
                                    this.Text = CheckPercentDisplayMode() ? tempdoubleValue.ToString(FormatString, Culture.NumberFormat) : internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AllowNull)
                    {
                        IntermediateValue = null;
                        this.Text = String.Empty;
                    }
                    else
                    {
                        if (ParsingMode == Parsers.Decimal)
                        {
                            IntermediateValue = 0;
                            this.Text = CheckPercentDisplayMode() ? tempValue.ToString(FormatString, Culture.NumberFormat) : internalValue.ToString(FormatString, Culture.NumberFormat);
                        }
                        else
                        {
                            IntermediateValue = 0;
                            this.Text = CheckPercentDisplayMode() ? tempdoubleValue.ToString(FormatString, Culture.NumberFormat) : internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                        }
                    }
                }
            }

            if(this.ValueChangedMode==ValueChange.OnLostFocus)
              Value = IntermediateValue;

            this.isFocused = false;
        }

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
            {
            base.OnGotFocus(e);
            this.isFocused = true;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if (!IsReadOnly && FocusManager.GetFocusedElement() == this)
#else
            if (FocusState == FocusState.Keyboard || FocusState == FocusState.Pointer ||FocusState==FocusState.Programmatic)
#endif
            {
                if (ParsingMode == Parsers.Decimal)
                {
                    if (Culture != null && IntermediateValue != null)
                    {
                        decimal.TryParse(IntermediateValue.ToString(), out internalValue);
                        MaskedText = internalValue.ToString(Culture.NumberFormat);
                        var isSelectedAll = this.SelectedText == this.Text;
                        SetInternalFormatString(Culture.NumberFormat.NumberDecimalSeparator);
                        this.Text = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        if (isSelectedAll)
                            this.SelectAll();
                        else
                            this.SelectionStart = Text.Length;
                    }
                    else
                    {
                        if (AllowNull)
                        {
                            Text = String.Empty;
                        }
                        else
                        {
                            this.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                        }
                        internalValue = 0;
                    }
                }
                else
                {
                    if (Culture != null && IntermediateValue != null)
                    {
                        MaskedText = internaldoubleValue.ToString(Culture.NumberFormat);
                        SetInternalFormatString(Culture.NumberFormat.NumberDecimalSeparator);
                        if (MaskedText.Contains(Culture.NumberFormat.NumberDecimalSeparator) && !MaskedText.EndsWith(Culture.NumberFormat.NumberDecimalSeparator))
                        {
                            if (MaskedText.Contains("E"))
                                InternalFormatString = "N" + GetDecimalLength(Text).ToString();
                        }
                        var isSelectedAll = this.SelectedText == this.Text;
                        this.Text = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                        if (isSelectedAll)
                            this.SelectAll();
                        else
                            this.SelectionStart = Text.Length;
                    }
                    else
                    {
                        if (AllowNull)
                        {
                            Text = String.Empty;
                        }
                        else
                        {
                            this.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                        }
                        internaldoubleValue = 0;
                    }
                }
            }
        }

        #endregion       

        #region Callback Methods

        /// <summary>
        /// Occurs when the Intermediate value has changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIntermediateValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericTextBox)obj != null)
            {
                SfNumericTextBox textBox = (SfNumericTextBox) obj;
                textBox.OnValueChanged(args);

                if (textBox.ValueChangedMode == ValueChange.OnKeyFocus && textBox.isFocused)
                {
                    textBox.isInternalChange = true;
                    textBox.SetValue(ValueProperty, args.NewValue);
                    textBox.isInternalChange = false;
                }

                if (!textBox.isFocused)
                {
                    if (textBox.ValueChanged != null)
                    {
                        ValueChangedEventArgs valueargs = new ValueChangedEventArgs()
                        {
                            NewValue = args.NewValue,
                            OldValue = args.OldValue
                        };

                        textBox.ValueChanged(textBox, valueargs);
                    }
                }
            }
        }

        private bool isInternalChange = false;

        /// <summary>
        /// Occurs when the Value has changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericTextBox)obj != null)
            {
                SfNumericTextBox box = obj as SfNumericTextBox;

                if (box.isFocused)
                {
                    if (box.ValueChanged != null)
                    {
                        ValueChangedEventArgs valueargs = new ValueChangedEventArgs()
                            {
                                NewValue = args.NewValue,
                                OldValue = args.OldValue
                            };

                        box.ValueChanged(box, valueargs);
                    }
                }
                else if(!box.isInternalChange)
                {
                    box.IntermediateValue = args.NewValue;
                    box.isInternalChange = true;
                    box.SetValue(ValueProperty, box.IntermediateValue);
                    box.isInternalChange = false;
                }
            }
        }

        /// <summary>
        /// Occurs when the Value has changed
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IntermediateValue == null)
            {
                if (ParsingMode == Parsers.Decimal)
                {
                    internalValue = 0;
                    if (AllowNull)
                    {
                        Text = String.Empty;
                    }
                    else
                    {
                        if (IntermediateValue == null)
                            IntermediateValue = 0;
                        this.Text = internalValue.ToString(FormatString, Culture.NumberFormat);
                    }
                    decimal old;
                    if (args.OldValue != null)
                    {
                        decimal.TryParse(args.OldValue.ToString(), out old);
                        OldValue = old;
                    }
                    else
                    {
                        OldValue = null;
                    }
                }
                else
                {
                    internaldoubleValue = 0;
                    if (AllowNull)
                    {
                        Text = String.Empty;
                    }
                    else
                    {
                        if (IntermediateValue == null)
                            IntermediateValue = 0;
                        this.Text = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                    }
                    double old;
                    if (args.OldValue != null)
                    {
                        double.TryParse(args.OldValue.ToString(), out old);
                        OldDoubleValue = old;
                    }
                    else
                    {
                        OldDoubleValue = null;
                    }
                }

            }
            else
            {
                bool isDouble;
                if (ParsingMode == Parsers.Decimal)
                {
                    isDouble = decimal.TryParse(IntermediateValue.ToString(), out internalValue);
                }
                else
                     isDouble = double.TryParse(IntermediateValue.ToString(), out internaldoubleValue);
                
                if (isDouble && Culture != null)
                {
                    String numberDecimalSeparator = Culture.NumberFormat.NumberDecimalSeparator;

                    if (ParsingMode == Parsers.Decimal)
                    {
                        decimal old;
                        if (args.OldValue != null)
                        {
                            decimal.TryParse(args.OldValue.ToString(), out old);
                            if (internalValue != old)
                                OldValue = old;
                        }
                        else
                        {
                            OldValue = null;
                        }
                    }
                    else
                    {
                        double old;
                        if (args.OldValue != null)
                        {
                            double.TryParse(args.OldValue.ToString(), out old);
                            if (internaldoubleValue != old)
                                OldDoubleValue = old;
                        }
                        else
                        {
                            OldDoubleValue = null;
                        }
                    }
                    if (this.ValueChanged != null)
                    {
                        //ValueChangedEventArgs valueargs = new ValueChangedEventArgs(){NewValue = args.NewValue,OldValue = args.OldValue};
                        //this.ValueChanged(this, valueargs);
                    }
                    if ((Text.Contains(numberDecimalSeparator) || MaskedText.Contains(numberDecimalSeparator)) && !MaskedText.EndsWith(numberDecimalSeparator) && InternalFormatString == "N0")
                    {
                        SetInternalFormatString(numberDecimalSeparator);
                    }
                    if(ParsingMode == Parsers.Decimal)
                        MaskedText = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                    else
                        MaskedText = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                    String originalText = String.Empty;

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (this.FocusState != FocusState.Unfocused)

                    {
#else
                    if(FocusManager.GetFocusedElement() == this)
                    {
                    #endif
                        if(ParsingMode == Parsers.Decimal)
                            originalText = internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        else
                            originalText = internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                    }
                    else
                    {
                        if(ParsingMode == Parsers.Decimal)
                            originalText = internalValue.ToString(FormatString, Culture.NumberFormat);
                        else
                            originalText = internaldoubleValue.ToString(FormatString, Culture.NumberFormat);                  
                    }
                    if (originalText.Length > MaxLength && MaxLength > 0)
                    {
                        if (originalText.Contains(numberDecimalSeparator))
                        {
                            originalText = originalText.Substring(0, MaxLength + (numberDecimalSeparator.Length));
                        }
                        else
                        {
                            originalText = originalText.Substring(0, MaxLength);
                        }

                        isDouble = double.TryParse(originalText, out internaldoubleValue);
                        isDouble = decimal.TryParse(originalText, out internalValue);
                        if (ParsingMode == Parsers.Decimal)
                            IntermediateValue = internalValue;
                        else
                            IntermediateValue = internaldoubleValue;
                    }
                    Text = originalText;
                   
                    MaskedText = this.Text;
                }
                else
                {
                    throw new InvalidCastException("Input was not in the correct format");
                }
            }
        }

        /// <summary>
        /// Occurs when the Maximum number of decimal digits has changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaximumNumberDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfNumericTextBox)obj != null)
            {
                ((SfNumericTextBox)obj).OnMaximumNumberDecimalDigitsChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the Maximum number of decimal digits has changed
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaximumNumberDecimalDigitsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (MaximumNumberDecimalDigits < 0)
            {
                throw new InvalidOperationException("MaximumNumberDecimalDigits should be greater than 0");
            }
        }

        private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfNumericTextBox instance = obj as SfNumericTextBox;
            instance.OnFormatStringChanged(args);
        }
        private static void OnAllowNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfNumericTextBox numericTextBox = obj as SfNumericTextBox;
            if (numericTextBox.IntermediateValue==null || numericTextBox.IntermediateValue.Equals(0))
            {
                if (numericTextBox.AllowNull)
                {
                    numericTextBox.Text = string.Empty;
                    numericTextBox.IntermediateValue = null;
                }
                else
                    numericTextBox.IntermediateValue = 0;
            }
        }

        private static void OnPercentDisplayModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfNumericTextBox numericTextBox = obj as SfNumericTextBox;
            if (args.NewValue != null && numericTextBox.IntermediateValue != null)
            {
                decimal.TryParse(numericTextBox.IntermediateValue.ToString(), out numericTextBox.internalValue);
                double.TryParse(numericTextBox.IntermediateValue.ToString(), out numericTextBox.internaldoubleValue);
                decimal tempValue = numericTextBox.internalValue;
                double tempdoubleValue = numericTextBox.internaldoubleValue;
                if (numericTextBox.CheckPercentDisplayMode())
                {
                    if (numericTextBox.ParsingMode == Parsers.Decimal)
                        tempValue = Convert.ToDecimal(Convert.ToDouble(tempValue)/100);
                    else
                        tempdoubleValue = tempdoubleValue /100;
                }
                if (!numericTextBox.isFocused)
                {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                if (FocusManager.GetFocusedElement() == numericTextBox)
#else
                    if (numericTextBox.FocusState != FocusState.Unfocused)
#endif
                    {
                        if (numericTextBox.Value != null)
                        {
                            if (numericTextBox.ParsingMode == Parsers.Decimal)
                                numericTextBox.Text = tempValue.ToString(numericTextBox.InternalFormatString, numericTextBox.Culture.NumberFormat);
                            else
                                numericTextBox.Text = tempdoubleValue.ToString(numericTextBox.InternalFormatString, numericTextBox.Culture.NumberFormat);
                        }
                        else if (numericTextBox.Value == null)
                            numericTextBox.Text = string.Empty;
                    }
                    else
                    {
                        if (numericTextBox.Value != null)
                        {
                            if (numericTextBox.ParsingMode == Parsers.Decimal)
                                numericTextBox.Text = tempValue.ToString(numericTextBox.FormatString, numericTextBox.Culture.NumberFormat);
                            else
                                numericTextBox.Text = tempdoubleValue.ToString(numericTextBox.FormatString, numericTextBox.Culture.NumberFormat);
                        }
                        else if (numericTextBox.Value == null)
                            numericTextBox.Text = string.Empty;
                    }
                }
            }
        }
        /// <summary>
        /// Occurs when the FormatString has changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Culture != null)
            {
                decimal tempValue = internalValue;
                double tempdoubleValue = internaldoubleValue;
                if (CheckPercentDisplayMode())
                {
                    if (ParsingMode == Parsers.Decimal)
                        tempValue = Convert.ToDecimal((Convert.ToDouble(internalValue.ToString("N", Culture.NumberFormat)) / 100));
                    else
                        tempdoubleValue = Convert.ToDouble(internaldoubleValue.ToString("N", Culture.NumberFormat)) / 100;
                }
                if (!isFocused)
                {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                if (FocusManager.GetFocusedElement()==this)
#else
                    if (this.FocusState != FocusState.Unfocused)
#endif
                    {
                        if (ParsingMode == Parsers.Decimal)
                            Text = CheckPercentDisplayMode() ? tempValue.ToString(FormatString, Culture.NumberFormat) : internalValue.ToString(InternalFormatString, Culture.NumberFormat);
                        else
                            Text = CheckPercentDisplayMode() ? tempdoubleValue.ToString(FormatString, Culture.NumberFormat) : internaldoubleValue.ToString(InternalFormatString, Culture.NumberFormat);
                    }
                    else
                    {
                        if (IntermediateValue != null)
                        {
                            if (ParsingMode == Parsers.Decimal)
                                Text = CheckPercentDisplayMode() ? tempValue.ToString(FormatString, Culture.NumberFormat) : internalValue.ToString(FormatString, Culture.NumberFormat);
                            else
                                Text = CheckPercentDisplayMode() ? tempdoubleValue.ToString(FormatString, Culture.NumberFormat) : internaldoubleValue.ToString(FormatString, Culture.NumberFormat);
                        }
                        else
                            Text = string.Empty;
                    }
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when current <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.NumericTextBox.Value"/> is changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event ValueChangedEventHandler ValueChanged;

        #endregion

    }

    /// <summary>
    /// Represents a set of parsers
    /// </summary>
    public enum Parsers
    {
        /// <summary>
        /// Parses value into Double
        /// </summary>
        Double,
        /// <summary>
        /// Parses value into Decimal
        /// </summary>
        Decimal
    }

    /// <summary>
    /// Represents events for value changes
    /// </summary>
    public enum ValueChange
    {
        /// <summary>
        /// When focus is lost
        /// </summary>
        OnLostFocus,
        /// <summary>
        /// When focus is obtained through keys
        /// </summary>
        OnKeyFocus
    }

}
