#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Shapes;

namespace Syncfusion.WP.Controls.Media
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Syncfusion.UI.Xaml.Controls.Navigation;

namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a control for enabling the user to select colors from
    /// a selection box or slider
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/>
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorTooltip"/>
    [ClassReference(IsReviewed = false)]
    public class SfColorPicker:Control
    {
#if WINRT
        internal ResourceWrapper resourceWrapper;

        internal string R { get; set; }
        internal string G { get; set; }
        internal string B { get; set; }
        internal string RGB { get; set; }
#endif

        #region private members

        private SfRadialSlider PART_RSlider;

        private Popup PreviousSlider;

        private bool IsRadialValueChanged;

        private Color _previousColor;

        private ColorSlider PART_Slider;

        private ColorSelectionBox PART_Box;

        private FrameworkElement PART_R;

        private Popup PART_RPopup;

        private Point pointerPosition;

        private bool IsSelectionBoxColorSet=false;

        private DispatcherTimer _timer;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private Pointer pointer;
#endif

        private SfRadialSlider PART_GSlider;

        private FrameworkElement PART_G;

        private Popup PART_GPopup;

        private SfRadialSlider PART_BSlider;

        private FrameworkElement PART_B;

        private Popup PART_BPopup;
#if WINRT
        private TextBlock PART_RGBTextblock;
        private TextBlock PART_RTextblock;
        private TextBlock PART_GTextblock;
        private TextBlock PART_BTextblock;
#endif
        private bool IsRadialPointerPressed = false;

        #endregion

       

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>  
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfColorPicker()
        {
            DefaultStyleKey= typeof(SfColorPicker);
            this.Loaded += SfColorPicker_Loaded;
            this.Unloaded += SfColorPicker_Unloaded;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0,0,3);

#if WINRT
            resourceWrapper = new ResourceWrapper();
            this.R = resourceWrapper.R;
            this.G = resourceWrapper.G;
            this.B = resourceWrapper.B;
            this.RGB = resourceWrapper.RGB;
#endif
            
        }

        void SfColorPicker_Unloaded(object sender, RoutedEventArgs e)
        {
#if WINRT
            Window.Current.Content.PointerPressed -= Content_PointerPressed;
            Window.Current.Content.PointerCaptureLost -= Content_PointerCaptureLost;
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
            Application.Current.RootVisual.MouseLeftButtonUp -= RootVisual_MouseLeftButtonUp;
            Application.Current.RootVisual.MouseLeave -= RootVisual_MouseLeave;
#endif
            this.Loaded -= SfColorPicker_Loaded;
            this.Unloaded -= SfColorPicker_Unloaded;
        }

        void SfColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
#if WINRT
            Window.Current.Content.PointerPressed += Content_PointerPressed;
            Window.Current.Content.PointerCaptureLost += Content_PointerCaptureLost;
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
            Application.Current.RootVisual.MouseLeftButtonUp += RootVisual_MouseLeftButtonUp;
            Application.Current.RootVisual.MouseLeave += RootVisual_MouseLeave;
#endif
            if (SelectedColor != null)
            {
                UpdateSelectedColor(SelectedColor);
            }
        }

#if WINRT
        void Content_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (!(e.OriginalSource is TextBlock))
            {
                if (PART_RPopup != null && PART_RPopup.IsOpen)
                    PART_RPopup.IsOpen = false;
                if (PART_GPopup != null && PART_GPopup.IsOpen)
                    PART_GPopup.IsOpen = false;
                if (PART_BPopup != null && PART_BPopup.IsOpen)
                    PART_BPopup.IsOpen = false;
                PreviousSlider = null;
            }
        }

        void Content_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!(e.OriginalSource is TextBlock))
            {
                if (PART_RPopup != null && PART_RPopup.IsOpen)
                    PART_RPopup.IsOpen = false;
                if (PART_GPopup != null && PART_GPopup.IsOpen)
                    PART_GPopup.IsOpen = false;
                if (PART_BPopup != null && PART_BPopup.IsOpen)
                    PART_BPopup.IsOpen = false;
                PreviousSlider = null;
            }
        }
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
        void RootVisual_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is TextBlock))
            {
                if (PART_RPopup != null && PART_RPopup.IsOpen)
                    PART_RPopup.IsOpen = false;
                if (PART_GPopup != null && PART_GPopup.IsOpen)
                    PART_GPopup.IsOpen = false;
                if (PART_BPopup != null && PART_BPopup.IsOpen)
                    PART_BPopup.IsOpen = false;
                PreviousSlider = null;
            }
        }

        void RootVisual_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!(e.OriginalSource is TextBlock))
            {
                if (PART_RPopup != null && PART_RPopup.IsOpen)
                    PART_RPopup.IsOpen = false;
                if (PART_GPopup != null && PART_GPopup.IsOpen)
                    PART_GPopup.IsOpen = false;
                if (PART_BPopup != null && PART_BPopup.IsOpen)
                    PART_BPopup.IsOpen = false;
                PreviousSlider = null;
            }
        }
#endif

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Returns a color from the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/> control that has been selected.
        /// </summary>
        /// <remarks>
        /// The color can be selected from the
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSelectionBox"/>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Media.ColorSlider"/>
        /// </remarks>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set {SetValue(SelectedColorProperty, value);        }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(SfColorPicker), new PropertyMetadata(Colors.Transparent, new PropertyChangedCallback(OnSelectedColorChanged)));

        /// <summary>
        /// Returns the previous color that has been selected from the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/> control.
        /// </summary>
        public Color PreviousColor
        {
            get { return (Color)GetValue(PreviousColorProperty); }
            set {SetValue(PreviousColorProperty, value); 
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousColorProperty =
            DependencyProperty.Register("PreviousColor", typeof(Color), typeof(SfColorPicker), new PropertyMetadata(Colors.Transparent));

        #endregion

        #region Override methods

        /// <summary>
        /// Initializes all the child elements of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Box = GetTemplateChild("PART_Box") as ColorSelectionBox;
            PART_Slider = GetTemplateChild("PART_Slider") as ColorSlider;
            PART_RSlider = GetTemplateChild("PART_RSlider") as SfRadialSlider;
            PART_R = GetTemplateChild("PART_R") as FrameworkElement;
            PART_RPopup = GetTemplateChild("PART_RPopup") as Popup;

            PART_GSlider = GetTemplateChild("PART_GSlider") as SfRadialSlider;
            PART_G = GetTemplateChild("PART_G") as FrameworkElement;
            PART_GPopup = GetTemplateChild("PART_GPopup") as Popup;

            PART_BSlider = GetTemplateChild("PART_BSlider") as SfRadialSlider;
            PART_B = GetTemplateChild("PART_B") as FrameworkElement;
            PART_BPopup = GetTemplateChild("PART_BPopup") as Popup;

#if WINRT
            #region Textblock_binding
            PART_RGBTextblock = GetTemplateChild("PART_RGBTextblock") as TextBlock;
            PART_RTextblock = GetTemplateChild("PART_RTextblock") as TextBlock;
            PART_GTextblock = GetTemplateChild("PART_GTextblock") as TextBlock;
            PART_BTextblock = GetTemplateChild("PART_BTextblock") as TextBlock;

            if (PART_RGBTextblock != null)
            {
                Binding binding = new Binding();
                binding.Source = resourceWrapper;
                binding.Mode = BindingMode.TwoWay;
                binding.Path = new PropertyPath("RGB");
                PART_RGBTextblock.SetBinding(TextBlock.TextProperty, binding);
            }

            if (PART_RTextblock != null)
            {
                Binding binding = new Binding();
                binding.Source = resourceWrapper;
                binding.Mode = BindingMode.TwoWay;
                binding.Path = new PropertyPath("R");
                PART_RTextblock.SetBinding(TextBlock.TextProperty, binding);
            }


            if (PART_GTextblock != null)
            {
                Binding binding = new Binding();
                binding.Source = resourceWrapper;
                binding.Mode = BindingMode.TwoWay;
                binding.Path = new PropertyPath("G");
                PART_GTextblock.SetBinding(TextBlock.TextProperty, binding);
            }


            if (PART_BTextblock != null)
            {
                Binding binding = new Binding();
                binding.Source = resourceWrapper;
                binding.Mode = BindingMode.TwoWay;
                binding.Path = new PropertyPath("B");
                PART_BTextblock.SetBinding(TextBlock.TextProperty, binding);
            }
            #endregion
#endif
            if (PART_R != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_R.ManipulationStarted += OnManipulationStarted;
                PART_R.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_R.PointerPressed += OnRadialPointerPressed;
                PART_R.PointerReleased += OnRadialPointerReleased;
#endif
             }

            if (PART_RSlider != null)
            {
                PART_RSlider.ValueChanged += OnRadialSliderValueChanged;
#if WINRT
                PART_RSlider.Loaded += PART_Slider_Loaded;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_RSlider.ManipulationCompleted  += OnManipulationCompleted;
#else
                PART_RSlider.PointerReleased += OnRadialPointerReleased;
#endif
            }

            if (PART_G != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_G.ManipulationStarted += OnManipulationStarted;
                PART_G.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_G.PointerPressed += OnRadialPointerPressed;
                PART_G.PointerReleased += OnRadialPointerReleased;
#endif
            }

            if (PART_GSlider != null)
            {
                PART_GSlider.ValueChanged += OnRadialSliderValueChanged;
#if WINRT
                PART_GSlider.Loaded += PART_Slider_Loaded;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_GSlider.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_GSlider.PointerReleased += OnRadialPointerReleased;
#endif
            }

            if (PART_B != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_B.ManipulationStarted += OnManipulationStarted;
                PART_B.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_B.PointerPressed += OnRadialPointerPressed;
                PART_B.PointerReleased += OnRadialPointerReleased;
#endif
            }

            if (PART_BSlider != null)
            {
                PART_BSlider.ValueChanged += OnRadialSliderValueChanged;
#if WINRT
                PART_BSlider.Loaded += PART_Slider_Loaded;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_BSlider.ManipulationCompleted += OnManipulationCompleted;
#else
                PART_BSlider.PointerReleased += OnRadialPointerReleased;
#endif
            }

            if (PART_Slider != null)
            {
                PART_Slider.ValueChanged += PartSliderValueChanged;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Slider.ManipulationCompleted += PartSliderManipulationCompleted;
#else
                PART_Slider.PointerReleased += OnPointerReleased;
#endif
            }

            if (PART_Box != null)
            {
                PART_Box.SelectionChanged += PartBoxSelectionChanged;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                PART_Box.ManipulationCompleted += PartSliderManipulationCompleted;
#else
                PART_Box.PointerReleased += OnPointerReleased;
#endif
            }
            this.LayoutUpdated+=SfColorPicker_LayoutUpdated;
            base.OnApplyTemplate();
        }

        
#if WINRT
        void PART_Slider_Loaded(object sender, RoutedEventArgs e)
        {
            SfRadialSlider sfRadialSlider = sender as SfRadialSlider;
            ContentPresenter contentPresenter = GetVisualChild<ContentPresenter>(sfRadialSlider);
            if (contentPresenter != null)
            {
                StackPanel stackPanel = GetVisualChild<StackPanel>(contentPresenter);
                if (stackPanel != null)
                {
                    Binding binding = new Binding();
                    binding.Source = resourceWrapper;
                    binding.Mode = BindingMode.TwoWay;
                    TextBlock textBlock = (TextBlock) VisualTreeHelper.GetChild(stackPanel, 1);
                    if (sfRadialSlider.Name == "PART_RSlider")
                        binding.Path = new PropertyPath("R");
                    else if (sfRadialSlider.Name == "PART_GSlider")
                        binding.Path = new PropertyPath("G");
                    else
                        binding.Path = new PropertyPath("B");
                    textBlock.SetBinding(TextBlock.TextProperty, binding);
                }
            }
        }
#endif
        #endregion

        #region events

        /// <summary>
        /// Invokes an event when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Media.SfColorPicker"/> conntrol's 
        /// selected color has changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler SelectedColorChanged;

        #endregion

#if WINRT
        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
#endif

        #region CallBacks
        private void SfColorPicker_LayoutUpdated(object sender, object e)
        {
            if (SelectedColor != null && PART_Slider != null && PART_Box != null)
            {
                Color SelectionBoxColor = PART_Box.ApplyColor(PART_Slider.Value, PART_Box.transform.TranslateX,
                                                        PART_Box.transform.TranslateY);
                if (SelectionBoxColor != SelectedColor)
                {
                    UpdateSelectedColor(SelectedColor);
                }
            }
        }
        private static void OnSelectedColorChanged(DependencyObject sender,DependencyPropertyChangedEventArgs e)
        {
            var control = sender as SfColorPicker;
            if (control != null) control.OnSelectedColorChanged(e);
        }

        private void OnSelectedColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && !IsRadialPointerPressed)
            {   //value will be used for programmatic color changes
                _previousColor =(Color) e.OldValue;
            }
            UpdateSelectedColor((Color)e.NewValue);

            if (SelectedColorChanged != null)
                SelectedColorChanged(this, e);
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void OnManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        void OnRadialPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (PreviousSlider != null && IsRadialValueChanged)
            {
                PreviousSlider.IsOpen = false;
                PreviousSlider = null;
            }
            IsRadialValueChanged = false;
            IsRadialPointerPressed = false;
            PreviousColor = _previousColor;
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void OnManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
#else
        void OnRadialPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            IsRadialValueChanged = false;
            if (PreviousSlider != null && PreviousSlider.IsOpen)
            {
                PreviousSlider.IsOpen = false;
            }

            IsRadialPointerPressed = true;
            _previousColor = SelectedColor;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            pointer = e.Pointer;
#endif
            if((sender as FrameworkElement).Name.Equals("PART_R"))
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                pointerPosition = e.ManipulationOrigin;
#else
                pointerPosition = e.GetCurrentPoint(PART_R).Position;
#endif
                ShowRadialPopup(PART_RPopup, PART_RSlider, pointerPosition);
            }
            else if((sender as FrameworkElement).Name.Equals("PART_G"))
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                pointerPosition = e.ManipulationOrigin;
#else
                pointerPosition = e.GetCurrentPoint(PART_G).Position;
#endif
                ShowRadialPopup(PART_GPopup, PART_GSlider, pointerPosition);
            }
            else if ((sender as FrameworkElement).Name.Equals("PART_B"))
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                pointerPosition = e.ManipulationOrigin;
#else
                pointerPosition = e.GetCurrentPoint(PART_B).Position;
#endif
                ShowRadialPopup(PART_BPopup, PART_BSlider, pointerPosition);
            }
        }

        private void ShowRadialPopup(Popup popup, SfRadialSlider slider, Point point)
        {
            pointerPosition = point;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            //Set the private flag to true, so that pointer will rotate.
            var members = slider.GetType().GetRuntimeFields();
            foreach (var fieldInfo in members.Where(fieldInfo => fieldInfo.Name == "_isPointerPressed"))
            {
                fieldInfo.SetValue(slider, true);
                break;
            }
#endif
            popup.Opened += OnRadialPopupOpened;
            if (PreviousSlider != popup)
            {
                popup.IsOpen = true;
                _timer.Tick += OntimerTickStarted;
                _timer.Start();
            }
            else
            {
                PreviousSlider = null;
            }
            slider.Opacity = 0;
        }

        void OntimerTickStarted(object sender, object e)
        {
            _timer.Stop();
            _timer.Tick -= OntimerTickStarted;
            if (PreviousSlider != null)
            {
                PreviousSlider.IsOpen = false;
                PreviousSlider = null;
            }
        }

        void OnRadialPopupOpened(object sender, object e)
        {
            SfRadialSlider Slider = null;
            Popup popup = (sender as Popup);

            if (popup != null)
            {
                popup.Opened -= OnRadialPopupOpened;

                if (popup.Name.Equals("PART_RPopup"))
                    Slider = PART_RSlider;
                else if (popup.Name.Equals("PART_GPopup"))
                    Slider = PART_GSlider;
                else if (popup.Name.Equals("PART_BPopup"))
                    Slider = PART_BSlider;
                PreviousSlider = popup;
            }
#if WPF||SILVERLIGHT||WINDOWS_PHONE_7
            var bindingFlags = BindingFlags.Instance |
                   BindingFlags.NonPublic |
                   BindingFlags.Public | BindingFlags.Static;
            var elements = Slider.GetType().GetFields(bindingFlags);
#else
            var elements = Slider.GetType().GetRuntimeFields();
#endif
            foreach (var fieldInfo in elements.Where(fieldInfo => fieldInfo.Name == "_radialPointer"))
            {
                var pointer = fieldInfo.GetValue(Slider) as RadialPointer;

                if (pointer != null)
                {
#if WPF||SILVERLIGHT||WINDOWS_PHONE_7
                    pointer.CaptureMouse();
                    var _elements = pointer.GetType().GetFields(bindingFlags);
#else
                    var _elements = pointer.GetType().GetRuntimeFields();
#endif
#if WINRT
                    pointer.CapturePointer(this.pointer);
#endif
                    foreach (var root in _elements.Where(_fieldInfo => _fieldInfo.Name == "_partRoot").Select(_fieldInfo => _fieldInfo.GetValue(pointer) as FrameworkElement))
                    {
                        if (root != null)
                        {
                            var ellipse = ((Grid)root).Children[1] as Ellipse;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                            if (ellipse != null && sender is Popup && (sender as Popup).IsOpen)
#else
                            if (ellipse != null)
#endif
                            {
                                var transform = ellipse.TransformToVisual(Slider);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                                var point = transform.Transform(new Point());
#else
                                var point = transform.TransformPoint(new Point());
#endif
                                var absolutepoint = new Point(pointerPosition.X - point.X, pointerPosition.Y - point.Y);
                                if (popup != null)
                                {
                                    if (absolutepoint.X < 0)
                                    {
                                        popup.HorizontalOffset = absolutepoint.X - 10;
                                    }
                                    else
                                    {
                                        popup.HorizontalOffset = absolutepoint.X + 10;
                                    }

                                    if (absolutepoint.Y < 0)
                                    {
                                        popup.VerticalOffset = absolutepoint.Y - 10;
                                    }
                                    else
                                    {
                                        popup.VerticalOffset = absolutepoint.Y + 10;
                                    }
                                    Slider.Opacity = 1;
                                }
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void OnRadialSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        void OnRadialSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            IsRadialValueChanged = true;
            _timer.Tick -= OntimerTickStarted;
            _timer.Stop();
            var slider = (sender as FrameworkElement).Name;
           if(slider.Equals("PART_RSlider"))
                SelectedColor = new Color { A = SelectedColor.A, R = (byte)e.NewValue, B = SelectedColor.B, G = SelectedColor.G };
           else if (slider.Equals("PART_GSlider"))
               SelectedColor = new Color { A = SelectedColor.A, R = SelectedColor.R, B = SelectedColor.B, G = (byte)e.NewValue };
           else if (slider.Equals("PART_BSlider"))
               SelectedColor = new Color { A = SelectedColor.A, R = SelectedColor.R, B = (byte)e.NewValue, G = SelectedColor.G };
        }

        void PartBoxSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Box != null)
            {
                if (PreviousSlider != null)
                {
                    PreviousSlider.IsOpen = false;
                    PreviousSlider = null;
                }
               IsSelectionBoxColorSet = true;
               SelectedColor = (Color)e.NewValue;
               IsSelectionBoxColorSet = false;
            }
        }

        void PartSliderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (PART_Box != null)
            {
                if (PreviousSlider != null)
                {
                    PreviousSlider.IsOpen = false;
                    PreviousSlider = null;
                }
                IsSelectionBoxColorSet = true;
                SelectedColor = PART_Box.ApplyColor((double)e.NewValue, PART_Box.transform.TranslateX,
                                                        PART_Box.transform.TranslateY);
                IsSelectionBoxColorSet = false;
            }
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        void PartSliderManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
#endif
        {
            //To set the previous color while moving the thumb of Colorselectionbox and color slider.
            if (PreviousColor != SelectedColor && PART_Box!=null)
            {
                if (sender is ColorSelectionBox)
                    PreviousColor = PART_Box.PreviousColor;
                else if (sender is ColorSlider && PART_Slider != null)
                {
                    PreviousColor = PART_Box.ApplyColor(PART_Slider.previousHue, PART_Box.transform.TranslateX,
                                                        PART_Box.transform.TranslateY);
                }
            }
        }

        void UpdateSelectedColor(Color color)
        {
            if (!IsSelectionBoxColorSet)
            {
                //update the previous color on programmatic color changes
                PreviousColor = _previousColor;
                if (PART_Slider != null)
                {
                    PART_Slider.UpdateSliderPosition(color);
                }
                if (PART_Box != null)
                {
                    PART_Box.UpdateThumbPoision(color);
                }
            }
            if (PART_RSlider != null && PART_GSlider != null && PART_BSlider != null && SelectedColor != null)
            {
                PART_RSlider.Value = SelectedColor.R;
                PART_GSlider.Value = SelectedColor.G;
                PART_BSlider.Value = SelectedColor.B;
            }
        }

        #endregion
    }
}
