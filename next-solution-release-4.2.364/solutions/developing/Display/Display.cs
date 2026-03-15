using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using Utilities;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
#if !WINDOWS_UWP
using System.Windows.Controls;
using System.Windows.Media;
using WPFUtilities.Extensions;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Display;
#endif

#if !WINDOWS_UWP
namespace Display
#else
namespace UWPDisplay
#endif
{
    public class Display : Gauge
    {
        private PropertyAnimator animator;
        public static DependencyProperty AnimatorProperty;
        bool bLoaded;
        bool bInit;
        public static readonly DependencyProperty DigitMarginProperty = DependencyProperty.Register("DigitMargin", typeof(double), typeof(Display),
#if !WINDOWS_UWP
            new UIPropertyMetadata(0.0, new PropertyChangedCallback(Display.OnPropertyChanged)));
#else
            new PropertyMetadata(0.0));
#endif
        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(Display),
#if !WINDOWS_UWP
            new UIPropertyMetadata("000", new PropertyChangedCallback(Display.OnPropertyChanged)));
#else
            new PropertyMetadata("000"));
#endif
        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register("ActualValue", typeof(double), typeof(Display), new PropertyMetadata(0.0, new PropertyChangedCallback(Display.OnActualValueChanged)));
        public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.Register("InnerMargin", typeof(Thickness), typeof(Display), new PropertyMetadata(new Thickness(1), new PropertyChangedCallback(Display.OnInnerMarginChanged)));
        public readonly static DependencyProperty NegativeNumberBrushProperty = DependencyProperty.Register("NegativeNumberBrush", typeof(Brush), typeof(Display), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(Display.OnPropertyChanged)));
        public readonly static DependencyProperty PositiveNumberBrushProperty = DependencyProperty.Register("PositiveNumberBrush", typeof(Brush), typeof(Display), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(Display.OnPropertyChanged)));
        public static readonly DependencyProperty DecimalBrushProperty = DependencyProperty.Register("DecimalBrush", typeof(Brush), typeof(Display), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(Display.OnPropertyChanged)));

        private void OnActualValueChanged(double oldValue, double newValue)
        {
            if(bInit)
                this.Draw();
        }
        private static void OnActualValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Display)o).OnActualValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnInnerMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Display)o).OnInnerMarginChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
            OnPropertyChanged(o, e);
        }

        private void OnInnerMarginChanged(Thickness oldValue, Thickness newValue)
        {
            this.innerMarginPropertySet = true;
        }
        
        public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Display gauge = d as Display;
            if (gauge != null)
            {
                gauge.oldValue = double.NaN;
                if (gauge.bInit)
                    gauge.Draw();
            }
        }
        private bool innerMarginPropertySet;
        private double oldValue = double.NaN;
        public static DependencyProperty ValueProperty;
        internal IDocument Document;
        IStringEditorManager stringManager;
        char decimalSeparator;
        bool bDesign;

        #region DP OverrideBaseProperties
#if !WINDOWS_UWP
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnBorderPropertyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(Display));
            dpd.AddValueChangedSafe(this, OnBorderPropertyChanged);

        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnBorderPropertyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(Display));
            dpd.RemoveValueChangedSafe(this, OnBorderPropertyChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            if (templateApplied && bInit)
                Draw();
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            if (templateApplied && bInit)
                Draw();
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            if (templateApplied && bInit)
                Draw();
        }
        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            if (templateApplied && bInit)
                Draw();
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (templateApplied && bInit)
            {
                PositiveNumberBrush = Foreground;
                Draw();
            }
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            if (this.templateApplied && bInit && !IsManipulationEnabled)
            {
                var cb = canvasBackground as Border;
                if (cb == null)
                    return;
                cb.Background = Background;
                Draw();
            }
        }


        private void OnBorderPropertyChanged(object sender, EventArgs e)
        {
            var control = sender as Display;
            if (control != null)
            {
                control.OnBorderPropertyChanged();
            }
        }
        protected virtual void OnBorderPropertyChanged()
        {
            if (this.templateApplied && bInit)
            {
                Draw();
            }
        }
#endif
#endregion

#if !WINDOWS_UWP
        static Display()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Display), new FrameworkPropertyMetadata(typeof(Display)));
        }
#endif
        public Display()
        {
            this.animator = new DoubleAnimator();
            this.animator.DesiredDurationMs = 750.0;

#if !WINDOWS_UWP
            OverrideBaseProperties();
#else
            DefaultStyleKey = typeof(Display);
#endif
            Loaded += (o,e) =>
            {
                if(!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    base.HorizontalContentAlignment = this.ReadLocalValue(HorizontalContentAlignmentProperty) == DependencyProperty.UnsetValue ? HorizontalAlignment.Center : HorizontalContentAlignment;
                    base.VerticalContentAlignment = this.ReadLocalValue(VerticalContentAlignmentProperty) == DependencyProperty.UnsetValue ? VerticalAlignment.Center : VerticalContentAlignment;
                    base.BorderBrush = this.ReadLocalValue(BorderBrushProperty) == DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.Black) : BorderBrush;
                    base.BorderThickness = this.ReadLocalValue(BorderThicknessProperty) == DependencyProperty.UnsetValue ? new Thickness(4) : BorderThickness;
                    base.VerticalContentAlignment = this.ReadLocalValue(VerticalContentAlignmentProperty) == DependencyProperty.UnsetValue ? VerticalAlignment.Center : VerticalContentAlignment;
                    base.FontSize = this.ReadLocalValue(FontSizeProperty) == DependencyProperty.UnsetValue ? 18 : FontSize;
                    base.FontFamily = this.ReadLocalValue(FontFamilyProperty) == DependencyProperty.UnsetValue ? new FontFamily("Arial Black") : FontFamily;
                    base.Foreground = this.ReadLocalValue(FontFamilyProperty) == DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.White) : Foreground;

                    var cb = canvasBackground as Border;
                    if (cb != null)
                        cb.Background = Background;
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                    {
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(null, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    Draw();
                    bInit = true;
                }

            };
        }

        protected override void ArrangeElement(FrameworkElement element)
        {
            if (Gauge.GetSizingMethod(element) == SizingMode.Explicit)
            {
                element.Width = base.ControlWidth;
                element.Height = base.ControlHeight;
            }
            else
            {
                base.GetLayoutSize(this);
                double num = base.ControlWidth / base.GetLayoutSize(element).Width;
                double num2 = base.ControlHeight / base.GetLayoutSize(element).Height;
                ScaleTransform transform = new ScaleTransform
                {
                    ScaleX = num,
                    ScaleY = num2
                };
                element.RenderTransform = transform;
            }
        }

        protected override void Draw()
        {
            if (templateApplied)
            {
                base.Draw();
                DrawValue();
            }
        }

        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            if (bDesign || stringManager == null)
                return;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                try
                {
                    decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray().FirstOrDefault();
                }
                catch (Exception)
                {
                    decimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator.ToCharArray().FirstOrDefault();
                }

                if (templateApplied)
                    DrawValue();
            }); 
        }

        private void DrawValue()
        {
            // var digitSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //var decimalSeparator = '.';
            
            //if (digitSeparator.Length > 0)
            //    decimalSeparator = digitSeparator[0];

            if (base.canvasElements != null)
            {
                double actualValue = this.ActualValue;
                base.canvasElements.Children.Clear();
                var chs = actualValue.ToString(this.FormatString, CultureInfo.CurrentCulture).ToCharArray();
                double? digitMargin = this.DigitMargin;
                double num2 = digitMargin.HasValue ? digitMargin.GetValueOrDefault() : (base.ControlWidth * 0.02);

                double width = Math.Floor((double)(((base.ControlWidth - this.InnerMargin.Left) - this.InnerMargin.Right) / ((double)actualValue.ToString(this.FormatString).Length)));
                double left = ((base.ControlWidth - this.InnerMargin.Left - this.InnerMargin.Right) - (width + num2) * chs.Count())/ 2;

                double d = this.InnerMargin.Left + left + num2;
                Brush numberBrush = PositiveNumberBrush;//this.Foreground;
                if (actualValue < 0.0)
                {
                    numberBrush = NegativeNumberBrush;
                }

                foreach (char ch in chs)
                {
                    Digit element = new Digit(this.DigitStyle, ch.ToString(), width, base.ControlHeight)
                    {
                        Foreground = numberBrush,
                        FontSize = FontSize
                    };
                    
                    base.canvasElements.Children.Add(element);
                    if (!double.IsNaN(d))
                    {
                        element.SetValue(Canvas.LeftProperty, d + num2);
                        d = (d + num2) + width;
                    }
                    if (ch == decimalSeparator)
                    {
                        numberBrush = DecimalBrush;
                    }
                }
            }
        }

        private Point NormalizePoint(Point p)
        {
            return new Point(((p.X * -1.0) + 1.0) / 2.0, ((p.Y * -1.0) + 1.0) / 2.0);
        }

#if !WINDOWS_UWP
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            templateApplied = true;
            Draw();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
#if !WINDOWS_UWP
            if (base.EnableAnimation && this.templateApplied)
            {
                if (Double.IsInfinity(newValue) || Double.IsNaN(newValue))
                    newValue = 0;

                if (Double.IsInfinity(this.ActualValue) || Double.IsNaN(this.ActualValue))
                    this.ActualValue = 0;

                this.animator.From = null;
                this.animator.To = newValue;
                this.animator.TargetObject = this;
                this.animator.TargetProperty = "ActualValue";
                Animator.Animate(this.animator, new TimeSpan(0, 0, 0, 0, (int)this.animator.DesiredDurationMs));
                this.animator.To = null;
            }
            else
#endif
            {
                this.ActualValue = newValue;
            }
        }

        public double ActualValue
        {
            get
            {
                return (double)base.GetValue(ActualValueProperty);
            }
            set
            {
                base.SetValue(ActualValueProperty, value);
            }
        }

        public Brush DecimalBrush
        {
            get
            {
                return (Brush)base.GetValue(DecimalBrushProperty);
            }
            set
            {
                base.SetValue(DecimalBrushProperty, value);
            }
        }

        public double DigitMargin
        {
            get
            {
                return (double)base.GetValue(DigitMarginProperty);
            }
            set
            {
                base.SetValue(DigitMarginProperty, value);
            }
        }

        public ResourceDictionary DigitStyle { get; set; }

        public string FormatString
        {
            get
            {
                return (string)base.GetValue(FormatStringProperty);
            }
            set
            {
                base.SetValue(FormatStringProperty, value);
            }
        }

        public Thickness InnerMargin
        {
            get
            {
                return (Thickness)base.GetValue(InnerMarginProperty);
            }
            set
            {
                base.SetValue(InnerMarginProperty, value);
            }
        }

        public Brush NegativeNumberBrush
        {
            get
            {
                return (Brush)base.GetValue(NegativeNumberBrushProperty);
            }
            set
            {
                base.SetValue(NegativeNumberBrushProperty, value);
            }
        }

        public Brush PositiveNumberBrush
        {
            get
            {
                return (Brush)base.GetValue(PositiveNumberBrushProperty);
            }
            set
            {
                base.SetValue(PositiveNumberBrushProperty, value);
            }
        }

#region IDIsposable
        bool bDispose;
        public override void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            base.Dispose();
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
#if !WINDOWS_UWP
            DetachOverrideBaseProperties();
#endif
        }
#endregion

    }
}
