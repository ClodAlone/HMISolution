using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Display
{
    public class Digit : Control
    {
        private bool hasCustomTemplate;
        private FrameworkElement rootElement;
        private double scaledHeight;
        private double scaledWidth;
        private bool templateApplied;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(Digit), new PropertyMetadata(new PropertyChangedCallback(Digit.OnValueChanged)));

#if !WINDOWS_UWP
        static Digit()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Digit), new FrameworkPropertyMetadata(typeof(Digit)));
        }
#endif
        public Digit()
        {
        }

        public Digit(ResourceDictionary rd, string value, double width, double height)
        {
            base.DefaultStyleKey = typeof(Digit);
#if !WINDOWS_UWP
            if ((rd != null) && rd.Contains(value))
#else
            if ((rd != null) && rd.ContainsKey(value))
#endif
            {
                base.Style = rd[value] as Style;
                this.hasCustomTemplate = true;
            }
            this.Value = value.ToString();
            this.scaledHeight = height;
            this.scaledWidth = width;
        }

        private void Draw()
        {
            if (this.templateApplied)
            {
                double num = this.scaledWidth / this.rootElement.Width;
                double num2 = this.scaledHeight / this.rootElement.Height;
                double num3 = Math.Min(num, num2);
                ScaleTransform transform = new ScaleTransform
                {
                    ScaleX = num3,
                    ScaleY = num3
                };
                TranslateTransform transform2 = new TranslateTransform
                {
                    Y = (this.scaledHeight - (this.rootElement.Height * num3)) / 2.0
                };
                TransformGroup group = new TransformGroup
                {
                    Children = { transform, transform2 }
                };
                base.RenderTransform = group;
            }
        }

#if !WINDOWS_UWP
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.rootElement = base.GetTemplateChild("LayoutRoot") as FrameworkElement;
            this.templateApplied = true;
            this.Draw();
        }

        private void OnValueChanged(string oldValue, string newValue)
        {
            this.Draw();
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Digit)o).OnValueChanged((string)e.OldValue, (string)e.NewValue);
        }

        public ResourceDictionary DigitStyle { get; set; }

        public string Value
        {
            get
            {
                return (string)base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
    }
}
