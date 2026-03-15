// ================================================================================
// PopupKeyboard.xaml.cs
//
// This file contains the implementation of a numeric on-screen keyboard using
// attached behaviors in WPF
// Weidong Shen - Dec. 2008
// ================================================================================
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace VirtualKeyboard
{
    /// <summary>
    /// Interaction logic for PopupKeyboard.xaml
    /// </summary>
    internal partial class PopupKeyboardUserControl : UserControl
    {
        #region Private data

        private Popup _parentPopup;
        private Storyboard storyboard;
        private const double AnimationDelay = 150;

        #endregion Private data

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PopupKeyboardUserControl()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Placement
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            Popup.PlacementProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        /// <summary>
        /// PlacementTarget
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
           Popup.PlacementTargetProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        /// <summary>
        /// PlacementRectangle
        /// </summary>
        public static readonly DependencyProperty PlacementRectangleProperty =
            Popup.PlacementRectangleProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public Rect PlacementRectangle
        {
            get { return (Rect)GetValue(PlacementRectangleProperty); }
            set { SetValue(PlacementRectangleProperty, value); }
        }

        /// <summary>
        /// HorizontalOffset
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            Popup.HorizontalOffsetProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// VerticalOffset
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            Popup.VerticalOffsetProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// StaysOpen
        /// </summary>
        public static readonly DependencyProperty StaysOpenProperty =
            Popup.StaysOpenProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public bool StaysOpen
        {
            get { return (bool)GetValue(StaysOpenProperty); }
            set { SetValue(StaysOpenProperty, value); }
        }

        /// <summary>
        /// CustomPopupPlacementCallback
        /// </summary>
        public static readonly DependencyProperty CustomPopupPlacementCallbackProperty =
            Popup.CustomPopupPlacementCallbackProperty.AddOwner(typeof(PopupKeyboardUserControl));

        public CustomPopupPlacementCallback CustomPopupPlacementCallback
        {
            get { return (CustomPopupPlacementCallback)GetValue(CustomPopupPlacementCallbackProperty); }
            set { SetValue(CustomPopupPlacementCallbackProperty, value); }
        }

        /// <summary>
        /// IsOpen
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            Popup.IsOpenProperty.AddOwner(
            typeof(PopupKeyboardUserControl),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(IsOpenChanged)));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// State
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State",
            typeof(KeyboardState),
            typeof(PopupKeyboardUserControl),
            new FrameworkPropertyMetadata(KeyboardState.Normal,
                new PropertyChangedCallback(StateChanged)));

        public KeyboardState State
        {
            get { return (KeyboardState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// NormalHeight
        /// </summary>
        public static readonly DependencyProperty NormalHeightProperty =
            DependencyProperty.Register("NormalHeight",
            typeof(double),
            typeof(PopupKeyboardUserControl),
            new FrameworkPropertyMetadata(0.0));

        public double NormalHeight
        {
            get { return (double)GetValue(NormalHeightProperty); }
            set { SetValue(NormalHeightProperty, value); }
        }

        /// <summary>
        /// NormalWidth
        /// </summary>
        public static readonly DependencyProperty NormalWidthProperty =
            DependencyProperty.Register("NormalWidth",
            typeof(double),
            typeof(PopupKeyboardUserControl),
            new FrameworkPropertyMetadata(0.0));

        public double NormalWidth
        {
            get { return (double)GetValue(NormalWidthProperty); }
            set { SetValue(NormalWidthProperty, value); }
        }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// PropertyChangedCallback method for IsOpen Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void IsOpenChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            PopupKeyboardUserControl ctrl = (PopupKeyboardUserControl)element;

            if ((bool)e.NewValue)
            {
                if (ctrl._parentPopup == null)
                {
                    ctrl.HookupParentPopup();
                }
            }
        }

        /// <summary>
        /// Create the Popup and attach the CustomControl to it.
        /// </summary>
        private void HookupParentPopup()
        {
            _parentPopup = new Popup();

            _parentPopup.AllowsTransparency = true;
            _parentPopup.PopupAnimation = PopupAnimation.Scroll;

            // Set Height and Width
            this.Height = this.NormalHeight;
            this.Width = this.NormalWidth;

            Popup.CreateRootPopup(_parentPopup, this);
        }

        /// <summary>
        /// PropertyChangedCallback method for State Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void StateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            PopupKeyboardUserControl ctrl = (PopupKeyboardUserControl)element;

            if ((KeyboardState)e.NewValue == KeyboardState.Normal)
            {
                ctrl.IsOpen = true;
            }
            else if ((KeyboardState)e.NewValue == KeyboardState.Hidden)
            {
                ctrl.HideKeyboard();
            }
        }

        /// <summary>
        /// Animation to hide keyboard
        /// </summary>
        private void HideKeyboard()
        {
            // Animation to hide the keyboard
            this.RegisterName("HidePopupKeyboard", this);

            storyboard = new Storyboard();
            storyboard.Completed += new EventHandler(storyboard_Completed);

            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.From = this.NormalWidth;
            widthAnimation.To = 0.0;
            widthAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDelay);
            widthAnimation.FillBehavior = FillBehavior.Stop;

            DoubleAnimation heightAnimation = new DoubleAnimation();
            heightAnimation.From = this.NormalHeight;
            heightAnimation.To = 0.0;
            heightAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDelay);
            heightAnimation.FillBehavior = FillBehavior.Stop;

            Storyboard.SetTargetName(widthAnimation, "HidePopupKeyboard");
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(PopupKeyboardUserControl.WidthProperty));
            Storyboard.SetTargetName(heightAnimation, "HidePopupKeyboard");
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(PopupKeyboardUserControl.HeightProperty));
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);

            storyboard.Begin(this);
        }

        /// <summary>
        /// Event handler for storyboard Completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void storyboard_Completed(object sender, EventArgs e)
        {
            this.IsOpen = false;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Method to update Popup window location
        /// </summary>
        public void LocationChange()
        {
            if (this._parentPopup != null)
            {
                this.IsOpen = false;
                _parentPopup.PopupAnimation = PopupAnimation.None;
                this.IsOpen = true;
                _parentPopup.PopupAnimation = PopupAnimation.Scroll;
            }
        }

        #endregion Public Methods

        #region Keyboard Constants

        private const uint KEYEVENTF_KEYUP = 0x2;  // Release key
        private const byte VK_BACK = 0x8;          // back space
        private const byte VK_LEFT = 0x25;
        private const byte VK_RIGHT = 0x27;
        private const byte VK_0 = 0x30;
        private const byte VK_1 = 0x31;
        private const byte VK_2 = 0x32;
        private const byte VK_3 = 0x33;
        private const byte VK_4 = 0x34;
        private const byte VK_5 = 0x35;
        private const byte VK_6 = 0x36;
        private const byte VK_7 = 0x37;
        private const byte VK_8 = 0x38;
        private const byte VK_9 = 0x39;

        #endregion Keyboard Constants

        #region Keyboard Private Methods

        /// <summary>
        /// Event handler for all numeric keyboard events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdNumericButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button key = (Button)sender;

                switch (key.Name)
                {
                    // Number 1
                    case "btn010300":
                        keybd_event(VK_1, 0, 0, (UIntPtr)0);
                        keybd_event(VK_1, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 2
                    case "btn010301":
                        keybd_event(VK_2, 0, 0, (UIntPtr)0);
                        keybd_event(VK_2, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 3
                    case "btn010302":
                        keybd_event(VK_3, 0, 0, (UIntPtr)0);
                        keybd_event(VK_3, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 4
                    case "btn010200":
                        keybd_event(VK_4, 0, 0, (UIntPtr)0);
                        keybd_event(VK_4, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 5
                    case "btn010201":
                        keybd_event(VK_5, 0, 0, (UIntPtr)0);
                        keybd_event(VK_5, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 6
                    case "btn010202":
                        keybd_event(VK_6, 0, 0, (UIntPtr)0);
                        keybd_event(VK_6, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 7
                    case "btn010100":
                        keybd_event(VK_7, 0, 0, (UIntPtr)0);
                        keybd_event(VK_7, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 8
                    case "btn010101":
                        keybd_event(VK_8, 0, 0, (UIntPtr)0);
                        keybd_event(VK_8, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 9
                    case "btn010102":
                        keybd_event(VK_9, 0, 0, (UIntPtr)0);
                        keybd_event(VK_9, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Number 0
                    case "btn010400":
                        keybd_event(VK_0, 0, 0, (UIntPtr)0);
                        keybd_event(VK_0, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Symbol minus sign
                    case "btn010103":
                        keybd_event(0xbd, 0, 0, (UIntPtr)0);
                        keybd_event(0xbd, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Back Space
                    case "btn010402":
                        keybd_event(VK_BACK, 0, 0, (UIntPtr)0);
                        keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Left arrow Key
                    case "btn010203":
                        keybd_event(VK_LEFT, 0, 0, (UIntPtr)0);
                        keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Right arrow Key
                    case "btn010303":
                        keybd_event(VK_RIGHT, 0, 0, (UIntPtr)0);
                        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                    // Symbol full stop
                    case "btn010401":
                        keybd_event(0xbe, 0, 0, (UIntPtr)0);
                        keybd_event(0xbe, 0, KEYEVENTF_KEYUP, (UIntPtr)0);
                        // event already handle
                        e.Handled = true;
                        break;
                }
            }
            catch
            {
                // Any exception handling here.  Otherwise, swallow the exception.
            }
        }

        /// <summary>
        /// Event handler to close the UserControl when close button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.State = KeyboardState.Hidden;
        }

        #endregion Keyboard Private Methods

        #region Windows API Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        #endregion Windows API Functions
    }

    public static class PopupKeyboard
    {
        #region Private data

        private static PopupKeyboardUserControl _popupKeyboardUserControl;
        private static readonly double MinimumWidth = 180.0;
        private static readonly double MinimumHeight = 200.0;

        #endregion Private data

        #region Public Attached Properties

        /// <summary>
        /// Placement
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement",
            typeof(PlacementMode),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(PlacementMode.Bottom,
                new PropertyChangedCallback(PopupKeyboard.OnPlacementChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// PlacementTarget
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
           DependencyProperty.RegisterAttached("PlacementTarget",
           typeof(UIElement),
           typeof(PopupKeyboard),
           new FrameworkPropertyMetadata(null,
               new PropertyChangedCallback(PopupKeyboard.OnPlacementTargetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (UIElement)element.GetValue(PlacementTargetProperty);
        }

        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementTargetProperty, value);
        }

        /// <summary>
        /// PlacementRectangle
        /// </summary>
        public static readonly DependencyProperty PlacementRectangleProperty =
            DependencyProperty.RegisterAttached("PlacementRectangle",
            typeof(Rect),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(Rect.Empty,
                new PropertyChangedCallback(PopupKeyboard.OnPlacementRectangleChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Rect GetPlacementRectangle(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (Rect)element.GetValue(PlacementRectangleProperty);
        }

        public static void SetPlacementRectangle(DependencyObject element, Rect value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementRectangleProperty, value);
        }

        /// <summary>
        /// HorizontalOffset
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("HorizontalOffset",
            typeof(double),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(0.0,
                new PropertyChangedCallback(PopupKeyboard.OnHorizontalOffsetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetHorizontalOffset(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static void SetHorizontalOffset(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// VerticalOffset
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset",
            typeof(double),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(0.0,
                new PropertyChangedCallback(PopupKeyboard.OnVerticalOffsetChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetVerticalOffset(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// CustomPopupPlacementCallback
        /// </summary>
        public static readonly DependencyProperty CustomPopupPlacementCallbackProperty =
            DependencyProperty.RegisterAttached("CustomPopupPlacementCallback",
            typeof(CustomPopupPlacementCallback),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(PopupKeyboard.OnCustomPopupPlacementCallbackChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static CustomPopupPlacementCallback GetCustomPopupPlacementCallback(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (CustomPopupPlacementCallback)element.GetValue(CustomPopupPlacementCallbackProperty);
        }

        public static void SetCustomPopupPlacementCallback(DependencyObject element, CustomPopupPlacementCallback value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(CustomPopupPlacementCallbackProperty, value);
        }

        /// <summary>
        /// State
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State",
            typeof(KeyboardState),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(KeyboardState.Normal,
                new PropertyChangedCallback(PopupKeyboard.OnStateChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static KeyboardState GetState(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (KeyboardState)element.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject element, KeyboardState value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(StateProperty, value);
        }

        /// <summary>
        /// Height
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height",
            typeof(double),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(MinimumHeight,
                new PropertyChangedCallback(PopupKeyboard.OnHeightChanged),
                new CoerceValueCallback(PopupKeyboard.CoerceHeight)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetHeight(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(HeightProperty);
        }

        public static void SetHeight(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(HeightProperty, value);
        }

        /// <summary>
        /// Width
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width",
            typeof(double),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(MinimumWidth,
                new PropertyChangedCallback(PopupKeyboard.OnWidthChanged),
                new CoerceValueCallback(PopupKeyboard.CoerceWidth)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static double GetWidth(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (double)element.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// IsEnabled
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool),
            typeof(PopupKeyboard),
            new FrameworkPropertyMetadata(false,
                new PropertyChangedCallback(PopupKeyboard.OnIsEnabledChanged)));

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetIsEnabled(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(IsEnabledProperty, value);
        }

        #endregion Public Attached Properties

        #region Private Methods

        /// <summary>
        /// PropertyChangedCallback method for Placement Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.Placement = PopupKeyboard.GetPlacement(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for PlacementTarget Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementTargetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.PlacementTarget = PopupKeyboard.GetPlacementTarget(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for PlacementRectangle Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnPlacementRectangleChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.PlacementRectangle = PopupKeyboard.GetPlacementRectangle(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for HorizontalOffset Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnHorizontalOffsetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.HorizontalOffset = PopupKeyboard.GetHorizontalOffset(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for VerticalOffset Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnVerticalOffsetChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.VerticalOffset = PopupKeyboard.GetVerticalOffset(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for CustomPopupPlacementCallback Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnCustomPopupPlacementCallbackChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.CustomPopupPlacementCallback = PopupKeyboard.GetCustomPopupPlacementCallback(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for State Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnStateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.State = PopupKeyboard.GetState(frameworkElement);
            }
        }

        /// <summary>
        /// PropertyChangedCallback method for Height Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnHeightChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.NormalHeight = PopupKeyboard.GetHeight(frameworkElement);
            }
        }

        /// <summary>
        /// CoerceValueCallback method for Height Attached Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceHeight(DependencyObject d, object value)
        {
            if ((double)value < PopupKeyboard.MinimumHeight)
                return PopupKeyboard.MinimumHeight;

            return value;
        }

        /// <summary>
        /// PropertyChangedCallback method for Width Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnWidthChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            if ((frameworkElement != null) && (PopupKeyboard._popupKeyboardUserControl != null))
            {
                _popupKeyboardUserControl.NormalWidth = PopupKeyboard.GetWidth(frameworkElement);
            }
        }

        /// <summary>
        /// CoerceValueCallback method for Width Attached Property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CoerceWidth(DependencyObject d, object value)
        {
            if ((double)value < PopupKeyboard.MinimumWidth)
                return PopupKeyboard.MinimumWidth;

            return value;
        }

        /// <summary>
        /// PropertyChangedCallback method for IsEnabled Attached Property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;

            // Attach & detach handlers for events GotKeyboardFocus, LostKeyboardFocus, MouseDown, and SizeChanged
            if (frameworkElement != null)
            {
                if (((bool)e.NewValue == true) && ((bool)e.OldValue == false))
                {
                    frameworkElement.AddHandler(FrameworkElement.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_GotKeyboardFocus), true);
                    frameworkElement.AddHandler(FrameworkElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_LostKeyboardFocus), true);
                    frameworkElement.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(frameworkElement_MouseDown), true);
                    frameworkElement.AddHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(frameworkElement_SizeChanged), true);
                }
                else if (((bool)e.NewValue == false) && ((bool)e.OldValue == true))
                {
                    frameworkElement.RemoveHandler(FrameworkElement.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_GotKeyboardFocus));
                    frameworkElement.RemoveHandler(FrameworkElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(frameworkElement_LostKeyboardFocus));
                    frameworkElement.RemoveHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(frameworkElement_MouseDown));
                    frameworkElement.RemoveHandler(FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(frameworkElement_SizeChanged));
                }
            }

            Window currentWindow = Window.GetWindow(element);

            // Attach or detach handler for event LocationChanged
            if (currentWindow != null)
            {
                if (((bool)e.NewValue == true) && ((bool)e.OldValue == false))
                {
                    currentWindow.LocationChanged += currentWindow_LocationChanged;
                }
                else if (((bool)e.NewValue == false) && ((bool)e.OldValue == true))
                {
                    currentWindow.LocationChanged -= currentWindow_LocationChanged;
                }
            }
        }

        /// <summary>
        /// Event handler for GotKeyboardFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (PopupKeyboard._popupKeyboardUserControl == null)
                {
                    _popupKeyboardUserControl = new PopupKeyboardUserControl();

                    // Set all the necessary properties
                    _popupKeyboardUserControl.Placement = PopupKeyboard.GetPlacement(frameworkElement);
                    _popupKeyboardUserControl.PlacementTarget = PopupKeyboard.GetPlacementTarget(frameworkElement);
                    _popupKeyboardUserControl.PlacementRectangle = PopupKeyboard.GetPlacementRectangle(frameworkElement);
                    _popupKeyboardUserControl.HorizontalOffset = PopupKeyboard.GetHorizontalOffset(frameworkElement);
                    _popupKeyboardUserControl.VerticalOffset = PopupKeyboard.GetVerticalOffset(frameworkElement);
                    _popupKeyboardUserControl.StaysOpen = true;
                    _popupKeyboardUserControl.CustomPopupPlacementCallback = PopupKeyboard.GetCustomPopupPlacementCallback(frameworkElement);
                    _popupKeyboardUserControl.State = PopupKeyboard.GetState(frameworkElement);
                    _popupKeyboardUserControl.NormalHeight = PopupKeyboard.GetHeight(frameworkElement);
                    _popupKeyboardUserControl.NormalWidth = PopupKeyboard.GetWidth(frameworkElement);

                    if (PopupKeyboard.GetState(frameworkElement) == KeyboardState.Normal)
                        PopupKeyboard._popupKeyboardUserControl.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Event handler for LostKeyboardFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (PopupKeyboard._popupKeyboardUserControl != null)
                {
                    // Retrieves the setting for the State property
                    PopupKeyboard.SetState(frameworkElement, _popupKeyboardUserControl.State);

                    PopupKeyboard._popupKeyboardUserControl.IsOpen = false;
                    PopupKeyboard._popupKeyboardUserControl = null;
                }
            }
        }

        /// <summary>
        /// Event handler for MouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (PopupKeyboard._popupKeyboardUserControl != null)
                {
                    // Double-click switches KeyboardState between Hidden and Normal
                    if (e.ClickCount == 2)
                    {
                        PopupKeyboard._popupKeyboardUserControl.State = (PopupKeyboard._popupKeyboardUserControl.State == KeyboardState.Hidden ? KeyboardState.Normal : KeyboardState.Hidden);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for SizeChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void frameworkElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (PopupKeyboard._popupKeyboardUserControl != null && 
                    PopupKeyboard._popupKeyboardUserControl.State == KeyboardState.Normal)
                {
                    PopupKeyboard._popupKeyboardUserControl.LocationChange();
                }
            }
        }

        /// <summary>
        /// Event handler for LocationChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void currentWindow_LocationChanged(object sender, EventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
            {
                if (PopupKeyboard._popupKeyboardUserControl != null &&
                    PopupKeyboard._popupKeyboardUserControl.State == KeyboardState.Normal)
                {
                    PopupKeyboard._popupKeyboardUserControl.LocationChange();
                }
            }
        }

        #endregion Private Methods
    }

    public enum KeyboardState
    {
        Normal,
        Hidden
    }
}
