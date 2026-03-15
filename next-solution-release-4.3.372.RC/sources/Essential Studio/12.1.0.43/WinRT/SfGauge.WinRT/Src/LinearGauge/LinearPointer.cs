#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    #region LinearPointer

    public class LinearPointer : DependencyObject
    {
        #region Constructor

        public LinearPointer()
        {
            resourceDictionary = new ResourceDictionary
                                     {
#if WINRT
                                         Source = new Uri("ms-appx:///Syncfusion.SfGauge.WinRT/LinearGauge/Themes/LinearGauge.xaml", UriKind.RelativeOrAbsolute)
#elif WINDOWSPHONE_8
                                         Source = new Uri("/Syncfusion.SfGauge.WP8;component/LinearGauge/Themes/LinearGauge.xaml", UriKind.Relative)
#elif WINDOWSPHONE_7
                                         Source = new Uri("/Syncfusion.SfGauge.WP7;component/LinearGauge/Themes/LinearGauge.xaml", UriKind.Relative)

#elif SILVERLIGHT
                                         Source = new Uri("/Syncfusion.SfGauge.Silverlight;component/LinearGauge/Themes/LinearGauge.xaml", UriKind.Relative)
#else
                                         Source = new Uri("/Syncfusion.SfGauge.WPF;component/LinearGauge/Themes/LinearGauge.xaml", UriKind.Relative)
#endif
                                     };
            SetSymbolForPointer();
        }

        #endregion

        #region Dependency Properties

        #region Value
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var linearPointer = (d as LinearPointer);
                linearPointer.SetPointerPosition();
            }
        }
        #endregion

        #region EnableAnimation
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(LinearPointer), new PropertyMetadata(true));
        #endregion

        #region PointerType
        public LinearPointerType PointerType
        {
            get { return (LinearPointerType)GetValue(PointerTypeProperty); }
            set { SetValue(PointerTypeProperty, value); }
        }

        public static readonly DependencyProperty PointerTypeProperty =
            DependencyProperty.Register("PointerType", typeof(LinearPointerType), typeof(LinearPointer), new PropertyMetadata(LinearPointerType.BarPointer, OnPointerTypeChanged));

        private static void OnPointerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var pointer = (d as LinearPointer);
                pointer.SetSymbolForPointer();
                pointer.SetPointerPosition();
            }
        }
        #endregion

        #region ShowPointer
        public bool ShowPointer
        {
            get { return (bool)GetValue(ShowPointerProperty); }
            set { SetValue(ShowPointerProperty, value); }
        }

        public static readonly DependencyProperty ShowPointerProperty =
            DependencyProperty.Register("ShowPointer", typeof(bool), typeof(LinearPointer), new PropertyMetadata(true, OnShowPointerChanged));

        private static void OnShowPointerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var linearPointer = (d as LinearPointer);
                linearPointer.PointerVisibility = (bool)(e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region BarPointerStrokeThickness
        public double BarPointerStrokeThickness
        {
            get { return (double)GetValue(BarPointerStrokeThicknessProperty); }
            set { SetValue(BarPointerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty BarPointerStrokeThicknessProperty =
            DependencyProperty.Register("BarPointerStrokeThickness", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN, OnBarPointerStrokeThicknessChanged));
        #endregion

        #region BarPointerStroke
        public Brush BarPointerStroke
        {
            get { return (Brush)GetValue(BarPointerStrokeProperty); }
            set { SetValue(BarPointerStrokeProperty, value); }
        }

        public static readonly DependencyProperty BarPointerStrokeProperty =
            DependencyProperty.Register("BarPointerStroke", typeof(Brush), typeof(LinearPointer), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        private static void OnBarPointerStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                (d as LinearPointer).BarPointerHeight = (double)e.NewValue;
                (d as LinearPointer).SetPointerPosition();
            }
        }
        #endregion

        #region SymbolPointerHeight
        public double SymbolPointerHeight
        {
            get { return (double)GetValue(SymbolPointerHeightProperty); }
            set { SetValue(SymbolPointerHeightProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerHeightProperty =
            DependencyProperty.Register("SymbolPointerHeight", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN, OnSymbolPointerHeightChanged));

        private static void OnSymbolPointerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                (d as LinearPointer).SymbolHeight = (double)e.NewValue;
            }
        }
        #endregion

        #region SymbolPointerWidth
        public double SymbolPointerWidth
        {
            get { return (double)GetValue(SymbolPointerWidthProperty); }
            set { SetValue(SymbolPointerWidthProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerWidthProperty =
            DependencyProperty.Register("SymbolPointerWidth", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN, OnSymbolPointerWidthChanged));

        private static void OnSymbolPointerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                (d as LinearPointer).SymbolWidth = (double)e.NewValue;
                (d as LinearPointer).SetPointerPosition();
            }
        }
        #endregion

        #region SymbolPointerStroke
        public Brush SymbolPointerStroke
        {
            get { return (Brush)GetValue(SymbolPointerStrokeProperty); }
            set { SetValue(SymbolPointerStrokeProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerStrokeProperty =
            DependencyProperty.Register("SymbolPointerStroke", typeof(Brush), typeof(LinearPointer), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));
        #endregion

        #region SymbolPointerStyle
        public LinearSymbolPointerStyle SymbolPointerStyle
        {
            get { return (LinearSymbolPointerStyle)GetValue(SymbolPointerStyleProperty); }
            set { SetValue(SymbolPointerStyleProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerStyleProperty =
            DependencyProperty.Register("SymbolPointerStyle", typeof(LinearSymbolPointerStyle), typeof(LinearPointer), new PropertyMetadata(LinearSymbolPointerStyle.Triangle, OnSymbolPointerStyleChanged));

        private static void OnSymbolPointerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var linearPointer = (d as LinearPointer);
                if (linearPointer.PointerType == LinearPointerType.SymbolPointer)
                {
                    linearPointer.SetSymbolForPointer();
                    linearPointer.SetPointerPosition();
                }
            }
        }
        #endregion

        #region SymbolPointerTemplate
        public DataTemplate SymbolPointerTemplate
        {
            get { return (DataTemplate)GetValue(SymbolPointerTemplateProperty); }
            set { SetValue(SymbolPointerTemplateProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerTemplateProperty =
            DependencyProperty.Register("SymbolPointerTemplate", typeof(DataTemplate), typeof(LinearPointer), new PropertyMetadata(null, OnSymbolPointerTemplateChanged));

        private static void OnSymbolPointerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var linearPointer = (d as LinearPointer);
                if (linearPointer.PointerType == LinearPointerType.SymbolPointer && linearPointer.SymbolPointerStyle == LinearSymbolPointerStyle.Custom)
                {
                    linearPointer.SetSymbolForPointer();
                    linearPointer.SetPointerPosition();
                }
            }
        }
        #endregion

        #region SymbolPointerPosition
        public LinearSymbolPointersPosition SymbolPointerPosition
        {
            get { return (LinearSymbolPointersPosition)GetValue(SymbolPointerPositionProperty); }
            set { SetValue(SymbolPointerPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPointerPositionProperty =
            DependencyProperty.Register("SymbolPointerPosition", typeof(LinearSymbolPointersPosition), typeof(LinearPointer), new PropertyMetadata(LinearSymbolPointersPosition.Below, OnSymbolPointersPositionChanged));

        private static void OnSymbolPointersPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearPointer)
            {
                var symbolPointer = (d as LinearPointer);
                symbolPointer.SetPointerPosition();
            }
        }
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region Symbol
        internal object Symbol
        {
            get { return GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(object), typeof(LinearPointer), new PropertyMetadata(null));
        #endregion

        #region BarPointerHeight
        internal double BarPointerHeight
        {
            get { return (double)GetValue(BarPointerHeightProperty); }
            set { SetValue(BarPointerHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarPointerHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BarPointerHeightProperty =
            DependencyProperty.Register("BarPointerHeight", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN));
        #endregion

        #region BarPointerWidth
        internal double BarPointerWidth
        {
            get { return (double)GetValue(BarPointerWidthProperty); }
            set { SetValue(BarPointerWidthProperty, value); }
        }

        internal static readonly DependencyProperty BarPointerWidthProperty =
            DependencyProperty.Register("BarPointerWidth", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN));
        #endregion

        #region SymbolWidth
        internal double SymbolWidth
        {
            get { return (double)GetValue(SymbolWidthProperty); }
            set { SetValue(SymbolWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolWidthProperty =
            DependencyProperty.Register("SymbolWidth", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN));
        #endregion

        #region SymbolHeight
        internal double SymbolHeight
        {
            get { return (double)GetValue(SymbolHeightProperty); }
            set { SetValue(SymbolHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolHeightProperty =
            DependencyProperty.Register("SymbolHeight", typeof(double), typeof(LinearPointer), new PropertyMetadata(double.NaN));
        #endregion

        #region PointerLeft
        internal double PointerLeft
        {
            get { return (double)GetValue(PointerLeftProperty); }
            set { SetValue(PointerLeftProperty, value); }
        }

        internal static readonly DependencyProperty PointerLeftProperty =
            DependencyProperty.Register("PointerLeft", typeof(double), typeof(LinearPointer), new PropertyMetadata(0d));
        #endregion

        #region PointerTop
        internal double PointerTop
        {
            get { return (double)GetValue(PointerTopProperty); }
            set { SetValue(PointerTopProperty, value); }
        }

        internal static readonly DependencyProperty PointerTopProperty =
            DependencyProperty.Register("PointerTop", typeof(double), typeof(LinearPointer), new PropertyMetadata(0d));
        #endregion

        #region PointerScaleY
        internal double PointerScaleY
        {
            get { return (double)GetValue(PointerScaleYProperty); }
            set { SetValue(PointerScaleYProperty, value); }
        }

        internal static readonly DependencyProperty PointerScaleYProperty =
            DependencyProperty.Register("PointerScaleY", typeof(double), typeof(LinearPointer), new PropertyMetadata(1d));
        #endregion

        #region PointerVisibility
        internal Visibility PointerVisibility
        {
            get { return (Visibility)GetValue(PointerVisibilityProperty); }
            set { SetValue(PointerVisibilityProperty, value); }
        }

        internal static readonly DependencyProperty PointerVisibilityProperty =
            DependencyProperty.Register("PointerVisibility", typeof(Visibility), typeof(LinearPointer), new PropertyMetadata(Visibility.Visible));

        #endregion

        #endregion

        #region CLR Properties

        public LinearScale ParentScale { get; internal set; }

        #endregion

        #region Private Members

        Storyboard symbolPointerStoryBoard, barPointerStoryBoard;
        DoubleAnimation symbolPointerAnimation, barPointerAnimation;
        readonly ResourceDictionary resourceDictionary;

        #endregion

        #region Implementation

        internal void SetPointerPosition()
        {
            if (ParentScale != null && ParentScale.ParentGauge != null)
            {
                double pointerValue = Value;
                if (Value < ParentScale.Minimum)
                    pointerValue = ParentScale.Minimum;
                else if (Value > ParentScale.Maximum)
                    pointerValue = ParentScale.Maximum;

                double length = (pointerValue - ParentScale.Minimum) * ParentScale.ScaleBarWidth / (ParentScale.Maximum - ParentScale.Minimum);

                if (!double.IsNaN(length))
                {
                    PointerVisibility = ShowPointer ? Visibility.Visible : Visibility.Collapsed;

                    if (PointerType == LinearPointerType.BarPointer)
                    {
                        barPointerAnimation.Duration = TimeSpan.FromMilliseconds(EnableAnimation ? 500 : 0);
                        barPointerAnimation.From = Double.IsNaN(BarPointerWidth) ? 0 : BarPointerWidth;

                        if (Double.IsNaN(BarPointerHeight))
                            BarPointerHeight = ParentScale.ScaleBarHeight / 2;
                        BarPointerWidth = length;
                        PointerTop = (ParentScale.ParentGauge.GaugeSize.Height - BarPointerHeight) / 2;

                        barPointerAnimation.To = BarPointerWidth;
                        barPointerStoryBoard.Begin();
                    }
                    else
                    {
                        symbolPointerAnimation.Duration = TimeSpan.FromMilliseconds(EnableAnimation ? 500 : 0);
                        symbolPointerAnimation.From = PointerLeft;

                        if (Double.IsNaN(SymbolHeight))
                            SymbolHeight = ParentScale.ScaleBarHeight / 2;
                        if (Double.IsNaN(SymbolWidth))
                            SymbolWidth = ParentScale.ScaleBarHeight / 2;

                        double pointerWidth = SymbolWidth, pointerHeight = SymbolHeight;
                        if (SymbolPointerStyle == LinearSymbolPointerStyle.Custom && SymbolPointerTemplate != null)
                        {
                            var frameworkElement = SymbolPointerTemplate.LoadContent() as FrameworkElement;
                            if (frameworkElement != null)
                            {
                                pointerWidth = (Double.IsNaN(frameworkElement.Width) ? 0 : frameworkElement.Width);
                                pointerHeight = (Double.IsNaN(frameworkElement.Height) ? 0 : frameworkElement.Height);
                            }
                        }

                        if (pointerWidth.Equals(0) || pointerHeight.Equals(0))
                            PointerVisibility = Visibility.Collapsed;

                        if (SymbolPointerPosition == LinearSymbolPointersPosition.Below)
                        {
                            PointerLeft = length - pointerWidth / 2;
                            PointerTop = (ParentScale.ParentGauge.GaugeSize.Height + ParentScale.ScaleBarHeight) / 2;
                        }
                        else if (SymbolPointerPosition == LinearSymbolPointersPosition.Above)
                        {
                            PointerLeft = length - pointerWidth / 2;
                            PointerTop = (ParentScale.ParentGauge.GaugeSize.Height - ParentScale.ScaleBarHeight) / 2;
                        }
                        else
                        {
                            PointerLeft = length - pointerWidth / 2;
                            PointerTop = (ParentScale.ParentGauge.GaugeSize.Height - pointerHeight) / 2;
                        }

                        symbolPointerAnimation.To = PointerLeft;
                        symbolPointerStoryBoard.Begin();

                        PointerScaleY = SymbolPointerPosition == LinearSymbolPointersPosition.Above ? -1 : 1;
                    }
                }
                else
                {
                    PointerVisibility = Visibility.Collapsed;
                }
            }
        }

        private void SetSymbolForPointer()
        {
            symbolPointerStoryBoard = barPointerStoryBoard = new Storyboard();
            symbolPointerAnimation = new DoubleAnimation
                                         {
                                             FillBehavior = FillBehavior.HoldEnd,
#if WINRT
                                             EnableDependentAnimation = true
#endif
                                         };

            barPointerAnimation = new DoubleAnimation
                                      {
                                          Duration = TimeSpan.FromMilliseconds(EnableAnimation ? 500 : 0),
                                          FillBehavior = FillBehavior.Stop,
#if WINRT
                                          EnableDependentAnimation = true
#endif
                                      };
            if (PointerType == LinearPointerType.SymbolPointer)
            {
                var symbolCanvas = new Canvas();
                UIElement symbolContent = null;

                if (SymbolPointerStyle == LinearSymbolPointerStyle.Triangle)
                {
                    var dataTemplate = resourceDictionary["TrianglePointerTemplateKey"] as DataTemplate;
                    if (dataTemplate != null)
                        symbolContent = dataTemplate.LoadContent() as UIElement;
                }
                else if (SymbolPointerStyle == LinearSymbolPointerStyle.Custom && SymbolPointerTemplate != null)
                    symbolContent = SymbolPointerTemplate.LoadContent() as UIElement;

                if (symbolContent != null)
                {
                    symbolCanvas.Children.Add(symbolContent);
                    symbolPointerStoryBoard.Children.Add(symbolPointerAnimation);
                    Storyboard.SetTarget(symbolPointerAnimation, symbolContent);

#if WINRT
                    Storyboard.SetTargetProperty(symbolPointerAnimation, "(Canvas.Left)");
#else
                    Storyboard.SetTargetProperty(symbolPointerAnimation, new PropertyPath("(Canvas.Left)"));                    
#endif
                }

                Symbol = symbolCanvas;
            }
            else
            {
                var dataTemplate = resourceDictionary["BarPointerTemplateKey"] as DataTemplate;
                if (dataTemplate != null)
                {
                    var symbolContent = dataTemplate.LoadContent() as UIElement;
                    Symbol = symbolContent;
                    barPointerStoryBoard.Children.Add(barPointerAnimation);
                    Storyboard.SetTarget(barPointerAnimation, symbolContent);
                }
#if WINRT
                Storyboard.SetTargetProperty(barPointerAnimation, "Width");
#else
                Storyboard.SetTargetProperty(barPointerAnimation, new PropertyPath("Width"));
                
#endif
            }
        }

        #endregion
    }

    #endregion

    #region LinearPointerCollection

    public class LinearPointerCollection : ObservableCollection<LinearPointer>
    {
    }

    #endregion
}
