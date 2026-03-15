using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Display
{
    public partial class Display : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", 
            typeof(double), typeof(Display), new PropertyMetadata(0.0, new PropertyChangedCallback(Display.OnValueChanged)));

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }


        UWPDisplay.Display contentDisplay = new UWPDisplay.Display();
        public Display()
        {
            this.InitializeComponent();
            Content = contentDisplay;

            var bindingWidth = new Binding() { Source = this, Path = new PropertyPath("Width") };
            BindingOperations.SetBinding(contentDisplay, FrameworkElement.WidthProperty, bindingWidth);

            var bindingHeight = new Binding() { Source = this, Path = new PropertyPath("Height") };
            BindingOperations.SetBinding(contentDisplay, FrameworkElement.HeightProperty, bindingHeight);
        }

        protected void OnValueChanged(double oldValue, double newValue)
        {
            contentDisplay.Value = newValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Display)o).OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

    }
}
