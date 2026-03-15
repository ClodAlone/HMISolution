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
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Data;

#if WPF

using Syncfusion.Licensing;

#endif

#if WPF

namespace Syncfusion.Windows.Shared
{
#endif
#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
{
#endif
#if WPF

    [SkinType(SkinVisualStyle = Skin.Blend,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
      Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
      Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
     Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/TimeSpanEdit/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT

    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Blend;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.VS2010;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue ,
      Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
    Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
   Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
 Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
 Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Default;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7 ,
Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Windows7;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Metro;component/TimeSpanEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
Type = typeof(TimeSpanEdit), XamlResource = "/Syncfusion.Theming.Transparent;component/TimeSpanEdit.xaml")]
#endif
    public class TimeSpanEdit : TextBox
    {
        /// <summary>
        ///
        /// </summary>
        public TimeSpanEdit()
        {
            DefaultStyleKey = typeof(TimeSpanEdit);
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(TimeSpanEdit));
            }
            this.Loaded += new RoutedEventHandler(TimeSpanEdit_Loaded);
            this.CommandBindings.Add(new CommandBinding(EditorCommands.Clear, ExecuteClearCommand, CanExecuteClearCommand));
#endif
#if SILVERLIGHT
            this.SelectionChanged -= new RoutedEventHandler(TimeSpanEdit_SelectionChanged);
            this.SelectionChanged += new RoutedEventHandler(TimeSpanEdit_SelectionChanged);
#endif
        }

#if WPF

        private void TimeSpanEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Format == string.Empty)
                this.Format = "d.h:m:s";
            if (EnableExtendedScrolling)
            {
                if (aLayer == null)
                    aLayer = AdornerLayer.GetAdornerLayer(this);

                if (aLayer != null)
                {
                    vAdorner = new ExtendedScrollingAdorner(this);
                    aLayer.Add(vAdorner);
                }
            }
            if (EnableTouch)
            {
                if (aLayer == null)
                    aLayer = AdornerLayer.GetAdornerLayer(this);

                if (aLayer != null)
                {
                    txtSelectionAdorner1 = new TextBoxSelectionAdorner(this);
                    aLayer.Add(txtSelectionAdorner1);
                }
            }
        }

        private void ExecuteClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Clear();
        }

        private void CanExecuteClearCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.Text.Length > 0;
        }

#endif

        #region Adorners

#if WPF
        private AdornerLayer aLayer;
        private TextBoxSelectionAdorner txtSelectionAdorner1;
        private ExtendedScrollingAdorner vAdorner;
#endif

        #endregion Adorners

        #region PrivateMembers

        private Dictionary<int, char> tPosition = new Dictionary<int, char>();

        private Dictionary<int, int> tLength = new Dictionary<int, int>();

        private Dictionary<int, int> tStart = new Dictionary<int, int>();

        private enum TimeSpanElements { Days, Hours, Minutes, Seconds, MilliSeconds };

        private TimeSpanElements SelectedSpan { get; set; }

        private bool isSelectionChanged = false;

        private bool selectionStartChanged = false;

        private int selectionStart = 0;

        private SpinCommand upCommand;

        private SpinCommand downCommand;

        private bool isDaysVisibile = false;

        private bool isHoursVisible = false;

        private bool isMinutesVisible = false;

        private bool isSecondsVisible = false;

        private bool isAppendDigit = false;

        private bool isSpinButtonPressed = false;

        private string keyCatch = string.Empty;

        #endregion PrivateMembers

        #region Wrappers

        /// <summary>
        ///
        /// </summary>
        public TimeSpan? Value
        {
            get { return (TimeSpan?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public TimeSpan MaxValue
        {
            get { return (TimeSpan)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public TimeSpan MinValue
        {
            get { return (TimeSpan)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string NullString
        {
            get { return (string)GetValue(NullStringProperty); }
            set { SetValue(NullStringProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowArrowButtons
        {
            get { return (bool)GetValue(ShowArrowButtonsProperty); }
            set { SetValue(ShowArrowButtonsProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IncrementOnScrolling
        {
            get { return (bool)GetValue(IncrementOnScrollingProperty); }
            set { SetValue(IncrementOnScrollingProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }

        #endregion Wrappers

        #region DependencyProperties

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan?), typeof(TimeSpanEdit), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0), new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(TimeSpan), typeof(TimeSpanEdit), new PropertyMetadata(TimeSpan.MinValue, new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(TimeSpan), typeof(TimeSpanEdit), new PropertyMetadata(TimeSpan.MaxValue, new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(TimeSpanEdit), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFormatStringChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for NullString.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NullStringProperty =
            DependencyProperty.Register("NullString", typeof(string), typeof(TimeSpanEdit), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnNullStringChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowArrowButtons.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowArrowButtonsProperty =
            DependencyProperty.Register("ShowArrowButtons", typeof(bool), typeof(TimeSpanEdit), new PropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IncrementOnScrolling.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IncrementOnScrollingProperty =
            DependencyProperty.Register("IncrementOnScrolling", typeof(bool), typeof(TimeSpanEdit), new PropertyMetadata(true));

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowNull.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowNullProperty =
            DependencyProperty.Register("AllowNull", typeof(bool), typeof(TimeSpanEdit), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowNullChanged)));

        #endregion DependencyProperties

        #region Events

        /// <summary>
        /// Event that is raised when Value property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        #endregion Events

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

#if WPF

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = true;
            base.OnContextMenuOpening(e);
        }

#endif
#if SILVERLIGHT
         private void TimeSpanEdit_ContextMenuOpening(object sender, RoutedEventArgs e)
         {
         }
#endif

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseWheel"/> event occurs to provide handling for the event in a derived class without attaching a delegate.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (IncrementOnScrolling)
            {
                if (e.Delta > 0)
                    IncreaseSpanValue();
                if (e.Delta < 0)
                    DecreaseSpanValue();
                if (tPosition.ContainsKey(this.SelectionStart))
                {
                    this.Select(tStart[this.SelectionStart], tLength[this.SelectionStart]);
                }
                else
                {
                    isSelectionChanged = false;
                    HandleLeft();
                }
            }
            base.OnMouseWheel(e);
        }

#if WPF

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
#endif
#if SILVERLIGHT
        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
#endif
            isSelectionChanged = false;
            keyCatch = string.Empty;
            base.OnMouseLeftButtonDown(e);
        }

#if WPF

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
#endif

#if SILVERLIGHT
        /// <summary>
        /// Called when <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#endif
            isSelectionChanged = false;
            if (this.Value == null)
            {
                e.Handled = true;
            }

            switch (e.Key)
            {
                case Key.Tab:
                    if (ModifierKeys.Shift == Keyboard.Modifiers)
                    {
                        if (this.SelectionStart + this.SelectionLength <= this.Text.Length && this.SelectionStart > 0)
                        {
                            HandleLeft();
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        if (this.SelectionStart + this.SelectionLength < this.Text.Length)
                        {
                            HandleRight();
                            e.Handled = true;
                        }
                    }
                    break;

                case Key.Up:
                    if (Value != null)
                    {
                        IncreaseSpanValue();
                        keyCatch = string.Empty;
                        e.Handled = true;
                    }
                    break;

                case Key.Down:
                    if (Value != null)
                    {
                        DecreaseSpanValue();
                        keyCatch = string.Empty;
                        e.Handled = true;
                    }
                    break;

                case Key.Left:
                    if (Value != null)
                    {
                        HandleLeft();
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    if (Value != null)
                    {
                        HandleRight();
                        e.Handled = true;
                    }
                    break;

                case Key.D0:
                    AppendDigit(0);
                    e.Handled = true;
                    break;

                case Key.D1:
                    AppendDigit(1);
                    e.Handled = true;
                    break;

                case Key.D2:
                    AppendDigit(2);
                    e.Handled = true;
                    break;

                case Key.D3:
                    AppendDigit(3);
                    e.Handled = true;
                    break;

                case Key.D4:
                    AppendDigit(4);
                    e.Handled = true;
                    break;

                case Key.D5:
                    AppendDigit(5);
                    e.Handled = true;
                    break;

                case Key.D6:
                    AppendDigit(6);
                    e.Handled = true;
                    break;

                case Key.D7:
                    AppendDigit(7);
                    e.Handled = true;
                    break;

                case Key.D8:
                    AppendDigit(8);
                    e.Handled = true;
                    break;

                case Key.D9:
                    AppendDigit(9);
                    e.Handled = true;
                    break;

                case Key.NumPad0:
                    AppendDigit(0);
                    e.Handled = true;
                    break;

                case Key.NumPad1:
                    AppendDigit(1);
                    e.Handled = true;
                    break;

                case Key.NumPad2:
                    AppendDigit(2);
                    e.Handled = true;
                    break;

                case Key.NumPad3:
                    AppendDigit(3);
                    e.Handled = true;
                    break;

                case Key.NumPad4:
                    AppendDigit(4);
                    e.Handled = true;
                    break;

                case Key.NumPad5:
                    AppendDigit(5);
                    e.Handled = true;
                    break;

                case Key.NumPad6:
                    AppendDigit(6);
                    e.Handled = true;
                    break;

                case Key.NumPad7:
                    AppendDigit(7);
                    e.Handled = true;
                    break;

                case Key.NumPad8:
                    AppendDigit(8);
                    e.Handled = true;
                    break;

                case Key.NumPad9:
                    AppendDigit(9);
                    e.Handled = true;
                    break;

                case Key.Delete:
                    if (Value != null)
                    {
                        keyCatch = string.Empty;
                        if (SelectionLength == this.Text.Length)
                        {
                            if (this.AllowNull)
                            {
                                this.Text = this.NullString;
                                this.Value = null;
                            }
                            else
                                this.Value = new TimeSpan(0, 0, 0, 0, 0);
                        }
                        else
                            AppendDigit(0);
                        e.Handled = true;
                    }
                    break;
#if WPF
                case Key.LeftShift:
                case Key.RightShift:
#endif

#if SILVERLIGHT
                case Key.Shift:
#endif
#if WPF
                case Key.End:
#endif
                   #if WPF
                case Key.A:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        base.SelectAll();
                    }
                    else
                    {
                        e.Handled = true;
                        keyCatch = string.Empty;
                    }
                    break;
                    #endif
                case Key.Enter:
                    break;  
                case Key.Home:
                case Key.PageDown:
                default:
                    e.Handled = true;
                    keyCatch = string.Empty;
                    break;
            }
            //base.OnKeyDown(e);
        }

        #endregion Overrides

        #region Implementation

        /// <summary>
        ///
        /// </summary>
        public SpinCommand UpCommand
        {
            get
            {
                if (upCommand == null)
                {
                    upCommand = new SpinCommand(param => UpExecute(), param => UpCanExecute());
                }
                return upCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public SpinCommand DownCommand
        {
            get
            {
                if (downCommand == null)
                {
                    downCommand = new SpinCommand(param => DownExecute(), param => DownCanExecute());
                }
                return downCommand;
            }
        }

        private void HandleRight()
        {
            int currentLength = this.SelectionStart;
            if (tPosition.ContainsKey(SelectionStart))
            {
                currentLength = this.SelectionStart + tLength[this.SelectionStart];
            }
            for (int i = currentLength; i < this.Text.Length; i++)
            {
                if (tPosition.ContainsKey(i + 1))
                {
                    this.selectionStartChanged = true;
                    selectionStart = i + 1;
                    this.SelectionStart = i + 1;
                    break;
                }
            }
            keyCatch = string.Empty;
        }

        private void HandleLeft()
        {
            for (int i = this.SelectionStart - 1; i >= 0; i--)
            {
                if (tPosition.ContainsKey(i))
                {
                    this.selectionStartChanged = true;
                    this.selectionStart = tStart[i];
                    this.Select(tStart[i], tLength[i]);
                    this.SelectionStart = tStart[i];
                    break;
                }
            }
            keyCatch = string.Empty;
        }

#if WPF

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
#endif
#if SILVERLIGHT
private void TimeSpanEdit_SelectionChanged(object sender, RoutedEventArgs e)
        {
#endif
            if (!isSelectionChanged && this.SelectionStart == 0 && this.SelectionLength == this.Text.Length)
            {
                isSelectionChanged = true;
                this.SelectAll();
                //#if WPF
                //e.Handled = true;
                //#endif
            }
            else
            {
                if (!isSelectionChanged && !isAppendDigit && !isSpinButtonPressed)
                {
                    if (selectionStartChanged)
                    {
                        if (tPosition.ContainsKey(selectionStart))
                        {
                            selectionStartChanged = false;
                            isSelectionChanged = true;
                            this.Select(tStart[selectionStart], tLength[selectionStart]);
                            this.SelectionStart = tStart[selectionStart];
                            //#if WPF
                            //e.Handled = true;
                            //#endif
                        }
                    }
                    if (tPosition.ContainsKey(this.SelectionStart))
                    {
                        switch (tPosition[this.SelectionStart])
                        {
                            case 'd':
                                SelectedSpan = TimeSpanElements.Days;
                                break;

                            case 'h':
                                SelectedSpan = TimeSpanElements.Hours;
                                break;

                            case 'm':
                                SelectedSpan = TimeSpanElements.Minutes;
                                break;

                            case 's':
                                SelectedSpan = TimeSpanElements.Seconds;
                                break;

                            case 'z':
                                SelectedSpan = TimeSpanElements.MilliSeconds;
                                break;
                        }
                        isSelectionChanged = true;
                        this.Select(tStart[this.SelectionStart], tLength[this.SelectionStart]);
                        this.SelectionStart = tStart[this.SelectionStart];
                    }
                    else
                    {
                        if (this.SelectionStart > 0)
                        {
                            this.SelectionStart = this.SelectionStart - 1;
                            OnSelectionChanged(e);
                        }
                        else
                        {
                            isSelectionChanged = true;
                            if (tPosition.Count > 0)
                                this.Select(this.SelectionStart, 1);
                            else
                                this.Select(this.SelectionStart, 0);
                        }
                    }
                }
            }
#if WPF
            //e.Handled = true;
            base.OnSelectionChanged(e);
#endif
        }

        private void AppendDigit(int val)
        {
            keyCatch += val.ToString();
            int value = 0;
            int.TryParse(keyCatch, out value);
            TimeSpan tspan = new TimeSpan();
            TimeSpan currentTSpan = new TimeSpan();
            isSelectionChanged = false;
            isAppendDigit = true;
            if (this.Value != null && keyCatch.Length < 9)
            {
                currentTSpan = this.Value.Value;
                switch (SelectedSpan)
                {
                    case TimeSpanElements.Days:
                        try
                        {
                            TimeSpan temp;
                            if (TimeSpan.TryParse(keyCatch, out temp))
                            {
                                tspan = new TimeSpan(value, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            else
                            {
                                tspan = new TimeSpan();
                                keyCatch = string.Empty;
                            }
                        }
                        catch
                        { }

                        break;

                    case TimeSpanElements.Hours:
                        if (isDaysVisibile)
                        {
                            if (value < 24)
                            {
                                tspan = new TimeSpan(currentTSpan.Days, value, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            else
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                                keyCatch = string.Empty;
                            }
                        }
                        else
                            tspan = new TimeSpan(0, value, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.Minutes:
                        if (isHoursVisible)
                        {
                            if (value < 60)
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, value, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            else
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                                keyCatch = string.Empty;
                            }
                        }
                        else
                            tspan = new TimeSpan(0, 0, value, currentTSpan.Seconds, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.Seconds:
                        if (isMinutesVisible)
                        {
                            if (value < 60)
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, value, currentTSpan.Milliseconds);
                            }
                            else
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                                keyCatch = string.Empty;
                            }
                        }
                        else
                            tspan = new TimeSpan(0, 0, 0, value, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.MilliSeconds:
                        if (isSecondsVisible)
                        {
                            if (value < 999)
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, value);
                            }
                            else
                            {
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                                keyCatch = string.Empty;
                            }
                        }
                        else
                            tspan = new TimeSpan(0, 0, 0, 0, value);
                        break;
                }
                if ((tspan.Days >= MinValue.Days && tspan.Days <= MaxValue.Days) || (tspan.Hours >= MinValue.Hours && tspan.Hours <= MaxValue.Hours) || (tspan.Minutes >= MinValue.Minutes && tspan.Minutes <= MaxValue.Minutes) || (tspan.Seconds >= MinValue.Seconds && tspan.Seconds <= MaxValue.Seconds) || (tspan.Milliseconds >= MinValue.Milliseconds && tspan.Milliseconds <= MaxValue.Milliseconds))
                    this.Value = tspan;
                else
                    keyCatch = string.Empty;
            }
            else
            {
                this.Value = this.MinValue;
                keyCatch = string.Empty;
            }
            isAppendDigit = false;
            if (tPosition.ContainsKey(this.SelectionStart))
            {
                this.Select(tStart[this.SelectionStart], tLength[this.SelectionStart]);
            }
        }

        private void DecreaseSpanValue()
        {
            try
            {
                bool flagset = false;
                TimeSpan tspan = new TimeSpan();
                TimeSpan currentTSpan = new TimeSpan();
                if (this.Value != null)
                {
                    currentTSpan = this.Value.Value;

                    switch (SelectedSpan)
                    {
                        case TimeSpanElements.Days:
                            if (currentTSpan.Days > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days - 1, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            break;

                        case TimeSpanElements.Hours:
                            if (currentTSpan.Hours > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours - 1, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            else if (currentTSpan.Hours == 0 && currentTSpan.Days > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days - 1, 23, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            break;

                        case TimeSpanElements.Minutes:
                            if (currentTSpan.Minutes > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes - 1, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            else if (currentTSpan.Minutes == 0 && currentTSpan.Hours > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours - 1, 59, currentTSpan.Seconds, currentTSpan.Milliseconds);
                            }
                            break;

                        case TimeSpanElements.Seconds:
                            if (currentTSpan.Seconds > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds - 1, currentTSpan.Milliseconds);
                            }
                            else if (currentTSpan.Seconds == 0 && currentTSpan.Minutes > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes - 1, 59, currentTSpan.Milliseconds);
                            }
                            break;

                        case TimeSpanElements.MilliSeconds:
                            if (currentTSpan.Milliseconds > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds - 1);
                            }
                            else if (currentTSpan.Milliseconds == 0 && currentTSpan.Seconds > 0)
                            {
                                flagset = true;
                                tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds - 1, 59);
                            }
                            break;
                    }
                    if (flagset == true)
                    {
                        if (tspan >= MinValue)
                            this.Value = tspan;
                    }
                    else
                        this.Value = currentTSpan;
                }
                flagset = false;
            }
            catch { }
        }

        private void IncreaseSpanValue()
        {
            TimeSpan tspan = new TimeSpan();
            TimeSpan currentTSpan = new TimeSpan();
            if (this.Value != null)
            {
                currentTSpan = this.Value.Value;
                switch (SelectedSpan)
                {
                    case TimeSpanElements.Days:
                        tspan = new TimeSpan(currentTSpan.Days + 1, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.Hours:
                        tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours + 1, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.Minutes:
                        tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes + 1, currentTSpan.Seconds, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.Seconds:
                        tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds + 1, currentTSpan.Milliseconds);
                        break;

                    case TimeSpanElements.MilliSeconds:
                        tspan = new TimeSpan(currentTSpan.Days, currentTSpan.Hours, currentTSpan.Minutes, currentTSpan.Seconds, currentTSpan.Milliseconds + 1);
                        break;
                }
                if (tspan <= MaxValue)
                    this.Value = tspan;
            }
        }

        internal void UpExecute()
        {
            isSpinButtonPressed = true;
            if (Value == null)
            {
                Value = MaxValue;
            }
            else
            {
                IncreaseSpanValue();
                selectionStartChanged = true;
                if (tPosition.ContainsKey(this.SelectionStart))
                {
                    this.Select(tStart[this.SelectionStart], tLength[this.SelectionStart]);
                }
                selectionStartChanged = false;
            }
            isSpinButtonPressed = false;
        }

        internal void DownExecute()
        {
            isSpinButtonPressed = true;
            if (Value == null)
            {
                Value = MinValue;
            }
            else
            {
                DecreaseSpanValue();
                selectionStartChanged = true;
                if (tPosition.ContainsKey(this.SelectionStart))
                {
                    this.Select(tStart[this.SelectionStart], tLength[this.SelectionStart]);
                }
                selectionStartChanged = false;
            }
            isSpinButtonPressed = false;
        }

        private bool UpCanExecute()
        {
            if (Value == null)
                return true;
            else
            {
                if (Value <= MaxValue)
                    return true;
                else
                    return false;
            }
        }

        private bool DownCanExecute()
        {
            if (Value == null)
                return true;
            else
            {
                if (Value >= MinValue)
                    return true;
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnNullStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnNullStringChanged(args);
        }

        private void OnNullStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Value == null)
                this.Text = NullString;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnMinValueChanged(args);
        }

        private void OnMaxValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (MinValue > MaxValue)
                throw new InvalidOperationException("MaxValue should not be lesser than MinValue");
            else if (Value > MaxValue)
                Value = MaxValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnMaxValueChanged(args);
        }

        private void OnMinValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (MaxValue < MinValue)
                throw new InvalidOperationException("MinValue should not be greater than MaxValue");
            else if (Value < MinValue)
                Value = MinValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnFormatStringChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnFormatStringChanged(args);
        }

        private void OnFormatStringChanged(DependencyPropertyChangedEventArgs args)
        {
            CreateDisplayText();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnAllowNullChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnAllowNullChanged(args);
        }

        private void OnAllowNullChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.Value == null)
                this.Text = NullString;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TimeSpanEdit instance = sender as TimeSpanEdit;
            instance.OnValueChanged(args);
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsReadOnly)
                return;
            if (this.Value > MaxValue)
            {
                this.Value = MaxValue;
            }
            else if (this.Value < MinValue)
            {
                this.Value = MinValue;
            }
            else
            {
                if (this.Value == null && !AllowNull)
                {
                    this.Value = new TimeSpan(0, 0, 0, 0);
                }
                else
                {
                    CreateDisplayText();
                    if (this.ValueChanged != null)
                    {
                        ValueChanged(this, args);
                    }
                }
            }
        }

        private void CreateDisplayText()
        {
            TimeSpan tspan = new TimeSpan();
            if (this.Value != null && this.Format != string.Empty)
            {
                tspan = this.Value.Value;
                string format = Format.ToString();
                string displayText = string.Empty;
                tPosition.Clear();
                tLength.Clear();
                tStart.Clear();

                if (Format != string.Empty)
                {
                    string[] literals = format.Split('\'');
                    bool isLiteral = false;
                    bool isStringRepeated = false;
                    for (int i = 0; i < Format.Length; i++)
                    {
                        char ch = format[i];
                        if (ch == '\'')
                        {
                            isLiteral = !isLiteral;
                            continue;
                        }

                        if (isLiteral && ch != '\'')
                        {
                            displayText += ch;
                            continue;
                        }
                        if (i + 1 < Format.Length)
                        {
                            if (format[i + 1] == format[i])
                            {
                                i++;
                                isStringRepeated = true;
                            }
                        }
                        if (ch == 'd')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Days.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'd');
                                    tLength.Add(displayText.Length + k, tspan.Days.ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isDaysVisibile = true;
                                }
                            }
                            catch { }
                            displayText += tspan.Days.ToString();
                            continue;
                        }
                        if (ch == 'h')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Hours.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'h');
                                    if (isDaysVisibile)
                                        tLength.Add(displayText.Length + k, tspan.Hours.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalHours).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isHoursVisible = true;
                                }
                            }
                            catch { }
                            if (isDaysVisibile)
                            {
                                if (isStringRepeated)
                                {
#if SyncfusionFramework4_0
                                    displayText += tspan.ToString("hh");
#endif
#if SyncfusionFramework3_5
                                    if (tspan.Hours.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Hours.ToString();
                                    else
                                        displayText += tspan.Hours.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Hours.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalHours).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 'm')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Minutes.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'm');
                                    if (isHoursVisible)
                                        tLength.Add(displayText.Length + k, tspan.Minutes.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalMinutes).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isMinutesVisible = true;
                                }
                            }
                            catch { }
                            if (isHoursVisible)
                            {
                                if (isStringRepeated)
                                {
#if SyncfusionFramework4_0
                                    displayText += tspan.ToString("mm");
#endif
#if SyncfusionFramework3_5
                                   if(tspan.Minutes.ToString().Length<2)
                                       displayText=displayText+0+tspan.Minutes.ToString();
                                   else
                                       displayText += tspan.Minutes.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Minutes.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalMinutes).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 's')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Seconds.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 's');
                                    if (isMinutesVisible)
                                        tLength.Add(displayText.Length + k, tspan.Seconds.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalSeconds).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isSecondsVisible = true;
                                }
                            }
                            catch { }
                            if (isMinutesVisible)
                            {
                                if (isStringRepeated)
                                {
#if SyncfusionFramework4_0
                                    displayText += tspan.ToString("ss");
#endif
#if SyncfusionFramework3_5
                                    if (tspan.Seconds.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Seconds.ToString();
                                    else
                                        displayText += tspan.Seconds.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Seconds.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalSeconds).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 'z')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Milliseconds.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'z');
                                    if (isSecondsVisible)
                                        tLength.Add(displayText.Length + k, tspan.Milliseconds.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalMilliseconds).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                }
                            }
                            catch { }
                            if (isSecondsVisible)
                            {
                                if (isStringRepeated)
                                {
#if SyncfusionFramework4_0
                                    displayText += tspan.ToString("fffffff");
#endif
#if SyncfusionFramework3_5
                                    if (tspan.Milliseconds.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Milliseconds.ToString();
                                    else
                                        displayText += tspan.Milliseconds.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Milliseconds.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalMilliseconds).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        displayText += ch;
                    }
                }

                int selectionStart = this.SelectionStart;
                this.selectionStart = this.SelectionStart;
                selectionStartChanged = true;
                this.Text = displayText;
                selectionStartChanged = false;
                this.SelectionStart = selectionStart;
            }
            else
            {
                this.Text = this.NullString;
            }
        }

        #endregion Implementation

#if WPF

        #region Touch

        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(TimeSpanEdit), new PropertyMetadata(false, OnEnableTouchChanged));

        public static void OnEnableTouchChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj as TimeSpanEdit != null)
                (obj as TimeSpanEdit).OnEnableTouchChanged(args);
        }

        private void OnEnableTouchChanged(DependencyPropertyChangedEventArgs args)
        {
            if (aLayer == null)
                aLayer = AdornerLayer.GetAdornerLayer(this);

            if ((bool)args.NewValue)
            {
                if (aLayer != null)
                {
                    txtSelectionAdorner1 = new TextBoxSelectionAdorner(this);
                    aLayer.Add(txtSelectionAdorner1);
                }
            }
            else
            {
                if (aLayer != null)
                {
                    aLayer.Remove(txtSelectionAdorner1);
                }
            }
        }

        #endregion Touch

        #region ExtendedScrolling

        public bool EnableExtendedScrolling
        {
            get { return (bool)GetValue(EnableExtendedScrollingProperty); }
            set { SetValue(EnableExtendedScrollingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableExtendedScrolling.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableExtendedScrollingProperty =
            DependencyProperty.Register("EnableExtendedScrolling", typeof(bool), typeof(TimeSpanEdit), new PropertyMetadata(false, OnEnableExtendedScrollingChanged));

        public static void OnEnableExtendedScrollingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj as TimeSpanEdit != null)
                (obj as TimeSpanEdit).OnEnableExtendedScrollingChanged(args);
        }

        private void OnEnableExtendedScrollingChanged(DependencyPropertyChangedEventArgs args)
        {
            if (aLayer == null)
                aLayer = AdornerLayer.GetAdornerLayer(this);

            if ((bool)args.NewValue)
            {
                if (aLayer != null)
                {
                    vAdorner = new ExtendedScrollingAdorner(this);
                    aLayer.Add(vAdorner);
                }
            }
            else
            {
                if (aLayer != null)
                {
                    aLayer.Remove(vAdorner);
                }
            }
        }

        #endregion ExtendedScrolling

#endif
    }

    /// <summary>
    ///
    /// </summary>
    public class SpinCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private readonly Predicate<Object> _canExecute = null;
        private readonly Action<Object> _executeAction = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        public SpinCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="executeAction"></param>
        public SpinCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
            _executeAction = executeAction;
        }

        /// <summary>
        ///
        /// </summary>
        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}