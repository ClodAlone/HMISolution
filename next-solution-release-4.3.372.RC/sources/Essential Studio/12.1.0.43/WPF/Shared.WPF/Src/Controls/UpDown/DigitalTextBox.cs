// <copyright file="DigitalTextBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents textbox that operates only on numerical data.
    /// </summary>   
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DigitalTextBox : Control
    {
        #region Class constants

        /// <summary>
        ///  Regex template for numbers without zero.
        /// </summary>
        private const string DEF_REGEX_NUMBERS = @"[1-9]";

        /// <summary>
        /// The Zero value.
        /// </summary>
        private const string C_zero = "0";

        /// <summary>
        /// Regex template for validating input text.
        /// </summary>
        private const string DEF_REGEX_INPUT_CONDITION = @"^[\d]+$";

        /// <summary>
        /// Regex option for validating input text.
        /// </summary>
        protected const RegexOptions DEF_REGEX = RegexOptions.Compiled;

        /// <summary>
        /// Default number format.
        /// </summary>
        protected const string DEF_NUMBER_FORMAT = "N";

        /// <summary>
        /// Maximum allowed count of digits for double value.
        /// </summary>
        protected const int DEF_MAXIMUM_DIGIT = 15;
        #endregion

        #region Private Members

        /// <summary>
        /// Regex for validating input text.
        /// </summary>
        private static Regex m_input_validator
          = new Regex(DEF_REGEX_INPUT_CONDITION, DEF_REGEX);

        /// <summary>
        /// Text block used for text processing.
        /// </summary>
        private TextBlock m_textBlock;

        /// <summary>
        /// Caret index.
        /// </summary>
        private int m_index;

        /// <summary>
        /// X-position of the text block.
        /// </summary>
        private double m_translate;

        /// <summary>
        /// Text value.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Default number format.
        /// </summary>
        private NumberFormatInfo m_defformat;

        /// <summary>
        /// Cursor for the control.
        /// </summary>
        private UpDownCursor m_cursor;

        /// <summary>
        /// Cursor position.
        /// </summary>
        private double m_cursorTranslate;

        /// <summary>
        /// Indicates whether number is negative.
        /// </summary>
        private bool m_isMinus = false;

        /// <summary>
        /// Part of template, using for rendering text selection.
        /// </summary>
        private Border m_selection = null;

        /// <summary>
        /// Used for creating selection.
        /// </summary>
        private bool m_mouseDown = false;

        /// <summary>
        /// Cached value for index of selection start.
        /// </summary>
        private int m_selectionStart = 0;

        /// <summary>
        /// Cached value for length of selected text.
        /// </summary>
        private int m_selectionLength = 0;

        /// <summary>
        /// Used for selection using keyboard.
        /// </summary>
        private int m_count = 0;

        /// <summary>
        /// Used to store the UpDown insyance.
        /// </summary>
        private UpDown updowncontrol;

        /// <summary>
        /// Used to stroe the selection width.
        /// </summary>
        private double selectionwidth = 0;

        /// <summary>
        /// Used to stroe the start position 
        /// </summary>
        private Rect startrect = new Rect();

        /// <summary>
        /// Used to stroe the end position
        /// </summary>
        private Rect endrect = new Rect();

        /// <summary>
        /// Used to check if entered value is a number.
        /// </summary>
        private bool isnumber = false;

        /// <summary>
        /// Used to store the substring which appears after the current pointer position.
        /// </summary>
        private string convalue = string.Empty;

        /// <summary>
        /// Used to store the group separator index.
        /// </summary>
        private ObservableCollection<int> SeparatorIndices = new ObservableCollection<int>();
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selection border.
        /// </summary>
        internal Border Selection
        {
            get
            {
                return m_selection;
            }

            set
            {
                m_selection = value;
            }
        }

        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        /// <value>The control.</value>
        internal UpDown Control
        {
            get
            {
                return updowncontrol;
            }

            set
            {
                updowncontrol = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the selected.
        /// </summary>
        /// <value>The length of the selected.</value>
        public int SelectedLength
        {
            get
            {
                return (int)GetValue(SelectedLengthProperty);
            }

            set
            {
                SetValue(SelectedLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that represents index of start character of the current selection. This is dependency property.        
        /// </summary>
        /// <value>
        /// Type: <see cref="Int32"/>
        /// </value>
        public int SelectionStart
        {
            get
            {
                return m_selectionStart;
            }

            set
            {
                SetValue(SelectionStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected text.
        /// </summary>
        /// <value>The selected text.</value>
        public string SelectedText
        {
            get
            {
                return (string)GetValue(SelectedTextProperty);
            }

            protected internal set
            {
                SetValue(SelectedTextPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the text box cursor.
        /// </summary>
        /// <value>The text box cursor.</value>
        internal UpDownCursor TextBoxCursor
        {
            get
            {
                return m_cursor;
            }
        }

        /// <summary>
        /// Gets the X-position of the text block.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        internal double Translate
        {
            get
            {
                return m_translate;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Globalization.NumberFormatInfo"/> object that is used 
        /// for formatting the number value. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="NumberFormatInfo"/>
        /// </value>
        /// <seealso cref="NumberFormatInfo"/>
        public NumberFormatInfo NumberFormatInfo
        {
            get
            {
                return (NumberFormatInfo)GetValue(NumberFormatInfoProperty);
            }

            set
            {
                if (value != null)
                {
                    m_defformat.NumberDecimalDigits = value.NumberDecimalDigits;
                }
                SetValue(NumberFormatInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the double value of the text. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double? Value
        {
            get
            {
                return (double?)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cursor template. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate CursorTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(CursorTemplateProperty);
            }

            set
            {
                SetValue(CursorTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is double.MinValue.
        /// </value>
        /// <seealso cref="double"/>
        public double MinValue
        {
            get
            {
                return (double)GetValue(MinValueProperty);
            }

            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is double.MaxValue.
        /// </value>
        /// <seealso cref="double"/>
        public double MaxValue
        {
            get
            {
                return (double)GetValue(MaxValueProperty);
            }

            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        ///  Identifies the <see cref="CursorPosition"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CursorPositionProperty =
           DependencyProperty.Register("CursorPosition", typeof(Thickness), typeof(DigitalTextBox), new UIPropertyMetadata(new Thickness(50, 0, 0, 0)));

        /// <summary>
        /// Identifies the <see cref="SelectedText"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey SelectedTextPropertyKey =
                DependencyProperty.RegisterReadOnly("SelectedText", typeof(string), typeof(DigitalTextBox), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSelectedTextChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTextProperty = SelectedTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the <see cref="SelectedLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedLengthProperty =
               DependencyProperty.Register("SelectedLength", typeof(int), typeof(DigitalTextBox), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnSelectedLengthChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
                DependencyProperty.Register("SelectionStart", typeof(int), typeof(DigitalTextBox), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnSelectionStartChanged)));

        /// <summary>
        /// Identifies <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(DigitalTextBox), new UIPropertyMetadata(double.MaxValue));



        /// <summary>
        /// Identifies <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(DigitalTextBox), new UIPropertyMetadata(double.MinValue));

        /// <summary>
        /// Identifies <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(DigitalTextBox), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies <see cref="NumberFormatInfo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty =
            DependencyProperty.Register("NumberFormatInfo", typeof(NumberFormatInfo), typeof(DigitalTextBox), new FrameworkPropertyMetadata(new NumberFormatInfo(), new PropertyChangedCallback(OnNumberFormatInfoChanged)));

        /// <summary>
        /// Identifies <see cref="CursorTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorTemplateProperty =
            DependencyProperty.Register("CursorTemplate", typeof(ControlTemplate), typeof(DigitalTextBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCursorTemplateChanged)));
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="DigitalTextBox"/> class.
        /// </summary>
        static DigitalTextBox()
        {
            //// This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //// This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DigitalTextBox), new FrameworkPropertyMetadata(typeof(DigitalTextBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalTextBox"/> class.
        /// </summary>
        public DigitalTextBox()
        {
            Initialize();
            this.Loaded += new RoutedEventHandler(DigitalTextBox_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the DigitalTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DigitalTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            Rect contentrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Backward);
            if (this.Control != null)
            {
                this.Control.CursorPosition = new Thickness(contentrect.Location.X, 0, 0, 0);
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            m_defformat = new NumberFormatInfo();
            m_defformat.NumberGroupSeparator = string.Empty;

            m_textBlock = new TextBlock();
            if (Value != null)
            {
                m_textBlock.Text = Convert.ToDouble(Value).ToString("N", NumberFormatInfo);
            }
            else
            {
                m_textBlock.Text = "";
            }
            m_textBlock.FontSize = FontSize;
            m_textBlock.TextAlignment = TextAlignment.Right;

            Binding fontBinding = new Binding("FontSize");
            fontBinding.Source = this;
            m_textBlock.SetBinding(TextBlock.FontSizeProperty, fontBinding);

            Binding flowBinding = new Binding("FlowDirection");
            flowBinding.Source = this;
            m_textBlock.SetBinding(TextBlock.FlowDirectionProperty, flowBinding);

            Binding foregroundBinding = new Binding("Foreground");
            foregroundBinding.Source = this;
            m_textBlock.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);

            this.AddVisualChild(m_textBlock);
            this.AddLogicalChild(m_textBlock);

            m_cursor = new UpDownCursor();
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="SelectedText"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedTextChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectionStart"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectionStartChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="NumberFormatInfo"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback NumberFormatInfoChanged;

        /// <summary>
        /// Event that is raised when <see cref="CursorTemplate"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CursorTemplateChanged;
        #endregion

        #region Static Methods
        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Calls OnNumberFormatInfoChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnNumberFormatInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnNumberFormatInfoChanged(e);
        }

        /// <summary>
        /// Calls OnCursorTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCursorTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnCursorTemplateChanged(e);
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/>�routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            m_selection.Width = 0;
            SelectedLength = 0;
            SelectionStart = 0;
        }

        /// <summary>
        /// Selects all the text in the control.
        /// </summary>
        private void SelectAll()
        {
            SelectionStart = 1;
            SelectedLength = m_textBlock.Text.Length;
            SelectText(SelectionStart - 1, SelectedLength);
            Rect startrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
            Rect endrect = m_textBlock.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            m_selection.HorizontalAlignment = HorizontalAlignment.Left;
            m_selection.Visibility = Visibility.Visible;
            m_selection.Margin = new Thickness(startrect.X, 0, 0, 0);
            m_selection.Width = Math.Abs(endrect.X - startrect.X);
        }

        /// <summary>
        /// Invoked when MouseDoubleClick event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            SelectAll();
        }

        /// <summary>
        /// Sets the bounds of the selection according to SelectionStart and SelectionLength.
        /// </summary>
        private void SetSelectionRectangle()
        {
            TextPointer contentStart = m_textBlock.ContentStart;

            if (m_selectionLength > 0)
            {
                m_cursor.Visibility = Visibility.Collapsed;
                m_selection.Visibility = Visibility.Visible;

                int textLen = m_textBlock.Text.Length;

                if (m_selectionStart >= 0 && m_selectionStart <= textLen)
                {
                    int index = m_selectionStart + m_selectionLength;
                    if (index > textLen + 1)
                    {
                        index = textLen + 1;
                    }

                    if (index == m_selectionLength)
                    {
                        index++;
                    }

                    double start = GetXOffset(contentStart, m_selectionStart);
                    double end = GetXOffset(contentStart, index);
                    int offset = m_selectionStart;

                    ResizeSelection(contentStart, start, end, offset);
                }
            }
            else
            {
                m_count = 0;
                ////m_selection.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets position of the text symbol.
        /// </summary>
        /// <param name="contentStart">ContentStart of the text block</param>
        /// <param name="startX">position number in the text</param>
        /// <returns>position of the text symbol</returns>
        private static double GetXOffset(TextPointer contentStart, int startX)
        {
            return contentStart.GetPositionAtOffset(startX).GetCharacterRect(LogicalDirection.Backward).X;
        }

        /// <summary>
        /// Resizes selection in the UpDown.
        /// </summary>
        /// <param name="contentStart">ContentStart of the text block</param>
        /// <param name="start">start of the selection</param>
        /// <param name="end">end of the selection</param>
        /// <param name="offset">selection offset</param>
        private void ResizeSelection(TextPointer contentStart, double start, double end, int offset)
        {
            m_selection.Width = Math.Abs(end - start);
            Point point = new Point(contentStart.GetPositionAtOffset(offset).GetCharacterRect(LogicalDirection.Forward).X, 0);
            m_selection.Arrange(new Rect(point, new Size(m_selection.Width, m_selection.ActualHeight)));
        }

        /// <summary>
        /// Invoked when PreviewMouseDown event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            m_mouseDown = true;
            m_selection.Visibility = Visibility.Collapsed;
            m_selection.Width = 0;
            SelectedText = string.Empty;
            SelectedLength = 0;
            Point curlocation = e.GetPosition(this);
            TextPointer p = m_textBlock.GetPositionFromPoint(curlocation, true);
            Rect r = p.GetCharacterRect(LogicalDirection.Backward);
            this.Control.CursorPosition = new Thickness(r.Location.X, 0, 0, 0);
            base.OnPreviewMouseDown(e);
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when PreviewMouseUp event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            m_mouseDown = false;
            e.Handled = true;
            base.OnPreviewMouseUp(e);
        }

        /// <summary>
        /// Invoked when PreviewMouseMove event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (m_mouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                Point location = e.GetPosition(this);
                if (m_textBlock.Text.Length != 0)
                {
                    int currentIndex = GetTextPointerPosition(location);
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    int startIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    if (startIndex == -1)
                    {
                        startIndex = 0;
                    }

                    int i = m_index;

                    TextPointer currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    endrect = currentpointer.GetCharacterRect(LogicalDirection.Forward);
                    TextPointer startpointer = m_textBlock.GetPositionFromPoint(location, true);
                    startrect = startpointer.GetCharacterRect(LogicalDirection.Forward);
                    m_selection.HorizontalAlignment = HorizontalAlignment.Left;
                    m_selection.Margin = new Thickness(Math.Min(startrect.Left, endrect.Left), 0, 0, 0);
                    m_selection.Width = Math.Abs(endrect.Location.X - startrect.Location.X);
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    m_selection.Visibility = Visibility.Visible;
                    int firstindex = GetTextPointerPosition(new Point(startrect.Left, 0));
                    int d = Math.Abs(currentIndex - startIndex);
                    SelectedLength = d;
                    SelectionStart = Math.Min(startIndex, currentIndex) + 1;
                    SelectText(Math.Min(startIndex, currentIndex), d);
                    if (this.Control != null)
                    {
                        if (SelectedLength > 0)
                        {
                            this.Control.CursorVisible = false;
                        }
                        else
                        {
                            this.Control.CursorVisible = true;
                        }
                    }
                }
            }

            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Gets position number of the TextPointer.
        /// </summary>
        /// <param name="location">location of the mouse pointer</param>
        /// <returns>position number of the TextPointer</returns>
        private int GetTextPointerPosition(Point location)
        {
            int index = m_index;
            int len = m_textBlock.Text.Length;

            if (len > 0)
            {
                if (m_textBlock.ContentStart != null)
                {
                    TextPointer p = m_textBlock.GetPositionFromPoint(location, true);
                    if (p != null)
                    {
                        index = m_textBlock.ContentStart.GetOffsetToPosition(p) - 1;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="CursorTemplateChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCursorTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                m_cursor.Template = (ControlTemplate)e.NewValue;
            }

            if (CursorTemplateChanged != null)
            {
                CursorTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// <see cref="NumberFormatInfoChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnNumberFormatInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_textBlock != null)
            {
                if (Value != null)
                {
                    m_textBlock.Text = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, NumberFormatInfo);
                }
                else
                {
                    m_textBlock.Text = "";
                }
            }

            if (NumberFormatInfoChanged != null)
            {
                NumberFormatInfoChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            double? newValue = 0;
            newValue = (double?)e.NewValue;
            if (newValue == null)
            {
                m_textBlock.Text = "";
            }
            if (newValue <= MaxValue && newValue >= MinValue)
            {
                if (newValue != null)
                {
                    m_textBlock.Text = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, NumberFormatInfo);
                }
                else
                {
                    m_textBlock.Text = "";
                }
            }
            else if (newValue > MaxValue)
            {
                Value = MaxValue;
                m_index = 1;
            }
            else if (newValue < MinValue)
            {
                Value = MinValue;
                m_index = 1;
            }

            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }

            double startposition = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward).Left;
            if (this.Control != null && this.FlowDirection != FlowDirection.RightToLeft)
            {
                if (this.Control.CursorPosition.Left < startposition)
                {
                    this.Control.CursorPosition = new Thickness(startposition, 0, 0, 0);
                }
            }

            if (moreselected)
            {
                double nextposition = 0;

                TextPointer currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                double width = CalculateCharacterWidth();
                if (movetostart)
                {
                    nextposition = startposition + width;
                }
                else
                {
                    nextposition = this.Control.CursorPosition.Left + width;
                }

                this.Control.CursorPosition = new Thickness(nextposition, 0, 0, 0);
                moreselected = false;
            }
        }

        /// <summary>
        /// Calculates the width of the character.
        /// </summary>
        /// <returns></returns>
        private double CalculateCharacterWidth()
        {
            string word = m_textBlock.Text;

            if (word.Length == 0)
            {
                return 0;
            }
            string s = "" + word[word.Length - 1];

            FormattedText formattedText = new FormattedText(s,
                                               CultureInfo.CurrentCulture,
                                                FlowDirection.LeftToRight,
                                                new Typeface(m_textBlock.FontFamily.ToString()), m_textBlock.FontSize, Brushes.Black);

            Size size = new Size(formattedText.Width, formattedText.Width);
            return size.Width;
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Overrides <see cref="System.Windows.Media.Visual.GetVisualChild"/>,
        /// and returns a child at the specified index from a collection
        /// of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested
        /// child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if
        /// the provided index is out of range, an exception is raised.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            switch (index)
            {
                case 0:
                    return m_textBlock;
                case 1:
                    return m_cursor;
                default:
                    throw new Exception("Index for Visual Child is wrong.");
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;

            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            m_count = 0;
            //e.Handled = true;
            base.OnLostKeyboardFocus(e);
        }

        /// <summary>
        /// Called to measure the control.
        /// </summary>
        /// <param name="constraint">Measurement constraints, a control
        /// cannot return a size larger than the constraint.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by constraint.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            m_textBlock.Measure(constraint);
            Size sizeText = m_textBlock.DesiredSize;
            //// m_cursor.Measure(constraint);

            base.MeasureOverride(constraint);

            return sizeText;
        }

        /// <summary>
        /// Called to arrange and size the content of the control.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used
        /// to arrange the content.</param>
        /// <returns>
        /// The control size.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {

            base.ArrangeOverride(arrangeBounds);
            return arrangeBounds;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            isnumber = false;
            if (Value != null)
            {
                m_text = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat);
            }
            else
            {
                m_text = "";
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                ProcessKeyboardModifiers(e);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (Key.A == e.Key)
                {
                    SelectAll();
                }
            }
            else
            {
                m_selection.Visibility = Visibility.Collapsed;
                m_selection.Width = 0;
                m_count = 0;
                if (Key.Home == e.Key)
                {
                    Rect startrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                    this.Control.CursorPosition = new Thickness(startrect.Location.X, 0, 0, 0);
                }
                else if (Key.End == e.Key)
                {
                    Rect endrect = m_textBlock.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
                    this.Control.CursorPosition = new Thickness(endrect.Location.X, 0, 0, 0);
                }
                else if (Key.Delete == e.Key)
                {
                    ProcessDeleteKey();
                    e.Handled = true;
                }
                else if (Key.Left == e.Key)
                {
                    if (m_index != 1)
                    {
                        m_index--;
                    }

                    SelectedLength = 0;
                    int currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    int n = currentIndex;
                    TextPointer p = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    TextPointer pback = p.GetNextInsertionPosition(LogicalDirection.Backward);
                    if (pback != null)
                    {
                        Rect r = pback.GetCharacterRect(LogicalDirection.Backward);
                        this.Control.CursorPosition = new Thickness(r.Location.X, 0, 0, 0);
                    }

                    e.Handled = true;
                }
                else if (Key.Right == e.Key)
                {
                    if (m_index <= m_text.Length)
                    {
                        m_index++;
                    }

                    SelectedLength = 0;
                    TextPointer p;

                    int currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    int n = currentIndex;
                    if (currentIndex == -1)
                    {
                        p = m_textBlock.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
                    }
                    else
                    {
                        p = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    }

                    TextPointer pfront = p.GetNextInsertionPosition(LogicalDirection.Forward);

                    if (pfront != null)
                    {
                        Rect r = pfront.GetCharacterRect(LogicalDirection.Forward);
                        this.Control.CursorPosition = new Thickness(r.Location.X, 0, 0, 0);
                    }

                    e.Handled = true;
                }
                else if (Key.Back == e.Key)
                {
                    ProcessBackKey();
                    e.Handled = true;
                }
                else if (IsNumberPressed(e))
                {
                    isnumber = true;
                    ////RemoveSelectedNumbers();
                    m_index = SelectionStart;
                }
                else if (Key.OemMinus == e.Key || Key.Subtract == e.Key)
                {
                    Rect startrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                    if (this.SelectedLength == m_textBlock.Text.Length)
                    {
                        this.Control.Isminuspressed = true;
                    }

                    if (this.Control.CursorPosition.Left == startrect.Location.X)
                    {
                        Value = -Value;
                    }
                }
                else if (Key.Tab == e.Key)
                {

                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }

            InvalidateMeasure();
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Gets the previous position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The character rect</returns>
        private Rect GetPreviousPosition(Point position)
        {
            TextPointer temp = m_textBlock.GetPositionFromPoint(position, true);
            TextPointer pback = temp.GetNextInsertionPosition(LogicalDirection.Backward);
            return pback.GetCharacterRect(LogicalDirection.Backward);
        }

        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The character rect</returns>
        private Rect GetNextPosition(Point position)
        {
            TextPointer temp = m_textBlock.GetPositionFromPoint(position, true);
            TextPointer pfront = temp.GetNextInsertionPosition(LogicalDirection.Forward);
            return pfront.GetCharacterRect(LogicalDirection.Forward);
        }

        /// <summary>
        /// Processing keys with shift.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> that
        /// contains the event data.</param>
        private void ProcessKeyboardModifiers(KeyEventArgs e)
        {
            int startIndex = m_index + 1;
            int endIndex = m_index + 1;
            int currentIndex = 0;
            int lastIndex = 0;
            int firstindex = 0;

            TextPointer currentpointer;
            switch (e.Key)
            {
                case Key.Home:

                    m_selection.HorizontalAlignment = HorizontalAlignment.Left;
                    currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    endrect = currentpointer.GetCharacterRect(LogicalDirection.Forward);
                    startrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                    m_selection.Margin = new Thickness(startrect.Left, 0, 0, 0);
                    m_selection.Width = endrect.Location.X - startrect.Location.X;
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    m_selection.Visibility = Visibility.Visible;
                    firstindex = GetTextPointerPosition(new Point(startrect.Left, 0));
                    int i = currentIndex - firstindex;
                    SelectedLength = i;
                    SelectionStart = 1;
                    SelectText(0, currentIndex);
                    e.Handled = true;
                    break;

                case Key.End:
                    endIndex = m_text.Length + 1;
                    m_count = m_textBlock.Text.Length - m_index;

                    currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    startrect = currentpointer.GetCharacterRect(LogicalDirection.Forward);
                    endrect = m_textBlock.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
                    m_selection.Width = Math.Abs(startrect.Location.X - endrect.Location.X);
                    selectionwidth = startrect.Location.X - endrect.Location.X;
                    m_selection.Visibility = Visibility.Visible;
                    lastIndex = GetTextPointerPosition(new Point(endrect.Left, 0));
                    m_selection.Margin = new Thickness(startrect.Left, 0, 0, 0);
                    SelectedLength = lastIndex - currentIndex;
                    SelectionStart = currentIndex;
                    SelectText(currentIndex, Math.Abs(SelectedLength));

                    e.Handled = true;
                    break;

                case Key.Left:

                    TextPointer pback = null;
                    currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    if (currentIndex == -1)
                    {
                        currentpointer = m_textBlock.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
                    }
                    else
                    {
                        currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    }

                    endrect = currentpointer.GetCharacterRect(LogicalDirection.Backward);
                    if (SelectedLength > 0)
                    {
                        double x;
                        if (this.FlowDirection == FlowDirection.LeftToRight)
                        {
                            if (startrect.Left >= this.Control.CursorPosition.Left)
                            {
                                x = this.Control.CursorPosition.Left + m_selection.Width;
                            }
                            else
                            {
                                x = this.Control.CursorPosition.Left - m_selection.Width;
                            }
                        }
                        else
                        {
                            if (startrect.Left <= this.Control.CursorPosition.Left)
                            {
                                x = this.Control.CursorPosition.Left - m_selection.Width;
                            }
                            else
                            {
                                x = this.Control.CursorPosition.Left + m_selection.Width;
                            }
                        }

                        TextPointer temp = m_textBlock.GetPositionFromPoint(new Point(x, 0), true);
                        pback = temp.GetNextInsertionPosition(LogicalDirection.Backward);
                    }
                    else
                    {
                        pback = currentpointer.GetNextInsertionPosition(LogicalDirection.Backward);
                    }

                    if (pback != null)
                    {
                        startrect = pback.GetCharacterRect(LogicalDirection.Backward);
                    }
                    else
                    {
                        startrect = m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Backward);
                    }

                    m_selection.HorizontalAlignment = HorizontalAlignment.Left;
                    m_selection.Width = Math.Abs(startrect.Location.X - endrect.Location.X);
                    m_selection.Visibility = Visibility.Visible;
                    firstindex = GetTextPointerPosition(new Point(startrect.Left, 0));
                    m_selection.Margin = new Thickness(Math.Min(startrect.Left, endrect.Left), 0, 0, 0);
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    if (firstindex == -1)
                    {
                        firstindex = 0;
                    }

                    SelectedLength = Math.Abs(firstindex - currentIndex);
                    SelectionStart = Math.Min(firstindex + 1, currentIndex + 1);
                    SelectText(currentIndex, Math.Abs(firstindex - currentIndex));

                    e.Handled = true;
                    break;

                case Key.Right:
                    m_count++;

                    startIndex = GetStartIndex();
                    endIndex = GetEndIndex();

                    if (endIndex > m_textBlock.Text.Length)
                    {
                        m_count--;
                    }

                    TextPointer pfront = null;
                    currentIndex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                    if (currentIndex == -1)
                    {
                        currentpointer = m_textBlock.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
                    }
                    else
                    {
                        currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    }

                    if (SelectedLength > 0)
                    {
                        double x;
                        if (this.FlowDirection == FlowDirection.LeftToRight)
                        {
                            if (endrect.Left > this.Control.CursorPosition.Left)
                            {
                                x = this.Control.CursorPosition.Left + m_selection.Width;
                            }
                            else
                            {
                                x = this.Control.CursorPosition.Left - m_selection.Width;
                            }
                        }
                        else
                        {
                            if (endrect.Left <= this.Control.CursorPosition.Left)
                            {
                                x = this.Control.CursorPosition.Left - m_selection.Width;
                            }
                            else
                            {
                                x = this.Control.CursorPosition.Left + m_selection.Width;
                            }
                        }

                        TextPointer temp = m_textBlock.GetPositionFromPoint(new Point(x, 0), true);
                        if (temp.GetCharacterRect(LogicalDirection.Forward).Left == m_textBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward).Left)
                        {
                            pfront = m_textBlock.ContentStart.GetPositionAtOffset(1, LogicalDirection.Forward);
                            pfront = pfront.GetPositionAtOffset(1, LogicalDirection.Forward);
                            //// r = f.GetCharacterRect(LogicalDirection.Forward);
                        }
                        else
                        {
                            pfront = temp.GetNextInsertionPosition(LogicalDirection.Forward);
                        }
                    }
                    else
                    {
                        pfront = currentpointer.GetNextInsertionPosition(LogicalDirection.Forward);
                    }

                    if (pfront != null)
                    {
                        endrect = pfront.GetCharacterRect(LogicalDirection.Forward);
                    }

                    m_selection.HorizontalAlignment = HorizontalAlignment.Left;
                    startrect = currentpointer.GetCharacterRect(LogicalDirection.Forward);
                    m_selection.Width = Math.Abs(startrect.Location.X - endrect.Location.X);
                    m_selection.Visibility = Visibility.Visible;
                    lastIndex = GetTextPointerPosition(new Point(endrect.Left, 0));
                    m_selection.Margin = new Thickness(Math.Min(startrect.Left, endrect.Left), 0, 0, 0);
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    SelectedLength = Math.Abs(lastIndex - currentIndex);
                    SelectionStart = Math.Min(lastIndex + 1, currentIndex + 1);
                    SelectText(currentIndex, Math.Abs(lastIndex - currentIndex));

                    e.Handled = true;
                    break;

                case Key.Delete:
                    m_count = 0;
                    break;

                case Key.Tab:
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Gets end index of the selection.
        /// </summary>
        /// <returns>end index of the selection</returns>
        private int GetEndIndex()
        {
            return m_index > m_index + m_count ? m_index + 1 : m_index + m_count + 1;
        }

        /// <summary>
        /// Gets start index of the selection.
        /// </summary>
        /// <returns>start index of the selection</returns>
        private int GetStartIndex()
        {
            return m_index < m_index + m_count ? m_index + 1 : m_index + m_count + 1;
        }

        /// <summary>
        /// Returns true if number key was pressed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs" /> that
        /// contains the event data.</param>
        /// <returns>true if number key was pressed</returns>
        private static bool IsNumberPressed(KeyEventArgs e)
        {
            return (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
        }

        /// <summary>
        /// Processing Backspace key.
        /// </summary>
        private void ProcessBackKey()
        {
            TextPointer currentpointer;
            if (SelectedLength > 0)
            {
                RemoveSelectedNumbers();
            }
            else
            {
                int currentindex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                int count = m_text.Length;
                if (currentindex == -1 || currentindex == 0)
                {
                    currentindex = 1;
                }
                string valuetext = "";
                if (Value != null)
                {
                    valuetext = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat);
                }

                int cc = CalculateSeparators(valuetext);
                int separatorcount = 0;

                foreach (int index in SeparatorIndices)
                {
                    if (index <= currentindex)
                    {
                        separatorcount++;
                    }
                }
                int removeindex;
                removeindex = currentindex - separatorcount - 1;
                if (removeindex == m_textBlock.Text.Length - separatorcount)
                {
                    removeindex = currentindex - separatorcount - 2;
                }

                if (m_text.Substring(removeindex, 1) == NumberFormatInfo.NumberDecimalSeparator)
                {
                    removeindex--;
                }
                m_text = m_text.Remove(removeindex, 1);

                SetValueFromText();
                if (m_textBlock.FlowDirection == FlowDirection.LeftToRight)
                {
                    if (NumberFormatInfo.NumberDecimalDigits > 0)
                    {
                        if (currentindex > m_textBlock.Text.Length)
                        {
                            currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                            currentpointer = currentpointer.GetInsertionPosition(LogicalDirection.Backward);
                            if (currentpointer != null)
                            {
                                Rect charrect = currentpointer.GetCharacterRect(LogicalDirection.Backward);
                                this.Control.CursorPosition = new Thickness(charrect.Left - 6, 0, 0, 0);
                            }
                        }
                        else
                        {
                            if (currentindex - separatorcount - 1 > m_text.IndexOf(NumberFormatInfo.NumberDecimalSeparator))
                            {
                                currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                                currentpointer = currentpointer.GetNextInsertionPosition(LogicalDirection.Backward);
                                if (currentpointer != null)
                                {
                                    Rect charrect = currentpointer.GetCharacterRect(LogicalDirection.Backward);
                                    this.Control.CursorPosition = new Thickness(charrect.Left, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                else if (m_textBlock.FlowDirection == FlowDirection.RightToLeft)
                {
                    currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    currentpointer = currentpointer.GetNextInsertionPosition(LogicalDirection.Backward);
                    if (currentpointer != null)
                    {
                        Rect charrect = currentpointer.GetCharacterRect(LogicalDirection.Backward);
                        this.Control.CursorPosition = new Thickness(charrect.Left, 0, 0, 0);
                    }
                }
            }
            SelectedLength = 0;
        }

        /// <summary>
        /// Processing delete key.
        /// </summary>
        private void ProcessDeleteKey()
        {
            if (SelectedLength > 0)
            {
                RemoveSelectedNumbers();
            }
            else
            {
                string valuetext = "";
                if (Value != null)
                {
                    valuetext = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat);
                }
                int currentindex = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0));
                int cc = CalculateSeparators(valuetext);
                int separatorcount = 0;

                foreach (int index in SeparatorIndices)
                {
                    if (index < currentindex)
                    {
                        separatorcount++;
                    }
                }

                if (currentindex == m_textBlock.Text.Length)
                {
                    if (m_textBlock.FlowDirection == FlowDirection.LeftToRight)
                    {
                        currentindex = m_textBlock.Text.Length - 1;
                    }
                }

                if (currentindex <= -1)
                {
                    currentindex = 0;
                }

                int removeindex = currentindex - separatorcount;
                if (currentindex < m_textBlock.Text.Length)
                {
                    if (m_text.Substring(removeindex, 1) == NumberFormatInfo.NumberDecimalSeparator)
                    {
                        removeindex++;
                    }

                    m_text = m_text.Remove(removeindex, 1);
                }
                SetValueFromText();
                if (NumberFormatInfo.NumberDecimalDigits == 0)
                {
                    if (m_textBlock.FlowDirection == FlowDirection.RightToLeft)
                    {
                        if (currentindex < m_textBlock.Text.Length)
                        {
                            TextPointer currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                            currentpointer = currentpointer.GetNextInsertionPosition(LogicalDirection.Backward);
                            if (currentpointer != null)
                            {
                                Rect charrect = currentpointer.GetCharacterRect(LogicalDirection.Backward);
                                this.Control.CursorPosition = new Thickness(charrect.Left, 0, 0, 0);
                            }
                        }
                    }
                }
            }

            SelectedLength = 0;
            m_textBlock.InvalidateMeasure();
        }

        /// <summary>
        /// Replacing selected numbers to 0.
        /// </summary>
        private void RemoveSelectedNumbers()
        {
            string selectedText = SelectedText;
            ////Regex rx = new Regex( DEF_REGEX_NUMBERS );
            ////SelectedText = rx.Replace( SelectedText, new MatchEvaluator( ClearNumbers ) );
            if (selectedText != string.Empty)
            {
                m_text = m_textBlock.Text.Replace(selectedText, string.Empty);
            }

            SetValueFromText();
        }

        /// <summary>
        /// Clears selected numbers to 0.
        /// </summary>
        /// <param name="m">regex match (numbers from 1 to 9)</param>
        /// <returns>zero value</returns>
        private string ClearNumbers(Match m)
        {
            return C_zero;
        }

        /// <summary>
        /// Parses double value from text.
        /// </summary>
        private void SetValueFromText()
        {
            double val = 0;

            if (m_text == string.Empty)
            {
                m_index = 1;
                string selectedText = SelectedText;
                Regex rx = new Regex(DEF_REGEX_NUMBERS);
                SelectedText = rx.Replace(SelectedText, new MatchEvaluator(ClearNumbers));

                if (string.IsNullOrEmpty(SelectedText))
                {
                    m_text = "0";
                }
                else
                {
                    m_text = m_textBlock.Text.Replace(selectedText, SelectedText);
                }
            }

            if (double.TryParse(m_text, NumberStyles.Number, NumberFormatInfo, out val))
            {
                Value = val;
            }
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <param name="text">The text value.</param>
        /// <returns>The string containing only digits</returns>
        private string GetNumber(string text)
        {
            string retString = String.Empty;

            if (text != null)
            {
                int len = text.Length;

                for (int i = 0; i < len; i++)
                {
                    char c = text[i];
                    if (c.ToString() == NumberFormatInfo.NumberDecimalSeparator)
                    {
                        break;
                    }

                    if (Char.IsDigit(c))
                    {
                        retString += c;
                    }
                }
            }

            return retString;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.TextCompositionManager.TextInput"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.TextCompositionEventArgs"/> that contains the event data.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (m_input_validator.IsMatch(e.Text))
            {
                if (Value != null)
                {
                    m_text = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat);
                }
                else
                {
                    m_text = "";
                }
                if (m_index - 1 <= m_text.Length)
                {
                    if (m_index > (m_text.Length - m_defformat.NumberDecimalDigits) && m_index - 1 != m_text.Length)
                    {
                        m_text = m_text.Insert(m_index - 1, e.Text);
                        m_text = m_text.Remove(m_index, 1);
                        ////Value = double.Parse(m_text, m_defformat);
                        if (m_index <= m_text.Length)
                        {
                            m_index++;
                        }
                    }
                    else if (!(m_defformat.NumberDecimalDigits > 0 && m_index - 1 == m_text.Length))
                    {
                        int integralPart = (int)this.Value;
                        if (integralPart == 0 && m_index == 1)
                        {
                            m_index++;
                        }

                        if (m_index == 0)
                        {
                            m_text = m_text.Insert(1, e.Text);
                        }
                        else
                        {
                            m_text = m_text.Insert(m_index - 1, e.Text);
                        }

                        double resParse = Convert.ToDouble(Value);

                        if (double.TryParse(m_text, NumberStyles.Number, m_defformat, out resParse))
                        {
                            ////Value = resParse;
                        }

                        if (Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat).Length == m_text.Length)
                        {
                            m_index++;
                        }
                    }
                }

            }
            else if (e.Text == this.NumberFormatInfo.NumberDecimalSeparator)
            {
                m_index = m_text.IndexOf(this.NumberFormatInfo.NumberDecimalSeparator) + 2;
            }

            if (isnumber)
            {
                ProcessNewValue(e.Text);
            }

            this.InvalidateMeasure();
            base.OnTextInput(e);
        }

        /// <summary>
        /// Calculates the separators.
        /// </summary>
        /// <param name="valuetext">The valuetext.</param>
        /// <returns>The separators</returns>
        private int CalculateSeparators(string valuetext)
        {
            SeparatorIndices.Clear();
            int c = 0;
            long index;
            int count = valuetext.Length;
            string numberstring = GetNumber(valuetext);
            count = numberstring.Length;
            int[] a = NumberFormatInfo.NumberGroupSizes;
            int groupsize = a[0];
            c = (int)Math.DivRem((long)count, (long)groupsize, out index);
            if (count > groupsize)
            {
                int i = (int)index;

                if (index == 0)
                {
                    c--;
                    i = groupsize;
                }

                SeparatorIndices.Add(i + 1);
                for (int j = 0; j < c - 1; j++)
                {
                    i = i + (groupsize + 1);
                    SeparatorIndices.Add(i);
                }

                return c;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Represents moreselected value
        /// </summary>
        bool moreselected = false;
        /// <summary>
        /// Represents movetostart
        /// </summary>
        bool movetostart = false;
        /// <summary>
        /// Processes the new value.
        /// </summary>
        /// <param name="input">The input.</param>
        private void ProcessNewValue(string input)
        {
            SeparatorIndices.Clear();
            string final = string.Empty;
            string valuetext = "";
            if (Value != null)
            {
                valuetext = Convert.ToDouble(Value).ToString(DEF_NUMBER_FORMAT, m_defformat);
            }
            int cc = CalculateSeparators(valuetext);

            if (SelectedLength == 0)
            {
                SelectionStart = GetTextPointerPosition(new Point(this.Control.CursorPosition.Left, 0)) + 1;

                if (SelectionStart == 0)
                {
                    SelectionStart = 1;
                }
                int scount = 0;
                foreach (int index in SeparatorIndices)
                {
                    if (index < SelectionStart)
                    {
                        scount++;
                    }
                }

                int decimalindex = valuetext.IndexOf(NumberFormatInfo.NumberDecimalSeparator) + 1;
                if (SelectionStart > (decimalindex + scount))
                {
                    TextPointer currentpointer = m_textBlock.GetPositionFromPoint(new Point(this.Control.CursorPosition.Left, 0), true);
                    currentpointer = currentpointer.GetNextInsertionPosition(LogicalDirection.Forward);
                    if (currentpointer != null)
                    {
                        Rect charrect = currentpointer.GetCharacterRect(LogicalDirection.Forward);
                        this.Control.CursorPosition = new Thickness(charrect.Left, 0, 0, 0);
                    }
                }
            }

            int start = SelectionStart;
            int separatorcount = 0;

            foreach (int index in SeparatorIndices)
            {
                if (index < start)
                {
                    separatorcount++;
                }
            }

            if (SelectedLength > 0)
            {
                if (SelectedLength >= m_textBlock.Text.Length)
                {
                    movetostart = true;
                }

                moreselected = true;
                if (start > 0 && SelectedLength <= m_textBlock.Text.Length)
                {

                    string selected = m_textBlock.Text.Substring(start - 1, SelectedLength);
                    int selectedseparatorcount = 0;
                    foreach (char s in selected)
                    {
                        if (s.ToString() == NumberFormatInfo.NumberGroupSeparator)
                        {
                            selectedseparatorcount++;
                        }
                    }

                    if (selected.StartsWith(NumberFormatInfo.NumberDecimalSeparator) && SelectedLength != 1)
                    {
                        start++;
                        SelectedLength--;
                    }

                    if (selected.StartsWith(NumberFormatInfo.NumberGroupSeparator) && selected.EndsWith(NumberFormatInfo.NumberGroupSeparator) && valuetext.Contains(NumberFormatInfo.NegativeSign))
                    {
                        start++;
                    }

                    if (selected == NumberFormatInfo.NumberDecimalSeparator)
                    {
                        convalue = valuetext.Substring(start - 1);
                    }
                    else
                    {
                        convalue = valuetext.Substring(start - separatorcount + SelectedLength - selectedseparatorcount - 1);
                    }

                    if (start == 0)
                    {
                        start = 1;
                    }

                    string newval = valuetext.Remove(start - separatorcount - 1);
                    final = newval.Insert(newval.Length, input);
                    if (!selected.StartsWith(NumberFormatInfo.NumberDecimalSeparator) && selected.Contains(NumberFormatInfo.NumberDecimalSeparator))
                    {
                        final = final.Insert(final.Length, NumberFormatInfo.NumberDecimalSeparator);
                    }

                    final = final.Insert(final.Length, convalue);

                }
                else
                {
                    final = valuetext;
                }

            }
            else
            {
                if (start - separatorcount - 1 <= valuetext.Length)
                {
                    final = valuetext.Insert(start - separatorcount - 1, input);
                }
                else
                {
                    final = valuetext.Insert(start - separatorcount - 2, input);
                }
            }

            double v;
            Double.TryParse(final, out v);
            Value = v;


            SelectedLength = 0;
            SelectionStart = 0;

        }

        #endregion

        #region Internals

        /// <summary>
        /// Calls OnSelectionStartChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnSelectionStartChanged(e);
        }

        /// <summary>
        /// Raises <see cref="SelectionStartChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectionStartChanged(DependencyPropertyChangedEventArgs e)
        {
            m_selectionStart = (int)e.NewValue;

            if (SelectionStartChanged != null)
            {
                SelectionStartChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectionLengthChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        private static void OnSelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnSelectionLengthChanged(e);
        }

        /// <summary>
        /// Called when [selected length changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.SelectedLength = (int)e.NewValue;
            instance.SelectText(instance.SelectionStart, instance.SelectedLength);
        }

        /// <summary>
        /// Raises <see cref="SelectionLengthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectionLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            ////   m_selectionLength = (int)e.NewValue;
            //// SetSelectionRectangle();
            //// SelectText(m_selectionStart, m_selectionLength);

            ////if (SelectionLengthChanged != null)
            ////{
            ////    SelectionLengthChanged(this, e);
            ////}
        }

        /// <summary>
        /// Calls OnSelectedTextChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        private static void OnSelectedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitalTextBox instance = (DigitalTextBox)d;
            instance.OnSelectedTextChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedTextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedTextChanged != null)
            {
                SelectedTextChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the index of the caret according to the specified point.
        /// </summary>
        /// <param name="location">
        /// Specified point according which index is calculating.
        /// </param>
        /// <returns>
        /// <see cref="Int32"/> value that represents index.
        /// </returns>
        private int GetPosition(Point location)
        {
            int len = m_textBlock.Text.Length;
            int index = m_index;

            if (len > 0)
            {
                if (m_textBlock.ContentStart != null)
                {
                    TextPointer p = m_textBlock.GetPositionFromPoint(location, true);
                    if (p != null)
                    {
                        index = m_textBlock.ContentStart.GetOffsetToPosition(p) - 2;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Selects a range of text in the text box.
        /// </summary>
        /// <param name="startIndex">The zero-based character index of the first character in the selection.</param>
        /// <param name="count">The length of the selection, in characters.</param>
        private void SelectText(int startIndex, int count)
        {
            if (m_textBlock.Text != string.Empty && startIndex >= 0 && count >= 0 && startIndex + count <= m_textBlock.Text.Length)
            {
                SelectedText = m_textBlock.Text.Substring(startIndex, count);
            }

            if (count == 0)
            {
                SelectedText = string.Empty;
            }
        }

        /// <summary>
        /// Calculates the position of the cursor.
        /// </summary>
        /// <param name="arrangeBounds">Size of the control.</param>
        private void CalculateTranslate(Size arrangeBounds)
        {
            int index = 0;
            for (int i = 0; i <= m_index - 1; i++)
            {
                if (m_textBlock.Text.Length > index)
                {
                    if (m_textBlock.Text[index].ToString() == NumberFormatInfo.NumberGroupSeparator)
                    {
                        i--;
                    }
                }

                index++;
            }

            m_translate = arrangeBounds.Width - m_textBlock.ActualWidth;
            m_cursorTranslate = m_textBlock.ContentStart.GetPositionAtOffset(index).GetCharacterRect(LogicalDirection.Forward).X + m_translate;
            if (Math.Abs(m_cursorTranslate - arrangeBounds.Width) < 0.001)
            {
                m_cursorTranslate -= 2d;
            }

            if (m_cursorTranslate < 0)
            {
                m_translate -= m_cursorTranslate;
                m_cursorTranslate = m_textBlock.ContentStart.GetPositionAtOffset(index).GetCharacterRect(LogicalDirection.Forward).X + m_translate;
            }
        }

        /// <summary>
        /// Finds index of the caret.
        /// </summary>
        /// <param name="location">Location of the mouse.</param>
        private void FindPosition(Point location)
        {
            location.X = location.X;
            if (m_textBlock.Text.Length != 0)
            {
                m_index = 0;
                TextPointer textPointer = m_textBlock.ContentStart;
                textPointer.GetPointerContext(LogicalDirection.Forward);
                double positionStart;
                for (int i = 2, len = m_textBlock.Text.Length + 1; i <= len; i++)
                {
                    positionStart = textPointer.GetPositionAtOffset(i).GetCharacterRect(LogicalDirection.Forward).X + m_translate;
                    m_index++;

                    if (m_textBlock.Text[i - 2].ToString() == NumberFormatInfo.NumberGroupSeparator)
                    {
                        m_index--;
                    }

                    if (location.X < positionStart)
                    {
                        i = len;
                    }
                }
            }
            else
            {
                m_index = 0;
            }

            InvalidateMeasure();
        }
        #endregion
    }
}
